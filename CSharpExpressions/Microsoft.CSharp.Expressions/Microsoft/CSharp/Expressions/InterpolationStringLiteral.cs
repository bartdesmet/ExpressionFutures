// Prototyping extended expression trees for C#.
//
// bartde - February 2020

using System.Dynamic.Utils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents an interpolation containing a string literal.
    /// </summary>
    public sealed class InterpolationStringLiteral : Interpolation
    {
        internal InterpolationStringLiteral(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the string literal used in the interpolation.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Interpolation Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitInterpolationStringLiteral(this);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="InterpolationStringLiteral"/> that represents an interpolation containing a string literal.
        /// </summary>
        /// <param name="value">The string literal used in the interpolation.</param>
        /// <returns>An instance of the <see cref="InterpolationStringLiteral"/>.</returns>
        public static InterpolationStringLiteral InterpolationStringLiteral(string value)
        {
            ContractUtils.RequiresNotNull(value, nameof(value));

            return new InterpolationStringLiteral(value);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="InterpolationStringLiteral" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Interpolation VisitInterpolationStringLiteral(InterpolationStringLiteral node)
        {
            return node;
        }
    }
}
