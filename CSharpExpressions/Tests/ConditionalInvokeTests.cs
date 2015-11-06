// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class ConditionalInvokeTests
    {
        [TestMethod]
        public void ConditionalInvoke_Factory_ArgumentChecking()
        {
            var expr = Expression.Default(typeof(Func<int>));

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalInvoke(default(Expression)));
        }

        [TestMethod]
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

                Assert.AreEqual(CSharpExpressionType.ConditionalInvoke, res.CSharpNodeType);
                Assert.AreSame(function, res.Expression);
                Assert.AreEqual(typeof(int?), res.Type);
                Assert.IsTrue(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }

            {
                var res = CSharpExpression.ConditionalInvoke(function, new[] { arg0, arg1 }.AsEnumerable());

                Assert.AreEqual(CSharpExpressionType.ConditionalInvoke, res.CSharpNodeType);
                Assert.AreSame(function, res.Expression);
                Assert.AreEqual(typeof(int?), res.Type);
                Assert.IsTrue(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }
        }

        [TestMethod]
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
            Assert.AreNotSame(upd1, res);
            Assert.AreSame(res.Arguments, upd1.Arguments);
            Assert.AreSame(function1, upd1.Expression);

            var upd2 = res.Update(function, new[] { arg1, arg0 });
            Assert.AreNotSame(upd2, res);
            Assert.AreSame(res.Expression, upd2.Expression);
            Assert.IsTrue(upd2.Arguments.SequenceEqual(new[] { arg1, arg0 }));
        }

        [TestMethod]
        public void ConditionalInvoke_Compile()
        {
        }

        [TestMethod]
        public void ConditionalInvoke_Visitor()
        {
            var invoke = MethodInfoOf((Func<int, int, int> f) => f.Invoke(default(int), default(int)));

            var parameters = invoke.GetParameters();

            var parameterArg1 = parameters[0];
            var parameterArg2 = parameters[1];

            var valueArg1 = Expression.Constant(1);
            var valueArg2 = Expression.Constant(2);

            var function = Expression.Constant(new Func<int, int, int>((x, y) => x + y));

            var res = CSharpExpression.ConditionalInvoke(function, CSharpExpression.Bind(parameterArg1, valueArg1), CSharpExpression.Bind(parameterArg2, valueArg2));

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitConditionalInvocation(ConditionalInvocationCSharpExpression node)
            {
                Visited = true;

                return base.VisitConditionalInvocation(node);
            }
        }
    }
}
