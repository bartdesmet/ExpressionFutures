// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    partial class CSharpExpressionOptimizer
    {
        // NB: This optimization takes away empty fault and finally blocks in a way similar to the C#
        //     compiler's optimization pass. Catch handlers are left untouched.

        protected override Expression VisitTry(TryExpression node)
        {
            var res = base.VisitTry(node);

            if (res is TryExpression tryStmt)
            {
                // NB: It's safe to take away empty fault and finally blocks; they don't have side-
                //     effects, don't alter exception propagation, and don't have useful properties.

                var @finally = tryStmt.Finally;
                MakeNullIfEmpty(ref @finally);

                var fault = tryStmt.Fault;
                MakeNullIfEmpty(ref fault);

                // NB: We obviously can't take away empty handlers; that'd cause subsequent handlers
                //     to get considered or the exception to propagate.

                var handlers = tryStmt.Handlers;

                // NB: However, we can take away *all* handlers if we know that the body of the try
                //     statement can't throw under any circumstance, so we check for purity below.
                //
                //     Note that we *can't* take away finally or fault blocks because they can be
                //     used for their runtime guarantees of being non-interrupted, e.g.
                //
                //       try { } finally { /* critical code */ }
                //
                //     This is a common pattern we shall not break by optimization of course.

                var body = tryStmt.Body;
                if (body.IsPure(true))
                {
                    handlers = null;
                }

                // NB: It's possible it all goes away, so we simply return the body in that case, which
                //     can be a non-empty pure expression.

                if ((handlers == null || handlers.Count == 0) && @finally == null && fault == null)
                {
                    return body;
                }

                // NB: As long as any of { handlers, finally, fault } exists, Update is fine to morph
                //     the original expression into a new one.

                return tryStmt.Update(body, handlers, @finally, fault);
            }

            return res;
        }

        private static void MakeNullIfEmpty(ref Expression node)
        {
            if (node != null && node.Type == typeof(void) && node.NodeType == ExpressionType.Default)
            {
                node = null;
            }
        }
    }
}
