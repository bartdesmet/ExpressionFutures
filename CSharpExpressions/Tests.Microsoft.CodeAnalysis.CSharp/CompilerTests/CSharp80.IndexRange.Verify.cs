// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp80_IndexRange
    {
        partial class Reviewed
        {
            // Index
            public override void CompilerTest_102D_8041() => OK(); // NB: Implicit conversion
            public override void CompilerTest_F139_15B7() => OK();
            public override void CompilerTest_A48D_CEC8() => OK();
            public override void CompilerTest_1228_4EE4() => OK();
            public override void CompilerTest_7287_5EFC() => OK();
            public override void CompilerTest_F0CB_598D() => OK();

            // Range
            public override void CompilerTest_16C2_7E40() => OK();
            public override void CompilerTest_9DAB_53E1() => OK();
            public override void CompilerTest_311C_4ADD() => OK();
            public override void CompilerTest_0C19_6AD1() => OK();
            public override void CompilerTest_1972_BC5D() => OK();
            public override void CompilerTest_6D28_52A3() => OK();
            public override void CompilerTest_2322_4FD7() => OK();
            public override void CompilerTest_96F6_B646() => OK();
            public override void CompilerTest_4A83_E78F() => OK();
            public override void CompilerTest_2C8C_A2E0() => OK();
            public override void CompilerTest_F408_CCEE() => OK();
            public override void CompilerTest_903C_C65A() => OK();
            public override void CompilerTest_E7FC_AC00() => OK();
            public override void CompilerTest_3718_C5C5() => OK();
            public override void CompilerTest_126E_F2C3() => OK();

            // Indexing
            public override void CompilerTest_838B_FA89() => OK();
            public override void CompilerTest_E7F0_C224() => OK();
            public override void CompilerTest_CF23_3FC2() => OK();

            // Slicing
            public override void CompilerTest_3669_DF79() => OK();
            public override void CompilerTest_3AF4_3ADE() => OK();
        }
    }
}
