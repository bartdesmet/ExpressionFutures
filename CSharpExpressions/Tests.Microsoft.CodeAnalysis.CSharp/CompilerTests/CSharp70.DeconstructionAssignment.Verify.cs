// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp70_DeconstructionAssignment
    {
        partial class Reviewed
        {
            // Assign - tuple
            public override void CompilerTest_B78B_4415() => OK();
            public override void CompilerTest_1D1C_97F0() => OK();

            // Assign - tuple nested
            public override void CompilerTest_6DA3_CACF() => OK();
            public override void CompilerTest_130F_B859() => OK();

            // Assign - deconstruct method
            public override void CompilerTest_702A_64C8() => OK();
            public override void CompilerTest_D0CE_7364() => OK();

            // Assign - variable targets
            public override void CompilerTest_AF2A_5B45() => OK();

            // ForEach
            public override void CompilerTest_6624_ABDB() => OK();
            public override void CompilerTest_CB13_ABDB() => OK();
            public override void CompilerTest_80A1_7FC7() => OK();
            public override void CompilerTest_C73C_5413() => OK();
        }
    }
}
