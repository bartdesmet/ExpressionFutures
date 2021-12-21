// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp30_Statements_Try
    {
        partial class Reviewed
        {
            public override void CompilerTest_880F_A24B() => OK();
            public override void CompilerTest_19B3_485B() => OK(); // REVIEW: Test is of type System.Exception; we'd expect a more general catch (see 8.10).
            public override void CompilerTest_0662_485B() => OK();
            public override void CompilerTest_F63E_8707() => OK();
            public override void CompilerTest_02EE_D49C() => OK();
            public override void CompilerTest_1C02_6E0D() => OK();
            public override void CompilerTest_744C_C5E7() => OK();
        }
    }
}
