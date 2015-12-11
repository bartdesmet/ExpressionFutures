// Prototyping extended expression trees for C#.
//
// bartde - December 2015

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
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
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

        [TestMethod]
        public void CSharp_ToCSharp_Test0()
        {
            Assert.AreEqual(dbg0, expr0.ToCSharp());
        }

        private Expression expr1 = /* BUG */ CSharpExpression.PostIncrementAssign(Expression.Parameter(typeof(int)), typeof(Math).GetMethod("Abs", new[] { typeof(int) }));
        private string dbg1 = @"p0 = Math.Abs(p0)";

        [TestMethod]
        public void CSharp_ToCSharp_Test1()
        {
            Assert.AreEqual(dbg1, expr1.ToCSharp());
        }

        private Expression expr2 = /* BUG */ CSharpExpression.PostDecrementAssign(Expression.Parameter(typeof(int)), typeof(Math).GetMethod("Abs", new[] { typeof(int) }));
        private string dbg2 = @"p0 = Math.Abs(p0)";

        [TestMethod]
        public void CSharp_ToCSharp_Test2()
        {
            Assert.AreEqual(dbg2, expr2.ToCSharp());
        }

        private Expression expr3 = CSharpExpression.PreDecrementCheckedAssign(Expression.Parameter(typeof(int)));
        private string dbg3 = @"checked(--p0)";

        [TestMethod]
        public void CSharp_ToCSharp_Test3()
        {
            Assert.AreEqual(dbg3, expr3.ToCSharp());
        }

        private Expression expr4 = CSharpExpression.AddAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg4 = @"p0 += 1";

        [TestMethod]
        public void CSharp_ToCSharp_Test4()
        {
            Assert.AreEqual(dbg4, expr4.ToCSharp());
        }

        private Expression expr5 = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 1, 1 }, Expression.Constant(42));
        private string dbg5 = @"new int[1, 1] { { 42 } }";

        [TestMethod]
        public void CSharp_ToCSharp_Test5()
        {
            Assert.AreEqual(dbg5, expr5.ToCSharp());
        }

        private Expression expr6 = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 1, 2 }, Expression.Constant(42), Expression.Constant(43));
        private string dbg6 = @"new int[1, 2] { { 42, 43 } }";

        [TestMethod]
        public void CSharp_ToCSharp_Test6()
        {
            Assert.AreEqual(dbg6, expr6.ToCSharp());
        }

        private Expression expr7 = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 1 }, Expression.Constant(42), Expression.Constant(43));
        private string dbg7 = @"new int[2, 1] { { 42 }, { 43 } }";

        [TestMethod]
        public void CSharp_ToCSharp_Test7()
        {
            Assert.AreEqual(dbg7, expr7.ToCSharp());
        }

        private Expression expr8 = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 2 }, Expression.Constant(42), Expression.Constant(43), Expression.Constant(44), Expression.Constant(45));
        private string dbg8 = @"new int[2, 2] { { 42, 43 }, { 44, 45 } }";

        [TestMethod]
        public void CSharp_ToCSharp_Test8()
        {
            Assert.AreEqual(dbg8, expr8.ToCSharp());
        }

        private Expression expr9 = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3, 5 }, Enumerable.Range(0, 2 * 3 * 5).Select(i => Expression.Constant(i)));
        private string dbg9 = @"new int[2, 3, 5] { { { 0, 1, 2, 3, 4 }, { 5, 6, 7, 8, 9 }, { 10, 11, 12, 13, 14 } }, { { 15, 16, 17, 18, 19 }, { 20, 21, 22, 23, 24 }, { 25, 26, 27, 28, 29 } } }";

        [TestMethod]
        public void CSharp_ToCSharp_Test9()
        {
            Assert.AreEqual(dbg9, expr9.ToCSharp());
        }

        private Expression expr10 = Expression.ArrayAccess(CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 1, 1 }, Expression.Constant(42)), Expression.Constant(0), Expression.Constant(0));
        private string dbg10 = @"new int[1, 1] { { 42 } }[0, 0]";

        [TestMethod]
        public void CSharp_ToCSharp_Test10()
        {
            Assert.AreEqual(dbg10, expr10.ToCSharp());
        }

        private Expression expr11 = CSharpExpression.Await(Expression.Default(typeof(Task<int>)));
        private string dbg11 = @"await default(Task<int>)";

        [TestMethod]
        public void CSharp_ToCSharp_Test11()
        {
            Assert.AreEqual(dbg11, expr11.ToCSharp());
        }

        private Expression expr12 = DynamicCSharpExpression.DynamicAwait(Expression.Default(typeof(Task<int>)), typeof(object));
        private string dbg12 = @"await default(Task<int>)";

        [TestMethod]
        public void CSharp_ToCSharp_Test12()
        {
            Assert.AreEqual(dbg12, expr12.ToCSharp());
        }

        private Expression expr13 = CSharpExpression.AsyncLambda<Func<Task<int>>>(CSharpExpression.Await(Expression.Default(typeof(Task<int>))));
        private string dbg13 = @"async () => await default(Task<int>)";

        [TestMethod]
        public void CSharp_ToCSharp_Test13()
        {
            Assert.AreEqual(dbg13, expr13.ToCSharp());
        }

        private Expression expr14 = CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Block(typeof(int), CSharpExpression.Await(Expression.Default(typeof(Task<int>)))));
        private string dbg14 = @"async () =>
{
    return await default(Task<int>);
}
";

        [TestMethod]
        public void CSharp_ToCSharp_Test14()
        {
            Assert.AreEqual(dbg14, expr14.ToCSharp());
        }

        private Expression expr15 = CSharpExpression.AsyncLambda<Func<Task<int>>>(CSharpExpression.Block(new[] { CSharpExpression.Await(Expression.Default(typeof(Task<int>))) }, _lbl3));
        private string dbg15 = @"async () =>
{
    return await default(Task<int>);
}
";

        [TestMethod]
        public void CSharp_ToCSharp_Test15()
        {
            Assert.AreEqual(dbg15, expr15.ToCSharp());
        }

        private Expression expr16 = CSharpExpression.AsyncLambda<Func<bool, string, Task<int>>>(CSharpExpression.Await(Expression.Default(typeof(Task<int>))), Expression.Parameter(typeof(bool), "b"), Expression.Parameter(typeof(string), "s"));
        private string dbg16 = @"async (bool b, string s) => await default(Task<int>)";

        [TestMethod]
        public void CSharp_ToCSharp_Test16()
        {
            Assert.AreEqual(dbg16, expr16.ToCSharp());
        }

        private Expression expr17 = CSharpExpression.Call(typeof(Math).GetMethod("Abs", new[] { typeof(int) }), CSharpExpression.Bind(typeof(Math).GetMethod("Abs", new[] { typeof(int) }).GetParameters()[0], Expression.Constant(42)));
        private string dbg17 = @"Math.Abs(42)";

        [TestMethod]
        public void CSharp_ToCSharp_Test17()
        {
            Assert.AreEqual(dbg17, expr17.ToCSharp());
        }

        private Expression expr18 = CSharpExpression.Call(Expression.Default(typeof(string)), typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) }), CSharpExpression.Bind(typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) }).GetParameters()[1], Expression.Constant(1)), CSharpExpression.Bind(typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) }).GetParameters()[0], Expression.Constant(2)));
        private string dbg18 = @"default(string).Substring(length: 1, startIndex: 2)";

        [TestMethod]
        public void CSharp_ToCSharp_Test18()
        {
            Assert.AreEqual(dbg18, expr18.ToCSharp());
        }

        private Expression expr19 = Expression.Property(CSharpExpression.Call(Expression.Default(typeof(string)), typeof(string).GetMethod("Substring", new[] { typeof(int) }), CSharpExpression.Bind(typeof(string).GetMethod("Substring", new[] { typeof(int) }).GetParameters()[0], Expression.Constant(42))), "Length");
        private string dbg19 = @"default(string).Substring(42).Length";

        [TestMethod]
        public void CSharp_ToCSharp_Test19()
        {
            Assert.AreEqual(dbg19, expr19.ToCSharp());
        }

        private Expression expr20 = CSharpExpression.Call(null, typeof(Activator).GetMethod("CreateInstance", new Type[0]).MakeGenericMethod(typeof(string)));
        private string dbg20 = @"Activator.CreateInstance<string>()";

        [TestMethod]
        public void CSharp_ToCSharp_Test20()
        {
            Assert.AreEqual(dbg20, expr20.ToCSharp());
        }

        private Expression expr21 = CSharpExpression.Invoke(Expression.Default(typeof(Action<int>)), CSharpExpression.Bind(typeof(Action<int>).GetMethod("Invoke").GetParameters()[0], Expression.Constant(42)));
        private string dbg21 = @"default(Action<int>)(42)";

        [TestMethod]
        public void CSharp_ToCSharp_Test21()
        {
            Assert.AreEqual(dbg21, expr21.ToCSharp());
        }

        private Expression expr22 = CSharpExpression.New(typeof(TimeSpan).GetConstructor(new[] { typeof(long) }), CSharpExpression.Bind(typeof(TimeSpan).GetConstructor(new[] { typeof(long) }).GetParameters()[0], Expression.Constant(42L)));
        private string dbg22 = @"new TimeSpan(42L)";

        [TestMethod]
        public void CSharp_ToCSharp_Test22()
        {
            Assert.AreEqual(dbg22, expr22.ToCSharp());
        }

        private Expression expr23 = Expression.Property(CSharpExpression.New(typeof(TimeSpan).GetConstructor(new[] { typeof(long) }), CSharpExpression.Bind(typeof(TimeSpan).GetConstructor(new[] { typeof(long) }).GetParameters()[0], Expression.Constant(42L))), "Ticks");
        private string dbg23 = @"new TimeSpan(42L).Ticks";

        [TestMethod]
        public void CSharp_ToCSharp_Test23()
        {
            Assert.AreEqual(dbg23, expr23.ToCSharp());
        }

        private Expression expr24 = CSharpExpression.Index(Expression.Default(typeof(List<int>)), typeof(List<int>).GetProperty("Item"), CSharpExpression.Bind(typeof(List<int>).GetProperty("Item").GetIndexParameters()[0], Expression.Constant(42)));
        private string dbg24 = @"default(List<int>)[42]";

        [TestMethod]
        public void CSharp_ToCSharp_Test24()
        {
            Assert.AreEqual(dbg24, expr24.ToCSharp());
        }

        private Expression expr25 = CSharpExpression.ConditionalArrayIndex(Expression.Default(typeof(string[])), Expression.Constant(42));
        private string dbg25 = @"default(string[])?[42]";

        [TestMethod]
        public void CSharp_ToCSharp_Test25()
        {
            Assert.AreEqual(dbg25, expr25.ToCSharp());
        }

        private Expression expr26 = CSharpExpression.ConditionalCall(Expression.Default(typeof(string)), typeof(string).GetMethod("Substring", new[] { typeof(int) }), CSharpExpression.Bind(typeof(string).GetMethod("Substring", new[] { typeof(int) }).GetParameters()[0], Expression.Constant(42)));
        private string dbg26 = @"default(string)?.Substring(42)";

        [TestMethod]
        public void CSharp_ToCSharp_Test26()
        {
            Assert.AreEqual(dbg26, expr26.ToCSharp());
        }

        private Expression expr27 = CSharpExpression.ConditionalIndex(Expression.Default(typeof(List<int>)), typeof(List<int>).GetProperty("Item"), CSharpExpression.Bind(typeof(List<int>).GetProperty("Item").GetIndexParameters()[0], Expression.Constant(42)));
        private string dbg27 = @"default(List<int>)?[42]";

        [TestMethod]
        public void CSharp_ToCSharp_Test27()
        {
            Assert.AreEqual(dbg27, expr27.ToCSharp());
        }

        private Expression expr28 = CSharpExpression.ConditionalProperty(Expression.Default(typeof(string)), typeof(string).GetProperty("Length"));
        private string dbg28 = @"default(string)?.Length";

        [TestMethod]
        public void CSharp_ToCSharp_Test28()
        {
            Assert.AreEqual(dbg28, expr28.ToCSharp());
        }

        private Expression expr29 = CSharpExpression.ConditionalAccess(Expression.Default(typeof(string)), CSharpExpression.ConditionalReceiver(typeof(string)), Expression.Property(CSharpExpression.ConditionalReceiver(typeof(string)), "Length"));
        private string dbg29 = @"default(string)?.Length";

        [TestMethod]
        public void CSharp_ToCSharp_Test29()
        {
            Assert.AreEqual(dbg29, expr29.ToCSharp());
        }

        private Expression expr30 = DynamicCSharpExpression.DynamicAdd(Expression.Constant(1), Expression.Constant(2));
        private string dbg30 = @"1 /*dynamic*/+ 2";

        [TestMethod]
        public void CSharp_ToCSharp_Test30()
        {
            Assert.AreEqual(dbg30, expr30.ToCSharp());
        }

        private Expression expr31 = DynamicCSharpExpression.DynamicAdd(DynamicCSharpExpression.DynamicArgument(Expression.Constant(1)), DynamicCSharpExpression.DynamicArgument(Expression.Constant(2)), CSharpBinderFlags.CheckedContext);
        private string dbg31 = @"checked(1 /*dynamic*/+ 2)";

        [TestMethod]
        public void CSharp_ToCSharp_Test31()
        {
            Assert.AreEqual(dbg31, expr31.ToCSharp());
        }

        private Expression expr32 = DynamicCSharpExpression.DynamicNegate(Expression.Constant(1));
        private string dbg32 = @"/*dynamic*/-1";

        [TestMethod]
        public void CSharp_ToCSharp_Test32()
        {
            Assert.AreEqual(dbg32, expr32.ToCSharp());
        }

        private Expression expr33 = DynamicCSharpExpression.DynamicConvert(Expression.Constant(1), typeof(int), CSharpBinderFlags.ConvertExplicit);
        private string dbg33 = @"/*dynamic*/(int)1";

        [TestMethod]
        public void CSharp_ToCSharp_Test33()
        {
            Assert.AreEqual(dbg33, expr33.ToCSharp());
        }

        private Expression expr34 = DynamicCSharpExpression.DynamicGetMember(Expression.Default(typeof(string)), "Length");
        private string dbg34 = @"default(string)/*dynamic*/.Length";

        [TestMethod]
        public void CSharp_ToCSharp_Test34()
        {
            Assert.AreEqual(dbg34, expr34.ToCSharp());
        }

        private Expression expr35 = DynamicCSharpExpression.DynamicGetIndex(Expression.Default(typeof(List<int>)), Expression.Constant(1));
        private string dbg35 = @"default(List<int>)/*dynamic*/[1]";

        [TestMethod]
        public void CSharp_ToCSharp_Test35()
        {
            Assert.AreEqual(dbg35, expr35.ToCSharp());
        }

        private Expression expr36 = DynamicCSharpExpression.DynamicInvoke(Expression.Default(typeof(Action<int>)), Expression.Constant(1));
        private string dbg36 = @"default(Action<int>)/*dynamic*/(1)";

        [TestMethod]
        public void CSharp_ToCSharp_Test36()
        {
            Assert.AreEqual(dbg36, expr36.ToCSharp());
        }

        private Expression expr37 = DynamicCSharpExpression.DynamicInvokeMember(typeof(Math), "Abs", Expression.Constant(1));
        private string dbg37 = @"Math/*dynamic*/.Abs(1)";

        [TestMethod]
        public void CSharp_ToCSharp_Test37()
        {
            Assert.AreEqual(dbg37, expr37.ToCSharp());
        }

        private Expression expr38 = DynamicCSharpExpression.DynamicInvokeMember(typeof(Activator), "CreateInstance", new[] { typeof(int) });
        private string dbg38 = @"Activator/*dynamic*/.CreateInstance<int>()";

        [TestMethod]
        public void CSharp_ToCSharp_Test38()
        {
            Assert.AreEqual(dbg38, expr38.ToCSharp());
        }

        private Expression expr39 = DynamicCSharpExpression.DynamicInvokeMember(Expression.Default(typeof(string)), "Substring", Expression.Constant(1));
        private string dbg39 = @"default(string)/*dynamic*/.Substring(1)";

        [TestMethod]
        public void CSharp_ToCSharp_Test39()
        {
            Assert.AreEqual(dbg39, expr39.ToCSharp());
        }

        private Expression expr40 = DynamicCSharpExpression.DynamicInvokeMember(Expression.Default(typeof(string)), "Substring", new[] { DynamicCSharpExpression.DynamicArgument(Expression.Constant(1), "startIndex") });
        private string dbg40 = @"default(string)/*dynamic*/.Substring(1)";

        [TestMethod]
        public void CSharp_ToCSharp_Test40()
        {
            Assert.AreEqual(dbg40, expr40.ToCSharp());
        }

        private Expression expr41 = DynamicCSharpExpression.DynamicInvokeMember(Expression.Default(typeof(string)), "Substring", new[] { DynamicCSharpExpression.DynamicArgument(Expression.Constant(1), "startIndex", CSharpArgumentInfoFlags.NamedArgument) });
        private string dbg41 = @"default(string)/*dynamic*/.Substring(startIndex: 1)";

        [TestMethod]
        public void CSharp_ToCSharp_Test41()
        {
            Assert.AreEqual(dbg41, expr41.ToCSharp());
        }

        private Expression expr42 = DynamicCSharpExpression.DynamicInvokeMember(Expression.Default(typeof(string)), "Substring", new[] { DynamicCSharpExpression.DynamicArgument(Expression.Constant(1), "startIndex", CSharpArgumentInfoFlags.NamedArgument), DynamicCSharpExpression.DynamicArgument(Expression.Constant(2), "length", CSharpArgumentInfoFlags.NamedArgument) });
        private string dbg42 = @"default(string)/*dynamic*/.Substring(startIndex: 1, length: 2)";

        [TestMethod]
        public void CSharp_ToCSharp_Test42()
        {
            Assert.AreEqual(dbg42, expr42.ToCSharp());
        }

        private Expression expr43 = DynamicCSharpExpression.DynamicInvokeConstructor(typeof(TimeSpan), Expression.Constant(1L));
        private string dbg43 = @"/*dynamic*/new TimeSpan(1L)";

        [TestMethod]
        public void CSharp_ToCSharp_Test43()
        {
            Assert.AreEqual(dbg43, expr43.ToCSharp());
        }

        private Expression expr44 = CSharpExpression.Block(new Expression[] { Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")) }, Expression.Label());
        private string dbg44 = @"{
    Console.WriteLine(""body"");
}";

        [TestMethod]
        public void CSharp_ToCSharp_Test44()
        {
            Assert.AreEqual(dbg44, expr44.ToCSharp());
        }

        private Expression expr45 = CSharpExpression.Block(new Expression[] { Expression.Constant(42) }, _lbl3);
        private string dbg45 = @"{
    return 42;
}";

        [TestMethod]
        public void CSharp_ToCSharp_Test45()
        {
            Assert.AreEqual(dbg45, expr45.ToCSharp());
        }

        private Expression expr46 = CSharpExpression.Block(new[] { Expression.Parameter(typeof(int)) }, new Expression[] { Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("stmt1")), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("stmt2")), Expression.Return(_lbl1) }, _lbl1);
        private string dbg46 = @"{
    int p0 /*(null)*/;
    Console.WriteLine(""stmt1"");
    Console.WriteLine(""stmt2"");
    return;
}";

        [TestMethod]
        public void CSharp_ToCSharp_Test46()
        {
            Assert.AreEqual(dbg46, expr46.ToCSharp());
        }

        private Expression expr47 = CSharpExpression.Block(new[] { Expression.Parameter(typeof(int)) }, new Expression[] { Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("stmt1")), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("stmt2")), Expression.Return(_lbl3, Expression.Constant(1)) }, _lbl3);
        private string dbg47 = @"{
    int p0 /*(null)*/;
    Console.WriteLine(""stmt1"");
    Console.WriteLine(""stmt2"");
    return 1;
}";

        [TestMethod]
        public void CSharp_ToCSharp_Test47()
        {
            Assert.AreEqual(dbg47, expr47.ToCSharp());
        }

        private Expression expr48 = Expression.Lambda(CSharpExpression.Block(new Expression[] { Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")) }, Expression.Label()));
        private string dbg48 = @"() =>
{
    Console.WriteLine(""body"");
}
";

        [TestMethod]
        public void CSharp_ToCSharp_Test48()
        {
            Assert.AreEqual(dbg48, expr48.ToCSharp());
        }

        private Expression expr49 = CSharpStatement.Do(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")), Expression.Constant(true));
        private string dbg49 = @"do
{
    Console.WriteLine(""body"");
}
while (true);";

        [TestMethod]
        public void CSharp_ToCSharp_Test49()
        {
            Assert.AreEqual(dbg49, expr49.ToCSharp());
        }

        private Expression expr50 = CSharpStatement.While(Expression.Constant(true), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg50 = @"while (true)
{
    Console.WriteLine(""body"");
}
";

        [TestMethod]
        public void CSharp_ToCSharp_Test50()
        {
            Assert.AreEqual(dbg50, expr50.ToCSharp());
        }

        private Expression expr51 = CSharpStatement.For(new ParameterExpression[0], new Expression[0], null, new Expression[0], Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg51 = @"for (;;)
{
    Console.WriteLine(""body"");
}
";

        [TestMethod]
        public void CSharp_ToCSharp_Test51()
        {
            Assert.AreEqual(dbg51, expr51.ToCSharp());
        }

        private Expression expr52 = CSharpStatement.For(new[] { _par1 }, new[] { Expression.Assign(_par1, Expression.Constant(1)) }, Expression.LessThan(Expression.Parameter(typeof(int)), Expression.Constant(10)), new[] { Expression.PostIncrementAssign(_par1) }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg52 = @"for (int p0 = 1; p1 < 10; p0++)
{
    Console.WriteLine(""body"");
}
";

        [TestMethod]
        public void CSharp_ToCSharp_Test52()
        {
            Assert.AreEqual(dbg52, expr52.ToCSharp());
        }

        private Expression expr53 = CSharpStatement.For(new[] { _par1 }, new[] { Expression.Assign(_par1, Expression.Constant(1)), Expression.Assign(_par1, Expression.Constant(2)) }, Expression.LessThan(Expression.Parameter(typeof(int)), Expression.Constant(10)), new[] { Expression.PostIncrementAssign(_par1) }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg53 = @"{
    int p0 /*(null)*/;
    for (p0 = 1, p0 = 2; p1 < 10; p0++)
    {
        Console.WriteLine(""body"");
    }
    
}";

        [TestMethod]
        public void CSharp_ToCSharp_Test53()
        {
            Assert.AreEqual(dbg53, expr53.ToCSharp());
        }

        private Expression expr54 = CSharpStatement.For(new[] { _par1 }, new[] { CSharpExpression.Assign(_par1, Expression.Constant(1)) }, Expression.LessThan(Expression.Parameter(typeof(int)), Expression.Constant(10)), new[] { Expression.PostIncrementAssign(_par1) }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg54 = @"for (int p0 = 1; p1 < 10; p0++)
{
    Console.WriteLine(""body"");
}
";

        [TestMethod]
        public void CSharp_ToCSharp_Test54()
        {
            Assert.AreEqual(dbg54, expr54.ToCSharp());
        }

        private Expression expr55 = CSharpStatement.For(new[] { _par1, _par2 }, new[] { Expression.Assign(_par1, Expression.Constant(0)), Expression.Assign(_par2, Expression.Constant(10)) }, Expression.LessThan(_par1, _par2), new[] { Expression.PostIncrementAssign(_par1), Expression.PreDecrementAssign(_par2) }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg55 = @"for (int p0 = 0, p1 = 10; p0 < p1; p0++, --p1)
{
    Console.WriteLine(""body"");
}
";

        [TestMethod]
        public void CSharp_ToCSharp_Test55()
        {
            Assert.AreEqual(dbg55, expr55.ToCSharp());
        }

        private Expression expr56 = CSharpStatement.For(new[] { _par1 }, new Expression[0], Expression.LessThan(Expression.Parameter(typeof(int)), Expression.Constant(10)), new[] { Expression.PostIncrementAssign(_par1) }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg56 = @"{
    int p0 /*(null)*/;
    for (; p1 < 10; p0++)
    {
        Console.WriteLine(""body"");
    }
    
}";

        [TestMethod]
        public void CSharp_ToCSharp_Test56()
        {
            Assert.AreEqual(dbg56, expr56.ToCSharp());
        }

        private Expression expr57 = CSharpStatement.For(new[] { _par1, _par3 }, new[] { Expression.Assign(_par1, Expression.Constant(0)), Expression.Assign(_par3, Expression.Constant(10L)) }, Expression.LessThan(_par1, _par2), new[] { Expression.PostIncrementAssign(_par1), Expression.PreDecrementAssign(_par2) }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg57 = @"{
    int p0 /*(null)*/;
    long p1 /*(null)*/;
    for (p0 = 0, p1 = 10L; p0 < p2; p0++, --p2)
    {
        Console.WriteLine(""body"");
    }
    
}";

        [TestMethod]
        public void CSharp_ToCSharp_Test57()
        {
            Assert.AreEqual(dbg57, expr57.ToCSharp());
        }

        private Expression expr58 = CSharpStatement.ForEach(Expression.Parameter(typeof(int)), Expression.Default(typeof(int[])), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg58 = @"foreach (int p0 /*(null)*/ in default(int[]))
{
    Console.WriteLine(""body"");
}
";

        [TestMethod]
        public void CSharp_ToCSharp_Test58()
        {
            Assert.AreEqual(dbg58, expr58.ToCSharp());
        }

        private Expression expr59 = CSharpStatement.Switch(Expression.Default(typeof(int)), Expression.Label("break"));
        private string dbg59 = @"switch (default(int))
{
    
}";

        [TestMethod]
        public void CSharp_ToCSharp_Test59()
        {
            Assert.AreEqual(dbg59, expr59.ToCSharp());
        }

        private Expression expr60 = CSharpStatement.Switch(Expression.Default(typeof(int)), Expression.Label("break"), CSharpStatement.SwitchCase(new object[] { 1, 2 }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("case 1,2"))), CSharpStatement.SwitchCaseDefault(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("default"))));
        private string dbg60 = @"switch (default(int))
{
    case 1:
    case 2:
        Console.WriteLine(""case 1,2"");
        break;
    default:
        Console.WriteLine(""default"");
        break;
}";

        [TestMethod]
        public void CSharp_ToCSharp_Test60()
        {
            Assert.AreEqual(dbg60, expr60.ToCSharp());
        }

        private Expression expr61 = CSharpStatement.Switch(Expression.Default(typeof(string)), _lbl1, new[] { Expression.Parameter(typeof(int)) }, new[] { CSharpStatement.SwitchCase(new object[] { "bar", "foo", "this is a \"quoted\" string", null, CSharpStatement.SwitchCaseDefaultValue }, Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")), Expression.Break(_lbl1)) });
        private string dbg61 = @"{
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

        [TestMethod]
        public void CSharp_ToCSharp_Test61()
        {
            Assert.AreEqual(dbg61, expr61.ToCSharp());
        }

        private Expression expr62 = CSharpStatement.Switch(Expression.Default(typeof(string)), _lbl1, new[] { _par1 }, new[] { CSharpStatement.SwitchCase(new object[] { "bar" }, Expression.Block(typeof(void), Expression.Assign(_par1, Expression.Constant(0))), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")), Expression.Break(_lbl1)) });
        private string dbg62 = @"switch (default(string))
{
    case ""bar"":
        int p0 /*(null)*/;
        {
            p0 = 0;
        }
        Console.WriteLine(""body"");
        break;
}";

        [TestMethod]
        public void CSharp_ToCSharp_Test62()
        {
            Assert.AreEqual(dbg62, expr62.ToCSharp());
        }

        private Expression expr63 = CSharpStatement.Switch(Expression.Default(typeof(string)), _lbl1, new[] { _par1 }, new[] { CSharpStatement.SwitchCase(new object[] { "bar" }, Expression.Block(typeof(void), new[] { _par2 }, Expression.Assign(_par1, _par2)), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("bar")), Expression.Break(_lbl1)), CSharpStatement.SwitchCase(new object[] { "foo" }, Expression.Block(typeof(void), Expression.Assign(_par1, Expression.Constant(1))), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("foo")), Expression.Break(_lbl1)) });
        private string dbg63 = @"{
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

        [TestMethod]
        public void CSharp_ToCSharp_Test63()
        {
            Assert.AreEqual(dbg63, expr63.ToCSharp());
        }

        private Expression expr64 = CSharpStatement.GotoLabel(Expression.Label());
        private string dbg64 = @"goto L0;";

        [TestMethod]
        public void CSharp_ToCSharp_Test64()
        {
            Assert.AreEqual(dbg64, expr64.ToCSharp());
        }

        private Expression expr65 = CSharpStatement.GotoCase(1);
        private string dbg65 = @"goto case 1;";

        [TestMethod]
        public void CSharp_ToCSharp_Test65()
        {
            Assert.AreEqual(dbg65, expr65.ToCSharp());
        }

        private Expression expr66 = CSharpStatement.GotoCase(null);
        private string dbg66 = @"goto case null;";

        [TestMethod]
        public void CSharp_ToCSharp_Test66()
        {
            Assert.AreEqual(dbg66, expr66.ToCSharp());
        }

        private Expression expr67 = CSharpStatement.GotoDefault();
        private string dbg67 = @"goto default;";

        [TestMethod]
        public void CSharp_ToCSharp_Test67()
        {
            Assert.AreEqual(dbg67, expr67.ToCSharp());
        }

        private Expression expr68 = CSharpStatement.Lock(Expression.Default(typeof(object)), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg68 = @"lock (default(object))
{
    Console.WriteLine(""body"");
}
";

        [TestMethod]
        public void CSharp_ToCSharp_Test68()
        {
            Assert.AreEqual(dbg68, expr68.ToCSharp());
        }

        private Expression expr69 = CSharpStatement.Using(Expression.Default(typeof(IDisposable)), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg69 = @"using (default(IDisposable))
{
    Console.WriteLine(""body"");
}
";

        [TestMethod]
        public void CSharp_ToCSharp_Test69()
        {
            Assert.AreEqual(dbg69, expr69.ToCSharp());
        }

        private Expression expr70 = CSharpStatement.Using(Expression.Parameter(typeof(IDisposable)), Expression.Default(typeof(IDisposable)), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("body")));
        private string dbg70 = @"using (IDisposable p0 /*(null)*/ = default(IDisposable))
{
    Console.WriteLine(""body"");
}
";

        [TestMethod]
        public void CSharp_ToCSharp_Test70()
        {
            Assert.AreEqual(dbg70, expr70.ToCSharp());
        }

        private Expression expr71 = Expression.AddChecked(CSharpExpression.PostIncrementAssign(_par1), Expression.Constant(1));
        private string dbg71 = @"p0++ /*checked(*/+/*)*/ 1";

        [TestMethod]
        public void CSharp_ToCSharp_Test71()
        {
            Assert.AreEqual(dbg71, expr71.ToCSharp());
        }

        private Expression expr72 = Expression.AddChecked(CSharpExpression.PostIncrementCheckedAssign(_par1), Expression.Constant(1));
        private string dbg72 = @"checked(p0++ + 1)";

        [TestMethod]
        public void CSharp_ToCSharp_Test72()
        {
            Assert.AreEqual(dbg72, expr72.ToCSharp());
        }

        private Expression expr73 = Expression.NegateChecked(CSharpExpression.PostIncrementAssign(_par1));
        private string dbg73 = @"/*checked(*/-/*)*/p0++";

        [TestMethod]
        public void CSharp_ToCSharp_Test73()
        {
            Assert.AreEqual(dbg73, expr73.ToCSharp());
        }

        private Expression expr74 = Expression.NegateChecked(CSharpExpression.PostIncrementCheckedAssign(_par1));
        private string dbg74 = @"checked(-p0++)";

        [TestMethod]
        public void CSharp_ToCSharp_Test74()
        {
            Assert.AreEqual(dbg74, expr74.ToCSharp());
        }

        private Expression expr75 = Expression.NegateChecked(CSharpExpression.AddAssign(_par1, Expression.Constant(1)));
        private string dbg75 = @"/*checked(*/-/*)*/(p0 += 1)";

        [TestMethod]
        public void CSharp_ToCSharp_Test75()
        {
            Assert.AreEqual(dbg75, expr75.ToCSharp());
        }

        private Expression expr76 = Expression.NegateChecked(CSharpExpression.AddAssignChecked(_par1, Expression.Constant(1)));
        private string dbg76 = @"checked(-(p0 += 1))";

        [TestMethod]
        public void CSharp_ToCSharp_Test76()
        {
            Assert.AreEqual(dbg76, expr76.ToCSharp());
        }

        private Expression expr77 = CSharpExpression.AddAssignChecked(_par1, Expression.Negate(Expression.Constant(1)));
        private string dbg77 = @"p0 /*checked(*/+=/*)*/ -(1)";

        [TestMethod]
        public void CSharp_ToCSharp_Test77()
        {
            Assert.AreEqual(dbg77, expr77.ToCSharp());
        }

        private Expression expr78 = CSharpExpression.AddAssignChecked(_par1, Expression.NegateChecked(Expression.Constant(1)));
        private string dbg78 = @"checked(p0 += -1)";

        [TestMethod]
        public void CSharp_ToCSharp_Test78()
        {
            Assert.AreEqual(dbg78, expr78.ToCSharp());
        }

        private Expression expr79 = Expression.NegateChecked(Expression.Convert(DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Negate, _par1), typeof(int)));
        private string dbg79 = @"/*checked(*/-/*)*/(int)/*dynamic*/-p0";

        [TestMethod]
        public void CSharp_ToCSharp_Test79()
        {
            Assert.AreEqual(dbg79, expr79.ToCSharp());
        }

        private Expression expr80 = Expression.NegateChecked(Expression.Convert(DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.NegateChecked, _par1), typeof(int)));
        private string dbg80 = @"checked(-(int)/*dynamic*/-p0)";

        [TestMethod]
        public void CSharp_ToCSharp_Test80()
        {
            Assert.AreEqual(dbg80, expr80.ToCSharp());
        }

        private Expression expr81 = Expression.NegateChecked(Expression.Convert(DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Add, _par1, Expression.Constant(1)), typeof(int)));
        private string dbg81 = @"/*checked(*/-/*)*/(int)(p0 /*dynamic*/+ 1)";

        [TestMethod]
        public void CSharp_ToCSharp_Test81()
        {
            Assert.AreEqual(dbg81, expr81.ToCSharp());
        }

        private Expression expr82 = Expression.NegateChecked(Expression.Convert(DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.AddChecked, _par1, Expression.Constant(1)), typeof(int)));
        private string dbg82 = @"checked(-(int)(p0 /*dynamic*/+ 1))";

        [TestMethod]
        public void CSharp_ToCSharp_Test82()
        {
            Assert.AreEqual(dbg82, expr82.ToCSharp());
        }

    }
}