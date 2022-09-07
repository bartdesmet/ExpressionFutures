﻿// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

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

        public static Expression Apply(LambdaExpression lambda, Expression expression)
        {
            // TODO: We can beta reduce the lambda explicitly here, iff the specified expression is pure
            //       or is used exactly once in the lambda before any other observable side-effect (i.e.
            //       don't cause re-evalation or out-of-order evaluation of e.g. an indexer or member).
            //
            //       The code below does a basic version of this, well-suited for the conversions that
            //       get generated by the compound assignment factories. See a DESIGN note in BinaryAssign
            //       for considerations on not using a lambda for that case.
            //
            //       Note that the LINQ expression compiler also inlines Invoke(Lambda, args) expressions
            //       so this optimization is mostly to make Reduce return a nicer form.

            if (lambda.Parameters.Count == 1)
            {
                var parameter = lambda.Parameters[0];
                var body = lambda.Body;

                switch (body.NodeType)
                {
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        var convert = (UnaryExpression)body;
                        if (convert.Operand == parameter)
                        {
                            return convert.Update(expression);
                        }
                        break;
                }
            }

            return Expression.Invoke(lambda, expression);
        }

        public static Expression InvokeLambdaWithSingleParameter(LambdaExpression lambda, Expression argument)
        {
            // NB: The reduction of extensions enables e.g. the use of MethodCallCSharpExpression with
            //     named and optional parameters to get reduced to a simpler construct. In the common case
            //     of an optional parameter for GetAsyncEnumerator(CancellationToken token = default), we
            //     end with up a MethodCallExpression that fills in the missing argument.

            var body = lambda.Body.ReduceExtensions();

            if (body is MethodCallExpression call)
            {
                var parameter = lambda.Parameters[0];
                var method = call.Method;

                var args = call.Arguments;

                bool RemainderArgsArePure(int firstIndex)
                {
                    for (int i = firstIndex; i < args.Count; i++)
                    {
                        var arg = args[i];

                        if (!IsPure(arg))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                // NB: This covers the common cases of e.g. Get[Async]Enumerator instance and extension
                //     methods that are wrapped in lambda expressions in EnumeratorInfo. The purity check
                //     for remaining parameters deals with defaults for optional parameters that end up
                //     as Constant nodes in these expressions (e.g. CancellationToken token = default).

                if (method.IsStatic)
                {
                    if (args.Count >= 1 && args[0] == parameter && RemainderArgsArePure(firstIndex: 1))
                    {
                        var newArgs = new List<Expression>(args)
                        {
                            [0] = argument
                        };

                        return call.Update(call.Object, newArgs);
                    }
                }
                else
                {
                    if (call.Object == parameter && RemainderArgsArePure(firstIndex: 0))
                    {
                        return call.Update(argument, args);
                    }
                }
            }

            return Expression.Invoke(lambda, argument);
        }

        public static Expression MakeWriteable(Expression lhs)
        {
            if (lhs is DiscardCSharpExpression)
            {
                return lhs.Reduce();
            }

            if (lhs.NodeType == ExpressionType.ArrayIndex)
            {
                var arrayIndex = (BinaryExpression)lhs;
                return Expression.ArrayAccess(arrayIndex.Left, arrayIndex.Right);
            }

            if (lhs.NodeType == ExpressionType.Call)
            {
                var arrayIndex = (MethodCallExpression)lhs;
                if (IsArrayAssignment(arrayIndex, out var array))
                {
                    return Expression.ArrayAccess(array, arrayIndex.Arguments);
                }
            }

            return lhs;
        }

        private static bool IsArrayAssignment(MethodCallExpression call, [NotNullWhen(true)] out Expression? array)
        {
            if (call.Object is Expression { Type: var type } obj)
            {
                var method = call.Method;

                if (!method.IsStatic && type.IsArray && method == type.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance))
                {
                    array = obj;
                    return true;
                }
            }

            array = null;
            return false;
        }
    }
}
