using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    [AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder))]
    public struct ValueTask
    {
        internal readonly object _obj;

        public ValueTask(Task task)
        {
            _obj = task;
        }

        public bool IsCompleted => _obj switch
        {
            null => true,
            Task t => t.IsCompleted,
            _ => false,
        };

        public ValueTaskAwaiter GetAwaiter() => new ValueTaskAwaiter(in this);

        internal void ThrowIfCompletedUnsuccessfully()
        {
            if (_obj is Task t)
            {
                t.GetAwaiter().GetResult();
            }
        }
    }

    [AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder<>))]
    public struct ValueTask<TResult>
    {
        internal readonly object _obj;
        private readonly TResult _result;

        public ValueTask(TResult result)
        {
            _result = result;
            _obj = null;
        }

        public ValueTask(Task<TResult> task)
        {
            _obj = task;
            _result = default;
        }

        public bool IsCompleted => _obj switch
        {
            null => true,
            Task<TResult> t => t.IsCompleted,
            _ => false,
        };

        public TResult Result => _obj switch
        {
            Task<TResult> t => t.Result,
            _ => _result,
        };

        public ValueTaskAwaiter<TResult> GetAwaiter() => new ValueTaskAwaiter<TResult>(in this);
    }
}
