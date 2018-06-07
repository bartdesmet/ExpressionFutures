// Prototyping extended expression trees for C#.
//
// bartde - June 2018

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a write-only anonymous expression.
    /// </summary>
    public sealed partial class DiscardCSharpExpression : CSharpExpression
    {
        internal DiscardCSharpExpression(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Discard;

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitDiscard(this);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            // REVIEW: It'd be easier to have discard support in System.Linq.Expressions so we can emit a parameter
            //         expression that does not require to be "in scope" and behaves as expression-to-void conversion,
            //         or a temporary local (e.g. for out parameters).

            // NB: The following reduction works fine to make reduction of C# assignment expressions work. However,
            //     using DiscardCSharpExpression with a regular Expression.Assign factory call won't work because the
            //     System.Linq.Expressions API does not know that this extension node is assignable (i.e. it does not
            //     reduce the node to find out it reduces to a field that's assignable). For out parameters, this node
            //     type works fine with all factory methods because these don't require that the argument passed to an
            //     out parameter is indeed an lval (presumably due to VB compat).

            return Expression.Field(null, typeof(Discard<>).MakeGenericType(Type), "_");
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="DiscardCSharpExpression"/> that represents a write-only anonymous expression.
        /// </summary>
        /// <param name="type">The type of the discard expressio.</param>
        /// <returns>An instance of the <see cref="DiscardCSharpExpression"/>.</returns>
        public static DiscardCSharpExpression Discard(Type type)
        {
            // REVIEW: See remarks in Reduce method above.

            ContractUtils.RequiresNotNull(type, nameof(type));

            TypeUtils.ValidateType(type);

            if (type.IsByRef)
                throw Error.TypeMustNotBeByRef();

            if (type.IsPointer)
                throw Error.TypeMustNotBePointer();

            return new DiscardCSharpExpression(type);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="DiscardCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDiscard(DiscardCSharpExpression node)
        {
            return node;
        }
    }
}
