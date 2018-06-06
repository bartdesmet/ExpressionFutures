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
        //           Note this intertwines with existing block optimizations, so we have to be careful. Some options are:
        //
        //           1. Use existing surrounding blocks (provided we don't cross lambda boundaries, where we introduce closures) and add
        //              new temporary variables. Temporary variables of the same type can be reused for many discards of compatible types.
        //           2. Introduce a new block, e.g. if no existing candidate block is found, to hold a temporary variable.
        //
        //           Note we still need a reduction for DiscardCSharpExpression that produces a valid assignment target in case the
        //           optimizer is not run. Obviously we can get rid of this restriction if we can push down this node type into the BCL.
    }
}
