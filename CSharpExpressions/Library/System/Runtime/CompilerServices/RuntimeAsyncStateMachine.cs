// Prototyping extended expression trees for C#.
//
// bartde - October 2015

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a runtime implementation of IAsyncStateMachine usable by runtime code generators.
    /// </summary>
    public class RuntimeAsyncStateMachine : IAsyncStateMachine
    {
        private readonly Action _moveNext;

        /// <summary>
        /// Creates a new runtim async state machine using the specified MoveNext implementation delagate.
        /// </summary>
        /// <param name="moveNext">Delegte to implement the MoveNext method.</param>
        public RuntimeAsyncStateMachine(Action moveNext)
        {
            _moveNext = moveNext;
        }

        /// <summary>
        /// Advances the state machine.
        /// </summary>
        public void MoveNext()
        {
            _moveNext();
        }

        /// <summary>
        /// Set the state machine.
        /// </summary>
        /// <param name="stateMachine">State machine.</param>
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }
}
