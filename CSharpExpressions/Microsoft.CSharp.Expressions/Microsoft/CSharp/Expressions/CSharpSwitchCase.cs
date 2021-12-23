﻿// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    // DESIGN: With the C# 7.0 proposal for pattern matching, it may make more sense to
    //         represent test values as expressions rather than objects. This also impacts
    //         our `goto case` support. See https://github.com/dotnet/roslyn/issues/5757.

    // TODO: Could possibly have optimized layouts for some governing types?

    /// <summary>
    /// Represents a switch case.
    /// </summary>
    public sealed partial class CSharpSwitchCase
    {
        internal CSharpSwitchCase(ReadOnlyCollection<object> testValues, ReadOnlyCollection<Expression> statements)
        {
            TestValues = testValues;
            Statements = statements;
        }

        /// <summary>
        /// Gets a collection of values to test for.
        /// </summary>
        public ReadOnlyCollection<object> TestValues { get; }

        /// <summary>
        /// Gets a collection of <see cref="Expression" /> nodes representing the body of the case.
        /// </summary>
        public ReadOnlyCollection<Expression> Statements { get; }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="statements">The <see cref="Statements" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public CSharpSwitchCase Update(IEnumerable<Expression> statements)
        {
            if (statements == this.Statements)
            {
                return this;
            }

            return CSharpExpression.SwitchCase(TestValues, statements);
        }
    }

    partial class CSharpExpression
    {
        private static readonly object s_SwitchCaseDefaultValue = new SwitchDefaultValue();
        private static ReadOnlyCollection<object> s_default;

        /// <summary>
        /// Gets an object representing the 'default' case.
        /// </summary>
        public static object SwitchCaseDefaultValue => s_SwitchCaseDefaultValue;

        // NB: Generic factory methods below help the user to provide consistent typing.
        //     The non-generic overload is used for easier binding without having to close the generic parameter.

        /// <summary>
        /// Creates a <see cref="CSharpSwitchCase"/> that represents a switch case.
        /// </summary>
        /// <typeparam name="T">The type of the test values.</typeparam>
        /// <param name="testValues">The collection of values to test for.</param>
        /// <param name="statements">The statements in the body of the case.</param>
        /// <returns>The created <see cref="CSharpSwitchCase"/>.</returns>
        public static CSharpSwitchCase SwitchCase<T>(IEnumerable<T> testValues, params Expression[] statements)
        {
            return SwitchCase(testValues, (IEnumerable<Expression>)statements);
        }

        /// <summary>
        /// Creates a <see cref="CSharpSwitchCase"/> that represents a switch case.
        /// </summary>
        /// <typeparam name="T">The type of the test values.</typeparam>
        /// <param name="testValues">The collection of values to test for.</param>
        /// <param name="statements">The statements in the body of the case.</param>
        /// <returns>The created <see cref="CSharpSwitchCase"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static CSharpSwitchCase SwitchCase<T>(IEnumerable<T> testValues, IEnumerable<Expression> statements)
        {
            ContractUtils.RequiresNotNull(testValues, nameof(testValues));
            ContractUtils.RequiresNotNull(statements, nameof(statements));

            // NB: We don't check the body for Break statements; worst case we'll insert one at the end during Reduce.
            //     Note that the semantics are nonetheless consistent with C#, i.e. no implicit fall-through.

            CheckValidSwitchType(typeof(T));

            var testValuesList = testValues.Select(value => (object)value).ToReadOnly();

            RequiresNotEmpty(testValuesList, nameof(testValues));

            var uniqueTestValues = new HashSet<object>();

            foreach (var value in testValuesList)
            {
                if (!uniqueTestValues.Add(value))
                {
                    throw Error.DuplicateTestValue(value.ToDebugString());
                }
            }

            var statementsList = GetStatements(statements);

            return new CSharpSwitchCase(testValuesList, statementsList);
        }

        /// <summary>
        /// Creates a <see cref="CSharpSwitchCase"/> that represents a switch case.
        /// </summary>
        /// <param name="testValues">The collection of values to test for.</param>
        /// <param name="statements">The statements in the body of the case.</param>
        /// <returns>The created <see cref="CSharpSwitchCase"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static CSharpSwitchCase SwitchCase(IEnumerable<object> testValues, params Expression[] statements)
        {
            return SwitchCase(testValues, (IEnumerable<Expression>)statements);
        }

        /// <summary>
        /// Creates a <see cref="CSharpSwitchCase"/> that represents a switch case.
        /// </summary>
        /// <param name="testValues">The collection of values to test for.</param>
        /// <param name="statements">The statements in the body of the case.</param>
        /// <returns>The created <see cref="CSharpSwitchCase"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static CSharpSwitchCase SwitchCase(IEnumerable<object> testValues, IEnumerable<Expression> statements)
        {
            RequiresNotNull(testValues, nameof(testValues));
            RequiresNotNull(statements, nameof(statements));

            // NB: We don't check the body for Break statements; worst case we'll insert one at the end during Reduce.
            //     Note that the semantics are nonetheless consistent with C#, i.e. no implicit fall-through.

            var testValuesList = testValues.ToReadOnly();
            RequiresNotEmpty(testValuesList, nameof(testValues));

            var testType = default(Type);

            var uniqueTestValues = new HashSet<object>();

            foreach (var testValue in testValuesList)
            {
                if (!uniqueTestValues.Add(testValue))
                {
                    throw Error.DuplicateTestValue(testValue);
                }

                // NB: Null is fine; every valid governing type in C# has a nullable variant (trivial for string).

                if (testValue != null && testValue != SwitchCaseDefaultValue)
                {
                    var testValueType = testValue.GetType();
                    if (testType == null)
                    {
                        testType = testValueType;
                    }
                    else
                    {
                        if (testType != testValueType)
                        {
                            throw Error.TestValuesShouldHaveConsistentType();
                        }
                    }
                }
            }

            var statementsList = GetStatements(statements);

            return new CSharpSwitchCase(testValuesList, statementsList);
        }

        /// <summary>
        /// Creates a <see cref="CSharpSwitchCase"/> that represents a default switch case.
        /// </summary>
        /// <param name="statements">The statements in the body of the case.</param>
        /// <returns>The created <see cref="CSharpSwitchCase"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static CSharpSwitchCase SwitchCaseDefault(params Expression[] statements)
        {
            return SwitchCaseDefault((IEnumerable<Expression>)statements);
        }

        /// <summary>
        /// Creates a <see cref="CSharpSwitchCase"/> that represents a default switch case.
        /// </summary>
        /// <param name="statements">The statements in the body of the case.</param>
        /// <returns>The created <see cref="CSharpSwitchCase"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static CSharpSwitchCase SwitchCaseDefault(IEnumerable<Expression> statements)
        {
            var testValues = s_default ?? (s_default = new TrueReadOnlyCollection<object>(new[] { s_SwitchCaseDefaultValue }));
            return SwitchCase(testValues, statements);
        }

        private static ReadOnlyCollection<Expression> GetStatements(IEnumerable<Expression> statements)
        {
            var statementsList = statements.ToReadOnly();

            // DESIGN: Require non-empty body with explicit Break or allow empty with implicit Break?
            //         Note we don't do a control flow analysis to check for the required Break on all paths.

            RequiresNotEmpty(statementsList, nameof(statements));
            RequiresNotNullItems(statementsList, nameof(statements));

            return statementsList;
        }

        class SwitchDefaultValue
        {
            public override string ToString()
            {
                return "default";
            }
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="CSharpSwitchCase" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpSwitchCase VisitSwitchCase(CSharpSwitchCase node)
        {
            return node.Update(Visit(node.Statements));
        }
    }
}
