// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class DynamicAwaitTests
    {
        [TestMethod]
        public void DynamicAwait_Factory_ArgumentChecking()
        {
            AssertEx.Throws<ArgumentNullException>(() => DynamicCSharpExpression.DynamicAwait(default(Expression)));
            AssertEx.Throws<ArgumentNullException>(() => DynamicCSharpExpression.DynamicAwait(default(Expression), false));
            AssertEx.Throws<ArgumentNullException>(() => DynamicCSharpExpression.DynamicAwait(default(Expression), false, typeof(DynamicAwaitTests)));
        }

        [TestMethod]
        public void DynamicAwait_Properties()
        {
            var e = Expression.Default(typeof(Task<int>));
            var expr = DynamicCSharpExpression.DynamicAwait(e);
            Assert.AreEqual(CSharpExpressionType.Await, expr.CSharpNodeType);
            Assert.AreSame(e, expr.Operand);
            Assert.AreEqual(typeof(object), expr.Type);
            Assert.IsTrue(expr.Info.IsDynamic);

            if (expr.Info is DynamicAwaitInfo d)
            {
                Assert.IsNull(d.Context);
                Assert.IsFalse(d.ResultDiscarded);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DynamicAwait_Properties_Void()
        {
            var e = Expression.Default(typeof(Task));
            var expr = DynamicCSharpExpression.DynamicAwait(e, true);
            Assert.AreEqual(CSharpExpressionType.Await, expr.CSharpNodeType);
            Assert.AreSame(e, expr.Operand);
            Assert.AreEqual(typeof(void), expr.Type);
            Assert.IsTrue(expr.Info.IsDynamic);

            if (expr.Info is DynamicAwaitInfo d)
            {
                Assert.IsNull(d.Context);
                Assert.IsTrue(d.ResultDiscarded);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DynamicAwait_Update()
        {
            var e = Expression.Default(typeof(Task<int>));
            var expr = DynamicCSharpExpression.DynamicAwait(e);
            Assert.AreSame(expr, expr.Update(e, expr.Info));

            var f = Expression.Default(typeof(Task<int>));
            var upd = expr.Update(f, expr.Info);
            Assert.AreNotSame(upd, expr);
            Assert.AreSame(f, upd.Operand);
        }

        [TestMethod]
        public void DynamicAwait_CantReduce()
        {
            var e = Expression.Default(typeof(Task<int>));
            var expr = DynamicCSharpExpression.DynamicAwait(e);

            Assert.IsFalse(expr.CanReduce);
            Assert.AreSame(expr, expr.Reduce());

            var f = Expression.Lambda<Func<object>>(expr);
            AssertEx.Throws<ArgumentException>(() => f.Compile());
        }

        [TestMethod]
        public void DynamicAwait_Visitor()
        {
            var res = DynamicCSharpExpression.DynamicAwait(Expression.Default(typeof(Task<int>)));

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        [TestMethod]
        public void DynamicAwait_Compile()
        {
            var e = Expression.Constant(Task.FromResult(42));
            var expr = DynamicCSharpExpression.DynamicAwait(e);

            var f = CSharpExpression.AsyncLambda<Func<Task<object>>>(expr);
            Assert.AreEqual(42, f.Compile()().Result);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitAwait(AwaitCSharpExpression node)
            {
                Visited = true;

                return base.VisitAwait(node);
            }
        }
    }
}
