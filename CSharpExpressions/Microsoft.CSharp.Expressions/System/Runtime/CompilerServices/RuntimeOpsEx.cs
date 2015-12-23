// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System.Diagnostics;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a set of runtime helpers.
    /// </summary>
    public static class RuntimeOpsEx
    {
        /// <summary>
        /// Performs a prefix assignment on the specified LHS.
        /// </summary>
        /// <typeparam name="T">The type of the LHS.</typeparam>
        /// <param name="lhs">The LHS to mutate.</param>
        /// <param name="functionalOp">The functional operation to carry out.</param>
        /// <returns>The value of LHS prior to assignment.</returns>
        public static T PreAssignByRef<T>(ref T lhs, Func<T, T> functionalOp)
        {
            return lhs = functionalOp(lhs);
        }

        /// <summary>
        /// Performs a postfix assignment on the specified LHS.
        /// </summary>
        /// <typeparam name="T">The type of the LHS.</typeparam>
        /// <param name="lhs">The LHS to mutate.</param>
        /// <param name="functionalOp">The functional operation to carry out.</param>
        /// <returns>The value of LHS after assignment.</returns>
        public static T PostAssignByRef<T>(ref T lhs, Func<T, T> functionalOp)
        {
            var temp = lhs;
            lhs = functionalOp(lhs);
            return temp;
        }

        /// <summary>
        /// Performs an operation on a by-ref receiver.
        /// </summary>
        /// <typeparam name="T">The type of the receiver.</typeparam>
        /// <typeparam name="R">The type of the result of the operation.</typeparam>
        /// <param name="obj">The receiver to apply the operation to.</param>
        /// <param name="functionalOp">The functional operation to carry out.</param>
        /// <returns>The result of the operation applied to the receiver.</returns>
        public static R WithByRef<T, R>(ref T obj, ByRef<T, R> functionalOp)
        {
            return functionalOp(ref obj);
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
    /// <typeparam name="T">The type of the receiver.</typeparam>
    /// <typeparam name="R">The type of the result of the operation.</typeparam>
    /// <param name="obj">The receiver to apply the operation to.</param>
    /// <returns>The result of the operation applied to the receiver.</returns>
    public delegate R ByRef<T, R>(ref T obj);
}
