// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.CSharp.Expressions
{
	partial class AssignUnaryCSharpExpression
	{
        private static CSharpExpressionType ConvertNodeType(ExpressionType nodeType)
        {
			switch (nodeType)
			{
				case ExpressionType.PreIncrementAssign: return CSharpExpressionType.PreIncrementAssign;
				case ExpressionType.PreDecrementAssign: return CSharpExpressionType.PreDecrementAssign;
				case ExpressionType.PostIncrementAssign: return CSharpExpressionType.PostIncrementAssign;
				case ExpressionType.PostDecrementAssign: return CSharpExpressionType.PostDecrementAssign;
				default:
					throw ContractUtils.Unreachable;
			}
        }

		private static ExpressionType ConvertNodeType(CSharpExpressionType nodeType)
        {
			switch (nodeType)
			{
				case CSharpExpressionType.PreIncrementAssign: return ExpressionType.PreIncrementAssign;
				case CSharpExpressionType.PreIncrementCheckedAssign: return ExpressionType.PreIncrementAssign;
				case CSharpExpressionType.PreDecrementAssign: return ExpressionType.PreDecrementAssign;
				case CSharpExpressionType.PreDecrementCheckedAssign: return ExpressionType.PreDecrementAssign;
				case CSharpExpressionType.PostIncrementAssign: return ExpressionType.PostIncrementAssign;
				case CSharpExpressionType.PostIncrementCheckedAssign: return ExpressionType.PostIncrementAssign;
				case CSharpExpressionType.PostDecrementAssign: return ExpressionType.PostDecrementAssign;
				case CSharpExpressionType.PostDecrementCheckedAssign: return ExpressionType.PostDecrementAssign;
				default:
					throw ContractUtils.Unreachable;
			}
        }
	}

	partial class CSharpExpression
	{
		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreIncrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PreIncrementAssign(Expression operand)
		{
			return PreIncrementAssign(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreIncrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PreIncrementAssign(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssign(Expression.PreIncrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreDecrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PreDecrementAssign(Expression operand)
		{
			return PreDecrementAssign(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreDecrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PreDecrementAssign(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssign(Expression.PreDecrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostIncrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PostIncrementAssign(Expression operand)
		{
			return PostIncrementAssign(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostIncrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PostIncrementAssign(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssign(Expression.PostIncrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostDecrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PostDecrementAssign(Expression operand)
		{
			return PostDecrementAssign(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostDecrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PostDecrementAssign(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssign(Expression.PostDecrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreIncrementCheckedAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PreIncrementCheckedAssign(Expression operand)
		{
			return PreIncrementCheckedAssign(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreIncrementCheckedAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PreIncrementCheckedAssign(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssignChecked(CSharpExpressionType.PreIncrementCheckedAssign, Expression.PreIncrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreDecrementCheckedAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PreDecrementCheckedAssign(Expression operand)
		{
			return PreDecrementCheckedAssign(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreDecrementCheckedAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PreDecrementCheckedAssign(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssignChecked(CSharpExpressionType.PreDecrementCheckedAssign, Expression.PreDecrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostIncrementCheckedAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PostIncrementCheckedAssign(Expression operand)
		{
			return PostIncrementCheckedAssign(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostIncrementCheckedAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PostIncrementCheckedAssign(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssignChecked(CSharpExpressionType.PostIncrementCheckedAssign, Expression.PostIncrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostDecrementCheckedAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PostDecrementCheckedAssign(Expression operand)
		{
			return PostDecrementCheckedAssign(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostDecrementCheckedAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="AssignUnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PostDecrementCheckedAssign(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssignChecked(CSharpExpressionType.PostDecrementCheckedAssign, Expression.PostDecrementAssign, operand, method);
		}

	}
}