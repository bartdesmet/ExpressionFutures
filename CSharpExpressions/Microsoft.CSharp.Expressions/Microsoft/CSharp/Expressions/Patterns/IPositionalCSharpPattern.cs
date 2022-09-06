// Prototyping extended expression trees for C#.
//
// bartde - December 2021

#nullable enable

using System.Collections.ObjectModel;

namespace Microsoft.CSharp.Expressions
{
    // REVIEW: This is really an artifact of modeling ITuple as a separate type which does not derive from
    //         the CSharpObjectPattern base class (because there's no designation in this case). We should
    //         consider reflecting the C# grammar instead. For now, we'll use an interface to cover both
    //         cases of "recursive" and "ITuple" patterns, and expose the pattern type for the caller to
    //         disambiguate if they want to.

    /// <summary>
    /// Interface for positional patterns.
    /// </summary>
    public interface IPositionalCSharpPattern
    {
        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        CSharpPatternType PatternType { get; }

        /// <summary>
        /// Gets the subpatterns to apply to the positional components.
        /// </summary>
        ReadOnlyCollection<PositionalCSharpSubpattern> Deconstruction { get; }
    }
}
