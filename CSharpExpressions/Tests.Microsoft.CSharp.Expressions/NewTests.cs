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
    public class NewTests
    {
        [Fact]
        public void New_Factory_ArgumentChecking()
        {
            // NB: A lot of checks are performed by LINQ helpers, so we omit tests for those cases.

            var ctor = ConstructorInfoOf(() => new TimeSpan(default(int), default(int), default(int)));

            var parameters = ctor.GetParameters();

            var hoursParameter = parameters[0];
            var minutesParameter = parameters[1];
            var secondsParameter = parameters[2];

            var hours = Expression.Constant(1);
            var minutes = Expression.Constant(2);
            var seconds = Expression.Constant(3);

            var argHours = CSharpExpression.Bind(hoursParameter, hours);
            var argMinutes = CSharpExpression.Bind(minutesParameter, minutes);
            var argSeconds = CSharpExpression.Bind(secondsParameter, seconds);

            var ticks = ConstructorInfoOf(() => new TimeSpan(default(long)));

            var ticksParameter = ticks.GetParameters()[0];

            var value = Expression.Constant(42L);
            var argTicks = CSharpExpression.Bind(ticksParameter, value);

            // duplicate
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.New(ctor, argHours, argHours));

            // unbound
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.New(ctor, argHours));

            // wrong member
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.New(ctor, argTicks));

            // null
            var bindings = new[] { argHours, argMinutes };
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.New(default(ConstructorInfo), bindings));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.New(default(ConstructorInfo), bindings.AsEnumerable()));

            // static ctor
            var cctor = typeof(S).GetConstructors(BindingFlags.Static | BindingFlags.NonPublic).Single(c => c.IsStatic);
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.New(cctor));
        }

        [Fact]
        public void New_Factory_Expression()
        {
            var constructor = ConstructorInfoOf(() => new C(default(int), default(int), default(int)));

            var args = new[] { Expression.Constant(1), Expression.Constant(2) };

            foreach (var e in new[]
            {
                CSharpExpression.New(constructor, args),
                CSharpExpression.New(constructor, args.AsEnumerable()),
            })
            {
                Assert.Equal(constructor, e.Constructor);

                Assert.Equal(2, e.Arguments.Count);

                Assert.Equal(constructor.GetParameters()[0], e.Arguments[0].Parameter);
                Assert.Equal(constructor.GetParameters()[1], e.Arguments[1].Parameter);

                Assert.Same(args[0], e.Arguments[0].Expression);
                Assert.Same(args[1], e.Arguments[1].Expression);
            }

            var tooLittle = args.Take(1);

            foreach (var f in new Func<NewCSharpExpression>[]
            {
                () => CSharpExpression.New(constructor, tooLittle),
                () => CSharpExpression.New(constructor, tooLittle.AsEnumerable()),
            })
            {
                AssertEx.Throws<ArgumentException>(() => f());
            }

            var tooMany = args.Concat(args).ToArray();

            foreach (var f in new Func<NewCSharpExpression>[]
            {
                () => CSharpExpression.New(constructor, tooMany),
                () => CSharpExpression.New(constructor, tooMany.AsEnumerable()),
            })
            {
                AssertEx.Throws<ArgumentException>(() => f());
            }
        }

        [Fact]
        public void New_Properties()
        {
            var ctor = ConstructorInfoOf(() => new TimeSpan(default(int), default(int), default(int)));

            var parameters = ctor.GetParameters();

            var hoursParameter = parameters[0];
            var minutesParameter = parameters[1];
            var secondsParameter = parameters[2];

            var hours = Expression.Constant(1);
            var minutes = Expression.Constant(2);
            var seconds = Expression.Constant(3);

            var arg0 = CSharpExpression.Bind(hoursParameter, hours);
            var arg1 = CSharpExpression.Bind(minutesParameter, minutes);
            var arg2 = CSharpExpression.Bind(secondsParameter, seconds);

            {
                var res = CSharpExpression.New(ctor, arg0, arg1, arg2);

                Assert.Equal(CSharpExpressionType.New, res.CSharpNodeType);
                Assert.Equal(ctor, res.Constructor);
                Assert.Equal(typeof(TimeSpan), res.Type);
                Assert.True(res.Arguments.SequenceEqual(new[] { arg0, arg1, arg2 }));
            }

            {
                var res = CSharpExpression.New(ctor, new[] { arg0, arg1, arg2 }.AsEnumerable());

                Assert.Equal(CSharpExpressionType.New, res.CSharpNodeType);
                Assert.Equal(ctor, res.Constructor);
                Assert.Equal(typeof(TimeSpan), res.Type);
                Assert.True(res.Arguments.SequenceEqual(new[] { arg0, arg1, arg2 }));
            }
        }

        [Fact]
        public void New_Update()
        {
            var ctor = ConstructorInfoOf(() => new TimeSpan(default(int), default(int), default(int)));

            var parameters = ctor.GetParameters();

            var hoursParameter = parameters[0];
            var minutesParameter = parameters[1];
            var secondsParameter = parameters[2];

            var hours = Expression.Constant(1);
            var minutes = Expression.Constant(2);
            var seconds = Expression.Constant(3);

            var arg0 = CSharpExpression.Bind(hoursParameter, hours);
            var arg1 = CSharpExpression.Bind(minutesParameter, minutes);
            var arg2 = CSharpExpression.Bind(secondsParameter, seconds);

            var res = CSharpExpression.New(ctor, arg0, arg1, arg2);

            var upd1 = res.Update(res.Arguments);
            Assert.Same(upd1, res);

            var upd2 = res.Update(new[] { arg1, arg0, arg2 });
            Assert.NotSame(upd2, res);
            Assert.True(upd2.Arguments.SequenceEqual(new[] { arg1, arg0, arg2 }));
        }

        [Fact]
        public void New_Compile1()
        {
            var ctor = ConstructorInfoOf(() => new TimeSpan(default(int), default(int), default(int)));

            var parameters = ctor.GetParameters();

            var parameterHours = parameters[0];
            var parameterMinutes = parameters[1];
            var parameterSeconds = parameters[2];

            var valueHours = Expression.Constant(1);
            var valueMinutes = Expression.Constant(2);
            var valueSeconds = Expression.Constant(3);

            AssertCompile<TimeSpan>(log =>
                CSharpExpression.New(ctor,
                    CSharpExpression.Bind(parameterHours, log(valueHours, "H")),
                    CSharpExpression.Bind(parameterMinutes, log(valueMinutes, "M")),
                    CSharpExpression.Bind(parameterSeconds, log(valueSeconds, "S"))
                ),
                new LogAndResult<TimeSpan> { Value = new TimeSpan(1, 2, 3), Log = { "H", "M", "S" } }
            );

            AssertCompile<TimeSpan>(log =>
                CSharpExpression.New(ctor,
                    CSharpExpression.Bind(parameterMinutes, log(valueMinutes, "M")),
                    CSharpExpression.Bind(parameterHours, log(valueHours, "H")),
                    CSharpExpression.Bind(parameterSeconds, log(valueSeconds, "S"))
                ),
                new LogAndResult<TimeSpan> { Value = new TimeSpan(1, 2, 3), Log = { "M", "H", "S" } }
            );
        }

        [Fact]
        public void New_Compile2()
        {
            var ctor = ConstructorInfoOf(() => new C(default(int), default(int), default(int)));

            var parameters = ctor.GetParameters();

            var parameterX = parameters[0];
            var parameterY = parameters[1];
            var parameterZ = parameters[2];

            var valueX = Expression.Constant(1);
            var valueY = Expression.Constant(2);
            var valueZ = Expression.Constant(3);

            AssertCompile<int>(log =>
                Expression.Field(
                    CSharpExpression.New(ctor,
                        CSharpExpression.Bind(parameterY, log(valueY, "Y")),
                        CSharpExpression.Bind(parameterX, log(valueX, "Z"))
                    ),
                    "Value"
                ),
                new LogAndResult<int> { Value = new C(1, 2).Value, Log = { "Y", "Z" } }
            );

            AssertCompile<int>(log =>
                Expression.Field(
                    CSharpExpression.New(ctor,
                        CSharpExpression.Bind(parameterX, log(valueX, "X")),
                        CSharpExpression.Bind(parameterY, log(valueY, "Y")),
                        CSharpExpression.Bind(parameterZ, log(valueZ, "Z"))
                    ),
                    "Value"
                ),
                new LogAndResult<int> { Value = new C(1, 2, 3).Value, Log = { "X", "Y", "Z" } }
            );
        }

        private void AssertCompile<T>(Func<Func<Expression, string, Expression>, Expression> createExpression, LogAndResult<T> expected)
        {
            var res = WithLogValue<T>(createExpression).Compile()();
            Assert.Equal(expected, res);
        }

        [Fact]
        public void New_Visitor()
        {
            var ctor = ConstructorInfoOf(() => new TimeSpan(default(long)));
            var valueParameter = ctor.GetParameters()[0];
            var value = Expression.Constant(42L);
            var res = CSharpExpression.New(ctor, CSharpExpression.Bind(valueParameter, value));

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitNew(NewCSharpExpression node)
            {
                Visited = true;

                return base.VisitNew(node);
            }
        }

        class C
        {
            public int Value;

            public C(int x, int y, int z = 42)
            {
                Value = x * y + z;
            }
        }

        class S
        {
            static S()
            {
            }
        }
    }
}
