// Prototyping extended expression trees for C#.
//
// bartde - February 2020

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a range in a sliceable object.
    /// </summary>
    public sealed partial class RangeCSharpExpression : CSharpExpression
    {
        internal RangeCSharpExpression(Expression? left, Expression? right, MethodBase? method, Type? type)
        {
            Left = left;
            Right = right;
            Method = method;
            Type = type ?? ((left?.Type.IsNullableType() ?? false) || (right?.Type.IsNullableType() ?? false) ? typeof(Range?) : typeof(Range));
        }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the lower bound index of the range.
        /// </summary>
        public Expression? Left { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the upper bound index of the range.
        /// </summary>
        public Expression? Right { get; }

        /// <summary>
        /// Gets the (optional) method or constructor used to create an instance of type <see cref="Range"/>.
        /// </summary>
        public MethodBase? Method { get; }

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        public override Type Type { get; }

        /// <summary>
        /// Gets a value indicating whether the index construction is lifted to a nullable type.
        /// </summary>
        public bool IsLifted => Type.IsNullableType();

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Range;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitRange(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="left">The <see cref="Left" /> property of the result.</param>
        /// <param name="right">The <see cref="Right" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public RangeCSharpExpression Update(Expression? left, Expression? right)
        {
            if (left == Left && right == Right)
            {
                return this;
            }

            return CSharpExpression.Range(left, right, Method, Type);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce() => IsLifted ? ReduceLifted() : MakeRange(Left, Right);

        private Expression ReduceLifted()
        {
            // TODO: More optimizations based on IsAlwaysNull and IsNeverNull are possible.

            var temps = default(List<ParameterExpression>);
            var stmts = default(List<Expression>);
            var notNullCheck = default(Expression);

            var left = PrepareOperand(Left);
            var right = PrepareOperand(Right);

            var make = MakeRange(left, right);

            make = Expression.Convert(make, Type);

            if (notNullCheck != null)
            {
                make = Expression.Condition(notNullCheck, make, Expression.Default(Type));
            }

            if (temps == null)
            {
                return make;
            }
            else
            {
                Debug.Assert(stmts != null);
                stmts.Add(make);
                return Expression.Block(temps, stmts);
            }

            Expression? PrepareOperand(Expression? operand)
            {
                if (operand == null)
                {
                    return null;
                }

                if (!operand.Type.IsNullableType())
                {
                    return operand;
                }

                temps ??= new List<ParameterExpression>(2);
                stmts ??= new List<Expression>(2);

                var temp = Expression.Parameter(operand.Type);
                temps.Add(temp);

                stmts.Add(Expression.Assign(temp, operand));

                var hasValue = Helpers.MakeNullableHasValue(temp);

                if (notNullCheck == null)
                {
                    notNullCheck = hasValue;
                }
                else
                {
                    notNullCheck = Expression.AndAlso(notNullCheck, hasValue);
                }

                return Helpers.MakeNullableGetValueOrDefault(temp);
            }
        }

        private Expression MakeRange(Expression? left, Expression? right)
        {
            return GetMethod() switch
            {
                MethodInfo m => Expression.Call(m, GetArgs()),
                ConstructorInfo c => Expression.New(c, GetArgs()),
                _ => throw ContractUtils.Unreachable,
            };

            List<Expression> GetArgs()
            {
                var args = new List<Expression>();

                if (left != null)
                {
                    args.Add(left);
                }

                if (right != null)
                {
                    args.Add(right);
                }

                return args;
            }

            MethodBase GetMethod()
            {
                if (Method != null)
                {
                    return Method;
                }

                if (Left == null && Right == null)
                {
                    return WellKnownMembers.RangeAll;
                }
                else if (Left == null)
                {
                    return WellKnownMembers.RangeEndAt;
                }
                else if (Right == null)
                {
                    return WellKnownMembers.RangeStartAt;
                }
                else
                {
                    return WellKnownMembers.RangeCtor;
                }
            }
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="RangeCSharpExpression"/> that represents a range in a sliceable object.
        /// </summary>
        /// <param name="left">The expression representing the lower bound index of the range.</param>
        /// <param name="right">The expression representing the upper bound index of the range.</param>
        /// <returns>The created <see cref="RangeCSharpExpression"/>.</returns>
        public static RangeCSharpExpression Range(Expression? left, Expression? right) => Range(left, right, method: null, type: null);

        /// <summary>
        /// Creates a <see cref="RangeCSharpExpression"/> that represents a range in a sliceable object.
        /// </summary>
        /// <param name="left">The expression representing the lower bound index of the range.</param>
        /// <param name="right">The expression representing the upper bound index of the range.</param>
        /// <param name="method">The method or constructor used to instantiate the index.</param>
        /// <param name="type">The index type, either <see cref="System.Index"/> or a nullable <see cref="System.Index"/>.</param>
        /// <returns>The created <see cref="RangeCSharpExpression"/>.</returns>
        public static RangeCSharpExpression Range(Expression? left, Expression? right, MethodBase? method, Type? type)
        {
            int operandCount = 0;
            bool lifted = false;

            CheckOperand(ref left, nameof(left), ref operandCount, ref lifted);
            CheckOperand(ref right, nameof(right), ref operandCount, ref lifted);

            if (method != null)
            {
                if (method.IsGenericMethodDefinition || method.GetReturnType() != typeof(Range))
                    throw Error.InvalidRangeMethod(nameof(method));

                if (method.MemberType == MemberTypes.Method && !method.IsStatic)
                    throw Error.InvalidRangeMethod(nameof(method));

                var parameters = method.GetParametersCached();

                if (parameters.Length != operandCount)
                    throw Error.InvalidRangeMethod(nameof(method));

                for (int i = 0; i < operandCount; i++)
                {
                    if (parameters[i].ParameterType != typeof(Index))
                        throw Error.InvalidRangeMethod(nameof(method));
                }
            }

            if (type != null)
            {
                if (type == typeof(Range))
                {
                    if (lifted)
                        throw Error.InvalidRangeType(type, nameof(type));
                }
                else if (type == typeof(Range?))
                {
                    if (!lifted)
                        throw Error.InvalidRangeType(type, nameof(type));
                }
                else
                {
                    throw Error.InvalidRangeType(type, nameof(type));
                }
            }

            return new RangeCSharpExpression(left, right, method, type);

            static void CheckOperand(ref Expression? operand, string paramName, ref int operandCount, ref bool lifted)
            {
                if (operand == null)
                {
                    return;
                }

                operandCount++;

                RequiresCanRead(operand, paramName);

                if (operand.Type != typeof(Index) && operand.Type != typeof(Index?))
                {
                    if (operand.Type == typeof(int))
                    {
                        operand = Expression.Convert(operand, typeof(Index));
                    }
                    else if (operand.Type == typeof(int?))
                    {
                        operand = Expression.Convert(operand, typeof(Index?));
                    }
                    else
                    {
                        throw Error.InvalidRangeOperandType(operand.Type, nameof(operand));
                    }
                }

                if (operand.Type.IsNullableType())
                {
                    lifted = true;
                }
            }
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="RangeCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitRange(RangeCSharpExpression node) =>
            node.Update(
                Visit(node.Left),
                Visit(node.Right)
            );
    }
}
