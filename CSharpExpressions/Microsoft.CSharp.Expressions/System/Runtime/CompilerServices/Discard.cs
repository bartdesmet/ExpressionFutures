// Prototyping extended expression trees for C#.
//
// bartde - December 2021

#nullable enable

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides an assignment target for discard expressions that were not optimized out.
    /// </summary>
    /// <typeparam name="T">The type of the value to assign.</typeparam>
    public static class Discard<T>
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible. (By design for discard.)

        /// <summary>
        /// The field to assign to.
        /// </summary>
        public static T? _;

#pragma warning restore CA2211 // Non-constant fields should not be visible. (By design for discard.)
    }
}
