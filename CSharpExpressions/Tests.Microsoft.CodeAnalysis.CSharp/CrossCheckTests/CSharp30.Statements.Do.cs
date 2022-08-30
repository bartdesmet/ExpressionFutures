// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Xunit;
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
        [Fact]
        public void CrossCheck_Do1()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = Return(0);

    do
    {
        if (i == 2)
        {
            Log(""continue"");
            i++;
            continue;
        }

        if (i == 5)
        {
            Log(""break"");
            break;
        }

        Log($""body({i})"");

        Return(i++);
    } while (Return(i < 10));

    Log(""After"");
}");
            f();
        }

        [Fact]
        public void CrossCheck_Do2()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = Return(0);

    do
    {
        Log($""body({i})"");

        Return(i++);
    } while (Return(i < 10));

    Log(""After"");
}");
            f();
        }

        [Fact]
        public void CrossCheck_Do_Conversion()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = Return(0);

    do
    {
        Log($""body({i})"");

        Return(i++);
    } while (new Booleany(i < 10));

    Log(""After"");
}");
            f();
        }

        [Fact]
        public void CrossCheck_Do_Locals()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var s = Return(""0"");

    do
    {
        Log($""body({s})"");

        Return(s += s.Length);
    } while (int.TryParse(s, out var x) && Return(x) > 0);

    Log(""After"");
}");
            f();
        }
    }
}
