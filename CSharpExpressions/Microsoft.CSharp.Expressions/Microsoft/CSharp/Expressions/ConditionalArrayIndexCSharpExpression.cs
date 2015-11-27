// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    // DESIGN: How about ConditionalArrayAccess which returns ConditionalIndexCSharpExpression?

    /// <summary>
    /// Represents a conditional (null-propagating) access to an array.
    /// </summary>
    public sealed partial class ConditionalArrayIndexCSharpExpression : OldConditionalAccessCSharpExpression
    {
        internal ConditionalArrayIndexCSharpExpression(Expression array, ReadOnlyCollection<Expression> indexes)
            : base(array)
        {
            Indexes = indexes;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.ConditionalArrayIndex;

        /// <summary>
        /// Gets the <see cref="Expression" /> that represents the array to index.
        /// </summary>
        public Expression Array => Expression; // NB: Just an alias

        /// <summary>
        /// Gets a collection of argument assignments.
        /// </summary>
        public ReadOnlyCollection<Expression> Indexes { get; }

        /// <summary>
        /// Gets the result type of the underlying access.
        /// </summary>
        protected override Type UnderlyingType => Array.Type.GetElementType();

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitConditionalArrayIndex(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="array">The <see cref="Array" /> property of the result.</param>
        /// <param name="indexes">The <see cref="Indexes" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ConditionalArrayIndexCSharpExpression Update(Expression array, IEnumerable<Expression> indexes)
        {
            if (array == Array && indexes == Indexes)
            {
                return this;
            }

            return CSharpExpression.ConditionalArrayIndex(array, indexes);
        }

        /// <summary>
        /// Reduces the expression to an unconditional non-null access on the specified expression.
        /// </summary>
        /// <param name="nonNull">Non-null expression to apply the access to.</param>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceAccess(Expression nonNull) => Indexes.Count > 1 ? (Expression)CSharpExpression.ArrayIndex(nonNull, Indexes) : (Expression)CSharpExpression.ArrayIndex(nonNull, Indexes[0]);
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="ConditionalArrayIndexCSharpExpression" /> that represents accessing an element in an array.
        /// </summary>
        /// <param name="array">An <see cref="Expression" /> that specifies the array to index.</param>
        /// <param name="indexes">An array of one or more of <see cref="Expression" /> objects that represent the indexes.</param>
        /// <returns>A <see cref="ConditionalArrayIndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalArrayIndex" /> and the <see cref="ConditionalArrayIndexCSharpExpression.Array" /> and <see cref="ConditionalArrayIndexCSharpExpression.Indexes" /> properties set to the specified values.</returns>
        public static ConditionalArrayIndexCSharpExpression ConditionalArrayIndex(Expression array, params Expression[] indexes)
        {
            return ConditionalArrayIndex(array, (IEnumerable<Expression>)indexes);
        }

        /// <summary>
        /// Creates a <see cref="ConditionalArrayIndexCSharpExpression" /> that represents accessing an element in an array.
        /// </summary>
        /// <param name="array">An <see cref="Expression" /> that specifies the array to index.</param>
        /// <param name="indexes">An <see cref="IEnumerable{T}" /> that contains <see cref="Expression" /> objects to use to populate the <see cref="ConditionalArrayIndexCSharpExpression.Indexes" /> collection.</param>
        /// <returns>A <see cref="ConditionalArrayIndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalArrayIndex" /> and the <see cref="ConditionalArrayIndexCSharpExpression.Array" /> and <see cref="ConditionalArrayIndexCSharpExpression.Indexes" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static ConditionalArrayIndexCSharpExpression ConditionalArrayIndex(Expression array, IEnumerable<Expression> indexes)
        {
            RequiresCanRead(array, nameof(array));
            ContractUtils.RequiresNotNull(indexes, nameof(indexes));

            if (!array.Type.IsArray)
            {
                throw LinqError.ArgumentMustBeArray();
            }

            var indexList = indexes.ToReadOnly();

            if (array.Type.GetArrayRank() != indexList.Count)
            {
                throw LinqError.IncorrectNumberOfIndexes();
            }

            foreach (var index in indexList)
            {
                RequiresCanRead(index, "indexes");

                if (index.Type != typeof(int))
                {
                    throw LinqError.ArgumentMustBeArrayIndexType();
                }
            }

            return new ConditionalArrayIndexCSharpExpression(array, indexList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ConditionalArrayIndexCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitConditionalArrayIndex(ConditionalArrayIndexCSharpExpression node)
        {
            return node.Update(Visit(node.Array), Visit(node.Indexes));
        }
    }
}
