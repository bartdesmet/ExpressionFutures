// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp73_TupleEquality_Reducing
    {
        partial class Reviewed
        {
            public override void CompilerTest_E117_F776() => OK();
            public override void CompilerTest_F202_FB1D() => OK();
            public override void CompilerTest_9411_0D00() => OK(); // CONSIDER: Extra temporary spill of variable can be avoided if we add analysis to ensure no writes occur
            public override void CompilerTest_36A3_3E5F() => OK();
            public override void CompilerTest_B5AA_125F() => OK();
            public override void CompilerTest_02A6_E636() => OK();
            public override void CompilerTest_0830_8FE6() => OK();
            public override void CompilerTest_B038_1EAB() => OK();
        }
    }
}
