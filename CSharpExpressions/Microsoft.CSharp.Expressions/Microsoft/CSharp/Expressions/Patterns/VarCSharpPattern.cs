// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a pattern that always matches and assigns the input to a variable.
    /// </summary>
    public sealed partial class VarCSharpPattern : CSharpObjectPattern
    {
        internal VarCSharpPattern(CSharpObjectPatternInfo info)
            : base(info)
        {
        }

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public override CSharpPatternType PatternType => CSharpPatternType.Var;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpPattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitVarPattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="variable">The <see cref="CSharpObjectPattern.Variable" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public VarCSharpPattern Update(ParameterExpression variable)
        {
            if (variable == Variable)
            {
                return this;
            }

            return CSharpPattern.Var(_info, variable);
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

            // NB: This can fail if there's a variable and the input type is not compatible. See Var factory.

            return CSharpPattern.Var(ObjectPatternInfo(PatternInfo(inputType, inputType), Variable));
        }

        /// <summary>
        /// Reduces the pattern by applying it the specified object.
        /// </summary>
        /// <param name="object">The object to match the pattern against.</param>
        /// <returns>The expression representing the pattern applied to the specified object.</returns>
        internal override Expression Reduce(Expression @object)
        {
            // NB: RecursiveCSharpPattern has a peephole optimization for the pattern below.

            if (Variable != null)
            {
                return
                    Expression.Block(
                        Expression.Assign(Variable, @object),
                        ConstantTrue
                    );
            }
            else
            {
                // NB: Ensure any side-effects in evaluating @object are retained.
                return PatternHelpers.Reduce(@object, _ => ConstantTrue);
            }
        }
    }

    partial class CSharpPattern
    {
        /// <summary>
        /// Creates a pattern that always matches and assigns the input to a variable.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <returns>A <see cref="DiscardCSharpPattern" /> that represents a pattern that always matches.</returns>
        public static VarCSharpPattern Var(CSharpObjectPatternInfo info)
        {
            RequiresNotNull(info, nameof(info));

            if (info.Info.InputType != info.Info.NarrowedType)
                throw Error.PatternInputAndNarrowedTypeShouldMatch(nameof(CSharpPatternType.Var), nameof(info));
            if (info.Variable != null && info.Variable.Type != info.Info.NarrowedType)
                throw Error.CannotAssignPatternResultToVariable(info.Variable.Type, info.Info.NarrowedType, nameof(info));

            return new VarCSharpPattern(info);
        }

        /// <summary>
        /// Creates a pattern that always matches and assigns the input to a variable.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="variable">The variable to assign to.</param>
        /// <returns>A <see cref="DiscardCSharpPattern" /> that represents a pattern that always matches.</returns>
        public static VarCSharpPattern Var(CSharpPatternInfo? info, ParameterExpression variable)
        {
            RequiresNotNull(variable, nameof(variable));

            info ??= PatternInfo(variable.Type, variable.Type);

            return Var(ObjectPatternInfo(info, variable));
        }

        /// <summary>
        /// Creates a pattern that always matches and assigns the input to a variable.
        /// </summary>
        /// <param name="variable">The variable to assign to.</param>
        /// <returns>A <see cref="DiscardCSharpPattern" /> that represents a pattern that always matches.</returns>
        public static VarCSharpPattern Var(ParameterExpression variable) => Var(info: null, variable);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="VarCSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitVarPattern(VarCSharpPattern node) =>
            node.Update(
                VisitAndConvert(node.Variable! /* NB: Non-null for var pattern. */, nameof(VisitVarPattern))
            );
    }
}
