// Prototyping extended expression trees for C#.
//
// bartde - November 2015

// NOTE: These tests are auto-generated and can *never* fail because they assert the outcome of DebugView()
//       at runtime against the outcome obtained at compile time. However, a human should read through the
//       cases to assert the outcome is as expected.
//
//       Regressions can still be caught given that the T4 won't be re-run unless it gets saved in the IDE.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xunit;

namespace Tests
{
    public class DebugViewTests
    {
        private Expression expr0 = Expression.Default(typeof(int));
        private string dbg0 = @"<Default Type=""System.Int32"" />";

        [Fact]
        public void DebugView_Test0()
        {
            Assert.Equal(dbg0, expr0.DebugView().ToString());
        }

        private Expression expr1 = Expression.Constant(1);
        private string dbg1 = @"<Constant Type=""System.Int32"" Value=""1"" />";

        [Fact]
        public void DebugView_Test1()
        {
            Assert.Equal(dbg1, expr1.DebugView().ToString());
        }

        private Expression expr2 = ((Expression<Func<int, int>>)(x => x));
        private string dbg2 = @"<Lambda Type=""System.Func`2[System.Int32,System.Int32]"">
  <Parameters>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
  </Parameters>
  <Body>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
  </Body>
</Lambda>";

        [Fact]
        public void DebugView_Test2()
        {
            Assert.Equal(dbg2, expr2.DebugView().ToString());
        }

        private Expression expr3 = ((Expression<Func<int, int>>)(x => x + 1)).Body;
        private string dbg3 = @"<Add Type=""System.Int32"">
  <Left>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
  </Left>
  <Right>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Right>
</Add>";

        [Fact]
        public void DebugView_Test3()
        {
            Assert.Equal(dbg3, expr3.DebugView().ToString());
        }

        private Expression expr4 = ((Expression<Func<TimeSpan, TimeSpan, TimeSpan>>)((a, b) => a + b)).Body;
        private string dbg4 = @"<Add Type=""System.TimeSpan"" Method=""System.TimeSpan op_Addition(System.TimeSpan, System.TimeSpan)"">
  <Left>
    <Parameter Type=""System.TimeSpan"" Id=""0"" Name=""a"" />
  </Left>
  <Right>
    <Parameter Type=""System.TimeSpan"" Id=""1"" Name=""b"" />
  </Right>
</Add>";

        [Fact]
        public void DebugView_Test4()
        {
            Assert.Equal(dbg4, expr4.DebugView().ToString());
        }

        private Expression expr5 = ((Expression<Func<int, int>>)(x => -x)).Body;
        private string dbg5 = @"<Negate Type=""System.Int32"">
  <Operand>
    <Parameter Type=""System.Int32"" Id=""0"" Name=""x"" />
  </Operand>
</Negate>";

        [Fact]
        public void DebugView_Test5()
        {
            Assert.Equal(dbg5, expr5.DebugView().ToString());
        }

        private Expression expr6 = ((Expression<Func<TimeSpan, TimeSpan>>)(x => -x)).Body;
        private string dbg6 = @"<Negate Type=""System.TimeSpan"" Method=""System.TimeSpan op_UnaryNegation(System.TimeSpan)"">
  <Operand>
    <Parameter Type=""System.TimeSpan"" Id=""0"" Name=""x"" />
  </Operand>
</Negate>";

        [Fact]
        public void DebugView_Test6()
        {
            Assert.Equal(dbg6, expr6.DebugView().ToString());
        }

        private Expression expr7 = ((Expression<Func<string, int>>)(s => s.Length)).Body;
        private string dbg7 = @"<MemberAccess Type=""System.Int32"" Member=""Int32 Length"">
  <Expression>
    <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
  </Expression>
</MemberAccess>";

        [Fact]
        public void DebugView_Test7()
        {
            Assert.Equal(dbg7, expr7.DebugView().ToString());
        }

        private Expression expr8 = ((Expression<Func<DateTime>>)(() => DateTime.Now)).Body;
        private string dbg8 = @"<MemberAccess Type=""System.DateTime"" Member=""System.DateTime Now"" />";

