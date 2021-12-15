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
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    public sealed partial class DeconstructionConversion : Conversion
    {
        internal DeconstructionConversion(LambdaExpression deconstruct, ReadOnlyCollection<Conversion> conversions)
        {
            Deconstruct = deconstruct;
            Conversions = conversions;
        }

        public LambdaExpression Deconstruct { get; }
        public ReadOnlyCollection<Conversion> Conversions { get; }

        public DeconstructionConversion Update(LambdaExpression deconstruct, IEnumerable<Conversion> conversions)
        {
            if (deconstruct == this.Deconstruct && conversions == this.Conversions)
            {
                return this;
            }

            return CSharpExpression.Deconstruct(deconstruct, conversions);
        }

        protected internal override Conversion Accept(CSharpExpressionVisitor visitor) => visitor.VisitDeconstructionConversion(this);
    }

    partial class CSharpExpressionVisitor
    {
        protected internal virtual Conversion VisitDeconstructionConversion(DeconstructionConversion node)
        {
            return node.Update(VisitAndConvert(node.Deconstruct, nameof(VisitDeconstructionConversion)), Visit(node.Conversions, VisitConversion));
        }
    }

    partial class CSharpExpression
    {
        // CONSIDER: Should we introduce CSharpConversion and use that as the factory?

        public static DeconstructionConversion Deconstruct(params Conversion[] conversions) => Deconstruct((IEnumerable<Conversion>)conversions);

        public static DeconstructionConversion Deconstruct(IEnumerable<Conversion> conversions)
        {
            return new DeconstructionConversion(deconstruct: null, conversions.ToReadOnly());
        }

        public static DeconstructionConversion Deconstruct(LambdaExpression deconstruct, params Conversion[] conversions) => Deconstruct(deconstruct, (IEnumerable<Conversion>)conversions);

        public static DeconstructionConversion Deconstruct(LambdaExpression deconstruct, IEnumerable<Conversion> conversions)
        {
            // Check deconstruct has N + 1 parameters, last N are out, returns void.
            // Check N output parameters can be bound to the conversions inputs.

            return new DeconstructionConversion(deconstruct, conversions.ToReadOnly());
        }

        public static LambdaExpression DeconstructLambda(Expression body, params ParameterExpression[] parameters)
        {
            RequiresCanRead(body, nameof(body));
            RequiresNotNullItems(parameters, nameof(parameters));

            if (body.Type != typeof(void))
                throw new Exception(); // TODO

            if (parameters.Length < 3)
                throw new Exception(); // TODO

            var types = new Type[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                if (i >= 1 && !parameters[i].IsByRef)
                    throw new Exception(); // TODO

                types[i] = parameters[i].Type;
            }

            var delegateType = DeconstructActionDelegateHelpers.GetDeconstructActionType(types);

            return delegateType != null ? Lambda(delegateType, body, parameters) : Lambda(body, parameters);
        }
    }
}
