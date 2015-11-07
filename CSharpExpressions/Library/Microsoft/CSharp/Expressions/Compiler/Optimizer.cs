// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    class Optimizer : CSharpExpressionVisitor
    {
        private static ExpressionVisitor s_optimizer = new Optimizer();

        private Optimizer()
        {
        }

        public static Expression Optimize(Expression expression)
        {
            return s_optimizer.Visit(expression);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            var res = (BlockExpression)base.VisitBlock(node);

            if (CanOptimize(res))
            {
                var expressions = default(List<Expression>);

                for (var i = 0; i < res.Expressions.Count; i++)
                {
                    var expression = res.Expressions[i];
                    var nested = expression as BlockExpression;
                    if (nested != null && CanOptimize(nested))
                    {
                        if (expressions == null)
                        {
                            expressions = new List<Expression>();

                            for (var j = 0; j < i; j++)
                            {
                                expressions.Add(res.Expressions[j]);
                            }
                        }

                        expressions.AddRange(nested.Expressions);
                    }
                    else if (expressions != null)
                    {
                        expressions.Add(expression);
                    }
                }

                if (expressions != null)
                {
                    return res.Update(res.Variables, expressions);
                }
            }

            return res;
        }

        private static bool CanOptimize(BlockExpression block)
        {
            // TODO: some more optimizations are possible even if the types don't match,
            //       e.g. flattening all but the last child block into the parent block.

            return block.Variables.Count == 0 && block.Result.Type == block.Type;
        }
    }
}
