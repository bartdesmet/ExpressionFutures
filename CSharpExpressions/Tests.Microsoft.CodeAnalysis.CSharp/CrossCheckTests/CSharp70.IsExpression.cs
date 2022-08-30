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
        public void CrossCheck_IsExpression_Constant_Object_Int32()
        {
            var f = Compile<Func<object, bool>>("o => Return(o) is 42");
            f(null);
            f(1);
            f(42);
            f(42L);
            f("foo");
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_Int32_Int32()
        {
            var f = Compile<Func<int, bool>>("x => Return(x) is 42");
            f(1);
            f(42);
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_NullableInt32_Int32()
        {
            var f = Compile<Func<int?, bool>>("x => Return(x) is 42");
            f(null);
            f(1);
            f(42);
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_Object_Decimal()
        {
            var f = Compile<Func<object, bool>>("o => Return(o) is 42.95m");
            f(null);
            f(1M);
            f(42.95m);
            f(42.95);
            f("foo");
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_Decimal_Decimal()
        {
            var f = Compile<Func<decimal, bool>>("x => Return(x) is 42.95m");
            f(1m);
            f(42.95m);
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_NullableDecimal_Decimal()
        {
            var f = Compile<Func<decimal?, bool>>("x => Return(x) is 42.95m");
            f(null);
            f(1m);
            f(42.95m);
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_Object_Enum()
        {
            var f = Compile<Func<object, bool>>("o => Return(o) is ConsoleColor.Red");
            f(null);
            f(ConsoleColor.Red);
            f(ConsoleColor.Yellow);
            f((int)ConsoleColor.Red);
            f("Red");
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_Enum_Enum()
        {
            var f = Compile<Func<ConsoleColor, bool>>("x => Return(x) is ConsoleColor.Red");
            f(ConsoleColor.Red);
            f(ConsoleColor.Yellow);
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_NullableEnum_Enum()
        {
            var f = Compile<Func<ConsoleColor?, bool>>("x => Return(x) is ConsoleColor.Red");
            f(null);
            f(ConsoleColor.Red);
            f(ConsoleColor.Yellow);
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_Object_String()
        {
            var f = Compile<Func<object, bool>>("o => Return(o) is \"foo\"");
            f(null);
            f("");
            f("foo");
            f(42);
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_String_String()
        {
            var f = Compile<Func<string, bool>>("s => Return(s) is \"foo\"");
            f(null);
            f("");
            f("foo");
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_Null_Object()
        {
            var f = Compile<Func<object, bool>>("o => Return(o) is null");
            f(null);
            f(42);
            f("foo");
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_Null_NullableInt32()
        {
            var f = Compile<Func<int?, bool>>("x => Return(x) is null");
            f(null);
            f(42);
        }

        [Fact]
        public void CrossCheck_IsExpression_Constant_Null_String()
        {
            var f = Compile<Func<string, bool>>("s => Return(s) is null");
            f(null);
            f("foo");
        }

        [Fact]
        public void CrossCheck_IsExpression_Declaration_String()
        {
            var f = Compile<Func<object, bool>>("o => Return(o) is string s && s.Length > 0");
            f(null);
            f(42);
            f("");
            f("foo");
        }

        [Fact]
        public void CrossCheck_IsExpression_Declaration_Int32()
        {
            var f = Compile<Func<object, bool>>("o => Return(o) is int x && x > 0");
            f(null);
            f(-42);
            f(0);
            f(42);
            f("foo");
        }

        [Fact]
        public void CrossCheck_IsExpression_Var_String()
        {
            var f = Compile<Func<string, bool>>("s => Return(s) is var t && t.Length > 0");
            AssertEx.Throws<NullReferenceException>(() => f(null));
            f("");
            f("foo");
        }
    }
}
