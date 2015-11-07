// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    internal static class AssignmentPercolator
    {
        public static Expression Percolate(Expression expression)
        {
            return Impl.Instance.Visit(expression);
        }

        class Impl : CSharpExpressionVisitor
        {
            public static readonly ExpressionVisitor Instance = new Impl();

            protected override Expression VisitBinary(BinaryExpression node)
            {
                var res = base.VisitBinary(node);

                if (res.NodeType == ExpressionType.Assign)
                {
                    var b = (BinaryExpression)res;
                    if (b.Right.NodeType == ExpressionType.Block && b.Conversion == null)
                    {
                        res = Percolate(b.Left, b.Right);
                    }
                }

                return res;
            }

            private static Expression Percolate(Expression result, Expression expr)
            {
                if (expr.NodeType == ExpressionType.Block)
                {
                    var block = (BlockExpression)expr;
                    var n = block.Expressions.Count;
                    var res = Percolate(result, block.Expressions[n - 1]);
                    return block.Update(block.Variables, block.Expressions.Take(n - 1).Concat(new[] { res }));
                }
                else
                {
                    return Expression.Assign(result, expr);
                }
            }
        }
    }
}