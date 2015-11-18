// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;

namespace System.Linq.Expressions.Compiler
{
    partial class StackSpiller
    {
        private Result RewriteAwaitExpression(Expression expr, Stack stack)
        {
            var node = (AwaitCSharpExpression)expr;

            ChildRewriter cr = new ChildRewriter(this, stack, 1);

            cr.Add(node.Operand);

            // NB: We always spill the stack for await, so unconditionally rewrite.

            var res = cr.Finish(node.Rewrite(cr[0]));

            return new Result(RewriteAction.SpillStack, res.Node);
        }
    }
}
