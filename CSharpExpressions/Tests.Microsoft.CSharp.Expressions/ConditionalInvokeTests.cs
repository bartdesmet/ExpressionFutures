// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    public class ConditionalInvokeTests
    {
        [Fact]
        public void ConditionalInvoke_Factory_ArgumentChecking()
        {
            var expr = Expression.Default(typeof(Func<int>));

            // null
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalInvoke(default(Expression)));
        }

        [Fact]
        public void ConditionalInvoke_Factory_Expression()
        {
            var function = Expression.Constant(new D((x, y) => x + y));
            var method = typeof(D).GetMethod("Invoke");

            var args = new[] { Expression.Constant(1) };

            foreach (var e in new[]
            {
                CSharpExpression.ConditionalInvoke(function, args),
                CSharpExpression.ConditionalInvoke(function, args.AsEnumerable()),
            })
            {
                Assert.Same(function, e.Expression);

                Assert.Single(e.Arguments);

                Assert.Equal(method.GetParameters()[0], e.Arguments[0].Parameter);

                Assert.Same(args[0], e.Arguments[0].Expression);
            }

            var tooLittle = new Expression[0];

            foreach (var f in new Func<ConditionalInvocationCSharpExpression>[]
            {
                () => CSharpExpression.ConditionalInvoke(function, tooLittle),
                () => CSharpExpression.ConditionalInvoke(function, tooLittle.AsEnumerable()),
            })
            {
                Assert.Throws<ArgumentException>(() => f());
            }

            var tooMany = new[] { Expression.Constant(1), Expression.Constant(2), Expression.Constant(3) };

            foreach (var f in new Func<ConditionalInvocationCSharpExpression>[]
            {
                () => CSharpExpression.ConditionalInvoke(function, tooMany),
                () => CSharpExpression.ConditionalInvoke(function, tooMany.AsEnumerable()),
            })
            {
                Assert.Throws<ArgumentException>(() => f());
            }
        }

        [Fact]
        public void ConditionalInvoke_Properties()
        {
            var invoke = MethodInfoOf((Func<int, int, int> f) => f.Invoke(default(int), default(int)));

            var parameters = invoke.GetParameters();

            var arg1Parameter = parameters[0];
            var arg2Parameter = parameters[1];

            var arg1Value = Expression.Constant(0);
            var arg2Value = Expression.Constant(1);

            var arg0 = CSharpExpression.Bind(arg1Parameter, arg1Value);
            var arg1 = CSharpExpression.Bind(arg2Parameter, arg2Value);

            var function = Expression.Constant(new Func<int, int, int>((x, y) => x + y));

            {
                var res = CSharpExpression.ConditionalInvoke(function, arg0, arg1);
                Assert.Equal(CSharpExpressionType.ConditionalAccess, res.CSharpNodeType);
                Assert.Same(function, res.Expression);
                Assert.Equal(typeof(int?), res.Type);
                Assert.True(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }

            {
                var res = CSharpExpression.ConditionalInvoke(function, new[] { arg0, arg1 }.AsEnumerable());
                Assert.Equal(CSharpExpressionType.ConditionalAccess, res.CSharpNodeType);
                Assert.Same(function, res.Expression);
                Assert.Equal(typeof(int?), res.Type);
                Assert.True(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }
        }

        [Fact]
        public void ConditionalInvoke_Update()
        {
            var invoke = MethodInfoOf((Func<int, int, int> f) => f.Invoke(default(int), default(int)));

            var parameters = invoke.GetParameters();

            var arg1Parameter = parameters[0];
            var arg2Parameter = parameters[1];

            var arg1Value = Expression.Constant(0);
            var arg2Value = Expression.Constant(1);

            var arg0 = CSharpExpression.Bind(arg1Parameter, arg1Value);
            var arg1 = CSharpExpression.Bind(arg2Parameter, arg2Value);

            var function = Expression.Constant(new Func<int, int, int>((x, y) => x + y));

            var res = CSharpExpression.ConditionalInvoke(function, arg0, arg1);

            var function1 = Expression.Constant(new Func<int, int, int>((x, y) => x + y));

            var upd1 = res.Update(function1, res.Arguments);
            Assert.NotSame(upd1, res);
            Assert.Same(res.Arguments, upd1.Arguments);
            Assert.Same(function1, upd1.Expression);

            var upd2 = res.Update(function, new[] { arg1, arg0 });
            Assert.NotSame(upd2, res);
            Assert.Same(res.Expression, upd2.Expression);
            Assert.True(upd2.Arguments.SequenceEqual(new[] { arg1, arg0 }));
        }

        [Fact]
        public void ConditionalInvoke_Compile()
        {
            var p1 = Expression.Parameter(typeof(Func<int>));
            var i1 = CSharpExpression.ConditionalInvoke(p1);
            var f1 = Expression.Lambda<Func<Func<int>, int?>>(i1, p1);
            var d1 = f1.Compile();

            Assert.Equal(42, d1(() => 42));
            Assert.Null(d1(null));

            var p2 = Expression.Parameter(typeof(Func<string>));
            var i2 = CSharpExpression.ConditionalInvoke(p2);
            var f2 = Expression.Lambda<Func<Func<string>, string>>(i2, p2);
            var d2 = f2.Compile();

            Assert.Equal("bar", d2(() => "bar"));
            Assert.Null(d2(null));
        }

        // TODO: tests to assert args are not evaluated if receiver is null
        // TODO: tests to assert receiver is only evaluated once

        delegate int D(int arg1, int arg2 = 42);
    }
}
