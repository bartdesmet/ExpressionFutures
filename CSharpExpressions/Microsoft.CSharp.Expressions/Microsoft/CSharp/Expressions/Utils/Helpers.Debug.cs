// Prototyping extended expression trees for C#.
//
// bartde - October 2015

namespace Microsoft.CSharp.Expressions
{
    internal static partial class Helpers
    {
        public static string ToDebugString(this object? o)
        {
            return o?.ToString() ?? "null";
        }

        private static readonly object s_null = new object();

        public static object OrNullSentinel(this object? o)
        {
            return o ?? s_null;
        }
    }
}
