// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    internal static class GotoRewriter
    {
        public static Expression Rewrite(Expression expression, out LabelTarget exitLabel, out ParameterExpression pendingBranch, out IDictionary<LabelTarget, LeaveLabelData> leaveLabels)
        {
            pendingBranch = Expression.Parameter(typeof(int));
            exitLabel = Expression.Label("__leave");

            var labelScanner = new LabelScanner();
            labelScanner.Visit(expression);

            var gotoScanner = new GotoScanner(labelScanner.Labels, exitLabel, pendingBranch);

            var res = gotoScanner.Visit(expression);

            leaveLabels = gotoScanner.LeaveLabels;
            return res;
        }

        class LabelScanner : ShallowVisitor
        {
            public readonly HashSet<LabelTarget> Labels = new HashSet<LabelTarget>();

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class doesn't pass null.")]
            protected override Expression VisitLabel(LabelExpression node)
            {
                Labels.Add(node.Target);

                return base.VisitLabel(node);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class doesn't pass null.")]
            protected override Expression VisitLoop(LoopExpression node)
            {
                if (node.BreakLabel != null)
                {
                    Labels.Add(node.BreakLabel);
                }

                if (node.ContinueLabel != null)
                {
                    Labels.Add(node.ContinueLabel);
                }

                return base.VisitLoop(node);
            }
        }

        class GotoScanner : ShallowVisitor
        {
            private readonly HashSet<LabelTarget> _labels;
            private readonly LabelTarget _exit;
            private readonly ParameterExpression _pendingBranch;
            public readonly IDictionary<LabelTarget, LeaveLabelData> LeaveLabels = new Dictionary<LabelTarget, LeaveLabelData>();

            public GotoScanner(HashSet<LabelTarget> labels, LabelTarget exit, ParameterExpression pendingBranch)
            {
                _labels = labels;
                _exit = exit;
                _pendingBranch = pendingBranch;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class doesn't pass null.")]
            protected override Expression VisitGoto(GotoExpression node)
            {
                var target = node.Target;

                if (!_labels.Contains(target))
                {
                    var data = default(LeaveLabelData);
                    if (!LeaveLabels.TryGetValue(target, out data))
                    {
                        var parameter = default(ParameterExpression);

                        if (target.Type != typeof(void))
                        {
                            parameter = Expression.Parameter(target.Type);
                        }

                        data = new LeaveLabelData
                        {
                            Index = LeaveLabels.Count + 1,
                            Target = target,
                            Value = parameter
                        };

                        LeaveLabels.Add(target, data);
                    }

                    var res = default(Expression);

                    if (data.Value != null)
                    {
                        res =
                            Expression.Block(
                                Expression.Assign(_pendingBranch, Helpers.CreateConstantInt32(data.Index)),
                                Expression.Assign(data.Value, node.Value),
                                Expression.Goto(_exit, node.Type)
                            );
                    }
                    else
                    {
                        res =
                            Expression.Block(
                                Expression.Assign(_pendingBranch, Helpers.CreateConstantInt32(data.Index)),
                                Expression.Goto(_exit, node.Type)
                            );
                    }

                    return res;
                }

                return base.VisitGoto(node);
            }
        }
    }
}
