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
        public void AssignUnary_Factory_PreIncrementAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PreIncrementAssignChecked(o);
                Assert.AreSame(o, a1.Operand);
                Assert.IsNull(a1.Method);

                var a2 = CSharpExpression.PreIncrementAssignChecked(o, m);
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
        public void AssignUnary_Factory_PreDecrementAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PreDecrementAssignChecked(o);
                Assert.AreSame(o, a1.Operand);
                Assert.IsNull(a1.Method);

                var a2 = CSharpExpression.PreDecrementAssignChecked(o, m);
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
        public void AssignUnary_Factory_PostIncrementAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PostIncrementAssignChecked(o);
                Assert.AreSame(o, a1.Operand);
                Assert.IsNull(a1.Method);

                var a2 = CSharpExpression.PostIncrementAssignChecked(o, m);
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
        public void AssignUnary_Factory_PostDecrementAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PostDecrementAssignChecked(o);
                Assert.AreSame(o, a1.Operand);
                Assert.IsNull(a1.Method);

                var a2 = CSharpExpression.PostDecrementAssignChecked(o, m);
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