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
        public void CrossCheck_MultidimensionalArrayInit1()
        {
            var f = Compile<Func<string>>(@"() => {
    var xs = new int[1, 1] { { Return(42) } };
    return $""{{ {{ {xs[0, 0]} }} }}"";
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_MultidimensionalArrayInit2()
        {
            var f = Compile<Func<string>>(@"() => {
    var xs = new int[2, 2] { { Return(2), Return(3) }, { Return(5), Return(7) } };
    return $""{{ {{ {xs[0, 0]}, {xs[0, 1]} }}, {{ {xs[1, 0]}, {xs[1, 1]} }} }}"";
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_MultidimensionalArrayInit3()
        {
            var f = Compile<Func<string>>(@"() => {
    var xs = new int[1, 2] { { Return(2), Return(3) } };
    return $""{{ {{ {xs[0, 0]}, {xs[0, 1]} }} }}"";
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_MultidimensionalArrayInit4()
        {
            var f = Compile<Func<string>>(@"() => {
    var xs = new int[2, 1] { { Return(2) }, { Return(3) } };
    return $""{{ {{ {xs[0, 0]} }}, {{ {xs[1, 0]} }} }}"";
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_MultidimensionalArrayInit5()
        {
            var f = Compile<Func<string>>(@"() => {
    var xs = new int[2, 3, 5]
    {
        {
            {
                Return(11), Return(12), Return(13), Return(14), Return(15)
            },
            {
                Return(21), Return(22), Return(23), Return(24), Return(25)
            },
            {
                Return(31), Return(32), Return(33), Return(34), Return(35)
            }
        },
        {
            {
                Return(41), Return(42), Return(43), Return(44), Return(45)
            },
            {
                Return(51), Return(52), Return(53), Return(54), Return(55)
            },
            {
                Return(61), Return(62), Return(63), Return(64), Return(65)
            }
        }
    };

    return $@""
    {{
        {{
            {{
                {xs[0, 0, 0]}, {xs[0, 0, 1]}, {xs[0, 0, 2]}, {xs[0, 0, 3]}, {xs[0, 0, 4]}
            }},
            {{
                {xs[0, 1, 0]}, {xs[0, 1, 1]}, {xs[0, 1, 2]}, {xs[0, 1, 3]}, {xs[0, 1, 4]}
            }},
            {{
                {xs[0, 2, 0]}, {xs[0, 2, 1]}, {xs[0, 2, 2]}, {xs[0, 2, 3]}, {xs[0, 2, 4]}
            }}
        }},
        {{
            {{
                {xs[1, 0, 0]}, {xs[1, 0, 1]}, {xs[1, 0, 2]}, {xs[1, 0, 3]}, {xs[1, 0, 4]}
            }},
            {{
                {xs[1, 1, 0]}, {xs[1, 1, 1]}, {xs[1, 1, 2]}, {xs[1, 1, 3]}, {xs[1, 1, 4]}
            }},
            {{
                {xs[1, 2, 0]}, {xs[1, 2, 1]}, {xs[1, 2, 2]}, {xs[1, 2, 3]}, {xs[1, 2, 4]}
            }}
        }}
    }}"";
}");
            f();
        }
    }
}
