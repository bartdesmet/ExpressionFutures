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
    public partial class CompilerTests_CSharp30_Statements_Switch
    {
        [TestMethod]
        public void CompilerTest_2156_D7F7()
        {
            // (Expression<Action<int>>)(x => { switch (x) {} })
            var actual = GetDebugView(@"(Expression<Action<int>>)(x => { switch (x) {} })");
            var expected = @"
<Lambda Type=""System.Action`1[System.Int32]"">
  <Parameters>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpSwitch Type=""System.Void"">
          <SwitchValue>
            <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
          </SwitchValue>
          <Cases />
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
        </CSharpSwitch>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_2156_D7F7();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_2156_D7F7() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_FCA9_3B3F()
        {
            // (Expression<Action<int>>)(x => { switch (x) { case 0: Console.Write('0'); break; } })
            var actual = GetDebugView(@"(Expression<Action<int>>)(x => { switch (x) { case 0: Console.Write('0'); break; } })");
            var expected = @"
<Lambda Type=""System.Action`1[System.Int32]"">
  <Parameters>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpSwitch Type=""System.Void"">
          <SwitchValue>
            <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
          </SwitchValue>
          <Cases>
            <CSharpSwitchCase TestValues=""0"">
              <Statements>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""0"" />
                  </Arguments>
                </Call>
                <Goto Type=""System.Void"" Kind=""Break"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""1"" />
                  </Target>
                </Goto>
              </Statements>
            </CSharpSwitchCase>
          </Cases>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
        </CSharpSwitch>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_FCA9_3B3F();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_FCA9_3B3F() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_6832_C62D()
        {
            // (Expression<Action<int>>)(x => { switch (x) { case 0: case 1: Console.Write('A'); break; } })
            var actual = GetDebugView(@"(Expression<Action<int>>)(x => { switch (x) { case 0: case 1: Console.Write('A'); break; } })");
            var expected = @"
<Lambda Type=""System.Action`1[System.Int32]"">
  <Parameters>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpSwitch Type=""System.Void"">
          <SwitchValue>
            <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
          </SwitchValue>
          <Cases>
            <CSharpSwitchCase TestValues=""0, 1"">
              <Statements>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""A"" />
                  </Arguments>
                </Call>
                <Goto Type=""System.Void"" Kind=""Break"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""1"" />
                  </Target>
                </Goto>
              </Statements>
            </CSharpSwitchCase>
          </Cases>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
        </CSharpSwitch>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_6832_C62D();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_6832_C62D() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_4E9F_42FD()
        {
            // (Expression<Action<int>>)(x => { switch (x) { case 0: Console.Write('A'); break; default: Console.Write('D'); break; } })
            var actual = GetDebugView(@"(Expression<Action<int>>)(x => { switch (x) { case 0: Console.Write('A'); break; default: Console.Write('D'); break; } })");
            var expected = @"
<Lambda Type=""System.Action`1[System.Int32]"">
  <Parameters>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpSwitch Type=""System.Void"">
          <SwitchValue>
            <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
          </SwitchValue>
          <Cases>
            <CSharpSwitchCase TestValues=""0"">
              <Statements>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""A"" />
                  </Arguments>
                </Call>
                <Goto Type=""System.Void"" Kind=""Break"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""1"" />
                  </Target>
                </Goto>
              </Statements>
            </CSharpSwitchCase>
            <CSharpSwitchCase TestValues=""default"">
              <Statements>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""D"" />
                  </Arguments>
                </Call>
                <Goto Type=""System.Void"" Kind=""Break"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""1"" />
                  </Target>
                </Goto>
              </Statements>
            </CSharpSwitchCase>
          </Cases>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
        </CSharpSwitch>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_4E9F_42FD();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_4E9F_42FD() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_30E5_8D77()
        {
            // (Expression<Action<int?>>)(x => { switch (x) { case 0: case null: Console.Write('N'); break; } })
            var actual = GetDebugView(@"(Expression<Action<int?>>)(x => { switch (x) { case 0: case null: Console.Write('N'); break; } })");
            var expected = @"
<Lambda Type=""System.Action`1[System.Nullable`1[System.Int32]]"">
  <Parameters>
    <Parameter Type=""System.Nullable`1[System.Int32]"" Id=""0"" Name=""x"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpSwitch Type=""System.Void"">
          <SwitchValue>
            <Parameter Type=""System.Nullable`1[System.Int32]"" Id=""0"" Name=""x"" />
          </SwitchValue>
          <Cases>
            <CSharpSwitchCase TestValues=""0, null"">
              <Statements>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""N"" />
                  </Arguments>
                </Call>
                <Goto Type=""System.Void"" Kind=""Break"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""1"" />
                  </Target>
                </Goto>
              </Statements>
            </CSharpSwitchCase>
          </Cases>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
        </CSharpSwitch>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_30E5_8D77();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_30E5_8D77() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_1754_0766()
        {
            // (Expression<Action<int?>>)(x => { switch (x) { case 0: goto case null; case null: Console.Write('N'); break; } })
            var actual = GetDebugView(@"(Expression<Action<int?>>)(x => { switch (x) { case 0: goto case null; case null: Console.Write('N'); break; } })");
            var expected = @"
<Lambda Type=""System.Action`1[System.Nullable`1[System.Int32]]"">
  <Parameters>
    <Parameter Type=""System.Nullable`1[System.Int32]"" Id=""0"" Name=""x"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpSwitch Type=""System.Void"">
          <SwitchValue>
            <Parameter Type=""System.Nullable`1[System.Int32]"" Id=""0"" Name=""x"" />
          </SwitchValue>
          <Cases>
            <CSharpSwitchCase TestValues=""0"">
              <Statements>
                <CSharpGotoCase Type=""System.Void"" Value=""null"" />
              </Statements>
            </CSharpSwitchCase>
            <CSharpSwitchCase TestValues=""null"">
              <Statements>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""N"" />
                  </Arguments>
                </Call>
                <Goto Type=""System.Void"" Kind=""Break"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""1"" />
                  </Target>
                </Goto>
              </Statements>
            </CSharpSwitchCase>
          </Cases>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
        </CSharpSwitch>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_1754_0766();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_1754_0766() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_AD7C_C86B()
        {
            // (Expression<Action<int?>>)(x => { switch (x) { case 0: Console.Write('N'); break; case null: goto case 0; } })
            var actual = GetDebugView(@"(Expression<Action<int?>>)(x => { switch (x) { case 0: Console.Write('N'); break; case null: goto case 0; } })");
            var expected = @"
<Lambda Type=""System.Action`1[System.Nullable`1[System.Int32]]"">
  <Parameters>
    <Parameter Type=""System.Nullable`1[System.Int32]"" Id=""0"" Name=""x"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpSwitch Type=""System.Void"">
          <SwitchValue>
            <Parameter Type=""System.Nullable`1[System.Int32]"" Id=""0"" Name=""x"" />
          </SwitchValue>
          <Cases>
            <CSharpSwitchCase TestValues=""0"">
              <Statements>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""N"" />
                  </Arguments>
                </Call>
                <Goto Type=""System.Void"" Kind=""Break"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""1"" />
                  </Target>
                </Goto>
              </Statements>
            </CSharpSwitchCase>
            <CSharpSwitchCase TestValues=""null"">
              <Statements>
                <CSharpGotoCase Type=""System.Void"" Value=""0"" />
              </Statements>
            </CSharpSwitchCase>
          </Cases>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
        </CSharpSwitch>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_AD7C_C86B();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_AD7C_C86B() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_3E56_6022()
        {
            // (Expression<Action<int?>>)(x => { switch (x) { case null: goto default; default: Console.Write('N'); break; } })
            var actual = GetDebugView(@"(Expression<Action<int?>>)(x => { switch (x) { case null: goto default; default: Console.Write('N'); break; } })");
            var expected = @"
<Lambda Type=""System.Action`1[System.Nullable`1[System.Int32]]"">
  <Parameters>
    <Parameter Type=""System.Nullable`1[System.Int32]"" Id=""0"" Name=""x"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpSwitch Type=""System.Void"">
          <SwitchValue>
            <Parameter Type=""System.Nullable`1[System.Int32]"" Id=""0"" Name=""x"" />
          </SwitchValue>
          <Cases>
            <CSharpSwitchCase TestValues=""null"">
              <Statements>
                <CSharpGotoDefault Type=""System.Void"" />
              </Statements>
            </CSharpSwitchCase>
            <CSharpSwitchCase TestValues=""default"">
              <Statements>
                <Call Type=""System.Void"" Method=""Void Write(Char)"">
                  <Arguments>
                    <Constant Type=""System.Char"" Value=""N"" />
                  </Arguments>
                </Call>
                <Goto Type=""System.Void"" Kind=""Break"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""1"" />
                  </Target>
                </Goto>
              </Statements>
            </CSharpSwitchCase>
          </Cases>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
        </CSharpSwitch>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_3E56_6022();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_3E56_6022() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_6D08_923C()
        {
            // (Expression<Action<int>>)(x => { switch (x) { default: break; } })
            var actual = GetDebugView(@"(Expression<Action<int>>)(x => { switch (x) { default: break; } })");
            var expected = @"
<Lambda Type=""System.Action`1[System.Int32]"">
  <Parameters>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpSwitch Type=""System.Void"">
          <SwitchValue>
            <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
          </SwitchValue>
          <Cases>
            <CSharpSwitchCase TestValues=""default"">
              <Statements>
                <Goto Type=""System.Void"" Kind=""Break"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""1"" />
                  </Target>
                </Goto>
              </Statements>
            </CSharpSwitchCase>
          </Cases>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""1"" />
          </BreakLabel>
        </CSharpSwitch>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_6D08_923C();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_6D08_923C() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_205E_7D00()
        {
            // (Expression<Action>)(() => { switch (int.Parse("1")) { } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { switch (int.Parse(""1"")) { } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpSwitch Type=""System.Void"">
          <SwitchValue>
            <Call Type=""System.Int32"" Method=""Int32 Parse(System.String)"">
              <Arguments>
                <Constant Type=""System.String"" Value=""1"" />
              </Arguments>
            </Call>
          </SwitchValue>
          <Cases />
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""0"" />
          </BreakLabel>
        </CSharpSwitch>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""1"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_205E_7D00();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_205E_7D00() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_A00C_0847()
        {
            // (Expression<Action>)(() => { switch (int.Parse("1")) { default: break; } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { switch (int.Parse(""1"")) { default: break; } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpSwitch Type=""System.Void"">
          <SwitchValue>
            <Call Type=""System.Int32"" Method=""Int32 Parse(System.String)"">
              <Arguments>
                <Constant Type=""System.String"" Value=""1"" />
              </Arguments>
            </Call>
          </SwitchValue>
          <Cases>
            <CSharpSwitchCase TestValues=""default"">
              <Statements>
                <Goto Type=""System.Void"" Kind=""Break"">
                  <Target>
                    <LabelTarget Type=""System.Void"" Id=""0"" />
                  </Target>
                </Goto>
              </Statements>
            </CSharpSwitchCase>
          </Cases>
          <BreakLabel>
            <LabelTarget Type=""System.Void"" Id=""0"" />
          </BreakLabel>
        </CSharpSwitch>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""1"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_A00C_0847();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_A00C_0847() => INCONCLUSIVE(); }

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
    partial class CompilerTests_CSharp30_Statements_Switch
    {
        partial class Reviewed
        {
            public override void CompilerTest_2156_D7F7() => OK();
            public override void CompilerTest_FCA9_3B3F() => OK();
            public override void CompilerTest_6832_C62D() => OK();
            public override void CompilerTest_4E9F_42FD() => OK();
            public override void CompilerTest_30E5_8D77() => OK();
            public override void CompilerTest_1754_0766() => OK();
            public override void CompilerTest_AD7C_C86B() => OK();
            public override void CompilerTest_3E56_6022() => OK();
            public override void CompilerTest_6D08_923C() => OK();
            public override void CompilerTest_205E_7D00() => OK();
            public override void CompilerTest_A00C_0847() => OK();
        }
    }
}
*/
}
