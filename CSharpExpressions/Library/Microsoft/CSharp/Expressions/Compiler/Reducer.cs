// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to reduce all extension nodes, except for await, into more primitive forms.
    /// This step is performed early on during async lambda rewriting so we don't have to worry about custom nodes.
    /// </summary>
    internal static class Reducer
    {
        public static Expression Reduce(Expression expression)
        {
            return Impl.Instance.Visit(expression);
        }

        class Impl : BetterExpressionVisitor
        {
            public static readonly ExpressionVisitor Instance = new Impl();

            protected override Expression VisitExtension(Expression node)
            {
                var csharp = node as CSharpExpression;
                if (csharp != null && csharp.CSharpNodeType == CSharpExpressionType.Await)
                {
                    var await = (AwaitCSharpExpression)csharp;
                    return await.Update(Visit(await.Operand));
                }

                return base.VisitExtension(node);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                // NB: This reduces assignment operators so that the stack spiller doesn't have to worry about it.
                if (node.CanReduce)
                {
                    return node.Reduce();
                }

                return base.VisitBinary(node);
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                // NB: This reduces assignment operators so that the stack spiller doesn't have to worry about it.
                if (node.CanReduce)
                {
                    return node.Reduce();
                }

                return base.VisitUnary(node);
            }
        }
    }
}
