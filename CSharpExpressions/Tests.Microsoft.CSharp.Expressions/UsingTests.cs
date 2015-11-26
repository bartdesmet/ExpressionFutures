// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq.Expressions;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class UsingTests
    {
        [TestMethod]
        public void Using_Factory_ArgumentChecking()
        {
            var variable = Expression.Parameter(typeof(FileStream));
            var resource = Expression.Default(typeof(IDisposable));
            var body = Expression.Empty();

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Using(default(Expression), body));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Using(resource, default(Expression)));

            // not IDisposable
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Using(Expression.Default(typeof(int)), body));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Using(Expression.Default(typeof(string)), body));

            // not assignable
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Using(variable, resource, body));
        }

        [TestMethod]
        public void Using_Properties()
        {
            var variable = Expression.Parameter(typeof(IDisposable));
            var resource = Expression.Default(typeof(IDisposable));
            var body = Expression.Empty();

            {
                var res = CSharpExpression.Using(resource, body);

                Assert.AreEqual(CSharpExpressionType.Using, res.CSharpNodeType);
                Assert.AreEqual(typeof(void), res.Type);
                Assert.IsNull(res.Variable);
                Assert.AreSame(resource, res.Resource);
                Assert.AreSame(body, res.Body);
            }

            {
                var res = CSharpExpression.Using(variable, resource, body);

                Assert.AreEqual(CSharpExpressionType.Using, res.CSharpNodeType);
                Assert.AreEqual(typeof(void), res.Type);
                Assert.AreSame(variable, res.Variable);
                Assert.AreSame(resource, res.Resource);
                Assert.AreSame(body, res.Body);
            }
        }

        [TestMethod]
        public void Using_Update()
        {
            var variable = Expression.Parameter(typeof(IDisposable));
            var resource = Expression.Default(typeof(IDisposable));
            var body = Expression.Empty();
            var res = CSharpExpression.Using(variable, resource, body);

            Assert.AreSame(res, res.Update(res.Variable, res.Resource, res.Body));

            var newVariable = Expression.Parameter(typeof(IDisposable));
            var newResource = Expression.Default(typeof(IDisposable));
            var newBody = Expression.Empty();

            var upd1 = res.Update(newVariable, res.Resource, res.Body);
            var upd2 = res.Update(res.Variable, newResource, res.Body);
            var upd3 = res.Update(res.Variable, res.Resource, newBody);

            Assert.AreSame(newVariable, upd1.Variable);
            Assert.AreSame(res.Resource, upd1.Resource);
            Assert.AreSame(res.Body, upd1.Body);

            Assert.AreSame(res.Variable, upd2.Variable);
            Assert.AreSame(newResource, upd2.Resource);
            Assert.AreSame(res.Body, upd2.Body);

            Assert.AreSame(res.Variable, upd3.Variable);
            Assert.AreSame(res.Resource, upd3.Resource);
            Assert.AreSame(newBody, upd3.Body);
        }

        [TestMethod]
        public void Using_Compile()
        {
            AssertCompile((log, append) =>
                CSharpExpression.Using(
                    Expression.Default(typeof(IDisposable)),
                    log("B")
                ),
                new LogAndResult<object> { Log = { "B" } }
            );

            AssertCompile((log, append) =>
                CSharpExpression.Using(
                    Expression.Default(typeof(D1?)),
                    log("B")
                ),
                new LogAndResult<object> { Log = { "B" } }
            );

            foreach (var t in new[] { typeof(D2), typeof(D3), typeof(D4), typeof(D5) })
            {
                AssertCompile((log, append) =>
                    CSharpExpression.Using(
                        Expression.New(t.GetConstructor(new[] { typeof(Action<string>) }), append),
                        log("B")
                    ),
                    new LogAndResult<object> { Log = { "B", "D" } }
                );

                if (t.IsValueType)
                {
                    AssertCompile((log, append) =>
                        CSharpExpression.Using(
                            Expression.Convert(Expression.New(t.GetConstructor(new[] { typeof(Action<string>) }), append), typeof(Nullable<>).MakeGenericType(t)),
                            log("B")
                        ),
                        new LogAndResult<object> { Log = { "B", "D" } }
                    );
                }

                var p1 = Expression.Parameter(typeof(IDisposable));

                AssertCompile((log, append) =>
                    CSharpExpression.Using(
                        p1,
                        Expression.Convert(Expression.New(t.GetConstructor(new[] { typeof(Action<string>) }), append), p1.Type),
                        log("B")
                    ),
                    new LogAndResult<object> { Log = { "B", "D" } }
                );

                var p2 = Expression.Parameter(t);

                AssertCompile((log, append) =>
                    CSharpExpression.Using(
                        p2,
                        Expression.New(t.GetConstructor(new[] { typeof(Action<string>) }), append),
                        Expression.Call(p2, t.GetMethod("Do"))
                    ),
                    new LogAndResult<object> { Log = { "B", "D" } }
                );

                if (t.IsValueType)
                {
                    var p3 = Expression.Parameter(typeof(Nullable<>).MakeGenericType(t));

                    AssertCompile((log, append) =>
                        CSharpExpression.Using(
                            p3,
                            Expression.Convert(Expression.New(t.GetConstructor(new[] { typeof(Action<string>) }), append), p3.Type),
                            Expression.Call(Expression.Property(p3, "Value"), t.GetMethod("Do"))
                        ),
                        new LogAndResult<object> { Log = { "B", "D" } }
                    );

                    AssertCompile((log, append) =>
                        CSharpExpression.Using(
                            p3,
                            Expression.Convert(Expression.New(t.GetConstructor(new[] { typeof(Action<string>) }), append), p3.Type),
                            Expression.Block(
                                Expression.Call(Expression.Property(p3, "Value"), t.GetMethod("Do")),
                                Expression.Assign(p3, Expression.Default(typeof(Nullable<>).MakeGenericType(t)))
                            )
                        ),
                        new LogAndResult<object> { Log = { "B" } }
                    );
                }
                else
                {
                    var p3 = Expression.Parameter(t);

                    AssertCompile((log, append) =>
                        CSharpExpression.Using(
                            p3,
                            Expression.Convert(Expression.New(t.GetConstructor(new[] { typeof(Action<string>) }), append), p3.Type),
                            Expression.Call(p3, t.GetMethod("Do"))
                        ),
                        new LogAndResult<object> { Log = { "B", "D" } }
                    );

                    AssertCompile((log, append) =>
                        CSharpExpression.Using(
                            p3,
                            Expression.Convert(Expression.New(t.GetConstructor(new[] { typeof(Action<string>) }), append), p3.Type),
                            Expression.Block(
                                Expression.Call(p3, t.GetMethod("Do")),
                                Expression.Assign(p3, Expression.Default(t))
                            )
                        ),
                        new LogAndResult<object> { Log = { "B" } }
                    );
                }
            }
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        public void Using_Visitor()
        {
            var resource = Expression.Default(typeof(IDisposable));
            var body = Expression.Empty();
            var res = CSharpExpression.Using(resource, body);

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitUsing(UsingCSharpStatement node)
            {
                Visited = true;

                return base.VisitUsing(node);
            }
        }

        struct D1 : IDisposable
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        struct D2 : IDisposable
        {
            private readonly Action<string> _log;

            public D2(Action<string> log)
            {
                _log = log;
            }

            public void Do()
            {
                _log("B");
            }

            public void Dispose()
            {
                _log("D");
            }
        }

        struct D3 : IDisposable
        {
            private readonly Action<string> _log;

            public D3(Action<string> log)
            {
                _log = log;
            }

            public void Do()
            {
                _log("B");
            }

            void IDisposable.Dispose()
            {
                _log("D");
            }
        }

        class D4 : IDisposable
        {
            private readonly Action<string> _log;

            public D4(Action<string> log)
            {
                _log = log;
            }

            public void Do()
            {
                _log("B");
            }

            public void Dispose()
            {
                _log("D");
            }
        }

        class D5 : IDisposable
        {
            private readonly Action<string> _log;

            public D5(Action<string> log)
            {
                _log = log;
            }

            public void Do()
            {
                _log("B");
            }

            void IDisposable.Dispose()
            {
                _log("D");
            }
        }
    }
}
