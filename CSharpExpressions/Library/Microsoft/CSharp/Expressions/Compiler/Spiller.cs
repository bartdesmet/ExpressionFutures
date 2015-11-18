// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Linq.Expressions.Compiler;
using System.Reflection;

namespace Microsoft.CSharp.Expressions.Compiler
{
    // NB: This is a devious way to leverage the StackSpiller from LINQ to rewrite our Await nodes.
    //     Ideally, we can plug in our custom node to the StackSpiller over there.

    // TODO: Replace with a real spiller that's more thorough than LINQ's. Current limitations lead to invalid
    //       control flow operations to resume an async operation when it's buried in a bigger expression, e.g.
    //
    //         -(await x)    -->   no spilling but state machine jumps into '-' operator
    //         1 + await x   -->   spills '1' but state machine jumps into '+' operator
    //         await x + 1   -->   no spilling but state machine jumps into '+' operator
    //
    //       The current approach with the AssignmentPercolator is too limiting but still useful with a better
    //       approach to spilling. I.e. if every await site results in a temporary and the containing expression
    //       gets rewritten to spill intermediate evaluations into temporaries (a la SSA) we can still push down
    //       the assignments of await results to temporaries using the AssignmentPercolator, e.g.
    //
    //           -(await x)
    //       -->
    //           { t0 = x; t1 = await t0; t2 = -t1; }
    //       -->
    //           { t0 = x; t1 = { a1 = t0.GetAwaiter(); ...; L: ...; r1 = a1.GetResult(); }; t2 = -t1; }
    //       -->
    //           result = { t0 = x; t1 = { a1 = t0.GetAwaiter(); ...; L: ...; r1 = a1.GetResult(); }; t2 = -t1; }
    //       -->
    //           result = { t0 = x; { a1 = t0.GetAwaiter(); ...; L: ...; t1 = r1 = a1.GetResult(); }; t2 = -t1; }
    //       -->
    //           { t0 = x; { a1 = t0.GetAwaiter(); ...; L: ...; t1 = r1 = a1.GetResult(); }; result = t2 = -t1; }

    /// <summary>
    /// Utility to perform stack spilling to ensure an empty evaluation stack upon starting the evaluation of an
    /// await expression.
    /// </summary>
    /// <remarks>
    /// An example of stack spilling is shown below:
    /// <code>
    ///   F(A, await T, B)
    /// </code>
    /// In here, A and T are evaluated before awaiting T. The result of evaluating those subexpressions has to be
    /// stored prior to performing the await. In case the asynchronous code path is picked, all this intermediate
    /// evaluation state needs to be kept on the heap in order to restore it after the asynchronous operation
    /// completes and prior to evaluating B. Stack spilling will effectively turn the code into:
    /// <code>
    ///   var __1 = A;
    ///   var __2 = T;
    ///   var __3 = await __2;
    ///   F(__1, __3, B)
    /// </code>
    /// where the compiler-generated variables will be hoisted.
    /// </remarks>
    internal static class Spiller
    {
#if OLD
        private static readonly SpillSiteDecorator s_decorator = new SpillSiteDecorator();
        private static readonly SpillSiteJanitor s_janitor = new SpillSiteJanitor();
#endif

        public static Expression Spill(Expression expression)
        {
#if OLD
            var decorated = s_decorator.Visit(expression);
            var spilled = StackSpiller.AnalyzeLambda(Expression.Lambda(decorated)).Body;

            var cleaned = s_janitor.Visit(spilled);
            return cleaned;
#else
            var decorated = expression;
            var spilled = StackSpiller.AnalyzeLambda(Expression.Lambda(decorated)).Body;

            var cleaned = spilled;
            return cleaned;
#endif
        }

#if OLD
        class SpillSiteDecorator : ShallowVisitor
        {
            [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected internal override Expression VisitAwait(AwaitCSharpExpression node)
            {
                var operand = Spiller.Spill(node.Operand);
                var updated = node.Update(operand);
                var quoted = SpillHelpers.Quote(updated);
                return Expression.TryFinally(quoted, Expression.Empty());
            }
        }

        class SpillSiteJanitor : ShallowVisitor
        {
            [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected override Expression VisitBlock(BlockExpression node)
            {
                // NB: This gets rid of SpilledExpressionBlock nodes
                return Expression.Block(node.Type, node.Variables, Visit(node.Expressions));
            }

            [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected override Expression VisitTry(TryExpression node)
            {
                var res = default(Expression);

                if (node.Handlers.Count == 0 && node.Finally != null)
                {
                    if (node.Finally.NodeType == ExpressionType.Default && node.Finally.Type == typeof(void))
                    {
                        if (node.Body.NodeType == ExpressionType.Call)
                        {
                            var unquoted = default(Expression);
                            if (SpillHelpers.TryUnquote((MethodCallExpression)node.Body, out unquoted))
                            {
                                res = Visit(unquoted);
                            }
                        }
                    }
                }

                if (res == null)
                {
                    res = base.VisitTry(node);
                }

                return res;
            }
        }
#endif
    }

#if OLD
    static class SpillHelpers
    {
        private static readonly MethodInfo MethodQuoteT = typeof(SpillHelpers).GetMethod(nameof(QuoteT));
        private static readonly MethodInfo MethodQuoteVoid = typeof(SpillHelpers).GetMethod(nameof(QuoteVoid));

        public static Expression Quote(Expression expression)
        {
            var quoted = Expression.Constant(expression, typeof(Expression));
            var method = expression.Type == typeof(void) ? MethodQuoteVoid : MethodQuoteT.MakeGenericMethod(expression.Type);
            return Expression.Call(method, quoted);
        }

        public static bool TryUnquote(MethodCallExpression expression, out Expression unquoted)
        {
            if (expression.Method.DeclaringType == typeof(SpillHelpers))
            {
                var quoted = (ConstantExpression)expression.Arguments[0];
                unquoted = (Expression)quoted.Value;
                return true;
            }

            unquoted = null;
            return false;
        }

        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "expression", Justification = "Used as marker method.")]
        public static T QuoteT<T>(Expression expression)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "expression", Justification = "Used as marker method.")]
        public static void QuoteVoid(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
#endif
}
