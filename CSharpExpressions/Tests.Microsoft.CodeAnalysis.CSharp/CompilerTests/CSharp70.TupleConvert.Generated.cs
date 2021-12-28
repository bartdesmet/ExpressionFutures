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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Microsoft.CodeAnalysis.CSharp.TestUtilities;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    [TestClass]
    public partial class CompilerTests_CSharp70_TupleConvert
    {
        [TestMethod]
        public void CompilerTest_5948_DA1F()
        {
            // (Expression<Func<(int, DateTime), (int, DateTime)?>>)(t => t)
            var actual = GetDebugView(@"(Expression<Func<(int, DateTime), (int, DateTime)?>>)(t => t)");
            var expected = @"
<Lambda Type=""System.Func`2[System.ValueTuple`2[System.Int32,System.DateTime],System.Nullable`1[System.ValueTuple`2[System.Int32,System.DateTime]]]"">
  <Parameters>
    <Parameter Type=""System.ValueTuple`2[System.Int32,System.DateTime]"" Id=""0"" Name=""t"" />
  </Parameters>
  <Body>
    <Convert Type=""System.Nullable`1[System.ValueTuple`2[System.Int32,System.DateTime]]"" IsLifted=""true"" IsLiftedToNull=""true"">
      <Operand>
        <Parameter Type=""System.ValueTuple`2[System.Int32,System.DateTime]"" Id=""0"" Name=""t"" />
      </Operand>
    </Convert>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_5948_DA1F();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_5948_DA1F() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_D7DE_3D5E()
        {
            // (Expression<Func<(int, DateTime), (long, DateTimeOffset)>>)(t => t)
            var actual = GetDebugView(@"(Expression<Func<(int, DateTime), (long, DateTimeOffset)>>)(t => t)");
            var expected = @"
<Lambda Type=""System.Func`2[System.ValueTuple`2[System.Int32,System.DateTime],System.ValueTuple`2[System.Int64,System.DateTimeOffset]]"">
  <Parameters>
    <Parameter Type=""System.ValueTuple`2[System.Int32,System.DateTime]"" Id=""0"" Name=""t"" />
  </Parameters>
  <Body>
    <CSharpTupleConvert Type=""System.ValueTuple`2[System.Int64,System.DateTimeOffset]"">
      <Operand>
        <Parameter Type=""System.ValueTuple`2[System.Int32,System.DateTime]"" Id=""0"" Name=""t"" />
      </Operand>
      <ElementConversions>
        <Lambda Type=""System.Func`2[System.Int32,System.Int64]"">
          <Parameters>
            <Parameter Type=""System.Int32"" Id=""1"" Name=""p"" />
          </Parameters>
          <Body>
            <Convert Type=""System.Int64"">
              <Operand>
                <Parameter Type=""System.Int32"" Id=""1"" Name=""p"" />
              </Operand>
            </Convert>
          </Body>
        </Lambda>
        <Lambda Type=""System.Func`2[System.DateTime,System.DateTimeOffset]"">
          <Parameters>
            <Parameter Type=""System.DateTime"" Id=""2"" Name=""p"" />
          </Parameters>
          <Body>
            <Convert Type=""System.DateTimeOffset"" Method=""System.DateTimeOffset op_Implicit(System.DateTime)"">
              <Operand>
                <Parameter Type=""System.DateTime"" Id=""2"" Name=""p"" />
              </Operand>
            </Convert>
          </Body>
        </Lambda>
      </ElementConversions>
    </CSharpTupleConvert>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_D7DE_3D5E();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_D7DE_3D5E() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_2695_BEC3()
        {
            // (Expression<Func<(int, DateTime), (int?, DateTime?)>>)(t => t)
            var actual = GetDebugView(@"(Expression<Func<(int, DateTime), (int?, DateTime?)>>)(t => t)");
            var expected = @"
<Lambda Type=""System.Func`2[System.ValueTuple`2[System.Int32,System.DateTime],System.ValueTuple`2[System.Nullable`1[System.Int32],System.Nullable`1[System.DateTime]]]"">
  <Parameters>
    <Parameter Type=""System.ValueTuple`2[System.Int32,System.DateTime]"" Id=""0"" Name=""t"" />
  </Parameters>
  <Body>
    <CSharpTupleConvert Type=""System.ValueTuple`2[System.Nullable`1[System.Int32],System.Nullable`1[System.DateTime]]"">
      <Operand>
        <Parameter Type=""System.ValueTuple`2[System.Int32,System.DateTime]"" Id=""0"" Name=""t"" />
      </Operand>
      <ElementConversions>
        <Lambda Type=""System.Func`2[System.Int32,System.Nullable`1[System.Int32]]"">
          <Parameters>
            <Parameter Type=""System.Int32"" Id=""1"" Name=""p"" />
          </Parameters>
          <Body>
            <Convert Type=""System.Nullable`1[System.Int32]"" IsLifted=""true"" IsLiftedToNull=""true"">
              <Operand>
                <Parameter Type=""System.Int32"" Id=""1"" Name=""p"" />
              </Operand>
            </Convert>
          </Body>
        </Lambda>
        <Lambda Type=""System.Func`2[System.DateTime,System.Nullable`1[System.DateTime]]"">
          <Parameters>
            <Parameter Type=""System.DateTime"" Id=""2"" Name=""p"" />
          </Parameters>
          <Body>
            <Convert Type=""System.Nullable`1[System.DateTime]"" IsLifted=""true"" IsLiftedToNull=""true"">
              <Operand>
                <Parameter Type=""System.DateTime"" Id=""2"" Name=""p"" />
              </Operand>
            </Convert>
          </Body>
        </Lambda>
      </ElementConversions>
    </CSharpTupleConvert>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_2695_BEC3();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_2695_BEC3() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_7419_0AE5()
        {
            // (Expression<Func<(int, int), (object, IFormattable)>>)(t => t)
            var actual = GetDebugView(@"(Expression<Func<(int, int), (object, IFormattable)>>)(t => t)");
            var expected = @"
<Lambda Type=""System.Func`2[System.ValueTuple`2[System.Int32,System.Int32],System.ValueTuple`2[System.Object,System.IFormattable]]"">
  <Parameters>
    <Parameter Type=""System.ValueTuple`2[System.Int32,System.Int32]"" Id=""0"" Name=""t"" />
  </Parameters>
  <Body>
    <CSharpTupleConvert Type=""System.ValueTuple`2[System.Object,System.IFormattable]"">
      <Operand>
        <Parameter Type=""System.ValueTuple`2[System.Int32,System.Int32]"" Id=""0"" Name=""t"" />
      </Operand>
      <ElementConversions>
        <Lambda Type=""System.Func`2[System.Int32,System.Object]"">
          <Parameters>
            <Parameter Type=""System.Int32"" Id=""1"" Name=""p"" />
          </Parameters>
          <Body>
            <Convert Type=""System.Object"">
              <Operand>
                <Parameter Type=""System.Int32"" Id=""1"" Name=""p"" />
              </Operand>
            </Convert>
          </Body>
        </Lambda>
        <Lambda Type=""System.Func`2[System.Int32,System.IFormattable]"">
          <Parameters>
            <Parameter Type=""System.Int32"" Id=""2"" Name=""p"" />
          </Parameters>
          <Body>
            <Convert Type=""System.IFormattable"">
              <Operand>
                <Parameter Type=""System.Int32"" Id=""2"" Name=""p"" />
              </Operand>
            </Convert>
          </Body>
        </Lambda>
      </ElementConversions>
    </CSharpTupleConvert>
  </Body>
</Lambda>";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_7419_0AE5();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_7419_0AE5() => INCONCLUSIVE(); }

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
    partial class CompilerTests_CSharp70_TupleConvert
    {
        partial class Reviewed
        {
            public override void CompilerTest_5948_DA1F() => OK();
            public override void CompilerTest_D7DE_3D5E() => OK();
            public override void CompilerTest_2695_BEC3() => OK();
            public override void CompilerTest_7419_0AE5() => OK();
        }
    }
}
*/
}
