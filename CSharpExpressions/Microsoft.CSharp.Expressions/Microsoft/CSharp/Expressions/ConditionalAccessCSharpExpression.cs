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
        // DESIGN: This node exposes the access expression in a strongly-typed manner which can make it easier to consume,
        //         for example in a visitor. Its use is limited to manually created conditional access expressions using
        //         specialized factories, although we could make the general-purpose factory instantiate a generic based on
        //         the access node type. It may have to check for valid uses anyway, so it could know what the access type
        //         is. Note that the code emitted by the Roslyn compiler will bind to the generic factory.
        //
        //         However, the typing benefit of this node is rather shallow given that the access expression could be
        //         more than just a single operation. It'd only capture the node type of the last operation. As such, it's
        //         only useful when users construct an explicit left-associative series of conditional access expressions,
        //         such that each access expression only involves a single operation, e.g.
        //
        //           ((x?.Foo)?.Bar(qux))?[baz]
        //               ^^^^  ^^^^^^^^^  ^^^^^
        //              Member   Call     Index
        //
        //         When the right-associative nature of the conditional access expression is used, access expressions can
        //         be arbitrarily complex, so the strong typing of the access expression is shallow, e.g.
        //
        //           x?.Foo.Bar(qux)?[baz].Corge()
        //             ^^^^^^^^^^^^^ ^^^^^^^^^^^^^
        //                  Call         Call
        //
        //         As such, we may just want to do away with this typing. We could still keep some of the factory methods
        //         to make it easier to construct conditional access nodes involving a single access operation.

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

            // TODO: More elaborate checking of `whenNotNull` would be possible, in particular to check the following:
            //
            //         - only supported operations can occur in the access expression
            //         - the conditional receiver is used in an input position
            //         - the conditional receiver is used exactly once
            //
            //       The only drawback is that those checks will involve a visit to the access expression, which doesn't have a
            //       constant cost. When those nodes are deeply nested, we'd be going over the same tree many times, unless we
            //       relax the check for the conditional receiver usage count to be at least once. This would also happen during
            //       a rewrite of the tree by means of a visitor, which causes factory invocations.
            //
            //       Note that the expression compiler can still catch such violations during reduction by only reducing one
            //       occurrence of each conditional receiver, therefore leaving any other occurrences unbound, which on its turn
            //       causes a reduction error for the unbound conditional receiver (which is not a reducible node).
            //
            //       What are the implications of not performing those checks? Nothing unsafe will happen but the node type will
            //       effectively capture a superset of the possibilities provided by the C# language for the construct. E.g.
            //       you could construct a null-conditional call where the conditional receiver is used for a parameter that's
            //       different from the receiving instance (or the `this` parameter for an extension method):
            //
            //         ConditionalAccess(bar, receiver, Call(foo, qux, { receiver }))  ==  foo.qux(bar) iff bar != null
            //                                               ^^^         ~~~~~~~~
            //                                            instance       argument
            //
            //       We could decide the cost of checking doesn't yield enough benefits given that it only applies to hand-
            //       crafted expressions (i.e. the compiler will never emit a non-standard form). We'd still have to support
            //       this behavior in future versions and consider it to have well-defined behavior, or document it as having
            //       undefined behavior (with little predecents) from the get-go.
            //
            //       Finally, note that we could do away with this problem by modeling the nodes differently without relying on
            //       a node for the conditional receiver. To do so, we'd have to introduce expressions for argument lists that
            //       don't include the receiver, which duplicates a lot of existing LINQ nodes. In such a design, the receiver
            //       would be implicit and always correspond to the "left-most source" in the access expression. It comes at
            //       the cost of not being able to reuse existing LINQ nodes which always have a bound receiver, but we could
            //       piggyback on the work to introduce C#-specific nodes for e.g. ParameterAssignment to create nodes for e.g.
            //       argument lists. Old LINQ nodes could be considered applications of more primitive partial nodes:
            //
            //         Call(o, m, args)   ==  Access(o, Call(m, args))   ==  o.m(args)
            //         Member(o, p)       ==  Access(o, Member(p))       ==  o.p
            //         Index(o, i, args)  ==  Access(o, Index(i, args))  ==  o.i[args]
            //         Invoke(f, args)    ==  Access(f, Invoke(args))    ==  o(args)
            //
            //       In this setting, we could still allow further composition of the access expressions by passing a list of
            //       access operations in lieu of the second operand to `Access`, e.g.
            //
            //                    Access(o, { Member(p), Call(m, args2) })  ==  o.p.m(args2)
            //
            //       Substituting `Access` for `ConditionalAccess` would mean the following:
            //
            //         ConditionalAccess(o, { Member(p), Call(m, args2) })  ==  o?.p.m(args2)
            //
            //       Note that it makes for a much stranger tree representation (though arguably the current form is weird too)
            //       with new partially applied `Member`, `Call`, `Index`, and `Invoke` constructs. Instead, we could nest them
            //       as well, by only having one (inner-most, left-most) access-unbound node in the access expression:
            //
            //         ConditionalAccess(o, Call(Member(   p), m, args2))
            //                                          ^^
            //                                         hole
            //
            //       With this form, the check is still expensive, because we need to check for exactly one unbound form in the
            //       source chain of the access expression.

            // DESIGN: We could make instances of type ConditionalAccessCSharpExpression<TExpression> and/or specialized subtypes
            //         if we decide to keep those. However, it may look strange if `whenNotNull` is not just a single access but
            //         consists of a 'chain' of operations. Right now, the idea is to have the specialized subtypes merely as a
            //         convenience when using factories by hand, but we could scrap it all and just stick with the primitive node.
            return new ConditionalAccessCSharpExpression(receiver, nonNullReceiver, whenNotNull);
        }
    }
}
