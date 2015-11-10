// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.Expressions.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class CSharpExpressionVisitorTests
    {
        [TestMethod]
        public void CSharpExpressionVisitor_NoChange()
        {
            var visitor = new V();

            foreach (var e in new Expression[]
            {
                Expression.Constant(1),
                CSharpExpression.Await(Expression.Default(typeof(Task)))
            })
            {
                Assert.AreSame(e, visitor.Visit(e));
            }
        }

        [TestMethod]
        public void CSharpExpressionVisitor_Reduce()
        {
            var visitor = new V();

            var r = visitor.Visit(new E());

            Assert.AreEqual(ExpressionType.Constant, r.NodeType);
        }

        class V : CSharpExpressionVisitor
        {
        }

        class E : Expression
        {
            public override Type Type
            {
                get
                {
                    return typeof(int);
                }
            }

            public override ExpressionType NodeType
            {
                get
                {
                    return ExpressionType.Extension;
                }
            }

            public override bool CanReduce
            {
                get
                {
                    return true;
                }
            }

            public override Expression Reduce()
            {
                return Expression.Constant(42);
            }
        }
    }
}