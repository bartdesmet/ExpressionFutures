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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Microsoft.CodeAnalysis.CSharp.TestUtilities;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    [TestClass]
    public partial class CompilerTests_CSharp30_Statements_While
    {
        [TestMethod]
        public void CompilerTest_C90B_9C05()
        {
            // (Expression<Action>)(() => { while (true) Console.Write('.'); })
            var actual = GetDebugView(@"(Expression<Action>)(() => { while (true) Console.Write('.'); })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <Block Type=""System.Void"">
      <Expressions>
        <CSharpWhile Type=""System.Void"">
          <Test>
            <Constant Type=""System.Boolean"" Value=""true"" />
          </Test>
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
        </CSharpWhile>
      </Expressions>
    </Block>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_C90B_9C05();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_C90B_9C05() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_C5C5_4E9F()
        {
            // (Expression<Action>)(() => { while (true) { Console.Write('.'); } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { while (true) { Console.Write('.'); } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <Block Type=""System.Void"">
      <Expressions>
        <CSharpWhile Type=""System.Void"">
          <Test>
            <Constant Type=""System.Boolean"" Value=""true"" />
          </Test>
          <Body>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""."" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""0"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </ContinueLabel>
        </CSharpWhile>
      </Expressions>
    </Block>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_C5C5_4E9F();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_C5C5_4E9F() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_7D34_66D1()
        {
            // (Expression<Action>)(() => { while (true) { break; } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { while (true) { break; } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpWhile Type=""System.Void"">
          <Test>
            <Constant Type=""System.Boolean"" Value=""true"" />
          </Test>
          <Body>
            <Block Type=""System.Void"">
              <Expressions>
                <Goto Type=""System.Void"" Kind=""Break"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""0"" />
                  </Target>
                </Goto>
              </Expressions>
            </Block>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""0"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </ContinueLabel>
        </CSharpWhile>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_7D34_66D1();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_7D34_66D1() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_40EC_DA92()
        {
            // (Expression<Action>)(() => { while (true) { continue; } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { while (true) { continue; } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <Block Type=""System.Void"">
      <Expressions>
        <CSharpWhile Type=""System.Void"">
          <Test>
            <Constant Type=""System.Boolean"" Value=""true"" />
          </Test>
          <Body>
            <Block Type=""System.Void"">
              <Expressions>
                <Goto Type=""System.Void"" Kind=""Continue"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""0"" />
                  </Target>
                </Goto>
              </Expressions>
            </Block>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""0"" />
          </ContinueLabel>
        </CSharpWhile>
      </Expressions>
    </Block>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_40EC_DA92();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_40EC_DA92() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_FC9A_C797()
        {
            // (Expression<Action>)(() => { while (true) { return; } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { while (true) { return; } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpWhile Type=""System.Void"">
          <Test>
            <Constant Type=""System.Boolean"" Value=""true"" />
          </Test>
          <Body>
            <Block Type=""System.Void"">
              <Expressions>
                <Goto Type=""System.Void"" Kind=""Return"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""0"" />
                  </Target>
                </Goto>
              </Expressions>
            </Block>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""2"" />
          </ContinueLabel>
        </CSharpWhile>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""0"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_FC9A_C797();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_FC9A_C797() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_6C15_9FA8()
        {
            // (Expression<Action>)(() => { int i = 0; while (i < 10) { if (i == 5) continue; if (i == 8) break; i++; } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { int i = 0; while (i < 10) { if (i == 5) continue; if (i == 8) break; i++; } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Variables>
        <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
      </Variables>
      <Statements>
        <CSharpAssign Type=""System.Int32"">
          <Left>
            <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
          </Left>
          <Right>
            <Constant Type=""System.Int32"" Value=""0"" />
          </Right>
        </CSharpAssign>
        <CSharpWhile Type=""System.Void"">
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
          <Body>
            <Block Type=""System.Int32"">
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
                <CSharpPostIncrementAssign Type=""System.Int32"">
                  <Operand>
                    <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
                  </Operand>
                </CSharpPostIncrementAssign>
              </Expressions>
            </Block>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""2"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </ContinueLabel>
        </CSharpWhile>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""3"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_6C15_9FA8();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_6C15_9FA8() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_242C_68A7()
        {
            // (Expression<Action<int, int>>)((x, y) => { while (x < 1) { while (y < 2) { break; } continue; } })
            var actual = GetDebugView(@"(Expression<Action<int, int>>)((x, y) => { while (x < 1) { while (y < 2) { break; } continue; } })");
            var expected = @"
<Lambda Type=""System.Action`2[System.Int32,System.Int32]"">
  <Parameters>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
    <Parameter Type=""System.Int32"" Id=""1"" Name=""y"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpWhile Type=""System.Void"">
          <Test>
            <LessThan Type=""System.Boolean"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""1"" />
              </Right>
            </LessThan>
          </Test>
          <Body>
            <Block Type=""System.Void"">
              <Expressions>
                <CSharpWhile Type=""System.Void"">
                  <Test>
                    <LessThan Type=""System.Boolean"">
                      <Left>
                        <Parameter Type=""System.Int32"" Id=""1"" Name=""y"" />
                      </Left>
                      <Right>
                        <Constant Type=""System.Int32"" Value=""2"" />
                      </Right>
                    </LessThan>
                  </Test>
                  <Body>
                    <Block Type=""System.Void"">
                      <Expressions>
                        <Goto Type=""System.Void"" Kind=""Break"">
                          <Target>
                            <LabelTarget Type=""System.Void"" Id=""2"" />
                          </Target>
                        </Goto>
                      </Expressions>
                    </Block>
                  </Body>
                  <BreakLabel>
                    <LabelTarget Type=""System.Void"" Id=""2"" />
                  </BreakLabel>
                  <ContinueLabel>
                    <LabelTarget Type=""System.Void"" Id=""3"" />
                  </ContinueLabel>
                </CSharpWhile>
                <Goto Type=""System.Void"" Kind=""Continue"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""4"" />
                  </Target>
                </Goto>
              </Expressions>
            </Block>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""5"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""4"" />
          </ContinueLabel>
        </CSharpWhile>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""6"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_242C_68A7();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_242C_68A7() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_2503_AEF6()
        {
            // (Expression<Action<int, int>>)((x, y) => { while (x < 1) { while (y < 2) { continue; } break; } })
            var actual = GetDebugView(@"(Expression<Action<int, int>>)((x, y) => { while (x < 1) { while (y < 2) { continue; } break; } })");
            var expected = @"
<Lambda Type=""System.Action`2[System.Int32,System.Int32]"">
  <Parameters>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
    <Parameter Type=""System.Int32"" Id=""1"" Name=""y"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpWhile Type=""System.Void"">
          <Test>
            <LessThan Type=""System.Boolean"">
              <Left>
                <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
              </Left>
              <Right>
                <Constant Type=""System.Int32"" Value=""1"" />
              </Right>
            </LessThan>
          </Test>
          <Body>
            <Block Type=""System.Void"">
              <Expressions>
                <CSharpWhile Type=""System.Void"">
                  <Test>
                    <LessThan Type=""System.Boolean"">
                      <Left>
                        <Parameter Type=""System.Int32"" Id=""1"" Name=""y"" />
                      </Left>
                      <Right>
                        <Constant Type=""System.Int32"" Value=""2"" />
                      </Right>
                    </LessThan>
                  </Test>
                  <Body>
                    <Block Type=""System.Void"">
                      <Expressions>
                        <Goto Type=""System.Void"" Kind=""Continue"">
                          <Target>
                            <LabelTarget Type=""System.Void"" Id=""2"" />
                          </Target>
                        </Goto>
                      </Expressions>
                    </Block>
                  </Body>
                  <BreakLabel>
                    <LabelTarget Type=""System.Void"" Id=""3"" />
                  </BreakLabel>
                  <ContinueLabel>
                    <LabelTarget Type=""System.Void"" Id=""2"" />
                  </ContinueLabel>
                </CSharpWhile>
                <Goto Type=""System.Void"" Kind=""Break"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""4"" />
                  </Target>
                </Goto>
              </Expressions>
            </Block>
          </Body>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""4"" />
          </BreakLabel>
          <ContinueLabel>
            <LabelTarget Type=""System.Void"" Id=""5"" />
          </ContinueLabel>
        </CSharpWhile>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""6"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_2503_AEF6();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_2503_AEF6() => INCONCLUSIVE(); }

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
    partial class CompilerTests_CSharp30_Statements_While
    {
        partial class Reviewed
        {
            public override void CompilerTest_C90B_9C05() => OK();
            public override void CompilerTest_C5C5_4E9F() => OK();
            public override void CompilerTest_7D34_66D1() => OK();
            public override void CompilerTest_40EC_DA92() => OK();
            public override void CompilerTest_FC9A_C797() => OK();
            public override void CompilerTest_6C15_9FA8() => OK();
            public override void CompilerTest_242C_68A7() => OK();
            public override void CompilerTest_2503_AEF6() => OK();
        }
    }
}
*/
}
