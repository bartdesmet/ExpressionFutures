// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    public class MemberInitializerTests
    {
        [Fact]
        public void MemberInitializer_Factory_ArgumentChecking()
        {
            // null checks
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.MemberInitializer(member: null, Expression.Constant(1)));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.MemberInitializer(typeof(Foo).GetField(nameof(Foo.Bar)), expression: null));

            // invalid member type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.MemberInitializer(typeof(Foo).GetConstructor(Type.EmptyTypes), Expression.Constant(1)));

            // static field or property
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.MemberInitializer(typeof(Foo).GetField(nameof(Foo.StaticBar)), Expression.Constant(1)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.MemberInitializer(typeof(Foo).GetProperty(nameof(Foo.StaticQux)), Expression.Constant(1)));

            // indexer
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.MemberInitializer(typeof(List<int>).GetProperty("Item"), Expression.Constant(1)));

            // incompatible types
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.MemberInitializer(typeof(Foo).GetProperty(nameof(Foo.Bar)), Expression.Constant(42L)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.MemberInitializer(typeof(Foo).GetProperty(nameof(Foo.Qux)), Expression.Constant("bar")));
        }

        [Fact]
        public void MemberInitializer_Update()
        {
            var i = Expression.Constant(1);
            var e1 = CSharpExpression.MemberInitializer(typeof(Foo).GetProperty(nameof(Foo.Qux)), i);

            var e2 = e1.Update(i);
            Assert.Same(e1, e2);

            var j = Expression.Constant(2);

            var e3 = e1.Update(j);
            Assert.NotSame(e1, e3);
            Assert.Same(j, e3.Expression);
        }

        [Fact]
        public void MemberInitializer_Visitor()
        {
            var i = Expression.Constant(1);
            var res = CSharpExpression.MemberInitializer(typeof(Foo).GetProperty(nameof(Foo.Qux)), i);

            var v = new V();
            Assert.Same(res, v.VisitMemberInitializer(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override MemberInitializer VisitMemberInitializer(MemberInitializer node)
            {
                Visited = true;

                return base.VisitMemberInitializer(node);
            }
        }

#pragma warning disable CS0649
        class Foo
        {
            public int Bar;
            public int Qux { get; set; }

            public static int StaticBar;
            public static int StaticQux { get; set; }
        }
#pragma warning restore CS0649
    }
}
