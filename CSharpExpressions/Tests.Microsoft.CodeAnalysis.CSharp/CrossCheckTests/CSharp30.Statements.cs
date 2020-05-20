// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        #region If

        [TestMethod]
        public void CrossCheck_IfThen1()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""before"");

    if (Return(b))
        Log(""if"");

    Log(""after"");
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_IfThen2()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""before"");

    if (Return(b))
    {
        Log(""if"");
    }

    Log(""after"");
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_IfThenElse1()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""before"");

    if (Return(b))
        Log(""if"");
    else
        Log(""else"");

    Log(""after"");
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_IfThenElse2()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""before"");

    if (Return(b))
    {
        Log(""if"");
    }
    else
    {
        Log(""else"");
    }

    Log(""after"");
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_IfThen_IsTrue()
        {
            // REVIEW: This uses a Call node for is_True; should it produce a cleaner tree?

            var f = Compile<Action<Truthy>>(@"b =>
{
    Log(""before"");

    if (Return(b))
    {
        Log(""if"");
    }

    Log(""after"");
}");
            f(new Truthy(false));
            f(new Truthy(true));
        }

        [TestMethod]
        public void CrossCheck_IfThen_Implicit()
        {
            var f = Compile<Action<Booleany>>(@"b =>
{
    Log(""before"");

    if (Return(b))
    {
        Log(""if"");
    }

    Log(""after"");
}");
            f(new Booleany(false));
            f(new Booleany(true));
        }

        #endregion

        #region Switch

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void CrossCheck_Switch_Char()
        {
            CrossCheck_Switch_Char_Core(new char[] { 'a', 'b', '0', '1', 'p', 'q', 'r', 'x', 'y', 'z', '\t', '\r', '\n' });
        }

        [TestMethod]
        public void CrossCheck_Switch_Char_Nullable()
        {
            CrossCheck_Switch_Char_Nullable_Core(new char?[] { null, 'a', 'b', '0', '1', 'p', 'q', 'r', 'x', 'y', 'z', '\t', '\r', '\n' });
        }

        [TestMethod]
        public void CrossCheck_Switch_Boolean()
        {
            CrossCheck_Switch_Boolean_Core(new bool[] { false, true });
        }

        [TestMethod]
        public void CrossCheck_Switch_Boolean_Nullable()
        {
            CrossCheck_Switch_Boolean_Nullable_Core(new bool?[] { null, false, true });
        }

        [TestMethod]
        public void CrossCheck_Switch_String()
        {
            CrossCheck_Switch_String_Core(new string[] { null, "", " ", "bar", "foo", "baz", "qux" });
        }

        [TestMethod]
        public void CrossCheck_Switch_Enum()
        {
            CrossCheck_Switch_Enum_Core(new ConsoleColor[] { ConsoleColor.White, ConsoleColor.Black, ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Cyan });
        }

        [TestMethod]
        public void CrossCheck_Switch_Enum_Nullable()
        {
            CrossCheck_Switch_Enum_Nullable_Core(new ConsoleColor?[] { null, ConsoleColor.White, ConsoleColor.Black, ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Cyan });
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        #endregion

        #region While

        [TestMethod]
        public void CrossCheck_While1()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = Return(0);

    while (Return(i < 10))
    {
        if (i == 2)
        {
            Log(""continue"");
            i++;
            continue;
        }

        if (i == 5)
        {
            Log(""break"");
            break;
        }

        Log($""body({i})"");

        Return(i++);
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_While2()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = Return(0);

    while (Return(i < 10))
    {
        Log($""body({i})"");

        Return(i++);
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_While_Conversion()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = Return(0);

    while (new Booleany(i < 10))
    {
        Log($""body({i})"");

        Return(i++);
    }

    Log(""After"");
}");
            f();
        }

        #endregion

        #region Do

        [TestMethod]
        public void CrossCheck_Do1()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = Return(0);

    do
    {
        if (i == 2)
        {
            Log(""continue"");
            i++;
            continue;
        }

        if (i == 5)
        {
            Log(""break"");
            break;
        }

        Log($""body({i})"");

        Return(i++);
    } while (Return(i < 10));

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Do2()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = Return(0);

    do
    {
        Log($""body({i})"");

        Return(i++);
    } while (Return(i < 10));

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Do_Conversion()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = Return(0);

    do
    {
        Log($""body({i})"");

        Return(i++);
    } while (new Booleany(i < 10));

    Log(""After"");
}");
            f();
        }

        #endregion

        #region For

        [TestMethod]
        public void CrossCheck_For1()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    for (var i = Return(0); Return(i < 10); Return(i++))
    {
        if (i == 2)
        {
            Log(""continue"");
            continue;
        }

        if (i == 5)
        {
            Log(""break"");
            break;
        }

        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_For2()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    for (var i = Return(0); Return(i < 10); Return(i++))
    {
        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_For_Conversion()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    for (var i = Return(0); new Booleany(i < 10); Return(i++))
    {
        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_For_Infinite()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = Return(0);

    for (;;)
    {
        if (i == Return(5))
        {
            Log(""break"");
            break;
        }

        Log($""body({i})"");

        Log(++i);
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_For_Complex()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    for (int i = Return(0), j = Return(10); Return(j >= i); Return(i++), Return(--j))
    {
        Log($""body({i}, {j})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_For_NoDeclarations()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    var i = 0;

    for (Log(""initializer""); Return(i < 10); Log(""iterator""))
    {
        Log($""body({i})"");
        i++;
    }

    Log(""After"");
}");
            f();
        }

        #endregion

        #region Foreach

        [TestMethod]
        public void CrossCheck_ForEach_IEnumerable()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    foreach (var i in Enumerable.Range(Return(0), Return(10)))
    {
        if (i == 2)
        {
            Log(""continue"");
            continue;
        }

        if (i == 5)
        {
            Log(""break"");
            break;
        }

        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_ForEach_String()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    foreach (var c in ""0123456789"")
    {
        var i = int.Parse(Return(c.ToString()));

        if (i == 2)
        {
            Log(""continue"");
            continue;
        }

        if (i == 5)
        {
            Log(""break"");
            break;
        }

        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_ForEach_QueryExpression()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    foreach (var i in from x in Enumerable.Range(Return(0), Return(10)) where Return(x) % Return(2) == Return(0) select Return(x))
    {
        if (i == 2)
        {
            Log(""continue"");
            continue;
        }

        if (i == 5)
        {
            Log(""break"");
            break;
        }

        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_ForEach_Array()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    foreach (var i in new[] { Return(0), 1, 2, 3, Return(4), 5, 6, 7, Return(8) })
    {
        if (i == 2)
        {
            Log(""continue"");
            continue;
        }

        if (i == 5)
        {
            Log(""break"");
            break;
        }

        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_ForEach_Convert()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    foreach (int i in new object[] { Return(0), 1, 2, 3, Return(4), 5, 6, 7, Return(8) })
    {
        if (i == 2)
        {
            Log(""continue"");
            continue;
        }

        if (i == 5)
        {
            Log(""break"");
            break;
        }

        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_ForEach_NoBreakContinue()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    foreach (var i in Enumerable.Range(Return(0), Return(10)))
    {
        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_ForEach_Pattern1()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    foreach (var i in new MyEnumerable(Return(10)))
    {
        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_ForEach_Pattern2()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    foreach (var i in new MyEnumerableValue(Return(10)))
    {
        Log($""body({i})"");
    }

    Log(""After"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_ForEach_NullReference()
        {
            var f = Compile<Action<IEnumerable<int>>>(@"xs =>
{
    Log(""Before"");

    foreach (var i in xs)
    {
        Log($""body({i})"");
    }

    Log(""After"");
}");
            AssertEx.Throws<NullReferenceException>(() => f(null));
        }

        [TestMethod]
        public void CrossCheck_ForEach_Dynamic()
        {
            var f = Compile<Action<dynamic>>(@"(dynamic xs) =>
{
    Log(""Before"");

    foreach (int i in xs)
    {
        Log($""body({i})"");
    }

    Log(""After"");
}");
            f(Enumerable.Range(0, 10));
            f(new[] { 0, 1, 2, 3, 4, 5 });
            f(new List<int> { 0, 1, 2, 3, 4, 5 });
        }

        #endregion

        #region Using

        [TestMethod]
        public void CrossCheck_Using_Class1()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""before"");

    using (b ? new ResourceClass(Log<string>) : null)
    {
        Log(""body"");
    }

    Log(""after"");
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_Using_Class2()
        {
            var f = Compile<Action<bool, bool>>(@"(b, c) =>
{
    Log(""before"");

    var r = b ? new ResourceClass(Log<string>) : null;
    using (r)
    {
        Log(""begin"");
        r?.Do(c);
        Log(""end"");
    }

    Log(""after"");
}");
            f(false, false);
            f(true, false);
            f(false, true);
            AssertEx.Throws<DivideByZeroException>(() => f(true, true));
        }

        [TestMethod]
        public void CrossCheck_Using_Class3()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""before"");

    IDisposable r = b ? new ResourceClass(Log<string>) : null;
    using (r)
    {
        Log(""body"");
    }

    Log(""after"");
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_Using_Struct1()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""before"");

    using (b ? (IDisposable)new ResourceStruct(Log<string>) : null)
    {
        Log(""body"");
    }

    Log(""after"");
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_Using_Struct2()
        {
            var f = Compile<Action<bool>>(@"c =>
{
    Log(""before"");

    using (var r = new ResourceStruct(Log<string>))
    {
        Log(""begin"");
        r.Do(c);
        Log(""end"");
    }

    Log(""after"");
}");
            f(false);
            AssertEx.Throws<DivideByZeroException>(() => f(true));
        }

        [TestMethod]
        public void CrossCheck_Using_Struct3()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""before"");

    IDisposable r = b ? (IDisposable)new ResourceStruct(Log<string>) : null;
    using (r)
    {
        Log(""body"");
    }

    Log(""after"");
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_Using_Struct4()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""before"");

    using (new ResourceStruct(Log<string>))
    {
        Log(""body"");
    }

    Log(""after"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Using_Dynamic()
        {
            var f = Compile<Action<dynamic>>(@"(dynamic d) =>
{
    Log(""before"");

    using (d)
    {
        Log(""body"");
    }

    Log(""after"");
}");
            f(new ResourceClass(_ => _));
            f(new ResourceStruct(_ => _));
        }

        [TestMethod]
        public void CrossCheck_Using_Many()
        {
            var f = Compile<Action<bool, bool>>(@"(bool b1, bool b2) =>
{
    Log(""before"");

    using (ResourceClass r1 = new ResourceClass(Log<string>), r2 = new ResourceClass(Log<string>))
    {
        Log(""begin"");
        r1?.Do(b1);
        r2?.Do(b2);
        Log(""end"");
    }

    Log(""after"");
}");
            f(false, false);
            AssertEx.Throws<DivideByZeroException>(() => f(true, false));
            AssertEx.Throws<DivideByZeroException>(() => f(false, true));
            AssertEx.Throws<DivideByZeroException>(() => f(true, true));
        }

        #endregion

        #region Try/Throw

        [TestMethod]
        public void CrossCheck_TryFinally()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""begin"");

    try
    {
        Log(""try"");

        if (b)
        {
            Log(""throw"");
            throw new DivideByZeroException();
        }

        Log(""leave"");
    }
    finally
    {
        Log(""finally"");
    }

    Log(""end"");
}");
            f(false);
            AssertEx.Throws<DivideByZeroException>(() => f(true));
        }

        [TestMethod]
        public void CrossCheck_TryCatch()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""begin"");

    try
    {
        Log(""try"");

        if (b)
        {
            Log(""throw"");
            throw new DivideByZeroException();
        }

        Log(""leave"");
    }
    catch (DivideByZeroException ex)
    {
        Log(""catch"");
        Log(ex.Message);
    }

    Log(""end"");
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_TryCatchAll()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""begin"");

    try
    {
        Log(""try"");

        if (b)
        {
            Log(""throw"");
            throw new DivideByZeroException();
        }

        Log(""leave"");
    }
    catch
    {
        Log(""catch"");
    }

    Log(""end"");
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_TryCatchMany()
        {
            var f = Compile<Action<int>>(@"i =>
{
    Log(""begin"");

    try
    {
        Log(""try"");

        switch (i)
        {
            case 0:
                Log(""throw DBZ"");
                throw new DivideByZeroException();
            case 1:
                Log(""throw OVF"");
                throw new OverflowException();
        }

        Log(""leave"");
    }
    catch (DivideByZeroException ex)
    {
        Log(""catch DBZ"");
        Log(ex.Message);
    }
    catch (OverflowException ex)
    {
        Log(""catch OVF"");
        Log(ex.Message);
    }

    Log(""end"");
}");
            f(0);
            f(1);
            f(2);
        }

        [TestMethod]
        public void CrossCheck_TryCatchFinally()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""begin"");

    try
    {
        Log(""try"");

        if (b)
        {
            Log(""throw"");
            throw new DivideByZeroException();
        }

        Log(""leave"");
    }
    catch (DivideByZeroException ex)
    {
        Log(""catch"");
        Log(ex.Message);
    }
    finally
    {
        Log(""finally"");
    }

    Log(""end"");
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_TryCatchManyFinally()
        {
            var f = Compile<Action<int>>(@"i =>
{
    Log(""begin"");

    try
    {
        Log(""try"");

        switch (i)
        {
            case 0:
                Log(""throw DBZ"");
                throw new DivideByZeroException();
            case 1:
                Log(""throw OVF"");
                throw new OverflowException();
        }

        Log(""leave"");
    }
    catch (DivideByZeroException ex)
    {
        Log(""catch DBZ"");
        Log(ex.Message);
    }
    catch (OverflowException ex)
    {
        Log(""catch OVF"");
        Log(ex.Message);
    }
    finally
    {
        Log(""finally"");
    }

    Log(""end"");
}");
            f(0);
            f(1);
            f(2);
        }

        [TestMethod]
        public void CrossCheck_TryCatch_Rethrow()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""begin"");

    try
    {
        Log(""try"");

        if (b)
        {
            Log(""throw"");
            throw new DivideByZeroException();
        }

        Log(""leave"");
    }
    catch (DivideByZeroException ex)
    {
        Log(""catch"");
        Log(ex.Message);
        throw;
    }

    Log(""end"");
}");
            f(false);
            AssertEx.Throws<DivideByZeroException>(() => f(true));
        }

        [TestMethod]
        public void CrossCheck_TryCatchFinally_Rethrow()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""begin"");

    try
    {
        Log(""try"");

        if (b)
        {
            Log(""throw"");
            throw new DivideByZeroException();
        }

        Log(""leave"");
    }
    catch (DivideByZeroException ex)
    {
        Log(""catch"");
        Log(ex.Message);
        throw;
    }
    finally
    {
        Log(""finally"");
    }

    Log(""end"");
}");
            f(false);
            AssertEx.Throws<DivideByZeroException>(() => f(true));
        }

        [Ignore] // See https://github.com/dotnet/coreclr/issues/1764 for restriction in CLR.
        [TestMethod]
        public void CrossCheck_TryCatchWhen()
        {
            var f = Compile<Action<bool, bool>>(@"(b, c) =>
{
    Log(""begin"");

    try
    {
        Log(""try"");

        if (b)
        {
            Log(""throw"");
            throw new DivideByZeroException();
        }

        Log(""leave"");
    }
    catch (DivideByZeroException ex) when (Return(c))
    {
        Log(""catch"");
        Log(ex.Message);
    }

    Log(""end"");
}");
            f(false, false);
            f(false, true);
            AssertEx.Throws<DivideByZeroException>(() => f(true, false));
            f(true, true);
        }

        #endregion

        #region Lock

        [TestMethod]
        public void CrossCheck_Lock()
        {
            var f = Compile<Action<object>>(@"o =>
{
    Log(""before"");

    lock (Return(o))
    {
        Log(""body"");
    }

    Log(""after"");
}");
            f(new object());
            AssertEx.Throws<ArgumentNullException>(() => f(null));
        }

        [TestMethod]
        public void CrossCheck_Lock_Error()
        {
            var f = Compile<Action<object>>(@"o =>
{
    Log(""before"");

    lock (Return(o))
    {
        Log(""body"");
        throw new DivideByZeroException();
    }

    Log(""after"");
}");
            AssertEx.Throws<DivideByZeroException>(() => f(new object()));
        }

        [TestMethod]
        public void CrossCheck_Lock_ManOrBoy()
        {
            var f = Compile<Func<int, int>>(@"N =>
{
    var n = 0;
    var o = new object();

    var t1 = Task.Run(() =>
    {
        for (var i = 0; i < N; i++)
        {
            lock (o)
            {
                n++;
            }
        }
    });

    var t2 = Task.Run(() =>
    {
        for (var i = 0; i < N; i++)
        {
            lock (o)
            {
                n++;
            }
        }
    });

    Task.WaitAll(t1, t2);

    return n;
}");
            Assert.AreEqual(20000, f(10000));
        }

        #endregion

        #region Goto/Label

        [TestMethod]
        public void CrossCheck_Goto1()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""begin"");

    goto bar;

    Log(""blackhole"");

bar:
    Log(""bar"");
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Goto2()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""begin"");

    var n = 0;

bar:

    Log(""bar"");

    if (n == 2)
    {
        goto exit;
    }

    Log(""1"");

    n++;

    Log(""2"");

    goto bar;

    Log(""blackhole"");

exit:
    
    Log(""exit"");
}");
            f();
        }

        #endregion

        #region Return

        [TestMethod]
        public void CrossCheck_Return_Void()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    Log(""begin"");

    if (b)
    {
        Log(""check"");
        return;
    }

    Log(""end"");
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_Return_NonVoid()
        {
            var f = Compile<Func<bool, int>>(@"b =>
{
    Log(""begin"");

    if (b)
    {
        Log(""check"");
        return -1;
    }

    Log(""end"");

    return 0;
}");
            f(false);
            f(true);
        }

        #endregion

        #region Lambda

        // TODO: add tests for quoted lambdas in statement bodies

        [TestMethod]
        public void CrossCheck_Lambda_Nested()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""begin"");
    
    Action a = () => Log(""a"");
    Action<int> b = x => Log($""b({x})"");
    
    Log(""middle"");

    a();
    b(42);

    Log(""end"");
}
");
            f();
        }

        [TestMethod]
        public void CrossCheck_Lambda_Nested_Closure()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""begin"");
    
    int x = 0;
    Func<int> @get = () => x;
    Action<int> @set = value => x = value;
    
    Log(""middle"");

    @set(Return(42));
    Log(@get());

    Log(""end"");
}
");
            f();
        }

        #endregion

        // TODO: Assert iterator bodies are not supported.
    }
}
