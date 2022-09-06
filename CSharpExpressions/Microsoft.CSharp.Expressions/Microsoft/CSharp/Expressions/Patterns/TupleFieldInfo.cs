// Prototyping extended expression trees for C#.
//
// bartde - December 2021

#nullable enable

using System;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents information about a tuple field for use in positional patterns.
    /// </summary>
    public sealed class TupleFieldInfo
    {
        internal TupleFieldInfo(string? name, int index, Type type)
        {
            Name = name;
            Index = index;
            Type = type;
        }

        /// <summary>
        /// Gets the user-specified name of the tuple field, if any.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Gets the index of the tuple field.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the type of the tuple field.
        /// </summary>
        public Type Type { get; }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an object representing a tuple field.
        /// </summary>
        /// <param name="fieldName">The name of the field in the tuple to match.</param>
        /// <param name="fieldIndex">The index of the field in the tuple to match.</param>
        /// <param name="type">The type of the tuple field.</param>
        /// <returns>An object representing a tuple field.</returns>
        public static TupleFieldInfo TupleFieldInfo(string? fieldName, int fieldIndex, Type type)
        {
            if (fieldIndex < 0)
                throw Error.TupleFieldIndexMustBePositive();

            RequiresNotNull(type, nameof(type));
            ValidateType(type, nameof(type));

            return new TupleFieldInfo(fieldName, fieldIndex, type);
        }
    }
}
