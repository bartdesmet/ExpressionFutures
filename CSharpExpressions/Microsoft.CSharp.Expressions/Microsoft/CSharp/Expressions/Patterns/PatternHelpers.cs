// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    internal static class PatternHelpers
    {
        public static Expression Reduce(Expression @object, Func<Expression, Expression> reduce) => Reduce(@object, reduce, vars: null, stmts: null);

        public static Expression Reduce(Expression @object, Func<Expression, Expression> reduce, List<ParameterExpression>? vars, List<Expression>? stmts)
        {
            if (@object.IsPure(readOnly: true))
            {
                if (@object is ParameterExpression p)
                {
                    // NB: Trick some purity checks down the line to not introduce more temps. Patterns do not cause assignments
                    //     to variables, so a variable input can be considered to be pure.

                    @object = new ReadOnlyTemporaryVariableExpression(p);
                }

                return reduce(@object);
            }
            else
            {
                var p = Expression.Parameter(@object.Type, "__obj");
                var r = new ReadOnlyTemporaryVariableExpression(p);

                if (vars != null)
                {
                    Debug.Assert(stmts != null);

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
                    throw Error.CannotUseNullValueInRelationalPattern(nameof(value));

                return;
            }

            if (value.Type.IsNullableType())
                throw Error.InvalidPatternConstantType(value.Type, nameof(value));

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
                        throw Error.CannotUsePatternConstantNaN(nameof(value));
                    break;
                case TypeCode.Double:
                    if (isRelational && double.IsNaN((double)value.Value))
                        throw Error.CannotUsePatternConstantNaN(nameof(value));
                    break;
                case TypeCode.Boolean:
                case TypeCode.String:
                    if (isRelational)
                        throw Error.InvalidRelationalPatternConstantType(value.Type, nameof(value));
                    break;
                default:
                    throw Error.InvalidPatternConstantType(value.Type, nameof(value));
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
            if (test is BlockExpression { Variables.Count: 0, Expressions.Count: 2, Result: ConstantExpression { Value: true } } b)
            {
                stmts.Add(b.Expressions[0]);
                return;
            }

            // NB: Peephole optimization for constant and relational patterns.
            if (test is BinaryExpression binary)
            {
                ExpressionType? GetInverted() =>
                    binary.NodeType switch
                    {
                        ExpressionType.Equal => ExpressionType.NotEqual,
                        ExpressionType.NotEqual => ExpressionType.Equal,
                        ExpressionType.LessThan => ExpressionType.GreaterThanOrEqual,
                        ExpressionType.LessThanOrEqual => ExpressionType.GreaterThan,
                        ExpressionType.GreaterThan => ExpressionType.LessThanOrEqual,
                        ExpressionType.GreaterThanOrEqual => ExpressionType.LessThan,
                        _ => null,
                    };

                var inverted = GetInverted();

                if (inverted != null)
                {
                    var invertedExpr = Expression.MakeBinary(inverted.Value, binary.Left, binary.Right);

                    AddFailIf(invertedExpr, exit, stmts);

                    return;
                }
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

        public static Expression AddNullCheck(Expression obj, Type? typeCheck, LabelTarget exit, List<ParameterExpression>  vars, List<Expression> stmts)
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
