// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using static System.Dynamic.Utils.ContractUtils;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a null-conditional access operation.
    /// </summary>
    public partial class ConditionalAccessCSharpExpression : ConditionalAccessCSharpExpression<Expression>
    {
        internal ConditionalAccessCSharpExpression(Expression receiver, ConditionalReceiver nonNullReceiver, Expression whenNotNull)
            : base(receiver, nonNullReceiver, whenNotNull)
        {
        }
    }

    /// <summary>
    /// Represents a null-conditional access operation.
    /// </summary>
    /// <typeparam name="TExpression">The type of the expression performed when the accessed receiver is non-null.</typeparam>
    public partial class ConditionalAccessCSharpExpression<TExpression> : CSharpExpression
        where TExpression : Expression
    {
        internal ConditionalAccessCSharpExpression(Expression receiver, ConditionalReceiver nonNullReceiver, TExpression whenNotNull)
        {
            Receiver = receiver;
            NonNullReceiver = nonNullReceiver;
            WhenNotNull = whenNotNull;
        }

        /// <summary>
        /// Gets the <see cref="System.Linq.Expressions.Expression"/> representing the conditionally accessed receiver.
        /// </summary>
        public Expression Receiver { get; }

        /// <summary>
        /// Gets the <see cref="ConditionalReceiver"/> representing the non-null receiver referenced in <see cref="WhenNotNull"/>.
        /// </summary>
        public ConditionalReceiver NonNullReceiver { get; }

        /// <summary>
        /// Gets the <see cref="System.Linq.Expressions.Expression"/> representing the operation to carry out on the <see cref="NonNullReceiver"/>.
        /// </summary>
        public TExpression WhenNotNull { get; }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="System.Linq.Expressions.Expression" /> represents. (Inherited from <see cref="System.Linq.Expressions.Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => WhenNotNull.Type.GetConditionalType();

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.ConditionalAccess;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitConditionalAccess(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="receiver">The <see cref="Receiver" /> property of the result.</param>
        /// <param name="nonNullReceiver">The <see cref="NonNullReceiver" /> property of the result.</param>
        /// <param name="whenNotNull">The <see cref="WhenNotNull" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ConditionalAccessCSharpExpression<TExpression> Update(Expression receiver, ConditionalReceiver nonNullReceiver, TExpression whenNotNull)
        {
            if (receiver == this.Receiver && nonNullReceiver == this.NonNullReceiver && whenNotNull == this.WhenNotNull)
            {
                return this;
            }

            return new ConditionalAccessCSharpExpression<TExpression>(receiver, nonNullReceiver, whenNotNull);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var resultType = Type;

            var receiverType = Receiver.Type;

            var receiver = Expression.Parameter(receiverType);
            var evalReceiver = Expression.Assign(receiver, Receiver);

            var nonNullCheck = default(Expression);
            var nonNull = default(Expression);

            if (receiverType.IsNullableType())
            {
                nonNullCheck = Expression.Property(receiver, "HasValue");
                nonNull = Expression.Property(receiver, "Value");
            }
            else
            {
                nonNullCheck = Expression.NotEqual(receiver, Expression.Default(receiverType));
                nonNull = receiver;
            }

            var whenNotNull = new SubstituteConditionalReceiver(NonNullReceiver, nonNull).Visit(WhenNotNull);

            var res = default(Expression);

            if (resultType != typeof(void))
            {
                var result = Expression.Parameter(resultType);

                if (whenNotNull.Type != result.Type)
                {
                    whenNotNull = Expression.Convert(whenNotNull, resultType);
                }

                res =
                    Expression.Block(
                        new[] { receiver, result },
                        evalReceiver,
                        Expression.IfThen(
                            nonNullCheck,
                            Expression.Assign(result, whenNotNull)
                        ),
                        result
                    );
            }
            else
            {
                res =
                    Expression.Block(
                        new[] { receiver },
                        evalReceiver,
                        Expression.IfThen(
                            nonNullCheck,
                            whenNotNull
                        )
                    );
            }

            return res;
        }

        class SubstituteConditionalReceiver : CSharpExpressionVisitor
        {
            private readonly ConditionalReceiver _receiver;
            private readonly Expression _nonNull;

            public SubstituteConditionalReceiver(ConditionalReceiver receiver, Expression nonNull)
            {
                _receiver = receiver;
                _nonNull = nonNull;
            }

            protected internal override Expression VisitConditionalReceiver(ConditionalReceiver node)
            {
                // TODO: We could check that we find the receiver exactly once.

                if (node == _receiver)
                {
                    return _nonNull;
                }

                return base.VisitConditionalReceiver(node);
            }

            protected internal override Expression VisitConditionalAccess<TWhenNotNull>(ConditionalAccessCSharpExpression<TWhenNotNull> node)
            {
                if (node.NonNullReceiver == _receiver)
                {
                    var receiver = Visit(node.Receiver);
                    return node.Update(receiver, node.NonNullReceiver, node.WhenNotNull);
                }

                return base.VisitConditionalAccess(node);
            }
        }

        internal static ConditionalReceiver MakeReceiver(Expression receiver)
        {
            return CSharpExpression.ConditionalReceiver(receiver.Type.GetNonNullableType());
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ConditionalAccessCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitConditionalAccess<TExpression>(ConditionalAccessCSharpExpression<TExpression> node)
            where TExpression : Expression
        {
            return node.Update(Visit(node.Receiver), VisitAndConvert(node.NonNullReceiver, nameof(VisitConditionalAccess)), VisitAndConvert(node.WhenNotNull, nameof(VisitConditionalAccess)));
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="ConditionalAccessCSharpExpression"/> representing a null-conditional access operation.
        /// </summary>
        /// <param name="receiver">The receiver to access conditionally.</param>
        /// <param name="nonNullReceiver">The non-null receiver used in the <paramref name="whenNotNull"/> expression.</param>
        /// <param name="whenNotNull">The operation to apply to the receiver when it's non-null.</param>
        /// <returns>A <see cref="Microsoft.CSharp.Expressions.ConditionalReceiver"/> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalReciever" /> and the <see cref="Expression.Type" /> property equal to the specified type.</returns>
        public static ConditionalAccessCSharpExpression ConditionalAccess(Expression receiver, ConditionalReceiver nonNullReceiver, Expression whenNotNull)
        {
            RequiresCanRead(receiver, nameof(receiver));
            RequiresNotNull(nonNullReceiver, nameof(nonNullReceiver));
            RequiresCanRead(whenNotNull, nameof(whenNotNull));

            var receiverType = receiver.Type;
            if (receiverType == typeof(void) || receiverType.IsByRef || (receiverType.IsValueType && !receiverType.IsNullableType()))
            {
                throw Error.InvalidConditionalReceiverExpressionType(receiverType);
            }

            var nonNullReceiverType = receiverType.GetNonNullReceiverType();
            if (nonNullReceiverType != nonNullReceiver.Type)
            {
                throw Error.ConditionalReceiverTypeMismatch(receiverType, nonNullReceiverType);
            }

            // DESIGN: We could make instances of type ConditionalAccessCSharpExpression<TExpression> and/or specialized subtypes
            //         if we decide to keep those. However, it may look strange if `whenNotNull` is not just a single access but
            //         consists of a 'chain' of operations. Right now, the idea is to have the specialized subtypes merely as a
            //         convenience when using factories by hand, but we could scrap it all and just stick with the primitive node.
            return new ConditionalAccessCSharpExpression(receiver, nonNullReceiver, whenNotNull);
        }
    }
}
