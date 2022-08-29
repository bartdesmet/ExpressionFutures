// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    public class NewMultidimensionalArrayInitTests
    {
        [Fact]
        public void NewMultidimensionalArrayInit_Factory_ArgumentChecking()
        {
            var type = typeof(int);
            var bounds = new int[] { 2, 3, 5 };
            var inits = Enumerable.Range(0, 30).Select(i => Expression.Constant(i));

            // null
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.NewMultidimensionalArrayInit(default(Type), bounds, inits));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.NewMultidimensionalArrayInit(type, default(int[]), inits));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.NewMultidimensionalArrayInit(type, bounds, default(IEnumerable<Expression>)));

            // typeof(void)
            Assert.Throws<ArgumentException>(() => CSharpExpression.NewMultidimensionalArrayInit(typeof(void), bounds, inits));

            // no bounds
            Assert.Throws<ArgumentException>(() => CSharpExpression.NewMultidimensionalArrayInit(type, new int[0], inits));

            // bound < 0
            Assert.Throws<ArgumentException>(() => CSharpExpression.NewMultidimensionalArrayInit(type, new[] { 2, -1, 5 }, inits));

            // wrong # of inits
            Assert.Throws<ArgumentException>(() => CSharpExpression.NewMultidimensionalArrayInit(type, bounds, inits.Take(29)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.NewMultidimensionalArrayInit(type, bounds, inits.Concat(inits).Take(31)));

            // element mismatch (NB: exception type comes from LINQ)
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.NewMultidimensionalArrayInit(typeof(long), bounds, inits));
        }

        [Fact]
        public void NewMultidimensionalArrayInit_Factory_CanQuote()
        {
            var e2 = (Expression<Func<int>>)(() => 42);
            var e1 = Expression.Constant(e2);
            var res = CSharpExpression.NewMultidimensionalArrayInit(typeof(Expression<Func<int>>), new[] { 1, 2 }, e1, e2);

            Assert.Equal(ExpressionType.Quote, res.Expressions[1].NodeType);
            Assert.Same(e2, ((UnaryExpression)res.Expressions[1]).Operand);
        }

        [Fact]
        public void NewMultidimensionalArrayInit_Properties()
        {
            var inits = Enumerable.Range(0, 6).Select(i => Expression.Constant(i)).ToArray();
            var res = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3 }, inits);

            Assert.Equal(CSharpExpressionType.NewMultidimensionalArrayInit, res.CSharpNodeType);
            Assert.Equal(typeof(int).MakeArrayType(2), res.Type);
            Assert.True(res.Expressions.SequenceEqual(inits));
        }

        [Fact]
        public void NewMultidimensionalArrayInit_GetExpression_ArgumentChecking()
        {
            var inits = Enumerable.Range(0, 6).Select(i => Expression.Constant(i));
            var res = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3 }, inits);

            // null
            Assert.Throws<ArgumentNullException>(() => res.GetExpression(null));

            // rank mismatch
            Assert.Throws<ArgumentException>(() => res.GetExpression());
            Assert.Throws<ArgumentException>(() => res.GetExpression(1));

            // out of range
            Assert.Throws<ArgumentOutOfRangeException>(() => res.GetExpression(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => res.GetExpression(2, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => res.GetExpression(0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => res.GetExpression(0, 4));
        }

        [Fact]
        public void NewMultidimensionalArrayInit_GetExpression()
        {
            var inits = Enumerable.Range(0, 6).Select(i => Expression.Constant(i)).ToArray();
            var res = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3 }, inits);

            Assert.Same(inits[0], res.GetExpression(0, 0));
            Assert.Same(inits[1], res.GetExpression(0, 1));
            Assert.Same(inits[2], res.GetExpression(0, 2));
            Assert.Same(inits[3], res.GetExpression(1, 0));
            Assert.Same(inits[4], res.GetExpression(1, 1));
            Assert.Same(inits[5], res.GetExpression(1, 2));
        }

        [Fact]
        public void NewMultidimensionalArrayInit_Update()
        {
            var inits = Enumerable.Range(0, 6).Select(i => Expression.Constant(i)).ToArray();
            var res = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3 }, inits);

            Assert.Same(res, res.Update(res.Expressions));

            var rev = inits.Reverse().ToArray();
            var upd = res.Update(rev);

            Assert.NotSame(res, upd);
            Assert.True(upd.Expressions.SequenceEqual(rev));
        }

        [Fact]
        public void NewMultidimensionalArrayInit_Compile()
        {
            var inits = Enumerable.Range(0, 6).Select(i => Expression.Constant(42 - i * 3)).ToArray();
            var expr = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3 }, inits);
            var res = Expression.Lambda<Func<int[,]>>(expr).Compile()();

            Assert.Equal(inits[0].Value, res[0, 0]);
            Assert.Equal(inits[1].Value, res[0, 1]);
            Assert.Equal(inits[2].Value, res[0, 2]);
            Assert.Equal(inits[3].Value, res[1, 0]);
            Assert.Equal(inits[4].Value, res[1, 1]);
            Assert.Equal(inits[5].Value, res[1, 2]);
        }

        [Fact]
        public void NewMultidimensionalArrayInit_Visitor()
        {
            var inits = Enumerable.Range(0, 6).Select(i => Expression.Constant(i)).ToArray();
            var res = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3 }, inits);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitNewMultidimensionalArrayInit(NewMultidimensionalArrayInitCSharpExpression node)
            {
                Visited = true;

                return base.VisitNewMultidimensionalArrayInit(node);
            }
        }
    }
}
