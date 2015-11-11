// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Linq.Expressions;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Add' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Add' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAdd(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.Add, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Add' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Add' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAdd(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.Add, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Add' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Add' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAdd(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.Add, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Add' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Add' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAdd(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.Add, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AddChecked' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AddChecked' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAddChecked(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.AddChecked, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AddChecked' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AddChecked' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAddChecked(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.AddChecked, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AddChecked' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AddChecked' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAddChecked(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.AddChecked, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AddChecked' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AddChecked' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAddChecked(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.AddChecked, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'And' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'And' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAnd(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.And, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'And' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'And' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAnd(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.And, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'And' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'And' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAnd(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.And, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'And' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'And' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAnd(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.And, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AndAlso' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AndAlso' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAndAlso(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.AndAlso, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AndAlso' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AndAlso' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAndAlso(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.AndAlso, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AndAlso' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AndAlso' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAndAlso(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.AndAlso, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AndAlso' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AndAlso' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicAndAlso(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.AndAlso, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ArrayIndex' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ArrayIndex' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicArrayIndex(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.ArrayIndex, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ArrayIndex' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ArrayIndex' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicArrayIndex(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.ArrayIndex, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ArrayIndex' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ArrayIndex' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicArrayIndex(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.ArrayIndex, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ArrayIndex' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ArrayIndex' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicArrayIndex(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.ArrayIndex, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Coalesce' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Coalesce' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicCoalesce(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.Coalesce, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Coalesce' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Coalesce' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicCoalesce(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.Coalesce, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Coalesce' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Coalesce' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicCoalesce(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.Coalesce, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Coalesce' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Coalesce' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicCoalesce(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.Coalesce, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Divide' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Divide' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicDivide(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.Divide, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Divide' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Divide' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicDivide(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.Divide, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Divide' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Divide' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicDivide(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.Divide, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Divide' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Divide' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicDivide(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.Divide, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Equal' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Equal' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicEqual(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.Equal, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Equal' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Equal' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicEqual(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.Equal, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Equal' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Equal' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicEqual(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.Equal, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Equal' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Equal' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicEqual(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.Equal, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ExclusiveOr' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ExclusiveOr' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicExclusiveOr(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.ExclusiveOr, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ExclusiveOr' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ExclusiveOr' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicExclusiveOr(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.ExclusiveOr, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ExclusiveOr' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ExclusiveOr' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicExclusiveOr(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.ExclusiveOr, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ExclusiveOr' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ExclusiveOr' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicExclusiveOr(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.ExclusiveOr, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'GreaterThan' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'GreaterThan' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicGreaterThan(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.GreaterThan, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'GreaterThan' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'GreaterThan' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicGreaterThan(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.GreaterThan, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'GreaterThan' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'GreaterThan' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicGreaterThan(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.GreaterThan, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'GreaterThan' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'GreaterThan' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicGreaterThan(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.GreaterThan, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'GreaterThanOrEqual' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'GreaterThanOrEqual' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicGreaterThanOrEqual(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.GreaterThanOrEqual, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'GreaterThanOrEqual' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'GreaterThanOrEqual' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicGreaterThanOrEqual(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.GreaterThanOrEqual, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'GreaterThanOrEqual' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'GreaterThanOrEqual' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicGreaterThanOrEqual(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.GreaterThanOrEqual, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'GreaterThanOrEqual' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'GreaterThanOrEqual' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicGreaterThanOrEqual(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.GreaterThanOrEqual, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LeftShift' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LeftShift' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicLeftShift(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.LeftShift, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LeftShift' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LeftShift' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicLeftShift(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.LeftShift, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LeftShift' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LeftShift' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicLeftShift(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.LeftShift, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LeftShift' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LeftShift' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicLeftShift(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.LeftShift, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LessThan' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LessThan' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicLessThan(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.LessThan, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LessThan' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LessThan' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicLessThan(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.LessThan, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LessThan' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LessThan' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicLessThan(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.LessThan, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LessThan' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LessThan' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicLessThan(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.LessThan, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LessThanOrEqual' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LessThanOrEqual' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicLessThanOrEqual(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.LessThanOrEqual, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LessThanOrEqual' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LessThanOrEqual' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicLessThanOrEqual(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.LessThanOrEqual, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LessThanOrEqual' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LessThanOrEqual' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicLessThanOrEqual(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.LessThanOrEqual, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LessThanOrEqual' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LessThanOrEqual' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicLessThanOrEqual(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.LessThanOrEqual, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Modulo' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Modulo' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicModulo(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.Modulo, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Modulo' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Modulo' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicModulo(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.Modulo, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Modulo' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Modulo' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicModulo(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.Modulo, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Modulo' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Modulo' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicModulo(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.Modulo, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Multiply' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Multiply' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicMultiply(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.Multiply, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Multiply' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Multiply' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicMultiply(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.Multiply, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Multiply' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Multiply' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicMultiply(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.Multiply, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Multiply' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Multiply' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicMultiply(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.Multiply, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'MultiplyChecked' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'MultiplyChecked' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicMultiplyChecked(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.MultiplyChecked, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'MultiplyChecked' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'MultiplyChecked' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicMultiplyChecked(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.MultiplyChecked, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'MultiplyChecked' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'MultiplyChecked' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicMultiplyChecked(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.MultiplyChecked, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'MultiplyChecked' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'MultiplyChecked' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicMultiplyChecked(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.MultiplyChecked, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'NotEqual' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'NotEqual' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicNotEqual(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.NotEqual, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'NotEqual' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'NotEqual' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicNotEqual(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.NotEqual, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'NotEqual' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'NotEqual' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicNotEqual(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.NotEqual, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'NotEqual' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'NotEqual' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicNotEqual(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.NotEqual, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Or' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Or' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicOr(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.Or, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Or' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Or' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicOr(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.Or, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Or' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Or' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicOr(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.Or, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Or' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Or' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicOr(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.Or, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'OrElse' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'OrElse' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicOrElse(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.OrElse, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'OrElse' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'OrElse' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicOrElse(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.OrElse, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'OrElse' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'OrElse' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicOrElse(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.OrElse, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'OrElse' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'OrElse' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicOrElse(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.OrElse, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Power' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Power' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicPower(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.Power, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Power' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Power' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicPower(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.Power, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Power' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Power' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicPower(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.Power, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Power' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Power' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicPower(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.Power, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'RightShift' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'RightShift' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicRightShift(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.RightShift, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'RightShift' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'RightShift' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicRightShift(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.RightShift, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'RightShift' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'RightShift' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicRightShift(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.RightShift, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'RightShift' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'RightShift' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicRightShift(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.RightShift, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Subtract' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Subtract' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicSubtract(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.Subtract, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Subtract' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Subtract' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicSubtract(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.Subtract, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Subtract' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Subtract' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicSubtract(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.Subtract, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'Subtract' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'Subtract' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicSubtract(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.Subtract, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'SubtractChecked' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'SubtractChecked' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicSubtractChecked(Expression left, Expression right)
        {
            return MakeDynamicBinary(ExpressionType.SubtractChecked, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'SubtractChecked' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'SubtractChecked' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicSubtractChecked(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinary(ExpressionType.SubtractChecked, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'SubtractChecked' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'SubtractChecked' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicSubtractChecked(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinary(ExpressionType.SubtractChecked, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'SubtractChecked' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'SubtractChecked' operation.</returns>
        public static BinaryDynamicCSharpExpression DynamicSubtractChecked(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinary(ExpressionType.SubtractChecked, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Negate' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Negate' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicNegate(Expression operand)
        {
            return MakeDynamicUnary(ExpressionType.Negate, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Negate' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Negate' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicNegate(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnary(ExpressionType.Negate, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Negate' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Negate' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicNegate(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnary(ExpressionType.Negate, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Negate' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Negate' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicNegate(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnary(ExpressionType.Negate, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'UnaryPlus' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'UnaryPlus' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicUnaryPlus(Expression operand)
        {
            return MakeDynamicUnary(ExpressionType.UnaryPlus, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'UnaryPlus' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'UnaryPlus' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicUnaryPlus(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnary(ExpressionType.UnaryPlus, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'UnaryPlus' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'UnaryPlus' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicUnaryPlus(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnary(ExpressionType.UnaryPlus, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'UnaryPlus' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'UnaryPlus' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicUnaryPlus(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnary(ExpressionType.UnaryPlus, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'NegateChecked' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'NegateChecked' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicNegateChecked(Expression operand)
        {
            return MakeDynamicUnary(ExpressionType.NegateChecked, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'NegateChecked' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'NegateChecked' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicNegateChecked(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnary(ExpressionType.NegateChecked, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'NegateChecked' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'NegateChecked' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicNegateChecked(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnary(ExpressionType.NegateChecked, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'NegateChecked' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'NegateChecked' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicNegateChecked(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnary(ExpressionType.NegateChecked, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Not' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Not' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicNot(Expression operand)
        {
            return MakeDynamicUnary(ExpressionType.Not, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Not' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Not' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicNot(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnary(ExpressionType.Not, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Not' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Not' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicNot(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnary(ExpressionType.Not, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Not' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Not' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicNot(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnary(ExpressionType.Not, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Decrement' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Decrement' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicDecrement(Expression operand)
        {
            return MakeDynamicUnary(ExpressionType.Decrement, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Decrement' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Decrement' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicDecrement(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnary(ExpressionType.Decrement, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Decrement' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Decrement' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicDecrement(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnary(ExpressionType.Decrement, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Decrement' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Decrement' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicDecrement(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnary(ExpressionType.Decrement, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Increment' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Increment' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicIncrement(Expression operand)
        {
            return MakeDynamicUnary(ExpressionType.Increment, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Increment' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Increment' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicIncrement(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnary(ExpressionType.Increment, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Increment' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Increment' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicIncrement(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnary(ExpressionType.Increment, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'Increment' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'Increment' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicIncrement(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnary(ExpressionType.Increment, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'OnesComplement' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'OnesComplement' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicOnesComplement(Expression operand)
        {
            return MakeDynamicUnary(ExpressionType.OnesComplement, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'OnesComplement' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'OnesComplement' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicOnesComplement(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnary(ExpressionType.OnesComplement, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'OnesComplement' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'OnesComplement' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicOnesComplement(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnary(ExpressionType.OnesComplement, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'OnesComplement' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'OnesComplement' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicOnesComplement(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnary(ExpressionType.OnesComplement, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'IsTrue' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'IsTrue' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicIsTrue(Expression operand)
        {
            return MakeDynamicUnary(ExpressionType.IsTrue, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'IsTrue' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'IsTrue' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicIsTrue(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnary(ExpressionType.IsTrue, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'IsTrue' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'IsTrue' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicIsTrue(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnary(ExpressionType.IsTrue, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'IsTrue' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'IsTrue' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicIsTrue(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnary(ExpressionType.IsTrue, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'IsFalse' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'IsFalse' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicIsFalse(Expression operand)
        {
            return MakeDynamicUnary(ExpressionType.IsFalse, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'IsFalse' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'IsFalse' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicIsFalse(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnary(ExpressionType.IsFalse, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'IsFalse' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'IsFalse' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicIsFalse(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnary(ExpressionType.IsFalse, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'IsFalse' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'IsFalse' operation.</returns>
        public static UnaryDynamicCSharpExpression DynamicIsFalse(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnary(ExpressionType.IsFalse, operand, binderFlags, context);
        }

        private static void CheckBinary(ExpressionType binaryType)
        {
            switch (binaryType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    break;
                default:
                    throw LinqError.NotSupported();
            }
        }

        private static void CheckUnary(ExpressionType unaryType)
        {
            switch (unaryType)
            {
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Decrement:
                case ExpressionType.Increment:
                case ExpressionType.OnesComplement:
                case ExpressionType.IsTrue:
                case ExpressionType.IsFalse:
                    break;
                default:
                    throw LinqError.NotSupported();
            }
        }
    }
}