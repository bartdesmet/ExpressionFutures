﻿// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    public class ConditionalCallTests
    {
        [Fact]
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

        [Fact]
        public void ConditionalCall_Factory_Extension()
        {
            var s = Expression.Constant("bar");
            var x = Expression.Constant(42);

            var reverse = MethodInfoOf(() => Ext.Reverse(default(string)));

            // needs instance
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalCall(null, reverse, new Expression[0]));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalCall(null, reverse, new[] { s }));

            // duplicate binding
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalCall(s, reverse, new[] { s }));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalCall(s, reverse, CSharpExpression.Bind(reverse.GetParameters()[0], s)));

            // wrong type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalCall(x, reverse));
        }

        [Fact]
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
                Assert.Same(obj, e.Object);

                Assert.Equal(method, e.Method);

                Assert.Equal(2, e.Arguments.Count);

                Assert.Equal(method.GetParameters()[0], e.Arguments[0].Parameter);
                Assert.Equal(method.GetParameters()[1], e.Arguments[1].Parameter);

                Assert.Same(args[0], e.Arguments[0].Expression);
                Assert.Same(args[1], e.Arguments[1].Expression);
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

        [Fact]
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
                Assert.Equal(CSharpExpressionType.ConditionalAccess, e.CSharpNodeType);
                Assert.Same(obj, e.Object);
                Assert.Equal(baz, e.Method);
                Assert.Equal(typeof(int?), e.Type);
                Assert.True(e.Arguments.SequenceEqual(new[] { arg }));
            }
        }

        [Fact]
        public void ConditionalCall_Update()
        {
            var baz = MethodInfoOf((Bar b) => b.Baz(default(int)));
            var parameter = baz.GetParameters()[0];

            var obj = Expression.Default(typeof(Bar));
            var value = Expression.Constant(0);

            var arg = CSharpExpression.Bind(parameter, value);

            var res = CSharpExpression.ConditionalCall(obj, baz, arg);

            Assert.Same(res, res.Update(res.Object, res.Arguments));

            var obj1 = Expression.Default(typeof(Bar));
            var upd1 = res.Update(obj1, res.Arguments);
            Assert.NotSame(upd1, res);
            Assert.Same(res.Arguments, upd1.Arguments);
            Assert.Same(obj1, upd1.Object);

            var newValue = Expression.Constant(1);
            var newArg = CSharpExpression.Bind(parameter, newValue);

            var upd2 = res.Update(obj, new[] { newArg });
            Assert.NotSame(upd2, res);
            Assert.Same(res.Object, upd2.Object);
            Assert.True(upd2.Arguments.SequenceEqual(new[] { newArg }));
        }

        [Fact]
        public void ConditionalCall_Compile_Ref()
        {
            var p = Expression.Parameter(typeof(Qux));
            var q = new Qux();

            var m1 = CSharpExpression.ConditionalCall(p, typeof(Qux).GetMethod("X"));
            var f1 = Expression.Lambda<Func<Qux, int?>>(m1, p);
            var d1 = f1.Compile();

            Assert.Equal(42, d1(q));
            Assert.Null(d1(null));

            var m2 = CSharpExpression.ConditionalCall(p, typeof(Qux).GetMethod("N"));
            var f2 = Expression.Lambda<Func<Qux, int?>>(m2, p);
            var d2 = f2.Compile();

            Assert.Equal(42, d2(q));
            Assert.Null(d2(null));

            var m3 = CSharpExpression.ConditionalCall(p, typeof(Qux).GetMethod("S"));
            var f3 = Expression.Lambda<Func<Qux, string>>(m3, p);
            var d3 = f3.Compile();

            Assert.Equal("bar", d3(q));
            Assert.Null(d3(null));

            var m4 = CSharpExpression.ConditionalCall(p, typeof(Qux).GetMethod("V"));
            var f4 = Expression.Lambda<Action<Qux>>(m4, p);
            var d4 = f4.Compile();

            d4(q); // does not throw
        }

        [Fact]
        public void ConditionalCall_Compile_Val()
        {
            var p = Expression.Parameter(typeof(Quz?));
            var q = new Quz();

            var m1 = CSharpExpression.ConditionalCall(p, typeof(Quz).GetMethod("X"));
            var f1 = Expression.Lambda<Func<Quz?, int?>>(m1, p);
            var d1 = f1.Compile();

            Assert.Equal(42, d1(q));
            Assert.Null(d1(null));

            var m2 = CSharpExpression.ConditionalCall(p, typeof(Quz).GetMethod("N"));
            var f2 = Expression.Lambda<Func<Quz?, int?>>(m2, p);
            var d2 = f2.Compile();

            Assert.Equal(42, d2(q));
            Assert.Null(d2(null));

            var m3 = CSharpExpression.ConditionalCall(p, typeof(Quz).GetMethod("S"));
            var f3 = Expression.Lambda<Func<Quz?, string>>(m3, p);
            var d3 = f3.Compile();

            Assert.Equal("bar", d3(q));
            Assert.Null(d3(null));
        }

        [Fact]
        public void ConditionalCall_Compile_Extension()
        {
            var reverse = MethodInfoOf(() => Ext.Reverse(default(string)));
            var reverseSubstring = MethodInfoOf(() => Ext.ReverseSubstring(default(string), default(int), default(int)));

            var p = Expression.Parameter(typeof(string));
            var q = "foobar";

            var m1 = CSharpExpression.ConditionalCall(p, reverse);
            var f1 = Expression.Lambda<Func<string, string>>(m1, p);
            var d1 = f1.Compile();

            Assert.Equal("raboof", d1(q));
            Assert.Null(d1(null));

            var m2 = CSharpExpression.ConditionalCall(p, reverseSubstring, new[] { Expression.Constant(2), Expression.Constant(3) });
            var f2 = Expression.Lambda<Func<string, string>>(m2, p);
            var d2 = f2.Compile();

            Assert.Equal("abo", d2(q));
            Assert.Null(d2(null));

            var ps = reverseSubstring.GetParameters();

            var m3 = CSharpExpression.ConditionalCall(p, reverseSubstring, CSharpExpression.Bind(ps[2], Expression.Constant(3)), CSharpExpression.Bind(ps[1], Expression.Constant(2)));
            var f3 = Expression.Lambda<Func<string, string>>(m3, p);
            var d3 = f3.Compile();

            Assert.Equal("abo", d3(q));
            Assert.Null(d3(null));
        }

        // TODO: tests to assert args are not evaluated if receiver is null
        // TODO: tests to assert receiver is only evaluated once

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

    static class Ext
    {
        public static string Reverse(this string s)
        {
            return new string(s.ToCharArray().Reverse().ToArray());
        }

        public static string ReverseSubstring(this string s, int start, int length)
        {
            return new string(s.Substring(start, length).ToCharArray().Reverse().ToArray());
        }
    }
}
