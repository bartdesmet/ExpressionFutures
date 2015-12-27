// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
        #region C# 3.0

        [TestMethod]
        public void CrossCheck_Arithmetic()
        {
            var f = Compile<Func<int>>("() => Return(1) + Return(2)");
            f();
        }

        [TestMethod]
        public void CrossCheck_ParamsArray()
        {
            var f = Compile<Func<string>>("() => string.Format(\"{0},{1},{2}\", 42, true, \"bar\")");
            f();
        }

        #endregion

        #region Multi-dimensional array initializers

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

        #endregion

        #region Conditional access

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

        #endregion

        #region Named parameters

        // TODO: any issues with interface methods?

        [TestMethod]
        public void CrossCheck_NamedParameters_Call()
        {
            var f = Compile<Func<int>>(@"() =>
{
    var b = new StrongBox<int>(1);
    Log(b.Value);
    return System.Threading.Interlocked.Exchange(value: Return(42), location1: ref b.Value);
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef1()
        {
            var f = Compile<Func<int[], int>>(@"xs =>
{
    return Utils.NamedParamByRef(y: Return(42), x: ref xs[0]);
}");
            f(new[] { 17 });
            AssertEx.Throws<IndexOutOfRangeException>(() => f(new int[0]));
            AssertEx.Throws<NullReferenceException>(() => f(null));
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef2()
        {
            var f = Compile<Func<StrongBox<int>, int>>(@"b =>
{
    return Utils.NamedParamByRef(y: Return(42), x: ref b.Value);
}");
            f(new StrongBox<int>(17));
            AssertEx.Throws<NullReferenceException>(() => f(null));
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_New()
        {
            var f = Compile<Func<int>>("() => new NamedAndOptionalParameters(z: Return(false), y: Return(\"foobar\"), x: Return(43)).Value");
            f();
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Index()
        {
            var f = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[z: Return(false), y: Return(\"foobar\"), x: Return(43)]");
            f();
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Invoke()
        {
            var f = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(z: Return(false), y: Return(\"foobar\"), x: Return(43))");
            f((x, y, z) => x + y.Length + (z ? 1 : 0));
        }

        #endregion

        #region Optional parameters

        // TODO: more cases
        // TODO: ref/out
        // TODO: generic methods with default(T)

        [TestMethod]
        public void CrossCheck_OptionalParameters_Call1()
        {
            var f1 = Compile<Func<int>>("() => Utils.OptionalParams()");
            f1();

            var f2 = Compile<Func<int>>("() => Utils.OptionalParams(Return(43))");
            f2();

            var f3 = Compile<Func<int>>("() => Utils.OptionalParams(Return(43), Return(\"foobar\"))");
            f3();

            var f4 = Compile<Func<int>>("() => Utils.OptionalParams(Return(43), Return(\"foobar\"), Return(false))");
            f4();
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_Call2()
        {
            var f1 = Compile<Func<int>>("() => Utils.OptionalParams(Return(43), y: Return(\"foobar\"))");
            f1();

            var f2 = Compile<Func<int>>("() => Utils.OptionalParams(Return(43), z: Return(false))");
            f2();

            var f3 = Compile<Func<int>>("() => Utils.OptionalParams(Return(43), Return(\"foobar\"), z: Return(false))");
            f3();
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_New1()
        {
            var f1 = Compile<Func<int>>("() => new NamedAndOptionalParameters().Value");
            f1();

            var f2 = Compile<Func<int>>("() => new NamedAndOptionalParameters(Return(43)).Value");
            f2();

            var f3 = Compile<Func<int>>("() => new NamedAndOptionalParameters(Return(43), Return(\"foobar\")).Value");
            f3();

            var f4 = Compile<Func<int>>("() => new NamedAndOptionalParameters(Return(43), Return(\"foobar\"), Return(false)).Value");
            f4();
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_New2()
        {
            var f1 = Compile<Func<int>>("() => new NamedAndOptionalParameters(Return(43), y: Return(\"foobar\")).Value");
            f1();

            var f2 = Compile<Func<int>>("() => new NamedAndOptionalParameters(Return(43), z: Return(false)).Value");
            f2();

            var f3 = Compile<Func<int>>("() => new NamedAndOptionalParameters(Return(43), Return(\"foobar\"), z: Return(false)).Value");
            f3();
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_Index1()
        {
            // NB: Need at least one argument for an indexer.

            var f2 = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[Return(43)]");
            f2();

            var f3 = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[Return(43), Return(\"foobar\")]");
            f3();

            var f4 = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[Return(43), Return(\"foobar\"), Return(false)]");
            f4();
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_Index2()
        {
            var f1 = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[Return(43), y: Return(\"foobar\")]");
            f1();

            var f2 = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[Return(43), z: Return(false)]");
            f2();

            var f3 = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[Return(43), Return(\"foobar\"), z: Return(false)]");
            f3();
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_Invoke1()
        {
            var d = new DelegateWithNamedAndOptionalParameters((x, y, z) => x + y.Length + (z ? 1 : 0));

            var f1 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f()");
            f1(d);

            var f2 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(Return(43))");
            f2(d);

            var f3 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(Return(43), Return(\"foobar\"))");
            f3(d);

            var f4 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(Return(43), Return(\"foobar\"), Return(false))");
            f4(d);
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_Invoke2()
        {
            var d = new DelegateWithNamedAndOptionalParameters((x, y, z) => x + y.Length + (z ? 1 : 0));

            var f1 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(Return(43), y: Return(\"foobar\"))");
            f1(d);

            var f2 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(Return(43), z: Return(false))");
            f2(d);

            var f3 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(Return(43), Return(\"foobar\"), z: Return(false))");
            f3(d);
        }

        #endregion

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

        #region Async

        // TODO: await with spilling of by-ref locals (known limitation)
        // TODO: more stack spilling cases

        [TestMethod]
        public void CrossCheck_Async_AwaitVoid()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        await Task.Yield();
    
        Log(""B"");
    
        return 42;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_AwaitNonVoid()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var x = await Task.FromResult(Return(42));
    
        Log(""B"");
    
        return x;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_AwaitDynamicVoid()
        {
            var f = Compile<Action<dynamic>>(@"(dynamic d) =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
    
        await d;
    
        Log(""B"");
    });
}");
            f(Task.Yield());
        }

        [TestMethod]
        public void CrossCheck_Async_AwaitDynamicNonVoid()
        {
            var f = Compile<Func<dynamic, int>>(@"(dynamic d) =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        int x = await d;
    
        Log(""B"");
    
        return x;
    });
}");
            f(Task.FromResult(42));
        }

        [TestMethod]
        public void CrossCheck_Async_ConfigureAwait()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var x = await Task.FromResult(42).ConfigureAwait(false);
    
        Log(""B"");
    
        return x;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_AwaitPatterns_Void()
        {
            foreach (var result in new[]
            {
                "new AwaitResult(AwaitTiming.Synchronous)",
                "new AwaitResult(AwaitTiming.Asynchronous)",
                "new AwaitResult(AwaitTiming.Racing)",
            })
            {
                foreach (var expr in new[]
                {
                    $"new AwaitableClassWithAwaiterClass({result})",
                    $"new AwaitableClassWithAwaiterStruct({result})",
                    $"new AwaitableStructWithAwaiterClass({result})",
                    $"new AwaitableStructWithAwaiterStruct({result})",
                })
                {
                    var f = Compile<Action>($@"() =>
{{
    AwaitVoid(async () =>
    {{
        Log(""before"");
    
        await {expr};
    
        Log(""after"");
    }});
}}");
                    f();
                }
            }
        }

        [TestMethod]
        public void CrossCheck_Async_AwaitPatterns_Void_Throws()
        {
            foreach (var result in new[]
            {
                "new AwaitResult(AwaitTiming.Synchronous, new DivideByZeroException())",
                "new AwaitResult(AwaitTiming.Asynchronous, new DivideByZeroException())",
                "new AwaitResult(AwaitTiming.Racing, new DivideByZeroException())",
            })
            {
                foreach (var expr in new[]
                {
                    $"new AwaitableClassWithAwaiterClass({result})",
                    $"new AwaitableClassWithAwaiterStruct({result})",
                    $"new AwaitableStructWithAwaiterClass({result})",
                    $"new AwaitableStructWithAwaiterStruct({result})",
                })
                {
                    var f = Compile<Action>($@"() =>
{{
    AwaitVoid(async () =>
    {{
        Log(""before"");
    
        await {expr};
    
        Log(""after"");
    }});
}}");
                    AssertEx.Throws<AggregateException>(() => f(), a => a.InnerException is DivideByZeroException);
                }
            }
        }

        [TestMethod]
        public void CrossCheck_Async_AwaitPatterns_NonVoid()
        {
            foreach (var result in new[]
            {
                "new AwaitResult<int>(AwaitTiming.Synchronous, 42)",
                "new AwaitResult<int>(AwaitTiming.Asynchronous, 42)",
                "new AwaitResult<int>(AwaitTiming.Racing, 42)",
            })
            {
                foreach (var expr in new[]
                {
                    $"new AwaitableClassWithAwaiterClass<int>({result})",
                    $"new AwaitableClassWithAwaiterStruct<int>({result})",
                    $"new AwaitableStructWithAwaiterClass<int>({result})",
                    $"new AwaitableStructWithAwaiterStruct<int>({result})",
                })
                {
                    var f = Compile<Func<int>>($@"() =>
{{
    return Await(async () =>
    {{
        Log(""before"");
    
        var res = await {expr};
    
        Log(""after"");

        return res;
    }});
}}");
                    f();
                }
            }
        }

        [TestMethod]
        public void CrossCheck_Async_AwaitPatterns_NonVoid_Throws()
        {
            foreach (var result in new[]
            {
                "new AwaitResult<int>(AwaitTiming.Synchronous, error: new DivideByZeroException())",
                "new AwaitResult<int>(AwaitTiming.Asynchronous, error: new DivideByZeroException())",
                "new AwaitResult<int>(AwaitTiming.Racing, error: new DivideByZeroException())",
            })
            {
                foreach (var expr in new[]
                {
                    $"new AwaitableClassWithAwaiterClass<int>({result})",
                    $"new AwaitableClassWithAwaiterStruct<int>({result})",
                    $"new AwaitableStructWithAwaiterClass<int>({result})",
                    $"new AwaitableStructWithAwaiterStruct<int>({result})",
                })
                {
                    var f = Compile<Func<int>>($@"() =>
{{
    return Await(async () =>
    {{
        Log(""before"");
    
        var res = await {expr};
    
        Log(""after"");

        return res;
    }});
}}");
                    AssertEx.Throws<AggregateException>(() => f(), a => a.InnerException is DivideByZeroException);
                }
            }
        }

        [TestMethod]
        public void CrossCheck_Async_Spilling_Binary()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var res = Return(1) + await Task.FromResult(Return(41));
    
        Log(""B"");
    
        return res;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Spilling_Call()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var res = Utils.Add(Return(1), await Task.FromResult(Return(41)));
    
        Log(""B"");
    
        return res;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Spilling_New()
        {
            var f = Compile<Func<long>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var res = new TimeSpan(Return(1983), await Task.FromResult(Return(2)), Return(11));
    
        Log(""B"");
    
        return res.Ticks;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Spilling_NewArrayInit()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var res = new int[] { Return(1), await Task.FromResult(Return(2)) };
    
        Log(""B"");
    
        return res[0];
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Spilling_NewArrayBounds()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var res = new int[Return(1), await Task.FromResult(Return(2))];
        res[0, 1] = 42;
    
        Log(""B"");
    
        return res[0, 1];
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Spilling_Index()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var arr = new int[3, 3] { { 40, 41, 42 }, { 43, 44, 45 }, { 46, 47, 48 } };
        var res = arr[Return(1), await Task.FromResult(Return(2))];
    
        Log(""B"");
    
        return res;
    });
}");
            f();
        }

        [Ignore] // BUG: Known limitation with stack spiller when dealing with by ref locals.
        [TestMethod]
        public void CrossCheck_Async_Spilling_ByRefLocals()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var x = 41;
        var res = System.Threading.Interlocked.Exchange(ref x, await Task.FromResult(42));
    
        Log(""B"");
    
        return res + x;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_AwaitInExpression()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var res = await Task.FromResult(Return(41)) + Return(1);
    
        Log(""B"");
    
        return res;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_TryFinally_AwaitInTry()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");

            res = await Task.FromResult(Return(42));

            Log(""after await"");
        }
        finally
        {
            Log(""finally"");
        }

        return res;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_TryFinally_AwaitInTry_Throws()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");

            res = Return(1) / (Return(42) - await Task.FromResult(Return(42)));

            Log(""after await"");
        }
        finally
        {
            Log(""finally"");
        }

        return res;
    });
}");
            AssertEx.Throws<AggregateException>(() => f(), a => a.InnerException is DivideByZeroException);
        }

        [TestMethod]
        public void CrossCheck_Async_TryFinally_AwaitInFinally()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");
        }
        finally
        {
            Log(""enter finally"");

            res = await Task.FromResult(Return(42));

            Log(""exit finally"");
        }

        return res;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_TryFinally_AwaitInFinally_Throws()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");
            throw new DivideByZeroException();
        }
        finally
        {
            Log(""enter finally"");

            res = await Task.FromResult(Return(42));

            Log(""exit finally"");
        }

        return res;
    });
}");
            AssertEx.Throws<AggregateException>(() => f(), a => a.InnerException is DivideByZeroException);
        }

        [TestMethod]
        public void CrossCheck_Async_TryCatch_AwaitInTry()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");

            res = await Task.FromResult(Return(42));

            Log(""after await"");
        }
        catch (DivideByZeroException)
        {
            Log(""in catch"");
        }
        finally
        {
            Log(""finally"");
        }

        return res;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_TryCatch_AwaitInTry_Throws()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");

            res = Return(1) / (Return(42) - await Task.FromResult(Return(42)));

            Log(""after await"");
        }
        catch (DivideByZeroException)
        {
            Log(""in catch"");
        }
        finally
        {
            Log(""finally"");
        }

        return res;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_TryCatch_AwaitInCatch()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");
        }
        catch (DivideByZeroException)
        {
            Log(""enter catch"");

            res = await Task.FromResult(Return(42));

            Log(""exit catch"");
        }

        return res;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_TryCatch_AwaitInCatch_Throws()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");

            throw new DivideByZeroException();
        }
        catch (DivideByZeroException)
        {
            Log(""enter catch"");

            res = await Task.FromResult(Return(42));

            Log(""exit catch"");
        }

        return res;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_TryCatch_AwaitInCatch_Rethrow()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");

            throw new DivideByZeroException();
        }
        catch (DivideByZeroException)
        {
            Log(""enter catch"");

            res = await Task.FromResult(Return(42));

            throw;
        }

        return res;
    });
}");
            AssertEx.Throws<AggregateException>(() => f(), a => a.InnerException is DivideByZeroException);
        }

        [Ignore] // See https://github.com/dotnet/coreclr/issues/1764 for restriction in CLR.
        [TestMethod]
        public void CrossCheck_Async_TryCatchWhen_AwaitInTry()
        {
            var f = Compile<Func<bool, int>>(@"b =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");

            res = await Task.FromResult(Return(42));

            Log(""after await"");
        }
        catch (DivideByZeroException) when (Return(b))
        {
            Log(""in catch"");
        }
        finally
        {
            Log(""finally"");
        }

        return res;
    });
}");
            f(true);
            AssertEx.Throws<AggregateException>(() => f(false), a => a.InnerException is DivideByZeroException);
        }

        [Ignore] // See https://github.com/dotnet/coreclr/issues/1764 for restriction in CLR.
        [TestMethod]
        public void CrossCheck_Async_TryCatchWhen_AwaitInTry_Throws()
        {
            var f = Compile<Func<bool, int>>(@"b =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");

            res = Return(1) / (Return(42) - await Task.FromResult(Return(42)));

            Log(""after await"");
        }
        catch (DivideByZeroException) when (Return(b))
        {
            Log(""in catch"");
        }
        finally
        {
            Log(""finally"");
        }

        return res;
    });
}");
            f(true);
            AssertEx.Throws<AggregateException>(() => f(false), a => a.InnerException is DivideByZeroException);
        }

        [Ignore] // See https://github.com/dotnet/coreclr/issues/1764 for restriction in CLR.
        [TestMethod]
        public void CrossCheck_Async_TryCatchWhen_AwaitInCatchWhen()
        {
            var f = Compile<Func<bool, int>>(@"b =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");
        }
        catch (DivideByZeroException) when (Return(b))
        {
            Log(""enter catch"");

            res = await Task.FromResult(Return(42));

            Log(""exit catch"");
        }

        return res;
    });
}");
            f(true);
            f(false);
        }

        [Ignore] // See https://github.com/dotnet/coreclr/issues/1764 for restriction in CLR.
        [TestMethod]
        public void CrossCheck_Async_TryCatchWhen_AwaitInCatchWhen_Throws()
        {
            var f = Compile<Func<bool, int>>(@"b =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");

            throw new DivideByZeroException();
        }
        catch (DivideByZeroException) when (Return(b))
        {
            Log(""enter catch"");

            res = await Task.FromResult(Return(42));

            Log(""exit catch"");
        }

        return res;
    });
}");
            f(true);
            AssertEx.Throws<AggregateException>(() => f(false), a => a.InnerException is DivideByZeroException);
        }

        [Ignore] // See https://github.com/dotnet/coreclr/issues/1764 for restriction in CLR.
        [TestMethod]
        public void CrossCheck_Async_TryCatchWhen_AwaitInCatchWhen_Rethrow()
        {
            var f = Compile<Func<bool, int>>(@"b =>
{
    return Await(async () =>
    {
        Log(""before try"");
    
        var res = default(int);
        try
        {
            Log(""in try"");

            throw new DivideByZeroException();
        }
        catch (DivideByZeroException) when (Return(b))
        {
            Log(""enter catch"");

            res = await Task.FromResult(Return(42));

            throw;
        }

        return res;
    });
}");
            AssertEx.Throws<AggregateException>(() => f(true), a => a.InnerException is DivideByZeroException);
            AssertEx.Throws<AggregateException>(() => f(false), a => a.InnerException is DivideByZeroException);
        }

        [TestMethod]
        public void CrossCheck_Async_TryNested1()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");

        try
        {
            Log(""B"");
            await Task.Yield();
            Log(""C"");

            try
            {
                Log(""D"");
                await Task.Yield();
                Log(""E"");
            }
            catch
            {
                Log(""F"");
            }
            finally
            {
                Log(""G"");
            }
        }
        catch
        {
            Log(""H"");
        }
        finally
        {
            Log(""I"");
        }

        Log(""J"");

        return 42;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_TryNested2()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
        await Task.Yield();
        Log(""B"");

        try
        {
            Log(""C"");
            await Task.Yield();
            Log(""D"");

            try
            {
                Log(""E"");
                await Task.Yield();
                Log(""F"");
            }
            catch
            {
                Log(""G"");
                await Task.Yield();
                Log(""H"");
            }
            finally
            {
                Log(""I"");
                await Task.Yield();
                Log(""J"");
            }

            Log(""K"");
            await Task.Yield();
            Log(""L"");
        }
        catch
        {
            Log(""M"");
            await Task.Yield();
            Log(""N"");
        }
        finally
        {
            Log(""O"");
            await Task.Yield();
            Log(""P"");
        }

        Log(""Q"");
        await Task.Yield();
        Log(""R"");

        return 42;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Try_ManOrBoy()
        {
            const int N = 9;

            var f = Compile<Func<IAwaitable[], int>>(@"tasks =>
{
    return Await(async () =>
    {
        Log(""A"");
        await tasks[0];
        Log(""B"");

        try
        {
            Log(""C"");
            await tasks[1];
            Log(""D"");

            try
            {
                Log(""E"");
                await tasks[2];
                Log(""F"");
            }
            catch (DivideByZeroException)
            {
                Log(""G"");
                await tasks[3];
                Log(""H"");
            }
            finally
            {
                Log(""I"");
                await tasks[4];
                Log(""J"");
            }

            Log(""K"");
            await tasks[5];
            Log(""L"");
        }
        catch (OverflowException)
        {
            Log(""M"");
            await tasks[6];
            Log(""N"");
        }
        finally
        {
            Log(""O"");
            await tasks[7];
            Log(""P"");
        }

        Log(""Q"");
        await tasks[8];
        Log(""R"");

        return 42;
    });
}");
            var allSync = Enumerable.Repeat(SyncAwaitable.Instance, N).ToArray();
            var allAsync = Enumerable.Repeat(AsyncAwaitable.Instance, N).ToArray();
            var allRacing = Enumerable.Repeat(RacingAwaitable.Instance, N).ToArray();

            f(allSync);
            f(allAsync);
            f(allRacing);

            var awaitAsn = AsyncAwaitable.Instance;
            var throwDBZ = new ThrowingAwaitable(new DivideByZeroException());
            var throwOVF = new ThrowingAwaitable(new OverflowException());
            var throwIOP = new ThrowingAwaitable(new InvalidOperationException());

            f(new IAwaitable[] { awaitAsn, awaitAsn, throwDBZ, awaitAsn, awaitAsn, awaitAsn, awaitAsn, awaitAsn, awaitAsn });
            f(new IAwaitable[] { awaitAsn, throwOVF, awaitAsn, awaitAsn, awaitAsn, awaitAsn, awaitAsn, awaitAsn, awaitAsn });
            f(new IAwaitable[] { awaitAsn, awaitAsn, throwOVF, awaitAsn, awaitAsn, awaitAsn, awaitAsn, awaitAsn, awaitAsn });
            f(new IAwaitable[] { awaitAsn, awaitAsn, throwDBZ, throwOVF, awaitAsn, awaitAsn, awaitAsn, awaitAsn, awaitAsn });
            f(new IAwaitable[] { awaitAsn, awaitAsn, awaitAsn, awaitAsn, throwOVF, awaitAsn, awaitAsn, awaitAsn, awaitAsn });
            f(new IAwaitable[] { awaitAsn, awaitAsn, awaitAsn, awaitAsn, awaitAsn, throwOVF, awaitAsn, awaitAsn, awaitAsn });

            // TODO: add more cases
        }

        [TestMethod]
        public void CrossCheck_Async_TryFinally_BranchPending()
        {
            var f = Compile<Func<int, int>>(@"(int b) =>
{
    return Await(async () =>
    {
        try
        {
            Log(""A"");
            await Task.Yield();

            try
            {
                Log(""B"");
                await Task.Yield();

                switch (b)
                {
                    case 1:
                        goto L1;
                    case 2:
                        goto L2;
                }
            }
            finally
            {
                Log(""C"");
                await Task.Yield();
            }

        L1:
            Log(""D"");
            await Task.Yield();
        }
        finally
        {
            Log(""E"");
            await Task.Yield();
        }

        Log(""F"");
        await Task.Yield();

    L2:
        Log(""G"");
        await Task.Yield();

        return 42;
    });
}");
            f(0);
            f(1);
            f(2);
        }

        [TestMethod]
        public void CrossCheck_Async_TryFinally_BranchPending_SyncTry()
        {
            var f = Compile<Func<int, int>>(@"(int b) =>
{
    return Await(async () =>
    {
        try
        {
            Log(""A"");

            try
            {
                Log(""B"");

                switch (b)
                {
                    case 1:
                        goto L1;
                    case 2:
                        goto L2;
                }
            }
            finally
            {
                Log(""C"");
            }

        L1:
            Log(""D"");
        }
        finally
        {
            Log(""E"");
            await Task.Yield();
        }

        Log(""F"");
        await Task.Yield();

    L2:
        Log(""G"");
        await Task.Yield();

        return 42;
    });
}");
            f(0);
            f(1);
            f(2);
        }

        [TestMethod]
        public void CrossCheck_Async_TryFinally_BranchPending_SyncFinally()
        {
            var f = Compile<Func<int, int>>(@"(int b) =>
{
    return Await(async () =>
    {
        try
        {
            Log(""A"");
            await Task.Yield();

            try
            {
                Log(""B"");
                await Task.Yield();

                switch (b)
                {
                    case 1:
                        goto L1;
                    case 2:
                        goto L2;
                }
            }
            finally
            {
                Log(""C"");
            }

        L1:
            Log(""D"");
            await Task.Yield();
        }
        finally
        {
            Log(""E"");
            await Task.Yield();
        }

        Log(""F"");
        await Task.Yield();

    L2:
        Log(""G"");
        await Task.Yield();

        return 42;
    });
}");
            f(0);
            f(1);
            f(2);
        }

        [TestMethod]
        public void CrossCheck_Async_TryFinally_BranchPending_Return()
        {
            var f = Compile<Func<int, int>>(@"(int b) =>
{
    return Await(async () =>
    {
        try
        {
            Log(""A"");
            await Task.Yield();

            try
            {
                Log(""B"");
                await Task.Yield();

                switch (b)
                {
                    case 1:
                        return 42;
                    case 2:
                        return 43;
                }
            }
            finally
            {
                Log(""C"");
                await Task.Yield();
            }

            Log(""D"");
            await Task.Yield();
        }
        finally
        {
            Log(""E"");
            await Task.Yield();
        }

        Log(""F"");
        await Task.Yield();

        return -1;
    });
}");
            f(0);
            f(1);
            f(2);
        }

        [TestMethod]
        public void CrossCheck_Async_TryCatch_BranchPending()
        {
            var f = Compile<Func<bool, int, int>>(@"(bool t, int b) =>
{
    return Await(async () =>
    {
        try
        {
            Log(""A"");
            await Task.Yield();

            try
            {
                Log(""B"");
                await Task.Yield();

                if (t)
                    throw new Exception(""Oops!"");
            }
            catch (Exception ex)
            {
                Log(""C"");
                Log(ex.Message);
                await Task.Yield();

                switch (b)
                {
                    case 1:
                        goto L1;
                    case 2:
                        goto L2;
                }
            }
            finally
            {
                Log(""D"");
                await Task.Yield();
            }

        L1:
            Log(""E"");
            await Task.Yield();
        }
        finally
        {
            Log(""F"");
            await Task.Yield();
        }

        Log(""G"");
        await Task.Yield();

    L2:
        Log(""H"");
        await Task.Yield();

        return 42;
    });
}");
            foreach (var t in new[] { false, true })
                foreach (var b in new[] { 0, 1, 2 })
                    f(t, b);
        }

        [TestMethod]
        public void CrossCheck_Async_TryCatch_BranchPending_Return()
        {
            var f = Compile<Func<bool, int, int>>(@"(bool t, int b) =>
{
    return Await(async () =>
    {
        try
        {
            Log(""A"");
            await Task.Yield();

            try
            {
                Log(""B"");
                await Task.Yield();

                if (t)
                    throw new Exception(""Oops!"");
            }
            catch (Exception ex)
            {
                Log(""C"");
                Log(ex.Message);
                await Task.Yield();

                switch (b)
                {
                    case 1:
                        return 42;
                    case 2:
                        return 43;
                }
            }
            finally
            {
                Log(""D"");
                await Task.Yield();
            }

            Log(""E"");
            await Task.Yield();
        }
        finally
        {
            Log(""F"");
            await Task.Yield();
        }

        Log(""G"");
        await Task.Yield();

        return -1;
    });
}");
            foreach (var t in new[] { false, true })
                foreach (var b in new[] { 0, 1, 2 })
                    f(t, b);
        }

        [TestMethod]
        public void CrossCheck_Async_Goto()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
        await Task.Yield();

        var i = 0;

    B:
        if (i < 10)
        {
            Log(""B"");
            await Task.Yield();
        }
        else
        {
            goto E;
        }

        i++;
        goto B;

    E:
        Log(""C"");
        await Task.Yield();
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_CovariantAssignment()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
        
        Base b = await Task.FromResult(new Derived());

        Log(b.ToString());
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_DeepAssignment()
        {
            var f = Compile<Func<bool>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
        
        int x;
        int y;
        x = y = await Task.FromResult(42);

        return x == y;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_If_Test1()
        {
            var f = Compile<Action<bool>>(@"b =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
        
        if (await Task.FromResult(Return(b)))
        {
            Log(""B"");
        }
        else
        {
            Log(""C"");
        }
    });
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_Async_Await_If_Test2()
        {
            var f = Compile<Func<bool, int>>(@"b =>
{
    return Await(async () =>
    {
        Log(""A"");
        
        if (await Task.FromResult(Return(b)))
        {
            Log(""B"");
        }
        else
        {
            Log(""C"");
        }

        return 42;
    });
}");
            f(false);
            f(true);
        }

        [TestMethod]
        public void CrossCheck_Async_Await_While_Test1()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
        
        var i = 0;
        while (await Task.FromResult(Return(i < 10)))
        {
            Log(""B"");
            i++;
        }
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_While_Test2()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
        
        var i = 0;
        while (await Task.FromResult(Return(i < 10)))
        {
            Log(""B"");
            i++;
        }

        return i;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_Do_Test1()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
        
        var i = 0;
        do
        {
            Log(""B"");
            i++;
        } while (await Task.FromResult(Return(i < 10)));
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_Do_Test2()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
        
        var i = 0;
        do
        {
            Log(""B"");
            i++;
        } while (await Task.FromResult(Return(i < 10)));

        return i;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_For_Test1()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
        
        for (var i = 0; await Task.FromResult(Return(i < 10)); i++)
        {
            Log(""B"");
        }
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_For_Test2()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
        
        for (var i = 0; await Task.FromResult(Return(i < 10)); i++)
        {
            Log(""B"");
        }

        return 42;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_ForEach_Collection1()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
        
        foreach (var x in await Task.FromResult(new[] { Return(1), Return(2) }))
        {
            Log(""B"" + x);
        }
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_ForEach_Collection2()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
        
        foreach (var x in await Task.FromResult(new[] { Return(1), Return(2) }))
        {
            Log(""B"" + x);
        }

        return 42;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_Using_Resource1()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
        
        using (await Task.FromResult(new ResourceClass(Log)))
        {
            Log(""B"");
        }
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_Using_Resource2()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
        
        using (await Task.FromResult(new ResourceClass(Log)))
        {
            Log(""B"");
        }

        return 42;
    });
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_Async_Await_Switch_SwitchValue1()
        {
            var f = Compile<Action<int>>(@"x =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
        
        switch (await Task.FromResult(Return(x)))
        {
            case 1:
                Log(""B"");
                break;
            case 2:
                Log(""C"");
                break;
            default:
                Log(""D"");
                break;
        }
    });
}");
            f(0);
            f(1);
            f(2);
        }

        [TestMethod]
        public void CrossCheck_Async_Await_Switch_SwitchValue2()
        {
            var f = Compile<Func<int, int>>(@"x =>
{
    return Await(async () =>
    {
        Log(""A"");
        
        switch (await Task.FromResult(Return(x)))
        {
            case 1:
                Log(""B"");
                return 9;
            case 2:
                Log(""C"");
                return 8;
            default:
                Log(""D"");
                return 7;
        }
    });
}");
            f(0);
            f(1);
            f(2);
        }

        #endregion

        #region Assignment

        [TestMethod]
        public void CrossCheck_Assignment_Primitives()
        {
            CrossCheck_Assignment_Core<byte>()(42);
            CrossCheck_Assignment_Core<sbyte>()(42);
            CrossCheck_Assignment_Core<ushort>()(42);
            CrossCheck_Assignment_Core<short>()(42);
            CrossCheck_Assignment_Core<uint>()(42);
            CrossCheck_Assignment_Core<int>()(42);
            CrossCheck_Assignment_Core<ulong>()(42UL);
            CrossCheck_Assignment_Core<long>()(42L);
            CrossCheck_Assignment_Core<float>()(42.0f);
            CrossCheck_Assignment_Core<double>()(42.0d);
            CrossCheck_Assignment_Core<decimal>()(42.0m);
            CrossCheck_Assignment_Core<bool>()(true);
            CrossCheck_Assignment_Core<char>()('a');
            CrossCheck_Assignment_Core<string>()("bar");
        }

        [TestMethod]
        public void CrossCheck_Assignment_More()
        {
            CrossCheck_Assignment_Core<byte?>()(42);
            CrossCheck_Assignment_Core<byte?>()(null);
            CrossCheck_Assignment_Core<ConsoleColor>()(ConsoleColor.Red);
            CrossCheck_Assignment_Core<ConsoleColor?>()(ConsoleColor.Red);
        }

        private Func<T, T> CrossCheck_Assignment_Core<T>()
        {
            var t = typeof(T).ToCSharp();

            var f1 = Compile<Func<T, T>>("x => { var y = x; return y; }");
            var f2 = Compile<Func<T, T>>($"x => {{ var b = new StrongBox<{t}>(); b.Value = x; return b.Value; }}");
            var f3 = Compile<Func<T, T>>($"x => {{ var b = new WeakBox<{t}>(); b.Value = x; return b.Value; }}");
            var f4 = Compile<Func<T, T>>($"x => {{ var a = new {t}[1]; a[0] = x; return a[0]; }}");
            var f5 = Compile<Func<T, T>>($"x => {{ var a = new {t}[1, 1]; a[0, 0] = x; return a[0, 0]; }}");
            var f6 = Compile<Func<T, T>>($"x => {{ var l = new List<{t}> {{ default({t}) }}; l[0] = x; return l[0]; }}");
            var f7 = Compile<Func<T, T>>($"x => {{ var l = new List<{t}> {{ default({t}) }}; l[index: 0] = x; return l[index: 0]; }}");
            var f8 = Compile<Func<T, T>>($"x => {{ var b = new WeakBox<{t}>(); b[0] = x; return b[0]; }}");

            return f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8;
        }

        #endregion

        #region Compound assignment

        // TODO: compound with conversions
        // TODO: compound with char

        [TestMethod]
        public void CrossCheck_CompoundAssignment_Integral()
        {
            Parallel.ForEach(new[] { "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=" }, op =>
            {
                CrossCheck_CompoundAssignment_Core<byte>(op)(43, 3);
                CrossCheck_CompoundAssignment_Core<sbyte>(op)(43, 3);
                CrossCheck_CompoundAssignment_Core<ushort>(op)(43, 3);
                CrossCheck_CompoundAssignment_Core<short>(op)(43, 3);
                CrossCheck_CompoundAssignment_Core<uint>(op)(43, 3);
                CrossCheck_CompoundAssignment_Core<int>(op)(43, 3);
                CrossCheck_CompoundAssignment_Core<ulong>(op)(43UL, 3);
                CrossCheck_CompoundAssignment_Core<long>(op)(43L, 3);
            });
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_Integral_Nullable()
        {
            Parallel.ForEach(new[] { "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=" }, op =>
            {
                var f1 = CrossCheck_CompoundAssignment_Core<byte?>(op);
                var f2 = CrossCheck_CompoundAssignment_Core<sbyte?>(op);
                var f3 = CrossCheck_CompoundAssignment_Core<ushort?>(op);
                var f4 = CrossCheck_CompoundAssignment_Core<short?>(op);
                var f5 = CrossCheck_CompoundAssignment_Core<uint?>(op);
                var f6 = CrossCheck_CompoundAssignment_Core<int?>(op);
                var f7 = CrossCheck_CompoundAssignment_Core<ulong?>(op);
                var f8 = CrossCheck_CompoundAssignment_Core<long?>(op);

                f1(43, 3);
                f2(43, 3);
                f3(43, 3);
                f4(43, 3);
                f5(43, 3);
                f6(43, 3);
                f7(43, 3);
                f8(43, 3);

                f1(43, null);
                f2(43, null);
                f3(43, null);
                f4(43, null);
                f5(43, null);
                f6(43, null);
                f7(43, null);
                f8(43, null);

                f1(null, 3);
                f2(null, 3);
                f3(null, 3);
                f4(null, 3);
                f5(null, 3);
                f6(null, 3);
                f7(null, 3);
                f8(null, 3);
            });
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_Integral_Overflow()
        {
            AssertCheckedThrows<byte>("+=", byte.MaxValue, 1);
            AssertCheckedThrows<sbyte>("+=", sbyte.MaxValue, 1);
            AssertCheckedThrows<ushort>("+=", ushort.MaxValue, 1);
            AssertCheckedThrows<short>("+=", short.MaxValue, 1);
            AssertCheckedThrows<uint>("+=", uint.MaxValue, 1);
            AssertCheckedThrows<int>("+=", int.MaxValue, 1);
            AssertCheckedThrows<ulong>("+=", ulong.MaxValue, 1);
            AssertCheckedThrows<long>("+=", long.MaxValue, 1);

            AssertCheckedThrows<byte>("-=", byte.MinValue, 1);
            AssertCheckedThrows<sbyte>("-=", sbyte.MinValue, 1);
            AssertCheckedThrows<ushort>("-=", ushort.MinValue, 1);
            AssertCheckedThrows<short>("-=", short.MinValue, 1);
            AssertCheckedThrows<uint>("-=", uint.MinValue, 1);
            AssertCheckedThrows<int>("-=", int.MinValue, 1);
            AssertCheckedThrows<ulong>("-=", ulong.MinValue, 1);
            AssertCheckedThrows<long>("-=", long.MinValue, 1);

            AssertCheckedThrows<byte>("*=", byte.MaxValue, 2);
            AssertCheckedThrows<sbyte>("*=", sbyte.MaxValue, 2);
            AssertCheckedThrows<ushort>("*=", ushort.MaxValue, 2);
            AssertCheckedThrows<short>("*=", short.MaxValue, 2);
            AssertCheckedThrows<uint>("*=", uint.MaxValue, 2);
            AssertCheckedThrows<int>("*=", int.MaxValue, 2);
            AssertCheckedThrows<ulong>("*=", ulong.MaxValue, 2);
            AssertCheckedThrows<long>("*=", long.MaxValue, 2);
        }

        private void AssertCheckedThrows<T>(string op, T value, T rhs)
        {
            AssertEx.Throws<OverflowException>(() => CrossCheck_CompoundAssignment_Checked_Core<T>(op)(value, rhs));
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_Float()
        {
            Parallel.ForEach(new[] { "+=", "-=", "*=", "/=", "%=" }, op =>
            {
                CrossCheck_CompoundAssignment_Core<float>(op)(42.0f, 3);
                CrossCheck_CompoundAssignment_Core<double>(op)(42.0d, 3);
                CrossCheck_CompoundAssignment_Core<decimal>(op)(42.0m, 3);
            });
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_Boolean()
        {
            Parallel.ForEach(new[] { "&=", "|=", "^=" }, op =>
            {
                var f = CrossCheck_CompoundAssignment_Core<bool>(op);

                foreach (var b1 in new[] { false, true })
                {
                    foreach (var b2 in new[] { false, true })
                    {
                        f(b1, b2);
                    }
                }
            });
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_Shift()
        {
            Parallel.ForEach(new[] { "<<=", ">>=" }, op =>
            {
                CrossCheck_CompoundAssignment_Core<int, int>(op)(42, 1);
                CrossCheck_CompoundAssignment_Core<uint, int>(op)(42, 1);
                CrossCheck_CompoundAssignment_Core<long, int>(op)(42, 1);
                CrossCheck_CompoundAssignment_Core<ulong, int>(op)(42, 1);
            });
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_String1()
        {
            var f = Compile<Func<string, string, string>>("(s, t) => { Log(s += t); return s; }");

            f("foo", "bar");
            f("", "bar");
            f("foo", "");
            f(null, "bar");
            f("foo", null);
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_String2()
        {
            var f = Compile<Func<string, object, string>>("(s, t) => { Log(s += t); return s; }");

            f("foo", "bar");
            f("foo", 42);
            f("foo", true);
            f("foo", null);
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_Delegate()
        {
            var f = Compile<Action>(@"() => {
    Action a = () => { Log(""a""); };
    Action b = () => { Log(""b""); };
    a += b;
    a();
    a -= b;
    a();
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_Enum_Add()
        {
            var f = Compile<Func<ConsoleColor, int, ConsoleColor>>("(e, u) => { Log(e += u); return e; }");

            f(ConsoleColor.Red, 1);
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_Enum_Subtract1()
        {
            var f = Compile<Func<ConsoleColor, int, ConsoleColor>>("(e, u) => { Log(e -= u); return e; }");

            f(ConsoleColor.Red, 1);
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_Enum_Subtract2()
        {
            var f = Compile<Func<ConsoleColor, ConsoleColor, ConsoleColor>>("(e1, e2) => { Log(e1 -= e2); return e1; }");

            f(ConsoleColor.Red, ConsoleColor.Blue);
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_Custom()
        {
            var f = Compile<Func<DateTime, TimeSpan, DateTime>>("(dt, ts) => { Log(dt += ts); return dt; }");

            f(new DateTime(1983, 2, 11), TimeSpan.FromHours(1));
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment_Custom_Nullable()
        {
            var f = Compile<Func<DateTime?, TimeSpan?, DateTime?>>("(dt, ts) => { Log(dt += ts); return dt; }");

            f(new DateTime(1983, 2, 11), TimeSpan.FromHours(1));
            f(new DateTime(1983, 2, 11), null);
            f(null, TimeSpan.FromHours(1));
            f(null, null);
        }

        [TestMethod]
        public void CrossCheck_CompoundAssignment()
        {
            var f = Compile<Func<int, int>>(@"i =>
{
    var b = new StrongBox<int>(i);
    Log(b.Value);
    var res = b.Value += Return(1);
    Log(res);
    return b.Value;
}");
            f(0);
            f(41);
        }

        [TestMethod] // See https://github.com/dotnet/corefx/issues/4984 for a relevant discussion
        public void CrossCheck_Issue4984_Binary_Repro1()
        {
            var f = Compile<Func<int, int>>(@"i =>
{
    var b = new WeakBox<int>();
    Log(b.Value);
    var res = b.Value += Return(1);
    Log(res);
    return b.Value;
}");
            f(0);
            f(41);
        }

        [TestMethod] // See https://github.com/dotnet/corefx/issues/4984 for a relevant discussion
        public void CrossCheck_Issue4984_Binary_Repro2()
        {
            var f = Compile<Func<int, int>>(@"i =>
{
    var b = new WeakBox<int>();
    Log(b[0]);
    var res = b[0] += Return(1);
    Log(res);
    return b[0];
}");
            f(0);
            f(41);
        }

        private Func<T, T, string> CrossCheck_CompoundAssignment_Core<T>(string op)
        {
            var t = typeof(T).ToCSharp();

            var f1 = Compile<Func<T, T, string>>($"(x, r) => {{ var y = x; var z = Log(y {op} r); return $\"({{y}},{{z}})\"; }}");
            var f2 = Compile<Func<T, T, string>>($"(x, r) => {{ var b = new StrongBox<{t}>(); var z = Log(b.Value {op} r); return $\"({{b.Value}},{{z}})\"; }}");
            var f3 = Compile<Func<T, T, string>>($"(x, r) => {{ var b = new WeakBox<{t}>(); var z = Log(b.Value {op} r); return $\"({{b.Value}},{{z}})\"; }}");
            var f4 = Compile<Func<T, T, string>>($"(x, r) => {{ var a = new {t}[1]; a[0] = x; var z = Log(a[0] {op} r); return $\"({{a[0]}},{{z}})\"; }}");
            var f5 = Compile<Func<T, T, string>>($"(x, r) => {{ var a = new {t}[1, 1]; a[0, 0] = x; var z = Log(a[0, 0] {op} r); return $\"({{a[0, 0]}},{{z}})\"; }}");
            var f6 = Compile<Func<T, T, string>>($"(x, r) => {{ var l = new List<{t}> {{ default({t}) }}; l[0] = x; var z = Log(l[0] {op} r); return $\"({{l[0]}},{{z}})\"; }}");
            var f7 = Compile<Func<T, T, string>>($"(x, r) => {{ var l = new List<{t}> {{ default({t}) }}; l[index: 0] = x; var z = Log(l[index: 0] {op} r); return $\"({{l[index: 0]}},{{z}})\"; }}");
            var f8 = Compile<Func<T, T, string>>($"(x, r) => {{ var b = new WeakBox<{t}>(); var z = Log(b[0] {op} r); return $\"({{b[0]}},{{z}})\"; }}");

            return f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8;
        }

        private Func<T, T, string> CrossCheck_CompoundAssignment_Checked_Core<T>(string op)
        {
            var t = typeof(T).ToCSharp();

            var f1 = Compile<Func<T, T, string>>($"(x, r) => {{ var y = x; var z = Log(checked(y {op} r)); return $\"({{y}},{{z}})\"; }}");
            var f2 = Compile<Func<T, T, string>>($"(x, r) => {{ var b = new StrongBox<{t}>(); var z = Log(checked(b.Value {op} r)); return $\"({{b.Value}},{{z}})\"; }}");
            var f3 = Compile<Func<T, T, string>>($"(x, r) => {{ var b = new WeakBox<{t}>(); var z = Log(checked(b.Value {op} r)); return $\"({{b.Value}},{{z}})\"; }}");
            var f4 = Compile<Func<T, T, string>>($"(x, r) => {{ var a = new {t}[1]; a[0] = x; var z = Log(checked(a[0] {op} r)); return $\"({{a[0]}},{{z}})\"; }}");
            var f5 = Compile<Func<T, T, string>>($"(x, r) => {{ var a = new {t}[1, 1]; a[0, 0] = x; var z = Log(checked(a[0, 0] {op} r)); return $\"({{a[0, 0]}},{{z}})\"; }}");
            var f6 = Compile<Func<T, T, string>>($"(x, r) => {{ var l = new List<{t}> {{ default({t}) }}; l[0] = x; var z = Log(checked(l[0] {op} r)); return $\"({{l[0]}},{{z}})\"; }}");
            var f7 = Compile<Func<T, T, string>>($"(x, r) => {{ var l = new List<{t}> {{ default({t}) }}; l[index: 0] = x; var z = Log(checked(l[index: 0] {op} r)); return $\"({{l[index: 0]}},{{z}})\"; }}");
            var f8 = Compile<Func<T, T, string>>($"(x, r) => {{ var b = new WeakBox<{t}>(); var z = Log(checked(b[0] {op} r)); return $\"({{b[0]}},{{z}})\"; }}");

            return f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8;
        }

        private Func<T, R, string> CrossCheck_CompoundAssignment_Core<T, R>(string op)
        {
            var t = typeof(T).ToCSharp();

            var f1 = Compile<Func<T, R, string>>($"(x, r) => {{ var y = x; var z = Log(y {op} r); return $\"({{y}},{{z}})\"; }}");
            var f2 = Compile<Func<T, R, string>>($"(x, r) => {{ var b = new StrongBox<{t}>(); var z = Log(b.Value {op} r); return $\"({{b.Value}},{{z}})\"; }}");
            var f3 = Compile<Func<T, R, string>>($"(x, r) => {{ var b = new WeakBox<{t}>(); var z = Log(b.Value {op} r); return $\"({{b.Value}},{{z}})\"; }}");
            var f4 = Compile<Func<T, R, string>>($"(x, r) => {{ var a = new {t}[1]; a[0] = x; var z = Log(a[0] {op} r); return $\"({{a[0]}},{{z}})\"; }}");
            var f5 = Compile<Func<T, R, string>>($"(x, r) => {{ var a = new {t}[1, 1]; a[0, 0] = x; var z = Log(a[0, 0] {op} r); return $\"({{a[0, 0]}},{{z}})\"; }}");
            var f6 = Compile<Func<T, R, string>>($"(x, r) => {{ var l = new List<{t}> {{ default({t}) }}; l[0] = x; var z = Log(l[0] {op} r); return $\"({{l[0]}},{{z}})\"; }}");
            var f7 = Compile<Func<T, R, string>>($"(x, r) => {{ var l = new List<{t}> {{ default({t}) }}; l[index: 0] = x; var z = Log(l[index: 0] {op} r); return $\"({{l[index: 0]}},{{z}})\"; }}");
            var f8 = Compile<Func<T, R, string>>($"(x, r) => {{ var b = new WeakBox<{t}>(); var z = Log(b[0] {op} r); return $\"({{b[0]}},{{z}})\"; }}");

            return f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8;
        }

        #endregion

        #region Unary increment/decrement

        // TODO: conversions

        [TestMethod]
        public void CrossCheck_UnaryAssignment_Integral()
        {
            Parallel.ForEach(new[] { "++", "--" }, op =>
            {
                CrossCheck_UnaryPreAssignment_Core<byte>(op)(43);
                CrossCheck_UnaryPreAssignment_Core<sbyte>(op)(43);
                CrossCheck_UnaryPreAssignment_Core<ushort>(op)(43);
                CrossCheck_UnaryPreAssignment_Core<short>(op)(43);
                CrossCheck_UnaryPreAssignment_Core<uint>(op)(43);
                CrossCheck_UnaryPreAssignment_Core<int>(op)(43);
                CrossCheck_UnaryPreAssignment_Core<ulong>(op)(43UL);
                CrossCheck_UnaryPreAssignment_Core<long>(op)(43L);
                CrossCheck_UnaryPreAssignment_Core<ConsoleColor>(op)(ConsoleColor.Red);

                CrossCheck_UnaryPostAssignment_Core<byte>(op)(43);
                CrossCheck_UnaryPostAssignment_Core<sbyte>(op)(43);
                CrossCheck_UnaryPostAssignment_Core<ushort>(op)(43);
                CrossCheck_UnaryPostAssignment_Core<short>(op)(43);
                CrossCheck_UnaryPostAssignment_Core<uint>(op)(43);
                CrossCheck_UnaryPostAssignment_Core<int>(op)(43);
                CrossCheck_UnaryPostAssignment_Core<ulong>(op)(43UL);
                CrossCheck_UnaryPostAssignment_Core<long>(op)(43L);
                CrossCheck_UnaryPostAssignment_Core<ConsoleColor>(op)(ConsoleColor.Red);
            });
        }

        [TestMethod]
        public void CrossCheck_UnaryAssignment_Integral_Nullable()
        {
            Parallel.ForEach(new[] { "++", "--" }, op =>
            {
                {
                    var f1 = CrossCheck_UnaryPreAssignment_Core<byte?>(op);
                    var f2 = CrossCheck_UnaryPreAssignment_Core<sbyte?>(op);
                    var f3 = CrossCheck_UnaryPreAssignment_Core<ushort?>(op);
                    var f4 = CrossCheck_UnaryPreAssignment_Core<short?>(op);
                    var f5 = CrossCheck_UnaryPreAssignment_Core<uint?>(op);
                    var f6 = CrossCheck_UnaryPreAssignment_Core<int?>(op);
                    var f7 = CrossCheck_UnaryPreAssignment_Core<ulong?>(op);
                    var f8 = CrossCheck_UnaryPreAssignment_Core<long?>(op);
                    var f9 = CrossCheck_UnaryPreAssignment_Core<ConsoleColor?>(op);

                    f1(43);
                    f2(43);
                    f3(43);
                    f4(43);
                    f5(43);
                    f6(43);
                    f7(43);
                    f8(43);
                    f9(ConsoleColor.Red);

                    f1(null);
                    f2(null);
                    f3(null);
                    f4(null);
                    f5(null);
                    f6(null);
                    f7(null);
                    f8(null);
                    f9(null);
                }

                {
                    var f1 = CrossCheck_UnaryPostAssignment_Core<byte?>(op);
                    var f2 = CrossCheck_UnaryPostAssignment_Core<sbyte?>(op);
                    var f3 = CrossCheck_UnaryPostAssignment_Core<ushort?>(op);
                    var f4 = CrossCheck_UnaryPostAssignment_Core<short?>(op);
                    var f5 = CrossCheck_UnaryPostAssignment_Core<uint?>(op);
                    var f6 = CrossCheck_UnaryPostAssignment_Core<int?>(op);
                    var f7 = CrossCheck_UnaryPostAssignment_Core<ulong?>(op);
                    var f8 = CrossCheck_UnaryPostAssignment_Core<long?>(op);
                    var f9 = CrossCheck_UnaryPostAssignment_Core<ConsoleColor?>(op);

                    f1(43);
                    f2(43);
                    f3(43);
                    f4(43);
                    f5(43);
                    f6(43);
                    f7(43);
                    f8(43);
                    f9(ConsoleColor.Red);

                    f1(null);
                    f2(null);
                    f3(null);
                    f4(null);
                    f5(null);
                    f6(null);
                    f7(null);
                    f8(null);
                    f9(null);
                }
            });
        }

        [TestMethod]
        public void CrossCheck_UnaryAssignment_Integral_Overflow()
        {
            AssertCheckedThrows<byte>("++", byte.MaxValue);
            AssertCheckedThrows<sbyte>("++", sbyte.MaxValue);
            AssertCheckedThrows<ushort>("++", ushort.MaxValue);
            AssertCheckedThrows<short>("++", short.MaxValue);
            AssertCheckedThrows<uint>("++", uint.MaxValue);
            AssertCheckedThrows<int>("++", int.MaxValue);
            AssertCheckedThrows<ulong>("++", ulong.MaxValue);
            AssertCheckedThrows<long>("++", long.MaxValue);

            AssertCheckedThrows<byte>("--", byte.MinValue);
            AssertCheckedThrows<sbyte>("--", sbyte.MinValue);
            AssertCheckedThrows<ushort>("--", ushort.MinValue);
            AssertCheckedThrows<short>("--", short.MinValue);
            AssertCheckedThrows<uint>("--", uint.MinValue);
            AssertCheckedThrows<int>("--", int.MinValue);
            AssertCheckedThrows<ulong>("--", ulong.MinValue);
            AssertCheckedThrows<long>("--", long.MinValue);
        }

        private void AssertCheckedThrows<T>(string op, T value)
        {
            AssertEx.Throws<OverflowException>(() => CrossCheck_UnaryPreAssignment_Checked_Core<T>(op)(value));
            AssertEx.Throws<OverflowException>(() => CrossCheck_UnaryPostAssignment_Checked_Core<T>(op)(value));
        }

        [TestMethod]
        public void CrossCheck_UnaryAssignment_Float()
        {
            Parallel.ForEach(new[] { "++", "--" }, op =>
            {
                CrossCheck_UnaryPreAssignment_Core<float>(op)(42.0f);
                CrossCheck_UnaryPreAssignment_Core<double>(op)(42.0d);
                CrossCheck_UnaryPreAssignment_Core<decimal>(op)(42.0m);

                CrossCheck_UnaryPostAssignment_Core<float>(op)(42.0f);
                CrossCheck_UnaryPostAssignment_Core<double>(op)(42.0d);
                CrossCheck_UnaryPostAssignment_Core<decimal>(op)(42.0m);
            });
        }

        [TestMethod] // See https://github.com/dotnet/corefx/issues/4984 for a relevant discussion
        public void CrossCheck_Issue4984_Unary_Repro1()
        {
            var f = Compile<Func<int, int>>(@"i =>
{
    var b = new WeakBox<int>();
    Log(b.Value);
    var res = b.Value++;
    Log(res);
    return b.Value;
}");
            f(0);
            f(41);
        }

        [TestMethod] // See https://github.com/dotnet/corefx/issues/4984 for a relevant discussion
        public void CrossCheck_Issue4984_Unary_Repro2()
        {
            var f = Compile<Func<int, int>>(@"i =>
{
    var b = new WeakBox<int>();
    Log(b[0]);
    var res = b[0]++;
    Log(res);
    return b[0];
}");
            f(0);
            f(41);
        }

        private Func<T, string> CrossCheck_UnaryPreAssignment_Core<T>(string op)
        {
            return CrossCheck_UnaryAssignment_Core<T>(op, null);
        }

        private Func<T, string> CrossCheck_UnaryPostAssignment_Core<T>(string op)
        {
            return CrossCheck_UnaryAssignment_Core<T>(null, op);
        }

        private Func<T, string> CrossCheck_UnaryAssignment_Core<T>(string pre, string post)
        {
            var t = typeof(T).ToCSharp();

            var f1 = Compile<Func<T, string>>($"x => {{ var y = x; var z = Log({pre}y{post}); return $\"({{y}},{{z}})\"; }}");
            var f2 = Compile<Func<T, string>>($"x => {{ var b = new StrongBox<{t}>(); var z = Log({pre}b.Value{post}); return $\"({{b.Value}},{{z}})\"; }}");
            var f3 = Compile<Func<T, string>>($"x => {{ var b = new WeakBox<{t}>(); var z = Log({pre}b.Value{post}); return $\"({{b.Value}},{{z}})\"; }}");
            var f4 = Compile<Func<T, string>>($"x => {{ var a = new {t}[1]; a[0] = x; var z = Log({pre}a[0]{post}); return $\"({{a[0]}},{{z}})\"; }}");
            var f5 = Compile<Func<T, string>>($"x => {{ var a = new {t}[1, 1]; a[0, 0] = x; var z = Log({pre}a[0, 0]{post}); return $\"({{a[0, 0]}},{{z}})\"; }}");
            var f6 = Compile<Func<T, string>>($"x => {{ var l = new List<{t}> {{ default({t}) }}; l[0] = x; var z = Log({pre}l[0]{post}); return $\"({{l[0]}},{{z}})\"; }}");
            var f7 = Compile<Func<T, string>>($"x => {{ var l = new List<{t}> {{ default({t}) }}; l[index: 0] = x; var z = Log({pre}l[index: 0]{post}); return $\"({{l[index: 0]}},{{z}})\"; }}");

            return f1 + f2 + f3 + f4 + f5 + f6 + f7;
        }

        private Func<T, string> CrossCheck_UnaryPreAssignment_Checked_Core<T>(string op)
        {
            return CrossCheck_UnaryAssignment_Checked_Core<T>(op, null);
        }

        private Func<T, string> CrossCheck_UnaryPostAssignment_Checked_Core<T>(string op)
        {
            return CrossCheck_UnaryAssignment_Checked_Core<T>(null, op);
        }

        private Func<T, string> CrossCheck_UnaryAssignment_Checked_Core<T>(string pre, string post)
        {
            var t = typeof(T).ToCSharp();

            var f1 = Compile<Func<T, string>>($"x => {{ var y = x; var z = Log(checked({pre}y{post})); return $\"({{y}},{{z}})\"; }}");
            var f2 = Compile<Func<T, string>>($"x => {{ var b = new StrongBox<{t}>(); var z = Log(checked({pre}b.Value{post})); return $\"({{b.Value}},{{z}})\"; }}");
            var f3 = Compile<Func<T, string>>($"x => {{ var b = new WeakBox<{t}>(); var z = Log(checked({pre}b.Value{post})); return $\"({{b.Value}},{{z}})\"; }}");
            var f4 = Compile<Func<T, string>>($"x => {{ var a = new {t}[1]; a[0] = x; var z = Log(checked({pre}a[0]{post})); return $\"({{a[0]}},{{z}})\"; }}");
            var f5 = Compile<Func<T, string>>($"x => {{ var a = new {t}[1, 1]; a[0, 0] = x; var z = Log(checked({pre}a[0, 0]{post})); return $\"({{a[0, 0]}},{{z}})\"; }}");
            var f6 = Compile<Func<T, string>>($"x => {{ var l = new List<{t}> {{ default({t}) }}; l[0] = x; var z = Log(checked({pre}l[0]{post})); return $\"({{l[0]}},{{z}})\"; }}");
            var f7 = Compile<Func<T, string>>($"x => {{ var l = new List<{t}> {{ default({t}) }}; l[index: 0] = x; var z = Log(checked({pre}l[index: 0]{post})); return $\"({{l[index: 0]}},{{z}})\"; }}");

            return f1 + f2 + f3 + f4 + f5 + f6 + f7;
        }

        #endregion

        #region Dynamic

        // TODO: with compile-time constants
        // TODO: index, invoke, new with ref/out parameters

        #region Unary

        [TestMethod]
        public void CrossCheck_Dynamic_Unary_Negate()
        {
            var f = CrossCheck_Dynamic_UnaryCore("-");

            var values = Integers.Where(o => o.GetType() != typeof(ulong)).Concat(Floats);

            foreach (var value in values)
            {
                f(value);
            }

            f(TimeSpan.FromSeconds(42));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Unary_NegateChecked()
        {
            // NB: byte, sbyte, ushort, short widen to int
            // NB: negate of uint becomes long
            // NB: negate of ulong not supported

            AssertDynamicUnaryCheckedThrows<int>("-", int.MinValue);
            AssertDynamicUnaryCheckedThrows<long>("-", long.MinValue);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Unary_UnaryPlus()
        {
            var f = CrossCheck_Dynamic_UnaryCore("+");

            var values = Integers.Concat(Floats);

            foreach (var value in values)
            {
                f(value);
            }

            f(TimeSpan.FromSeconds(42));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Unary_OnesComplement()
        {
            var f = CrossCheck_Dynamic_UnaryCore("~");

            var values = Integers;

            foreach (var value in values)
            {
                f(value);
            }
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Unary_Not()
        {
            var f = CrossCheck_Dynamic_UnaryCore("!");

            var values = Booleans;

            foreach (var value in values)
            {
                f(value);
            }
        }

        private void AssertDynamicUnaryCheckedThrows<T>(string op, T operand)
        {
            var f = CrossCheck_Dynamic_UnaryCore_Checked(op);

            AssertEx.Throws<OverflowException>(() => f(operand));
        }

        #endregion

        #region Binary

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_Add()
        {
            var f = CrossCheck_Dynamic_BinaryCore("+");

            var ls = Integers.Concat(Floats);
            var rs = Integers2.Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(new DateTime(1983, 2, 11), TimeSpan.FromDays(32 * 365.25));
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(1));
            f(ConsoleColor.Red, 1);
            f("foo", "bar");
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_AddChecked()
        {
            // NB: byte, sbyte, ushort, short widen to int

            AssertDynamicBinaryCheckedThrows<int>("+", int.MaxValue, 1);
            AssertDynamicBinaryCheckedThrows<uint>("+", uint.MaxValue, 1);
            AssertDynamicBinaryCheckedThrows<long>("+", long.MaxValue, 1);
            AssertDynamicBinaryCheckedThrows<ulong>("+", ulong.MaxValue, 1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_Subtract()
        {
            var f = CrossCheck_Dynamic_BinaryCore("-");

            var ls = Integers.Concat(Floats);
            var rs = Integers2.Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(new DateTime(1983, 2, 11), TimeSpan.FromDays(32 * 365.25));
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(1));
            f(ConsoleColor.Red, 1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_SubtractChecked()
        {
            // NB: byte, sbyte, ushort, short widen to int

            AssertDynamicBinaryCheckedThrows<int>("-", int.MinValue, 1);
            AssertDynamicBinaryCheckedThrows<uint>("-", uint.MinValue, 1);
            AssertDynamicBinaryCheckedThrows<long>("-", long.MinValue, 1);
            AssertDynamicBinaryCheckedThrows<ulong>("-", ulong.MinValue, 1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_Multiply()
        {
            var f = CrossCheck_Dynamic_BinaryCore("*");

            var ls = Integers.Concat(Floats);
            var rs = Integers2.Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_MultiplyChecked()
        {
            // NB: byte, sbyte, ushort, short widen to int

            AssertDynamicBinaryCheckedThrows<int>("*", int.MaxValue, 2);
            AssertDynamicBinaryCheckedThrows<uint>("*", uint.MaxValue, 2);
            AssertDynamicBinaryCheckedThrows<long>("*", long.MaxValue, 2);
            AssertDynamicBinaryCheckedThrows<ulong>("*", ulong.MaxValue, 2);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_Divide()
        {
            var f = CrossCheck_Dynamic_BinaryCore("/");

            var ls = Integers.Concat(Floats);
            var rs = Integers2.Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_Modulo()
        {
            var f = CrossCheck_Dynamic_BinaryCore("%");

            var ls = Integers.Concat(Floats);
            var rs = Integers2.Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_And()
        {
            var f = CrossCheck_Dynamic_BinaryCore("&");

            var ls = Integers.Concat(Booleans).Concat(Booleans);
            var rs = Integers2.Concat(Booleans).Concat(Booleans.Reverse());
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(BindingFlags.Public | BindingFlags.Instance, BindingFlags.Instance);
            f(BindingFlags.Public | BindingFlags.Instance, BindingFlags.Static);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_Or()
        {
            var f = CrossCheck_Dynamic_BinaryCore("|");

            var ls = Integers.Concat(Booleans).Concat(Booleans);
            var rs = Integers2.Concat(Booleans).Concat(Booleans.Reverse());
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(BindingFlags.Public, BindingFlags.Instance);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_ExclusiveOr()
        {
            var f = CrossCheck_Dynamic_BinaryCore("^");

            var ls = Integers.Concat(Booleans).Concat(Booleans);
            var rs = Integers2.Concat(Booleans).Concat(Booleans.Reverse());
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(BindingFlags.Public, BindingFlags.Instance);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_AndAlso()
        {
            var f = CrossCheck_Dynamic_BinaryCore("&&");

            var ls = Booleans;
            var rs = Booleans;
            var values = from l in ls from r in rs select new { l, r };

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_OrElse()
        {
            var f = CrossCheck_Dynamic_BinaryCore("||");

            var ls = Booleans;
            var rs = Booleans;
            var values = from l in ls from r in rs select new { l, r };

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_LeftShift()
        {
            var f = CrossCheck_Dynamic_BinaryCore("<<");

            foreach (var s in new[] { 0, 1, 2, 3, 0x1F, 0x20 })
            {
                f((int)42, s);
                f((uint)42, s);
            }

            foreach (var s in new[] { 0, 1, 2, 3, 0x3F, 0x40 })
            {
                f((long)42, s);
                f((ulong)42, s);
            }
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_RightShift()
        {
            var f = CrossCheck_Dynamic_BinaryCore(">>");

            foreach (var s in new[] { 0, 1, 2, 3, 0x1F, 0x20 })
            {
                f((int)42, s);
                f((uint)42, s);
            }

            foreach (var s in new[] { 0, 1, 2, 3, 0x3F, 0x40 })
            {
                f((long)42, s);
                f((ulong)42, s);
            }
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_Equal()
        {
            var f = CrossCheck_Dynamic_BinaryCore("==");

            var ls = Integers.Concat(Integers).Concat(Floats).Concat(Floats).Concat(Booleans).Concat(Booleans);
            var rs = Integers.Concat(Integers2).Concat(Floats).Concat(Floats2).Concat(Booleans).Concat(Booleans.Reverse());
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            // TODO: null cases (doesn't work with Return<T> because T can't be inferred)

            f("bar", "bar");
            f("bar", "foo");
            f(TimeSpan.Zero, TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(42));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_NotEqual()
        {
            var f = CrossCheck_Dynamic_BinaryCore("!=");

            var ls = Integers.Concat(Integers).Concat(Floats).Concat(Floats).Concat(Booleans).Concat(Booleans);
            var rs = Integers.Concat(Integers2).Concat(Floats).Concat(Floats2).Concat(Booleans).Concat(Booleans.Reverse());
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            // TODO: null cases (doesn't work with Return<T> because T can't be inferred)

            f("bar", "bar");
            f("bar", "foo");
            f(TimeSpan.Zero, TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(42));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_LessThan()
        {
            var f = CrossCheck_Dynamic_BinaryCore("<");

            var ls = Integers.Concat(Integers).Concat(Floats).Concat(Floats);
            var rs = Integers.Concat(Integers2).Concat(Floats).Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(TimeSpan.Zero, TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(42));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_LessThanOrEqual()
        {
            var f = CrossCheck_Dynamic_BinaryCore("<=");

            var ls = Integers.Concat(Integers).Concat(Floats).Concat(Floats);
            var rs = Integers.Concat(Integers2).Concat(Floats).Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(TimeSpan.Zero, TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(42));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_GreaterThan()
        {
            var f = CrossCheck_Dynamic_BinaryCore(">");

            var ls = Integers.Concat(Integers).Concat(Floats).Concat(Floats);
            var rs = Integers.Concat(Integers2).Concat(Floats).Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(TimeSpan.Zero, TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(42));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Binary_GreaterThanOrEqual()
        {
            var f = CrossCheck_Dynamic_BinaryCore(">=");

            var ls = Integers.Concat(Integers).Concat(Floats).Concat(Floats);
            var rs = Integers.Concat(Integers2).Concat(Floats).Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(TimeSpan.Zero, TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(42));
        }

        private void AssertDynamicBinaryCheckedThrows<T>(string op, T left, T right)
        {
            var f = CrossCheck_Dynamic_BinaryCore_Checked(op);

            AssertEx.Throws<OverflowException>(() => f(left, right));
        }

        #endregion

        #region Member

        [TestMethod]
        public void CrossCheck_Dynamic_GetMember1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => d.Length");

            f("");
            f("bar");
            f(new int[0]);
            f(new int[42]);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_GetMember2()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => d.Value");

            f(new StrongBox<int>());
            f(new StrongBox<int>(42));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_GetMember3()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => d.Ticks");

            f(TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42));
        }

        #endregion

        #region Index

        [TestMethod]
        public void CrossCheck_Dynamic_GetIndex1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => d[1]");

            f("bar");
            f(new int[2] { 41, 42 });
            f(new List<int> { 41, 42 });
            f(new Dictionary<int, string> { { 1, "one" } });

            AssertEx.Throws<IndexOutOfRangeException>(() => f(""));
            AssertEx.Throws<IndexOutOfRangeException>(() => f(new int[0]));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f(new List<int>()));
            AssertEx.Throws<KeyNotFoundException>(() => f(new Dictionary<int, string> { { 0, "zero" } }));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_GetIndex2()
        {
            var f = Compile<Func<int[], dynamic, dynamic>>("(int[] xs, dynamic d) => xs[d]");

            f(new int[2] { 41, 42 }, 0);
            f(new int[2] { 41, 42 }, 1);

            AssertEx.Throws<IndexOutOfRangeException>(() => f(new int[0], 0));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_GetIndex3()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic, dynamic>>("(dynamic d, dynamic e, dynamic f) => d[e, f]");

            f(new int[1, 1] { { 42 } }, 0, 0);
            f(new long[1, 2] { { 41, 42 } }, 0, 1);
            f(new double[2, 1] { { 41 }, { 42 } }, 1, 0);
            f(new string[2, 2] { { "40", "41" }, { "43", "42" } }, 1, 1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_GetIndex4()
        {
            var f = Compile<Func<int[,], dynamic, dynamic, dynamic>>("(int[,] xs, dynamic e, dynamic f) => xs[e, f]");

            f(new int[1, 1] { { 42 } }, 0, 0);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_GetIndex5()
        {
            var f = Compile<Func<dynamic, int, int, dynamic>>("(dynamic d, int e, int f) => d[e, f]");

            f(new int[1, 1] { { 42 } }, 0, 0);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_GetIndex_Named()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic>>("(dynamic xs, dynamic index) => xs[index: Return(index)]");

            f(new List<int> { 2, 3, 5 }, 1);
        }

        #endregion

        #region Invoke

        [TestMethod]
        public void CrossCheck_Dynamic_Invoke1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => d(42)");

            f(new Func<int, int>(x => x + 1));
            f(new Func<int, string>(x => x.ToString()));
            f(new Func<long, long>(x => x * 2L));

            AssertEx.Throws<RuntimeBinderException>(() => f(null));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Invoke2()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic>>("(dynamic d, dynamic e) => d(e)");

            f(new Func<int, int>(x => x + 1), 42);
            f(new Func<int, string>(x => x.ToString()), 42);
            f(new Func<long, long>(x => x * 2L), 42);

            f(new Func<string, string>(s => s.ToUpper()), "bar");
            f(new Func<string, string>(s => s ?? "null"), null);

            AssertEx.Throws<NullReferenceException>(() => f(new Func<string, string>(s => s.ToUpper()), null));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Invoke3()
        {
            var f = Compile<Func<Func<int, int>, dynamic, dynamic>>("(Func<int, int> f, dynamic d) => f(d)");

            f(new Func<int, int>(x => x + 1), 42);
            f(new Func<int, int>(x => x + 1), (byte)42);

            // REVIEW: RuntimeBinderException (C#) != NullReferenceException (ET)
            // AssertEx.Throws<NullReferenceException>(() => f(null, 42));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Invoke_Named()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic, dynamic, dynamic>>("(dynamic f, dynamic arg1, dynamic arg2, dynamic arg3) => f(arg2: Return(arg2), arg3: Return(arg3), arg1: Return(arg1))");

            f(new Func<int, int, int, int>((a, b, c) => a * b + c), 2, 3, 5);
        }

        #endregion

        #region InvokeMember

        [TestMethod]
        public void CrossCheck_Dynamic_InvokeMember_ToString()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => d.ToString()");

            f(new object());
            f(42);
            f(42L);
            f("foo");
            f(true);
            f(new DateTime(1983, 2, 11));
            f(new AppDomainSetup());
            f(ConsoleColor.Red);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_InvokeMember_Static()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => int.Parse(d)");

            f("42");
            AssertEx.Throws<FormatException>(() => f("bar"));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_InvokeMember_Out()
        {
            var f = Compile<Action<dynamic>>(@"(dynamic d) => {
    int res;
    bool b = /* dynamic convert */ int.TryParse(d, out res);

    Log(b);
    Log(res);
}");

            f("42");
            f("bar");
        }

        [TestMethod]
        public void CrossCheck_Dynamic_InvokeMember_Ref1()
        {
            var f = Compile<Action<dynamic>>(@"(dynamic d) => {
    int value = 41;
    int res = /* dynamic convert */ System.Threading.Interlocked.Exchange(ref value, d);

    Log(value);
    Log(res);
}");

            f(42);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_InvokeMember_Ref2()
        {
            var f = Compile<Action<dynamic, dynamic>>(@"(dynamic location, dynamic value) => {
    var res = System.Threading.Interlocked.Exchange(ref location, value);

    Log(location);
    Log(res);
}");

            f(41, 42);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_InvokeMember_Overloads()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => Math.Abs(d)");

            f(-42);
            f(-42L);
            f(-42.0);
            f(-42.0m);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_InvokeMember_Static_Generic()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => Utils.DynamicInvokeWithGeneric<double>(new Func<string, string>(Log), d)");

            f(42);
            f(true);
            f("bar");
        }

        [TestMethod]
        public void CrossCheck_Dynamic_InvokeMember_Instance_Generic()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic>>("(dynamic d, dynamic x) => d.Return<int>(x)");

            f(new DynamicInvoker(), 42);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_InvokeMember_Named()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic, dynamic>>("(dynamic s, dynamic startIndex, dynamic length) => s.Substring(length: Return(length), startIndex: Return(startIndex))");

            f("foobar", 2, 3);
        }

        #endregion

        #region InvokeConstructor

        [TestMethod]
        public void CrossCheck_Dynamic_InvokeConstructor1()
        {
            var f = Compile<Func<dynamic, TimeSpan>>("(dynamic d) => new TimeSpan(d)");

            f(42);
            f(42L);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_InvokeConstructor2()
        {
            var f = Compile<Func<dynamic, dynamic, DateTimeOffset>>("(dynamic d1, dynamic d2) => new DateTimeOffset(d1, d2)");

            var dt = new DateTime(1983, 2, 11);

            f(dt, TimeSpan.FromHours(1));
            f(dt.Ticks, TimeSpan.FromHours(1));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_InvokeConstructor_Named()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic, TimeSpan>>("(dynamic hours, dynamic minutes, dynamic seconds) => new TimeSpan(minutes: Return(minutes), hours: Return(hours), seconds: Return(seconds))");

            f(2, 3, 5);
        }

        #endregion

        #region Convert

        [TestMethod]
        public void CrossCheck_Dynamic_Convert1()
        {
            var f = Compile<Func<dynamic, DateTimeOffset>>("(dynamic d) => (DateTimeOffset)d");

            f(new DateTime(1983, 2, 11));
            f(new DateTimeOffset(new DateTime(1983, 2, 11), TimeSpan.FromHours(1)));
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Convert2()
        {
            var f = Compile<Func<dynamic, int>>("(dynamic d) => checked((int)d)");

            f(42);
            f(42L);
            AssertEx.Throws<OverflowException>(() => f(int.MaxValue + 1L));
        }

        #endregion

        #region Assignment

        [TestMethod]
        public void CrossCheck_Dynamic_Assign1()
        {
            var f = Compile<Action<dynamic>>("(dynamic d) => { Log<object>(d); var e = d; Log<object>(e); }");

            f(1);
            f("bar");
            f(null);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Assign2()
        {
            var f = Compile<Action<int>>("(int x) => { Log(x); dynamic d = x; Log(d); }");

            f(42);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_Assign3()
        {
            var f = Compile<Action<dynamic>>("(dynamic d) => { Log(d); int x = d; Log(x); }"); // also has convert

            f(42);
        }

        #endregion

        #region Compound assignment

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_Variable1()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic>>("(dynamic d, dynamic e) => { Log(d += Return(e)); return d; }");

            f(41, 1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_Variable2()
        {
            var f = Compile<Func<int, dynamic, dynamic>>("(int d, dynamic e) => { Log(d += Return(e)); return d; }");

            f(41, 1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_Variable3()
        {
            var f = Compile<Func<dynamic, int, dynamic>>("(dynamic d, int e) => { Log(d += Return(e)); return d; }");

            f(41, 1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_ArrayIndex1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { dynamic d = new[] { 41 }; Log(d[Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_ArrayIndex2()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { int[] d = new[] { 41 }; Log(d[Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_ArrayIndex3()
        {
            var f = Compile<Func<int, dynamic>>("(int e) => { dynamic d = new[] { 41 }; Log(d[Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_Index1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { dynamic d = new List<int> { 41 }; Log(d[Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_Index2()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { List<int> d = new List<int> { 41 }; Log(d[Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_Index3()
        {
            var f = Compile<Func<int, dynamic>>("(int e) => { dynamic d = new List<int> { 41 }; Log(d[Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_CSharpIndex1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { dynamic d = new List<int> { 41 }; Log(d[index: Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_CSharpIndex2()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { List<int> d = new List<int> { 41 }; Log(d[index: Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_CSharpIndex3()
        {
            var f = Compile<Func<int, dynamic>>("(int e) => { dynamic d = new List<int> { 41 }; Log(d[index: Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_Member1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { dynamic d = new StrongBox<int> { Value = 41 }; Log(d.Value += Return(e)); return d.Value; }");

            f(1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_Member2()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { StrongBox<int> d = new StrongBox<int> { Value = 41 }; Log(d.Value += Return(e)); return d.Value; }");

            f(1);
        }

        [TestMethod]
        public void CrossCheck_Dynamic_CompoundAssign_Member3()
        {
            var f = Compile<Func<int, dynamic>>("(int e) => { dynamic d = new StrongBox<int> { Value = 41 }; Log(d.Value += Return(e)); return d.Value; }");

            f(1);
        }

        // TODO: all operator kinds
        // TODO: checked variations
        // TODO: event handlers

        #endregion

        #region Unary increment/decrement

        // TODO
        // TODO: checked variations

        #endregion

        private Func<dynamic, dynamic> CrossCheck_Dynamic_UnaryCore(string op)
        {
            return Compile<Func<dynamic, dynamic>>($"(dynamic d) => {op}Return(d)");
        }

        private Func<dynamic, dynamic> CrossCheck_Dynamic_UnaryCore_Checked(string op)
        {
            return Compile<Func<dynamic, dynamic>>($"(dynamic d) => checked({op}Return(d))");
        }

        private Func<dynamic, dynamic, dynamic> CrossCheck_Dynamic_BinaryCore(string op)
        {
            return Compile<Func<dynamic, dynamic, dynamic>>($"(dynamic l, dynamic r) => Return(l) {op} Return(r)");
        }

        private Func<dynamic, dynamic, dynamic> CrossCheck_Dynamic_BinaryCore_Checked(string op)
        {
            return Compile<Func<dynamic, dynamic, dynamic>>($"(dynamic l, dynamic r) => checked(Return(l) {op} Return(r))");
        }

        private IEnumerable<object> Integers = new object[]
        {
            (byte)42,
            (sbyte)42,
            (ushort)42,
            (short)42,
            (uint)42,
            (int)42,
            (ulong)42,
            (long)42,
        };

        private IEnumerable<object> Integers2 = new object[]
        {
            (byte)3,
            (sbyte)3,
            (ushort)3,
            (short)3,
            (uint)3,
            (int)3,
            (ulong)3,
            (long)3,
        };

        private IEnumerable<object> Floats = new object[]
        {
            (float)3.14,
            (double)3.14,
            (decimal)3.14
        };

        private IEnumerable<object> Floats2 = new object[]
        {
            (float)2.72,
            (double)2.72,
            (decimal)2.72
        };

        private IEnumerable<object> Booleans = new object[]
        {
            false,
            true,
        };

        // TODO: should we add char cases as well?

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
