// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace Microsoft.CSharp.Expressions
{
    public abstract class CSharpSubpattern
    {
        protected CSharpSubpattern(CSharpPattern pattern)
        {
            Pattern = pattern;
        }

        public abstract CSharpSubpatternType SubpatternType { get; }

        public CSharpPattern Pattern { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal abstract CSharpSubpattern Accept(CSharpExpressionVisitor visitor);
    }
}
