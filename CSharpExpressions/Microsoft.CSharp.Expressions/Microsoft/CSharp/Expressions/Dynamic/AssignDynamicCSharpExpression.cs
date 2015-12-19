// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using static Microsoft.CSharp.Expressions.Helpers;

namespace Microsoft.CSharp.Expressions
{
    partial class AssignBinaryDynamicCSharpExpression
    {
        private Expression ReduceAssign()
        {
            var lhs = Left.Expression;

            var getMember = lhs as GetMemberDynamicCSharpExpression;
            if (getMember != null)
            {
                return getMember.ReduceAssignment(Right.Expression, Flags, Left.Flags, Right.Flags);
            }

            var getIndex = lhs as GetIndexDynamicCSharpExpression;
            if (getIndex != null)
            {
                return getIndex.ReduceAssignment(Right.Expression, Flags, Left.Flags, Right.Flags);
            }

            return ReduceStaticAssign(Left.Expression);
        }

        private Expression ReduceStaticAssign(Expression lhs)
        {
            lhs = EnsureWriteable(lhs);
            var rhs = Right.Expression;

            if (!TypeUtils.AreReferenceAssignable(lhs.Type, rhs.Type))
            {
                rhs = DynamicConvert(rhs, lhs.Type, CSharpBinderFlags.None, Context);
            }

            return Expression.Assign(lhs, rhs);
        }
    }

    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates a new expression representing a dynamically bound binary assignment operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAssign(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.Assign, DynamicArgument(left), DynamicArgument(right), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary assignment operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAssign(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.Assign, left, right, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary assignment operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.Assign, left, right, binderFlags, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary assignment operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.Assign, left, right, binderFlags, context);
        }
    }
}
