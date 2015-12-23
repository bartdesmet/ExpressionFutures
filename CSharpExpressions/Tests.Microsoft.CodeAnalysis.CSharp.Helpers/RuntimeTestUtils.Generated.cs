// Prototyping extended expression trees for C#.
//
// bartde - December 2015

// NB: These get included in the runtime compilations of snippets to evaluate using Roslyn.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class AwaitableClassWithAwaiterClass
{
    private readonly AwaitResult _result;

    public AwaitableClassWithAwaiterClass(AwaitResult result)
    {
        _result = result;
    }

    public AwaiterClass GetAwaiter()
    {
        return new AwaiterClass(_result);
    }
}

public class AwaitableClassWithAwaiterStruct
{
    private readonly AwaitResult _result;

    public AwaitableClassWithAwaiterStruct(AwaitResult result)
    {
        _result = result;
    }

    public AwaiterStruct GetAwaiter()
    {
        return new AwaiterStruct(_result);
    }
}

public struct AwaitableStructWithAwaiterClass
{
    private readonly AwaitResult _result;

    public AwaitableStructWithAwaiterClass(AwaitResult result)
    {
        _result = result;
    }

    public AwaiterClass GetAwaiter()
    {
        return new AwaiterClass(_result);
    }
}

public struct AwaitableStructWithAwaiterStruct
{
    private readonly AwaitResult _result;

    public AwaitableStructWithAwaiterStruct(AwaitResult result)
    {
        _result = result;
    }

    public AwaiterStruct GetAwaiter()
    {
        return new AwaiterStruct(_result);
    }
}

public class AwaiterClass : INotifyCompletion
{
    private readonly AwaitResult _result;

    public AwaiterClass(AwaitResult result)
    {
        _result = result;
    }

    public bool IsCompleted => _result.IsCompleted;
    public void GetResult() => _result.GetResult();
    public void OnCompleted(Action continuation) => _result.OnCompleted(continuation);
}

public struct AwaiterStruct : INotifyCompletion
{
    private readonly AwaitResult _result;

    public AwaiterStruct(AwaitResult result)
    {
        _result = result;
    }

    public bool IsCompleted => _result.IsCompleted;
    public void GetResult() => _result.GetResult();
    public void OnCompleted(Action continuation) => _result.OnCompleted(continuation);
}

public class AwaitableClassWithAwaiterClass<T>
{
    private readonly AwaitResult<T> _result;

    public AwaitableClassWithAwaiterClass(AwaitResult<T> result)
    {
        _result = result;
    }

    public AwaiterClass<T> GetAwaiter()
    {
        return new AwaiterClass<T>(_result);
    }
}

public class AwaitableClassWithAwaiterStruct<T>
{
    private readonly AwaitResult<T> _result;

    public AwaitableClassWithAwaiterStruct(AwaitResult<T> result)
    {
        _result = result;
    }

    public AwaiterStruct<T> GetAwaiter()
    {
        return new AwaiterStruct<T>(_result);
    }
}

public struct AwaitableStructWithAwaiterClass<T>
{
    private readonly AwaitResult<T> _result;

    public AwaitableStructWithAwaiterClass(AwaitResult<T> result)
    {
        _result = result;
    }

    public AwaiterClass<T> GetAwaiter()
    {
        return new AwaiterClass<T>(_result);
    }
}

public struct AwaitableStructWithAwaiterStruct<T>
{
    private readonly AwaitResult<T> _result;

    public AwaitableStructWithAwaiterStruct(AwaitResult<T> result)
    {
        _result = result;
    }

    public AwaiterStruct<T> GetAwaiter()
    {
        return new AwaiterStruct<T>(_result);
    }
}

public class AwaiterClass<T> : INotifyCompletion
{
    private readonly AwaitResult<T> _result;

    public AwaiterClass(AwaitResult<T> result)
    {
        _result = result;
    }

    public bool IsCompleted => _result.IsCompleted;
    public T GetResult() => _result.GetResult();
    public void OnCompleted(Action continuation) => _result.OnCompleted(continuation);
}

public struct AwaiterStruct<T> : INotifyCompletion
{
    private readonly AwaitResult<T> _result;

    public AwaiterStruct(AwaitResult<T> result)
    {
        _result = result;
    }

    public bool IsCompleted => _result.IsCompleted;
    public T GetResult() => _result.GetResult();
    public void OnCompleted(Action continuation) => _result.OnCompleted(continuation);
}

public class AwaitResult
{
    public AwaitResult(AwaitTiming timing, Exception error = null)
    {
        Timing = timing;
        Error = error;
    }

    public AwaitTiming Timing { get; }
    public Exception Error { get; }
    
    public bool IsCompleted => Timing == AwaitTiming.Synchronous;

    public void OnCompleted(Action continuation)
    {
        switch (Timing)
        {
            case AwaitTiming.Synchronous:
            case AwaitTiming.Racing:
                continuation();
                break;
            case AwaitTiming.Asynchronous:
                Task.Run(continuation);
                break;
        }
    }

    public void GetResult()
    {
        if (Error != null)
            throw Error;
    }
}

public class AwaitResult<T> : AwaitResult
{
    public AwaitResult(AwaitTiming timing, T result = default(T), Exception error = null)
        : base(timing, error)
    {
        Result = result;
    }

    public T Result { get; }

    public new T GetResult()
    {
        if (Error != null)
            throw Error;

        return Result;
    }
}


public enum AwaitTiming
{
    Synchronous,
    Asynchronous,
    Racing,
}