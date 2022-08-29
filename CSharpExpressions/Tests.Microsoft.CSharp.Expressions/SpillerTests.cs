// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.Expressions.Compiler;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class SpillerTests
    {
        [Fact]
        public void Spiller_Basics()
        {
            // NB: This really tests the LINQ stack spiller but with our ceremony around it.

            var a = Expression.Constant(1);
            var b = Expression.Constant(2);

            var x = Expression.TryFinally(a, Expression.Empty());
            var y = Expression.TryFinally(b, Expression.Empty());

            var e = Expression.Add(x, y);

            var r = Spiller.Spill(e);

            Assert.Equal(ExpressionType.Block, r.NodeType);
            var si = (BlockExpression)r;

            Assert.Equal(2, si.Variables.Count);
            var sv1 = si.Variables[0];
            var sv2 = si.Variables[1];

            Assert.Single(si.Expressions); // NB: LINQ stack spiller has a spurious block
            var si1 = si.Expressions[0];

            Assert.Equal(ExpressionType.Block, si1.NodeType);
            var sb = (BlockExpression)si1;

            Assert.Equal(3, sb.Expressions.Count);
            var se1 = sb.Expressions[0];
            var se2 = sb.Expressions[1];
            var se3 = sb.Expressions[2];

            Assert.Equal(ExpressionType.Assign, se1.NodeType);
            var sa1 = (BinaryExpression)se1;
            Assert.Same(sv1, sa1.Left);
            //Assert.Same(x, sa1.Right); // NB: LINQ stack spiller could clone a child tree; should use an equality comparer here

            Assert.Equal(ExpressionType.Assign, se2.NodeType);
            var sa2 = (BinaryExpression)se2;
            Assert.Same(sv2, sa2.Left);
            //Assert.Same(y, sa2.Right); // NB: LINQ stack spiller could clone a child tree; should use an equality comparer here

            Assert.Equal(ExpressionType.Add, se3.NodeType);
            var sa3 = (BinaryExpression)se3;
            Assert.Same(sv1, sa3.Left);
            Assert.Same(sv2, sa3.Right);
        }

        [Fact]
        public void Spiller_Await()
        {
            var a = Expression.Constant(Task.FromResult(1));
            var b = Expression.Constant(Task.FromResult(2));

            var x = CSharpExpression.Await(a);
            var y = CSharpExpression.Await(b);

            var e = Expression.Add(x, y);

            var r = Spiller.Spill(e);

            Assert.Equal(ExpressionType.Block, r.NodeType);
            var si = (BlockExpression)r;

            Assert.Equal(2, si.Variables.Count);
            var sv1 = si.Variables[0];
            var sv2 = si.Variables[1];

            Assert.Single(si.Expressions); // NB: LINQ stack spiller has a spurious block
            var si1 = si.Expressions[0];

            Assert.Equal(ExpressionType.Block, si1.NodeType);
            var sb = (BlockExpression)si1;

            Assert.Equal(3, sb.Expressions.Count);
            var se1 = sb.Expressions[0];
            var se2 = sb.Expressions[1];
            var se3 = sb.Expressions[2];

            Assert.Equal(ExpressionType.Assign, se1.NodeType);
            var sa1 = (BinaryExpression)se1;
            Assert.Same(sv1, sa1.Left);
            //Assert.Same(x, sa1.Right); // NB: LINQ stack spiller could clone a child tree; should use an equality comparer here

            Assert.Equal(ExpressionType.Assign, se2.NodeType);
            var sa2 = (BinaryExpression)se2;
            Assert.Same(sv2, sa2.Left);
            //Assert.Same(y, sa2.Right); // NB: LINQ stack spiller could clone a child tree; should use an equality comparer here

            Assert.Equal(ExpressionType.Add, se3.NodeType);
            var sa3 = (BinaryExpression)se3;
            Assert.Same(sv1, sa3.Left);
            Assert.Same(sv2, sa3.Right);
        }

        // TODO: Add test for nested spilling.
    }
}
