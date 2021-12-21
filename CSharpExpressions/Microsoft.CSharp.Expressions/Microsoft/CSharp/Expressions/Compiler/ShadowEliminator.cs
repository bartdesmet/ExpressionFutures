// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to eliminate shadowed variables by ensuring usage of unique ParameterExpression nodes.
    /// This simplifies other stages of async lambda rewriting.
    /// </summary>
    /// <remarks>
    /// As an example, consider the use of a single ParameterExpression p0 instance in the following:
    /// <code>
    ///   Lambda(Lambda(p0, p0), p0)
    /// </code>
    /// This lambda expression is semantically equivalent to an expression that uses two unique ParameterExpression
    /// instances, p0 and p1, as follows:
    /// <code>
    ///   Lambda(Lambda(p0, p0), p1)
    /// </code>
    /// In order to perform this type of rewrite, the shadow eliminator keeps track of expressions that introduce
    /// a scopes and rewrites any variable that's shadowed by those nested scopes.
    /// </remarks>
    internal static class ShadowEliminator
    {
        public static Expression Eliminate(Expression expression) => new Impl().Visit(expression);

        private sealed class Impl : ParameterSubstitutionVisitor
        {
            private readonly Stack<HashSet<ParameterExpression>> _env = new Stack<HashSet<ParameterExpression>>();

            protected override void Push(IEnumerable<ParameterExpression> variables)
            {
                var newEnv = new HashSet<ParameterExpression>(variables);
                var subst = new Dictionary<ParameterExpression, Expression>();

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

            protected override void Pop()
            {
                _env.Pop();
                _subst.Pop();
            }
        }
    }
}
