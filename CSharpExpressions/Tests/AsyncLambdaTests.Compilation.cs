// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Tests.ReflectionUtils;

namespace Tests
{
    partial class AsyncLambdaTests
    {
        [TestMethod]
        public void AsyncLambda_Compilation_Simple1()
        {
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Constant(42));
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.AreEqual(42, r);
        }

        [TestMethod]
        public void AsyncLambda_Compilation_Simple2()
        {
            var e = CSharpExpression.AsyncLambda<Func<Task>>(Expression.Constant(42));
            var f = e.Compile();
            var t = f();
            t.Wait();
        }

        [TestMethod]
        public void AsyncLambda_Compilation_Simple3()
        {
            var log = new List<string>();
            var a = (Expression<Action>)(() => log.Add("OK"));

            var e = CSharpExpression.AsyncLambda<Func<Task>>(a.Body);
            var f = e.Compile();
            var t = f();

            // Add happens on sync code path
            Assert.IsTrue(new[] { "OK" }.SequenceEqual(log));

            t.Wait();
        }

        [TestMethod]
        public void AsyncLambda_Compilation_Simple4()
        {
            var log = new List<string>();
            var a = (Expression<Action>)(() => log.Add("OK"));

            var e = CSharpExpression.AsyncLambda<Action>(a.Body);
            var f = e.Compile();
            f();

            // Add happens on sync code path
            Assert.IsTrue(new[] { "OK" }.SequenceEqual(log));
        }

        [TestMethod]
        public void AsyncLambda_Compilation_Simple5()
        {
            var v = Expression.Constant(Task.FromResult(42));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(CSharpExpression.Await(v));
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.AreEqual(42, r);
        }

        [TestMethod]
        public void AsyncLambda_Compilation_Spilling()
        {
            var v1 = Expression.Constant(Task.FromResult(1));
            var v2 = Expression.Constant(Task.FromResult(2));
            var add = Expression.Add(CSharpExpression.Await(v1), CSharpExpression.Await(v2));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(add);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.AreEqual(3, r);
        }

        [TestMethod]
        public void AsyncLambda_Compilation_Hoisting()
        {
            var fromResultMethod = MethodInfoOf(() => Task.FromResult(default(int)));
            var i = Expression.Parameter(typeof(int));
            var res = Expression.Parameter(typeof(int));
            var brk = Expression.Label();
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.Block(
                    new[] { i, res },
                    Expression.Assign(i, Expression.Constant(0)),
                    Expression.Loop(
                        Expression.Block(
                            Expression.IfThen(
                                Expression.Equal(i, Expression.Constant(10)),
                                Expression.Break(brk)
                            ),
                            Expression.AddAssign(
                                res,
                                CSharpExpression.Await(Expression.Call(fromResultMethod, i))
                            ),
                            Expression.PostIncrementAssign(i)
                        ), brk
                    ),
                    res
                )
            );
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.AreEqual(Enumerable.Range(0, 10).Sum(), r);
        }
    }
}
