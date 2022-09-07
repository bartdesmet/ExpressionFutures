// Prototyping extended expression trees for C#.
//
// bartde - December 2015

#nullable enable

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Delegate for an operation applied to a by-ref receiver.
    /// </summary>
    /// <typeparam name="TReceiver">The type of the receiver.</typeparam>
    /// <typeparam name="TResult">The type of the result of the operation.</typeparam>
    /// <param name="obj">The receiver to apply the operation to.</param>
    /// <returns>The result of the operation applied to the receiver.</returns>
    public delegate TResult FuncByRef<TReceiver, TResult>(ref TReceiver obj);
}
