// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a switch statement.
    /// </summary>
    public sealed partial class PatternSwitchCSharpStatement : SwitchCSharpStatementBase
    {
        internal PatternSwitchCSharpStatement(Expression switchValue, LabelTarget breakLabel, ReadOnlyCollection<ParameterExpression> variables, ReadOnlyCollection<SwitchSection> sections)
            : base(switchValue, breakLabel, variables)
        {
            Sections = sections;
        }

        /// <summary>
        /// Gets the collection of switch sections.
        /// </summary>
        public ReadOnlyCollection<SwitchSection> Sections { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitSwitch(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="switchValue">The <see cref="SwitchCSharpStatementBase.SwitchValue" /> property of the result.</param>
        /// <param name="breakLabel">The <see cref="SwitchCSharpStatementBase.BreakLabel"/> property of the result.</param>
        /// <param name="variables">The <see cref="SwitchCSharpStatementBase.Variables" /> property of the result.</param>
        /// <param name="sections">The <see cref="Sections" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public PatternSwitchCSharpStatement Update(Expression switchValue, LabelTarget breakLabel, IEnumerable<ParameterExpression>? variables, IEnumerable<SwitchSection>? sections)
        {
            if (switchValue == SwitchValue && breakLabel == BreakLabel && SameElements(ref variables, Variables) && SameElements(ref sections, Sections))
            {
                return this;
            }

            return CSharpExpression.SwitchStatement(switchValue, breakLabel, variables, sections);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            //
            // NB: The code below is a poor man's linear evaluation of all the sections. It can be improved by building a DAG
            //     similar to the code gen in Roslyn.
            //

            var obj = Expression.Parameter(SwitchValue.Type, "__obj");

            var stmts = new List<Expression>();

            var defaultBlock = default(Expression);

            foreach (var section in Sections)
            {
                var tests = default(Expression);
                var sectionStmts = new List<Expression>();
                var defaultLabel = default(LabelTarget);

                foreach (var label in section.Labels)
                {
                    var currentLabel = label.Label;

                    if (label.IsDefault)
                    {
                        Debug.Assert(defaultLabel == null); // NB: Factory should catch this.

                        currentLabel ??= Expression.Label("default");
                        defaultLabel = currentLabel;
                    }
                    else
                    {
                        if (currentLabel != null)
                        {
                            // This supports 'goto case C' by adding a label.
                            sectionStmts.Add(Expression.Label(currentLabel));
                        }

                        var test = label.Pattern.Reduce(obj);

                        if (label.WhenClause != null)
                        {
                            test = IsAlwaysTrue(test) ? label.WhenClause : Expression.AndAlso(test, label.WhenClause);
                        }

                        tests = tests == null ? test : Expression.OrElse(tests, test);
                    }
                }

                if (defaultLabel != null)
                {
                    Debug.Assert(defaultBlock == null); // NB: Factory should catch this.

                    var defaultStmts = new List<Expression>
                    {
                        // This supports 'goto default' by adding a label.
                        Expression.Label(defaultLabel)
                    };

                    AppendSectionStatements(defaultStmts);

                    defaultBlock = Expression.Block(typeof(void), defaultStmts);

                    if (section.Labels.Count > 1)
                    {
                        // This handles the case where 'default' is not by itself. We'll keep a case here to evaluate the patterns,
                        // but make the section jump to the default block we'll emit at the end.
                        sectionStmts.Add(Expression.Goto(defaultLabel));
                    }
                }
                else
                {
                    AppendSectionStatements(sectionStmts);
                }

                if (sectionStmts.Count > 0)
                {
                    var sectionStmtsBlocks = (Expression)Expression.Block(typeof(void), sectionStmts);

                    var sectionBody = tests != null
                        ? Expression.IfThen(tests, sectionStmtsBlocks)
                        : sectionStmtsBlocks;

                    var sectionBlock = section.Locals.Count > 0
                        ? Expression.Block(typeof(void), section.Locals, new[] { sectionBody })
                        : sectionBody;

                    stmts.Add(sectionBlock);
                }

                void AppendSectionStatements(List<Expression> sectionStmts)
                {
                    for (int i = 0, n = section.Statements.Count; i < n; i++)
                    {
                        var stmt = section.Statements[i];

                        sectionStmts.Add(stmt);

                        if (i == n - 1 && stmt.NodeType != ExpressionType.Goto)
                        {
                            sectionStmts.Add(Expression.Goto(BreakLabel));
                        }
                    }
                }

                static bool IsAlwaysTrue(Expression e) => e is ConstantExpression c && c.Value is bool b && b;
            }

            if (defaultBlock != null)
            {
                stmts.Add(defaultBlock);
            }

            var res =
                Expression.Block(
                    typeof(void),
                    new[] { obj },
                    Expression.Assign(obj, SwitchValue),
                    stmts.Count == 0
                        ? Expression.Empty()
                        : Expression.Block(typeof(void), Variables, stmts),
                    Expression.Label(BreakLabel)
                );

            return res;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="PatternSwitchCSharpStatement"/> that represents a switch statement.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="breakLabel">The break label of the switch statement.</param>
        /// <param name="sections">The list of sections.</param>
        /// <returns>The created <see cref="PatternSwitchCSharpStatement"/>.</returns>
        public static PatternSwitchCSharpStatement SwitchStatement(Expression switchValue, LabelTarget breakLabel, params SwitchSection[]? sections) =>
            SwitchStatement(switchValue, breakLabel, variables: null, (IEnumerable<SwitchSection>?)sections);

        /// <summary>
        /// Creates a <see cref="PatternSwitchCSharpStatement"/> that represents a switch statement.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="breakLabel">The break label of the switch statement.</param>
        /// <param name="sections">The list of sections.</param>
        /// <returns>The created <see cref="PatternSwitchCSharpStatement"/>.</returns>
        public static PatternSwitchCSharpStatement SwitchStatement(Expression switchValue, LabelTarget breakLabel, IEnumerable<SwitchSection>? sections) =>
            SwitchStatement(switchValue, breakLabel, variables: null, sections);

        /// <summary>
        /// Creates a <see cref="PatternSwitchCSharpStatement"/> that represents a switch statement.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="breakLabel">The break label of the switch statement.</param>
        /// <param name="variables">The variables in scope of the sections.</param>
        /// <param name="sections">The list of sections.</param>
        /// <returns>The created <see cref="PatternSwitchCSharpStatement"/>.</returns>
        public static PatternSwitchCSharpStatement SwitchStatement(Expression switchValue, LabelTarget breakLabel, IEnumerable<ParameterExpression>? variables, params SwitchSection[]? sections) =>
            SwitchStatement(switchValue, breakLabel, variables, (IEnumerable<SwitchSection>?)sections);

        /// <summary>
        /// Creates a <see cref="PatternSwitchCSharpStatement"/> that represents a switch statement.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="breakLabel">The break label of the switch statement.</param>
        /// <param name="variables">The variables in scope of the sections.</param>
        /// <param name="sections">The list of sections.</param>
        /// <returns>The created <see cref="PatternSwitchCSharpStatement"/>.</returns>
        public static PatternSwitchCSharpStatement SwitchStatement(Expression switchValue, LabelTarget breakLabel, IEnumerable<ParameterExpression>? variables, IEnumerable<SwitchSection>? sections)
        {
            RequiresCanRead(switchValue, nameof(switchValue));
            RequiresNotNull(breakLabel, nameof(breakLabel));

            if (switchValue.Type == typeof(void))
                throw ArgumentCannotBeOfTypeVoid(nameof(switchValue));

            if (breakLabel.Type != typeof(void))
                throw Error.SwitchBreakLabelShouldBeVoid(nameof(breakLabel));

#pragma warning disable CA1062 // Validate arguments of public methods. (See bug https://github.com/dotnet/roslyn-analyzers/issues/6163)
            var sectionsList = sections.ToReadOnly();

            var n = sectionsList.Count;

            if (n > 0)
            {
                var foundDefaultLabel = false;
                var allLabels = new HashSet<LabelTarget>();

                for (var i = 0; i < n; i++)
                {
                    var section = sectionsList[i];

                    RequiresNotNull(section, nameof(sections));

                    foreach (var label in section.Labels)
                    {
                        if (label.Label != null && !allLabels.Add(label.Label))
                            throw Error.DuplicateLabelInSwitchStatement(label.Label, nameof(sections), i);

                        if (label.IsDefault)
                        {
                            if (foundDefaultLabel)
                                throw Error.FoundMoreThanOneDefaultLabel(nameof(sections), i);

                            foundDefaultLabel = true;
                        }
                    }

                    // NB: There's at least one label and we've checked that all labels are uniformly typed.

                    var inputType = section.Labels[0].Pattern.InputType;

                    if (!AreReferenceAssignable(inputType, switchValue.Type))
                        throw Error.SwitchValueTypeDoesNotMatchPatternInputType(inputType, switchValue.Type, nameof(sections), i);
                }
            }
#pragma warning restore CA1062 // Validate arguments of public methods

            var variableList = CheckUniqueVariables(variables, nameof(variables));

            return new PatternSwitchCSharpStatement(switchValue, breakLabel, variableList, sectionsList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="PatternSwitchCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitSwitch(PatternSwitchCSharpStatement node) =>
            node.Update(
                Visit(node.SwitchValue),
                VisitLabelTarget(node.BreakLabel),
                VisitAndConvert(node.Variables, nameof(VisitSwitch)),
                Visit(node.Sections, VisitSwitchSection)
            );
    }
}
