// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions.Compiler;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    public class TypedLabelRewriterTests
    {
        [Fact]
        public void TypedLabelRewriter_Simple()
        {
            var rw = new TypedLabelRewriter();

            var l = Expression.Label(typeof(int));
            var m = Expression.Label();

            var p = Expression.Parameter(typeof(bool));
            var e =
                Expression.Block(
                    Expression.Goto(m),
                    Expression.Label(m),
                    Expression.Block(
                        Expression.IfThen(
                            p,
                            Expression.Goto(l, Expression.Constant(42))
                        )
                    ),
                    Expression.Label(l, Expression.Constant(0))
                );

            var f1 = Expression.Lambda<Func<bool, int>>(e, p);
            var d1 = f1.Compile();

            var r = rw.Visit(e);

            var f2 = Expression.Lambda<Func<bool, int>>(r, p);
            var d2 = f2.Compile();

            Assert.Equal(d1(false), d2(false));
            Assert.Equal(d1(true), d2(true));
        }

        // TODO: add test for nested cases
        // TODO: add test for branching out of exception handlers etc.
    }
}
