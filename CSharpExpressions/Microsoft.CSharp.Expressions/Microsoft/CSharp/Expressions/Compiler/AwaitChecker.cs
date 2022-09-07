﻿// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Linq.Expressions;
using static System.FormattableString;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to check for usage of await in forbidden places such as lock statements and exception filters.
    /// This step runs first during async lambda rewriting in order to raise errors if needed.
    /// </summary>
    internal static class AwaitChecker
    {
        public static void Check(Expression body) => new Impl().Visit(body);

        private sealed class Impl : CSharpExpressionVisitor
        {
            private readonly Stack<string> _forbidden = new Stack<string>();

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                _forbidden.Push(nameof(LambdaExpression));
                {
                    base.VisitLambda(node);
                }
                _forbidden.Pop();

                return node;
            }

            protected internal override Expression VisitAsyncLambda<T>(AsyncCSharpExpression<T> node) => node;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                if (node.Filter != null)
                {
                    Visit(node.Body);

                    _forbidden.Push(nameof(CatchBlock));
                    {
                        Visit(node.Filter);
                    }
                    _forbidden.Pop();

                    return node;
                }

                return base.VisitCatchBlock(node);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected internal override Expression VisitLock(LockCSharpStatement node)
            {
                Visit(node.Expression);

                _forbidden.Push(nameof(LockCSharpStatement));
                {
                    Visit(node.Body);
                }
                _forbidden.Pop();

                return node;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected override SwitchCase VisitSwitchCase(SwitchCase node)
            {
                // NB: This restriction makes stack spilling for switch cases easier. C# doesn't allow
                //     non-constant test values anyway. We could allow it if we lower SwitchExpression
                //     nodes with non-constant test values into Conditional nodes prior to spilling, so
                //     we can keep a lexical evaluation order.

                Visit(node.Body);

                _forbidden.Push(Invariant($"{nameof(SwitchCase)}.{nameof(SwitchCase.TestValues)}"));
                {
                    Visit(node.TestValues);
                }
                _forbidden.Pop();

                return node;
            }

            protected internal override Expression VisitAwait(AwaitCSharpExpression node)
            {
                if (_forbidden.Count > 0)
                {
                    throw Error.AwaitForbiddenHere(_forbidden.Peek());
                }

                return base.VisitAwait(node);
            }
        }
    }
}