// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    public sealed class AwaitCSharpExpression : UnaryCSharpExpression
    {
        internal AwaitCSharpExpression(Expression operand)
            :base(operand)
        {
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Await;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitAwait(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="operand">The <see cref="Operand" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public AwaitCSharpExpression Update(Expression operand)
        {
            if (operand == Operand)
            {
                return this;
            }

            return CSharpExpression.Await(operand);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an <see cref="AwaitCSharpExpression"/> that represents awaiting an asynchronous operation.
        /// </summary>
        /// <param name="operand">An <see cref="Expression" /> that specifies the asynchronous operation to await.</param>
        /// <returns>An instance of the <see cref="AwaitCSharpExpression"/>.</returns>
        public static AwaitCSharpExpression Await(Expression operand)
        {
            ContractUtils.RequiresNotNull(operand, nameof(operand));

            // TODO: Overloads that specify the awaiter to use, validation of await pattern, etc.

            return new AwaitCSharpExpression(operand);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="AwaitCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitAwait(AwaitCSharpExpression node)
        {
            return node.Update(Visit(node.Operand));
        }
    }
}
