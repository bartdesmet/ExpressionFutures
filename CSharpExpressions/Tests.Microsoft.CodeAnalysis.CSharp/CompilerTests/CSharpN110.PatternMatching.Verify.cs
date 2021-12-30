// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp110_PatternMatching
    {
        partial class Reviewed
        {
            // List pattern - Array
            public override void CompilerTest_62B2_B138() => OK();
            public override void CompilerTest_5F9A_3106() => OK();
            public override void CompilerTest_78B5_4AC6() => OK();
            public override void CompilerTest_D6FE_2688() => OK();

            // List pattern - String
            public override void CompilerTest_5C89_352D() => OK();
            public override void CompilerTest_3064_1ACC() => OK();
            public override void CompilerTest_57A5_FF9E() => OK();

            // List pattern - List
            public override void CompilerTest_A02D_6A56() => OK();
            public override void CompilerTest_ED9D_BF0C() => OK();
            public override void CompilerTest_FB44_8752() => OK();
            public override void CompilerTest_3A56_04D4() => OK();

            // Slice pattern - Array
            public override void CompilerTest_ADF5_FF71() => OK();
            public override void CompilerTest_30F8_A1FF() => OK();
            public override void CompilerTest_DE25_4566() => OK();
            public override void CompilerTest_4E09_514D() => OK();
            public override void CompilerTest_A52D_B4CD() => OK();

            // Slice pattern - String
            public override void CompilerTest_D697_D368() => OK();
            public override void CompilerTest_E026_7511() => OK();
            public override void CompilerTest_9665_3D75() => OK();
            public override void CompilerTest_C735_7D9F() => OK();

            // Slice pattern - List
            public override void CompilerTest_D9D1_E051() => OK();
            public override void CompilerTest_1B3B_81F8() => OK();
            public override void CompilerTest_91EC_446C() => OK();
            public override void CompilerTest_8836_D862() => OK();
        }
    }
}
