// Prototyping extended expression trees for C#.
//
// bartde - January 2022

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests
    {
        [TestMethod]
        public void ExpressionTreeWithClosure()
        {
            var code = @"
using System;
using System.Linq.Expressions;

public static class Test
{
    public static int Run()
    {
        int num = 0;
        Expression<Action> e = () => num++;
        e.Compile()();
        return num;
    }
}
";

            var asm = TestUtilities.Compile(code, out _);
            var res = (int)asm.GetType("Test").GetMethod("Run").Invoke(null, new object[0]);
            Assert.AreEqual(1, res);
        }
    }
}
