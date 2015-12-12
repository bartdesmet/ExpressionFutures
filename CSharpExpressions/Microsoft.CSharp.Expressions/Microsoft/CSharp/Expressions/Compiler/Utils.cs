// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Microsoft.CSharp.Expressions.Compiler
{
    static class Utils
    {
        public static Expression CreateRethrow(Expression err, Expression beforeThrow = null)
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
                Expression.Assign(exStronglyTyped, Expression.TypeAs(err, typeof(Exception)));

            exprs[i++] =
                Expression.IfThenElse(
                    Expression.ReferenceEqual(exStronglyTyped, Expression.Default(typeof(Exception))),
                    Expression.Throw(err), // NB: The C# compiler doesn't emit code to null out the hoisted local; maybe we should?
                    Expression.Call(
                        Expression.Call(
                            typeof(ExceptionDispatchInfo).GetMethod(nameof(ExceptionDispatchInfo.Capture), BindingFlags.Public | BindingFlags.Static),
                            exStronglyTyped
                        ),
                        typeof(ExceptionDispatchInfo).GetMethod(nameof(ExceptionDispatchInfo.Throw), BindingFlags.Public | BindingFlags.Instance)
                    )
                );

            exprs[i++] =
                Expression.Assign(err, Expression.Default(err.Type));

            return
                Expression.Block(
                    typeof(void),
                    new[] { exStronglyTyped },
                    exprs
                );
        }
    }
}
