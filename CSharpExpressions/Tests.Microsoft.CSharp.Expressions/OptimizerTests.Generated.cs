// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using Microsoft.CSharp.Expressions;

namespace Tests
{
    partial class OptimizerTests
    {
        [TestMethod]
        public void Optimizer_Blocks_0()
        {
            var expression = Expression.Block(Expression.Empty());
            var expected = Expression.Empty();
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_1()
        {
            var expression = Expression.Block(Expression.Empty(), Expression.Empty());
            var expected = Expression.Empty();
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_2()
        {
            var expression = Expression.Block(CW, Expression.Empty());
            var expected = CW;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_3()
        {
            var expression = Expression.Block(Expression.Empty(), CW);
            var expected = CW;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_4()
        {
            var expression = Expression.Block(Expression.Empty(), CW, Expression.Empty());
            var expected = CW;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_5()
        {
            var expression = Expression.Block(Expression.Empty(), CWI(1), Expression.Empty(), CWI(2), Expression.Empty());
            var expected = Expression.Block(CWI(1), CWI(2));
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_6()
        {
            var expression = Expression.Block(CWI(1), CWI(2));
            var expected = Expression.Block(CWI(1), CWI(2));
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_7()
        {
            var expression = Expression.Block(Expression.Block(CWI(1)), CWI(2));
            var expected = Expression.Block(CWI(1), CWI(2));
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_8()
        {
            var expression = Expression.Block(CWI(1), Expression.Block(CWI(2)));
            var expected = Expression.Block(CWI(1), CWI(2));
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_9()
        {
            var expression = Expression.Block(Expression.Block(CWI(1), CWI(2)));
            var expected = Expression.Block(CWI(1), CWI(2));
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_10()
        {
            var expression = Expression.Block(Expression.Block(CWI(1), Expression.Empty()), Expression.Empty(), Expression.Block(CWI(2), Expression.Empty()));
            var expected = Expression.Block(CWI(1), CWI(2));
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_11()
        {
            var expression = Expression.Block(new[] { P1 }, Expression.Empty());
            var expected = Expression.Block(new[] { P1 }, Expression.Empty());
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_12()
        {
            var expression = Expression.Block(new[] { P1 }, CW);
            var expected = Expression.Block(new[] { P1 }, CW);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_13()
        {
            var expression = Expression.Block(new[] { P1 }, Expression.Block(CW));
            var expected = Expression.Block(new[] { P1 }, CW);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_14()
        {
            var expression = Expression.Block(new[] { P1 }, Expression.Block(CW, Expression.Empty()));
            var expected = Expression.Block(new[] { P1 }, CW);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_15()
        {
            var expression = Expression.Block(new[] { P1 }, Expression.Block(new[] { P2 }, CW));
            var expected = Expression.Block(new[] { P1 }, Expression.Block(new[] { P2 }, CW));
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_16()
        {
            var expression = CSharpExpression.Block(new[] { Expression.Empty() }, RET);
            var expected = Expression.Empty();
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_17()
        {
            var expression = CSharpExpression.Block(new[] { Expression.Block(CW) }, RET);
            var expected = CW;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_18()
        {
            var expression = Expression.Block(CSharpExpression.Block(new[] { CW }, RET));
            var expected = CW;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_19()
        {
            var expression = CSharpExpression.Block(new[] { P1 }, new[] { Expression.Block(CW) }, RET);
            var expected = CSharpExpression.Block(new[] { P1 }, new[] { CW }, RET);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_20()
        {
            var expression = CSharpExpression.Block(new[] { Expression.Block(new[] { P1 }, CW) }, RET);
            var expected = Expression.Block(new[] { P1 }, CW);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_21()
        {
            var expression = Expression.Block(new[] { CSharpExpression.Block(new[] { P1 }, new[] { CW }, RET) });
            var expected = CSharpExpression.Block(new[] { P1 }, new[] { CW }, RET);
            AssertOptimize(expression, expected);
        }

    }
}
