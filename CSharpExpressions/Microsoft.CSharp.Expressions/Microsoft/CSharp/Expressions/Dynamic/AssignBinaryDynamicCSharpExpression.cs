// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Microsoft.CSharp.RuntimeBinder;

using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    using static DynamicHelpers;

    /// <summary>
    /// Represents a dynamically bound binary assignment operation.
    /// </summary>
    public sealed partial class AssignBinaryDynamicCSharpExpression : DynamicCSharpExpression
    {
        internal AssignBinaryDynamicCSharpExpression(Type? context, CSharpBinderFlags binderFlags, CSharpExpressionType binaryType, DynamicCSharpArgument left, DynamicCSharpArgument right)
            : base(context, binderFlags)
        {
            OperationNodeType = binaryType;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.DynamicBinaryAssign;

        /// <summary>
        /// Gets the expression kind of the dynamically bound operation.
        /// </summary>
        public CSharpExpressionType OperationNodeType { get; }

        /// <summary>
        /// Gets the left operand of the binary operation.
        /// </summary>
        public DynamicCSharpArgument Left { get; }

        /// <summary>
        /// Gets the right operand of the binary operation.
        /// </summary>
        public DynamicCSharpArgument Right { get; }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="System.Linq.Expressions.Expression" /> represents. (Inherited from <see cref="System.Linq.Expressions.Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => Left.Expression.Type;

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            switch (OperationNodeType)
            {
                case CSharpExpressionType.Assign:
                    return ReduceAssign();
                case CSharpExpressionType.NullCoalescingAssign:
                    return ReduceNullCoalescingAssign();
            }

            var candidateAccessor = OperationNodeType switch
            {
                CSharpExpressionType.AddAssign or CSharpExpressionType.AddAssignChecked => "add_",
                CSharpExpressionType.SubtractAssign or CSharpExpressionType.SubtractAssignChecked => "remove_",
                _ => null
            };

            if (candidateAccessor != null)
            {
                // NB: The lhs we get in the functionOp callback already has its operands stored
                //     in temps, so we can safely pass it to WithEventCheck which will decompose
                //     the member to retrieve its LHS.

                if (Left.Expression is GetMemberDynamicCSharpExpression dynamicMember && dynamicMember.Arguments.Count == 0)
                {
                    return WithEventCheck(dynamicMember, candidateAccessor);
                }
            }

            return ReduceCore(Left);
        }

        private Expression ReduceCore(DynamicCSharpArgument left)
        {
            var functionalOp = new Func<Expression, Expression>(lhs =>
            {
                var operation = OperationNodeType switch
                {
                    CSharpExpressionType.AddAssign or CSharpExpressionType.AddAssignChecked => ExpressionType.AddAssign,
                    CSharpExpressionType.SubtractAssign or CSharpExpressionType.SubtractAssignChecked => ExpressionType.SubtractAssign,
                    CSharpExpressionType.MultiplyAssign or CSharpExpressionType.MultiplyAssignChecked => ExpressionType.MultiplyAssign,
                    CSharpExpressionType.DivideAssign => ExpressionType.DivideAssign,
                    CSharpExpressionType.ModuloAssign => ExpressionType.ModuloAssign,
                    CSharpExpressionType.AndAssign => ExpressionType.AndAssign,
                    CSharpExpressionType.OrAssign => ExpressionType.OrAssign,
                    CSharpExpressionType.ExclusiveOrAssign => ExpressionType.ExclusiveOrAssign,
                    CSharpExpressionType.LeftShiftAssign => ExpressionType.LeftShiftAssign,
                    CSharpExpressionType.RightShiftAssign => ExpressionType.RightShiftAssign,
                    _ => throw Unreachable,
                };

                var args = new[]
                {
                    CSharpArgumentInfo.Create(GetArgumentInfoFlags(Left), null),
                    CSharpArgumentInfo.Create(GetArgumentInfoFlags(Right), null),
                };

                var binder = Binder.BinaryOperation(Flags, operation, Context, args);
                var dynamic = DynamicHelpers.MakeDynamic(typeof(object), binder, new[] { lhs, Right.Expression }, new[] { lhs.Type, Right.Expression.Type });

                var leftType = Left.Expression.Type;
                if (leftType != dynamic.Type)
                {
                    var convert = Binder.Convert(CSharpBinderFlags.ConvertExplicit, leftType, Context);
                    dynamic = DynamicHelpers.MakeDynamic(leftType, convert, new[] { dynamic }, null);
                }

                return dynamic;
            });

            var flags = Flags | CSharpBinderFlags.ValueFromCompoundAssignment;

            var res = DynamicHelpers.ReduceDynamicAssignment(left, functionalOp, flags);

            return res;
        }

        private static CSharpArgumentInfoFlags GetArgumentInfoFlags(DynamicCSharpArgument argument)
        {
            // TODO: Can we infer UseCompileTimeType?
            return argument.Flags;
        }

        private Expression WithEventCheck(GetMemberDynamicCSharpExpression member, string accessor)
        {
            var obj = member.Object;
            var name = member.Name;
            var args = member.Arguments;

            Debug.Assert(args.Count == 0);

            var lhs = Expression.Parameter(obj.Type, "__objTemp");
            var left = Left.Update(member.Update(lhs, args));

            var isEventBinder = Binder.IsEvent(CSharpBinderFlags.None, name, Context);
            var isEventCheck = DynamicHelpers.MakeDynamic(typeof(bool), isEventBinder, new[] { lhs }, null);

            var accessorName = accessor + name;
            var accessorArgs = new[]
            {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, name: null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, name: null),
            };

            var invokeAccessorBinder = Binder.InvokeMember(CSharpBinderFlags.InvokeSpecialName | CSharpBinderFlags.ResultDiscarded, accessorName, null, Context, accessorArgs);
            var invokeAccessor = DynamicHelpers.MakeDynamic(typeof(object), invokeAccessorBinder, new[] { lhs, Right.Expression }, null);

            var ifNotEvent = ReduceCore(left);

            var res =
                Expression.Block(
                    new[] { lhs },
                    Expression.Assign(lhs, obj),
                    Expression.Condition(isEventCheck, invokeAccessor, ifNotEvent)
                );

            return res;
        }

        /// <summary>
        /// Reduces the dynamic expression to a binder and a set of arguments to apply the operation to.
        /// </summary>
        /// <param name="binder">The binder used to perform the dynamic operation.</param>
        /// <param name="arguments">The arguments to apply the dynamic operation to.</param>
        /// <param name="argumentTypes">The types of the arguments to use for the dynamic call site. Return null to infer types.</param>
        protected override void ReduceDynamic(out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[]? argumentTypes) => throw Unreachable;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitDynamicBinaryAssign(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="left">The <see cref="Left" /> property of the result.</param>
        /// <param name="right">The <see cref="Right" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public AssignBinaryDynamicCSharpExpression Update(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            if (left == Left && right == Right)
            {
                return this;
            }

            return DynamicCSharpExpression.MakeDynamicBinaryAssign(OperationNodeType, left, right, Flags, Context);
        }
    }

    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates a new expression representing a dynamically bound binary assignment operation.
        /// </summary>
        /// <param name="binaryType">The type of the binary operation to perform.</param>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression MakeDynamicBinaryAssign(CSharpExpressionType binaryType, Expression left, Expression right) =>
            MakeDynamicBinaryAssign(binaryType, DynamicArgument(left), DynamicArgument(right), CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary assignment operation.
        /// </summary>
        /// <param name="binaryType">The type of the binary operation to perform.</param>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression MakeDynamicBinaryAssign(CSharpExpressionType binaryType, DynamicCSharpArgument left, DynamicCSharpArgument right) =>
            MakeDynamicBinaryAssign(binaryType, left, right, CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary assignment operation with the specified binder flags.
        /// </summary>
        /// <param name="binaryType">The type of the binary operation to perform.</param>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression MakeDynamicBinaryAssign(CSharpExpressionType binaryType, DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags) =>
            MakeDynamicBinaryAssign(binaryType, left, right, binderFlags, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary assignment operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="binaryType">The type of the binary operation to perform.</param>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression MakeDynamicBinaryAssign(CSharpExpressionType binaryType, DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type? context)
        {
            RequiresNotNull(left, nameof(left));
            RequiresNotNull(right, nameof(right));

            RequiresCanWrite(left.Expression, nameof(left));
            RequiresCanRead(right.Expression, nameof(right));

            CheckBinaryAssign(binaryType);

            switch (binaryType)
            {
                case CSharpExpressionType.AddAssignChecked:
                case CSharpExpressionType.SubtractAssignChecked:
                case CSharpExpressionType.MultiplyAssignChecked:
                    binderFlags |= CSharpBinderFlags.CheckedContext;
                    break;
            }

            return new AssignBinaryDynamicCSharpExpression(context, binderFlags, binaryType, left, right);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="AssignBinaryDynamicCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDynamicBinaryAssign(AssignBinaryDynamicCSharpExpression node) =>
            node.Update(
                VisitDynamicArgument(node.Left),
                VisitDynamicArgument(node.Right)
            );
    }
}
