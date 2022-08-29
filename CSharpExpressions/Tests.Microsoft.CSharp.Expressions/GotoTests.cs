// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    public class GotoTests
    {
        [Fact]
        public void GotoLabel_Factory_ArgumentChecking()
        {
            // null
            Assert.Throws<ArgumentNullException>(() => CSharpStatement.GotoLabel(default(LabelTarget)));

            // type
            var l = Expression.Label(typeof(int));
            Assert.Throws<ArgumentException>(() => CSharpStatement.GotoLabel(l));
        }

        [Fact]
        public void GotoCase_Factory_ArgumentChecking()
        {
            Assert.Throws<ArgumentException>(() => CSharpStatement.GotoCase(0.0));
            Assert.Throws<ArgumentException>(() => CSharpStatement.GotoCase(0.0f));
            Assert.Throws<ArgumentException>(() => CSharpStatement.GotoCase(0.0m));
            Assert.Throws<ArgumentException>(() => CSharpStatement.GotoCase(TimeSpan.Zero));
        }

        [Fact]
        public void GotoLabel_Properties()
        {
            var l = Expression.Label();
            var g = CSharpStatement.GotoLabel(l);

            Assert.Equal(typeof(void), g.Type);
            Assert.Equal(CSharpExpressionType.Goto, g.CSharpNodeType);
            Assert.Equal(CSharpGotoKind.GotoLabel, g.Kind);
            Assert.Same(l, g.Target);

            Assert.True(g.CanReduce);
        }

        [Fact]
        public void GotoCase_Properties()
        {
            var o = (object)42;
            var g = CSharpStatement.GotoCase(o);

            Assert.Equal(typeof(void), g.Type);
            Assert.Equal(CSharpExpressionType.Goto, g.CSharpNodeType);
            Assert.Equal(CSharpGotoKind.GotoCase, g.Kind);
            Assert.Same(o, g.Value);

            Assert.False(g.CanReduce);
            Assert.Throws<InvalidOperationException>(() => g.Reduce());
        }

        [Fact]
        public void GotoDefault_Properties()
        {
            var g = CSharpStatement.GotoDefault();

            Assert.Equal(typeof(void), g.Type);
            Assert.Equal(CSharpExpressionType.Goto, g.CSharpNodeType);
            Assert.Equal(CSharpGotoKind.GotoDefault, g.Kind);

            Assert.False(g.CanReduce);
            Assert.Throws<InvalidOperationException>(() => g.Reduce());
        }

        [Fact]
        public void GotoDefault_NoSingleton()
        {
            var g1 = CSharpStatement.GotoDefault();
            var g2 = CSharpStatement.GotoDefault();

            Assert.NotSame(g1, g2);
        }

        [Fact]
        public void GotoLabel_Update()
        {
            var l = Expression.Label();
            var m = Expression.Label();
            var g = CSharpStatement.GotoLabel(l);

            var u1 = g.Update(l);
            Assert.Same(g, u1);

            var u2 = g.Update(m);
            Assert.NotSame(g, u2);
            Assert.Same(m, u2.Target);
        }

        [Fact]
        public void GotoLabel_Visitor()
        {
            var l = Expression.Label();
            var g = CSharpStatement.GotoLabel(l);

            var v = new V();
            Assert.Same(g, v.Visit(g));
            Assert.Equal(g.Kind, v.VisitedKind);
        }

        [Fact]
        public void GotoCase_Visitor()
        {
            var g = CSharpStatement.GotoCase(42);

            var v = new V();
            Assert.Same(g, v.Visit(g));
            Assert.Equal(g.Kind, v.VisitedKind);
        }

        [Fact]
        public void GotoDefault_Visitor()
        {
            var g = CSharpStatement.GotoDefault();

            var v = new V();
            Assert.Same(g, v.Visit(g));
            Assert.Equal(g.Kind, v.VisitedKind);
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
