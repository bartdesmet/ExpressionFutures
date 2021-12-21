// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Dynamic.Utils;
using System.IO;
using System.Linq;
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
                Assert.IsNull(res.Declarations);
                Assert.AreSame(resource, res.Resource);
                Assert.AreSame(body, res.Body);
            }

            {
                var res = CSharpExpression.Using(variable, resource, body);

                Assert.AreEqual(CSharpExpressionType.Using, res.CSharpNodeType);
                Assert.AreEqual(typeof(void), res.Type);
                Assert.IsNull(res.Resource);
                Assert.AreEqual(1, res.Declarations.Count);
                Assert.AreSame(variable, res.Declarations[0].Variable);
                Assert.AreSame(resource, res.Declarations[0].Expression);
                Assert.AreSame(body, res.Body);
            }
        }

        [TestMethod]
        public void Using_Update_WithResource()
        {
            var resource = Expression.Default(typeof(IDisposable));
            var body = Expression.Empty();
            var res = CSharpExpression.Using(resource, body);

            Assert.AreSame(res, res.Update(res.Variables, res.Resource, res.Declarations, res.Body, res.AwaitInfo));

            var newResource = Expression.Default(typeof(IDisposable));
            var newBody = Expression.Empty();

            var upd1 = res.Update(res.Variables, newResource, res.Declarations, res.Body, res.AwaitInfo);
            var upd2 = res.Update(res.Variables, res.Resource, res.Declarations, newBody, res.AwaitInfo);

            Assert.AreSame(res.Variables, upd1.Variables);
            Assert.AreSame(newResource, upd1.Resource);
            Assert.AreSame(res.Declarations, upd1.Declarations);
            Assert.AreSame(res.Body, upd1.Body);

            Assert.AreSame(res.Variables, upd2.Variables);
            Assert.AreSame(res.Resource, upd2.Resource);
            Assert.AreSame(res.Declarations, upd2.Declarations);
            Assert.AreSame(newBody, upd2.Body);
        }

        [TestMethod]
        public void Using_Update_WithDeclarations()
        {
            var variable = Expression.Parameter(typeof(IDisposable));
            var resource = Expression.Default(typeof(IDisposable));
            var body = Expression.Empty();
            var res = CSharpExpression.Using(variable, resource, body);

            Assert.IsTrue(res.Variables.SequenceEqual(new[] { variable }));
            Assert.AreEqual(1, res.Declarations.Count);
            Assert.AreSame(variable, res.Declarations[0].Variable);
            Assert.AreSame(resource, res.Declarations[0].Expression);

            Assert.AreSame(res, res.Update(res.Variables, res.Resource, res.Declarations, res.Body, res.AwaitInfo));

            // REVIEW: The overlap between variables and declarations is a bit messy.

            var newVariable = Expression.Parameter(typeof(IDisposable));
            var newVariables = new[] { newVariable };
            var newResource = Expression.Default(typeof(IDisposable));
            var newDeclaration = res.Declarations[0].Update(newVariable, newResource);
            var newResources = new[] { newDeclaration };
            var newBody = Expression.Empty();

            var upd1 = res.Update(newVariables, res.Resource, newResources, res.Body, res.AwaitInfo);
            var upd2 = res.Update(res.Variables, res.Resource, res.Declarations, newBody, res.AwaitInfo);

            Assert.IsTrue(newVariables.SequenceEqual(upd1.Variables));
            Assert.IsTrue(newResources.SequenceEqual(upd1.Declarations));
            Assert.AreSame(res.Body, upd1.Body);

            Assert.AreSame(res.Variables, upd2.Variables);
            Assert.AreSame(res.Declarations, upd2.Declarations);
            Assert.AreSame(newBody, upd2.Body);
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

            foreach (var t in new[] { typeof(D2), typeof(D3), typeof(D4), typeof(D5), typeof(D6) })
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

        sealed class D6 : IDisposable
        {
            private readonly Action<string> _log;

            public D6(Action<string> log)
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
    }
}
