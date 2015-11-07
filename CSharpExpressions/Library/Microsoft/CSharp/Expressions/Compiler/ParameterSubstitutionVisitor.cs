// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Base class for visitors that enable substituting variables while keeping track of scopes.
    /// </summary>
    internal abstract class ParameterSubstitutionVisitor : ScopeTrackingVisitor
    {
        protected readonly Stack<IDictionary<ParameterExpression, ParameterExpression>> _subst = new Stack<IDictionary<ParameterExpression, ParameterExpression>>();

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
    }
}
