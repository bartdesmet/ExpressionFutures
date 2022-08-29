// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.CSharp.Expressions;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xunit;
using static Tests.TestHelpers;

namespace Tests
{
    public class TypePatternTests
    {
        [Fact]
        public void TypePattern_ArgumentChecking()
        {
            // null checks
            AssertEx.Throws<ArgumentNullException>(() => CSharpPattern.Type(type: null));
            AssertEx.Throws<ArgumentNullException>(() => CSharpPattern.Type(CSharpPattern.PatternInfo(typeof(object), typeof(object)), type: null));

            // invalid type
            AssertEx.Throws<ArgumentException>(() => CSharpPattern.Type(typeof(void)));
            AssertEx.Throws<ArgumentException>(() => CSharpPattern.Type(typeof(int).MakeByRefType()));
            AssertEx.Throws<ArgumentException>(() => CSharpPattern.Type(typeof(int).MakePointerType()));

            // incompatible types
            AssertEx.Throws<ArgumentException>(() => CSharpPattern.Type(CSharpPattern.PatternInfo(typeof(object), typeof(int)), typeof(long)));
        }

        [Fact]
        public void TypePattern_Properties()
        {
            var p = CSharpPattern.Type(CSharpPattern.PatternInfo(typeof(object), typeof(int)), typeof(int));

            Assert.Equal(typeof(object), p.InputType);
            Assert.Equal(typeof(int), p.NarrowedType);
        }

        [Fact]
        public void TypePattern_Properties_Object()
        {
            var p = CSharpPattern.Type(typeof(int));

            Assert.Equal(typeof(object), p.InputType);
            Assert.Equal(typeof(int), p.NarrowedType);
        }

        [Fact]
        public void TypePattern_ChangeType()
        {
            var p = CSharpPattern.Type(CSharpPattern.PatternInfo(typeof(object), typeof(int)), typeof(int));

            Assert.Same(p, p.ChangeType(typeof(object)));

            var q = (TypeCSharpPattern)p.ChangeType(typeof(ValueType));

            Assert.NotSame(p, q);
            Assert.Equal(typeof(ValueType), q.InputType);
            Assert.Equal(typeof(int), q.NarrowedType);
        }

        [Fact]
        public void TypePattern_ChangeType_Triggered()
        {
            var p = CSharpPattern.Type(typeof(int));

            Assert.Equal(typeof(object), p.InputType);
            Assert.Equal(typeof(int), p.NarrowedType);

            var value = typeof(StrongBox<IFormattable>).GetField(nameof(StrongBox<IFormattable>.Value));
            var property = CSharpPattern.PropertySubpattern(p, value);

            Assert.NotSame(p, property.Pattern);
            Assert.Equal(typeof(IFormattable), property.Pattern.InputType);
            Assert.Equal(typeof(int), property.Pattern.NarrowedType);
        }

        [Fact]
        public void TypePattern_Visitor()
        {
            var p = CSharpPattern.Type(CSharpPattern.PatternInfo(typeof(object), typeof(int)), typeof(int));

            var visitor = new V();

            visitor.VisitPattern(p);

            Assert.True(visitor.Visited);
        }

        [Fact]
        public void TypePattern_Reduce()
        {
            var res = CSharpPattern.Type(CSharpPattern.PatternInfo(typeof(object), typeof(int)), typeof(int));

            AssertCompile((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.Constant(42, typeof(object)));
                return res.Reduce(obj);
            }, new LogAndResult<bool> { Value = true, Log = { "O" } });

            AssertCompile((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.Constant(42L, typeof(object)));
                return res.Reduce(obj);
            }, new LogAndResult<bool> { Value = false, Log = { "O" } });

            AssertCompile((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.Constant(null, typeof(object)));
                return res.Reduce(obj);
            }, new LogAndResult<bool> { Value = false, Log = { "O" } });
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<bool> expected)
        {
            var res = WithLog<bool>(createExpression).Compile()();
            Assert.Equal(expected, res);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override CSharpPattern VisitTypePattern(TypeCSharpPattern node)
            {
                Visited = true;

                return base.VisitTypePattern(node);
            }
        }
    }
}
