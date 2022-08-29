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
    partial class AssignUnaryTests
    {
        [Fact]
        public void AssignUnary_Factory_PreIncrementAssign()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PreIncrementAssign(o);
                Assert.Same(o, a1.Operand);
                Assert.Null(a1.Method);

                var a2 = CSharpExpression.PreIncrementAssign(o, m);
                Assert.Same(o, a2.Operand);
                Assert.Same(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.Same(a3, a2);

                var a4 = a2.Update(a);
                Assert.Same(a, a4.Operand);
                Assert.Same(m, a4.Method);
            }
        }

        [Fact]
        public void AssignUnary_Factory_PreDecrementAssign()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PreDecrementAssign(o);
                Assert.Same(o, a1.Operand);
                Assert.Null(a1.Method);

                var a2 = CSharpExpression.PreDecrementAssign(o, m);
                Assert.Same(o, a2.Operand);
                Assert.Same(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.Same(a3, a2);

                var a4 = a2.Update(a);
                Assert.Same(a, a4.Operand);
                Assert.Same(m, a4.Method);
            }
        }

        [Fact]
        public void AssignUnary_Factory_PostIncrementAssign()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PostIncrementAssign(o);
                Assert.Same(o, a1.Operand);
                Assert.Null(a1.Method);

                var a2 = CSharpExpression.PostIncrementAssign(o, m);
                Assert.Same(o, a2.Operand);
                Assert.Same(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.Same(a3, a2);

                var a4 = a2.Update(a);
                Assert.Same(a, a4.Operand);
                Assert.Same(m, a4.Method);
            }
        }

        [Fact]
        public void AssignUnary_Factory_PostDecrementAssign()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PostDecrementAssign(o);
                Assert.Same(o, a1.Operand);
                Assert.Null(a1.Method);

                var a2 = CSharpExpression.PostDecrementAssign(o, m);
                Assert.Same(o, a2.Operand);
                Assert.Same(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.Same(a3, a2);

                var a4 = a2.Update(a);
                Assert.Same(a, a4.Operand);
                Assert.Same(m, a4.Method);
            }
        }

        [Fact]
        public void AssignUnary_Factory_PreIncrementAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PreIncrementAssignChecked(o);
                Assert.Same(o, a1.Operand);
                Assert.Null(a1.Method);

                var a2 = CSharpExpression.PreIncrementAssignChecked(o, m);
                Assert.Same(o, a2.Operand);
                Assert.Same(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.Same(a3, a2);

                var a4 = a2.Update(a);
                Assert.Same(a, a4.Operand);
                Assert.Same(m, a4.Method);
            }
        }

        [Fact]
        public void AssignUnary_Factory_PreDecrementAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PreDecrementAssignChecked(o);
                Assert.Same(o, a1.Operand);
                Assert.Null(a1.Method);

                var a2 = CSharpExpression.PreDecrementAssignChecked(o, m);
                Assert.Same(o, a2.Operand);
                Assert.Same(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.Same(a3, a2);

                var a4 = a2.Update(a);
                Assert.Same(a, a4.Operand);
                Assert.Same(m, a4.Method);
            }
        }

        [Fact]
        public void AssignUnary_Factory_PostIncrementAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PostIncrementAssignChecked(o);
                Assert.Same(o, a1.Operand);
                Assert.Null(a1.Method);

                var a2 = CSharpExpression.PostIncrementAssignChecked(o, m);
                Assert.Same(o, a2.Operand);
                Assert.Same(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.Same(a3, a2);

                var a4 = a2.Update(a);
                Assert.Same(a, a4.Operand);
                Assert.Same(m, a4.Method);
            }
        }

        [Fact]
        public void AssignUnary_Factory_PostDecrementAssignChecked()
        {
            var a = Expression.Parameter(typeof(int));

            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                var a1 = CSharpExpression.PostDecrementAssignChecked(o);
                Assert.Same(o, a1.Operand);
                Assert.Null(a1.Method);

                var a2 = CSharpExpression.PostDecrementAssignChecked(o, m);
                Assert.Same(o, a2.Operand);
                Assert.Same(m, a2.Method);

                var a3 = a2.Update(o);
                Assert.Same(a3, a2);

                var a4 = a2.Update(a);
                Assert.Same(a, a4.Operand);
                Assert.Same(m, a4.Method);
            }
        }

    }
}
