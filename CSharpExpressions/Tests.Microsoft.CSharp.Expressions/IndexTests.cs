// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    public class IndexTests
    {
        [Fact]
        public void Index_Factory_ArgumentChecking()
        {
            // NB: A lot of checks are performed by LINQ helpers, so we omit tests for those cases.

            var substring = PropertyInfoOf((S s) => s[default(int), default(int)]);

            var parameters = substring.GetIndexParameters();

            var startIndexParameter = parameters[0];
            var lengthParameter = parameters[1];

            var obj = Expression.Constant(new S("bar"));
            var startIndex = Expression.Constant(0);
            var length = Expression.Constant(1);

            var argStartIndex = CSharpExpression.Bind(startIndexParameter, startIndex);
            var argLength = CSharpExpression.Bind(lengthParameter, length);

            var listIndex = PropertyInfoOf((List<int> xs) => xs[default(int)]);

            var valueParameter = listIndex.GetIndexParameters()[0];

            var value = Expression.Constant(42);
            var argValue = CSharpExpression.Bind(valueParameter, value);

            // duplicate
            Assert.Throws<ArgumentException>(() => CSharpExpression.Index(obj, substring, argStartIndex, argStartIndex));

            // unbound
            Assert.Throws<ArgumentException>(() => CSharpExpression.Index(obj, substring, argLength));

            // wrong member
            Assert.Throws<ArgumentException>(() => CSharpExpression.Index(obj, substring, argValue));

            // null
            var bindings = new[] { argStartIndex, argLength };
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Index(default(Expression), substring, bindings));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Index(default(Expression), substring, bindings.AsEnumerable()));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Index(obj, default(PropertyInfo), bindings));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Index(obj, default(PropertyInfo), bindings.AsEnumerable()));

            // only setter
            Assert.Throws<ArgumentException>(() => CSharpExpression.Index(Expression.Default(typeof(X)), typeof(X).GetProperty("Item")));
        }

        [Fact]
        public void Index_Factory_Expression()
        {
            var obj = Expression.Constant(new S("foo"));
            var substring = PropertyInfoOf((S s) => s[default(int), default(int)]);
            var substringGet = substring.GetGetMethod(true);

            var args = new[] { Expression.Constant(1) };

            foreach (var e in new[]
            {
                CSharpExpression.Index(obj, substring, args),
                CSharpExpression.Index(obj, substring, args.AsEnumerable()),
            })
            {
                Assert.Same(obj, e.Object);

                Assert.Single(e.Arguments);

                Assert.Equal(substring.GetIndexParameters()[0], e.Arguments[0].Parameter);

                Assert.Same(args[0], e.Arguments[0].Expression);
            }

            foreach (var e in new[]
            {
                CSharpExpression.Index(obj, substringGet, args),
                CSharpExpression.Index(obj, substringGet, args.AsEnumerable()),
            })
            {
                Assert.Same(obj, e.Object);

                Assert.Single(e.Arguments);

                Assert.Equal(substringGet.GetParameters()[0], e.Arguments[0].Parameter);

                Assert.Same(args[0], e.Arguments[0].Expression);
            }

            var tooLittle = new Expression[0];

            foreach (var f in new Func<IndexCSharpExpression>[]
            {
                () => CSharpExpression.Index(obj, substring, tooLittle),
                () => CSharpExpression.Index(obj, substring, tooLittle.AsEnumerable()),
            })
            {
                Assert.Throws<ArgumentException>(() => f());
            }

            var tooMany = new[] { Expression.Constant(1), Expression.Constant(2), Expression.Constant(3) };

            foreach (var f in new Func<IndexCSharpExpression>[]
            {
                () => CSharpExpression.Index(obj, substring, tooMany),
                () => CSharpExpression.Index(obj, substring, tooMany.AsEnumerable()),
            })
            {
                Assert.Throws<ArgumentException>(() => f());
            }
        }

        [Fact]
        public void Index_Factory_CanUseMethod()
        {
            var substringMethod = MethodInfoOf((S s) => s[default(int), default(int)]);
            var substringProperty = PropertyInfoOf((S s) => s[default(int), default(int)]);

            var parameters = substringMethod.GetParameters();

            var startIndexParameter = parameters[0];
            var lengthParameter = parameters[1];

            var obj = Expression.Constant(new S("foobar"));
            var startIndex = Expression.Constant(1);
            var length = Expression.Constant(2);

            var argStartIndex = CSharpExpression.Bind(startIndexParameter, startIndex);
            var argLength = CSharpExpression.Bind(lengthParameter, length);

            var expr1 = CSharpExpression.Index(obj, substringMethod, argStartIndex, argLength);
            Assert.Equal(substringProperty, expr1.Indexer);
            AssertCompile<string>(_ => expr1, new LogAndResult<string> { Value = "foobar".Substring(1, 2) });

            var expr2 = CSharpExpression.Index(obj, substringMethod, new[] { argStartIndex, argLength }.AsEnumerable());
            Assert.Equal(substringProperty, expr2.Indexer);
            AssertCompile<string>(_ => expr2, new LogAndResult<string> { Value = "foobar".Substring(1, 2) });
        }

        [Fact]
        public void Index_Properties()
        {
            var substring = PropertyInfoOf((S s) => s[default(int), default(int)]);

            var parameters = substring.GetIndexParameters();

            var startIndexParameter = parameters[0];
            var lengthParameter = parameters[1];

            var obj = Expression.Constant(new S("bar"));
            var startIndex = Expression.Constant(0);
            var length = Expression.Constant(1);

            var arg0 = CSharpExpression.Bind(startIndexParameter, startIndex);
            var arg1 = CSharpExpression.Bind(lengthParameter, length);

            {
                var res = CSharpExpression.Index(obj, substring, arg0, arg1);

                Assert.Equal(CSharpExpressionType.Index, res.CSharpNodeType);
                Assert.Same(obj, res.Object);
                Assert.Equal(substring, res.Indexer);
                Assert.Equal(typeof(string), res.Type);
                Assert.True(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }

            {
                var res = CSharpExpression.Index(obj, substring, new[] { arg0, arg1 }.AsEnumerable());

                Assert.Equal(CSharpExpressionType.Index, res.CSharpNodeType);
                Assert.Same(obj, res.Object);
                Assert.Equal(substring, res.Indexer);
                Assert.Equal(typeof(string), res.Type);
                Assert.True(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }
        }

        [Fact]
        public void Index_Update()
        {
            var substring = PropertyInfoOf((S s) => s[default(int), default(int)]);

            var parameters = substring.GetIndexParameters();

            var startIndexParameter = parameters[0];
            var lengthParameter = parameters[1];

            var obj = Expression.Constant(new S("bar"));
            var startIndex = Expression.Constant(0);
            var length = Expression.Constant(1);

            var arg0 = CSharpExpression.Bind(startIndexParameter, startIndex);
            var arg1 = CSharpExpression.Bind(lengthParameter, length);

            var res = CSharpExpression.Index(obj, substring, arg0, arg1);

            Assert.Same(res, res.Update(res.Object, res.Arguments));

            var obj1 = Expression.Constant(new S("foo"));
            var upd1 = res.Update(obj1, res.Arguments);
            Assert.NotSame(upd1, res);
            Assert.Same(res.Arguments, upd1.Arguments);
            Assert.Same(obj1, upd1.Object);

            var upd2 = res.Update(obj, new[] { arg1, arg0 });
            Assert.NotSame(upd2, res);
            Assert.Same(res.Object, upd2.Object);
            Assert.True(upd2.Arguments.SequenceEqual(new[] { arg1, arg0 }));
        }

        [Fact]
        public void Index_Compile()
        {
            var method = PropertyInfoOf((S s) => s[default(int), default(int)]);

            var parameters = method.GetIndexParameters();

            var parameterStartIndex = parameters[0];
            var parameterLength = parameters[1];

            var obj = Expression.Constant(new S("foobar"));
            var valueStartIndex = Expression.Constant(1);
            var valueLength = Expression.Constant(2);

            AssertCompile<string>(log =>
                CSharpExpression.Index(log(obj, "O"), method,
                    CSharpExpression.Bind(parameterStartIndex, log(valueStartIndex, "S")),
                    CSharpExpression.Bind(parameterLength, log(valueLength, "L"))
                ),
                new LogAndResult<string> { Value = "foobar".Substring(1, 2), Log = { "O", "S", "L" } }
            );

            AssertCompile<string>(log =>
                CSharpExpression.Index(log(obj, "O"), method,
                    CSharpExpression.Bind(parameterLength, log(valueLength, "L")),
                    CSharpExpression.Bind(parameterStartIndex, log(valueStartIndex, "S"))
                ),
                new LogAndResult<string> { Value = "foobar".Substring(1, 2), Log = { "O", "L", "S" } }
            );

            AssertCompile<string>(log =>
                CSharpExpression.Index(log(obj, "O"), method,
                    CSharpExpression.Bind(parameterStartIndex, log(valueStartIndex, "S"))
                ),
                new LogAndResult<string> { Value = "foobar".Substring(1), Log = { "O", "S" } }
            );
        }

        private void AssertCompile<T>(Func<Func<Expression, string, Expression>, Expression> createExpression, LogAndResult<T> expected)
        {
            var res = WithLogValue<T>(createExpression).Compile()();
            Assert.Equal(expected, res);
        }

        [Fact]
        public void Index_Visitor()
        {
            var listIndex = PropertyInfoOf((List<int> xs) => xs[default(int)]);
            var valueParameter = listIndex.GetIndexParameters()[0];
            var list = Expression.Constant(new List<int>());
            var value = Expression.Constant(42);
            var res = CSharpExpression.Index(list, listIndex, CSharpExpression.Bind(valueParameter, value));

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitIndex(IndexCSharpExpression node)
            {
                Visited = true;

                return base.VisitIndex(node);
            }
        }

        [Fact]
        public void Index_ReflectionFacts()
        {
            var indexer = PropertyInfoOf((S s) => s[default(int), default(int)]);
            var getter = MethodInfoOf((S s) => s[default(int), default(int)]);

            var indexerParameterOpt = indexer.GetIndexParameters()[1];
            var getterParameterOpt = getter.GetParameters()[1];

            // NB: The Reduce method relies on both of these to have the default value available.
            Assert.True(indexerParameterOpt.HasDefaultValue);
            Assert.True(getterParameterOpt.HasDefaultValue);
        }

        class S
        {
            private readonly string _s;

            public S(string s)
            {
                _s = s;
            }

            public string this[int startIndex, int length = -1]
            {
                get
                {
                    if (length == -1)
                    {
                        return _s.Substring(startIndex);
                    }

                    return _s.Substring(startIndex, length);
                }
            }
        }

        class X
        {
            public object this[int x]
            {
                set { }
            }
        }
    }
}
