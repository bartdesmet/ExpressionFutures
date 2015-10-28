// Prototyping support for interpretation of TryExpression with a fault handler or exception filters.
//
// bartde - October 2015

namespace System.Runtime.CompilerServices
{
    public static class ExceptionHandling
    {
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
