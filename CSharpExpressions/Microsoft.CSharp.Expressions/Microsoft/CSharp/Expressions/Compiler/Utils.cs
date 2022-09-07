// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Contains a set of utilities for code generation.
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Creates an expression to rethrow the exception specified in <paramref name="exception"/>, preserving the original stack trace if possible.
        /// </summary>
        /// <param name="exception">Expression representing the exception to rethrow. This expression can be of any type; if the type derives from <see cref="System.Exception"/>, the generated expression will use <see cref="ExceptionDispatchInfo.Throw(Exception)"/> to rethrow the exception preserving the stack trace.</param>
        /// <param name="beforeThrow">Expression to emit before the rethrow code.</param>
        /// <returns>Expression to rethrow the exception specified in <paramref name="exception"/>, optionally prepended by the expression specified in <paramref name="beforeThrow"/>.</returns>
        public static Expression CreateRethrow(Expression exception, Expression? beforeThrow = null)
        {
            var exprCount = (beforeThrow == null ? 0 : 1) /* before */ + 1 /* assign */ + 1 /* if */ + 1 /* clear */;

            var exprs = new Expression[exprCount];

            var i = 0;

            if (beforeThrow != null)
            {
                exprs[i++] = beforeThrow;
            }

            var exStronglyTyped = Expression.Parameter(typeof(Exception), "__exception");

            exprs[i++] =
                Expression.Assign(exStronglyTyped, Expression.TypeAs(exception, typeof(Exception)));

            exprs[i++] =
                Expression.IfThenElse(
                    Expression.ReferenceEqual(exStronglyTyped, Expression.Default(typeof(Exception))),
                    Expression.Throw(exception), // NB: The C# compiler doesn't emit code to null out the hoisted local; maybe we should?
                    Expression.Call(
                        Expression.Call(
                            ExceptionDispatchInfoCapture,
                            exStronglyTyped
                        ),
                        ExceptionDispatchInfoThrow
                    )
                );

            exprs[i++] =
                Expression.Assign(exception, Expression.Default(exception.Type));

            return
                Expression.Block(
                    typeof(void),
                    new[] { exStronglyTyped },
                    exprs
                );
        }

        private static MethodInfo? s_ediCapture, s_ediThrow;
        private static MethodInfo ExceptionDispatchInfoCapture => s_ediCapture ??= typeof(ExceptionDispatchInfo).GetMethod(nameof(ExceptionDispatchInfo.Capture), BindingFlags.Public | BindingFlags.Static)!; // TODO: well-known members
        private static MethodInfo ExceptionDispatchInfoThrow => s_ediThrow ??= typeof(ExceptionDispatchInfo).GetMethod(nameof(ExceptionDispatchInfo.Throw), BindingFlags.Public | BindingFlags.Instance)!; // TODO: well-known members
    }
}
