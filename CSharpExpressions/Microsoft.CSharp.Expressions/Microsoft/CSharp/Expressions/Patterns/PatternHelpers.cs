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
        public static Expression Reduce(Expression @object, Func<Expression, Expression> reduce) => Reduce(@object, reduce, vars: null, stmts: null);

        public static Expression Reduce(Expression @object, Func<Expression, Expression> reduce, List<ParameterExpression> vars, List<Expression> stmts)
        {
            if (@object.IsPure(readOnly: true))
            {
                return reduce(@object);
            }
            else
            {
                var p = Expression.Parameter(@object.Type, "__obj");
                var r = new ReadOnlyTemporaryVariableExpression(p);

                if (vars != null)
                {
                    vars.Add(p);
                    stmts.Add(Expression.Assign(p, @object));
                    return reduce(r);
                }
                else
                {
                    return
                        Expression.Block(
                            new[] { p },
                            Expression.Assign(p, @object),
                            reduce(r)
                        );
                }
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

        public static void AddFailIf(Expression test, LabelTarget exit, List<Expression> stmts)
        {
            var expr =
                Expression.IfThen(
                    test,
                    Expression.Goto(exit, ConstantFalse)
                );

            stmts.Add(expr);
        }

        public static Expression AddNullCheck(Expression obj, Type typeCheck, LabelTarget exit, List<ParameterExpression>  vars, List<Expression> stmts)
        {
            void emitTypeCheck(Type type)
            {
                // NB: Implies null check.
                AddFailIfNot(Expression.TypeIs(obj, type), exit, stmts);

                var temp = Expression.Parameter(type, "__objT");

                vars.Add(temp);
                stmts.Add(Expression.Assign(temp, Expression.Convert(obj, type)));

                obj = temp;
            }

            if (typeCheck != null)
            {
                emitTypeCheck(typeCheck);
            }
            else if (!obj.Type.IsValueType)
            {
                AddFailIf(Expression.ReferenceEqual(obj, Expression.Constant(null, obj.Type)), exit, stmts);
            }
            else if (obj.Type.IsNullableType())
            {
                emitTypeCheck(obj.Type.GetNonNullableType());
            }

            return obj;
        }
    }
}
