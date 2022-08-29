// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using Xunit;

namespace Tests
{
    internal class AssertEx
    {
        internal static void Throws<T>(Action action, Func<T, bool> assert)
            where T : Exception
        {
            var failed = false;

            try
            {
                action();
            }
            catch (T ex)
            {
                Assert.True(assert(ex));

                failed = true;
            }

            Assert.True(failed);
        }
    }
}