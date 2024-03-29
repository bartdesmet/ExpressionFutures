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
    public partial class CompilerTests_CSharp50_AsyncAwait
    {
        [Fact]
        public void CompilerTest_0FFA_9FD5()
        {
            // (Expression<Func<Task<int>, Task<int>>>)(async t => await t)
            var actual = GetDebugView(@"(Expression<Func<Task<int>, Task<int>>>)(async t => await t)");
            var expected = @"
<Lambda Type=""System.Func`2[System.Threading.Tasks.Task`1[System.Int32],System.Threading.Tasks.Task`1[System.Int32]]"">
  <Parameters>
    <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""0"" Name=""t"" />
  </Parameters>
  <Body>
    <Invoke Type=""System.Threading.Tasks.Task`1[System.Int32]"">
      <Expression>
        <CSharpAsyncLambda Type=""System.Func`2[System.Threading.Tasks.Task`1[System.Int32],System.Threading.Tasks.Task`1[System.Int32]]"">
          <Parameters>
            <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""0"" Name=""t"" />
          </Parameters>
          <Body>
            <CSharpAwait Type=""System.Int32"">
              <Info>
                <StaticAwaitInfo IsCompleted=""Boolean IsCompleted"" GetResult=""Int32 GetResult()"">
                  <GetAwaiter>
                    <Lambda Type=""System.Func`2[System.Threading.Tasks.Task`1[System.Int32],System.Runtime.CompilerServices.TaskAwaiter`1[System.Int32]]"">
                      <Parameters>
                        <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""1"" Name=""p"" />
                      </Parameters>
                      <Body>
                        <Call Type=""System.Runtime.CompilerServices.TaskAwaiter`1[System.Int32]"" Method=""System.Runtime.CompilerServices.TaskAwaiter`1[System.Int32] GetAwaiter()"">
                          <Object>
                            <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""1"" Name=""p"" />
                          </Object>
                          <Arguments />
                        </Call>
                      </Body>
                    </Lambda>
                  </GetAwaiter>
                </StaticAwaitInfo>
              </Info>
              <Operand>
                <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""0"" Name=""t"" />
              </Operand>
            </CSharpAwait>
          </Body>
        </CSharpAsyncLambda>
      </Expression>
      <Arguments>
        <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""0"" Name=""t"" />
      </Arguments>
    </Invoke>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_0FFA_9FD5();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_0FFA_9FD5() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_83AE_58B0()
        {
            // (Expression<Func<Task<int>, Task<int>>>)(async t => await t.ConfigureAwait(false))
            var actual = GetDebugView(@"(Expression<Func<Task<int>, Task<int>>>)(async t => await t.ConfigureAwait(false))");
            var expected = @"
<Lambda Type=""System.Func`2[System.Threading.Tasks.Task`1[System.Int32],System.Threading.Tasks.Task`1[System.Int32]]"">
  <Parameters>
    <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""0"" Name=""t"" />
  </Parameters>
  <Body>
    <Invoke Type=""System.Threading.Tasks.Task`1[System.Int32]"">
      <Expression>
        <CSharpAsyncLambda Type=""System.Func`2[System.Threading.Tasks.Task`1[System.Int32],System.Threading.Tasks.Task`1[System.Int32]]"">
          <Parameters>
            <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""0"" Name=""t"" />
          </Parameters>
          <Body>
            <CSharpAwait Type=""System.Int32"">
              <Info>
                <StaticAwaitInfo IsCompleted=""Boolean IsCompleted"" GetResult=""Int32 GetResult()"">
                  <GetAwaiter>
                    <Lambda Type=""System.Func`2[System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1[System.Int32],System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1+ConfiguredTaskAwaiter[System.Int32]]"">
                      <Parameters>
                        <Parameter Type=""System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1[System.Int32]"" Id=""1"" Name=""p"" />
                      </Parameters>
                      <Body>
                        <Call Type=""System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1+ConfiguredTaskAwaiter[System.Int32]"" Method=""ConfiguredTaskAwaiter GetAwaiter()"">
                          <Object>
                            <Parameter Type=""System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1[System.Int32]"" Id=""1"" Name=""p"" />
                          </Object>
                          <Arguments />
                        </Call>
                      </Body>
                    </Lambda>
                  </GetAwaiter>
                </StaticAwaitInfo>
              </Info>
              <Operand>
                <Call Type=""System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1[System.Int32]"" Method=""System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1[System.Int32] ConfigureAwait(Boolean)"">
                  <Object>
                    <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""0"" Name=""t"" />
                  </Object>
                  <Arguments>
                    <Constant Type=""System.Boolean"" Value=""false"" />
                  </Arguments>
                </Call>
              </Operand>
            </CSharpAwait>
          </Body>
        </CSharpAsyncLambda>
      </Expression>
      <Arguments>
        <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""0"" Name=""t"" />
      </Arguments>
    </Invoke>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_83AE_58B0();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_83AE_58B0() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_4DC5_94D3()
        {
            // (Expression<Func<dynamic, Task<dynamic>>>)(async d => await d)
            var actual = GetDebugView(@"(Expression<Func<dynamic, Task<dynamic>>>)(async d => await d)");
            var expected = @"
<Lambda Type=""System.Func`2[System.Object,System.Threading.Tasks.Task`1[System.Object]]"">
  <Parameters>
    <Parameter Type=""System.Object"" Id=""0"" Name=""d"" />
  </Parameters>
  <Body>
    <Invoke Type=""System.Threading.Tasks.Task`1[System.Object]"">
      <Expression>
        <CSharpAsyncLambda Type=""System.Func`2[System.Object,System.Threading.Tasks.Task`1[System.Object]]"">
          <Parameters>
            <Parameter Type=""System.Object"" Id=""0"" Name=""d"" />
          </Parameters>
          <Body>
            <CSharpAwait Type=""System.Object"">
              <Info>
                <DynamicAwaitInfo ResultDiscarded=""false"" Context=""Expressions"" />
              </Info>
              <Operand>
                <Parameter Type=""System.Object"" Id=""0"" Name=""d"" />
              </Operand>
            </CSharpAwait>
          </Body>
        </CSharpAsyncLambda>
      </Expression>
      <Arguments>
        <Parameter Type=""System.Object"" Id=""0"" Name=""d"" />
      </Arguments>
    </Invoke>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_4DC5_94D3();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_4DC5_94D3() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_5DFD_94D3()
        {
            // (Expression<Func<dynamic, Task<object>>>)(async d => await d)
            var actual = GetDebugView(@"(Expression<Func<dynamic, Task<object>>>)(async d => await d)");
            var expected = @"
<Lambda Type=""System.Func`2[System.Object,System.Threading.Tasks.Task`1[System.Object]]"">
  <Parameters>
    <Parameter Type=""System.Object"" Id=""0"" Name=""d"" />
  </Parameters>
  <Body>
    <Invoke Type=""System.Threading.Tasks.Task`1[System.Object]"">
      <Expression>
        <CSharpAsyncLambda Type=""System.Func`2[System.Object,System.Threading.Tasks.Task`1[System.Object]]"">
          <Parameters>
            <Parameter Type=""System.Object"" Id=""0"" Name=""d"" />
          </Parameters>
          <Body>
            <CSharpAwait Type=""System.Object"">
              <Info>
                <DynamicAwaitInfo ResultDiscarded=""false"" Context=""Expressions"" />
              </Info>
              <Operand>
                <Parameter Type=""System.Object"" Id=""0"" Name=""d"" />
              </Operand>
            </CSharpAwait>
          </Body>
        </CSharpAsyncLambda>
      </Expression>
      <Arguments>
        <Parameter Type=""System.Object"" Id=""0"" Name=""d"" />
      </Arguments>
    </Invoke>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_5DFD_94D3();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_5DFD_94D3() => INCONCLUSIVE(); }

        [Fact]
        public void CompilerTest_1A0E_037C()
        {
            // (Expression<Func<dynamic, Task<int>>>)(async d => await d)
            var actual = GetDebugView(@"(Expression<Func<dynamic, Task<int>>>)(async d => await d)");
            var expected = @"
<Lambda Type=""System.Func`2[System.Object,System.Threading.Tasks.Task`1[System.Int32]]"">
  <Parameters>
    <Parameter Type=""System.Object"" Id=""0"" Name=""d"" />
  </Parameters>
  <Body>
    <Invoke Type=""System.Threading.Tasks.Task`1[System.Int32]"">
      <Expression>
        <CSharpAsyncLambda Type=""System.Func`2[System.Object,System.Threading.Tasks.Task`1[System.Int32]]"">
          <Parameters>
            <Parameter Type=""System.Object"" Id=""0"" Name=""d"" />
          </Parameters>
          <Body>
            <CSharpDynamicConvert Type=""System.Int32"" Context=""Expressions"">
              <Expression>
                <CSharpAwait Type=""System.Object"">
                  <Info>
                    <DynamicAwaitInfo ResultDiscarded=""false"" Context=""Expressions"" />
                  </Info>
                  <Operand>
                    <Parameter Type=""System.Object"" Id=""0"" Name=""d"" />
                  </Operand>
                </CSharpAwait>
              </Expression>
            </CSharpDynamicConvert>
          </Body>
        </CSharpAsyncLambda>
      </Expression>
      <Arguments>
        <Parameter Type=""System.Object"" Id=""0"" Name=""d"" />
      </Arguments>
    </Invoke>
  </Body>
</Lambda>";
            Assert.Equal(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_1A0E_037C();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_1A0E_037C() => INCONCLUSIVE(); }

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
    partial class CompilerTests_CSharp50_AsyncAwait
    {
        partial class Reviewed
        {
            public override void CompilerTest_0FFA_9FD5() => OK();
            public override void CompilerTest_83AE_58B0() => OK();
            public override void CompilerTest_4DC5_94D3() => OK();
            public override void CompilerTest_5DFD_94D3() => OK();
            public override void CompilerTest_1A0E_037C() => OK();
        }
    }
}
*/
}
