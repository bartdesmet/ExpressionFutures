// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Xunit;

namespace Tests
{
    public class HelpersTests
    {
        [Fact]
        public void Helpers_CreateConstantInt32()
        {
            for (var i = -1023; i <= 1024; i++)
            {
                Assert.Equal(i, Helpers.CreateConstantInt32(i).Value);
            }
        }

        [Fact]
        public void Helpers_IsVector()
        {
            Assert.True(typeof(int[]).IsVector());
            Assert.True(typeof(int).MakeArrayType().IsVector());

            Assert.False(typeof(int[,]).IsVector());
            Assert.False(typeof(int).MakeArrayType(1).IsVector());
            Assert.False(typeof(int).MakeArrayType(2).IsVector());
        }
    }
}
