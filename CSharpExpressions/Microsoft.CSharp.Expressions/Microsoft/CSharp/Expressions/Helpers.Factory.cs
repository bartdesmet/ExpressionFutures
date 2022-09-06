﻿// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System;
using System.Collections.Generic;
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

        public static Expression CreateVoid(params Expression[]? expressions)
        {
            return CreateVoid((IList<Expression>?)expressions);
        }

        public static Expression CreateVoid(IList<Expression>? expressions)
        {
            if (expressions == null || expressions.Count == 0)
            {
                return Expression.Empty();
            }

            if (expressions.Count == 1)
            {
                var expression = expressions[0];

                if (expression.Type == typeof(void))
                {
                    return expression;
                }

                if (expression is BlockExpression block)
                {
                    return Expression.Block(typeof(void), block.Variables, block.Expressions);
                }
            }

            return Expression.Block(typeof(void), expressions);
        }

        public static Expression Comma(List<ParameterExpression> variables, List<Expression> statements)
        {
            if (variables.Count == 0 && statements.Count == 1)
            {
                return statements[0];
            }

            return Expression.Block(variables, statements);
        }
    }
}
