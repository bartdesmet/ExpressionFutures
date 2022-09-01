// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a conversion of an object involving deconstruction and recursive conversions of the components obtained through deconstruction.
    /// </summary>
    public sealed partial class DeconstructionConversion : Conversion
    {
        private Type _inputType, _resultType;

        internal DeconstructionConversion(LambdaExpression deconstruct, ReadOnlyCollection<Conversion> conversions)
        {
            Deconstruct = deconstruct;
            Conversions = conversions;
        }

        /// <summary>
        /// Gets the input type of the conversion.
        /// </summary>
        public override Type InputType
        {
            get
            {
                Type GetInputType()
                {
                    if (Deconstruct != null)
                    {
                        return Deconstruct.Parameters[0].Type;
                    }

                    var n = Conversions.Count;

                    var types = new Type[n];

                    for (var i = 0; i < n; i++)
                    {
                        types[i] = Conversions[i].InputType;
                    }

                    return Helpers.MakeTupleType(types);
                }

                return _inputType ??= GetInputType();
            }
        }

        /// <summary>
        /// Gets the result type of the conversion.
        /// </summary>
        public override Type ResultType
        {
            get
            {
                Type GetResultType()
                {
                    var n = Conversions.Count;

                    var types = new Type[n];

                    for (var i = 0; i < n; i++)
                    {
                        types[i] = Conversions[i].ResultType;
                    }

                    return Helpers.MakeTupleType(types);
                }

                return _resultType ??= GetResultType();
            }
        }

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the deconstruction step.
        /// </summary>
        public LambdaExpression Deconstruct { get; }

        /// <summary>
        /// Gets a collection of conversions applied to the components obtained during the <see cref="Deconstruct"/> step.
        /// </summary>
        public ReadOnlyCollection<Conversion> Conversions { get; }

        /// <summary>
        /// Creates a new conversion that is like this one, but using the supplied children. If all of the children are the same, it will return this conversion.
        /// </summary>
        /// <param name="deconstruct">The <see cref="Deconstruct" /> property of the result.</param>
        /// <param name="conversions">The <see cref="Conversions" /> property of the result.</param>
        /// <returns>This conversion if no children changed, or an conversion with the updated children.</returns>
        public DeconstructionConversion Update(LambdaExpression deconstruct, IEnumerable<Conversion> conversions)
        {
            if (deconstruct == Deconstruct && conversions == Conversions)
            {
                return this;
            }

            return CSharpExpression.Deconstruct(deconstruct, conversions);
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override Conversion Accept(CSharpExpressionVisitor visitor) => visitor.VisitDeconstructionConversion(this);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="DeconstructionConversion" />.
        /// </summary>
        /// <param name="node">The conversion to visit.</param>
        /// <returns>The modified conversion, if it or any subexpression was modified; otherwise, returns the original conversion.</returns>
        protected internal virtual Conversion VisitDeconstructionConversion(DeconstructionConversion node)
        {
            return node.Update(VisitAndConvert(node.Deconstruct, nameof(VisitDeconstructionConversion)), Visit(node.Conversions, VisitConversion));
        }
    }

    partial class CSharpExpression
    {
        // CONSIDER: Should we introduce CSharpConversion and use that as the factory?

        /// <summary>
        /// Creates a deconstruction conversion for a tuple type using the specified <paramref name="conversions"/> to apply to the tuple components.
        /// </summary>
        /// <param name="conversions">The conversions to apply to the tuple components.</param>
        /// <returns>A <see cref="DeconstructionConversion"/> object representing the deconstruction of a tuple.</returns>
        public static DeconstructionConversion Deconstruct(params Conversion[] conversions) => Deconstruct(deconstruct: null, (IEnumerable<Conversion>)conversions);

        /// <summary>
        /// Creates a deconstruction conversion for a tuple type using the specified <paramref name="conversions"/> to apply to the tuple components.
        /// </summary>
        /// <param name="conversions">The conversions to apply to the tuple components.</param>
        /// <returns>A <see cref="DeconstructionConversion"/> object representing the deconstruction of a tuple.</returns>
        public static DeconstructionConversion Deconstruct(IEnumerable<Conversion> conversions) => Deconstruct(deconstruct: null, conversions);

        /// <summary>
        /// Creates a deconstruction conversion using the specified <paramref name="conversions"/> to apply to the components obtained through deconstruction.
        /// </summary>
        /// <param name="deconstruct">The deconstruction lambda to invoke if the input is not a tuple type.</param>
        /// <param name="conversions">The conversions to apply to the tuple components.</param>
        /// <returns>A <see cref="DeconstructionConversion"/> object representing the deconstruction of an object.</returns>
        public static DeconstructionConversion Deconstruct(LambdaExpression deconstruct, params Conversion[] conversions) => Deconstruct(deconstruct, (IEnumerable<Conversion>)conversions);

        /// <summary>
        /// Creates a deconstruction conversion using the specified <paramref name="conversions"/> to apply to the components obtained through deconstruction.
        /// </summary>
        /// <param name="deconstruct">The deconstruction lambda to invoke if the input is not a tuple type.</param>
        /// <param name="conversions">The conversions to apply to the tuple components.</param>
        /// <returns>A <see cref="DeconstructionConversion"/> object representing the deconstruction of an object.</returns>
        public static DeconstructionConversion Deconstruct(LambdaExpression deconstruct, IEnumerable<Conversion> conversions)
        {
            var conversionsList = conversions.ToReadOnly();

            RequiresNotNullItems(conversionsList, nameof(conversions));

            if (deconstruct != null)
            {
                CheckDeconstructLambda(deconstruct.Body, deconstruct.Parameters);

                var n = deconstruct.Parameters.Count;

                if (n - 1 != conversionsList.Count)
                    throw Error.DeconstructionParameterCountShouldMatchConversionCount();

                for (var i = 1; i < n; i++)
                {
                    var parameter = deconstruct.Parameters[i];
                    var conversion = conversionsList[i - 1];

                    if (!TypeUtils.AreReferenceAssignable(conversion.InputType, parameter.Type))
                        throw Error.DeconstructionParameterNotAssignableToConversion(i, parameter.Type, conversion.InputType);
                }
            }

            return new DeconstructionConversion(deconstruct, conversionsList);
        }

        /// <summary>
        /// Creates a lambda expression representing a deconstruction of an object, typically involving the invocation of a <c>Deconstruct</c> method.
        /// </summary>
        /// <remarks>
        /// A deconstruction lambda accepts an object in its first parameter and deconstructs it into the subsequent parameters which are passed by reference.
        /// </remarks>
        /// <param name="body">The body of the lambda expression performing the deconstruction.</param>
        /// <param name="parameters">The parameters of the lambda expression, where the first parameter represents the object to deconstruct, and subsequent parameters are output parameter receiving the components returned by the deconstruction.</param>
        /// <returns>A lambda expression representing the deconstruction of an object.</returns>
        public static LambdaExpression DeconstructLambda(Expression body, params ParameterExpression[] parameters)
        {
            RequiresCanRead(body, nameof(body));
            RequiresNotNullItems(parameters, nameof(parameters));

            CheckDeconstructLambda(body, parameters);

            var types = new Type[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                types[i] = parameters[i].Type;
            }

            var delegateType = DeconstructActionDelegateHelpers.GetDeconstructActionType(types);

            // NB: Duplicate parameters will be caught by the Lambda factory.

            return delegateType != null ? Lambda(delegateType, body, parameters) : Lambda(body, parameters);
        }

        private static void CheckDeconstructLambda(Expression body, IList<ParameterExpression> parameters)
        {
            if (body.Type != typeof(void))
                throw Error.DeconstructionShouldReturnVoid();

            if (parameters.Count < 3)
                throw Error.DeconstructionShouldHaveThreeOrMoreParameters();

            for (int i = 1, n = parameters.Count; i < n; i++)
            {
                if (!parameters[i].IsByRef)
                    throw Error.DeconstructionParameterShouldBeByRef(i);
            }
        }
    }
}
