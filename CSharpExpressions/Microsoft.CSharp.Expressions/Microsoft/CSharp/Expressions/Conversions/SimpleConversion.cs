// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Linq.Expressions;

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
            // Check single parameter.

            return new SimpleConversion(conversion);
        }
    }
}
