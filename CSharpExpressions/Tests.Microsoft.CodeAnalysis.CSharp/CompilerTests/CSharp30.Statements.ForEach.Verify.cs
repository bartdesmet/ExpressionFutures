// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp30_Statements_ForEach
    {
        partial class Reviewed
        {
            public override void CompilerTest_245A_0917() => OK();
            public override void CompilerTest_DA7B_AAFD() => OK();
            public override void CompilerTest_34B8_D561() => OK();
            public override void CompilerTest_3958_FB90() => OK();
            public override void CompilerTest_1525_8CFA() => OK(); // REVIEW: Rely on runtime library to infer the right GetEnumerator method, or pass it to the factory method? (NB: No extension methods are considered, so should be able to guarantee that we can find it at runtime.)
            public override void CompilerTest_720D_2F5A() => OK();
            public override void CompilerTest_0041_2906() => OK();
            public override void CompilerTest_AE67_825B() => OK();
            public override void CompilerTest_8AE9_7D52() => OK();
        }
    }
}
