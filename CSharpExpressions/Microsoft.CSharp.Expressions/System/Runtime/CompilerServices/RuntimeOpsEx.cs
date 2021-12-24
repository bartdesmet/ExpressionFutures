// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a set of runtime helpers.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "To be merged with BCL type.")]
    public static class RuntimeOpsEx
    {
        /// <summary>
        /// Performs a prefix assignment on the specified LHS.
        /// </summary>
        /// <typeparam name="TObject">The type of the LHS.</typeparam>
        /// <param name="lhs">The LHS to mutate.</param>
        /// <param name="functionalOp">The functional operation to carry out.</param>
        /// <returns>The value of LHS prior to assignment.</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Required for functionality.")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Called by generated code with non-null reference.")]
        public static TObject PreAssignByRef<TObject>(ref TObject lhs, Func<TObject, TObject> functionalOp)
        {
            return lhs = functionalOp(lhs);
        }

        /// <summary>
        /// Performs a postfix assignment on the specified LHS.
        /// </summary>
        /// <typeparam name="TObject">The type of the LHS.</typeparam>
        /// <param name="lhs">The LHS to mutate.</param>
        /// <param name="functionalOp">The functional operation to carry out.</param>
        /// <returns>The value of LHS after assignment.</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Required for functionality.")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Called by generated code with non-null reference.")]
        public static TObject PostAssignByRef<TObject>(ref TObject lhs, Func<TObject, TObject> functionalOp)
        {
            var temp = lhs;
            lhs = functionalOp(lhs);
            return temp;
        }

        /// <summary>
        /// Performs an operation on a by-ref receiver.
        /// </summary>
        /// <typeparam name="TReceiver">The type of the receiver.</typeparam>
        /// <typeparam name="TResult">The type of the result of the operation.</typeparam>
        /// <param name="obj">The receiver to apply the operation to.</param>
        /// <param name="functionalOp">The functional operation to carry out.</param>
        /// <returns>The result of the operation applied to the receiver.</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Required for functionality.")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Called by generated code with non-null reference.")]
        public static TResult WithByRef<TReceiver, TResult>(ref TReceiver obj, FuncByRef<TReceiver, TResult> functionalOp)
        {
            return functionalOp(ref obj);
        }

        /// <summary>
        /// Slices the specified array using the specified range.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <param name="array">The array to slice.</param>
        /// <param name="range">An object that determines the portion of <paramref name="array"/> to include in the slice.</param>
        /// <returns>The subarray defined by <paramref name="range"/>.</returns>
        public static T[] GetSubArray<T>(T[] array, Range range)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            (int offset, int length) = range.GetOffsetAndLength(array.Length);

            if (typeof(T).IsValueType || typeof(T[]) == array.GetType())
            {
                // We know the type of the array to be exactly T[].

                if (length == 0)
                {
                    return Array.Empty<T>();
                }

                var dest = new T[length];
                Array.Copy(array, offset, dest, 0, length);
                return dest;
            }
            else
            {
                // The array is actually a U[] where U:T.
                T[] dest = (T[])Array.CreateInstance(array.GetType().GetElementType()!, length);
                Array.Copy(array, offset, dest, 0, length);
                return dest;
            }
        }

#if ENABLE_CALLERINFO

        /// <summary>
        /// Gets the caller member name.
        /// </summary>
        /// <returns>The caller member name, if found; otherwise, null.</returns>
        public static string GetCallerMemberName()
        {
            return new StackTrace(1, true).GetFrame(0)?.GetMethod()?.Name;
        }

        /// <summary>
        /// Gets the caller line number.
        /// </summary>
        /// <returns>The caller line number, if found; otherwise, null.</returns>
        public static int? GetCallerLineNumber()
        {
            var line = new StackTrace(1, true).GetFrame(0)?.GetFileLineNumber();

            // NB: Lines are one-based; if missing, 0 is returned.
            if (line == 0)
            {
                line = null;
            }

            return line;
        }

        /// <summary>
        /// Gets the caller file path.
        /// </summary>
        /// <returns>The caller file path, if found; otherwise, null.</returns>
        public static string GetCallerFilePath()
        {
            return new StackTrace(1, true).GetFrame(0)?.GetFileName();
        }

#endif
    }

    /// <summary>
    /// Delegate for an operation applied to a by-ref receiver.
    /// </summary>
    /// <typeparam name="TReceiver">The type of the receiver.</typeparam>
    /// <typeparam name="TResult">The type of the result of the operation.</typeparam>
    /// <param name="obj">The receiver to apply the operation to.</param>
    /// <returns>The result of the operation applied to the receiver.</returns>
    public delegate TResult FuncByRef<TReceiver, TResult>(ref TReceiver obj);
}
