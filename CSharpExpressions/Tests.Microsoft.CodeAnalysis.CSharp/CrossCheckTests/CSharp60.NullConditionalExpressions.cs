// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
        public void CrossCheck_ConditionalAccess_Member1()
        {
            var f = Compile<Func<string, int?>>("s => Return(s)?.Length");
            f("bar");
            f(null);
        }

        [TestMethod]
        public void CrossCheck_ConditionalAccess_Member2()
        {
            var f = Compile<Func<TimeSpan?, int?>>("t => Return(t)?.Seconds");
            f(TimeSpan.Zero);
            f(new TimeSpan(2, 3, 5));
            f(null);
        }

        [TestMethod]
        public void CrossCheck_ConditionalAccess_Call1()
        {
            var f = Compile<Func<string, string>>("s => Return(s)?.Substring(Return(1), Return(2))");
            f("bar");
            f(null);
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f(""));
        }

        [TestMethod]
        public void CrossCheck_ConditionalAccess_Call2()
        {
            var f = Compile<Func<TimeSpan?, TimeSpan?>>("t => Return(t)?.Add(new TimeSpan(Return(1), Return(2), Return(3)))");
            f(TimeSpan.Zero);
            f(new TimeSpan(2, 3, 5));
            f(null);
        }

        [TestMethod]
        public void CrossCheck_ConditionalAccess_Index1()
        {
            var f = Compile<Func<string, char?>>("s => Return(s)?[Return(1)]");
            f("bar");
            f(null);
        }

        [TestMethod]
        public void CrossCheck_ConditionalAccess_Index2()
        {
            var f = Compile<Func<int[], int?>>("xs => Return(xs)?[Return(1)]");
            f(new[] { 2, 3, 5 });
            f(null);
            AssertEx.Throws<IndexOutOfRangeException>(() => f(new int[0]));
        }

        [TestMethod]
        public void CrossCheck_ConditionalAccess_Index3()
        {
            var f = Compile<Func<List<int>, int?>>("xs => Return(xs)?[Return(1)]");
            f(new List<int> { 2, 3, 5 });
            f(null);
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f(new List<int>()));
        }

        [TestMethod]
        public void CrossCheck_ConditionalAccess_Invoke()
        {
            var f = Compile<Func<Func<int, int>, int?>>("f => Return(f)?.Invoke(Return(1))");
            f(x => x * 2);
            f(null);
        }

        [TestMethod]
        public void CrossCheck_ConditionalAccess_Many()
        {
            var i = 1;

            foreach (var series in new[]
            {
                new[]
                {
                    ".Bar",
                    "[Return(1)]",
                    ".Foo(Return(2))",
                },

                new[]
                {
                    ".Bar[Return(1)]",
                    ".Bar.Foo(Return(2))",
                    "[Return(1)].Bar",
                    "[Return(1)].Foo(Return(2))",
                    ".Foo(Return(2)).Bar",
                    ".Foo(Return(2))[Return(1)]",
                },

                new[]
                {
                    ".Bar[Return(1)].Foo(Return(2))",
                    ".Bar.Foo(Return(2))[Return(1)]",
                    "[Return(1)].Bar.Foo(Return(2))",
                    "[Return(1)].Foo(Return(2)).Bar",
                    ".Foo(Return(2)).Bar[Return(1)]",
                    ".Foo(Return(2))[Return(1)].Bar",
                },
            })
            {
                foreach (var ifNotNull in series)
                {
                    var f = Compile<Func<Conditional, Conditional>>($"c => c?{ifNotNull}");
                    f(null);
                    f(new Conditional(i));

                    if (i > 1)
                    {
                        AssertEx.Throws<NullReferenceException>(() => f(new Conditional(i - 2)));
                    }
                }

                i++;
            }
        }

        [TestMethod]
        public void CrossCheck_ConditionalAccess_Loop()
        {
            var f = Compile<Action<string[]>>(@"ss =>
{
    foreach (var s in ss)
    {
        Log(s?.Length);
    }
}");
            f(new[] { "bar", null, "foobar", "qux" });
        }
    }
}
