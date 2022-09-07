// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a lock statement.
    /// </summary>
    public sealed partial class LockCSharpStatement : CSharpStatement
    {
        private static MethodInfo? s_enterMethod, s_exitMethod;

        internal LockCSharpStatement(Expression expression, Expression body)
        {
            Expression = expression;
            Body = body;
        }

        /// <summary>
        /// Gets the <see cref="System.Linq.Expressions.Expression" /> representing the object to lock on.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Gets the <see cref="System.Linq.Expressions.Expression" /> representing the body.
        /// </summary>
        public Expression Body { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Lock;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitLock(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <param name="body">The <see cref="Body" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public LockCSharpStatement Update(Expression expression, Expression body)
        {
            if (expression == Expression && body == Body)
            {
                return this;
            }

            return CSharpExpression.Lock(expression, body);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            var enterMethod = s_enterMethod ??= typeof(Monitor).GetMethod(nameof(Monitor.Enter), new[] { typeof(object), typeof(bool).MakeByRefType() })!; // TODO: well-known members
            var exitMethod = s_exitMethod ??= typeof(Monitor).GetMethod(nameof(Monitor.Exit), new[] { typeof(object) })!; // TODO: well-known members

            var temp = Expression.Parameter(Expression.Type, "__lock");
            var lockTaken = Expression.Parameter(typeof(bool), "__lockWasTaken");

            var enterExpr = Expression.Call(enterMethod, temp, lockTaken);
            var body = Body is BlockExpression block
                ? block.Update(block.Variables, block.Expressions.AddFirst(enterExpr))
                : Expression.Block(enterExpr, Body);
            
            var res =
                Expression.Block(
                    new[] { lockTaken, temp },
                    // NB: Assignment is important here, cf. the `CrossCheck_Lock_ManOrBoy` test where
                    //     a loop is repeatedly entering and exiting the lock. If the variable is still
                    //     set to true from the last iteration, the `Enter` method won't re-acquire the
                    //     lock thus causing `Exit` to complain with a `SynchronizationLockException`.
                    Expression.Assign(lockTaken, ConstantFalse),
                    Expression.Assign(temp, Expression),
                    Expression.TryFinally(
                        body,
                        Expression.IfThen(lockTaken,
                            Expression.Call(exitMethod, temp)
                        )
                    )
                );

            return res;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="LockCSharpStatement"/> that represents a lock statement.
        /// </summary>
        /// <param name="expression">The object to lock on.</param>
        /// <param name="body">The body of the statement.</param>
        /// <returns>The created <see cref="LockCSharpStatement"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static LockCSharpStatement Lock(Expression expression, Expression body)
        {
            RequiresCanRead(expression, nameof(expression));
            RequiresCanRead(body, nameof(body));

            ValidateType(expression.Type, nameof(expression));

            if (expression.Type.IsValueType)
                throw Error.LockNeedsReferenceType(expression.Type);

            return new LockCSharpStatement(expression, body);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="LockCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitLock(LockCSharpStatement node) =>
            node.Update(
                Visit(node.Expression),
                Visit(node.Body)
            );
    }
}
