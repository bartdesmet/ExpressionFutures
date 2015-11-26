// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class ParameterSubstitutorTests
    {
        [TestMethod]
        public void ParameterSubstitutor_Basics()
        {
            var x = Expression.Parameter(typeof(int));
            var y = Expression.Parameter(typeof(int));

            var e1 = Expression.Add(x, Expression.Constant(1));
            var r1 = (BinaryExpression)ParameterSubstitutor.Substitute(e1, x, y);
            Assert.AreSame(y, r1.Left);

            var e2 = Expression.Block(new[] { x }, x);
            var r2 = (BlockExpression)ParameterSubstitutor.Substitute(e2, x, y);
            Assert.AreSame(x, r2.Expressions[0]);

            var e3 = Expression.Block(Expression.Add(x, Expression.Constant(3)));
            var r3 = (BinaryExpression)((BlockExpression)ParameterSubstitutor.Substitute(e3, x, y)).Expressions[0];
            Assert.AreSame(y, r3.Left);
        }
    }
}
