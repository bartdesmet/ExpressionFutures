// Prototyping extended expression trees for C#.
//
// bartde - May 2020

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp70_TupleLiterals
    {
        partial class Reviewed
        {
            // Literal
            public override void CompilerTest_4091_A89E() => OK();
            public override void CompilerTest_ACFD_39F6() => OK();
            public override void CompilerTest_694F_FB96() => OK();

            // Access
            public override void CompilerTest_2E96_C81B() => OK();
            public override void CompilerTest_B949_DD0E() => OK();
            public override void CompilerTest_BC9B_C81B() => OK(); // REVIEW: Use of 'Item1' instead of having tuple name info for 'x'.
            public override void CompilerTest_8000_DD0E() => OK(); // REVIEW: Use of 'Item2' instead of having tuple name info for 's'.
            public override void CompilerTest_2A80_EB79() => OK(); // REVIEW: Use of 'Rest.Item3' instead of having tuple name info for 'j'.
        }
    }
}
