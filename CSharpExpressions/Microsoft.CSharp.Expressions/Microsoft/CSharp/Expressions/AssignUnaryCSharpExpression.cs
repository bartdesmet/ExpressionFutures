// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;

using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a unary assignment operation.
    /// </summary>
    public abstract partial class AssignUnaryCSharpExpression : UnaryCSharpExpression
    {
        internal AssignUnaryCSharpExpression(CSharpExpressionType unaryType, Expression operand)
            : base(operand)
        {
            CSharpNodeType = unaryType;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />.
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType { get; }

        /// <summary>
        /// Gets the static type of the expression.
        /// </summary>
        public override Type Type => Operand.Type;

        /// <summary>
        /// Gets the implementing method for the unary operation.
        /// </summary>
        /// <returns>The <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</returns>
        public abstract MethodInfo Method { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitUnaryAssign(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="operand">The <see cref="UnaryCSharpExpression.Operand" /> property of the result. </param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public AssignUnaryCSharpExpression Update(Expression operand)
        {
            if (operand == Operand)
            {
                return this;
            }

            return CSharpExpression.MakeUnaryAssign(CSharpNodeType, operand, Method);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var operand = MakeWriteable(Operand);

            var operandType = operand.Type;

            if (Method != null)
            {
                var functionalOp = new Func<Expression, Expression>(lhs =>
                {
                    return Expression.Call(Method, lhs);
                });

                return ReduceAssignment(operand, functionalOp, IsPrefix, null);
            }
            else if (IsCSharpSpecificUnaryAssignNumeric(operandType) || IsCheckedUnary)
            {
                // TODO: Move conversion notions to factory and support in expression translation of Roslyn.

                var isNullableOperandType = operandType.IsNullableType();
                var nonNullOperandType = operandType.GetNonNullableType();
                var intermediateType = nonNullOperandType.IsEnum ? nonNullOperandType.GetEnumUnderlyingType() : typeof(int);

                var operandParameter = Expression.Parameter(operandType, "__operand");
                var convertType = isNullableOperandType ? typeof(Nullable<>).MakeGenericType(intermediateType) : intermediateType;
                var convertOperand = IsCheckedUnary ? Expression.ConvertChecked(operandParameter, convertType) : Expression.Convert(operandParameter, convertType);
                var operandConversion = Expression.Lambda(convertOperand, operandParameter);

                var functionalOp = new Func<Expression, Expression>(lhs =>
                {
                    var res = FunctionalOp(lhs);

                    res = IsCheckedUnary ? Expression.ConvertChecked(res, operandType) : Expression.Convert(res, operandType);

                    return res;
                });

                return ReduceAssignment(operand, functionalOp, IsPrefix, operandConversion);
            }
            else
            {
                return ReduceAssignment(operand, FunctionalOp, IsPrefix);
            }
        }

        private bool IsPrefix
        {
            get
            {
                switch (CSharpNodeType)
                {
                    case CSharpExpressionType.PreIncrementAssign:
                    case CSharpExpressionType.PreDecrementAssign:
                    case CSharpExpressionType.PreIncrementAssignChecked:
                    case CSharpExpressionType.PreDecrementAssignChecked:
                        return true;
                }

                return false;
            }
        }

        private bool IsCheckedUnary
        {
            get
            {
                switch (CSharpNodeType)
                {
                    case CSharpExpressionType.PreIncrementAssignChecked:
                    case CSharpExpressionType.PreDecrementAssignChecked:
                    case CSharpExpressionType.PostIncrementAssignChecked:
                    case CSharpExpressionType.PostDecrementAssignChecked:
                        return true;
                }

                return false;
            }
        }

        private Expression FunctionalOp(Expression operand)
        {
            var one = GetConstantOne(operand.Type);

            switch (CSharpNodeType)
            {
                case CSharpExpressionType.PreIncrementAssignChecked:
                case CSharpExpressionType.PostIncrementAssignChecked:
                    return Expression.AddChecked(operand, one);

                case CSharpExpressionType.PreDecrementAssignChecked:
                case CSharpExpressionType.PostDecrementAssignChecked:
                    return Expression.SubtractChecked(operand, one);

                case CSharpExpressionType.PreIncrementAssign:
                case CSharpExpressionType.PostIncrementAssign:
                    return Expression.Add(operand, one);

                case CSharpExpressionType.PreDecrementAssign:
                case CSharpExpressionType.PostDecrementAssign:
                    return Expression.Subtract(operand, one);

                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private static Expression GetConstantOne(Type type) =>
            type.GetNonNullableType().GetTypeCode() switch
            {
                TypeCode.UInt16 => Expression.Constant((ushort)1, type),
                TypeCode.UInt32 => Expression.Constant((uint)1, type),
                TypeCode.UInt64 => Expression.Constant((ulong)1, type),
                TypeCode.Int16 => Expression.Constant((short)1, type),
                TypeCode.Int32 => Expression.Constant(1, type),// NB: We don't have a nullable cached instance
                TypeCode.Int64 => Expression.Constant((long)1, type),
                TypeCode.Single => Expression.Constant((float)1, type),
                TypeCode.Double => Expression.Constant((double)1, type),
                _ => throw ContractUtils.Unreachable,
            };

        internal static AssignUnaryCSharpExpression Make(CSharpExpressionType unaryType, Expression operand, MethodInfo method)
        {
            ValidateCustomUnaryAssign(unaryType, operand, ref method);

            // TODO: Add optimized layouts

            return new Custom(unaryType, operand, method);
        }

        private static void ValidateCustomUnaryAssign(CSharpExpressionType unaryType, Expression operand, ref MethodInfo method)
        {
            var operandType = operand.Type;

            // NB: Just leverage LINQ to do the dirty work to check everything.This could produce mysterious error
            //     messages. (TODO: Review what's most appropriate here.)

            var resultType = operandType;

            if (!IsCSharpSpecificUnaryAssignNumeric(operandType))
            {
                var operandDummy = Expression.Parameter(operandType, "__operand");
                var functionalOp = FunctionalOp(unaryType, operandDummy, method);

                method ??= functionalOp.Method;

                resultType = functionalOp.Type;
            }

            if (!AreEquivalent(resultType, operand.Type))
                throw Error.InvalidUnaryAssignmentWithOperands(unaryType, operand.Type);
        }

        private static UnaryExpression FunctionalOp(CSharpExpressionType unaryType, Expression operand, MethodInfo method)
        {
            switch (unaryType)
            {
                case CSharpExpressionType.PreDecrementAssign:
                case CSharpExpressionType.PreDecrementAssignChecked:
                    return Expression.PreDecrementAssign(operand, method);
                case CSharpExpressionType.PreIncrementAssign:
                case CSharpExpressionType.PreIncrementAssignChecked:
                    return Expression.PreIncrementAssign(operand, method);
                case CSharpExpressionType.PostDecrementAssign:
                case CSharpExpressionType.PostDecrementAssignChecked:
                    return Expression.PostDecrementAssign(operand, method);
                case CSharpExpressionType.PostIncrementAssign:
                case CSharpExpressionType.PostIncrementAssignChecked:
                    return Expression.PostIncrementAssign(operand, method);
            }

            throw LinqError.UnhandledUnary(unaryType);
        }

        internal sealed class Custom : AssignUnaryCSharpExpression
        {
            public Custom(CSharpExpressionType unaryType, Expression operand, MethodInfo method)
                : base(unaryType, operand)
            {
                Method = method;
            }

            public override MethodInfo Method { get; }
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an expression representing a unary assignment operation.
        /// </summary>
        /// <param name="unaryType">The type of assignment represented.</param>
        /// <param name="operand">The operand of the assignment operation, i.e. the assignment target.</param>
        /// <param name="method">The method implementing the assignment operation.</param>
        /// <returns>A new <see cref="AssignUnaryCSharpExpression"/> instance representing the unary assignment.</returns>
        public static AssignUnaryCSharpExpression MakeUnaryAssign(CSharpExpressionType unaryType, Expression operand, MethodInfo method) =>
            unaryType switch
            {
                CSharpExpressionType.PreIncrementAssign => PreIncrementAssign(operand, method),
                CSharpExpressionType.PreIncrementAssignChecked => PreIncrementAssignChecked(operand, method),
                CSharpExpressionType.PreDecrementAssign => PreDecrementAssign(operand, method),
                CSharpExpressionType.PreDecrementAssignChecked => PreDecrementAssignChecked(operand, method),
                CSharpExpressionType.PostIncrementAssign => PostIncrementAssign(operand, method),
                CSharpExpressionType.PostIncrementAssignChecked => PostIncrementAssignChecked(operand, method),
                CSharpExpressionType.PostDecrementAssign => PostDecrementAssign(operand, method),
                CSharpExpressionType.PostDecrementAssignChecked => PostDecrementAssignChecked(operand, method),
                _ => throw LinqError.UnhandledUnary(unaryType),
            };

        private static AssignUnaryCSharpExpression MakeUnaryAssignCore(CSharpExpressionType unaryType, Expression operand, MethodInfo method)
        {
            RequiresCanRead(operand, nameof(operand));
            Helpers.RequiresCanWrite(operand, nameof(operand));

            return AssignUnaryCSharpExpression.Make(unaryType, operand, method);
        }

        internal static bool IsCSharpSpecificUnaryAssignNumeric(Type type)
        {
            type = type.GetNonNullableType();

            if (!type.IsEnum)
            {
                switch (type.GetTypeCode())
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Char:
                        return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="AssignUnaryCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitUnaryAssign(AssignUnaryCSharpExpression node) =>
            node.Update(
                Visit(node.Operand)
            );
    }
}
