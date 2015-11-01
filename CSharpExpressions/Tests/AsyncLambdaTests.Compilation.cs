// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static Tests.ReflectionUtils;

namespace Tests
{
    partial class AsyncLambdaTests
    {
        [TestMethod]
        public void AsyncLambda_Compilation_NotInFilter()
        {
            var p = Expression.Parameter(typeof(Exception));

            var expr = Expression.TryCatch(
                Expression.Empty(),
                Expression.Catch(
                    p,
                    Expression.Empty(),
                    CSharpExpression.Await(Expression.Constant(Task.FromResult(true)))
                )
            );

            var e = CSharpExpression.AsyncLambda<Func<Task>>(expr);
            AssertEx.Throws<InvalidOperationException>(() => e.Compile());
        }

        [TestMethod]
        public void AsyncLambda_Compilation_Simple0()
        {
            var p = Expression.Parameter(typeof(int));

            var e1 = CSharpExpression.AsyncLambda<Func<int, Task<int>>>(p, p);
            var e2 = CSharpExpression.AsyncLambda(typeof(Func<int, Task<int>>), p, p);
            var e3 = CSharpExpression.AsyncLambda<Func<int, Task<int>>>(p, p);

            foreach (AsyncCSharpExpression<Func<int, Task<int>>> e in new[] { e1, e2, e3 })
            {
                Assert.AreEqual(42, e.Compile()(42).Result);
            }
        }

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

        [TestMethod]
        public void AsyncLambda_Compilation_HoistingParameters()
        {
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var x = Expression.Parameter(typeof(int));
            var e = CSharpExpression.AsyncLambda<Func<int, Task<int>>>(
                Expression.Block(
                    CSharpExpression.Await(yield),
                    x
                ),
                x
            );
            var f = e.Compile();
            var t = f(42);
            var r = t.Result;
            Assert.AreEqual(42, r);
        }

        [TestMethod]
        public void AsyncLambda_Compilation_ResumeInTry1()
        {
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.Block(
                    Expression.TryCatch(
                        Expression.Block(
                            CSharpExpression.Await(yield),
                            Expression.Constant(42)
                        ),
                        Expression.Catch(Expression.Parameter(typeof(Exception)),
                            Expression.Constant(-1)
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.AreEqual(42, r);
        }

        [TestMethod]
        public void AsyncLambda_Compilation_ResumeInTry2()
        {
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.Block(
                    Expression.TryCatch(
                        Expression.Block(
                            CSharpExpression.Await(yield),
                            Expression.Divide(Expression.Constant(1), Expression.Constant(0))
                        ),
                        Expression.Catch(Expression.Parameter(typeof(DivideByZeroException)),
                            Expression.Constant(-1)
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void AsyncLambda_Compilation_ResumeInTry3()
        {
            var p = Expression.Parameter(typeof(D));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<D, Task<int>>>(
                Expression.Block(
                    Expression.TryFinally(
                        Expression.Block(
                            CSharpExpression.Await(yield),
                            Expression.Call(p, typeof(D).GetMethod("Do"))
                        ),
                        Expression.Call(p, typeof(D).GetMethod("Dispose"))
                    )
                ),
                p
            );
            var d = new D();
            var f = e.Compile();
            var t = f(d);
            var r = t.Result;
            Assert.AreEqual(42, r);
            Assert.IsTrue(d.IsDisposed);
        }

        [TestMethod]
        public void AsyncLambda_Compilation_ReducibleNodes()
        {
            var fromResultMethod = MethodInfoOf(() => Task.FromResult(default(int)));
            var i = Expression.Parameter(typeof(int));
            var res = Expression.Parameter(typeof(int));
            var brk = Expression.Label();
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.Block(
                    new[] { i, res },
                    Expression.Assign(i, Expression.Constant(0)),
                    CSharpExpression.While(Expression.LessThan(i, Expression.Constant(10)),
                        Expression.Block(
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

        class D : IDisposable
        {
            public bool IsDisposed;

            public int Do()
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("this");

                return 42;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
