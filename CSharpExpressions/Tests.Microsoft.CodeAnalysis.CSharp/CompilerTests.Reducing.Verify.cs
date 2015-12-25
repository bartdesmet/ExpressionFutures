// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests
    {
        partial class Reviewed
        {
            // By-ref with named parameters
            public override void CompilerTest_6A97_3CC7() => OK();
            public override void CompilerTest_BFB2_9E66() => OK();
            public override void CompilerTest_4609_D626() => OK();

            // Assign
            public override void CompilerTest_579D_8D67() => OK();
            public override void CompilerTest_93F1_077C() => OK();
            public override void CompilerTest_2F3A_1F59() => OK();
            public override void CompilerTest_430E_BB89() => OK();
            public override void CompilerTest_AEF8_094C() => OK();

            // Compound assign
            public override void CompilerTest_044F_5A03() => OK();
            public override void CompilerTest_E0C9_816B() => OK();
            public override void CompilerTest_7C94_EEF3() => OK();
            public override void CompilerTest_79AE_F9F6() => OK();
            public override void CompilerTest_08DA_705B() => OK();

            // Prefix unary assign
            public override void CompilerTest_2115_5A03() => OK();
            public override void CompilerTest_89F1_816B() => OK();
            public override void CompilerTest_205F_EEF3() => OK();
            public override void CompilerTest_FB63_F9F6() => OK();
            public override void CompilerTest_30FE_705B() => OK();

            // Postfix unary assign
            public override void CompilerTest_00CF_4CB4() => OK();
            public override void CompilerTest_FE9D_1C3B() => OK();
            public override void CompilerTest_BB88_FBEA() => OK();
            public override void CompilerTest_55F5_73B2() => OK(); // REVIEW: Additional block emitted
            public override void CompilerTest_0564_D471() => OK();

            // Lock
            public override void CompilerTest_9551_2A52() => OK();
        }
    }
}
