// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    // NB: The naming convention differs from other dynamic nodes because of the inheritance hierarchy.
    //     All this node does is reduce the await pattern into dynamic operations.

    /// <summary>
    /// Represents an expression that awaits an asynchronous operation using a dynamically typed operand.
    /// </summary>
    public sealed class DynamicAwaitCSharpExpression : AwaitCSharpExpression
    {
        internal DynamicAwaitCSharpExpression(Expression operand, Type context)
            : base(operand, null, typeof(object))
        {
            Context = context;
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type" /> that indicates where this operation is used.
        /// </summary>
        public Type Context { get; }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children.
        /// </summary>
        /// <param name="operand">The <see cref="UnaryCSharpExpression.Operand" /> property of the result.</param>
        /// <returns>An expression with the updated children.</returns>
        protected internal override AwaitCSharpExpression Rewrite(Expression operand)
        {
            return DynamicCSharpExpression.DynamicAwait(operand);
        }

        internal override Expression ReduceGetAwaiter()
        {
            // TODO: support ICriticalNotifyCompletion?
            return
                Expression.Convert(
                    DynamicCSharpExpression.DynamicInvokeMember(
                        Operand,
                        "GetAwaiter",
                        Array.Empty<Type>(),
                        Array.Empty<DynamicCSharpArgument>(),
                        CSharpBinderFlags.None,
                        Context
                    ),
                    typeof(INotifyCompletion)
                );
        }

        internal override Expression ReduceGetResult(Expression awaiter)
        {
            return 
                DynamicCSharpExpression.DynamicInvokeMember(
                    awaiter,
                    "GetResult",
                    Array.Empty<Type>(),
                    Array.Empty<DynamicCSharpArgument>(),
                    CSharpBinderFlags.None,
                    Context
                );
        }

        internal override Expression ReduceIsCompleted(Expression awaiter)
        {
            return
                DynamicCSharpExpression.DynamicConvert(
                    DynamicCSharpExpression.DynamicGetMember(
                        awaiter,
                        "IsCompleted",
                        Array.Empty<DynamicCSharpArgument>(),
                        CSharpBinderFlags.None,
                        Context
                    ),
                    typeof(bool),
                    CSharpBinderFlags.ConvertExplicit,
                    Context
                );
        }
    }

    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates an <see cref="DynamicAwaitCSharpExpression"/> that represents awaiting an asynchronous operation.
        /// </summary>
        /// <param name="operand">An <see cref="Expression" /> that specifies the asynchronous operation to await.</param>
        /// <returns>An instance of the <see cref="DynamicAwaitCSharpExpression"/>.</returns>
        public static DynamicAwaitCSharpExpression DynamicAwait(Expression operand)
        {
            return DynamicAwait(operand, null);
        }

        /// <summary>
        /// Creates an <see cref="DynamicAwaitCSharpExpression"/> that represents awaiting an asynchronous operation.
        /// </summary>
        /// <param name="operand">An <see cref="Expression" /> that specifies the asynchronous operation to await.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>An instance of the <see cref="DynamicAwaitCSharpExpression"/>.</returns>
        public static DynamicAwaitCSharpExpression DynamicAwait(Expression operand, Type context)
        {
            RequiresCanRead(operand, nameof(operand));

            return new DynamicAwaitCSharpExpression(operand, context);
        }
    }
}
