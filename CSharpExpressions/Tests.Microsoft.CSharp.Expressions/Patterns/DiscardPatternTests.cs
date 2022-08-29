// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.CSharp.Expressions;
using System;
using System.Linq.Expressions;
using Xunit;
using static Tests.TestHelpers;

namespace Tests
{
    public class DiscardPatternTests
    {
        [Fact]
        public void DiscardPattern_ArgumentChecking()
        {
            // invalid typing
            AssertEx.Throws<ArgumentException>(() => CSharpPattern.Discard(CSharpPattern.PatternInfo(typeof(object), typeof(int))));
        }

        [Fact]
        public void DiscardPattern_Properties()
        {
            var p = CSharpPattern.Discard(CSharpPattern.PatternInfo(typeof(int), typeof(int)));

            Assert.Equal(typeof(int), p.InputType);
            Assert.Equal(typeof(int), p.NarrowedType);
        }

        [Fact]
        public void DiscardPattern_Properties_Object()
        {
            var p = CSharpPattern.Discard();

            Assert.Equal(typeof(object), p.InputType);
            Assert.Equal(typeof(object), p.NarrowedType);
        }

        [Fact]
        public void DiscardPattern_ChangeType()
        {
            var p = CSharpPattern.Discard(CSharpPattern.PatternInfo(typeof(int), typeof(int)));

            Assert.Same(p, p.ChangeType(typeof(int)));

            var q = (DiscardCSharpPattern)p.ChangeType(typeof(long));

            Assert.NotSame(p, q);
            Assert.Equal(typeof(long), q.InputType);
            Assert.Equal(typeof(long), q.NarrowedType);
        }

        [Fact]
        public void DiscardPattern_ChangeType_Triggered()
        {
            var discard = CSharpPattern.Discard();

            var length = typeof(string).GetProperty(nameof(string.Length));
            var property = CSharpPattern.PropertySubpattern(discard, length);

            Assert.NotSame(discard, property.Pattern);
            Assert.Equal(typeof(int), property.Pattern.InputType);
            Assert.Equal(typeof(int), property.Pattern.NarrowedType);
        }

        [Fact]
        public void DiscardPattern_Visitor()
        {
            var p = CSharpPattern.Discard(CSharpPattern.PatternInfo(typeof(int), typeof(int)));

            var visitor = new V();

            visitor.VisitPattern(p);

            Assert.True(visitor.Visited);
        }

        [Fact]
        public void DiscardPattern_Reduce()
        {
            var res = CSharpPattern.Discard(CSharpPattern.PatternInfo(typeof(int), typeof(int)));

            AssertCompile((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.Constant(42));
                return res.Reduce(obj);
            }, new LogAndResult<bool> { Value = true, Log = { "O" } });
        }

        [Fact]
        public void DiscardPattern_Reduce_IncompatibleType()
        {
            var res = CSharpPattern.Discard(typeof(int));

            AssertEx.Throws<ArgumentException>(() => res.Reduce(Expression.Default(typeof(long))));
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<bool> expected)
        {
            var res = WithLog<bool>(createExpression).Compile()();
            Assert.Equal(expected, res);
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
