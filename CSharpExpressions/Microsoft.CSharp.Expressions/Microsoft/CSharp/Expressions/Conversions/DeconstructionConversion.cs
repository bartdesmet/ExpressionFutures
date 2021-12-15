// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;

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
            return Lambda(body, parameters);
        }
    }
}
