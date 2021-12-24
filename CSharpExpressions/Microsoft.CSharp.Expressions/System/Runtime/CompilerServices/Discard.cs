// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides an assignment target for discard expressions that were not optimized out.
    /// </summary>
    /// <typeparam name="T">The type of the value to assign.</typeparam>
    public static class Discard<T>
    {
        /// <summary>
        /// The field to assign to.
        /// </summary>
        public static T _;
    }
}