        [Fact]
        public void DebugView_Test8()
        {
            Assert.Equal(dbg8, expr8.DebugView().ToString());
        }

        private Expression expr9 = ((Expression<Func<string, string>>)(s => s.Substring(1))).Body;
        private string dbg9 = @"<Call Type=""System.String"" Method=""System.String Substring(Int32)"">
  <Object>
    <Parameter Type=""System.String"" Id=""0"" Name=""s"" />
  </Object>
  <Arguments>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Arguments>
</Call>";

        [Fact]
        public void DebugView_Test9()
        {
            Assert.Equal(dbg9, expr9.DebugView().ToString());
        }

        private Expression expr10 = ((Expression<Func<string, int>>)(x => int.Parse(x))).Body;
        private string dbg10 = @"<Call Type=""System.Int32"" Method=""Int32 Parse(System.String)"">
  <Arguments>
    <Parameter Type=""System.String"" Id=""0"" Name=""x"" />
  </Arguments>
</Call>";

        [Fact]
        public void DebugView_Test10()
        {
            Assert.Equal(dbg10, expr10.DebugView().ToString());
        }

        private Expression expr11 = ((Expression<Func<bool, int, int, int>>)((b, t, f) => b ? t : f)).Body;
        private string dbg11 = @"<Conditional Type=""System.Int32"">
  <Test>
    <Parameter Type=""System.Boolean"" Id=""0"" Name=""b"" />
  </Test>
  <IfTrue>
    <Parameter Type=""System.Int32"" Id=""1"" Name=""t"" />
  </IfTrue>
  <IfFalse>
    <Parameter Type=""System.Int32"" Id=""2"" Name=""f"" />
  </IfFalse>
</Conditional>";

        [Fact]
        public void DebugView_Test11()
        {
            Assert.Equal(dbg11, expr11.DebugView().ToString());
        }

        private Expression expr12 = Expression.IfThen(Expression.Constant(true), Expression.Empty());
        private string dbg12 = @"<Conditional Type=""System.Void"">
  <Test>
    <Constant Type=""System.Boolean"" Value=""true"" />
  </Test>
  <IfTrue>
    <Default Type=""System.Void"" />
  </IfTrue>
  <IfFalse>
    <Default Type=""System.Void"" />
  </IfFalse>
</Conditional>";

        [Fact]
        public void DebugView_Test12()
        {
            Assert.Equal(dbg12, expr12.DebugView().ToString());
        }

        private Expression expr13 = ((Expression<Func<List<string>, int, string>>)((ss, i) => ss[i])).Body;
        private string dbg13 = @"<Call Type=""System.String"" Method=""System.String get_Item(Int32)"">
  <Object>
    <Parameter Type=""System.Collections.Generic.List`1[System.String]"" Id=""0"" Name=""ss"" />
  </Object>
  <Arguments>
    <Parameter Type=""System.Int32"" Id=""1"" Name=""i"" />
  </Arguments>
</Call>";

        [Fact]
        public void DebugView_Test13()
        {
            Assert.Equal(dbg13, expr13.DebugView().ToString());
        }

        private Expression expr14 = Expression.MakeIndex(Expression.Default(typeof(List<string>)), typeof(List<string>).GetProperty("Item"), new[] { Expression.Constant(1) });
        private string dbg14 = @"<Index Type=""System.String"" Indexer=""System.String Item [Int32]"">
  <Object>
    <Default Type=""System.Collections.Generic.List`1[System.String]"" />
  </Object>
  <Arguments>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Arguments>
</Index>";

        [Fact]
        public void DebugView_Test14()
        {
            Assert.Equal(dbg14, expr14.DebugView().ToString());
        }

        private Expression expr15 = ((Expression<Func<TimeSpan>>)(() => new TimeSpan())).Body;
        private string dbg15 = @"<New Type=""System.TimeSpan"">
  <Arguments />
</New>";

        [Fact]
        public void DebugView_Test15()
        {
            Assert.Equal(dbg15, expr15.DebugView().ToString());
        }

        private Expression expr16 = ((Expression<Func<TimeSpan>>)(() => new TimeSpan(1L))).Body;
        private string dbg16 = @"<New Type=""System.TimeSpan"" Constructor=""Void .ctor(Int64)"">
  <Arguments>
    <Constant Type=""System.Int64"" Value=""1"" />
  </Arguments>
</New>";

