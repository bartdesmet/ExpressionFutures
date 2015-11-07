// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Base class for C# expression visitors that don't recurse into nested lambdas or async lambdas.
    /// This visitor type is used to avoid rewrites that would span nested lambda boundaries.
    /// </summary>
    internal abstract class ShallowVisitor : CSharpExpressionVisitor
    {
        protected internal override Expression VisitAsyncLambda<T>(AsyncCSharpExpression<T> node)
        {
            // NB: Keep hands off nested lambdas
            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            // NB: Keep hands off nested lambdas
            return node;
        }
    }
}
