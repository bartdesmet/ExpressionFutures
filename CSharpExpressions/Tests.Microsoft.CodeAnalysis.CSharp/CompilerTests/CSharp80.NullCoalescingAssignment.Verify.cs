// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp80_NullCoalescingAssignment
    {
        partial class Reviewed
        {
            // Null-coalescing assignment
            public override void CompilerTest_A465_3AAF() => OK();
            public override void CompilerTest_07E0_24A3() => OK();
            public override void CompilerTest_22AE_900C() => OK();

            // Null-coalescing assignment with dynamic
            public override void CompilerTest_0FE1_86B6() => OK();
            public override void CompilerTest_75E3_213A() => OK();
            public override void CompilerTest_9CD5_03D0() => OK();
            public override void CompilerTest_330E_2E05() => OK();
            public override void CompilerTest_B81A_515D() => OK();
            public override void CompilerTest_7AF5_0A0E() => OK();
            public override void CompilerTest_50DE_079F() => OK();
            public override void CompilerTest_FE8E_B94B() => OK();
            public override void CompilerTest_9440_85D5() => OK();
            public override void CompilerTest_2C1D_9A84() => OK();
            public override void CompilerTest_F57B_8C67() => OK();
            public override void CompilerTest_734B_65DC() => OK();
            public override void CompilerTest_BBC0_6EA4() => OK();
            public override void CompilerTest_D860_6F20() => OK();
            public override void CompilerTest_7A6C_546D() => OK();
        }
    }
}
