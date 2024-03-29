﻿// Prototyping extended expression trees for C#.
//
// bartde - May 2020

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
        public void CrossCheck_TupleLiteral()
        {
            var f = Compile<Func<int, string, (int, string)>>("(x, s) => (Return(x), Return(s))");
            f(1, null);
            f(1, "bar");
        }

        [Fact]
        public void CrossCheck_TupleLiteral_Named()
        {
            var f = Compile<Func<int, string, (int, string)>>("(x, s) => (x: Return(x), s: Return(s))");
            f(1, null);
            f(1, "bar");
        }

        [Fact]
        public void CrossCheck_TupleLiteral_Constants()
        {
            var f = Compile<Func<(int, string)>>("() => (1, \"foo\")");
            f();
        }

        [Fact]
        public void CrossCheck_TupleLiteral_Nested()
        {
            var f = Compile<Func<int, string, bool, (int, (string, bool))>>("(x, s, b) => (Return(x), (Return(s), Return(b)))");
            f(1, null, true);
            f(1, "bar", false);
        }

        [Fact]
        public void CrossCheck_TupleLiteral_Many()
        {
            var f = Compile<Func<(int, int, int, int, int, int, int, int, int, int)>>("() => (Return(0), Return(1), Return(2), Return(3), Return(4), Return(5), Return(6), Return(7), Return(8), Return(9))");
            f();
        }

        [Fact]
        public void CrossCheck_TupleLiteral_Null()
        {
            var f1 = Compile<Func<(int, string)>>("() => (0, null)");
            f1();

            var f2 = Compile<Func<(int, string)>>("() => ((int, string))(0, null)");
            f2();
        }
    }
}
