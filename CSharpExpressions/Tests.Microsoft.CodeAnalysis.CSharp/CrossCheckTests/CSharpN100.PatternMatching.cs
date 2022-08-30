// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Xunit;
using System;
using System.Runtime.CompilerServices;

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
        public void CrossCheck_IsExpression_Property_Nested()
        {
            var f = Compile<Func<DateTimeOffset, bool>>("d => d is { Date.Year: 2021 }");
            f(new DateTimeOffset(new DateTime(1983, 2, 11)));
            f(new DateTimeOffset(new DateTime(2021, 12, 17)));
        }

        [Fact]
        public void CrossCheck_IsExpression_Property_Nested_Deep()
        {
            var f = Compile<Func<StrongBox<DateTimeOffset>, bool>>("b => b is { Value.Date.Year: 2021 }");
            f(new StrongBox<DateTimeOffset>(new DateTimeOffset(new DateTime(1983, 2, 11))));
            f(new StrongBox<DateTimeOffset>(new DateTimeOffset(new DateTime(2021, 12, 17))));
        }

        [Fact]
        public void CrossCheck_IsExpression_Property_Nested_Nullable()
        {
            var f = Compile<Func<StrongBox<DateTimeOffset?>, bool>>("b => b is { Value.Date.Year: 2021 }");
            f(new StrongBox<DateTimeOffset?>());
            f(new StrongBox<DateTimeOffset?>(new DateTimeOffset(new DateTime(1983, 2, 11))));
            f(new StrongBox<DateTimeOffset?>(new DateTimeOffset(new DateTime(2021, 12, 17))));
        }

        [Fact]
        public void CrossCheck_IsExpression_Property_Nested_Tuple()
        {
            var f = Compile<Func<int, bool>>("y => { var t = (x: 0, yz: (y: y, z: false)); return t is { yz.y: 3 }; }");
            f(1);
            f(2);
            f(3);
        }
    }
}
