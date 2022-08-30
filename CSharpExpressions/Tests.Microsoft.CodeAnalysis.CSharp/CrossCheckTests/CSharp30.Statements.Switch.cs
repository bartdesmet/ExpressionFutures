// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
        public void CrossCheck_Switch_Integral()
        {
            var values = Enumerable.Range(0, 20);

            CrossCheck_Switch_Integral_Core<byte>(values);
            CrossCheck_Switch_Integral_Core<sbyte>(values);
            CrossCheck_Switch_Integral_Core<ushort>(values);
            CrossCheck_Switch_Integral_Core<short>(values);
            CrossCheck_Switch_Integral_Core<uint>(values);
            CrossCheck_Switch_Integral_Core<int>(values);
            CrossCheck_Switch_Integral_Core<ulong>(values);
            CrossCheck_Switch_Integral_Core<long>(values);
        }

        [Fact]
        public void CrossCheck_Switch_Integral_Nullable()
        {
            var values = Enumerable.Range(0, 20).Select(x => (int?)x).Concat(new int?[] { null });

            CrossCheck_Switch_Integral_Nullable_Core<byte?>(values);
            CrossCheck_Switch_Integral_Nullable_Core<sbyte?>(values);
            CrossCheck_Switch_Integral_Nullable_Core<ushort?>(values);
            CrossCheck_Switch_Integral_Nullable_Core<short?>(values);
            CrossCheck_Switch_Integral_Nullable_Core<uint?>(values);
            CrossCheck_Switch_Integral_Nullable_Core<int?>(values);

            // TODO: Produces invalid code in Roslyn, see https://github.com/dotnet/roslyn/issues/7625. Reenable
            //       when fixed in Roslyn.

            //CrossCheck_Switch_Integral_Nullable_Core<ulong?>(values);
            //CrossCheck_Switch_Integral_Nullable_Core<long?>(values);
        }

        [Fact]
        public void CrossCheck_Switch_Char()
        {
            CrossCheck_Switch_Char_Core(new char[] { 'a', 'b', '0', '1', 'p', 'q', 'r', 'x', 'y', 'z', '\t', '\r', '\n' });
        }

        [Fact]
        public void CrossCheck_Switch_Char_Nullable()
        {
            CrossCheck_Switch_Char_Nullable_Core(new char?[] { null, 'a', 'b', '0', '1', 'p', 'q', 'r', 'x', 'y', 'z', '\t', '\r', '\n' });
        }

        [Fact]
        public void CrossCheck_Switch_Boolean()
        {
            CrossCheck_Switch_Boolean_Core(new bool[] { false, true });
        }

        [Fact]
        public void CrossCheck_Switch_Boolean_Nullable()
        {
            CrossCheck_Switch_Boolean_Nullable_Core(new bool?[] { null, false, true });
        }

        [Fact]
        public void CrossCheck_Switch_String()
        {
            CrossCheck_Switch_String_Core(new string[] { null, "", " ", "bar", "foo", "baz", "qux" });
        }

        [Fact]
        public void CrossCheck_Switch_Enum()
        {
            CrossCheck_Switch_Enum_Core(new ConsoleColor[] { ConsoleColor.White, ConsoleColor.Black, ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Cyan });
        }

        [Fact]
        public void CrossCheck_Switch_Enum_Nullable()
        {
            CrossCheck_Switch_Enum_Nullable_Core(new ConsoleColor?[] { null, ConsoleColor.White, ConsoleColor.Black, ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Cyan });
        }

        [Fact]
        public void CrossCheck_Switch_Empty()
        {
            var f = Compile<Action<int>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
    }

    Log(""after"");
}");
            f(42);
        }

        [Fact]
        public void CrossCheck_Switch_NoBreak()
        {
            var f = Compile<Func<bool, int>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case true:
            return 1;
        default:
            return 0;
    }

    Log(""after"");
}");
            f(false);
            f(true);
        }

        [Fact]
        public void CrossCheck_Switch_Goto()
        {
            var f = Compile<Action<int>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case 1:
            Log(""odd"");
            break;
        case 2:
            Log(""2"");
            goto case 4;
        case 3:
        case 5:
            Log(""3-5"");
            goto case 1;
        case 4:
        case 6:
            Log(""4-6"");
            goto case 8;
        case 8:
            Log(""even"");
            break;
        default:
            Log(""dunno"");
            break;
    }

    Log(""after"");
}");
            for (var i = 0; i < 10; i++)
            {
                f(i);
            }
        }

        [Fact]
        public void CrossCheck_Switch_Goto_Nullable()
        {
            var f = Compile<Action<int?>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case null:
            Log(""null"");
            goto default;
        case 0:
            goto case null;
        case 1:
            Log(""odd"");
            break;
        case 2:
            Log(""2"");
            goto case 4;
        case 3:
        case 5:
            Log(""3-5"");
            goto case 1;
        case 4:
        case 6:
            Log(""4-6"");
            goto case 8;
        case 8:
            Log(""even"");
            break;
        default:
            Log(""dunno"");
            break;
    }

    Log(""after"");
}");
            f(null);

            for (var i = 0; i < 10; i++)
            {
                f(i);
            }
        }

        [Fact]
        public void CrossCheck_Switch_Goto_String()
        {
            var f = Compile<Action<string>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case ""bar"":
            Log(""meta"");
            break;
        case ""foo"":
        case ""qux"":
            Log(""foo"");
            goto case ""bar"";
        case null:
            Log(""null"");
            break;
        default:
            Log(""non-meta"");
            break;
    }

    Log(""after"");
}");
            f(null);
            f("bar");
            f("foo");
            f("qux");
            f("I'm not meta");
        }

        [Fact]
        public void CrossCheck_Switch_Goto_Enum()
        {
            var f = Compile<Action<ConsoleColor>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case ConsoleColor.Red:
            Log(""RGB"");
            break;
        case ConsoleColor.Blue:
        case ConsoleColor.Green:
            Log(""B|G"");
            goto case ConsoleColor.Red;
        default:
            Log(""other"");
            break;
    }

    Log(""after"");
}");
            f(ConsoleColor.Red);
            f(ConsoleColor.Blue);
            f(ConsoleColor.Green);
            f(ConsoleColor.Cyan);
        }

        [Fact]
        public void CrossCheck_Switch_Conversion()
        {
            var f = Compile<Action<Inty>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case 0:
            Log(""zero"");
            break;
        case 1:
            Log(""one"");
            break;
    }

    Log(""after"");
}");
            f(new Inty(0));
            f(new Inty(1));
        }

        [Fact]
        public void CrossCheck_Switch_Locals()
        {
            var f = Compile<Action<int>>(@"x =>
{
    switch (Return(x))
    {
        case 0:
            int a = 1;
            Log($""zero({a})"");
            break;
        case 1:
            int b = 3;
            Log($""one({b})"");
            break;
    }
}");
            f(new Inty(0));
            f(new Inty(1));
        }

        private void CrossCheck_Switch_Integral_Core<T>(IEnumerable<int> values)
        {
            var f = Compile<Action<T>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case 0:
            Log(""zero"");
            break;
        case 1:
            Log(""one"");
            break;
        case 7:
        case 8:
        case 9:
            Log(""large"");
            break;
        case 17:
        default:
            Log(""other"");
            break;
    }

    Log(""after"");
}");

            var p = Expression.Parameter(typeof(int));
            var cast = Expression.Lambda<Func<int, T>>(Expression.Convert(p, typeof(T)), p).Compile();
            var cases = values.Select(cast);

            foreach (var x in cases)
            {
                f(x);
            }
        }

        private void CrossCheck_Switch_Integral_Nullable_Core<T>(IEnumerable<int?> values)
        {
            var f = Compile<Action<T>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case null:
            Log(""null"");
            break;
        case 0:
            Log(""zero"");
            break;
        case 1:
            Log(""one"");
            break;
        case 7:
        case 8:
        case 9:
            Log(""large"");
            break;
        case 17:
        default:
            Log(""other"");
            break;
    }

    Log(""after"");
}");

            var p = Expression.Parameter(typeof(int?));
            var cast = Expression.Lambda<Func<int?, T>>(Expression.Convert(p, typeof(T)), p).Compile();
            var cases = values.Select(cast);

            foreach (var x in cases)
            {
                f(x);
            }
        }

        private void CrossCheck_Switch_Char_Core(IEnumerable<char> values)
        {
            var f = Compile<Action<char>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case 'a':
            Log(""a"");
            break;
        case '0':
            Log(""zero"");
            break;
        case 'x':
        case 'y':
        case 'z':
            Log(""math"");
            break;
        case '\t':
        case '\r':
            Log(""control"");
            break;
        case 'q':
        default:
            Log(""other"");
            break;
    }

    Log(""after"");
}");

            foreach (var x in values)
            {
                f(x);
            }
        }

        private void CrossCheck_Switch_Char_Nullable_Core(IEnumerable<char?> values)
        {
            var f = Compile<Action<char?>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case null:
            Log(""null"");
            break;
        case 'a':
            Log(""a"");
            break;
        case '0':
            Log(""zero"");
            break;
        case 'x':
        case 'y':
        case 'z':
            Log(""math"");
            break;
        case '\t':
        case '\r':
            Log(""control"");
            break;
        case 'q':
        default:
            Log(""other"");
            break;
    }

    Log(""after"");
}");

            foreach (var x in values)
            {
                f(x);
            }
        }

        private void CrossCheck_Switch_Boolean_Core(IEnumerable<bool> values)
        {
            var f = Compile<Action<bool>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case true:
            Log(""true"");
            break;
        case false:
            Log(""false"");
            break;
    }

    Log(""after"");
}");

            foreach (var x in values)
            {
                f(x);
            }
        }

        private void CrossCheck_Switch_Boolean_Nullable_Core(IEnumerable<bool?> values)
        {
            var f = Compile<Action<bool?>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case true:
            Log(""true"");
            break;
        case false:
            Log(""false"");
            break;
        case null:
            Log(""null"");
            break;
    }

    Log(""after"");
}");

            foreach (var x in values)
            {
                f(x);
            }
        }

        private void CrossCheck_Switch_String_Core(IEnumerable<string> values)
        {
            var f = Compile<Action<string>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case """":
            Log(""empty"");
            break;
        case ""qux"":
            Log(""qux"");
            break;
        case ""foo"":
        case ""bar"":
            Log(""foobar"");
            break;
        case null:
            Log(""null"");
            break;
        default:
            Log(""other"");
            break;
    }

    Log(""after"");
}");

            foreach (var x in values)
            {
                f(x);
            }
        }

        private void CrossCheck_Switch_Enum_Core(IEnumerable<ConsoleColor> values)
        {
            var f = Compile<Action<ConsoleColor>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case ConsoleColor.Red:
        case ConsoleColor.Blue:
        case ConsoleColor.Green:
            Log(""RGB"");
            break;
        case ConsoleColor.Black:
        case ConsoleColor.White:
            Log(""B|W"");
            break;
        case ConsoleColor.Yellow:
            Log(""submarine"");
            break;
        default:
            Log(""other"");
            break;
    }

    Log(""after"");
}");

            foreach (var x in values)
            {
                f(x);
            }
        }

        private void CrossCheck_Switch_Enum_Nullable_Core(IEnumerable<ConsoleColor?> values)
        {
            var f = Compile<Action<ConsoleColor?>>(@"x =>
{
    Log(""before"");

    switch (Return(x))
    {
        case ConsoleColor.Red:
        case ConsoleColor.Blue:
        case ConsoleColor.Green:
            Log(""RGB"");
            break;
        case ConsoleColor.Black:
        case ConsoleColor.White:
            Log(""B|W"");
            break;
        case ConsoleColor.Yellow:
            Log(""submarine"");
            break;
        default:
            Log(""other"");
            break;
        case null:
            Log(""null"");
            break;
    }

    Log(""after"");
}");

            foreach (var x in values)
            {
                f(x);
            }
        }
    }
}
