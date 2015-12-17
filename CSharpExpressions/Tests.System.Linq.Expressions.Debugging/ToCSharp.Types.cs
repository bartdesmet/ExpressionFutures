// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class ToCSharpTypeTests
    {
        [TestMethod]
        public void ToCSharpType_Null()
        {
            Assert.IsNull(default(Type).ToCSharp());
        }

        [TestMethod]
        public void ToCSharpType_Simple()
        {
            var cases = new Dictionary<Type, string>
            {
                { typeof(int), "int" },
                { typeof(int?), "int?" },
                { typeof(int[]), "int[]" },
                { typeof(int[,]), "int[,]" },
                { typeof(List<>), "List<>" },
                { typeof(List<>).GetGenericArguments()[0], "T" },
                { typeof(Dictionary<,>), "Dictionary<,>" },
                { typeof(List<int>), "List<int>" },
            };

            foreach (var kv in cases)
            {
                Assert.AreEqual(kv.Value, kv.Key.ToCSharp());
                Assert.AreEqual(kv.Value, kv.Key.ToCSharp("System", "System.Collections.Generic"));
            }
        }

        [TestMethod]
        public void ToCSharpType_CustomNamespace()
        {
            var cases = new Dictionary<Type, string>
            {
                { typeof(List<>), "System.Collections.Generic.List<>" },
            };

            foreach (var kv in cases)
            {
                Assert.AreEqual(kv.Value, kv.Key.ToCSharp(new string[0]));
                Assert.AreEqual(kv.Value, kv.Key.ToCSharp(default(string[])));
                Assert.AreEqual(kv.Value, kv.Key.ToCSharp("System"));
            }
        }
    }
}
