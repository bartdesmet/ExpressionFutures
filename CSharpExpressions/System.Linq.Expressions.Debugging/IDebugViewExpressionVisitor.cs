// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Xml.Linq;

namespace System.Linq.Expressions
{
    // DESIGN: Maybe we just add an ExpressionVisitor<T> to the BCL?

    /// <summary>
    /// Interface for expression visitors that produce a debug view.
    /// </summary>
    public interface IDebugViewExpressionVisitor
    {
        /// <summary>
        /// Gets the debug view for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to get a debug view for.</param>
        /// <returns>Debug view for the specified expression.</returns>
        XNode GetDebugView(Expression expression);
    }
}
