﻿// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void CrossCheck_ForEach_Array_MultiDimensional()
        {
            var f = Compile<Action>(@"() =>
{
    Log(""Before"");

    foreach (var x in new int[2, 3, 5] { { {9, 8, 7, 6, 5}, {11, 12, 13, 14, 15}, {25, 24, 26, 23, 27} }, { {99, 98, 97, 96, 95}, {81, 82, 83, 84, 85}, {75, 74, 76, 73, 77} } })
    {
        if (x % 2 == 0)
        {
            Log(""continue"");
            continue;
        }

        if (x == 83)
        {
            Log(""break"");
            break;
        }

        Log($""body({x})"");
    }

    Log(""After"");
}");
            f();
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        // TODO: Add tests for dispose behavior using helper types from manual tests
        //       in order to ensure that the generated EnumeratorInfo from Roslyn does
        //       match our expectations.
    }
}
