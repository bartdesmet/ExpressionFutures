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
				case CSharpExpressionType.PreIncrementAssignChecked: return ExpressionType.PreIncrementAssign;
				case CSharpExpressionType.PreDecrementAssign: return ExpressionType.PreDecrementAssign;
				case CSharpExpressionType.PreDecrementAssignChecked: return ExpressionType.PreDecrementAssign;
				case CSharpExpressionType.PostIncrementAssign: return ExpressionType.PostIncrementAssign;
				case CSharpExpressionType.PostIncrementAssignChecked: return ExpressionType.PostIncrementAssign;
				case CSharpExpressionType.PostDecrementAssign: return ExpressionType.PostDecrementAssign;
				case CSharpExpressionType.PostDecrementAssignChecked: return ExpressionType.PostDecrementAssign;
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
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PreIncrementAssign(Expression operand)
		{
			return PreIncrementAssign(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreIncrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PreIncrementAssign(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssign(CSharpExpressionType.PreIncrementAssign, Expression.PreIncrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreDecrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PreDecrementAssign(Expression operand)
		{
			return PreDecrementAssign(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreDecrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PreDecrementAssign(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssign(CSharpExpressionType.PreDecrementAssign, Expression.PreDecrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostIncrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PostIncrementAssign(Expression operand)
		{
			return PostIncrementAssign(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostIncrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PostIncrementAssign(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssign(CSharpExpressionType.PostIncrementAssign, Expression.PostIncrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostDecrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PostDecrementAssign(Expression operand)
		{
			return PostDecrementAssign(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostDecrementAssign.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignUnaryCSharpExpression PostDecrementAssign(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssign(CSharpExpressionType.PostDecrementAssign, Expression.PostDecrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreIncrementAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PreIncrementAssignChecked(Expression operand)
		{
			return PreIncrementAssignChecked(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreIncrementAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PreIncrementAssignChecked(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssignChecked(CSharpExpressionType.PreIncrementAssignChecked, Expression.PreIncrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreDecrementAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PreDecrementAssignChecked(Expression operand)
		{
			return PreDecrementAssignChecked(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreDecrementAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PreDecrementAssignChecked(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssignChecked(CSharpExpressionType.PreDecrementAssignChecked, Expression.PreDecrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostIncrementAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PostIncrementAssignChecked(Expression operand)
		{
			return PostIncrementAssignChecked(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostIncrementAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PostIncrementAssignChecked(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssignChecked(CSharpExpressionType.PostIncrementAssignChecked, Expression.PostIncrementAssign, operand, method);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostDecrementAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PostDecrementAssignChecked(Expression operand)
		{
			return PostDecrementAssignChecked(operand, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostDecrementAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
		public static AssignUnaryCSharpExpression PostDecrementAssignChecked(Expression operand, MethodInfo method)
		{
			return MakeUnaryAssignChecked(CSharpExpressionType.PostDecrementAssignChecked, Expression.PostDecrementAssign, operand, method);
		}

	}
}