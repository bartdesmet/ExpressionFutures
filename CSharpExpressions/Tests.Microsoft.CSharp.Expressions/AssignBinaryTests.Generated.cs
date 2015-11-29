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
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.AddAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.AddAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.AddAssign(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_SubtractAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.SubtractAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.SubtractAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.SubtractAssign(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_MultiplyAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.MultiplyAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.MultiplyAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.MultiplyAssign(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_DivideAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.DivideAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.DivideAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.DivideAssign(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_ModuloAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.ModuloAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.ModuloAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.ModuloAssign(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_AndAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.AndAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.AndAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.AndAssign(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_OrAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.OrAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.OrAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.OrAssign(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_ExclusiveOrAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.ExclusiveOrAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.ExclusiveOrAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.ExclusiveOrAssign(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_LeftShiftAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.LeftShiftAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.LeftShiftAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.LeftShiftAssign(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_RightShiftAssign()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.RightShiftAssign(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.RightShiftAssign(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.RightShiftAssign(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_AddAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.AddAssignChecked(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.AddAssignChecked(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.AddAssignChecked(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_MultiplyAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.MultiplyAssignChecked(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.MultiplyAssignChecked(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.MultiplyAssignChecked(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

        [TestMethod]
        public void AssignBinary_Factory_SubtractAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));
            var b = Expression.Constant(1);
            var x = Expression.Parameter(typeof(int));
            var z = Expression.Lambda(x, x);

            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var c = Expression.Lambda(x, x);

                var a1 = CSharpExpression.SubtractAssignChecked(l, r);
                Assert.AreSame(l, a1.Left);
                Assert.AreSame(r, a1.Right);
                Assert.IsNull(a1.Method);
                Assert.IsNull(a1.Conversion);

                var a2 = CSharpExpression.SubtractAssignChecked(l, r, m);
                Assert.AreSame(l, a2.Left);
                Assert.AreSame(r, a2.Right);
                Assert.AreSame(m, a2.Method);
                Assert.IsNull(a2.Conversion);

                var a3 = CSharpExpression.SubtractAssignChecked(l, r, m, c);
                Assert.AreSame(l, a3.Left);
                Assert.AreSame(r, a3.Right);
                Assert.AreSame(m, a3.Method);
                Assert.AreSame(c, a3.Conversion);

                var a4 = a3.Update(l, c, r);
                Assert.AreSame(a4, a3);

                var a5 = a3.Update(a, c, r);
                Assert.AreSame(a, a5.Left);
                Assert.AreSame(r, a5.Right);
                Assert.AreSame(c, a5.Conversion);
                Assert.AreSame(m, a5.Method);

                var a6 = a3.Update(l, z, r);
                Assert.AreSame(l, a6.Left);
                Assert.AreSame(r, a6.Right);
                Assert.AreSame(z, a6.Conversion);
                Assert.AreSame(m, a6.Method);

                var a7 = a3.Update(l, c, b);
                Assert.AreSame(l, a7.Left);
                Assert.AreSame(b, a7.Right);
                Assert.AreSame(c, a7.Conversion);
                Assert.AreSame(m, a7.Method);
            }
        }

    }
}