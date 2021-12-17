// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp30_Assignments_Reducing
    {
        partial class Reviewed
        {
            // Assign
            public override void CompilerTest_579D_1565() => OK();
            public override void CompilerTest_93F1_0B14() => OK();
            public override void CompilerTest_2F3A_3301() => OK();
            public override void CompilerTest_430E_E061() => OK(); // REVIEW: Spills xs to a temporary. Could be optimized.
            public override void CompilerTest_AEF8_513D() => OK();

            // Compound assign
            public override void CompilerTest_044F_E9C5() => OK();
            public override void CompilerTest_E0C9_5074() => OK(); // REVIEW: Spills xs to a temporary. Could be optimized.
            public override void CompilerTest_7C94_4A5D() => OK(); // REVIEW: Spills xs to a temporary. Could be optimized.
            public override void CompilerTest_79AE_D22A() => OK(); // REVIEW: Spills xs to a temporary. Could be optimized.
            public override void CompilerTest_08DA_1642() => OK(); // REVIEW: Spills b to a temporary. Could be optimized.

            // Prefix unary assign
            public override void CompilerTest_2115_E9C5() => OK();
            public override void CompilerTest_89F1_5074() => OK(); // REVIEW: Spills xs to a temporary. Could be optimized.
            public override void CompilerTest_205F_4A5D() => OK(); // REVIEW: Spills xs to a temporary. Could be optimized.
            public override void CompilerTest_FB63_D22A() => OK(); // REVIEW: Spills xs to a temporary. Could be optimized.
            public override void CompilerTest_30FE_1642() => OK(); // REVIEW: Spills b to a temporary. Could be optimized.

            // Postfix unary assign
            public override void CompilerTest_00CF_E2F6() => OK();
            public override void CompilerTest_FE9D_CA70() => OK(); // REVIEW: Spills xs to a temporary. Could be optimized.
            public override void CompilerTest_BB88_996D() => OK(); // REVIEW: Spills xs to a temporary. Could be optimized.
            public override void CompilerTest_55F5_5F50() => OK(); // REVIEW: Spills xs to a temporary. Could be optimized. Additional block emitted.
            public override void CompilerTest_0564_DD7A() => OK(); // REVIEW: Spills b to a temporary. Could be optimized.
        }
    }
}
