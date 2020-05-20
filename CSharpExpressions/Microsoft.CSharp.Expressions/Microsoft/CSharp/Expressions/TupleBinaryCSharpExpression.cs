﻿// Prototyping extended expression trees for C#.
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
        public bool IsLifted => Left.Type.IsNullableType() || Right.Type.IsNullableType();

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
            if (left == this.Left && right == this.Right && equalityChecks == this.EqualityChecks)
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

            var left = RemoveNullableIfNeverNull(Left);
            var right = RemoveNullableIfNeverNull(Right);

            var temps = new List<ParameterExpression>();
            var stmts = new List<Expression>();

            var leftTemp = Expression.Parameter(left.Type, "__left");
            temps.Add(leftTemp);
            stmts.Add(Expression.Assign(leftTemp, left));

            var rightTemp = Expression.Parameter(right.Type, "__right");
            temps.Add(rightTemp);
            stmts.Add(Expression.Assign(rightTemp, right));

            Expression res;

            if (left.Type.IsNullableType() || right.Type.IsNullableType())
            {
                var valueTemps = new List<ParameterExpression>();
                var valueStmts = new List<Expression>();

                (Expression hasValue, ParameterExpression valueTemp) GetNullableExpressions(ParameterExpression operandTemp, string valueTempName)
                {
                    if (operandTemp.Type.IsNullableType())
                    {
                        var value = Helpers.MakeNullableGetValueOrDefault(operandTemp);
                        var valueTemp = Expression.Parameter(value.Type, valueTempName);

                        valueTemps.Add(valueTemp);
                        valueStmts.Add(Expression.Assign(valueTemp, value));

                        return (Helpers.MakeNullableHasValue(operandTemp), valueTemp);
                    }
                    else
                    {
                        return (Expression.Constant(true), operandTemp);
                    }
                }

                var (leftHasValue, leftValueTemp) = GetNullableExpressions(leftTemp, "__leftVal");
                var (rightHasValue, rightValueTemp) = GetNullableExpressions(rightTemp, "__rightVal");

                var nullNull = Expression.Constant(CSharpNodeType == CSharpExpressionType.TupleEqual);
                var nullNonNull = Expression.Constant(CSharpNodeType == CSharpExpressionType.TupleNotEqual);

                valueStmts.Add(GetEvalExpr(leftValueTemp, rightValueTemp));

                var nonNullNonNull = Expression.Block(valueTemps, valueStmts);

                res = MakeCondition(MakeEqual(leftHasValue, rightHasValue), MakeCondition(leftHasValue, nonNullNonNull, nullNull), nullNonNull);

                static Expression MakeEqual(Expression l, Expression r)
                {
                    if (Helpers.IsConst(l, true))
                    {
                        return r;
                    }
                    else if (Helpers.IsConst(r, true))
                    {
                        return l;
                    }

                    return Expression.Equal(l, r);
                }

                static Expression MakeCondition(Expression test, Expression ifTrue, Expression ifFalse)
                {
                    if (Helpers.IsConst(test, true))
                    {
                        return ifTrue;
                    }
                    else if (Helpers.IsConst(test, false))
                    {
                        return ifFalse;
                    }
                    else if (Helpers.IsConst(ifTrue, true) && Helpers.IsConst(ifFalse, false))
                    {
                        return test;
                    }

                    return Expression.Condition(test, ifTrue, ifFalse);
                }
            }
            else
            {
                res = GetEvalExpr(leftTemp, rightTemp);
            }

            stmts.Add(res);

            return Expression.Block(temps, stmts);

            static Expression RemoveNullableIfNeverNull(Expression e)
            {
                //
                // NB: This is a common case emitted by the C# compiler, e.g.
                //
                //       (T l, T? r) => t == r
                //       (T? l, T r) => t == r
                //
                //      where T is a tuple type, resulting in
                //
                //       (T l, T? r) => (T?)l == r
                //       (T? l, T r) => l == (T?)r
                //
                //      We can optimize this case by unlifting the operand, which removes a HasValue check and the use of a temporary for Value.
                //

                if (e.Type.IsNullableType() && e is UnaryExpression { NodeType: ExpressionType.Convert, Operand: var o, Method: null } && o.Type == e.Type.GetNonNullableType())
                {
                    return o;
                }

                return e;
            }

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

                    static bool IsBinaryEquality(BinaryExpression b) => b.NodeType == ExpressionType.Equal || b.NodeType == ExpressionType.NotEqual;
                    static bool IsBinaryAppliedToParameters(BinaryExpression b, LambdaExpression c) => b.Left == c.Parameters[0] && b.Right == c.Parameters[1];
                    static bool IsTupleBinaryEquality(TupleBinaryCSharpExpression b) => b.CSharpNodeType == CSharpExpressionType.TupleEqual || b.CSharpNodeType == CSharpExpressionType.TupleNotEqual;
                    static bool IsTupleBinaryAppliedToParameters(TupleBinaryCSharpExpression b, LambdaExpression c) => b.Left == c.Parameters[0] && b.Right == c.Parameters[1];

                    var expr = equalityCheck.Body switch
                    {
                        ConstantExpression c => (Expression)c, // NB: This is commonly emitted by the C# compiler for null == null and null != null checks that occur in tuple literals.
                        DefaultExpression _ => Expression.Constant(false),
                        BinaryExpression binary when IsBinaryEquality(binary) && IsBinaryAppliedToParameters(binary, equalityCheck)
                            => binary.Update(leftChild, null, rightChild),
                        TupleBinaryCSharpExpression binary when IsTupleBinaryEquality(binary) && IsTupleBinaryAppliedToParameters(binary, equalityCheck)
                            => binary.Update(leftChild, rightChild, binary.EqualityChecks), // CONSIDER: We could flatten the && or || across these.
                        UnaryExpression { Operand: BinaryExpression binary, NodeType: ExpressionType.Convert } unary when IsBinaryEquality(binary) && IsBinaryAppliedToParameters(binary, equalityCheck)
                            => unary.Update(binary.Update(leftChild, conversion: null, rightChild)),
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

            // CONSIDER: If no equality checks are specified, generate default ones (using Equal/NotEqual or TupleEqual/TupleNotEqual for elements)?

            var checks = equalityChecks.ToReadOnly();

            if (checks.Count != arityLeft)
            {
                throw Error.InvalidEqualityCheckCount(arityLeft);
            }

            ContractUtils.RequiresNotNullItems(checks, nameof(equalityChecks));

            var leftTypes = Helpers.GetTupleComponentTypes(leftType).ToArray();
            var rightTypes = Helpers.GetTupleComponentTypes(rightType).ToArray();

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
        /// <summary>
        /// Creates a <see cref="TupleBinaryCSharpExpression" /> that represents a tuple equality expression.
        /// </summary>
        /// <param name="left">The <see cref="Expression" /> representing the left operand.</param>
        /// <param name="right">The <see cref="Expression" /> representing the right operand.</param>
        /// <param name="equalityChecks">An array of one or more of <see cref="LambdaExpression" /> objects that represent the equality checks to apply to the tuple elements.</param>
        /// <returns>A <see cref="TupleBinaryCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleEqual" /> and the <see cref="TupleBinaryCSharpExpression.Left" />, <see cref="TupleBinaryCSharpExpression.Right" />, and <see cref="TupleBinaryCSharpExpression.EqualityChecks" /> properties set to the specified values.</returns>
        public static TupleBinaryCSharpExpression TupleEqual(Expression left, Expression right, params LambdaExpression[] equalityChecks)
        {
            return TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleEqual, left, right, equalityChecks);
        }

        /// <summary>
        /// Creates a <see cref="TupleBinaryCSharpExpression" /> that represents a tuple equality expression.
        /// </summary>
        /// <param name="left">The <see cref="Expression" /> representing the left operand.</param>
        /// <param name="right">The <see cref="Expression" /> representing the right operand.</param>
        /// <param name="equalityChecks">An array of one or more of <see cref="LambdaExpression" /> objects that represent the equality checks to apply to the tuple elements.</param>
        /// <returns>A <see cref="TupleBinaryCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleEqual" /> and the <see cref="TupleBinaryCSharpExpression.Left" />, <see cref="TupleBinaryCSharpExpression.Right" />, and <see cref="TupleBinaryCSharpExpression.EqualityChecks" /> properties set to the specified values.</returns>
        public static TupleBinaryCSharpExpression TupleEqual(Expression left, Expression right, IEnumerable<LambdaExpression> equalityChecks)
        {
            return TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleEqual, left, right, equalityChecks);
        }

        /// <summary>
        /// Creates a <see cref="TupleBinaryCSharpExpression" /> that represents a tuple inequality expression.
        /// </summary>
        /// <param name="left">The <see cref="Expression" /> representing the left operand.</param>
        /// <param name="right">The <see cref="Expression" /> representing the right operand.</param>
        /// <param name="equalityChecks">An array of one or more of <see cref="LambdaExpression" /> objects that represent the inequality checks to apply to the tuple elements.</param>
        /// <returns>A <see cref="TupleBinaryCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleNotEqual" /> and the <see cref="TupleBinaryCSharpExpression.Left" />, <see cref="TupleBinaryCSharpExpression.Right" />, and <see cref="TupleBinaryCSharpExpression.EqualityChecks" /> properties set to the specified values.</returns>
        public static TupleBinaryCSharpExpression TupleNotEqual(Expression left, Expression right, params LambdaExpression[] equalityChecks)
        {
            return TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleNotEqual, left, right, equalityChecks);
        }

        /// <summary>
        /// Creates a <see cref="TupleBinaryCSharpExpression" /> that represents a tuple inequality expression.
        /// </summary>
        /// <param name="left">The <see cref="Expression" /> representing the left operand.</param>
        /// <param name="right">The <see cref="Expression" /> representing the right operand.</param>
        /// <param name="equalityChecks">An array of one or more of <see cref="LambdaExpression" /> objects that represent the inequality checks to apply to the tuple elements.</param>
        /// <returns>A <see cref="TupleBinaryCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleNotEqual" /> and the <see cref="TupleBinaryCSharpExpression.Left" />, <see cref="TupleBinaryCSharpExpression.Right" />, and <see cref="TupleBinaryCSharpExpression.EqualityChecks" /> properties set to the specified values.</returns>
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