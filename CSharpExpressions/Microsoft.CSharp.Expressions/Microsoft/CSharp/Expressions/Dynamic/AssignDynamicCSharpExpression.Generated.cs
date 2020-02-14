// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Linq.Expressions;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AddAssign' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AddAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAddAssign(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.AddAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AddAssign' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AddAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAddAssign(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.AddAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AddAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AddAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicAddAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.AddAssign, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AddAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AddAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicAddAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.AddAssign, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AndAssign' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AndAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAndAssign(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.AndAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AndAssign' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AndAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAndAssign(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.AndAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AndAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AndAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicAndAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.AndAssign, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AndAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AndAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicAndAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.AndAssign, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'DivideAssign' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'DivideAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicDivideAssign(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.DivideAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'DivideAssign' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'DivideAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicDivideAssign(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.DivideAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'DivideAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'DivideAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicDivideAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.DivideAssign, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'DivideAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'DivideAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicDivideAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.DivideAssign, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ExclusiveOrAssign' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ExclusiveOrAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicExclusiveOrAssign(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.ExclusiveOrAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ExclusiveOrAssign' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ExclusiveOrAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicExclusiveOrAssign(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.ExclusiveOrAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ExclusiveOrAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ExclusiveOrAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicExclusiveOrAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.ExclusiveOrAssign, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ExclusiveOrAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ExclusiveOrAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicExclusiveOrAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.ExclusiveOrAssign, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LeftShiftAssign' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LeftShiftAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicLeftShiftAssign(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.LeftShiftAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LeftShiftAssign' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LeftShiftAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicLeftShiftAssign(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.LeftShiftAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LeftShiftAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LeftShiftAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicLeftShiftAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.LeftShiftAssign, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'LeftShiftAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'LeftShiftAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicLeftShiftAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.LeftShiftAssign, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ModuloAssign' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ModuloAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicModuloAssign(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.ModuloAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ModuloAssign' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ModuloAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicModuloAssign(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.ModuloAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ModuloAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ModuloAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicModuloAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.ModuloAssign, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'ModuloAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'ModuloAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicModuloAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.ModuloAssign, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'MultiplyAssign' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'MultiplyAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicMultiplyAssign(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.MultiplyAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'MultiplyAssign' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'MultiplyAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicMultiplyAssign(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.MultiplyAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'MultiplyAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'MultiplyAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicMultiplyAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.MultiplyAssign, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'MultiplyAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'MultiplyAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicMultiplyAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.MultiplyAssign, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'OrAssign' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'OrAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicOrAssign(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.OrAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'OrAssign' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'OrAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicOrAssign(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.OrAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'OrAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'OrAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicOrAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.OrAssign, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'OrAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'OrAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicOrAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.OrAssign, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'RightShiftAssign' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'RightShiftAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicRightShiftAssign(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.RightShiftAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'RightShiftAssign' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'RightShiftAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicRightShiftAssign(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.RightShiftAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'RightShiftAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'RightShiftAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicRightShiftAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.RightShiftAssign, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'RightShiftAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'RightShiftAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicRightShiftAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.RightShiftAssign, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'SubtractAssign' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'SubtractAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicSubtractAssign(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.SubtractAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'SubtractAssign' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'SubtractAssign' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicSubtractAssign(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.SubtractAssign, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'SubtractAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'SubtractAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicSubtractAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.SubtractAssign, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'SubtractAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'SubtractAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicSubtractAssign(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.SubtractAssign, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AddAssignChecked' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AddAssignChecked' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAddAssignChecked(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.AddAssignChecked, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AddAssignChecked' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AddAssignChecked' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicAddAssignChecked(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.AddAssignChecked, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AddAssignChecked' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AddAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicAddAssignChecked(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.AddAssignChecked, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'AddAssignChecked' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'AddAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicAddAssignChecked(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.AddAssignChecked, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'MultiplyAssignChecked' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'MultiplyAssignChecked' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicMultiplyAssignChecked(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.MultiplyAssignChecked, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'MultiplyAssignChecked' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'MultiplyAssignChecked' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicMultiplyAssignChecked(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.MultiplyAssignChecked, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'MultiplyAssignChecked' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'MultiplyAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicMultiplyAssignChecked(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.MultiplyAssignChecked, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'MultiplyAssignChecked' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'MultiplyAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicMultiplyAssignChecked(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.MultiplyAssignChecked, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'SubtractAssignChecked' operation.
        /// </summary>
        /// <param name="left">The expression representing the left operand of the operation.</param>
        /// <param name="right">The expression representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'SubtractAssignChecked' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicSubtractAssignChecked(Expression left, Expression right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.SubtractAssignChecked, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'SubtractAssignChecked' operation.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'SubtractAssignChecked' operation.</returns>
        public static AssignBinaryDynamicCSharpExpression DynamicSubtractAssignChecked(DynamicCSharpArgument left, DynamicCSharpArgument right)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.SubtractAssignChecked, left, right);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'SubtractAssignChecked' operation with the specified binder flags.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound binary 'SubtractAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicSubtractAssignChecked(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.SubtractAssignChecked, left, right, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound binary 'SubtractAssignChecked' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="left">The dynamic argument representing the left operand of the operation.</param>
        /// <param name="right">The dynamic argument representing the right operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound binary 'SubtractAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignBinaryDynamicCSharpExpression DynamicSubtractAssignChecked(DynamicCSharpArgument left, DynamicCSharpArgument right, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicBinaryAssign(CSharpExpressionType.SubtractAssignChecked, left, right, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreIncrementAssign' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreIncrementAssign' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPreIncrementAssign(Expression operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreIncrementAssign, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreIncrementAssign' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreIncrementAssign' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPreIncrementAssign(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreIncrementAssign, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreIncrementAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreIncrementAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPreIncrementAssign(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreIncrementAssign, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreIncrementAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreIncrementAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPreIncrementAssign(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreIncrementAssign, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreDecrementAssign' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreDecrementAssign' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPreDecrementAssign(Expression operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreDecrementAssign, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreDecrementAssign' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreDecrementAssign' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPreDecrementAssign(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreDecrementAssign, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreDecrementAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreDecrementAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPreDecrementAssign(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreDecrementAssign, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreDecrementAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreDecrementAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPreDecrementAssign(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreDecrementAssign, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostIncrementAssign' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostIncrementAssign' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPostIncrementAssign(Expression operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostIncrementAssign, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostIncrementAssign' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostIncrementAssign' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPostIncrementAssign(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostIncrementAssign, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostIncrementAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostIncrementAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPostIncrementAssign(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostIncrementAssign, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostIncrementAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostIncrementAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPostIncrementAssign(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostIncrementAssign, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostDecrementAssign' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostDecrementAssign' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPostDecrementAssign(Expression operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostDecrementAssign, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostDecrementAssign' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostDecrementAssign' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPostDecrementAssign(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostDecrementAssign, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostDecrementAssign' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostDecrementAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPostDecrementAssign(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostDecrementAssign, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostDecrementAssign' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostDecrementAssign' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPostDecrementAssign(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostDecrementAssign, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreIncrementAssignChecked' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreIncrementAssignChecked' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPreIncrementAssignChecked(Expression operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreIncrementAssignChecked, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreIncrementAssignChecked' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreIncrementAssignChecked' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPreIncrementAssignChecked(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreIncrementAssignChecked, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreIncrementAssignChecked' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreIncrementAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPreIncrementAssignChecked(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreIncrementAssignChecked, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreIncrementAssignChecked' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreIncrementAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPreIncrementAssignChecked(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreIncrementAssignChecked, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreDecrementAssignChecked' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreDecrementAssignChecked' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPreDecrementAssignChecked(Expression operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreDecrementAssignChecked, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreDecrementAssignChecked' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreDecrementAssignChecked' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPreDecrementAssignChecked(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreDecrementAssignChecked, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreDecrementAssignChecked' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreDecrementAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPreDecrementAssignChecked(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreDecrementAssignChecked, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PreDecrementAssignChecked' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PreDecrementAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPreDecrementAssignChecked(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PreDecrementAssignChecked, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostIncrementAssignChecked' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostIncrementAssignChecked' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPostIncrementAssignChecked(Expression operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostIncrementAssignChecked, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostIncrementAssignChecked' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostIncrementAssignChecked' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPostIncrementAssignChecked(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostIncrementAssignChecked, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostIncrementAssignChecked' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostIncrementAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPostIncrementAssignChecked(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostIncrementAssignChecked, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostIncrementAssignChecked' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostIncrementAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPostIncrementAssignChecked(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostIncrementAssignChecked, operand, binderFlags, context);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostDecrementAssignChecked' operation.
        /// </summary>
        /// <param name="operand">The expression representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostDecrementAssignChecked' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPostDecrementAssignChecked(Expression operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostDecrementAssignChecked, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostDecrementAssignChecked' operation.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostDecrementAssignChecked' operation.</returns>
        public static AssignUnaryDynamicCSharpExpression DynamicPostDecrementAssignChecked(DynamicCSharpArgument operand)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostDecrementAssignChecked, operand);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostDecrementAssignChecked' operation with the specified binder flags.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostDecrementAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPostDecrementAssignChecked(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostDecrementAssignChecked, operand, binderFlags);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound unary 'PostDecrementAssignChecked' operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="operand">The dynamic argument representing the operand of the operation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound unary 'PostDecrementAssignChecked' operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Inherited from the type name.")]
        public static AssignUnaryDynamicCSharpExpression DynamicPostDecrementAssignChecked(DynamicCSharpArgument operand, CSharpBinderFlags binderFlags, Type context)
        {
            return MakeDynamicUnaryAssign(CSharpExpressionType.PostDecrementAssignChecked, operand, binderFlags, context);
        }

        private static void CheckBinaryAssign(CSharpExpressionType binaryType)
        {
            switch (binaryType)
            {
                case CSharpExpressionType.Assign:
                case CSharpExpressionType.NullCoalescingAssign:
                case CSharpExpressionType.AddAssign:
                case CSharpExpressionType.AndAssign:
                case CSharpExpressionType.DivideAssign:
                case CSharpExpressionType.ExclusiveOrAssign:
                case CSharpExpressionType.LeftShiftAssign:
                case CSharpExpressionType.ModuloAssign:
                case CSharpExpressionType.MultiplyAssign:
                case CSharpExpressionType.OrAssign:
                case CSharpExpressionType.RightShiftAssign:
                case CSharpExpressionType.SubtractAssign:
                case CSharpExpressionType.AddAssignChecked:
                case CSharpExpressionType.MultiplyAssignChecked:
                case CSharpExpressionType.SubtractAssignChecked:
                    break;
                default:
                    throw LinqError.NotSupported();
            }
        }

        private static void CheckUnaryAssign(CSharpExpressionType unaryType)
        {
            switch (unaryType)
            {
                case CSharpExpressionType.PreIncrementAssign:
                case CSharpExpressionType.PreDecrementAssign:
                case CSharpExpressionType.PostIncrementAssign:
                case CSharpExpressionType.PostDecrementAssign:
                case CSharpExpressionType.PreIncrementAssignChecked:
                case CSharpExpressionType.PreDecrementAssignChecked:
                case CSharpExpressionType.PostIncrementAssignChecked:
                case CSharpExpressionType.PostDecrementAssignChecked:
                    break;
                default:
                    throw LinqError.NotSupported();
            }
        }
    }
}