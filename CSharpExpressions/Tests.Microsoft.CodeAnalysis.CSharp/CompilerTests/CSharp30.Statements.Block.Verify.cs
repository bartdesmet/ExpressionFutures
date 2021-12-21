// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp30_Statements_Block
    {
        partial class Reviewed
        {
            public override void CompilerTest_A8D0_49C3() => OK();
            public override void CompilerTest_197A_C7FA() => OK();
            public override void CompilerTest_27AA_4144() => OK(); // DESIGN: Use of CSharpBlock versus Block
        }
    }
}
