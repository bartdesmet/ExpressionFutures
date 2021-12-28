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
using System.Runtime.CompilerServices;
using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;

using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

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
            if (left == Left && right == Right && SameElements(ref equalityChecks, EqualityChecks))
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
            var left = RemoveNullableIfNeverNull(Left);
            var right = RemoveNullableIfNeverNull(Right);

            var temps = new List<ParameterExpression>();
            var stmts = new List<Expression>();

            left = SpillToTemps(left, EqualityChecks, "__left");
            right = SpillToTemps(right, EqualityChecks, "__right");

            stmts.Add(Reduce(left, right, EqualityChecks));

            return Comma(temps, stmts);

            Expression SpillToTemps(Expression tuple, ReadOnlyCollection<LambdaExpression> equalityChecks, string prefix)
            {
                //
                // CONSIDER: Handle TupleConvertCSharpExpression by analyzing if all conversions are implicit, unchecked, and non-user-defined
                //           ones, in which case we can spill the operand to a temp and defer applying the conversions to elements.
                //

                if (tuple is TupleLiteralCSharpExpression literal)
                {
                    List<Expression> newArgs = null;

                    var args = literal.Arguments;

                    void EnsureArgs(int max)
                    {
                        if (newArgs == null)
                        {
                            newArgs = new List<Expression>(args.Count);

                            for (int j = 0; j < max; j++)
                            {
                                newArgs.Add(args[j]);
                            }
                        }
                    }

                    for (int i = 0, n = args.Count; i < n; i++)
                    {
                        var arg = args[i];

                        if (Helpers.IsPure(arg))
                        {
                            if (newArgs != null)
                            {
                                newArgs.Add(arg);
                            }
                        }
                        else if (arg is TupleLiteralCSharpExpression nestedLiteral && IsNestedTupleBinaryEqualityCheck(CSharpNodeType, equalityChecks[i], out var nestedEqualityChecks))
                        {
                            var newArg = SpillToTemps(nestedLiteral, nestedEqualityChecks, prefix + i + "_");

                            if (newArg != arg)
                            {
                                EnsureArgs(i);
                            }

                            if (newArgs != null)
                            {
                                newArgs.Add(newArg);
                            }
                        }
                        else
                        {
                            var tmp = Expression.Parameter(arg.Type, prefix + i);
                            temps.Add(tmp);
                            stmts.Add(Expression.Assign(tmp, arg));

                            EnsureArgs(i);
                            newArgs.Add(tmp);
                        }
                    }

                    return newArgs != null ? literal.Update(newArgs) : literal;
                }
                else
                {
                    //
                    // CONSIDER: Add a purity check to avoid spilling here in some cases, e.g. t => t == (1, 2). This needs an additional analysis step to ensure
                    //           the the variable is not being written to. We can do so by finding all assignments and `ref` uses of variables.
                    //

                    var tmp = Expression.Parameter(tuple.Type, prefix);
                    temps.Add(tmp);
                    stmts.Add(Expression.Assign(tmp, tuple));

                    return tmp;
                }

                static bool IsNestedTupleBinaryEqualityCheck(CSharpExpressionType kind, LambdaExpression equalityCheck, out ReadOnlyCollection<LambdaExpression> equalityChecks)
                {
                    if (equalityCheck.Body is TupleBinaryCSharpExpression binary && binary.CSharpNodeType == kind && binary.Left == equalityCheck.Parameters[0] && binary.Right == equalityCheck.Parameters[1])
                    {
                        equalityChecks = binary.EqualityChecks;
                        return true;
                    }

                    equalityChecks = null;
                    return false;
                }
            }

            Expression Reduce(Expression left, Expression right, ReadOnlyCollection<LambdaExpression> equalityChecks)
            {
                left = RemoveNullableIfNeverNull(left);
                right = RemoveNullableIfNeverNull(right);

                if (left.Type.IsNullableType() || right.Type.IsNullableType())
                {
                    (Expression hasValue, Expression valueTemp) GetNullableExpressions(Expression operand)
                    {
                        if (operand.Type.IsNullableType())
                        {
                            var temp = Expression.Parameter(operand.Type);

                            temps.Add(temp);
                            stmts.Add(Expression.Assign(temp, operand));

                            return (MakeNullableHasValue(temp), MakeNullableGetValueOrDefault(temp));
                        }
                        else
                        {
                            return (ConstantTrue, operand);
                        }
                    }

                    var (leftHasValue, leftValue) = GetNullableExpressions(left);
                    var (rightHasValue, rightValue) = GetNullableExpressions(right);

                    var nullNull = CreateConstantBoolean(CSharpNodeType == CSharpExpressionType.TupleEqual);
                    var nullNonNull = CreateConstantBoolean(CSharpNodeType == CSharpExpressionType.TupleNotEqual);

                    var nonNullNonNull = ReduceNonNull(leftValue, rightValue, equalityChecks);

                    return MakeCondition(MakeEqual(leftHasValue, rightHasValue), MakeCondition(leftHasValue, nonNullNonNull, nullNull), nullNonNull);

                    static Expression MakeEqual(Expression l, Expression r)
                    {
                        if (IsConst(l, true))
                        {
                            return r;
                        }
                        else if (IsConst(r, true))
                        {
                            return l;
                        }

                        return Expression.Equal(l, r);
                    }

                    static Expression MakeCondition(Expression test, Expression ifTrue, Expression ifFalse)
                    {
                        if (IsConst(test, true))
                        {
                            return ifTrue;
                        }
                        else if (IsConst(test, false))
                        {
                            return ifFalse;
                        }
                        else if (IsConst(ifTrue, true) && IsConst(ifFalse, false))
                        {
                            return test;
                        }

                        return Expression.Condition(test, ifTrue, ifFalse);
                    }
                }
                else
                {
                    return ReduceNonNull(left, right, equalityChecks);
                }

                Expression ReduceNonNull(Expression left, Expression right, ReadOnlyCollection<LambdaExpression> equalityChecks)
                {
                    Expression res = null;

                    for (int i = 0, n = equalityChecks.Count; i < n; i++)
                    {
                        var lhs = GetComponent(left, i);
                        var rhs = GetComponent(right, i);

                        var check = GetEqualityCheck(lhs, rhs, equalityChecks[i]);

                        if (res == null)
                        {
                            res = check;
                        }
                        else
                        {
                            res = Expression.MakeBinary(CSharpNodeType == CSharpExpressionType.TupleEqual ? ExpressionType.AndAlso : ExpressionType.OrElse, res, check);
                        }
                    }

                    return res;

                    static Expression GetComponent(Expression tuple, int i)
                    {
                        if (tuple is TupleLiteralCSharpExpression literal)
                        {
                            return literal.Arguments[i];
                        }
                        else
                        {
                            return GetTupleItemAccess(tuple, i);
                        }
                    }

                    Expression GetEqualityCheck(Expression lhs, Expression rhs, LambdaExpression equalityCheck)
                    {
                        static bool IsBinaryEquality(BinaryExpression b) => b.NodeType == ExpressionType.Equal || b.NodeType == ExpressionType.NotEqual;
                        static bool IsBinaryAppliedToParameters(BinaryExpression b, LambdaExpression c) => b.Left == c.Parameters[0] && b.Right == c.Parameters[1];
                        static bool IsTupleBinaryAppliedToParameters(TupleBinaryCSharpExpression b, LambdaExpression c) => b.Left == c.Parameters[0] && b.Right == c.Parameters[1];

                        var expr = equalityCheck.Body switch
                        {
                            ConstantExpression c => (Expression)c, // NB: This is commonly emitted by the C# compiler for null == null and null != null checks that occur in tuple literals.
                            DefaultExpression _ => ConstantFalse,
                            BinaryExpression binary when IsBinaryEquality(binary) && IsBinaryAppliedToParameters(binary, equalityCheck)
                                => binary.Update(lhs, null, rhs),
                            UnaryExpression { Operand: BinaryExpression binary, NodeType: ExpressionType.Convert } unary when IsBinaryEquality(binary) && IsBinaryAppliedToParameters(binary, equalityCheck)
                                => unary.Update(binary.Update(lhs, conversion: null, rhs)),
                            TupleBinaryCSharpExpression binary when binary.CSharpNodeType == CSharpNodeType && IsTupleBinaryAppliedToParameters(binary, equalityCheck)
                                => Reduce(lhs, rhs, binary.EqualityChecks),
                            _ => Expression.Invoke(equalityCheck, lhs, rhs),
                        };

                        return expr;
                    }
                }
            }

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

            var arityLeft = GetTupleArity(leftType);
            var arityRight = GetTupleArity(rightType);

            if (arityLeft != arityRight)
                throw Error.TupleComponentCountMismatch(leftType, rightType);

            var leftTypes = GetTupleComponentTypes(leftType).ToArray();
            var rightTypes = GetTupleComponentTypes(rightType).ToArray();

            // CONSIDER: If no equality checks are specified, generate default ones (using Equal/NotEqual or TupleEqual/TupleNotEqual for elements)?

            ReadOnlyCollection<LambdaExpression> checks;

            if (equalityChecks == null)
            {
                var inferredChecks = new LambdaExpression[arityLeft];

                for (int i = 0; i < arityLeft; i++)
                {
                    var leftComponentType = leftTypes[i];
                    var rightComponentType = rightTypes[i];

                    var leftComponentParameter = Expression.Parameter(leftComponentType);
                    var rightComponentParameter = Expression.Parameter(rightComponentType);

                    var equalityCheckBody =
                        IsTupleType(leftComponentType) && IsTupleType(rightComponentType)
                            ? (Expression)Make(kind, leftComponentParameter, rightComponentParameter, equalityChecks: null)
                            : kind == CSharpExpressionType.TupleEqual
                                ? Expression.Equal(leftComponentParameter, rightComponentParameter)
                                : Expression.NotEqual(leftComponentParameter, rightComponentParameter);

                    inferredChecks[i] = Expression.Lambda(equalityCheckBody, leftComponentParameter, rightComponentParameter);
                }

                checks = new TrueReadOnlyCollection<LambdaExpression>(inferredChecks);
            }
            else
            {
                checks = equalityChecks.ToReadOnly();

                if (checks.Count != arityLeft)
                    throw Error.InvalidEqualityCheckCount(arityLeft);

                RequiresNotNullItems(checks, nameof(equalityChecks));

                for (int i = 0; i < arityLeft; i++)
                {
                    CheckEqualityCheck(kind, checks[i], leftTypes[i], rightTypes[i]);
                }
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

                var operandType = operand.Type.GetNonNullableType();

                if (!IsTupleType(operandType))
                    throw Error.InvalidTupleType(operand.Type);

                return operandType;
            }

            static void CheckEqualityCheck(CSharpExpressionType nodeType, LambdaExpression check, Type left, Type right)
            {
                var method = check.Type.GetMethod("Invoke");
                var parameters = method.GetParametersCached();

                if (parameters.Length != 2)
                    throw LinqError.IncorrectNumberOfMethodCallArguments(check);

                if (!AreEquivalent(method.ReturnType, typeof(bool)))
                    throw LinqError.OperandTypesDoNotMatchParameters(nodeType, check.ToString());

                if (!ParameterIsAssignable(parameters[0], left) || !ParameterIsAssignable(parameters[1], right))
                    throw LinqError.OperandTypesDoNotMatchParameters(nodeType, check.ToString());
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
        /// <returns>A <see cref="TupleBinaryCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleEqual" /> and the <see cref="TupleBinaryCSharpExpression.Left" />, <see cref="TupleBinaryCSharpExpression.Right" />, and <see cref="TupleBinaryCSharpExpression.EqualityChecks" /> properties set to the specified values.</returns>
        public static TupleBinaryCSharpExpression TupleEqual(Expression left, Expression right) =>
            TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleEqual, left, right, equalityChecks: null);

        /// <summary>
        /// Creates a <see cref="TupleBinaryCSharpExpression" /> that represents a tuple equality expression.
        /// </summary>
        /// <param name="left">The <see cref="Expression" /> representing the left operand.</param>
        /// <param name="right">The <see cref="Expression" /> representing the right operand.</param>
        /// <param name="equalityChecks">An array of one or more of <see cref="LambdaExpression" /> objects that represent the equality checks to apply to the tuple elements.</param>
        /// <returns>A <see cref="TupleBinaryCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleEqual" /> and the <see cref="TupleBinaryCSharpExpression.Left" />, <see cref="TupleBinaryCSharpExpression.Right" />, and <see cref="TupleBinaryCSharpExpression.EqualityChecks" /> properties set to the specified values.</returns>
        public static TupleBinaryCSharpExpression TupleEqual(Expression left, Expression right, params LambdaExpression[] equalityChecks) =>
            TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleEqual, left, right, equalityChecks);

        /// <summary>
        /// Creates a <see cref="TupleBinaryCSharpExpression" /> that represents a tuple equality expression.
        /// </summary>
        /// <param name="left">The <see cref="Expression" /> representing the left operand.</param>
        /// <param name="right">The <see cref="Expression" /> representing the right operand.</param>
        /// <param name="equalityChecks">An array of one or more of <see cref="LambdaExpression" /> objects that represent the equality checks to apply to the tuple elements.</param>
        /// <returns>A <see cref="TupleBinaryCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleEqual" /> and the <see cref="TupleBinaryCSharpExpression.Left" />, <see cref="TupleBinaryCSharpExpression.Right" />, and <see cref="TupleBinaryCSharpExpression.EqualityChecks" /> properties set to the specified values.</returns>
        public static TupleBinaryCSharpExpression TupleEqual(Expression left, Expression right, IEnumerable<LambdaExpression> equalityChecks) =>
            TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleEqual, left, right, equalityChecks);

        /// <summary>
        /// Creates a <see cref="TupleBinaryCSharpExpression" /> that represents a tuple inequality expression.
        /// </summary>
        /// <param name="left">The <see cref="Expression" /> representing the left operand.</param>
        /// <param name="right">The <see cref="Expression" /> representing the right operand.</param>
        /// <returns>A <see cref="TupleBinaryCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleNotEqual" /> and the <see cref="TupleBinaryCSharpExpression.Left" />, <see cref="TupleBinaryCSharpExpression.Right" />, and <see cref="TupleBinaryCSharpExpression.EqualityChecks" /> properties set to the specified values.</returns>
        public static TupleBinaryCSharpExpression TupleNotEqual(Expression left, Expression right) =>
            TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleNotEqual, left, right, equalityChecks: null);

        /// <summary>
        /// Creates a <see cref="TupleBinaryCSharpExpression" /> that represents a tuple inequality expression.
        /// </summary>
        /// <param name="left">The <see cref="Expression" /> representing the left operand.</param>
        /// <param name="right">The <see cref="Expression" /> representing the right operand.</param>
        /// <param name="equalityChecks">An array of one or more of <see cref="LambdaExpression" /> objects that represent the inequality checks to apply to the tuple elements.</param>
        /// <returns>A <see cref="TupleBinaryCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleNotEqual" /> and the <see cref="TupleBinaryCSharpExpression.Left" />, <see cref="TupleBinaryCSharpExpression.Right" />, and <see cref="TupleBinaryCSharpExpression.EqualityChecks" /> properties set to the specified values.</returns>
        public static TupleBinaryCSharpExpression TupleNotEqual(Expression left, Expression right, params LambdaExpression[] equalityChecks) =>
            TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleNotEqual, left, right, equalityChecks);

        /// <summary>
        /// Creates a <see cref="TupleBinaryCSharpExpression" /> that represents a tuple inequality expression.
        /// </summary>
        /// <param name="left">The <see cref="Expression" /> representing the left operand.</param>
        /// <param name="right">The <see cref="Expression" /> representing the right operand.</param>
        /// <param name="equalityChecks">An array of one or more of <see cref="LambdaExpression" /> objects that represent the inequality checks to apply to the tuple elements.</param>
        /// <returns>A <see cref="TupleBinaryCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleNotEqual" /> and the <see cref="TupleBinaryCSharpExpression.Left" />, <see cref="TupleBinaryCSharpExpression.Right" />, and <see cref="TupleBinaryCSharpExpression.EqualityChecks" /> properties set to the specified values.</returns>
        public static TupleBinaryCSharpExpression TupleNotEqual(Expression left, Expression right, IEnumerable<LambdaExpression> equalityChecks) =>
            TupleBinaryCSharpExpression.Make(CSharpExpressionType.TupleNotEqual, left, right, equalityChecks);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="TupleLiteralCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitTupleBinary(TupleBinaryCSharpExpression node) =>
            node.Update(
                Visit(node.Left),
                Visit(node.Right),
                VisitAndConvert(node.EqualityChecks, nameof(VisitTupleBinary))
            );
    }
}
