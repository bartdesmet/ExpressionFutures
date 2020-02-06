// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    [TestClass]
    public partial class CompilerTests
    {
        private TDelegate Compile<TDelegate>(string code)
        {
            var res = TestUtilities.FuncEval<TDelegate>(code);

            var exp = res.Expression.Compile();
            var fnc = res.Function;
            var log = res.Log;

            var invoke = typeof(TDelegate).GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var resultType = returnType == typeof(void) ? typeof(object) : returnType;
            var parameters = invoke.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();

            var evalExp = CreateInvoke<TDelegate>(exp, returnType, parameters, log);
            var evalFnc = CreateInvoke<TDelegate>(fnc, returnType, parameters, log);

            var evalExpVar = Expression.Parameter(evalExp.Type);
            var evalFncVar = Expression.Parameter(evalFnc.Type);

            var evalExpAsg = Expression.Assign(evalExpVar, evalExp);
            var evalFncAsg = Expression.Assign(evalFncVar, evalFnc);

            var assertMethod = typeof(CompilerTests).GetMethod(nameof(CheckResults), BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(resultType);
            var assert = Expression.Call(assertMethod, evalExpVar, evalFncVar);

            var returnMethod = evalFnc.Type.GetMethod("Return");
            var body = Expression.Block(new[] { evalExpVar, evalFncVar }, evalExpAsg, evalFncAsg, assert, Expression.Call(evalFncVar, returnMethod));

            var f = Expression.Lambda<TDelegate>(body, parameters);
            return f.Compile();
        }

        private static void CheckResults<TResult>(LogAndResult<TResult> expression, LogAndResult<TResult> function)
        {
            if (!expression.Equals(function))
            {
                throw new InvalidOperationException("Results don't match.");
            }
        }

        private static Expression CreateInvoke<TDelegate>(TDelegate f, Type returnType, ParameterExpression[] parameters, List<string> log)
        {
            var inv = Expression.Invoke(Expression.Constant(f, typeof(TDelegate)), parameters);

            var resultType = returnType == typeof(void) ? typeof(object) : returnType;
            var result = Expression.Parameter(resultType);

            var eval = (Expression)inv;
            if (returnType != typeof(void))
            {
                eval = Expression.Block(typeof(void), Expression.Assign(result, eval));
            }

            var err = Expression.Parameter(typeof(Exception));
            var ex = Expression.Parameter(typeof(Exception));
            eval = Expression.TryCatch(eval, Expression.Catch(ex, Expression.Block(typeof(void), Expression.Assign(err, ex))));

            var logAndResultType = typeof(LogAndResult<>).MakeGenericType(resultType);
            var logAndResultCtor = logAndResultType.GetConstructors().Single(c => c.GetParameters().Length == 3);

            var logValue = Expression.Constant(log);
            var copyLog = Expression.New(typeof(List<string>).GetConstructor(new[] { typeof(IEnumerable<string>) }), logValue);
            var clearLog = Expression.Call(logValue, typeof(List<string>).GetMethod("Clear", Array.Empty<Type>()));

            var loggedResult = Expression.Parameter(logAndResultType);
            var logResult = Expression.Assign(loggedResult, Expression.New(logAndResultCtor, copyLog, result, err));

            var res = Expression.Block(new[] { result, err, loggedResult }, eval, logResult, clearLog, loggedResult);
            return res;
        }

        partial class Review
        {
            protected void INCONCLUSIVE() { Assert.Inconclusive(); }
        }

        partial class Reviewed : Review
        {
            private void OK() { }
            private void FAIL(string message = "") { Assert.Fail(message); }
        }

        private readonly Reviewed Verify = new Reviewed();
    }
}
