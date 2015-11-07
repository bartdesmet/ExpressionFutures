// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to rewrite catch handlers that contain asynchronous operations.
    /// </summary>
    internal class CatchRewriter : AwaitTrackingVisitor
    {
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
                            res = Expression.TryCatch(res, handlers.ToArray());
                            newHandlers.Clear();
                        }

                        // TODO: rewrite handler to allow resume
                        // TODO: pend/unpend branches out of the handler body

                        throw new NotImplementedException();
                    }
                    else
                    {
                        newHandlers.Add(handler.Update(handler.Variable, handler.Filter, newBody)); // NB: filters with await are rejected by the Checker
                    }
                }

                if (newHandlers.Count > 0)
                {
                    res = Expression.TryCatch(res, handlers.ToArray());
                }

                if (node.Finally != null)
                {
                    res = Expression.TryFinally(res, Visit(node.Finally));
                }

                if (node.Fault != null)
                {
                    res = Expression.TryFault(res, Visit(node.Fault));
                }
            }
            else
            {
                res = base.VisitTry(node);
            }

            return res;
        }
    }
}
