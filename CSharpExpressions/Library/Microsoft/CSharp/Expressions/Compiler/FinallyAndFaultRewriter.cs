// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to rewrite finally and fault handlers that contain asynchronous operations.
    /// </summary>
    internal class FinallyAndFaultRewriter : AwaitTrackingVisitor
    {
        // NB: C# doesn't have fault handlers, so we should likely reject that in the Checker.
        //
        //     However, the implementation below supports fault handlers, which could come in handy
        //     if a) we ever do support fault handlers, and b) if any language construct needs to
        //     lower to a fault handler (e.g. some cases for iterators come to mind). In case the
        //     latter is ever needed, the order of lowering steps will have to be reconsidered in
        //     the Compile/Reduce methods for AsyncLambda.

        private int _n;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class doesn't pass null.")]
        protected override Expression VisitTry(TryExpression node)
        {
            var res = default(Expression);

            if (node.Finally != null || node.Fault != null)
            {
                var body = Visit(node.Body);
                var handlers = Visit(node.Handlers, VisitCatchBlock);

                if (node.Finally != null)
                {
                    Debug.Assert(node.Fault == null);

                    var @finally = default(Expression);

                    if (VisitAndFindAwait(node.Finally, out @finally))
                    {
                        if (handlers.Count != 0)
                        {
                            body = Expression.TryCatch(body, handlers.ToArray());
                        }

                        res = RewriteHandler(body, @finally, isFault: false);
                    }
                    else
                    {
                        res = node.Update(body, handlers, @finally, null);
                    }
                }
                else
                {
                    Debug.Assert(node.Finally == null);

                    var fault = default(Expression);

                    if (VisitAndFindAwait(node.Fault, out fault))
                    {
                        Debug.Assert(handlers.Count == 0);

                        res = RewriteHandler(body, fault, isFault: true);
                    }
                    else
                    {
                        res = node.Update(body, handlers, null, fault);
                    }
                }
            }
            else
            {
                res = base.VisitTry(node);
            }

            return res;
        }

        private Expression RewriteHandler(Expression body, Expression handler, bool isFault)
        {
            var leaveLabels = default(IDictionary<LabelTarget, LeaveLabelData>);
            var exitLabel = default(LabelTarget);
            var pendingBranch = default(ParameterExpression);
            body = GotoRewriter.Rewrite(body, out exitLabel, out pendingBranch, out leaveLabels);

            var err = Expression.Parameter(typeof(object), "__error" + _n++);
            var ex = Expression.Parameter(typeof(object), "__ex" + _n++);

            var saveException = default(Expression);
            var value = default(ParameterExpression);

            if (body.Type == typeof(void))
            {
                saveException = Expression.Block(typeof(void), Expression.Assign(err, ex));
            }
            else
            {
                value = Expression.Parameter(body.Type, "__result" + _n++);
                body = Expression.Assign(value, body);
                saveException = Expression.Block(Expression.Assign(err, ex), Expression.Default(body.Type));
            }

            // NB: This lowering technique is what the C# compiler applies as well. It's not a 100% semantics-
            //     preserving in combination with exception filters though. In particular, the timing of those
            //     filters will be different compared to the synchronous case:
            //
            //       try { try { T } finally { F } } catch (E) when (X) { C }
            //
            //     Consider the case where T throws an exception of a type assignable to E:
            //
            //     - If F is synchronous, the order will be T, X, F, C
            //     - If F is asynchronous, the order will be T, F, X, C

            var lowered =
                Expression.TryCatch(
                    body,
                    Expression.Catch(ex, saveException)
                );

            var whenFaulted = default(Expression);
            var whenDone = default(Expression);

            if (isFault)
            {
                whenFaulted = handler;
            }
            else
            {
                whenFaulted = Expression.Empty();
                whenDone = handler;
            }

            var rethrow =
                Expression.IfThen(
                    Expression.ReferenceNotEqual(err, Expression.Default(typeof(object))),
                    Utils.CreateRethrow(err, whenFaulted)
                );

            var vars = new List<ParameterExpression> { err };
            var exprs = new List<Expression> { lowered };

            if (leaveLabels.Count > 0)
            {
                exprs.Add(Expression.Label(exitLabel));
            }

            if (whenDone != null)
            {
                exprs.Add(whenDone);
            }

            exprs.Add(rethrow);

            if (leaveLabels.Count > 0)
            {
                vars.Add(pendingBranch);

                var cases = new List<SwitchCase>();

                foreach (var leaveLabel in leaveLabels.Values)
                {
                    var index = Helpers.CreateConstantInt32(leaveLabel.Index);
                    var valueVariable = leaveLabel.Value;
                    var jump = Expression.Goto(leaveLabel.Target, valueVariable);

                    var @case = Expression.SwitchCase(jump, index);
                    cases.Add(@case);

                    if (valueVariable != null)
                    {
                        vars.Add(valueVariable);
                    }
                }

                exprs.Add(Expression.Switch(pendingBranch, cases.ToArray()));
            }

            if (body.Type != typeof(void))
            {
                vars.Add(value);
                exprs.Add(value);
            }

            var res = Expression.Block(vars, exprs);

            return res;
        }
    }
}
