// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace Microsoft.CSharp.Expressions
{
    public abstract partial class Conversion
    {
        // CONSIDER: Add some type info for the input and output?

        protected internal abstract Conversion Accept(CSharpExpressionVisitor visitor);
    }

    partial class CSharpExpressionVisitor
    {
        protected internal virtual Conversion VisitConversion(Conversion node)
        {
            return node.Accept(this);
        }
    }
}
