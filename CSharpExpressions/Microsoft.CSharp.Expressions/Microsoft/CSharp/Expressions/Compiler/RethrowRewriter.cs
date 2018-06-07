// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to rewrite rethrow statements.
    /// </summary>
    internal static class RethrowRewriter
    {
        public static Expression Rewrite(Expression expression, Expression throwReplacement) => new Impl(throwReplacement).Visit(expression);

        private sealed class Impl : ShallowVisitor
        {
            private readonly Expression _replacement;

            public Impl(Expression replacement)
            {
                _replacement = replacement;
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                return node; // NB: In nested catch blocks, the meaning of a rethrow changes.
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class doesn't pass null.")]
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
}
