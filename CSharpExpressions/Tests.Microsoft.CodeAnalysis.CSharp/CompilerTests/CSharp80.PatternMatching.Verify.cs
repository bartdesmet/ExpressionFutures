// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp80_PatternMatching
    {
        partial class Reviewed
        {
            // Positional - Tuple
            public override void CompilerTest_DED2_7DBC() => OK();
            public override void CompilerTest_D281_A3B2() => OK();

            // Positional - Deconstruct (instance method)
            public override void CompilerTest_650A_479E() => OK();
            public override void CompilerTest_9402_3821() => OK();

            // Positional - Deconstruct (extension method)
            public override void CompilerTest_0D16_94A2() => OK();
            public override void CompilerTest_C5CA_8C03() => OK();

            // Positional - ITuple
            public override void CompilerTest_6693_5ADB() => OK();

            // Property
            public override void CompilerTest_6496_E347() => OK();
            public override void CompilerTest_2EDD_999F() => OK();
            public override void CompilerTest_0ACF_C204() => OK();
        }
    }
}
