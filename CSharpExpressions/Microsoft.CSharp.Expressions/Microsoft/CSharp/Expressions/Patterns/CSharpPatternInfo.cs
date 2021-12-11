// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents type information about a pattern.
    /// </summary>
    public sealed class CSharpPatternInfo
    {
        internal CSharpPatternInfo(Type inputType, Type narrowedType)
        {
            InputType = inputType;
            NarrowedType = narrowedType;
        }

        /// <summary>
        /// Gets the <see cref="Type" /> of the input expressions handled by the pattern.
        /// </summary>
        public Type InputType { get; }

        /// <summary>
        /// Gets the <see cref="Type" /> the pattern narrows the input type to in case of a successful match.
        /// </summary>
        public Type NarrowedType { get; }
    }

    partial class CSharpPattern
    {
        /// <summary>
        /// Creates an object holding type information about a pattern.
        /// </summary>
        /// <param name="inputType">The type of the input expressions handled by the pattern.</param>
        /// <param name="narrowedType">The type the pattern narrows the input type to in case of a successful match.</param>
        /// <returns>A <see cref="CSharpPatternInfo" /> object holding type information about a pattern.</returns>
        public static CSharpPatternInfo PatternInfo(Type inputType, Type narrowedType)
        {
            checkPatternType(inputType, nameof(inputType));
            checkPatternType(narrowedType, nameof(narrowedType));

            // NB: Don't check for narrowedType.IsNullableType() because var patterns can produce it.

            return new CSharpPatternInfo(inputType, narrowedType);

            static void checkPatternType(Type type, string name)
            {
                RequiresNotNull(type, name);
                ValidatePatternType(type);
            }
        }
    }
}
