// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    // NB: We only add new factory methods for Try statements because the DLR node is just fine.
    //     However, we need custom factory methods to ensure the expression has type `void`.

    partial class CSharpStatement
    {
        /// <summary>
        /// Creates a <see cref="TryExpression" /> representing a try block with any number of catch statements and neither a fault nor finally block.
        /// </summary>
        /// <param name="body">The body of the try block.</param>
        /// <param name="handlers">The array of zero or more <see cref="CatchBlock" /> expressions representing the catch statements to be associated with the try block.</param>
        /// <returns>The created <see cref="TryExpression" />.</returns>
        public new static TryExpression TryCatch(Expression body, params CatchBlock[] handlers)
        {
            return Expression.MakeTry(typeof(void), body, null, null, handlers);
        }

        /// <summary>Creates a <see cref="TryExpression" /> representing a try block with a finally block and no catch statements.</summary>
        /// <param name="body">The body of the try block.</param>
        /// <param name="finally">The body of the finally block.</param>
        /// <returns>The created <see cref="TryExpression" />.</returns>
        public new static TryExpression TryFinally(Expression body, Expression @finally)
        {
            return Expression.MakeTry(typeof(void), body, @finally, null, null);
        }

        /// <summary>Creates a <see cref="TryExpression" /> representing a try block with any number of catch statements and a finally block.</summary>
        /// <param name="body">The body of the try block.</param>
        /// <param name="finally">The body of the finally block.</param>
        /// <param name="handlers">The array of zero or more <see cref="CatchBlock" /> expressions representing the catch statements to be associated with the try block.</param>
        /// <returns>The created <see cref="TryExpression" />.</returns>
        public new static TryExpression TryCatchFinally(Expression body, Expression @finally, params CatchBlock[] handlers)
        {
            return Expression.MakeTry(typeof(void), body, @finally, null, handlers);
        }
    }
}
