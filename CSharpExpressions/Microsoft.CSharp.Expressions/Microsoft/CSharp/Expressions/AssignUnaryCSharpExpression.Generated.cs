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
        private static ExpressionType ConvertNodeType(CSharpExpressionType nodeType) =>
            nodeType switch
            {
                CSharpExpressionType.PreIncrementAssign => ExpressionType.PreIncrementAssign,
                CSharpExpressionType.PreIncrementAssignChecked => ExpressionType.PreIncrementAssign,
                CSharpExpressionType.PreDecrementAssign => ExpressionType.PreDecrementAssign,
                CSharpExpressionType.PreDecrementAssignChecked => ExpressionType.PreDecrementAssign,
                CSharpExpressionType.PostIncrementAssign => ExpressionType.PostIncrementAssign,
                CSharpExpressionType.PostIncrementAssignChecked => ExpressionType.PostIncrementAssign,
                CSharpExpressionType.PostDecrementAssign => ExpressionType.PostDecrementAssign,
                CSharpExpressionType.PostDecrementAssignChecked => ExpressionType.PostDecrementAssign,
                _ => throw ContractUtils.Unreachable
            };
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreIncrementAssign.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        public static new AssignUnaryCSharpExpression PreIncrementAssign(Expression operand) =>
            PreIncrementAssign(operand, method: null);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreIncrementAssign.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignUnaryCSharpExpression PreIncrementAssign(Expression operand, MethodInfo method) =>
            MakeUnaryAssignCore(CSharpExpressionType.PreIncrementAssign, operand, method);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreDecrementAssign.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        public static new AssignUnaryCSharpExpression PreDecrementAssign(Expression operand) =>
            PreDecrementAssign(operand, method: null);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreDecrementAssign.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignUnaryCSharpExpression PreDecrementAssign(Expression operand, MethodInfo method) =>
            MakeUnaryAssignCore(CSharpExpressionType.PreDecrementAssign, operand, method);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostIncrementAssign.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        public static new AssignUnaryCSharpExpression PostIncrementAssign(Expression operand) =>
            PostIncrementAssign(operand, method: null);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostIncrementAssign.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignUnaryCSharpExpression PostIncrementAssign(Expression operand, MethodInfo method) =>
            MakeUnaryAssignCore(CSharpExpressionType.PostIncrementAssign, operand, method);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostDecrementAssign.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        public static new AssignUnaryCSharpExpression PostDecrementAssign(Expression operand) =>
            PostDecrementAssign(operand, method: null);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostDecrementAssign.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
        public static new AssignUnaryCSharpExpression PostDecrementAssign(Expression operand, MethodInfo method) =>
            MakeUnaryAssignCore(CSharpExpressionType.PostDecrementAssign, operand, method);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreIncrementAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        public static AssignUnaryCSharpExpression PreIncrementAssignChecked(Expression operand) =>
            PreIncrementAssignChecked(operand, method: null);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreIncrementAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
        public static AssignUnaryCSharpExpression PreIncrementAssignChecked(Expression operand, MethodInfo method) =>
            MakeUnaryAssignCore(CSharpExpressionType.PreIncrementAssignChecked, operand, method);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreDecrementAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        public static AssignUnaryCSharpExpression PreDecrementAssignChecked(Expression operand) =>
            PreDecrementAssignChecked(operand, method: null);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PreDecrementAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
        public static AssignUnaryCSharpExpression PreDecrementAssignChecked(Expression operand, MethodInfo method) =>
            MakeUnaryAssignCore(CSharpExpressionType.PreDecrementAssignChecked, operand, method);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostIncrementAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        public static AssignUnaryCSharpExpression PostIncrementAssignChecked(Expression operand) =>
            PostIncrementAssignChecked(operand, method: null);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostIncrementAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
        public static AssignUnaryCSharpExpression PostIncrementAssignChecked(Expression operand, MethodInfo method) =>
            MakeUnaryAssignCore(CSharpExpressionType.PostIncrementAssignChecked, operand, method);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostDecrementAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        public static AssignUnaryCSharpExpression PostDecrementAssignChecked(Expression operand) =>
            PostDecrementAssignChecked(operand, method: null);

        /// <summary>
        /// Creates a <see cref="AssignUnaryCSharpExpression" /> that represents a compound assignment of type PostDecrementAssignChecked.
        /// </summary>
        /// <returns>A <see cref="AssignUnaryCSharpExpression" /> that represents the operation.</returns>
        /// <param name="operand">An <see cref="Expression" /> to set the <see cref="UnaryCSharpExpression.Operand" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="AssignUnaryCSharpExpression.Method" /> property equal to.</param>
        public static AssignUnaryCSharpExpression PostDecrementAssignChecked(Expression operand, MethodInfo method) =>
            MakeUnaryAssignCore(CSharpExpressionType.PostDecrementAssignChecked, operand, method);

    }
}
