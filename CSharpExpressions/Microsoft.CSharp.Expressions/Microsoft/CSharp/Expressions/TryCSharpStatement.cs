// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a try statement.
    /// </summary>
    public sealed partial class TryCSharpStatement : CSharpStatement
    {
        internal TryCSharpStatement(Expression tryBlock, ReadOnlyCollection<CSharpCatchBlock> catchBlocks, Expression? finallyBlock)
        {
            TryBlock = tryBlock;
            CatchBlocks = catchBlocks;
            FinallyBlock = finallyBlock;
        }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the try block.
        /// </summary>
        public Expression TryBlock { get; }

        /// <summary>
        /// Gets a collection of <see cref="CSharpCatchBlock" /> representing the catch blocks.
        /// </summary>
        public ReadOnlyCollection<CSharpCatchBlock> CatchBlocks { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the finally block.
        /// </summary>
        public Expression? FinallyBlock { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Try;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitTry(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="tryBlock">The <see cref="TryBlock" /> property of the result.</param>
        /// <param name="catchBlocks">The <see cref="CatchBlocks" /> property of the result.</param>
        /// <param name="finallyBlock">The <see cref="FinallyBlock" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public TryCSharpStatement Update(Expression tryBlock, IEnumerable<CSharpCatchBlock> catchBlocks, Expression? finallyBlock)
        {
            if (tryBlock == TryBlock && SameElements(ref catchBlocks!, CatchBlocks) && finallyBlock == FinallyBlock)
            {
                return this;
            }

            return Try(tryBlock, catchBlocks, finallyBlock);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            var temps = new List<ParameterExpression>();

            var catchBlocks = new List<CatchBlock>(CatchBlocks.Count);

            foreach (var catchBlock in CatchBlocks)
            {
                catchBlocks.Add(catchBlock.Reduce(temps.Add));
            }

            var tryExpr = Expression.MakeTry(typeof(void), TryBlock, FinallyBlock, fault: null, catchBlocks);

            if (temps.Count == 0)
            {
                return tryExpr;
            }

            return Expression.Block(temps, tryExpr);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="TryCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitTry(TryCSharpStatement node) =>
            node.Update(
                Visit(node.TryBlock),
                Visit(node.CatchBlocks, VisitCatchBlock),
                Visit(node.FinallyBlock)
            );
    }

    partial class CSharpStatement
    {
        /// <summary>
        /// Creates a <see cref="TryCSharpStatement" /> representing a try block with any number of catch blocks and no finally block.
        /// </summary>
        /// <param name="body">The body of the try block.</param>
        /// <param name="handlers">The array of <see cref="CSharpCatchBlock" /> expressions representing the catch blocks to be associated with the try block.</param>
        /// <returns>The created <see cref="TryCSharpStatement" />.</returns>
        public static TryCSharpStatement TryCatch(Expression body, params CSharpCatchBlock[] handlers) => Try(body, handlers, finallyBlock: null);

        /// <summary>
        /// Creates a <see cref="TryCSharpStatement" /> representing a try block with a finally block and no catch statements.
        /// </summary>
        /// <param name="body">The body of the try block.</param>
        /// <param name="finally">The body of the finally block.</param>
        /// <returns>The created <see cref="TryCSharpStatement" />.</returns>
        public static new TryCSharpStatement TryFinally(Expression body, Expression @finally) => Try(body, handlerBlocks: null, @finally);

        /// <summary>
        /// Creates a <see cref="TryCSharpStatement" /> representing a try block with any number of catch blocks and a finally block.
        /// </summary>
        /// <param name="body">The body of the try block.</param>
        /// <param name="finally">The body of the finally block.</param>
        /// <param name="handlers">The array of zero or more <see cref="CSharpCatchBlock" /> expressions representing the catch blocks to be associated with the try block.</param>
        /// <returns>The created <see cref="TryCSharpStatement" />.</returns>
        public static TryCSharpStatement TryCatchFinally(Expression body, Expression @finally, params CSharpCatchBlock[] handlers) => Try(body, handlers, @finally);

        /// <summary>
        /// Creates a <see cref="TryCSharpStatement" /> representing a try block with zero or more catch blocks and an optional finally block.
        /// </summary>
        /// <param name="tryBlock">The try block.</param>
        /// <param name="handlerBlocks">The array of zero or more <see cref="CSharpCatchBlock" /> expressions representing the catch blocks to be associated with the try block.</param>
        /// <param name="finallyBlock">The finally block.</param>
        /// <returns>The created <see cref="TryCSharpStatement" />.</returns>
        public static TryCSharpStatement Try(Expression tryBlock, IEnumerable<CSharpCatchBlock>? handlerBlocks, Expression? finallyBlock)
        {
            RequiresCanRead(tryBlock, nameof(tryBlock));

            var handlers = handlerBlocks.ToReadOnly();
            RequiresNotNullItems(handlers, nameof(handlerBlocks));

            if (finallyBlock != null)
            {
                RequiresCanRead(finallyBlock, nameof(finallyBlock));
            }
            else if (handlers.Count == 0)
            {
                throw TryMustHaveCatchFinallyOrFault();
            }

            return new TryCSharpStatement(tryBlock, handlers, finallyBlock);
        }
    }
}
