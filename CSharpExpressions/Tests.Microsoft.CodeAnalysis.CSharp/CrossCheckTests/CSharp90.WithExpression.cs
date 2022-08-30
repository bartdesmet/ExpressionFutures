// Prototyping extended expression trees for C#.
//
// bartde - December 2021

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
        public void CrossCheck_WithExpression_Record1()
        {
            var f = Compile<Func<string>>(@"() => {
                var q =
                    Return(new Person { Name = ""Bart"", Age = 8 }) with
                    {
                        Name = Return(""Homer""),
                        Age = Return(21)
                    };
                return q.ToString();
            }");

            f();
        }

        [Fact]
        public void CrossCheck_WithExpression_Record2()
        {
            var f = Compile<Func<string>>(@"() => {
                var q =
                    Return(new Person { Name = ""Bart"", Age = 8 }) with
                    {
                        Name = Return(""Homer"")
                    };
                return q.ToString();
            }");

            f();
        }

        [Fact]
        public void CrossCheck_WithExpression_Record3()
        {
            var f = Compile<Func<string>>(@"() => {
                var q =
                    Return(new Person { Name = ""Bart"", Age = 8 }) with
                    {
                        Age = Return(21)
                    };
                return q.ToString();
            }");

            f();
        }

        [Fact]
        public void CrossCheck_WithExpression_Struct1()
        {
            var f = Compile<Func<string>>(@"() => {
                var q =
                    Return(new Point { X = 1, Y = 2 }) with
                    {
                        Y = Return(4),
                        X = Return(3)
                    };
                return $""{q.X}, {q.Y}"";
            }");

            f();
        }

        [Fact]
        public void CrossCheck_WithExpression_Struct2()
        {
            var f = Compile<Func<string>>(@"() => {
                var q =
                    Return(new Point { X = 1, Y = 2 }) with
                    {
                        Y = Return(4)
                    };
                return $""{q.X}, {q.Y}"";
            }");

            f();
        }

        [Fact]
        public void CrossCheck_WithExpression_Struct3()
        {
            var f = Compile<Func<string>>(@"() => {
                var q =
                    Return(new Point { X = 1, Y = 2 }) with
                    {
                        X = Return(3)
                    };
                return $""{q.X}, {q.Y}"";
            }");

            f();
        }

        [Fact]
        public void CrossCheck_WithExpression_Anonymous1()
        {
            var f = Compile<Func<string>>(@"() => {
                var q =
                    Return(new { x = 1, y = 2 }) with
                    {
                        y = Return(4),
                        x = Return(3)
                    };
                return ""{q.x}, {q.y}"";
            }");

            f();
        }

        [Fact]
        public void CrossCheck_WithExpression_Anonymous2()
        {
            var f = Compile<Func<string>>(@"() => {
                var q =
                    Return(new { x = 1, y = 2 }) with
                    {
                        y = Return(4)
                    };
                return ""{q.x}, {q.y}"";
            }");

            f();
        }

        [Fact]
        public void CrossCheck_WithExpression_Anonymous3()
        {
            var f = Compile<Func<string>>(@"() => {
                var q =
                    Return(new { x = 1, y = 2 }) with
                    {
                        x = Return(3)
                    };
                return ""{q.x}, {q.y}"";
            }");

            f();
        }
    }
}
