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

            // Dynamic invoke
            public override void CompilerTest_B5A3_A9EE() => OK();
            public override void CompilerTest_CFDD_AFEF() => OK();
            public override void CompilerTest_F090_0B2E() => OK();
            public override void CompilerTest_3646_7B2B() => OK();
            public override void CompilerTest_DC58_213C() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_5C4C_E9EB() => OK(); // REVIEW: Do we have all the info needed about the target which has no CSharpDynamicArgument(compile-time type comes to mind)?

            // Dynamic index
            public override void CompilerTest_55B7_3EFA() => OK();
            public override void CompilerTest_7760_936B() => OK();
            public override void CompilerTest_7FDD_6511() => OK();
            public override void CompilerTest_D809_C13E() => OK();
            public override void CompilerTest_9998_022C() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_F77D_040F() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_604B_718E() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_D83C_2D6C() => OK(); // REVIEW: Is UseCompileTimeType expected?
            public override void CompilerTest_F586_7D7E() => OK(); // REVIEW: Do we have all the info needed about the target which has no CSharpDynamicArgument (compile-time type comes to mind)?

            // Async/await
            public override void CompilerTest_0FFA_7AF2() => OK(); // DESIGN: Artificial Lambda/Invoke wrapper returned by AsyncLambda
            public override void CompilerTest_83AE_26E4() => OK(); // DESIGN: Artificial Lambda/Invoke wrapper returned by AsyncLambda
            public override void CompilerTest_4DC5_243C() => OK(); // DESIGN: Artificial Lambda/Invoke wrapper returned by AsyncLambda
            public override void CompilerTest_5DFD_243C() => OK(); // DESIGN: Artificial Lambda/Invoke wrapper returned by AsyncLambda
            public override void CompilerTest_1A0E_F439() => OK(); // DESIGN: Artificial Lambda/Invoke wrapper returned by AsyncLambda

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
