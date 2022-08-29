// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.Expressions.Compiler;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    public class ShallowVisitorTests
    {
        [Fact]
        public void ShallowVisitor_Basics()
        {
            var v = new V();

            var c = Expression.Constant(42);
            var f = Expression.Lambda(c);
            var a = CSharpExpression.AsyncLambda(c);

            Assert.NotSame(c, v.Visit(c));
            Assert.Same(f, v.Visit(f));
            Assert.Same(a, v.Visit(a));
        }

        class V : ShallowVisitor
        {
            protected override Expression VisitConstant(ConstantExpression node)
            {
                return Expression.Default(node.Type);
            }
        }
    }
}
