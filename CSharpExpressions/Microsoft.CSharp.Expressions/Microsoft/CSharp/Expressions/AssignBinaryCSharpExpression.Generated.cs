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
        private static ExpressionType ConvertNodeType(CSharpExpressionType nodeType) =>
            // NB: Only used for ToCSharp pretty printing; maybe we can remove it?
            nodeType switch
            {
                CSharpExpressionType.Assign => ExpressionType.Assign,
                CSharpExpressionType.AddAssign => ExpressionType.AddAssign,
                CSharpExpressionType.SubtractAssign => ExpressionType.SubtractAssign,
                CSharpExpressionType.MultiplyAssign => ExpressionType.MultiplyAssign,
                CSharpExpressionType.DivideAssign => ExpressionType.DivideAssign,
                CSharpExpressionType.ModuloAssign => ExpressionType.ModuloAssign,
                CSharpExpressionType.AndAssign => ExpressionType.AndAssign,
                CSharpExpressionType.OrAssign => ExpressionType.OrAssign,
                CSharpExpressionType.ExclusiveOrAssign => ExpressionType.ExclusiveOrAssign,
                CSharpExpressionType.LeftShiftAssign => ExpressionType.LeftShiftAssign,
                CSharpExpressionType.RightShiftAssign => ExpressionType.RightShiftAssign,
                CSharpExpressionType.AddAssignChecked => ExpressionType.AddAssignChecked,
                CSharpExpressionType.MultiplyAssignChecked => ExpressionType.MultiplyAssignChecked,
                CSharpExpressionType.SubtractAssignChecked => ExpressionType.SubtractAssignChecked,
                CSharpExpressionType.NullCoalescingAssign => ExpressionType.Coalesce,
                _ => throw ContractUtils.Unreachable
            };
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents an assignment.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression Assign(Expression left, Expression right) =>
            MakeBinaryAssignCore(CSharpExpressionType.Assign, left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a null-coalescing assignment.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static AssignBinaryCSharpExpression NullCoalescingAssign(Expression left, Expression right) =>
            MakeBinaryAssignCore(CSharpExpressionType.NullCoalescingAssign, left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression AddAssign(Expression left, Expression right) =>
            AddAssign(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression AddAssign(Expression left, Expression right, MethodInfo method) =>
            AddAssign(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression AddAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            AddAssign(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression AddAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.AddAssign, left, right, method, finalConversion, leftConversion);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression SubtractAssign(Expression left, Expression right) =>
            SubtractAssign(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression SubtractAssign(Expression left, Expression right, MethodInfo method) =>
            SubtractAssign(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression SubtractAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            SubtractAssign(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression SubtractAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.SubtractAssign, left, right, method, finalConversion, leftConversion);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression MultiplyAssign(Expression left, Expression right) =>
            MultiplyAssign(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression MultiplyAssign(Expression left, Expression right, MethodInfo method) =>
            MultiplyAssign(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression MultiplyAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            MultiplyAssign(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression MultiplyAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.MultiplyAssign, left, right, method, finalConversion, leftConversion);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type DivideAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression DivideAssign(Expression left, Expression right) =>
            DivideAssign(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type DivideAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression DivideAssign(Expression left, Expression right, MethodInfo method) =>
            DivideAssign(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type DivideAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression DivideAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            DivideAssign(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type DivideAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression DivideAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.DivideAssign, left, right, method, finalConversion, leftConversion);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ModuloAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression ModuloAssign(Expression left, Expression right) =>
            ModuloAssign(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ModuloAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression ModuloAssign(Expression left, Expression right, MethodInfo method) =>
            ModuloAssign(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ModuloAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression ModuloAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            ModuloAssign(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ModuloAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression ModuloAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.ModuloAssign, left, right, method, finalConversion, leftConversion);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AndAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression AndAssign(Expression left, Expression right) =>
            AndAssign(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AndAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression AndAssign(Expression left, Expression right, MethodInfo method) =>
            AndAssign(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AndAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression AndAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            AndAssign(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AndAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression AndAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.AndAssign, left, right, method, finalConversion, leftConversion);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type OrAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression OrAssign(Expression left, Expression right) =>
            OrAssign(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type OrAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression OrAssign(Expression left, Expression right, MethodInfo method) =>
            OrAssign(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type OrAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression OrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            OrAssign(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type OrAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression OrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.OrAssign, left, right, method, finalConversion, leftConversion);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ExclusiveOrAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression ExclusiveOrAssign(Expression left, Expression right) =>
            ExclusiveOrAssign(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ExclusiveOrAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method) =>
            ExclusiveOrAssign(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ExclusiveOrAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            ExclusiveOrAssign(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type ExclusiveOrAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.ExclusiveOrAssign, left, right, method, finalConversion, leftConversion);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type LeftShiftAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression LeftShiftAssign(Expression left, Expression right) =>
            LeftShiftAssign(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type LeftShiftAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method) =>
            LeftShiftAssign(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type LeftShiftAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            LeftShiftAssign(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type LeftShiftAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.LeftShiftAssign, left, right, method, finalConversion, leftConversion);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type RightShiftAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression RightShiftAssign(Expression left, Expression right) =>
            RightShiftAssign(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type RightShiftAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression RightShiftAssign(Expression left, Expression right, MethodInfo method) =>
            RightShiftAssign(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type RightShiftAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression RightShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            RightShiftAssign(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type RightShiftAssign.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression RightShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.RightShiftAssign, left, right, method, finalConversion, leftConversion);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression AddAssignChecked(Expression left, Expression right) =>
            AddAssignChecked(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression AddAssignChecked(Expression left, Expression right, MethodInfo method) =>
            AddAssignChecked(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression AddAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            AddAssignChecked(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type AddAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression AddAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.AddAssignChecked, left, right, method, finalConversion, leftConversion);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression MultiplyAssignChecked(Expression left, Expression right) =>
            MultiplyAssignChecked(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method) =>
            MultiplyAssignChecked(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            MultiplyAssignChecked(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type MultiplyAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.MultiplyAssignChecked, left, right, method, finalConversion, leftConversion);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression SubtractAssignChecked(Expression left, Expression right) =>
            SubtractAssignChecked(left, right, method: null, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method) =>
            SubtractAssignChecked(left, right, method, finalConversion: null, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static new AssignBinaryCSharpExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion) =>
            SubtractAssignChecked(left, right, method, conversion, leftConversion: null);

        /// <summary>
        /// Creates a <see cref="AssignBinaryCSharpExpression" /> that represents a compound assignment of type SubtractAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignBinaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="left">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Left" /> property equal to.</param>
        /// <param name="right">An <see cref="Expression" /> to set the <see cref="BinaryCSharpExpression.Right" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignBinaryCSharpExpression.Method" /> property equal to.</param>
        /// <param name="finalConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.FinalConversion" /> property equal to.</param>
        /// <param name="leftConversion">A <see cref="LambdaExpression" /> to set the <see cref="AssignBinaryCSharpExpression.LeftConversion" /> property equal to.</param>
        public static AssignBinaryCSharpExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion) =>
            MakeBinaryAssignCore(CSharpExpressionType.SubtractAssignChecked, left, right, method, finalConversion, leftConversion);

    }
}
