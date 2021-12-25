// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a negated pattern.
    /// </summary>
    public sealed partial class NotCSharpPattern : CSharpPattern
    {
        public NotCSharpPattern(CSharpPatternInfo info, CSharpPattern negated)
            : base(info)
        {
            Negated = negated;
        }

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public override CSharpPatternType PatternType => CSharpPatternType.Not;

        /// <summary>
        /// Gets the pattern to negate.
        /// </summary>
        public CSharpPattern Negated { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpPattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitNotPattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="negated">The <see cref="Negated" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public NotCSharpPattern Update(CSharpPattern negated)
        {
            if (negated == Negated)
            {
                return this;
            }

            return CSharpPattern.Not(PatternInfo(InputType, NarrowedType), negated);
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

            var negated = Negated.ChangeType(inputType);

            return CSharpPattern.Not(info: null, negated);
        }

        /// <summary>
        /// Reduces the pattern by applying it the specified object.
        /// </summary>
        /// <param name="object">The object to match the pattern against.</param>
        /// <returns>The expression representing the pattern applied to the specified object.</returns>
        internal override Expression Reduce(Expression @object)
        {
            return PatternHelpers.Reduce(@object, obj => Expression.Not(Negated.Reduce(obj)));
        }
    }

    partial class CSharpPattern
    {
        /// <summary>
        /// Creates a negated pattern.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="negated">The pattern to negate.</param>
        /// <returns>A <see cref="NotCSharpPattern" /> representing a negated pattern.</returns>
        public static NotCSharpPattern Not(CSharpPatternInfo info, CSharpPattern negated)
        {
            RequiresNotNull(negated, nameof(negated));

            if (info != null)
            {
                if (info.InputType != info.NarrowedType)
                    throw Error.PatternInputAndNarrowedTypeShouldMatch(nameof(CSharpPatternType.Not));

                RequiresCompatiblePatternTypes(info.InputType, negated.InputType);
            }
            else 
            {
                info = PatternInfo(negated.InputType, negated.InputType);
            }

            return new NotCSharpPattern(info, negated);
        }

        /// <summary>
        /// Creates a negated pattern.
        /// </summary>
        /// <param name="negated">The pattern to negate.</param>
        /// <returns>A <see cref="NotCSharpPattern" /> representing a negated pattern.</returns>
        public static NotCSharpPattern Not(CSharpPattern negated) => Not(info: null, negated);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="NotCSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitNotPattern(NotCSharpPattern node) =>
            node.Update(
                VisitPattern(node.Negated)
            );
    }
}
