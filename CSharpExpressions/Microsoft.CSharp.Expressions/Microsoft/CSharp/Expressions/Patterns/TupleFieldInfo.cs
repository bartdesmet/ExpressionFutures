// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents information about a tuple field for use in positional patterns.
    /// </summary>
    public sealed class TupleFieldInfo
    {
        internal TupleFieldInfo(string name, int index)
        {
            Name = name;
            Index = index;
        }

        /// <summary>
        /// Gets the user-specified name of the tuple field, if any.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the index of the tuple field.
        /// </summary>
        public int Index { get; }
    }
}
