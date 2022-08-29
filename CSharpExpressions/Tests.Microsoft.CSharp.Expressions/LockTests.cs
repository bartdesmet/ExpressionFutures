// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class LockTests
    {
        [Fact]
        public void Lock_Factory_ArgumentChecking()
        {
            var @lock = Expression.Constant(new object());
            var body = Expression.Empty();

            // null
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Lock(default(Expression), body));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Lock(@lock, default(Expression)));

            // value type
            Assert.Throws<ArgumentException>(() => CSharpExpression.Lock(Expression.Default(typeof(int)), body));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Lock(Expression.Default(typeof(int?)), body));
        }

        [Fact]
        public void Lock_Properties()
        {
            var @lock = Expression.Constant(new object());
            var body = Expression.Empty();
            var res = CSharpExpression.Lock(@lock, body);

            Assert.Equal(CSharpExpressionType.Lock, res.CSharpNodeType);
            Assert.Equal(typeof(void), res.Type);
            Assert.Same(@lock, res.Expression);
            Assert.Same(body, res.Body);
        }

        [Fact]
        public void Lock_Update()
        {
            var @lock = Expression.Constant(new object());
            var body = Expression.Empty();
            var res = CSharpExpression.Lock(@lock, body);

            Assert.Same(res, res.Update(res.Expression, res.Body));

            var newLock = Expression.Constant(new object());
            var newBody = Expression.Empty();

            var upd1 = res.Update(newLock, res.Body);
            var upd2 = res.Update(res.Expression, newBody);

            Assert.Same(newLock, upd1.Expression);
            Assert.Same(res.Body, upd1.Body);

            Assert.Same(res.Expression, upd2.Expression);
            Assert.Same(newBody, upd2.Body);
        }

        [Fact]
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
            Assert.False(Monitor.TryEnter(o));

            unlock.Set();
            t.Wait();
            Assert.True(Monitor.TryEnter(o));

            Monitor.Exit(o);
        }

        [Fact]
        public void Lock_Visitor()
        {
            var @lock = Expression.Constant(new object());
            var body = Expression.Empty();
            var res = CSharpExpression.Lock(@lock, body);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
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
