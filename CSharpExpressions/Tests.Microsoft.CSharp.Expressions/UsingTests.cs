﻿// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Dynamic.Utils;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using static Tests.TestHelpers;

namespace Tests
{
    public class UsingTests
    {
        [Fact]
        public void Using_Factory_ArgumentChecking()
        {
            var variable = Expression.Parameter(typeof(FileStream));
            var resource = Expression.Default(typeof(IDisposable));
            var body = Expression.Empty();

            // null
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Using(default(Expression), body));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Using(resource, default(Expression)));

            // not IDisposable
            Assert.Throws<ArgumentException>(() => CSharpExpression.Using(Expression.Default(typeof(int)), body));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Using(Expression.Default(typeof(string)), body));

            // not assignable
            Assert.Throws<ArgumentException>(() => CSharpExpression.Using(variable, resource, body));
        }

        [Fact]
        public void Using_Properties()
        {
            var variable = Expression.Parameter(typeof(IDisposable));
            var resource = Expression.Default(typeof(IDisposable));
            var body = Expression.Empty();

            {
                var res = CSharpExpression.Using(resource, body);

                Assert.Equal(CSharpExpressionType.Using, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.Null(res.Declarations);
                Assert.Same(resource, res.Resource);
                Assert.Same(body, res.Body);
            }

            {
                var res = CSharpExpression.Using(variable, resource, body);

                Assert.Equal(CSharpExpressionType.Using, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.Null(res.Resource);
                Assert.Single(res.Declarations);
                Assert.Same(variable, res.Declarations[0].Variable);
                Assert.Same(resource, res.Declarations[0].Expression);
                Assert.Same(body, res.Body);
            }
        }

        [Fact]
        public void Using_Update_WithResource()
        {
            var resource = Expression.Default(typeof(IDisposable));
            var body = Expression.Empty();
            var res = CSharpExpression.Using(resource, body);

            Assert.Same(res, res.Update(res.Variables, res.Resource, res.Declarations, res.Body, res.AwaitInfo, res.PatternDispose));

            var newResource = Expression.Default(typeof(IDisposable));
            var newBody = Expression.Empty();

            var upd1 = res.Update(res.Variables, newResource, res.Declarations, res.Body, res.AwaitInfo, res.PatternDispose);
            var upd2 = res.Update(res.Variables, res.Resource, res.Declarations, newBody, res.AwaitInfo, res.PatternDispose);

            Assert.Same(res.Variables, upd1.Variables);
            Assert.Same(newResource, upd1.Resource);
            Assert.Same(res.Declarations, upd1.Declarations);
            Assert.Same(res.Body, upd1.Body);

            Assert.Same(res.Variables, upd2.Variables);
            Assert.Same(res.Resource, upd2.Resource);
            Assert.Same(res.Declarations, upd2.Declarations);
            Assert.Same(newBody, upd2.Body);
        }

        [Fact]
        public void Using_Update_WithDeclarations()
        {
            var variable = Expression.Parameter(typeof(IDisposable));
            var resource = Expression.Default(typeof(IDisposable));
            var body = Expression.Empty();
            var res = CSharpExpression.Using(variable, resource, body);

            Assert.True(res.Variables.SequenceEqual(new[] { variable }));
            Assert.Single(res.Declarations);
            Assert.Same(variable, res.Declarations[0].Variable);
            Assert.Same(resource, res.Declarations[0].Expression);

            Assert.Same(res, res.Update(res.Variables, res.Resource, res.Declarations, res.Body, res.AwaitInfo, res.PatternDispose));

            // REVIEW: The overlap between variables and declarations is a bit messy.

            var newVariable = Expression.Parameter(typeof(IDisposable));
            var newVariables = new[] { newVariable };
            var newResource = Expression.Default(typeof(IDisposable));
            var newDeclaration = res.Declarations[0].Update(newVariable, newResource);
            var newResources = new[] { newDeclaration };
            var newBody = Expression.Empty();

            var upd1 = res.Update(newVariables, res.Resource, newResources, res.Body, res.AwaitInfo, res.PatternDispose);
            var upd2 = res.Update(res.Variables, res.Resource, res.Declarations, newBody, res.AwaitInfo, res.PatternDispose);

            Assert.True(newVariables.SequenceEqual(upd1.Variables));
            Assert.True(newResources.SequenceEqual(upd1.Declarations));
            Assert.Same(res.Body, upd1.Body);

            Assert.Same(res.Variables, upd2.Variables);
            Assert.Same(res.Declarations, upd2.Declarations);
            Assert.Same(newBody, upd2.Body);
        }

        [Fact]
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
            Assert.Equal(expected, res);
        }

        [Fact]
        public void Using_Visitor()
        {
            var resource = Expression.Default(typeof(IDisposable));
            var body = Expression.Empty();
            var res = CSharpExpression.Using(resource, body);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
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
