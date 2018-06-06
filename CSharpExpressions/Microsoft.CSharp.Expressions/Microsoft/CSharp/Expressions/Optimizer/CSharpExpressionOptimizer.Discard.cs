// Prototyping extended expression trees for C#.
//
// bartde - June 2018

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    partial class CSharpExpressionOptimizer
    {
        // NB: This optimization removes unnecessary discards.

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitBinaryAssign(AssignBinaryCSharpExpression node)
        {
            if (node.CSharpNodeType == CSharpExpressionType.Assign)
            {
                if (node.Left is DiscardCSharpExpression)
                {
                    return Visit(node.Right);
                }
            }

            return base.VisitBinaryAssign(node);
        }

        // CONSIDER: We could add another optimization that keeps track of block expressions and turns discards into temporaries in blocks.
        //           Note this intertwines with existing block optimizations, so we have to be careful.
    }
}
