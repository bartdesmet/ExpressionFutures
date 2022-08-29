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
    public class InvocationTests
    {
        [Fact]
        public void Invoke_Factory_ArgumentChecking()
        {
            // NB: A lot of checks are performed by LINQ helpers, so we omit tests for those cases.

            var invoke = MethodInfoOf((Func<int, int, int> f) => f.Invoke(default(int), default(int)));

            var parameters = invoke.GetParameters();

            var arg1Parameter = parameters[0];
            var arg2Parameter = parameters[1];

            var arg1 = Expression.Constant(0);
            var arg2 = Expression.Constant(1);

            var argArg1 = CSharpExpression.Bind(arg1Parameter, arg1);
            var argArg2 = CSharpExpression.Bind(arg2Parameter, arg2);

            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));

            var valueParameter = cout.GetParameters()[0];

            var value = Expression.Constant(42);
            var argValue = CSharpExpression.Bind(valueParameter, value);

            var function = Expression.Constant(new Func<int, int, int>((x, y) => x + y));

            // duplicate
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Invoke(function, argArg1, argArg1));

            // unbound
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Invoke(function, argArg1));

            // wrong member
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Invoke(function, argValue));

            // null
            var bindings = new[] { argArg1, argArg2 };
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Invoke(default(Expression), bindings));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Invoke(default(Expression), bindings.AsEnumerable()));
        }

        [Fact]
        public void Invoke_Factory_Expression()
        {
            var function = Expression.Constant(new D((x, y) => x + y));
            var method = typeof(D).GetMethod("Invoke");

            var args = new[] { Expression.Constant(1) };

            foreach (var e in new[]
            {
                CSharpExpression.Invoke(function, args),
                CSharpExpression.Invoke(function, args.AsEnumerable()),
            })
            {
                Assert.Same(function, e.Expression);

                Assert.Single(e.Arguments);

                Assert.Equal(method.GetParameters()[0], e.Arguments[0].Parameter);

                Assert.Same(args[0], e.Arguments[0].Expression);
            }

            var tooLittle = new Expression[0];

            foreach (var f in new Func<InvocationCSharpExpression>[]
            {
                () => CSharpExpression.Invoke(function, tooLittle),
                () => CSharpExpression.Invoke(function, tooLittle.AsEnumerable()),
            })
            {
                AssertEx.Throws<ArgumentException>(() => f());
            }

            var tooMany = new[] { Expression.Constant(1), Expression.Constant(2), Expression.Constant(3) };

            foreach (var f in new Func<InvocationCSharpExpression>[]
            {
                () => CSharpExpression.Invoke(function, tooMany),
                () => CSharpExpression.Invoke(function, tooMany.AsEnumerable()),
            })
            {
                AssertEx.Throws<ArgumentException>(() => f());
            }
        }

        [Fact]
        public void Invoke_Properties()
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
                var res = CSharpExpression.Invoke(function, arg0, arg1);

                Assert.Equal(CSharpExpressionType.Invoke, res.CSharpNodeType);
                Assert.Same(function, res.Expression);
                Assert.Equal(typeof(int), res.Type);
                Assert.True(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }

            {
                var res = CSharpExpression.Invoke(function, new[] { arg0, arg1 }.AsEnumerable());

                Assert.Equal(CSharpExpressionType.Invoke, res.CSharpNodeType);
                Assert.Same(function, res.Expression);
                Assert.Equal(typeof(int), res.Type);
                Assert.True(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }
        }

        [Fact]
        public void Invoke_Update()
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

            var res = CSharpExpression.Invoke(function, arg0, arg1);

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
        public void Invoke_Compile1()
        {
            var invoke = MethodInfoOf((Func<int, int, int> f) => f.Invoke(default(int), default(int)));

            var parameters = invoke.GetParameters();

            var parameterArg1 = parameters[0];
            var parameterArg2 = parameters[1];

            var valueArg1 = Expression.Constant(1);
            var valueArg2 = Expression.Constant(2);

            var function = Expression.Constant(new Func<int, int, int>((x, y) => x + y));

            AssertCompile<int>(log =>
                CSharpExpression.Invoke(log(function, "F"),
                    CSharpExpression.Bind(parameterArg1, log(valueArg1, "1")),
                    CSharpExpression.Bind(parameterArg2, log(valueArg2, "2"))
                ),
                new LogAndResult<int> { Value = 1 + 2, Log = { "F", "1", "2" } }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Invoke(log(function, "F"),
                    CSharpExpression.Bind(parameterArg2, log(valueArg2, "2")),
                    CSharpExpression.Bind(parameterArg1, log(valueArg1, "1"))
                ),
                new LogAndResult<int> { Value = 1 + 2, Log = { "F", "2", "1" } }
            );
        }

        [Fact]
        public void Invoke_Compile2()
        {
            var invoke = MethodInfoOf((D f) => f.Invoke(default(int), default(int)));

            var parameters = invoke.GetParameters();

            var parameterArg1 = parameters[0];
            var parameterArg2 = parameters[1];

            var valueArg1 = Expression.Constant(1);
            var valueArg2 = Expression.Constant(2);

            var function = Expression.Constant(new D((x, y) => x + y));

            AssertCompile<int>(log =>
                CSharpExpression.Invoke(log(function, "F"),
                    CSharpExpression.Bind(parameterArg1, log(valueArg1, "1")),
                    CSharpExpression.Bind(parameterArg2, log(valueArg2, "2"))
                ),
                new LogAndResult<int> { Value = 1 + 2, Log = { "F", "1", "2" } }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Invoke(log(function, "F"),
                    CSharpExpression.Bind(parameterArg1, log(valueArg1, "1"))
                ),
                new LogAndResult<int> { Value = 1 + 42, Log = { "F", "1" } }
            );
        }

        private void AssertCompile<T>(Func<Func<Expression, string, Expression>, Expression> createExpression, LogAndResult<T> expected)
        {
            var res = WithLogValue<T>(createExpression).Compile()();
            Assert.Equal(expected, res);
        }

        [Fact]
        public void Invoke_Visitor()
        {
            var invoke = MethodInfoOf((Func<int, int, int> f) => f.Invoke(default(int), default(int)));

            var parameters = invoke.GetParameters();

            var parameterArg1 = parameters[0];
            var parameterArg2 = parameters[1];

            var valueArg1 = Expression.Constant(1);
            var valueArg2 = Expression.Constant(2);

            var function = Expression.Constant(new Func<int, int, int>((x, y) => x + y));

            var res = CSharpExpression.Invoke(function, CSharpExpression.Bind(parameterArg1, valueArg1), CSharpExpression.Bind(parameterArg2, valueArg2));

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitInvocation(InvocationCSharpExpression node)
            {
                Visited = true;

                return base.VisitInvocation(node);
            }
        }

        delegate int D(int arg1, int arg2 = 42);
    }
}
