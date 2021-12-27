// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a binary pattern that's either a conjunction or a disjunction of two patterns.
    /// </summary>
    public sealed partial class BinaryCSharpPattern : CSharpPattern
    {
        internal BinaryCSharpPattern(CSharpPatternInfo info, CSharpPatternType patternType, CSharpPattern left, CSharpPattern right)
            : base(info)
        {
            PatternType = patternType;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public override CSharpPatternType PatternType { get; }

        /// <summary>
        /// Gets the left pattern.
        /// </summary>
        public CSharpPattern Left { get; }

        /// <summary>
        /// Gets the right pattern.
        /// </summary>
        public CSharpPattern Right { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpPattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitBinaryPattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="left">The <see cref="Left" /> property of the result.</param>
        /// <param name="right">The <see cref="Right" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public BinaryCSharpPattern Update(CSharpPattern left, CSharpPattern right)
        {
            if (left == Left && right == Right)
            {
                return this;
            }

            return CSharpPattern.MakeBinary(_info, PatternType, left, right);
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

            var left = Left.ChangeType(inputType);
            var right = Right;

            if (PatternType == CSharpPatternType.Or)
            {
                right = Right.ChangeType(inputType);
            }

            return CSharpPattern.MakeBinary(info: null, PatternType, left, right);
        }

        /// <summary>
        /// Reduces the pattern by applying it the specified object.
        /// </summary>
        /// <param name="object">The object to match the pattern against.</param>
        /// <returns>The expression representing the pattern applied to the specified object.</returns>
        internal override Expression Reduce(Expression @object)
        {
            Expression createTest(Expression obj)
            {
                return Expression.MakeBinary(PatternType == CSharpPatternType.And ? ExpressionType.AndAlso : ExpressionType.OrElse, Left.Reduce(obj), Right.Reduce(obj));
            }

            return PatternHelpers.Reduce(@object, createTest);
        }
    }

    partial class CSharpPattern
    {
        /// <summary>
        /// Creates an and pattern with the specified left and right patterns.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="left">The left pattern.</param>
        /// <param name="right">The right pattern.</param>
        /// <returns>A <see cref="NotCSharpPattern" /> representing an and pattern.</returns>
        public static BinaryCSharpPattern And(CSharpPatternInfo info, CSharpPattern left, CSharpPattern right)
        {
            RequiresNotNull(left, nameof(left));
            RequiresNotNull(right, nameof(right));

            info ??= PatternInfo(left.InputType, right.NarrowedType);

            RequiresCompatiblePatternTypes(info.InputType, left.InputType);
            RequiresCompatiblePatternTypes(left.NarrowedType, right.InputType);
            RequiresCompatiblePatternTypes(right.NarrowedType, info.NarrowedType);

            return new BinaryCSharpPattern(info, CSharpPatternType.And, left, right);
        }

        /// <summary>
        /// Creates an and pattern with the specified left and right patterns.
        /// </summary>
        /// <param name="left">The left pattern.</param>
        /// <param name="right">The right pattern.</param>
        /// <returns>A <see cref="NotCSharpPattern" /> representing an and pattern.</returns>
        public static BinaryCSharpPattern And(CSharpPattern left, CSharpPattern right) => And(info: null, left, right);

        /// <summary>
        /// Creates an or pattern with the specified left and right patterns.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="left">The left pattern.</param>
        /// <param name="right">The right pattern.</param>
        /// <returns>A <see cref="NotCSharpPattern" /> representing an or pattern.</returns>
        public static BinaryCSharpPattern Or(CSharpPatternInfo info, CSharpPattern left, CSharpPattern right)
        {
            RequiresNotNull(left, nameof(left));
            RequiresNotNull(right, nameof(right));

            RequiresCompatiblePatternTypes(left.InputType, right.InputType);

            if (info == null)
            {
                var inputType = left.InputType;
                var narrowedType = FindLeastSpecificType(inputType, left, right);
                info = PatternInfo(inputType, narrowedType);
            }
            else
            {
                var leastSpecificType = FindLeastSpecificType(info.InputType, left, right);
                RequiresCompatiblePatternTypes(leastSpecificType, info.NarrowedType);
            }

            RequiresCompatiblePatternTypes(info.InputType, left.InputType);
            RequiresCompatiblePatternTypes(info.InputType, right.InputType);

            return new BinaryCSharpPattern(info, CSharpPatternType.Or, left, right);
        }

        /// <summary>
        /// Creates an or pattern with the specified left and right patterns.
        /// </summary>
        /// <param name="left">The left pattern.</param>
        /// <param name="right">The right pattern.</param>
        /// <returns>A <see cref="NotCSharpPattern" /> representing an or pattern.</returns>
        public static BinaryCSharpPattern Or(CSharpPattern left, CSharpPattern right) => Or(info: null, left, right);

        /// <summary>
        /// Creates a binary pattern with the specified left and right patterns.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="type">The type of the pattern.</param>
        /// <param name="left">The left pattern.</param>
        /// <param name="right">The right pattern.</param>
        /// <returns>A <see cref="NotCSharpPattern" /> representing a binary pattern.</returns>
        public static BinaryCSharpPattern MakeBinary(CSharpPatternInfo info, CSharpPatternType type, CSharpPattern left, CSharpPattern right) =>
            type switch
            {
                CSharpPatternType.And => And(info, left, right),
                CSharpPatternType.Or => Or(info, left, right),
                _ => throw Error.InvalidBinaryPatternType(type)
            };

        private static Type FindLeastSpecificType(Type inputType, CSharpPattern left, CSharpPattern right)
        {
            // NB: This is ported from roslyn\src\Compilers\CSharp\Portable\Binder\Binder_Patterns.cs.

            var narrowedTypeCandidates = new List<Type>(2);
            collectCandidates(left, narrowedTypeCandidates);
            collectCandidates(right, narrowedTypeCandidates);
            return leastSpecificType(narrowedTypeCandidates) ?? inputType;

            static void collectCandidates(CSharpPattern pat, List<Type> candidates)
            {
                if (pat is BinaryCSharpPattern p && p.PatternType == CSharpPatternType.Or)
                {
                    collectCandidates(p.Left, candidates);
                    collectCandidates(p.Right, candidates);
                }
                else
                {
                    candidates.Add(pat.NarrowedType);
                }
            }

            static Type leastSpecificType(List<Type> candidates)
            {
                Debug.Assert(candidates.Count >= 2);

                Type bestSoFar = candidates[0];

                // first pass: select a candidate for which no other has been shown to be an improvement.
                for (int i = 1, n = candidates.Count; i < n; i++)
                {
                    Type candidate = candidates[i];
                    bestSoFar = lessSpecificCandidate(bestSoFar, candidate) ?? bestSoFar;
                }

                // second pass: check that it is no more specific than any candidate.
                for (int i = 0, n = candidates.Count; i < n; i++)
                {
                    Type candidate = candidates[i];
                    Type spoiler = lessSpecificCandidate(candidate, bestSoFar);
                    if (spoiler is null)
                    {
                        bestSoFar = null;
                        break;
                    }

                    // Our specificity criteria are transitive
                    Debug.Assert(spoiler == bestSoFar);
                }

                return bestSoFar;
            }

            // Given a candidate least specific type so far, attempt to refine it with a possibly less specific candidate.
            static Type lessSpecificCandidate(Type bestSoFar, Type possiblyLessSpecificCandidate)
            {
                if (bestSoFar == possiblyLessSpecificCandidate)
                {
                    // When the types are equivalent, merge them.
                    return bestSoFar;
                }
                else if (TypeUtils.HasReferenceConversion(bestSoFar, possiblyLessSpecificCandidate)) // REVIEW: Same semantics as Roslyn?
                {
                    // When there is an implicit reference conversion from T to U, U is less specific
                    return possiblyLessSpecificCandidate;
                }
                else if (TypeUtils.IsImplicitBoxingConversion(bestSoFar, possiblyLessSpecificCandidate)) // REVIEW: Same semantics as Roslyn?
                {
                    // when there is a boxing conversion from T to U, U is less specific.
                    return possiblyLessSpecificCandidate;
                }
                else
                {
                    // We have no improved candidate to offer.
                    return null;
                }
            }
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="BinaryCSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitBinaryPattern(BinaryCSharpPattern node) =>
            node.Update(
                VisitPattern(node.Left),
                VisitPattern(node.Right)
            );
    }
}
