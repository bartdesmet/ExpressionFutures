// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp30_Statements_Return
    {
        partial class Reviewed
        {
            public override void CompilerTest_6102_7F8E() => OK();
            public override void CompilerTest_AEF8_BB4B() => OK();
            public override void CompilerTest_7381_AA02() => /*INCONCLUSIVE()*/ OK(); // TODO: Degenerates into an expression body
        }
    }
}
