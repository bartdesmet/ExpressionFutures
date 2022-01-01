// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
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
        [TestMethod]
        public void CrossCheck_AwaitForEach_IAsyncEnumerable()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var xs = new MyAsyncEnumerable(Log);

        Log(""B"");

        int sum = 0;

        await foreach (var x in xs)
        {
            Log(""C"");

            sum += x;

            Log(""D"");
        }

        Log(""E"");

        return sum;
    });
}", typeof(MyAsyncEnumerable).Assembly);

            f();
        }

        [TestMethod]
        public void CrossCheck_AwaitForEach_IAsyncEnumerable_ConfigureAwait()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var xs = new MyAsyncEnumerable(Log);

        Log(""B"");

        int sum = 0;

        await foreach (var x in xs.ConfigureAwait(false))
        {
            Log(""C"");

            sum += x;

            Log(""D"");
        }

        Log(""E"");

        return sum;
    });
}", typeof(MyAsyncEnumerable).Assembly);

            f();
        }
    }
}

public sealed class MyAsyncEnumerable : IAsyncEnumerable<int>
{
    private readonly Func<string, string> _log;

    public MyAsyncEnumerable(Func<string, string> log)
    {
        _log = log;
        _log("MyAsyncEnumerable::.ctor");
    }

    public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        _log("MyAsyncEnumerable::GetAsyncEnumerator");
        return new Enumerator(_log);
    }

    private sealed class Enumerator : IAsyncEnumerator<int>
    {
        private readonly Func<string, string> _log;
        private int _i = -1;

        public Enumerator(Func<string, string> log)
        {
            _log = log;
            _log("Enumerator::.ctor");
        }

        public int Current
        {
            get
            {
                _log("Enumerator::Current");
                return _i;
            }
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            _log("Enumerator::MoveNextAsync - Begin");

            await Task.Yield();
            
            _log("Enumerator::MoveNextAsync - Continue");
            
            if (_i >= 10)
            {
                _log("Enumerator::MoveNextAsync - End (false)");

                return false;
            }

            _i++;

            _log("Enumerator::MoveNextAsync - End (true)");

            return true;
        }

        public async ValueTask DisposeAsync()
        {
            _log("Enumerator::DisposeAsync - Begin");
            await Task.Yield();
            _log("Enumerator::DisposeAsync - End");
        }
    }
}
