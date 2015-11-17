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
    /// Represents a dynamically bound unary operation.
    /// </summary>
    public sealed class UnaryDynamicCSharpExpression : DynamicCSharpExpression
    {
        internal UnaryDynamicCSharpExpression(Type context, CSharpBinderFlags binderFlags, ExpressionType unaryType, DynamicCSharpArgument operand)
            : base(context, binderFlags)
        {
            OperationNodeType = unaryType;
            Operand = operand;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.DynamicUnary;

        /// <summary>
        /// Gets the expression kind of the dynamically bound operation.
        /// </summary>
        public ExpressionType OperationNodeType { get; }

        /// <summary>
        /// Gets the operand of the unary operation.
        /// </summary>
        public DynamicCSharpArgument Operand { get; }

        /// <summary>
        /// Reduces the dynamic expression to a binder and a set of arguments to apply the operation to.
        /// </summary>
        /// <param name="binder">The binder used to perform the dynamic operation.</param>
        /// <param name="arguments">The arguments to apply the dynamic operation to.</param>
        /// <param name="argumentTypes">The types of the arguments to use for the dynamic call site. Return null to infer types.</param>
        protected override void ReduceDynamic(out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[] argumentTypes)
        {
            binder = Binder.UnaryOperation(Flags, OperationNodeType, Context, new[] { Operand.ArgumentInfo });
            arguments = new[] { Operand.Expression };
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
            return visitor.VisitDynamicUnary(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="operand">The <see cref="Operand" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public UnaryDynamicCSharpExpression Update(DynamicCSharpArgument operand)
        {
            if (operand == this.Operand)
            {
                return this;
            }

            return DynamicCSharpExpression.MakeDynamicUnary(OperationNodeType, operand, Flags, Context);
        }
    }

    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates a new expression representing a dynamically bound unary operation.
        /// </summary>
        /// <param name="unaryType">The type of the unary operation to perform.</param>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary operation.</returns>
        public static UnaryDynamicCSharpExpression MakeDynamicUnary(ExpressionType unaryType, Expression operand)
        {
            return MakeDynamicUnary(unaryType, DynamicArgument(operand), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary operation.
        /// </summary>
        /// <param name="unaryType">The type of the unary operation to perform.</param>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary operation.</returns>
        public static UnaryDynamicCSharpExpression MakeDynamicUnary(ExpressionType unaryType, DynamicCSharpArgument operand)
        {
            return MakeDynamicUnary(unaryType, operand, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary operation with the specified binder flags.
        /// </summary>
        /// <param name="unaryType">The type of the unary operation to perform.</param>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary operation.</returns>
        public static UnaryDynamicCSharpExpression MakeDynamicUnary(ExpressionType unaryType, DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnary(unaryType, operand, binderFlags, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="unaryType">The type of the unary operation to perform.</param>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary operation.</returns>
        public static UnaryDynamicCSharpExpression MakeDynamicUnary(ExpressionType unaryType, DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            ContractUtils.RequiresNotNull(operand, nameof(operand));

            CheckUnary(unaryType);

            switch (unaryType)
            {
                case ExpressionType.NegateChecked:
                case ExpressionType.ConvertChecked:
                    binderFlags |= CSharpBinderFlags.CheckedContext;
                    break;
            }

            return new UnaryDynamicCSharpExpression(context, binderFlags, unaryType, operand);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="UnaryDynamicCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDynamicUnary(UnaryDynamicCSharpExpression node)
        {
            return node.Update(VisitDynamicArgument(node.Operand));
        }
    }
}
