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
    /// Represents a while loop.
    /// </summary>
    public sealed partial class WhileCSharpStatement : ConditionalLoopCSharpStatement
    {
        internal WhileCSharpStatement(Expression test, Expression body, LabelTarget? breakLabel, LabelTarget? continueLabel, ReadOnlyCollection<ParameterExpression> locals)
            : base(test, body, breakLabel, continueLabel, locals)
        {
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.While;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitWhile(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="breakLabel">The <see cref="LoopCSharpStatement.BreakLabel" /> property of the result.</param>
        /// <param name="continueLabel">The <see cref="LoopCSharpStatement.ContinueLabel" /> property of the result.</param>
        /// <param name="test">The <see cref="ConditionalLoopCSharpStatement.Test" /> property of the result.</param>
        /// <param name="body">The <see cref="LoopCSharpStatement.Body" /> property of the result.</param>
        /// <param name="locals">The <see cref="ConditionalLoopCSharpStatement.Locals" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public WhileCSharpStatement Update(LabelTarget? breakLabel, LabelTarget? continueLabel, Expression test, Expression body, IEnumerable<ParameterExpression> locals)
        {
            if (breakLabel == BreakLabel && continueLabel == ContinueLabel && test == Test && body == Body && SameElements(ref locals, Locals))
            {
                return this;
            }

            return CSharpExpression.While(test, body, breakLabel, continueLabel, locals);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            var @break = BreakLabel ?? Expression.Label("__break");
            var @continue = ContinueLabel;

            var loop =
                Expression.Loop(
                    Expression.Block(
                        Expression.IfThen(
                            Expression.Not(Test),
                            Expression.Break(@break)
                        ),
                        Body
                    ),
                    @break,
                    @continue
                );

            if (Locals.Count > 0)
            {
                return
                    Expression.Block(
                        typeof(void),
                        Locals,
                        loop
                    );
            }

            return loop;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="WhileCSharpStatement"/> that represents a while loop.
        /// </summary>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="body">The body of the loop.</param>
        /// <returns>The created <see cref="WhileCSharpStatement"/>.</returns>
        public static WhileCSharpStatement While(Expression test, Expression body) => While(test, body, @break: null, @continue: null, locals: null);

        /// <summary>
        /// Creates a <see cref="WhileCSharpStatement"/> that represents a while loop.
        /// </summary>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <returns>The created <see cref="WhileCSharpStatement"/>.</returns>
        public static WhileCSharpStatement While(Expression test, Expression body, LabelTarget? @break) => While(test, body, @break, @continue: null, locals: null);

        /// <summary>
        /// Creates a <see cref="WhileCSharpStatement"/> that represents a while loop.
        /// </summary>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <returns>The created <see cref="WhileCSharpStatement"/>.</returns>
        public static WhileCSharpStatement While(Expression test, Expression body, LabelTarget? @break, LabelTarget? @continue) => While(test, body, @break, @continue, locals: null);

        /// <summary>
        /// Creates a <see cref="WhileCSharpStatement"/> that represents a while loop.
        /// </summary>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <param name="locals">The variables that are in scope of the loop.</param>
        /// <returns>The created <see cref="WhileCSharpStatement"/>.</returns>
        public static WhileCSharpStatement While(Expression test, Expression body, LabelTarget? @break, LabelTarget? @continue, IEnumerable<ParameterExpression>? locals)
        {
            var localsList = CheckUniqueVariables(locals, nameof(locals));

            ValidateLoop(test, body, @break, @continue);

            return new WhileCSharpStatement(test, body, @break, @continue, localsList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="WhileCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitWhile(WhileCSharpStatement node) =>
            node.Update(
                VisitLabelTarget(node.BreakLabel),
                VisitLabelTarget(node.ContinueLabel),
                Visit(node.Test),
                Visit(node.Body),
                VisitAndConvert(node.Locals, nameof(VisitDo))
            );
    }
}
