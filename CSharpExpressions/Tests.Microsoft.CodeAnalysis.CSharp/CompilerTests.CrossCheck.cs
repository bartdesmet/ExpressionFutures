// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests
    {
        [TestMethod]
        public void CrossCheck_Arithmetic()
        {
            var f = Compile<Func<int>>("() => Return(1) + Return(2)");
            f();
        }

        [TestMethod]
        public void CrossCheck_NullConditional()
        {
            var f = Compile<Func<string, int?>>("s => s?.Length");
            f("bar");
            f(null);
        }

        [TestMethod]
        public void CrossCheck_NamedParameters()
        {
            var f = Compile<Func<int>>(@"() =>
{
    var b = new StrongBox<int>(1);
    Log(b.Value);
    return System.Threading.Interlocked.Exchange(value: Return(42), location1: ref b.Value);
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_ForEach()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    for (var i = Return(0); Return(i < 10); Return(i++))
    {
        if (i == 2)
        {
            Log(""continue"");
            continue;
        }

        if (i == 5)
        {
            Log(""break"");
            break;
        }

        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment()
        {
            var f = Compile<Func<int, int>>(@"i =>
{
    var b = new StrongBox<int>(i);
    Log(b.Value);
    var res = b.Value += Return(1);
    Log(res);
    return b.Value;
}");
            f(0);
            f(41);
        }

        [TestMethod]
        [Ignore] // See https://github.com/dotnet/corefx/issues/4984; we may have to fix this with C#-specific nodes
        public void CrossCheck_CompoundAssignment_Issue()
        {
            var f = Compile<Func<int, int>>(@"i =>
{
    var b = new WeakBox<int>();
    Log(b.Value);
    var res = b.Value += Return(1);
    Log(res);
    return b.Value;
}");
            f(0);
            f(41);
        }
    }
}
