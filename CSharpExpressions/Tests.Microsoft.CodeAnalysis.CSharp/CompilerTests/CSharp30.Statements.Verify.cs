// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp30_Statements
    {
        partial class Reviewed
        {
            // Block
            public override void CompilerTest_A8D0_49C3() => OK();
            public override void CompilerTest_197A_C7FA() => OK();
            public override void CompilerTest_27AA_4144() => OK(); // DESIGN: Use of CSharpBlock versus Block

            // Empty
            public override void CompilerTest_0BD6_C135() => OK();
            public override void CompilerTest_7F95_E445() => OK();

            // Return
            public override void CompilerTest_6102_7F8E() => OK();
            public override void CompilerTest_AEF8_BB4B() => OK();
            public override void CompilerTest_7381_AA02() => INCONCLUSIVE(); // TODO: Degenerates into an expression body

            // Label/Goto
            public override void CompilerTest_BBBC_6128() => OK();
            public override void CompilerTest_6FC7_B4A6() => OK();

            // If
            public override void CompilerTest_C043_D2B0() => OK();
            public override void CompilerTest_2216_A3C9() => OK();
            public override void CompilerTest_6319_CF5C() => OK();
            public override void CompilerTest_1D89_F94D() => OK();
            public override void CompilerTest_B73D_03FA() => OK();
            public override void CompilerTest_5419_99E8() => OK();
            public override void CompilerTest_EB64_66C6() => OK();

            // While
            public override void CompilerTest_C90B_9C05() => OK(); // REVIEW: Infinite loop, so no CSharpBlock at the top level?
            public override void CompilerTest_C5C5_4E9F() => OK(); // REVIEW: Infinite loop, so no CSharpBlock at the top level?
            public override void CompilerTest_7D34_66D1() => OK();
            public override void CompilerTest_40EC_DA92() => OK(); // REVIEW: Infinite loop, so no CSharpBlock at the top level?
            public override void CompilerTest_FC9A_C797() => OK();
            public override void CompilerTest_6C15_9FA8() => OK();
            public override void CompilerTest_242C_68A7() => OK();
            public override void CompilerTest_2503_AEF6() => OK();

            // Do
            public override void CompilerTest_6674_1E31() => OK(); // REVIEW: Infinite loop, so no CSharpBlock at the top level?
            public override void CompilerTest_B6D5_79C3() => OK();

            // For
            public override void CompilerTest_25E2_35E6() => OK(); // REVIEW: Infinite loop, so no CSharpBlock at the top level?
            public override void CompilerTest_AD48_86CB() => OK();
            public override void CompilerTest_5EF7_9418() => OK();
            public override void CompilerTest_26D5_E9FE() => OK();
            public override void CompilerTest_F7F3_AD6D() => OK();

            // ForEach
            public override void CompilerTest_245A_0917() => OK();
            public override void CompilerTest_DA7B_AAFD() => OK();
            public override void CompilerTest_34B8_D561() => OK();
            public override void CompilerTest_3958_FB90() => OK();
            public override void CompilerTest_1525_8CFA() => OK(); // REVIEW: Rely on runtime library to infer the right GetEnumerator method, or pass it to the factory method? (NB: No extension methods are considered, so should be able to guarantee that we can find it at runtime.)
            public override void CompilerTest_720D_2F5A() => OK();
            public override void CompilerTest_0041_2906() => OK();
            public override void CompilerTest_AE67_825B() => OK();
            public override void CompilerTest_8AE9_7D52() => OK();

            // Using
            public override void CompilerTest_5598_03A6() => OK();
            public override void CompilerTest_62CA_03A6() => OK();
            public override void CompilerTest_BB7C_25AF() => OK();
            public override void CompilerTest_51A3_8AB4() => OK();
            public override void CompilerTest_57C3_FE40() => OK();
            public override void CompilerTest_7AF8_56F5() => OK();
            public override void CompilerTest_7005_B197() => OK();

            // Lock
            public override void CompilerTest_2CF2_18B2() => OK();
            public override void CompilerTest_CD60_A086() => OK();

            // Try
            public override void CompilerTest_880F_A24B() => OK();
            public override void CompilerTest_19B3_485B() => OK(); // REVIEW: Test is of type System.Exception; we'd expect a more general catch (see 8.10).
            public override void CompilerTest_0662_485B() => OK();
            public override void CompilerTest_F63E_8707() => OK();
            public override void CompilerTest_02EE_D49C() => OK();
            public override void CompilerTest_1C02_6E0D() => OK();
            public override void CompilerTest_744C_C5E7() => OK();

            // Throw
            public override void CompilerTest_9329_A4F3() => OK();
            public override void CompilerTest_F778_9166() => OK();
            public override void CompilerTest_34FC_99EF() => OK();

            // Switch
            public override void CompilerTest_2156_D7F7() => OK();
            public override void CompilerTest_FCA9_3B3F() => OK();
            public override void CompilerTest_6832_C62D() => OK();
            public override void CompilerTest_4E9F_42FD() => OK();
            public override void CompilerTest_30E5_8D77() => OK();
            public override void CompilerTest_1754_0766() => OK();
            public override void CompilerTest_AD7C_C86B() => OK();
            public override void CompilerTest_3E56_6022() => OK();
            public override void CompilerTest_6D08_923C() => OK();
            public override void CompilerTest_205E_7D00() => OK();
            public override void CompilerTest_A00C_0847() => OK();
        }
    }
}
