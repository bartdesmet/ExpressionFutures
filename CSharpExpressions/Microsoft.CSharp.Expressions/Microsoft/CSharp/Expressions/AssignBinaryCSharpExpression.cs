// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.ExpressionStubs;
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
        private AssignBinaryCSharpExpression(CSharpExpressionType binaryType, Expression left, Expression right)
            : base(left, right)
        {
            CSharpNodeType = binaryType;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />.
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType { get; }

        /// <summary>
        /// Gets the static type of the expression.
        /// </summary>
        public override Type Type => Left.Type;

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
        public bool IsLifted
        {
            get
            {
                // NB: Same logic as LINQ's BinaryExpression, modulo the absence of the Coalesce case.

                if (CSharpNodeType == CSharpExpressionType.Assign || CSharpNodeType == CSharpExpressionType.NullCoalescingAssign)
                {
                    return false;
                }

                if (Left.Type.IsNullableType())
                {
                    return Method == null || !TypeUtils.AreEquivalent(Method.GetParametersCached()[0].ParameterType.GetNonRefType(), Left.Type);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the expression tree node represents a lifted call to an operator whose return type is lifted to a nullable type.
        /// </summary>
        /// <returns>true if the operator's return type is lifted to a nullable type; otherwise, false.</returns>
        public bool IsLiftedToNull => IsLifted && Type.IsNullableType();

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
        /// <param name="left">The <see cref="BinaryCSharpExpression.Left" /> property of the result. </param>
        /// <param name="leftConversion">The <see cref="LeftConversion" /> property of the result.</param>
        /// <param name="right">The <see cref="BinaryCSharpExpression.Right" /> property of the result. </param>
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

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var left = MakeWriteable(Left);

            if (CSharpNodeType == CSharpExpressionType.NullCoalescingAssign)
            {
                // NB: NullCoalescingAssign is handled by the Reduce implementation on the derived type.

                throw ContractUtils.Unreachable;
            }

            if (CSharpNodeType == CSharpExpressionType.Assign)
            {
                // NB: We still use `ReduceAssignment` here to deal with IndexCSharpExpression which is not
                //     a valid LHS to Expression.Assign.

                return left switch
                {
                    IndexCSharpExpression indexCSharp => indexCSharp.ReduceAssign(lhs => Expression.Assign(lhs, Right)),
                    ArrayAccessCSharpExpression arrayAccessCSharp => arrayAccessCSharp.ReduceAssign(lhs => Expression.Assign(lhs, Right)),
                    IndexerAccessCSharpExpression indexerAccessCSharp => indexerAccessCSharp.ReduceAssign(lhs => Expression.Assign(lhs, Right)),
                    _ => Expression.Assign(left, Right)
                };
            }

            return ReduceAssignment(left, leftConversion: LeftConversion, functionalOp: lhs =>
            {
                var res = (Expression)FunctionalOp(CSharpNodeType, lhs, Right, Method);

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

        internal static BinaryExpression FunctionalOp(CSharpExpressionType binaryType, Expression left, Expression right, MethodInfo method)
        {
            return binaryType switch
            {
                CSharpExpressionType.Assign => Expression.Assign(left, right),
                CSharpExpressionType.AddAssign => Expression.Add(left, right, method),
                CSharpExpressionType.AndAssign => Expression.And(left, right, method),
                CSharpExpressionType.DivideAssign => Expression.Divide(left, right, method),
                CSharpExpressionType.ExclusiveOrAssign => Expression.ExclusiveOr(left, right, method),
                CSharpExpressionType.LeftShiftAssign => Expression.LeftShift(left, right, method),
                CSharpExpressionType.ModuloAssign => Expression.Modulo(left, right, method),
                CSharpExpressionType.MultiplyAssign => Expression.Multiply(left, right, method),
                CSharpExpressionType.OrAssign => Expression.Or(left, right, method),
                CSharpExpressionType.RightShiftAssign => Expression.RightShift(left, right, method),
                CSharpExpressionType.SubtractAssign => Expression.Subtract(left, right, method),
                CSharpExpressionType.AddAssignChecked => Expression.AddChecked(left, right, method),
                CSharpExpressionType.MultiplyAssignChecked => Expression.MultiplyChecked(left, right, method),
                CSharpExpressionType.SubtractAssignChecked => Expression.SubtractChecked(left, right, method),
                _ => throw LinqError.UnhandledBinary(binaryType),
            };
        }

        internal static AssignBinaryCSharpExpression Make(CSharpExpressionType binaryType, Expression left, Expression right, MethodInfo method, LambdaExpression leftConversion, LambdaExpression finalConversion)
        {
            if (binaryType == CSharpExpressionType.NullCoalescingAssign)
            {
                return MakeNullCoalescingAssign(left, right, method, leftConversion, finalConversion);
            }
            else
            {
                return MakeCustomBinaryAssign(binaryType, left, right, method, leftConversion, finalConversion);
            }
        }

        private static AssignBinaryCSharpExpression MakeNullCoalescingAssign(Expression left, Expression right, MethodInfo method, LambdaExpression leftConversion, LambdaExpression finalConversion)
        {
            if (method != null || leftConversion != null || finalConversion != null)
            {
                throw Error.InvalidNullCoalescingAssignmentArguments();
            }

            ValidateCoalesceArgTypes(left.Type, right.Type);

            return new NullCoalescingAssignment(left, right);
        }

        private static AssignBinaryCSharpExpression MakeCustomBinaryAssign(CSharpExpressionType binaryType, Expression left, Expression right, MethodInfo method, LambdaExpression leftConversion, LambdaExpression finalConversion)
        {
            ValidateCustomBinaryAssign(binaryType, left, right, ref method, leftConversion, finalConversion);

            // TODO: Add optimized layouts

            return new WithConversions(binaryType, left, right, method, leftConversion, finalConversion);
        }

        private static void ValidateCustomBinaryAssign(CSharpExpressionType binaryType, Expression left, Expression right, ref MethodInfo method, LambdaExpression leftConversion, LambdaExpression finalConversion)
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
            var functionalOp = FunctionalOp(binaryType, leftDummy, rightDummy, method);

            if (method == null)
            {
                method = functionalOp.Method;
            }

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

        private static Type ValidateConversion(CSharpExpressionType nodeType, Type inputType, LambdaExpression conversion)
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

        internal class WithConversions : AssignBinaryCSharpExpression
        {
            internal WithConversions(CSharpExpressionType binaryType, Expression left, Expression right, MethodInfo method, LambdaExpression leftConversion, LambdaExpression finalConversion)
                : base(binaryType, left, right)
            {
                Method = method;
                LeftConversion = leftConversion;
                FinalConversion = finalConversion;
            }

            public override MethodInfo Method { get; }
            public override LambdaExpression LeftConversion { get; }
            public override LambdaExpression FinalConversion { get; }
        }

        internal class NullCoalescingAssignment : AssignBinaryCSharpExpression
        {
            internal NullCoalescingAssignment(Expression left, Expression right)
                : base(CSharpExpressionType.NullCoalescingAssign, left, right)
            {
            }

            public override Type Type
            {
                get
                {
                    var leftType = Left.Type;

                    if (leftType.IsNullableType())
                    {
                        var underlyingLeftType = leftType.GetNonNullableType();

                        if (Right.Type == underlyingLeftType)
                        {
                            return underlyingLeftType;
                        }
                    }

                    return leftType;
                }
            }

            public override MethodInfo Method => null;
            public override LambdaExpression LeftConversion => null;
            public override LambdaExpression FinalConversion => null;

            private bool IsNullableValueTypeAssignment
            {
                get
                {
                    var leftType = Left.Type;

                    if (!leftType.IsNullableType())
                    {
                        return false;
                    }

                    return leftType.GetNonNullableType() == Right.Type;
                }
            }

            public override Expression Reduce()
            {
                var left = MakeWriteable(Left);

                if (IsNullableValueTypeAssignment)
                {
                    return ReduceAssign(left, assign: lhs =>
                    {
                        var stmts = new List<Expression>();
                        var temps = new List<ParameterExpression>();

                        var tmp = Expression.Variable(Type, "__tmp");
                        temps.Add(tmp);

                        var lhsVal = (Expression)lhs;

                        if (!IsPure(lhs))
                        {
                            var lhsValVar = Expression.Variable(lhs.Type, "__lhsVal");
                            temps.Add(lhsValVar);

                            var assignLhsVal = Expression.Assign(lhsValVar, lhs);
                            stmts.Add(assignLhsVal);

                            lhsVal = lhsValVar;
                        }

                        var assignTmp = Expression.Assign(tmp, Helpers.MakeNullableGetValueOrDefault(lhsVal));
                        stmts.Add(assignTmp);

                        var assignLhs = Expression.Block(Expression.Assign(tmp, Right), Expression.Assign(lhs, Expression.Convert(tmp, lhs.Type)), tmp);
                        var conditional = Expression.Condition(Helpers.MakeNullableHasValue(lhsVal), tmp, assignLhs);
                        stmts.Add(conditional);

                        return Expression.Block(temps, stmts);

                        static bool IsPure(Expression e)
                        {
                            //
                            // Local decision on purity based on ReduceAssign spilling children of nodes to temporaries. E.g. for a member access,
                            // we end up with __obj.Member, where __obj cannot be assigned to. If Member happens to be a field, we can safely access
                            // it multiple times without a side-effect (but not if it's a property).
                            //
                            // NB: Roslyn relies on ref locals. We don't have those in expression trees.
                            //

                            if (e.NodeType == ExpressionType.Parameter)
                            {
                                return true;
                            }

                            if (e.NodeType == ExpressionType.MemberAccess)
                            {
                                var m = (MemberExpression)e;
                                return m.Member.MemberType == MemberTypes.Field;
                            }

                            return false;
                        }
                    });
                }
                else
                {
                    return ReduceAssignment(left, leftConversion: null, functionalOp: lhs => Expression.Coalesce(lhs, Right));
                }
            }
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an expression representing a binary assignment operation.
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
            return binaryType switch
            {
                CSharpExpressionType.Assign => Assign(left, right),
                CSharpExpressionType.AddAssign => AddAssign(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.AndAssign => AndAssign(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.DivideAssign => DivideAssign(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.ExclusiveOrAssign => ExclusiveOrAssign(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.LeftShiftAssign => LeftShiftAssign(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.ModuloAssign => ModuloAssign(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.MultiplyAssign => MultiplyAssign(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.OrAssign => OrAssign(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.RightShiftAssign => RightShiftAssign(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.SubtractAssign => SubtractAssign(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.AddAssignChecked => AddAssignChecked(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.MultiplyAssignChecked => MultiplyAssignChecked(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.SubtractAssignChecked => SubtractAssignChecked(left, right, method, leftConversion, finalConversion),
                CSharpExpressionType.NullCoalescingAssign => NullCoalescingAssign(left, right),
                _ => throw LinqError.UnhandledBinary(binaryType),
            };
        }

        private static AssignBinaryCSharpExpression MakeBinaryAssignCore(CSharpExpressionType binaryType, Expression left, Expression right, MethodInfo method, LambdaExpression finalConversion, LambdaExpression leftConversion)
        {
            Helpers.RequiresCanWrite(left, nameof(left));
            RequiresCanRead(right, nameof(right));

            // NB: We could return a BinaryExpression in case the lhs is not one of our index nodes, but it'd change
            //     the return type to Expression which isn't nice to consume. Also, the Update method would either
            //     have to change to return Expression or we should have an AssignBinary node to hold a Binary node
            //     underneath it. This said, a specialized layout for the case where the custom node trivially wraps
            //     a LINQ node could be useful (just make Left virtual).

            if (binaryType != CSharpExpressionType.Assign && binaryType != CSharpExpressionType.NullCoalescingAssign)
            {
                var leftType = left.Type;
                var rightType = right.Type;

                if (leftType == typeof(string))
                {
                    if (method == null)
                    {
                        if (binaryType == CSharpExpressionType.AddAssign || binaryType == CSharpExpressionType.AddAssignChecked)
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
                        if (binaryType == CSharpExpressionType.AddAssign || binaryType == CSharpExpressionType.AddAssignChecked)
                        {
                            method = typeof(Delegate).GetMethod(nameof(Delegate.Combine), new[] { typeof(Delegate), typeof(Delegate) });
                        }
                        else if (binaryType == CSharpExpressionType.SubtractAssign || binaryType == CSharpExpressionType.SubtractAssignChecked)
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
                }
                else if (IsCSharpSpecificCompoundNumeric(leftType))
                {
                    // NB: If any of these are passed, we'll assume the types all line up. The call to
                    //     the ValidateCustomBinaryAssign method below will check that's indeed the case.

                    if (method == null && leftConversion == null && finalConversion == null)
                    {
                        var isChecked = IsCheckedBinary(binaryType);

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

                        if (rightType != convertType)
                        {
                            // DESIGN: Should our factory do this or just reject the input? On the one hand,
                            //         C# allows e.g. byte += byte, so if this is a C#-specific API it may be
                            //         reasonable for the user to expect such a tree can be built. On the
                            //         other hand, it's very unlike the expression tree API to insert nodes
                            //         on behalf of the user in the factories. Note that Roslyn often models
                            //         conversions as properties on a node using a `Conversion` objects
                            //         which would be handy to keep the shape from the tree isomorphic to the
                            //         bound nodes in the compiler. Note though that the RHS of a compound
                            //         assignment doesn't have such a conversion and the compiler will insert
                            //         a convert node in this case, so this is really just a convenience in
                            //         our factory method to mimic that behavior.
                            right = Expression.Convert(right, convertType);
                        }
                    }
                }
            }

            return AssignBinaryCSharpExpression.Make(binaryType, left, right, method, leftConversion, finalConversion);
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

        private static bool IsCheckedBinary(CSharpExpressionType type)
        {
            switch (type)
            {
                case CSharpExpressionType.AddAssignChecked:
                case CSharpExpressionType.MultiplyAssignChecked:
                case CSharpExpressionType.SubtractAssignChecked:
                    return true;
            }

            return false;
        }
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
