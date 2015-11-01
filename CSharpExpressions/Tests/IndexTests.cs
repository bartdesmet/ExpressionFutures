// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class IndexTests
    {
        [TestMethod]
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
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Index(obj, substring, argStartIndex, argStartIndex));

            // unbound
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Index(obj, substring, argLength));

            // wrong member
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Index(obj, substring, argValue));

            // null
            var bindings = new[] { argStartIndex, argLength };
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Index(default(Expression), substring, bindings));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Index(default(Expression), substring, bindings.AsEnumerable()));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Index(obj, default(PropertyInfo), bindings));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Index(obj, default(PropertyInfo), bindings.AsEnumerable()));
        }

        [TestMethod]
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

                Assert.AreEqual(CSharpExpressionType.Index, res.CSharpNodeType);
                Assert.AreSame(obj, res.Object);
                Assert.AreEqual(substring, res.Indexer);
                Assert.AreEqual(typeof(string), res.Type);
                Assert.IsTrue(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }

            {
                var res = CSharpExpression.Index(obj, substring, new[] { arg0, arg1 }.AsEnumerable());

                Assert.AreEqual(CSharpExpressionType.Index, res.CSharpNodeType);
                Assert.AreSame(obj, res.Object);
                Assert.AreEqual(substring, res.Indexer);
                Assert.AreEqual(typeof(string), res.Type);
                Assert.IsTrue(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }
        }

        [TestMethod]
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

            Assert.AreSame(res, res.Update(res.Object, res.Arguments));

            var obj1 = Expression.Constant(new S("foo"));
            var upd1 = res.Update(obj1, res.Arguments);
            Assert.AreNotSame(upd1, res);
            Assert.AreSame(res.Arguments, upd1.Arguments);
            Assert.AreSame(obj1, upd1.Object);

            var upd2 = res.Update(obj, new[] { arg1, arg0 });
            Assert.AreNotSame(upd2, res);
            Assert.AreSame(res.Object, upd2.Object);
            Assert.IsTrue(upd2.Arguments.SequenceEqual(new[] { arg1, arg0 }));
        }

        [TestMethod]
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
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        public void Index_Visitor()
        {
            var listIndex = PropertyInfoOf((List<int> xs) => xs[default(int)]);
            var valueParameter = listIndex.GetIndexParameters()[0];
            var list = Expression.Constant(new List<int>());
            var value = Expression.Constant(42);
            var res = CSharpExpression.Index(list, listIndex, CSharpExpression.Bind(valueParameter, value));

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected override Expression VisitIndex(IndexCSharpExpression node)
            {
                Visited = true;

                return base.VisitIndex(node);
            }
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
    }
}
