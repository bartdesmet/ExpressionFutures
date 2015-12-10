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

		private static ExpressionType ConvertNodeType(CSharpExpressionType nodeType)
        {
			switch (nodeType)
			{
				case CSharpExpressionType.Assign: return ExpressionType.Assign;
				case CSharpExpressionType.AddAssign: return ExpressionType.AddAssign;
				case CSharpExpressionType.SubtractAssign: return ExpressionType.SubtractAssign;
				case CSharpExpressionType.MultiplyAssign: return ExpressionType.MultiplyAssign;
				case CSharpExpressionType.DivideAssign: return ExpressionType.DivideAssign;
				case CSharpExpressionType.ModuloAssign: return ExpressionType.ModuloAssign;
				case CSharpExpressionType.AndAssign: return ExpressionType.AndAssign;
				case CSharpExpressionType.OrAssign: return ExpressionType.OrAssign;
				case CSharpExpressionType.ExclusiveOrAssign: return ExpressionType.ExclusiveOrAssign;
				case CSharpExpressionType.LeftShiftAssign: return ExpressionType.LeftShiftAssign;
				case CSharpExpressionType.RightShiftAssign: return ExpressionType.RightShiftAssign;
				case CSharpExpressionType.AddAssignChecked: return ExpressionType.AddAssignChecked;
				case CSharpExpressionType.MultiplyAssignChecked: return ExpressionType.MultiplyAssignChecked;
				case CSharpExpressionType.SubtractAssignChecked: return ExpressionType.SubtractAssignChecked;
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
			return MakeBinaryAssign(ExpressionType.Assign, (l, r, m, c) => Expression.Assign(l, r), left, right, null, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression AddAssign(Expression left, Expression right)
		{
			return AddAssign(left, right, null, null, null);
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
			return AddAssign(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression AddAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return AddAssign(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression AddAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.AddAssign, Expression.AddAssign, left, right, method, finalConversion, leftConversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression SubtractAssign(Expression left, Expression right)
		{
			return SubtractAssign(left, right, null, null, null);
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
			return SubtractAssign(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression SubtractAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return SubtractAssign(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression SubtractAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.SubtractAssign, Expression.SubtractAssign, left, right, method, finalConversion, leftConversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression MultiplyAssign(Expression left, Expression right)
		{
			return MultiplyAssign(left, right, null, null, null);
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
			return MultiplyAssign(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression MultiplyAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MultiplyAssign(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression MultiplyAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.MultiplyAssign, Expression.MultiplyAssign, left, right, method, finalConversion, leftConversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type DivideAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression DivideAssign(Expression left, Expression right)
		{
			return DivideAssign(left, right, null, null, null);
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
			return DivideAssign(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type DivideAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression DivideAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return DivideAssign(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type DivideAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression DivideAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.DivideAssign, Expression.DivideAssign, left, right, method, finalConversion, leftConversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ModuloAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression ModuloAssign(Expression left, Expression right)
		{
			return ModuloAssign(left, right, null, null, null);
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
			return ModuloAssign(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ModuloAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression ModuloAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return ModuloAssign(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ModuloAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression ModuloAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.ModuloAssign, Expression.ModuloAssign, left, right, method, finalConversion, leftConversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AndAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression AndAssign(Expression left, Expression right)
		{
			return AndAssign(left, right, null, null, null);
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
			return AndAssign(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AndAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression AndAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return AndAssign(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AndAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression AndAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.AndAssign, Expression.AndAssign, left, right, method, finalConversion, leftConversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type OrAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression OrAssign(Expression left, Expression right)
		{
			return OrAssign(left, right, null, null, null);
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
			return OrAssign(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type OrAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression OrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return OrAssign(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type OrAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression OrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.OrAssign, Expression.OrAssign, left, right, method, finalConversion, leftConversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ExclusiveOrAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression ExclusiveOrAssign(Expression left, Expression right)
		{
			return ExclusiveOrAssign(left, right, null, null, null);
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
			return ExclusiveOrAssign(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ExclusiveOrAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return ExclusiveOrAssign(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ExclusiveOrAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.ExclusiveOrAssign, Expression.ExclusiveOrAssign, left, right, method, finalConversion, leftConversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type LeftShiftAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression LeftShiftAssign(Expression left, Expression right)
		{
			return LeftShiftAssign(left, right, null, null, null);
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
			return LeftShiftAssign(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type LeftShiftAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return LeftShiftAssign(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type LeftShiftAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.LeftShiftAssign, Expression.LeftShiftAssign, left, right, method, finalConversion, leftConversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type RightShiftAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression RightShiftAssign(Expression left, Expression right)
		{
			return RightShiftAssign(left, right, null, null, null);
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
			return RightShiftAssign(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type RightShiftAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression RightShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return RightShiftAssign(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type RightShiftAssign.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression RightShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.RightShiftAssign, Expression.RightShiftAssign, left, right, method, finalConversion, leftConversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression AddAssignChecked(Expression left, Expression right)
		{
			return AddAssignChecked(left, right, null, null, null);
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
			return AddAssignChecked(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression AddAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return AddAssignChecked(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression AddAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.AddAssignChecked, Expression.AddAssignChecked, left, right, method, finalConversion, leftConversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression MultiplyAssignChecked(Expression left, Expression right)
		{
			return MultiplyAssignChecked(left, right, null, null, null);
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
			return MultiplyAssignChecked(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return MultiplyAssignChecked(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.MultiplyAssignChecked, Expression.MultiplyAssignChecked, left, right, method, finalConversion, leftConversion);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression SubtractAssignChecked(Expression left, Expression right)
		{
			return SubtractAssignChecked(left, right, null, null, null);
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
			return SubtractAssignChecked(left, right, method, null, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static new AssignBinaryCSharpExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			return SubtractAssignChecked(left, right, method, conversion, null);
		}

		/// <summary>
		/// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssignChecked.
		/// </summary>
		/// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
		/// <param name="left">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="Expression" /> to set the <see cref="AssignBinaryCSharpExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
		/// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
		/// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
		public static AssignBinaryCSharpExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
		{
			return MakeBinaryAssign(ExpressionType.SubtractAssignChecked, Expression.SubtractAssignChecked, left, right, method, finalConversion, leftConversion);
		}

	}
}