        [Fact]
        public void DebugView_Test16()
        {
            Assert.Equal(dbg16, expr16.DebugView().ToString());
        }

        private Expression expr17 = ((Expression<Func<object>>)(() => new { a = 1 })).Body;
        private string dbg17 = @"<New Type=""&lt;&gt;f__AnonymousType0`1[System.Int32]"" Constructor=""Void .ctor(Int32)"">
  <Arguments>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Arguments>
  <Members>
    <Member>Int32 a</Member>
  </Members>
</New>";

        [Fact]
        public void DebugView_Test17()
        {
            Assert.Equal(dbg17, expr17.DebugView().ToString());
        }

        private Expression expr18 = ((Expression<Func<int[,]>>)(() => new int[1, 2])).Body;
        private string dbg18 = @"<NewArrayBounds Type=""System.Int32[,]"">
  <Expressions>
    <Constant Type=""System.Int32"" Value=""1"" />
    <Constant Type=""System.Int32"" Value=""2"" />
  </Expressions>
</NewArrayBounds>";

        [Fact]
        public void DebugView_Test18()
        {
            Assert.Equal(dbg18, expr18.DebugView().ToString());
        }

        private Expression expr19 = ((Expression<Func<int[]>>)(() => new int[] { 1, 2 })).Body;
        private string dbg19 = @"<NewArrayInit Type=""System.Int32[]"">
  <Expressions>
    <Constant Type=""System.Int32"" Value=""1"" />
    <Constant Type=""System.Int32"" Value=""2"" />
  </Expressions>
</NewArrayInit>";

        [Fact]
        public void DebugView_Test19()
        {
            Assert.Equal(dbg19, expr19.DebugView().ToString());
        }

