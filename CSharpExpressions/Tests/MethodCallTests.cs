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
    public class MethodCallTests
    {
        [TestMethod]
        public void MethodCall_Factory_ArgumentChecking()
        {
            // NB: A lot of checks are performed by LINQ helpers, so we omit tests for those cases.

            var substring = MethodInfoOf((string s) => s.Substring(default(int), default(int)));

            var startIndexParameter = substring.GetParameters()[0];
            var lengthParameter = substring.GetParameters()[1];

            var obj = Expression.Constant("bar");
            var startIndex = Expression.Constant(0);
            var length = Expression.Constant(1);

            var argStartIndex = CSharpExpression.Bind(startIndexParameter, startIndex);
            var argLength = CSharpExpression.Bind(lengthParameter, length);

            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));

            var valueParameter = cout.GetParameters()[0];

            var value = Expression.Constant(42);
            var argValue = CSharpExpression.Bind(valueParameter, value);

            // duplicate
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Call(obj, substring, argStartIndex, argStartIndex));

            // unbound
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Call(obj, substring, argStartIndex));

            // wrong member
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Call(obj, substring, argValue));

            // null
            var bindings = new[] { argStartIndex, argLength };
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Call(default(MethodInfo), bindings));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Call(default(MethodInfo), bindings.AsEnumerable()));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Call(obj, default(MethodInfo), bindings));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Call(obj, default(MethodInfo), bindings.AsEnumerable()));
        }

        [TestMethod]
        public void MethodCall_Properties_Instance()
        {
            var substring = MethodInfoOf((string s) => s.Substring(default(int), default(int)));

            var startIndexParameter = substring.GetParameters()[0];
            var lengthParameter = substring.GetParameters()[1];

            var obj = Expression.Constant("bar");
            var startIndex = Expression.Constant(0);
            var length = Expression.Constant(1);

            var arg0 = CSharpExpression.Bind(startIndexParameter, startIndex);
            var arg1 = CSharpExpression.Bind(lengthParameter, length);

            {
                var res = CSharpExpression.Call(obj, substring, arg0, arg1);

                Assert.AreEqual(CSharpExpressionType.Call, res.CSharpNodeType);
                Assert.AreSame(obj, res.Object);
                Assert.AreEqual(substring, res.Method);
                Assert.AreEqual(typeof(string), res.Type);
                Assert.IsTrue(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }

            {
                var res = CSharpExpression.Call(obj, substring, new[] { arg0, arg1 }.AsEnumerable());

                Assert.AreEqual(CSharpExpressionType.Call, res.CSharpNodeType);
                Assert.AreSame(obj, res.Object);
                Assert.AreEqual(substring, res.Method);
                Assert.AreEqual(typeof(string), res.Type);
                Assert.IsTrue(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }
        }

        [TestMethod]
        public void MethodCall_Properties_Static()
        {
            var min = MethodInfoOf(() => Math.Min(default(int), default(int)));

            var val1Parameter = min.GetParameters()[0];
            var val2Parameter = min.GetParameters()[1];

            var val1 = Expression.Constant(0);
            var val2 = Expression.Constant(1);

            var arg0 = CSharpExpression.Bind(val1Parameter, val1);
            var arg1 = CSharpExpression.Bind(val2Parameter, val2);

            {
                var res = CSharpExpression.Call(min, arg0, arg1);

                Assert.AreEqual(CSharpExpressionType.Call, res.CSharpNodeType);
                Assert.IsNull(res.Object);
                Assert.AreEqual(min, res.Method);
                Assert.AreEqual(typeof(int), res.Type);
                Assert.IsTrue(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }

            {
                var res = CSharpExpression.Call(min, new[] { arg0, arg1 }.AsEnumerable());

                Assert.AreEqual(CSharpExpressionType.Call, res.CSharpNodeType);
                Assert.IsNull(res.Object);
                Assert.AreEqual(min, res.Method);
                Assert.AreEqual(typeof(int), res.Type);
                Assert.IsTrue(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }
        }

        [TestMethod]
        public void MethodCall_Update()
        {
            var substring = MethodInfoOf((string s) => s.Substring(default(int), default(int)));

            var startIndexParameter = substring.GetParameters()[0];
            var lengthParameter = substring.GetParameters()[1];

            var obj = Expression.Constant("bar");
            var startIndex = Expression.Constant(0);
            var length = Expression.Constant(1);

            var arg0 = CSharpExpression.Bind(startIndexParameter, startIndex);
            var arg1 = CSharpExpression.Bind(lengthParameter, length);

            var res = CSharpExpression.Call(obj, substring, arg0, arg1);

            Assert.AreSame(res, res.Update(res.Object, res.Arguments));

            var obj1 = Expression.Constant("foo");
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
        public void MethodCall_Compile_Static()
        {
            var method = MethodInfoOf(() => F(default(int), default(int), default(int)));

            var parameters = method.GetParameters();
            var parameterX = parameters[0];
            var parameterY = parameters[1];
            var parameterZ = parameters[2];

            var valueX = Expression.Constant(1);
            var valueY = Expression.Constant(2);
            var valueZ = Expression.Constant(3);

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterX, log(valueX, "X")),
                    CSharpExpression.Bind(parameterY, log(valueY, "Y"))
                ),
                new LogAndResult<int> { Value = F(1, 2), Log = { "X", "Y" } }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterY, log(valueY, "Y")),
                    CSharpExpression.Bind(parameterX, log(valueX, "X"))
                ),
                new LogAndResult<int> { Value = F(1, 2), Log = { "Y", "X" } }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterY, log(valueY, "Y")),
                    CSharpExpression.Bind(parameterX, log(valueX, "X")),
                    CSharpExpression.Bind(parameterZ, log(valueZ, "Z"))
                ),
                new LogAndResult<int> { Value = F(1, 2, 3), Log = { "Y", "X", "Z" } }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterX, log(valueX, "X")),
                    CSharpExpression.Bind(parameterY, log(valueY, "Y")),
                    CSharpExpression.Bind(parameterZ, log(valueZ, "Z"))
                ),
                new LogAndResult<int> { Value = F(1, 2, 3), Log = { "X", "Y", "Z" } }
            );
        }

        [TestMethod]
        public void MethodCall_Compile_Instance()
        {
            var method = MethodInfoOf((string s) => s.Substring(default(int), default(int)));

            var parameters = method.GetParameters();
            var parameterStartIndex = parameters[0];
            var parameterLength = parameters[1];

            var obj = Expression.Constant("foobar");
            var valueStartIndex = Expression.Constant(1);
            var valueLength = Expression.Constant(2);

            AssertCompile<string>(log =>
                CSharpExpression.Call(log(obj, "O"), method,
                    CSharpExpression.Bind(parameterStartIndex, log(valueStartIndex, "S")),
                    CSharpExpression.Bind(parameterLength, log(valueLength, "L"))
                ),
                new LogAndResult<string> { Value = "foobar".Substring(1, 2), Log = { "O", "S", "L" } }
            );

            AssertCompile<string>(log =>
                CSharpExpression.Call(log(obj, "O"), method,
                    CSharpExpression.Bind(parameterLength, log(valueLength, "L")),
                    CSharpExpression.Bind(parameterStartIndex, log(valueStartIndex, "S"))
                ),
                new LogAndResult<string> { Value = "foobar".Substring(1, 2), Log = { "O", "L", "S" } }
            );
        }

        private void AssertCompile<T>(Func<Func<Expression, string, Expression>, Expression> createExpression, LogAndResult<T> expected)
        {
            var res = WithLogValue<T>(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        public void MethodCall_Visitor()
        {
            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));
            var valueParameter = cout.GetParameters()[0];
            var value = Expression.Constant(42);
            var res = CSharpExpression.Call(cout, CSharpExpression.Bind(valueParameter, value));

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        static int F(int x, int y, int z = 42)
        {
            return x * y + z;
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected override Expression VisitMethodCall(MethodCallCSharpExpression node)
            {
                Visited = true;

                return base.VisitMethodCall(node);
            }
        }
    }
}
