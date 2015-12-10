// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to rewrite catch handlers that contain asynchronous operations.
    /// </summary>
    internal class CatchRewriter : AwaitTrackingVisitor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class doesn't pass null.")]
        protected override Expression VisitTry(TryExpression node)
        {
            var res = default(Expression);

            var handlers = node.Handlers;
            var n = handlers.Count;

            if (n > 0)
            {
                res = Visit(node.Body);

                var newHandlers = new List<CatchBlock>();

                for (var i = 0; i < n; i++)
                {
                    var handler = handlers[i];

                    var newBody = default(Expression);

                    if (VisitAndFindAwait(handler.Body, out newBody))
                    {
                        if (newHandlers.Count > 0)
                        {
                            res = Expression.TryCatch(res, newHandlers.ToArray());
                            newHandlers.Clear();
                        }

                        var catchExceptionVariable = Expression.Parameter(handler.Test, "__caughtException");
                        var exceptionVariable = handler.Variable ?? Expression.Parameter(handler.Test, "__exception");

                        var handlerBody =
                            Expression.Block(
                                Expression.Assign(exceptionVariable, catchExceptionVariable),
                                Expression.Default(handler.Body.Type)
                            );

                        var newFilter = default(Expression);

                        if (handler.Filter != null)
                        {
                            newFilter = ParameterSubstitutor.Substitute(handler.Filter, exceptionVariable, catchExceptionVariable);
                        }

                        var rethrow = Utils.CreateRethrow(exceptionVariable);
                        newBody = RethrowRewriter.Rewrite(newBody, rethrow);

                        var newHandler = handler.Update(catchExceptionVariable, newFilter, handlerBody);
                        var newTry = Expression.TryCatch(res, newHandler);

                        if (newTry.Type != typeof(void))
                        {
                            var tryResult = Expression.Parameter(newTry.Type, "__tryValue");

                            res =
                                Expression.Block(
                                    new[] { tryResult, exceptionVariable },
                                    Expression.Assign(tryResult, newTry),
                                    Expression.Condition(
                                        Expression.NotEqual(exceptionVariable, Expression.Default(exceptionVariable.Type)),
                                        newBody,
                                        tryResult
                                    )
                                );
                        }
                        else
                        {
                            res =
                                Expression.Block(
                                    new[] { exceptionVariable },
                                    newTry,
                                    Expression.IfThen(
                                        Expression.NotEqual(exceptionVariable, Expression.Default(exceptionVariable.Type)),
                                        newBody
                                    )
                                );
                        }

                        // TODO: pend/unpend branches out of the handler body
                    }
                    else
                    {
                        newHandlers.Add(handler.Update(handler.Variable, handler.Filter, newBody)); // NB: filters with await are rejected by the Checker
                    }
                }

                if (newHandlers.Count > 0)
                {
                    res = Expression.TryCatch(res, newHandlers.ToArray());
                }

                if (node.Finally != null)
                {
                    res = Expression.TryFinally(res, Visit(node.Finally));
                }

                Debug.Assert(node.Fault == null); // NB: Factories in LINQ prevent the combo of handlers with fault
            }
            else
            {
                res = base.VisitTry(node);
            }

            return res;
        }
    }
}
