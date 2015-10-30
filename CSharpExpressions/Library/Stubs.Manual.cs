// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.ObjectModel;

namespace System.Runtime.CompilerServices
{
    // NOTE: This could should be removed in the product and use the real TrueReadOnlyCollection<T>.

    internal sealed class TrueReadOnlyCollection<T> : ReadOnlyCollection<T>
    {
        internal TrueReadOnlyCollection(T[] list) : base(list)
        {
        }
    }
}
