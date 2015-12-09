// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a dynamically bound binary operation.
    /// </summary>
    public sealed partial class BinaryDynamicCSharpExpression : DynamicCSharpExpression
    {
        internal BinaryDynamicCSharpExpression(Type context, CSharpBinderFlags binderFlags, ExpressionType binaryType, DynamicCSharpArgument left, DynamicCSharpArgument right)
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
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.DynamicBinary;

        /// <summary>
        /// Gets the expression kind of the dynamically bound operation.
        /// </summary>
        public ExpressionType OperationNodeType { get; }

        /// <summary>
        /// Gets the left operand of the binary operation.
        /// </summary>
        public DynamicCSharpArgument Left { get; }

        /// <summary>
        /// Gets the right operand of the binary operation.
        /// </summary>
        public DynamicCSharpArgument Right { get; }

        /// <summary>
        /// Reduces the dynamic expression to a binder and a set of arguments to apply the operation to.
        /// </summary>
        /// <param name="binder">The binder used to perform the dynamic operation.</param>
        /// <param name="arguments">The arguments to apply the dynamic operation to.</param>
        /// <param name="argumentTypes">The types of the arguments to use for the dynamic call site. Return null to infer types.</param>
        protected override void ReduceDynamic(out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[] argumentTypes)
        {
            // TODO: AndAlso and OrElse need IsFalse and IsTrue unary operations as well

            binder = Binder.BinaryOperation(Flags, OperationNodeType, Context, new[] { Left.ArgumentInfo, Right.ArgumentInfo });
            arguments = new[] { Left.Expression, Right.Expression };
            argumentTypes = null;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitDynamicBinary(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="left">The <see cref="Left" /> property of the result.</param>
        /// <param name="right">The <see cref="Right" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public BinaryDynamicCSharpExpression Update(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            if (left == this.Left && right == this.Right)
            {
                return this;
            }

            return DynamicCSharpExpression.MakeDynamicBinary(OperationNodeType, left, right, Flags, Context);
        }
    }

    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates a new expression representing a dynamically bound binary operation.
        /// </summary>
        /// <param name="binaryType">The type of the binary operation to perform.</param>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static BinaryDynamicCSharpExpression MakeDynamicBinary(ExpressionType binaryType, Expression left, Expression right)
        {
            return MakeDynamicBinary(binaryType, DynamicArgument(left), DynamicArgument(right), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary operation.
        /// </summary>
        /// <param name="binaryType">The type of the binary operation to perform.</param>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static BinaryDynamicCSharpExpression MakeDynamicBinary(ExpressionType binaryType, DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(binaryType, left, right, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary operation with the specified binder flags.
        /// </summary>
        /// <param name="binaryType">The type of the binary operation to perform.</param>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static BinaryDynamicCSharpExpression MakeDynamicBinary(ExpressionType binaryType, DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(binaryType, left, right, binderFlags, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="binaryType">The type of the binary operation to perform.</param>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static BinaryDynamicCSharpExpression MakeDynamicBinary(ExpressionType binaryType, DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            ContractUtils.RequiresNotNull(left, nameof(left));
            ContractUtils.RequiresNotNull(right, nameof(right));

            CheckBinary(binaryType);

            switch (binaryType)
            {
                case ExpressionType.AddChecked:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.SubtractChecked:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.SubtractAssignChecked:
                    binderFlags |= CSharpBinderFlags.CheckedContext;
                    break;
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    binderFlags |= CSharpBinderFlags.BinaryOperationLogical;
                    break;
            }

            return new BinaryDynamicCSharpExpression(context, binderFlags, binaryType, left, right);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="BinaryDynamicCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDynamicBinary(BinaryDynamicCSharpExpression node)
        {
            return node.Update(VisitDynamicArgument(node.Left), VisitDynamicArgument(node.Right));
        }
    }
}
