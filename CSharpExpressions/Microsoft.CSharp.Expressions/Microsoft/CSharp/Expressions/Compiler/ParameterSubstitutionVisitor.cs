// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Base class for visitors that enable substituting variables while keeping track of scopes.
    /// </summary>
    internal abstract class ParameterSubstitutionVisitor : ScopeTrackingVisitor
    {
        protected readonly Stack<IDictionary<ParameterExpression, Expression>> _subst = new Stack<IDictionary<ParameterExpression, Expression>>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            foreach (var subst in _subst)
            {
                if (subst.TryGetValue(node, out Expression? res))
                {
                    return res;
                }
            }

            return node;
        }
    }
}
