using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    public readonly struct ValueTaskAwaiter : ICriticalNotifyCompletion
    {
        private readonly ValueTask _value;

        internal ValueTaskAwaiter(in ValueTask value) => _value = value;

        public bool IsCompleted => _value.IsCompleted;

        public void GetResult() => _value.ThrowIfCompletedUnsuccessfully();

        public void OnCompleted(Action continuation)
        {
            if (_value._obj is Task t)
            {
                t.GetAwaiter().OnCompleted(continuation);
            }
            else
            {
                Task.CompletedTask.GetAwaiter().OnCompleted(continuation);
            }
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (_value._obj is Task t)
            {
                t.GetAwaiter().UnsafeOnCompleted(continuation);
            }
            else
            {
                Task.CompletedTask.GetAwaiter().UnsafeOnCompleted(continuation);
            }
        }
    }

    public readonly struct ValueTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly ValueTask<TResult> _value;

        internal ValueTaskAwaiter(in ValueTask<TResult> value) => _value = value;

        public bool IsCompleted => _value.IsCompleted;

        public TResult GetResult() => _value.Result;

        public void OnCompleted(Action continuation)
        {
            if (_value._obj is Task<TResult> t)
            {
                t.GetAwaiter().OnCompleted(continuation);
            }
            else
            {
                Task.CompletedTask.GetAwaiter().OnCompleted(continuation);
            }
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (_value._obj is Task<TResult> t)
            {
                t.GetAwaiter().UnsafeOnCompleted(continuation);
            }
            else
            {
                Task.CompletedTask.GetAwaiter().UnsafeOnCompleted(continuation);
            }
        }
    }
}
