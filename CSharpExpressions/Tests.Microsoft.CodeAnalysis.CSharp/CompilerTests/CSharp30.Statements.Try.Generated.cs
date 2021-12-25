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
    public partial class CompilerTests_CSharp30_Statements_Try
    {
        [TestMethod]
        public void CompilerTest_880F_FB51()
        {
            // (Expression<Action>)(() => { try { Console.Write('T'); } finally { Console.Write('F'); } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { try { Console.Write('T'); } finally { Console.Write('F'); } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpTry Type=""System.Void"">
          <TryBlock>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""T"" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </TryBlock>
          <FinallyBlock>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""F"" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </FinallyBlock>
        </CSharpTry>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""0"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_880F_FB51();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_880F_FB51() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_19B3_BE4C()
        {
            // (Expression<Action>)(() => { try { Console.Write('T'); } catch { Console.Write('C'); } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { try { Console.Write('T'); } catch { Console.Write('C'); } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpTry Type=""System.Void"">
          <TryBlock>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""T"" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </TryBlock>
          <CatchBlocks>
            <CSharpCatchBlock Test=""System.Object"">
              <Body>
                <Block Type=""System.Void"">
                  <Expressions>
                    <Call Type=""System.Void"" Method=""Void Write(Char)"">
                      <Arguments>
                        <Constant Type=""System.Char"" Value=""C"" />
                      </Arguments>
                    </Call>
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
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_19B3_BE4C();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_19B3_BE4C() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_0662_BEAD()
        {
            // (Expression<Action>)(() => { try { Console.Write('T'); } catch (Exception) { Console.Write('C'); } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { try { Console.Write('T'); } catch (Exception) { Console.Write('C'); } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpTry Type=""System.Void"">
          <TryBlock>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""T"" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </TryBlock>
          <CatchBlocks>
            <CSharpCatchBlock Test=""System.Exception"">
              <Body>
                <Block Type=""System.Void"">
                  <Expressions>
                    <Call Type=""System.Void"" Method=""Void Write(Char)"">
                      <Arguments>
                        <Constant Type=""System.Char"" Value=""C"" />
                      </Arguments>
                    </Call>
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
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_0662_BEAD();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_0662_BEAD() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_F63E_8A43()
        {
            // (Expression<Action>)(() => { try { Console.Write('T'); } catch (Exception e) { Console.WriteLine(e); } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { try { Console.Write('T'); } catch (Exception e) { Console.WriteLine(e); } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpTry Type=""System.Void"">
          <TryBlock>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""T"" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </TryBlock>
          <CatchBlocks>
            <CSharpCatchBlock>
              <Variables>
                <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
              </Variables>
              <Variable>
                <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
              </Variable>
              <Body>
                <Block Type=""System.Void"">
                  <Expressions>
                    <Call Type=""System.Void"" Method=""Void WriteLine(System.Object)"">
                      <Arguments>
                        <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
                      </Arguments>
                    </Call>
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
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_F63E_8A43();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_F63E_8A43() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_02EE_D95C()
        {
            // (Expression<Action>)(() => { try { Console.Write('T'); } catch (Exception e) when (e != null) { Console.WriteLine(e); } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { try { Console.Write('T'); } catch (Exception e) when (e != null) { Console.WriteLine(e); } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpTry Type=""System.Void"">
          <TryBlock>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""T"" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </TryBlock>
          <CatchBlocks>
            <CSharpCatchBlock>
              <Variables>
                <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
              </Variables>
              <Variable>
                <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
              </Variable>
              <Body>
                <Block Type=""System.Void"">
                  <Expressions>
                    <Call Type=""System.Void"" Method=""Void WriteLine(System.Object)"">
                      <Arguments>
                        <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
                      </Arguments>
                    </Call>
                  </Expressions>
                </Block>
              </Body>
              <Filter>
                <NotEqual Type=""System.Boolean"">
                  <Left>
                    <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
                  </Left>
                  <Right>
                    <Constant Type=""System.Object"" Value=""null"" />
                  </Right>
                </NotEqual>
              </Filter>
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
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_02EE_D95C();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_02EE_D95C() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_1C02_2560()
        {
            // (Expression<Action>)(() => { try { Console.Write('T'); } catch (InvalidOperationException) { Console.Write('I'); } catch (OverflowException) { Console.Write('O'); } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { try { Console.Write('T'); } catch (InvalidOperationException) { Console.Write('I'); } catch (OverflowException) { Console.Write('O'); } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpTry Type=""System.Void"">
          <TryBlock>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""T"" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </TryBlock>
          <CatchBlocks>
            <CSharpCatchBlock Test=""System.InvalidOperationException"">
              <Body>
                <Block Type=""System.Void"">
                  <Expressions>
                    <Call Type=""System.Void"" Method=""Void Write(Char)"">
                      <Arguments>
                        <Constant Type=""System.Char"" Value=""I"" />
                      </Arguments>
                    </Call>
                  </Expressions>
                </Block>
              </Body>
            </CSharpCatchBlock>
            <CSharpCatchBlock Test=""System.OverflowException"">
              <Body>
                <Block Type=""System.Void"">
                  <Expressions>
                    <Call Type=""System.Void"" Method=""Void Write(Char)"">
                      <Arguments>
                        <Constant Type=""System.Char"" Value=""O"" />
                      </Arguments>
                    </Call>
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
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_1C02_2560();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_1C02_2560() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_744C_4609()
        {
            // (Expression<Action>)(() => { try { Console.Write('T'); } catch (Exception e) { Console.WriteLine(e); } finally { Console.Write('F'); } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { try { Console.Write('T'); } catch (Exception e) { Console.WriteLine(e); } finally { Console.Write('F'); } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpTry Type=""System.Void"">
          <TryBlock>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""T"" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </TryBlock>
          <CatchBlocks>
            <CSharpCatchBlock>
              <Variables>
                <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
              </Variables>
              <Variable>
                <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
              </Variable>
              <Body>
                <Block Type=""System.Void"">
                  <Expressions>
                    <Call Type=""System.Void"" Method=""Void WriteLine(System.Object)"">
                      <Arguments>
                        <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
                      </Arguments>
                    </Call>
                  </Expressions>
                </Block>
              </Body>
            </CSharpCatchBlock>
          </CatchBlocks>
          <FinallyBlock>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""F"" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </FinallyBlock>
        </CSharpTry>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""1"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_744C_4609();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_744C_4609() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_1014_3118()
        {
            // (Expression<Action>)(() => { try { Console.Write('T'); } catch (Exception e) when (e is ArgumentException a) { Console.WriteLine(a); } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { try { Console.Write('T'); } catch (Exception e) when (e is ArgumentException a) { Console.WriteLine(a); } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpTry Type=""System.Void"">
          <TryBlock>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""T"" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </TryBlock>
          <CatchBlocks>
            <CSharpCatchBlock>
              <Variables>
                <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
                <Parameter Type=""System.ArgumentException"" Id=""1"" Name=""a"" />
              </Variables>
              <Variable>
                <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
              </Variable>
              <Body>
                <Block Type=""System.Void"">
                  <Expressions>
                    <Call Type=""System.Void"" Method=""Void WriteLine(System.Object)"">
                      <Arguments>
                        <Parameter Type=""System.ArgumentException"" Id=""1"" Name=""a"" />
                      </Arguments>
                    </Call>
                  </Expressions>
                </Block>
              </Body>
              <Filter>
                <CSharpIsPattern Type=""System.Boolean"">
                  <Expression>
                    <Parameter Type=""System.Exception"" Id=""0"" Name=""e"" />
                  </Expression>
                  <Pattern>
                    <DeclarationPattern InputType=""System.Exception"" NarrowedType=""System.ArgumentException"" Type=""System.ArgumentException"">
                      <Variable>
                        <Parameter Type=""System.ArgumentException"" Id=""1"" Name=""a"" />
                      </Variable>
                    </DeclarationPattern>
                  </Pattern>
                </CSharpIsPattern>
              </Filter>
            </CSharpCatchBlock>
          </CatchBlocks>
        </CSharpTry>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_1014_3118();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_1014_3118() => INCONCLUSIVE(); }

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
    partial class CompilerTests_CSharp30_Statements_Try
    {
        partial class Reviewed
        {
            public override void CompilerTest_880F_FB51() => OK();
            public override void CompilerTest_19B3_BE4C() => OK();
            public override void CompilerTest_0662_BEAD() => OK();
            public override void CompilerTest_F63E_8A43() => OK();
            public override void CompilerTest_02EE_D95C() => OK();
            public override void CompilerTest_1C02_2560() => OK();
            public override void CompilerTest_744C_4609() => OK();
            public override void CompilerTest_1014_3118() => OK();
        }
    }
}
*/
}