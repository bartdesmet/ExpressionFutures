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
        public void CrossCheck_TupleEquality()
        {
            var eq = Compile<Func<(int, int), (int, int), bool>>("(t1, t2) => Return(t1) == Return(t2)");
            eq((1, 2), (1, 2));
            eq((1, 2), (-1, 2));
            eq((1, 2), (1, -2));

            var ne = Compile<Func<(int, int), (int, int), bool>>("(t1, t2) => Return(t1) != Return(t2)");
            ne((1, 2), (1, 2));
            ne((1, 2), (-1, 2));
            ne((1, 2), (1, -2));
        }

        [TestMethod]
        public void CrossCheck_TupleEquality_Lifted()
        {
            var eq = Compile<Func<(int, int)?, (int, int)?, bool>>("(t1, t2) => Return(t1) == Return(t2)");

            eq(null, null);
            eq((1, 2), null);
            eq(null, (1, 2));

            eq((1, 2), (1, 2));
            eq((1, 2), (-1, 2));
            eq((1, 2), (1, -2));

            var ne = Compile<Func<(int, int)?, (int, int)?, bool>>("(t1, t2) => Return(t1) != Return(t2)");

            ne(null, null);
            ne((1, 2), null);
            ne(null, (1, 2));

            ne((1, 2), (1, 2));
            ne((1, 2), (-1, 2));
            ne((1, 2), (1, -2));
        }

        [TestMethod]
        public void CrossCheck_TupleEquality_Literal()
        {
            Compile<Func<bool>>("() => (Return(1, \"L1\"), Return(2, \"L2\")) == (Return(1, \"R1\"), Return(2, \"R2\"))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), Return(2, \"L2\")) == (Return(-1, \"R1\"), Return(2, \"R2\"))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), Return(2, \"L2\")) == (Return(-1, \"R1\"), Return(-2, \"R2\"))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), Return(2, \"L2\")) != (Return(1, \"R1\"), Return(2, \"R2\"))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), Return(2, \"L2\")) != (Return(-1, \"R1\"), Return(2, \"R2\"))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), Return(2, \"L2\")) != (Return(1, \"R1\"), Return(-2, \"R2\"))")();

            var eq1 = Compile<Func<(int, int), bool>>("t1 => Return(t1) == (Return(1), Return(2))");
            eq1((1, 2));
            eq1((-1, 2));
            eq1((1, -2));

            var eq2 = Compile<Func<(int, int), bool>>("t2 => (Return(1), Return(2)) == Return(t2)");
            eq2((1, 2));
            eq2((-1, 2));
            eq2((1, -2));

            var ne1 = Compile<Func<(int, int), bool>>("t1 => Return(t1) != (Return(1), Return(2))");
            ne1((1, 2));
            ne1((-1, 2));
            ne1((1, -2));

            var ne2 = Compile<Func<(int, int), bool>>("t2 => (Return(1), Return(2)) != Return(t2)");
            ne2((1, 2));
            ne2((-1, 2));
            ne2((1, -2));
        }

        [TestMethod]
        public void CrossCheck_TupleEquality_Null()
        {
            Compile<Func<bool>>("() => (1, null, 2L) == (1, \"\", 2L)")();
            Compile<Func<bool>>("() => (1, null, 2L) != (1, \"\", 2L)")();
        }

        [TestMethod]
        [Ignore] // BUG: These literals have no type, causing the ExpressionLambdaRewriter to assert.
        public void CrossCheck_TupleEquality_NullNull()
        {
            Compile<Func<bool>>("() => (1, null, 2L) == (1, null, 2L)")();
            Compile<Func<bool>>("() => (1, null, 2L) != (1, null, 2L)")();
        }
    }
}
