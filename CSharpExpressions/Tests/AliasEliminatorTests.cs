// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.Expressions.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class AliasEliminatorTests
    {
        [TestMethod]
        public void AliasEliminator_NoAlias()
        {
            var v1 = Expression.Parameter(typeof(int));
            var v2 = Expression.Parameter(typeof(int));
            var e1 = Expression.Parameter(typeof(Exception));
            var e2 = Expression.Parameter(typeof(Exception));
            var d1 = Expression.Parameter(typeof(IDisposable));
            var d2 = Expression.Parameter(typeof(IDisposable));
            var r1 = Expression.Default(typeof(IDisposable));
            var r2 = Expression.Default(typeof(IDisposable));
            var c1 = Expression.Parameter(typeof(int[]));
            var c2 = Expression.Parameter(typeof(int[]));
            var i1 = Expression.Default(typeof(int));
            var b1 = Expression.Default(typeof(bool));

            var es = new Expression[]
            {
                Expression.Block(new[] { v1 }, Expression.Block(new[] { v2 }, Expression.Add(v1, v2))),
                Expression.Lambda(Expression.Lambda(Expression.Add(v1, v2), v2), v1),
                Expression.TryCatch(Expression.Empty(), Expression.Catch(e1, Expression.TryCatch(Expression.Empty(), Expression.Catch(e2, Expression.Empty())))),
                Expression.TryCatch(Expression.Empty(), Expression.Catch(typeof(Exception), Expression.TryCatch(Expression.Empty(), Expression.Catch(typeof(Exception), Expression.Empty())))),
                CSharpExpression.Using(d1, r1, CSharpExpression.Using(d2, r1, Expression.Empty())),
                CSharpExpression.Using(r1, CSharpExpression.Using(r1, Expression.Empty())),
                CSharpExpression.ForEach(v1, c1, CSharpExpression.ForEach(v2, c2, Expression.Empty())),
                CSharpExpression.For(new[] { Expression.Assign(v1, i1) }, b1, null, CSharpExpression.For(new[] { Expression.Assign(v2, i1) }, b1, null, Expression.Empty())),
            };

            foreach (var e in es)
            {
                Assert.AreSame(e, AliasEliminator.Eliminate(e));
            }
        }

        [TestMethod]
        public void AliasEliminator_Alias_Block()
        {
            var x = Expression.Parameter(typeof(int));

            var e = Expression.Block(new[] { x }, Expression.Block(new[] { x }, x));
            var r = (BlockExpression)AliasEliminator.Eliminate(e);

            var v1 = r.Variables[0];
            var e1 = (BlockExpression)r.Expressions[0];
            var v2 = e1.Variables[0];
            var e2 = e1.Expressions[0];

            Assert.AreSame(v2, e2);
            Assert.AreNotSame(v1, v2);
        }

        [TestMethod]
        public void AliasEliminator_Alias_Lambda()
        {
            var x = Expression.Parameter(typeof(int));

            var e = Expression.Lambda(Expression.Lambda(x, x), x);
            var r = (LambdaExpression)AliasEliminator.Eliminate(e);

            var v1 = r.Parameters[0];
            var e1 = (LambdaExpression)r.Body;
            var v2 = e1.Parameters[0];
            var e2 = e1.Body;

            Assert.AreSame(v2, e2);
            Assert.AreNotSame(v1, v2);
        }

        [TestMethod]
        public void AliasEliminator_Alias_Catch()
        {
            var x = Expression.Parameter(typeof(Exception));

            var e = Expression.TryCatch(Expression.Empty(), Expression.Catch(x, Expression.TryCatch(Expression.Empty(), Expression.Catch(x, Expression.Block(typeof(void), x)))));
            var r = (TryExpression)AliasEliminator.Eliminate(e);

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
        public void AliasEliminator_Alias_Using()
        {
            var x = Expression.Parameter(typeof(IDisposable));
            var y = Expression.Default(typeof(IDisposable));

            var e = CSharpExpression.Using(x, y, CSharpExpression.Using(x, y, x));
            var r = (UsingCSharpStatement)AliasEliminator.Eliminate(e);

            var v1 = r.Variable;
            var e1 = (UsingCSharpStatement)r.Body;
            var v2 = e1.Variable;
            var e2 = e1.Body;

            Assert.AreSame(v2, e2);
            Assert.AreNotSame(v1, v2);
        }

        [TestMethod]
        public void AliasEliminator_Alias_ForEach()
        {
            var x = Expression.Parameter(typeof(int));
            var xs = Expression.Default(typeof(int[]));

            var e = CSharpExpression.ForEach(x, xs, CSharpExpression.ForEach(x, xs, x));
            var r = (ForEachCSharpStatement)AliasEliminator.Eliminate(e);

            var v1 = r.Variable;
            var e1 = (ForEachCSharpStatement)r.Body;
            var v2 = e1.Variable;
            var e2 = e1.Body;

            Assert.AreSame(v2, e2);
            Assert.AreNotSame(v1, v2);
        }

        [TestMethod]
        public void AliasEliminator_Alias_For()
        {
            var x = Expression.Parameter(typeof(int));
            var i = Expression.Assign(x, Expression.Constant(0));
            var b = Expression.Default(typeof(bool));
            var xs = Expression.Default(typeof(int[]));

            var e = CSharpExpression.For(new[] { i }, b, null, CSharpExpression.For(new[] { i }, b, null, x));
            var r = (ForCSharpStatement)AliasEliminator.Eliminate(e);

            var v1 = r.Variables[0];
            var e1 = (ForCSharpStatement)r.Body;
            var v2 = e1.Variables[0];
            var e2 = e1.Body;

            Assert.AreSame(v2, e2);
            Assert.AreNotSame(v1, v2);
        }

        // TODO: add more tests that nest the different constructs

        [TestMethod]
        public void AliasEliminator_Siblings()
        {
            var x = Expression.Parameter(typeof(int));

            var b1 = Expression.Block(new[] { x }, Expression.Empty());
            var b2 = Expression.Block(new[] { x }, Expression.Empty());

            var b = Expression.Block(b1, b2);

            var r = (BlockExpression)AliasEliminator.Eliminate(b);

            var r1 = (BlockExpression)r.Expressions[0];
            var r2 = (BlockExpression)r.Expressions[1];

            Assert.AreNotSame(r1.Variables[0], r2.Variables[0]);
        }
    }
}
