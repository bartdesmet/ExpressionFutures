// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace Tests
{
    partial class OptimizerTests
    {
		[TestMethod]
		public void Optimizer_0()
		{
			var expression = Expression.Block(Expression.Empty());
			var expected = Expression.Empty();
			AssertOptimize(expression, expected);
		}

    }
}
