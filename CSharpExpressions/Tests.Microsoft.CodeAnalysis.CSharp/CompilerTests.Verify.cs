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

            // Named parameters for constructors
            public override void CompilerTest_00C1_AE5C() => OK();
            public override void CompilerTest_D9CA_6B19() => OK();

            // Named parameters for indexers
            public override void CompilerTest_EDEC_D0C9() => OK();

            // Named parameters for invocations
            public override void CompilerTest_6271_EABC() => OK();
            public override void CompilerTest_053A_671C() => OK();

            // Dynamic unary
            public override void CompilerTest_B473_5F62() => OK();
            public override void CompilerTest_8E43_6B25() => OK();
            public override void CompilerTest_CE27_211A() => OK();
            public override void CompilerTest_EA86_0AAE() => OK();
            public override void CompilerTest_7005_E877() => OK(); // DESIGN: Uses Subtract, not SubtractChecked; Flags contains CheckedContext

            // Dynamic binary
            public override void CompilerTest_775D_E1DB() => OK();
            public override void CompilerTest_84B6_6376() => OK();
            public override void CompilerTest_754C_C121() => OK();
            public override void CompilerTest_6E28_DBCA() => OK();
            public override void CompilerTest_A085_4C3D() => OK();
            public override void CompilerTest_DE17_40A5() => OK();
            public override void CompilerTest_5CD0_0AE2() => OK();
            public override void CompilerTest_0C84_0C91() => OK();
            public override void CompilerTest_FF88_BC1C() => OK();
            public override void CompilerTest_F2A3_DCF3() => OK();
            public override void CompilerTest_3DBC_7187() => OK();
            public override void CompilerTest_5002_22B5() => OK();
            public override void CompilerTest_1A1B_4504() => OK();
            public override void CompilerTest_8DF7_1EFB() => OK();
            public override void CompilerTest_5F46_EC7B() => OK();
            public override void CompilerTest_106F_B1CC() => OK();
            public override void CompilerTest_2275_966C() => OK(); // DESIGN: Uses And, not AndAlso; Flags contains BinaryOperationLogical
            public override void CompilerTest_FB20_C3F6() => OK(); // DESIGN: Uses Or, not OrElse; Flags contains BinaryOperationLogical
            public override void CompilerTest_191C_CEEB() => OK(); // DESIGN: Uses Add, not AddChecked; Flags contains CheckedContext

            // Dynamic convert
            public override void CompilerTest_6647_1258() => OK();
            public override void CompilerTest_93FA_3B2C() => OK();
            public override void CompilerTest_8FB3_87A6() => OK();

            // Dynamic get member
            public override void CompilerTest_AB76_B2ED() => OK();

            // Dynamic invoke member
            public override void CompilerTest_14B0_8F1F() => OK();
            public override void CompilerTest_ABB7_09A0() => OK();
            public override void CompilerTest_647C_869C() => OK();
            public override void CompilerTest_A62F_E0EA() => OK();
            public override void CompilerTest_6E0D_C117() => OK();
            public override void CompilerTest_9AE8_6C9F() => OK(); // REVIEW: UseCompileTimeType, will expression type match what the compiler has inferred?
            public override void CompilerTest_2069_9E36() => OK(); // REVIEW: UseCompileTimeType, will expression type match what the compiler has inferred?
            public override void CompilerTest_4C1E_1B45() => OK();
            public override void CompilerTest_5D0B_BB53() => OK();
            public override void CompilerTest_644B_2D72() => OK();
            public override void CompilerTest_3B77_970B() => OK();
            public override void CompilerTest_3418_D0D2() => OK();
            public override void CompilerTest_0946_2AB1() => OK();
            public override void CompilerTest_1B29_3F4D() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_EEB8_ED24() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_7349_817B() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_76D5_13EF() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_8619_DB42() => OK(); // REVIEW: Do we have all the info needed about the target which has no CSharpDynamicArgument (compile-time type comes to mind)?
            public override void CompilerTest_4B4B_894F() => OK();

            // Dynamic invoke
            public override void CompilerTest_B5A3_A9EE() => OK();
            public override void CompilerTest_CFDD_AFEF() => OK();
            public override void CompilerTest_F090_0B2E() => OK();
            public override void CompilerTest_3646_7B2B() => OK();
            public override void CompilerTest_DC58_213C() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_5C4C_E9EB() => OK(); // REVIEW: Do we have all the info needed about the target which has no CSharpDynamicArgument(compile-time type comes to mind)?

            // Dynamic get index
            public override void CompilerTest_55B7_3EFA() => OK();
            public override void CompilerTest_7760_936B() => OK();
            public override void CompilerTest_7FDD_6511() => OK();
            public override void CompilerTest_D809_C13E() => OK();
            public override void CompilerTest_9998_022C() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_F77D_040F() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_604B_718E() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_D83C_2D6C() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_447A_FCCE() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_F586_7D7E() => OK(); // REVIEW: Do we have all the info needed about the target which has no CSharpDynamicArgument (compile-time type comes to mind)?

            // Dynamic invoke constructor
            public override void CompilerTest_22AA_5962() => OK();
            public override void CompilerTest_29E3_8116() => OK();
            public override void CompilerTest_2CE0_A2D8() => OK();
            public override void CompilerTest_D55B_C6C6() => OK();

            // Async/await
            public override void CompilerTest_0FFA_7AF2() => OK(); // DESIGN: Artificial Lambda/Invoke wrapper returned by AsyncLambda
            public override void CompilerTest_83AE_26E4() => OK(); // DESIGN: Artificial Lambda/Invoke wrapper returned by AsyncLambda
            public override void CompilerTest_4DC5_243C() => OK(); // DESIGN: Artificial Lambda/Invoke wrapper returned by AsyncLambda
            public override void CompilerTest_5DFD_243C() => OK(); // DESIGN: Artificial Lambda/Invoke wrapper returned by AsyncLambda
            public override void CompilerTest_1A0E_F439() => OK(); // DESIGN: Artificial Lambda/Invoke wrapper returned by AsyncLambda

            // Conditional access
            public override void CompilerTest_B340_BC70() => OK();
            public override void CompilerTest_9320_B6D2() => OK();
            public override void CompilerTest_A997_18C3() => OK();
            public override void CompilerTest_A5F9_6775() => OK();
            public override void CompilerTest_F165_9386() => OK();
            public override void CompilerTest_2462_8DFD() => OK();
            public override void CompilerTest_3041_FAE0() => OK();
            public override void CompilerTest_CB0C_60AB() => OK();
            public override void CompilerTest_CF40_3D45() => OK(); // DESIGN: Should we emit InvocationExpression here?
            public override void CompilerTest_4241_E360() => OK();

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

            // Assignments binary
            public override void CompilerTest_F94F_ACD8() => OK();
            public override void CompilerTest_044F_75C3() => OK();
            public override void CompilerTest_094E_50F3() => OK();
            public override void CompilerTest_8BE1_F8B3() => OK();
            public override void CompilerTest_F3D0_9CC6() => OK();
            public override void CompilerTest_EB02_125F() => OK();
            public override void CompilerTest_6EC4_3570() => OK();
            public override void CompilerTest_7D09_CB1F() => OK();
            public override void CompilerTest_ED89_FB15() => OK();
            public override void CompilerTest_6090_2158() => OK();
            public override void CompilerTest_60B4_667C() => OK();
            public override void CompilerTest_2DAA_E873() => OK();
            public override void CompilerTest_7F56_2B0B() => OK();
            public override void CompilerTest_27E3_F016() => OK();
            public override void CompilerTest_467C_C565() => OK();
            public override void CompilerTest_B13A_1A72() => OK();
            public override void CompilerTest_EE3F_1481() => OK();
            public override void CompilerTest_79AE_726E() => OK();
            public override void CompilerTest_A709_4D47() => OK();
            public override void CompilerTest_8CA7_C849() => OK();
            public override void CompilerTest_8BFE_5348() => OK();
            public override void CompilerTest_18CC_52FA() => OK();
            public override void CompilerTest_C8F7_C9E3() => OK();
            public override void CompilerTest_3859_A369() => OK();
            public override void CompilerTest_3AC9_62A5() => OK();
            public override void CompilerTest_93FC_34D5() => OK();
            public override void CompilerTest_1D2F_75F1() => OK();
            public override void CompilerTest_1EDE_AB2B() => OK();
            public override void CompilerTest_A171_21C8() => OK();

            // Assignments unary
            public override void CompilerTest_D47A_F8E7() => OK();
            public override void CompilerTest_00CF_77BB() => OK();
            public override void CompilerTest_9C51_C4A7() => OK();
            public override void CompilerTest_2115_438C() => OK();
            public override void CompilerTest_09D9_B341() => OK();
            public override void CompilerTest_C9BD_E678() => OK();
            public override void CompilerTest_1C12_48DE() => OK();
            public override void CompilerTest_B58C_6908() => OK();
            public override void CompilerTest_2817_C567() => OK();
            public override void CompilerTest_F997_C02C() => OK();
            public override void CompilerTest_C633_A890() => OK();
            public override void CompilerTest_893C_377B() => OK();
            public override void CompilerTest_A586_1339() => OK();
            public override void CompilerTest_B251_4951() => OK();
            public override void CompilerTest_98BA_B755() => OK();
            public override void CompilerTest_5002_1E6C() => OK();
            public override void CompilerTest_2E6E_C166() => OK();


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
            public override void CompilerTest_245A_DA7A() => OK();
            public override void CompilerTest_DA7B_B67A() => OK();
            public override void CompilerTest_34B8_D672() => OK();
            public override void CompilerTest_3958_5948() => OK();
            public override void CompilerTest_1525_79A2() => OK(); // REVIEW: Rely on runtime library to infer the right GetEnumerator method, or pass it to the factory method? (NB: No extension methods are considered, so should be able to guarantee that we can find it at runtime.)
            public override void CompilerTest_720D_1B2C() => OK();
            public override void CompilerTest_0041_AAB8() => OK();
            public override void CompilerTest_AE67_91A3() => OK();

            // Using
            public override void CompilerTest_5598_03A6() => OK();
            public override void CompilerTest_62CA_03A6() => OK();
            public override void CompilerTest_BB7C_2A2A() => OK();
            public override void CompilerTest_51A3_E043() => OK();
            public override void CompilerTest_57C3_49DB() => OK();

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
        }
    }
}
