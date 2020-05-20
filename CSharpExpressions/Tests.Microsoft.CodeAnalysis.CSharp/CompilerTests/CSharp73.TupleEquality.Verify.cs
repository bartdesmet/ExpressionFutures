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

            // Null
            public override void CompilerTest_E4CE_8C7A() => OK();
            public override void CompilerTest_0B1F_2641() => OK();
        }
    }
}
