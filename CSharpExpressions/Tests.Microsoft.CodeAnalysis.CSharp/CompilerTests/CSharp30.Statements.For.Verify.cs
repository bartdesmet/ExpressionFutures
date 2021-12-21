// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp30_Statements_For
    {
        partial class Reviewed
        {
            public override void CompilerTest_25E2_35E6() => OK(); // REVIEW: Infinite loop, so no CSharpBlock at the top level?
            public override void CompilerTest_AD48_86CB() => OK();
            public override void CompilerTest_5EF7_9418() => OK();
            public override void CompilerTest_26D5_E9FE() => OK();
            public override void CompilerTest_F7F3_AD6D() => OK();
        }
    }
}
