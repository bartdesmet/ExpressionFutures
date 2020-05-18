// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
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
            var operand = Expression.Constant((1, 2));
            var type = typeof(ValueTuple<long, long>);
            var convert1 = (Expression<Func<int, long>>)(x => x);
            var convert2 = (Expression<Func<int, long>>)(x => x);

            var it = CSharpExpression.TupleConvert(operand, type, new LambdaExpression[] { convert1, convert2 });

            Assert.AreEqual(CSharpExpressionType.TupleConvert, it.CSharpNodeType);
            Assert.AreEqual(type, it.Type);
            Assert.AreSame(operand, it.Operand);
            Assert.AreEqual(2, it.ElementConversions.Count);
            Assert.AreSame(convert1, it.ElementConversions[0]);
            Assert.AreSame(convert2, it.ElementConversions[1]);
            Assert.IsFalse(it.IsLifted);
            Assert.IsFalse(it.IsLiftedToNull);

            var nullableDest = typeof(Nullable<>).MakeGenericType(type);
            var nullableSrc = typeof(Nullable<>).MakeGenericType(operand.Type);

            var lifted1 = CSharpExpression.TupleConvert(operand, nullableDest, new LambdaExpression[] { convert1, convert2 }); ;

            Assert.AreEqual(nullableDest, lifted1.Type);
            Assert.IsTrue(lifted1.IsLifted);
            Assert.IsTrue(lifted1.IsLiftedToNull);

            var lifted2 = CSharpExpression.TupleConvert(Expression.Convert(operand, nullableSrc), type, new LambdaExpression[] { convert1, convert2 }); ;

            Assert.AreEqual(type, lifted2.Type);
            Assert.IsTrue(lifted2.IsLifted);
            Assert.IsFalse(lifted2.IsLiftedToNull);

            var lifted3 = CSharpExpression.TupleConvert(Expression.Convert(operand, nullableSrc), nullableDest, new LambdaExpression[] { convert1, convert2 }); ;

            Assert.AreEqual(nullableDest, lifted3.Type);
            Assert.IsTrue(lifted3.IsLifted);
            Assert.IsTrue(lifted3.IsLiftedToNull);
        }

        [TestMethod]
        public void TupleConvert_Update()
        {
            var operand = Expression.Constant((1, 2));
            var type = typeof(ValueTuple<long, long>);
            var convert1 = (Expression<Func<int, long>>)(x => x);
            var convert2 = (Expression<Func<int, long>>)(x => x);
            var it = CSharpExpression.TupleConvert(operand, type, new LambdaExpression[] { convert1, convert2 });

            Assert.AreSame(it, it.Update(it.Operand, it.ElementConversions));

            var newOperand = Expression.Constant((2, 3));
            var new1 = it.Update(newOperand, it.ElementConversions);
            Assert.AreSame(newOperand, new1.Operand);
            Assert.AreSame(it.ElementConversions, new1.ElementConversions);

            var convert3 = (Expression<Func<int, long>>)(x => x);
            var new2 = it.Update(it.Operand, new[] { convert1, convert3 });
            Assert.AreSame(it.Operand, new2.Operand);
            Assert.AreEqual(2, it.ElementConversions.Count);
            Assert.AreSame(convert1, new2.ElementConversions[0]);
            Assert.AreSame(convert3, new2.ElementConversions[1]);
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
            var convertIntToLong = (Expression<Func<int, long>>)(x => x);
            var convertLongToInt = (Expression<Func<long, int>>)(x => (int)x);

            var convertIntToObject = (Expression<Func<int, object>>)(x => x);
            var convertObjectToInt = (Expression<Func<object, int>>)(x => (int)x);

            var convertDateTimeToDateTimeOffset = (Expression<Func<DateTime, DateTimeOffset>>)(x => x);

            var conversions = new LambdaExpression[] { convertIntToLong, convertLongToInt, convertIntToObject, convertObjectToInt, convertDateTimeToDateTimeOffset };

            var dt = DateTime.Now;
            var dto = (DateTimeOffset)dt;

            {
                var f = CreateTupleConvert<ValueTuple<int, long, int, object, DateTime>, ValueTuple<long, int, object, int, DateTimeOffset>>(conversions);

                Assert.AreEqual((1, 2, 3, 4, dto), f((1, 2, 3, 4, dt)));
            }

            {
                var f = CreateTupleConvert<ValueTuple<int, long, int, object, DateTime>?, ValueTuple<long, int, object, int, DateTimeOffset>?>(conversions);

                Assert.AreEqual((1, 2, 3, 4, dto), f((1, 2, 3, 4, dt)));
                Assert.AreEqual(null, f(null));
            }

            {
                var f = CreateTupleConvert<ValueTuple<int, long, int, object, DateTime>, ValueTuple<long, int, object, int, DateTimeOffset>?>(conversions);

                Assert.AreEqual((1, 2, 3, 4, dto), f((1, 2, 3, 4, dt)));
            }

            {
                var f = CreateTupleConvert<ValueTuple<int, long, int, object, DateTime>?, ValueTuple<long, int, object, int, DateTimeOffset>>(conversions);

                Assert.AreEqual((1, 2, 3, 4, dto), f((1, 2, 3, 4, dt)));
                AssertEx.Throws<InvalidOperationException>(() => f(null));
            }
        }

        [TestMethod]
        public void TupleConvert_Reduce_Nested()
        {
            var convertIntToLong = (Expression<Func<int, long>>)(x => x);
            var convertLongToInt = (Expression<Func<long, int>>)(x => (int)x);

            var convertIntToObject = (Expression<Func<int, object>>)(x => x);

            var p = Expression.Parameter(typeof(object));
            var convertObjectToInt = Expression.Lambda<Func<object, int>>(Expression.Unbox(p, typeof(int)), p);

            var inner = CreateTupleConvertExpression<ValueTuple<int, object>, ValueTuple<long, int>>(convertIntToLong, convertObjectToInt);
            var outer = CreateTupleConvert<ValueTuple<long, int, ValueTuple<int, object>>, ValueTuple<int, object, ValueTuple<long, int>>>(convertLongToInt, convertIntToObject, inner);

            Assert.AreEqual((1, 2, (3, 4)), outer((1, 2, (3, 4))));
        }

        private static Func<T, R> CreateTupleConvert<T, R>(params LambdaExpression[] elementConversions)
        {
            return CreateTupleConvertExpression<T, R>(elementConversions).Compile();
        }

        private static Expression<Func<T, R>> CreateTupleConvertExpression<T, R>(params LambdaExpression[] elementConversions)
        {
            var p = Expression.Parameter(typeof(T));
            var c = CSharpExpression.TupleConvert(p, typeof(R), elementConversions);
            return Expression.Lambda<Func<T, R>>(c, p);
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

    /*
     * TODO: Compiler test cases.
     * 
        // Nop
        (Expression<Func<(int,int),(int,int)>>)(t => t)

        // TupleConvert
        (Expression<Func<(int,int),(long,long)>>)(t => t)

        // Convert
        (Expression<Func<(int,int),(int,int)>>)(t => ((int,int))t)
        (Expression<Func<(int,int),(int,int)?>>)(t => t)
        (Expression<Func<(int,int)?,(int,int)>>)(t => ((int,int))t)
        (Expression<Func<(int,int)?,(int,int)?>>)(t => t)

        // TupleConvert (Lifted)
        (Expression<Func<(int,int),(long,long)?>>)(t => t)
        (Expression<Func<(int,int)?,(long,long)>>)(t => ((long,long))t)
        (Expression<Func<(int,int)?,(long,long)?>>)(t => t)
     * 
     */
}
