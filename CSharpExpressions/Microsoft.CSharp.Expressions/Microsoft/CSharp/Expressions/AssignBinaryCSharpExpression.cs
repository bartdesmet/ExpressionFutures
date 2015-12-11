// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using static Microsoft.CSharp.Expressions.Helpers;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    // DESIGN: Does FinalConversion leak too many C# details that are hard to make sense of by the user?
    //         For one thing, the node is losely mirrored to the BoundNodes in Roslyn, which could be the
    //         strategy for expression tree APIs going forward. Also, 7.17.2 is pretty clear about the use
    //         of a conversion in the expansion of the compound assignment.

    /// <summary>
    /// Represents a binary assignment operation.
    /// </summary>
    public abstract partial class AssignBinaryCSharpExpression : BinaryCSharpExpression
    {
        private AssignBinaryCSharpExpression(Expression left, Expression right)
            : base(left, right)
        {
        }

        /// <summary>
        /// Gets the implementing method for the binary operation.
        /// </summary>
        /// <returns>The <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</returns>
        public abstract MethodInfo Method { get; }

        /// <summary>
        /// Gets the type conversion function that is used to convert the left hand side of the compound assignment operation prior to use by the underlying operation.
        /// </summary>
        /// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that represents a type conversion function.</returns>
        public abstract LambdaExpression LeftConversion { get; }

        /// <summary>
        /// Gets the type conversion function that is used to convert the result of the underlying operation prior to assignment to the left hand side of the compound assignment operation.
        /// </summary>
        /// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that represents a type conversion function.</returns>
        public abstract LambdaExpression FinalConversion { get; }

        /// <summary>
        /// Gets a value that indicates whether the expression tree node represents a lifted call to an operator.
        /// </summary>
        /// <returns>true if the node represents a lifted call; otherwise, false.</returns>
        public abstract bool IsLifted { get; }

        /// <summary>
        /// Gets a value that indicates whether the expression tree node represents a lifted call to an operator whose return type is lifted to a nullable type.
        /// </summary>
        /// <returns>true if the operator's return type is lifted to a nullable type; otherwise, false.</returns>
        public abstract bool IsLiftedToNull { get; }

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
        /// <param name="leftConversion">The <see cref="LeftConversion" /> property of the result.</param>
        /// <param name="right">The <see cref="Right" /> property of the result. </param>
        /// <param name="finalConversion">The <see cref="FinalConversion" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public AssignBinaryCSharpExpression Update(Expression left, LambdaExpression leftConversion, Expression right, LambdaExpression finalConversion)
        {
            if (left == base.Left && leftConversion == this.LeftConversion && right == base.Right && finalConversion == this.FinalConversion)
            {
                return this;
            }

            return CSharpExpression.MakeBinaryAssign(CSharpNodeType, left, right, Method, finalConversion, leftConversion);
        }

        internal class Primitive : AssignBinaryCSharpExpression
        {
            private readonly BinaryExpression _expression;

            internal Primitive(BinaryExpression expression, Expression left)
                : base(left, expression.Right)
            {
                _expression = expression;
            }

            public override CSharpExpressionType CSharpNodeType => ConvertNodeType(_expression.NodeType);
            public override Type Type => _expression.Type;

            public override MethodInfo Method => _expression.Method;
            public override LambdaExpression LeftConversion => null;
            public override LambdaExpression FinalConversion => _expression.Conversion;
            public override bool IsLifted => _expression.IsLifted;
            public override bool IsLiftedToNull => _expression.IsLiftedToNull;

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

        internal class WithConversions : AssignBinaryCSharpExpression
        {
            private readonly ExpressionType _binaryType;

            internal WithConversions(ExpressionType binaryType, Expression left, Expression right, MethodInfo method, LambdaExpression leftConversion, LambdaExpression finalConversion)
                : base(left, right)
            {
                _binaryType = binaryType;
                Method = method;
                LeftConversion = leftConversion;
                FinalConversion = finalConversion;
            }

            public override CSharpExpressionType CSharpNodeType => ConvertNodeType(_binaryType);
            public override Type Type => Left.Type;

            public override MethodInfo Method { get; }
            public override LambdaExpression LeftConversion { get; }
            public override LambdaExpression FinalConversion { get; }
            public override bool IsLifted
            {
                get
                {
                    // NB: Same logic as LINQ's BinaryExpression, modulo the absence of Assign and Coalesce cases.
                    if (Left.Type.IsNullableType())
                    {
                        return Method == null || !TypeUtils.AreEquivalent(Method.GetParametersCached()[0].ParameterType.GetNonRefType(), Left.Type);
                    }

                    return false;
                }
            }
            public override bool IsLiftedToNull => this.IsLifted && this.Type.IsNullableType();

            public override Expression Reduce()
            {
                return ReduceAssignment(Left, leftConversion: LeftConversion, functionalOp: lhs =>
                {
                    var res = FunctionalOp(_binaryType, lhs, Right, Method);

                    if (FinalConversion != null)
                    {
                        // DESIGN: Maybe we shouldn't use a LambdaExpression for the left conversion if it's trivial,
                        //         cf. use of ConversionIsNotSupportedForArithmeticTypes in LINQ APIs. Instead, we
                        //         could just insert a Convert[Checked] node. The current design allows for user-
                        //         defined conversions as well.
                        res = Apply(FinalConversion, res);
                    }

                    return res;
                });
            }

            internal static Expression FunctionalOp(ExpressionType binaryType, Expression left, Expression right, MethodInfo method)
            {
                if (right.Type != left.Type)
                {
                    // DESIGN: Should our factory allow a mismatch or should we require the right operand to be
                    //         converted already? Note the C# compiler will emit a Convert node here.
                    right = Expression.Convert(right, left.Type);
                }

                switch (binaryType)
                {
                    case ExpressionType.AddAssign:
                        return Expression.Add(left, right, method);
                    case ExpressionType.AndAssign:
                        return Expression.And(left, right, method);
                    case ExpressionType.DivideAssign:
                        return Expression.Divide(left, right, method);
                    case ExpressionType.ExclusiveOrAssign:
                        return Expression.ExclusiveOr(left, right, method);
                    case ExpressionType.LeftShiftAssign:
                        return Expression.LeftShift(left, right, method);
                    case ExpressionType.ModuloAssign:
                        return Expression.Modulo(left, right, method);
                    case ExpressionType.MultiplyAssign:
                        return Expression.Multiply(left, right, method);
                    case ExpressionType.OrAssign:
                        return Expression.Or(left, right, method);
                    case ExpressionType.RightShiftAssign:
                        return Expression.RightShift(left, right, method);
                    case ExpressionType.SubtractAssign:
                        return Expression.Subtract(left, right, method);
                    case ExpressionType.AddAssignChecked:
                        return Expression.AddChecked(left, right, method);
                    case ExpressionType.MultiplyAssignChecked:
                        return Expression.MultiplyChecked(left, right, method);
                    case ExpressionType.SubtractAssignChecked:
                        return Expression.SubtractChecked(left, right, method);
                }

                throw LinqError.UnhandledBinary(binaryType);
            }
        }

        internal static AssignBinaryCSharpExpression Make(ExpressionType binaryType, Expression left, Expression right, MethodInfo method, LambdaExpression leftConversion, LambdaExpression finalConversion)
        {
            ValidateCustomBinaryAssign(binaryType, left, right, method, leftConversion, finalConversion);

            return new WithConversions(binaryType, left, right, method, leftConversion, finalConversion);
        }

        private static void ValidateCustomBinaryAssign(ExpressionType binaryType, Expression left, Expression right, MethodInfo method, LambdaExpression leftConversion, LambdaExpression finalConversion)
        {
            var leftType = left.Type;
            var rightType = right.Type;

            if (leftConversion != null)
            {
                leftType = ValidateConversion(binaryType, leftType, leftConversion);
            }

            // NB: Just leverage LINQ to do the dirty work to check everything. Note that this assumes that the
            //     left conversion performed widening if that's required for the underlying operation. However,
            //     the use of FunctionalOp below will widen the right operand to match the left operand's type,
            //     which is what we do during Reduce as well (see remarks in FunctionalOp). This could produce
            //     mysterious error messages. (TODO: Review what's most appropriate here.)
            //
            // NB: We can't have it check the final conversion, because it doesn't allow these without the use of
            //     a custom method, so we check that ourselves further down.

            var leftDummy = Expression.Parameter(leftType, "__left");
            var rightDummy = Expression.Parameter(rightType, "__right");
            var functionalOp = WithConversions.FunctionalOp(binaryType, leftDummy, rightDummy, method);

            var resultType = functionalOp.Type;

            if (finalConversion != null)
            {
                resultType = ValidateConversion(binaryType, resultType, finalConversion);
            }

            if (!TypeUtils.AreEquivalent(resultType, left.Type))
            {
                throw Error.InvalidCompoundAssignmentWithOperands(binaryType, left.Type, right.Type);
            }
        }

        private static Type ValidateConversion(ExpressionType nodeType, Type inputType, LambdaExpression conversion)
        {
            var invoke = conversion.Type.GetMethod("Invoke");

            var invokeParameters = invoke.GetParametersCached();
            if (invokeParameters.Length != 1)
            {
                throw LinqError.IncorrectNumberOfMethodCallArguments(conversion);
            }

            if (!TypeUtils.AreEquivalent(invokeParameters[0].ParameterType, inputType))
            {
                throw LinqError.OperandTypesDoNotMatchParameters(nodeType, conversion.ToString());
            }

            return invoke.ReturnType;
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
        /// <param name="leftConversion">The conversion function used to convert the left hand side of the compound assignment prior to use by the underlying operation.</param>
        /// <param name="finalConversion">The conversion function used to convert the result of the underlying operation prior to assignment to the left hand side of the compound assignment operation.</param>
        /// <returns>A new <see cref="AssignBinaryCSharpExpression"/> instance representing the binary assignment.</returns>
        public static AssignBinaryCSharpExpression MakeBinaryAssign(CSharpExpressionType binaryType, Expression left, Expression right, MethodInfo method, LambdaExpression leftConversion, LambdaExpression finalConversion)
        {
            switch (binaryType)
            {
                case CSharpExpressionType.Assign:
                    return Assign(left, right);
                case CSharpExpressionType.AddAssign:
                    return AddAssign(left, right, method, leftConversion, finalConversion);
                case CSharpExpressionType.AndAssign:
                    return AndAssign(left, right, method, leftConversion, finalConversion);
                case CSharpExpressionType.DivideAssign:
                    return DivideAssign(left, right, method, leftConversion, finalConversion);
                case CSharpExpressionType.ExclusiveOrAssign:
                    return ExclusiveOrAssign(left, right, method, leftConversion, finalConversion);
                case CSharpExpressionType.LeftShiftAssign:
                    return LeftShiftAssign(left, right, method, leftConversion, finalConversion);
                case CSharpExpressionType.ModuloAssign:
                    return ModuloAssign(left, right, method, leftConversion, finalConversion);
                case CSharpExpressionType.MultiplyAssign:
                    return MultiplyAssign(left, right, method, leftConversion, finalConversion);
                case CSharpExpressionType.OrAssign:
                    return OrAssign(left, right, method, leftConversion, finalConversion);
                case CSharpExpressionType.RightShiftAssign:
                    return RightShiftAssign(left, right, method, leftConversion, finalConversion);
                case CSharpExpressionType.SubtractAssign:
                    return SubtractAssign(left, right, method, leftConversion, finalConversion);
                case CSharpExpressionType.AddAssignChecked:
                    return AddAssignChecked(left, right, method, leftConversion, finalConversion);
                case CSharpExpressionType.MultiplyAssignChecked:
                    return MultiplyAssignChecked(left, right, method, leftConversion, finalConversion);
                case CSharpExpressionType.SubtractAssignChecked:
                    return SubtractAssignChecked(left, right, method, leftConversion, finalConversion);
            }

            throw LinqError.UnhandledBinary(binaryType);
        }

        private static AssignBinaryCSharpExpression MakeBinaryAssign(ExpressionType binaryType, BinaryAssignFactory factory, Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
        {
            if (binaryType == ExpressionType.Assign)
            {
                var lhs = GetLhs(left, nameof(left));
                var assign = factory(lhs, right, method, finalConversion);
                return new AssignBinaryCSharpExpression.Primitive(assign, left);
            }
            else
            {
                return MakeBinaryCompoundAssign(binaryType, factory, left, right, method, finalConversion, leftConversion);
            }
        }

        private static AssignBinaryCSharpExpression MakeBinaryCompoundAssign(ExpressionType binaryType, BinaryAssignFactory factory, Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
        {
            var lhs = GetLhs(left, nameof(left));

            // NB: We could return a BinaryExpression in case the lhs is not one of our index nodes, but it'd change
            //     the return type to Expression which isn't nice to consume. Also, the Update method would either
            //     have to change to return Expression or we should have an AssignBinary node to hold a Binary node
            //     underneath it. This said, a specialized layout for the case where the custom node trivially wraps
            //     a LINQ node could be useful (just make Left virtual).

            var isCSharpSpecific = false;

            var leftType = left.Type;
            var rightType = right.Type;

            if (leftType == typeof(string))
            {
                if (method == null)
                {
                    // NB: This can be represented using the LINQ node; just need the method to be resolved. So, no need to
                    //     mark it as C# specific.

                    if (binaryType == ExpressionType.AddAssign || binaryType == ExpressionType.AddAssignChecked)
                    {
                        if (rightType == typeof(string))
                        {
                            method = typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) });
                        }
                        else
                        {
                            method = typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(object) });

                            if (!TypeUtils.AreReferenceAssignable(typeof(object), rightType))
                            {
                                // DESIGN: Should our factory do this our just reject the input?
                                right = Expression.Convert(right, typeof(object));
                            }
                        }
                    }
                    else
                    {
                        throw Error.InvalidCompoundAssignment(binaryType, typeof(string));
                    }
                }
            }
            else if (typeof(MulticastDelegate).IsAssignableFrom(leftType))
            {
                if (leftType == typeof(MulticastDelegate))
                {
                    throw Error.InvalidCompoundAssignmentWithOperands(binaryType, leftType, rightType);
                }

                // NB: This checks for assignment with variance checks in mind, e.g.
                //
                //       Action<string> s = ...;
                //       Action<object> o = ...;
                //       s += o;

                if (!TypeUtils.AreReferenceAssignable(leftType, rightType))
                {
                    throw Error.InvalidCompoundAssignmentWithOperands(binaryType, leftType, rightType);
                }

                if (method == null)
                {
                    if (binaryType == ExpressionType.AddAssign || binaryType == ExpressionType.AddAssignChecked)
                    {
                        method = typeof(Delegate).GetMethod(nameof(Delegate.Combine), new[] { typeof(Delegate), typeof(Delegate) });
                    }
                    else if (binaryType == ExpressionType.SubtractAssign || binaryType == ExpressionType.SubtractAssignChecked)
                    {
                        method = typeof(Delegate).GetMethod(nameof(Delegate.Remove), new[] { typeof(Delegate), typeof(Delegate) });
                    }
                    else
                    {
                        throw Error.InvalidCompoundAssignment(binaryType, leftType);
                    }

                    if (finalConversion == null)
                    {
                        var resultParameter = Expression.Parameter(typeof(Delegate), "__result");
                        var convertResult = Expression.Convert(resultParameter, leftType);
                        finalConversion = Expression.Lambda(convertResult, resultParameter);
                    }
                }

                isCSharpSpecific = true;
            }
            else if (IsCSharpSpecificCompoundNumeric(leftType))
            {
                // NB: If any of these are passed, we'll assume the types all line up. The call to
                //     the ValidateCustomBinaryAssign method below will check that's indeed the case.

                if (method == null && leftConversion == null && finalConversion == null)
                {
                    var isChecked = IsChecked(binaryType);

                    var isNullabeLeftType = leftType.IsNullableType();
                    var nonNullLeftType = leftType.GetNonNullableType();
                    var intermediateType = nonNullLeftType.IsEnum ? nonNullLeftType.GetEnumUnderlyingType() : typeof(int);

                    var leftParameter = Expression.Parameter(leftType, "__left");
                    var convertType = isNullabeLeftType ? typeof(Nullable<>).MakeGenericType(intermediateType) : intermediateType;
                    var convertLeft = isChecked ? Expression.ConvertChecked(leftParameter, convertType) : Expression.Convert(leftParameter, convertType);
                    leftConversion = Expression.Lambda(convertLeft, leftParameter);

                    var resultParameter = Expression.Parameter(convertType, "__result");
                    var convertResult = isChecked ? Expression.ConvertChecked(resultParameter, leftType) : Expression.Convert(resultParameter, leftType);
                    finalConversion = Expression.Lambda(convertResult, resultParameter);
                }

                isCSharpSpecific = true;
            }

            if (isCSharpSpecific || leftConversion != null)
            {
                return AssignBinaryCSharpExpression.Make(binaryType, left, right, method, leftConversion, finalConversion);
            }

            var assign = factory(lhs, right, method, finalConversion);
            return new AssignBinaryCSharpExpression.Primitive(assign, left);
        }

        private static bool IsCSharpSpecificCompoundNumeric(Type type)
        {
            type = type.GetNonNullableType();

            if (!type.IsEnum)
            {
                switch (type.GetTypeCode())
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
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

        private static bool IsChecked(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.AddAssignChecked:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.SubtractAssignChecked:
                    return true;
            }

            return false;
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
            return node.Update(Visit(node.Left), VisitAndConvert(node.LeftConversion, nameof(VisitBinaryAssign)), Visit(node.Right), VisitAndConvert(node.FinalConversion, nameof(VisitBinaryAssign)));
        }
    }
}
