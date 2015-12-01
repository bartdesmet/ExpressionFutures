// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using static Tests.ReflectionUtils;

namespace Tests
{
    partial class AssignBinaryTests
    {
        [TestMethod]
        public void AssignBinary_Factory_AddAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.AddAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.AddAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.AddAssign(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.AddAssign(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_SubtractAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.SubtractAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.SubtractAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.SubtractAssign(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.SubtractAssign(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_MultiplyAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.MultiplyAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.MultiplyAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.MultiplyAssign(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.MultiplyAssign(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_DivideAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.DivideAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.DivideAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.DivideAssign(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.DivideAssign(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_ModuloAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.ModuloAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.ModuloAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.ModuloAssign(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.ModuloAssign(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_AndAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.AndAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.AndAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.AndAssign(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.AndAssign(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_OrAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.OrAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.OrAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.OrAssign(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.OrAssign(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_ExclusiveOrAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.ExclusiveOrAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.ExclusiveOrAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.ExclusiveOrAssign(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.ExclusiveOrAssign(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_LeftShiftAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.LeftShiftAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.LeftShiftAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.LeftShiftAssign(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.LeftShiftAssign(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_RightShiftAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.RightShiftAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.RightShiftAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.RightShiftAssign(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.RightShiftAssign(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_AddAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.AddAssignChecked(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.AddAssignChecked(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.AddAssignChecked(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.AddAssignChecked(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_MultiplyAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.MultiplyAssignChecked(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.MultiplyAssignChecked(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.MultiplyAssignChecked(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.MultiplyAssignChecked(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_SubtractAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var dl = Expression.Lambda(x, x);
            var df = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var cl = Expression.Lambda(x, x);
                var cf = Expression.Lambda(x, x);

                var a1 = CSharpExpression.SubtractAssignChecked(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.LeftConversion);
				Assert.IsNull(a1.FinalConversion);

                var a2 = CSharpExpression.SubtractAssignChecked(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.LeftConversion);
				Assert.IsNull(a2.FinalConversion);

                var a3 = CSharpExpression.SubtractAssignChecked(l, r, m, cf);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(cf, a3.FinalConversion);
				Assert.IsNull(a3.LeftConversion);

                var a4 = CSharpExpression.SubtractAssignChecked(l, r, m, cf, cl);
                Assert.AreSame(l, a4.Left);
                Assert.AreSame(r, a4.Right);
                Assert.AreSame(m, a4.Method);
                Assert.AreSame(cf, a4.FinalConversion);
				Assert.AreSame(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.AreSame(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.AreSame(a, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(m, a6.Method);
                Assert.AreSame(cf, a6.FinalConversion);
				Assert.AreSame(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(r, a7.Right);
                Assert.AreSame(m, a7.Method);
                Assert.AreSame(dl, a7.LeftConversion);
				Assert.AreSame(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.AreSame(l, a8.Left);
                Assert.AreSame(b, a8.Right);
                Assert.AreSame(m, a8.Method);
                Assert.AreSame(cl, a8.LeftConversion);
				Assert.AreSame(cf, a8.FinalConversion);

				var a9 = a4.Update(l, cl, r, df);
                Assert.AreSame(l, a9.Left);
                Assert.AreSame(r, a9.Right);
                Assert.AreSame(m, a9.Method);
                Assert.AreSame(cl, a9.LeftConversion);
				Assert.AreSame(df, a9.FinalConversion);
            }
        }

    }
}