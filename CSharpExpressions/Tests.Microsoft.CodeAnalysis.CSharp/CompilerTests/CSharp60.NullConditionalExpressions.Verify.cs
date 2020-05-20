// Prototyping extended expression trees for C#.
//
// bartde - November 2015

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp60_NullConditionalExpressions
    {
        partial class Reviewed
        {
            // Conditional access
            public override void CompilerTest_B340_BC70() => OK();
            public override void CompilerTest_9320_B6D2() => OK();
            public override void CompilerTest_A997_18C3() => OK();
            public override void CompilerTest_A5F9_6775() => OK();
            public override void CompilerTest_F165_9386() => OK();
            public override void CompilerTest_2462_8DFD() => OK();
            public override void CompilerTest_3041_FAE0() => OK();
            public override void CompilerTest_CB0C_60AB() => OK();
            public override void CompilerTest_CF40_3D45() => OK(); // DESIGN: Should we emit InvocationExpression here?
            public override void CompilerTest_4241_E360() => OK();
        }
    }
}
