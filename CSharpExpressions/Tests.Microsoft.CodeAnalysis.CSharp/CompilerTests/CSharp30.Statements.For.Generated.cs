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
    public partial class CompilerTests_CSharp30_Statements_For
    {
        [Fact]
        public void CompilerTest_25E2_35E6()
        {
            // (Expression<Action>)(() => { for (;;) Console.Write('.'); })
            var actual = GetDebugView(@"(Expression<Action>)(() => { for (;;) Console.Write('.'); })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <Block Type=""System.Void"">
      <Expressions>
        <CSharpFor Type=""System.Void"">
          <Body>
            <Call Type=""System.Void"" Method=""Void Write(Char)"">
              <Arguments>
                <Constant Type=""System.Char"" Value=""."" />
              </Arguments>
            </Call>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""0"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </ContinueLabel>
        </CSharpFor>
      </Expressions>
    </Block>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_25E2_35E6();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_25E2_35E6() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_AD48_86CB()
        {
            // (Expression<Action>)(() => { for (var i = 0; i < 10; i++) Console.Write(i); })
            var actual = GetDebugView(@"(Expression<Action>)(() => { for (var i = 0; i < 10; i++) Console.Write(i); })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpFor Type=""System.Void"">
          <Variables>
            <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
          </Variables>
          <Initializers>
            <CSharpAssign Type=""System.Int32"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""0"" />
              </Right>
            </CSharpAssign>
          </Initializers>
          <Test>
            <LessThan Type=""System.Boolean"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""10"" />
              </Right>
            </LessThan>
          </Test>
          <Iterators>
            <CSharpPostIncrementAssign Type=""System.Int32"">
              <Operand>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Operand>
            </CSharpPostIncrementAssign>
          </Iterators>
          <Body>
            <Call Type=""System.Void"" Method=""Void Write(Int32)"">
              <Arguments>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Arguments>
            </Call>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""2"" />
          </ContinueLabel>
        </CSharpFor>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""3"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_AD48_86CB();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_AD48_86CB() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_5EF7_9418()
        {
            // (Expression<Action>)(() => { for (var i = 0; i < 10; i++) { Console.Write(i); } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { for (var i = 0; i < 10; i++) { Console.Write(i); } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpFor Type=""System.Void"">
          <Variables>
            <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
          </Variables>
          <Initializers>
            <CSharpAssign Type=""System.Int32"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""0"" />
              </Right>
            </CSharpAssign>
          </Initializers>
          <Test>
            <LessThan Type=""System.Boolean"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""10"" />
              </Right>
            </LessThan>
          </Test>
          <Iterators>
            <CSharpPostIncrementAssign Type=""System.Int32"">
              <Operand>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Operand>
            </CSharpPostIncrementAssign>
          </Iterators>
          <Body>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void Write(Int32)"">
                  <Arguments>
                    <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""2"" />
          </ContinueLabel>
        </CSharpFor>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""3"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_5EF7_9418();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_5EF7_9418() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_26D5_E9FE()
        {
            // (Expression<Action>)(() => { for (var i = 0; i < 10; i++) { if (i == 5) continue; if (i == 8) break; } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { for (var i = 0; i < 10; i++) { if (i == 5) continue; if (i == 8) break; } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpFor Type=""System.Void"">
          <Variables>
            <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
          </Variables>
          <Initializers>
            <CSharpAssign Type=""System.Int32"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""0"" />
              </Right>
            </CSharpAssign>
          </Initializers>
          <Test>
            <LessThan Type=""System.Boolean"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""10"" />
              </Right>
            </LessThan>
          </Test>
          <Iterators>
            <CSharpPostIncrementAssign Type=""System.Int32"">
              <Operand>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Operand>
            </CSharpPostIncrementAssign>
          </Iterators>
          <Body>
            <Block Type=""System.Void"">
              <Expressions>
                <Conditional Type=""System.Void"">
                  <Test>
                    <Equal Type=""System.Boolean"">
                      <Left>
                        <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
                      </Left>
                      <Right>
                        <Constant Type=""System.Int32"" Value=""5"" />
                      </Right>
                    </Equal>
                  </Test>
                  <IfTrue>
                    <Goto Type=""System.Void"" Kind=""Continue"">
                      <Target>
                        <LabelTarget Type=""System.Void"" Id=""1"" />
                      </Target>
                    </Goto>
                  </IfTrue>
                  <IfFalse>
                    <Default Type=""System.Void"" />
                  </IfFalse>
                </Conditional>
                <Conditional Type=""System.Void"">
                  <Test>
                    <Equal Type=""System.Boolean"">
                      <Left>
                        <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
                      </Left>
                      <Right>
                        <Constant Type=""System.Int32"" Value=""8"" />
                      </Right>
                    </Equal>
                  </Test>
                  <IfTrue>
                    <Goto Type=""System.Void"" Kind=""Break"">
                      <Target>
                        <LabelTarget Type=""System.Void"" Id=""2"" />
                      </Target>
                    </Goto>
                  </IfTrue>
                  <IfFalse>
                    <Default Type=""System.Void"" />
                  </IfFalse>
                </Conditional>
              </Expressions>
            </Block>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""2"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </ContinueLabel>
        </CSharpFor>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""3"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_26D5_E9FE();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_26D5_E9FE() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_F7F3_AD6D()
        {
            // (Expression<Action>)(() => { for (int i = 1, j = 2; i < 3; i += 4, j -= 5) Console.Write('.'); })
            var actual = GetDebugView(@"(Expression<Action>)(() => { for (int i = 1, j = 2; i < 3; i += 4, j -= 5) Console.Write('.'); })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpFor Type=""System.Void"">
          <Variables>
            <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
            <Parameter Type=""System.Int32"" Id=""1"" Name=""j"" />
          </Variables>
          <Initializers>
            <CSharpAssign Type=""System.Int32"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""1"" />
              </Right>
            </CSharpAssign>
            <CSharpAssign Type=""System.Int32"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""1"" Name=""j"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""2"" />
              </Right>
            </CSharpAssign>
          </Initializers>
          <Test>
            <LessThan Type=""System.Boolean"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""3"" />
              </Right>
            </LessThan>
          </Test>
          <Iterators>
            <CSharpAddAssign Type=""System.Int32"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""4"" />
              </Right>
            </CSharpAddAssign>
            <CSharpSubtractAssign Type=""System.Int32"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""1"" Name=""j"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""5"" />
              </Right>
            </CSharpSubtractAssign>
          </Iterators>
          <Body>
            <Call Type=""System.Void"" Method=""Void Write(Char)"">
              <Arguments>
                <Constant Type=""System.Char"" Value=""."" />
              </Arguments>
            </Call>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""2"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""3"" />
          </ContinueLabel>
        </CSharpFor>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""4"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_F7F3_AD6D();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_F7F3_AD6D() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_C446_5291()
        {
            // (Expression<Action>)(() => { for (string s = "0"; int.TryParse(s, out var x); s += s.Length) Console.Write(x); })
            var actual = GetDebugView(@"(Expression<Action>)(() => { for (string s = ""0""; int.TryParse(s, out var x); s += s.Length) Console.Write(x); })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpFor Type=""System.Void"">
          <Variables>
            <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
          </Variables>
          <Initializers>
            <CSharpAssign Type=""System.String"">
              <Left>
                <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
              </Left>
              <Right>
                <Constant Type=""System.String"" Value=""0"" />
              </Right>
            </CSharpAssign>
          </Initializers>
          <Locals>
            <Parameter Type=""System.Int32"" Id=""1"" Name=""x"" />
          </Locals>
          <Test>
            <Call Type=""System.Boolean"" Method=""Boolean TryParse(System.String, Int32 ByRef)"">
              <Arguments>
                <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
                <Parameter Type=""System.Int32"" Id=""1"" Name=""x"" />
              </Arguments>
            </Call>
          </Test>
          <Iterators>
            <CSharpAddAssign Type=""System.String"" Method=""System.String Concat(System.Object, System.Object)"">
              <Left>
                <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
              </Left>
              <Right>
                <Convert Type=""System.Object"">
                  <Operand>
                    <MemberAccess Type=""System.Int32"" Member=""Int32 Length"">
                      <Expression>
                        <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
                      </Expression>
                    </MemberAccess>
                  </Operand>
                </Convert>
              </Right>
            </CSharpAddAssign>
          </Iterators>
          <Body>
            <Call Type=""System.Void"" Method=""Void Write(Int32)"">
              <Arguments>
                <Parameter Type=""System.Int32"" Id=""1"" Name=""x"" />
              </Arguments>
            </Call>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""2"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""3"" />
          </ContinueLabel>
        </CSharpFor>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""4"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_C446_5291();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_C446_5291() => INCONCLUSIVE(); }

        partial class Review
        {
            protected void INCONCLUSIVE() { /* Assert.Inconclusive(); */ Assert.Fail("INCONCLUSIVE"); }
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
    partial class CompilerTests_CSharp30_Statements_For
    {
        partial class Reviewed
        {
            public override void CompilerTest_25E2_35E6() => OK();
            public override void CompilerTest_AD48_86CB() => OK();
            public override void CompilerTest_5EF7_9418() => OK();
            public override void CompilerTest_26D5_E9FE() => OK();
            public override void CompilerTest_F7F3_AD6D() => OK();
            public override void CompilerTest_C446_5291() => OK();
        }
    }
}
*/
}
