// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Linq.Expressions;
using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    public sealed partial class SimpleConversion : Conversion
    {
        internal SimpleConversion(LambdaExpression conversion)
        {
            Conversion = conversion;
        }

        public LambdaExpression Conversion { get; }

        public SimpleConversion Update(LambdaExpression conversion)
        {
            if (conversion == this.Conversion)
            {
                return this;
            }

            return CSharpExpression.Convert(conversion);
        }

        protected internal override Conversion Accept(CSharpExpressionVisitor visitor) => visitor.VisitSimpleConversion(this);
    }

    partial class CSharpExpressionVisitor
    {
        protected internal virtual Conversion VisitSimpleConversion(SimpleConversion node)
        {
            return node.Update(VisitAndConvert(node.Conversion, nameof(VisitSimpleConversion)));
        }
    }

    partial class CSharpExpression
    {
        // CONSIDER: Should we introduce CSharpConversion and use that as the factory?

        public static SimpleConversion Convert(LambdaExpression conversion)
        {
            RequiresNotNull(conversion, nameof(conversion));

            if (conversion.Parameters.Count != 1)
                throw new Exception(); // TODO

            if (conversion.ReturnType == typeof(void))
                throw new Exception(); // TODO

            return new SimpleConversion(conversion);
        }

        // CONSIDER: Add specialized derived types of SimpleConversion for trivial conversions:
        //
        //             Identity:          (T x) => x
        //             Convert[Checked]:  (T x) => (R)x    with/without Method
        //             Nullable:          (T x) => (T?)x
    }
}
