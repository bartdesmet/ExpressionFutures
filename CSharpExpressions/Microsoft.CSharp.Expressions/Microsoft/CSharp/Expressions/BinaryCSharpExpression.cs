// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents an expression that has a binary operator.
    /// </summary>
    public abstract partial class BinaryCSharpExpression : CSharpExpression
    {
        internal BinaryCSharpExpression(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Gets the left operand of the binary operation.
        /// </summary>
        /// <returns>An <see cref="Expression" /> that represents the left operand of the binary operation.</returns>
        public Expression Left { get; }

        /// <summary>
        /// Gets the right operand of the binary operation.
        /// </summary>
        /// <returns>An <see cref="Expression" /> that represents the left operand of the binary operation.</returns>
        public Expression Right { get; }

        // DESIGN: Add other properties a la LINQ? E.g. our Assign nodes have a bunch of properties that could move up.
        //         Sticking with a class hierarchy with more layers of specialization for now.
    }
}
