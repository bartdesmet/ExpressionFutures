// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to percolate assignments down to the last innermost expression within blocks.
    /// This step is used to push down the assignment of an asynchronous lambda body's result to a local variable.
    /// </summary>
    /// <remarks>
    /// During async lambda rewriting, we turn the body of the lambda into a try {} catch {} statement in order
    /// to catch any exceptions and set them on the resulting task. To assign the successful result of evaluating
    /// the body to the task, we introduce a local variable and perform a top-level assignment of the rewritten
    /// body expression to that local. This leads to an assignment whose rhs can contain jump targets introduced
    /// by the rewrite to the async state machine. In order to make such jumps valid, we push down the assignment
    /// to the result variable.
    /// 
    /// For example, consider the following async lambda body:
    /// 
    /// <code>
    ///   A
    ///   await T;
    ///   B
    /// </code>
    /// 
    /// This turns into a state machine roughly similar to:
    /// 
    /// <code>
    ///   switch (__state)
    ///   {
    ///     case 1:
    ///       goto L1;
    ///       break;
    ///     case 2:
    ///       goto L2;
    ///       break;
    ///   }
    /// 
    ///   body;
    /// </code>
    /// 
    /// where the body corresponds to:
    /// 
    /// <code>
    ///   L1:
    ///     A
    ///     __awaiter = T.GetAwaiter();
    ///     if (!__awaiter.IsCompleted)
    ///     {
    ///        __state = 2;
    ///        __awaiter.OnCompleted(resume);
    ///        goto __exit;
    ///     }
    /// 
    ///   L2:
    ///     __awaiter.GetResult();
    ///     B
    /// </code>
    /// 
    /// where __state and __awaiter are hoisted variables. Assume that B's return type is non-void, therefore requiring
    /// us to save the result in order to set it on the resulting Task via the method builder. We do so by introducing
    /// a simple top-level assignment:
    /// 
    /// <code>
    ///   __result = body
    /// </code>
    /// 
    /// However, because the body contains jump targets used by the state machine's dispatch table, we have to percolate
    /// down assignments. This causes the body to be rewritten to:
    /// 
    /// <code>
    ///   L1:
    ///     ...
    ///   L2:
    ///     ...
    ///     __result = B
    /// </code>
    /// 
    /// Note that B may be a non-void label expression with a default value itself. The case is covered separately via
    /// the TypedLabelRewriter.
    /// </remarks>
    internal static class AssignmentPercolator
    {
        public static Expression Percolate(Expression expression) => Impl.Instance.Visit(expression);

        // NB: We really only need percolation for node types we introduce during the rewrite steps for async lambdas.
        //     For example, we introduce Block nodes at will in various places, and introduce Conditional nodes when
        //     rewriting non-void Try expressions. For completeness, a few other node types are handled below as well,
        //     which can come in handy later.

        private sealed class Impl : ShallowVisitor
        {
            public static readonly ExpressionVisitor Instance = new Impl();

            protected override Expression VisitBinary(BinaryExpression node)
            {
                var res = base.VisitBinary(node);

                if (res.NodeType == ExpressionType.Assign)
                {
                    var b = (BinaryExpression)res;
                    if (ShouldPercolate(b.Right.NodeType) && b.Conversion == null)
                    {
                        res = Percolate(b.Left, b.Right);
                    }
                }

                return res;
            }

            private static Expression Percolate(Expression result, Expression expr)
            {
                switch (expr.NodeType)
                {
                    // TODO: Loop
                    case ExpressionType.Block:
                        {
                            var block = (BlockExpression)expr;

                            var n = block.Expressions.Count;
                            var res = Percolate(result, block.Expressions[n - 1]);

                            return block.Update(block.Variables, block.Expressions.Take(n - 1).Concat(new[] { res }));
                        }
                    case ExpressionType.Conditional:
                        {
                            var cond = (ConditionalExpression)expr;

                            var ifTrue = Percolate(result, cond.IfTrue);
                            var ifFalse = Percolate(result, cond.IfFalse);

                            return cond.Update(cond.Test, ifTrue, ifFalse);
                        }
                    case ExpressionType.Try:
                        {
                            var @try = (TryExpression)expr;

                            var body = Percolate(result, @try.Body);

                            var oldHandlers = @try.Handlers;
                            var newHandlers = (IEnumerable<CatchBlock>)oldHandlers;

                            var n = oldHandlers.Count;
                            if (n > 0)
                            {
                                var handlers = new List<CatchBlock>(n);

                                for (var i = 0; i < n; i++)
                                {
                                    var handler = oldHandlers[i];

                                    var catchBody = Percolate(result, handler.Body);

                                    var newHandler = handler.Update(handler.Variable, handler.Filter, catchBody);
                                    handlers.Add(newHandler);
                                }

                                newHandlers = handlers;
                            }

                            return @try.Update(body, newHandlers, @try.Finally, @try.Fault);
                        }
                    case ExpressionType.Switch:
                        {
                            var @switch = (SwitchExpression)expr;

                            var defaultBody = Percolate(result, @switch.DefaultBody);

                            var oldCases = @switch.Cases;
                            var n = oldCases.Count;
                            var newCases = new List<SwitchCase>(n);

                            for (var i = 0; i < n; i++)
                            {
                                var @case = oldCases[i];

                                var caseBody = Percolate(result, @case.Body);

                                var newCase = @case.Update(@case.TestValues, caseBody);
                                newCases.Add(newCase);
                            }

                            return @switch.Update(@switch.SwitchValue, newCases, defaultBody);
                        }
                    default:
                        {
                            var res = (Expression)Expression.Assign(result, expr);

                            // NB: This is quite subtle. We don't want to change the type of the entire
                            //     tree but could be faced with covariant assignments. If we reduce to
                            //     a less derived type because of the LHS of the assignment being less
                            //     derived, Update methods for the nodes above us, such as Block, will
                            //     complain. So we convert the entire assignment to its RHS type if the
                            //     types don't match. An example is shown below:
                            //
                            //       Base b = await (Derived)x;
                            //
                            //     We know this is safe to do because a plain assignment can only be an
                            //     assignment where the RHS is reference assignable to the LHS (so not
                            //     even a boxing conversion is allowed), hence there will be no side-
                            //     effects due to user-defined conversions or so. We also know it can't
                            //     fail because the RHS is not changing typing.
                            //
                            //     Note this could be avoided if we know that we can change all nodes
                            //     encountered in percolation to void types. However, the expression
                            //     API lacks "statement expression" nodes which clearly indicate such
                            //     an intent, so detecting these "as void" nodes is a bit cumbersome.
                            //
                            //     Also note that our percolator doesn't distinguish between (reduced)
                            //     await nodes and other nodes, so we can't assume we'd only be changing
                            //     the type of assignment caused by await. While it's invalid to write
                            //     the following in C# today, it is possible in the expression API:
                            //
                            //       /*Base*/ b = { A; B; (Derived)x; }; // an expression block
                            //
                            //     If we percolate the assignment here, the type of the block changes
                            //     which can have implications to the type checking of parent nodes. So
                            //     our percolation can also make otherwise invalid "jump into expression"
                            //     cases that are rejected by the lambda compiler become valid given
                            //     that we push down assignments for these as well.
                            //
                            //     Alternatively, we could only do this for await sites if we keep track
                            //     of assignment sites introduced by the await rewriter and only consider
                            //     the parent nodes of those sites as requiring percolation.
                            if (res.Type != expr.Type)
                            {
                                res = Expression.Convert(res, expr.Type);
                            }
                            return res;
                        }
                }
            }

            private static bool ShouldPercolate(ExpressionType type)
            {
                var res = false;

                switch (type)
                {
                    // TODO: Loop
                    case ExpressionType.Block:
                    case ExpressionType.Conditional:
                    case ExpressionType.Try:
                    case ExpressionType.Switch:
                        res = true;
                        break;
                }

                return res;
            }
        }
    }
}