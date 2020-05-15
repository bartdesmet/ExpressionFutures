// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public partial class TupleConvertTests
    {
        [TestMethod]
        public void TupleConvert_Factory_ArgumentChecking()
        {
            // null checks
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.TupleConvert(null, typeof(int), new LambdaExpression[0]));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.TupleConvert(Expression.Constant((1, 2)), null, new LambdaExpression[0]));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.TupleConvert(Expression.Constant((1, 2)), typeof(ValueTuple<int, int>), new LambdaExpression[2] { null, null }));

            // not a tuple type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleConvert(Expression.Constant(1), typeof(ValueTuple<int>), new LambdaExpression[0]));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleConvert(Expression.Constant((1, 2)), typeof(int), new LambdaExpression[0]));

            // mismatched tuple arities
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleConvert(Expression.Constant((1, 2)), typeof(ValueTuple<int>), new LambdaExpression[0]));

            // invalid conversion count
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleConvert(Expression.Constant((1, 2)), typeof(ValueTuple<long, long>), new LambdaExpression[0]));

            // invalid conversion signature
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleConvert(Expression.Constant((1, 2)), typeof(ValueTuple<long, long>), new LambdaExpression[] { (Expression<Func<long>>)(() => 1), (Expression<Func<int, long>>)(x => x) }));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleConvert(Expression.Constant((1, 2)), typeof(ValueTuple<long, long>), new LambdaExpression[] { (Expression<Func<int, int, long>>)((x, y) => x), (Expression<Func<int, long>>)(x => x) }));

            // invalid conversion types
            // NB: exception type derived from LINQ helpers
            AssertEx.Throws<InvalidOperationException>(() => CSharpExpression.TupleConvert(Expression.Constant((1, 2)), typeof(ValueTuple<long, long>), new LambdaExpression[] { (Expression<Func<bool, long>>)(b => 1), (Expression<Func<int, long>>)(x => x) }));
            AssertEx.Throws<InvalidOperationException>(() => CSharpExpression.TupleConvert(Expression.Constant((1, 2)), typeof(ValueTuple<long, long>), new LambdaExpression[] { (Expression<Func<int, bool>>)(_ => false), (Expression<Func<int, long>>)(x => x) }));
        }

        [TestMethod]
        public void TupleConvert_Factory_Properties()
        {
            // TODO
        }

        [TestMethod]
        public void TupleConvert_Update()
        {
            // TODO
        }

        [TestMethod]
        public void TupleConvert_Visitor()
        {
            var p = Expression.Parameter(typeof(int), "p");
            var convert = Expression.Lambda<Func<int, long>>(Expression.Convert(p, typeof(long)), p);
            var res = CSharpExpression.TupleConvert(Expression.Constant(new ValueTuple<int>(42)), typeof(ValueTuple<long>), new[] { convert });

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitTupleConvert(TupleConvertCSharpExpression node)
            {
                Visited = true;

                return base.VisitTupleConvert(node);
            }
        }

        [TestMethod]
        public void TupleConvert_Reduce()
        {
            // TODO
        }

        [TestMethod]
        public void TupleConvert_SideEffects()
        {
            var p = Expression.Parameter(typeof(int));

            AssertCompile<(int, int)>((log, append) =>
            {
                var conversions = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int>>(Expression.Block(log(i.ToString()), Expression.Increment(p)), p));

                return CSharpExpression.TupleConvert(Expression.Constant((0, 1)), typeof(ValueTuple<int, int>), conversions);
            }, new LogAndResult<(int, int)> { Log = { "1", "2" }, Value = (1, 2) });

            AssertCompile<(int, int, int, int, int, int, int, int)>((log, append) =>
            {
                var conversions = Enumerable.Range(1, 8).Select(i => Expression.Lambda<Func<int, int>>(Expression.Block(log(i.ToString()), Expression.Increment(p)), p));

                return CSharpExpression.TupleConvert(Expression.Constant((0, 1, 2, 3, 4, 5, 6, 7)), typeof(ValueTuple<int, int, int, int, int, int, int, ValueTuple<int>>), conversions);
            }, new LogAndResult<(int, int, int, int, int, int, int, int)> { Log = { "1", "2", "3", "4", "5", "6", "7", "8" }, Value = (1, 2, 3, 4, 5, 6, 7, 8) });
        }

        private void AssertCompile<T>(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<T> expected)
        {
            var res = WithLog<T>(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }
    }
}
