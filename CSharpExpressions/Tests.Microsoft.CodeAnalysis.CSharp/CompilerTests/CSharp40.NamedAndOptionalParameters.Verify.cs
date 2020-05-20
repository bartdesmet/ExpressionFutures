// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp40_NamedAndOptionalParameters
    {
        partial class Reviewed
        {
            // Named parameters for calls
            public override void CompilerTest_E9F4_7C15() => OK();
            public override void CompilerTest_4EB1_83FD() => OK();
            public override void CompilerTest_C437_AA4C() => OK();
            public override void CompilerTest_4C39_BCFC() => OK();
            public override void CompilerTest_7E8C_AA4C() => OK(); // DESIGN: Is it ok to infer the name of parameters even though the user didn't specify them explicitly?

            // Named parameters for constructors
            public override void CompilerTest_00C1_AE5C() => OK();
            public override void CompilerTest_D9CA_6B19() => OK();

            // Named parameters for indexers
            public override void CompilerTest_EDEC_D0C9() => OK();

            // Named parameters for invocations
            public override void CompilerTest_6271_EABC() => OK();
            public override void CompilerTest_053A_671C() => OK();
        }
    }
}
