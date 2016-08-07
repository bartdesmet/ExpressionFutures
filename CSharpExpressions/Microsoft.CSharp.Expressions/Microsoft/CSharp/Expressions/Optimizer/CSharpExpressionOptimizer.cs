// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System.Collections.Generic;

namespace Microsoft.CSharp.Expressions.Compiler
{
    // TODO: We could invoke this optimizer in the Reduce method of nodes in order to optimize
    //       the expression prior to being used by the LambdaCompiler in LINQ.
    //
    //       Unfortunately, the Compile method doesn't have a flag to indicate the optimization
    //       level, which we could benefit from if it would get passed down to Reduce. If we
    //       decide it's fine to run the optimization for every Reduce, we could invoke our
    //       optimization step from CSharpExpression.Reduce prior to and/or after performing
    //       the core reduction logic for the node.
    //
    //       Note that Reduce is quite a heavy step already although it often does not involve
    //       recursion into children (with the exception of nodes such as ConditionalAccess) so
    //       we would be adding a tree walk and potentially a tree rewrite to the Reduce step.
    //       Given that the compiler already does things such as stack spilling which can cause
    //       complete rewrites of the tree, the additional cost of allocations could be deemed
    //       worth it in exchange for good optimizations (to be measured, but repeated invocation
    //       of compiled expressions is a common use case).
    //
    //       If we invoke the optimizer during Reduce, we should proceed a bit careful though. In
    //       particular, given that reduction proceeds top-down, it is possible we repeat the
    //       same optimization pass over child nodes repeatedly. Consider the following case:
    //
    //               LINQEXP
    //                  |
    //                CSEXP
    //               /     \
    //          LINQEXP   LINQEXP
    //             |
    //           CSEXP
    //
    //       As the lambda compiler reduces the top-level CSEXP node, our optimizer can recurse
    //       over the child nodes causing child nodes to get rewritten. If this rewrite keeps a
    //       child CSEXP node, the continued top-down reduction driven by the lambda compiler will
    //       encounter this child CSEXP node and its Reduce method will re-invoke the optimizer.
    //
    //       If the top-level optimization has already ensured this node is optimal (why wouldn't
    //       it, given that it has even more context than the child node would have!), another
    //       pass over it is plain wasteful. A few strategies are possible:
    //
    //         - perform the deep optimization and don't worry about the additional passes over
    //           child nodes, at least ensuring those will be no-ops (need to reach a fixed point
    //           in the optimization to guarantee this); however, the number of passes over child
    //           nodes can get quite high given it's dependent on the depth of the tree
    //          
    //         - make optimization shallow, i.e. don't ensure child nodes don't get optimized, in
    //           order to make a subsequent Reduce of child nodes non-trivial and causing every
    //           node to be optimized only once; this makes optimization more local (can't take
    //           advantage of parent node context) which could be less effective
    //
    //         - make optimization deep but insert marker nodes around child nodes denoting those
    //           as already optimized; upon the continued recursive reduction by the lamba
    //           compiler, those marker nodes would simply reduce their child node without any
    //           additional attempt at reduction
    //
    //       The last strategy seems the most promising if we consider optimization only to take
    //       place in the context of the lambda compiler. However, if a user is in charge of an
    //       optimization by means of calling Reduce explicitly or by calling some Optimize method
    //       we expose (as we do right now to test this functionality, but it could make sense to
    //       keep it public given that optimization can make sense without compilation, e.g. when
    //       serializing an expression tree), those fake nodes seem cumbersome.
    //
    //       We can hit the best of both worlds by making the Optimize method perform a deep
    //       optimization but omitting marker nodes, while Reduce inserts those marker nodes. Only
    //       the case where a user calls Reduce is affected by this decision. Having the lambda
    //       compiler call a specialized Reduce method with an optimization level would be an ideal
    //       situation.
    //
    //       There's one alternative to all of this though; we could make the top-level CSEXP node
    //       responsible for doing a deep reduction when its Reduce method is called, proceeding
    //       simultaneously with an optimization step. Effectively we intertwine the reduction and
    //       optimization logic under the umbrella of Reduce.

    partial class CSharpExpressionOptimizer : CSharpExpressionVisitor
    {
        private static List<T> Clone<T>(IList<T> source, int maxIndex)
        {
            var res = new List<T>(maxIndex);

            for (var i = 0; i < maxIndex; i++)
            {
                res.Add(source[i]);
            }

            return res;
        }
    }
}
