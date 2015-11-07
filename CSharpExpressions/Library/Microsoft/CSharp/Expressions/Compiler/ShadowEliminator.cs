// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    internal static class ShadowEliminator
    {
        public static Expression Eliminate(Expression expression)
        {
            return new Impl().Visit(expression);
        }

        class Impl : CSharpExpressionVisitor
        {
            private readonly Stack<HashSet<ParameterExpression>> _env = new Stack<HashSet<ParameterExpression>>();
            private readonly Stack<IDictionary<ParameterExpression, ParameterExpression>> _subst = new Stack<IDictionary<ParameterExpression, ParameterExpression>>();

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected override Expression VisitBlock(BlockExpression node)
            {
                Push(node.Variables);

                var res = base.VisitBlock(node);

                Pop();

                return res;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                Push(node.Variable != null ? new[] { node.Variable } : Array.Empty<ParameterExpression>());

                var res = base.VisitCatchBlock(node);

                Pop();

                return res;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                Push(node.Parameters);

                var res = base.VisitLambda<T>(node);

                Pop();

                return res;
            }

            // NB: Strictly speaking, we don't need to handle C# nodes here (other than keeping Await unreduced),
            //     because the shadow eliminator runs after the reducer. However, we keep those here for general
            //     utility and also to deal with the case where we may reshuffle rewrite steps. We could #if them
            //     out if we want to reduce code size.

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected internal override Expression VisitUsing(UsingCSharpStatement node)
            {
                Push(node.Variable != null ? new[] { node.Variable } : Array.Empty<ParameterExpression>());

                var res = base.VisitUsing(node);

                Pop();

                return res;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected internal override Expression VisitForEach(ForEachCSharpStatement node)
            {
                Push(new[] { node.Variable });

                var res = base.VisitForEach(node);

                Pop();

                return res;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected internal override Expression VisitFor(ForCSharpStatement node)
            {
                var n = node.Initializers.Count;

                var variables = new ParameterExpression[n];

                for (var i = 0; i < n; i++)
                {
                    variables[i] = (ParameterExpression)node.Initializers[i].Left;
                }

                Push(variables);

                var res = base.VisitFor(node);

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
                            subst[p] = Expression.Parameter(p.Type, p.Name);
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
}