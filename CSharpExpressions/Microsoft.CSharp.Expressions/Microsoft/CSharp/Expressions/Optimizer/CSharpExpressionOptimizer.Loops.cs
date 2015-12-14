// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.CSharp.Expressions.Compiler
{
    partial class CSharpExpressionOptimizer
    {
        // NB: This optimization eliminates break and continue labels in loops if they're
        //     not referenced in the loop body. Note we only scan child nodes (generally, we
        //     don't have parent context around, especially if it's a LINQ expression) so a 
        //     weird construction by the user to attempt to jump to a break or continue label
        //     for a loop from a parent or sibling node will still cause the label to get
        //     eliminated if it's not referenced from the loop body. This is fine given that
        //     a) the behavior of such a construction is undefined to begin with (it could
        //     bypass initialization logic of loops), and b) the lambda compiler's branch
        //     analysis is likely to detect it as an invalid jump, causing compilation to bail
        //     out. Note that we can't protect against this in factory methods for loops for
        //     the same reason as the partial analysis here: we don't have references to
        //     parent nodes to check for references to the specified labels. Worst case, the
        //     exception thrown from the Compile method changes from "invalid branch" to "jump
        //     to undefined label" because we took away the label definition from the loop.

        private readonly Stack<LoopLabelInfo> _loopLabels = new Stack<LoopLabelInfo>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitWhile(WhileCSharpStatement node)
        {
            var test = Visit(node.Test);

            PushLabelInfo(node);

            var body = Visit(node.Body);

            var @break = default(LabelTarget);
            var @continue = default(LabelTarget);
            PopLabelInfo(out @break, out @continue);

            return node.Update(@break, @continue, test, body);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitDo(DoCSharpStatement node)
        {
            PushLabelInfo(node);

            var body = Visit(node.Body);

            var @break = default(LabelTarget);
            var @continue = default(LabelTarget);
            PopLabelInfo(out @break, out @continue);

            var test = Visit(node.Test);

            return node.Update(@break, @continue, test, body);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitFor(ForCSharpStatement node)
        {
            // NB: If we do optimizations involving variables, we'll need to track scopes here.

            var variables = VisitAndConvert(node.Variables, nameof(VisitFor));
            var initializers = Visit(node.Initializers);
            var test = Visit(node.Test);
            var iterators = OptimizeIterators(Visit(node.Iterators));

            PushLabelInfo(node);

            var body = Visit(node.Body);

            var @break = default(LabelTarget);
            var @continue = default(LabelTarget);
            PopLabelInfo(out @break, out @continue);

            return node.Update(@break, @continue, variables, initializers, test, iterators, body);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitForEach(ForEachCSharpStatement node)
        {
            // NB: If we do optimizations involving variables, we'll need to track scopes here.

            var variable = VisitAndConvert(node.Variable, nameof(VisitForEach));
            var collection = Visit(node.Collection);
            var conversion = VisitAndConvert(node.Conversion, nameof(VisitForEach));

            PushLabelInfo(node);

            var body = Visit(node.Body);

            var @break = default(LabelTarget);
            var @continue = default(LabelTarget);
            PopLabelInfo(out @break, out @continue);

            return node.Update(@break, @continue, variable, collection, conversion, body);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected override Expression VisitLoop(LoopExpression node)
        {
            PushLabelInfo(node);

            var body = Visit(node.Body);

            var @break = default(LabelTarget);
            var @continue = default(LabelTarget);
            PopLabelInfo(out @break, out @continue);

            return node.Update(@break, @continue, body);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected override Expression VisitGoto(GotoExpression node)
        {
            // NB: We ignore classifying by Kind to distinguish Break from Continue;
            //     after all, the user can cheat about the goto kind and still achieve
            //     the branching to take place.

            MarkLoopLabel(node.Target);

            return base.VisitGoto(node);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitGotoLabel(GotoLabelCSharpStatement node)
        {
            MarkLoopLabel(node.Target);

            return base.VisitGotoLabel(node);
        }

        private void MarkLoopLabel(LabelTarget target)
        {
            // NB: We don't honor the scoping of loops to do a more shallow scan for references
            //     to the labels (i.e. nested scopes change the meaning of "break" and "continue"
            //     in C#). We don't because the library allows break and continue statements that
            //     target a loop other than the immediately enclosing one. We could attempt to
            //     reject such constructs during the Reduce phase (or even from the loop factory
            //     methods if we can afford a scan of child nodes to detect such constructions,
            //     which *are* allowed in the DLR btw). In that case, we could restrict this scan
            //     to the immediately enclosing loop. It seems there's no harm in allowing to
            //     break from a loop higher up, so we don't attempt to reject it currently.

            foreach (var labelInfo in _loopLabels)
            {
                if (labelInfo.Break == target)
                {
                    labelInfo.BreakHasReference = true;
                    break;
                }

                if (labelInfo.Continue == target)
                {
                    labelInfo.ContinueHasReference = true;
                    break;
                }
            }
        }

        private static IList<Expression> OptimizeIterators(IList<Expression> iterators)
        {
            // NB: This optimizes commonly used unary postfix assignment operations which would
            //     reduce to blocks with extra temporaries at a later stage of compilation, e.g.
            //     during stack spilling both in our API (for async) and in LINQ.
            //
            //     We could invest in a proper dead variable eliminator deep inside the lambda
            //     compiler, but until we do so we'll optimize this common pattern. We choose to
            //     be very narrow and only optimize typical index counting patterns for loops,
            //     such as i++ and --i where i is a variable of type Int32. It'd be safe for
            //     more cases, but those are very rare.

            var res = default(IList<Expression>);

            var n = iterators.Count;

            for (var i = 0; i < n; i++ /* an example of what we optimize for here */)
            {
                var iterator = iterators[i];
                var rewritten = OptimizeIterator(iterator);

                if (iterator != rewritten && res == null)
                {
                    res = Clone(iterators, i);
                }
                
                if (res != null)
                {
                    res.Add(rewritten);
                }
            }

            if (res != null)
            {
                return res;
            }

            return iterators;
        }

        private static Expression OptimizeIterator(Expression expression)
        {
            // NB: Just being narrow and checking for the typical loop case. We could add other
            //     type and checked variants of the unary assignments as well.

            if (expression.Type == typeof(int))
            {
                var unary = expression as UnaryExpression;
                if (unary != null)
                {
                    switch (unary.NodeType)
                    {
                        case ExpressionType.PostIncrementAssign:
                            return OptimizeIterator(expression, Expression.PreIncrementAssign, unary.Operand, unary.Method);
                        case ExpressionType.PostDecrementAssign:
                            return OptimizeIterator(expression, Expression.PreDecrementAssign, unary.Operand, unary.Method);
                    }
                }
                else
                {
                    var csassign = expression as AssignUnaryCSharpExpression;
                    if (csassign != null)
                    {
                        switch (csassign.CSharpNodeType)
                        {
                            case CSharpExpressionType.PostIncrementAssign:
                                return OptimizeIterator(expression, CSharpExpression.PreIncrementAssign, csassign.Operand, csassign.Method);
                            case CSharpExpressionType.PostDecrementAssign:
                                return OptimizeIterator(expression, CSharpExpression.PreDecrementAssign, csassign.Operand, csassign.Method);
                        }
                    }
                }
            }

            return expression;
        }

        private static Expression OptimizeIterator(Expression expression, Func<Expression, MethodInfo, Expression> factory, Expression operand, MethodInfo method)
        {
            if (operand.NodeType == ExpressionType.Parameter)
            {
                // NB: Method should be null with our current narrow choice, but passing it here
                //     in case we decide to apply this optimization in more cases later.

                return factory(operand, method);
            }

            return expression;
        }

        private void PushLabelInfo(LoopExpression loop)
        {
            _loopLabels.Push(new LoopLabelInfo { Break = loop.BreakLabel, Continue = loop.ContinueLabel });
        }

        private void PushLabelInfo(LoopCSharpStatement loop)
        {
            _loopLabels.Push(new LoopLabelInfo { Break = loop.BreakLabel, Continue = loop.ContinueLabel });
        }

        private void PopLabelInfo(out LabelTarget @break, out LabelTarget @continue)
        {
            var labelInfo = _loopLabels.Pop();

            @break = labelInfo.BreakHasReference ? labelInfo.Break : null;
            @continue = labelInfo.ContinueHasReference ? labelInfo.Continue : null;
        }

        class LoopLabelInfo
        {
            public LabelTarget Break;
            public bool BreakHasReference;

            public LabelTarget Continue;
            public bool ContinueHasReference;
        }
    }
}
