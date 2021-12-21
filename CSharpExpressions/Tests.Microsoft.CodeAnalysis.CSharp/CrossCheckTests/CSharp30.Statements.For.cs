// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    // NB: The tests cross-check the outcome of evaluating a lambda expression - specified as a string in the
    //     test cases - in two ways. First, by converting the lambda expression to a delegate type and running
    //     IL code produced by the compiler. Second, by converting the lambda expression to an expression tree
    //     using the extended expression tree support in our modified Roslyn build and compiling the expression
    //     at runtime (therefore invoking our Reduce methods).
    //
    //     It is assumed that the outcome has proper equality defined (i.e. EqualityComparer<T>.Default should
    //     return a meaningful equality comparer to assert evaluation outcomes against each other). If the
    //     evaluation results in an exception, its type is cross-checked.
    //
    //     In addition to cross-checking the evaluation outcome, a log is maintained and cross-checked, which
    //     is useful to assert the order of side-effects. The code fragments can write to this log by means of
    //     the Log method and the Return method (to prepend returning a value of type T with a logging side-
    //     effect).

    partial class CompilerTests
    {
        [TestMethod]
        public void CrossCheck_For1()
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
        public void CrossCheck_For2()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    for (var i = Return(0); Return(i < 10); Return(i++))
    {
        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_For_Conversion()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    for (var i = Return(0); new Booleany(i < 10); Return(i++))
    {
        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_For_Infinite()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = Return(0);

    for (;;)
    {
        if (i == Return(5))
        {
            Log(""break"");
            break;
        }

        Log($""body({i})"");

        Log(++i);
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_For_Complex()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    for (int i = Return(0), j = Return(10); Return(j >= i); Return(i++), Return(--j))
    {
        Log($""body({i}, {j})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_For_NoDeclarations()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = 0;

    for (Log(""initializer""); Return(i < 10); Log(""iterator""))
    {
        Log($""body({i})"");
        i++;
    }

    Log(""After"");
}");
            f();
        }
    }
}
