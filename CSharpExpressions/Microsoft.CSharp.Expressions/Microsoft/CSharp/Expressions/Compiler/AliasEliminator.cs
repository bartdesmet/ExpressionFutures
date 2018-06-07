// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to eliminate aliased variables by ensuring usage of unique ParameterExpression nodes.
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
    internal static class AliasEliminator
    {
        public static Expression Eliminate(Expression expression) => new Impl().Visit(expression);

        private sealed class Impl : ParameterSubstitutionVisitor
        {
            private readonly HashSet<ParameterExpression> _env = new HashSet<ParameterExpression>();

            protected override void Push(IEnumerable<ParameterExpression> variables)
            {
                var subst = new Dictionary<ParameterExpression, ParameterExpression>();

                foreach (var var in variables)
                {
                    if (!_env.Add(var))
                    {
                        subst[var] = Expression.Parameter(var.Type, var.Name);
                    }
                }

                _subst.Push(subst);
            }

            protected override void Pop() => _subst.Pop();
        }
    }
}