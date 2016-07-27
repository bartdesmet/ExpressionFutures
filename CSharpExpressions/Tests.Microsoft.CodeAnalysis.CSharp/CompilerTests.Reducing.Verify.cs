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
            public override void CompilerTest_BFB2_C4CC() => OK();
            public override void CompilerTest_4609_DFEE() => OK();

            // Assign
            public override void CompilerTest_579D_1565() => OK();
            public override void CompilerTest_93F1_0B14() => OK();
            public override void CompilerTest_2F3A_3301() => OK();
            public override void CompilerTest_430E_E061() => OK();
            public override void CompilerTest_AEF8_513D() => OK();

            // Compound assign
            public override void CompilerTest_044F_E9C5() => OK();
            public override void CompilerTest_E0C9_5074() => OK();
            public override void CompilerTest_7C94_4A5D() => OK();
            public override void CompilerTest_79AE_D22A() => OK();
            public override void CompilerTest_08DA_1642() => OK();

            // Prefix unary assign
            public override void CompilerTest_2115_E9C5() => OK();
            public override void CompilerTest_89F1_5074() => OK();
            public override void CompilerTest_205F_4A5D() => OK();
            public override void CompilerTest_FB63_D22A() => OK();
            public override void CompilerTest_30FE_1642() => OK();

            // Postfix unary assign
            public override void CompilerTest_00CF_E2F6() => OK();
            public override void CompilerTest_FE9D_CA70() => OK();
            public override void CompilerTest_BB88_996D() => OK();
            public override void CompilerTest_55F5_5F50() => OK(); // REVIEW: Additional block emitted
            public override void CompilerTest_0564_DD7A() => OK();

            // Lock
            public override void CompilerTest_9551_2A52() => OK();
        }
    }
}
