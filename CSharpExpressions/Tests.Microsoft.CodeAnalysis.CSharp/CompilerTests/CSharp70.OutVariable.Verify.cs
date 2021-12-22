// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp70_OutVariable
    {
        partial class Reviewed
        {
            // Out variable
            public override void CompilerTest_C10F_4079() => OK(); // REVIEW: Extra block gets introduced due to lack of Locals on Lambda.
            public override void CompilerTest_3002_1169() => OK(); // REVIEW: See above.

            //
            // CONSIDER: To keep the tree isomorphic to the user's code, we could either add Locals to the Lambda node,
            //           or introduce a notion of a CompilerGenerated flag on nodes that are introduced due to partial
            //           lowering prior to quotation. This may also be useful to annotate implicit conversions etc.
            //
        }
    }
}
