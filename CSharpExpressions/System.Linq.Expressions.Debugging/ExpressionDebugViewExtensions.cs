// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Xml.Linq;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Provides static extension methods for debugging expression trees.
    /// </summary>
    public static class ExpressionDebugViewExtensions
    {
        /// <summary>
        /// Gets a debugging representation of the specified expression.
        /// </summary>
        /// <param name="expression">The expression to get a debug view for.</param>
        /// <returns>An XML node representing the structure of the specified expression.</returns>
        public static XNode DebugView(this Expression expression)
        {
            return new DebugViewExpressionVisitor().GetDebugView(expression);
        }
    }
}
