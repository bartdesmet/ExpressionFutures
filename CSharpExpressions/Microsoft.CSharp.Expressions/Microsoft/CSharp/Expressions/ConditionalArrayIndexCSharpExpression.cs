// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    // DESIGN: How about ConditionalArrayAccess which returns ConditionalIndexCSharpExpression?

    /// <summary>
    /// Represents a conditional (null-propagating) access to an array.
    /// </summary>
    public sealed partial class ConditionalArrayIndexCSharpExpression : ConditionalAccessCSharpExpression<IndexExpression>
    {
        internal ConditionalArrayIndexCSharpExpression(Expression array, ReadOnlyCollection<Expression> indexes)
            : this(array, MakeReceiver(array), indexes)
        {
        }

        private ConditionalArrayIndexCSharpExpression(Expression array, ConditionalReceiver receiver, ReadOnlyCollection<Expression> indexes)
            : this(array, receiver, MakeAccess(receiver, indexes))
        {
        }

        private ConditionalArrayIndexCSharpExpression(Expression array, ConditionalReceiver receiver, IndexExpression access)
            : base(array, receiver, access)
        {
        }

        private static IndexExpression MakeAccess(ConditionalReceiver receiver, ReadOnlyCollection<Expression> indexes) =>
            Expression.ArrayAccess(receiver, indexes); // TODO: call ctor directly

        /// <summary>
        /// Gets the <see cref="Expression" /> that represents the array to index.
        /// </summary>
        public Expression Array => Receiver; // NB: Just an alias

        /// <summary>
        /// Gets a collection of argument assignments.
        /// </summary>
        public ReadOnlyCollection<Expression> Indexes => WhenNotNull.Arguments;

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="array">The <see cref="Array" /> property of the result.</param>
        /// <param name="indexes">The <see cref="Indexes" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ConditionalArrayIndexCSharpExpression Update(Expression array, IEnumerable<Expression> indexes)
        {
            if (array == Array && SameElements(ref indexes, Indexes))
            {
                return this;
            }

            return CSharpExpression.ConditionalArrayIndex(array, indexes);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        internal override Expression AcceptConditionalAccess(CSharpExpressionVisitor visitor) => visitor.VisitConditionalArrayIndex(this);

        internal override ConditionalAccessCSharpExpression<IndexExpression> Rewrite(Expression receiver, ConditionalReceiver nonNullReceiver, IndexExpression whenNotNull) =>
            new ConditionalArrayIndexCSharpExpression(receiver, nonNullReceiver, whenNotNull);
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="ConditionalArrayIndexCSharpExpression" /> that represents accessing an element in an array.
        /// </summary>
        /// <param name="array">An <see cref="Expression" /> that specifies the array to index.</param>
        /// <param name="indexes">An array of one or more of <see cref="Expression" /> objects that represent the indexes.</param>
        /// <returns>A <see cref="ConditionalArrayIndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalAccess" /> and the <see cref="ConditionalArrayIndexCSharpExpression.Array" /> and <see cref="ConditionalArrayIndexCSharpExpression.Indexes" /> properties set to the specified values.</returns>
        public static ConditionalArrayIndexCSharpExpression ConditionalArrayIndex(Expression array, params Expression[] indexes) => ConditionalArrayIndex(array, (IEnumerable<Expression>)indexes);

        /// <summary>
        /// Creates a <see cref="ConditionalArrayIndexCSharpExpression" /> that represents accessing an element in an array.
        /// </summary>
        /// <param name="array">An <see cref="Expression" /> that specifies the array to index.</param>
        /// <param name="indexes">An <see cref="IEnumerable{T}" /> that contains <see cref="Expression" /> objects to use to populate the <see cref="ConditionalArrayIndexCSharpExpression.Indexes" /> collection.</param>
        /// <returns>A <see cref="ConditionalArrayIndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalAccess" /> and the <see cref="ConditionalArrayIndexCSharpExpression.Array" /> and <see cref="ConditionalArrayIndexCSharpExpression.Indexes" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static ConditionalArrayIndexCSharpExpression ConditionalArrayIndex(Expression array, IEnumerable<Expression> indexes)
        {
            RequiresCanRead(array, nameof(array));
            RequiresNotNull(indexes, nameof(indexes));

            if (!array.Type.IsArray)
                throw ArgumentMustBeArray(nameof(array));

            var indexList = indexes.ToReadOnly();

            if (array.Type.GetArrayRank() != indexList.Count)
                throw IncorrectNumberOfIndexes();

            foreach (var index in indexList)
            {
                RequiresCanRead(index, nameof(indexes));

                if (index.Type != typeof(int))
                    throw ArgumentMustBeArrayIndexType(nameof(indexes));
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
        protected internal virtual Expression VisitConditionalArrayIndex(ConditionalArrayIndexCSharpExpression node) =>
            node.Update(
                Visit(node.Array),
                Visit(node.Indexes)
            );
    }
}
