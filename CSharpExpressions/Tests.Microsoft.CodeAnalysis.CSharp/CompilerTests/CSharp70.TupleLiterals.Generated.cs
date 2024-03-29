﻿// Prototyping extended expression trees for C#.
//
// bartde - May 2020

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
    public partial class CompilerTests_CSharp70_TupleLiterals
    {
        [Fact]
        public void CompilerTest_4091_A89E()
        {
            // (Expression<Func<(int, string)>>)(() => (42, "bar"))
            var actual = GetDebugView(@"(Expression<Func<(int, string)>>)(() => (42, ""bar""))");
            var expected = @"
<Lambda Type=""System.Func`1[System.ValueTuple`2[System.Int32,System.String]]"">
  <Parameters />
  <Body>
    <TupleLiteral Type=""System.ValueTuple`2[System.Int32,System.String]"">
      <Arguments>
        <Constant Type=""System.Int32"" Value=""42"" />
        <Constant Type=""System.String"" Value=""bar"" />
      </Arguments>
    </TupleLiteral>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_4091_A89E();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_4091_A89E() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_ACFD_39F6()
        {
            // (Expression<Func<int, string, (int, string)>>)((i, s) => (i, s))
            var actual = GetDebugView(@"(Expression<Func<int, string, (int, string)>>)((i, s) => (i, s))");
            var expected = @"
<Lambda Type=""System.Func`3[System.Int32,System.String,System.ValueTuple`2[System.Int32,System.String]]"">
  <Parameters>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
    <Parameter Type=""System.String"" Id=""1"" Name=""s"" />
  </Parameters>
  <Body>
    <TupleLiteral Type=""System.ValueTuple`2[System.Int32,System.String]"">
      <Arguments>
        <Argument Name=""i"">
          <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
        </Argument>
        <Argument Name=""s"">
          <Parameter Type=""System.String"" Id=""1"" Name=""s"" />
        </Argument>
      </Arguments>
    </TupleLiteral>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_ACFD_39F6();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_ACFD_39F6() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_694F_FB96()
        {
            // (Expression<Func<int, string, (int, string)>>)((i, s) => (x: i, name: s))
            var actual = GetDebugView(@"(Expression<Func<int, string, (int, string)>>)((i, s) => (x: i, name: s))");
            var expected = @"
<Lambda Type=""System.Func`3[System.Int32,System.String,System.ValueTuple`2[System.Int32,System.String]]"">
  <Parameters>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
    <Parameter Type=""System.String"" Id=""1"" Name=""s"" />
  </Parameters>
  <Body>
    <TupleLiteral Type=""System.ValueTuple`2[System.Int32,System.String]"">
      <Arguments>
        <Argument Name=""x"">
          <Parameter Type=""System.Int32"" Id=""0"" Name=""i"" />
        </Argument>
        <Argument Name=""name"">
          <Parameter Type=""System.String"" Id=""1"" Name=""s"" />
        </Argument>
      </Arguments>
    </TupleLiteral>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_694F_FB96();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_694F_FB96() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_2E96_C81B()
        {
            // (Expression<Func<(int, string), int>>)(t => t.Item1)
            var actual = GetDebugView(@"(Expression<Func<(int, string), int>>)(t => t.Item1)");
            var expected = @"
<Lambda Type=""System.Func`2[System.ValueTuple`2[System.Int32,System.String],System.Int32]"">
  <Parameters>
    <Parameter Type=""System.ValueTuple`2[System.Int32,System.String]"" Id=""0"" Name=""t"" />
  </Parameters>
  <Body>
    <MemberAccess Type=""System.Int32"" Member=""Int32 Item1"">
      <Expression>
        <Parameter Type=""System.ValueTuple`2[System.Int32,System.String]"" Id=""0"" Name=""t"" />
      </Expression>
    </MemberAccess>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_2E96_C81B();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_2E96_C81B() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_B949_DD0E()
        {
            // (Expression<Func<(int, string), string>>)(t => t.Item2)
            var actual = GetDebugView(@"(Expression<Func<(int, string), string>>)(t => t.Item2)");
            var expected = @"
<Lambda Type=""System.Func`2[System.ValueTuple`2[System.Int32,System.String],System.String]"">
  <Parameters>
    <Parameter Type=""System.ValueTuple`2[System.Int32,System.String]"" Id=""0"" Name=""t"" />
  </Parameters>
  <Body>
    <MemberAccess Type=""System.String"" Member=""System.String Item2"">
      <Expression>
        <Parameter Type=""System.ValueTuple`2[System.Int32,System.String]"" Id=""0"" Name=""t"" />
      </Expression>
    </MemberAccess>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_B949_DD0E();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_B949_DD0E() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_BC9B_C81B()
        {
            // (Expression<Func<(int x, string s), int>>)(t => t.x)
            var actual = GetDebugView(@"(Expression<Func<(int x, string s), int>>)(t => t.x)");
            var expected = @"
<Lambda Type=""System.Func`2[System.ValueTuple`2[System.Int32,System.String],System.Int32]"">
  <Parameters>
    <Parameter Type=""System.ValueTuple`2[System.Int32,System.String]"" Id=""0"" Name=""t"" />
  </Parameters>
  <Body>
    <MemberAccess Type=""System.Int32"" Member=""Int32 Item1"">
      <Expression>
        <Parameter Type=""System.ValueTuple`2[System.Int32,System.String]"" Id=""0"" Name=""t"" />
      </Expression>
    </MemberAccess>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_BC9B_C81B();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_BC9B_C81B() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_8000_DD0E()
        {
            // (Expression<Func<(int x, string s), string>>)(t => t.s)
            var actual = GetDebugView(@"(Expression<Func<(int x, string s), string>>)(t => t.s)");
            var expected = @"
<Lambda Type=""System.Func`2[System.ValueTuple`2[System.Int32,System.String],System.String]"">
  <Parameters>
    <Parameter Type=""System.ValueTuple`2[System.Int32,System.String]"" Id=""0"" Name=""t"" />
  </Parameters>
  <Body>
    <MemberAccess Type=""System.String"" Member=""System.String Item2"">
      <Expression>
        <Parameter Type=""System.ValueTuple`2[System.Int32,System.String]"" Id=""0"" Name=""t"" />
      </Expression>
    </MemberAccess>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_8000_DD0E();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_8000_DD0E() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_2A80_EB79()
        {
            // (Expression<Func<(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j), int>>)(t => t.j)
            var actual = GetDebugView(@"(Expression<Func<(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j), int>>)(t => t.j)");
            var expected = @"
<Lambda Type=""System.Func`2[System.ValueTuple`8[System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.ValueTuple`3[System.Int32,System.Int32,System.Int32]],System.Int32]"">
  <Parameters>
    <Parameter Type=""System.ValueTuple`8[System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.ValueTuple`3[System.Int32,System.Int32,System.Int32]]"" Id=""0"" Name=""t"" />
  </Parameters>
  <Body>
    <MemberAccess Type=""System.Int32"" Member=""Int32 Item3"">
      <Expression>
        <MemberAccess Type=""System.ValueTuple`3[System.Int32,System.Int32,System.Int32]"" Member=""System.ValueTuple`3[System.Int32,System.Int32,System.Int32] Rest"">
          <Expression>
            <Parameter Type=""System.ValueTuple`8[System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.ValueTuple`3[System.Int32,System.Int32,System.Int32]]"" Id=""0"" Name=""t"" />
          </Expression>
        </MemberAccess>
      </Expression>
    </MemberAccess>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_2A80_EB79();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_2A80_EB79() => INCONCLUSIVE(); }

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
    partial class CompilerTests_CSharp70_TupleLiterals
    {
        partial class Reviewed
        {
            public override void CompilerTest_4091_A89E() => OK();
            public override void CompilerTest_ACFD_39F6() => OK();
            public override void CompilerTest_694F_FB96() => OK();
            public override void CompilerTest_2E96_C81B() => OK();
            public override void CompilerTest_B949_DD0E() => OK();
            public override void CompilerTest_BC9B_C81B() => OK();
            public override void CompilerTest_8000_DD0E() => OK();
            public override void CompilerTest_2A80_EB79() => OK();
        }
    }
}
*/
}
