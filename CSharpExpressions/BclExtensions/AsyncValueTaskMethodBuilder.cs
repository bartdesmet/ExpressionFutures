using System.Management.Instrumentation;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    // NB: These are minimal, but functional, stubs. They simply wrap around the Task<T>-based builder types.

    public struct AsyncValueTaskMethodBuilder
    {
        private AsyncTaskMethodBuilder _builder;

        private AsyncValueTaskMethodBuilder(bool _) => _builder = new AsyncTaskMethodBuilder();

        public static AsyncValueTaskMethodBuilder Create() => new AsyncValueTaskMethodBuilder(false);

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
            => _builder.Start(ref stateMachine);

        public void SetStateMachine(IAsyncStateMachine stateMachine) => throw new NotSupportedException();

        public void SetResult() => _builder.SetResult();

        public void SetException(Exception exception) => _builder.SetException(exception);

        public ValueTask Task => new ValueTask(_builder.Task);

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
            => _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
            => _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
    }

    public struct AsyncValueTaskMethodBuilder<TResult>
    {
        private AsyncTaskMethodBuilder<TResult> _builder;

        private AsyncValueTaskMethodBuilder(bool _) => _builder = new AsyncTaskMethodBuilder<TResult>();

        public static AsyncValueTaskMethodBuilder<TResult> Create() => new AsyncValueTaskMethodBuilder<TResult>(false);

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
            => _builder.Start(ref stateMachine);

        public void SetStateMachine(IAsyncStateMachine stateMachine) => throw new NotSupportedException();

        public void SetResult(TResult result) => _builder.SetResult(result);

        public void SetException(Exception exception) => _builder.SetException(exception);

        public ValueTask<TResult> Task => new ValueTask<TResult>(_builder.Task);

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
            => _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
            => _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
    }
}
