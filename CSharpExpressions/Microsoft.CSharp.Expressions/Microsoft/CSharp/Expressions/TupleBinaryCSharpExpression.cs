// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a tuple binary operation.
    /// </summary>
    public abstract partial class TupleBinaryCSharpExpression : CSharpExpression
    {
        internal TupleBinaryCSharpExpression(Expression left, Expression right, ReadOnlyCollection<LambdaExpression> equalityChecks)
        {
            Left = left;
            Right = right;
            EqualityChecks = equalityChecks;
        }

        /// <summary>
        /// Gets the expression representing the left operand.
        /// </summary>
        public Expression Left { get; }

        /// <summary>
        /// Gets the expression representing the right operand.
        /// </summary>
        public Expression Right { get; }

        /// <summary>
        /// Gets the equality checks applied to the elements of the tuple.
        /// </summary>
        public ReadOnlyCollection<LambdaExpression> EqualityChecks { get; }

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        public override Type Type => typeof(bool);

        /// <summary>
        /// Gets a value indicating whether the binary operation is lifted.
        /// </summary>
        public bool IsLifted => Left.Type.IsNullableType();

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitTupleBinary(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="left">The <see cref="Left" /> property of the result.</param>
        /// <param name="right">The <see cref="Right" /> property of the result.</param>
        /// <param name="equalityChecks">The <see cref="EqualityChecks"/> property of the reslt.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public TupleBinaryCSharpExpression Update(Expression left, Expression right, IEnumerable<LambdaExpression> equalityChecks)
        {
            if (left == this.Left && right == this.Right && EqualityChecks == this.EqualityChecks)
            {
                return this;
            }

            return Make(CSharpNodeType, left, right, equalityChecks);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            //
            // TODO: Optimizations for common cases, e.g.
            //
            //         t == (1, 2)
            //         (a, b) == (c, d)
            //
            //        possibly with implicit tuple conversions or nullable conversions applied.
            //

            var temps = new List<ParameterExpression>();
            var stmts = new List<Expression>();

            var leftTemp = Expression.Parameter(Left.Type, "__left");
            temps.Add(leftTemp);
            stmts.Add(Expression.Assign(leftTemp, Left));

            var rightTemp = Expression.Parameter(Right.Type, "__right");
            temps.Add(rightTemp);
            stmts.Add(Expression.Assign(rightTemp, Right));

            Expression res;

            if (!IsLifted)
            {
                res = GetEvalExpr(leftTemp, rightTemp);
            }
            else
            {
                static (Expression hasValue, ParameterExpression valueTemp, Expression assignValue) GetNullableExpressions(Expression e, string valueTempName)
                {
                    var value = Expression.Property(e, "Value");
                    var valueTemp = Expression.Parameter(value.Type, valueTempName);

                    return (Expression.Property(e, "HasValue"), valueTemp, Expression.Assign(valueTemp, value));
                }

                var (leftHasValue, leftValueTemp, leftValueAssign) = GetNullableExpressions(leftTemp, "__leftVal");
                var (rightHasValue, rightValueTemp, rightValueAssign) = GetNullableExpressions(rightTemp, "__rightVal");

                var nullNull = Expression.Constant(CSharpNodeType == CSharpExpressionType.TupleEqual);
                var nullNonNull = Expression.Constant(CSharpNodeType == CSharpExpressionType.TupleNotEqual);
                var nonNullNonNull = Expression.Block(new[] { leftValueTemp, rightValueTemp }, leftValueAssign, rightValueAssign, GetEvalExpr(leftValueTemp, rightValueTemp));

                res = Expression.Condition(Expression.Equal(leftHasValue, rightHasValue), Expression.Condition(leftHasValue, nonNullNonNull, nullNull), nullNonNull);
            }

            stmts.Add(res);

            return Expression.Block(temps, stmts);

            Expression GetEvalExpr(Expression lhs, Expression rhs)
            {
                var exprs = Core(EqualityChecks, lhs, rhs).ToList();

                return exprs.Aggregate((l, r) => CSharpNodeType == CSharpExpressionType.TupleEqual ? Expression.AndAlso(l, r) : Expression.OrElse(l, r));
            }

            IEnumerable<Expression> Core(ReadOnlyCollection<LambdaExpression> equalityChecks, Expression lhs, Expression rhs)
            {
                for (int i = 0, n = equalityChecks.Count; i < n; i++)
                {
                    var leftChild = Helpers.GetTupleItemAccess(lhs, i);
                    var rightChild = Helpers.GetTupleItemAccess(rhs, i);

                    var equalityCheck = equalityChecks[i];

                    var expr = equalityCheck.Body switch
                    {
                        ConstantExpression c => (Expression)c,
                        DefaultExpression _ => Expression.Constant(false),
                        BinaryExpression { Left: var leftOp, Right: var rightOp } binary
                            when (binary.NodeType == ExpressionType.Equal || binary.NodeType == ExpressionType.NotEqual)
                                && leftOp == equalityCheck.Parameters[0]
                                && rightOp == equalityCheck.Parameters[1]
                            => binary.Update(leftChild, null, rightChild),
                        TupleBinaryCSharpExpression { Left: var leftOp, Right: var rightOp } binary
                            when (binary.CSharpNodeType == CSharpExpressionType.TupleEqual || binary.CSharpNodeType == CSharpExpressionType.TupleNotEqual)
                                && leftOp == equalityCheck.Parameters[0]
                                && rightOp == equalityCheck.Parameters[1]
                            => binary.Update(leftChild, rightChild, binary.EqualityChecks), // CONSIDER: We could flatten the && or || across these.
                        UnaryExpression { Operand: BinaryExpression { Left: var leftOp, Right: var rightOp } binary } unary
                            when unary.NodeType == ExpressionType.Convert
                                && (binary.NodeType == ExpressionType.Equal || binary.NodeType == ExpressionType.NotEqual)
                                && leftOp == equalityCheck.Parameters[0]
                                && rightOp == equalityCheck.Parameters[1]
                            => unary.Update(binary.Update(leftChild, null, rightChild)),
                        _ => Expression.Invoke(equalityCheck, leftChild, rightChild),
                    };

                    yield return expr;
                }
            }
        }

        internal sealed new class TupleEqual : TupleBinaryCSharpExpression
        {
            public TupleEqual(Expression left, Expression right, ReadOnlyCollection<LambdaExpression> equalityChecks)
                : base(left, right, equalityChecks)
            {
            }

            public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.TupleEqual;
        }

        internal sealed new class TupleNotEqual : TupleBinaryCSharpExpression
        {
            public TupleNotEqual(Expression left, Expression right, ReadOnlyCollection<LambdaExpression> equalityChecks)
                : base(left, right, equalityChecks)
            {
            }

            public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.TupleNotEqual;
        }

        internal static TupleBinaryCSharpExpression Make(CSharpExpressionType kind, Expression left, Expression right, IEnumerable<LambdaExpression> equalityChecks)
        {
            Debug.Assert(kind == CSharpExpressionType.TupleEqual || kind == CSharpExpressionType.TupleNotEqual);

            var leftType = CheckOperandAndGetNonNullableTupleType(left, nameof(left));
            var rightType = CheckOperandAndGetNonNullableTupleType(right, nameof(right));

            var arityLeft = Helpers.GetTupleArity(leftType);
            var arityRight = Helpers.GetTupleArity(rightType);

            if (arityLeft != arityRight)
            {
                throw Error.TupleComponentCountMismatch(leftType, rightType);
            }

            var checks = equalityChecks.ToReadOnly();

            var leftTypes = Helpers.GetTupleComponentTypes(leftType).ToArray();
            var rightTypes = Helpers.GetTupleComponentTypes(rightType).ToArray();

            ContractUtils.RequiresNotNullItems(checks, nameof(equalityChecks));

            for (int i = 0; i < arityLeft; i++)
            {
                CheckEqualityCheck(kind, checks[i], leftTypes[i], rightTypes[i]);
            }

            if (kind == CSharpExpressionType.TupleEqual)
            {
                return new TupleEqual(left, right, checks);
            }
            else
            {
                return new TupleNotEqual(left, right, checks);
            }

            static Type CheckOperandAndGetNonNullableTupleType(Expression operand, string name)
            {
                RequiresCanRead(operand, name);

                var operandType = operand.Type;

                if (operandType.IsNullableType())
                {
                    operandType = operandType.GetNonNullableType();
                }

                if (!Helpers.IsTupleType(operandType))
                {
                    throw Error.InvalidTupleType(operand.Type);
                }

                return operandType;
            }

            static void CheckEqualityCheck(CSharpExpressionType nodeType, LambdaExpression check, Type left, Type right)
            {
                var method = check.Type.GetMethod("Invoke");
                var parameters = method.GetParametersCached();

                if (parameters.Length != 2)
                {
                    throw LinqError.IncorrectNumberOfMethodCallArguments(check);
                }

                if (!TypeUtils.AreEquivalent(method.ReturnType, typeof(bool)))
                {
                    throw LinqError.OperandTypesDoNotMatchParameters(nodeType, check.ToString());
                }

                if (!ParameterIsAssignable(parameters[0], left) || !ParameterIsAssignable(parameters[1], right))
                {
                    throw LinqError.OperandTypesDoNotMatchParameters(nodeType, check.ToString());
                }
            }
        }
    }

    partial class CSharpExpression
    {
        public static TupleBinaryCSharpExpression TupleEqual(Expression left, Expression right, params LambdaExpression[] equalityChecks)
        {
            return TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleEqual, left, right, equalityChecks);
        }

        public static TupleBinaryCSharpExpression TupleEqual(Expression left, Expression right, IEnumerable<LambdaExpression> equalityChecks)
        {
            return TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleEqual, left, right, equalityChecks);
        }

        public static TupleBinaryCSharpExpression TupleNotEqual(Expression left, Expression right, params LambdaExpression[] equalityChecks)
        {
            return TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleNotEqual, left, right, equalityChecks);
        }

        public static TupleBinaryCSharpExpression TupleNotEqual(Expression left, Expression right, IEnumerable<LambdaExpression> equalityChecks)
        {
            return TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleNotEqual, left, right, equalityChecks);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="TupleLiteralCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitTupleBinary(TupleBinaryCSharpExpression node)
        {
            return node.Update(Visit(node.Left), Visit(node.Right), VisitAndConvert(node.EqualityChecks, nameof(VisitTupleBinary)));
        }
    }
}
