// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
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
        [TestMethod]
        public void CrossCheck_IsExpression_Relational_Object_Int32()
        {
            foreach (var op in new[] { "<", "<=", ">", ">=" })
            {
                var f = Compile<Func<object, bool>>($"o => o is {op} 42");
                f(null);
                f(41);
                f(42);
                f(43);
                f(42L);
            }
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Relational_NullableInt32_Int32()
        {
            foreach (var op in new[] { "<", "<=", ">", ">=" })
            {
                var f = Compile<Func<int?, bool>>($"x => x is {op} 42");
                f(null);
                f(41);
                f(42);
                f(43);
            }
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Relational_Int32_Int32()
        {
            foreach (var op in new[] { "<", "<=", ">", ">=" })
            {
                var f = Compile<Func<int, bool>>($"x => x is {op} 42");
                f(41);
                f(42);
                f(43);
            }
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Relational_Object_Decimal()
        {
            foreach (var op in new[] { "<", "<=", ">", ">=" })
            {
                var f = Compile<Func<object, bool>>($"x => x is {op} 49.95m");
                f(null);
                f(49.90m);
                f(49.95m);
                f(50.00m);
                f(49.95);
            }
        }

        [TestMethod]
        public void CrossCheck_IsExpression_And()
        {
            var f = Compile<Func<int, bool>>("x => x is >= 0 and < 10");

            for (var i = -2; i < 12; i++)
            {
                f(i);
            }
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Or()
        {
            var f = Compile<Func<DateTime, bool>>("date => date is { Year: 2020, Month: 5, Day: 19 or 20 or 21 }");

            foreach (var year in new[] { 2019, 2020, 2021 })
            {
                foreach (var month in new[] { 4, 5, 6 })
                {
                    foreach (var day in Enumerable.Range(18, 5))
                    {
                        f(new DateTime(year, month, day));
                    }
                }
            }
        }

        [TestMethod]
        public void CrossCheck_IsExpression_AndOr()
        {
            var f = Compile<Func<char, bool>>("c => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or '.' or ','");

            for (var c = (char)0; c < (char)255; c++)
            {
                f(c);
            }
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Not_Null()
        {
            var f = Compile<Func<object, bool>>($"o => o is not null");
            f(null);
            f(42);
            f("bar");
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Not_Int32()
        {
            var f = Compile<Func<object, bool>>($"o => o is not 42");
            f(null);
            f(41);
            f(42);
            f(42L);
            f("bar");
        }
    }
}
