// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        [TestMethod]
        public void CrossCheck_IsExpression_Positional_Tuple()
        {
            var f = Compile<Func<(int, string), bool>>("t => t is (1, \"bar\")");
            f((0, ""));
            f((0, "bar"));
            f((1, ""));
            f((1, "bar"));
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Positional_Tuple_Named()
        {
            var f = Compile<Func<(int x, string s), bool>>("((int x, string s) t) => t is (x: 1, s: \"bar\")");
            f((x: 0, s: ""));
            f((x: 0, s: "bar"));
            f((x: 1, s: ""));
            f((x: 1, s: "bar"));
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Positional_Tuple_NullableComponents()
        {
            var f = Compile<Func<(int?, long?), bool>>("t => t is (42, null)");
            f((42, null));
            f((42, 123));
            f((null, 123));
            f((null, null));
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Positional_Tuple_Nested()
        {
            var f = Compile<Func<(int, ((long, char), bool)), bool>>("t => t is (42, ((123L, 'a'), true))");
            f((41, ((123L, 'a'), true)));
            f((42, ((124L, 'a'), true)));
            f((42, ((123L, 'b'), true)));
            f((42, ((123L, 'a'), false)));
            f((42, ((123L, 'a'), true)));
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Positional_Tuple_HighArity()
        {
            var f = Compile<Func<(int, string, char, bool, double, ConsoleColor, byte, long, uint), bool>>("t => t is (1, \"bar\", 'a', true, Math.PI, ConsoleColor.Red, (byte)255, 42L, (uint)123)");
            f((0, "bar", 'a', true, Math.PI, ConsoleColor.Red, (byte)255, 42L, (uint)123));
            f((1, "foo", 'a', true, Math.PI, ConsoleColor.Red, (byte)255, 42L, (uint)123));
            f((1, "bar", 'b', true, Math.PI, ConsoleColor.Red, (byte)255, 42L, (uint)123));
            f((1, "bar", 'a', false, Math.PI, ConsoleColor.Red, (byte)255, 42L, (uint)123));
            f((1, "bar", 'a', true, Math.E, ConsoleColor.Red, (byte)255, 42L, (uint)123));
            f((1, "bar", 'a', true, Math.PI, ConsoleColor.Green, (byte)255, 42L, (uint)123));
            f((1, "bar", 'a', true, Math.PI, ConsoleColor.Red, (byte)254, 42L, (uint)123));
            f((1, "bar", 'a', true, Math.PI, ConsoleColor.Red, (byte)255, 41L, (uint)123));
            f((1, "bar", 'a', true, Math.PI, ConsoleColor.Red, (byte)255, 42L, (uint)124));
            f((1, "bar", 'a', true, Math.PI, ConsoleColor.Red, (byte)255, 42L, (uint)123));
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Positional_Deconstruct_Instance1()
        {
            var f = Compile<Func<int, int, bool>>("(x, y) => new Point(Log) { X = x, Y = y } is (1, 2)");
            f(0, 0);
            f(1, 0);
            f(0, 2);
            f(1, 2);
            f(2, 1);
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Positional_Deconstruct_Instance2()
        {
            var f = Compile<Func<int, int, bool>>("(x, y) => new Point(Log) { X = x, Y = y } is (x: 1, y: 2)");
            f(0, 0);
            f(1, 0);
            f(0, 2);
            f(1, 2);
            f(2, 1);
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Positional_Deconstruct_Static1()
        {
            var f = Compile<Func<int, int, bool>>("(x, y) => new Point2D(Log) { X = x, Y = y } is (1, 2)");
            f(0, 0);
            f(1, 0);
            f(0, 2);
            f(1, 2);
            f(2, 1);
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Positional_Deconstruct_Static2()
        {
            var f = Compile<Func<int, int, bool>>("(x, y) => new Point2D(Log) { X = x, Y = y } is (x: 1, y: 2)");
            f(0, 0);
            f(1, 0);
            f(0, 2);
            f(1, 2);
            f(2, 1);
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Property_All()
        {
            var f = Compile<Func<int, int, bool>>("(x, y) => new Point(Log) { X = x, Y = y } is { X: 1, Y: 2 }");
            f(0, 0);
            f(1, 0);
            f(0, 2);
            f(1, 2);
            f(2, 1);
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Property_Reordered()
        {
            var f = Compile<Func<int, int, bool>>("(x, y) => new Point(Log) { X = x, Y = y } is { Y: 2, X: 1 }");
            f(0, 0);
            f(1, 0);
            f(0, 2);
            f(1, 2);
            f(2, 1);
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Property_Some()
        {
            var f = Compile<Func<int, int, bool>>("(x, y) => new Point(Log) { X = x, Y = y } is { Y: 2 }");
            f(0, 0);
            f(1, 0);
            f(0, 2);
            f(1, 2);
            f(2, 1);
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Property_Field()
        {
            var f = Compile<Func<StrongBox<int>, bool>>("b => b is { Value: 42 }");
            f(null);
            f(new StrongBox<int>());
            f(new StrongBox<int>(41));
            f(new StrongBox<int>(42));
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Property_Field_Nullable()
        {
            var f = Compile<Func<StrongBox<int?>, bool>>("b => b is { Value: 42 }");
            f(null);
            f(new StrongBox<int?>());
            f(new StrongBox<int?>(41));
            f(new StrongBox<int?>(42));
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Property_Field_Tuple()
        {
            var f = Compile<Func<(int x, string s), bool>>("((int x, string s) t) => t is { x: 42, s: \"bar\" }");
            f((41, "bar"));
            f((42, null));
            f((42, "bar"));
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Property_NotNullCheck()
        {
            var f = Compile<Func<object, bool>>("o => o is { }");
            f(null);
            f(new object());
            f("foo");
            f(42);
        }

        [TestMethod]
        public void CrossCheck_IsExpression_Property_NotNullCheck_Nullable()
        {
            var f = Compile<Func<int?, bool>>("x => x is { }");
            f(null);
            f(42);
        }
    }
}
