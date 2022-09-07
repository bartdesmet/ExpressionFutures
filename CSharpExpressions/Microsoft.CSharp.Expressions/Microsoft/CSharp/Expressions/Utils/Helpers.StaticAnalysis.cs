// Prototyping extended expression trees for C#.
//
// bartde - October 2015

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

        public static bool IsPure(this Expression expression, bool readOnly = false)
        {
            //
            // CONSIDER: Improve purity analysis to handle more cases.
            //
            // - Convert can be pure for some cases when applied to a pure operand, e.g.
            //   - nullable lifting T -> T?
            //   - widening conversions, e.g. char to int
            //   - conversions to a base type or an interface type known to be implemented
            // - New can be pure for T? if the value is pure
            //

            switch (expression.NodeType)
            {
                case ExpressionType.Default:

                //
                // PERF: There's a caveat on allowing a ConstantExpression to be duplicated if it's a non-primitive
                //       value type, because the generated code will repeatedly unbox it from an `object` storage
                //       slot in the compiled lambda's environment. We should likely have more options on IsPure to
                //       indicate the usage intent if the expression is pure, e.g. `UseManyTimes`.
                //
                case ExpressionType.Constant:

                case ExpressionType.Unbox:
                case ExpressionType.Lambda:
                case ExpressionType.Quote:
                    return true;

                // NB: Parameters are only pure if used in a read-only setting, i.e. they can be dropped without
                //     loss of side-effects. If they can be assigned to, e.g. in the context of named parameter
                //     analysis where a parameter occurs in an argument and may get assigned to, it is unsafe to
                //     consider them pure for that purpose.
                case ExpressionType.Parameter:
                    return readOnly;
            }

            if (expression is ReadOnlyTemporaryVariableExpression)
            {
                return true;
            }

            return false;
        }
    }
}
