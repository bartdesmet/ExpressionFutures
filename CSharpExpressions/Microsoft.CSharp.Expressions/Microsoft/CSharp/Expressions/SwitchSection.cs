// Prototyping extended expression trees for C#.
//
// bartde - Decmeber 2021

#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a section of a switch statement.
    /// </summary>
    public sealed partial class SwitchSection
    {
        internal SwitchSection(ReadOnlyCollection<ParameterExpression> locals, ReadOnlyCollection<SwitchLabel> labels, ReadOnlyCollection<Expression> statements)
        {
            Locals = locals;
            Labels = labels;
            Statements = statements;
        }

        /// <summary>
        /// Gets a collection of <see cref="ParameterExpression"/> representing the variables that are in scope of the section.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Locals { get; }

        /// <summary>
        /// Gets a collection of <see cref="SwitchLabel"/> nodes that are handled by the section.
        /// </summary>
        public ReadOnlyCollection<SwitchLabel> Labels { get; }

        /// <summary>
        /// Gets a collection of <see cref="Expression" /> nodes representing the body of the section.
        /// </summary>
        public ReadOnlyCollection<Expression> Statements { get; }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="locals">The <see cref="Locals" /> property of the result.</param>
        /// <param name="labels">The <see cref="Labels" /> property of the result.</param>
        /// <param name="statements">The <see cref="Statements" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public SwitchSection Update(IEnumerable<ParameterExpression>? locals, IEnumerable<SwitchLabel> labels, IEnumerable<Expression> statements)
        {
            if (SameElements(ref locals, Locals) && SameElements(ref labels!, Labels) && SameElements(ref statements!, Statements))
            {
                return this;
            }

            return CSharpExpression.SwitchSection(locals, labels, statements);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.SwitchSection"/> that represents a switch statement section.
        /// </summary>
        /// <param name="label">The single label handled by the section.</param>
        /// <param name="statements">The statements in the body of the section.</param>
        /// <returns>The created <see cref="Microsoft.CSharp.Expressions.SwitchSection"/>.</returns>
        public static SwitchSection SwitchSection(SwitchLabel label, params Expression[] statements) =>
            SwitchSection(locals: null, new[] { label }, (IEnumerable<Expression>)statements);

        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.SwitchSection"/> that represents a switch statement section.
        /// </summary>
        /// <param name="label">The single label handled by the section.</param>
        /// <param name="statements">The statements in the body of the section.</param>
        /// <returns>The created <see cref="Microsoft.CSharp.Expressions.SwitchSection"/>.</returns>
        public static SwitchSection SwitchSection(SwitchLabel label, IEnumerable<Expression> statements) =>
            SwitchSection(locals: null, new[] { label }, statements);

        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.SwitchSection"/> that represents a switch statement section.
        /// </summary>
        /// <param name="labels">The labels handled by the section.</param>
        /// <param name="statements">The statements in the body of the section.</param>
        /// <returns>The created <see cref="Microsoft.CSharp.Expressions.SwitchSection"/>.</returns>
        public static SwitchSection SwitchSection(IEnumerable<SwitchLabel> labels, params Expression[] statements) =>
            SwitchSection(locals: null, labels, (IEnumerable<Expression>)statements);

        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.SwitchSection"/> that represents a switch statement section.
        /// </summary>
        /// <param name="labels">The labels handled by the section.</param>
        /// <param name="statements">The statements in the body of the section.</param>
        /// <returns>The created <see cref="Microsoft.CSharp.Expressions.SwitchSection"/>.</returns>
        public static SwitchSection SwitchSection(IEnumerable<SwitchLabel> labels, IEnumerable<Expression> statements) =>
            SwitchSection(locals: null, labels, statements);

        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.SwitchSection"/> that represents a switch statement section.
        /// </summary>
        /// <param name="locals">The locals that are in scope of the section.</param>
        /// <param name="labels">The labels handled by the section.</param>
        /// <param name="statements">The statements in the body of the section.</param>
        /// <returns>The created <see cref="Microsoft.CSharp.Expressions.SwitchSection"/>.</returns>
        public static SwitchSection SwitchSection(IEnumerable<ParameterExpression>? locals, IEnumerable<SwitchLabel> labels, params Expression[] statements) =>
            SwitchSection(locals, labels, (IEnumerable<Expression>)statements);

        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.SwitchSection"/> that represents a switch statement section.
        /// </summary>
        /// <param name="locals">The locals that are in scope of the section.</param>
        /// <param name="labels">The labels handled by the section.</param>
        /// <param name="statements">The statements in the body of the section.</param>
        /// <returns>The created <see cref="Microsoft.CSharp.Expressions.SwitchSection"/>.</returns>
        public static SwitchSection SwitchSection(IEnumerable<ParameterExpression>? locals, IEnumerable<SwitchLabel> labels, IEnumerable<Expression> statements)
        {
            var localsList = CheckUniqueVariables(locals, nameof(locals));

            var labelsList = labels.ToReadOnly();

            RequiresNotEmpty(labelsList, nameof(labels));
            RequiresNotNullItems(labelsList, nameof(labels));

            var allLabels = new HashSet<LabelTarget>();
            var patternInputType = default(Type);

            foreach (var label in labelsList)
            {
                if (label.Label != null && !allLabels.Add(label.Label))
                    throw Error.DuplicateLabelInSwitchSection(label.Label);

                if (patternInputType == null)
                {
                    patternInputType = label.Pattern.InputType;
                }
                else if (patternInputType != label.Pattern.InputType)
                {
                    throw Error.InconsistentPatternInputType(label.Pattern.InputType, patternInputType);
                }
            }

            var statementsList = GetStatements(statements);

            return new SwitchSection(localsList, labelsList, statementsList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="SwitchSection" />.
        /// </summary>
        /// <param name="node">The switch section to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual SwitchSection VisitSwitchSection(SwitchSection node) =>
            node.Update(
                VisitAndConvert(node.Locals, nameof(VisitSwitchSection)),
                Visit(node.Labels, VisitSwitchLabel),
                Visit(node.Statements)
            );
    }
}
