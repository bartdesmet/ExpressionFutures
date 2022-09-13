// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Xunit;
using System;
using System.Linq;
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
        // TODO: await with spilling of by-ref locals (known limitation)
        // TODO: more stack spilling cases

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void CrossCheck_Async_Spilling_ByRefLocals1()
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

        [Fact]
        public void CrossCheck_Async_Spilling_ByRefLocals2()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var xs = new[] { 41 };
        var res = System.Threading.Interlocked.Exchange(ref xs[Return(int.Parse(""0""))], await Task.FromResult(Return(42)));
    
        Log(""B"");
    
        return res + xs[0];
    });
}");
            f();
        }

        [Fact]
        public void CrossCheck_Async_Spilling_ByRefLocals3()
        {
            var f = Compile<Func<int>>(@"() =>
{
    return Await(async () =>
    {
        Log(""A"");
    
        var b = new WeakBox<int> { Value = 41 };
        var res = System.Threading.Interlocked.Exchange(ref b.Value, await Task.FromResult(Return(42)));
    
        Log(""B"");
    
        return res + b.Value;
    });
}");
            f();
        }

        [Fact]
        public void CrossCheck_Async_Spilling_ByRefReceivers1()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
    
        var t = TimeSpan.FromSeconds(5);
        var res = t.Subtract(await Task.FromResult(TimeSpan.FromSeconds(2)));
    
        Log(""B"");

        Log(res.Ticks);
    });
}");
            f();
        }

        [Fact]
        public void CrossCheck_Async_Spilling_ByRefReceivers2()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
    
        var ts = new[] { TimeSpan.FromSeconds(5) };
        var res = ts[Return(int.Parse(""0""))].Subtract(await Task.FromResult(Return(TimeSpan.FromSeconds(2))));
    
        Log(""B"");

        Log(res.Ticks);
    });
}");
            f();
        }

        [Fact]
        public void CrossCheck_Async_Spilling_ByRefReceivers3()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        Log(""A"");
    
        var b = new WeakBox<TimeSpan> { Value = TimeSpan.FromSeconds(5) };
        var res = b.Value.Subtract(await Task.FromResult(Return(TimeSpan.FromSeconds(2))));
    
        Log(""B"");

        Log(res.Ticks);
    });
}");
            f();
        }

        [Fact]
        public void CrossCheck_Async_Spilling_MemberInit_Assign1()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        var res = new WeakBox<int> { Value = await Task.FromResult(Return(42)) };
    
        Log(res.Value);
    });
}");
            f();
        }

        [Fact]
        public void CrossCheck_Async_Spilling_MemberInit_Assign2()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        var res = new WeakBox<string> { Value = await Task.FromResult(Return(""bar"")) };
    
        Log(res.Value);
    });
}");
            f();
        }

        [Fact]
        public void CrossCheck_Async_Spilling_MemberInit_Member()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        var res = new WeakBox<WeakBox<int>> { Value = { Value = await Task.FromResult(Return(42)) } };
    
        Log(res.Value.Value);
    });
}");
            f();
        }

        [Fact]
        public void CrossCheck_Async_Spilling_MemberInit_ManOrBoy()
        {
            var f = Compile<Action>(@"() =>
{
    AwaitVoid(async () =>
    {
        var res = new MemberInitStruct(Log<string>)
        {
            X = await Task.FromResult(Return(1)),
            Z =
            {
                A = await Task.FromResult(Return(2)),
                B = await Task.FromResult(Return(3))
            },
            Y = await Task.FromResult(Return(4)),
            XS =
            {
                await Task.FromResult(Return(5))
            }
        };

        Log($""{res.X},{res.Y},{res.Z.A},{res.Z.B},{res.XS[0]}"");
    });
}");
            f();
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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
            f(false);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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
    }
}
