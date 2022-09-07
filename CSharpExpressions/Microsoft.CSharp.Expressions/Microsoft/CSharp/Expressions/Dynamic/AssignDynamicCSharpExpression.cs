// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq.Expressions;

using Microsoft.CSharp.RuntimeBinder;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    partial class AssignBinaryDynamicCSharpExpression
    {
        private Expression ReduceAssign()
        {
            var lhs = Left.Expression;

            return lhs switch
            {
                GetMemberDynamicCSharpExpression getMember => getMember.ReduceAssignment(Right.Expression, Flags, Left.Flags, Right.Flags),
                GetIndexDynamicCSharpExpression getIndex => getIndex.ReduceAssignment(Right.Expression, Flags, Left.Flags, Right.Flags),
                _ => ReduceStaticAssign(),
            };

            Expression ReduceStaticAssign()
            {
                lhs = MakeWriteable(lhs);
                var rhs = Right.Expression;

                if (!TypeUtils.AreReferenceAssignable(lhs.Type, rhs.Type))
                {
                    rhs = DynamicConvert(rhs, lhs.Type, CSharpBinderFlags.None, Context);
                }

                return Expression.Assign(lhs, rhs);
            }
        }

        private Expression ReduceNullCoalescingAssign()
        {
            var temps = new List<ParameterExpression>();
            var stmts = new List<Expression>();

            Expression lhs;
            Expression assign;

            switch (Left.Expression)
            {
                case GetMemberDynamicCSharpExpression getMember:
                    {
                        var getMemberLhs = getMember.TransformToLhs(temps, stmts);
                        lhs = getMemberLhs;
                        assign = getMemberLhs.ReduceAssignment(Right.Expression, Flags, Left.Flags, Right.Flags);
                    }
                    break;
                case GetIndexDynamicCSharpExpression getIndex:
                    {
                        var getIndexLhs = getIndex.TransformToLhs(temps, stmts);
                        lhs = getIndexLhs;
                        assign = getIndexLhs.ReduceAssignment(Right.Expression, Flags, Left.Flags, Right.Flags);
                    }
                    break;
                default:
                    {
                        lhs = MakeWriteable(Left.Expression);
                        assign = Expression.Assign(lhs, Right.Expression);
                    }
                    break;
            }
            
            var coalesce = Expression.Coalesce(lhs, assign);
            stmts.Add(coalesce);

            return Expression.Block(Type, temps, stmts);
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
        public static AssignBinaryDynamicCSharpExpression DynamicAssign(Expression left, Expression right) =>
            MakeDynamicBinaryAssign(CSharpExpressionType.Assign, DynamicArgument(left), DynamicArgument(right), CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary assignment operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAssign(DynamicCSharpArgument left, DynamicCSharpArgument right) =>
            MakeDynamicBinaryAssign(CSharpExpressionType.Assign, left, right, CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary assignment operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags) =>
            MakeDynamicBinaryAssign(CSharpExpressionType.Assign, left, right, binderFlags, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary assignment operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type? context) =>
            MakeDynamicBinaryAssign(CSharpExpressionType.Assign, left, right, binderFlags, context);

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary null-coalescing assignment operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicNullCoalescingAssign(Expression left, Expression right) =>
            MakeDynamicBinaryAssign(CSharpExpressionType.NullCoalescingAssign, DynamicArgument(left), DynamicArgument(right), CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary null-coalescing assignment operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicNullCoalescingAssign(DynamicCSharpArgument left, DynamicCSharpArgument right) =>
            MakeDynamicBinaryAssign(CSharpExpressionType.NullCoalescingAssign, left, right, CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary null-coalescing assignment operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicNullCoalescingAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags) =>
            MakeDynamicBinaryAssign(CSharpExpressionType.NullCoalescingAssign, left, right, binderFlags, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary null-coalescing assignment operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicNullCoalescingAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type? context) =>
            MakeDynamicBinaryAssign(CSharpExpressionType.NullCoalescingAssign, left, right, binderFlags, context);
    }
}
