// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    public class ToCSharpTypeTests
    {
        [Fact]
        public void ToCSharpType_Null()
        {
            Assert.Null(default(Type).ToCSharp());
        }

        [Fact]
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
                Assert.Equal(kv.Value, kv.Key.ToCSharp());
                Assert.Equal(kv.Value, kv.Key.ToCSharp("System", "System.Collections.Generic"));
            }
        }

        [Fact]
        public void ToCSharpType_CustomNamespace()
        {
            var cases = new Dictionary<Type, string>
            {
                { typeof(List<>), "System.Collections.Generic.List<>" },
            };

            foreach (var kv in cases)
            {
                Assert.Equal(kv.Value, kv.Key.ToCSharp(new string[0]));
                Assert.Equal(kv.Value, kv.Key.ToCSharp(default(string[])));
                Assert.Equal(kv.Value, kv.Key.ToCSharp("System"));
            }
        }
    }
}
