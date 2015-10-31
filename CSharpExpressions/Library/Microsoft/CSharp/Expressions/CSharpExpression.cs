// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Base class for C#-specific expression tree nodes.
    /// </summary>
    public abstract partial class CSharpExpression : Expression
    {
        internal CSharpExpression()
        {
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />.
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public abstract CSharpExpressionType CSharpNodeType
        {
            get;
        }

        /// <summary>
        /// Gets a value that indicates whether the expression tree node can be reduced. 
        /// </summary>
        public sealed override bool CanReduce
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
        /// </summary>
        public sealed override ExpressionType NodeType
        {
            get
            {
                return ExpressionType.Extension;
            }
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal abstract Expression Accept(CSharpExpressionVisitor visitor);
    }
}
