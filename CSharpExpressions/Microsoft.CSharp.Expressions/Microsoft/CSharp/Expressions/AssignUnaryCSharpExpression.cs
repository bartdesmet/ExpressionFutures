// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using static Microsoft.CSharp.Expressions.Helpers;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a unary assignment operation.
    /// </summary>
    public abstract partial class AssignUnaryCSharpExpression : UnaryCSharpExpression
    {
        internal AssignUnaryCSharpExpression(Expression operand)
            : base(operand)
        {
        }

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
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitUnaryAssign(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="operand">The <see cref="Operand" /> property of the result. </param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public AssignUnaryCSharpExpression Update(Expression operand)
        {
            if (operand == base.Operand)
            {
                return this;
            }

            return CSharpExpression.MakeUnaryAssign(CSharpNodeType, operand, Method);
        }

        internal class Unchecked : AssignUnaryCSharpExpression
        {
            private readonly UnaryExpression _expression;

            public Unchecked(UnaryExpression expression, Expression operand)
                : base(operand)
            {
                _expression = expression;
            }

            public override CSharpExpressionType CSharpNodeType => ConvertNodeType(_expression.NodeType);
            public override Type Type => _expression.Type;
            public override MethodInfo Method => _expression.Method;

            public override Expression Reduce()
            {
                var index = Operand as IndexCSharpExpression;
                if (index != null)
                {
                    return index.ReduceAssign(operand => _expression.Update(operand));
                }

                return _expression;
            }
        }

        internal class UncheckedWithNodeType : Unchecked
        {
            public UncheckedWithNodeType(UnaryExpression expression, Expression operand, CSharpExpressionType nodeType)
                : base(expression, operand)
            {
                CSharpNodeType = nodeType;
            }

            public override CSharpExpressionType CSharpNodeType { get; }
        }

        internal class Custom : AssignUnaryCSharpExpression
        {
            public Custom(CSharpExpressionType unaryType, Expression operand)
                : base(operand)
            {
                CSharpNodeType = unaryType;
            }

            public override CSharpExpressionType CSharpNodeType { get; }
            public override Type Type => Operand.Type; // NB: those operations don't change the operand type
            public override MethodInfo Method => null; // NB: if a method was specified, it became Unchecked

            private bool IsPrefix
            {
                get
                {
                    switch (CSharpNodeType)
                    {
                        case CSharpExpressionType.PreIncrementAssign:
                        case CSharpExpressionType.PreDecrementAssign:
                        case CSharpExpressionType.PreIncrementCheckedAssign:
                        case CSharpExpressionType.PreDecrementCheckedAssign:
                            return true;
                    }

                    return false;
                }
            }

            private bool IsChecked
            {
                get
                {
                    switch (CSharpNodeType)
                    {
                        case CSharpExpressionType.PreIncrementCheckedAssign:
                        case CSharpExpressionType.PreDecrementCheckedAssign:
                        case CSharpExpressionType.PostIncrementCheckedAssign:
                        case CSharpExpressionType.PostDecrementCheckedAssign:
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
                    case CSharpExpressionType.PreIncrementCheckedAssign:
                    case CSharpExpressionType.PostIncrementCheckedAssign:
                        return Expression.AddChecked(operand, one);

                    case CSharpExpressionType.PreDecrementCheckedAssign:
                    case CSharpExpressionType.PostDecrementCheckedAssign:
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

            private Expression GetConstantOne(Type type)
            {
                switch (type.GetNonNullableType().GetTypeCode())
                {
                    case TypeCode.UInt16:
                        return Expression.Constant((ushort)1, type);
                    case TypeCode.UInt32:
                        return Expression.Constant((uint)1, type);
                    case TypeCode.UInt64:
                        return Expression.Constant((ulong)1, type);
                    case TypeCode.Int16:
                        return Expression.Constant((short)1, type);
                    case TypeCode.Int32:
                        return Helpers.CreateConstantInt32(1);
                    case TypeCode.Int64:
                        return Expression.Constant((long)1, type);
                    case TypeCode.Single:
                        return Expression.Constant((float)1, type);
                    case TypeCode.Double:
                        return Expression.Constant((double)1, type);
                    default:
                        throw ContractUtils.Unreachable;
                }
            }

            public override Expression Reduce()
            {
                var operandType = Operand.Type;

                if (IsCSharpSpecificUnaryAssignNumeric(operandType))
                {
                    var isNullableOperandType = operandType.IsNullableType();
                    var nonNullOperandType = operandType.GetNonNullableType();
                    var intermediateType = nonNullOperandType.IsEnum ? nonNullOperandType.GetEnumUnderlyingType() : typeof(int);

                    var operandParameter = Expression.Parameter(operandType, "__operand");
                    var convertType = isNullableOperandType ? typeof(Nullable<>).MakeGenericType(intermediateType) : intermediateType;
                    var convertOperand = IsChecked ? Expression.ConvertChecked(operandParameter, convertType) : Expression.Convert(operandParameter, convertType);
                    var operandConversion = Expression.Lambda(convertOperand, operandParameter);

                    var functionalOp = new Func<Expression, Expression>(lhs =>
                    {
                        var res = FunctionalOp(lhs);

                        res = IsChecked ? Expression.ConvertChecked(res, operandType) : Expression.Convert(res, operandType);

                        return res;
                    });

                    return ReduceAssignment(Operand, functionalOp, IsPrefix, operandConversion);
                }
                else
                {
                    return ReduceAssignment(Operand, FunctionalOp, IsPrefix);
                }
            }
        }

        internal static AssignUnaryCSharpExpression Make(CSharpExpressionType unaryType, Expression operand)
        {
            RequiresCanRead(operand, nameof(operand));
            Helpers.RequiresCanWrite(operand, nameof(operand));

            return new Custom(unaryType, operand);
        }
    }

    partial class CSharpExpression
    {
        public static AssignUnaryCSharpExpression MakeUnaryAssign(CSharpExpressionType unaryType, Expression operand, MethodInfo method)
        {
            switch (unaryType)
            {
                case CSharpExpressionType.PreIncrementAssign:
                    return PreIncrementAssign(operand, method);
                case CSharpExpressionType.PreIncrementCheckedAssign:
                    return PreIncrementCheckedAssign(operand, method);
                case CSharpExpressionType.PreDecrementAssign:
                    return PreDecrementAssign(operand, method);
                case CSharpExpressionType.PreDecrementCheckedAssign:
                    return PreDecrementCheckedAssign(operand, method);
                case CSharpExpressionType.PostIncrementAssign:
                    return PostIncrementAssign(operand, method);
                case CSharpExpressionType.PostIncrementCheckedAssign:
                    return PostIncrementCheckedAssign(operand, method);
                case CSharpExpressionType.PostDecrementAssign:
                    return PostDecrementAssign(operand, method);
                case CSharpExpressionType.PostDecrementCheckedAssign:
                    return PostDecrementCheckedAssign(operand, method);
            }

            throw LinqError.UnhandledUnary(unaryType);
        }

        private static AssignUnaryCSharpExpression MakeUnaryAssign(CSharpExpressionType unaryType, UnaryAssignFactory factory, Expression operand, MethodInfo method)
        {
            if (IsCSharpSpecificUnaryAssignNumeric(operand.Type) && method == null)
            {
                return AssignUnaryCSharpExpression.Make(unaryType, operand);
            }

            var lhs = GetLhs(operand, nameof(operand));

            // NB: We could return a UnaryExpression in case the lhs is not one of our index nodes, but it'd change
            //     the return type to Expression which isn't nice to consume. Also, the Update method would either
            //     have to change to return Expression or we should have an AssignUnary node to hold a Unary node
            //     underneath it. This said, a specialized layout for the case where the custom node trivially wraps
            //     a LINQ node could be useful (just make Operand virtual).

            var assign = factory(lhs, method);
            return new AssignUnaryCSharpExpression.Unchecked(assign, operand);
        }

        private static AssignUnaryCSharpExpression MakeUnaryAssignChecked(CSharpExpressionType unaryType, UnaryAssignFactory factory, Expression operand, MethodInfo method)
        {
            if (IsCSharpSpecificUnaryAssignNumeric(operand.Type) && method == null)
            {
                return AssignUnaryCSharpExpression.Make(unaryType, operand);
            }

            var lhs = GetLhs(operand, nameof(operand));
            var assign = factory(lhs, method);

            if (method != null)
            {
                // NB: If a method is specified, the underlying operation won't be checked, but we still need
                //     to surface the original node type, so we have a little special node to do that.
                return new AssignUnaryCSharpExpression.UncheckedWithNodeType(assign, operand, unaryType);
            }
            else
            {
                return new AssignUnaryCSharpExpression.Custom(unaryType, operand);
            }
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

        delegate UnaryExpression UnaryAssignFactory(Expression operand, MethodInfo method);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="AssignUnaryCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitUnaryAssign(AssignUnaryCSharpExpression node)
        {
            return node.Update(Visit(node.Operand));
        }
    }
}
