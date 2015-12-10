// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a do...while loop.
    /// </summary>
    public sealed partial class DoCSharpStatement : ConditionalLoopCSharpStatement
    {
        internal DoCSharpStatement(Expression body, Expression test, LabelTarget breakLabel, LabelTarget continueLabel)
            : base(test, body, breakLabel, continueLabel)
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
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitDo(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="breakLabel">The <see cref="LoopCSharpStatement.BreakLabel" /> property of the result.</param>
        /// <param name="continueLabel">The <see cref="LoopCSharpStatement.ContinueLabel" /> property of the result.</param>
        /// <param name="body">The <see cref="LoopCSharpStatement.Body" /> property of the result.</param>
        /// <param name="test">The <see cref="ConditionalLoopCSharpStatement.Test" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public DoCSharpStatement Update(LabelTarget breakLabel, LabelTarget continueLabel, Expression body, Expression test)
        {
            if (breakLabel == this.BreakLabel && continueLabel == this.ContinueLabel && body == this.Body && test == this.Test)
            {
                return this;
            }

            return CSharpExpression.Do(body, test, breakLabel, continueLabel);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            var @break = BreakLabel ?? Expression.Label("__break");
            var @continue = ContinueLabel ?? Expression.Label("__continue");
            var begin = Expression.Label("__begin");

            var loop =
                Expression.Block(
                    typeof(void),
                    Expression.Label(begin),
                    Body,
                    Expression.Label(@continue),
                    Expression.IfThen(
                        Test,
                        Expression.Goto(begin)
                    ),
                    Expression.Label(@break)
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
        public static DoCSharpStatement Do(Expression body, Expression test)
        {
            return Do(body, test, null, null);
        }

        /// <summary>
        /// Creates a <see cref="DoCSharpStatement"/> that represents a do...while loop.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <returns>The created <see cref="DoCSharpStatement"/>.</returns>
        public static DoCSharpStatement Do(Expression body, Expression test, LabelTarget @break)
        {
            return Do(body, test, @break, null);
        }

        /// <summary>
        /// Creates a <see cref="DoCSharpStatement"/> that represents a do...while loop.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <returns>The created <see cref="DoCSharpStatement"/>.</returns>
        public static DoCSharpStatement Do(Expression body, Expression test, LabelTarget @break, LabelTarget @continue)
        {
            ValidateLoop(test, body, @break, @continue);

            return new DoCSharpStatement(body, test, @break, @continue);
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
        protected internal virtual Expression VisitDo(DoCSharpStatement node)
        {
            return node.Update(VisitLabelTarget(node.BreakLabel), VisitLabelTarget(node.ContinueLabel), Visit(node.Body), Visit(node.Test));
        }
    }
}
