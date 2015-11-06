// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class HelpersTests
    {
        [TestMethod]
        public void Helpers_CreateConstantInt32()
        {
            for (var i = -1023; i <= 1024; i++)
            {
                Assert.AreEqual(i, Helpers.CreateConstantInt32(i).Value);
            }
        }

        [TestMethod]
        public void Helpers_IsVector()
        {
            Assert.IsTrue(typeof(int[]).IsVector());
            Assert.IsTrue(typeof(int).MakeArrayType().IsVector());

            Assert.IsFalse(typeof(int[,]).IsVector());
            Assert.IsFalse(typeof(int).MakeArrayType(1).IsVector());
            Assert.IsFalse(typeof(int).MakeArrayType(2).IsVector());
        }
    }
}
