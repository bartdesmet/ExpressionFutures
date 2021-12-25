// Prototyping extended expression trees for C#.
//
// bartde - February 2020

using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Microsoft.CSharp.RuntimeBinder;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents binding information for dynamically bound await operations.
    /// </summary>
    public sealed class DynamicAwaitInfo : AwaitInfo
    {
        internal DynamicAwaitInfo(Type context, bool resultDiscarded)
        {
            Context = context;
            ResultDiscarded = resultDiscarded;
        }

        /// <summary>
        /// Indicates whether the await operation is dynamically bound.
        /// </summary>
        public override bool IsDynamic => true;

        /// <summary>
        /// Gets the type of the object returned by the await operation.
        /// </summary>
        public override Type Type => ResultDiscarded ? typeof(void) : typeof(object);

        /// <summary>
        /// Gets the context in which the dynamic operation is bound.
        /// </summary>
        public Type Context { get; }

        /// <summary>
        /// Indicates whether the result of the await operation is discarded.
        /// </summary>
        public bool ResultDiscarded { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override AwaitInfo Accept(CSharpExpressionVisitor visitor) => visitor.VisitAwaitInfo(this);

        internal override void RequiresCanBind(Expression operand)
        {
            // NB: Can always bind dynamically.
        }

        internal override Expression ReduceGetAwaiter(Expression operand)
        {
            // TODO: support ICriticalNotifyCompletion?
            return
                Expression.Convert(
                    DynamicCSharpExpression.DynamicInvokeMember(
                        operand,
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
                    ResultDiscarded ? CSharpBinderFlags.ResultDiscarded : CSharpBinderFlags.None,
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
        /// Creates an object representing a dynamically bound await operation.
        /// </summary>
        /// <param name="context">The context in which the dynamic operation is bound.</param>
        /// <param name="resultDiscarded">Indicates whether the result of the await operation is discarded.</param>
        /// <returns>An object representing binding information for await operations.</returns>
        public static DynamicAwaitInfo DynamicAwaitInfo(Type context, bool resultDiscarded) =>
            // NB: This is the overload the C# compiler binds to.
            new DynamicAwaitInfo(context, resultDiscarded);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="DynamicAwaitInfo" />.
        /// </summary>
        /// <param name="node">The object to visit.</param>
        /// <returns>The modified object, if it or any subexpression was modified; otherwise, returns the original object.</returns>
        protected internal virtual AwaitInfo VisitAwaitInfo(DynamicAwaitInfo node) => node;
    }
}
