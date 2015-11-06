// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a conditional (null-propagating) access to an indexer.
    /// </summary>
    public sealed class ConditionalIndexCSharpExpression : ConditionalAccessCSharpExpression
    {
        internal ConditionalIndexCSharpExpression(Expression expression, PropertyInfo indexer, ReadOnlyCollection<ParameterAssignment> arguments)
            : base(expression)
        {
            Indexer = indexer;
            Arguments = arguments;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.ConditionalIndex;

        /// <summary>
        /// Gets the <see cref="Expression" /> that represents the object to index.
        /// </summary>
        public Expression Object => Expression; // NB: Just an alias for familiarity with IndexExpression

        /// <summary>
        /// Gets the <see cref="PropertyInfo" /> for the indexer property.
        /// </summary>
        public PropertyInfo Indexer { get; }

        /// <summary>
        /// Gets a collection of argument assignments.
        /// </summary>
        public ReadOnlyCollection<ParameterAssignment> Arguments { get; }

        /// <summary>
        /// Gets the result type of the underlying access.
        /// </summary>
        protected override Type UnderlyingType => Indexer.PropertyType;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitConditionalIndex(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="object">The <see cref="Object" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ConditionalIndexCSharpExpression Update(Expression @object, IEnumerable<ParameterAssignment> arguments)
        {
            if (@object == Object && arguments == Arguments)
            {
                return this;
            }

            return CSharpExpression.ConditionalIndex(@object, Indexer, arguments);
        }

        /// <summary>
        /// Reduces the expression to an unconditional non-null access on the specified expression.
        /// </summary>
        /// <param name="nonNull">Non-null expression to apply the access to.</param>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceAccess(Expression nonNull) => CSharpExpression.Index(nonNull, Indexer, Arguments);
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="ConditionalIndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="MethodInfo" /> representing an accessor of the property to index.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> that represents the indexer arguments.</param>
        /// <returns>A <see cref="ConditionalIndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalIndex" /> and the <see cref="ConditionalIndexCSharpExpression.Object" />, <see cref="ConditionalIndexCSharpExpression.Indexer" />, and <see cref="ConditionalIndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static ConditionalIndexCSharpExpression ConditionalIndex(Expression instance, MethodInfo indexer, params ParameterAssignment[] arguments)
        {
            return ConditionalIndex(instance, indexer, (IEnumerable<ParameterAssignment>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="ConditionalIndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="MethodInfo" /> representing an accessor of the property to index.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="ConditionalIndexCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="ConditionalIndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalIndex" /> and the <see cref="ConditionalIndexCSharpExpression.Object" />, <see cref="ConditionalIndexCSharpExpression.Indexer" />, and <see cref="ConditionalIndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static ConditionalIndexCSharpExpression ConditionalIndex(Expression instance, MethodInfo indexer, IEnumerable<ParameterAssignment> arguments)
        {
            ContractUtils.RequiresNotNull(indexer, nameof(indexer));

            var property = GetProperty(indexer);
            return ConditionalIndexCore(instance, property, indexer.GetParametersCached(), arguments);
        }

        /// <summary>
        /// Creates a <see cref="ConditionalIndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="PropertyInfo" /> representing the property to index.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> that represents the indexer arguments.</param>
        /// <returns>A <see cref="ConditionalIndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalIndex" /> and the <see cref="ConditionalIndexCSharpExpression.Object" />, <see cref="ConditionalIndexCSharpExpression.Indexer" />, and <see cref="ConditionalIndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static ConditionalIndexCSharpExpression ConditionalIndex(Expression instance, PropertyInfo indexer, params ParameterAssignment[] arguments)
        {
            return ConditionalIndex(instance, indexer, (IEnumerable<ParameterAssignment>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="ConditionalIndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="PropertyInfo" /> representing the property to index.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="ConditionalIndexCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="ConditionalIndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalIndex" /> and the <see cref="ConditionalIndexCSharpExpression.Object" />, <see cref="ConditionalIndexCSharpExpression.Indexer" />, and <see cref="ConditionalIndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static ConditionalIndexCSharpExpression ConditionalIndex(Expression instance, PropertyInfo indexer, IEnumerable<ParameterAssignment> arguments)
        {
            return ConditionalIndexCore(instance, indexer, null, arguments);
        }

        private static ConditionalIndexCSharpExpression ConditionalIndexCore(Expression instance, PropertyInfo indexer, ParameterInfo[] parameters, IEnumerable<ParameterAssignment> arguments)
        {
            ContractUtils.RequiresNotNull(instance, nameof(instance));
            ContractUtils.RequiresNotNull(indexer, nameof(indexer));

            RequiresCanRead(instance, nameof(instance));

            var argList = arguments.ToReadOnly();

            var type = instance.Type.GetNonNullableType();
            ValidateIndexer(type, indexer, ref parameters, argList);

            return new ConditionalIndexCSharpExpression(instance, indexer, argList);
        }

        // TODO: add overloads with just Expression[] or IEnumerable<Expression>
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ConditionalIndexCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitConditionalIndex(ConditionalIndexCSharpExpression node)
        {
            return node.Update(Visit(node.Object), Visit(node.Arguments, VisitParameterAssignment));
        }
    }
}
