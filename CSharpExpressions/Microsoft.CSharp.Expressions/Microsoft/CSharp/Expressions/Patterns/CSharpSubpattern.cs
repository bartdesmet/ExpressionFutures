// Prototyping extended expression trees for C#.
//
// bartde - December 2021

#nullable enable

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Base class for subpatterns.
    /// </summary>
    public abstract partial class CSharpSubpattern
    {
        internal CSharpSubpattern(CSharpPattern pattern) => Pattern = pattern;

        /// <summary>
        /// Gets the type of the subpattern.
        /// </summary>
        public abstract CSharpSubpatternType SubpatternType { get; }

        /// <summary>
        /// Gets the pattern applied by the subpattern.
        /// </summary>
        public CSharpPattern Pattern { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal abstract CSharpSubpattern Accept(CSharpExpressionVisitor visitor);
    }
}
