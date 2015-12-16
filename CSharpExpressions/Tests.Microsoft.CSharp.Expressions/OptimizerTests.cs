// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public partial class OptimizerTests
    {
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
