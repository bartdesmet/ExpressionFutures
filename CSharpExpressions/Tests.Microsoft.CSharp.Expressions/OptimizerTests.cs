// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public partial class OptimizerTests
    {
        private static readonly Expression CW = Expression.Call(typeof(Console).GetMethod("WriteLine", new Type[0]));
        private static readonly Func<int, Expression> CWI = i => Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(int) }), Expression.Constant(i));
        private static readonly ParameterExpression P1 = Expression.Parameter(typeof(int), "x");
        private static readonly ParameterExpression P2 = Expression.Parameter(typeof(int), "y");
        private static readonly LabelTarget RET = Expression.Label(typeof(void), "return");

        [TestMethod]
        public void Optimizer_Null()
        {
            AssertOptimize(null, null);
        }

        private void AssertOptimize(Expression expression, Expression expected)
        {
            var actual = expression.Optimize();

            var actualView = actual.DebugView();
            var expectedView = expected.DebugView();

            var actualStr = actualView?.ToString();
            var expectedStr = expectedView?.ToString();

            Assert.AreEqual(expectedStr, actualStr);
        }
    }
}
