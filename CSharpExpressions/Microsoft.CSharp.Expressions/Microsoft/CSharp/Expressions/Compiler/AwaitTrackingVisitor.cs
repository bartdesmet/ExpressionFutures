// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Base classes for visitors that track await expressions in order to detect those in various constructs.
    /// This type of visitor is used to detect await in exception handlers etc.
    /// </summary>
    internal abstract class AwaitTrackingVisitor : ShallowVisitor
    {
        private readonly Stack<StrongBox<bool>> _hasAwait = new();

        protected internal override Expression VisitAwait(AwaitCSharpExpression node)
        {
            if (_hasAwait.Count > 0)
            {
                _hasAwait.Peek().Value = true;
            }

            return base.VisitAwait(node);
        }

        protected bool VisitAndFindAwait(Expression expression, out Expression res)
        {
            _hasAwait.Push(new StrongBox<bool>());

            res = Visit(expression);

            var hasAwait = _hasAwait.Pop().Value;

            if (hasAwait && _hasAwait.Count > 0)
            {
                _hasAwait.Peek().Value = true;
            }

            return hasAwait;
        }
    }
}
