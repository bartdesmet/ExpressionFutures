// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class ShadowEliminatorTests
    {
        [TestMethod]
        public void ShadowEliminator_NoShadow()
        {
            var v1 = Expression.Parameter(typeof(int));
            var v2 = Expression.Parameter(typeof(int));
            var e1 = Expression.Parameter(typeof(Exception));
            var e2 = Expression.Parameter(typeof(Exception));
            var d1 = Expression.Parameter(typeof(IDisposable));
            var d2 = Expression.Parameter(typeof(IDisposable));
            var r1 = Expression.Default(typeof(IDisposable));
            var r2 = Expression.Default(typeof(IDisposable));

            var es = new Expression[]
            {
                Expression.Block(new[] { v1 }, Expression.Block(new[] { v2 }, Expression.Add(v1, v2))),
                Expression.Lambda(Expression.Lambda(Expression.Add(v1, v2), v2), v1),
                Expression.TryCatch(Expression.Empty(), Expression.Catch(e1, Expression.TryCatch(Expression.Empty(), Expression.Catch(e2, Expression.Empty())))),
                Expression.TryCatch(Expression.Empty(), Expression.Catch(typeof(Exception), Expression.TryCatch(Expression.Empty(), Expression.Catch(typeof(Exception), Expression.Empty())))),
                CSharpExpression.Using(d1, r1, CSharpExpression.Using(d2, r1, Expression.Empty())),
                CSharpExpression.Using(r1, CSharpExpression.Using(r1, Expression.Empty())),
            };

            foreach (var e in es)
            {
                Assert.AreSame(e, new ShadowEliminator().Visit(e));
            }
        }

        [TestMethod]
        public void ShadowEliminator_Shadow_Block()
        {
            var x = Expression.Parameter(typeof(int));

            var e = Expression.Block(new[] { x }, Expression.Block(new[] { x }, x));
            var r = (BlockExpression)new ShadowEliminator().Visit(e);

            var v1 = r.Variables[0];
            var e1 = (BlockExpression)r.Expressions[0];
            var v2 = e1.Variables[0];
            var e2 = e1.Expressions[0];

            Assert.AreSame(v2, e2);
            Assert.AreNotSame(v1, v2);
        }

        [TestMethod]
        public void ShadowEliminator_Shadow_Lambda()
        {
            var x = Expression.Parameter(typeof(int));

            var e = Expression.Lambda(Expression.Lambda(x, x), x);
            var r = (LambdaExpression)new ShadowEliminator().Visit(e);

            var v1 = r.Parameters[0];
            var e1 = (LambdaExpression)r.Body;
            var v2 = e1.Parameters[0];
            var e2 = e1.Body;

            Assert.AreSame(v2, e2);
            Assert.AreNotSame(v1, v2);
        }

        [TestMethod]
        public void ShadowEliminator_Shadow_Catch()
        {
            var x = Expression.Parameter(typeof(Exception));

            var e = Expression.TryCatch(Expression.Empty(), Expression.Catch(x, Expression.TryCatch(Expression.Empty(), Expression.Catch(x, Expression.Block(typeof(void), x)))));
            var r = (TryExpression)new ShadowEliminator().Visit(e);

            var h1 = r.Handlers[0];
            var v1 = h1.Variable;
            var e1 = (TryExpression)h1.Body;
            var h2 = e1.Handlers[0];
            var v2 = h2.Variable;
            var e2 = ((BlockExpression)h2.Body).Expressions[0];

            Assert.AreSame(v2, e2);
            Assert.AreNotSame(v1, v2);
        }

        [TestMethod]
        public void ShadowEliminator_Shadow_Using()
        {
            var x = Expression.Parameter(typeof(IDisposable));
            var y = Expression.Default(typeof(IDisposable));

            var e = CSharpExpression.Using(x, y, CSharpExpression.Using(x, y, x));
            var r = (UsingCSharpStatement)new ShadowEliminator().Visit(e);

            var v1 = r.Variable;
            var e1 = (UsingCSharpStatement)r.Body;
            var v2 = e1.Variable;
            var e2 = e1.Body;

            Assert.AreSame(v2, e2);
            Assert.AreNotSame(v1, v2);
        }

        // TODO: add more tests that nest the different constructs
    }
}
