// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a slice pattern.
    /// </summary>
    public sealed partial class SliceCSharpPattern : CSharpPattern
    {
        internal SliceCSharpPattern(CSharpPatternInfo info, LambdaExpression? indexerAccess, CSharpPattern? pattern)
            : base(info)
        {
            IndexerAccess = indexerAccess;
            Pattern = pattern;
        }

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public override CSharpPatternType PatternType => CSharpPatternType.Slice;

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the indexer access used to retrieve a slice from the collection.
        /// </summary>
        public LambdaExpression? IndexerAccess { get; }

        /// <summary>
        /// Gets the <see cref="CSharpPattern"/> representing the optional pattern to apply to the slice.
        /// </summary>
        public CSharpPattern? Pattern { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpPattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitSlicePattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="indexerAccess">The <see cref="IndexerAccess" /> property of the result.</param>
        /// <param name="pattern">The <see cref="Pattern" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public SliceCSharpPattern Update(LambdaExpression? indexerAccess, CSharpPattern? pattern)
        {
            if (indexerAccess == IndexerAccess && pattern == Pattern)
            {
                return this;
            }

            return CSharpPattern.Slice(_info, indexerAccess, pattern);
        }

        /// <summary>
        /// Changes the input type to the specified type.
        /// </summary>
        /// <remarks>
        /// This functionality can be used when a pattern is pass to an expression or statement that applies the pattern.
        /// </remarks>
        /// <param name="inputType">The new input type.</param>
        /// <returns>The original pattern rewritten to use the specified input type.</returns>
        public override CSharpPattern ChangeType(Type inputType)
        {
            if (inputType == InputType)
            {
                return this;
            }

            return CSharpPattern.Slice(PatternInfo(inputType, NarrowedType), IndexerAccess, Pattern);
        }

        /// <summary>
        /// Reduces the pattern by applying it the specified object.
        /// </summary>
        /// <param name="object">The object to match the pattern against.</param>
        /// <returns>The expression representing the pattern applied to the specified object.</returns>
        internal override Expression Reduce(Expression @object) => Reduce(@object, length: null, Range.All);

        /// <summary>
        /// Reduces the slice pattern by applying it the specified range in the specified object.
        /// </summary>
        /// <param name="object">The object to match the pattern against.</param>
        /// <param name="length">The (optional) precomputed length of the object.</param>
        /// <param name="range">The range to extract from the object.</param>
        /// <returns>The expression representing the pattern applied to the specified object and range.</returns>
        internal Expression Reduce(Expression @object, Expression? length, Range range)
        {
            Expression GetSlice(Expression obj)
            {
                var rangeExpr = Expression.Constant(range);

                if (IndexerAccess.Body is ArrayAccessCSharpExpression a &&
                    a.Array == IndexerAccess.Parameters[0] &&
                    a.Indexes.Count == 1 &&
                    a.Indexes[0] == IndexerAccess.Parameters[1])
                {
                    var arrayAccess = a.Update(obj, new[] { rangeExpr });
                    return arrayAccess.Reduce(length);
                }

                if (IndexerAccess.Body is IndexerAccessCSharpExpression i &&
                    i.Object == IndexerAccess.Parameters[0] &&
                    i.Argument == IndexerAccess.Parameters[1])
                {
                    var indexerAccess = i.Update(obj, rangeExpr);
                    return indexerAccess.Reduce(length);
                }

                return Expression.Invoke(IndexerAccess, obj, rangeExpr);
            }

            return PatternHelpers.Reduce(@object, obj =>
            {
                if (IndexerAccess != null && Pattern is { PatternType: not CSharpPatternType.Discard } pattern)
                {
                    var slice = GetSlice(obj);

                    return pattern.Reduce(slice);
                }

                return ConstantTrue;
            });
        }
    }

    partial class CSharpPattern
    {
        // TODO: Add convenience overloads.

        /// <summary>
        /// Creates a slice pattern.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="indexerAccess">The <see cref="LambdaExpression"/> representing the indexer access used to retrieve a slice from the collection.</param>
        /// <param name="pattern">The <see cref="CSharpPattern"/> representing the optional pattern to apply to the slice.</param>
        /// <returns>A <see cref="SliceCSharpPattern" /> representing a slice pattern.</returns>
        public static SliceCSharpPattern Slice(CSharpPatternInfo info, LambdaExpression? indexerAccess, CSharpPattern? pattern)
        {
            RequiresNotNull(info, nameof(info));

            var collectionType = info.InputType;

            if (indexerAccess != null)
            {
                RequiresCanRead(indexerAccess, nameof(indexerAccess));

                if (indexerAccess.Parameters.Count != 2)
                    throw Error.IndexerAccessShouldHaveTwoParameters(nameof(indexerAccess));

                if (!AreEquivalent(indexerAccess.Parameters[0].Type, collectionType))
                    throw Error.IndexerAccessFirstParameterShouldHaveCollectionType(collectionType, nameof(indexerAccess));

                if (indexerAccess.Parameters[1].Type != typeof(Range))
                    throw Error.IndexerAccessSecondParameterInvalidType(typeof(Range), nameof(indexerAccess));

                if (indexerAccess.ReturnType == typeof(void))
                    throw Error.ElementTypeCannotBeVoid(nameof(indexerAccess));
            }

            if (pattern != null)
            {
                RequiresNotNull(indexerAccess, nameof(indexerAccess));

                RequiresCompatiblePatternTypes(indexerAccess!.ReturnType, ref pattern);
                RequiresCompatiblePatternTypes(pattern.NarrowedType, info.NarrowedType);
            }

            return new SliceCSharpPattern(info, indexerAccess, pattern);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="SliceCSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitSlicePattern(SliceCSharpPattern node) =>
            node.Update(
                VisitAndConvert(node.IndexerAccess, nameof(VisitSlicePattern)),
                VisitPattern(node.Pattern)
            );
    }
}
