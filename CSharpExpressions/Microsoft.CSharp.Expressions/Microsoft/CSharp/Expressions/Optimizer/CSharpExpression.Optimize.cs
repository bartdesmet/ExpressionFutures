// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.CSharp.Expressions.Compiler;

namespace System.Linq.Expressions
{
    // DESIGN: It'd be great to split this functionality across LINQ and C# so all expression trees
    //         can benefit from optimization steps. This is yet another case where we need visitors
    //         that initiate a visit in LINQ APIs and dispatch into extension nodes without causing
    //         reduction of nodes (cf. ToCSharp). We may want to generalize this mechanism in a way
    //         that avoids heavy allocation patterns of extension node visitors while also reducing
    //         the number of domain-specific visitor interfaces that all domains have to aware of.

    /// <summary>
    /// Provides extensions methods for expressions to apply optimizations.
    /// </summary>
    public static class ExpressionOptimizationExtensions
    {
        /// <summary>
        /// Optimizes the expression to a more optimal (smaller and/or faster) form.
        /// </summary>
        /// <param name="expression">The expression to optimize.</param>
        /// <returns>The optimized expression.</returns>
        public static Expression Optimize(this Expression expression)
        {
            var optimizer = new CSharpExpressionOptimizer();

            var res = optimizer.Visit(expression);

            return res;
        }
    }
}
