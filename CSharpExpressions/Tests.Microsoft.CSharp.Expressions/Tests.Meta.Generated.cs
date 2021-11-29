﻿// Prototyping extended expression trees for C#.
//
// bartde - November 2015

// NOTE: These tests are auto-generated and can *never* fail because they assert the outcome of DebugView()
//       at runtime against the outcome obtained at compile time. However, a human should read through the
//       cases to assert the outcome is as expected.
//
//       Regressions can still be caught given that the T4 won't be re-run unless it gets saved in the IDE.

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class DebugViewTests
    {
        private Expression expr0 = CSharpExpression.PostIncrementAssign(Expression.Parameter(typeof(int)));
        private string dbg0 = @"<CSharpPostIncrementAssign Type=""System.Int32"">
  <Operand>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </Operand>
</CSharpPostIncrementAssign>";

        [TestMethod]
        public void CSharp_DebugView_Test0()
        {
            Assert.AreEqual(dbg0, expr0.DebugView().ToString());
        }

        private Expression expr1 = CSharpExpression.PreDecrementAssignChecked(Expression.Parameter(typeof(int)));
        private string dbg1 = @"<CSharpPreDecrementAssignChecked Type=""System.Int32"">
  <Operand>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </Operand>
</CSharpPreDecrementAssignChecked>";

        [TestMethod]
        public void CSharp_DebugView_Test1()
        {
            Assert.AreEqual(dbg1, expr1.DebugView().ToString());
        }

        private Expression expr2 = CSharpExpression.AddAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg2 = @"<CSharpAddAssign Type=""System.Int32"">
  <Left>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </Left>
  <Right>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Right>
</CSharpAddAssign>";

        [TestMethod]
        public void CSharp_DebugView_Test2()
        {
            Assert.AreEqual(dbg2, expr2.DebugView().ToString());
        }

        private Expression expr3 = CSharpExpression.NullCoalescingAssign(Expression.Parameter(typeof(int?)), Expression.Constant(1));
        private string dbg3 = @"<CSharpNullCoalescingAssign Type=""System.Int32"">
  <Left>
    <Parameter Type=""System.Nullable`1[System.Int32]"" Id=""0"" />
  </Left>
  <Right>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Right>
</CSharpNullCoalescingAssign>";

        [TestMethod]
        public void CSharp_DebugView_Test3()
        {
            Assert.AreEqual(dbg3, expr3.DebugView().ToString());
        }

        private Expression expr4 = CSharpExpression.NullCoalescingAssign(Expression.Parameter(typeof(string)), Expression.Constant("foo"));
        private string dbg4 = @"<CSharpNullCoalescingAssign Type=""System.String"">
  <Left>
    <Parameter Type=""System.String"" Id=""0"" />
  </Left>
  <Right>
    <Constant Type=""System.String"" Value=""foo"" />
  </Right>
</CSharpNullCoalescingAssign>";

        [TestMethod]
        public void CSharp_DebugView_Test4()
        {
            Assert.AreEqual(dbg4, expr4.DebugView().ToString());
        }

        private Expression expr5 = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 1, 1 }, Expression.Constant(42));
        private string dbg5 = @"<CSharpNewMultidimensionalArrayInit Type=""System.Int32[,]"" Bounds=""1, 1"">
  <Expressions>
    <Constant Type=""System.Int32"" Value=""42"" />
  </Expressions>
</CSharpNewMultidimensionalArrayInit>";

        [TestMethod]
        public void CSharp_DebugView_Test5()
        {
            Assert.AreEqual(dbg5, expr5.DebugView().ToString());
        }

        private Expression expr6 = CSharpExpression.Await(Expression.Default(typeof(Task<int>)));
        private string dbg6 = @"<CSharpAwait Type=""System.Int32"">
  <Info>
    <StaticAwaitInfo IsCompleted=""Boolean IsCompleted"" GetResult=""Int32 GetResult()"">
      <GetAwaiter>
        <Lambda Type=""System.Func`2[System.Threading.Tasks.Task`1[System.Int32],System.Runtime.CompilerServices.TaskAwaiter`1[System.Int32]]"">
          <Parameters>
            <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""0"" />
          </Parameters>
          <Body>
            <Call Type=""System.Runtime.CompilerServices.TaskAwaiter`1[System.Int32]"" Method=""System.Runtime.CompilerServices.TaskAwaiter`1[System.Int32] GetAwaiter()"">
              <Object>
                <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""0"" />
              </Object>
              <Arguments />
            </Call>
          </Body>
        </Lambda>
      </GetAwaiter>
    </StaticAwaitInfo>
  </Info>
  <Operand>
    <Default Type=""System.Threading.Tasks.Task`1[System.Int32]"" />
  </Operand>
</CSharpAwait>";

        [TestMethod]
        public void CSharp_DebugView_Test6()
        {
            Assert.AreEqual(dbg6, expr6.DebugView().ToString());
        }

        private Expression expr7 = DynamicCSharpExpression.DynamicAwait(Expression.Default(typeof(Task<int>)), false, typeof(object));
        private string dbg7 = @"<CSharpAwait Type=""System.Object"">
  <Info>
    <DynamicAwaitInfo ResultDiscarded=""false"" Context=""System.Object"" />
  </Info>
  <Operand>
    <Default Type=""System.Threading.Tasks.Task`1[System.Int32]"" />
  </Operand>
</CSharpAwait>";

        [TestMethod]
        public void CSharp_DebugView_Test7()
        {
            Assert.AreEqual(dbg7, expr7.DebugView().ToString());
        }

        private Expression expr8 = CSharpExpression.AsyncLambda<Func<Task<int>>>(CSharpExpression.Await(Expression.Default(typeof(Task<int>))));
        private string dbg8 = @"<CSharpAsyncLambda Type=""System.Func`1[System.Threading.Tasks.Task`1[System.Int32]]"">
  <Parameters />
  <Body>
    <CSharpAwait Type=""System.Int32"">
      <Info>
        <StaticAwaitInfo IsCompleted=""Boolean IsCompleted"" GetResult=""Int32 GetResult()"">
          <GetAwaiter>
            <Lambda Type=""System.Func`2[System.Threading.Tasks.Task`1[System.Int32],System.Runtime.CompilerServices.TaskAwaiter`1[System.Int32]]"">
              <Parameters>
                <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""0"" />
              </Parameters>
              <Body>
                <Call Type=""System.Runtime.CompilerServices.TaskAwaiter`1[System.Int32]"" Method=""System.Runtime.CompilerServices.TaskAwaiter`1[System.Int32] GetAwaiter()"">
                  <Object>
                    <Parameter Type=""System.Threading.Tasks.Task`1[System.Int32]"" Id=""0"" />
                  </Object>
                  <Arguments />
                </Call>
              </Body>
            </Lambda>
          </GetAwaiter>
        </StaticAwaitInfo>
      </Info>
      <Operand>
        <Default Type=""System.Threading.Tasks.Task`1[System.Int32]"" />
      </Operand>
    </CSharpAwait>
  </Body>
</CSharpAsyncLambda>";

        [TestMethod]
        public void CSharp_DebugView_Test8()
        {
            Assert.AreEqual(dbg8, expr8.DebugView().ToString());
        }

        private Expression expr9 = CSharpExpression.Call(typeof(Math).GetMethod("Abs", new[] { typeof(int) }), CSharpExpression.Bind(typeof(Math).GetMethod("Abs", new[] { typeof(int) }).GetParameters()[0], Expression.Constant(42)));
        private string dbg9 = @"<CSharpCall Type=""System.Int32"" Method=""Int32 Abs(Int32)"">
  <Arguments>
    <ParameterAssignment Parameter=""Int32 value"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""42"" />
      </Expression>
    </ParameterAssignment>
  </Arguments>
</CSharpCall>";

        [TestMethod]
        public void CSharp_DebugView_Test9()
        {
            Assert.AreEqual(dbg9, expr9.DebugView().ToString());
        }

        private Expression expr10 = CSharpExpression.Call(Expression.Default(typeof(string)), typeof(string).GetMethod("Substring", new[] { typeof(int) }), CSharpExpression.Bind(typeof(string).GetMethod("Substring", new[] { typeof(int) }).GetParameters()[0], Expression.Constant(42)));
        private string dbg10 = @"<CSharpCall Type=""System.String"" Method=""System.String Substring(Int32)"">
  <Object>
    <Default Type=""System.String"" />
  </Object>
  <Arguments>
    <ParameterAssignment Parameter=""Int32 startIndex"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""42"" />
      </Expression>
    </ParameterAssignment>
  </Arguments>
</CSharpCall>";

        [TestMethod]
        public void CSharp_DebugView_Test10()
        {
            Assert.AreEqual(dbg10, expr10.DebugView().ToString());
        }

        private Expression expr11 = CSharpExpression.Invoke(Expression.Default(typeof(Action<int>)), CSharpExpression.Bind(typeof(Action<int>).GetMethod("Invoke").GetParameters()[0], Expression.Constant(42)));
        private string dbg11 = @"<CSharpInvoke Type=""System.Void"">
  <Expression>
    <Default Type=""System.Action`1[System.Int32]"" />
  </Expression>
  <Arguments>
    <ParameterAssignment Parameter=""Int32 obj"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""42"" />
      </Expression>
    </ParameterAssignment>
  </Arguments>
</CSharpInvoke>";

        [TestMethod]
        public void CSharp_DebugView_Test11()
        {
            Assert.AreEqual(dbg11, expr11.DebugView().ToString());
        }

        private Expression expr12 = CSharpExpression.New(typeof(TimeSpan).GetConstructor(new[] { typeof(long) }), CSharpExpression.Bind(typeof(TimeSpan).GetConstructor(new[] { typeof(long) }).GetParameters()[0], Expression.Constant(42L)));
        private string dbg12 = @"<CSharpNew Type=""System.TimeSpan"" Constructor=""Void .ctor(Int64)"">
  <Arguments>
    <ParameterAssignment Parameter=""Int64 ticks"">
      <Expression>
        <Constant Type=""System.Int64"" Value=""42"" />
      </Expression>
    </ParameterAssignment>
  </Arguments>
</CSharpNew>";

        [TestMethod]
        public void CSharp_DebugView_Test12()
        {
            Assert.AreEqual(dbg12, expr12.DebugView().ToString());
        }

        private Expression expr13 = CSharpExpression.Index(Expression.Default(typeof(List<int>)), typeof(List<int>).GetProperty("Item"), CSharpExpression.Bind(typeof(List<int>).GetProperty("Item").GetIndexParameters()[0], Expression.Constant(42)));
        private string dbg13 = @"<CSharpIndex Type=""System.Int32"" Indexer=""Int32 Item [Int32]"">
  <Object>
    <Default Type=""System.Collections.Generic.List`1[System.Int32]"" />
  </Object>
  <Arguments>
    <ParameterAssignment Parameter=""Int32 index"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""42"" />
      </Expression>
    </ParameterAssignment>
  </Arguments>
</CSharpIndex>";

        [TestMethod]
        public void CSharp_DebugView_Test13()
        {
            Assert.AreEqual(dbg13, expr13.DebugView().ToString());
        }

        private Expression expr14 = CSharpExpression.ConditionalArrayIndex(Expression.Default(typeof(string[])), Expression.Constant(42));
        private string dbg14 = @"<CSharpConditionalArrayIndex Type=""System.String"">
  <Array>
    <Default Type=""System.String[]"" />
  </Array>
  <Indexes>
    <Constant Type=""System.Int32"" Value=""42"" />
  </Indexes>
</CSharpConditionalArrayIndex>";

        [TestMethod]
        public void CSharp_DebugView_Test14()
        {
            Assert.AreEqual(dbg14, expr14.DebugView().ToString());
        }

        private Expression expr15 = CSharpExpression.ConditionalCall(Expression.Default(typeof(string)), typeof(string).GetMethod("Substring", new[] { typeof(int) }), CSharpExpression.Bind(typeof(string).GetMethod("Substring", new[] { typeof(int) }).GetParameters()[0], Expression.Constant(42)));
        private string dbg15 = @"<CSharpConditionalCall Type=""System.String"" Method=""System.String Substring(Int32)"">
  <Object>
    <Default Type=""System.String"" />
  </Object>
  <Arguments>
    <ParameterAssignment Parameter=""Int32 startIndex"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""42"" />
      </Expression>
    </ParameterAssignment>
  </Arguments>
</CSharpConditionalCall>";

        [TestMethod]
        public void CSharp_DebugView_Test15()
        {
            Assert.AreEqual(dbg15, expr15.DebugView().ToString());
        }

        private Expression expr16 = CSharpExpression.ConditionalInvoke(Expression.Default(typeof(Action<int>)), CSharpExpression.Bind(typeof(Action<int>).GetMethod("Invoke").GetParameters()[0], Expression.Constant(42)));
        private string dbg16 = @"<CSharpConditionalInvoke Type=""System.Void"">
  <Expression>
    <Default Type=""System.Action`1[System.Int32]"" />
  </Expression>
  <Arguments>
    <ParameterAssignment Parameter=""Int32 obj"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""42"" />
      </Expression>
    </ParameterAssignment>
  </Arguments>
</CSharpConditionalInvoke>";

        [TestMethod]
        public void CSharp_DebugView_Test16()
        {
            Assert.AreEqual(dbg16, expr16.DebugView().ToString());
        }

        private Expression expr17 = CSharpExpression.ConditionalIndex(Expression.Default(typeof(List<int>)), typeof(List<int>).GetProperty("Item"), CSharpExpression.Bind(typeof(List<int>).GetProperty("Item").GetIndexParameters()[0], Expression.Constant(42)));
        private string dbg17 = @"<CSharpConditionalIndex Type=""System.Nullable`1[System.Int32]"" Indexer=""Int32 Item [Int32]"">
  <Object>
    <Default Type=""System.Collections.Generic.List`1[System.Int32]"" />
  </Object>
  <Arguments>
    <ParameterAssignment Parameter=""Int32 index"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""42"" />
      </Expression>
    </ParameterAssignment>
  </Arguments>
</CSharpConditionalIndex>";

        [TestMethod]
        public void CSharp_DebugView_Test17()
        {
            Assert.AreEqual(dbg17, expr17.DebugView().ToString());
        }

        private Expression expr18 = CSharpExpression.ConditionalProperty(Expression.Default(typeof(string)), typeof(string).GetProperty("Length"));
        private string dbg18 = @"<CSharpConditionalMemberAccess Type=""System.Nullable`1[System.Int32]"" Member=""Int32 Length"">
  <Expression>
    <Default Type=""System.String"" />
  </Expression>
</CSharpConditionalMemberAccess>";

        [TestMethod]
        public void CSharp_DebugView_Test18()
        {
            Assert.AreEqual(dbg18, expr18.DebugView().ToString());
        }

        private Expression expr19 = CSharpExpression.ConditionalAccess(Expression.Default(typeof(string)), CSharpExpression.ConditionalReceiver(typeof(string)), Expression.Property(CSharpExpression.ConditionalReceiver(typeof(string)), "Length"));
        private string dbg19 = @"<CSharpConditionalAccess Type=""System.Nullable`1[System.Int32]"">
  <Receiver>
    <Default Type=""System.String"" />
  </Receiver>
  <NonNullReceiver>
    <ConditionalReceiver Id=""0"" Type=""System.String"" />
  </NonNullReceiver>
  <WhenNotNull>
    <MemberAccess Type=""System.Int32"" Member=""Int32 Length"">
      <Expression>
        <ConditionalReceiver Id=""1"" Type=""System.String"" />
      </Expression>
    </MemberAccess>
  </WhenNotNull>
</CSharpConditionalAccess>";

        [TestMethod]
        public void CSharp_DebugView_Test19()
        {
            Assert.AreEqual(dbg19, expr19.DebugView().ToString());
        }

        private Expression expr20 = DynamicCSharpExpression.DynamicAdd(Expression.Constant(1), Expression.Constant(2));
        private string dbg20 = @"<CSharpDynamicBinary Type=""System.Object"" OperationNodeType=""Add"">
  <Left>
    <DynamicCSharpArgument Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Expression>
    </DynamicCSharpArgument>
  </Left>
  <Right>
    <DynamicCSharpArgument Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""2"" />
      </Expression>
    </DynamicCSharpArgument>
  </Right>
</CSharpDynamicBinary>";

        [TestMethod]
        public void CSharp_DebugView_Test20()
        {
            Assert.AreEqual(dbg20, expr20.DebugView().ToString());
        }

        private Expression expr21 = DynamicCSharpExpression.DynamicAdd(DynamicCSharpExpression.DynamicArgument(Expression.Constant(1)), DynamicCSharpExpression.DynamicArgument(Expression.Constant(2)), CSharpBinderFlags.CheckedContext);
        private string dbg21 = @"<CSharpDynamicBinary Type=""System.Object"" OperationNodeType=""Add"" Flags=""CheckedContext"">
  <Left>
    <DynamicCSharpArgument Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Expression>
    </DynamicCSharpArgument>
  </Left>
  <Right>
    <DynamicCSharpArgument Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""2"" />
      </Expression>
    </DynamicCSharpArgument>
  </Right>
</CSharpDynamicBinary>";

        [TestMethod]
        public void CSharp_DebugView_Test21()
        {
            Assert.AreEqual(dbg21, expr21.DebugView().ToString());
        }

        private Expression expr22 = DynamicCSharpExpression.DynamicAdd(DynamicCSharpExpression.DynamicArgument(Expression.Constant(1)), DynamicCSharpExpression.DynamicArgument(Expression.Constant(2)), CSharpBinderFlags.CheckedContext, typeof(object));
        private string dbg22 = @"<CSharpDynamicBinary Type=""System.Object"" OperationNodeType=""Add"" Flags=""CheckedContext"" Context=""System.Object"">
  <Left>
    <DynamicCSharpArgument Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Expression>
    </DynamicCSharpArgument>
  </Left>
  <Right>
    <DynamicCSharpArgument Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""2"" />
      </Expression>
    </DynamicCSharpArgument>
  </Right>
</CSharpDynamicBinary>";

        [TestMethod]
        public void CSharp_DebugView_Test22()
        {
            Assert.AreEqual(dbg22, expr22.DebugView().ToString());
        }

        private Expression expr23 = DynamicCSharpExpression.DynamicNegate(Expression.Constant(1));
        private string dbg23 = @"<CSharpDynamicUnary Type=""System.Object"" OperationNodeType=""Negate"">
  <Operand>
    <DynamicCSharpArgument Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Expression>
    </DynamicCSharpArgument>
  </Operand>
</CSharpDynamicUnary>";

        [TestMethod]
        public void CSharp_DebugView_Test23()
        {
            Assert.AreEqual(dbg23, expr23.DebugView().ToString());
        }

        private Expression expr24 = DynamicCSharpExpression.DynamicConvert(Expression.Constant(1), typeof(int));
        private string dbg24 = @"<CSharpDynamicConvert Type=""System.Int32"">
  <Expression>
    <Constant Type=""System.Int32"" Value=""1"" />
  </Expression>
</CSharpDynamicConvert>";

        [TestMethod]
        public void CSharp_DebugView_Test24()
        {
            Assert.AreEqual(dbg24, expr24.DebugView().ToString());
        }

        private Expression expr25 = DynamicCSharpExpression.DynamicGetMember(Expression.Default(typeof(string)), "Length");
        private string dbg25 = @"<CSharpDynamicGetMember Type=""System.Object"" Name=""Length"">
  <Object>
    <Default Type=""System.String"" />
  </Object>
</CSharpDynamicGetMember>";

        [TestMethod]
        public void CSharp_DebugView_Test25()
        {
            Assert.AreEqual(dbg25, expr25.DebugView().ToString());
        }

        private Expression expr26 = DynamicCSharpExpression.DynamicGetIndex(Expression.Default(typeof(List<int>)), Expression.Constant(1));
        private string dbg26 = @"<CSharpDynamicGetIndex Type=""System.Object"">
  <Object>
    <Default Type=""System.Collections.Generic.List`1[System.Int32]"" />
  </Object>
  <Arguments>
    <DynamicCSharpArgument Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Expression>
    </DynamicCSharpArgument>
  </Arguments>
</CSharpDynamicGetIndex>";

        [TestMethod]
        public void CSharp_DebugView_Test26()
        {
            Assert.AreEqual(dbg26, expr26.DebugView().ToString());
        }

        private Expression expr27 = DynamicCSharpExpression.DynamicInvoke(Expression.Default(typeof(Action<int>)), Expression.Constant(1));
        private string dbg27 = @"<CSharpDynamicInvoke Type=""System.Object"">
  <Expression>
    <Default Type=""System.Action`1[System.Int32]"" />
  </Expression>
  <Arguments>
    <DynamicCSharpArgument Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Expression>
    </DynamicCSharpArgument>
  </Arguments>
</CSharpDynamicInvoke>";

        [TestMethod]
        public void CSharp_DebugView_Test27()
        {
            Assert.AreEqual(dbg27, expr27.DebugView().ToString());
        }

        private Expression expr28 = DynamicCSharpExpression.DynamicInvokeMember(typeof(Math), "Abs", Expression.Constant(1));
        private string dbg28 = @"<CSharpDynamicInvokeMember Type=""System.Object"" Target=""System.Math"" Name=""Abs"">
  <Arguments>
    <DynamicCSharpArgument Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Expression>
    </DynamicCSharpArgument>
  </Arguments>
</CSharpDynamicInvokeMember>";

        [TestMethod]
        public void CSharp_DebugView_Test28()
        {
            Assert.AreEqual(dbg28, expr28.DebugView().ToString());
        }

        private Expression expr29 = DynamicCSharpExpression.DynamicInvokeMember(typeof(Activator), "CreateInstance", new[] { typeof(int) });
        private string dbg29 = @"<CSharpDynamicInvokeMember Type=""System.Object"" Target=""System.Activator"" Name=""CreateInstance"" TypeArguments=""System.Int32"">
  <Arguments />
</CSharpDynamicInvokeMember>";

        [TestMethod]
        public void CSharp_DebugView_Test29()
        {
            Assert.AreEqual(dbg29, expr29.DebugView().ToString());
        }

        private Expression expr30 = DynamicCSharpExpression.DynamicInvokeMember(Expression.Default(typeof(string)), "Substring", Expression.Constant(1));
        private string dbg30 = @"<CSharpDynamicInvokeMember Type=""System.Object"" Name=""Substring"">
  <Object>
    <Default Type=""System.String"" />
  </Object>
  <Arguments>
    <DynamicCSharpArgument Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Expression>
    </DynamicCSharpArgument>
  </Arguments>
</CSharpDynamicInvokeMember>";

        [TestMethod]
        public void CSharp_DebugView_Test30()
        {
            Assert.AreEqual(dbg30, expr30.DebugView().ToString());
        }

        private Expression expr31 = DynamicCSharpExpression.DynamicInvokeMember(Expression.Default(typeof(string)), "Substring", new[] { DynamicCSharpExpression.DynamicArgument(Expression.Constant(1), "startIndex") });
        private string dbg31 = @"<CSharpDynamicInvokeMember Type=""System.Object"" Name=""Substring"">
  <Object>
    <Default Type=""System.String"" />
  </Object>
  <Arguments>
    <DynamicCSharpArgument Name=""startIndex"" Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Expression>
    </DynamicCSharpArgument>
  </Arguments>
</CSharpDynamicInvokeMember>";

        [TestMethod]
        public void CSharp_DebugView_Test31()
        {
            Assert.AreEqual(dbg31, expr31.DebugView().ToString());
        }

        private Expression expr32 = DynamicCSharpExpression.DynamicInvokeMember(Expression.Default(typeof(string)), "Substring", new[] { DynamicCSharpExpression.DynamicArgument(Expression.Constant(1), "startIndex", CSharpArgumentInfoFlags.NamedArgument) });
        private string dbg32 = @"<CSharpDynamicInvokeMember Type=""System.Object"" Name=""Substring"">
  <Object>
    <Default Type=""System.String"" />
  </Object>
  <Arguments>
    <DynamicCSharpArgument Name=""startIndex"" Flags=""NamedArgument"">
      <Expression>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Expression>
    </DynamicCSharpArgument>
  </Arguments>
</CSharpDynamicInvokeMember>";

        [TestMethod]
        public void CSharp_DebugView_Test32()
        {
            Assert.AreEqual(dbg32, expr32.DebugView().ToString());
        }

        private Expression expr33 = DynamicCSharpExpression.DynamicInvokeConstructor(typeof(TimeSpan), Expression.Constant(1L));
        private string dbg33 = @"<CSharpDynamicInvokeConstructor Type=""System.TimeSpan"">
  <Arguments>
    <DynamicCSharpArgument Flags=""UseCompileTimeType, Constant"">
      <Expression>
        <Constant Type=""System.Int64"" Value=""1"" />
      </Expression>
    </DynamicCSharpArgument>
  </Arguments>
</CSharpDynamicInvokeConstructor>";

        [TestMethod]
        public void CSharp_DebugView_Test33()
        {
            Assert.AreEqual(dbg33, expr33.DebugView().ToString());
        }

        private Expression expr34 = CSharpExpression.Block(new Expression[] { Expression.Empty() }, Expression.Label());
        private string dbg34 = @"<CSharpBlock Type=""System.Void"">
  <Statements>
    <Default Type=""System.Void"" />
  </Statements>
  <ReturnLabel>
    <LabelTarget Type=""System.Void"" Id=""0"" />
  </ReturnLabel>
</CSharpBlock>";

        [TestMethod]
        public void CSharp_DebugView_Test34()
        {
            Assert.AreEqual(dbg34, expr34.DebugView().ToString());
        }

        private Expression expr35 = CSharpExpression.Block(new[] { Expression.Parameter(typeof(int)) }, new Expression[] { Expression.Empty() }, Expression.Label());
        private string dbg35 = @"<CSharpBlock Type=""System.Void"">
  <Variables>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </Variables>
  <Statements>
    <Default Type=""System.Void"" />
  </Statements>
  <ReturnLabel>
    <LabelTarget Type=""System.Void"" Id=""1"" />
  </ReturnLabel>
</CSharpBlock>";

        [TestMethod]
        public void CSharp_DebugView_Test35()
        {
            Assert.AreEqual(dbg35, expr35.DebugView().ToString());
        }

        private Expression expr36 = Expression.Block(CSharpExpression.Block(new Expression[] { Expression.Empty() }, Expression.Label()));
        private string dbg36 = @"<Block Type=""System.Void"">
  <Expressions>
    <CSharpBlock Type=""System.Void"">
      <Statements>
        <Default Type=""System.Void"" />
      </Statements>
      <ReturnLabel>
        <LabelTarget Type=""System.Void"" Id=""0"" />
      </ReturnLabel>
    </CSharpBlock>
  </Expressions>
</Block>";

        [TestMethod]
        public void CSharp_DebugView_Test36()
        {
            Assert.AreEqual(dbg36, expr36.DebugView().ToString());
        }

        private Expression expr37 = CSharpStatement.Do(Expression.Empty(), Expression.Constant(true));
        private string dbg37 = @"<CSharpDo Type=""System.Void"">
  <Body>
    <Default Type=""System.Void"" />
  </Body>
  <Test>
    <Constant Type=""System.Boolean"" Value=""true"" />
  </Test>
</CSharpDo>";

        [TestMethod]
        public void CSharp_DebugView_Test37()
        {
            Assert.AreEqual(dbg37, expr37.DebugView().ToString());
        }

        private Expression expr38 = CSharpStatement.Do(Expression.Empty(), Expression.Constant(true), Expression.Label("break"), Expression.Label("continue"));
        private string dbg38 = @"<CSharpDo Type=""System.Void"">
  <Body>
    <Default Type=""System.Void"" />
  </Body>
  <Test>
    <Constant Type=""System.Boolean"" Value=""true"" />
  </Test>
  <BreakLabel>
    <LabelTarget Type=""System.Void"" Id=""0"" Name=""break"" />
  </BreakLabel>
  <ContinueLabel>
    <LabelTarget Type=""System.Void"" Id=""1"" Name=""continue"" />
  </ContinueLabel>
</CSharpDo>";

        [TestMethod]
        public void CSharp_DebugView_Test38()
        {
            Assert.AreEqual(dbg38, expr38.DebugView().ToString());
        }

        private Expression expr39 = CSharpStatement.While(Expression.Constant(true), Expression.Empty());
        private string dbg39 = @"<CSharpWhile Type=""System.Void"">
  <Test>
    <Constant Type=""System.Boolean"" Value=""true"" />
  </Test>
  <Body>
    <Default Type=""System.Void"" />
  </Body>
</CSharpWhile>";

        [TestMethod]
        public void CSharp_DebugView_Test39()
        {
            Assert.AreEqual(dbg39, expr39.DebugView().ToString());
        }

        private Expression expr40 = CSharpStatement.While(Expression.Constant(true), Expression.Empty(), Expression.Label("break"), Expression.Label("continue"));
        private string dbg40 = @"<CSharpWhile Type=""System.Void"">
  <Test>
    <Constant Type=""System.Boolean"" Value=""true"" />
  </Test>
  <Body>
    <Default Type=""System.Void"" />
  </Body>
  <BreakLabel>
    <LabelTarget Type=""System.Void"" Id=""0"" Name=""break"" />
  </BreakLabel>
  <ContinueLabel>
    <LabelTarget Type=""System.Void"" Id=""1"" Name=""continue"" />
  </ContinueLabel>
</CSharpWhile>";

        [TestMethod]
        public void CSharp_DebugView_Test40()
        {
            Assert.AreEqual(dbg40, expr40.DebugView().ToString());
        }

        private Expression expr41 = CSharpStatement.For(new ParameterExpression[0], new Expression[0], null, new Expression[0], Expression.Empty());
        private string dbg41 = @"<CSharpFor Type=""System.Void"">
  <Body>
    <Default Type=""System.Void"" />
  </Body>
</CSharpFor>";

        [TestMethod]
        public void CSharp_DebugView_Test41()
        {
            Assert.AreEqual(dbg41, expr41.DebugView().ToString());
        }

        private Expression expr42 = CSharpStatement.For(new[] { Expression.Parameter(typeof(int)) }, new[] { Expression.Assign(Expression.Parameter(typeof(int)), Expression.Constant(1)) }, Expression.LessThan(Expression.Parameter(typeof(int)), Expression.Constant(10)), new[] { Expression.PostIncrementAssign(Expression.Parameter(typeof(int))) }, Expression.Empty());
        private string dbg42 = @"<CSharpFor Type=""System.Void"">
  <Variables>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </Variables>
  <Initializers>
    <Assign Type=""System.Int32"">
      <Left>
        <Parameter Type=""System.Int32"" Id=""1"" />
      </Left>
      <Right>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Right>
    </Assign>
  </Initializers>
  <Test>
    <LessThan Type=""System.Boolean"">
      <Left>
        <Parameter Type=""System.Int32"" Id=""2"" />
      </Left>
      <Right>
        <Constant Type=""System.Int32"" Value=""10"" />
      </Right>
    </LessThan>
  </Test>
  <Iterators>
    <PostIncrementAssign Type=""System.Int32"">
      <Operand>
        <Parameter Type=""System.Int32"" Id=""3"" />
      </Operand>
    </PostIncrementAssign>
  </Iterators>
  <Body>
    <Default Type=""System.Void"" />
  </Body>
</CSharpFor>";

        [TestMethod]
        public void CSharp_DebugView_Test42()
        {
            Assert.AreEqual(dbg42, expr42.DebugView().ToString());
        }

        private Expression expr43 = CSharpStatement.For(new[] { Expression.Parameter(typeof(int)) }, new[] { Expression.Assign(Expression.Parameter(typeof(int)), Expression.Constant(1)) }, Expression.LessThan(Expression.Parameter(typeof(int)), Expression.Constant(10)), new[] { Expression.PostIncrementAssign(Expression.Parameter(typeof(int))) }, Expression.Empty(), Expression.Label("break"), Expression.Label("continue"));
        private string dbg43 = @"<CSharpFor Type=""System.Void"">
  <Variables>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </Variables>
  <Initializers>
    <Assign Type=""System.Int32"">
      <Left>
        <Parameter Type=""System.Int32"" Id=""1"" />
      </Left>
      <Right>
        <Constant Type=""System.Int32"" Value=""1"" />
      </Right>
    </Assign>
  </Initializers>
  <Test>
    <LessThan Type=""System.Boolean"">
      <Left>
        <Parameter Type=""System.Int32"" Id=""2"" />
      </Left>
      <Right>
        <Constant Type=""System.Int32"" Value=""10"" />
      </Right>
    </LessThan>
  </Test>
  <Iterators>
    <PostIncrementAssign Type=""System.Int32"">
      <Operand>
        <Parameter Type=""System.Int32"" Id=""3"" />
      </Operand>
    </PostIncrementAssign>
  </Iterators>
  <Body>
    <Default Type=""System.Void"" />
  </Body>
  <BreakLabel>
    <LabelTarget Type=""System.Void"" Id=""4"" Name=""break"" />
  </BreakLabel>
  <ContinueLabel>
    <LabelTarget Type=""System.Void"" Id=""5"" Name=""continue"" />
  </ContinueLabel>
</CSharpFor>";

        [TestMethod]
        public void CSharp_DebugView_Test43()
        {
            Assert.AreEqual(dbg43, expr43.DebugView().ToString());
        }

        private Expression expr44 = CSharpStatement.ForEach(Expression.Parameter(typeof(int)), Expression.Default(typeof(int[])), Expression.Empty());
        private string dbg44 = @"<CSharpForEach Type=""System.Void"">
  <Variable>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </Variable>
  <Collection>
    <Default Type=""System.Int32[]"" />
  </Collection>
  <Body>
    <Default Type=""System.Void"" />
  </Body>
</CSharpForEach>";

        [TestMethod]
        public void CSharp_DebugView_Test44()
        {
            Assert.AreEqual(dbg44, expr44.DebugView().ToString());
        }

        private Expression expr45 = CSharpStatement.ForEach(Expression.Parameter(typeof(int)), Expression.Default(typeof(int[])), Expression.Empty(), Expression.Label("break"), Expression.Label("continue"));
        private string dbg45 = @"<CSharpForEach Type=""System.Void"">
  <Variable>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </Variable>
  <Collection>
    <Default Type=""System.Int32[]"" />
  </Collection>
  <Body>
    <Default Type=""System.Void"" />
  </Body>
  <BreakLabel>
    <LabelTarget Type=""System.Void"" Id=""1"" Name=""break"" />
  </BreakLabel>
  <ContinueLabel>
    <LabelTarget Type=""System.Void"" Id=""2"" Name=""continue"" />
  </ContinueLabel>
</CSharpForEach>";

        [TestMethod]
        public void CSharp_DebugView_Test45()
        {
            Assert.AreEqual(dbg45, expr45.DebugView().ToString());
        }

        private Expression expr46 = CSharpStatement.ForEach(Expression.Parameter(typeof(int)), Expression.Default(typeof(int[])), Expression.Empty(), Expression.Label("break"), Expression.Label("continue"), Expression.Lambda(Expression.Default(typeof(int)), Expression.Parameter(typeof(int))));
        private string dbg46 = @"<CSharpForEach Type=""System.Void"">
  <Variable>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </Variable>
  <Conversion>
    <Lambda Type=""System.Func`2[System.Int32,System.Int32]"">
      <Parameters>
        <Parameter Type=""System.Int32"" Id=""1"" />
      </Parameters>
      <Body>
        <Default Type=""System.Int32"" />
      </Body>
    </Lambda>
  </Conversion>
  <Collection>
    <Default Type=""System.Int32[]"" />
  </Collection>
  <Body>
    <Default Type=""System.Void"" />
  </Body>
  <BreakLabel>
    <LabelTarget Type=""System.Void"" Id=""2"" Name=""break"" />
  </BreakLabel>
  <ContinueLabel>
    <LabelTarget Type=""System.Void"" Id=""3"" Name=""continue"" />
  </ContinueLabel>
</CSharpForEach>";

        [TestMethod]
        public void CSharp_DebugView_Test46()
        {
            Assert.AreEqual(dbg46, expr46.DebugView().ToString());
        }

        private Expression expr47 = CSharpStatement.Switch(Expression.Default(typeof(int)), Expression.Label("break"));
        private string dbg47 = @"<CSharpSwitch Type=""System.Void"">
  <SwitchValue>
    <Default Type=""System.Int32"" />
  </SwitchValue>
  <Cases />
  <BreakLabel>
    <LabelTarget Type=""System.Void"" Id=""0"" Name=""break"" />
  </BreakLabel>
</CSharpSwitch>";

        [TestMethod]
        public void CSharp_DebugView_Test47()
        {
            Assert.AreEqual(dbg47, expr47.DebugView().ToString());
        }

        private Expression expr48 = CSharpStatement.Switch(Expression.Default(typeof(int)), Expression.Label("break"), CSharpStatement.SwitchCase(new object[] { 1, 2 }, Expression.Empty()), CSharpStatement.SwitchCaseDefault(Expression.Empty()));
        private string dbg48 = @"<CSharpSwitch Type=""System.Void"">
  <SwitchValue>
    <Default Type=""System.Int32"" />
  </SwitchValue>
  <Cases>
    <CSharpSwitchCase TestValues=""1, 2"">
      <Statements>
        <Default Type=""System.Void"" />
      </Statements>
    </CSharpSwitchCase>
    <CSharpSwitchCase TestValues=""default"">
      <Statements>
        <Default Type=""System.Void"" />
      </Statements>
    </CSharpSwitchCase>
  </Cases>
  <BreakLabel>
    <LabelTarget Type=""System.Void"" Id=""0"" Name=""break"" />
  </BreakLabel>
</CSharpSwitch>";

        [TestMethod]
        public void CSharp_DebugView_Test48()
        {
            Assert.AreEqual(dbg48, expr48.DebugView().ToString());
        }

        private Expression expr49 = CSharpStatement.Switch(Expression.Default(typeof(string)), Expression.Label("break"), new[] { Expression.Parameter(typeof(int)) }, new[] { CSharpStatement.SwitchCase(new object[] { "bar", "foo", "this is a \"quoted\" string", null, CSharpStatement.SwitchCaseDefaultValue }, Expression.Empty()) });
        private string dbg49 = @"<CSharpSwitch Type=""System.Void"">
  <SwitchValue>
    <Default Type=""System.String"" />
  </SwitchValue>
  <Variables>
    <Parameter Type=""System.Int32"" Id=""0"" />
  </Variables>
  <Cases>
    <CSharpSwitchCase TestValues=""&quot;bar&quot;, &quot;foo&quot;, &quot;this is a \&quot;quoted\&quot; string&quot;, null, default"">
      <Statements>
        <Default Type=""System.Void"" />
      </Statements>
    </CSharpSwitchCase>
  </Cases>
  <BreakLabel>
    <LabelTarget Type=""System.Void"" Id=""1"" Name=""break"" />
  </BreakLabel>
</CSharpSwitch>";

        [TestMethod]
        public void CSharp_DebugView_Test49()
        {
            Assert.AreEqual(dbg49, expr49.DebugView().ToString());
        }

        private Expression expr50 = CSharpStatement.GotoLabel(Expression.Label());
        private string dbg50 = @"<CSharpGoto Type=""System.Void"">
  <Target>
    <LabelTarget Type=""System.Void"" Id=""0"" />
  </Target>
</CSharpGoto>";

        [TestMethod]
        public void CSharp_DebugView_Test50()
        {
            Assert.AreEqual(dbg50, expr50.DebugView().ToString());
        }

        private Expression expr51 = CSharpStatement.GotoCase(1);
        private string dbg51 = @"<CSharpGotoCase Type=""System.Void"" Value=""1"" />";

        [TestMethod]
        public void CSharp_DebugView_Test51()
        {
            Assert.AreEqual(dbg51, expr51.DebugView().ToString());
        }

        private Expression expr52 = CSharpStatement.GotoDefault();
        private string dbg52 = @"<CSharpGotoDefault Type=""System.Void"" />";

        [TestMethod]
        public void CSharp_DebugView_Test52()
        {
            Assert.AreEqual(dbg52, expr52.DebugView().ToString());
        }

        private Expression expr53 = CSharpStatement.Lock(Expression.Default(typeof(object)), Expression.Empty());
        private string dbg53 = @"<CSharpLock Type=""System.Void"">
  <Expression>
    <Default Type=""System.Object"" />
  </Expression>
  <Body>
    <Default Type=""System.Void"" />
  </Body>
</CSharpLock>";

        [TestMethod]
        public void CSharp_DebugView_Test53()
        {
            Assert.AreEqual(dbg53, expr53.DebugView().ToString());
        }

        private Expression expr54 = CSharpStatement.Using(Expression.Default(typeof(IDisposable)), Expression.Empty());
        private string dbg54 = @"<CSharpUsing Type=""System.Void"">
  <Resource>
    <Default Type=""System.IDisposable"" />
  </Resource>
  <Body>
    <Default Type=""System.Void"" />
  </Body>
</CSharpUsing>";

        [TestMethod]
        public void CSharp_DebugView_Test54()
        {
            Assert.AreEqual(dbg54, expr54.DebugView().ToString());
        }

        private Expression expr55 = CSharpStatement.Using(Expression.Parameter(typeof(IDisposable)), Expression.Default(typeof(IDisposable)), Expression.Empty());
        private string dbg55 = @"<CSharpUsing Type=""System.Void"">
  <Variable>
    <Parameter Type=""System.IDisposable"" Id=""0"" />
  </Variable>
  <Resource>
    <Default Type=""System.IDisposable"" />
  </Resource>
  <Body>
    <Default Type=""System.Void"" />
  </Body>
</CSharpUsing>";

        [TestMethod]
        public void CSharp_DebugView_Test55()
        {
            Assert.AreEqual(dbg55, expr55.DebugView().ToString());
        }

    }
}
