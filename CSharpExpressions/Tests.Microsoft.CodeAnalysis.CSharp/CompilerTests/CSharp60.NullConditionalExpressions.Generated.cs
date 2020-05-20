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
    public partial class CompilerTests_CSharp60_NullConditionalExpressions
    {
        [TestMethod]
        public void CompilerTest_B340_BC70()
        {
            // (Expression<Func<string, int?>>)(s => s?.Length)
            var actual = GetDebugView(@"(Expression<Func<string, int?>>)(s => s?.Length)");
            var expected = @"
<Lambda Type=""System.Func`2[System.String,System.Nullable`1[System.Int32]]"">
  <Parameters>
    <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
  </Parameters>
  <Body>
    <CSharpConditionalAccess Type=""System.Nullable`1[System.Int32]"">
      <Receiver>
        <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
      </Receiver>
      <NonNullReceiver>
        <ConditionalReceiver Id=""1"" Type=""System.String"" />
      </NonNullReceiver>
      <WhenNotNull>
        <MemberAccess Type=""System.Int32"" Member=""Int32 Length"">
          <Expression>
            <ConditionalReceiver Id=""1"" Type=""System.String"" />
          </Expression>
        </MemberAccess>
      </WhenNotNull>
    </CSharpConditionalAccess>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_B340_BC70();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_B340_BC70() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_9320_B6D2()
        {
            // (Expression<Func<string, int?>>)(s => s?.ToUpper()?.Length)
            var actual = GetDebugView(@"(Expression<Func<string, int?>>)(s => s?.ToUpper()?.Length)");
            var expected = @"
<Lambda Type=""System.Func`2[System.String,System.Nullable`1[System.Int32]]"">
  <Parameters>
    <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
  </Parameters>
  <Body>
    <CSharpConditionalAccess Type=""System.Nullable`1[System.Int32]"">
      <Receiver>
        <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
      </Receiver>
      <NonNullReceiver>
        <ConditionalReceiver Id=""1"" Type=""System.String"" />
      </NonNullReceiver>
      <WhenNotNull>
        <CSharpConditionalAccess Type=""System.Nullable`1[System.Int32]"">
          <Receiver>
            <Call Type=""System.String"" Method=""System.String ToUpper()"">
              <Object>
                <ConditionalReceiver Id=""1"" Type=""System.String"" />
              </Object>
              <Arguments />
            </Call>
          </Receiver>
          <NonNullReceiver>
            <ConditionalReceiver Id=""2"" Type=""System.String"" />
          </NonNullReceiver>
          <WhenNotNull>
            <MemberAccess Type=""System.Int32"" Member=""Int32 Length"">
              <Expression>
                <ConditionalReceiver Id=""2"" Type=""System.String"" />
              </Expression>
            </MemberAccess>
          </WhenNotNull>
        </CSharpConditionalAccess>
      </WhenNotNull>
    </CSharpConditionalAccess>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_9320_B6D2();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_9320_B6D2() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_A997_18C3()
        {
            // (Expression<Func<string, string>>)(s => s?.ToUpper())
            var actual = GetDebugView(@"(Expression<Func<string, string>>)(s => s?.ToUpper())");
            var expected = @"
<Lambda Type=""System.Func`2[System.String,System.String]"">
  <Parameters>
    <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
  </Parameters>
  <Body>
    <CSharpConditionalAccess Type=""System.String"">
      <Receiver>
        <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
      </Receiver>
      <NonNullReceiver>
        <ConditionalReceiver Id=""1"" Type=""System.String"" />
      </NonNullReceiver>
      <WhenNotNull>
        <Call Type=""System.String"" Method=""System.String ToUpper()"">
          <Object>
            <ConditionalReceiver Id=""1"" Type=""System.String"" />
          </Object>
          <Arguments />
        </Call>
      </WhenNotNull>
    </CSharpConditionalAccess>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_A997_18C3();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_A997_18C3() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_A5F9_6775()
        {
            // (Expression<Func<string, string>>)(s => s?.Substring(1)?.ToUpper())
            var actual = GetDebugView(@"(Expression<Func<string, string>>)(s => s?.Substring(1)?.ToUpper())");
            var expected = @"
<Lambda Type=""System.Func`2[System.String,System.String]"">
  <Parameters>
    <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
  </Parameters>
  <Body>
    <CSharpConditionalAccess Type=""System.String"">
      <Receiver>
        <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
      </Receiver>
      <NonNullReceiver>
        <ConditionalReceiver Id=""1"" Type=""System.String"" />
      </NonNullReceiver>
      <WhenNotNull>
        <CSharpConditionalAccess Type=""System.String"">
          <Receiver>
            <Call Type=""System.String"" Method=""System.String Substring(Int32)"">
              <Object>
                <ConditionalReceiver Id=""1"" Type=""System.String"" />
              </Object>
              <Arguments>
                <Constant Type=""System.Int32"" Value=""1"" />
              </Arguments>
            </Call>
          </Receiver>
          <NonNullReceiver>
            <ConditionalReceiver Id=""2"" Type=""System.String"" />
          </NonNullReceiver>
          <WhenNotNull>
            <Call Type=""System.String"" Method=""System.String ToUpper()"">
              <Object>
                <ConditionalReceiver Id=""2"" Type=""System.String"" />
              </Object>
              <Arguments />
            </Call>
          </WhenNotNull>
        </CSharpConditionalAccess>
      </WhenNotNull>
    </CSharpConditionalAccess>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_A5F9_6775();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_A5F9_6775() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_F165_9386()
        {
            // (Expression<Func<DateTimeOffset?, TimeSpan?>>)(d => d?.Offset)
            var actual = GetDebugView(@"(Expression<Func<DateTimeOffset?, TimeSpan?>>)(d => d?.Offset)");
            var expected = @"
<Lambda Type=""System.Func`2[System.Nullable`1[System.DateTimeOffset],System.Nullable`1[System.TimeSpan]]"">
  <Parameters>
    <Parameter Type=""System.Nullable`1[System.DateTimeOffset]"" Id=""0"" Name=""d"" />
  </Parameters>
  <Body>
    <CSharpConditionalAccess Type=""System.Nullable`1[System.TimeSpan]"">
      <Receiver>
        <Parameter Type=""System.Nullable`1[System.DateTimeOffset]"" Id=""0"" Name=""d"" />
      </Receiver>
      <NonNullReceiver>
        <ConditionalReceiver Id=""1"" Type=""System.DateTimeOffset"" />
      </NonNullReceiver>
      <WhenNotNull>
        <MemberAccess Type=""System.TimeSpan"" Member=""System.TimeSpan Offset"">
          <Expression>
            <ConditionalReceiver Id=""1"" Type=""System.DateTimeOffset"" />
          </Expression>
        </MemberAccess>
      </WhenNotNull>
    </CSharpConditionalAccess>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_F165_9386();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_F165_9386() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_2462_8DFD()
        {
            // (Expression<Func<DateTimeOffset?, int?>>)(d => (d?.Offset)?.Hours)
            var actual = GetDebugView(@"(Expression<Func<DateTimeOffset?, int?>>)(d => (d?.Offset)?.Hours)");
            var expected = @"
<Lambda Type=""System.Func`2[System.Nullable`1[System.DateTimeOffset],System.Nullable`1[System.Int32]]"">
  <Parameters>
    <Parameter Type=""System.Nullable`1[System.DateTimeOffset]"" Id=""0"" Name=""d"" />
  </Parameters>
  <Body>
    <CSharpConditionalAccess Type=""System.Nullable`1[System.Int32]"">
      <Receiver>
        <CSharpConditionalAccess Type=""System.Nullable`1[System.TimeSpan]"">
          <Receiver>
            <Parameter Type=""System.Nullable`1[System.DateTimeOffset]"" Id=""0"" Name=""d"" />
          </Receiver>
          <NonNullReceiver>
            <ConditionalReceiver Id=""1"" Type=""System.DateTimeOffset"" />
          </NonNullReceiver>
          <WhenNotNull>
            <MemberAccess Type=""System.TimeSpan"" Member=""System.TimeSpan Offset"">
              <Expression>
                <ConditionalReceiver Id=""1"" Type=""System.DateTimeOffset"" />
              </Expression>
            </MemberAccess>
          </WhenNotNull>
        </CSharpConditionalAccess>
      </Receiver>
      <NonNullReceiver>
        <ConditionalReceiver Id=""2"" Type=""System.TimeSpan"" />
      </NonNullReceiver>
      <WhenNotNull>
        <MemberAccess Type=""System.Int32"" Member=""Int32 Hours"">
          <Expression>
            <ConditionalReceiver Id=""2"" Type=""System.TimeSpan"" />
          </Expression>
        </MemberAccess>
      </WhenNotNull>
    </CSharpConditionalAccess>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_2462_8DFD();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_2462_8DFD() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_3041_FAE0()
        {
            // (Expression<Func<DateTimeOffset?, int?>>)(d => d?.Offset.Hours)
            var actual = GetDebugView(@"(Expression<Func<DateTimeOffset?, int?>>)(d => d?.Offset.Hours)");
            var expected = @"
<Lambda Type=""System.Func`2[System.Nullable`1[System.DateTimeOffset],System.Nullable`1[System.Int32]]"">
  <Parameters>
    <Parameter Type=""System.Nullable`1[System.DateTimeOffset]"" Id=""0"" Name=""d"" />
  </Parameters>
  <Body>
    <CSharpConditionalAccess Type=""System.Nullable`1[System.Int32]"">
      <Receiver>
        <Parameter Type=""System.Nullable`1[System.DateTimeOffset]"" Id=""0"" Name=""d"" />
      </Receiver>
      <NonNullReceiver>
        <ConditionalReceiver Id=""1"" Type=""System.DateTimeOffset"" />
      </NonNullReceiver>
      <WhenNotNull>
        <MemberAccess Type=""System.Int32"" Member=""Int32 Hours"">
          <Expression>
            <MemberAccess Type=""System.TimeSpan"" Member=""System.TimeSpan Offset"">
              <Expression>
                <ConditionalReceiver Id=""1"" Type=""System.DateTimeOffset"" />
              </Expression>
            </MemberAccess>
          </Expression>
        </MemberAccess>
      </WhenNotNull>
    </CSharpConditionalAccess>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_3041_FAE0();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_3041_FAE0() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_CB0C_60AB()
        {
            // (Expression<Func<string, char?>>)(s => s?[42])
            var actual = GetDebugView(@"(Expression<Func<string, char?>>)(s => s?[42])");
            var expected = @"
<Lambda Type=""System.Func`2[System.String,System.Nullable`1[System.Char]]"">
  <Parameters>
    <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
  </Parameters>
  <Body>
    <CSharpConditionalAccess Type=""System.Nullable`1[System.Char]"">
      <Receiver>
        <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
      </Receiver>
      <NonNullReceiver>
        <ConditionalReceiver Id=""1"" Type=""System.String"" />
      </NonNullReceiver>
      <WhenNotNull>
        <CSharpIndex Type=""System.Char"" Indexer=""Char Chars [Int32]"">
          <Object>
            <ConditionalReceiver Id=""1"" Type=""System.String"" />
          </Object>
          <Arguments>
            <ParameterAssignment Parameter=""Int32 index"">
              <Expression>
                <Constant Type=""System.Int32"" Value=""42"" />
              </Expression>
            </ParameterAssignment>
          </Arguments>
        </CSharpIndex>
      </WhenNotNull>
    </CSharpConditionalAccess>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_CB0C_60AB();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_CB0C_60AB() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_CF40_3D45()
        {
            // (Expression<Func<Func<int, int>, int?>>)(f => f?.Invoke(42))
            var actual = GetDebugView(@"(Expression<Func<Func<int, int>, int?>>)(f => f?.Invoke(42))");
            var expected = @"
<Lambda Type=""System.Func`2[System.Func`2[System.Int32,System.Int32],System.Nullable`1[System.Int32]]"">
  <Parameters>
    <Parameter Type=""System.Func`2[System.Int32,System.Int32]"" Id=""0"" Name=""f"" />
  </Parameters>
  <Body>
    <CSharpConditionalAccess Type=""System.Nullable`1[System.Int32]"">
      <Receiver>
        <Parameter Type=""System.Func`2[System.Int32,System.Int32]"" Id=""0"" Name=""f"" />
      </Receiver>
      <NonNullReceiver>
        <ConditionalReceiver Id=""1"" Type=""System.Func`2[System.Int32,System.Int32]"" />
      </NonNullReceiver>
      <WhenNotNull>
        <Call Type=""System.Int32"" Method=""Int32 Invoke(Int32)"">
          <Object>
            <ConditionalReceiver Id=""1"" Type=""System.Func`2[System.Int32,System.Int32]"" />
          </Object>
          <Arguments>
            <Constant Type=""System.Int32"" Value=""42"" />
          </Arguments>
        </Call>
      </WhenNotNull>
    </CSharpConditionalAccess>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_CF40_3D45();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_CF40_3D45() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_4241_E360()
        {
            // (Expression<Func<string, string>>)(s => s?.Substring(length: 1, startIndex: 0).ToUpper()?.ToLower())
            var actual = GetDebugView(@"(Expression<Func<string, string>>)(s => s?.Substring(length: 1, startIndex: 0).ToUpper()?.ToLower())");
            var expected = @"
<Lambda Type=""System.Func`2[System.String,System.String]"">
  <Parameters>
    <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
  </Parameters>
  <Body>
    <CSharpConditionalAccess Type=""System.String"">
      <Receiver>
        <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
      </Receiver>
      <NonNullReceiver>
        <ConditionalReceiver Id=""1"" Type=""System.String"" />
      </NonNullReceiver>
      <WhenNotNull>
        <CSharpConditionalAccess Type=""System.String"">
          <Receiver>
            <Call Type=""System.String"" Method=""System.String ToUpper()"">
              <Object>
                <CSharpCall Type=""System.String"" Method=""System.String Substring(Int32, Int32)"">
                  <Object>
                    <ConditionalReceiver Id=""1"" Type=""System.String"" />
                  </Object>
                  <Arguments>
                    <ParameterAssignment Parameter=""Int32 length"">
                      <Expression>
                        <Constant Type=""System.Int32"" Value=""1"" />
                      </Expression>
                    </ParameterAssignment>
                    <ParameterAssignment Parameter=""Int32 startIndex"">
                      <Expression>
                        <Constant Type=""System.Int32"" Value=""0"" />
                      </Expression>
                    </ParameterAssignment>
                  </Arguments>
                </CSharpCall>
              </Object>
              <Arguments />
            </Call>
          </Receiver>
          <NonNullReceiver>
            <ConditionalReceiver Id=""2"" Type=""System.String"" />
          </NonNullReceiver>
          <WhenNotNull>
            <Call Type=""System.String"" Method=""System.String ToLower()"">
              <Object>
                <ConditionalReceiver Id=""2"" Type=""System.String"" />
              </Object>
              <Arguments />
            </Call>
          </WhenNotNull>
        </CSharpConditionalAccess>
      </WhenNotNull>
    </CSharpConditionalAccess>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_4241_E360();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_4241_E360() => INCONCLUSIVE(); }

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
    partial class CompilerTests_CSharp60_NullConditionalExpressions
    {
        partial class Reviewed
        {
            public override void CompilerTest_B340_BC70() => OK();
            public override void CompilerTest_9320_B6D2() => OK();
            public override void CompilerTest_A997_18C3() => OK();
            public override void CompilerTest_A5F9_6775() => OK();
            public override void CompilerTest_F165_9386() => OK();
            public override void CompilerTest_2462_8DFD() => OK();
            public override void CompilerTest_3041_FAE0() => OK();
            public override void CompilerTest_CB0C_60AB() => OK();
            public override void CompilerTest_CF40_3D45() => OK();
            public override void CompilerTest_4241_E360() => OK();
        }
    }
}
*/
}