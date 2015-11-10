// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class RethrowRewriterTests
    {
        [TestMethod]
        public void RethrowRewriter_Basics()
        {
            var t1 = Expression.Rethrow();
            var s1 = Expression.Empty();
            var r1 = RethrowRewriter.Rewrite(t1, s1);
            Assert.AreSame(s1, r1);

            var t2 = Expression.TryCatch(Expression.Empty(), Expression.Catch(typeof(Exception), t1));
            var s2 = Expression.Empty();
            var r2 = RethrowRewriter.Rewrite(t2, s2);
            Assert.AreSame(t2, r2);

            var t3 = Expression.Block(Expression.Negate(Expression.Constant(1)), t1, Expression.Empty());
            var s3 = Expression.Empty();
            var r3 = (BlockExpression)RethrowRewriter.Rewrite(t3, s3);
            Assert.AreSame(t3.Expressions[0], r3.Expressions[0]);
            Assert.AreSame(s3, r3.Expressions[1]);
            Assert.AreSame(t3.Expressions[2], r3.Expressions[2]);

            var t4 = Expression.Throw(Expression.Constant(new Exception()));
            var s4 = Expression.Empty();
            var r4 = RethrowRewriter.Rewrite(t4, s4);
            Assert.AreSame(t4, r4);
        }
    }
}
