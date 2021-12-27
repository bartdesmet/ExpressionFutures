// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a pattern that always matches.
    /// </summary>
    public sealed partial class DiscardCSharpPattern : CSharpPattern
    {
        internal DiscardCSharpPattern(CSharpPatternInfo info)
            : base(info)
        {
        }

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public override CSharpPatternType PatternType => CSharpPatternType.Discard;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpPattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitDiscardPattern(this);

        /// <summary>
        /// Changes the input type to the specified type.
        /// </summary>
        /// <remarks>
        /// This functionality can be used when a pattern is pass to an expression or statement that applies the pattern.
        /// </remarks>
        /// <param name="inputType">The new input type.</param>
        /// <returns>The original pattern rewritten to use the specified input type.</returns>
        public override CSharpPattern ChangeType(Type inputType)
        {
            if (inputType == InputType)
            {
                return this;
            }

            return CSharpPattern.Discard(inputType);
        }

        /// <summary>
        /// Reduces the pattern by applying it the specified object.
        /// </summary>
        /// <param name="object">The object to match the pattern against.</param>
        /// <returns>The expression representing the pattern applied to the specified object.</returns>
        internal override Expression Reduce(Expression @object)
        {
            // NB: RecursiveCSharpPattern has a peephole optimization for the pattern below.

            // NB: Ensure any side-effects in evaluating @object are retained.
            return PatternHelpers.Reduce(@object, _ => ConstantTrue);
        }
    }

    partial class CSharpPattern
    {
        /// <summary>
        /// Creates a pattern that always matches.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <returns>A <see cref="DiscardCSharpPattern" /> that represents a pattern that always matches.</returns>
        public static DiscardCSharpPattern Discard(CSharpPatternInfo info)
        {
            if (info != null)
            {
                if (info.InputType != info.NarrowedType)
                    throw Error.PatternInputAndNarrowedTypeShouldMatch(nameof(CSharpPatternType.Discard));
            }
            else
            {
                info = PatternInfo(typeof(object), typeof(object));
            }

            return new DiscardCSharpPattern(info);
        }

        /// <summary>
        /// Creates a pattern that always matches.
        /// </summary>
        /// <param name="type">The type of the input of the pattern.</param>
        /// <returns>A <see cref="DiscardCSharpPattern" /> that represents a pattern that always matches.</returns>
        public static DiscardCSharpPattern Discard(Type type) => Discard(PatternInfo(type, type));

        /// <summary>
        /// Creates a pattern that always matches.
        /// </summary>
        /// <returns>A <see cref="DiscardCSharpPattern" /> that represents a pattern that always matches.</returns>
        public static DiscardCSharpPattern Discard() => Discard(info: null);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="DiscardCSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitDiscardPattern(DiscardCSharpPattern node) => node;
    }
}
