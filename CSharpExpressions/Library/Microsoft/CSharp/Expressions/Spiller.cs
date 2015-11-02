// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Linq.Expressions.Compiler;
using System.Reflection;

namespace Microsoft.CSharp.Expressions
{
    // NB: This is a devious way to leverage the StackSpiller from LINQ to rewrite our Await nodes.
    //     Ideally, we can plug in our custom node to the StackSpiller over there.

    internal static class Spiller
    {
        private static readonly SpillSiteDecorator s_decorator = new SpillSiteDecorator();
        private static readonly SpillSiteJanitor s_janitor = new SpillSiteJanitor();

        public static Expression Spill(Expression expression)
        {
            var decorated = s_decorator.Visit(expression);
            var spilled = StackSpiller.AnalyzeLambda(Expression.Lambda(decorated)).Body;

            var cleaned = s_janitor.Visit(spilled);
            return cleaned;
        }

        class SpillSiteDecorator : CSharpExpressionVisitor
        {
            [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected internal override Expression VisitAwait(AwaitCSharpExpression node)
            {
                var operand = Spiller.Spill(node.Operand);
                var updated = node.Update(operand);
                var quoted = SpillHelpers.Quote(updated);
                return Expression.TryFinally(quoted, Expression.Empty());
            }

            protected internal override Expression VisitAsyncLambda<T>(AsyncCSharpExpression<T> node)
            {
                // NB: Keep hands off nested lambdas
                return node;
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                // NB: Keep hands off nested lambdas
                return node;
            }
        }

        class SpillSiteJanitor : CSharpExpressionVisitor
        {
            [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected override Expression VisitBlock(BlockExpression node)
            {
                // NB: This gets rrid of SpilledExpressionBlock nodes
                return Expression.Block(node.Type, node.Variables, Visit(node.Expressions));
            }

            [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected override Expression VisitTry(TryExpression node)
            {
                if (node.Handlers.Count == 0 && node.Finally != null)
                {
                    if (node.Finally.NodeType == ExpressionType.Default && node.Finally.Type == typeof(void))
                    {
                        if (node.Body.NodeType == ExpressionType.Call)
                        {
                            var unquoted = default(Expression);
                            if (SpillHelpers.TryUnquote((MethodCallExpression)node.Body, out unquoted))
                            {
                                return Visit(unquoted);
                            }
                        }
                    }
                }

                return base.VisitTry(node);
            }

            protected internal override Expression VisitAsyncLambda<T>(AsyncCSharpExpression<T> node)
            {
                // NB: Keep hands off nested lambdas
                return node;
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                // NB: Keep hands off nested lambdas
                return node;
            }
        }
    }

    static class SpillHelpers
    {
        public static readonly MethodInfo MethodQuoteT = typeof(SpillHelpers).GetMethod(nameof(QuoteT));
        public static readonly MethodInfo MethodQuoteVoid = typeof(SpillHelpers).GetMethod(nameof(QuoteVoid));

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
}
