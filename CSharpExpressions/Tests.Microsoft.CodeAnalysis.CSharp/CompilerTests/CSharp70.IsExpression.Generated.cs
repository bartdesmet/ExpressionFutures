﻿// Prototyping extended expression trees for C#.
//
// bartde - December 2021

// NB: Running these tests can take a *VERY LONG* time because it invokes the C# compiler for every test
//     case in order to obtain an expression tree object. Be patient when running these tests.

// NB: These tests are generated from a list of expressions in the .tt file by invoking the C# compiler at
//     text template processing time by the T4 engine. See TestUtilities for the helper functions that call
//     into the compiler, load the generated assembly, extract the Expression objects through reflection on
//     the generated type, and call DebugView() on those. The resulting DebugView string is emitted in this
//     file as `expected` variables. The original expression is escaped and gets passed ot the GetDebugView
//     helper method to obtain `actual`, which causes the C# compiler to run at test execution time, using
//     the same helper library, thus obtaining the DebugView string again. This serves a number of goals:
//
//       1. At test generation time, a custom Roslyn build can be invoked to test the implicit conversion
//          of a lambda expression to an expression tree, which involves the changes made to support the
//          C# expression library in this solution. Any fatal compiler errors will come out at that time.
//
//       2. Reflection on the properties in the emitted class causes a deferred execution of the factory
//          method calls generated by the Roslyn compiler for the implicit conversion of the lambda to an
//          expression tree. Any exceptions thrown by the factory methods will show up as well during test
//          generation time, allowing issues to be uncovered.
//
//       3. The string literals in the `expected` variables are inspectable by a human to assert that the
//          compiler has indeed generated an expression representation that's homo-iconic to the original
//          expression that was provided in the test.
//
//       4. Any changes to the compiler or the runtime library could cause regressions. Because template
//          processing of the T4 only takes place upon editing the .tt file, the generated test code won't
//          change. As such, any regression can cause test failures which allows to detect any changes to
//          compiler or runtime library behavior.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Microsoft.CodeAnalysis.CSharp.TestUtilities;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    [TestClass]
    public partial class CompilerTests_CSharp70_IsExpression
    {
        [TestMethod]
        public void CompilerTest_9704_AF25()
        {
            // (Expression<Func<object, bool>>)(o => o is string s && s.Length > 0)
            var actual = GetDebugView(@"(Expression<Func<object, bool>>)(o => o is string s && s.Length > 0)");
            var expected = @"
<Lambda Type=""System.Func`2[System.Object,System.Boolean]"">
  <Parameters>
    <Parameter Type=""System.Object"" Id=""0"" Name=""o"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Boolean"">
      <Variables>
        <Parameter Type=""System.String"" Id=""1"" Name=""s"" />
      </Variables>
      <Statements>
        <AndAlso Type=""System.Boolean"">
          <Left>
            <CSharpIsPattern Type=""System.Boolean"">
              <Expression>
                <Parameter Type=""System.Object"" Id=""0"" Name=""o"" />
              </Expression>
              <Pattern>
                <DeclarationPattern InputType=""System.Object"" NarrowedType=""System.String"" Type=""System.String"">
                  <Variable>
                    <Parameter Type=""System.String"" Id=""1"" Name=""s"" />
                  </Variable>
                </DeclarationPattern>
              </Pattern>
            </CSharpIsPattern>
          </Left>
          <Right>
            <GreaterThan Type=""System.Boolean"">
              <Left>
                <MemberAccess Type=""System.Int32"" Member=""Int32 Length"">
                  <Expression>
                    <Parameter Type=""System.String"" Id=""1"" Name=""s"" />
                  </Expression>
                </MemberAccess>
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""0"" />
              </Right>
            </GreaterThan>
          </Right>
        </AndAlso>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Boolean"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_9704_AF25();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_9704_AF25() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_BCC8_0566()
        {
            // (Expression<Func<object, bool>>)(o => o is 42)
            var actual = GetDebugView(@"(Expression<Func<object, bool>>)(o => o is 42)");
            var expected = @"
<Lambda Type=""System.Func`2[System.Object,System.Boolean]"">
  <Parameters>
    <Parameter Type=""System.Object"" Id=""0"" Name=""o"" />
  </Parameters>
  <Body>
    <CSharpIsPattern Type=""System.Boolean"">
      <Expression>
        <Parameter Type=""System.Object"" Id=""0"" Name=""o"" />
      </Expression>
      <Pattern>
        <ConstantPattern InputType=""System.Object"" NarrowedType=""System.Int32"">
          <Constant Type=""System.Int32"" Value=""42"" />
        </ConstantPattern>
      </Pattern>
    </CSharpIsPattern>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_BCC8_0566();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_BCC8_0566() => INCONCLUSIVE(); }

        partial class Review
        {
            protected void INCONCLUSIVE() { Assert.Inconclusive(); }
        }

        partial class Reviewed : Review
        {
            private void OK() { }
            private void FAIL(string message = "") { Assert.Fail(message); }
        }

        private readonly Reviewed Verify = new Reviewed();
    }

/*
// NB: The code generated below accepts all tests. *DON'T* just copy/paste this to the .Verify.cs file
//     but review the tests one by one. This output is included in case a minor change is made to debug
//     output produced by DebugView() and all hashes are invalidated. In that case, this output can be
//     copied and pasted into .Verify.cs.

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests_CSharp70_IsExpression
    {
        partial class Reviewed
        {
            public override void CompilerTest_9704_AF25() => OK();
            public override void CompilerTest_BCC8_0566() => OK();
        }
    }
}
*/
}