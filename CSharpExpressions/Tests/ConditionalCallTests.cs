// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class ConditionalCallTests
    {
        [TestMethod]
        public void ConditionalCall_Factory_ArgumentChecking()
        {
            var expr = Expression.Default(typeof(Bar));
            var other = Expression.Default(typeof(string));
            var methodName = "Foo";
            var methodInfo = typeof(Bar).GetMethod(methodName);

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalCall(default(Expression), methodInfo));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalCall(expr, default(MethodInfo)));

            // static
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalCall(expr, expr.Type.GetMethod("Qux")));

            // wrong declaring type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalCall(other, methodInfo));
        }

        [TestMethod]
        public void ConditionalCall_Factory_Expression()
        {
            var method = MethodInfoOf((C c) => c.F(default(int), default(int), default(int)));

            var obj = Expression.Constant(new C());
            var args = new[] { Expression.Constant(1), Expression.Constant(2) };

            foreach (var e in new[]
            {
                CSharpExpression.ConditionalCall(obj, method, args),
                CSharpExpression.ConditionalCall(obj, method, args.AsEnumerable()),
            })
            {
                Assert.AreSame(obj, e.Object);

                Assert.AreEqual(method, e.Method);

                Assert.AreEqual(2, e.Arguments.Count);

                Assert.AreEqual(method.GetParameters()[0], e.Arguments[0].Parameter);
                Assert.AreEqual(method.GetParameters()[1], e.Arguments[1].Parameter);

                Assert.AreSame(args[0], e.Arguments[0].Expression);
                Assert.AreSame(args[1], e.Arguments[1].Expression);
            }

            var tooLittle = args.Take(1);

            foreach (var f in new Func<ConditionalMethodCallCSharpExpression>[]
            {
                () => CSharpExpression.ConditionalCall(obj, method, tooLittle),
                () => CSharpExpression.ConditionalCall(obj, method, tooLittle.AsEnumerable()),
            })
            {
                AssertEx.Throws<ArgumentException>(() => f());
            }

            var tooMany = args.Concat(args).ToArray();

            foreach (var f in new Func<ConditionalMethodCallCSharpExpression>[]
            {
                () => CSharpExpression.ConditionalCall(obj, method, tooMany),
                () => CSharpExpression.ConditionalCall(obj, method, tooMany.AsEnumerable()),
            })
            {
                AssertEx.Throws<ArgumentException>(() => f());
            }
        }

        [TestMethod]
        public void ConditionalCall_Properties()
        {
            var baz = MethodInfoOf((Bar b) => b.Baz(default(int)));
            var parameter = baz.GetParameters()[0];

            var obj = Expression.Default(typeof(Bar));
            var value = Expression.Constant(0);

            var arg = CSharpExpression.Bind(parameter, value);

            foreach (var e in new[]
            {
                CSharpExpression.ConditionalCall(obj, baz, arg),
                CSharpExpression.ConditionalCall(obj, baz, new[] { arg }.AsEnumerable()),
            })
            {
                Assert.AreEqual(CSharpExpressionType.ConditionalCall, e.CSharpNodeType);
                Assert.AreSame(obj, e.Object);
                Assert.AreEqual(baz, e.Method);
                Assert.AreEqual(typeof(int?), e.Type);
                Assert.IsTrue(e.Arguments.SequenceEqual(new[] { arg }));
            }
        }

        [TestMethod]
        public void ConditionalCall_Update()
        {
            var baz = MethodInfoOf((Bar b) => b.Baz(default(int)));
            var parameter = baz.GetParameters()[0];

            var obj = Expression.Default(typeof(Bar));
            var value = Expression.Constant(0);

            var arg = CSharpExpression.Bind(parameter, value);

            var res = CSharpExpression.ConditionalCall(obj, baz, arg);

            Assert.AreSame(res, res.Update(res.Object, res.Arguments));

            var obj1 = Expression.Default(typeof(Bar));
            var upd1 = res.Update(obj1, res.Arguments);
            Assert.AreNotSame(upd1, res);
            Assert.AreSame(res.Arguments, upd1.Arguments);
            Assert.AreSame(obj1, upd1.Object);

            var upd2 = res.Update(obj, new[] { arg });
            Assert.AreNotSame(upd2, res);
            Assert.AreSame(res.Object, upd2.Object);
            Assert.IsTrue(upd2.Arguments.SequenceEqual(new[] { arg }));
        }

        [TestMethod]
        public void ConditionalCall_Compile_Ref()
        {
            var p = Expression.Parameter(typeof(Qux));
            var q = new Qux();

            var m1 = CSharpExpression.ConditionalCall(p, typeof(Qux).GetMethod("X"));
            var f1 = Expression.Lambda<Func<Qux, int?>>(m1, p);
            var d1 = f1.Compile();

            Assert.AreEqual(42, d1(q));
            Assert.IsNull(d1(null));

            var m2 = CSharpExpression.ConditionalCall(p, typeof(Qux).GetMethod("N"));
            var f2 = Expression.Lambda<Func<Qux, int?>>(m2, p);
            var d2 = f2.Compile();

            Assert.AreEqual(42, d2(q));
            Assert.IsNull(d2(null));

            var m3 = CSharpExpression.ConditionalCall(p, typeof(Qux).GetMethod("S"));
            var f3 = Expression.Lambda<Func<Qux, string>>(m3, p);
            var d3 = f3.Compile();

            Assert.AreEqual("bar", d3(q));
            Assert.IsNull(d3(null));

            var m4 = CSharpExpression.ConditionalCall(p, typeof(Qux).GetMethod("V"));
            var f4 = Expression.Lambda<Action<Qux>>(m4, p);
            var d4 = f4.Compile();

            d4(q); // does not throw
        }

        [TestMethod]
        public void ConditionalCall_Compile_Val()
        {
            var p = Expression.Parameter(typeof(Quz?));
            var q = new Quz();

            var m1 = CSharpExpression.ConditionalCall(p, typeof(Quz).GetMethod("X"));
            var f1 = Expression.Lambda<Func<Quz?, int?>>(m1, p);
            var d1 = f1.Compile();

            Assert.AreEqual(42, d1(q));
            Assert.IsNull(d1(null));

            var m2 = CSharpExpression.ConditionalCall(p, typeof(Quz).GetMethod("N"));
            var f2 = Expression.Lambda<Func<Quz?, int?>>(m2, p);
            var d2 = f2.Compile();

            Assert.AreEqual(42, d2(q));
            Assert.IsNull(d2(null));

            var m3 = CSharpExpression.ConditionalCall(p, typeof(Quz).GetMethod("S"));
            var f3 = Expression.Lambda<Func<Quz?, string>>(m3, p);
            var d3 = f3.Compile();

            Assert.AreEqual("bar", d3(q));
            Assert.IsNull(d3(null));
        }

        // TODO: tests to assert args are not evaluated if receiver is null
        // TODO: tests to assert receiver is only evaluated once

        [TestMethod]
        public void ConditionalCall_Visitor()
        {
            var expr = Expression.Default(typeof(Bar));
            var method = expr.Type.GetMethod("Foo");
            var res = CSharpExpression.ConditionalCall(expr, method);

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitConditionalMethodCall(ConditionalMethodCallCSharpExpression node)
            {
                Visited = true;

                return base.VisitConditionalMethodCall(node);
            }
        }

        class Bar
        {
            public int Foo() { return 0; }
            public int Baz(int x) { return 0; }
            public static int Qux() { return 0; }
        }

        class Qux
        {
            public int X() => 42;
            public int? N() => 42;
            public string S() => "bar";
            public void V() { }
        }

        struct Quz
        {
            public int X() => 42;
            public int? N() => 42;
            public string S() => "bar";
        }

        class C
        {
            public int F(int x, int y, int z = 42)
            {
                return x * y + z;
            }
        }
    }
}
