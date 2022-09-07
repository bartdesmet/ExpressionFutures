// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a switch statement case.
    /// </summary>
    public sealed partial class SwitchLabel
    {
        internal SwitchLabel(LabelTarget? label, CSharpPattern pattern, Expression? whenClause)
        {
            Label = label;
            Pattern = pattern;
            WhenClause = whenClause;
        }

        /// <summary>
        /// Gets the <see cref="LabelTarget"/> representing the optional label to jump to the case, e.g. using a <c>goto case</c> statement.
        /// </summary>
        public LabelTarget? Label { get; }

        /// <summary>
        /// Gets the <see cref="CSharpPattern" /> representing the pattern matched by the case.
        /// </summary>
        public CSharpPattern Pattern { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the optional when clause.
        /// </summary>
        public Expression? WhenClause { get; }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="label">The <see cref="Label" /> property of the result.</param>
        /// <param name="pattern">The <see cref="Pattern" /> property of the result.</param>
        /// <param name="whenClause">The <see cref="WhenClause" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public SwitchLabel Update(LabelTarget? label, CSharpPattern pattern, Expression? whenClause)
        {
            if (label == Label && pattern == Pattern && whenClause == WhenClause)
            {
                return this;
            }

            return CSharpExpression.SwitchLabel(label, pattern, whenClause);
        }

        // NB: 'case _' is not supported in C#; instead, 'default' is translated to a discard pattern.
        // CONSIDER: Roslyn could add a check (label.Syntax.Kind() == SyntaxKind.DefaultSwitchLabel) to emit a different factory call to denote 'default'.

        internal bool IsDefault => Pattern is DiscardCSharpPattern && WhenClause == null;
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.SwitchLabel"/> that represents a switch case.
        /// </summary>
        /// <param name="pattern">The pattern matched by the switch case.</param>
        /// <returns>The created <see cref="Microsoft.CSharp.Expressions.SwitchLabel"/>.</returns>
        public static SwitchLabel SwitchLabel(CSharpPattern pattern) => SwitchLabel(label: null, pattern, whenClause: null);

        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.SwitchLabel"/> that represents a switch case.
        /// </summary>
        /// <param name="pattern">The pattern matched by the switch case.</param>
        /// <param name="whenClause">The expression representing the optional when clause.</param>
        /// <returns>The created <see cref="Microsoft.CSharp.Expressions.SwitchLabel"/>.</returns>
        public static SwitchLabel SwitchLabel(CSharpPattern pattern, Expression? whenClause) => SwitchLabel(label: null, pattern, whenClause);

        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.SwitchLabel"/> that represents a switch case.
        /// </summary>
        /// <param name="label">The label used to jump to the case, e.g. using a <c>goto case</c> statement.</param>
        /// <param name="pattern">The pattern matched by the switch case.</param>
        /// <returns>The created <see cref="Microsoft.CSharp.Expressions.SwitchLabel"/>.</returns>
        public static SwitchLabel SwitchLabel(LabelTarget? label, CSharpPattern pattern) => SwitchLabel(label, pattern, whenClause: null);

        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.SwitchLabel"/> that represents a switch case.
        /// </summary>
        /// <param name="label">The label used to jump to the case, e.g. using a <c>goto case</c> statement.</param>
        /// <param name="pattern">The pattern matched by the switch case.</param>
        /// <param name="whenClause">The expression representing the optional when clause.</param>
        /// <returns>The created <see cref="Microsoft.CSharp.Expressions.SwitchLabel"/>.</returns>
        public static SwitchLabel SwitchLabel(LabelTarget? label, CSharpPattern pattern, Expression? whenClause)
        {
            if (label != null && label.Type != typeof(void))
                throw Error.SwitchLabelTargetShouldBeVoid();

            RequiresNotNull(pattern, nameof(pattern));

            if (whenClause != null)
            {
                RequiresCanRead(whenClause, nameof(whenClause));

                if (whenClause.Type != typeof(bool))
                    throw Error.WhenClauseShouldBeBoolean();
            }

            return new SwitchLabel(label, pattern, whenClause);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="SwitchLabel" />.
        /// </summary>
        /// <param name="node">The switch label to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual SwitchLabel VisitSwitchLabel(SwitchLabel node) =>
            node.Update(
                VisitLabelTarget(node.Label),
                VisitPattern(node.Pattern),
                Visit(node.WhenClause)
            );
    }
}
