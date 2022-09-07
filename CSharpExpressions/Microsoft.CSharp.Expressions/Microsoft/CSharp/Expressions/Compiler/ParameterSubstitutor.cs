// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to substitute variables while keeping shadowing rules in mind.
    /// This is used to rewrite filters when we change the variable of the catch block.
    /// </summary>
    internal static class ParameterSubstitutor
    {
        [return: NotNullIfNotNull("expression")]
        public static Expression? Substitute(Expression? expression, ParameterExpression original, Expression replacement)
        {
            return Substitute(expression, new Dictionary<ParameterExpression, Expression> { { original, replacement } });
        }

        [return: NotNullIfNotNull("expression")]
        public static Expression? Substitute(Expression? expression, IDictionary<ParameterExpression, Expression> substitutions)
        {
            return new Impl(substitutions).Visit(expression);
        }

        private sealed class Impl : ParameterSubstitutionVisitor
        {
            private readonly IDictionary<ParameterExpression, Expression> _substitutions;
            private readonly Stack<HashSet<ParameterExpression>> _env = new();

            public Impl(IDictionary<ParameterExpression, Expression> substitutions)
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
                if (_substitutions.TryGetValue(node, out Expression? replacement))
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