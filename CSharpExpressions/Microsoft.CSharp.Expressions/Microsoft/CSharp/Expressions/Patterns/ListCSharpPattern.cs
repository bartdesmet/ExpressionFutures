// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;
    using static PatternHelpers;

    /// <summary>
    /// Represents a list pattern.
    /// </summary>
    public sealed partial class ListCSharpPattern : CSharpObjectPattern
    {
        internal ListCSharpPattern(CSharpObjectPatternInfo info, LambdaExpression lengthAccess, LambdaExpression indexerAccess, ReadOnlyCollection<CSharpPattern> patterns)
            : base(info)
        {
            LengthAccess = lengthAccess;
            IndexerAccess = indexerAccess;
            Patterns = patterns;
        }

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public override CSharpPatternType PatternType => CSharpPatternType.List;

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the expression used to retrieve the collection's length or element count.
        /// </summary>
        public LambdaExpression LengthAccess { get; }

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the indexer access used to retrieve an element from the collection.
        /// </summary>
        public LambdaExpression IndexerAccess { get; }

        /// <summary>
        /// Gets a list of <see cref="CSharpPattern"/> patterns to apply to the elements of the collection, optionally containing a slice pattern.
        /// </summary>
        public ReadOnlyCollection<CSharpPattern> Patterns { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpPattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitListPattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="variable">The <see cref="Variable" /> property of the result.</param>
        /// <param name="lengthAccess">The <see cref="LengthAccess" /> property of the result.</param>
        /// <param name="indexerAccess">The <see cref="IndexerAccess" /> property of the result.</param>
        /// <param name="patterns">The <see cref="Patterns" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ListCSharpPattern Update(ParameterExpression variable, LambdaExpression lengthAccess, LambdaExpression indexerAccess, IEnumerable<CSharpPattern> patterns)
        {
            if (variable == Variable && lengthAccess == LengthAccess && indexerAccess == IndexerAccess && SameElements(ref patterns, Patterns))
            {
                return this;
            }

            return CSharpPattern.List(ObjectPatternInfo(_info, variable), lengthAccess, indexerAccess, patterns);
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

            return CSharpPattern.List(ObjectPatternInfo(PatternInfo(inputType, NarrowedType), Variable), LengthAccess, IndexerAccess, Patterns);
        }

        /// <summary>
        /// Reduces the pattern by applying it the specified object.
        /// </summary>
        /// <param name="object">The object to match the pattern against.</param>
        /// <returns>The expression representing the pattern applied to the specified object.</returns>
        internal override Expression Reduce(Expression @object)
        {
            Expression GetLength(Expression obj)
            {
                if (LengthAccess.Body is MemberExpression m &&
                    m.Expression == LengthAccess.Parameters[0])
                {
                    return m.Update(obj);
                }

                return Expression.Invoke(LengthAccess, obj);
            }

            Expression GetElement(Expression obj, Expression length, Index index)
            {
                var indexExpr = Expression.Constant(index);

                if (IndexerAccess.Body is ArrayAccessCSharpExpression a &&
                    a.Array == IndexerAccess.Parameters[0] &&
                    a.Indexes.Count == 1 &&
                    a.Indexes[0] == IndexerAccess.Parameters[1])
                {
                    var arrayAccess = a.Update(obj, new[] { indexExpr });
                    return arrayAccess.Reduce(length);
                }
                
                if (IndexerAccess.Body is IndexerAccessCSharpExpression i &&
                    i.Object == IndexerAccess.Parameters[0] &&
                    i.Argument == IndexerAccess.Parameters[1])
                {
                    var indexerAccess = i.Update(obj, indexExpr);
                    return indexerAccess.Reduce(length);
                }

                return Expression.Invoke(IndexerAccess, obj, indexExpr);
            }

            bool HasSlice()
            {
                foreach (var p in Patterns)
                {
                    if (p.PatternType == CSharpPatternType.Slice)
                    {
                        return true;
                    }
                }

                return false;
            }

            return PatternHelpers.Reduce(@object, obj =>
            {
                var exit = Expression.Label(typeof(bool), "__return");

                var vars = new List<ParameterExpression>();
                var stmts = new List<Expression>();

                obj = AddNullCheck(obj, typeCheck: null, exit, vars, stmts);

                var n = Patterns.Count;

                var length = GetLength(obj);

                var hasSlice = HasSlice();

                if (hasSlice)
                {
                    var lengthTemp = Expression.Parameter(length.Type, "__len");
                    vars.Add(lengthTemp);
                    stmts.Add(Expression.Assign(lengthTemp, length));
                    length = new ReadOnlyTemporaryVariableExpression(lengthTemp);
                }

                var negatedLengthCheck = hasSlice
                    ? Expression.LessThan(length, CreateConstantInt32(n - 1))
                    : Expression.NotEqual(length, CreateConstantInt32(n));

                AddFailIf(negatedLengthCheck, exit, stmts);

                var hasSeenSlice = false;

                for (int i = 0; i < n; i++)
                {
                    var p = Patterns[i];

                    if (p is SliceCSharpPattern slice)
                    {
                        hasSeenSlice = true;

                        var rangeStart = new Index(i);
                        var rangeEnd = new Index(n - i - 1, fromEnd: true);
                        var range = new Range(rangeStart, rangeEnd);

                        var sliceCheck = slice.Reduce(obj, length, range);

                        AddFailIfNot(sliceCheck, exit, stmts);
                    }
                    else if (p.PatternType != CSharpPatternType.Discard)
                    {
                        var index = hasSeenSlice
                            ? new Index(n - i, fromEnd: true)
                            : new Index(i);

                        var element = GetElement(obj, length, index);
                        var elementCheck = p.Reduce(element);

                        AddFailIfNot(elementCheck, exit, stmts);
                    }
                }

                if (Variable != null)
                {
                    stmts.Add(Expression.Assign(Variable, obj));
                }

                stmts.Add(Expression.Label(exit, ConstantTrue));

                return Expression.Block(vars, stmts);
            });
        }
    }

    partial class CSharpPattern
    {
        // TODO: Add convenience overloads.

        /// <summary>
        /// Creates a list pattern.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="lengthAccess">The <see cref="LambdaExpression"/> representing the expression used to retrieve the collection's length or element count.</param>
        /// <param name="indexerAccess">The <see cref="LambdaExpression"/> representing the indexer access used to retrieve an element from the collection.</param>
        /// <param name="patterns">The list of <see cref="CSharpPattern"/> patterns to apply to the elements of the collection, optionally containing a slice pattern.</param>
        /// <returns>A <see cref="ListCSharpPattern" /> representing a list pattern.</returns>
        public static ListCSharpPattern List(CSharpObjectPatternInfo info, LambdaExpression lengthAccess, LambdaExpression indexerAccess, params CSharpPattern[] patterns) =>
            List(info, lengthAccess, indexerAccess, (IEnumerable<CSharpPattern>)patterns);

        /// <summary>
        /// Creates a list pattern.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="lengthAccess">The <see cref="LambdaExpression"/> representing the expression used to retrieve the collection's length or element count.</param>
        /// <param name="indexerAccess">The <see cref="LambdaExpression"/> representing the indexer access used to retrieve an element from the collection.</param>
        /// <param name="patterns">The list of <see cref="CSharpPattern"/> patterns to apply to the elements of the collection, optionally containing a slice pattern.</param>
        /// <returns>A <see cref="ListCSharpPattern" /> representing a list pattern.</returns>
        public static ListCSharpPattern List(CSharpObjectPatternInfo info, LambdaExpression lengthAccess, LambdaExpression indexerAccess, IEnumerable<CSharpPattern> patterns)
        {
            RequiresNotNull(info, nameof(info));

            var nonNullInputType = info.Info.InputType.GetNonNullableType();
            var collectionType = info.Info.NarrowedType;

            if (!AreEquivalent(nonNullInputType, collectionType))
                throw Error.ListPatternInputTypeInvalid(nonNullInputType, collectionType);

            RequiresCanRead(lengthAccess, nameof(lengthAccess));

            if (lengthAccess.Parameters.Count != 1)
                throw Error.LengthAccessShouldHaveOneParameter();

            if (!AreEquivalent(lengthAccess.Parameters[0].Type, collectionType))
                throw Error.LengthAccessParameterShouldHaveCollectionType(collectionType);

            if (lengthAccess.ReturnType != typeof(int))
                throw Error.LengthAccessShouldReturnInt32();

            RequiresCanRead(indexerAccess, nameof(indexerAccess));

            if (indexerAccess.Parameters.Count != 2)
                throw Error.IndexerAccessShouldHaveTwoParameters();

            if (!AreEquivalent(indexerAccess.Parameters[0].Type, collectionType))
                throw Error.IndexerAccessFirstParameterShouldHaveCollectionType(collectionType);

            if (indexerAccess.Parameters[1].Type != typeof(Index))
                throw Error.IndexerAccessSecondParameterInvalidType(typeof(Index));

            var elementType = indexerAccess.ReturnType;

            if (elementType == typeof(void))
                throw Error.ElementTypeCannotBeVoid();

            var patternsList = patterns.ToReadOnly();

            RequiresNotNullItems(patternsList, nameof(patterns));

            var hasSlice = false;

            foreach (var pattern in patternsList)
            {
                if (pattern.PatternType == CSharpPatternType.Slice)
                {
                    if (hasSlice)
                        throw Error.MoreThanOneSlicePattern();

                    hasSlice = true;

                    RequiresCompatiblePatternTypes(collectionType, pattern.InputType);
                }
                else
                {
                    RequiresCompatiblePatternTypes(elementType, pattern.InputType);
                }
            }

            return new ListCSharpPattern(info, lengthAccess, indexerAccess, patternsList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ListCSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitListPattern(ListCSharpPattern node) =>
            node.Update(
                VisitAndConvert(node.Variable, nameof(VisitListPattern)),
                VisitAndConvert(node.LengthAccess, nameof(VisitListPattern)),
                VisitAndConvert(node.IndexerAccess, nameof(VisitListPattern)),
                Visit(node.Patterns, VisitPattern)
            );
    }
}
