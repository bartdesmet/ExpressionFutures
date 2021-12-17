// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp70_IsExpression_Reducing
    {
        partial class Reviewed
        {
            public override void CompilerTest_9704_1B54() => OK(); // REVIEW: The top-level compiler-generated block is awkward.
            public override void CompilerTest_BCC8_04B2() => OK();
        }
    }
}
