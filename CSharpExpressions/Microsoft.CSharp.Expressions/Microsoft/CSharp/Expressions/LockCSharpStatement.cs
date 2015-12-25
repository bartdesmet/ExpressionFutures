// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a lock statement.
    /// </summary>
    public sealed partial class LockCSharpStatement : CSharpStatement
    {
        private static MethodInfo s_enterMethod, s_exitMethod;

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
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitLock(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <param name="body">The <see cref="Body" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public LockCSharpStatement Update(Expression expression, Expression body)
        {
            if (expression == this.Expression && body == this.Body)
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
            var enterMethod = s_enterMethod ?? (s_enterMethod = typeof(Monitor).GetMethod("Enter", new[] { typeof(object), typeof(bool).MakeByRefType() }));
            var exitMethod = s_exitMethod ?? (s_exitMethod = typeof(Monitor).GetMethod("Exit", new[] { typeof(object) }));

            var lockTaken = Expression.Parameter(typeof(bool), "__lockWasTaken");
            
            var res =
                Expression.Block(
                    new[] { lockTaken },
                    // NB: Assignment is important here, cf. the `CrossCheck_Lock_ManOrBoy` test where
                    //     a loop is repeatedly entering and exiting the lock. If the variable is still
                    //     set to true from the last iteration, the `Enter` method won't re-acquire the
                    //     lock thus causing `Exit` to complain with a `SynchronizationLockException`.
                    Expression.Assign(lockTaken, Expression.Constant(false)),
                    Expression.TryFinally(
                        Expression.Block(
                            Expression.Call(s_enterMethod, Expression, lockTaken),
                            Body
                        ),
                        Expression.IfThen(lockTaken,
                            Expression.Call(s_exitMethod, Expression)
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

            ValidateType(expression.Type);

            if (expression.Type.IsValueType)
            {
                throw Error.LockNeedsReferenceType(expression.Type);
            }

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
        protected internal virtual Expression VisitLock(LockCSharpStatement node)
        {
            return node.Update(Visit(node.Expression), Visit(node.Body));
        }
    }
}
