// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.Expressions.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class SpillerTests
    {
        [TestMethod]
        public void Spiller_Basics()
        {
            // NB: This really tests the LINQ stack spiller but with our ceremony around it.

            var a = Expression.Constant(1);
            var b = Expression.Constant(2);

            var x = Expression.TryFinally(a, Expression.Empty());
            var y = Expression.TryFinally(b, Expression.Empty());

            var e = Expression.Add(x, y);

            var r = Spiller.Spill(e);

            Assert.AreEqual(ExpressionType.Block, r.NodeType);
            var si = (BlockExpression)r;

            Assert.AreEqual(2, si.Variables.Count);
            var sv1 = si.Variables[0];
            var sv2 = si.Variables[1];

            Assert.AreEqual(1, si.Expressions.Count); // NB: LINQ stack spiller has a spurious block
            var si1 = si.Expressions[0];

            Assert.AreEqual(ExpressionType.Block, si1.NodeType);
            var sb = (BlockExpression)si1;

            Assert.AreEqual(3, sb.Expressions.Count);
            var se1 = sb.Expressions[0];
            var se2 = sb.Expressions[1];
            var se3 = sb.Expressions[2];

            Assert.AreEqual(ExpressionType.Assign, se1.NodeType);
            var sa1 = (BinaryExpression)se1;
            Assert.AreSame(sv1, sa1.Left);
            //Assert.AreSame(x, sa1.Right); // NB: LINQ stack spiller could clone a child tree; should use an equality comparer here

            Assert.AreEqual(ExpressionType.Assign, se2.NodeType);
            var sa2 = (BinaryExpression)se2;
            Assert.AreSame(sv2, sa2.Left);
            //Assert.AreSame(y, sa2.Right); // NB: LINQ stack spiller could clone a child tree; should use an equality comparer here

            Assert.AreEqual(ExpressionType.Add, se3.NodeType);
            var sa3 = (BinaryExpression)se3;
            Assert.AreSame(sv1, sa3.Left);
            Assert.AreSame(sv2, sa3.Right);
        }

        [TestMethod]
        public void Spiller_Await()
        {
            var a = Expression.Constant(Task.FromResult(1));
            var b = Expression.Constant(Task.FromResult(2));

            var x = CSharpExpression.Await(a);
            var y = CSharpExpression.Await(b);

            var e = Expression.Add(x, y);

            var r = Spiller.Spill(e);

            Assert.AreEqual(ExpressionType.Block, r.NodeType);
            var si = (BlockExpression)r;

            Assert.AreEqual(2, si.Variables.Count);
            var sv1 = si.Variables[0];
            var sv2 = si.Variables[1];

            Assert.AreEqual(1, si.Expressions.Count); // NB: LINQ stack spiller has a spurious block
            var si1 = si.Expressions[0];

            Assert.AreEqual(ExpressionType.Block, si1.NodeType);
            var sb = (BlockExpression)si1;

            Assert.AreEqual(3, sb.Expressions.Count);
            var se1 = sb.Expressions[0];
            var se2 = sb.Expressions[1];
            var se3 = sb.Expressions[2];

            Assert.AreEqual(ExpressionType.Assign, se1.NodeType);
            var sa1 = (BinaryExpression)se1;
            Assert.AreSame(sv1, sa1.Left);
            //Assert.AreSame(x, sa1.Right); // NB: LINQ stack spiller could clone a child tree; should use an equality comparer here

            Assert.AreEqual(ExpressionType.Assign, se2.NodeType);
            var sa2 = (BinaryExpression)se2;
            Assert.AreSame(sv2, sa2.Left);
            //Assert.AreSame(y, sa2.Right); // NB: LINQ stack spiller could clone a child tree; should use an equality comparer here

            Assert.AreEqual(ExpressionType.Add, se3.NodeType);
            var sa3 = (BinaryExpression)se3;
            Assert.AreSame(sv1, sa3.Left);
            Assert.AreSame(sv2, sa3.Right);
        }

        // TODO: Add test for nested spilling.
    }
}
