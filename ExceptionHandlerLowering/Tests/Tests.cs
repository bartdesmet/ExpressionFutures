// Prototyping support for interpretation of TryExpression with a fault handler or exception filters.
//
// bartde - October 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void LambdaExpressionExtensions_ArgumentChecking()
        {
            AssertEx.Throws<ArgumentNullException>(() => LambdaExpressionExtensions.CompileWithExceptionHandling(default(LambdaExpression)));
            AssertEx.Throws<ArgumentNullException>(() => LambdaExpressionExtensions.CompileWithExceptionHandling(default(Expression<Action>)));
        }
    }
}
