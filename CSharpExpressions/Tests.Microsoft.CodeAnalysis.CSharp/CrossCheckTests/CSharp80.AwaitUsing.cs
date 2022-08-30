// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using Xunit;
using System;
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
        [Fact]
        public void CrossCheck_AwaitUsing_IAsyncDisposable()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        await using (var d = new MyAsyncDisposable(Log))
        {
            Log(""B"");
    
            var res = await d.GetResultAsync();

            Log(""C"");

            return res;
        }
    });
}", typeof(MyAsyncDisposable).Assembly);

            f();
        }

        [Fact]
        public void CrossCheck_AwaitUsing_PatternDispose()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var d = new MyAsyncDisposable(Log);

        await using (d.ConfigureAwait(false))
        {
            Log(""B"");
    
            var res = await d.GetResultAsync();

            Log(""C"");

            return res;
        }
    });
}", typeof(MyAsyncDisposable).Assembly);

            f();
        }
    }
}

public sealed class MyAsyncDisposable : IAsyncDisposable
{
    private readonly Func<string, string> _log;

    public MyAsyncDisposable(Func<string, string> log)
    {
        _log = log;
        _log(".ctor");
    }

    public async Task<int> GetResultAsync()
    {
        _log("RB");
        await Task.Yield();
        _log("RE");
        return 42;
    }

    public async ValueTask DisposeAsync()
    {
        _log("DB");
        await Task.Yield();
        _log("DE");
    }
}
