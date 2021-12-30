// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class DiscardPatternTests
    {
        [TestMethod]
        public void DiscardPattern_ArgumentChecking()
        {
            // invalid typing
            AssertEx.Throws<ArgumentException>(() => CSharpPattern.Discard(CSharpPattern.PatternInfo(typeof(object), typeof(int))));
        }

        [TestMethod]
        public void DiscardPattern_Properties()
        {
            var p = CSharpPattern.Discard(CSharpPattern.PatternInfo(typeof(int), typeof(int)));

            Assert.AreEqual(typeof(int), p.InputType);
            Assert.AreEqual(typeof(int), p.NarrowedType);
        }

        [TestMethod]
        public void DiscardPattern_Properties_Object()
        {
            var p = CSharpPattern.Discard();

            Assert.AreEqual(typeof(object), p.InputType);
            Assert.AreEqual(typeof(object), p.NarrowedType);
        }

        [TestMethod]
        public void DiscardPattern_ChangeType()
        {
            var p = CSharpPattern.Discard(CSharpPattern.PatternInfo(typeof(int), typeof(int)));

            Assert.AreSame(p, p.ChangeType(typeof(int)));

            var q = (DiscardCSharpPattern)p.ChangeType(typeof(long));

            Assert.AreNotSame(p, q);
            Assert.AreEqual(typeof(long), q.InputType);
            Assert.AreEqual(typeof(long), q.NarrowedType);
        }

        [TestMethod]
        public void DiscardPattern_ChangeType_Triggered()
        {
            var discard = CSharpPattern.Discard();

            var length = typeof(string).GetProperty(nameof(string.Length));
            var property = CSharpPattern.PropertySubpattern(discard, length);

            Assert.AreNotSame(discard, property.Pattern);
            Assert.AreEqual(typeof(int), property.Pattern.InputType);
            Assert.AreEqual(typeof(int), property.Pattern.NarrowedType);
        }

        [TestMethod]
        public void DiscardPattern_Visitor()
        {
            var p = CSharpPattern.Discard(CSharpPattern.PatternInfo(typeof(int), typeof(int)));

            var visitor = new V();

            visitor.VisitPattern(p);

            Assert.IsTrue(visitor.Visited);
        }

        [TestMethod]
        public void DiscardPattern_Reduce()
        {
            var res = CSharpPattern.Discard(CSharpPattern.PatternInfo(typeof(int), typeof(int)));

            AssertCompile((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.Constant(42));
                return res.Reduce(obj);
            }, new LogAndResult<bool> { Value = true, Log = { "O" } });
        }

        [TestMethod]
        public void DiscardPattern_Reduce_IncompatibleType()
        {
            var res = CSharpPattern.Discard(typeof(int));

            AssertEx.Throws<ArgumentException>(() => res.Reduce(Expression.Default(typeof(long))));
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<bool> expected)
        {
            var res = WithLog<bool>(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override CSharpPattern VisitDiscardPattern(DiscardCSharpPattern node)
            {
                Visited = true;

                return base.VisitDiscardPattern(node);
            }
        }
    }
}
