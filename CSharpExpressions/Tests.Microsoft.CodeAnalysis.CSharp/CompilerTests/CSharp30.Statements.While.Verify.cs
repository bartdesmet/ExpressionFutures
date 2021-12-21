// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp30_Statements_While
    {
        partial class Reviewed
        {
            public override void CompilerTest_C90B_9C05() => OK(); // REVIEW: Infinite loop, so no CSharpBlock at the top level?
            public override void CompilerTest_C5C5_4E9F() => OK(); // REVIEW: Infinite loop, so no CSharpBlock at the top level?
            public override void CompilerTest_7D34_66D1() => OK();
            public override void CompilerTest_40EC_DA92() => OK(); // REVIEW: Infinite loop, so no CSharpBlock at the top level?
            public override void CompilerTest_FC9A_C797() => OK();
            public override void CompilerTest_6C15_9FA8() => OK();
            public override void CompilerTest_242C_68A7() => OK();
            public override void CompilerTest_2503_AEF6() => OK();
        }
    }
}
