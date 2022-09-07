// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

using Microsoft.CSharp.Expressions.Compiler;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a catch block used in a try statement.
    /// </summary>
    public sealed partial class CSharpCatchBlock
    {
        internal CSharpCatchBlock(ReadOnlyCollection<ParameterExpression> variables, Type test, ParameterExpression? variable, Expression body, Expression? filter)
        {
            Variables = variables;
            Test = test;
            Variable = variable;
            Body = body;
            Filter = filter;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the exception to catch.
        /// </summary>
        public Type Test { get; }

        /// <summary>
        /// Gets a collection of <see cref="ParameterExpression" /> nodes representing the local variables introduced by the statement.
        /// </summary>
        /// <remarks>
        /// This collection contains the <see cref="Variable"/>, if specified, as well as locals introduced in <see cref="Filter"/>.
        /// </remarks>
        public ReadOnlyCollection<ParameterExpression> Variables { get; }

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> representing the variable holding the exception object.
        /// </summary>
        public ParameterExpression? Variable { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> representing the body.
        /// </summary>
        public Expression Body { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> representing the filter.
        /// </summary>
        public Expression? Filter { get; }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="variables">The <see cref="Variables" /> property of the result.</param>
        /// <param name="variable">The <see cref="Variable" /> property of the result.</param>
        /// <param name="body">The <see cref="Body" /> property of the result.</param>
        /// <param name="filter">The <see cref="Filter" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public CSharpCatchBlock Update(IEnumerable<ParameterExpression> variables, ParameterExpression? variable, Expression body, Expression? filter)
        {
            if (SameElements(ref variables!, Variables) && variable == Variable && body == Body && filter == Filter)
            {
                return this;
            }

            return CSharpStatement.MakeCatchBlock(variables, Test, variable, body, filter);
        }

        internal CatchBlock Reduce(Action<ParameterExpression> hoistVariable)
        {
            // NB: System.Linq.Expressions does not support locals that cross the filter and the body of a CatchBlock, so we have to
            //     hoist these variables to a surrounding scope. In order to avoid shadowing other uses of the same variable object,
            //     e.g. in the try block, in another catch block, or in the finally block, we perform variable substitution in the
            //     filter and the body expression in order to guarantee that the hoisted variable is unique.
            //
            //     For example:
            //
            //       Block({ a }, {
            //         Try { /* use a */ }
            //         Catch (e) When (e is A a) { /* use a */ }
            //       })
            //
            //     If we hoist 'a' from the 'When' clause to a new block surrounding 'Try', it will shadow the 'a' from the outer
            //     block and change the meaning of the try block's usage of 'a'.
            //
            //       Block({ a }, {
            //         Block({ a }, {                               // BUG
            //           Try { /* use a */ }                        // BUG
            //           Catch (e) When (e is A a) { /* use a */ }
            //         })
            //       })
            //
            //     Instead, we substitute the variables associated with the Catch block using new ones which are thus guaranteed
            //     not to be used elsewhere and are safe for hoisting.
            //
            //       Block({ a }, {
            //         Block({ t }, {                               // FIX
            //           Try { /* use a */ }                        // OKAY, refers to the original 'a'
            //           Catch (e) When (e is A t) { /* use t */ }  // FIX, we introduce 't' in the catch clause subexpressions
            //         })
            //       })

            var filter = Filter;
            var body = Body;

            if (Variables.Count > 0)
            {
                var variablesToRename = new Dictionary<ParameterExpression, Expression>();

                foreach (var variable in Variables)
                {
                    if (variable != Variable) // NB: Variable gets its own scope by CatchBlock so does not need hosting.
                    {
                        var newVariable = Expression.Parameter(variable.Type, variable.Name);
                        variablesToRename.Add(variable, newVariable);
                        hoistVariable(newVariable);
                    }
                }

                body = ParameterSubstitutor.Substitute(body, variablesToRename);
                filter = ParameterSubstitutor.Substitute(filter, variablesToRename);
            }

            return Expression.MakeCatchBlock(Test, Variable, body, filter);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="CSharpCatchBlock" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpCatchBlock VisitCatchBlock(CSharpCatchBlock node) =>
            node.Update(
                VisitAndConvert(node.Variables, nameof(VisitCatchBlock)),
                VisitAndConvert(node.Variable, nameof(VisitCatchBlock)),
                Visit(node.Body),
                Visit(node.Filter)
            );
    }

    partial class CSharpStatement
    {
        /// <summary>
        /// Creates a catch all block.
        /// </summary>
        /// <param name="body">The body of the catch block.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static CSharpCatchBlock Catch(Expression body) => MakeCatchBlock(variables: null, type: null, variable: null, body, filter: null);

        /// <summary>
        /// Creates a catch all block with a filter.
        /// </summary>
        /// <param name="body">The body of the catch block.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static CSharpCatchBlock Catch(Expression body, Expression? filter) => MakeCatchBlock(variables: null, type: null, variable: null, body, filter);

        /// <summary>
        /// Creates a catch all block.
        /// </summary>
        /// <param name="variables">The variables introduced by the catch block.</param>
        /// <param name="body">The body of the catch block.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static CSharpCatchBlock Catch(IEnumerable<ParameterExpression> variables, Expression body) => MakeCatchBlock(variables, type: null, variable: null, body, filter: null);

        /// <summary>
        /// Creates a catch all block with a filter.
        /// </summary>
        /// <param name="variables">The variables introduced by the catch block.</param>
        /// <param name="body">The body of the catch block.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static CSharpCatchBlock Catch(IEnumerable<ParameterExpression> variables, Expression body, Expression? filter) => MakeCatchBlock(variables, type: null, variable: null, body, filter);

        /// <summary>
        /// Creates a catch block that handles exceptions of the specified type.
        /// </summary>
        /// <param name="test">The type of the exceptions to handle.</param>
        /// <param name="body">The body of the catch block.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static new CSharpCatchBlock Catch(Type test, Expression body) => MakeCatchBlock(variables: null, test, variable: null, body, filter: null);

        /// <summary>
        /// Creates a catch block that handles exceptions of the specified type and applies a filter.
        /// </summary>
        /// <param name="test">The type of the exceptions to handle.</param>
        /// <param name="body">The body of the catch block.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static new CSharpCatchBlock Catch(Type test, Expression body, Expression? filter) => MakeCatchBlock(variables: null, test, variable: null, body, filter);

        /// <summary>
        /// Creates a catch block that handles exceptions of the specified type.
        /// </summary>
        /// <param name="variables">The variables introduced by the catch block.</param>
        /// <param name="test">The type of the exceptions to handle.</param>
        /// <param name="body">The body of the catch block.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static CSharpCatchBlock Catch(IEnumerable<ParameterExpression> variables, Type test, Expression body) => MakeCatchBlock(variables, test, variable: null, body, filter: null);

        /// <summary>
        /// Creates a catch block that handles exceptions of the specified type and applies a filter.
        /// </summary>
        /// <param name="variables">The variables introduced by the catch block.</param>
        /// <param name="test">The type of the exceptions to handle.</param>
        /// <param name="body">The body of the catch block.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static CSharpCatchBlock Catch(IEnumerable<ParameterExpression> variables, Type test, Expression body, Expression? filter) => MakeCatchBlock(variables, test, variable: null, body, filter);

        /// <summary>
        /// Creates a catch block that handles exceptions of the type specified by the variable.
        /// </summary>
        /// <param name="variable">The variable to assign a caught exception to.</param>
        /// <param name="body">The body of the catch block.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static new CSharpCatchBlock Catch(ParameterExpression variable, Expression body) => Catch(variable, body, filter: null);

        /// <summary>
        /// Creates a catch block that handles exceptions of the type specified by the variable and applies a filter.
        /// </summary>
        /// <param name="variable">The variable to assign a caught exception to.</param>
        /// <param name="body">The body of the catch block.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static new CSharpCatchBlock Catch(ParameterExpression variable, Expression body, Expression? filter)
        {
            RequiresNotNull(variable, nameof(variable));

            return MakeCatchBlock(new[] { variable }, variable.Type, variable, body, filter: null);
        }

        /// <summary>
        /// Creates a catch block that handles exceptions of the type specified by the variable.
        /// </summary>
        /// <param name="variables">The variables introduced by the catch block.</param>
        /// <param name="variable">The variable to assign a caught exception to.</param>
        /// <param name="body">The body of the catch block.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static CSharpCatchBlock Catch(IEnumerable<ParameterExpression> variables, ParameterExpression variable, Expression body) => Catch(variables, variable, body, filter: null);

        /// <summary>
        /// Creates a catch block that handles exceptions of the type specified by the variable and applies a filter.
        /// </summary>
        /// <param name="variables">The variables introduced by the catch block.</param>
        /// <param name="variable">The variable to assign a caught exception to.</param>
        /// <param name="body">The body of the catch block.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static CSharpCatchBlock Catch(IEnumerable<ParameterExpression> variables, ParameterExpression variable, Expression body, Expression? filter)
        {
            RequiresNotNull(variable, nameof(variable));

            return MakeCatchBlock(variables, variable.Type, variable, body, filter);
        }

        /// <summary>
        /// Creates a catch block.
        /// </summary>
        /// <param name="variables">The variables introduced by the catch block.</param>
        /// <param name="type">The type of the exceptions to handle.</param>
        /// <param name="variable">The variable to assign a caught exception to.</param>
        /// <param name="body">The body of the catch block.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The created <see cref="CSharpCatchBlock"/>.</returns>
        public static CSharpCatchBlock MakeCatchBlock(IEnumerable<ParameterExpression>? variables, Type? type, ParameterExpression? variable, Expression body, Expression? filter)
        {
            var variablesList = CheckUniqueVariables(variables, nameof(variables));

            Requires(variable == null || AreEquivalent(variable.Type, type), nameof(variable));

            if (variable != null)
            {
                if (variable.IsByRef)
                    throw VariableMustNotBeByRef(variable, variable.Type, nameof(variable));

                // REVIEW: See UsingCSharpStatement for a similar situation.

                if (!variablesList.Contains(variable))
                    throw Error.CatchVariableNotInScope(variable);

                if (type == null)
                {
                    type = variable.Type;
                }
                else if (!AreEquivalent(type, variable.Type))
                {
                    throw Error.CatchTypeNotEquivalentWithVariableType(type, variable.Type);
                }
            }
            else if (type != null)
            {
                ValidateType(type, nameof(type));
            }
            else
            {
                type = typeof(object); // NB: Used to catch all.
            }

            RequiresCanRead(body, nameof(body));

            if (filter != null)
            {
                RequiresCanRead(filter, nameof(filter));

                if (filter.Type != typeof(bool))
                    throw ArgumentMustBeBoolean(nameof(filter));
            }

            return new CSharpCatchBlock(variablesList, type, variable, body, filter);
        }
    }
}
