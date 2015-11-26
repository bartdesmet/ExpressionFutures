// Prototyping extended expression trees for C#.
//
// bartde - October 2015

namespace System.Linq.Expressions
{
    // NB: This should be removed once we have fixes for the issues mentioned below.

    /// <summary>
    /// Expression visitor that fixes some issues.
    /// </summary>
    /// <remarks>
    /// Addresses the following issues:
    /// - https://github.com/dotnet/corefx/issues/4400
    /// </remarks>
    public class BetterExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Visits a block.
        /// </summary>
        /// <param name="node">The node to visit.</param>
        /// <returns>The result of visiting the node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class doesn't pass null.")]
        protected override Expression VisitBlock(BlockExpression node)
        {
            var variables = VisitAndConvert(node.Variables, nameof(VisitBlock));
            var expressions = Visit(node.Expressions);
            return node.Update(variables, expressions);
        }
    }
}
