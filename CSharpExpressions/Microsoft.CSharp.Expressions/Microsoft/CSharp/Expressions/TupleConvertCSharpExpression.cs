// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a tuple conversion.
    /// </summary>
    public sealed partial class TupleConvertCSharpExpression : CSharpExpression
    {
        internal TupleConvertCSharpExpression(Expression operand, Type type, ReadOnlyCollection<LambdaExpression> elementConversions)
        {
            Operand = operand;
            Type = type;
            ElementConversions = elementConversions;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.TupleConvert;

        /// <summary>
        /// Gets the expression representing the tuple to convert.
        /// </summary>
        public Expression Operand { get; }

        /// <summary>
        /// Gets the conversion operations applied to the elements of the tuple.
        /// </summary>
        public ReadOnlyCollection<LambdaExpression> ElementConversions { get; }

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        public override Type Type { get; }

        /// <summary>
        /// Gets a value indicating whether the tuple conversion is lifted.
        /// </summary>
        public bool IsLifted => Type.IsNullableType() || Operand.Type.IsNullableType();

        /// <summary>
        /// Gets a value indicating whether the tuple conversion whose return type is lifted to a nullable type.
        /// </summary>
        public bool IsLiftedToNull => IsLifted && Type.IsNullableType();

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitTupleConvert(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="operand">The <see cref="Operand" /> property of the result.</param>
        /// <param name="elementConversions">The <see cref="ElementConversions" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public TupleConvertCSharpExpression Update(Expression operand, IEnumerable<LambdaExpression>? elementConversions)
        {
            if (operand == Operand && SameElements(ref elementConversions, ElementConversions))
            {
                return this;
            }

            return CSharpExpression.TupleConvert(operand, Type, elementConversions);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var operand = Operand;

            var temps = new List<ParameterExpression>();
            var stmts = new List<Expression>();


            //
            // NB: We can't check IsPure with the readOnly parameter set to true, because the user conversions may assign to a variable.
            //     To mitigate this, we'd have to check all element conversions to make sure they don't assign to the variable, which is
            //     very unlikely, though a hand-written conversion lambda could have such a side-effect. One could argue that a side-
            //     effecting conversion that mutates the parent tuple yields undefined behavior.
            //

            if (!Helpers.IsPure(Operand))
            {
                var operandVariable = Expression.Parameter(Operand.Type, "__t");
                
                temps.Add(operandVariable);
                stmts.Add(Expression.Assign(operandVariable, Operand));

                operand = operandVariable;
            }

            Expression res;

            if (operand.Type.IsNullableType())
            {
                var nonNullOperand = MakeNullableGetValue(operand); // NB: Use of Nullable<T>.Value to ensure proper exception is thrown if null.
                var nonNullOperandVariable = Expression.Parameter(nonNullOperand.Type, "__nonNull");

                var args = GetConversions(nonNullOperandVariable);

                if (Type.IsNullableType())
                {
                    //
                    // T? -> U?
                    //
                    // var t = operand;
                    //
                    // if (t.HasValue)
                    // {
                    //    var v = t.Value;
                    //    return (U?)new U(...);
                    // }
                    // else
                    // {
                    //    return default(U?);
                    // }
                    //

                    var hasValueTest = MakeNullableHasValue(operand);

                    var nullValue = Expression.Default(Type);

                    var convertedTuple = Expression.Convert(CSharpExpression.TupleLiteral(Type.GetNonNullableType(), args, argumentNames: null), Type);
                    var nonNullValue = Expression.Block(new[] { nonNullOperandVariable }, Expression.Assign(nonNullOperandVariable, nonNullOperand), convertedTuple);

                    res = Expression.Condition(hasValueTest, nonNullValue, nullValue);
                }
                else
                {
                    //
                    // T? -> U
                    //
                    // var t = operand;
                    // var v = operand.Value; // NB: May throw
                    // return new U(...);

                    temps.Add(nonNullOperandVariable);
                    stmts.Add(Expression.Assign(nonNullOperandVariable, nonNullOperand));

                    res = CSharpExpression.TupleLiteral(Type, args, argumentNames: null);
                }
            }
            else
            {
                var args = GetConversions(operand);

                var targetType = Type.GetNonNullableType();

                var convertedTuple = CSharpExpression.TupleLiteral(targetType, args, argumentNames: null);

                if (Type.IsNullableType())
                {
                    //
                    // T -> U?
                    //
                    // var t = operand;
                    // return (U?)new U(...);

                    res = Expression.Convert(convertedTuple, Type);
                }
                else
                {
                    //
                    // T -> U
                    //
                    // var t = operand;
                    // return new U(...);
                    //

                    res = convertedTuple;
                }
            }

            stmts.Add(res);

            return Comma(temps, stmts);

            List<Expression> GetConversions(Expression operand)
            {
                var n = ElementConversions.Count;

                var args = new List<Expression>(n);

                for (int i = 0; i < n; i++)
                {
                    var conversion = ElementConversions[i];

                    var item = GetTupleItemAccess(operand, i);

                    var conversionParameter = conversion.Parameters[0];

                    if (conversion.Body is UnaryExpression { Operand: var unaryOperand } unary && unaryOperand == conversionParameter && IsConvert(unary.NodeType))
                    {
                        args.Add(unary.Update(item));
                    }
                    else if (conversion.Body is TupleConvertCSharpExpression { Operand: var convertOperand } convert && convertOperand == conversionParameter)
                    {
                        args.Add(convert.Update(item, convert.ElementConversions));
                    }
                    else
                    {
                        //
                        // CONSIDER: Accessing a tuple item is a pure operation, so it can be reordered. We could try to inline the item expression
                        //           into the lambda body (beta reduction). However, the common cases (i.e. those generated by the C# compiler) should
                        //           be handled above, so this may not be worth it. Also, this type of optimization could be done elsewhere, more
                        //           globally (though it'd have to analyze that the fields in the tuple are not being written to).
                        //

                        args.Add(Expression.Invoke(conversion, item));
                    }

                    static bool IsConvert(ExpressionType nodeType) =>
                        nodeType == ExpressionType.Convert ||
                        nodeType == ExpressionType.ConvertChecked ||
                        nodeType == ExpressionType.Unbox;
                }

                return args;
            }
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="TupleConvertCSharpExpression" /> that represents a tuple conversion.
        /// </summary>
        /// <param name="operand">The <see cref="Expression" /> representing the tuple to convert.</param>
        /// <param name="type">The <see cref="Type" /> that represents the tuple type to convert to.</param>
        /// <returns>A <see cref="TupleConvertCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleConvert" /> and the <see cref="TupleConvertCSharpExpression.Operand" /> and <see cref="TupleConvertCSharpExpression.ElementConversions" /> properties set to the specified values.</returns>
        public static TupleConvertCSharpExpression TupleConvert(Expression operand, Type type) => TupleConvert(operand, type, elementConversions: null);

        /// <summary>
        /// Creates a <see cref="TupleConvertCSharpExpression" /> that represents a tuple conversion.
        /// </summary>
        /// <param name="operand">The <see cref="Expression" /> representing the tuple to convert.</param>
        /// <param name="type">The <see cref="Type" /> that represents the tuple type to convert to.</param>
        /// <param name="elementConversions">An array of one or more of <see cref="LambdaExpression" /> objects that represent the conversions of the tuple elements.</param>
        /// <returns>A <see cref="TupleConvertCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleConvert" /> and the <see cref="TupleConvertCSharpExpression.Operand" /> and <see cref="TupleConvertCSharpExpression.ElementConversions" /> properties set to the specified values.</returns>
        public static TupleConvertCSharpExpression TupleConvert(Expression operand, Type type, params LambdaExpression[]? elementConversions) =>
            TupleConvert(operand, type, (IEnumerable<LambdaExpression>?)elementConversions);

        /// <summary>
        /// Creates a <see cref="TupleConvertCSharpExpression" /> that represents a tuple conversion.
        /// </summary>
        /// <param name="operand">The <see cref="Expression" /> representing the tuple to convert.</param>
        /// <param name="type">The <see cref="Type" /> that represents the tuple type to convert to.</param>
        /// <param name="elementConversions">An array of one or more of <see cref="LambdaExpression" /> objects that represent the conversions of the tuple elements.</param>
        /// <returns>A <see cref="TupleConvertCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleConvert" /> and the <see cref="TupleConvertCSharpExpression.Operand" /> and <see cref="TupleConvertCSharpExpression.ElementConversions" /> properties set to the specified values.</returns>
        public static TupleConvertCSharpExpression TupleConvert(Expression operand, Type type, IEnumerable<LambdaExpression>? elementConversions)
        {
            RequiresCanRead(operand, nameof(operand));

            var sourceType = operand.Type;

            if (sourceType.IsNullableType())
            {
                sourceType = sourceType.GetNonNullableType();
            }

            if (!IsTupleType(sourceType))
                throw Error.InvalidTupleType(operand.Type);

            RequiresNotNull(type, nameof(type));

            var destinationType = type;

            if (destinationType.IsNullableType())
            {
                destinationType = destinationType.GetNonNullableType();
            }

            if (!IsTupleType(destinationType))
                throw Error.InvalidTupleType(type);

            var arityFrom = GetTupleArity(sourceType);
            var arityTo = GetTupleArity(destinationType);

            if (arityFrom != arityTo)
                throw Error.TupleComponentCountMismatch(sourceType, destinationType);

            var fromTypes = GetTupleComponentTypes(sourceType).ToArray();
            var toTypes = GetTupleComponentTypes(destinationType).ToArray();

            ReadOnlyCollection<LambdaExpression> conversions;

            if (elementConversions == null)
            {
                var inferredConversions = new LambdaExpression[arityFrom];

                for (int i = 0; i < arityFrom; i++)
                {
                    var fromComponentType = fromTypes[i];
                    var toComponentType = toTypes[i];

                    var fromComponentParameter = Expression.Parameter(fromComponentType);

                    var conversionBody =
                        IsTupleType(fromComponentType.GetNonNullableType()) && IsTupleType(toComponentType.GetNonNullableType())
                            ? (Expression)TupleConvert(fromComponentParameter, toComponentType, elementConversions: null)
                            : Expression.Convert(fromComponentParameter, toComponentType);

                    inferredConversions[i] = Expression.Lambda(conversionBody, fromComponentParameter);
                }

                conversions = inferredConversions.ToReadOnlyUnsafe();
            }
            else
            {
                conversions = elementConversions.ToReadOnly();

                if (conversions.Count != arityFrom)
                    throw Error.InvalidElementConversionCount(arityFrom);

                RequiresNotNullItems(conversions, nameof(elementConversions));

                for (int i = 0; i < arityFrom; i++)
                {
                    CheckConversion(conversions[i], fromTypes[i], toTypes[i], nameof(elementConversions), i);
                }
            }

            return new TupleConvertCSharpExpression(operand, type, conversions);

            static void CheckConversion(LambdaExpression conversion, Type from, Type to, string paramName, int index)
            {
                var method = conversion.Type.GetMethod("Invoke")!;
                var parameters = method.GetParametersCached();

                if (parameters.Length != 1)
                    throw IncorrectNumberOfMethodCallArguments(conversion, paramName, index);

                if (!TypeUtils.AreEquivalent(method.ReturnType, to))
                    throw OperandTypesDoNotMatchParameters(CSharpExpressionType.TupleConvert, conversion.ToString(), paramName, index);

                if (!ParameterIsAssignable(parameters[0], from))
                    throw OperandTypesDoNotMatchParameters(CSharpExpressionType.TupleConvert, conversion.ToString(), paramName, index);
            }
        }
    }
    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="TupleConvertCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitTupleConvert(TupleConvertCSharpExpression node) =>
            node.Update(
                Visit(node.Operand),
                VisitAndConvert(node.ElementConversions, nameof(VisitTupleConvert))
            );
    }
}
