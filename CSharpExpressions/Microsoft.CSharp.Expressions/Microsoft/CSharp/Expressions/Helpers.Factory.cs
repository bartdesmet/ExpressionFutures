// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System;
using System.Linq.Expressions;

using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    internal static partial class Helpers
    {
        private const int MinConstInt32 = -2;
        private const int MaxConstInt32 = 7;
        private static ConstantExpression[]? s_constInt32;

        public static ConstantExpression CreateConstantInt32(int value)
        {
            if (value >= MinConstInt32 && value <= MaxConstInt32)
            {
                var index = value - MinConstInt32;
                var consts = s_constInt32 ??= new ConstantExpression[MaxConstInt32 - MinConstInt32 + 1];
                return consts[index] ?? (consts[index] = Expression.Constant(value));
            }

            return Expression.Constant(value);
        }

        private static ConstantExpression? s_true, s_false;

        public static ConstantExpression ConstantTrue => s_true ??= Expression.Constant(true);
        public static ConstantExpression ConstantFalse => s_false ??= Expression.Constant(false);

        public static ConstantExpression CreateConstantBoolean(bool value) => value ? ConstantTrue : ConstantFalse;

        private static ConstantExpression? s_nullConstant;

        public static ConstantExpression ConstantNull => s_nullConstant ??= Expression.Constant(null, typeof(object));

        public static Expression CreateConvert(Expression expr, Type type)
        {
            if (AreEquivalent(expr.Type, type))
            {
                return expr;
            }

            return Expression.Convert(expr, type);
        }
    }
}
