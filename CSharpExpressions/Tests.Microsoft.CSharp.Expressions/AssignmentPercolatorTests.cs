// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions.Compiler;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    public class AssignmentPercolatorTests
    {
        [Fact]
        public void AssignmentPercolator_Block()
        {
            var b = Expression.Block(Expression.Empty(), Expression.Constant(42));
            var v = Expression.Parameter(typeof(int));
            var e = Expression.Assign(v, b);
            var r = AssignmentPercolator.Percolate(e);

            Assert.Equal(ExpressionType.Block, r.NodeType);
            var t = (BlockExpression)r;
            Assert.Same(b.Expressions[0], t.Expressions[0]);
            Assert.Equal(ExpressionType.Assign, t.Expressions[1].NodeType);
            var a = (BinaryExpression)t.Expressions[1];
            Assert.Same(v, a.Left);
            Assert.Same(b.Expressions[1], a.Right);
        }

        [Fact]
        public void AssignmentPercolator_Conditional()
        {
            var c = Expression.Condition(Expression.Constant(true), Expression.Constant(42), Expression.Constant(43));
            var v = Expression.Parameter(typeof(int));
            var e = Expression.Assign(v, c);
            var r = AssignmentPercolator.Percolate(e);

            Assert.Equal(ExpressionType.Conditional, r.NodeType);
            var t = (ConditionalExpression)r;
            Assert.Same(c.Test, t.Test);
            Assert.Equal(ExpressionType.Assign, t.IfTrue.NodeType);
            Assert.Equal(ExpressionType.Assign, t.IfFalse.NodeType);
            var a1 = (BinaryExpression)t.IfTrue;
            var a2 = (BinaryExpression)t.IfFalse;
            Assert.Same(v, a1.Left);
            Assert.Same(v, a2.Left);
            Assert.Same(c.IfTrue, a1.Right);
            Assert.Same(c.IfFalse, a2.Right);
        }

        [Fact]
        public void AssignmentPercolator_TryFinally()
        {
            var c = Expression.TryFinally(Expression.Constant(42), Expression.Empty());
            var v = Expression.Parameter(typeof(int));
            var e = Expression.Assign(v, c);
            var r = AssignmentPercolator.Percolate(e);

            Assert.Equal(ExpressionType.Try, r.NodeType);
            var t = (TryExpression)r;
            Assert.Same(c.Finally, t.Finally);
            Assert.Equal(ExpressionType.Assign, t.Body.NodeType);
            var a = (BinaryExpression)t.Body;
            Assert.Same(v, a.Left);
            Assert.Same(c.Body, a.Right);
        }

        [Fact]
        public void AssignmentPercolator_TryCatch()
        {
            var c = Expression.TryCatch(Expression.Constant(42), Expression.Catch(typeof(Exception), Expression.Constant(43)));
            var v = Expression.Parameter(typeof(int));
            var e = Expression.Assign(v, c);
            var r = AssignmentPercolator.Percolate(e);

            Assert.Equal(ExpressionType.Try, r.NodeType);
            var t = (TryExpression)r;
            Assert.Equal(ExpressionType.Assign, t.Body.NodeType);
            Assert.Equal(ExpressionType.Assign, t.Handlers[0].Body.NodeType);
            var a1 = (BinaryExpression)t.Body;
            var a2 = (BinaryExpression)t.Handlers[0].Body;
            Assert.Same(v, a1.Left);
            Assert.Same(v, a2.Left);
            Assert.Same(c.Body, a1.Right);
            Assert.Same(c.Handlers[0].Body, a2.Right);
        }

        [Fact]
        public void AssignmentPercolator_Switch()
        {
            var c = Expression.Switch(Expression.Constant(42), Expression.Constant(43), Expression.SwitchCase(Expression.Constant(44), Expression.Constant(45)));
            var v = Expression.Parameter(typeof(int));
            var e = Expression.Assign(v, c);
            var r = AssignmentPercolator.Percolate(e);

            Assert.Equal(ExpressionType.Switch, r.NodeType);
            var t = (SwitchExpression)r;
            Assert.Equal(ExpressionType.Assign, t.DefaultBody.NodeType);
            Assert.Equal(ExpressionType.Assign, t.Cases[0].Body.NodeType);
            var a1 = (BinaryExpression)t.DefaultBody;
            var a2 = (BinaryExpression)t.Cases[0].Body;
            Assert.Same(v, a1.Left);
            Assert.Same(v, a2.Left);
            Assert.Same(c.DefaultBody, a1.Right);
            Assert.Same(c.Cases[0].Body, a2.Right);
        }
    }
}