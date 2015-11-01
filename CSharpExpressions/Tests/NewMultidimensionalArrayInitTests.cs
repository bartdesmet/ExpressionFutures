// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class NewMultidimensionalArrayInitTests
    {
        [TestMethod]
        public void NewMultidimensionalArrayInit_Factory_ArgumentChecking()
        {
            var type = typeof(int);
            var bounds = new int[] { 2, 3, 5 };
            var inits = Enumerable.Range(0, 30).Select(i => Expression.Constant(i));

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.NewMultidimensionalArrayInit(default(Type), bounds, inits));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.NewMultidimensionalArrayInit(type, default(int[]), inits));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.NewMultidimensionalArrayInit(type, bounds, default(IEnumerable<Expression>)));

            // typeof(void)
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.NewMultidimensionalArrayInit(typeof(void), bounds, inits));

            // no bounds
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.NewMultidimensionalArrayInit(type, new int[0], inits));

            // bound < 0
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.NewMultidimensionalArrayInit(type, new[] { 2, -1, 5 }, inits));

            // wrong # of inits
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.NewMultidimensionalArrayInit(type, bounds, inits.Take(29)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.NewMultidimensionalArrayInit(type, bounds, inits.Concat(inits).Take(31)));

            // element mismatch (NB: exception type comes from LINQ)
            AssertEx.Throws<InvalidOperationException>(() => CSharpExpression.NewMultidimensionalArrayInit(typeof(long), bounds, inits));
        }

        [TestMethod]
        public void NewMultidimensionalArrayInit_Factory_CanQuote()
        {
            var e2 = (Expression<Func<int>>)(() => 42);
            var e1 = Expression.Constant(e2);
            var res = CSharpExpression.NewMultidimensionalArrayInit(typeof(Expression<Func<int>>), new[] { 1, 2 }, e1, e2);

            Assert.AreEqual(ExpressionType.Quote, res.Expressions[1].NodeType);
            Assert.AreSame(e2, ((UnaryExpression)res.Expressions[1]).Operand);
        }

        [TestMethod]
        public void NewMultidimensionalArrayInit_Properties()
        {
            var inits = Enumerable.Range(0, 6).Select(i => Expression.Constant(i)).ToArray();
            var res = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3 }, inits);

            Assert.AreEqual(CSharpExpressionType.NewMultidimensionalArrayInit, res.CSharpNodeType);
            Assert.AreEqual(typeof(int).MakeArrayType(2), res.Type);
            Assert.IsTrue(res.Expressions.SequenceEqual(inits));
        }

        [TestMethod]
        public void NewMultidimensionalArrayInit_GetExpression_ArgumentChecking()
        {
            var inits = Enumerable.Range(0, 6).Select(i => Expression.Constant(i));
            var res = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3 }, inits);

            // null
            AssertEx.Throws<ArgumentNullException>(() => res.GetExpression(null));

            // rank mismatch
            AssertEx.Throws<ArgumentException>(() => res.GetExpression());
            AssertEx.Throws<ArgumentException>(() => res.GetExpression(1));

            // out of range
            AssertEx.Throws<ArgumentOutOfRangeException>(() => res.GetExpression(-1, 0));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => res.GetExpression(2, 0));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => res.GetExpression(0, -1));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => res.GetExpression(0, 4));
        }

        [TestMethod]
        public void NewMultidimensionalArrayInit_GetExpression()
        {
            var inits = Enumerable.Range(0, 6).Select(i => Expression.Constant(i)).ToArray();
            var res = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3 }, inits);

            Assert.AreSame(inits[0], res.GetExpression(0, 0));
            Assert.AreSame(inits[1], res.GetExpression(0, 1));
            Assert.AreSame(inits[2], res.GetExpression(0, 2));
            Assert.AreSame(inits[3], res.GetExpression(1, 0));
            Assert.AreSame(inits[4], res.GetExpression(1, 1));
            Assert.AreSame(inits[5], res.GetExpression(1, 2));
        }

        [TestMethod]
        public void NewMultidimensionalArrayInit_Update()
        {
            var inits = Enumerable.Range(0, 6).Select(i => Expression.Constant(i)).ToArray();
            var res = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3 }, inits);

            Assert.AreSame(res, res.Update(res.Expressions));

            var rev = inits.Reverse().ToArray();
            var upd = res.Update(rev);

            Assert.AreNotSame(res, upd);
            Assert.IsTrue(upd.Expressions.SequenceEqual(rev));
        }

        [TestMethod]
        public void NewMultidimensionalArrayInit_Compile()
        {
            var inits = Enumerable.Range(0, 6).Select(i => Expression.Constant(42 - i * 3)).ToArray();
            var expr = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3 }, inits);
            var res = Expression.Lambda<Func<int[,]>>(expr).Compile()();

            Assert.AreEqual(inits[0].Value, res[0, 0]);
            Assert.AreEqual(inits[1].Value, res[0, 1]);
            Assert.AreEqual(inits[2].Value, res[0, 2]);
            Assert.AreEqual(inits[3].Value, res[1, 0]);
            Assert.AreEqual(inits[4].Value, res[1, 1]);
            Assert.AreEqual(inits[5].Value, res[1, 2]);
        }

        [TestMethod]
        public void NewMultidimensionalArrayInit_Visitor()
        {
            var inits = Enumerable.Range(0, 6).Select(i => Expression.Constant(i)).ToArray();
            var res = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3 }, inits);

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected override Expression VisitNewMultidimensionalArrayInit(NewMultidimensionalArrayInitCSharpExpression node)
            {
                Visited = true;

                return base.VisitNewMultidimensionalArrayInit(node);
            }
        }
    }
}
