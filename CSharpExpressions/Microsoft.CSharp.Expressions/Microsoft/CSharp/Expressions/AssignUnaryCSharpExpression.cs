// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
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
        public Expression Update(Expression operand)
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
                if (Operand is IndexCSharpExpression)
                {
                    throw new NotImplementedException(); // TODO
                }

                return _expression;
            }
        }

        internal class Checked : AssignUnaryCSharpExpression
        {
            public Checked(CSharpExpressionType unaryType, Expression operand)
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
                        case CSharpExpressionType.PreIncrementCheckedAssign:
                        case CSharpExpressionType.PreDecrementCheckedAssign:
                            return true;
                    }

                    return false;
                }
            }

            private ExpressionType FunctionalOp
            {
                get
                {
                    switch (CSharpNodeType)
                    {
                        case CSharpExpressionType.PreIncrementCheckedAssign:
                        case CSharpExpressionType.PostIncrementCheckedAssign:
                            return ExpressionType.AddChecked;

                        case CSharpExpressionType.PreDecrementCheckedAssign:
                        case CSharpExpressionType.PostDecrementCheckedAssign:
                            return ExpressionType.SubtractChecked;

                        default:
                            throw ContractUtils.Unreachable;
                    }
                }
            }

            public override Expression Reduce()
            {
                switch (Operand.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        return ReduceMember();
                    case ExpressionType.Index:
                        return ReduceIndex();
                    case ExpressionType.Parameter:
                        return ReduceVariable();
                }

                if (Operand is IndexCSharpExpression)
                {
                    return ReduceIndexCSharp();
                }

                throw ContractUtils.Unreachable;
            }

            private Expression ReduceMember()
            {
                throw new NotImplementedException(); // TODO
            }

            private Expression ReduceIndex()
            {
                throw new NotImplementedException(); // TODO
            }

            private Expression ReduceVariable()
            {
                throw new NotImplementedException(); // TODO
            }

            private Expression ReduceIndexCSharp()
            {
                throw new NotImplementedException(); // TODO
            }
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

        private static AssignUnaryCSharpExpression MakeUnaryAssign(UnaryAssignFactory factory, Expression operand, MethodInfo method)
        {
            var lhs = GetLhs(operand);

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
            if (method != null)
            {
                return MakeUnaryAssign(factory, operand, method);
            }

            var lhs = GetLhs(operand);

            // NB: This has the side-effect of performing type checks
            factory(lhs, null);

            return new AssignUnaryCSharpExpression.Checked(unaryType, operand);
        }

        private static Expression GetLhs(Expression operand)
        {
            var lhs = operand;

            var index = operand as IndexCSharpExpression;
            if (index != null)
            {
                // TODO: check indexer has setter
                lhs = Expression.Parameter(operand.Type);
            }

            return lhs;
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
