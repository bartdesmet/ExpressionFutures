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
    public sealed class SwitchCSharpStatement : CSharpStatement
    {
        internal SwitchCSharpStatement(Expression switchValue, LabelTarget breakLabel, ReadOnlyCollection<CSharpSwitchCase> cases, Expression defaultBody)
        {
            SwitchValue = switchValue;
            BreakLabel = breakLabel;
            Cases = cases;
            DefaultBody = defaultBody;
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
        /// Gets the collection of switch cases.
        /// </summary>
        public ReadOnlyCollection<CSharpSwitchCase> Cases { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the default case defaultBody.
        /// </summary>
        public Expression DefaultBody { get; }

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
        /// <param name="cases">The <see cref="Cases" /> property of the result.</param>
        /// <param name="defaultBody">The <see cref="DefaultBody" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public SwitchCSharpStatement Update(Expression switchValue, LabelTarget breakLabel, IEnumerable<CSharpSwitchCase> cases, Expression defaultBody)
        {
            if (switchValue == this.SwitchValue && breakLabel == this.BreakLabel && cases == this.Cases && defaultBody == this.DefaultBody)
            {
                return this;
            }

            return CSharpExpression.Switch(switchValue, breakLabel, defaultBody, cases);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            var governingType = SwitchValue.Type;

            var vars = default(ParameterExpression[]);
            var exprs = default(Expression[]);

            var @break = Expression.Label(BreakLabel);

            if (governingType.IsNullableType())
            {
                var governingTypeNonNull = governingType.GetNonNullableType();

                // NB: The DLR switch expression does not optimize the nullable case into a switch table after a null check.
                //     We can do this here instead, but we could consider moving this logic to the DLR too.

                var valueLocal = Expression.Parameter(governingType);
                vars = new[] { valueLocal };

                var assignSwitchValue = Expression.Assign(valueLocal, SwitchValue);
                var hasValue = Expression.Property(valueLocal, "HasValue");
                var value = Expression.Property(valueLocal, "Value");

                var nullCase = default(CSharpSwitchCase);
                var nonNullCases = default(List<CSharpSwitchCase>);

                var hasNull = false;

                var n = Cases.Count;
                for (var i = 0; i < n; i++)
                {
                    var @case = Cases[i];

                    var foundNull = false;

                    foreach (var testValue in @case.TestValues)
                    {
                        if (testValue == null)
                        {
                            Debug.Assert(!hasNull);
                            hasNull = true;
                            foundNull = true;
                        }
                    }

                    // NB: We could optimize the case where there are other test values besides null in the null-case. However,
                    //     we'd have to hoist the case body out of the switch while still having switch cases for the non-null
                    //     test values to branch into the hoisted body. This is doable, but it seems rare to have null alongside
                    //     non-null test values in a switch case. Also, the reduced form would become more clunky for the reader
                    //     although we have other precedents a la async lambdas of course :-).

                    if (@case.TestValues.Count == 1 && foundNull)
                    {
                        nullCase = @case;

                        Debug.Assert(nonNullCases == null);

                        nonNullCases = new List<CSharpSwitchCase>(n - 1);
                        for (var j = 0; j < i; j++)
                        {
                            nonNullCases.Add(Cases[j]);
                        }
                    }
                    else if (nonNullCases != null)
                    {
                        nonNullCases.Add(@case);
                    }
                }

                if (nullCase != null)
                {
                    // We found a case with only a 'null' test value; we can lower to a null-check followed by a non-null switch,
                    // and move the 'null' case to an else branch.

                    var lowered = LowerSwitchStatement(nonNullCases, nullCase, governingTypeNonNull);

                    var @switch = Expression.Switch(value, lowered.DefaultBody, lowered.NonNullCases);

                    var nullCheck = Expression.IfThenElse(hasValue, @switch, lowered.NullCaseBody);

                    exprs = new Expression[]
                    {
                        assignSwitchValue,
                        nullCheck,
                        @break,
                    };
                }
                else if (hasNull)
                {
                    // We have a case with a 'null' test value but it has other non-null test values too; we can't lower to non-
                    // null types. This should be the rare case. The DLR will further lower to if-then-else branches.

                    var lowered = LowerSwitchStatement(Cases, null, governingType);

                    var @switch = Expression.Switch(SwitchValue, lowered.DefaultBody, lowered.NonNullCases);

                    exprs = new Expression[]
                    {
                        @switch,
                        @break,
                    };
                }
                else
                {
                    // We have no 'null' test value whatsoever; we can lower to a null-check followed by a non-null switch.

                    var lowered = LowerSwitchStatement(Cases, null, governingTypeNonNull);

                    var defaultBody = lowered.DefaultBody;

                    if (defaultBody != null)
                    {
                        var defaultLabel = Expression.Label();

                        var @switch = Expression.Switch(value, Expression.Goto(defaultLabel), lowered.NonNullCases);

                        var defaultCase = Expression.Block(Expression.Label(defaultLabel), defaultBody);

                        var nullCheck = Expression.IfThenElse(hasValue, @switch, defaultCase);

                        exprs = new Expression[]
                        {
                            assignSwitchValue,
                            nullCheck,
                            @break,
                        };
                    }
                    else
                    {
                        var @switch = Expression.Switch(value, lowered.NonNullCases);

                        var nullCheck = Expression.IfThen(hasValue, @switch);

                        exprs = new Expression[]
                        {
                            assignSwitchValue,
                            nullCheck,
                            @break,
                        };
                    }
                }
            }
            else
            {
                var lowered = LowerSwitchStatement(Cases, null, governingType);

                Debug.Assert(lowered.NullCaseBody == null);

                exprs = new Expression[]
                {
                    Expression.Switch(SwitchValue, lowered.DefaultBody, lowered.NonNullCases),
                    @break
                };
            }

            return Expression.Block(vars, exprs);
        }

        private LoweredSwitchStatement LowerSwitchStatement(IList<CSharpSwitchCase> nonNullCases, CSharpSwitchCase nullCase, Type testValueType)
        {
            var testValueToCaseMap = new Dictionary<object, CSharpSwitchCase>();

            var analyzer = new SwitchCaseGotoAnalyzer();

            foreach (var @case in Cases)
            {
                foreach (var testValue in @case.TestValues)
                {
                    testValueToCaseMap.Add(testValue.OrNullSentinel(), @case);
                }

                analyzer.Analyze(@case);
            }

            if (DefaultBody != null)
            {
                analyzer.AnalyzeDefault(DefaultBody);
            }

            var caseHasJumpInto = new HashSet<CSharpSwitchCase>();
            var defaultHasJumpInto = false;

            foreach (var info in analyzer.AllSwitchCaseInfos)
            {
                if (info.HasGotoDefault)
                {
                    if (DefaultBody == null)
                    {
                        throw Error.InvalidGotoDefault();
                    }

                    defaultHasJumpInto = true;
                }

                foreach (var gotoCase in info.GotoCases)
                {
                    var @case = default(CSharpSwitchCase);
                    if (!testValueToCaseMap.TryGetValue(gotoCase.OrNullSentinel(), out @case))
                    {
                        throw Error.InvalidGotoCase(gotoCase.ToDebugString());
                    }

                    caseHasJumpInto.Add(@case);
                }
            }

            if (caseHasJumpInto.Count > 0 || defaultHasJumpInto)
            {
                var caseJumpTargets = new Dictionary<CSharpSwitchCase, LabelTarget>();
                var defaultJumpTarget = default(LabelTarget);

                foreach (var @case in caseHasJumpInto)
                {
                    caseJumpTargets.Add(@case, Expression.Label(FormattableString.Invariant($"__case<{@case.TestValues[0].ToDebugString()}>")));
                }

                if (defaultHasJumpInto)
                {
                    defaultJumpTarget = Expression.Label("__default");
                }

                var rewriter = new SwitchCaseRewriter(testValue => caseJumpTargets[testValueToCaseMap[testValue.OrNullSentinel()]], defaultJumpTarget);

                var newNonNullCases = new SwitchCase[nonNullCases.Count];

                var i = 0;
                foreach (var @case in nonNullCases)
                {
                    var newBody = rewriter.Visit(@case.Body);

                    var jumpTarget = default(LabelTarget);
                    if (caseJumpTargets.TryGetValue(@case, out jumpTarget))
                    {
                        newBody = Expression.Block(typeof(void), Expression.Label(jumpTarget), newBody);
                    }

                    newNonNullCases[i++] = ConvertSwitchCase(@case, newBody, testValueType);
                }

                var newNullCaseBody = rewriter.Visit(nullCase?.Body);

                newNullCaseBody = EnsureVoid(newNullCaseBody);

                if (nullCase != null)
                {
                    var jumpTarget = default(LabelTarget);
                    if (caseJumpTargets.TryGetValue(nullCase, out jumpTarget))
                    {
                        newNullCaseBody = Expression.Block(typeof(void), Expression.Label(jumpTarget), newNullCaseBody);
                    }
                }

                var newDefaultBody = rewriter.Visit(DefaultBody);

                if (defaultHasJumpInto)
                {
                    newDefaultBody = Expression.Block(typeof(void), Expression.Label(defaultJumpTarget), newDefaultBody);
                }

                newDefaultBody = EnsureVoid(newDefaultBody);

                return new LoweredSwitchStatement
                {
                    DefaultBody = newDefaultBody,
                    NonNullCases = newNonNullCases,
                    NullCaseBody = newNullCaseBody,
                };
            }
            else
            {
                var newNonNullCases = nonNullCases.Select(@case => ConvertSwitchCase(@case, @case.Body, testValueType)).ToArray();
                var newDefaultBody = EnsureVoid(DefaultBody);
                var newNullCaseBody = EnsureVoid(nullCase?.Body);

                return new LoweredSwitchStatement
                {
                    DefaultBody = newDefaultBody,
                    NonNullCases = newNonNullCases,
                    NullCaseBody = newNullCaseBody,
                };
            }
        }

        private static SwitchCase ConvertSwitchCase(CSharpSwitchCase @case, Expression body, Type type)
        {
            return Expression.SwitchCase(EnsureVoid(body), @case.TestValues.Select(testValue => Expression.Constant(testValue, type)));
        }

        private static Expression EnsureVoid(Expression expression)
        {
            if (expression != null && expression.Type != typeof(void))
            {
                expression = Expression.Block(typeof(void), expression);
            }

            return expression;
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
            public SwitchCaseInfo? Default;

            private SwitchCaseInfo _info;
            private static HashSet<object> s_empty;

            public IEnumerable<SwitchCaseInfo> AllSwitchCaseInfos
            {
                get
                {
                    foreach (var info in SwitchCaseInfos.Values)
                    {
                        yield return info;
                    }

                    if (Default != null)
                    {
                        yield return Default.Value;
                    }
                }
            }

            public void Analyze(CSharpSwitchCase @case)
            {
                var info = Analyze(@case.Body);

                SwitchCaseInfos.Add(@case, info);
            }

            public void AnalyzeDefault(Expression expression)
            {
                var info = Analyze(expression);

                Default = info;
            }

            private SwitchCaseInfo Analyze(Expression expr)
            {
                Debug.Assert(_info.HasGotoDefault == false);
                Debug.Assert(_info.GotoCases == null);

                Visit(expr);

                if (_info.GotoCases == null)
                {
                    _info.GotoCases = (s_empty ?? (s_empty = new HashSet<object>()));
                }

                var info = _info;
                _info = default(SwitchCaseInfo);
                return info;
            }

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
            public Expression DefaultBody;
            public SwitchCase[] NonNullCases;
            public Expression NullCaseBody;
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
            return Switch(switchValue, breakLabel, null, (IEnumerable<CSharpSwitchCase>)cases);
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
                        else
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

            // NB: No check for DefaultBody to be of type void; we'll make it void in Reduce if need be.

            return new SwitchCSharpStatement(switchValue, breakLabel, casesList, defaultBody);
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
            return node.Update(Visit(node.SwitchValue), VisitLabelTarget(node.BreakLabel), Visit(node.Cases, VisitSwitchCase), Visit(node.DefaultBody));
        }
    }
}
