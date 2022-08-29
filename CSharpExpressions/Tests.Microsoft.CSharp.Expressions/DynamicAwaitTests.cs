// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class DynamicAwaitTests
    {
        [Fact]
        public void DynamicAwait_Factory_ArgumentChecking()
        {
            Assert.Throws<ArgumentNullException>(() => DynamicCSharpExpression.DynamicAwait(default(Expression)));
            Assert.Throws<ArgumentNullException>(() => DynamicCSharpExpression.DynamicAwait(default(Expression), false));
            Assert.Throws<ArgumentNullException>(() => DynamicCSharpExpression.DynamicAwait(default(Expression), false, typeof(DynamicAwaitTests)));
        }

        [Fact]
        public void DynamicAwait_Properties()
        {
            var e = Expression.Default(typeof(Task<int>));
            var expr = DynamicCSharpExpression.DynamicAwait(e);
            Assert.Equal(CSharpExpressionType.Await, expr.CSharpNodeType);
            Assert.Same(e, expr.Operand);
            Assert.Equal(typeof(object), expr.Type);
            Assert.True(expr.Info.IsDynamic);

            var d = expr.Info as DynamicAwaitInfo;
            Assert.NotNull(d);
            Assert.Null(d.Context);
            Assert.False(d.ResultDiscarded);
        }

        [Fact]
        public void DynamicAwait_Properties_Void()
        {
            var e = Expression.Default(typeof(Task));
            var expr = DynamicCSharpExpression.DynamicAwait(e, true);
            Assert.Equal(CSharpExpressionType.Await, expr.CSharpNodeType);
            Assert.Same(e, expr.Operand);
            Assert.Equal(typeof(void), expr.Type);
            Assert.True(expr.Info.IsDynamic);

            var d = expr.Info as DynamicAwaitInfo;
            Assert.NotNull(d);
            Assert.Null(d.Context);
            Assert.True(d.ResultDiscarded);
        }

        [Fact]
        public void DynamicAwait_Update()
        {
            var e = Expression.Default(typeof(Task<int>));
            var expr = DynamicCSharpExpression.DynamicAwait(e);
            Assert.Same(expr, expr.Update(e, expr.Info));

            var f = Expression.Default(typeof(Task<int>));
            var upd = expr.Update(f, expr.Info);
            Assert.NotSame(upd, expr);
            Assert.Same(f, upd.Operand);
        }

        [Fact]
        public void DynamicAwait_CantReduce()
        {
            var e = Expression.Default(typeof(Task<int>));
            var expr = DynamicCSharpExpression.DynamicAwait(e);

            Assert.False(expr.CanReduce);
            Assert.Same(expr, expr.Reduce());

            var f = Expression.Lambda<Func<object>>(expr);
            Assert.Throws<ArgumentException>(() => f.Compile());
        }

        [Fact]
        public void DynamicAwait_Visitor()
        {
            var res = DynamicCSharpExpression.DynamicAwait(Expression.Default(typeof(Task<int>)));

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        [Fact]
        public void DynamicAwait_Compile()
        {
            var e = Expression.Constant(Task.FromResult(42));
            var expr = DynamicCSharpExpression.DynamicAwait(e);

            var f = CSharpExpression.AsyncLambda<Func<Task<object>>>(expr);
            Assert.Equal(42, f.Compile()().Result);
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
