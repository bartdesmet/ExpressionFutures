// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class ConditionalArrayIndexTests
    {
        [TestMethod]
        public void ConditionalArrayIndex_Factory_ArgumentChecking()
        {
            var array = Expression.Default(typeof(string[]));
            var indexes = new[] { Expression.Constant(1) };

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalArrayIndex(default(Expression), indexes));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalArrayIndex(array, default(Expression[])));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalArrayIndex(array, default(IEnumerable<Expression>)));

            // not an array
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalArrayIndex(Expression.Default(typeof(long))));

            // wrong amount of indexes
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalArrayIndex(Expression.Default(typeof(string[]))));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalArrayIndex(Expression.Default(typeof(string[])), Expression.Constant(1), Expression.Constant(2)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalArrayIndex(Expression.Default(typeof(string[,]))));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalArrayIndex(Expression.Default(typeof(string[,])), Expression.Constant(1)));

            // indexes not of type int
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalArrayIndex(Expression.Default(typeof(string[,])), Expression.Constant(1), Expression.Constant("bar")));
        }

        [TestMethod]
        public void ConditionalArrayIndex_Properties()
        {
            var array = Expression.Default(typeof(string[]));
            var indexes = new[] { Expression.Constant(0) };

            foreach (var e in new[]
            {
                CSharpExpression.ConditionalArrayIndex(array, indexes),
                CSharpExpression.ConditionalArrayIndex(array, indexes.AsEnumerable()),
            })
            {
                Assert.AreEqual(CSharpExpressionType.ConditionalAccess, e.CSharpNodeType);
                Assert.AreSame(array, e.Array);
                Assert.AreEqual(typeof(string), e.Type);
                Assert.IsTrue(e.Indexes.SequenceEqual(indexes));
            }
        }

        [TestMethod]
        public void ConditionalArrayIndex_Update()
        {
            var array = Expression.Default(typeof(string[]));
            var indexes = new[] { Expression.Constant(0) };

            var res = CSharpExpression.ConditionalArrayIndex(array, indexes);

            Assert.AreSame(res, res.Update(res.Array, res.Indexes));

            var obj1 = Expression.Default(typeof(string[]));
            var upd1 = res.Update(obj1, res.Indexes);
            Assert.AreNotSame(upd1, res);
            Assert.AreSame(res.Indexes, upd1.Indexes);
            Assert.AreSame(obj1, upd1.Array);

            var newIndexes = new[] { Expression.Constant(1) };

            var upd2 = res.Update(array, newIndexes);
            Assert.AreNotSame(upd2, res);
            Assert.AreSame(res.Array, upd2.Array);
            Assert.IsTrue(upd2.Indexes.SequenceEqual(newIndexes));
        }

        [TestMethod]
        public void ConditionalArrayIndex_Compile_Ref()
        {
            var px = Expression.Parameter(typeof(string[]));
            var ax = new[] { Expression.Constant(0) };
            var mx = CSharpExpression.ConditionalArrayIndex(px, ax);
            var fx = Expression.Lambda<Func<string[], string>>(mx, px);
            var dx = fx.Compile();
            Assert.AreEqual("bar", dx(new[] { "bar" }));
            Assert.IsNull(dx(null));
        }

        [TestMethod]
        public void ConditionalArrayIndex_Compile_Ref_Multidimensional()
        {
            var px = Expression.Parameter(typeof(string[,]));
            var ax = new[] { Expression.Constant(0), Expression.Constant(0) };
            var mx = CSharpExpression.ConditionalArrayIndex(px, ax);
            var fx = Expression.Lambda<Func<string[,], string>>(mx, px);
            var dx = fx.Compile();
            Assert.AreEqual("bar", dx(new string[1, 1] { { "bar" } }));
            Assert.IsNull(dx(null));
        }

        [TestMethod]
        public void ConditionalArrayIndex_Compile_Val()
        {
            var px = Expression.Parameter(typeof(int[]));
            var ax = new[] { Expression.Constant(0) };
            var mx = CSharpExpression.ConditionalArrayIndex(px, ax);
            var fx = Expression.Lambda<Func<int[], int?>>(mx, px);
            var dx = fx.Compile();
            Assert.AreEqual(42, dx(new[] { 42 }));
            Assert.IsNull(dx(null));
        }

        [TestMethod]
        public void ConditionalArrayIndex_Compile_Val_Multidimensional()
        {
            var px = Expression.Parameter(typeof(int[,]));
            var ax = new[] { Expression.Constant(0), Expression.Constant(0) };
            var mx = CSharpExpression.ConditionalArrayIndex(px, ax);
            var fx = Expression.Lambda<Func<int[,], int?>>(mx, px);
            var dx = fx.Compile();
            Assert.AreEqual(42, dx(new int[1, 1] { { 42 } }));
            Assert.IsNull(dx(null));
        }

        // TODO: tests to assert args are not evaluated if receiver is null
        // TODO: tests to assert receiver is only evaluated once
    }
}