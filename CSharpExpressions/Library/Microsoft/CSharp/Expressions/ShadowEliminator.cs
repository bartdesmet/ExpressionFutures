// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    class ShadowEliminator : CSharpExpressionVisitor
    {
        private readonly Stack<HashSet<ParameterExpression>> _env = new Stack<HashSet<ParameterExpression>>();
        private readonly Stack<IDictionary<ParameterExpression, ParameterExpression>> _subst = new Stack<IDictionary<ParameterExpression, ParameterExpression>>();

        protected override Expression VisitBlock(BlockExpression node)
        {
            Push(node.Variables);

            var res = base.VisitBlock(node);

            Pop();

            return res;
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            Push(node.Variable != null ? new[] { node.Variable } : Array.Empty<ParameterExpression>());

            var res = base.VisitCatchBlock(node);

            Pop();

            return res;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Push(node.Parameters);

            var res = base.VisitLambda<T>(node);

            Pop();

            return res;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            foreach (var subst in _subst)
            {
                var res = default(ParameterExpression);
                if (subst.TryGetValue(node, out res))
                {
                    return res;
                }
            }

            return node;
        }

        private void Push(IEnumerable<ParameterExpression> variables)
        {
            var newEnv = new HashSet<ParameterExpression>(variables);
            var subst = new Dictionary<ParameterExpression, ParameterExpression>();

            foreach (var env in _env)
            {
                if (env.Overlaps(newEnv))
                {
                    foreach (var p in newEnv.Intersect(env))
                    {
                        subst[p] = Expression.Parameter(p.Type,p.Name);
                    }
                }
            }

            _env.Push(newEnv);
            _subst.Push(subst);
        }

        private void Pop()
        {
            _env.Pop();
            _subst.Pop();
        }
    }
}
