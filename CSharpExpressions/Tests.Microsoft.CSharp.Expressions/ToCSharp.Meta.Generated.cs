﻿// Prototyping extended expression trees for C#.
//
// bartde - December 2015

// NOTE: These tests are auto-generated and can *never* fail because they assert the outcome of DebugView()
//       at runtime against the outcome obtained at compile time. However, a human should read through the
//       cases to assert the outcome is as expected.
//
//       Regressions can still be caught given that the T4 won't be re-run unless it gets saved in the IDE.

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ToCSharpTests
    {
        private static LabelTarget _lbl1 = Expression.Label();
        private static LabelTarget _lbl2 = Expression.Label();
        private static LabelTarget _lbl3 = Expression.Label(typeof(int));
        private static ParameterExpression _par1 = Expression.Parameter(typeof(int));
        private static ParameterExpression _par2 = Expression.Parameter(typeof(int));
        private static ParameterExpression _par3 = Expression.Parameter(typeof(long));

        private Expression expr0 = CSharpExpression.PostIncrementAssign(Expression.Parameter(typeof(int)));
        private string dbg0 = @"p0++";

        [Fact]
        public void CSharp_ToCSharp_Test0()
        {
            Assert.Equal(dbg0, expr0.ToCSharp());
        }

        private Expression expr1 = /* BUG */ CSharpExpression.PostIncrementAssign(Expression.Parameter(typeof(int)), typeof(Math).GetMethod("Abs", new[] { typeof(int) }));
        private string dbg1 = @"p0 = Math.Abs(p0)";

        [Fact]
        public void CSharp_ToCSharp_Test1()
        {
            Assert.Equal(dbg1, expr1.ToCSharp());
        }

        private Expression expr2 = /* BUG */ CSharpExpression.PostDecrementAssign(Expression.Parameter(typeof(int)), typeof(Math).GetMethod("Abs", new[] { typeof(int) }));
        private string dbg2 = @"p0 = Math.Abs(p0)";

        [Fact]
        public void CSharp_ToCSharp_Test2()
        {
            Assert.Equal(dbg2, expr2.ToCSharp());
        }

        private Expression expr3 = CSharpExpression.PreDecrementAssignChecked(Expression.Parameter(typeof(int)));
        private string dbg3 = @"checked(--p0)";

        [Fact]
        public void CSharp_ToCSharp_Test3()
        {
            Assert.Equal(dbg3, expr3.ToCSharp());
        }

        private Expression expr4 = CSharpExpression.AddAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg4 = @"p0 += 1";

        [Fact]
        public void CSharp_ToCSharp_Test4()
        {
            Assert.Equal(dbg4, expr4.ToCSharp());
        }

        private Expression expr5 = CSharpExpression.NullCoalescingAssign(Expression.Parameter(typeof(int?)), Expression.Constant(1));
        private string dbg5 = @"p0 ??= 1";

        [Fact]
        public void CSharp_ToCSharp_Test5()
        {
            Assert.Equal(dbg5, expr5.ToCSharp());
        }

        private Expression expr6 = CSharpExpression.NullCoalescingAssign(Expression.Parameter(typeof(string)), Expression.Constant("foo"));
        private string dbg6 = @"p0 ??= ""foo""";

        [Fact]
        public void CSharp_ToCSharp_Test6()
        {
            Assert.Equal(dbg6, expr6.ToCSharp());
        }

        private Expression expr7 = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 1, 1 }, Expression.Constant(42));
        private string dbg7 = @"new int[1, 1] { { 42 } }";

        [Fact]
        public void CSharp_ToCSharp_Test7()
        {
            Assert.Equal(dbg7, expr7.ToCSharp());
        }

        private Expression expr8 = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 1, 2 }, Expression.Constant(42), Expression.Constant(43));
        private string dbg8 = @"new int[1, 2] { { 42, 43 } }";

        [Fact]
        public void CSharp_ToCSharp_Test8()
        {
            Assert.Equal(dbg8, expr8.ToCSharp());
        }

        private Expression expr9 = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 1 }, Expression.Constant(42), Expression.Constant(43));
        private string dbg9 = @"new int[2, 1] { { 42 }, { 43 } }";

        [Fact]
        public void CSharp_ToCSharp_Test9()
        {
            Assert.Equal(dbg9, expr9.ToCSharp());
        }

        private Expression expr10 = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 2 }, Expression.Constant(42), Expression.Constant(43), Expression.Constant(44), Expression.Constant(45));
        private string dbg10 = @"new int[2, 2] { { 42, 43 }, { 44, 45 } }";

        [Fact]
        public void CSharp_ToCSharp_Test10()
        {
            Assert.Equal(dbg10, expr10.ToCSharp());
        }

        private Expression expr11 = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3, 5 }, Enumerable.Range(0, 2 * 3 * 5).Select(i => Expression.Constant(i)));
        private string dbg11 = @"new int[2, 3, 5] { { { 0, 1, 2, 3, 4 }, { 5, 6, 7, 8, 9 }, { 10, 11, 12, 13, 14 } }, { { 15, 16, 17, 18, 19 }, { 20, 21, 22, 23, 24 }, { 25, 26, 27, 28, 29 } } }";

        [Fact]
        public void CSharp_ToCSharp_Test11()
        {
            Assert.Equal(dbg11, expr11.ToCSharp());
        }

        private Expression expr12 = Expression.ArrayAccess(CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 1, 1 }, Expression.Constant(42)), Expression.Constant(0), Expression.Constant(0));
        private string dbg12 = @"new int[1, 1] { { 42 } }[0, 0]";

        [Fact]
        public void CSharp_ToCSharp_Test12()
        {
            Assert.Equal(dbg12, expr12.ToCSharp());
        }

        private Expression expr13 = CSharpExpression.Await(Expression.Default(typeof(Task<int>)));
        private string dbg13 = @"await default(Task<int>)";

        [Fact]
        public void CSharp_ToCSharp_Test13()
        {
            Assert.Equal(dbg13, expr13.ToCSharp());
        }

        private Expression expr14 = DynamicCSharpExpression.DynamicAwait(Expression.Default(typeof(Task<int>)), false, typeof(object));
        private string dbg14 = @"await default(Task<int>)";

        [Fact]
        public void CSharp_ToCSharp_Test14()
        {
            Assert.Equal(dbg14, expr14.ToCSharp());
        }

        private Expression expr15 = CSharpExpression.AsyncLambda<Func<Task<int>>>(CSharpExpression.Await(Expression.Default(typeof(Task<int>))));
        private string dbg15 = @"async () => await default(Task<int>)";

        [Fact]
        public void CSharp_ToCSharp_Test15()
        {
            Assert.Equal(dbg15, expr15.ToCSharp());
        }

        private Expression expr16 = CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Block(typeof(int), CSharpExpression.Await(Expression.Default(typeof(Task<int>)))));
        private string dbg16 = @"async () =>
{
    return await default(Task<int>);
}";

        [Fact]
        public void CSharp_ToCSharp_Test16()
        {
            Assert.Equal(dbg16, expr16.ToCSharp());
        }

        private Expression expr17 = CSharpExpression.AsyncLambda<Func<Task<int>>>(CSharpExpression.Block(new[] { CSharpExpression.Await(Expression.Default(typeof(Task<int>))) }, _lbl3));
        private string dbg17 = @"async () =>
{
    return await default(Task<int>);
}";

        [Fact]
        public void CSharp_ToCSharp_Test17()
        {
            Assert.Equal(dbg17, expr17.ToCSharp());
        }

        private Expression expr18 = CSharpExpression.AsyncLambda<Func<bool, string, Task<int>>>(CSharpExpression.Await(Expression.Default(typeof(Task<int>))), Expression.Parameter(typeof(bool), "b"), Expression.Parameter(typeof(string), "s"));
        private string dbg18 = @"async (bool b, string s) => await default(Task<int>)";

        [Fact]
        public void CSharp_ToCSharp_Test18()
        {
            Assert.Equal(dbg18, expr18.ToCSharp());
        }

        private Expression expr19 = CSharpExpression.Call(typeof(Math).GetMethod("Abs", new[] { typeof(int) }), CSharpExpression.Bind(typeof(Math).GetMethod("Abs", new[] { typeof(int) }).GetParameters()[0], Expression.Constant(42)));
        private string dbg19 = @"Math.Abs(42)";

        [Fact]
        public void CSharp_ToCSharp_Test19()
        {
            Assert.Equal(dbg19, expr19.ToCSharp());
        }

        private Expression expr20 = CSharpExpression.Call(Expression.Default(typeof(string)), typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) }), CSharpExpression.Bind(typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) }).GetParameters()[1], Expression.Constant(1)), CSharpExpression.Bind(typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) }).GetParameters()[0], Expression.Constant(2)));
        private string dbg20 = @"default(string).Substring(length: 1, startIndex: 2)";

        [Fact]
        public void CSharp_ToCSharp_Test20()
        {
            Assert.Equal(dbg20, expr20.ToCSharp());
        }

        private Expression expr21 = Expression.Property(CSharpExpression.Call(Expression.Default(typeof(string)), typeof(string).GetMethod("Substring", new[] { typeof(int) }), CSharpExpression.Bind(typeof(string).GetMethod("Substring", new[] { typeof(int) }).GetParameters()[0], Expression.Constant(42))), "Length");
        private string dbg21 = @"default(string).Substring(42).Length";

        [Fact]
        public void CSharp_ToCSharp_Test21()
        {
            Assert.Equal(dbg21, expr21.ToCSharp());
        }

        private Expression expr22 = CSharpExpression.Call(null, typeof(Activator).GetMethod("CreateInstance", new Type[0]).MakeGenericMethod(typeof(string)));
        private string dbg22 = @"Activator.CreateInstance<string>()";

        [Fact]
        public void CSharp_ToCSharp_Test22()
        {
            Assert.Equal(dbg22, expr22.ToCSharp());
        }

        private Expression expr23 = CSharpExpression.Invoke(Expression.Default(typeof(Action<int>)), CSharpExpression.Bind(typeof(Action<int>).GetMethod("Invoke").GetParameters()[0], Expression.Constant(42)));
        private string dbg23 = @"default(Action<int>)(42)";

        [Fact]
        public void CSharp_ToCSharp_Test23()
        {
            Assert.Equal(dbg23, expr23.ToCSharp());
        }

        private Expression expr24 = CSharpExpression.New(typeof(TimeSpan).GetConstructor(new[] { typeof(long) }), CSharpExpression.Bind(typeof(TimeSpan).GetConstructor(new[] { typeof(long) }).GetParameters()[0], Expression.Constant(42L)));
        private string dbg24 = @"new TimeSpan(42L)";

        [Fact]
        public void CSharp_ToCSharp_Test24()
        {
            Assert.Equal(dbg24, expr24.ToCSharp());
        }

        private Expression expr25 = Expression.Property(CSharpExpression.New(typeof(TimeSpan).GetConstructor(new[] { typeof(long) }), CSharpExpression.Bind(typeof(TimeSpan).GetConstructor(new[] { typeof(long) }).GetParameters()[0], Expression.Constant(42L))), "Ticks");
        private string dbg25 = @"new TimeSpan(42L).Ticks";

        [Fact]
        public void CSharp_ToCSharp_Test25()
        {
            Assert.Equal(dbg25, expr25.ToCSharp());
        }

        private Expression expr26 = CSharpExpression.Index(Expression.Default(typeof(List<int>)), typeof(List<int>).GetProperty("Item"), CSharpExpression.Bind(typeof(List<int>).GetProperty("Item").GetIndexParameters()[0], Expression.Constant(42)));
        private string dbg26 = @"default(List<int>)[42]";

        [Fact]
        public void CSharp_ToCSharp_Test26()
        {
            Assert.Equal(dbg26, expr26.ToCSharp());
        }

        private Expression expr27 = CSharpExpression.ConditionalArrayIndex(Expression.Default(typeof(string[])), Expression.Constant(42));
        private string dbg27 = @"default(string[])?[42]";

        [Fact]
        public void CSharp_ToCSharp_Test27()
        {
            Assert.Equal(dbg27, expr27.ToCSharp());
        }

        private Expression expr28 = CSharpExpression.ConditionalCall(Expression.Default(typeof(string)), typeof(string).GetMethod("Substring", new[] { typeof(int) }), CSharpExpression.Bind(typeof(string).GetMethod("Substring", new[] { typeof(int) }).GetParameters()[0], Expression.Constant(42)));
        private string dbg28 = @"default(string)?.Substring(42)";

        [Fact]
        public void CSharp_ToCSharp_Test28()
        {
            Assert.Equal(dbg28, expr28.ToCSharp());
        }

        private Expression expr29 = CSharpExpression.ConditionalIndex(Expression.Default(typeof(List<int>)), typeof(List<int>).GetProperty("Item"), CSharpExpression.Bind(typeof(List<int>).GetProperty("Item").GetIndexParameters()[0], Expression.Constant(42)));
        private string dbg29 = @"default(List<int>)?[42]";

        [Fact]
        public void CSharp_ToCSharp_Test29()
        {
            Assert.Equal(dbg29, expr29.ToCSharp());
        }

        private Expression expr30 = CSharpExpression.ConditionalProperty(Expression.Default(typeof(string)), typeof(string).GetProperty("Length"));
        private string dbg30 = @"default(string)?.Length";

        [Fact]
        public void CSharp_ToCSharp_Test30()
        {
            Assert.Equal(dbg30, expr30.ToCSharp());
        }

        private Expression expr31 = CSharpExpression.ConditionalAccess(Expression.Default(typeof(string)), CSharpExpression.ConditionalReceiver(typeof(string)), Expression.Property(CSharpExpression.ConditionalReceiver(typeof(string)), "Length"));
        private string dbg31 = @"default(string)?.Length";

        [Fact]
        public void CSharp_ToCSharp_Test31()
        {
            Assert.Equal(dbg31, expr31.ToCSharp());
        }

        private Expression expr32 = DynamicCSharpExpression.DynamicAdd(Expression.Constant(1), Expression.Constant(2));
        private string dbg32 = @"1 /*dynamic*/+ 2";

        [Fact]
        public void CSharp_ToCSharp_Test32()
        {
            Assert.Equal(dbg32, expr32.ToCSharp());
        }

        private Expression expr33 = DynamicCSharpExpression.DynamicAdd(DynamicCSharpExpression.DynamicArgument(Expression.Constant(1)), DynamicCSharpExpression.DynamicArgument(Expression.Constant(2)), CSharpBinderFlags.CheckedContext);
        private string dbg33 = @"checked(1 /*dynamic*/+ 2)";

        [Fact]
        public void CSharp_ToCSharp_Test33()
        {
            Assert.Equal(dbg33, expr33.ToCSharp());
        }

        private Expression expr34 = DynamicCSharpExpression.DynamicNegate(Expression.Constant(1));
        private string dbg34 = @"/*dynamic*/-1";

        [Fact]
        public void CSharp_ToCSharp_Test34()
        {
            Assert.Equal(dbg34, expr34.ToCSharp());
        }

        private Expression expr35 = DynamicCSharpExpression.DynamicConvert(Expression.Constant(1), typeof(int), CSharpBinderFlags.ConvertExplicit);
        private string dbg35 = @"/*dynamic*/(int)1";

        [Fact]
        public void CSharp_ToCSharp_Test35()
        {
            Assert.Equal(dbg35, expr35.ToCSharp());
        }

        private Expression expr36 = DynamicCSharpExpression.DynamicGetMember(Expression.Default(typeof(string)), "Length");
        private string dbg36 = @"default(string)/*dynamic*/.Length";

        [Fact]
        public void CSharp_ToCSharp_Test36()
        {
            Assert.Equal(dbg36, expr36.ToCSharp());
        }

        private Expression expr37 = DynamicCSharpExpression.DynamicGetIndex(Expression.Default(typeof(List<int>)), Expression.Constant(1));
        private string dbg37 = @"default(List<int>)/*dynamic*/[1]";

        [Fact]
        public void CSharp_ToCSharp_Test37()
        {
            Assert.Equal(dbg37, expr37.ToCSharp());
        }

        private Expression expr38 = DynamicCSharpExpression.DynamicInvoke(Expression.Default(typeof(Action<int>)), Expression.Constant(1));
        private string dbg38 = @"default(Action<int>)/*dynamic*/(1)";

        [Fact]
        public void CSharp_ToCSharp_Test38()
        {
            Assert.Equal(dbg38, expr38.ToCSharp());
        }

        private Expression expr39 = DynamicCSharpExpression.DynamicInvokeMember(typeof(Math), "Abs", Expression.Constant(1));
        private string dbg39 = @"Math/*dynamic*/.Abs(1)";

        [Fact]
        public void CSharp_ToCSharp_Test39()
        {
            Assert.Equal(dbg39, expr39.ToCSharp());
        }

        private Expression expr40 = DynamicCSharpExpression.DynamicInvokeMember(typeof(Activator), "CreateInstance", new[] { typeof(int) });
        private string dbg40 = @"Activator/*dynamic*/.CreateInstance<int>()";

        [Fact]
        public void CSharp_ToCSharp_Test40()
        {
            Assert.Equal(dbg40, expr40.ToCSharp());
        }

        private Expression expr41 = DynamicCSharpExpression.DynamicInvokeMember(Expression.Default(typeof(string)), "Substring", Expression.Constant(1));
        private string dbg41 = @"default(string)/*dynamic*/.Substring(1)";

        [Fact]
        public void CSharp_ToCSharp_Test41()
        {
            Assert.Equal(dbg41, expr41.ToCSharp());
        }

        private Expression expr42 = DynamicCSharpExpression.DynamicInvokeMember(Expression.Default(typeof(string)), "Substring", new[] { DynamicCSharpExpression.DynamicArgument(Expression.Constant(1), "startIndex") });
        private string dbg42 = @"default(string)/*dynamic*/.Substring(1)";

        [Fact]
        public void CSharp_ToCSharp_Test42()
        {
            Assert.Equal(dbg42, expr42.ToCSharp());
        }

        private Expression expr43 = DynamicCSharpExpression.DynamicInvokeMember(Expression.Default(typeof(string)), "Substring", new[] { DynamicCSharpExpression.DynamicArgument(Expression.Constant(1), "startIndex", CSharpArgumentInfoFlags.NamedArgument) });
        private string dbg43 = @"default(string)/*dynamic*/.Substring(startIndex: 1)";

        [Fact]
        public void CSharp_ToCSharp_Test43()
        {
            Assert.Equal(dbg43, expr43.ToCSharp());
        }

        private Expression expr44 = DynamicCSharpExpression.DynamicInvokeMember(Expression.Default(typeof(string)), "Substring", new[] { DynamicCSharpExpression.DynamicArgument(Expression.Constant(1), "startIndex", CSharpArgumentInfoFlags.NamedArgument), DynamicCSharpExpression.DynamicArgument(Expression.Constant(2), "length", CSharpArgumentInfoFlags.NamedArgument) });
        private string dbg44 = @"default(string)/*dynamic*/.Substring(startIndex: 1, length: 2)";

        [Fact]
        public void CSharp_ToCSharp_Test44()
        {
            Assert.Equal(dbg44, expr44.ToCSharp());
        }

        private Expression expr45 = DynamicCSharpExpression.DynamicInvokeConstructor(typeof(TimeSpan), Expression.Constant(1L));
        private string dbg45 = @"/*dynamic*/new TimeSpan(1L)";

        [Fact]
        public void CSharp_ToCSharp_Test45()
        {
            Assert.Equal(dbg45, expr45.ToCSharp());
        }

        private Expression expr46 = DynamicCSharpExpression.DynamicAddAssign(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg46 = @"p0 /*dynamic*/+= 1";

        [Fact]
        public void CSharp_ToCSharp_Test46()
        {
            Assert.Equal(dbg46, expr46.ToCSharp());
        }

        private Expression expr47 = DynamicCSharpExpression.DynamicAddAssignChecked(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg47 = @"checked(p0 /*dynamic*/+= 1)";

        [Fact]
        public void CSharp_ToCSharp_Test47()
        {
            Assert.Equal(dbg47, expr47.ToCSharp());
        }

        private Expression expr48 = DynamicCSharpExpression.DynamicSubtractAssign(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg48 = @"p0 /*dynamic*/-= 1";

        [Fact]
        public void CSharp_ToCSharp_Test48()
        {
            Assert.Equal(dbg48, expr48.ToCSharp());
        }

        private Expression expr49 = DynamicCSharpExpression.DynamicSubtractAssignChecked(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg49 = @"checked(p0 /*dynamic*/-= 1)";

        [Fact]
        public void CSharp_ToCSharp_Test49()
        {
            Assert.Equal(dbg49, expr49.ToCSharp());
        }

        private Expression expr50 = DynamicCSharpExpression.DynamicMultiplyAssign(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg50 = @"p0 /*dynamic*/*= 1";

        [Fact]
        public void CSharp_ToCSharp_Test50()
        {
            Assert.Equal(dbg50, expr50.ToCSharp());
        }

        private Expression expr51 = DynamicCSharpExpression.DynamicMultiplyAssignChecked(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg51 = @"checked(p0 /*dynamic*/*= 1)";

        [Fact]
        public void CSharp_ToCSharp_Test51()
        {
            Assert.Equal(dbg51, expr51.ToCSharp());
        }

        private Expression expr52 = DynamicCSharpExpression.DynamicDivideAssign(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg52 = @"p0 /*dynamic*//= 1";

        [Fact]
        public void CSharp_ToCSharp_Test52()
        {
            Assert.Equal(dbg52, expr52.ToCSharp());
        }

        private Expression expr53 = DynamicCSharpExpression.DynamicModuloAssign(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg53 = @"p0 /*dynamic*/%= 1";

        [Fact]
        public void CSharp_ToCSharp_Test53()
        {
            Assert.Equal(dbg53, expr53.ToCSharp());
        }

        private Expression expr54 = DynamicCSharpExpression.DynamicAndAssign(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg54 = @"p0 /*dynamic*/&= 1";

        [Fact]
        public void CSharp_ToCSharp_Test54()
        {
            Assert.Equal(dbg54, expr54.ToCSharp());
        }

        private Expression expr55 = DynamicCSharpExpression.DynamicOrAssign(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg55 = @"p0 /*dynamic*/|= 1";

        [Fact]
        public void CSharp_ToCSharp_Test55()
        {
            Assert.Equal(dbg55, expr55.ToCSharp());
        }

        private Expression expr56 = DynamicCSharpExpression.DynamicExclusiveOrAssign(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg56 = @"p0 /*dynamic*/^= 1";

        [Fact]
        public void CSharp_ToCSharp_Test56()
        {
            Assert.Equal(dbg56, expr56.ToCSharp());
        }

        private Expression expr57 = DynamicCSharpExpression.DynamicLeftShiftAssign(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg57 = @"p0 /*dynamic*/<<= 1";

        [Fact]
        public void CSharp_ToCSharp_Test57()
        {
            Assert.Equal(dbg57, expr57.ToCSharp());
        }

        private Expression expr58 = DynamicCSharpExpression.DynamicRightShiftAssign(Expression.Parameter(typeof(object)), Expression.Constant(1));
        private string dbg58 = @"p0 /*dynamic*/>>= 1";

        [Fact]
        public void CSharp_ToCSharp_Test58()
        {
            Assert.Equal(dbg58, expr58.ToCSharp());
        }

        private Expression expr59 = DynamicCSharpExpression.DynamicPreIncrementAssign(Expression.Parameter(typeof(object)));
        private string dbg59 = @"/*dynamic*/++p0";

        [Fact]
        public void CSharp_ToCSharp_Test59()
        {
            Assert.Equal(dbg59, expr59.ToCSharp());
        }

        private Expression expr60 = DynamicCSharpExpression.DynamicPreIncrementAssignChecked(Expression.Parameter(typeof(object)));
        private string dbg60 = @"checked(/*dynamic*/++p0)";

        [Fact]
        public void CSharp_ToCSharp_Test60()
        {
            Assert.Equal(dbg60, expr60.ToCSharp());
        }

        private Expression expr61 = DynamicCSharpExpression.DynamicPreDecrementAssign(Expression.Parameter(typeof(object)));
        private string dbg61 = @"/*dynamic*/--p0";

        [Fact]
        public void CSharp_ToCSharp_Test61()
        {
            Assert.Equal(dbg61, expr61.ToCSharp());
        }

        private Expression expr62 = DynamicCSharpExpression.DynamicPreDecrementAssignChecked(Expression.Parameter(typeof(object)));
        private string dbg62 = @"checked(/*dynamic*/--p0)";

        [Fact]
        public void CSharp_ToCSharp_Test62()
        {
            Assert.Equal(dbg62, expr62.ToCSharp());
        }

        private Expression expr63 = DynamicCSharpExpression.DynamicPostIncrementAssign(Expression.Parameter(typeof(object)));
        private string dbg63 = @"p0/*dynamic*/++";

        [Fact]
        public void CSharp_ToCSharp_Test63()
        {
            Assert.Equal(dbg63, expr63.ToCSharp());
        }

        private Expression expr64 = DynamicCSharpExpression.DynamicPostIncrementAssignChecked(Expression.Parameter(typeof(object)));
        private string dbg64 = @"checked(p0/*dynamic*/++)";

        [Fact]
        public void CSharp_ToCSharp_Test64()
        {
            Assert.Equal(dbg64, expr64.ToCSharp());
        }

        private Expression expr65 = DynamicCSharpExpression.DynamicPostDecrementAssign(Expression.Parameter(typeof(object)));
        private string dbg65 = @"p0/*dynamic*/--";

        [Fact]
        public void CSharp_ToCSharp_Test65()
        {
            Assert.Equal(dbg65, expr65.ToCSharp());
        }

        private Expression expr66 = DynamicCSharpExpression.DynamicPostDecrementAssignChecked(Expression.Parameter(typeof(object)));
        private string dbg66 = @"checked(p0/*dynamic*/--)";

        [Fact]
        public void CSharp_ToCSharp_Test66()
        {
            Assert.Equal(dbg66, expr66.ToCSharp());
        }

        private Expression expr67 = CSharpExpression.Block(new Expression[] { Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")) }, Expression.Label());
        private string dbg67 = @"{
    Console.WriteLine(""body"");
}";

        [Fact]
        public void CSharp_ToCSharp_Test67()
        {
            Assert.Equal(dbg67, expr67.ToCSharp());
        }

        private Expression expr68 = CSharpExpression.Block(new Expression[] { Expression.Constant(42) }, _lbl3);
        private string dbg68 = @"{
    return 42;
}";

        [Fact]
        public void CSharp_ToCSharp_Test68()
        {
            Assert.Equal(dbg68, expr68.ToCSharp());
        }

        private Expression expr69 = CSharpExpression.Block(new[] { Expression.Parameter(typeof(int)) }, new Expression[] { Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("stmt1")), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("stmt2")), Expression.Return(_lbl1) }, _lbl1);
        private string dbg69 = @"{
    int p0 /*(null)*/;
    Console.WriteLine(""stmt1"");
    Console.WriteLine(""stmt2"");
    return;
}";

        [Fact]
        public void CSharp_ToCSharp_Test69()
        {
            Assert.Equal(dbg69, expr69.ToCSharp());
        }

        private Expression expr70 = CSharpExpression.Block(new[] { Expression.Parameter(typeof(int)) }, new Expression[] { Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("stmt1")), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("stmt2")), Expression.Return(_lbl3, Expression.Constant(1)) }, _lbl3);
        private string dbg70 = @"{
    int p0 /*(null)*/;
    Console.WriteLine(""stmt1"");
    Console.WriteLine(""stmt2"");
    return 1;
}";

        [Fact]
        public void CSharp_ToCSharp_Test70()
        {
            Assert.Equal(dbg70, expr70.ToCSharp());
        }

        private Expression expr71 = Expression.Lambda(CSharpExpression.Block(new Expression[] { Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")) }, Expression.Label()));
        private string dbg71 = @"() =>
{
    Console.WriteLine(""body"");
}";

        [Fact]
        public void CSharp_ToCSharp_Test71()
        {
            Assert.Equal(dbg71, expr71.ToCSharp());
        }

        private Expression expr72 = CSharpStatement.Do(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")), Expression.Constant(true));
        private string dbg72 = @"do
{
    Console.WriteLine(""body"");
}
while (true);";

        [Fact]
        public void CSharp_ToCSharp_Test72()
        {
            Assert.Equal(dbg72, expr72.ToCSharp());
        }

        private Expression expr73 = CSharpStatement.While(Expression.Constant(true), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg73 = @"while (true)
    Console.WriteLine(""body"");";

        [Fact]
        public void CSharp_ToCSharp_Test73()
        {
            Assert.Equal(dbg73, expr73.ToCSharp());
        }

        private Expression expr74 = CSharpStatement.For(new ParameterExpression[0], new Expression[0], null, new Expression[0], Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg74 = @"for (;;)
    Console.WriteLine(""body"");";

        [Fact]
        public void CSharp_ToCSharp_Test74()
        {
            Assert.Equal(dbg74, expr74.ToCSharp());
        }

        private Expression expr75 = CSharpStatement.For(new[] { _par1 }, new[] { Expression.Assign(_par1, Expression.Constant(1)) }, Expression.LessThan(Expression.Parameter(typeof(int)), Expression.Constant(10)), new[] { Expression.PostIncrementAssign(_par1) }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg75 = @"for (int p0 = 1; p1 < 10; p0++)
    Console.WriteLine(""body"");";

        [Fact]
        public void CSharp_ToCSharp_Test75()
        {
            Assert.Equal(dbg75, expr75.ToCSharp());
        }

        private Expression expr76 = CSharpStatement.For(new[] { _par1 }, new[] { Expression.Assign(_par1, Expression.Constant(1)), Expression.Assign(_par1, Expression.Constant(2)) }, Expression.LessThan(Expression.Parameter(typeof(int)), Expression.Constant(10)), new[] { Expression.PostIncrementAssign(_par1) }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg76 = @"{
    int p0 /*(null)*/;
    for (p0 = 1, p0 = 2; p1 < 10; p0++)
        Console.WriteLine(""body"");
}";

        [Fact]
        public void CSharp_ToCSharp_Test76()
        {
            Assert.Equal(dbg76, expr76.ToCSharp());
        }

        private Expression expr77 = CSharpStatement.For(new[] { _par1 }, new[] { CSharpExpression.Assign(_par1, Expression.Constant(1)) }, Expression.LessThan(Expression.Parameter(typeof(int)), Expression.Constant(10)), new[] { Expression.PostIncrementAssign(_par1) }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg77 = @"for (int p0 = 1; p1 < 10; p0++)
    Console.WriteLine(""body"");";

        [Fact]
        public void CSharp_ToCSharp_Test77()
        {
            Assert.Equal(dbg77, expr77.ToCSharp());
        }

        private Expression expr78 = CSharpStatement.For(new[] { _par1, _par2 }, new[] { Expression.Assign(_par1, Expression.Constant(0)), Expression.Assign(_par2, Expression.Constant(10)) }, Expression.LessThan(_par1, _par2), new[] { Expression.PostIncrementAssign(_par1), Expression.PreDecrementAssign(_par2) }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg78 = @"for (int p0 = 0, p1 = 10; p0 < p1; p0++, --p1)
    Console.WriteLine(""body"");";

        [Fact]
        public void CSharp_ToCSharp_Test78()
        {
            Assert.Equal(dbg78, expr78.ToCSharp());
        }

        private Expression expr79 = CSharpStatement.For(new[] { _par1 }, new Expression[0], Expression.LessThan(Expression.Parameter(typeof(int)), Expression.Constant(10)), new[] { Expression.PostIncrementAssign(_par1) }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg79 = @"{
    int p0 /*(null)*/;
    for (; p1 < 10; p0++)
        Console.WriteLine(""body"");
}";

        [Fact]
        public void CSharp_ToCSharp_Test79()
        {
            Assert.Equal(dbg79, expr79.ToCSharp());
        }

        private Expression expr80 = CSharpStatement.For(new[] { _par1, _par3 }, new[] { Expression.Assign(_par1, Expression.Constant(0)), Expression.Assign(_par3, Expression.Constant(10L)) }, Expression.LessThan(_par1, _par2), new[] { Expression.PostIncrementAssign(_par1), Expression.PreDecrementAssign(_par2) }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg80 = @"{
    int p0 /*(null)*/;
    long p1 /*(null)*/;
    for (p0 = 0, p1 = 10L; p0 < p2; p0++, --p2)
        Console.WriteLine(""body"");
}";

        [Fact]
        public void CSharp_ToCSharp_Test80()
        {
            Assert.Equal(dbg80, expr80.ToCSharp());
        }

        private Expression expr81 = CSharpStatement.ForEach(Expression.Parameter(typeof(int)), Expression.Default(typeof(int[])), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg81 = @"foreach (int p0 /*(null)*/ in default(int[]))
    Console.WriteLine(""body"");";

        [Fact]
        public void CSharp_ToCSharp_Test81()
        {
            Assert.Equal(dbg81, expr81.ToCSharp());
        }

        private Expression expr82 = CSharpStatement.Switch(Expression.Default(typeof(int)), Expression.Label("break"));
        private string dbg82 = @"switch (default(int))
{
    
}";

        [Fact]
        public void CSharp_ToCSharp_Test82()
        {
            Assert.Equal(dbg82, expr82.ToCSharp());
        }

        private Expression expr83 = CSharpStatement.Switch(Expression.Default(typeof(int)), Expression.Label("break"), CSharpStatement.SwitchCase(new object[] { 1, 2 }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("case 1,2"))), CSharpStatement.SwitchCaseDefault(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("default"))));
        private string dbg83 = @"switch (default(int))
{
    case 1:
    case 2:
        Console.WriteLine(""case 1,2"");
        break;
    default:
        Console.WriteLine(""default"");
        break;
}";

        [Fact]
        public void CSharp_ToCSharp_Test83()
        {
            Assert.Equal(dbg83, expr83.ToCSharp());
        }

        private Expression expr84 = CSharpStatement.Switch(Expression.Default(typeof(string)), _lbl1, new[] { Expression.Parameter(typeof(int)) }, new[] { CSharpStatement.SwitchCase(new object[] { "bar", "foo", "this is a \"quoted\" string", null, CSharpStatement.SwitchCaseDefaultValue }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")), Expression.Break(_lbl1)) });
        private string dbg84 = @"{
    int p0 /*(null)*/;
    switch (default(string))
    {
        case ""bar"":
        case ""foo"":
        case ""this is a \""quoted\"" string"":
        case null:
        default:
            Console.WriteLine(""body"");
            break;
    }
}";

        [Fact]
        public void CSharp_ToCSharp_Test84()
        {
            Assert.Equal(dbg84, expr84.ToCSharp());
        }

        private Expression expr85 = CSharpStatement.Switch(Expression.Default(typeof(string)), _lbl1, new[] { _par1 }, new[] { CSharpStatement.SwitchCase(new object[] { "bar" }, Expression.Block(typeof(void), Expression.Assign(_par1, Expression.Constant(0))), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")), Expression.Break(_lbl1)) });
        private string dbg85 = @"switch (default(string))
{
    case ""bar"":
        int p0 /*(null)*/;
        {
            p0 = 0;
        }
        Console.WriteLine(""body"");
        break;
}";

        [Fact]
        public void CSharp_ToCSharp_Test85()
        {
            Assert.Equal(dbg85, expr85.ToCSharp());
        }

        private Expression expr86 = CSharpStatement.Switch(Expression.Default(typeof(string)), _lbl1, new[] { _par1 }, new[] { CSharpStatement.SwitchCase(new object[] { "bar" }, Expression.Block(typeof(void), new[] { _par2 }, Expression.Assign(_par1, _par2)), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("bar")), Expression.Break(_lbl1)), CSharpStatement.SwitchCase(new object[] { "foo" }, Expression.Block(typeof(void), Expression.Assign(_par1, Expression.Constant(1))), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("foo")), Expression.Break(_lbl1)) });
        private string dbg86 = @"{
    int p0 /*(null)*/;
    switch (default(string))
    {
        case ""bar"":
            {
                int p1 /*(null)*/;
                p0 = p1;
            }
            Console.WriteLine(""bar"");
            break;
        case ""foo"":
            {
                p0 = 1;
            }
            Console.WriteLine(""foo"");
            break;
    }
}";

        [Fact]
        public void CSharp_ToCSharp_Test86()
        {
            Assert.Equal(dbg86, expr86.ToCSharp());
        }

        private Expression expr87 = CSharpStatement.GotoLabel(Expression.Label());
        private string dbg87 = @"goto L0;";

        [Fact]
        public void CSharp_ToCSharp_Test87()
        {
            Assert.Equal(dbg87, expr87.ToCSharp());
        }

        private Expression expr88 = CSharpStatement.GotoCase(1);
        private string dbg88 = @"goto case 1;";

        [Fact]
        public void CSharp_ToCSharp_Test88()
        {
            Assert.Equal(dbg88, expr88.ToCSharp());
        }

        private Expression expr89 = CSharpStatement.GotoCase(null);
        private string dbg89 = @"goto case null;";

        [Fact]
        public void CSharp_ToCSharp_Test89()
        {
            Assert.Equal(dbg89, expr89.ToCSharp());
        }

        private Expression expr90 = CSharpStatement.GotoDefault();
        private string dbg90 = @"goto default;";

        [Fact]
        public void CSharp_ToCSharp_Test90()
        {
            Assert.Equal(dbg90, expr90.ToCSharp());
        }

        private Expression expr91 = CSharpStatement.Lock(Expression.Default(typeof(object)), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg91 = @"lock (default(object))
    Console.WriteLine(""body"");";

        [Fact]
        public void CSharp_ToCSharp_Test91()
        {
            Assert.Equal(dbg91, expr91.ToCSharp());
        }

        private Expression expr92 = CSharpStatement.Using(Expression.Default(typeof(IDisposable)), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg92 = @"using (default(IDisposable))
    Console.WriteLine(""body"");";

        [Fact]
        public void CSharp_ToCSharp_Test92()
        {
            Assert.Equal(dbg92, expr92.ToCSharp());
        }

        private Expression expr93 = CSharpStatement.Using(Expression.Parameter(typeof(IDisposable)), Expression.Default(typeof(IDisposable)), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg93 = @"using (IDisposable p0 /*(null)*/ = default(IDisposable))
    Console.WriteLine(""body"");";

        [Fact]
        public void CSharp_ToCSharp_Test93()
        {
            Assert.Equal(dbg93, expr93.ToCSharp());
        }

        private Expression expr94 = Expression.AddChecked(CSharpExpression.PostIncrementAssign(_par1), Expression.Constant(1));
        private string dbg94 = @"p0++ /*checked(*/+/*)*/ 1";

        [Fact]
        public void CSharp_ToCSharp_Test94()
        {
            Assert.Equal(dbg94, expr94.ToCSharp());
        }

        private Expression expr95 = Expression.AddChecked(CSharpExpression.PostIncrementAssignChecked(_par1), Expression.Constant(1));
        private string dbg95 = @"checked(p0++ + 1)";

        [Fact]
        public void CSharp_ToCSharp_Test95()
        {
            Assert.Equal(dbg95, expr95.ToCSharp());
        }

        private Expression expr96 = Expression.NegateChecked(CSharpExpression.PostIncrementAssign(_par1));
        private string dbg96 = @"/*checked(*/-/*)*/p0++";

        [Fact]
        public void CSharp_ToCSharp_Test96()
        {
            Assert.Equal(dbg96, expr96.ToCSharp());
        }

        private Expression expr97 = Expression.NegateChecked(CSharpExpression.PostIncrementAssignChecked(_par1));
        private string dbg97 = @"checked(-p0++)";

        [Fact]
        public void CSharp_ToCSharp_Test97()
        {
            Assert.Equal(dbg97, expr97.ToCSharp());
        }

        private Expression expr98 = Expression.NegateChecked(CSharpExpression.AddAssign(_par1, Expression.Constant(1)));
        private string dbg98 = @"/*checked(*/-/*)*/(p0 += 1)";

        [Fact]
        public void CSharp_ToCSharp_Test98()
        {
            Assert.Equal(dbg98, expr98.ToCSharp());
        }

        private Expression expr99 = Expression.NegateChecked(CSharpExpression.AddAssignChecked(_par1, Expression.Constant(1)));
        private string dbg99 = @"checked(-(p0 += 1))";

        [Fact]
        public void CSharp_ToCSharp_Test99()
        {
            Assert.Equal(dbg99, expr99.ToCSharp());
        }

        private Expression expr100 = CSharpExpression.AddAssignChecked(_par1, Expression.Negate(Expression.Constant(1)));
        private string dbg100 = @"p0 /*checked(*/+=/*)*/ -(1)";

        [Fact]
        public void CSharp_ToCSharp_Test100()
        {
            Assert.Equal(dbg100, expr100.ToCSharp());
        }

        private Expression expr101 = CSharpExpression.AddAssignChecked(_par1, Expression.NegateChecked(Expression.Constant(1)));
        private string dbg101 = @"checked(p0 += -1)";

        [Fact]
        public void CSharp_ToCSharp_Test101()
        {
            Assert.Equal(dbg101, expr101.ToCSharp());
        }

        private Expression expr102 = Expression.NegateChecked(Expression.Convert(DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Negate, _par1), typeof(int)));
        private string dbg102 = @"/*checked(*/-/*)*/(int)/*dynamic*/-p0";

        [Fact]
        public void CSharp_ToCSharp_Test102()
        {
            Assert.Equal(dbg102, expr102.ToCSharp());
        }

        private Expression expr103 = Expression.NegateChecked(Expression.Convert(DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.NegateChecked, _par1), typeof(int)));
        private string dbg103 = @"checked(-(int)/*dynamic*/-p0)";

        [Fact]
        public void CSharp_ToCSharp_Test103()
        {
            Assert.Equal(dbg103, expr103.ToCSharp());
        }

        private Expression expr104 = Expression.NegateChecked(Expression.Convert(DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Add, _par1, Expression.Constant(1)), typeof(int)));
        private string dbg104 = @"/*checked(*/-/*)*/(int)(p0 /*dynamic*/+ 1)";

        [Fact]
        public void CSharp_ToCSharp_Test104()
        {
            Assert.Equal(dbg104, expr104.ToCSharp());
        }

        private Expression expr105 = Expression.NegateChecked(Expression.Convert(DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.AddChecked, _par1, Expression.Constant(1)), typeof(int)));
        private string dbg105 = @"checked(-(int)(p0 /*dynamic*/+ 1))";

        [Fact]
        public void CSharp_ToCSharp_Test105()
        {
            Assert.Equal(dbg105, expr105.ToCSharp());
        }

        private Expression expr106 = Expression.Negate(Expression.Convert(DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Negate, _par1), typeof(int)));
        private string dbg106 = @"-(int)/*dynamic*/-p0";

        [Fact]
        public void CSharp_ToCSharp_Test106()
        {
            Assert.Equal(dbg106, expr106.ToCSharp());
        }

        private Expression expr107 = Expression.Negate(Expression.Convert(DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.NegateChecked, _par1), typeof(int)));
        private string dbg107 = @"-(int)checked(/*dynamic*/-p0)";

        [Fact]
        public void CSharp_ToCSharp_Test107()
        {
            Assert.Equal(dbg107, expr107.ToCSharp());
        }

        private Expression expr108 = Expression.Negate(Expression.Convert(DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Add, _par1, Expression.Constant(1)), typeof(int)));
        private string dbg108 = @"-(int)(p0 /*dynamic*/+ 1)";

        [Fact]
        public void CSharp_ToCSharp_Test108()
        {
            Assert.Equal(dbg108, expr108.ToCSharp());
        }

        private Expression expr109 = Expression.Negate(Expression.Convert(DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.AddChecked, _par1, Expression.Constant(1)), typeof(int)));
        private string dbg109 = @"-(int)(checked(p0 /*dynamic*/+ 1))";

        [Fact]
        public void CSharp_ToCSharp_Test109()
        {
            Assert.Equal(dbg109, expr109.ToCSharp());
        }

        private Expression expr110 = DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Increment, _par1);
        private string dbg110 = @"p0/*dynamic*/ + 1";

        [Fact]
        public void CSharp_ToCSharp_Test110()
        {
            Assert.Equal(dbg110, expr110.ToCSharp());
        }

        private Expression expr111 = DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Decrement, _par1);
        private string dbg111 = @"p0/*dynamic*/ - 1";

        [Fact]
        public void CSharp_ToCSharp_Test111()
        {
            Assert.Equal(dbg111, expr111.ToCSharp());
        }

        private Expression expr112 = DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.IsTrue, _par1);
        private string dbg112 = @"p0/*dynamic*/ == true";

        [Fact]
        public void CSharp_ToCSharp_Test112()
        {
            Assert.Equal(dbg112, expr112.ToCSharp());
        }

        private Expression expr113 = DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.IsFalse, _par1);
        private string dbg113 = @"p0/*dynamic*/ == false";

        [Fact]
        public void CSharp_ToCSharp_Test113()
        {
            Assert.Equal(dbg113, expr113.ToCSharp());
        }

        private Expression expr114 = DynamicCSharpExpression.DynamicPreIncrementAssign(_par1);
        private string dbg114 = @"/*dynamic*/++p0";

        [Fact]
        public void CSharp_ToCSharp_Test114()
        {
            Assert.Equal(dbg114, expr114.ToCSharp());
        }

        private Expression expr115 = DynamicCSharpExpression.DynamicPreDecrementAssign(_par1);
        private string dbg115 = @"/*dynamic*/--p0";

        [Fact]
        public void CSharp_ToCSharp_Test115()
        {
            Assert.Equal(dbg115, expr115.ToCSharp());
        }

        private Expression expr116 = DynamicCSharpExpression.DynamicPostIncrementAssign(_par1);
        private string dbg116 = @"p0/*dynamic*/++";

        [Fact]
        public void CSharp_ToCSharp_Test116()
        {
            Assert.Equal(dbg116, expr116.ToCSharp());
        }

        private Expression expr117 = DynamicCSharpExpression.DynamicPostDecrementAssign(_par1);
        private string dbg117 = @"p0/*dynamic*/--";

        [Fact]
        public void CSharp_ToCSharp_Test117()
        {
            Assert.Equal(dbg117, expr117.ToCSharp());
        }

        private Expression expr118 = DynamicCSharpExpression.DynamicPreIncrementAssignChecked(_par1);
        private string dbg118 = @"checked(/*dynamic*/++p0)";

        [Fact]
        public void CSharp_ToCSharp_Test118()
        {
            Assert.Equal(dbg118, expr118.ToCSharp());
        }

        private Expression expr119 = DynamicCSharpExpression.DynamicPreDecrementAssignChecked(_par1);
        private string dbg119 = @"checked(/*dynamic*/--p0)";

        [Fact]
        public void CSharp_ToCSharp_Test119()
        {
            Assert.Equal(dbg119, expr119.ToCSharp());
        }

        private Expression expr120 = DynamicCSharpExpression.DynamicPostIncrementAssignChecked(_par1);
        private string dbg120 = @"checked(p0/*dynamic*/++)";

        [Fact]
        public void CSharp_ToCSharp_Test120()
        {
            Assert.Equal(dbg120, expr120.ToCSharp());
        }

        private Expression expr121 = DynamicCSharpExpression.DynamicPostDecrementAssignChecked(_par1);
        private string dbg121 = @"checked(p0/*dynamic*/--)";

        [Fact]
        public void CSharp_ToCSharp_Test121()
        {
            Assert.Equal(dbg121, expr121.ToCSharp());
        }

        private Expression expr122 = CSharpExpression.InterpolatedString();
        private string dbg122 = @"$""""";

        [Fact]
        public void CSharp_ToCSharp_Test122()
        {
            Assert.Equal(dbg122, expr122.ToCSharp());
        }

        private Expression expr123 = CSharpExpression.InterpolatedString(CSharpExpression.InterpolationStringLiteral("foo"));
        private string dbg123 = @"$""foo""";

        [Fact]
        public void CSharp_ToCSharp_Test123()
        {
            Assert.Equal(dbg123, expr123.ToCSharp());
        }

        private Expression expr124 = CSharpExpression.InterpolatedString(CSharpExpression.InterpolationStringLiteral("x = "), CSharpExpression.InterpolationStringInsert(Expression.Constant(42)));
        private string dbg124 = @"$""x = {42}""";

        [Fact]
        public void CSharp_ToCSharp_Test124()
        {
            Assert.Equal(dbg124, expr124.ToCSharp());
        }

        private Expression expr125 = CSharpExpression.InterpolatedString(CSharpExpression.InterpolationStringLiteral("x = "), CSharpExpression.InterpolationStringInsert(Expression.Constant(42), "N"));
        private string dbg125 = @"$""x = {42:N}""";

        [Fact]
        public void CSharp_ToCSharp_Test125()
        {
            Assert.Equal(dbg125, expr125.ToCSharp());
        }

        private Expression expr126 = CSharpExpression.InterpolatedString(CSharpExpression.InterpolationStringLiteral("x = "), CSharpExpression.InterpolationStringInsert(Expression.Constant(42), "N", 2));
        private string dbg126 = @"$""x = {42,2:N}""";

        [Fact]
        public void CSharp_ToCSharp_Test126()
        {
            Assert.Equal(dbg126, expr126.ToCSharp());
        }

        private Expression expr127 = CSharpExpression.InterpolatedString(CSharpExpression.InterpolationStringLiteral("x = "), CSharpExpression.InterpolationStringInsert(Expression.Constant(42), 2));
        private string dbg127 = @"$""x = {42,2}""";

        [Fact]
        public void CSharp_ToCSharp_Test127()
        {
            Assert.Equal(dbg127, expr127.ToCSharp());
        }

        private Expression expr128 = CSharpExpression.Discard(typeof(int));
        private string dbg128 = @"_";

        [Fact]
        public void CSharp_ToCSharp_Test128()
        {
            Assert.Equal(dbg128, expr128.ToCSharp());
        }

        private Expression expr129 = CSharpExpression.TupleLiteral(typeof(ValueTuple<int, int>), Expression.Constant(1), Expression.Constant(2));
        private string dbg129 = @"(1, 2)";

        [Fact]
        public void CSharp_ToCSharp_Test129()
        {
            Assert.Equal(dbg129, expr129.ToCSharp());
        }

        private Expression expr130 = CSharpExpression.TupleLiteral(typeof(ValueTuple<int, int>), new Expression[] { Expression.Constant(1), Expression.Constant(2) }, new[] { "x", "y" });
        private string dbg130 = @"(x: 1, y: 2)";

        [Fact]
        public void CSharp_ToCSharp_Test130()
        {
            Assert.Equal(dbg130, expr130.ToCSharp());
        }

        private Expression expr131 = CSharpExpression.FromEndIndex(Expression.Constant(1));
        private string dbg131 = @"^1";

        [Fact]
        public void CSharp_ToCSharp_Test131()
        {
            Assert.Equal(dbg131, expr131.ToCSharp());
        }

        private Expression expr132 = CSharpExpression.Range(Expression.Constant(1), Expression.Constant(2));
        private string dbg132 = @"(Index)1..(Index)2";

        [Fact]
        public void CSharp_ToCSharp_Test132()
        {
            Assert.Equal(dbg132, expr132.ToCSharp());
        }

        private Expression expr133 = CSharpExpression.Range(Expression.Constant(1), null);
        private string dbg133 = @"(Index)1..";

        [Fact]
        public void CSharp_ToCSharp_Test133()
        {
            Assert.Equal(dbg133, expr133.ToCSharp());
        }

        private Expression expr134 = CSharpExpression.Range(null, Expression.Constant(2));
        private string dbg134 = @"..(Index)2";

        [Fact]
        public void CSharp_ToCSharp_Test134()
        {
            Assert.Equal(dbg134, expr134.ToCSharp());
        }

        private Expression expr135 = CSharpExpression.Range(null, null);
        private string dbg135 = @"..";

        [Fact]
        public void CSharp_ToCSharp_Test135()
        {
            Assert.Equal(dbg135, expr135.ToCSharp());
        }

        private Expression expr136 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.Null());
        private string dbg136 = @"o is (object)null";

        [Fact]
        public void CSharp_ToCSharp_Test136()
        {
            Assert.Equal(dbg136, expr136.ToCSharp());
        }

        private Expression expr137 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.Constant(Expression.Constant(42)));
        private string dbg137 = @"o is 42";

        [Fact]
        public void CSharp_ToCSharp_Test137()
        {
            Assert.Equal(dbg137, expr137.ToCSharp());
        }

        private Expression expr138 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.Var(Expression.Parameter(typeof(int), "x")));
        private string dbg138 = @"o is var x";

        [Fact]
        public void CSharp_ToCSharp_Test138()
        {
            Assert.Equal(dbg138, expr138.ToCSharp());
        }

        private Expression expr139 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.Declaration(Expression.Parameter(typeof(int), "x")));
        private string dbg139 = @"o is int x";

        [Fact]
        public void CSharp_ToCSharp_Test139()
        {
            Assert.Equal(dbg139, expr139.ToCSharp());
        }

        private Expression expr140 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.Type(typeof(int)));
        private string dbg140 = @"o is int";

        [Fact]
        public void CSharp_ToCSharp_Test140()
        {
            Assert.Equal(dbg140, expr140.ToCSharp());
        }

        private Expression expr141 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.Discard());
        private string dbg141 = @"o is _";

        [Fact]
        public void CSharp_ToCSharp_Test141()
        {
            Assert.Equal(dbg141, expr141.ToCSharp());
        }

        private Expression expr142 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.LessThan(Expression.Constant(42)));
        private string dbg142 = @"o is < 42";

        [Fact]
        public void CSharp_ToCSharp_Test142()
        {
            Assert.Equal(dbg142, expr142.ToCSharp());
        }

        private Expression expr143 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.LessThanOrEqual(Expression.Constant(42)));
        private string dbg143 = @"o is <= 42";

        [Fact]
        public void CSharp_ToCSharp_Test143()
        {
            Assert.Equal(dbg143, expr143.ToCSharp());
        }

        private Expression expr144 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.GreaterThan(Expression.Constant(42)));
        private string dbg144 = @"o is > 42";

        [Fact]
        public void CSharp_ToCSharp_Test144()
        {
            Assert.Equal(dbg144, expr144.ToCSharp());
        }

        private Expression expr145 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.GreaterThanOrEqual(Expression.Constant(42)));
        private string dbg145 = @"o is >= 42";

        [Fact]
        public void CSharp_ToCSharp_Test145()
        {
            Assert.Equal(dbg145, expr145.ToCSharp());
        }

        private Expression expr146 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.Not(CSharpPattern.Null()));
        private string dbg146 = @"o is not (object)null";

        [Fact]
        public void CSharp_ToCSharp_Test146()
        {
            Assert.Equal(dbg146, expr146.ToCSharp());
        }

        private Expression expr147 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.And(CSharpPattern.Constant(Expression.Constant(42)), CSharpPattern.Constant(Expression.Constant(43))));
        private string dbg147 = @"o is 42 and 43";

        [Fact]
        public void CSharp_ToCSharp_Test147()
        {
            Assert.Equal(dbg147, expr147.ToCSharp());
        }

        private Expression expr148 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.Or(CSharpPattern.Constant(Expression.Constant(42)), CSharpPattern.Constant(Expression.Constant(43))));
        private string dbg148 = @"o is 42 or 43";

        [Fact]
        public void CSharp_ToCSharp_Test148()
        {
            Assert.Equal(dbg148, expr148.ToCSharp());
        }

        private Expression expr149 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.Property(typeof(string), CSharpPattern.PropertySubpattern(CSharpPattern.Constant(Expression.Constant(1)), typeof(string).GetProperty("Length"))));
        private string dbg149 = @"o is string { Length: 1 }";

        [Fact]
        public void CSharp_ToCSharp_Test149()
        {
            Assert.Equal(dbg149, expr149.ToCSharp());
        }

        private Expression expr150 = CSharpExpression.IsPattern(Expression.Parameter(typeof(object), "o"), CSharpPattern.Property(typeof(KeyValuePair<int, int>), CSharpPattern.PropertySubpattern(CSharpPattern.Constant(Expression.Constant(1)), typeof(KeyValuePair<int, int>).GetProperty("Key")), CSharpPattern.PropertySubpattern(CSharpPattern.Constant(Expression.Constant(2)), typeof(KeyValuePair<int, int>).GetProperty("Value"))));
        private string dbg150 = @"o is KeyValuePair<int, int> { Key: 1, Value: 2 }";

        [Fact]
        public void CSharp_ToCSharp_Test150()
        {
            Assert.Equal(dbg150, expr150.ToCSharp());
        }

        private Expression expr151 = CSharpExpression.IsPattern(Expression.Parameter(typeof(int[]), "xs"), CSharpPattern.List(typeof(int[]), CSharpPattern.Constant(Expression.Constant(1)), CSharpPattern.Constant(Expression.Constant(2))));
        private string dbg151 = @"xs is [1, 2]";

        [Fact]
        public void CSharp_ToCSharp_Test151()
        {
            Assert.Equal(dbg151, expr151.ToCSharp());
        }

        private Expression expr152 = CSharpExpression.IsPattern(Expression.Parameter(typeof(int[]), "xs"), CSharpPattern.List(Expression.Parameter(typeof(int[]), "ys"), CSharpPattern.Constant(Expression.Constant(1)), CSharpPattern.Constant(Expression.Constant(2))));
        private string dbg152 = @"xs is [1, 2] ys";

        [Fact]
        public void CSharp_ToCSharp_Test152()
        {
            Assert.Equal(dbg152, expr152.ToCSharp());
        }

    }
}
