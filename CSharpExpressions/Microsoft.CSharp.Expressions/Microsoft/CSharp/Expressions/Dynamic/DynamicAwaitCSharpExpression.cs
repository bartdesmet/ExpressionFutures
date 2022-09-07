// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates an <see cref="AwaitCSharpExpression"/> that represents awaiting an asynchronous operation.
        /// </summary>
        /// <param name="operand">An <see cref="Expression" /> that specifies the asynchronous operation to await.</param>
        /// <returns>An instance of the <see cref="AwaitCSharpExpression"/>.</returns>
        public static AwaitCSharpExpression DynamicAwait(Expression operand) =>
            DynamicAwait(operand, resultDiscarded: false, context: null);

        /// <summary>
        /// Creates an <see cref="AwaitCSharpExpression"/> that represents awaiting an asynchronous operation.
        /// </summary>
        /// <param name="operand">An <see cref="Expression" /> that specifies the asynchronous operation to await.</param>
        /// <param name="resultDiscarded">Indicates whether the result of the await operation is discarded, causing the expression to have type <see cref="System.Void"/>.</param>
        /// <returns>An instance of the <see cref="AwaitCSharpExpression"/>.</returns>
        public static AwaitCSharpExpression DynamicAwait(Expression operand, bool resultDiscarded) =>
            DynamicAwait(operand, resultDiscarded, context: null);

        /// <summary>
        /// Creates an <see cref="AwaitCSharpExpression"/> that represents awaiting an asynchronous operation.
        /// </summary>
        /// <param name="operand">An <see cref="Expression" /> that specifies the asynchronous operation to await.</param>
        /// <param name="resultDiscarded">Indicates whether the result of the await operation is discarded, causing the expression to have type <see cref="System.Void"/>.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>An instance of the <see cref="AwaitCSharpExpression"/>.</returns>
        public static AwaitCSharpExpression DynamicAwait(Expression operand, bool resultDiscarded, Type? context)
        {
            RequiresCanRead(operand, nameof(operand));

            return Await(operand, DynamicAwaitInfo(context, resultDiscarded));
        }
    }
}
