// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;
using System.Linq.Expressions.Compiler;

namespace Microsoft.CSharp.Expressions.Compiler
{
    // NB: This utility reuses the stack spiller from LINQ but extends it with support for Await nodes. An earlier
    //     version of this utility tried to reuse the stack spiller from LINQ without copying the code, namely by
    //     temporarily wrapping Await nodes in Try blocks in order for the stack spiller to cause spilling. This
    //     is too shallow though, because we'd still be faced with state machine jumps into expressions, e.g. for
    //     cases such as:
    //
    //         -(await x)    -->   no spilling but state machine jumps into '-' operator
    //         1 + await x   -->   spills '1' but state machine jumps into '+' operator
    //         await x + 1   -->   no spilling but state machine jumps into '+' operator
    //
    //     The modified spiller will correctly cause the parent nodes of any Await expression to perform spilling
    //     of operands in order to make sure that the jump to the resumption point of an Await expression always
    //     has an empty evaluation stack.
    //
    //     See the history of this file to see the original temporary approach reusing the LINQ stack spiller as-is
    //     and see StackSpiller.cs for the modified version (with minimal changes in order to make diffing between
    //     the original version and our copy easier).

    /// <summary>
    /// Utility to perform stack spilling to ensure an empty evaluation stack upon starting the evaluation of an
    /// await expression.
    /// </summary>
    /// <remarks>
    /// An example of stack spilling is shown below:
    /// <code>
    ///   F(A, await T, B)
    /// </code>
    /// In here, A and T are evaluated before awaiting T. The result of evaluating those subexpressions has to be
    /// stored prior to performing the await. In case the asynchronous code path is picked, all this intermediate
    /// evaluation state needs to be kept on the heap in order to restore it after the asynchronous operation
    /// completes and prior to evaluating B. Stack spilling will effectively turn the code into:
    /// <code>
    ///   var __1 = A;
    ///   var __2 = T;
    ///   var __3 = await __2;
    ///   F(__1, __3, B)
    /// </code>
    /// where the compiler-generated variables will be hoisted.
    /// </remarks>
    internal static class Spiller
    {
        public static Expression Spill(Expression expression)
        {
            var spilled = StackSpiller.AnalyzeLambda(Expression.Lambda(expression)).Body;
            return spilled;
        }
    }
}
