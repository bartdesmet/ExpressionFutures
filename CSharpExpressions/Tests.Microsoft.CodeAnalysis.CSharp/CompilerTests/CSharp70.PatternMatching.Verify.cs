// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp70_PatternMatching
    {
        partial class Reviewed
        {
            // var
            public override void CompilerTest_1065_9158() => OK();
            public override void CompilerTest_9792_2260() => OK();
            public override void CompilerTest_0C5C_E70E() => OK();

            // Declaration
            public override void CompilerTest_6A5C_E12F() => OK();
            public override void CompilerTest_F64B_2FFA() => OK();
            public override void CompilerTest_C2E0_5E14() => OK();

            // Constant - Null
            public override void CompilerTest_E7AD_1A3A() => OK();
            public override void CompilerTest_8AB8_BC27() => OK();
            public override void CompilerTest_DD5E_7EDD() => OK();

            // Constant - Int32
            public override void CompilerTest_BCC8_0566() => OK();
            public override void CompilerTest_9EC8_A4DC() => OK();
            public override void CompilerTest_6BE1_D655() => OK();

            // Constant - Misc
            public override void CompilerTest_6C86_EFA0() => OK();
            public override void CompilerTest_06E8_C93B() => OK();
            public override void CompilerTest_D3F0_6F64() => OK();
            public override void CompilerTest_2177_7A0C() => OK();
            public override void CompilerTest_4EE9_55F5() => OK();
        }
    }
}
