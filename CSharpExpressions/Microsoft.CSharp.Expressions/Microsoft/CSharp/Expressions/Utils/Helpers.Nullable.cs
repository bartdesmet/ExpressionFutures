// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq.Expressions;

using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    internal static partial class Helpers
    {
        public static Expression MakeNullableHasValue(Expression e)
        {
            Debug.Assert(e.Type.IsNullableType());

            return Expression.Property(e, "HasValue");
        }

        public static Expression MakeNullableGetValueOrDefault(Expression e)
        {
            Debug.Assert(e.Type.IsNullableType());

            return Expression.Call(e, "GetValueOrDefault", typeArguments: null);
        }

        public static Expression MakeNullableGetValue(Expression e)
        {
            Debug.Assert(e.Type.IsNullableType());

            return Expression.Property(e, "Value");
        }
    }
}
