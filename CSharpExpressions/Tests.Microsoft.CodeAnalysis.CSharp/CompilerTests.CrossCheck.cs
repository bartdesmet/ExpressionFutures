// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
        #region C# 3.0

        [TestMethod]
        public void CrossCheck_Arithmetic()
        {
            var f = Compile<Func<int>>("() => Return(1) + Return(2)");
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

        [TestMethod]
        public void CrossCheck_NamedParameters()
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
        public void CrossCheck_NamedParameters_ByRef1()
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
        public void CrossCheck_NamedParameters_ByRef2()
        {
            var f = Compile<Func<StrongBox<int>, int>>(@"b =>
{
    return Utils.NamedParamByRef(y: Return(42), x: ref b.Value);
}");
            f(new StrongBox<int>(17));
            AssertEx.Throws<NullReferenceException>(() => f(null));
        }

        #endregion

        #region For

        [TestMethod]
        public void CrossCheck_For()
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

        #endregion

        #region Foreach

        [TestMethod]
        public void CrossCheck_ForEach1()
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
        public void CrossCheck_ForEach2()
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
        public void CrossCheck_ForEach3()
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
        public void CrossCheck_ForEach4()
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
        public void CrossCheck_ForEach5()
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

        #endregion

        #region Async

        [TestMethod]
        public void CrossCheck_Async1()
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
        public void CrossCheck_Async2()
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
        public void CrossCheck_Async3()
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

        #endregion

        #region Assignment

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

        #endregion
    }
}
