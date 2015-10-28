// Prototyping support for interpretation of TryExpression with a fault handler or exception filters.
//
// bartde - October 2015

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a set of methods used as compilation targets for emulating fault handlers and exception filters.
    /// </summary>
    public static class ExceptionHandling
    {
        /// <summary>
        /// Executes a protected region with an exception filter.
        /// </summary>
        /// <typeparam name="TException">The type of the exception to filter and optionally handle.</typeparam>
        /// <param name="body">Function to execute the body of the protected region.</param>
        /// <param name="filter">Function to determine whether an exception of the specified type can be handled.</param>
        /// <param name="handler">Function to execute upon handling the exception.</param>
        /// <returns>Control flow transfer descriptor.</returns>
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Infrastructure only. Compiled code won't pass null references.")]
        public static LeaveHandlerData TryFilter<TException>(Func<LeaveHandlerData> body, Func<TException, bool> filter, Func<TException, LeaveHandlerData> handler)
            where TException : Exception
        {
            try
            {
                return body();
            }
            catch (TException ex) when (filter(ex))
            {
                return handler(ex);
            }
        }

        /// <summary>
        /// Executes a protected region with a fault handler.
        /// </summary>
        /// <param name="body">Function to execute the body of the protected region.</param>
        /// <param name="handler">Function to execute when an exception has occurred.</param>
        /// <returns>Control flow transfer descriptor.</returns>
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Infrastructure only. Compiled code won't pass null references.")]
        public static LeaveHandlerData TryFault(Func<LeaveHandlerData> body, Action handler)
        {
            var result = default(LeaveHandlerData);
            var success = false;
            try
            {
                result = body();
                success = true;
            }
            finally
            {
                if (!success)
                {
                    handler();
                }
            }

            return result;
        }
    }
}
