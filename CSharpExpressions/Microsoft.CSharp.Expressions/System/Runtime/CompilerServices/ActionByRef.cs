// Prototyping extended expression trees for C#.
//
// bartde - January 2022

#nullable enable

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Delegate for an operation applied to a by-ref receiver.
    /// </summary>
    /// <typeparam name="TReceiver">The type of the receiver.</typeparam>
    /// <typeparam name="TArgument">The type of the argument.</typeparam>
    /// <param name="obj">The receiver to apply the operation to.</param>
    /// <param name="arg">The argument to pass to the operation.</param>
    public delegate void ActionByRef<TReceiver, TArgument>(ref TReceiver obj, TArgument arg);
}
