// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
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
        public TupleConvertCSharpExpression Update(Expression operand, IEnumerable<LambdaExpression> elementConversions)
        {
            if (operand == this.Operand && elementConversions == this.ElementConversions)
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

            ParameterExpression tmp = null;

            // NB: We can't check IsPure with the readOnly parameter set to true, because the user conversions may assign to a variable.
            //     To mitigate this, we'd have to check all element conversions to make sure they don't assign to the variable.

            if (!Helpers.IsPure(Operand))
            {
                tmp = Expression.Parameter(Operand.Type, "__t");
                operand = tmp;
            }

            var n = ElementConversions.Count;

            var args = new List<Expression>(n);

            for (int i = 0; i < n; i++)
            {
                var conversion = ElementConversions[i];

                var item = Helpers.GetTupleItemAccess(operand, i);

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
                    args.Add(Expression.Invoke(conversion, item));
                }

                static bool IsConvert(ExpressionType nodeType)
                {
                    return nodeType == ExpressionType.Convert || nodeType == ExpressionType.ConvertChecked || nodeType == ExpressionType.Unbox;
                }
            }

            var res = CSharpExpression.TupleLiteral(Type, args, argumentNames: null);

            if (tmp != null)
            {
                return Expression.Block(new[] { tmp }, Expression.Assign(tmp, Operand), res);
            }

            return res;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="TupleConvertCSharpExpression" /> that represents a tuple conversion.
        /// </summary>
        /// <param name="operand">The <see cref="Expression" /> representing the tuple to convert.</param>
        /// <param name="type">The <see cref="Type" /> that represents the tuple type to convert to.</param>
        /// <param name="elementConversions">An array of one or more of <see cref="LambdaExpression" /> objects that represent the conversions of the tuple elements.</param>
        /// <returns></returns>
        public static TupleConvertCSharpExpression TupleConvert(Expression operand, Type type, params LambdaExpression[] elementConversions)
        {
            return TupleConvert(operand, type, (IEnumerable<LambdaExpression>)elementConversions);
        }

        public static TupleConvertCSharpExpression TupleConvert(Expression operand, Type type, IEnumerable<LambdaExpression> elementConversions)
        {
            RequiresCanRead(operand, nameof(operand));

            if (!Helpers.IsTupleType(operand.Type))
            {
                throw Error.InvalidTupleType(operand.Type);
            }

            ContractUtils.RequiresNotNull(type, nameof(type));

            if (!Helpers.IsTupleType(type))
            {
                throw Error.InvalidTupleType(type);
            }

            var arityFrom = Helpers.GetTupleArity(operand.Type);
            var arityTo = Helpers.GetTupleArity(type);

            if (arityFrom != arityTo)
            {
                throw Error.TupleComponentCountMismatch(operand.Type, type);
            }

            var conversions = elementConversions.ToReadOnly();

            if (conversions.Count != arityFrom)
            {
                throw Error.InvalidElementConversionCount(arityFrom);
            }

            var fromTypes = Helpers.GetTupleComponentTypes(operand.Type).ToArray();
            var toTypes = Helpers.GetTupleComponentTypes(type).ToArray();

            ContractUtils.RequiresNotNullItems(conversions, nameof(conversions));

            for (int i = 0; i < arityFrom; i++)
            {
                CheckConversion(conversions[i], fromTypes[i], toTypes[i]);
            }

            return new TupleConvertCSharpExpression(operand, type, conversions);

            static void CheckConversion(LambdaExpression conversion, Type from, Type to)
            {
                var method = conversion.Type.GetMethod("Invoke");
                var parameters = method.GetParametersCached();

                if (parameters.Length != 1)
                {
                    throw LinqError.IncorrectNumberOfMethodCallArguments(conversion);
                }

                if (!TypeUtils.AreEquivalent(method.ReturnType, to))
                {
                    throw LinqError.OperandTypesDoNotMatchParameters(CSharpExpressionType.TupleConvert, conversion.ToString());
                }

                if (!ParameterIsAssignable(parameters[0], from))
                {
                    throw LinqError.OperandTypesDoNotMatchParameters(CSharpExpressionType.TupleConvert, conversion.ToString());
                }
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
        protected internal virtual Expression VisitTupleConvert(TupleConvertCSharpExpression node)
        {
            return node.Update(Visit(node.Operand), VisitAndConvert(node.ElementConversions, nameof(VisitTupleConvert)));
        }
    }
}
