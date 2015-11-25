// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Base class for visitors that track variable scopes.
    /// </summary>
    internal abstract class ScopeTrackingVisitor : CSharpExpressionVisitor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected override Expression VisitBlock(BlockExpression node)
        {
            Push(node.Variables);

            var res = base.VisitBlock(node);

            Pop();

            return res;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            Push(node.Variable != null ? new[] { node.Variable } : Array.Empty<ParameterExpression>());

            var res = base.VisitCatchBlock(node);

            Pop();

            return res;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Push(node.Parameters);

            var res = base.VisitLambda<T>(node);

            Pop();

            return res;
        }

        // NB: Strictly speaking, we don't need to handle C# nodes here (other than keeping Await unreduced),
        //     because the shadow eliminator runs after the reducer. However, we keep those here for general
        //     utility and also to deal with the case where we may reshuffle rewrite steps. We could #if them
        //     out if we want to reduce code size.

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitBlock(BlockCSharpExpression node)
        {
            Push(node.Variables);

            var res = base.VisitBlock(node);

            Pop();

            return res;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitUsing(UsingCSharpStatement node)
        {
            Push(node.Variable != null ? new[] { node.Variable } : Array.Empty<ParameterExpression>());

            var res = base.VisitUsing(node);

            Pop();

            return res;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitForEach(ForEachCSharpStatement node)
        {
            Push(new[] { node.Variable });

            var res = base.VisitForEach(node);

            Pop();

            return res;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitFor(ForCSharpStatement node)
        {
            Push(node.Variables);

            var res = base.VisitFor(node);

            Pop();

            return res;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitSwitch(SwitchCSharpStatement node)
        {
            var switchValue = Visit(node.SwitchValue);

            Push(node.Variables);

            var res = node.Update(switchValue, VisitLabelTarget(node.BreakLabel), VisitAndConvert(node.Variables, nameof(VisitSwitch)), Visit(node.Cases, VisitSwitchCase));

            Pop();

            return res;
        }

        protected abstract override Expression VisitParameter(ParameterExpression node);

        protected abstract void Push(IEnumerable<ParameterExpression> variables);

        protected abstract void Pop();
    }
}
