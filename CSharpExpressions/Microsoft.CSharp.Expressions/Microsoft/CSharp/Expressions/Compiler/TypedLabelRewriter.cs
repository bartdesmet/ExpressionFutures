// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Typed labels can cause issues for async method rewrites. This rewriter gets rid of them by storing
    /// their result in variables and using void-typed labels instead.
    /// </summary>
    /// <remarks>
    /// An example of a rewrite that's problematic is this:
    /// <code>
    ///   var label = Expression.Label(typeof(int));
    ///   var expr =
    ///     Expression.Block(
    ///       Expression.Return(label, Expression.Constant(42)),
    ///       Expression.Label(label, Expression.Constant(0))
    ///     );
    /// </code>
    /// The problem pops up when performing assignment percolation when we push down assignments in the
    /// outer try block generated for async methods. This causes the last statement in the block to turn
    /// into the rhs of an assignment, which is invalid for labels.
    /// </remarks>
    internal class TypedLabelRewriter : ShallowVisitor
    {
        // NB: Only have to deal with jumps to labels defined in outer blocks; cf. LabelInfo.ValidateJump.
        // TODO: Check cases where we have shadowing, e.g. with Loop.

        private readonly Stack<IDictionary<LabelTarget, LabelInfo>> _labels = new Stack<IDictionary<LabelTarget, LabelInfo>>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class doesn't pass null.")]
        protected override Expression VisitBlock(BlockExpression node)
        {
            var map = default(IDictionary<LabelTarget, LabelInfo>);

            foreach (var e in node.Expressions)
            {
                if (e.NodeType == ExpressionType.Label)
                {
                    var l = (LabelExpression)e;
                    if (l.Type != typeof(void))
                    {
                        map ??= new Dictionary<LabelTarget, LabelInfo>();

                        var name = l.Target.Name;
                        var newLabel = Expression.Label(typeof(void), name);
                        var labelValue = Expression.Parameter(l.Type, name);

                        var info = new LabelInfo
                        {
                            Target = newLabel,
                            Value = labelValue,
                        };

                        map.Add(l.Target, info);
                    }
                }
            }

            if (map != null)
            {
                _labels.Push(map);
            }

            var res = (BlockExpression)base.VisitBlock(node);

            if (map != null)
            {
                _labels.Pop();

                var newVariables = map.Values.Select(i => i.Value);
                res = res.Update(res.Variables.Concat(newVariables), res.Expressions);
            }

            return res;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class doesn't pass null.")]
        protected override Expression VisitLabel(LabelExpression node)
        {
            if (TryGetLabelInfo(node.Target, out LabelInfo info))
            {
                Debug.Assert(node.DefaultValue != null, "Expected non-void label to have default value.");

                var variable = info.Value;
                var newTarget = info.Target;

                var res =
                    Expression.Block(
                        Expression.Assign(variable, node.DefaultValue),
                        Expression.Label(newTarget),
                        variable
                    );

                return res;
            }

            return base.VisitLabel(node);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class doesn't pass null.")]
        protected override Expression VisitGoto(GotoExpression node)
        {
            if (TryGetLabelInfo(node.Target, out LabelInfo info))
            {
                Debug.Assert(node.Value != null, "Expected non-void goto to have value.");

                var variable = info.Value;
                var newTarget = info.Target;

                var res =
                    Expression.Block(
                        Expression.Assign(variable, node.Value),
                        Expression.Goto(newTarget, node.Type)
                    );

                return res;
            }

            return base.VisitGoto(node);
        }

        private bool TryGetLabelInfo(LabelTarget target, out LabelInfo info)
        {
            if (target.Type != typeof(void))
            {
                foreach (var scope in _labels)
                {
                    if (scope.TryGetValue(target, out info))
                    {
                        return true;
                    }
                }
            }

            info = default;
            return false;
        }

        struct LabelInfo
        {
            public LabelTarget Target;
            public ParameterExpression Value;
        }
    }
}
