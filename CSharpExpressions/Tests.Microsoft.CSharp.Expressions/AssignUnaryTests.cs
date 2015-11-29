// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static Tests.ReflectionUtils;

namespace Tests
{
    [TestClass]
    public partial class AssignUnaryTests
    {
        [TestMethod]
        public void AssignUnary_Factory_ArgumentChecking()
        {
            var o = Expression.Parameter(typeof(int));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.MakeUnaryAssign(CSharpExpressionType.Await, o, null));
        }

        [TestMethod]
        public void AssignUnary_Factory_MakeUnaryAssign()
        {
            foreach (var o in GetLhs())
            {
                var m = MethodInfoOf(() => Op(0));

                foreach (var n in new[]
                {
                    CSharpExpressionType.PreIncrementAssign,
                    CSharpExpressionType.PreIncrementCheckedAssign,
                    CSharpExpressionType.PreDecrementAssign,
                    CSharpExpressionType.PreDecrementCheckedAssign,
                    CSharpExpressionType.PostIncrementAssign,
                    CSharpExpressionType.PostIncrementCheckedAssign,
                    CSharpExpressionType.PostDecrementAssign,
                    CSharpExpressionType.PostDecrementCheckedAssign,
                })
                {
                    var a1 = CSharpExpression.MakeUnaryAssign(n, o, m);
                    Assert.AreEqual(n, a1.CSharpNodeType);
                    Assert.AreSame(o, a1.Operand);
                    Assert.AreSame(m, a1.Method);
                    Assert.AreEqual(typeof(int), a1.Type);

                    var a2 = CSharpExpression.MakeUnaryAssign(n, o, null);
                    Assert.AreEqual(n, a2.CSharpNodeType);
                    Assert.AreSame(o, a2.Operand);
                    Assert.IsNull(a2.Method);
                    Assert.AreEqual(typeof(int), a2.Type);
                }
            }
        }

        private static int Op(int x)
        {
            throw new NotImplementedException();
        }
        
        private static IEnumerable<Expression> GetLhs()
        {
            yield return Expression.Parameter(typeof(int));
            yield return Expression.Field(Expression.Parameter(typeof(StrongBox<int>)), "Value");
            yield return Expression.MakeIndex(Expression.Parameter(typeof(List<int>)), typeof(List<int>).GetProperty("Item"), new[] { Expression.Constant(0) });
            yield return Expression.ArrayAccess(Expression.Parameter(typeof(int[])), Expression.Constant(0));
            yield return CSharpExpression.Index(Expression.Parameter(typeof(List<int>)), typeof(List<int>).GetProperty("Item"), CSharpExpression.Bind(typeof(List<int>).GetProperty("Item").GetIndexParameters()[0], Expression.Constant(0)));
        }
    }
}
