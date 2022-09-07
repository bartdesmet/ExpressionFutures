// Prototyping extended expression trees for C#.
//
// bartde - February 2020

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents an interpolation used in an interpolated string expression.
    /// </summary>
    public abstract partial class Interpolation
    {
        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal abstract Interpolation Accept(CSharpExpressionVisitor visitor);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="Interpolation" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Interpolation VisitInterpolation(Interpolation node) => node.Accept(this);
    }
}
