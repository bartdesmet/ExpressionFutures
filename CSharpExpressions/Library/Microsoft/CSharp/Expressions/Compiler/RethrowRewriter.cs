// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to rewrite rethrow statements.
    /// </summary>
    internal class RethrowRewriter : ShallowVisitor
    {
        private readonly Expression _replacement;

        public RethrowRewriter(Expression replacement)
        {
            _replacement = replacement;
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return node; // NB: In nested catch blocks, the meaning of a rethrow changes.
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Throw && node.Operand == null)
            {
                return _replacement;
            }

            return base.VisitUnary(node);
        }
    }
}
