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
    //         whether that flag is set in order to de-nullify the receiver type prior to checking for
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
    public abstract partial class ConditionalAccessCSharpExpression : CSharpExpression
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
            // NB: We optimize for cases where the receiver is a conditional access itself.
            //
            //     For example, consider the following expression:
            //
            //       x?.Y?.Z
            //
            //     We want to turn this into a series of nested conditionals, like this:
            //
            //       var result = default(T);
            //
            //       var t0 = x;
            //       if (t0 != null)
            //       {
            //         var t1 = t0.Y;
            //         if (t1 != null)
            //         {
            //           var t2 = t1.Z;
            //           result = t2;
            //         }
            //       }
            //
            //       return result;
            //
            //     In order to achieve this, we'll build up the resulting expression inside-out by
            //     checking for the Expression being a ConditionalAccess node itself.

            var res = default(Expression);

            var resultType = Type;
            var resultVariable = default(ParameterExpression);

            var i = 0;
            var variable = default(ParameterExpression);

            if (resultType != typeof(void))
            {
                resultVariable = Expression.Parameter(resultType, "__result");
                variable = Expression.Parameter(resultType, "__temp" + i++);
                res = Expression.Assign(resultVariable, variable);
            }
            else
            {
                res = Expression.Empty();
            }

            var current = this;
            var expression = default(Expression);

            while (current != null)
            {
                expression = current.Expression;
                var expressionType = expression.Type;

                var newVariable = Expression.Parameter(expressionType, "__temp" + i++);

                var nonNullCheck = default(Expression);
                var nonNull = default(Expression);

                if (expressionType.IsNullableType())
                {
                    nonNullCheck = Expression.Property(newVariable, "HasValue");
                    nonNull = Expression.Property(newVariable, "Value");
                }
                else
                {
                    nonNullCheck = Expression.NotEqual(newVariable, Expression.Default(expressionType));
                    nonNull = newVariable;
                }

                var eval = current.ReduceAccess(nonNull);

                var whenNotNull = default(Expression);

                if (eval.Type != typeof(void))
                {
                    if (eval.Type != variable.Type)
                    {
                        eval = Expression.Convert(eval, variable.Type);
                    }

                    eval = Expression.Assign(variable, eval);

                    whenNotNull =
                        Expression.Block(
                            new[] { variable },
                            eval,
                            res
                        );
                }
                else
                {
                    whenNotNull =
                        Expression.Block(
                            eval,
                            res
                        );
                }

                res =
                    Expression.IfThen(
                        nonNullCheck,
                        whenNotNull
                    );

                variable = newVariable;

                current = expression as ConditionalAccessCSharpExpression;
            }

            if (resultVariable != null)
            {
                res =
                    Expression.Block(
                        new[] { resultVariable, variable },
                        Expression.Assign(variable, expression),
                        res,
                        resultVariable
                    );
            }
            else
            {
                res =
                    Expression.Block(
                        new[] { variable },
                        Expression.Assign(variable, expression),
                        res
                    );
            }

            // NB: We could save on temporary locals for the left-most Expression if it's a pure
            //     expression (e.g. a Constant or a Parameter). Similarly, we could save on a
            //     temporary local for the result. However, it may be better to implement this as
            //     a general-purpose optimizer (cf. the Optimizer class) such that other reductions
            //     can benefit from it as well.

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