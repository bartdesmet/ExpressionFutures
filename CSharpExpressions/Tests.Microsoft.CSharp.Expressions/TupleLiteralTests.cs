// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using static Tests.TestHelpers;

namespace Tests
{
    public partial class TupleLiteralTests
    {
        [Fact]
        public void TupleLiteral_Factory_ArgumentChecking()
        {
            var type1 = typeof(ValueTuple<int>);
            var type2 = typeof(ValueTuple<int, bool>);
            var type3 = typeof(ValueTuple<int, bool, string>);
            var type4 = typeof(ValueTuple<int, bool, string, char>);
            var type5 = typeof(ValueTuple<int, bool, string, char, long>);
            var type6 = typeof(ValueTuple<int, bool, string, char, long, double>);
            var type7 = typeof(ValueTuple<int, bool, string, char, long, double, byte>);
            var type8 = typeof(ValueTuple<int, bool, string, char, long, double, byte, ValueTuple<decimal>>);
            var type9 = typeof(ValueTuple<int, bool, string, char, long, double, byte, ValueTuple<decimal, short>>);
            var type10 = typeof(ValueTuple<int, bool, string, char, long, double, byte, ValueTuple<decimal, short, float>>);
            var type11 = typeof(ValueTuple<int, bool, string, char, long, double, byte, ValueTuple<decimal, short, float, ulong>>);
            var type12 = typeof(ValueTuple<int, bool, string, char, long, double, byte, ValueTuple<decimal, short, float, ulong, sbyte>>);
            var type13 = typeof(ValueTuple<int, bool, string, char, long, double, byte, ValueTuple<decimal, short, float, ulong, sbyte, ushort>>);
            var type14 = typeof(ValueTuple<int, bool, string, char, long, double, byte, ValueTuple<decimal, short, float, ulong, sbyte, ushort, uint>>);
            var type15 = typeof(ValueTuple<int, bool, string, char, long, double, byte, ValueTuple<decimal, short, float, ulong, sbyte, ushort, uint, ValueTuple<object>>>);

            var tupleTypes = new[] { type1, type2, type3, type4, type5, type6, type7, type8, type9, type10, type11, type12, type13, type14, type15 };
            var argTypes = new[] { typeof(int), typeof(bool), typeof(string), typeof(char), typeof(long), typeof(double), typeof(byte), typeof(decimal), typeof(short), typeof(float), typeof(ulong), typeof(sbyte), typeof(ushort), typeof(uint), typeof(object) };
            var args = argTypes.Select(t => Expression.Default(t)).ToArray();
            var names = Enumerable.Range(1, tupleTypes.Length).Select(i => ((char)('a' + i)).ToString()).ToArray();

            // null checks
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.TupleLiteral(null, args, null));

            // valid
            for (int i = 0; i < tupleTypes.Length; i++)
            {
                Assert.NotNull(CSharpExpression.TupleLiteral(tupleTypes[i], args.Take(i + 1), null));
                Assert.NotNull(CSharpExpression.TupleLiteral(tupleTypes[i], args.Take(i + 1).ToArray(), null));
                Assert.NotNull(CSharpExpression.TupleLiteral(tupleTypes[i], args.Take(i + 1), names.Take(i + 1)));
            }

