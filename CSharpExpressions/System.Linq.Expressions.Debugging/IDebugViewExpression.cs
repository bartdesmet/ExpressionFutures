// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Xml.Linq;

namespace System.Linq.Expressions
{
    // DESIGN: Maybe we just add an ExpressionVisitor<T> to the BCL?

    /// <summary>
    /// Interface for expressions that support generating a debug view.
    /// </summary>
    public interface IDebugViewExpression
    {
        /// <summary>
        /// Dispatches the current node to the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor to dispatch to.</param>
        /// <returns>The result of visiting the node.</returns>
        XNode Accept(IDebugViewExpressionVisitor visitor);
    }
}
