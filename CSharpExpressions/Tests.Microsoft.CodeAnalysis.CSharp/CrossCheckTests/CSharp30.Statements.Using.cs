// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
