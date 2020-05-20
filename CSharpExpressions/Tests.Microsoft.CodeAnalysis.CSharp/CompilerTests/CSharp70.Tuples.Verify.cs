// Prototyping extended expression trees for C#.
//
// bartde - May 2020

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp70_Tuples
    {
        partial class Reviewed
        {
            // Literal
            public override void CompilerTest_4091_A89E() => OK();
            public override void CompilerTest_ACFD_39F6() => OK();
            public override void CompilerTest_694F_FB96() => OK();

            // Convert
            public override void CompilerTest_5948_DA1F() => OK(); // NB: Nullable conversion uses Convert
            public override void CompilerTest_D7DE_3D5E() => OK();
        }
    }
}
