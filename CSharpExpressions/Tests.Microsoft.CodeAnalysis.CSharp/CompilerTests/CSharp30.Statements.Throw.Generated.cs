﻿// Prototyping extended expression trees for C#.
//
// bartde - November 2015

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

using Xunit;
using static Tests.Microsoft.CodeAnalysis.CSharp.TestUtilities;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    public partial class CompilerTests_CSharp30_Statements_Throw
    {
        [Fact]
        public void CompilerTest_9329_A4F3()
        {
            // (Expression<Action>)(() => { throw new Exception(); })
            var actual = GetDebugView(@"(Expression<Action>)(() => { throw new Exception(); })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <Block Type=""System.Void"">
      <Expressions>
        <Throw Type=""System.Void"">
          <Operand>
            <New Type=""System.Exception"" Constructor=""Void .ctor()"">
              <Arguments />
            </New>
          </Operand>
        </Throw>
      </Expressions>
    </Block>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_9329_A4F3();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_9329_A4F3() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_F778_2DE0()
        {
            // (Expression<Action>)(() => { try { } catch { throw; } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { try { } catch { throw; } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpTry Type=""System.Void"">
          <TryBlock>
            <Default Type=""System.Void"" />
          </TryBlock>
          <CatchBlocks>
            <CSharpCatchBlock Test=""System.Object"">
              <Body>
                <Block Type=""System.Void"">
                  <Expressions>
                    <Throw Type=""System.Void"" />
                  </Expressions>
                </Block>
              </Body>
            </CSharpCatchBlock>
          </CatchBlocks>
        </CSharpTry>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""0"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_F778_2DE0();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_F778_2DE0() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_34FC_EEF8()
        {
            // (Expression<Action>)(() => { try { } catch (Exception ex) { throw ex; } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { try { } catch (Exception ex) { throw ex; } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpTry Type=""System.Void"">
          <TryBlock>
            <Default Type=""System.Void"" />
          </TryBlock>
          <CatchBlocks>
            <CSharpCatchBlock>
              <Variables>
                <Parameter Type=""System.Exception"" Id=""0"" Name=""ex"" />
              </Variables>
              <Variable>
                <Parameter Type=""System.Exception"" Id=""0"" Name=""ex"" />
              </Variable>
              <Body>
                <Block Type=""System.Void"">
                  <Expressions>
                    <Throw Type=""System.Void"">
                      <Operand>
                        <Parameter Type=""System.Exception"" Id=""0"" Name=""ex"" />
                      </Operand>
                    </Throw>
                  </Expressions>
                </Block>
              </Body>
            </CSharpCatchBlock>
          </CatchBlocks>
        </CSharpTry>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""1"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_34FC_EEF8();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_34FC_EEF8() => INCONCLUSIVE(); }

        partial class Review
        {
            protected void INCONCLUSIVE() { /* Assert.Inconclusive(); */ Assert.True(false, "INCONCLUSIVE"); }
        }

        partial class Reviewed : Review
        {
            private void OK() { }
            private void FAIL(string message = "") { Assert.True(false, message); }
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
    partial class CompilerTests_CSharp30_Statements_Throw
    {
        partial class Reviewed
        {
            public override void CompilerTest_9329_A4F3() => OK();
            public override void CompilerTest_F778_2DE0() => OK();
            public override void CompilerTest_34FC_EEF8() => OK();
        }
    }
}
*/
}
