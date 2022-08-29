// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq.Expressions;
using Xunit;
using static Tests.ReflectionUtils;

namespace Tests
{
    partial class AssignBinaryTests
    {
        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.AddAssign(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.AddAssign(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.AddAssign(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.SubtractAssign(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.SubtractAssign(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.SubtractAssign(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.MultiplyAssign(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.MultiplyAssign(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.MultiplyAssign(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.DivideAssign(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.DivideAssign(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.DivideAssign(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.ModuloAssign(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.ModuloAssign(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.ModuloAssign(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.AndAssign(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.AndAssign(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.AndAssign(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.OrAssign(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.OrAssign(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.OrAssign(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.ExclusiveOrAssign(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.ExclusiveOrAssign(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.ExclusiveOrAssign(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.LeftShiftAssign(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.LeftShiftAssign(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.LeftShiftAssign(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.RightShiftAssign(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.RightShiftAssign(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.RightShiftAssign(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.AddAssignChecked(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.AddAssignChecked(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.AddAssignChecked(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.MultiplyAssignChecked(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.MultiplyAssignChecked(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.MultiplyAssignChecked(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

        [Fact]
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
                Assert.Same(l, a1.Left);
                Assert.Same(r, a1.Right);
                Assert.Null(a1.Method);
                Assert.Null(a1.LeftConversion);
                Assert.Null(a1.FinalConversion);

                var a2 = CSharpExpression.SubtractAssignChecked(l, r, m);
                Assert.Same(l, a2.Left);
                Assert.Same(r, a2.Right);
                Assert.Same(m, a2.Method);
                Assert.Null(a2.LeftConversion);
                Assert.Null(a2.FinalConversion);

                var a3 = CSharpExpression.SubtractAssignChecked(l, r, m, cf);
                Assert.Same(l, a3.Left);
                Assert.Same(r, a3.Right);
                Assert.Same(m, a3.Method);
                Assert.Same(cf, a3.FinalConversion);
                Assert.Null(a3.LeftConversion);

                var a4 = CSharpExpression.SubtractAssignChecked(l, r, m, cf, cl);
                Assert.Same(l, a4.Left);
                Assert.Same(r, a4.Right);
                Assert.Same(m, a4.Method);
                Assert.Same(cf, a4.FinalConversion);
                Assert.Same(cl, a4.LeftConversion);

                var a5 = a4.Update(l, cl, r, cf);
                Assert.Same(a5, a4);

                var a6 = a4.Update(a, cl, r, cf);
                Assert.Same(a, a6.Left);
                Assert.Same(r, a6.Right);
                Assert.Same(m, a6.Method);
                Assert.Same(cf, a6.FinalConversion);
                Assert.Same(cl, a6.LeftConversion);

                var a7 = a4.Update(l, dl, r, cf);
                Assert.Same(l, a7.Left);
                Assert.Same(r, a7.Right);
                Assert.Same(m, a7.Method);
                Assert.Same(dl, a7.LeftConversion);
                Assert.Same(cf, a7.FinalConversion);

                var a8 = a4.Update(l, cl, b, cf);
                Assert.Same(l, a8.Left);
                Assert.Same(b, a8.Right);
                Assert.Same(m, a8.Method);
                Assert.Same(cl, a8.LeftConversion);
                Assert.Same(cf, a8.FinalConversion);

                var a9 = a4.Update(l, cl, r, df);
                Assert.Same(l, a9.Left);
                Assert.Same(r, a9.Right);
                Assert.Same(m, a9.Method);
                Assert.Same(cl, a9.LeftConversion);
                Assert.Same(df, a9.FinalConversion);
            }
        }

    }
}
