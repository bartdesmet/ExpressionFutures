// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Linq.Expressions;
using System.Reflection;
using LinqError = System.Linq.Expressions.Error;
using static Microsoft.CSharp.Expressions.Helpers;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a binary assignment operation.
    /// </summary>
    public sealed partial class AssignBinaryCSharpExpression : BinaryCSharpExpression
    {
        private readonly BinaryExpression _expression;

        internal AssignBinaryCSharpExpression(BinaryExpression expression, Expression left)
            : base(left, expression.Right)
        {
            _expression = expression;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => ConvertNodeType(_expression.NodeType);

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => _expression.Type;

        /// <summary>
        /// Gets the implementing method for the binary operation.
        /// </summary>
		/// <returns>The <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</returns>
        public MethodInfo Method => _expression.Method;

        /// <summary>
        /// Gets the type conversion function that is used by a compound assignment operation.
        /// </summary>
		/// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that represents a type conversion function.</returns>
        public LambdaExpression Conversion => _expression.Conversion;

        /// <summary>
        /// Gets a value that indicates whether the expression tree node represents a lifted call to an operator.
        /// </summary>
		/// <returns>true if the node represents a lifted call; otherwise, false.</returns>
        public bool IsLifted => _expression.IsLifted;

        /// <summary>
        /// Gets a value that indicates whether the expression tree node represents a lifted call to an operator whose return type is lifted to a nullable type.
        /// </summary>
		/// <returns>true if the operator's return type is lifted to a nullable type; otherwise, false.</returns>
        public bool IsLiftedToNull => _expression.IsLiftedToNull;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitBinaryAssign(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="left">The <see cref="Left" /> property of the result. </param>
        /// <param name="conversion">The <see cref="Conversion" /> property of the result.</param>
        /// <param name="right">The <see cref="Right" /> property of the result. </param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public AssignBinaryCSharpExpression Update(Expression left, LambdaExpression conversion, Expression right)
        {
            if (left == base.Left && conversion == this.Conversion && right == base.Right)
            {
                return this;
            }

            return CSharpExpression.MakeBinaryAssign(CSharpNodeType, left, right, Method, conversion);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var index = Left as IndexCSharpExpression;
            if (index != null)
            {
                return index.ReduceAssign(left => _expression.Update(left, _expression.Conversion, _expression.Right));
            }

            return _expression;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an expression representing the binary assignment operation.
        /// </summary>
        /// <param name="binaryType">The type of assignment represented.</param>
        /// <param name="left">The left operand of the assignment operation, i.e. the assignment target.</param>
        /// <param name="right">The right operation of the assignment operation.</param>
        /// <param name="method">The method implementing the assignment operation.</param>
        /// <param name="conversion">The conversion function used by the compound assignment.</param>
        /// <returns>A new <see cref="AssignBinaryCSharpExpression"/> instance representing the binary assignment.</returns>
        public static AssignBinaryCSharpExpression MakeBinaryAssign(CSharpExpressionType binaryType, Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            switch (binaryType)
            {
                case CSharpExpressionType.Assign:
                    return Assign(left, right);
                case CSharpExpressionType.AddAssign:
                    return AddAssign(left, right, method, conversion);
                case CSharpExpressionType.AndAssign:
                    return AndAssign(left, right, method, conversion);
                case CSharpExpressionType.DivideAssign:
                    return DivideAssign(left, right, method, conversion);
                case CSharpExpressionType.ExclusiveOrAssign:
                    return ExclusiveOrAssign(left, right, method, conversion);
                case CSharpExpressionType.LeftShiftAssign:
                    return LeftShiftAssign(left, right, method, conversion);
                case CSharpExpressionType.ModuloAssign:
                    return ModuloAssign(left, right, method, conversion);
                case CSharpExpressionType.MultiplyAssign:
                    return MultiplyAssign(left, right, method, conversion);
                case CSharpExpressionType.OrAssign:
                    return OrAssign(left, right, method, conversion);
                case CSharpExpressionType.RightShiftAssign:
                    return RightShiftAssign(left, right, method, conversion);
                case CSharpExpressionType.SubtractAssign:
                    return SubtractAssign(left, right, method, conversion);
                case CSharpExpressionType.AddAssignChecked:
                    return AddAssignChecked(left, right, method, conversion);
                case CSharpExpressionType.MultiplyAssignChecked:
                    return MultiplyAssignChecked(left, right, method, conversion);
                case CSharpExpressionType.SubtractAssignChecked:
                    return SubtractAssignChecked(left, right, method, conversion);
            }

            throw LinqError.UnhandledBinary(binaryType);
        }

        private static AssignBinaryCSharpExpression MakeBinaryAssign(ExpressionType binaryType, BinaryAssignFactory factory, Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            var lhs = GetLhs(left, nameof(left));

            // NB: We could return a BinaryExpression in case the lhs is not one of our index nodes, but it'd change
            //     the return type to Expression which isn't nice to consume. Also, the Update method would either
            //     have to change to return Expression or we should have an AssignBinary node to hold a Binary node
            //     underneath it. This said, a specialized layout for the case where the custom node trivially wraps
            //     a LINQ node could be useful (just make Left virtual).

            if (binaryType == ExpressionType.AddAssign && left.Type == typeof(string) && method == null)
            {
                // NB: It looks like GetMethod supports contravariant parameters. E.g. if right.Type is object, the
                //     following returns the Concat(object, object) overload even though typeof(string) is specified.
                method = typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), right.Type });
            }

            var assign = factory(lhs, right, method, conversion);
            return new AssignBinaryCSharpExpression(assign, left);
        }

        delegate BinaryExpression BinaryAssignFactory(Expression left, Expression right, MethodInfo method, LambdaExpression conversion);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="AssignBinaryCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitBinaryAssign(AssignBinaryCSharpExpression node)
        {
            return node.Update(Visit(node.Left), VisitAndConvert(node.Conversion, nameof(VisitBinaryAssign)), Visit(node.Right));
        }
    }
}
