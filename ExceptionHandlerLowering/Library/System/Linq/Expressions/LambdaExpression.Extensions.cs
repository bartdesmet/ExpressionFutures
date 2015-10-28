namespace System.Linq.Expressions
{
    /// <summary>
    /// Provides a set of extension methods for LambdaExpression and Expression{T}.
    /// </summary>
    public static class LambdaExpressionExtensions
    {
        /// <summary>
        /// Compiles the specified <paramref name="expression"/> with the exception handling lowering rewrite.
        /// </summary>
        /// <param name="expression">The expression to compile.</param>
        /// <returns>Compiled delegate for the specified <paramref name="expression"/>.</returns>
        public static Delegate CompileWithExceptionHandling(this LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return ExceptionHandlingLowering.Instance.VisitAndConvert(expression, nameof(CompileWithExceptionHandling)).Compile();
        }

        /// <summary>
        /// Compiles the specified <paramref name="expression"/> with the exception handling lowering rewrite.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate to compile.</typeparam>
        /// <param name="expression">The expression to compile.</param>
        /// <returns>Compiled delegate for the specified <paramref name="expression"/>.</returns>
        public static TDelegate CompileWithExceptionHandling<TDelegate>(this Expression<TDelegate> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return ExceptionHandlingLowering.Instance.VisitAndConvert(expression, nameof(CompileWithExceptionHandling)).Compile();
        }
    }
}
