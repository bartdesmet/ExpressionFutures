// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using Microsoft.CSharp.Expressions;

namespace Tests
{
    partial class OptimizerTests
    {
        [TestMethod]
        public void Optimizer_Blocks_0()
        {
            var expression = Expression.Block(E);
            var expected = E;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_1()
        {
            var expression = Expression.Block(E, E);
            var expected = E;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_2()
        {
            var expression = Expression.Block(CW, E);
            var expected = CW;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_3()
        {
            var expression = Expression.Block(E, CW);
            var expected = CW;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_4()
        {
            var expression = Expression.Block(E, CW, E);
            var expected = CW;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_5()
        {
            var expression = Expression.Block(E, CWI(1), E, CWI(2), E);
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
            var expression = Expression.Block(Expression.Block(CWI(1), E), E, Expression.Block(CWI(2), E));
            var expected = Expression.Block(CWI(1), CWI(2));
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Blocks_11()
        {
            var expression = Expression.Block(new[] { P1 }, E);
            var expected = Expression.Block(new[] { P1 }, E);
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
            var expression = Expression.Block(new[] { P1 }, Expression.Block(CW, E));
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
            var expression = CSharpExpression.Block(new[] { E }, RET);
            var expected = E;
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

        [TestMethod]
        public void Optimizer_Loops_0()
        {
            var expression = Expression.Loop(CW);
            var expected = Expression.Loop(CW);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_1()
        {
            var expression = Expression.Loop(CW, BRK);
            var expected = Expression.Loop(CW);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_2()
        {
            var expression = Expression.Loop(CW, BRK, CNT);
            var expected = Expression.Loop(CW);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_3()
        {
            var expression = Expression.Loop(Expression.Block(CW, Expression.Break(BRK)), BRK, CNT);
            var expected = Expression.Loop(Expression.Block(CW, Expression.Break(BRK)), BRK, null);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_4()
        {
            var expression = Expression.Loop(Expression.Block(CW, Expression.Continue(CNT)), BRK, CNT);
            var expected = Expression.Loop(Expression.Block(CW, Expression.Continue(CNT)), null, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_5()
        {
            var expression = Expression.Loop(Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            var expected = Expression.Loop(Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_6()
        {
            var expression = CSharpStatement.While(B, Expression.Block(CW, Expression.Break(BRK)), BRK, CNT);
            var expected = CSharpStatement.While(B, Expression.Block(CW, Expression.Break(BRK)), BRK, null);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_7()
        {
            var expression = CSharpStatement.While(B, Expression.Block(CW, Expression.Continue(CNT)), BRK, CNT);
            var expected = CSharpStatement.While(B, Expression.Block(CW, Expression.Continue(CNT)), null, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_8()
        {
            var expression = CSharpStatement.While(B, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            var expected = CSharpStatement.While(B, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_9()
        {
            var expression = CSharpStatement.Do(Expression.Block(CW, Expression.Break(BRK)), B, BRK, CNT);
            var expected = CSharpStatement.Do(Expression.Block(CW, Expression.Break(BRK)), B, BRK, null);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_10()
        {
            var expression = CSharpStatement.Do(Expression.Block(CW, Expression.Continue(CNT)), B, BRK, CNT);
            var expected = CSharpStatement.Do(Expression.Block(CW, Expression.Continue(CNT)), B, null, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_11()
        {
            var expression = CSharpStatement.Do(Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), B, BRK, CNT);
            var expected = CSharpStatement.Do(Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), B, BRK, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_12()
        {
            var expression = CSharpStatement.For(null, B, null, Expression.Block(CW, Expression.Break(BRK)), BRK, CNT);
            var expected = CSharpStatement.For(null, B, null, Expression.Block(CW, Expression.Break(BRK)), BRK, null);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_13()
        {
            var expression = CSharpStatement.For(null, B, null, Expression.Block(CW, Expression.Continue(CNT)), BRK, CNT);
            var expected = CSharpStatement.For(null, B, null, Expression.Block(CW, Expression.Continue(CNT)), null, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_14()
        {
            var expression = CSharpStatement.For(null, B, null, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            var expected = CSharpStatement.For(null, B, null, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_15()
        {
            var expression = CSharpStatement.ForEach(P1, C, Expression.Block(CW, Expression.Break(BRK)), BRK, CNT);
            var expected = CSharpStatement.ForEach(P1, C, Expression.Block(CW, Expression.Break(BRK)), BRK, null);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_16()
        {
            var expression = CSharpStatement.ForEach(P1, C, Expression.Block(CW, Expression.Continue(CNT)), BRK, CNT);
            var expected = CSharpStatement.ForEach(P1, C, Expression.Block(CW, Expression.Continue(CNT)), null, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_17()
        {
            var expression = CSharpStatement.ForEach(P1, C, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            var expected = CSharpStatement.ForEach(P1, C, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_18()
        {
            var expression = CSharpStatement.For(new[] { Expression.Assign(P1, Expression.Constant(0)) }, B, new[] { CWI(1), Expression.PostIncrementAssign(P1) }, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            var expected = CSharpStatement.For(new[] { Expression.Assign(P1, Expression.Constant(0)) }, B, new[] { CWI(1), Expression.PreIncrementAssign(P1) }, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_19()
        {
            var expression = CSharpStatement.For(new[] { Expression.Assign(P1, Expression.Constant(0)) }, B, new[] { Expression.PostDecrementAssign(P1), CWI(1) }, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            var expected = CSharpStatement.For(new[] { Expression.Assign(P1, Expression.Constant(0)) }, B, new[] { Expression.PreDecrementAssign(P1), CWI(1) }, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_20()
        {
            var expression = CSharpStatement.For(new[] { Expression.Assign(P1, Expression.Constant(0)) }, B, new[] { CWI(1), CSharpExpression.PostIncrementAssign(P1) }, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            var expected = CSharpStatement.For(new[] { Expression.Assign(P1, Expression.Constant(0)) }, B, new[] { CWI(1), CSharpExpression.PreIncrementAssign(P1) }, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_21()
        {
            var expression = CSharpStatement.For(new[] { Expression.Assign(P1, Expression.Constant(0)) }, B, new[] { CWI(1), CSharpExpression.PostDecrementAssign(P1), CWI(2) }, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            var expected = CSharpStatement.For(new[] { Expression.Assign(P1, Expression.Constant(0)) }, B, new[] { CWI(1), CSharpExpression.PreDecrementAssign(P1), CWI(2) }, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_22()
        {
            var expression = CSharpStatement.For(new[] { Expression.Assign(P1, Expression.Constant(0)) }, B, new[] { CWI(1), Expression.PreIncrementAssign(P1) }, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            var expected = CSharpStatement.For(new[] { Expression.Assign(P1, Expression.Constant(0)) }, B, new[] { CWI(1), Expression.PreIncrementAssign(P1) }, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Loops_23()
        {
            var expression = CSharpStatement.For(new[] { Expression.Assign(P1, Expression.Constant(0)) }, B, new[] { CWI(1), CSharpExpression.PreDecrementAssign(P1), CWI(2) }, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            var expected = CSharpStatement.For(new[] { Expression.Assign(P1, Expression.Constant(0)) }, B, new[] { CWI(1), CSharpExpression.PreDecrementAssign(P1), CWI(2) }, Expression.Block(CW, Expression.Break(BRK), Expression.Continue(CNT)), BRK, CNT);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_0()
        {
            var expression = Expression.TryFinally(E, E);
            var expected = E;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_1()
        {
            var expression = Expression.TryFinally(CW, E);
            var expected = CW;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_2()
        {
            var expression = Expression.TryFinally(CI(1), E);
            var expected = CI(1);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_3()
        {
            var expression = Expression.TryFinally(E, CW);
            var expected = Expression.TryFinally(E, CW);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_4()
        {
            var expression = Expression.TryFault(E, E);
            var expected = E;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_5()
        {
            var expression = Expression.TryFault(CW, E);
            var expected = CW;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_6()
        {
            var expression = Expression.TryFault(CI(1), E);
            var expected = CI(1);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_7()
        {
            var expression = Expression.TryFault(E, CW);
            var expected = Expression.TryFault(E, CW);
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_8()
        {
            var expression = Expression.TryCatch(E, Expression.Catch(typeof(Exception), CW));
            var expected = E;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_9()
        {
            var expression = Expression.TryCatch(CW, Expression.Catch(typeof(Exception), CW));
            var expected = Expression.TryCatch(CW, Expression.Catch(typeof(Exception), CW));
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_10()
        {
            var expression = Expression.TryCatch(CW, Expression.Catch(typeof(Exception), E));
            var expected = Expression.TryCatch(CW, Expression.Catch(typeof(Exception), E));
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_11()
        {
            var expression = Expression.TryCatchFinally(E, E, Expression.Catch(typeof(Exception), CW));
            var expected = E;
            AssertOptimize(expression, expected);
        }

        [TestMethod]
        public void Optimizer_Try_12()
        {
            var expression = Expression.TryCatchFinally(E, CW, Expression.Catch(typeof(Exception), CW));
            var expected = Expression.TryFinally(E, CW);
            AssertOptimize(expression, expected);
        }

    }
}
