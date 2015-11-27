// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests
    {
        partial class Reviewed
        {
            // C# 3.0 supported expressions
            public override void CompilerTest_9D30_AA02() => OK();
            public override void CompilerTest_3ECF_6910() => OK();

            // Multi-dimensional array initializers
            public override void CompilerTest_F51F_71B6() => OK();
            public override void CompilerTest_E70E_4B35() => OK();
            public override void CompilerTest_59A0_FFB9() => OK();
            public override void CompilerTest_789A_453A() => OK();

            // Named parameters for calls
            public override void CompilerTest_E9F4_7C15() => OK();
            public override void CompilerTest_4EB1_83FD() => OK();
            public override void CompilerTest_C437_AA4C() => OK();
            public override void CompilerTest_4C39_BCFC() => OK();

            // Block
            public override void CompilerTest_A8D0_49C3() => OK();
            public override void CompilerTest_197A_9EF8() => OK();
            public override void CompilerTest_27AA_544E() => OK(); // DESIGN: Use of CSharpBlock versus Block

            // Empty
            public override void CompilerTest_0BD6_C135() => OK();
            public override void CompilerTest_7F95_E445() => OK();

            // Return
            public override void CompilerTest_6102_7F8E() => OK();
            public override void CompilerTest_AEF8_9F07() => OK();
            public override void CompilerTest_7381_AA02() => INCONCLUSIVE(); // TODO: Degenerates into an expression body

            // Label/Goto
            public override void CompilerTest_BBBC_A048() => INCONCLUSIVE(); // TODO: Misses label name
            public override void CompilerTest_6FC7_B707() => INCONCLUSIVE(); // TODO: Misses label name
        }
    }
}
