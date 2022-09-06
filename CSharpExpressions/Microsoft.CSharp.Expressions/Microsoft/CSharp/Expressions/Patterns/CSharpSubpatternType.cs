// Prototyping extended expression trees for C#.
//
// bartde - December 2021

#nullable enable

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Describes the subpattern types for the C# expression tree pattern nodes.
    /// </summary>
    public enum CSharpSubpatternType
    {
        /// <summary>
        /// A positional subpattern, e.g. 'x: 1' in 'Point (x: 1, y: 2)'.
        /// </summary>
        Positional,

        /// <summary>
        /// A property subpattern, e.g. 'X: 1' in 'Point { X: 1, Y: 2 }'.
        /// </summary>
        Property,
    }
}
