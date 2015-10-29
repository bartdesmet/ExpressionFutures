// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a visitor or rewriter for C# expression trees.
    /// </summary>
    /// <remarks>
    /// This class is designed to be inherited to create more specialized
    /// classes whose functionality requires traversing, examining or copying
    /// an expression tree.
    /// </remarks>
    public abstract partial class CSharpExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the extension expression.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitExtension(Expression node)
        {
            var csharpExpression = node as CSharpExpression;
            if (csharpExpression != null)
            {
                return csharpExpression.Accept(this);
            }

            return base.VisitExtension(node);
        }
    }
}
