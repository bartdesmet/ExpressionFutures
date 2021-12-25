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
    public partial class CompilerTests_CSharp30_Statements_Using
    {
        [TestMethod]
        public void CompilerTest_5598_03A6()
        {
            // (Expression<Action<IDisposable>>)(d => { using (d) Console.Write('.'); })
            var actual = GetDebugView(@"(Expression<Action<IDisposable>>)(d => { using (d) Console.Write('.'); })");
            var expected = @"
<Lambda Type=""System.Action`1[System.IDisposable]"">
  <Parameters>
    <Parameter Type=""System.IDisposable"" Id=""0"" Name=""d"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpUsing Type=""System.Void"">
          <Resource>
            <Parameter Type=""System.IDisposable"" Id=""0"" Name=""d"" />
          </Resource>
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
        </CSharpUsing>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""1"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_5598_03A6();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_5598_03A6() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_62CA_03A6()
        {
            // (Expression<Action<IDisposable>>)(d => { using (d) { Console.Write('.'); } })
            var actual = GetDebugView(@"(Expression<Action<IDisposable>>)(d => { using (d) { Console.Write('.'); } })");
            var expected = @"
<Lambda Type=""System.Action`1[System.IDisposable]"">
  <Parameters>
    <Parameter Type=""System.IDisposable"" Id=""0"" Name=""d"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpUsing Type=""System.Void"">
          <Resource>
            <Parameter Type=""System.IDisposable"" Id=""0"" Name=""d"" />
          </Resource>
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
        </CSharpUsing>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""1"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_62CA_03A6();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_62CA_03A6() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_BB7C_25AF()
        {
            // (Expression<Action<IDisposable>>)(d => { using (var e = d) Console.WriteLine(e); })
            var actual = GetDebugView(@"(Expression<Action<IDisposable>>)(d => { using (var e = d) Console.WriteLine(e); })");
            var expected = @"
<Lambda Type=""System.Action`1[System.IDisposable]"">
  <Parameters>
    <Parameter Type=""System.IDisposable"" Id=""0"" Name=""d"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpUsing Type=""System.Void"">
          <Variables>
            <Parameter Type=""System.IDisposable"" Id=""1"" Name=""e"" />
          </Variables>
          <Declarations>
            <LocalDeclaration>
              <Variable>
                <Parameter Type=""System.IDisposable"" Id=""1"" Name=""e"" />
              </Variable>
              <Expression>
                <Parameter Type=""System.IDisposable"" Id=""0"" Name=""d"" />
              </Expression>
            </LocalDeclaration>
          </Declarations>
          <Body>
            <Call Type=""System.Void"" Method=""Void WriteLine(System.Object)"">
              <Arguments>
                <Parameter Type=""System.IDisposable"" Id=""1"" Name=""e"" />
              </Arguments>
            </Call>
          </Body>
        </CSharpUsing>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_BB7C_25AF();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_BB7C_25AF() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_51A3_8AB4()
        {
            // (Expression<Action<IDisposable>>)(d => { using (var e = d) { Console.WriteLine(e); } })
            var actual = GetDebugView(@"(Expression<Action<IDisposable>>)(d => { using (var e = d) { Console.WriteLine(e); } })");
            var expected = @"
<Lambda Type=""System.Action`1[System.IDisposable]"">
  <Parameters>
    <Parameter Type=""System.IDisposable"" Id=""0"" Name=""d"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpUsing Type=""System.Void"">
          <Variables>
            <Parameter Type=""System.IDisposable"" Id=""1"" Name=""e"" />
          </Variables>
          <Declarations>
            <LocalDeclaration>
              <Variable>
                <Parameter Type=""System.IDisposable"" Id=""1"" Name=""e"" />
              </Variable>
              <Expression>
                <Parameter Type=""System.IDisposable"" Id=""0"" Name=""d"" />
              </Expression>
            </LocalDeclaration>
          </Declarations>
          <Body>
            <Block Type=""System.Void"">
              <Expressions>
                <Call Type=""System.Void"" Method=""Void WriteLine(System.Object)"">
                  <Arguments>
                    <Parameter Type=""System.IDisposable"" Id=""1"" Name=""e"" />
                  </Arguments>
                </Call>
              </Expressions>
            </Block>
          </Body>
        </CSharpUsing>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_51A3_8AB4();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_51A3_8AB4() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_57C3_FE40()
        {
            // (Expression<Action>)(() => { using (var fs = File.OpenRead("foo.txt")) { } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { using (var fs = File.OpenRead(""foo.txt"")) { } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpUsing Type=""System.Void"">
          <Variables>
            <Parameter Type=""System.IO.FileStream"" Id=""0"" Name=""fs"" />
          </Variables>
          <Declarations>
            <LocalDeclaration>
              <Variable>
                <Parameter Type=""System.IO.FileStream"" Id=""0"" Name=""fs"" />
              </Variable>
              <Expression>
                <Call Type=""System.IO.FileStream"" Method=""System.IO.FileStream OpenRead(System.String)"">
                  <Arguments>
                    <Constant Type=""System.String"" Value=""foo.txt"" />
                  </Arguments>
                </Call>
              </Expression>
            </LocalDeclaration>
          </Declarations>
          <Body>
            <Default Type=""System.Void"" />
          </Body>
        </CSharpUsing>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""1"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_57C3_FE40();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_57C3_FE40() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_7AF8_56F5()
        {
            // (Expression<Action>)(() => { using (FileStream fs1 = File.OpenRead("foo.txt"), fs2 = File.OpenRead("bar.txt")) { } })
            var actual = GetDebugView(@"(Expression<Action>)(() => { using (FileStream fs1 = File.OpenRead(""foo.txt""), fs2 = File.OpenRead(""bar.txt"")) { } })");
            var expected = @"
<Lambda Type=""System.Action"">
  <Parameters />
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpUsing Type=""System.Void"">
          <Variables>
            <Parameter Type=""System.IO.FileStream"" Id=""0"" Name=""fs1"" />
            <Parameter Type=""System.IO.FileStream"" Id=""1"" Name=""fs2"" />
          </Variables>
          <Declarations>
            <LocalDeclaration>
              <Variable>
                <Parameter Type=""System.IO.FileStream"" Id=""0"" Name=""fs1"" />
              </Variable>
              <Expression>
                <Call Type=""System.IO.FileStream"" Method=""System.IO.FileStream OpenRead(System.String)"">
                  <Arguments>
                    <Constant Type=""System.String"" Value=""foo.txt"" />
                  </Arguments>
                </Call>
              </Expression>
            </LocalDeclaration>
            <LocalDeclaration>
              <Variable>
                <Parameter Type=""System.IO.FileStream"" Id=""1"" Name=""fs2"" />
              </Variable>
              <Expression>
                <Call Type=""System.IO.FileStream"" Method=""System.IO.FileStream OpenRead(System.String)"">
                  <Arguments>
                    <Constant Type=""System.String"" Value=""bar.txt"" />
                  </Arguments>
                </Call>
              </Expression>
            </LocalDeclaration>
          </Declarations>
          <Body>
            <Default Type=""System.Void"" />
          </Body>
        </CSharpUsing>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_7AF8_56F5();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_7AF8_56F5() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_7005_B197()
        {
            // (Expression<Action<object>>)(o => { using (o is IDisposable d ? d : null) {} })
            var actual = GetDebugView(@"(Expression<Action<object>>)(o => { using (o is IDisposable d ? d : null) {} })");
            var expected = @"
<Lambda Type=""System.Action`1[System.Object]"">
  <Parameters>
    <Parameter Type=""System.Object"" Id=""0"" Name=""o"" />
  </Parameters>
  <Body>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <CSharpUsing Type=""System.Void"">
          <Variables>
            <Parameter Type=""System.IDisposable"" Id=""1"" Name=""d"" />
          </Variables>
          <Resource>
            <Conditional Type=""System.IDisposable"">
              <Test>
                <CSharpIsPattern Type=""System.Boolean"">
                  <Expression>
                    <Parameter Type=""System.Object"" Id=""0"" Name=""o"" />
                  </Expression>
                  <Pattern>
                    <DeclarationPattern InputType=""System.Object"" NarrowedType=""System.IDisposable"" Type=""System.IDisposable"">
                      <Variable>
                        <Parameter Type=""System.IDisposable"" Id=""1"" Name=""d"" />
                      </Variable>
                    </DeclarationPattern>
                  </Pattern>
                </CSharpIsPattern>
              </Test>
              <IfTrue>
                <Parameter Type=""System.IDisposable"" Id=""1"" Name=""d"" />
              </IfTrue>
              <IfFalse>
                <Constant Type=""System.IDisposable"" Value=""null"" />
              </IfFalse>
            </Conditional>
          </Resource>
          <Body>
            <Default Type=""System.Void"" />
          </Body>
        </CSharpUsing>
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""2"" />
      </ReturnLabel>
    </CSharpBlock>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_7005_B197();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_7005_B197() => INCONCLUSIVE(); }

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
    partial class CompilerTests_CSharp30_Statements_Using
    {
        partial class Reviewed
        {
            public override void CompilerTest_5598_03A6() => OK();
            public override void CompilerTest_62CA_03A6() => OK();
            public override void CompilerTest_BB7C_25AF() => OK();
            public override void CompilerTest_51A3_8AB4() => OK();
            public override void CompilerTest_57C3_FE40() => OK();
            public override void CompilerTest_7AF8_56F5() => OK();
            public override void CompilerTest_7005_B197() => OK();
        }
    }
}
*/
}