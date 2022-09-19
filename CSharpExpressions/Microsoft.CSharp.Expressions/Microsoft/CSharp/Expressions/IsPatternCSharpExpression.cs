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
    /// <summary>
    /// Represents an is expression with a pattern.
    /// </summary>
    public sealed partial class IsPatternCSharpExpression : CSharpExpression
    {
        internal IsPatternCSharpExpression(Expression expression, CSharpPattern pattern)
        {
            Expression = expression;
            Pattern = pattern;
        }

        /// <summary>
        /// Gets the <see cref="System.Linq.Expressions.Expression" /> to check using the pattern.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Gets the <see cref="CSharpPattern" /> representing the pattern to check.
        /// </summary>
        public CSharpPattern Pattern { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.IsPattern;

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        public override Type Type => typeof(bool);

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitIsPattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <param name="pattern">The <see cref="Pattern" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public IsPatternCSharpExpression Update(Expression expression, CSharpPattern pattern)
        {
            if (expression == Expression && pattern == Pattern)
            {
                return this;
            }

            return CSharpExpression.IsPattern(expression, pattern);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce() => Pattern.Reduce(Expression);
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an <see cref="IsPatternCSharpExpression"/> that represents an is expression using a pattern.
        /// </summary>
        /// <param name="expression">The expression to check using the pattern.</param>
        /// <param name="pattern">The pattern to check.</param>
        /// <returns>The created <see cref="IsPatternCSharpExpression"/>.</returns>
        public static IsPatternCSharpExpression IsPattern(Expression expression, CSharpPattern pattern)
        {
            RequiresCanRead(expression, nameof(expression));

            RequiresNotNull(pattern, nameof(pattern));

            ValidateType(expression.Type, nameof(expression));

            return new IsPatternCSharpExpression(expression, pattern);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="IsPatternCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitIsPattern(IsPatternCSharpExpression node) =>
            node.Update(
                Visit(node.Expression),
                VisitPattern(node.Pattern)
            );
    }
}
