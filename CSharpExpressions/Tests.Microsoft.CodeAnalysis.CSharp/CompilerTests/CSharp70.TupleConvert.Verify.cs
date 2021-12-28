// Prototyping extended expression trees for C#.
//
// bartde - May 2020

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp70_TupleConvert
    {
        partial class Reviewed
        {
            public override void CompilerTest_5948_DA1F() => OK(); // NB: Nullable conversion uses Convert
            public override void CompilerTest_D7DE_3D5E() => OK();
            public override void CompilerTest_2695_BEC3() => OK();
            public override void CompilerTest_7419_0AE5() => OK();
        }
    }
}
