// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents an expression that has a unary operator.
    /// </summary>
    public abstract class UnaryCSharpExpression : CSharpExpression
    {
        internal UnaryCSharpExpression(Expression operand)
        {
            Operand = operand;
        }

        /// <summary>
        /// Gets the operand of the unary operation.
        /// </summary>
        /// <returns>An <see cref="Expression" /> that represents the operand of the unary operation.</returns>
        public Expression Operand { get; }
    }
}
