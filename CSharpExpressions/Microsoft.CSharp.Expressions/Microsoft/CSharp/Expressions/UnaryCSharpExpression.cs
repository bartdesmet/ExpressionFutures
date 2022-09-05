// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents an expression that has a unary operator.
    /// </summary>
    public abstract partial class UnaryCSharpExpression : CSharpExpression
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

        // DESIGN: Add other properties a la LINQ? E.g. Method would make sense for our Await node even.
        //         Sticking with a class hierarchy with more layers of specialization for now.
    }
}
