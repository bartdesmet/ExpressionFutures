// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.Expressions.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class ShallowVisitorTests
    {
        [TestMethod]
        public void ShallowVisitor_Basics()
        {
            var v = new V();

            var c = Expression.Constant(42);
            var f = Expression.Lambda(c);
            var a = CSharpExpression.AsyncLambda(c);

            Assert.AreNotSame(c, v.Visit(c));
            Assert.AreSame(f, v.Visit(f));
            Assert.AreSame(a, v.Visit(a));
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
