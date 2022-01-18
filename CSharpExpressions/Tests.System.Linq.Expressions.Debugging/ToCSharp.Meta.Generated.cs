// Prototyping extended expression trees for C#.
//
// bartde - December 2015

// NOTE: These tests are auto-generated and can *never* fail because they assert the outcome of DebugView()
//       at runtime against the outcome obtained at compile time. However, a human should read through the
//       cases to assert the outcome is as expected.
//
//       Regressions can still be caught given that the T4 won't be re-run unless it gets saved in the IDE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Tests
{
    [TestClass]
    public class ToCSharpTests
    {
		private static LabelTarget _lbl1 = Expression.Label();
		private static LabelTarget _lbl2 = Expression.Label();
		private static LabelTarget _lbl3 = Expression.Label(typeof(int));

        private Expression expr0 = Expression.Parameter(typeof(int));
        private string dbg0 = @"p0";

        [TestMethod]
        public void ToCSharp_Test0()
        {
            Assert.AreEqual(dbg0, expr0.ToCSharp());
        }

        private Expression expr1 = Expression.Parameter(typeof(int), "");
        private string dbg1 = @"p0";

        [TestMethod]
        public void ToCSharp_Test1()
        {
            Assert.AreEqual(dbg1, expr1.ToCSharp());
        }

        private Expression expr2 = Expression.Parameter(typeof(int), " ");
        private string dbg2 = @"p0";

        [TestMethod]
        public void ToCSharp_Test2()
        {
            Assert.AreEqual(dbg2, expr2.ToCSharp());
        }

        private Expression expr3 = Expression.Parameter(typeof(int), "_Bar_123");
        private string dbg3 = @"_Bar_123";

        [TestMethod]
        public void ToCSharp_Test3()
        {
            Assert.AreEqual(dbg3, expr3.ToCSharp());
        }

        private Expression expr4 = Expression.Parameter(typeof(int), "@bar");
        private string dbg4 = @"@bar";

        [TestMethod]
        public void ToCSharp_Test4()
        {
            Assert.AreEqual(dbg4, expr4.ToCSharp());
        }

        private Expression expr5 = Expression.Parameter(typeof(int), "class");
        private string dbg5 = @"@class";

        [TestMethod]
        public void ToCSharp_Test5()
        {
            Assert.AreEqual(dbg5, expr5.ToCSharp());
        }

        private Expression expr6 = Expression.Parameter(typeof(int), "not valid");
        private string dbg6 = @"p0";

        [TestMethod]
        public void ToCSharp_Test6()
        {
            Assert.AreEqual(dbg6, expr6.ToCSharp());
        }

        private Expression expr7 = Expression.Label(Expression.Label(typeof(void)));
        private string dbg7 = @"L0 /*(null)*/:";

        [TestMethod]
        public void ToCSharp_Test7()
        {
            Assert.AreEqual(dbg7, expr7.ToCSharp());
        }

        private Expression expr8 = Expression.Label(Expression.Label(typeof(void), ""));
        private string dbg8 = @"L0 /**/:";

        [TestMethod]
        public void ToCSharp_Test8()
        {
            Assert.AreEqual(dbg8, expr8.ToCSharp());
        }

        private Expression expr9 = Expression.Label(Expression.Label(typeof(void), " "));
        private string dbg9 = @"L0 /* */:";

        [TestMethod]
        public void ToCSharp_Test9()
        {
            Assert.AreEqual(dbg9, expr9.ToCSharp());
        }

        private Expression expr10 = Expression.Label(Expression.Label(typeof(void), "_Bar_123"));
        private string dbg10 = @"_Bar_123:";

        [TestMethod]
        public void ToCSharp_Test10()
        {
            Assert.AreEqual(dbg10, expr10.ToCSharp());
        }

        private Expression expr11 = Expression.Label(Expression.Label(typeof(void), "@bar"));
        private string dbg11 = @"@bar:";

        [TestMethod]
        public void ToCSharp_Test11()
        {
            Assert.AreEqual(dbg11, expr11.ToCSharp());
        }

        private Expression expr12 = Expression.Label(Expression.Label(typeof(void), "this"));
        private string dbg12 = @"@this:";

        [TestMethod]
        public void ToCSharp_Test12()
        {
            Assert.AreEqual(dbg12, expr12.ToCSharp());
        }

        private Expression expr13 = Expression.Property(Expression.Default(typeof(string)), "Length");
        private string dbg13 = @"default(string).Length";

        [TestMethod]
        public void ToCSharp_Test13()
        {
            Assert.AreEqual(dbg13, expr13.ToCSharp());
        }

        private Expression expr14 = Expression.Call(Expression.Call(Expression.Default(typeof(string)), typeof(string).GetMethod("ToUpper", new Type[0])), typeof(string).GetMethod("ToLower", new Type[0]));
        private string dbg14 = @"default(string).ToUpper().ToLower()";

        [TestMethod]
        public void ToCSharp_Test14()
        {
            Assert.AreEqual(dbg14, expr14.ToCSharp());
        }

        private Expression expr15 = Expression.Default(typeof(int));
        private string dbg15 = @"default(int)";

        [TestMethod]
        public void ToCSharp_Test15()
        {
            Assert.AreEqual(dbg15, expr15.ToCSharp());
        }

        private Expression expr16 = Expression.Default(typeof(string));
        private string dbg16 = @"default(string)";

        [TestMethod]
        public void ToCSharp_Test16()
        {
            Assert.AreEqual(dbg16, expr16.ToCSharp());
        }

        private Expression expr17 = Expression.Default(typeof(int[]));
        private string dbg17 = @"default(int[])";

        [TestMethod]
        public void ToCSharp_Test17()
        {
            Assert.AreEqual(dbg17, expr17.ToCSharp());
        }

        private Expression expr18 = Expression.Default(typeof(int[,]));
        private string dbg18 = @"default(int[,])";

        [TestMethod]
        public void ToCSharp_Test18()
        {
            Assert.AreEqual(dbg18, expr18.ToCSharp());
        }

        private Expression expr19 = Expression.Default(typeof(int?));
        private string dbg19 = @"default(int?)";

        [TestMethod]
        public void ToCSharp_Test19()
        {
            Assert.AreEqual(dbg19, expr19.ToCSharp());
        }

        private Expression expr20 = Expression.Default(typeof(Dictionary<List<int>, bool?[]>));
        private string dbg20 = @"default(Dictionary<List<int>, bool?[]>)";

        [TestMethod]
        public void ToCSharp_Test20()
        {
            Assert.AreEqual(dbg20, expr20.ToCSharp());
        }

        private Expression expr21 = Expression.Default(typeof(void));
        private string dbg21 = @";";

        [TestMethod]
        public void ToCSharp_Test21()
        {
            Assert.AreEqual(dbg21, expr21.ToCSharp());
        }

        private Expression expr22 = Expression.Constant(42);
        private string dbg22 = @"42";

        [TestMethod]
        public void ToCSharp_Test22()
        {
            Assert.AreEqual(dbg22, expr22.ToCSharp());
        }

        private Expression expr23 = Expression.Constant(42L);
        private string dbg23 = @"42L";

        [TestMethod]
        public void ToCSharp_Test23()
        {
            Assert.AreEqual(dbg23, expr23.ToCSharp());
        }

        private Expression expr24 = Expression.Constant(42UL);
        private string dbg24 = @"42UL";

        [TestMethod]
        public void ToCSharp_Test24()
        {
            Assert.AreEqual(dbg24, expr24.ToCSharp());
        }

        private Expression expr25 = Expression.Constant(42.0);
        private string dbg25 = @"42D";

        [TestMethod]
        public void ToCSharp_Test25()
        {
            Assert.AreEqual(dbg25, expr25.ToCSharp());
        }

        private Expression expr26 = Expression.Constant(42.0f);
        private string dbg26 = @"42F";

        [TestMethod]
        public void ToCSharp_Test26()
        {
            Assert.AreEqual(dbg26, expr26.ToCSharp());
        }

        private Expression expr27 = Expression.Constant(42.0m);
        private string dbg27 = @"42.0M";

        [TestMethod]
        public void ToCSharp_Test27()
        {
            Assert.AreEqual(dbg27, expr27.ToCSharp());
        }

        private Expression expr28 = Expression.Constant((byte)42);
        private string dbg28 = @"(byte)42";

        [TestMethod]
        public void ToCSharp_Test28()
        {
            Assert.AreEqual(dbg28, expr28.ToCSharp());
        }

        private Expression expr29 = Expression.Constant((sbyte)42);
        private string dbg29 = @"(sbyte)42";

        [TestMethod]
        public void ToCSharp_Test29()
        {
            Assert.AreEqual(dbg29, expr29.ToCSharp());
        }

        private Expression expr30 = Expression.Constant((short)42);
        private string dbg30 = @"(short)42";

        [TestMethod]
        public void ToCSharp_Test30()
        {
            Assert.AreEqual(dbg30, expr30.ToCSharp());
        }

        private Expression expr31 = Expression.Constant((ushort)42);
        private string dbg31 = @"(ushort)42";

        [TestMethod]
        public void ToCSharp_Test31()
        {
            Assert.AreEqual(dbg31, expr31.ToCSharp());
        }

        private Expression expr32 = Expression.Constant((uint)42);
        private string dbg32 = @"42U";

        [TestMethod]
        public void ToCSharp_Test32()
        {
            Assert.AreEqual(dbg32, expr32.ToCSharp());
        }

        private Expression expr33 = Expression.Constant(null, typeof(object));
        private string dbg33 = @"(object)null";

        [TestMethod]
        public void ToCSharp_Test33()
        {
            Assert.AreEqual(dbg33, expr33.ToCSharp());
        }

        private Expression expr34 = Expression.Constant(null, typeof(string));
        private string dbg34 = @"(string)null";

        [TestMethod]
        public void ToCSharp_Test34()
        {
            Assert.AreEqual(dbg34, expr34.ToCSharp());
        }

        private Expression expr35 = Expression.Constant(null, typeof(int?));
        private string dbg35 = @"(int?)null";

        [TestMethod]
        public void ToCSharp_Test35()
        {
            Assert.AreEqual(dbg35, expr35.ToCSharp());
        }

        private Expression expr36 = Expression.Constant(ConsoleColor.Red);
        private string dbg36 = @"ConsoleColor.Red";

        [TestMethod]
        public void ToCSharp_Test36()
        {
            Assert.AreEqual(dbg36, expr36.ToCSharp());
        }

        private Expression expr37 = Expression.Constant(System.IO.FileAccess.Read | System.IO.FileAccess.Write);
        private string dbg37 = @"System.IO.FileAccess.ReadWrite";

        [TestMethod]
        public void ToCSharp_Test37()
        {
            Assert.AreEqual(dbg37, expr37.ToCSharp());
        }

        private Expression expr38 = Expression.Constant(System.IO.FileShare.Read | System.IO.FileShare.Write | System.IO.FileShare.Delete);
        private string dbg38 = @"__c0";

        [TestMethod]
        public void ToCSharp_Test38()
        {
            Assert.AreEqual(dbg38, expr38.ToCSharp());
        }

        private Expression expr39 = Expression.Constant(DateTime.Now);
        private string dbg39 = @"__c0";

        [TestMethod]
        public void ToCSharp_Test39()
        {
            Assert.AreEqual(dbg39, expr39.ToCSharp());
        }

        private Expression expr40 = Expression.Constant(DateTime.Now, typeof(object));
        private string dbg40 = @"(object)__c0";

        [TestMethod]
        public void ToCSharp_Test40()
        {
            Assert.AreEqual(dbg40, expr40.ToCSharp());
        }

        private Expression expr41 = Expression.Constant('"');
        private string dbg41 = @"'""'";

        [TestMethod]
        public void ToCSharp_Test41()
        {
            Assert.AreEqual(dbg41, expr41.ToCSharp());
        }

        private Expression expr42 = Expression.Constant('x');
        private string dbg42 = @"'x'";

        [TestMethod]
        public void ToCSharp_Test42()
        {
            Assert.AreEqual(dbg42, expr42.ToCSharp());
        }

        private Expression expr43 = Expression.Constant("bar");
        private string dbg43 = @"""bar""";

        [TestMethod]
        public void ToCSharp_Test43()
        {
            Assert.AreEqual(dbg43, expr43.ToCSharp());
        }

        private Expression expr44 = Expression.Constant("bar	foo");
        private string dbg44 = @"""bar\tfoo""";

        [TestMethod]
        public void ToCSharp_Test44()
        {
            Assert.AreEqual(dbg44, expr44.ToCSharp());
        }

        private Expression expr45 = Expression.Constant("barfoo");
        private string dbg45 = @"""bar\afoo""";

        [TestMethod]
        public void ToCSharp_Test45()
        {
            Assert.AreEqual(dbg45, expr45.ToCSharp());
        }

        private Expression expr46 = Expression.Constant("barfoo");
        private string dbg46 = @"""bar\ffoo""";

        [TestMethod]
        public void ToCSharp_Test46()
        {
            Assert.AreEqual(dbg46, expr46.ToCSharp());
        }

        private Expression expr47 = Expression.Constant("barfoo");
        private string dbg47 = @"""bar\bfoo""";

        [TestMethod]
        public void ToCSharp_Test47()
        {
            Assert.AreEqual(dbg47, expr47.ToCSharp());
        }

        private Expression expr48 = Expression.Constant("barfoo");
        private string dbg48 = @"""bar\vfoo""";

        [TestMethod]
        public void ToCSharp_Test48()
        {
            Assert.AreEqual(dbg48, expr48.ToCSharp());
        }

        private Expression expr49 = Expression.Constant("bar\\foo");
        private string dbg49 = @"""bar\\foo""";

        [TestMethod]
        public void ToCSharp_Test49()
        {
            Assert.AreEqual(dbg49, expr49.ToCSharp());
        }

        private Expression expr50 = Expression.Constant("bar'foo");
        private string dbg50 = @"""bar'foo""";

        [TestMethod]
        public void ToCSharp_Test50()
        {
            Assert.AreEqual(dbg50, expr50.ToCSharp());
        }

        private Expression expr51 = Expression.Add(Expression.Constant(1), Expression.Constant(2));
        private string dbg51 = @"1 + 2";

        [TestMethod]
        public void ToCSharp_Test51()
        {
            Assert.AreEqual(dbg51, expr51.ToCSharp());
        }

        private Expression expr52 = Expression.AddChecked(Expression.Constant(1), Expression.Constant(2));
        private string dbg52 = @"checked(1 + 2)";

        [TestMethod]
        public void ToCSharp_Test52()
        {
            Assert.AreEqual(dbg52, expr52.ToCSharp());
        }

        private Expression expr53 = Expression.Subtract(Expression.Constant(1), Expression.Constant(2));
        private string dbg53 = @"1 - 2";

        [TestMethod]
        public void ToCSharp_Test53()
        {
            Assert.AreEqual(dbg53, expr53.ToCSharp());
        }

        private Expression expr54 = Expression.SubtractChecked(Expression.Constant(1), Expression.Constant(2));
        private string dbg54 = @"checked(1 - 2)";

        [TestMethod]
        public void ToCSharp_Test54()
        {
            Assert.AreEqual(dbg54, expr54.ToCSharp());
        }

        private Expression expr55 = Expression.Multiply(Expression.Constant(1), Expression.Constant(2));
        private string dbg55 = @"1 * 2";

        [TestMethod]
        public void ToCSharp_Test55()
        {
            Assert.AreEqual(dbg55, expr55.ToCSharp());
        }

        private Expression expr56 = Expression.MultiplyChecked(Expression.Constant(1), Expression.Constant(2));
        private string dbg56 = @"checked(1 * 2)";

        [TestMethod]
        public void ToCSharp_Test56()
        {
            Assert.AreEqual(dbg56, expr56.ToCSharp());
        }

        private Expression expr57 = Expression.Divide(Expression.Constant(1), Expression.Constant(2));
        private string dbg57 = @"1 / 2";

        [TestMethod]
        public void ToCSharp_Test57()
        {
            Assert.AreEqual(dbg57, expr57.ToCSharp());
        }

        private Expression expr58 = Expression.Modulo(Expression.Constant(1), Expression.Constant(2));
        private string dbg58 = @"1 % 2";

        [TestMethod]
        public void ToCSharp_Test58()
        {
            Assert.AreEqual(dbg58, expr58.ToCSharp());
        }

        private Expression expr59 = Expression.And(Expression.Constant(1), Expression.Constant(2));
        private string dbg59 = @"1 & 2";

        [TestMethod]
        public void ToCSharp_Test59()
        {
            Assert.AreEqual(dbg59, expr59.ToCSharp());
        }

        private Expression expr60 = Expression.Or(Expression.Constant(1), Expression.Constant(2));
        private string dbg60 = @"1 | 2";

        [TestMethod]
        public void ToCSharp_Test60()
        {
            Assert.AreEqual(dbg60, expr60.ToCSharp());
        }

        private Expression expr61 = Expression.ExclusiveOr(Expression.Constant(1), Expression.Constant(2));
        private string dbg61 = @"1 ^ 2";

        [TestMethod]
        public void ToCSharp_Test61()
        {
            Assert.AreEqual(dbg61, expr61.ToCSharp());
        }

        private Expression expr62 = Expression.Equal(Expression.Constant(1), Expression.Constant(2));
        private string dbg62 = @"1 == 2";

        [TestMethod]
        public void ToCSharp_Test62()
        {
            Assert.AreEqual(dbg62, expr62.ToCSharp());
        }

        private Expression expr63 = Expression.NotEqual(Expression.Constant(1), Expression.Constant(2));
        private string dbg63 = @"1 != 2";

        [TestMethod]
        public void ToCSharp_Test63()
        {
            Assert.AreEqual(dbg63, expr63.ToCSharp());
        }

        private Expression expr64 = Expression.LessThan(Expression.Constant(1), Expression.Constant(2));
        private string dbg64 = @"1 < 2";

        [TestMethod]
        public void ToCSharp_Test64()
        {
            Assert.AreEqual(dbg64, expr64.ToCSharp());
        }

        private Expression expr65 = Expression.LessThanOrEqual(Expression.Constant(1), Expression.Constant(2));
        private string dbg65 = @"1 <= 2";

        [TestMethod]
        public void ToCSharp_Test65()
        {
            Assert.AreEqual(dbg65, expr65.ToCSharp());
        }

        private Expression expr66 = Expression.GreaterThan(Expression.Constant(1), Expression.Constant(2));
        private string dbg66 = @"1 > 2";

        [TestMethod]
        public void ToCSharp_Test66()
        {
            Assert.AreEqual(dbg66, expr66.ToCSharp());
        }

        private Expression expr67 = Expression.GreaterThanOrEqual(Expression.Constant(1), Expression.Constant(2));
        private string dbg67 = @"1 >= 2";

        [TestMethod]
        public void ToCSharp_Test67()
        {
            Assert.AreEqual(dbg67, expr67.ToCSharp());
        }

        private Expression expr68 = Expression.LeftShift(Expression.Constant(1), Expression.Constant(2));
        private string dbg68 = @"1 << 2";

        [TestMethod]
        public void ToCSharp_Test68()
        {
            Assert.AreEqual(dbg68, expr68.ToCSharp());
        }

        private Expression expr69 = Expression.RightShift(Expression.Constant(1), Expression.Constant(2));
        private string dbg69 = @"1 >> 2";

        [TestMethod]
        public void ToCSharp_Test69()
        {
            Assert.AreEqual(dbg69, expr69.ToCSharp());
        }

        private Expression expr70 = Expression.AndAlso(Expression.Constant(true), Expression.Constant(false));
        private string dbg70 = @"true && false";

        [TestMethod]
        public void ToCSharp_Test70()
        {
            Assert.AreEqual(dbg70, expr70.ToCSharp());
        }

        private Expression expr71 = Expression.OrElse(Expression.Constant(true), Expression.Constant(false));
        private string dbg71 = @"true || false";

        [TestMethod]
        public void ToCSharp_Test71()
        {
            Assert.AreEqual(dbg71, expr71.ToCSharp());
        }

        private Expression expr72 = Expression.Coalesce(Expression.Constant(null, typeof(string)), Expression.Constant("null"));
        private string dbg72 = @"(string)null ?? ""null""";

        [TestMethod]
        public void ToCSharp_Test72()
        {
            Assert.AreEqual(dbg72, expr72.ToCSharp());
        }

        private Expression expr73 = Expression.Power(Expression.Constant(1.0), Expression.Constant(2.0));
        private string dbg73 = @"Math.Pow(1D, 2D)";

        [TestMethod]
        public void ToCSharp_Test73()
        {
            Assert.AreEqual(dbg73, expr73.ToCSharp());
        }

        private Expression expr74 = Expression.Assign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg74 = @"p0 = 1";

        [TestMethod]
        public void ToCSharp_Test74()
        {
            Assert.AreEqual(dbg74, expr74.ToCSharp());
        }

        private Expression expr75 = Expression.AddAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg75 = @"p0 += 1";

        [TestMethod]
        public void ToCSharp_Test75()
        {
            Assert.AreEqual(dbg75, expr75.ToCSharp());
        }

        private Expression expr76 = Expression.SubtractAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg76 = @"p0 -= 1";

        [TestMethod]
        public void ToCSharp_Test76()
        {
            Assert.AreEqual(dbg76, expr76.ToCSharp());
        }

        private Expression expr77 = Expression.MultiplyAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg77 = @"p0 *= 1";

        [TestMethod]
        public void ToCSharp_Test77()
        {
            Assert.AreEqual(dbg77, expr77.ToCSharp());
        }

        private Expression expr78 = Expression.AddAssignChecked(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg78 = @"checked(p0 += 1)";

        [TestMethod]
        public void ToCSharp_Test78()
        {
            Assert.AreEqual(dbg78, expr78.ToCSharp());
        }

        private Expression expr79 = Expression.SubtractAssignChecked(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg79 = @"checked(p0 -= 1)";

        [TestMethod]
        public void ToCSharp_Test79()
        {
            Assert.AreEqual(dbg79, expr79.ToCSharp());
        }

        private Expression expr80 = Expression.MultiplyAssignChecked(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg80 = @"checked(p0 *= 1)";

        [TestMethod]
        public void ToCSharp_Test80()
        {
            Assert.AreEqual(dbg80, expr80.ToCSharp());
        }

        private Expression expr81 = Expression.DivideAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg81 = @"p0 /= 1";

        [TestMethod]
        public void ToCSharp_Test81()
        {
            Assert.AreEqual(dbg81, expr81.ToCSharp());
        }

        private Expression expr82 = Expression.ModuloAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg82 = @"p0 %= 1";

        [TestMethod]
        public void ToCSharp_Test82()
        {
            Assert.AreEqual(dbg82, expr82.ToCSharp());
        }

        private Expression expr83 = Expression.AndAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg83 = @"p0 &= 1";

        [TestMethod]
        public void ToCSharp_Test83()
        {
            Assert.AreEqual(dbg83, expr83.ToCSharp());
        }

        private Expression expr84 = Expression.OrAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg84 = @"p0 |= 1";

        [TestMethod]
        public void ToCSharp_Test84()
        {
            Assert.AreEqual(dbg84, expr84.ToCSharp());
        }

        private Expression expr85 = Expression.ExclusiveOrAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg85 = @"p0 ^= 1";

        [TestMethod]
        public void ToCSharp_Test85()
        {
            Assert.AreEqual(dbg85, expr85.ToCSharp());
        }

        private Expression expr86 = Expression.LeftShiftAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg86 = @"p0 <<= 1";

        [TestMethod]
        public void ToCSharp_Test86()
        {
            Assert.AreEqual(dbg86, expr86.ToCSharp());
        }

        private Expression expr87 = Expression.RightShiftAssign(Expression.Parameter(typeof(int)), Expression.Constant(1));
        private string dbg87 = @"p0 >>= 1";

        [TestMethod]
        public void ToCSharp_Test87()
        {
            Assert.AreEqual(dbg87, expr87.ToCSharp());
        }

        private Expression expr88 = Expression.PowerAssign(Expression.Parameter(typeof(double)), Expression.Constant(1.0));
        private string dbg88 = @"p0 = Math.Pow(p0, 1D)";

        [TestMethod]
        public void ToCSharp_Test88()
        {
            Assert.AreEqual(dbg88, expr88.ToCSharp());
        }

        private Expression expr89 = Expression.Add(Expression.Constant(1.0), Expression.Constant(2.0), typeof(Math).GetMethod("Pow", new[] { typeof(double), typeof(double) }));
        private string dbg89 = @"Math.Pow(1D, 2D)";

        [TestMethod]
        public void ToCSharp_Test89()
        {
            Assert.AreEqual(dbg89, expr89.ToCSharp());
        }

        private Expression expr90 = Expression.Add(Expression.Add(Expression.Constant(1), Expression.Constant(2)), Expression.Constant(3));
        private string dbg90 = @"1 + 2 + 3";

        [TestMethod]
        public void ToCSharp_Test90()
        {
            Assert.AreEqual(dbg90, expr90.ToCSharp());
        }

        private Expression expr91 = Expression.Add(Expression.Constant(1), Expression.Add(Expression.Constant(2), Expression.Constant(3)));
        private string dbg91 = @"1 + 2 + 3";

        [TestMethod]
        public void ToCSharp_Test91()
        {
            Assert.AreEqual(dbg91, expr91.ToCSharp());
        }

        private Expression expr92 = Expression.Add(Expression.Subtract(Expression.Constant(1), Expression.Constant(2)), Expression.Constant(3));
        private string dbg92 = @"1 - 2 + 3";

        [TestMethod]
        public void ToCSharp_Test92()
        {
            Assert.AreEqual(dbg92, expr92.ToCSharp());
        }

        private Expression expr93 = Expression.Add(Expression.Constant(1), Expression.Subtract(Expression.Constant(2), Expression.Constant(3)));
        private string dbg93 = @"1 + 2 - 3";

        [TestMethod]
        public void ToCSharp_Test93()
        {
            Assert.AreEqual(dbg93, expr93.ToCSharp());
        }

        private Expression expr94 = Expression.Add(Expression.Multiply(Expression.Constant(1), Expression.Constant(2)), Expression.Constant(3));
        private string dbg94 = @"1 * 2 + 3";

        [TestMethod]
        public void ToCSharp_Test94()
        {
            Assert.AreEqual(dbg94, expr94.ToCSharp());
        }

        private Expression expr95 = Expression.Add(Expression.Constant(1), Expression.Multiply(Expression.Constant(2), Expression.Constant(3)));
        private string dbg95 = @"1 + 2 * 3";

        [TestMethod]
        public void ToCSharp_Test95()
        {
            Assert.AreEqual(dbg95, expr95.ToCSharp());
        }

        private Expression expr96 = Expression.Multiply(Expression.Constant(1), Expression.Subtract(Expression.Constant(2), Expression.Constant(3)));
        private string dbg96 = @"1 * (2 - 3)";

        [TestMethod]
        public void ToCSharp_Test96()
        {
            Assert.AreEqual(dbg96, expr96.ToCSharp());
        }

        private Expression expr97 = Expression.Multiply(Expression.Subtract(Expression.Constant(1), Expression.Constant(2)), Expression.Constant(3));
        private string dbg97 = @"(1 - 2) * 3";

        [TestMethod]
        public void ToCSharp_Test97()
        {
            Assert.AreEqual(dbg97, expr97.ToCSharp());
        }

        private Expression expr98 = Expression.Subtract(Expression.Constant(1), Expression.Subtract(Expression.Constant(2), Expression.Constant(3)));
        private string dbg98 = @"1 - (2 - 3)";

        [TestMethod]
        public void ToCSharp_Test98()
        {
            Assert.AreEqual(dbg98, expr98.ToCSharp());
        }

        private Expression expr99 = Expression.Subtract(Expression.Subtract(Expression.Constant(1), Expression.Constant(2)), Expression.Constant(3));
        private string dbg99 = @"1 - 2 - 3";

        [TestMethod]
        public void ToCSharp_Test99()
        {
            Assert.AreEqual(dbg99, expr99.ToCSharp());
        }

        private Expression expr100 = Expression.And(Expression.And(Expression.Constant(1), Expression.Constant(2)), Expression.Constant(3));
        private string dbg100 = @"1 & 2 & 3";

        [TestMethod]
        public void ToCSharp_Test100()
        {
            Assert.AreEqual(dbg100, expr100.ToCSharp());
        }

        private Expression expr101 = Expression.And(Expression.Or(Expression.Constant(1), Expression.Constant(2)), Expression.Constant(3));
        private string dbg101 = @"(1 | 2) & 3";

        [TestMethod]
        public void ToCSharp_Test101()
        {
            Assert.AreEqual(dbg101, expr101.ToCSharp());
        }

        private Expression expr102 = Expression.Or(Expression.And(Expression.Constant(1), Expression.Constant(2)), Expression.Constant(3));
        private string dbg102 = @"1 & 2 | 3";

        [TestMethod]
        public void ToCSharp_Test102()
        {
            Assert.AreEqual(dbg102, expr102.ToCSharp());
        }

        private Expression expr103 = Expression.NegateChecked(Expression.Add(Expression.Constant(1), Expression.Constant(2)));
        private string dbg103 = @"/*checked(*/-/*)*/(1 + 2)";

        [TestMethod]
        public void ToCSharp_Test103()
        {
            Assert.AreEqual(dbg103, expr103.ToCSharp());
        }

        private Expression expr104 = Expression.AddChecked(Expression.Add(Expression.Constant(1), Expression.Constant(2)), Expression.Constant(3));
        private string dbg104 = @"1 + 2 /*checked(*/+/*)*/ 3";

        [TestMethod]
        public void ToCSharp_Test104()
        {
            Assert.AreEqual(dbg104, expr104.ToCSharp());
        }

        private Expression expr105 = Expression.Negate(Expression.Constant(1));
        private string dbg105 = @"-(1)";

        [TestMethod]
        public void ToCSharp_Test105()
        {
            Assert.AreEqual(dbg105, expr105.ToCSharp());
        }

        private Expression expr106 = Expression.NegateChecked(Expression.Constant(1));
        private string dbg106 = @"checked(-1)";

        [TestMethod]
        public void ToCSharp_Test106()
        {
            Assert.AreEqual(dbg106, expr106.ToCSharp());
        }

        private Expression expr107 = Expression.Negate(Expression.Negate(Expression.Parameter(typeof(int))));
        private string dbg107 = @"--p0";

        [TestMethod]
        public void ToCSharp_Test107()
        {
            Assert.AreEqual(dbg107, expr107.ToCSharp());
        }

        private Expression expr108 = Expression.UnaryPlus(Expression.Constant(1));
        private string dbg108 = @"+1";

        [TestMethod]
        public void ToCSharp_Test108()
        {
            Assert.AreEqual(dbg108, expr108.ToCSharp());
        }

        private Expression expr109 = Expression.UnaryPlus(Expression.UnaryPlus(Expression.Parameter(typeof(int))));
        private string dbg109 = @"++p0";

        [TestMethod]
        public void ToCSharp_Test109()
        {
            Assert.AreEqual(dbg109, expr109.ToCSharp());
        }

        private Expression expr110 = Expression.OnesComplement(Expression.Constant(1));
        private string dbg110 = @"~1";

        [TestMethod]
        public void ToCSharp_Test110()
        {
            Assert.AreEqual(dbg110, expr110.ToCSharp());
        }

        private Expression expr111 = Expression.Not(Expression.Constant(true));
        private string dbg111 = @"!true";

        [TestMethod]
        public void ToCSharp_Test111()
        {
            Assert.AreEqual(dbg111, expr111.ToCSharp());
        }

        private Expression expr112 = Expression.Not(Expression.Constant(1));
        private string dbg112 = @"~1";

        [TestMethod]
        public void ToCSharp_Test112()
        {
            Assert.AreEqual(dbg112, expr112.ToCSharp());
        }

        private Expression expr113 = Expression.Convert(Expression.Parameter(typeof(object), "o"), typeof(int));
        private string dbg113 = @"(int)o";

        [TestMethod]
        public void ToCSharp_Test113()
        {
            Assert.AreEqual(dbg113, expr113.ToCSharp());
        }

        private Expression expr114 = Expression.ConvertChecked(Expression.Parameter(typeof(long), "o"), typeof(int));
        private string dbg114 = @"checked((int)o)";

        [TestMethod]
        public void ToCSharp_Test114()
        {
            Assert.AreEqual(dbg114, expr114.ToCSharp());
        }

        private Expression expr115 = Expression.TypeAs(Expression.Parameter(typeof(object), "o"), typeof(string));
        private string dbg115 = @"o as string";

        [TestMethod]
        public void ToCSharp_Test115()
        {
            Assert.AreEqual(dbg115, expr115.ToCSharp());
        }

        private Expression expr116 = Expression.ArrayLength(Expression.Parameter(typeof(int[])));
        private string dbg116 = @"p0.Length";

        [TestMethod]
        public void ToCSharp_Test116()
        {
            Assert.AreEqual(dbg116, expr116.ToCSharp());
        }

        private Expression expr117 = Expression.PreIncrementAssign(Expression.Parameter(typeof(int)));
        private string dbg117 = @"++p0";

        [TestMethod]
        public void ToCSharp_Test117()
        {
            Assert.AreEqual(dbg117, expr117.ToCSharp());
        }

        private Expression expr118 = Expression.PreDecrementAssign(Expression.Parameter(typeof(int)));
        private string dbg118 = @"--p0";

        [TestMethod]
        public void ToCSharp_Test118()
        {
            Assert.AreEqual(dbg118, expr118.ToCSharp());
        }

        private Expression expr119 = Expression.PostIncrementAssign(Expression.Parameter(typeof(int)));
        private string dbg119 = @"p0++";

        [TestMethod]
        public void ToCSharp_Test119()
        {
            Assert.AreEqual(dbg119, expr119.ToCSharp());
        }

        private Expression expr120 = Expression.PostDecrementAssign(Expression.Parameter(typeof(int)));
        private string dbg120 = @"p0--";

        [TestMethod]
        public void ToCSharp_Test120()
        {
            Assert.AreEqual(dbg120, expr120.ToCSharp());
        }

        private Expression expr121 = Expression.IsTrue(Expression.Parameter(typeof(bool)));
        private string dbg121 = @"p0 == true";

        [TestMethod]
        public void ToCSharp_Test121()
        {
            Assert.AreEqual(dbg121, expr121.ToCSharp());
        }

        private Expression expr122 = Expression.IsFalse(Expression.Parameter(typeof(bool)));
        private string dbg122 = @"p0 == false";

        [TestMethod]
        public void ToCSharp_Test122()
        {
            Assert.AreEqual(dbg122, expr122.ToCSharp());
        }

        private Expression expr123 = Expression.Increment(Expression.Parameter(typeof(int)));
        private string dbg123 = @"p0 + 1";

        [TestMethod]
        public void ToCSharp_Test123()
        {
            Assert.AreEqual(dbg123, expr123.ToCSharp());
        }

        private Expression expr124 = Expression.Decrement(Expression.Parameter(typeof(int)));
        private string dbg124 = @"p0 - 1";

        [TestMethod]
        public void ToCSharp_Test124()
        {
            Assert.AreEqual(dbg124, expr124.ToCSharp());
        }

        private Expression expr125 = Expression.Unbox(Expression.Parameter(typeof(object), "o"), typeof(int));
        private string dbg125 = @"/* unbox */o";

        [TestMethod]
        public void ToCSharp_Test125()
        {
            Assert.AreEqual(dbg125, expr125.ToCSharp());
        }

        private Expression expr126 = Expression.Negate(Expression.Constant(1), typeof(Math).GetMethod("Abs", new[] { typeof(int) }));
        private string dbg126 = @"Math.Abs(1)";

        [TestMethod]
        public void ToCSharp_Test126()
        {
            Assert.AreEqual(dbg126, expr126.ToCSharp());
        }

        private Expression expr127 = Expression.Convert(Expression.Constant(1), typeof(int), typeof(Math).GetMethod("Abs", new[] { typeof(int) }));
        private string dbg127 = @"Math.Abs(1)";

        [TestMethod]
        public void ToCSharp_Test127()
        {
            Assert.AreEqual(dbg127, expr127.ToCSharp());
        }

        private Expression expr128 = Expression.ConvertChecked(Expression.Constant(1), typeof(int), typeof(Math).GetMethod("Abs", new[] { typeof(int) }));
        private string dbg128 = @"Math.Abs(1)";

        [TestMethod]
        public void ToCSharp_Test128()
        {
            Assert.AreEqual(dbg128, expr128.ToCSharp());
        }

        private Expression expr129 = /* BUG */ Expression.PostIncrementAssign(Expression.Parameter(typeof(int)), typeof(Math).GetMethod("Abs", new[] { typeof(int) }));
        private string dbg129 = @"p0 = Math.Abs(p0)";

        [TestMethod]
        public void ToCSharp_Test129()
        {
            Assert.AreEqual(dbg129, expr129.ToCSharp());
        }

        private Expression expr130 = /* BUG */ Expression.PostDecrementAssign(Expression.Parameter(typeof(int)), typeof(Math).GetMethod("Abs", new[] { typeof(int) }));
        private string dbg130 = @"p0 = Math.Abs(p0)";

        [TestMethod]
        public void ToCSharp_Test130()
        {
            Assert.AreEqual(dbg130, expr130.ToCSharp());
        }

        private Expression expr131 = Expression.Condition(Expression.Constant(true), Expression.Constant(1), Expression.Constant(2));
        private string dbg131 = @"true ? 1 : 2";

        [TestMethod]
        public void ToCSharp_Test131()
        {
            Assert.AreEqual(dbg131, expr131.ToCSharp());
        }

        private Expression expr132 = Expression.Property(null, typeof(DateTime).GetProperty("Now"));
        private string dbg132 = @"DateTime.Now";

        [TestMethod]
        public void ToCSharp_Test132()
        {
            Assert.AreEqual(dbg132, expr132.ToCSharp());
        }

        private Expression expr133 = Expression.Property(Expression.Constant("bar"), typeof(string).GetProperty("Length"));
        private string dbg133 = @"""bar"".Length";

        [TestMethod]
        public void ToCSharp_Test133()
        {
            Assert.AreEqual(dbg133, expr133.ToCSharp());
        }

        private Expression expr134 = Expression.Call(Expression.Constant("bar"), typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) }), Expression.Constant(0), Expression.Constant(1));
        private string dbg134 = @"""bar"".Substring(0, 1)";

        [TestMethod]
        public void ToCSharp_Test134()
        {
            Assert.AreEqual(dbg134, expr134.ToCSharp());
        }

        private Expression expr135 = Expression.Call(null, typeof(Activator).GetMethod("CreateInstance", new Type[0]).MakeGenericMethod(typeof(string)));
        private string dbg135 = @"Activator.CreateInstance<string>()";

        [TestMethod]
        public void ToCSharp_Test135()
        {
            Assert.AreEqual(dbg135, expr135.ToCSharp());
        }

        private Expression expr136 = Expression.Call(null, typeof(int).GetMethod("TryParse", new Type[] { typeof(string), typeof(int).MakeByRefType() }), Expression.Constant("bar"), Expression.Parameter(typeof(int), "res"));
        private string dbg136 = @"int.TryParse(""bar"", out res)";

        [TestMethod]
        public void ToCSharp_Test136()
        {
            Assert.AreEqual(dbg136, expr136.ToCSharp());
        }

        private Expression expr137 = Expression.Call(null, typeof(System.Threading.Interlocked).GetMethod("Exchange", new Type[] { typeof(int).MakeByRefType(), typeof(int) }), Expression.Parameter(typeof(int), "x"), Expression.Constant(1));
        private string dbg137 = @"System.Threading.Interlocked.Exchange(ref x, 1)";

        [TestMethod]
        public void ToCSharp_Test137()
        {
            Assert.AreEqual(dbg137, expr137.ToCSharp());
        }

        private Expression expr138 = Expression.New(typeof(List<int>).GetConstructor(new Type[0]));
        private string dbg138 = @"new List<int>()";

        [TestMethod]
        public void ToCSharp_Test138()
        {
            Assert.AreEqual(dbg138, expr138.ToCSharp());
        }

        private Expression expr139 = Expression.New(typeof(TimeSpan).GetConstructor(new[] { typeof(int), typeof(int), typeof(int) }), Expression.Constant(0), Expression.Constant(1), Expression.Constant(2));
        private string dbg139 = @"new TimeSpan(0, 1, 2)";

        [TestMethod]
        public void ToCSharp_Test139()
        {
            Assert.AreEqual(dbg139, expr139.ToCSharp());
        }

        private Expression expr140 = ((Expression<Func<object>>)(() => new {})).Body;
        private string dbg140 = @"new { }";

        [TestMethod]
        public void ToCSharp_Test140()
        {
            Assert.AreEqual(dbg140, expr140.ToCSharp());
        }

        private Expression expr141 = ((Expression<Func<object>>)(() => new { a = 1, b = true })).Body;
        private string dbg141 = @"new { a = 1, b = true }";

        [TestMethod]
        public void ToCSharp_Test141()
        {
            Assert.AreEqual(dbg141, expr141.ToCSharp());
        }

        private Expression expr142 = ((Expression<Func<object>>)(() => new int[1, 2])).Body;
        private string dbg142 = @"new int[1, 2]";

        [TestMethod]
        public void ToCSharp_Test142()
        {
            Assert.AreEqual(dbg142, expr142.ToCSharp());
        }

        private Expression expr143 = ((Expression<Func<object>>)(() => new int[] { 2, 3, 5 })).Body;
        private string dbg143 = @"new int[] { 2, 3, 5 }";

        [TestMethod]
        public void ToCSharp_Test143()
        {
            Assert.AreEqual(dbg143, expr143.ToCSharp());
        }

        private Expression expr144 = ((Expression<Func<int[], int>>)(xs => xs[0])).Body;
        private string dbg144 = @"xs[0]";

        [TestMethod]
        public void ToCSharp_Test144()
        {
            Assert.AreEqual(dbg144, expr144.ToCSharp());
        }

        private Expression expr145 = ((Expression<Func<List<int>, int>>)(xs => xs[0])).Body /* TODO: decompile? */;
        private string dbg145 = @"xs.get_Item(0)";

        [TestMethod]
        public void ToCSharp_Test145()
        {
            Assert.AreEqual(dbg145, expr145.ToCSharp());
        }

        private Expression expr146 = Expression.MakeIndex(Expression.Parameter(typeof(List<int>), "xs"), typeof(List<int>).GetProperty("Item"), new[] { Expression.Constant(0) });
        private string dbg146 = @"xs[0]";

        [TestMethod]
        public void ToCSharp_Test146()
        {
            Assert.AreEqual(dbg146, expr146.ToCSharp());
        }

        private Expression expr147 = ((Expression<Func<Func<int>, int>>)(f => f())).Body;
        private string dbg147 = @"f()";

        [TestMethod]
        public void ToCSharp_Test147()
        {
            Assert.AreEqual(dbg147, expr147.ToCSharp());
        }

        private Expression expr148 = ((Expression<Func<Func<int, int>, int>>)(f => f(42))).Body;
        private string dbg148 = @"f(42)";

        [TestMethod]
        public void ToCSharp_Test148()
        {
            Assert.AreEqual(dbg148, expr148.ToCSharp());
        }

        private Expression expr149 = ((Expression<Func<List<int>>>)(() => new List<int> { 2, 3, 5 })).Body;
        private string dbg149 = @"new List<int>() { 2, 3, 5 }";

        [TestMethod]
        public void ToCSharp_Test149()
        {
            Assert.AreEqual(dbg149, expr149.ToCSharp());
        }

        private Expression expr150 = ((Expression<Func<Dictionary<int, bool>>>)(() => new Dictionary<int, bool> { { 2, true }, { 3, false } })).Body;
        private string dbg150 = @"new Dictionary<int, bool>() { { 2, true }, { 3, false } }";

        [TestMethod]
        public void ToCSharp_Test150()
        {
            Assert.AreEqual(dbg150, expr150.ToCSharp());
        }

        private Expression expr151 = ((Expression<Func<object>>)(() => new AppDomainSetup { ApplicationBase = "bar", ApplicationName = "foo" })).Body;
        private string dbg151 = @"new AppDomainSetup() { ApplicationBase = ""bar"", ApplicationName = ""foo"" }";

        [TestMethod]
        public void ToCSharp_Test151()
        {
            Assert.AreEqual(dbg151, expr151.ToCSharp());
        }

        private Expression expr152 = ((Expression<Func<object>>)(() => new StrongBox<AppDomainSetup> { Value = { ApplicationBase = "bar", ApplicationName = "foo" } })).Body;
        private string dbg152 = @"new System.Runtime.CompilerServices.StrongBox<AppDomainSetup>() { Value = { ApplicationBase = ""bar"", ApplicationName = ""foo"" } }";

        [TestMethod]
        public void ToCSharp_Test152()
        {
            Assert.AreEqual(dbg152, expr152.ToCSharp());
        }

        private Expression expr153 = ((Expression<Func<object>>)(() => new StrongBox<int> { Value = 1 })).Body;
        private string dbg153 = @"new System.Runtime.CompilerServices.StrongBox<int>() { Value = 1 }";

        [TestMethod]
        public void ToCSharp_Test153()
        {
            Assert.AreEqual(dbg153, expr153.ToCSharp());
        }

        private Expression expr154 = ((Expression<Func<object>>)(() => new StrongBox<StrongBox<int>> { Value = { Value = 1 } })).Body;
        private string dbg154 = @"new System.Runtime.CompilerServices.StrongBox<System.Runtime.CompilerServices.StrongBox<int>>() { Value = { Value = 1 } }";

        [TestMethod]
        public void ToCSharp_Test154()
        {
            Assert.AreEqual(dbg154, expr154.ToCSharp());
        }

        private Expression expr155 = ((Expression<Func<object>>)(() => new StrongBox<List<int>> { Value = { 2, 3, 5 } })).Body;
        private string dbg155 = @"new System.Runtime.CompilerServices.StrongBox<List<int>>() { Value = { 2, 3, 5 } }";

        [TestMethod]
        public void ToCSharp_Test155()
        {
            Assert.AreEqual(dbg155, expr155.ToCSharp());
        }

        private Expression expr156 = Expression.TypeIs(Expression.Parameter(typeof(object)), typeof(int));
        private string dbg156 = @"p0 is int";

        [TestMethod]
        public void ToCSharp_Test156()
        {
            Assert.AreEqual(dbg156, expr156.ToCSharp());
        }

        private Expression expr157 = Expression.TypeEqual(Expression.Parameter(typeof(object)), typeof(int));
        private string dbg157 = @"p0?.GetType() == typeof(int)";

        [TestMethod]
        public void ToCSharp_Test157()
        {
            Assert.AreEqual(dbg157, expr157.ToCSharp());
        }

        private Expression expr158 = Expression.IfThen(Expression.Constant(true), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("true")));
        private string dbg158 = @"if (true)
    Console.WriteLine(""true"");";

        [TestMethod]
        public void ToCSharp_Test158()
        {
            Assert.AreEqual(dbg158, expr158.ToCSharp());
        }

        private Expression expr159 = Expression.IfThenElse(Expression.Constant(true), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("true")), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("false")));
        private string dbg159 = @"if (true)
    Console.WriteLine(""true"");
else
    Console.WriteLine(""false"");";

        [TestMethod]
        public void ToCSharp_Test159()
        {
            Assert.AreEqual(dbg159, expr159.ToCSharp());
        }

        private Expression expr160 = Expression.IfThen(Expression.Constant(true), Expression.Block(Expression.Empty()));
        private string dbg160 = @"if (true)
{
    ;
}";

        [TestMethod]
        public void ToCSharp_Test160()
        {
            Assert.AreEqual(dbg160, expr160.ToCSharp());
        }

        private Expression expr161 = Expression.IfThenElse(Expression.Constant(true), Expression.Block(Expression.Empty()), Expression.Block(Expression.Empty()));
        private string dbg161 = @"if (true)
{
    ;
}
else
{
    ;
}";

        [TestMethod]
        public void ToCSharp_Test161()
        {
            Assert.AreEqual(dbg161, expr161.ToCSharp());
        }

        private Expression expr162 = ((Expression<Func<int>>)(() => 42));
        private string dbg162 = @"() => 42";

        [TestMethod]
        public void ToCSharp_Test162()
        {
            Assert.AreEqual(dbg162, expr162.ToCSharp());
        }

        private Expression expr163 = ((Expression<Func<int, int>>)(x => x));
        private string dbg163 = @"(int x) => x";

        [TestMethod]
        public void ToCSharp_Test163()
        {
            Assert.AreEqual(dbg163, expr163.ToCSharp());
        }

        private Expression expr164 = ((Expression<Func<string, int, bool>>)((s, x) => s.Length == x));
        private string dbg164 = @"(string s, int x) => s.Length == x";

        [TestMethod]
        public void ToCSharp_Test164()
        {
            Assert.AreEqual(dbg164, expr164.ToCSharp());
        }

        private Expression expr165 = Expression.Lambda<Action>(Expression.Block(Expression.Default(typeof(void))));
        private string dbg165 = @"() =>
{
    ;
}";

        [TestMethod]
        public void ToCSharp_Test165()
        {
            Assert.AreEqual(dbg165, expr165.ToCSharp());
        }

        private Expression expr166 = Expression.Lambda(Expression.Constant(0), Expression.Parameter(typeof(int).MakeByRefType()));
        private string dbg166 = @"(ref int p0 /*(null)*/) => 0";

        [TestMethod]
        public void ToCSharp_Test166()
        {
            Assert.AreEqual(dbg166, expr166.ToCSharp());
        }

        private Expression expr167 = Expression.Rethrow();
        private string dbg167 = @"throw";

        [TestMethod]
        public void ToCSharp_Test167()
        {
            Assert.AreEqual(dbg167, expr167.ToCSharp());
        }

        private Expression expr168 = Expression.Throw(Expression.Parameter(typeof(Exception), "ex"));
        private string dbg168 = @"throw ex";

        [TestMethod]
        public void ToCSharp_Test168()
        {
            Assert.AreEqual(dbg168, expr168.ToCSharp());
        }

        private Expression expr169 = Expression.Block(Expression.Throw(Expression.Parameter(typeof(Exception), "ex")));
        private string dbg169 = @"{
    throw ex;
}";

        [TestMethod]
        public void ToCSharp_Test169()
        {
            Assert.AreEqual(dbg169, expr169.ToCSharp());
        }

        private Expression expr170 = Expression.Block(Expression.Empty(), Expression.Empty());
        private string dbg170 = @"{
    ;
    ;
}";

        [TestMethod]
        public void ToCSharp_Test170()
        {
            Assert.AreEqual(dbg170, expr170.ToCSharp());
        }

        private Expression expr171 = Expression.Block(Expression.Empty(), Expression.Constant(42));
        private string dbg171 = @"{
    ;
    /*return*/ 42/*;*/
}";

        [TestMethod]
        public void ToCSharp_Test171()
        {
            Assert.AreEqual(dbg171, expr171.ToCSharp());
        }

        private Expression expr172 = Expression.Block(new[] { Expression.Parameter(typeof(int), "x"), Expression.Parameter(typeof(int), "y"), Expression.Parameter(typeof(string), "s") }, Expression.Empty());
        private string dbg172 = @"{
    int x, y;
    string s;
    ;
}";

        [TestMethod]
        public void ToCSharp_Test172()
        {
            Assert.AreEqual(dbg172, expr172.ToCSharp());
        }

        private Expression expr173 = Expression.TryFinally(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("try")), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("finally")));
        private string dbg173 = @"try
{
    Console.WriteLine(""try"");
}
finally
{
    Console.WriteLine(""finally"");
}";

        [TestMethod]
        public void ToCSharp_Test173()
        {
            Assert.AreEqual(dbg173, expr173.ToCSharp());
        }

        private Expression expr174 = Expression.TryFault(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("try")), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("fault")));
        private string dbg174 = @"try
{
    Console.WriteLine(""try"");
}
fault
{
    Console.WriteLine(""fault"");
}";

        [TestMethod]
        public void ToCSharp_Test174()
        {
            Assert.AreEqual(dbg174, expr174.ToCSharp());
        }

        private Expression expr175 = Expression.TryCatch(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("try")), Expression.Catch(typeof(object), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("catch"))));
        private string dbg175 = @"try
{
    Console.WriteLine(""try"");
}
catch
{
    Console.WriteLine(""catch"");
}";

        [TestMethod]
        public void ToCSharp_Test175()
        {
            Assert.AreEqual(dbg175, expr175.ToCSharp());
        }

        private Expression expr176 = Expression.TryCatch(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("try")), Expression.Catch(typeof(Exception), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("catch"))));
        private string dbg176 = @"try
{
    Console.WriteLine(""try"");
}
catch (Exception)
{
    Console.WriteLine(""catch"");
}";

        [TestMethod]
        public void ToCSharp_Test176()
        {
            Assert.AreEqual(dbg176, expr176.ToCSharp());
        }

        private Expression expr177 = Expression.TryCatch(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("try")), Expression.Catch(Expression.Parameter(typeof(Exception), "ex"), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("catch"))));
        private string dbg177 = @"try
{
    Console.WriteLine(""try"");
}
catch (Exception ex)
{
    Console.WriteLine(""catch"");
}";

        [TestMethod]
        public void ToCSharp_Test177()
        {
            Assert.AreEqual(dbg177, expr177.ToCSharp());
        }

        private Expression expr178 = Expression.TryCatch(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("try")), Expression.Catch(Expression.Parameter(typeof(Exception), "ex"), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("catch")), Expression.Constant(true)));
        private string dbg178 = @"try
{
    Console.WriteLine(""try"");
}
catch (Exception ex) when (true)
{
    Console.WriteLine(""catch"");
}";

        [TestMethod]
        public void ToCSharp_Test178()
        {
            Assert.AreEqual(dbg178, expr178.ToCSharp());
        }

        private Expression expr179 = Expression.Loop(Expression.Empty());
        private string dbg179 = @"while (true)
    ;";

        [TestMethod]
        public void ToCSharp_Test179()
        {
            Assert.AreEqual(dbg179, expr179.ToCSharp());
        }

        private Expression expr180 = Expression.Loop(Expression.Block(Expression.Empty()));
        private string dbg180 = @"while (true)
{
    ;
}";

        [TestMethod]
        public void ToCSharp_Test180()
        {
            Assert.AreEqual(dbg180, expr180.ToCSharp());
        }

        private Expression expr181 = Expression.Loop(Expression.Break(_lbl1), _lbl1, _lbl2);
        private string dbg181 = @"while (true)
    break;";

        [TestMethod]
        public void ToCSharp_Test181()
        {
            Assert.AreEqual(dbg181, expr181.ToCSharp());
        }

        private Expression expr182 = Expression.Loop(Expression.Continue(_lbl2), _lbl1, _lbl2);
        private string dbg182 = @"while (true)
    continue;";

        [TestMethod]
        public void ToCSharp_Test182()
        {
            Assert.AreEqual(dbg182, expr182.ToCSharp());
        }

        private Expression expr183 = Expression.Loop(Expression.Loop(Expression.Break(_lbl1)), _lbl1, _lbl2);
        private string dbg183 = @"while (true)
{
    L0 /*(null)*/:
    while (true)
        goto L1;
}
L1 /*(null)*/:";

        [TestMethod]
        public void ToCSharp_Test183()
        {
            Assert.AreEqual(dbg183, expr183.ToCSharp());
        }

        private Expression expr184 = Expression.Loop(Expression.Loop(Expression.Block(Expression.Continue(_lbl2))), _lbl1, _lbl2);
        private string dbg184 = @"while (true)
{
    L0 /*(null)*/:
    while (true)
    {
        goto L0;
    }
}
L1 /*(null)*/:";

        [TestMethod]
        public void ToCSharp_Test184()
        {
            Assert.AreEqual(dbg184, expr184.ToCSharp());
        }

        private Expression expr185 = Expression.Loop(Expression.Block(Expression.Loop(Expression.Block(Expression.Continue(_lbl2)))), _lbl1, _lbl2);
        private string dbg185 = @"while (true)
{
    L0 /*(null)*/:
    while (true)
    {
        goto L0;
    }
}
L1 /*(null)*/:";

        [TestMethod]
        public void ToCSharp_Test185()
        {
            Assert.AreEqual(dbg185, expr185.ToCSharp());
        }

        private Expression expr186 = Expression.Break(_lbl1);
        private string dbg186 = @"goto L0;";

        [TestMethod]
        public void ToCSharp_Test186()
        {
            Assert.AreEqual(dbg186, expr186.ToCSharp());
        }

        private Expression expr187 = Expression.Continue(_lbl1);
        private string dbg187 = @"goto L0;";

        [TestMethod]
        public void ToCSharp_Test187()
        {
            Assert.AreEqual(dbg187, expr187.ToCSharp());
        }

        private Expression expr188 = Expression.Return(_lbl1);
        private string dbg188 = @"return;";

        [TestMethod]
        public void ToCSharp_Test188()
        {
            Assert.AreEqual(dbg188, expr188.ToCSharp());
        }

        private Expression expr189 = Expression.Break(_lbl3, Expression.Constant(1));
        private string dbg189 = @"goto L0/*(1)*/;";

        [TestMethod]
        public void ToCSharp_Test189()
        {
            Assert.AreEqual(dbg189, expr189.ToCSharp());
        }

        private Expression expr190 = Expression.Return(_lbl3, Expression.Constant(1));
        private string dbg190 = @"return 1;";

        [TestMethod]
        public void ToCSharp_Test190()
        {
            Assert.AreEqual(dbg190, expr190.ToCSharp());
        }

        private Expression expr191 = Expression.Switch(Expression.Constant(1), Expression.SwitchCase(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("case 2")), Expression.Constant(2)), Expression.SwitchCase(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("case 3 & 4")), Expression.Constant(3), Expression.Constant(4)));
        private string dbg191 = @"switch (1)
{
    case 2:
        Console.WriteLine(""case 2"");
        break;
    case 3:
    case 4:
        Console.WriteLine(""case 3 & 4"");
        break;
}";

        [TestMethod]
        public void ToCSharp_Test191()
        {
            Assert.AreEqual(dbg191, expr191.ToCSharp());
        }

        private Expression expr192 = Expression.Switch(Expression.Constant(1), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("default")), Expression.SwitchCase(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("case 2")), Expression.Constant(2)), Expression.SwitchCase(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("case 3 & 4")), Expression.Constant(3), Expression.Constant(4)));
        private string dbg192 = @"switch (1)
{
    case 2:
        Console.WriteLine(""case 2"");
        break;
    case 3:
    case 4:
        Console.WriteLine(""case 3 & 4"");
        break;
    default:
        Console.WriteLine(""default"");
        break;
}";

        [TestMethod]
        public void ToCSharp_Test192()
        {
            Assert.AreEqual(dbg192, expr192.ToCSharp());
        }

        private Expression expr193 = Expression.Switch(Expression.Constant(1), Expression.SwitchCase(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("case 2")), Expression.UnaryPlus(Expression.Constant(2))));
        private string dbg193 = @"switch (1)
{
    case /* +2 */:
        Console.WriteLine(""case 2"");
        break;
}";

        [TestMethod]
        public void ToCSharp_Test193()
        {
            Assert.AreEqual(dbg193, expr193.ToCSharp());
        }

        private Expression expr194 = Expression.ClearDebugInfo(Expression.SymbolDocument("foo"));
        private string dbg194 = @"";

        [TestMethod]
        public void ToCSharp_Test194()
        {
            Assert.AreEqual(dbg194, expr194.ToCSharp());
        }

        private Expression expr195 = Expression.DebugInfo(Expression.SymbolDocument("foo"), 1, 2, 3, 4);
        private string dbg195 = @"";

        [TestMethod]
        public void ToCSharp_Test195()
        {
            Assert.AreEqual(dbg195, expr195.ToCSharp());
        }

        private Expression expr196 = Expression.RuntimeVariables(Expression.Parameter(typeof(int)), Expression.Parameter(typeof(int)));
        private string dbg196 = @"RuntimeVariables(p0, p1)";

        [TestMethod]
        public void ToCSharp_Test196()
        {
            Assert.AreEqual(dbg196, expr196.ToCSharp());
        }

        private Expression expr197 = Expression.Dynamic(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.None, "foo", typeof(int), new Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[0]), typeof(int), Expression.Constant(1));
        private string dbg197 = @"1/*dynamic*/.foo";

        [TestMethod]
        public void ToCSharp_Test197()
        {
            Assert.AreEqual(dbg197, expr197.ToCSharp());
        }

        private Expression expr198 = Expression.Dynamic(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.None, ExpressionType.Add, typeof(int), new Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[0]), typeof(int), Expression.Constant(1), Expression.Constant(2));
        private string dbg198 = @"1 /*dynamic*/ + 2";

        [TestMethod]
        public void ToCSharp_Test198()
        {
            Assert.AreEqual(dbg198, expr198.ToCSharp());
        }

        private Expression expr199 = ((Expression<Func<IQueryable<int>, IQueryable<int>>>)(xs => xs.Where(x => x > 0)));
        private string dbg199 = @"(IQueryable<int> xs) => xs.Where<int>((int x) => x > 0)";

        [TestMethod]
        public void ToCSharp_Test199()
        {
            Assert.AreEqual(dbg199, expr199.ToCSharp());
        }

        private Expression expr200 = ((Expression<Func<IQueryable<int>, IQueryable>>)(xs => xs.Where(x => x > 0).Select(x => new { x })));
        private string dbg200 = @"(IQueryable<int> xs) => xs.Where<int>((int x) => x > 0).Select((int x) => new { x = x })";

        [TestMethod]
        public void ToCSharp_Test200()
        {
            Assert.AreEqual(dbg200, expr200.ToCSharp());
        }

        private Expression expr201 = Expression.Lambda(Expression.Block(Expression.Empty()));
        private string dbg201 = @"() =>
{
    ;
}";

        [TestMethod]
        public void ToCSharp_Test201()
        {
            Assert.AreEqual(dbg201, expr201.ToCSharp());
        }

        private Expression expr202 = Expression.Lambda(Expression.Block(Expression.Constant(1)));
        private string dbg202 = @"() =>
{
    return 1;
}";

        [TestMethod]
        public void ToCSharp_Test202()
        {
            Assert.AreEqual(dbg202, expr202.ToCSharp());
        }

        private Expression expr203 = Expression.Lambda(Expression.Block(Expression.Return(_lbl1)));
        private string dbg203 = @"() =>
{
    return;
}";

        [TestMethod]
        public void ToCSharp_Test203()
        {
            Assert.AreEqual(dbg203, expr203.ToCSharp());
        }

        private Expression expr204 = Expression.Lambda(Expression.Block(Expression.Return(_lbl3, Expression.Constant(1))));
        private string dbg204 = @"() =>
{
    return 1;
}";

        [TestMethod]
        public void ToCSharp_Test204()
        {
            Assert.AreEqual(dbg204, expr204.ToCSharp());
        }

        private Expression expr205 = Expression.IfThen(Expression.Constant(true), Expression.IfThenElse(Expression.Constant(true), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("if/if")), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("I'm not a dangler!"))));
        private string dbg205 = @"if (true)
    if (true)
        Console.WriteLine(""if/if"");
    else
        Console.WriteLine(""I'm not a dangler!"");";

        [TestMethod]
        public void ToCSharp_Test205()
        {
            Assert.AreEqual(dbg205, expr205.ToCSharp());
        }

        private Expression expr206 = Expression.IfThenElse(Expression.Constant(true), Expression.IfThen(Expression.Constant(true), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("if/if"))), Expression.Call(null, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Constant("don't dangle me!")));
        private string dbg206 = @"if (true)
{
    if (true)
        Console.WriteLine(""if/if"");
}
else
    Console.WriteLine(""don't dangle me!"");";

        [TestMethod]
        public void ToCSharp_Test206()
        {
            Assert.AreEqual(dbg206, expr206.ToCSharp());
        }

    }
}