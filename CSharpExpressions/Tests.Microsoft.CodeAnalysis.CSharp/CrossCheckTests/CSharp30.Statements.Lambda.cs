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
        // TODO: add tests for quoted lambdas in statement bodies

        [TestMethod]
        public void CrossCheck_Lambda_Nested()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""begin"");
    
    Action a = () => Log(""a"");
    Action<int> b = x => Log($""b({x})"");
    
    Log(""middle"");

    a();
    b(42);

    Log(""end"");
}
");
            f();
        }

        [TestMethod]
        public void CrossCheck_Lambda_Nested_Closure()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""begin"");
    
    int x = 0;
    Func<int> @get = () => x;
    Action<int> @set = value => x = value;
    
    Log(""middle"");

    @set(Return(42));
    Log(@get());

    Log(""end"");
}
");
            f();
        }

        // TODO: Assert iterator bodies are not supported.
    }
}
