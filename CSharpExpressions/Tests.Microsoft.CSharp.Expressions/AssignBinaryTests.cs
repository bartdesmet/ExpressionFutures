// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    public partial class AssignBinaryTests
    {
        [Fact]
        public void AssignBinary_Factory_ArgumentChecking()
        {
            var l = Expression.Parameter(typeof(int));
            var r = Expression.Constant(2);

            Assert.Throws<ArgumentException>(() => CSharpExpression.MakeBinaryAssign(CSharpExpressionType.Await, l, r, null, null, null));
        }

        [Fact]
        public void AssignBinary_Factory_String_ArgumentChecking()
        {
            var s = Expression.Parameter(typeof(string));

            Assert.Throws<ArgumentException>(() => CSharpExpression.SubtractAssign(s, Expression.Default(typeof(string))));
            Assert.Throws<ArgumentException>(() => CSharpExpression.SubtractAssign(s, Expression.Default(typeof(object))));

            // the following are valid
            Assert.NotNull(CSharpExpression.AddAssign(s, Expression.Default(typeof(string))));
            Assert.NotNull(CSharpExpression.AddAssign(s, Expression.Default(typeof(int))));
            Assert.NotNull(CSharpExpression.AddAssign(s, Expression.Default(typeof(object))));
            Assert.NotNull(CSharpExpression.AddAssignChecked(s, Expression.Default(typeof(string))));
            Assert.NotNull(CSharpExpression.AddAssignChecked(s, Expression.Default(typeof(int))));
            Assert.NotNull(CSharpExpression.AddAssignChecked(s, Expression.Default(typeof(object))));
        }

        [Fact]
        public void AssignBinary_Factory_Delegate_ArgumentChecking()
        {
            // NB: LINQ checks this one
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.AddAssignChecked(Expression.Parameter(typeof(Delegate)), Expression.Default(typeof(Delegate))));

            // NB: Our library checks this one (TODO: should we make the exceptions consistent?)
            Assert.Throws<ArgumentException>(() => CSharpExpression.SubtractAssignChecked(Expression.Parameter(typeof(MulticastDelegate)), Expression.Default(typeof(MulticastDelegate))));

            var d = Expression.Parameter(typeof(Action<string>));

            Assert.Throws<ArgumentException>(() => CSharpExpression.AddAssignChecked(d, Expression.Default(typeof(Action))));
            Assert.Throws<ArgumentException>(() => CSharpExpression.SubtractAssign(d, Expression.Default(typeof(Action<int>))));
            Assert.Throws<ArgumentException>(() => CSharpExpression.DivideAssign(d, Expression.Default(typeof(Action<string>))));

            // the following are valid
            Assert.NotNull(CSharpExpression.AddAssign(d, Expression.Default(typeof(Action<string>))));
            Assert.NotNull(CSharpExpression.AddAssign(d, Expression.Default(typeof(Action<object>))));
            Assert.NotNull(CSharpExpression.AddAssignChecked(d, Expression.Default(typeof(Action<string>))));
            Assert.NotNull(CSharpExpression.AddAssignChecked(d, Expression.Default(typeof(Action<object>))));
            Assert.NotNull(CSharpExpression.SubtractAssign(d, Expression.Default(typeof(Action<string>))));
            Assert.NotNull(CSharpExpression.SubtractAssign(d, Expression.Default(typeof(Action<object>))));
            Assert.NotNull(CSharpExpression.SubtractAssignChecked(d, Expression.Default(typeof(Action<string>))));
            Assert.NotNull(CSharpExpression.SubtractAssignChecked(d, Expression.Default(typeof(Action<object>))));
        }

        [Fact]
        public void AssignBinary_Factory_MakeBinaryAssign()
        {
            foreach (var l in GetLhs())
            {
                var r = Expression.Constant(2);
                var m = MethodInfoOf(() => Op(0, 0));
                var x = Expression.Parameter(typeof(int));
                var cf = Expression.Lambda(x, x);
                var cl = Expression.Lambda(x, x);

                foreach (var n in new[]
                {
                    CSharpExpressionType.Assign,
                    CSharpExpressionType.AddAssign,
                    CSharpExpressionType.AndAssign,
                    CSharpExpressionType.DivideAssign,
                    CSharpExpressionType.ExclusiveOrAssign,
                    CSharpExpressionType.LeftShiftAssign,
                    CSharpExpressionType.ModuloAssign,
                    CSharpExpressionType.MultiplyAssign,
                    CSharpExpressionType.OrAssign,
                    CSharpExpressionType.RightShiftAssign,
                    CSharpExpressionType.SubtractAssign,
                    CSharpExpressionType.AddAssignChecked,
                    CSharpExpressionType.MultiplyAssignChecked,
                    CSharpExpressionType.SubtractAssignChecked,
                })
                {
                    var a1 = CSharpExpression.MakeBinaryAssign(n, l, r, m, cf, cl);
                    Assert.Equal(n, a1.CSharpNodeType);
                    Assert.Same(l, a1.Left);
                    Assert.Same(r, a1.Right);
                    Assert.Equal(typeof(int), a1.Type);
                    Assert.False(a1.IsLiftedToNull);
                    Assert.False(a1.IsLifted);

                    if (n == CSharpExpressionType.Assign)
                    {
                        Assert.Null(a1.Method);
                        Assert.Null(a1.LeftConversion);
                        Assert.Null(a1.FinalConversion);
                    }
                    else
                    {
                        Assert.Same(m, a1.Method);
                        Assert.Same(cl, a1.LeftConversion);
                        Assert.Same(cf, a1.FinalConversion);
                    }

                    var a2 = CSharpExpression.MakeBinaryAssign(n, l, r, m, null, null);
                    Assert.Equal(n, a2.CSharpNodeType);
                    Assert.Same(l, a2.Left);
                    Assert.Same(r, a2.Right);
                    Assert.Equal(typeof(int), a2.Type);
                    Assert.False(a2.IsLiftedToNull);
                    Assert.False(a2.IsLifted);
                    Assert.Null(a2.LeftConversion);
                    Assert.Null(a2.FinalConversion);

                    if (n == CSharpExpressionType.Assign)
                    {
                        Assert.Null(a2.Method);
                    }
                    else
                    {
                        Assert.Same(m, a2.Method);
                    }

                    var a3 = CSharpExpression.MakeBinaryAssign(n, l, r, null, null, null);
                    Assert.Equal(n, a3.CSharpNodeType);
                    Assert.Same(l, a3.Left);
                    Assert.Same(r, a3.Right);
                    Assert.Equal(typeof(int), a3.Type);
                    Assert.False(a3.IsLiftedToNull);
                    Assert.False(a3.IsLifted);
                    Assert.Null(a3.Method);
                    Assert.Null(a3.LeftConversion);
                    Assert.Null(a3.FinalConversion);
                }
            }
        }

        [Fact]
        public void AssignBinary_Parameter_Compile()
        {
            foreach (var t in new[]
            {
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(char),
                typeof(float),
                typeof(double),
                typeof(decimal),
            })
            {
                var toString = t.GetMethod("ToString", Array.Empty<Type>());

                var exp = "42";
                if (t == typeof(char))
                {
                    exp = ((char)42).ToString();
                }

                AssertCompile((log, append) =>
                {
                    var x = Expression.Parameter(t);
                    var y = Expression.Parameter(t);

                    var val = Expression.Convert(Expression.Constant(41), t);
                    var one = Expression.Convert(Expression.Constant(1), t);

                    return
                        Expression.Block(
                            new[] { x, y },
                            Expression.Assign(x, val),
                            Expression.Assign(y, CSharpExpression.AddAssign(x, Expression.Block(log("V"), one))),
                            Expression.Invoke(append, Expression.Call(x, toString)),
                            Expression.Invoke(append, Expression.Call(y, toString))
                        );
                }, new LogAndResult<object> { Log = { "V", exp, exp } });
            }
        }

        [Fact]
        public void AssignBinary_Index_Compile()
        {
            foreach (var t in new[]
            {
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(char),
                typeof(float),
                typeof(double),
                typeof(decimal),
            })
            {
                var toString = t.GetMethod("ToString", Array.Empty<Type>());

                var exp = "42";
                if (t == typeof(char))
                {
                    exp = ((char)42).ToString();
                }

                AssertCompile((log, append) =>
                {
                    var y = Expression.Parameter(t);

                    var val = Expression.Convert(Expression.Constant(41), t);
                    var one = Expression.Convert(Expression.Constant(1), t);

                    var list = typeof(List<>).MakeGenericType(t);
                    var ctor = list.GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(t) });
                    var item = list.GetProperty("Item");

                    var index =
                        CSharpExpression.Index(
                            Expression.Block(
                                log("L"),
                                Expression.New(ctor, Expression.NewArrayInit(t, Expression.Default(t), val))
                            ),
                            item,
                            CSharpExpression.Bind(
                                item.GetIndexParameters()[0],
                                Expression.Block(
                                    log("I"),
                                    Expression.Constant(1)
                                )
                            )
                        );

                    return
                        Expression.Block(
                            new[] { y },
                            Expression.Assign(y, CSharpExpression.AddAssign(index, Expression.Block(log("V"), one))),
                            Expression.Invoke(append, Expression.Call(y, toString))
                        );
                }, new LogAndResult<object> { Log = { "L", "I", "V", exp } });
            }

            // TODO: tests with multiple indexer parameters out of order
        }

        [Fact]
        public void AssignBinary_Compile_Overflow()
        {
            var p = Expression.Parameter(typeof(byte));
            var f = Expression.Lambda<Func<byte, byte>>(CSharpExpression.AddAssignChecked(p, Expression.Constant((byte)1, typeof(byte))), p).Compile();

            Assert.Equal((byte)42, f(41));
            Assert.Throws<OverflowException>(() => f(byte.MaxValue));
        }

        [Fact]
        public void AssignBinary_CustomMethod()
        {
            var p = Expression.Parameter(typeof(byte));
            var m = typeof(AssignBinaryTests).GetMethod(nameof(ByteOp), BindingFlags.NonPublic | BindingFlags.Static);
            var f = Expression.Lambda<Func<byte, byte>>(CSharpExpression.AddAssign(p, Expression.Constant((byte)1, typeof(byte)), m), p).Compile();

            Assert.Equal((byte)40, f(41));
        }

        [Fact]
        public void AssignBinary_CustomConverts()
        {
            var p = Expression.Parameter(typeof(byte));

            // (byte lhs) => (int)(lhs * 2)
            var x = Expression.Parameter(typeof(byte));
            var c = Expression.Lambda(Expression.Multiply(Expression.Convert(x, typeof(int)), Expression.Constant(2)), x);

            // (int res) => (byte)(res / 3)
            var y = Expression.Parameter(typeof(int));
            var d = Expression.Lambda(Expression.Convert(Expression.Divide(y, Expression.Constant(3)), typeof(byte)), y);

            // (int lhs) => lhs + 1
            var f = Expression.Lambda<Func<byte, byte>>(CSharpExpression.AddAssign(p, Expression.Constant(1, typeof(int)), null, d, c), p).Compile();

            Assert.Equal((byte)7, f(10));
        }

        [Fact]
        public void AssignBinary_MutableStruct_Field()
        {
            var p = Expression.Parameter(typeof(MutableStruct<int>));
            var f = Expression.Field(p, "Value");
            var a = CSharpExpression.AddAssign(f, Expression.Constant(1));
            var b = Expression.Call(typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) }), Expression.Convert(a, typeof(object)), Expression.Convert(f, typeof(object)));
            var l = Expression.Lambda<Func<MutableStruct<int>, string>>(b, p);
            var c = l.Compile();

            var m = new MutableStruct<int>();
            var r = c(m);
            Assert.Equal("11", r);
        }

        [Fact]
        public void AssignBinary_MutableStruct_Index()
        {
            var p = Expression.Parameter(typeof(MutableStruct<int>));
            var f = Expression.MakeIndex(p, p.Type.GetProperty("Item"), new[] { Expression.Constant(0) });
            var a = CSharpExpression.AddAssign(f, Expression.Constant(1));
            var b = Expression.Call(typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) }), Expression.Convert(a, typeof(object)), Expression.Convert(f, typeof(object)));
            var l = Expression.Lambda<Func<MutableStruct<int>, string>>(b, p);
            var c = l.Compile();

            var m = new MutableStruct<int>();
            var r = c(m);
            Assert.Equal("11", r);
        }

        [Fact]
        public void AssignBinary_Visitor()
        {
            var res = CSharpExpression.MakeBinaryAssign(CSharpExpressionType.AddAssign, Expression.Parameter(typeof(int)), Expression.Constant(1), null, null, null);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        [Fact]
        public void AssignBinary_NullCoalescing_Factory_ArgumentChecking()
        {
            var p0 = Expression.Parameter(typeof(int));
            var p1 = Expression.Parameter(typeof(int));

            var p2 = Expression.Parameter(typeof(int?));
            var p3 = Expression.Parameter(typeof(int?));

            var p4 = Expression.Parameter(typeof(string));
            var p5 = Expression.Parameter(typeof(string));

            // NB: Inherits exception behavior from ValidateCoalesceArgTypes.
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.NullCoalescingAssign(p0, p1));
            Assert.Throws<ArgumentException>(() => CSharpExpression.NullCoalescingAssign(p4, p2));
            Assert.Throws<ArgumentException>(() => CSharpExpression.NullCoalescingAssign(p2, p4));

            // the following are valid
            CSharpExpression.NullCoalescingAssign(p2, p0);
            CSharpExpression.NullCoalescingAssign(p2, p3);
            CSharpExpression.NullCoalescingAssign(p4, p5);
        }

        [Fact]
        public void AssignBinary_NullCoalescing_ReferenceType()
        {
            var p = Expression.Parameter(typeof(string));

            var f1 = Expression.Lambda<Func<string, string>>(Expression.Block(CSharpExpression.NullCoalescingAssign(p, Expression.Constant("foo"))), p);
            var f2 = Expression.Lambda<Func<string, string>>(Expression.Block(CSharpExpression.NullCoalescingAssign(p, Expression.Constant("foo")), p), p);

            var d1 = f1.Compile();
            var d2 = f2.Compile();

            Assert.Equal("bar", d1("bar"));
            Assert.Equal("bar", d2("bar"));

            Assert.Equal("foo", d1(null));
            Assert.Equal("foo", d2(null));
        }

        [Fact]
        public void AssignBinary_NullCoalescing_NullableValueType()
        {
            var p = Expression.Parameter(typeof(int?));

            var f1 = Expression.Lambda<Func<int?, int>>(Expression.Block(CSharpExpression.NullCoalescingAssign(p, Expression.Constant(42))), p);
            var f2 = Expression.Lambda<Func<int?, int?>>(Expression.Block(CSharpExpression.NullCoalescingAssign(p, Expression.Constant(42)), p), p);

            var d1 = f1.Compile();
            var d2 = f2.Compile();

            Assert.Equal(1, d1(1));
            Assert.Equal(1, d2(1));

            Assert.Equal(42, d1(null));
            Assert.Equal(42, d2(null));
        }

        [Fact]
        public void AssignBinary_NullCoalescing_Index_SpillArgsToTemps()
        {
            var toString = typeof(int).GetMethod("ToString", Array.Empty<Type>());

            foreach (var val in new[] { new List<int?> { 41, null, 43 }, new List<int?> { 41, 42, 43 } })
            {
                var exp = (val[1] ?? 99).ToString();

                AssertCompile((log, append) =>
                {
                    var xs = Expression.Parameter(typeof(List<int?>));
                    var x = Expression.Parameter(typeof(int));

                    var one = Expression.Constant(1);
                    var index = Expression.Block(log("I"), one);
                    var item = typeof(List<int?>).GetProperty("Item");
                    var element_side_effect = Expression.MakeIndex(xs,item, new Expression[] { index });
                    var element_no_side_effect = Expression.MakeIndex(xs, item, new Expression[] { one });

                    return
                        Expression.Block(
                            new[] { xs, x },
                            Expression.Assign(xs, Expression.Constant(val)),
                            Expression.Assign(x, CSharpExpression.NullCoalescingAssign(element_side_effect, Expression.Constant(99))),
                            Expression.Invoke(append, Expression.Call(x, toString)),
                            Expression.Invoke(append, Expression.Call(Expression.Convert(element_no_side_effect, typeof(int)), toString))
                        );
                }, new LogAndResult<object> { Log = { "I", exp, exp } });
            }
        }

        [Fact]
        public void AssignBinary_NullCoalescing_Index_DontReadMultipleTimes()
        {
            var toString = typeof(int).GetMethod("ToString", Array.Empty<Type>());

            foreach (var val in new[] { new List<int?> { 41, null, 43 }, new List<int?> { 41, 42, 43 } })
            {
                var exp = (val[1] ?? 99).ToString();

                AssertCompile((log, append) =>
                {
                    var xs = Expression.Parameter(typeof(IndexOnceGetOnceSet));
                    var x = Expression.Parameter(typeof(int));

                    var one = Expression.Constant(1);
                    var index = Expression.Block(log("I"), one);
                    var element_side_effect = Expression.MakeIndex(xs, typeof(IndexOnceGetOnceSet).GetProperty("Item"), new Expression[] { index });
                    var element_no_side_effect = Expression.Invoke(Expression.Constant(new Func<int?>(() => val[1])));

                    return
                        Expression.Block(
                            new[] { xs, x },
                            Expression.Assign(xs, Expression.Constant(new IndexOnceGetOnceSet { Values = val })),
                            Expression.Assign(x, CSharpExpression.NullCoalescingAssign(element_side_effect, Expression.Constant(99))),
                            Expression.Invoke(append, Expression.Call(x, toString)),
                            Expression.Invoke(append, Expression.Call(Expression.Convert(element_no_side_effect, typeof(int)), toString))
                        );
                }, new LogAndResult<object> { Log = { "I", exp, exp } });
            }
        }

        private static int Op(int x, int y)
        {
            throw new NotImplementedException();
        }

        private static byte ByteOp(byte a, byte b)
        {
            return (byte)(a - b);
        }

        private static IEnumerable<Expression> GetLhs()
        {
            yield return Expression.Parameter(typeof(int));
            yield return Expression.Field(Expression.Parameter(typeof(StrongBox<int>)), "Value");
            yield return Expression.MakeIndex(Expression.Parameter(typeof(List<int>)), typeof(List<int>).GetProperty("Item"), new[] { Expression.Constant(0) });
            yield return Expression.ArrayAccess(Expression.Parameter(typeof(int[])), Expression.Constant(0));
            yield return CSharpExpression.Index(Expression.Parameter(typeof(List<int>)), typeof(List<int>).GetProperty("Item"), CSharpExpression.Bind(typeof(List<int>).GetProperty("Item").GetIndexParameters()[0], Expression.Constant(0)));
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.Equal(expected, res);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitBinaryAssign(AssignBinaryCSharpExpression node)
            {
                Visited = true;

                return base.VisitBinaryAssign(node);
            }
        }
    }

    struct MutableStruct<T>
    {
        public T this[int x]
        {
            get { return Value; }
            set { Value = value; }
        }

        public T Value;
    }

    class IndexOnceGetOnceSet
    {
        public List<int?> Values;

        public int? this[int i]
        {
            get
            {
                Assert.False(++_getCount > 1);

                return Values[i];
            }

            set
            {
                Assert.False(++_setCount > 1);

                Values[i] = value;
            }
        }

        private int _getCount;
        private int _setCount;
    }
}
