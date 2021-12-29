// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    internal static class PatternHelpers
    {
        public static Expression Reduce(Expression @object, Func<Expression, Expression> reduce)
        {
            if (@object.NodeType == ExpressionType.Parameter)
            {
                return reduce(@object);
            }
            else
            {
                var p = Expression.Parameter(@object.Type, "__obj");

                return
                    Expression.Block(
                        new[] { p },
                        Expression.Assign(p, @object),
                        reduce(p)
                    );
            }
        }

        public static Expression Reduce(Expression @object, Func<Expression, Expression> reduce, List<ParameterExpression> vars, List<Expression> stmts)
        {
            if (@object.NodeType == ExpressionType.Parameter)
            {
                return reduce(@object);
            }
            else
            {
                var p = Expression.Parameter(@object.Type, "__obj");
                vars.Add(p);
                stmts.Add(Expression.Assign(p, @object));
                return reduce(p);
            }
        }

        public static void CheckConstant(ConstantExpression value, bool isRelational)
        {
            if (value.Value == null)
            {
                if (isRelational)
                    throw Error.CannotUseNullValueInRelationalPattern();

                return;
            }

            if (value.Type.IsNullableType() && value.Value != null)
                throw Error.InvalidPatternConstantType(value.Type);

            if (value.Type == typeof(IntPtr) || value.Type == typeof(UIntPtr))
                return;

            switch (Type.GetTypeCode(value.Type.GetNonNullableType()))
            {
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                    break;
                case TypeCode.Single:
                    if (isRelational && float.IsNaN((float)value.Value))
                        throw Error.CannotUsePatternConstantNaN();
                    break;
                case TypeCode.Double:
                    if (isRelational && double.IsNaN((double)value.Value))
                        throw Error.CannotUsePatternConstantNaN();
                    break;
                case TypeCode.Boolean:
                case TypeCode.String:
                    if (isRelational)
                        throw Error.InvalidRelationalPatternConstantType(value.Type);
                    break;
                default:
                    throw Error.InvalidPatternConstantType(value.Type);
            }
        }

        public static void AddFailIfNot(Expression test, LabelTarget exit, List<Expression> stmts)
        {
            // NB: Peephole optimization for _ pattern.
            if (test is ConstantExpression { Value: true })
            {
                return;
            }

            // NB: Peephole optimization for var pattern.
            if (test is BlockExpression b && b.Variables.Count == 0 && b.Expressions.Count == 2 && b.Result is ConstantExpression { Value: true })
            {
                stmts.Add(b.Expressions[0]);
                return;
            }

            var expr =
                Expression.IfThen(
                    Expression.Not(test),
                    Expression.Goto(exit, ConstantFalse)
                );

            stmts.Add(expr);
        }
    }
}
