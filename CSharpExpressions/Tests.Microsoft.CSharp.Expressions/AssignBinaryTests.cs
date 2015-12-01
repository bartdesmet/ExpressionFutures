// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public partial class AssignBinaryTests
    {
        [TestMethod]
        public void AssignBinary_Factory_ArgumentChecking()
        {
            var l = Expression.Parameter(typeof(int));
            var r = Expression.Constant(2);

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.MakeBinaryAssign(CSharpExpressionType.Await, l, r, null, null, null));
        }

        [TestMethod]
        public void AssignBinary_Factory_String_ArgumentChecking()
        {
            var s = Expression.Parameter(typeof(string));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.SubtractAssign(s, Expression.Default(typeof(string))));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.SubtractAssign(s, Expression.Default(typeof(object))));

            // the following are valid
            Assert.IsNotNull(CSharpExpression.AddAssign(s, Expression.Default(typeof(string))));
            Assert.IsNotNull(CSharpExpression.AddAssign(s, Expression.Default(typeof(int))));
            Assert.IsNotNull(CSharpExpression.AddAssign(s, Expression.Default(typeof(object))));
            Assert.IsNotNull(CSharpExpression.AddAssignChecked(s, Expression.Default(typeof(string))));
            Assert.IsNotNull(CSharpExpression.AddAssignChecked(s, Expression.Default(typeof(int))));
            Assert.IsNotNull(CSharpExpression.AddAssignChecked(s, Expression.Default(typeof(object))));
        }

        [TestMethod]
        public void AssignBinary_Factory_Delegate_ArgumentChecking()
        {
            // NB: LINQ checks this one
            AssertEx.Throws<InvalidOperationException>(() => CSharpExpression.AddAssignChecked(Expression.Parameter(typeof(Delegate)), Expression.Default(typeof(Delegate))));

            // NB: Our library checks this one (TODO: should we make the exceptions consistent?)
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.SubtractAssignChecked(Expression.Parameter(typeof(MulticastDelegate)), Expression.Default(typeof(MulticastDelegate))));

            var d = Expression.Parameter(typeof(Action<string>));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AddAssignChecked(d, Expression.Default(typeof(Action))));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.SubtractAssign(d, Expression.Default(typeof(Action<int>))));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.DivideAssign(d, Expression.Default(typeof(Action<string>))));

            // the following are valid
            Assert.IsNotNull(CSharpExpression.AddAssign(d, Expression.Default(typeof(Action<string>))));
            Assert.IsNotNull(CSharpExpression.AddAssign(d, Expression.Default(typeof(Action<object>))));
            Assert.IsNotNull(CSharpExpression.AddAssignChecked(d, Expression.Default(typeof(Action<string>))));
            Assert.IsNotNull(CSharpExpression.AddAssignChecked(d, Expression.Default(typeof(Action<object>))));
            Assert.IsNotNull(CSharpExpression.SubtractAssign(d, Expression.Default(typeof(Action<string>))));
            Assert.IsNotNull(CSharpExpression.SubtractAssign(d, Expression.Default(typeof(Action<object>))));
            Assert.IsNotNull(CSharpExpression.SubtractAssignChecked(d, Expression.Default(typeof(Action<string>))));
            Assert.IsNotNull(CSharpExpression.SubtractAssignChecked(d, Expression.Default(typeof(Action<object>))));
        }

        [TestMethod]
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
                    Assert.AreEqual(n, a1.CSharpNodeType);
                    Assert.AreSame(l, a1.Left);
                    Assert.AreSame(r, a1.Right);
                    Assert.AreEqual(typeof(int), a1.Type);
                    Assert.IsFalse(a1.IsLiftedToNull);
                    Assert.IsFalse(a1.IsLifted);

                    if (n == CSharpExpressionType.Assign)
                    {
                        Assert.IsNull(a1.Method);
                        Assert.IsNull(a1.LeftConversion);
                        Assert.IsNull(a1.FinalConversion);
                    }
                    else
                    {
                        Assert.AreSame(m, a1.Method);
                        Assert.AreSame(cl, a1.LeftConversion);
                        Assert.AreSame(cf, a1.FinalConversion);
                    }

                    var a2 = CSharpExpression.MakeBinaryAssign(n, l, r, m, null, null);
                    Assert.AreEqual(n, a2.CSharpNodeType);
                    Assert.AreSame(l, a2.Left);
                    Assert.AreSame(r, a2.Right);
                    Assert.AreEqual(typeof(int), a2.Type);
                    Assert.IsFalse(a2.IsLiftedToNull);
                    Assert.IsFalse(a2.IsLifted);
                    Assert.IsNull(a2.LeftConversion);
                    Assert.IsNull(a2.FinalConversion);

                    if (n == CSharpExpressionType.Assign)
                    {
                        Assert.IsNull(a2.Method);
                    }
                    else
                    {
                        Assert.AreSame(m, a2.Method);
                    }

                    var a3 = CSharpExpression.MakeBinaryAssign(n, l, r, null, null, null);
                    Assert.AreEqual(n, a3.CSharpNodeType);
                    Assert.AreSame(l, a3.Left);
                    Assert.AreSame(r, a3.Right);
                    Assert.AreEqual(typeof(int), a3.Type);
                    Assert.IsFalse(a3.IsLiftedToNull);
                    Assert.IsFalse(a3.IsLifted);
                    Assert.IsNull(a3.Method);
                    Assert.IsNull(a3.LeftConversion);
                    Assert.IsNull(a3.FinalConversion);
                }
            }
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void AssignBinary_Compile_Overflow()
        {
            var p = Expression.Parameter(typeof(byte));
            var f = Expression.Lambda<Func<byte, byte>>(CSharpExpression.AddAssignChecked(p, Expression.Constant((byte)1, typeof(byte))), p).Compile();

            Assert.AreEqual((byte)42, f(41));
            AssertEx.Throws<OverflowException>(() => f(byte.MaxValue));
        }

        [TestMethod]
        public void AssignBinary_CustomMethod()
        {
            var p = Expression.Parameter(typeof(byte));
            var m = typeof(AssignBinaryTests).GetMethod(nameof(ByteOp), BindingFlags.NonPublic | BindingFlags.Static);
            var f = Expression.Lambda<Func<byte, byte>>(CSharpExpression.AddAssign(p, Expression.Constant((byte)1, typeof(byte)), m), p).Compile();

            Assert.AreEqual((byte)40, f(41));
        }

        [TestMethod]
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

            Assert.AreEqual((byte)7, f(10));
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
            Assert.AreEqual(expected, res);
        }
    }
}
