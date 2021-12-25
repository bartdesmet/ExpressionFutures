// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Microsoft.CSharp.RuntimeBinder;

using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    using static DynamicHelpers;

    /// <summary>
    /// Represents a dynamically bound unary assignment operation.
    /// </summary>
    public sealed partial class AssignUnaryDynamicCSharpExpression : DynamicCSharpExpression
    {
        internal AssignUnaryDynamicCSharpExpression(Type context, CSharpBinderFlags binderFlags, CSharpExpressionType unaryType, DynamicCSharpArgument operand)
            : base(context, binderFlags)
        {
            OperationNodeType = unaryType;
            Operand = operand;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.DynamicUnaryAssign;

        /// <summary>
        /// Gets the expression kind of the dynamically bound operation.
        /// </summary>
        public CSharpExpressionType OperationNodeType { get; }

        /// <summary>
        /// Gets the operand of the unary operation.
        /// </summary>
        public DynamicCSharpArgument Operand { get; }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="System.Linq.Expressions.Expression" /> represents. (Inherited from <see cref="System.Linq.Expressions.Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => Operand.Expression.Type;

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var functionalOp = new Func<Expression, Expression>(lhs =>
            {
                var operation = default(ExpressionType);

                switch (OperationNodeType)
                {
                    case CSharpExpressionType.PreIncrementAssign:
                    case CSharpExpressionType.PreIncrementAssignChecked:
                    case CSharpExpressionType.PostIncrementAssign:
                    case CSharpExpressionType.PostIncrementAssignChecked:
                        operation = ExpressionType.Increment;
                        break;
                    case CSharpExpressionType.PreDecrementAssign:
                    case CSharpExpressionType.PreDecrementAssignChecked:
                    case CSharpExpressionType.PostDecrementAssign:
                    case CSharpExpressionType.PostDecrementAssignChecked:
                        operation = ExpressionType.Decrement;
                        break;

                    default:
                        throw Unreachable;
                }

                var args = new[]
                {
                    CSharpArgumentInfo.Create(Operand.Flags, name: null)
                };

                var binder = Binder.UnaryOperation(Flags, operation, Context, args);
                var dynamic = DynamicHelpers.MakeDynamic(typeof(object), binder, new[] { lhs }, new[] { lhs.Type });

                // NB: no conversion needed in the unary case

                return dynamic;
            });

            var flags = Flags | CSharpBinderFlags.ValueFromCompoundAssignment;

            var res = ReduceDynamicAssignment(Operand, functionalOp, flags, IsPrefix);

            return res;
        }

        private bool IsPrefix
        {
            get
            {
                switch (OperationNodeType)
                {
                    case CSharpExpressionType.PreIncrementAssign:
                    case CSharpExpressionType.PreDecrementAssign:
                    case CSharpExpressionType.PreIncrementAssignChecked:
                    case CSharpExpressionType.PreDecrementAssignChecked:
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Reduces the dynamic expression to a binder and a set of arguments to apply the operation to.
        /// </summary>
        /// <param name="binder">The binder used to perform the dynamic operation.</param>
        /// <param name="arguments">The arguments to apply the dynamic operation to.</param>
        /// <param name="argumentTypes">The types of the arguments to use for the dynamic call site. Return null to infer types.</param>
        protected override void ReduceDynamic(out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[] argumentTypes) => throw Unreachable;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitDynamicUnaryAssign(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="operand">The <see cref="Operand" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public AssignUnaryDynamicCSharpExpression Update(DynamicCSharpArgument operand)
        {
            if (operand == Operand)
            {
                return this;
            }

            return DynamicCSharpExpression.MakeDynamicUnaryAssign(OperationNodeType, operand, Flags, Context);
        }
    }

    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates a new expression representing a dynamically bound unary assignment operation.
        /// </summary>
        /// <param name="unaryType">The type of the unary operation to perform.</param>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary assignment operation.</returns>
        public static AssignUnaryDynamicCSharpExpression MakeDynamicUnaryAssign(CSharpExpressionType unaryType, Expression operand) =>
            MakeDynamicUnaryAssign(unaryType, DynamicArgument(operand), CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary assignment operation.
        /// </summary>
        /// <param name="unaryType">The type of the unary operation to perform.</param>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary assignment operation.</returns>
        public static AssignUnaryDynamicCSharpExpression MakeDynamicUnaryAssign(CSharpExpressionType unaryType, DynamicCSharpArgument operand) =>
            MakeDynamicUnaryAssign(unaryType, operand, CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary assignment operation with the specified binder flags.
        /// </summary>
        /// <param name="unaryType">The type of the unary operation to perform.</param>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary assignment operation.</returns>
        public static AssignUnaryDynamicCSharpExpression MakeDynamicUnaryAssign(CSharpExpressionType unaryType, DynamicCSharpArgument operand, CSharpBinderFlags binderFlags) =>
            MakeDynamicUnaryAssign(unaryType, operand, binderFlags, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary assignment operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="unaryType">The type of the unary operation to perform.</param>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary assignment operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static AssignUnaryDynamicCSharpExpression MakeDynamicUnaryAssign(CSharpExpressionType unaryType, DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            RequiresNotNull(operand, nameof(operand));

            RequiresCanRead(operand.Expression, nameof(operand));
            RequiresCanWrite(operand.Expression, nameof(operand));

            CheckUnaryAssign(unaryType);

            switch (unaryType)
            {
                case CSharpExpressionType.PreIncrementAssignChecked:
                case CSharpExpressionType.PreDecrementAssignChecked:
                case CSharpExpressionType.PostIncrementAssignChecked:
                case CSharpExpressionType.PostDecrementAssignChecked:
                    binderFlags |= CSharpBinderFlags.CheckedContext;
                    break;
            }

            return new AssignUnaryDynamicCSharpExpression(context, binderFlags, unaryType, operand);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="AssignUnaryDynamicCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDynamicUnaryAssign(AssignUnaryDynamicCSharpExpression node) =>
            node.Update(
                VisitDynamicArgument(node.Operand)
            );
    }
}
