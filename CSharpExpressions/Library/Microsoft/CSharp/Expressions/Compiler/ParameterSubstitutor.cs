// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to substitute variables while keeping shadowing rules in mind.
    /// This is used to rewrite filters when we change the variable of the catch block.
    /// </summary>
    internal static class ParameterSubstitutor
    {
        public static Expression Substitute(Expression expression, ParameterExpression original, ParameterExpression replacement)
        {
            return Substitute(expression, new Dictionary<ParameterExpression, ParameterExpression> { { original, replacement } });
        }

        public static Expression Substitute(Expression expression, IDictionary<ParameterExpression, ParameterExpression> substitutions)
        {
            return new Impl(substitutions).Visit(expression);
        }

        class Impl : ParameterSubstitutionVisitor
        {
            private readonly IDictionary<ParameterExpression, ParameterExpression> _substitutions;
            private readonly Stack<HashSet<ParameterExpression>> _env = new Stack<HashSet<ParameterExpression>>();

            public Impl(IDictionary<ParameterExpression, ParameterExpression> substitutions)
            {
                _substitutions = substitutions;
            }

            protected override void Push(IEnumerable<ParameterExpression> variables)
            {
                _env.Push(new HashSet<ParameterExpression>(variables));
            }

            protected override void Pop()
            {
                _env.Pop();
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                var replacement = default(ParameterExpression);
                if (_substitutions.TryGetValue(node, out replacement))
                {
                    var found = false;

                    foreach (var env in _env)
                    {
                        if (env.Contains(node))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        return replacement;
                    }
                }

                return node;
            }
        }
    }
}