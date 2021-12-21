// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp30_Statements_Do
    {
        partial class Reviewed
        {
            public override void CompilerTest_6674_1E31() => OK(); // REVIEW: Infinite loop, so no CSharpBlock at the top level?
            public override void CompilerTest_B6D5_79C3() => OK();
        }
    }
}
