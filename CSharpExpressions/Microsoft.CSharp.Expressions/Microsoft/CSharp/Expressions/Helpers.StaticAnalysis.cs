// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System.Dynamic.Utils;
using System.Linq.Expressions;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    internal static partial class Helpers
    {
        public static bool IsConst(Expression e, bool value)
        {
            return e is ConstantExpression { Value: var val } && val is bool b && b == value;
        }

        public static bool IsAlwaysNull(Expression e)
        {
            // NB: Could add support for no-op conversions.

            return e.NodeType switch
            {
                ExpressionType.Constant => ((ConstantExpression)e).Value == null,
                ExpressionType.Default => !e.Type.IsValueType || e.Type.IsNullableType(),
                ExpressionType.New => e.Type.IsNullableType() && ((NewExpression)e).Arguments.Count == 0, // e.g. new int?()
                _ => false,
            };
        }

        public static bool IsNeverNull(Expression e)
        {
            // NB: Could add support for no-op conversions.

            return e.NodeType switch
            {
                ExpressionType.Constant => ((ConstantExpression)e).Value != null,
                ExpressionType.New => e.Type.IsNullableType() && ((NewExpression)e).Arguments.Count == 1, // e.g. new int?(42)
                _ => false,
            };
        }
    }
}
