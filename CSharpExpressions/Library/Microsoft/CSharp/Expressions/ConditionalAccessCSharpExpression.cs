// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    // DESIGN: We have a few options to model the conditional access operators. Starting from the Roslyn
    //         approach (cf. BoundConditionalAccess), there's a bit of a mismatch with the philosophy of
    //         the expression APIs where we've modeled nodes like Call as having a receiver *as well as* 
    //         parameter bindings. Conditional accesses in Roslyn rely on member bindings which include
    //         the target member and the argument list (if any). In some way, the non-conditional nodes
    //         in the expression API bundle such a member binding with a receiver and flatten it into a
    //         single object. The conditional nodes in Roslyn rely on the two being separate so they can
    //         be composed in a ConditionalAccessExpression.
    //
    //         Options to get a familiar expression API are:
    //
    //         - extend the existing non-conditional nodes with an IsLiftedToNull property; if set true,
    //           behavior becomes conditional (borrowing the "lifted" terminology from Binary and Unary)
    //         - have parallel node types for conditional variants, e.g. ConditionalCall, with the same
    //           properties but a different node type; variants could derive from a common base class
    //         - introduce a single ConditionalAccess node type which wraps any node that supports the
    //           conditional access pattern, i.e. any expression with a receiver
    //
    //         While the third approach is tempting, it's plagued with some issues. In particular, the
    //         type of such a node would be the nullable equivalent of the underlying node's type, which
    //         means it doesn't compose well with other existing expressions (e.g. a Call on a nullable
    //         value type won't accept methods for the underlying non-null type). As such, we're giving
    //         the second approach a try now. It has the benefit of still having one node per construct
    //         as is appears in the user surface, meaning that expressions see a form that's close to
    //         what the user wrote. During the Reduce step, we can optimize the generated code by using
    //         branches rather than a bunch of nested if-else nodes.
    //
    //         Note that the third approach could be made to work if we'd extend existing nodes to have
    //         an option to be "lifted over null" upon construction. The factories would simply check
    //         whether that flag is set in order to de-nullify the receiver type  prior to checking for
    //         compatibility with the specified member. The compilation of such nodes would have to be
    //         changed to take this new null-lifted mode into account. Even though the default setting
    //         for the flag would be backwards compatible, it would still break custom visitors that
    //         didn't know about the null-lifting properties and inspect the tree without taking this
    //         into account (rewrites with Update would still work fine though). This, too, seems to
    //         suggest that option two is the better one to pursue, short of modeling the construct in
    //         a way closer to Roslyn, possibly at the cost of familiarity.

    /// <summary>
    /// Base class for conditional access (null-propagating) expressions.
    /// </summary>
    public abstract class ConditionalAccessCSharpExpression : CSharpExpression
    {
        /// <summary>
        /// Creates a new conditional access expression.
        /// </summary>
        /// <param name="expression">The expression to access conditionally.</param>
        protected ConditionalAccessCSharpExpression(Expression expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// Gets the <see cref="System.Linq.Expressions.Expression"/> representing the conditionally accessed receiver.
        /// </summary>
        public Expression Expression
        {
            get;
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="System.Linq.Expressions.Expression" /> represents. (Inherited from <see cref="System.Linq.Expressions.Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public sealed override Type Type => UnderlyingType.GetConditionalType();

        /// <summary>
        /// Gets the result type of the underlying access.
        /// </summary>
        protected abstract Type UnderlyingType { get; }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public sealed override Expression Reduce()
        {
            // TODO: optimize for the case where Expression is a conditional access itself
            //       note we could possibly reuse variables of the same type as well

            var expressionType = Expression.Type;
            var variable = Expression.Parameter(expressionType);

            var resultType = Type;
            var resultVariable = default(ParameterExpression);

            if (resultType != typeof(void))
            {
                resultVariable = Expression.Parameter(resultType);
            }

            var assignExpressionToVariable = Expression.Assign(variable, Expression);

            var nonNullCheck = default(Expression);

            var nonNull = (Expression)variable;
            if (expressionType.IsNullableType())
            {
                nonNull = Expression.Property(variable, "Value");
                nonNullCheck = Expression.Property(variable, "HasValue");
            }
            else
            {
                nonNullCheck = Expression.NotEqual(variable, Expression.Default(expressionType));
            }

            var eval = ReduceAccess(nonNull);

            var result = default(Expression);
            if (resultType.IsNullableType())
            {
                result = Expression.Convert(eval, resultType);
            }
            else
            {
                result = eval;
            }

            var res = default(Expression);

            if (resultType != typeof(void))
            {
                res =
                    Expression.Block(
                        new[] { variable, resultVariable },
                        assignExpressionToVariable,
                        Expression.IfThen(
                            nonNullCheck,
                            Expression.Assign(resultVariable, result)
                        ),
                        resultVariable
                    );
            }
            else
            {
                res =
                    Expression.Block(
                        new[] { variable },
                        assignExpressionToVariable,
                        Expression.IfThen(
                            nonNullCheck,
                            result
                        )
                    );
            }

            return res;
        }

        /// <summary>
        /// Reduces the expression to an unconditional non-null access on the specified expression.
        /// </summary>
        /// <param name="nonNull">Non-null expression to apply the access to.</param>
        /// <returns>The reduced expression.</returns>
        protected abstract Expression ReduceAccess(Expression nonNull);
    }
}