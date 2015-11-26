// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class GotoTests
    {
        [TestMethod]
        public void GotoLabel_Factory_ArgumentChecking()
        {
            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.GotoLabel(default(LabelTarget)));

            // type
            var l = Expression.Label(typeof(int));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.GotoLabel(l));
        }

        [TestMethod]
        public void GotoCase_Factory_ArgumentChecking()
        {
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.GotoCase(0.0));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.GotoCase(0.0f));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.GotoCase(0.0m));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.GotoCase(TimeSpan.Zero));
        }

        [TestMethod]
        public void GotoLabel_Properties()
        {
            var l = Expression.Label();
            var g = CSharpStatement.GotoLabel(l);

            Assert.AreEqual(typeof(void), g.Type);
            Assert.AreEqual(CSharpExpressionType.Goto, g.CSharpNodeType);
            Assert.AreEqual(CSharpGotoKind.GotoLabel, g.Kind);
            Assert.AreSame(l, g.Target);

            Assert.IsTrue(g.CanReduce);
        }

        [TestMethod]
        public void GotoCase_Properties()
        {
            var o = (object)42;
            var g = CSharpStatement.GotoCase(o);

            Assert.AreEqual(typeof(void), g.Type);
            Assert.AreEqual(CSharpExpressionType.Goto, g.CSharpNodeType);
            Assert.AreEqual(CSharpGotoKind.GotoCase, g.Kind);
            Assert.AreSame(o, g.Value);

            Assert.IsFalse(g.CanReduce);
            AssertEx.Throws<InvalidOperationException>(() => g.Reduce());
        }

        [TestMethod]
        public void GotoDefault_Properties()
        {
            var g = CSharpStatement.GotoDefault();

            Assert.AreEqual(typeof(void), g.Type);
            Assert.AreEqual(CSharpExpressionType.Goto, g.CSharpNodeType);
            Assert.AreEqual(CSharpGotoKind.GotoDefault, g.Kind);

            Assert.IsFalse(g.CanReduce);
            AssertEx.Throws<InvalidOperationException>(() => g.Reduce());
        }

        [TestMethod]
        public void GotoDefault_NoSingleton()
        {
            var g1 = CSharpStatement.GotoDefault();
            var g2 = CSharpStatement.GotoDefault();

            Assert.AreNotSame(g1, g2);
        }

        [TestMethod]
        public void GotoLabel_Update()
        {
            var l = Expression.Label();
            var m = Expression.Label();
            var g = CSharpStatement.GotoLabel(l);

            var u1 = g.Update(l);
            Assert.AreSame(g, u1);

            var u2 = g.Update(m);
            Assert.AreNotSame(g, u2);
            Assert.AreSame(m, u2.Target);
        }

        [TestMethod]
        public void GotoLabel_Visitor()
        {
            var l = Expression.Label();
            var g = CSharpStatement.GotoLabel(l);

            var v = new V();
            Assert.AreSame(g, v.Visit(g));
            Assert.AreEqual(g.Kind, v.VisitedKind);
        }

        [TestMethod]
        public void GotoCase_Visitor()
        {
            var g = CSharpStatement.GotoCase(42);

            var v = new V();
            Assert.AreSame(g, v.Visit(g));
            Assert.AreEqual(g.Kind, v.VisitedKind);
        }

        [TestMethod]
        public void GotoDefault_Visitor()
        {
            var g = CSharpStatement.GotoDefault();

            var v = new V();
            Assert.AreSame(g, v.Visit(g));
            Assert.AreEqual(g.Kind, v.VisitedKind);
        }

        class V : CSharpExpressionVisitor
        {
            public CSharpGotoKind VisitedKind;

            protected internal override Expression VisitGotoCase(GotoCaseCSharpStatement node)
            {
                VisitedKind = node.Kind;

                return base.VisitGotoCase(node);
            }

            protected internal override Expression VisitGotoDefault(GotoDefaultCSharpStatement node)
            {
                VisitedKind = node.Kind;

                return base.VisitGotoDefault(node);
            }

            protected internal override Expression VisitGotoLabel(GotoLabelCSharpStatement node)
            {
                VisitedKind = node.Kind;

                return base.VisitGotoLabel(node);
            }
        }
    }
}
