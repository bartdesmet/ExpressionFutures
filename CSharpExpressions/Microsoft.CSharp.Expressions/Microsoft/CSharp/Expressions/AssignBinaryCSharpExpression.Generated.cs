// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.CSharp.Expressions
{
	partial class AssignBinaryCSharpExpression
	{
        private static CSharpExpressionType ConvertNodeType(ExpressionType nodeType)
        {
			switch (nodeType)
			{
				case ExpressionType.Assign: return CSharpExpressionType.Assign;
				case ExpressionType.AddAssign: return CSharpExpressionType.AddAssign;
				case ExpressionType.SubtractAssign: return CSharpExpressionType.SubtractAssign;
				case ExpressionType.MultiplyAssign: return CSharpExpressionType.MultiplyAssign;
				case ExpressionType.DivideAssign: return CSharpExpressionType.DivideAssign;
				case ExpressionType.ModuloAssign: return CSharpExpressionType.ModuloAssign;
				case ExpressionType.AndAssign: return CSharpExpressionType.AndAssign;
				case ExpressionType.OrAssign: return CSharpExpressionType.OrAssign;
				case ExpressionType.ExclusiveOrAssign: return CSharpExpressionType.ExclusiveOrAssign;
				case ExpressionType.LeftShiftAssign: return CSharpExpressionType.LeftShiftAssign;
				case ExpressionType.RightShiftAssign: return CSharpExpressionType.RightShiftAssign;
				case ExpressionType.AddAssignChecked: return CSharpExpressionType.AddAssignChecked;
				case ExpressionType.MultiplyAssignChecked: return CSharpExpressionType.MultiplyAssignChecked;
				case ExpressionType.SubtractAssignChecked: return CSharpExpressionType.SubtractAssignChecked;
				default:
					throw ContractUtils.Unreachable;
			}
        }
	}

	partial class CSharpExpression
	{
		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents an assignment.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression Assign(Expression left, Expression right)
		{
			return MakeBinaryAssign((l, r, m, c) => Expression.Assign(l, r), left, right, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression AddAssign(Expression left, Expression right)
		{
			return AddAssign(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression AddAssign(Expression left, Expression right, MethodInfo method)
		{
			return AddAssign(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression AddAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.AddAssign, left, right, method, conversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression SubtractAssign(Expression left, Expression right)
		{
			return SubtractAssign(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression SubtractAssign(Expression left, Expression right, MethodInfo method)
		{
			return SubtractAssign(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression SubtractAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.SubtractAssign, left, right, method, conversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression MultiplyAssign(Expression left, Expression right)
		{
			return MultiplyAssign(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression MultiplyAssign(Expression left, Expression right, MethodInfo method)
		{
			return MultiplyAssign(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression MultiplyAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.MultiplyAssign, left, right, method, conversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type DivideAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression DivideAssign(Expression left, Expression right)
		{
			return DivideAssign(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type DivideAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression DivideAssign(Expression left, Expression right, MethodInfo method)
		{
			return DivideAssign(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type DivideAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression DivideAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.DivideAssign, left, right, method, conversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ModuloAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression ModuloAssign(Expression left, Expression right)
		{
			return ModuloAssign(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ModuloAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression ModuloAssign(Expression left, Expression right, MethodInfo method)
		{
			return ModuloAssign(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ModuloAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression ModuloAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.ModuloAssign, left, right, method, conversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AndAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression AndAssign(Expression left, Expression right)
		{
			return AndAssign(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AndAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression AndAssign(Expression left, Expression right, MethodInfo method)
		{
			return AndAssign(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AndAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression AndAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.AndAssign, left, right, method, conversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type OrAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression OrAssign(Expression left, Expression right)
		{
			return OrAssign(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type OrAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression OrAssign(Expression left, Expression right, MethodInfo method)
		{
			return OrAssign(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type OrAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression OrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.OrAssign, left, right, method, conversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ExclusiveOrAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression ExclusiveOrAssign(Expression left, Expression right)
		{
			return ExclusiveOrAssign(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ExclusiveOrAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method)
		{
			return ExclusiveOrAssign(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ExclusiveOrAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.ExclusiveOrAssign, left, right, method, conversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type LeftShiftAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression LeftShiftAssign(Expression left, Expression right)
		{
			return LeftShiftAssign(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type LeftShiftAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method)
		{
			return LeftShiftAssign(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type LeftShiftAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.LeftShiftAssign, left, right, method, conversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type RightShiftAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression RightShiftAssign(Expression left, Expression right)
		{
			return RightShiftAssign(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type RightShiftAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression RightShiftAssign(Expression left, Expression right, MethodInfo method)
		{
			return RightShiftAssign(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type RightShiftAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression RightShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.RightShiftAssign, left, right, method, conversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression AddAssignChecked(Expression left, Expression right)
		{
			return AddAssignChecked(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression AddAssignChecked(Expression left, Expression right, MethodInfo method)
		{
			return AddAssignChecked(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression AddAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.AddAssignChecked, left, right, method, conversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression MultiplyAssignChecked(Expression left, Expression right)
		{
			return MultiplyAssignChecked(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method)
		{
			return MultiplyAssignChecked(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.MultiplyAssignChecked, left, right, method, conversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression SubtractAssignChecked(Expression left, Expression right)
		{
			return SubtractAssignChecked(left, right);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method)
		{
			return SubtractAssignChecked(left, right, method, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.Conversion" /> property equal to.</param></param>
		public static new AssignBinaryCSharpExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MakeBinaryAssign(Expression.SubtractAssignChecked, left, right, method, conversion);
		}

	}
}