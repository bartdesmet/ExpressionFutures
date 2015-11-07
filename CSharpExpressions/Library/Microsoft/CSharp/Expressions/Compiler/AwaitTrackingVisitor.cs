// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.CSharp.Expressions.Compiler
{
    internal abstract class AwaitTracker : ShallowVisitor
    {
        private readonly Stack<StrongBox<bool>> _hasAwait = new Stack<StrongBox<bool>>();

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
