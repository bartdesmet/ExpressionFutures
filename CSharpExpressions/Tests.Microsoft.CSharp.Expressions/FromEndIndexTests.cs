// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.CSharp.Expressions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    public class FromEndIndexTests
    {
        [Fact]
        public void FromEndIndex_Factory_ArgumentChecking()
        {
            // null checks
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.FromEndIndex(operand: null));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.FromEndIndex(operand: null, method: null, type: null));

            // invalid operand type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(42L)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(42L), method: null, type: null));

            // invalid type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0, typeof(int)), method: null, typeof(int)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0, typeof(int)), method: null, typeof(Index?)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0, typeof(int?)), method: null, typeof(Index)));

            // invalid method
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.Generic)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.DoesNotReturnIndex)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.NotStatic)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.NoArgs)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.OneArgInvalid)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.TwoArgsInvalid1)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.TwoArgsInvalid2)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.TooManyArgs)), typeof(Index)));
        }

        [Fact]
        public void FromEndIndex_Update()
        {
            var i = Expression.Constant(1);
            var e1 = CSharpExpression.FromEndIndex(i);

            var e2 = e1.Update(i);
            Assert.Same(e1, e2);

            var j = Expression.Constant(2);

            var e3 = e1.Update(j);
            Assert.NotSame(e1, e3);
            Assert.Same(j, e3.Operand);
        }

        [Fact]
        public void FromEndIndex_Visitor()
        {
            var res = CSharpExpression.FromEndIndex(Expression.Constant(1));

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitFromEndIndex(FromEndIndexCSharpExpression node)
            {
                Visited = true;

                return base.VisitFromEndIndex(node);
            }
        }

        [Fact]
        public void FromEndIndex_Compile()
        {
            foreach (var e in new Func<Expression, FromEndIndexCSharpExpression>[]
            {
                arg => CSharpExpression.FromEndIndex(arg),
                arg => CSharpExpression.FromEndIndex(arg, method: null, typeof(Index)),
                arg => CSharpExpression.FromEndIndex(arg, method: typeof(Index).GetConstructor(new[] { typeof(int), typeof(bool) }), typeof(Index)),
                arg => CSharpExpression.FromEndIndex(arg, method: typeof(Index).GetMethod(nameof(Index.FromEnd)), typeof(Index)),
            })
            {
                foreach (var i in new (Expression op, Index value)[]
                {
                    (Expression.Default(typeof(int)), Index.FromEnd(0)),
                    (Expression.Constant(0), Index.FromEnd(0)),
                    (Expression.Constant(1), Index.FromEnd(1)),
                })
                {
                    var expr = e(i.op);
                    var f = Expression.Lambda<Func<Index>>(expr).Compile();
                    var res = f();
                    Assert.Equal(i.value, res);
                }
            }
        }

        [Fact]
        public void FromEndIndex_Compile_Lifted()
        {
            foreach (var e in new Func<Expression, FromEndIndexCSharpExpression>[]
            {
                arg => CSharpExpression.FromEndIndex(arg),
                arg => CSharpExpression.FromEndIndex(arg, method: null, typeof(Index?)),
                arg => CSharpExpression.FromEndIndex(arg, method: typeof(Index).GetConstructor(new[] { typeof(int), typeof(bool) }), typeof(Index?)),
                arg => CSharpExpression.FromEndIndex(arg, method: typeof(Index).GetMethod(nameof(Index.FromEnd)), typeof(Index?)),
            })
            {
                foreach (var i in new (Expression op, Index? value)[]
                {
                    (Expression.Default(typeof(int?)), null),
                    (Expression.Constant(0, typeof(int?)), Index.FromEnd(0)),
                    (Expression.Constant(1, typeof(int?)), Index.FromEnd(1)),
                })
                {
                    var expr = e(i.op);
                    var f = Expression.Lambda<Func<Index?>>(expr).Compile();
                    var res = f();
                    Assert.Equal(i.value, res);
                }
            }
        }

        class InvalidIndexFactoryMethods
        {
            public void Generic<T>() { }
            public int DoesNotReturnIndex() => 42;
            public Index NotStatic() => new Index(42);
            public static Index NoArgs() => new Index(42);
            public static Index OneArgInvalid(long x) => new Index((int)x);
            public static Index TwoArgsInvalid1(int x, int y) => new Index((int)x);
            public static Index TwoArgsInvalid2(long x, bool b) => new Index((int)x);
            public static Index TooManyArgs(int x, bool b, int y) => new Index(x, b);
        }
    }
}
