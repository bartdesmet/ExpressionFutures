// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a do...while loop.
    /// </summary>
    public sealed partial class DoCSharpStatement : ConditionalLoopCSharpStatement
    {
        internal DoCSharpStatement(Expression body, Expression test, LabelTarget? breakLabel, LabelTarget? continueLabel, ReadOnlyCollection<ParameterExpression> locals)
            : base(test, body, breakLabel, continueLabel, locals)
        {
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Do;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitDo(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="breakLabel">The <see cref="LoopCSharpStatement.BreakLabel" /> property of the result.</param>
        /// <param name="continueLabel">The <see cref="LoopCSharpStatement.ContinueLabel" /> property of the result.</param>
        /// <param name="body">The <see cref="LoopCSharpStatement.Body" /> property of the result.</param>
        /// <param name="test">The <see cref="ConditionalLoopCSharpStatement.Test" /> property of the result.</param>
        /// <param name="locals">The <see cref="ConditionalLoopCSharpStatement.Locals" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public DoCSharpStatement Update(LabelTarget? breakLabel, LabelTarget? continueLabel, Expression body, Expression test, IEnumerable<ParameterExpression> locals)
        {
            if (breakLabel == BreakLabel && continueLabel == ContinueLabel && body == Body && test == Test && SameElements(ref locals, this.Locals))
            {
                return this;
            }

            return CSharpExpression.Do(body, test, breakLabel, continueLabel, locals);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            var n = 1 /* begin */ + 1 /* Body */ + 1 /* if */;

            if (BreakLabel != null)
            {
                n++;
            }

            if (ContinueLabel != null)
            {
                n++;
            }

            var exprs = new Expression[n];

            var i = 0;

            var begin = Expression.Label("__begin");

            exprs[i++] =
                Expression.Label(begin);
            exprs[i++] =
                Body;

            if (ContinueLabel != null)
            {
                exprs[i++] =
                    Expression.Label(ContinueLabel);
            }

            exprs[i++] =
                Expression.IfThen(
                    Test!, // NB: Inherited from base but guaranteed non-null for Do.
                    Expression.Goto(begin)
                );

            if (BreakLabel != null)
            {
                exprs[i++] =
                    Expression.Label(BreakLabel);
            }

            // NB: We consider all variables to be in scope of the condition and body of the loop, even though
            //     in C# e.g. `out` variables in the condition are not usable in the body.

            var loop =
                Expression.Block(
                    typeof(void),
                    Locals,
                    exprs
                );

            return loop;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="DoCSharpStatement"/> that represents a do...while loop.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <returns>The created <see cref="DoCSharpStatement"/>.</returns>
        public static DoCSharpStatement Do(Expression body, Expression test) => Do(body, test, @break: null, @continue: null, locals: null);

        /// <summary>
        /// Creates a <see cref="DoCSharpStatement"/> that represents a do...while loop.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <returns>The created <see cref="DoCSharpStatement"/>.</returns>
        public static DoCSharpStatement Do(Expression body, Expression test, LabelTarget? @break) => Do(body, test, @break, @continue: null, locals: null);

        /// <summary>
        /// Creates a <see cref="DoCSharpStatement"/> that represents a do...while loop.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <returns>The created <see cref="DoCSharpStatement"/>.</returns>
        public static DoCSharpStatement Do(Expression body, Expression test, LabelTarget? @break, LabelTarget? @continue) => Do(body, test, @break, @continue, locals: null);

        /// <summary>
        /// Creates a <see cref="DoCSharpStatement"/> that represents a do...while loop.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <param name="locals">The variables that are in scope of the loop.</param>
        /// <returns>The created <see cref="DoCSharpStatement"/>.</returns>
        public static DoCSharpStatement Do(Expression body, Expression test, LabelTarget? @break, LabelTarget? @continue, IEnumerable<ParameterExpression>? locals)
        {
            var localsList = CheckUniqueVariables(locals, nameof(locals)); 

            ValidateLoop(test, body, @break, @continue);

            return new DoCSharpStatement(body, test, @break, @continue, localsList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="DoCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDo(DoCSharpStatement node) =>
            node.Update(
                VisitLabelTarget(node.BreakLabel),
                VisitLabelTarget(node.ContinueLabel),
                Visit(node.Body),
                Visit(node.Test!), // NB: Inherited from base but guaranteed non-null for Do.
                VisitAndConvert(node.Locals, nameof(VisitDo))
            );
    }
}
