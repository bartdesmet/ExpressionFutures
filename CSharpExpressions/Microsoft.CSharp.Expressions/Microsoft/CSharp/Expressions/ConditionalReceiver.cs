// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    //NB: We use a custom node instead of a ParameterExpression in order to ensure it's not writeable and such that we
    //    don't have to worry scoping rules for variables.

    /// <summary>
    /// Represents the non-null receiver of a null-conditional access operation.
    /// </summary>
    public sealed partial class ConditionalReceiver : CSharpExpression
    {
        internal ConditionalReceiver(Type type) => Type = type;

        /// <summary>
        /// Gets the static type of the expression that this <see cref="System.Linq.Expressions.Expression" /> represents. (Inherited from <see cref="System.Linq.Expressions.Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.ConditionalReceiver;

        /// <summary>
        /// Gets a value that indicates whether the expression tree node can be reduced. 
        /// </summary>
        public override bool CanReduce => false;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitConditionalReceiver(this);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ConditionalReceiver" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitConditionalReceiver(ConditionalReceiver node) => node;
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="Expressions.ConditionalReceiver"/> representing the non-null receiver of a null-conditional access operation.
        /// </summary>
        /// <param name="type">The type of the non-null receiver.</param>
        /// <returns>A <see cref="Expressions.ConditionalReceiver"/> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalReceiver" /> and the <see cref="Expression.Type" /> property equal to the specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static ConditionalReceiver ConditionalReceiver(Type type)
        {
            RequiresNotNull(type, nameof(type));

            if (type == typeof(void) || type.IsByRef || type.IsNullableType())
                throw Error.InvalidConditionalReceiverType(type);

            return new ConditionalReceiver(type);
        }
    }
}
