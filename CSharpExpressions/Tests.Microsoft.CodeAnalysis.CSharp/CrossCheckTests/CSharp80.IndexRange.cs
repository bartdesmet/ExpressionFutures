// Prototyping extended expression trees for C#.
//
// bartde - May 2020

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
        public void CrossCheck_Index_ImplicitConversion()
        {
            var f = Compile<Func<int, Index>>("i => i");
            f(1);
        }

        [TestMethod]
        public void CrossCheck_Index_ImplicitConversion_Lifted()
        {
            var f = Compile<Func<int?, Index?>>("i => i");
            f(null);
            f(1);
        }

        [TestMethod]
        public void CrossCheck_Index_FromEnd()
        {
            var f = Compile<Func<int, Index>>("i => ^i");
            f(1);

            var g = Compile<Func<int, Index>>("i => ^Return(i)");
            g(1);
        }

        [TestMethod]
        public void CrossCheck_Index_FromEnd_Lifted()
        {
            var f = Compile<Func<int?, Index?>>("i => ^i");
            f(1);

            var g = Compile<Func<int?, Index?>>("i => ^Return(i)");
            g(1);
        }

        [TestMethod]
        public void CrossCheck_Range()
        {
            Compile<Func<Range>>("() => ..")();

            Compile<Func<Range>>("() => 1..")();
            Compile<Func<Range>>("() => ..2")();

            Compile<Func<Range>>("() => 1..2")();

            Compile<Func<Range>>("() => Return(1)..")();
            Compile<Func<Range>>("() => ..Return(2)")();

            Compile<Func<Range>>("() => Return(1)..Return(2)")();
        }

        [TestMethod]
        public void CrossCheck_Range_Index()
        {
            Compile<Func<Index, Range>>("i => i..")(1);
            Compile<Func<Index, Range>>("i => ..i")(2);

            Compile<Func<Index, Index, Range>>("(i, j) => i..j")(1, 2);

            Compile<Func<Index, Range>>("i => Return(i)..")(1);
            Compile<Func<Index, Range>>("i => ..Return(i)")(2);

            Compile<Func<Index, Index, Range>>("(i, j) => Return(i)..Return(j)")(1, 2);
        }

        [TestMethod]
        public void CrossCheck_Range_Lifted()
        {
            var f1 = Compile<Func<Index?, Range?>>("i => i..");
            f1(null);
            f1(1);

            var f2 = Compile<Func<Index?, Range?>>("i => ..i");
            f2(null);
            f2(2);

            var f3 = Compile<Func<Index?, Index?, Range?>>("(i, j) => i..j");
            f3(null, null);
            f3(null, 2);
            f3(1, null);
            f3(1, 2);

            var f4 = Compile<Func<Index?, Range?>>("i => Return(i)..");
            f4(null);
            f4(1);

            var f5 = Compile<Func<Index?, Range?>>("i => ..Return(i)");
            f5(null);
            f5(2);

            var f6 = Compile<Func<Index?, Index?, Range?>>("(i, j) => Return(i)..Return(j)");
            f6(null, null);
            f6(null, 2);
            f6(1, null);
            f6(1, 2);
        }
    }
}
