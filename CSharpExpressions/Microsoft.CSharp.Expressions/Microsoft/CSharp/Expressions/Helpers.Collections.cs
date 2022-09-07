// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System.Collections.Generic;
using System.Dynamic.Utils;

namespace Microsoft.CSharp.Expressions
{
    internal static partial class Helpers
    {
        public static bool SameElements<T>(ref IEnumerable<T>? replacement, IReadOnlyList<T>? current) where T : class
        {
            if (replacement == current) // Relatively common case, so particularly useful to take the short-circuit.
            {
                return true;
            }

            if (replacement == null) // Treat null as empty.
            {
                return current!.Count == 0;
            }

            // Ensure arguments is safe to enumerate twice.
            // If we have to build a collection, build a TrueReadOnlyCollection<T>
            // so it won't be built a second time if used.
            if (replacement is not ICollection<T> replacementCol)
            {
                replacement = replacementCol = replacement.ToReadOnly();
            }

            return SameElementsInCollection(replacementCol, current);
        }

        public static bool SameElementsInCollection<T>(ICollection<T> replacement, IReadOnlyList<T>? current) where T : class
        {
            int count = current?.Count ?? 0;

            if (replacement.Count != count)
            {
                return false;
            }

            if (current != null && count != 0)
            {
                int index = 0;

                foreach (T replacementObject in replacement)
                {
                    if (replacementObject != current[index])
                    {
                        return false;
                    }

                    index++;
                }
            }

            return true;
        }
    }
}
