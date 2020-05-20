// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    // NB: We have a separate node for switch statements in order to address a few shortcomings in LINQ:
    //     - Ability to have an empty switch
    //     - Support for "goto case" and "goto default" in case bodies

    /// <summary>
    /// Represents a switch statement.
    /// </summary>
    public sealed partial class SwitchCSharpStatement : CSharpStatement
    {
        internal SwitchCSharpStatement(Expression switchValue, LabelTarget breakLabel, ReadOnlyCollection<ParameterExpression> variables, ReadOnlyCollection<CSharpSwitchCase> cases)
        {
            SwitchValue = switchValue;
            BreakLabel = breakLabel;
            Variables = variables;
            Cases = cases;
        }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the value to be tested against each case.
        /// </summary>
        public Expression SwitchValue { get; }

        /// <summary>
        /// Gets the <see cref="LabelTarget"/> representing the break label of the switch statement.
        /// </summary>
        public LabelTarget BreakLabel { get; }

        /// <summary>
        /// Gets a collection of variables in scope for the switch cases.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Variables { get; }

        /// <summary>
        /// Gets the collection of switch cases.
        /// </summary>
        public ReadOnlyCollection<CSharpSwitchCase> Cases { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Switch;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitSwitch(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="switchValue">The <see cref="SwitchValue" /> property of the result.</param>
        /// <param name="breakLabel">The <see cref="BreakLabel"/> property of the result.</param>
        /// <param name="variables">The <see cref="Variables" /> property of the result.</param>
        /// <param name="cases">The <see cref="Cases" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public SwitchCSharpStatement Update(Expression switchValue, LabelTarget breakLabel, IEnumerable<ParameterExpression> variables, IEnumerable<CSharpSwitchCase> cases)
        {
            if (switchValue == this.SwitchValue && breakLabel == this.BreakLabel && variables == this.Variables && cases == this.Cases)
            {
                return this;
            }

            return CSharpExpression.Switch(switchValue, breakLabel, variables, cases);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            // Step 1 - Rewrite GotoCase and GotoDefault statements.
            var cases = ReduceGotos(Cases);

            // Step 2 - Analyze the switch to find special labels (default and null) and determine their use.
            var analysis = Analyze(cases);

            // Step 3 - Make sure the default case (if it exists) is by itself.
            analysis.EnsureLonelyDefault();

            // Step 4 - Lower the switch statement.
            if (SwitchValue.Type.IsNullableType())
            {
                return ReduceNullable(analysis);
            }
            else
            {
                return ReduceNonNullable(analysis);
            }
        }

        private static IList<CSharpSwitchCase> ReduceGotos(IList<CSharpSwitchCase> cases)
        {
            var testValueToCaseMap = new Dictionary<object, CSharpSwitchCase>();

            var analyzer = new SwitchCaseGotoAnalyzer();

            var defaultCase = default(CSharpSwitchCase);

            foreach (var @case in cases)
            {
                foreach (var testValue in @case.TestValues)
                {
                    if (testValue == SwitchCaseDefaultValue)
                    {
                        defaultCase = @case;
                    }

                    testValueToCaseMap.Add(testValue.OrNullSentinel(), @case);
                }

                analyzer.Analyze(@case);
            }

            var caseHasJumpInto = new HashSet<CSharpSwitchCase>();

            foreach (var info in analyzer.SwitchCaseInfos.Values)
            {
                if (info.HasGotoDefault)
                {
                    if (defaultCase == null)
                    {
                        throw Error.InvalidGotoDefault();
                    }

                    caseHasJumpInto.Add(defaultCase);
                }

                foreach (var gotoCase in info.GotoCases)
                {
                    if (!testValueToCaseMap.TryGetValue(gotoCase.OrNullSentinel(), out CSharpSwitchCase @case))
                    {
                        throw Error.InvalidGotoCase(gotoCase.ToDebugString());
                    }

                    caseHasJumpInto.Add(@case);
                }
            }

            if (caseHasJumpInto.Count > 0)
            {
                var caseJumpTargets = new Dictionary<CSharpSwitchCase, LabelTarget>();
                var defaultJumpTarget = default(LabelTarget);

                foreach (var @case in caseHasJumpInto)
                {
                    var label = default(LabelTarget);

                    if (@case == defaultCase)
                    {
                        label = defaultJumpTarget = Expression.Label("__default");
                    }
                    else
                    {
                        label = Expression.Label(FormattableString.Invariant($"__case<{@case.TestValues[0].ToDebugString()}>"));
                    }

                    caseJumpTargets.Add(@case, label);
                }

                var rewriter = new SwitchCaseRewriter(testValue => caseJumpTargets[testValueToCaseMap[testValue.OrNullSentinel()]], defaultJumpTarget);

                var newCases = new CSharpSwitchCase[cases.Count];

                var i = 0;
                foreach (var @case in cases)
                {
                    var newBody = rewriter.Visit(@case.Statements);

                    if (caseJumpTargets.TryGetValue(@case, out LabelTarget jumpTarget))
                    {
                        newBody = newBody.AddFirst(Expression.Label(jumpTarget)).ToReadOnly();
                    }

                    newCases[i++] = @case.Update(newBody);
                }

                return newCases;
            }
            else
            {
                return cases;
            }
        }

        private static SwitchAnalysis Analyze(IList<CSharpSwitchCase> cases)
        {
            var res = new SwitchAnalysis
            {
                OtherCases = new List<CSharpSwitchCase>()
            };

            var n = cases.Count;
            for (var i = 0; i < n; i++)
            {
                var @case = cases[i];

                var foundNull = false;
                var foundDefault = false;

                foreach (var testValue in @case.TestValues)
                {
                    if (testValue == null)
                    {
                        foundNull = true;
                    }
                    else if (testValue == SwitchCaseDefaultValue)
                    {
                        foundDefault = true;
                    }
                }

                // NB: We could optimize the case where there are other test values besides null in the null-case. However,
                //     we'd have to hoist the case body out of the switch while still having switch cases for the non-null
                //     test values to branch into the hoisted body. This is doable, but it seems rare to have null alongside
                //     non-null test values in a switch case. Also, the reduced form would become more clunky for the reader
                //     although we have other precedents a la async lambdas of course :-).

                var lonely = @case.TestValues.Count == 1;

                if (foundNull)
                {
                    res.NullCase = @case;
                    res.IsNullLonely = lonely;
                }

                if (foundDefault)
                {
                    res.DefaultCase = @case;
                    res.IsDefaultLonely = lonely;
                }

                if (!foundNull && !foundDefault)
                {
                    res.OtherCases.Add(@case);
                }
            }

            return res;
        }

        private Expression ReduceNonNullable(SwitchAnalysis analysis)
        {
            var lowered = LowerSwitchStatement(analysis, SwitchValue.Type, hoistNull: false);

            Expression res;

            if (Variables.Count > 0)
            {
                // NB: Variable scope should not include the switch value.

                var value = Expression.Parameter(SwitchValue.Type);

                var @switch = lowered.Make(value, lowered.DefaultCase);

                var exprs = new Expression[]
                {
                    Expression.Assign(value, SwitchValue),
                    WithVariableScope(@switch),
                    Expression.Label(BreakLabel),
                };

                res = Expression.Block(new TrueReadOnlyCollection<ParameterExpression>(new[] { value }), new TrueReadOnlyCollection<Expression>(exprs));
            }
            else
            {
                var @switch = lowered.Make(SwitchValue, lowered.DefaultCase);

                var exprs = new Expression[]
                {
                    @switch,
                    Expression.Label(BreakLabel),
                };

                res = Expression.Block(new TrueReadOnlyCollection<Expression>(exprs));
            }

            return res;
        }

        private Expression ReduceNullable(SwitchAnalysis analysis)
        {
            var governingType = SwitchValue.Type;
            var governingTypeNonNull = governingType.GetNonNullableType();

            var valueLocal = Expression.Parameter(governingType, "__value");
            var vars = new[] { valueLocal };
            var assignSwitchValue = Expression.Assign(valueLocal, SwitchValue);

            Expression body;

            // NB: The DLR switch expression does not optimize the nullable case into a switch table after a null check.
            //     We can do this here instead, but we could consider moving this logic to the DLR too.

            if (analysis.NullCase != null)
            {
                if (analysis.IsNullLonely)
                {
                    // We found a case with only a 'null' test value; we can lower to a null-check followed by a non-null switch,
                    // and move the 'null' case to an else branch.

                    var hasValue = Helpers.MakeNullableHasValue(valueLocal);
                    var value = Helpers.MakeNullableGetValueOrDefault(valueLocal);

                    var lowered = LowerSwitchStatement(analysis, governingTypeNonNull, hoistNull: true);

                    var @switch = lowered.Make(value, lowered.DefaultCase);

                    body = Expression.IfThenElse(hasValue, @switch, lowered.NullCase);
                }
                else
                {
                    // We have a case with a 'null' test value but it has other non-null test values too; we can't lower to non-
                    // null types. This should be the rare case. The DLR will further lower to if-then-else branches.

                    var lowered = LowerSwitchStatement(analysis, governingType, hoistNull: false);

                    body = lowered.Make(valueLocal, lowered.DefaultCase);
                }
            }
            else
            {
                // We have no 'null' test value whatsoever; we can lower to a null-check followed by a non-null switch.

                var hasValue = Helpers.MakeNullableHasValue(valueLocal);
                var value = Helpers.MakeNullableGetValueOrDefault(valueLocal);

                var lowered = LowerSwitchStatement(analysis, governingTypeNonNull, hoistNull: false);

                var defaultBody = lowered.DefaultCase;

                if (defaultBody != null)
                {
                    var defaultLabel = Expression.Label("__default");

                    var @switch = lowered.Make(value, Expression.Goto(defaultLabel));

                    var defaultCase = Expression.Block(Expression.Label(defaultLabel), defaultBody);

                    body = Expression.IfThenElse(hasValue, @switch, defaultCase);
                }
                else
                {
                    var @switch = lowered.Make(value, null);

                    body = Expression.IfThen(hasValue, @switch);
                }
            }

            // NB: Variable scope should not include the switch value.

            var exprs = new Expression[]
            {
                assignSwitchValue,
                WithVariableScope(body),
                Expression.Label(BreakLabel),
            };

            return Expression.Block(new TrueReadOnlyCollection<ParameterExpression>(vars), new TrueReadOnlyCollection<Expression>(exprs));
        }

        private Expression WithVariableScope(Expression expression)
        {
            if (Variables.Count > 0)
            {
                // REVIEW: Should we ensure all variables get assigned default values? Cf. when it's used
                //         in a loop and the locals don't get reinitialized. Or should we assume there's
                //         definite assignment (or enforce it)?
                expression = Expression.Block(Variables, expression);
            }

            return expression;
        }

        private static LoweredSwitchStatement LowerSwitchStatement(SwitchAnalysis analysis, Type testValueType, bool hoistNull = false)
        {
            var res = new LoweredSwitchStatement();

            var defaultCase = analysis.DefaultCase;

            if (defaultCase != null)
            {
                Debug.Assert(analysis.IsDefaultLonely);
                Debug.Assert(defaultCase.TestValues.Count == 1);

                var body = MakeBlock(defaultCase.Statements);
                res.DefaultCase = body;
            }

            var otherCases = analysis.OtherCases;
            var n = otherCases.Count;

            var cases = new List<SwitchCase>(n);
            for (var i = 0; i < n; i++)
            {
                var @case = ConvertSwitchCase(otherCases[i], testValueType);
                cases.Add(@case);
            }

            var nullCase = analysis.NullCase;

            if (nullCase != null)
            {
                if (hoistNull)
                {
                    Debug.Assert(analysis.IsNullLonely);
                    Debug.Assert(nullCase.TestValues.Count == 1);

                    var body = MakeBlock(nullCase.Statements);
                    res.NullCase = body;
                }
                else
                {
                    var @case = ConvertSwitchCase(nullCase, testValueType);
                    cases.Add(@case);
                }
            }

            res.Cases = cases;

            return res;
        }

        private static SwitchCase ConvertSwitchCase(CSharpSwitchCase @case, Type type)
        {
            return Expression.SwitchCase(MakeBlock(@case.Statements), @case.TestValues.Select(testValue => Expression.Constant(testValue, type)));
        }

        private static Expression MakeBlock(IList<Expression> expressions)
        {
            return Helpers.CreateVoid(expressions);
        }

        class ShallowSwitchCSharpExpressionVisitor : CSharpExpressionVisitor
        {
            protected internal override Expression VisitSwitch(SwitchCSharpStatement node)
            {
                // NB: Nested switch statements end the reach of "goto case" and "goto default" statements.
                return node;
            }

            protected override Expression VisitSwitch(SwitchExpression node)
            {
                // NB: Nested switch statements end the reach of "goto case" and "goto default" statements.
                // DESIGN: Should we just not care about non-C# switches here?
                return node;
            }
        }

        class SwitchCaseGotoAnalyzer : ShallowSwitchCSharpExpressionVisitor
        {
            public readonly IDictionary<CSharpSwitchCase, SwitchCaseInfo> SwitchCaseInfos = new Dictionary<CSharpSwitchCase, SwitchCaseInfo>();

            private SwitchCaseInfo _info;
            private static HashSet<object> s_empty;

            public void Analyze(CSharpSwitchCase @case)
            {
                Debug.Assert(_info.HasGotoDefault == false);
                Debug.Assert(_info.GotoCases == null);

                foreach (var stmt in @case.Statements)
                {
                    Visit(stmt);
                }

                if (_info.GotoCases == null)
                {
                    _info.GotoCases = (s_empty ?? (s_empty = new HashSet<object>()));
                }

                SwitchCaseInfos.Add(@case, _info);

                _info = default;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected internal override Expression VisitGotoCase(GotoCaseCSharpStatement node)
            {
                if (_info.GotoCases == null)
                {
                    _info.GotoCases = new HashSet<object>();
                }

                _info.GotoCases.Add(node.Value);

                return node;
            }

            protected internal override Expression VisitGotoDefault(GotoDefaultCSharpStatement node)
            {
                _info.HasGotoDefault = true;

                return node;
            }
        }

        class SwitchCaseRewriter : ShallowSwitchCSharpExpressionVisitor
        {
            private readonly Func<object, LabelTarget> _getGotoCaseLabel;
            private readonly LabelTarget _gotoDefaultLabel;

            public SwitchCaseRewriter(Func<object, LabelTarget> getGotoCaseLabel, LabelTarget gotoDefaultLabel)
            {
                _getGotoCaseLabel = getGotoCaseLabel;
                _gotoDefaultLabel = gotoDefaultLabel;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected internal override Expression VisitGotoCase(GotoCaseCSharpStatement node)
            {
                return Expression.Goto(_getGotoCaseLabel(node.Value));
            }

            protected internal override Expression VisitGotoDefault(GotoDefaultCSharpStatement node)
            {
                return Expression.Goto(_gotoDefaultLabel);
            }
        }

        struct SwitchCaseInfo
        {
            public HashSet<object> GotoCases;
            public bool HasGotoDefault;
        }

        struct LoweredSwitchStatement
        {
            public IList<SwitchCase> Cases;
            public Expression NullCase;
            public Expression DefaultCase;

            public Expression Make(Expression switchValue, Expression defaultBody)
            {
                if (Cases.Count > 0)
                {
                    return Expression.Switch(switchValue, defaultBody, null, Cases);
                }
                else
                {
                    var canDropSwitchValue = switchValue.IsPure(readOnly: true);
                    var hasDefaultBody = defaultBody != null;

                    Expression[] exprs;

                    if (canDropSwitchValue)
                    {
                        if (hasDefaultBody)
                        {
                            exprs = new[] { defaultBody };
                        }
                        else
                        {
                            return Expression.Empty();
                        }
                    }
                    else
                    {
                        if (hasDefaultBody)
                        {
                            exprs = new[] { switchValue, defaultBody };
                        }
                        else
                        {
                            exprs = new[] { switchValue };
                        }
                    }

                    return Helpers.CreateVoid(exprs);
                }
            }
        }

        class SwitchAnalysis
        {
            public bool IsDefaultLonely;
            public CSharpSwitchCase DefaultCase;

            public bool IsNullLonely;
            public CSharpSwitchCase NullCase;

            public IList<CSharpSwitchCase> OtherCases;

            public void EnsureLonelyDefault()
            {
                if (DefaultCase != null && !IsDefaultLonely)
                {
                    // We have a default case but it's mingled up with other cases, e.g.
                    //
                    //   switch (x)
                    //   {
                    //     case 1:
                    //     case 2:
                    //     case default:
                    //       ...
                    //       break;
                    //   }
                    //
                    // We can simply drop the other test values for compilation purposes, e.g.
                    //
                    //  switch (x)
                    //  {
                    //    case default:
                    //       ...
                    //       break;
                    //  }

                    var roDefaultTestValues = new TrueReadOnlyCollection<object>(new[] { SwitchCaseDefaultValue });
                    var newDefaultCase = new CSharpSwitchCase(roDefaultTestValues, DefaultCase.Statements);

                    if (DefaultCase == NullCase)
                    {
                        NullCase = null;
                        IsNullLonely = false;
                    }

                    DefaultCase = newDefaultCase;
                    IsDefaultLonely = true;
                }
            }
        }
    }

    partial class CSharpExpression
    {
        // DESIGN: Could we allow creation using LINQ SwitchCase nodes as well? How about Break etc?

        /// <summary>
        /// Creates a <see cref="SwitchCSharpStatement"/> that represents a switch statement.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="breakLabel">The break label of the switch statement.</param>
        /// <param name="cases">The set of cases to switch on.</param>
        /// <returns>The created <see cref="SwitchCSharpStatement"/>.</returns>
        public static SwitchCSharpStatement Switch(Expression switchValue, LabelTarget breakLabel, params CSharpSwitchCase[] cases)
        {
            return Switch(switchValue, breakLabel, default(IEnumerable<ParameterExpression>), (IEnumerable<CSharpSwitchCase>)cases);
        }

        /// <summary>
        /// Creates a <see cref="SwitchCSharpStatement"/> that represents a switch statement.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="breakLabel">The break label of the switch statement.</param>
        /// <param name="cases">The set of cases to switch on.</param>
        /// <returns>The created <see cref="SwitchCSharpStatement"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static SwitchCSharpStatement Switch(Expression switchValue, LabelTarget breakLabel, IEnumerable<CSharpSwitchCase> cases)
        {
            return Switch(switchValue, breakLabel, default(IEnumerable<ParameterExpression>), cases);
        }

        /// <summary>
        /// Creates a <see cref="SwitchCSharpStatement"/> that represents a switch statement.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="breakLabel">The break label of the switch statement.</param>
        /// <param name="variables">The variables in scope of the cases.</param>
        /// <param name="cases">The set of cases to switch on.</param>
        /// <returns>The created <see cref="SwitchCSharpStatement"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static SwitchCSharpStatement Switch(Expression switchValue, LabelTarget breakLabel, IEnumerable<ParameterExpression> variables, IEnumerable<CSharpSwitchCase> cases)
        {
            RequiresCanRead(switchValue, nameof(switchValue));
            RequiresNotNull(breakLabel, nameof(breakLabel));

            if (switchValue.Type == typeof(void))
            {
                throw LinqError.ArgumentCannotBeOfTypeVoid();
            }

            if (breakLabel.Type != typeof(void))
            {
                throw Error.SwitchBreakLabelShouldBeVoid();
            }

            var type = switchValue.Type;

            CheckValidSwitchType(type);

            var isNullable = type == typeof(string) || type.IsNullableType();
            var nonNullType = type.GetNonNullableType();

            // NB: Switch in C# can be empty, so less checks than LINQ here. Also no custom comparison method.

            var casesList = cases.ToReadOnly();
            if (casesList.Count > 0)
            {
                var testValues = new HashSet<object>();

                foreach (var @case in casesList)
                {
                    RequiresNotNull(@case, nameof(cases));

                    foreach (var value in @case.TestValues)
                    {
                        if (!testValues.Add(value))
                        {
                            throw Error.DuplicateTestValue(value.ToDebugString());
                        }

                        if (value == null)
                        {
                            if (!isNullable)
                            {
                                throw Error.SwitchCantHaveNullCase(type);
                            }
                        }
                        else if (value != SwitchCaseDefaultValue)
                        {
                            var valueType = value.GetType().GetNonNullableType();

                            if (valueType != nonNullType)
                            {
                                throw Error.SwitchCaseHasIncompatibleType(value.GetType(), type);
                            }
                        }
                    }
                }
            }

            var variableList = variables.ToReadOnly();

            // NB: No check for DefaultBody to be of type void; we'll make it void in Reduce if need be.

            return new SwitchCSharpStatement(switchValue, breakLabel, variableList, casesList);
        }

        /// <summary>
        /// Creates a <see cref="SwitchCSharpStatement"/> that represents a switch statement.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="breakLabel">The break label of the switch statement.</param>
        /// <param name="defaultBody">The body of the default case.</param>
        /// <param name="cases">The set of cases to switch on.</param>
        /// <returns>The created <see cref="SwitchCSharpStatement"/>.</returns>
        public static SwitchCSharpStatement Switch(Expression switchValue, LabelTarget breakLabel, Expression defaultBody, params CSharpSwitchCase[] cases)
        {
            return Switch(switchValue, breakLabel, defaultBody, (IEnumerable<CSharpSwitchCase>)cases);
        }

        /// <summary>
        /// Creates a <see cref="SwitchCSharpStatement"/> that represents a switch statement.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="breakLabel">The break label of the switch statement.</param>
        /// <param name="defaultBody">The body of the default case.</param>
        /// <param name="cases">The set of cases to switch on.</param>
        /// <returns>The created <see cref="SwitchCSharpStatement"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static SwitchCSharpStatement Switch(Expression switchValue, LabelTarget breakLabel, Expression defaultBody, IEnumerable<CSharpSwitchCase> cases)
        {
            if (defaultBody != null)
            {
                var @default = new[] { CSharpStatement.SwitchCaseDefault(defaultBody) };

                if (cases != null)
                {
                    cases = cases.Concat(@default);
                }
                else
                {
                    cases = @default;
                }
            }

            return Switch(switchValue, breakLabel, default(IEnumerable<ParameterExpression>), cases);
        }

        private static void CheckValidSwitchType(Type type)
        {
            if (!IsValidSwitchType(type))
            {
                throw Error.InvalidSwitchType(type);
            }
        }

        private static bool IsValidSwitchType(Type type)
        {
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.String:
                    return true;
            }

            return false;
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="SwitchCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitSwitch(SwitchCSharpStatement node)
        {
            return node.Update(Visit(node.SwitchValue), VisitLabelTarget(node.BreakLabel), VisitAndConvert(node.Variables, nameof(VisitSwitch)), Visit(node.Cases, VisitSwitchCase));
        }
    }
}
