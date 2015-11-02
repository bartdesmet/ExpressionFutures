// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class LockTests
    {
        [TestMethod]
        public void Lock_Factory_ArgumentChecking()
        {
            var @lock = Expression.Constant(new object());
            var body = Expression.Empty();

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Lock(default(Expression), body));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Lock(@lock, default(Expression)));

            // value type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Lock(Expression.Default(typeof(int)), body));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Lock(Expression.Default(typeof(int?)), body));
        }

        [TestMethod]
        public void Lock_Properties()
        {
            var @lock = Expression.Constant(new object());
            var body = Expression.Empty();
            var res = CSharpExpression.Lock(@lock, body);

            Assert.AreEqual(CSharpExpressionType.Lock, res.CSharpNodeType);
            Assert.AreEqual(typeof(void), res.Type);
            Assert.AreSame(@lock, res.Expression);
            Assert.AreSame(body, res.Body);
        }

        [TestMethod]
        public void Lock_Update()
        {
            var @lock = Expression.Constant(new object());
            var body = Expression.Empty();
            var res = CSharpExpression.Lock(@lock, body);

            Assert.AreSame(res, res.Update(res.Expression, res.Body));

            var newLock = Expression.Constant(new object());
            var newBody = Expression.Empty();

            var upd1 = res.Update(newLock, res.Body);
            var upd2 = res.Update(res.Expression, newBody);

            Assert.AreSame(newLock, upd1.Expression);
            Assert.AreSame(res.Body, upd1.Body);

            Assert.AreSame(res.Expression, upd2.Expression);
            Assert.AreSame(newBody, upd2.Body);
        }

        [TestMethod]
        public void Lock_Compile()
        {
            var o = new object();

            var locked = new ManualResetEvent(false);
            var unlock = new ManualResetEvent(false);

            var setLocked = ((Expression<Action>)(() => locked.Set())).Body;
            var waitUnlock = ((Expression<Action>)(() => unlock.WaitOne())).Body;

            var expr = CSharpExpression.Lock(
                Expression.Constant(o),
                Expression.Block(
                    setLocked,
                    waitUnlock
                )
            );
            var f = Expression.Lambda<Action>(expr).Compile();

            var t = Task.Run(f);
            locked.WaitOne();
            Assert.IsFalse(Monitor.TryEnter(o));

            unlock.Set();
            t.Wait();
            Assert.IsTrue(Monitor.TryEnter(o));

            Monitor.Exit(o);
        }

        [TestMethod]
        public void Lock_Visitor()
        {
            var @lock = Expression.Constant(new object());
            var body = Expression.Empty();
            var res = CSharpExpression.Lock(@lock, body);

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitLock(LockCSharpStatement node)
            {
                Visited = true;

                return base.VisitLock(node);
            }
        }
    }
}
