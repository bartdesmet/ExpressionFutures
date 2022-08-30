// Prototyping extended expression trees for C#.
//
// bartde - December 2015

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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact(Skip = "See https://github.com/dotnet/coreclr/issues/1764 for restriction in CLR.")]
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
    }
}
