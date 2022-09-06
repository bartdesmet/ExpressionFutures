// Prototyping extended expression trees for C#.
//
// bartde - December 2021

#nullable enable

using System;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Base class for conversions.
    /// </summary>
    public abstract partial class Conversion
    {
        /// <summary>
        /// Gets the input type of the conversion.
        /// </summary>
        public abstract Type InputType { get; }

        /// <summary>
        /// Gets the result type of the conversion.
        /// </summary>
        public abstract Type ResultType { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal abstract Conversion Accept(CSharpExpressionVisitor visitor);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="Conversion" />.
        /// </summary>
        /// <param name="node">The conversion to visit.</param>
        /// <returns>The modified conversion, if it or any subexpression was modified; otherwise, returns the original conversion.</returns>
        protected internal virtual Conversion VisitConversion(Conversion node) => node.Accept(this);
    }
}
