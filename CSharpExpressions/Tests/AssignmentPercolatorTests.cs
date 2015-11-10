// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class AssignmentPercolatorTests
    {
        [TestMethod]
        public void AssignmentPercolator_Block()
        {
            var b = Expression.Block(Expression.Empty(), Expression.Constant(42));
            var v = Expression.Parameter(typeof(int));
            var e = Expression.Assign(v, b);
            var r = AssignmentPercolator.Percolate(e);

            Assert.AreEqual(ExpressionType.Block, r.NodeType);
            var t = (BlockExpression)r;
            Assert.AreSame(b.Expressions[0], t.Expressions[0]);
            Assert.AreEqual(ExpressionType.Assign, t.Expressions[1].NodeType);
            var a = (BinaryExpression)t.Expressions[1];
            Assert.AreSame(v, a.Left);
            Assert.AreSame(b.Expressions[1], a.Right);
        }

        [TestMethod]
        public void AssignmentPercolator_Conditional()
        {
            var c = Expression.Condition(Expression.Constant(true), Expression.Constant(42), Expression.Constant(43));
            var v = Expression.Parameter(typeof(int));
            var e = Expression.Assign(v, c);
            var r = AssignmentPercolator.Percolate(e);

            Assert.AreEqual(ExpressionType.Conditional, r.NodeType);
            var t = (ConditionalExpression)r;
            Assert.AreSame(c.Test, t.Test);
            Assert.AreEqual(ExpressionType.Assign, t.IfTrue.NodeType);
            Assert.AreEqual(ExpressionType.Assign, t.IfFalse.NodeType);
            var a1 = (BinaryExpression)t.IfTrue;
            var a2 = (BinaryExpression)t.IfFalse;
            Assert.AreSame(v, a1.Left);
            Assert.AreSame(v, a2.Left);
            Assert.AreSame(c.IfTrue, a1.Right);
            Assert.AreSame(c.IfFalse, a2.Right);
        }

        [TestMethod]
        public void AssignmentPercolator_TryFinally()
        {
            var c = Expression.TryFinally(Expression.Constant(42), Expression.Empty());
            var v = Expression.Parameter(typeof(int));
            var e = Expression.Assign(v, c);
            var r = AssignmentPercolator.Percolate(e);

            Assert.AreEqual(ExpressionType.Try, r.NodeType);
            var t = (TryExpression)r;
            Assert.AreSame(c.Finally, t.Finally);
            Assert.AreEqual(ExpressionType.Assign, t.Body.NodeType);
            var a = (BinaryExpression)t.Body;
            Assert.AreSame(v, a.Left);
            Assert.AreSame(c.Body, a.Right);
        }

        [TestMethod]
        public void AssignmentPercolator_TryCatch()
        {
            var c = Expression.TryCatch(Expression.Constant(42), Expression.Catch(typeof(Exception), Expression.Constant(43)));
            var v = Expression.Parameter(typeof(int));
            var e = Expression.Assign(v, c);
            var r = AssignmentPercolator.Percolate(e);

            Assert.AreEqual(ExpressionType.Try, r.NodeType);
            var t = (TryExpression)r;
            Assert.AreEqual(ExpressionType.Assign, t.Body.NodeType);
            Assert.AreEqual(ExpressionType.Assign, t.Handlers[0].Body.NodeType);
            var a1 = (BinaryExpression)t.Body;
            var a2 = (BinaryExpression)t.Handlers[0].Body;
            Assert.AreSame(v, a1.Left);
            Assert.AreSame(v, a2.Left);
            Assert.AreSame(c.Body, a1.Right);
            Assert.AreSame(c.Handlers[0].Body, a2.Right);
        }

        [TestMethod]
        public void AssignmentPercolator_Switch()
        {
            var c = Expression.Switch(Expression.Constant(42), Expression.Constant(43), Expression.SwitchCase(Expression.Constant(44), Expression.Constant(45)));
            var v = Expression.Parameter(typeof(int));
            var e = Expression.Assign(v, c);
            var r = AssignmentPercolator.Percolate(e);

            Assert.AreEqual(ExpressionType.Switch, r.NodeType);
            var t = (SwitchExpression)r;
            Assert.AreEqual(ExpressionType.Assign, t.DefaultBody.NodeType);
            Assert.AreEqual(ExpressionType.Assign, t.Cases[0].Body.NodeType);
            var a1 = (BinaryExpression)t.DefaultBody;
            var a2 = (BinaryExpression)t.Cases[0].Body;
            Assert.AreSame(v, a1.Left);
            Assert.AreSame(v, a2.Left);
            Assert.AreSame(c.DefaultBody, a1.Right);
            Assert.AreSame(c.Cases[0].Body, a2.Right);
        }
    }
}