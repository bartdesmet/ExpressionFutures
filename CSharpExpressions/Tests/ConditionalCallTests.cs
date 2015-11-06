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
        public void ConditionalCall_Compile()
        {
        }

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
    }
}
