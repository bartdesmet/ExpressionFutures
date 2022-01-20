// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp100_InterpolatedStringHandlers
    {
        partial class Reviewed
        {
            // Basics
            public override void CompilerTest_87DF_089A() => OK();
            public override void CompilerTest_7395_1C24() => OK();
            public override void CompilerTest_7B35_4D2E() => OK();
            
            // Alignment and format
            public override void CompilerTest_856E_8640() => OK();
            public override void CompilerTest_3CDF_C020() => OK();
            public override void CompilerTest_A49F_41E1() => OK();

            // Construction with `out bool` and Append methods returning bool.
            public override void CompilerTest_C2F4_B3FA() => OK();
            public override void CompilerTest_D484_7A71() => OK();
            public override void CompilerTest_A221_B2CB() => OK();
            
            // Dynamic
            public override void CompilerTest_A227_1CB0() => OK();
            public override void CompilerTest_B033_4813() => OK();
            public override void CompilerTest_1D5D_1392() => OK();
            public override void CompilerTest_A6E0_98ED() => OK();

            // Binary string concat
            public override void CompilerTest_56C2_F36B() => OK();
            public override void CompilerTest_3809_1F73() => OK();

            // Argument indices
            public override void CompilerTest_1FF7_87E2() => OK();
        }
    }
}
