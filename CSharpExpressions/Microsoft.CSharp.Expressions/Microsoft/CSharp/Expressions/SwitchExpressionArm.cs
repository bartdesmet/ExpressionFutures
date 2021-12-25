// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a switch expression arm.
    /// </summary>
    public sealed partial class SwitchExpressionArm
    {
        internal SwitchExpressionArm(ReadOnlyCollection<ParameterExpression> variables, CSharpPattern pattern, Expression whenClause, Expression value)
        {
            Variables = variables;
            Pattern = pattern;
            WhenClause = whenClause;
            Value = value;
        }

        /// <summary>
        /// Gets a collection of <see cref="ParameterExpression"/> representing local variables for the arm.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Variables { get; }

        /// <summary>
        /// Gets the <see cref="CSharpPattern" /> representing the pattern matched by the arm.
        /// </summary>
        public CSharpPattern Pattern { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the optional when clause.
        /// </summary>
        public Expression WhenClause { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the value returned by the arm.
        /// </summary>
        public Expression Value { get; }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="variables">The <see cref="Variables" /> property of the result.</param>
        /// <param name="pattern">The <see cref="Pattern" /> property of the result.</param>
        /// <param name="whenClause">The <see cref="WhenClause" /> property of the result.</param>
        /// <param name="value">The <see cref="Value" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public SwitchExpressionArm Update(IEnumerable<ParameterExpression> variables, CSharpPattern pattern, Expression whenClause, Expression value)
        {
            if (SameElements(ref variables, Variables) && pattern == Pattern && whenClause == WhenClause && value == Value)
            {
                return this;
            }

            return CSharpExpression.SwitchExpressionArm(variables, pattern, whenClause, value);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="SwitchExpressionArm"/> that represents a switch expression arm.
        /// </summary>
        /// <param name="pattern">The pattern matched by the arm.</param>
        /// <param name="value">The expression representing the value returned by the arm.</param>
        /// <returns>The created <see cref="SwitchExpressionArm"/>.</returns>
        public static SwitchExpressionArm SwitchExpressionArm(CSharpPattern pattern, Expression value) =>
            SwitchExpressionArm(variables: null, pattern, whenClause: null, value);

        /// <summary>
        /// Creates a <see cref="SwitchExpressionArm"/> that represents a switch expression arm.
        /// </summary>
        /// <param name="variables">The variables that are in scope of the arm.</param>
        /// <param name="pattern">The pattern matched by the arm.</param>
        /// <param name="value">The expression representing the value returned by the arm.</param>
        /// <returns>The created <see cref="SwitchExpressionArm"/>.</returns>
        public static SwitchExpressionArm SwitchExpressionArm(IEnumerable<ParameterExpression> variables, CSharpPattern pattern, Expression value) =>
            SwitchExpressionArm(variables, pattern, whenClause: null, value);

        /// <summary>
        /// Creates a <see cref="SwitchExpressionArm"/> that represents a switch expression arm.
        /// </summary>
        /// <param name="pattern">The pattern matched by the arm.</param>
        /// <param name="whenClause">The expression representing the optional when clause.</param>
        /// <param name="value">The expression representing the value returned by the arm.</param>
        /// <returns>The created <see cref="SwitchExpressionArm"/>.</returns>
        public static SwitchExpressionArm SwitchExpressionArm(CSharpPattern pattern, Expression whenClause, Expression value) =>
            SwitchExpressionArm(variables: null, pattern, whenClause, value);

        /// <summary>
        /// Creates a <see cref="SwitchExpressionArm"/> that represents a switch expression arm.
        /// </summary>
        /// <param name="variables">The variables that are in scope of the arm.</param>
        /// <param name="pattern">The pattern matched by the arm.</param>
        /// <param name="whenClause">The expression representing the optional when clause.</param>
        /// <param name="value">The expression representing the value returned by the arm.</param>
        /// <returns>The created <see cref="SwitchExpressionArm"/>.</returns>
        public static SwitchExpressionArm SwitchExpressionArm(IEnumerable<ParameterExpression> variables, CSharpPattern pattern, Expression whenClause, Expression value)
        {
            var variablesList = CheckUniqueVariables(variables, nameof(variables));

            RequiresNotNull(pattern, nameof(pattern));

            if (whenClause != null)
            {
                RequiresCanRead(whenClause, nameof(whenClause));

                if (whenClause.Type != typeof(bool))
                    throw Error.WhenClauseShouldBeBoolean();
            }

            RequiresCanRead(value, nameof(value));

            if (value.Type == typeof(void))
                throw Error.SwitchExpressionArmValueShouldNotBeVoid();

            return new SwitchExpressionArm(variablesList, pattern, whenClause, value);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="SwitchExpressionArm" />.
        /// </summary>
        /// <param name="node">The switch expression arm to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual SwitchExpressionArm VisitSwitchExpressionArm(SwitchExpressionArm node) =>
            node.Update(
                VisitAndConvert(node.Variables, nameof(VisitSwitchExpressionArm)),
                VisitPattern(node.Pattern),
                Visit(node.WhenClause),
                Visit(node.Value)
            );
    }
}
