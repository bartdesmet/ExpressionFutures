// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp90_PatternMatching
    {
        partial class Reviewed
        {
            // Relational
            public override void CompilerTest_EF4D_84CA() => OK();
            public override void CompilerTest_F939_C326() => OK();
            public override void CompilerTest_13ED_00A4() => OK();
            public override void CompilerTest_58D1_B6D4() => OK();
            public override void CompilerTest_3F05_79B5() => OK();
            public override void CompilerTest_645C_AE55() => OK();
            public override void CompilerTest_A294_ED2A() => OK();
            public override void CompilerTest_AD89_871F() => OK();

            // And
            public override void CompilerTest_ADBD_4404() => OK();

            // Or
            public override void CompilerTest_9DC2_F0F3() => OK();

            // Not
            public override void CompilerTest_0343_2E7E() => OK();
            public override void CompilerTest_B907_F017() => OK();

            // MSDN samples
            public override void CompilerTest_B4EA_1FE3() => OK();
            public override void CompilerTest_39C5_8985() => OK();
        }
    }
}
