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
    partial class AssignUnaryTests
    {
        [TestMethod]
        public void AssignUnary_Factory_PreIncrementAssign()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PreIncrementAssign(o);
                Assert.AreSame(o, a1.Operand);
                Assert.IsNull(a1.Method);

                var a2 = CSharpExpression.PreIncrementAssign(o, m);
                Assert.AreSame(o, a2.Operand);
                Assert.AreSame(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.AreSame(a3, a2);

                var a4 = a2.Update(a);
                Assert.AreSame(a, a4.Operand);
                Assert.AreSame(m, a4.Method);
            }
        }

        [TestMethod]
        public void AssignUnary_Factory_PreDecrementAssign()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PreDecrementAssign(o);
                Assert.AreSame(o, a1.Operand);
                Assert.IsNull(a1.Method);

                var a2 = CSharpExpression.PreDecrementAssign(o, m);
                Assert.AreSame(o, a2.Operand);
                Assert.AreSame(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.AreSame(a3, a2);

                var a4 = a2.Update(a);
                Assert.AreSame(a, a4.Operand);
                Assert.AreSame(m, a4.Method);
            }
        }

        [TestMethod]
        public void AssignUnary_Factory_PostIncrementAssign()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PostIncrementAssign(o);
                Assert.AreSame(o, a1.Operand);
                Assert.IsNull(a1.Method);

                var a2 = CSharpExpression.PostIncrementAssign(o, m);
                Assert.AreSame(o, a2.Operand);
                Assert.AreSame(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.AreSame(a3, a2);

                var a4 = a2.Update(a);
                Assert.AreSame(a, a4.Operand);
                Assert.AreSame(m, a4.Method);
            }
        }

        [TestMethod]
        public void AssignUnary_Factory_PostDecrementAssign()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PostDecrementAssign(o);
                Assert.AreSame(o, a1.Operand);
                Assert.IsNull(a1.Method);

                var a2 = CSharpExpression.PostDecrementAssign(o, m);
                Assert.AreSame(o, a2.Operand);
                Assert.AreSame(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.AreSame(a3, a2);

                var a4 = a2.Update(a);
                Assert.AreSame(a, a4.Operand);
                Assert.AreSame(m, a4.Method);
            }
        }

        [TestMethod]
        public void AssignUnary_Factory_PreIncrementCheckedAssign()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PreIncrementCheckedAssign(o);
                Assert.AreSame(o, a1.Operand);
                Assert.IsNull(a1.Method);

                var a2 = CSharpExpression.PreIncrementCheckedAssign(o, m);
                Assert.AreSame(o, a2.Operand);
                Assert.AreSame(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.AreSame(a3, a2);

                var a4 = a2.Update(a);
                Assert.AreSame(a, a4.Operand);
                Assert.AreSame(m, a4.Method);
            }
        }

        [TestMethod]
        public void AssignUnary_Factory_PreDecrementCheckedAssign()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PreDecrementCheckedAssign(o);
                Assert.AreSame(o, a1.Operand);
                Assert.IsNull(a1.Method);

                var a2 = CSharpExpression.PreDecrementCheckedAssign(o, m);
                Assert.AreSame(o, a2.Operand);
                Assert.AreSame(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.AreSame(a3, a2);

                var a4 = a2.Update(a);
                Assert.AreSame(a, a4.Operand);
                Assert.AreSame(m, a4.Method);
            }
        }

        [TestMethod]
        public void AssignUnary_Factory_PostIncrementCheckedAssign()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PostIncrementCheckedAssign(o);
                Assert.AreSame(o, a1.Operand);
                Assert.IsNull(a1.Method);

                var a2 = CSharpExpression.PostIncrementCheckedAssign(o, m);
                Assert.AreSame(o, a2.Operand);
                Assert.AreSame(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.AreSame(a3, a2);

                var a4 = a2.Update(a);
                Assert.AreSame(a, a4.Operand);
                Assert.AreSame(m, a4.Method);
            }
        }

        [TestMethod]
        public void AssignUnary_Factory_PostDecrementCheckedAssign()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PostDecrementCheckedAssign(o);
                Assert.AreSame(o, a1.Operand);
                Assert.IsNull(a1.Method);

                var a2 = CSharpExpression.PostDecrementCheckedAssign(o, m);
                Assert.AreSame(o, a2.Operand);
                Assert.AreSame(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.AreSame(a3, a2);

                var a4 = a2.Update(a);
                Assert.AreSame(a, a4.Operand);
                Assert.AreSame(m, a4.Method);
            }
        }

    }
}