            // argument count
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(type1, args.Take(2), null));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(type2, args.Take(1), null));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(type2, args.Take(3), null));

            // name count
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(type2, args.Take(2), new string[0]));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(type2, args.Take(2), names.Take(1)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(type2, args.Take(2), names.Take(3)));

            // invalid tuple type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(typeof(int), args.Take(1), null));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(typeof(List<int>), args.Take(1), null));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(typeof(Tuple<int>), args.Take(1), null));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(typeof(ValueTuple<int, bool, string, char, long, double, byte, decimal>), args.Take(8), null)); // Rest needs to be nested tuple

            // invalid argument type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(type2, new[] { args[0], args[2] }, null));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(type2, new[] { args[1], args[0] }, null));
        }

        [Fact]
        public void TupleLiteral_Factory_ArgumentChecking_InferType()
        {
            // empty
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral());

            // null component
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(new Expression[] { null }));

            // void
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(Expression.Empty()));

            // name count mismatch
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.TupleLiteral(new Expression[] { Expression.Constant(1) }, new string[] { "x", "y" }));
        }

        [Fact]
        public void TupleLiteral_Factory_Properties()
        {
            var type = typeof(ValueTuple<int, bool>);

            var arg1 = Expression.Constant(42);
            var arg2 = Expression.Constant(true);

            var name1 = "x";
            var name2 = "b";

            var res1 = CSharpExpression.TupleLiteral(type, new[] { arg1, arg2 }, null);

            Assert.Same(type, res1.Type);
            Assert.Equal(2, res1.Arguments.Count);
            Assert.Same(arg1, res1.Arguments[0]);
            Assert.Same(arg2, res1.Arguments[1]);
            Assert.Null(res1.ArgumentNames);

            var res2 = CSharpExpression.TupleLiteral(type, new[] { arg1, arg2 }, new[] { name1, name2 });

            Assert.Same(type, res2.Type);
            Assert.Equal(2, res2.Arguments.Count);
            Assert.Same(arg1, res2.Arguments[0]);
            Assert.Same(arg2, res2.Arguments[1]);
            Assert.Equal(2, res2.ArgumentNames.Count);
            Assert.Same(name1, res2.ArgumentNames[0]);
            Assert.Same(name2, res2.ArgumentNames[1]);
        }

        [Fact]
        public void TupleLiteral_Factory_Properties_InferType()
        {
            var type = typeof(ValueTuple<int, bool>);

            var arg1 = Expression.Constant(42);
            var arg2 = Expression.Constant(true);

            var name1 = "x";
            var name2 = "b";

            var res1 = CSharpExpression.TupleLiteral(new[] { arg1, arg2 }, null);

            Assert.Same(type, res1.Type);
            Assert.Equal(2, res1.Arguments.Count);
            Assert.Same(arg1, res1.Arguments[0]);
            Assert.Same(arg2, res1.Arguments[1]);
            Assert.Null(res1.ArgumentNames);

            var res2 = CSharpExpression.TupleLiteral(new[] { arg1, arg2 }, new[] { name1, name2 });

            Assert.Same(type, res2.Type);
            Assert.Equal(2, res2.Arguments.Count);
            Assert.Same(arg1, res2.Arguments[0]);
            Assert.Same(arg2, res2.Arguments[1]);
            Assert.Equal(2, res2.ArgumentNames.Count);
            Assert.Same(name1, res2.ArgumentNames[0]);
            Assert.Same(name2, res2.ArgumentNames[1]);
        }

        [Fact]
        public void TupleLiteral_Update()
        {
            var type = typeof(ValueTuple<int, bool>);

            var arg1 = Expression.Constant(42);
            var arg2 = Expression.Constant(true);

            var arg1_ = Expression.Constant(43);

            var t1 = CSharpExpression.TupleLiteral(type, new[] { arg1, arg2 }, null);

            var r1 = t1.Update(t1.Arguments);
            Assert.Same(t1, r1);

            var r2 = t1.Update(new[] { arg1_, arg2 });
            Assert.Equal(2, r2.Arguments.Count);
            Assert.Same(arg1_, r2.Arguments[0]);
            Assert.Same(arg2, r2.Arguments[1]);
        }

        [Fact]
        public void TupleLiteral_Visitor()
        {
            var res = CSharpExpression.TupleLiteral(typeof(ValueTuple<int>), new[] { Expression.Constant(42) }, null);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitTupleLiteral(TupleLiteralCSharpExpression node)
            {
                Visited = true;

                return base.VisitTupleLiteral(node);
            }
        }

        [Fact]
        public void TupleLiteral_Reduce()
        {
            var res1 = Expression.Lambda<Func<ValueTuple<int>>>(CSharpExpression.TupleLiteral(typeof(ValueTuple<int>), new[] { Expression.Constant(42) }, null));
            var val1 = res1.Compile()();
            Assert.Equal(42, val1.Item1);

            var res2 = Expression.Lambda<Func<(int, bool)>>(CSharpExpression.TupleLiteral(typeof(ValueTuple<int, bool>), new[] { Expression.Constant(42), Expression.Constant(true) }, null));
            var val2 = res2.Compile()();
            Assert.Equal(42, val2.Item1);
            Assert.True(val2.Item2);

            var res7 = Expression.Lambda<Func<(int, int, int, int, int, int, int)>>(CSharpExpression.TupleLiteral(typeof(ValueTuple<int, int, int, int, int, int, int>), Enumerable.Range(1, 7).Select(i => Expression.Constant(i)), null));
            var val7 = res7.Compile()();
            Assert.Equal(1, val7.Item1);
            Assert.Equal(2, val7.Item2);
            Assert.Equal(3, val7.Item3);
            Assert.Equal(4, val7.Item4);
            Assert.Equal(5, val7.Item5);
            Assert.Equal(6, val7.Item6);
            Assert.Equal(7, val7.Item7);

            var res8 = Expression.Lambda<Func<(int, int, int, int, int, int, int, int)>>(CSharpExpression.TupleLiteral(typeof(ValueTuple<int, int, int, int, int, int, int, ValueTuple<int>>), Enumerable.Range(1, 8).Select(i => Expression.Constant(i)), null));
            var val8 = res8.Compile()();
            Assert.Equal(1, val8.Item1);
            Assert.Equal(2, val8.Item2);
            Assert.Equal(3, val8.Item3);
            Assert.Equal(4, val8.Item4);
            Assert.Equal(5, val8.Item5);
            Assert.Equal(6, val8.Item6);
            Assert.Equal(7, val8.Item7);
            Assert.Equal(8, val8.Item8); // NB: smoke and mirrors
            Assert.Equal(8, val8.Rest.Item1);

            var res15 = Expression.Lambda<Func<(int, int, int, int, int, int, int, int, int, int, int, int, int, int, int)>>(CSharpExpression.TupleLiteral(typeof(ValueTuple<int, int, int, int, int, int, int, ValueTuple<int, int, int, int, int, int, int, ValueTuple<int>>>), Enumerable.Range(1, 15).Select(i => Expression.Constant(i)), null));
            var val15 = res15.Compile()();
            Assert.Equal(1, val15.Item1);
            Assert.Equal(2, val15.Item2);
            Assert.Equal(3, val15.Item3);
            Assert.Equal(4, val15.Item4);
            Assert.Equal(5, val15.Item5);
            Assert.Equal(6, val15.Item6);
            Assert.Equal(7, val15.Item7);
            Assert.Equal(8, val15.Item8); // NB: smoke and mirrors
            Assert.Equal(9, val15.Item9); // NB: smoke and mirrors
            Assert.Equal(10, val15.Item10); // NB: smoke and mirrors
            Assert.Equal(11, val15.Item11); // NB: smoke and mirrors
            Assert.Equal(12, val15.Item12); // NB: smoke and mirrors
            Assert.Equal(13, val15.Item13); // NB: smoke and mirrors
            Assert.Equal(14, val15.Item14); // NB: smoke and mirrors
            Assert.Equal(15, val15.Item15); // NB: smoke and mirrors
            Assert.Equal(8, val15.Rest.Item1);
            Assert.Equal(9, val15.Rest.Item2);
            Assert.Equal(10, val15.Rest.Item3);
            Assert.Equal(11, val15.Rest.Item4);
            Assert.Equal(12, val15.Rest.Item5);
            Assert.Equal(13, val15.Rest.Item6);
            Assert.Equal(14, val15.Rest.Item7);
            Assert.Equal(15, val15.Rest.Rest.Item1);
        }

        [Fact]
        public void TupleLiteral_SideEffects()
        {
            AssertCompile<(int, int)>((log, append) =>
            {
                var args = Enumerable.Range(1, 2).Select(i => Expression.Block(log(i.ToString()), Expression.Constant(i)));

                return CSharpExpression.TupleLiteral(typeof(ValueTuple<int, int>), args, null);
            }, new LogAndResult<(int, int)> { Log = { "1", "2" }, Value = (1, 2) });

            AssertCompile<(int, int, int, int, int, int, int, int)>((log, append) =>
            {
                var args = Enumerable.Range(1, 8).Select(i => Expression.Block(log(i.ToString()), Expression.Constant(i)));

                return CSharpExpression.TupleLiteral(typeof(ValueTuple<int, int, int, int, int, int, int, ValueTuple<int>>), args, null);
            }, new LogAndResult<(int, int, int, int, int, int, int, int)> { Log = { "1", "2", "3", "4", "5", "6", "7", "8" }, Value = (1, 2, 3, 4, 5, 6, 7, 8) });
        }

        private void AssertCompile<T>(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<T> expected)
        {
            var res = WithLog<T>(createExpression).Compile()();
            Assert.Equal(expected, res);
        }
    }
}
