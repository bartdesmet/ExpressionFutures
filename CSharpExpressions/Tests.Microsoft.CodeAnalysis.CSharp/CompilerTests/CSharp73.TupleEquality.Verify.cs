// Prototyping extended expression trees for C#.
//
// bartde - May 2020

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp73_TupleEquality
    {
        partial class Reviewed
        {
            // Non-null
            public override void CompilerTest_41C6_D6EC() => OK();
            public override void CompilerTest_1183_EE62() => OK();
            public override void CompilerTest_E39A_78A2() => OK();
            public override void CompilerTest_9A78_F30E() => OK();
            public override void CompilerTest_B73C_68B9() => OK();
            public override void CompilerTest_530F_CF5A() => OK();

            // Null
            public override void CompilerTest_E4CE_9D5B() => OK();
            public override void CompilerTest_0B1F_7212() => OK();

            // Nested
            public override void CompilerTest_A78C_7F57() => OK();
            public override void CompilerTest_235E_24DF() => OK();
            public override void CompilerTest_D6B8_C461() => OK();
            public override void CompilerTest_5CFE_A5A4() => OK();
            public override void CompilerTest_27FD_7B61() => OK();
            public override void CompilerTest_157E_20F1() => OK();
            public override void CompilerTest_2CC6_948A() => OK();
            public override void CompilerTest_B81E_0DAE() => OK();

            // Spurious conversions
            public override void CompilerTest_BC36_A04C() => OK(); // NB: Widening conversion of (char, byte) to (int, int) occurs in tree.
        }
    }
}
