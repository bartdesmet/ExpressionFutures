// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    internal class AssertEx
    {
        internal static void Throws<T>(Action action)
            where T : Exception
        {
            Throws<T>(action, _ => true);
        }

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
                Assert.IsTrue(assert(ex));

                failed = true;
            }

            Assert.IsTrue(failed);
        }
    }
}