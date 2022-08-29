// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Xunit;
using static Tests.ReflectionUtils;

namespace Tests
{
    public class ParameterAssignmentTests
    {
        [Fact]
        public void ParameterAssignment_Factory_ArgumentChecking()
        {
            var method = typeof(C).GetMethod("F");
            var parameter = method.GetParameters()[0];
            var name = parameter.Name;
            var expr = Expression.Constant(42);
            
            // null
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Bind(default(ParameterInfo), expr));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Bind(parameter, default(Expression)));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Bind(default(MethodInfo), name, expr));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Bind(method, default(string), expr));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Bind(method, name, default(Expression)));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Bind(default(MethodInfo), 0, expr));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Bind(method, 0, default(Expression)));

            // can't find
            Assert.Throws<ArgumentException>(() => CSharpExpression.Bind(method, "y", expr));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Bind(method, -1, expr));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Bind(method, 1, expr));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Bind(method, 2, expr));

            // type mismatch
            Assert.Throws<ArgumentException>(() => CSharpExpression.Bind(parameter, Expression.Constant("bar")));
        }

        [Fact]
        public void ParameterAssignment_Factory_CanQuote()
        {
            var where = MethodInfoOf(() => Queryable.Where(default(IQueryable<int>), default(Expression<Func<int, bool>>)));

            var filterParameter = where.GetParameters()[1];
            var filter = (Expression<Func<int, bool>>)(i => i < 0);

            var res = CSharpExpression.Bind(filterParameter, filter);

            Assert.Equal(ExpressionType.Quote, res.Expression.NodeType);
            Assert.Same(filter, ((UnaryExpression)res.Expression).Operand);
        }

        [Fact]
        public void ParameterAssignment_Factory_SupportByRef()
        {
            var exchange = MethodInfoOf((int x) => Interlocked.Exchange(ref x, x));

            var refParameter = exchange.GetParameters()[0];
            var expr = Expression.Parameter(typeof(int));

            var res = CSharpExpression.Bind(refParameter, expr);

            Assert.Same(refParameter, res.Parameter);
            Assert.Same(expr, res.Expression);
        }

        [Fact]
        public void ParameterAssignment_Properties()
        {
            var method = typeof(C).GetMethod("F");
            var parameter = method.GetParameters()[0];
            var expr = Expression.Constant(42);

            {
                var res = CSharpExpression.Bind(parameter, expr);

                Assert.Same(parameter, res.Parameter);
                Assert.Same(expr, res.Expression);
            }

            {
                var res = CSharpExpression.Bind(method, parameter.Name, expr);

                Assert.Same(parameter, res.Parameter);
                Assert.Same(expr, res.Expression);
            }

            {
                var res = CSharpExpression.Bind(method, 0, expr);

                Assert.Same(parameter, res.Parameter);
                Assert.Same(expr, res.Expression);
            }
        }

        [Fact]
        public void ParameterAssignment_Update()
        {
            var method = typeof(C).GetMethod("F");
            var parameter = method.GetParameters()[0];
            var expr = Expression.Constant(42);

            var res = CSharpExpression.Bind(parameter, expr);

            Assert.Same(res, res.Update(res.Expression));

            var rev = Expression.Constant(43);
            var upd = res.Update(rev);

            Assert.NotSame(res, upd);
            Assert.Same(rev, upd.Expression);
        }

        [Fact]
        public void ParameterAssignment_Visitor()
        {
            var method = typeof(C).GetMethod("F");
            var parameter = method.GetParameters()[0];
            var expr = Expression.Constant(42);

            var bind = CSharpExpression.Bind(parameter, expr);

            var res = CSharpExpression.Call(method, bind);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class C
        {
            public static void F(int x)
            {
            }
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected override ParameterAssignment VisitParameterAssignment(ParameterAssignment node)
            {
                Visited = true;

                return base.VisitParameterAssignment(node);
            }
        }
    }
}
