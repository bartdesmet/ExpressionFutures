// Prototyping extended expression trees for C#.
//
// bartde - February 2020

using System;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Base class for binding information for await operations.
    /// </summary>
    public abstract partial class AwaitInfo
    {
        /// <summary>
        /// Indicates whether the await operation is dynamically bound.
        /// </summary>
        public abstract bool IsDynamic { get; }

        /// <summary>
        /// Gets the type of the object returned by the await operation.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal abstract AwaitInfo Accept(CSharpExpressionVisitor visitor);

        internal abstract void RequiresCanBind(Expression operand);

        internal abstract Expression ReduceGetAwaiter(Expression operand);
        internal abstract Expression ReduceGetResult(Expression awaiter);
        internal abstract Expression ReduceIsCompleted(Expression awaiter);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="AwaitInfo" />.
        /// </summary>
        /// <param name="node">The object to visit.</param>
        /// <returns>The modified object, if it or any subexpression was modified; otherwise, returns the original object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual AwaitInfo VisitAwaitInfo(AwaitInfo node)
        {
            if (node == null)
            {
                return null;
            }

            return node.Accept(this);
        }
    }
}