        private Expression expr20 = ((Expression<Func<Func<int, int, int>, int, int, int>>)((f, x, y) => f(x, y))).Body;
        private string dbg20 = @"<Invoke Type=""System.Int32"">
  <Expression>
    <Parameter Type=""System.Func`3[System.Int32,System.Int32,System.Int32]"" Id=""0"" Name=""f"" />
  </Expression>
  <Arguments>
    <Parameter Type=""System.Int32"" Id=""1"" Name=""x"" />
    <Parameter Type=""System.Int32"" Id=""2"" Name=""y"" />
  </Arguments>
</Invoke>";

        [Fact]
        public void DebugView_Test20()
        {
            Assert.Equal(dbg20, expr20.DebugView().ToString());
        }

        private Expression expr21 = ((Expression<Func<object, bool>>)(o => o is string)).Body;
        private string dbg21 = @"<TypeIs Type=""System.Boolean"" TypeOperand=""System.String"">
  <Expression>
    <Parameter Type=""System.Object"" Id=""0"" Name=""o"" />
  </Expression>
</TypeIs>";

        [Fact]
        public void DebugView_Test21()
        {
            Assert.Equal(dbg21, expr21.DebugView().ToString());
        }

        private Expression expr22 = ((Expression<Func<StrongBox<int>>>)(() => new StrongBox<int>() { Value = 1 })).Body;
        private string dbg22 = @"<MemberInit Type=""System.Runtime.CompilerServices.StrongBox`1[System.Int32]"">
  <NewExpression>
    <New Type=""System.Runtime.CompilerServices.StrongBox`1[System.Int32]"" Constructor=""Void .ctor()"">
      <Arguments />
    </New>
  </NewExpression>
  <Bindings>
    <Assignment Member=""Int32 Value"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Expression>
    </Assignment>
  </Bindings>
</MemberInit>";

        [Fact]
        public void DebugView_Test22()
        {
            Assert.Equal(dbg22, expr22.DebugView().ToString());
        }

        private Expression expr23 = ((Expression<Func<StrongBox<StrongBox<int>>>>)(() => new StrongBox<StrongBox<int>>() { Value = { Value = 1 } })).Body;
        private string dbg23 = @"<MemberInit Type=""System.Runtime.CompilerServices.StrongBox`1[System.Runtime.CompilerServices.StrongBox`1[System.Int32]]"">
  <NewExpression>
    <New Type=""System.Runtime.CompilerServices.StrongBox`1[System.Runtime.CompilerServices.StrongBox`1[System.Int32]]"" Constructor=""Void .ctor()"">
      <Arguments />
    </New>
  </NewExpression>
  <Bindings>
    <MemberBinding Member=""System.Runtime.CompilerServices.StrongBox`1[System.Int32] Value"">
      <Bindings>
        <Assignment Member=""Int32 Value"">
          <Expression>
            <Constant Type=""System.Int32"" Value=""1"" />
          </Expression>
        </Assignment>
      </Bindings>
    </MemberBinding>
  </Bindings>
</MemberInit>";

        [Fact]
        public void DebugView_Test23()
        {
            Assert.Equal(dbg23, expr23.DebugView().ToString());
        }

        private Expression expr24 = ((Expression<Func<StrongBox<List<int>>>>)(() => new StrongBox<List<int>>() { Value = { 1 } })).Body;
        private string dbg24 = @"<MemberInit Type=""System.Runtime.CompilerServices.StrongBox`1[System.Collections.Generic.List`1[System.Int32]]"">
  <NewExpression>
    <New Type=""System.Runtime.CompilerServices.StrongBox`1[System.Collections.Generic.List`1[System.Int32]]"" Constructor=""Void .ctor()"">
      <Arguments />
    </New>
  </NewExpression>
  <Bindings>
    <ListBinding Member=""System.Collections.Generic.List`1[System.Int32] Value"">
      <Initializers>
        <ElementInit AddMethod=""Void Add(Int32)"">
          <Arguments>
            <Constant Type=""System.Int32"" Value=""1"" />
          </Arguments>
        </ElementInit>
      </Initializers>
    </ListBinding>
  </Bindings>
</MemberInit>";

        [Fact]
        public void DebugView_Test24()
        {
            Assert.Equal(dbg24, expr24.DebugView().ToString());
        }

        private Expression expr25 = ((Expression<Func<List<int>>>)(() => new List<int>() { 1 })).Body;
        private string dbg25 = @"<ListInit Type=""System.Collections.Generic.List`1[System.Int32]"">
  <NewExpression>
    <New Type=""System.Collections.Generic.List`1[System.Int32]"" Constructor=""Void .ctor()"">
      <Arguments />
    </New>
  </NewExpression>
  <Initializers>
    <ElementInit AddMethod=""Void Add(Int32)"">
      <Arguments>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Arguments>
    </ElementInit>
  </Initializers>
</ListInit>";

        [Fact]
        public void DebugView_Test25()
        {
            Assert.Equal(dbg25, expr25.DebugView().ToString());
        }

        private Expression expr26 = Expression.Block(Expression.Empty());
        private string dbg26 = @"<Block Type=""System.Void"">
  <Expressions>
    <Default Type=""System.Void"" />
  </Expressions>
</Block>";

        [Fact]
        public void DebugView_Test26()
        {
            Assert.Equal(dbg26, expr26.DebugView().ToString());
        }

        private Expression expr27 = Expression.Block(new[] { Expression.Parameter(typeof(int)) }, Expression.Empty());
        private string dbg27 = @"<Block Type=""System.Void"">
  <Variables>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </Variables>
  <Expressions>
    <Default Type=""System.Void"" />
  </Expressions>
</Block>";

        [Fact]
        public void DebugView_Test27()
        {
            Assert.Equal(dbg27, expr27.DebugView().ToString());
        }

        private Expression expr28 = Expression.Block(typeof(int), Expression.Constant(1));
        private string dbg28 = @"<Block Type=""System.Int32"">
  <Expressions>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Expressions>
</Block>";

        [Fact]
        public void DebugView_Test28()
        {
            Assert.Equal(dbg28, expr28.DebugView().ToString());
        }

        private Expression expr29 = Expression.Switch(Expression.Parameter(typeof(int)), Expression.Constant(1), new[] { Expression.SwitchCase(Expression.Constant(2), Expression.Constant(3), Expression.Constant(4)) });
        private string dbg29 = @"<Switch Type=""System.Int32"">
  <SwitchValue>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </SwitchValue>
  <Cases>
    <SwitchCase>
      <Body>
        <Constant Type=""System.Int32"" Value=""2"" />
      </Body>
      <TestValues>
        <Constant Type=""System.Int32"" Value=""3"" />
        <Constant Type=""System.Int32"" Value=""4"" />
      </TestValues>
    </SwitchCase>
  </Cases>
  <DefaultBody>
    <Constant Type=""System.Int32"" Value=""1"" />
  </DefaultBody>
</Switch>";

        [Fact]
        public void DebugView_Test29()
        {
            Assert.Equal(dbg29, expr29.DebugView().ToString());
        }

        private Expression expr30 = Expression.Switch(Expression.Parameter(typeof(int)), new[] { Expression.SwitchCase(Expression.Empty(), Expression.Constant(3), Expression.Constant(4)) });
        private string dbg30 = @"<Switch Type=""System.Void"">
  <SwitchValue>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </SwitchValue>
  <Cases>
    <SwitchCase>
      <Body>
        <Default Type=""System.Void"" />
      </Body>
      <TestValues>
        <Constant Type=""System.Int32"" Value=""3"" />
        <Constant Type=""System.Int32"" Value=""4"" />
      </TestValues>
    </SwitchCase>
  </Cases>
</Switch>";

        [Fact]
        public void DebugView_Test30()
        {
            Assert.Equal(dbg30, expr30.DebugView().ToString());
        }

        private Expression expr31 = Expression.Label(Expression.Label());
        private string dbg31 = @"<Label Type=""System.Void"">
  <Target>
    <LabelTarget Type=""System.Void"" Id=""0"" />
  </Target>
</Label>";

        [Fact]
        public void DebugView_Test31()
        {
            Assert.Equal(dbg31, expr31.DebugView().ToString());
        }

        private Expression expr32 = Expression.Label(Expression.Label(typeof(int)), Expression.Default(typeof(int)));
        private string dbg32 = @"<Label Type=""System.Int32"">
  <Target>
    <LabelTarget Type=""System.Int32"" Id=""0"" />
  </Target>
  <DefaultValue>
    <Default Type=""System.Int32"" />
  </DefaultValue>
</Label>";

        [Fact]
        public void DebugView_Test32()
        {
            Assert.Equal(dbg32, expr32.DebugView().ToString());
        }

        private Expression expr33 = Expression.Label(Expression.Label(typeof(int), "foo"), Expression.Default(typeof(int)));
        private string dbg33 = @"<Label Type=""System.Int32"">
  <Target>
    <LabelTarget Type=""System.Int32"" Id=""0"" Name=""foo"" />
  </Target>
  <DefaultValue>
    <Default Type=""System.Int32"" />
  </DefaultValue>
</Label>";

        [Fact]
        public void DebugView_Test33()
        {
            Assert.Equal(dbg33, expr33.DebugView().ToString());
        }

        private Expression expr34 = Expression.Break(Expression.Label());
        private string dbg34 = @"<Goto Type=""System.Void"" Kind=""Break"">
  <Target>
    <LabelTarget Type=""System.Void"" Id=""0"" />
  </Target>
</Goto>";

        [Fact]
        public void DebugView_Test34()
        {
            Assert.Equal(dbg34, expr34.DebugView().ToString());
        }

        private Expression expr35 = Expression.Break(Expression.Label(typeof(int)), Expression.Constant(1));
        private string dbg35 = @"<Goto Type=""System.Void"" Kind=""Break"">
  <Target>
    <LabelTarget Type=""System.Int32"" Id=""0"" />
  </Target>
  <Value>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Value>
</Goto>";

        [Fact]
        public void DebugView_Test35()
        {
            Assert.Equal(dbg35, expr35.DebugView().ToString());
        }

        private Expression expr36 = Expression.Continue(Expression.Label());
        private string dbg36 = @"<Goto Type=""System.Void"" Kind=""Continue"">
  <Target>
    <LabelTarget Type=""System.Void"" Id=""0"" />
  </Target>
</Goto>";

        [Fact]
        public void DebugView_Test36()
        {
            Assert.Equal(dbg36, expr36.DebugView().ToString());
        }

        private Expression expr37 = Expression.Goto(Expression.Label());
        private string dbg37 = @"<Goto Type=""System.Void"" Kind=""Goto"">
  <Target>
    <LabelTarget Type=""System.Void"" Id=""0"" />
  </Target>
</Goto>";

        [Fact]
        public void DebugView_Test37()
        {
            Assert.Equal(dbg37, expr37.DebugView().ToString());
        }

        private Expression expr38 = Expression.Goto(Expression.Label(), Expression.Constant(1));
        private string dbg38 = @"<Goto Type=""System.Void"" Kind=""Goto"">
  <Target>
    <LabelTarget Type=""System.Void"" Id=""0"" />
  </Target>
  <Value>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Value>
</Goto>";

        [Fact]
        public void DebugView_Test38()
        {
            Assert.Equal(dbg38, expr38.DebugView().ToString());
        }

        private Expression expr39 = Expression.TryCatch(Expression.Constant(1), Expression.Catch(Expression.Parameter(typeof(Exception)), Expression.Constant(2)));
        private string dbg39 = @"<Try Type=""System.Int32"">
  <Body>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Body>
  <Handlers>
    <CatchBlock>
      <Variable>
        <Parameter Type=""System.Exception"" Id=""0"" />
      </Variable>
      <Body>
        <Constant Type=""System.Int32"" Value=""2"" />
      </Body>
    </CatchBlock>
  </Handlers>
</Try>";

        [Fact]
        public void DebugView_Test39()
        {
            Assert.Equal(dbg39, expr39.DebugView().ToString());
        }

        private Expression expr40 = Expression.TryCatch(Expression.Constant(1), Expression.Catch(typeof(Exception), Expression.Constant(2)));
        private string dbg40 = @"<Try Type=""System.Int32"">
  <Body>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Body>
  <Handlers>
    <CatchBlock Test=""System.Exception"">
      <Body>
        <Constant Type=""System.Int32"" Value=""2"" />
      </Body>
    </CatchBlock>
  </Handlers>
</Try>";

        [Fact]
        public void DebugView_Test40()
        {
            Assert.Equal(dbg40, expr40.DebugView().ToString());
        }

        private Expression expr41 = Expression.TryCatch(Expression.Constant(1), Expression.Catch(Expression.Parameter(typeof(Exception)), Expression.Constant(2), Expression.Constant(true)));
        private string dbg41 = @"<Try Type=""System.Int32"">
  <Body>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Body>
  <Handlers>
    <CatchBlock>
      <Variable>
        <Parameter Type=""System.Exception"" Id=""0"" />
      </Variable>
      <Body>
        <Constant Type=""System.Int32"" Value=""2"" />
      </Body>
      <Filter>
        <Constant Type=""System.Boolean"" Value=""true"" />
      </Filter>
    </CatchBlock>
  </Handlers>
</Try>";

        [Fact]
        public void DebugView_Test41()
        {
            Assert.Equal(dbg41, expr41.DebugView().ToString());
        }

        private Expression expr42 = Expression.TryCatch(Expression.Constant(1), Expression.Catch(typeof(Exception), Expression.Constant(2), Expression.Constant(true)));
        private string dbg42 = @"<Try Type=""System.Int32"">
  <Body>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Body>
  <Handlers>
    <CatchBlock Test=""System.Exception"">
      <Body>
        <Constant Type=""System.Int32"" Value=""2"" />
      </Body>
      <Filter>
        <Constant Type=""System.Boolean"" Value=""true"" />
      </Filter>
    </CatchBlock>
  </Handlers>
</Try>";

        [Fact]
        public void DebugView_Test42()
        {
            Assert.Equal(dbg42, expr42.DebugView().ToString());
        }

        private Expression expr43 = Expression.TryCatchFinally(Expression.Constant(1), Expression.Empty(), Expression.Catch(Expression.Parameter(typeof(Exception)), Expression.Constant(2)));
        private string dbg43 = @"<Try Type=""System.Int32"">
  <Body>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Body>
  <Handlers>
    <CatchBlock>
      <Variable>
        <Parameter Type=""System.Exception"" Id=""0"" />
      </Variable>
      <Body>
        <Constant Type=""System.Int32"" Value=""2"" />
      </Body>
    </CatchBlock>
  </Handlers>
  <Finally>
    <Default Type=""System.Void"" />
  </Finally>
</Try>";

        [Fact]
        public void DebugView_Test43()
        {
            Assert.Equal(dbg43, expr43.DebugView().ToString());
        }

        private Expression expr44 = Expression.TryFinally(Expression.Constant(1), Expression.Empty());
        private string dbg44 = @"<Try Type=""System.Int32"">
  <Body>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Body>
  <Finally>
    <Default Type=""System.Void"" />
  </Finally>
</Try>";

        [Fact]
        public void DebugView_Test44()
        {
            Assert.Equal(dbg44, expr44.DebugView().ToString());
        }

        private Expression expr45 = Expression.TryFault(Expression.Constant(1), Expression.Empty());
        private string dbg45 = @"<Try Type=""System.Int32"">
  <Body>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Body>
  <Fault>
    <Default Type=""System.Void"" />
  </Fault>
</Try>";

        [Fact]
        public void DebugView_Test45()
        {
            Assert.Equal(dbg45, expr45.DebugView().ToString());
        }

        private Expression expr46 = Expression.Loop(Expression.Empty());
        private string dbg46 = @"<Loop Type=""System.Void"">
  <Body>
    <Default Type=""System.Void"" />
  </Body>
</Loop>";

        [Fact]
        public void DebugView_Test46()
        {
            Assert.Equal(dbg46, expr46.DebugView().ToString());
        }

        private Expression expr47 = Expression.Loop(Expression.Empty(), Expression.Label(typeof(void), "break"));
        private string dbg47 = @"<Loop Type=""System.Void"">
  <Body>
    <Default Type=""System.Void"" />
  </Body>
  <BreakLabel>
    <LabelTarget Type=""System.Void"" Id=""0"" Name=""break"" />
  </BreakLabel>
</Loop>";

        [Fact]
        public void DebugView_Test47()
        {
            Assert.Equal(dbg47, expr47.DebugView().ToString());
        }

        private Expression expr48 = Expression.Loop(Expression.Empty(), Expression.Label(typeof(void), "break"), Expression.Label(typeof(void), "continue"));
        private string dbg48 = @"<Loop Type=""System.Void"">
  <Body>
    <Default Type=""System.Void"" />
  </Body>
  <BreakLabel>
    <LabelTarget Type=""System.Void"" Id=""0"" Name=""break"" />
  </BreakLabel>
  <ContinueLabel>
    <LabelTarget Type=""System.Void"" Id=""1"" Name=""continue"" />
  </ContinueLabel>
</Loop>";

        [Fact]
        public void DebugView_Test48()
        {
            Assert.Equal(dbg48, expr48.DebugView().ToString());
        }

        private Expression expr49 = Expression.RuntimeVariables(Expression.Parameter(typeof(int)));
        private string dbg49 = @"<RuntimeVariables Type=""System.Runtime.CompilerServices.IRuntimeVariables"">
  <Variables>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </Variables>
</RuntimeVariables>";

        [Fact]
        public void DebugView_Test49()
        {
            Assert.Equal(dbg49, expr49.DebugView().ToString());
        }

        private Expression expr50 = Expression.ClearDebugInfo(Expression.SymbolDocument("foo"));
        private string dbg50 = @"<DebugInfo Type=""System.Void"" FileName=""foo"" IsClear=""true"" />";

        [Fact]
        public void DebugView_Test50()
        {
            Assert.Equal(dbg50, expr50.DebugView().ToString());
        }

        private Expression expr51 = Expression.DebugInfo(Expression.SymbolDocument("foo"), 1, 2, 3, 4);
        private string dbg51 = @"<DebugInfo Type=""System.Void"" FileName=""foo"" StartLine=""1"" StartColumn=""2"" EndLine=""3"" EndColumn=""4"" />";

        [Fact]
        public void DebugView_Test51()
        {
            Assert.Equal(dbg51, expr51.DebugView().ToString());
        }

    }
}