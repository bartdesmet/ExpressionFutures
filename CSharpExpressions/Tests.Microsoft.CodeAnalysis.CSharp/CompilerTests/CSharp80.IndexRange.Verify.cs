// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp80_IndexRange
    {
        partial class Reviewed
        {
            // Index
            public override void CompilerTest_EDDE_8041() => OK(); // NB: Implicit conversion
            public override void CompilerTest_F139_15B7() => OK();

            // Range
            public override void CompilerTest_7E29_52A3() => OK();
        }
    }
}
