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
using static Tests.TestHelpers;

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
                    CSharpExpressionType.PreIncrementAssignChecked,
                    CSharpExpressionType.PreDecrementAssign,
                    CSharpExpressionType.PreDecrementAssignChecked,
                    CSharpExpressionType.PostIncrementAssign,
                    CSharpExpressionType.PostIncrementAssignChecked,
                    CSharpExpressionType.PostDecrementAssign,
                    CSharpExpressionType.PostDecrementAssignChecked,
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

        [TestMethod]
        public void AssignUnary_Parameter_Compile()
        {
            var toString = MethodInfoOf((int a) => a.ToString());

            foreach (var n in new[]
            {
                CSharpExpressionType.PreIncrementAssign,
                CSharpExpressionType.PreIncrementAssignChecked,
                CSharpExpressionType.PreDecrementAssign,
                CSharpExpressionType.PreDecrementAssignChecked,
                CSharpExpressionType.PostIncrementAssign,
                CSharpExpressionType.PostIncrementAssignChecked,
                CSharpExpressionType.PostDecrementAssign,
                CSharpExpressionType.PostDecrementAssignChecked,
            })
            {
                var inc = n.ToString().Contains("Inc");
                var pre = n.ToString().Contains("Pre");

                var exp = new string[2];

                exp[0] = inc ? "42" : "40";
                exp[1] = pre ? exp[0] : "41";

                AssertCompile((log, append) =>
                {
                    var x = Expression.Parameter(typeof(int));
                    var y = Expression.Parameter(typeof(int));

                    return
                        Expression.Block(
                            new[] { x, y },
                            Expression.Assign(x, Expression.Constant(41)),
                            Expression.Assign(y, CSharpExpression.MakeUnaryAssign(n, x, null)),
                            Expression.Invoke(append, Expression.Call(x, toString)),
                            Expression.Invoke(append, Expression.Call(y, toString))
                        );
                }, new LogAndResult<object> { Log = { exp[0], exp[1] } });
            }
        }

        [TestMethod]
        public void AssignUnary_Member_Static_Compile()
        {
            var toString = MethodInfoOf((int a) => a.ToString());

            foreach (var n in new[]
            {
                CSharpExpressionType.PreIncrementAssign,
                CSharpExpressionType.PreIncrementAssignChecked,
                CSharpExpressionType.PreDecrementAssign,
                CSharpExpressionType.PreDecrementAssignChecked,
                CSharpExpressionType.PostIncrementAssign,
                CSharpExpressionType.PostIncrementAssignChecked,
                CSharpExpressionType.PostDecrementAssign,
                CSharpExpressionType.PostDecrementAssignChecked,
            })
            {
                var inc = n.ToString().Contains("Inc");
                var pre = n.ToString().Contains("Pre");

                var exp = new string[2];

                exp[0] = inc ? "42" : "40";
                exp[1] = pre ? exp[0] : "41";

                AssertCompile((log, append) =>
                {
                    var y = Expression.Parameter(typeof(int));

                    var member = Expression.Field(null, typeof(StaticHolder).GetField("Value"));

                    return
                        Expression.Block(
                            new[] { y },
                            Expression.Assign(member, Expression.Constant(41)),
                            Expression.Assign(y, CSharpExpression.MakeUnaryAssign(n, member, null)),
                            Expression.Invoke(append, Expression.Call(member, toString)),
                            Expression.Invoke(append, Expression.Call(y, toString))
                        );
                }, new LogAndResult<object> { Log = { exp[0], exp[1] } });
            }
        }

        [TestMethod]
        public void AssignUnary_Member_Instance_Compile()
        {
            var toString = MethodInfoOf((int a) => a.ToString());

            foreach (var n in new[]
            {
                CSharpExpressionType.PreIncrementAssign,
                CSharpExpressionType.PreIncrementAssignChecked,
                CSharpExpressionType.PreDecrementAssign,
                CSharpExpressionType.PreDecrementAssignChecked,
                CSharpExpressionType.PostIncrementAssign,
                CSharpExpressionType.PostIncrementAssignChecked,
                CSharpExpressionType.PostDecrementAssign,
                CSharpExpressionType.PostDecrementAssignChecked,
            })
            {
                var inc = n.ToString().Contains("Inc");
                var pre = n.ToString().Contains("Pre");

                var exp = new string[2];

                exp[0] = inc ? "42" : "40";
                exp[1] = pre ? exp[0] : "41";

                AssertCompile((log, append) =>
                {
                    var h = Expression.Parameter(typeof(Holder<int>));
                    var y = Expression.Parameter(typeof(int));

                    var member = Expression.Property(h, typeof(Holder<int>).GetProperty("Value"));
                    var field = Expression.Field(h, typeof(Holder<int>).GetField("_value"));

                    return
                        Expression.Block(
                            new[] { h, y },
                            Expression.Assign(h, Expression.New(h.Type.GetConstructors()[0], append, Expression.Constant(41))),
                            Expression.Assign(y, CSharpExpression.MakeUnaryAssign(n, member, null)),
                            Expression.Invoke(append, Expression.Call(field, toString)),
                            Expression.Invoke(append, Expression.Call(y, toString))
                        );
                }, new LogAndResult<object> { Log = { "G", "S", exp[0], exp[1] } });
            }
        }

        [TestMethod]
        public void AssignUnary_Index_Compile()
        {
            var toString = MethodInfoOf((int a) => a.ToString());

            foreach (var n in new[]
            {
                CSharpExpressionType.PreIncrementAssign,
                CSharpExpressionType.PreIncrementAssignChecked,
                CSharpExpressionType.PreDecrementAssign,
                CSharpExpressionType.PreDecrementAssignChecked,
                CSharpExpressionType.PostIncrementAssign,
                CSharpExpressionType.PostIncrementAssignChecked,
                CSharpExpressionType.PostDecrementAssign,
                CSharpExpressionType.PostDecrementAssignChecked,
            })
            {
                var inc = n.ToString().Contains("Inc");
                var pre = n.ToString().Contains("Pre");

                var exp = new string[2];

                exp[0] = inc ? "42" : "40";
                exp[1] = pre ? exp[0] : "41";

                AssertCompile((log, append) =>
                {
                    var h = Expression.Parameter(typeof(Holder<int>));
                    var y = Expression.Parameter(typeof(int));

                    var index =
                        Expression.MakeIndex(
                            h,
                            typeof(Holder<int>).GetProperty("Item"),
                            new Expression[]
                            {
                                Expression.Block(log("I1"), Expression.Constant(1)),
                                Expression.Block(log("I2"), Expression.Constant(2)),
                            }
                        );

                    var field = Expression.Field(h, typeof(Holder<int>).GetField("_value"));

                    return
                        Expression.Block(
                            new[] { h, y },
                            Expression.Assign(h, Expression.New(h.Type.GetConstructors()[0], append, Expression.Constant(41))),
                            Expression.Assign(y, CSharpExpression.MakeUnaryAssign(n, index, null)),
                            Expression.Invoke(append, Expression.Call(field, toString)),
                            Expression.Invoke(append, Expression.Call(y, toString))
                        );
                }, new LogAndResult<object> { Log = { "I1", "I2", "GI", "SI", exp[0], exp[1] } });
            }
        }

        [TestMethod]
        public void AssignUnary_CSharpIndex_Compile()
        {
            var toString = MethodInfoOf((int a) => a.ToString());

            foreach (var n in new[]
            {
                CSharpExpressionType.PreIncrementAssign,
                CSharpExpressionType.PreIncrementAssignChecked,
                CSharpExpressionType.PreDecrementAssign,
                CSharpExpressionType.PreDecrementAssignChecked,
                CSharpExpressionType.PostIncrementAssign,
                CSharpExpressionType.PostIncrementAssignChecked,
                CSharpExpressionType.PostDecrementAssign,
                CSharpExpressionType.PostDecrementAssignChecked,
            })
            {
                var inc = n.ToString().Contains("Inc");
                var pre = n.ToString().Contains("Pre");

                var exp = new string[2];

                exp[0] = inc ? "42" : "40";
                exp[1] = pre ? exp[0] : "41";

                AssertCompile((log, append) =>
                {
                    var h = Expression.Parameter(typeof(Holder<int>));
                    var y = Expression.Parameter(typeof(int));

                    var item = typeof(Holder<int>).GetProperty("Item");
                    var pars = item.GetIndexParameters();

                    var index =
                        CSharpExpression.Index(
                            h,
                            item,
                            new[]
                            {
                                CSharpExpression.Bind(pars[1], Expression.Block(log("I1"), Expression.Constant(2))),
                                CSharpExpression.Bind(pars[0], Expression.Block(log("I2"), Expression.Constant(1))),
                            }
                        );

                    var field = Expression.Field(h, typeof(Holder<int>).GetField("_value"));

                    return
                        Expression.Block(
                            new[] { h, y },
                            Expression.Assign(h, Expression.New(h.Type.GetConstructors()[0], append, Expression.Constant(41))),
                            Expression.Assign(y, CSharpExpression.MakeUnaryAssign(n, index, null)),
                            Expression.Invoke(append, Expression.Call(field, toString)),
                            Expression.Invoke(append, Expression.Call(y, toString))
                        );
                }, new LogAndResult<object> { Log = { "I1", "I2", "GI", "SI", exp[0], exp[1] } });
            }
        }

        [TestMethod]
        public void AssignUnary_Overflow()
        {
            var x = Expression.Parameter(typeof(int));

            {
                var f = Expression.Lambda<Func<int, int>>(CSharpExpression.PostIncrementAssignChecked(x), x).Compile();
                AssertEx.Throws<OverflowException>(() => f(int.MaxValue));
            }

            {
                var f = Expression.Lambda<Func<int, int>>(CSharpExpression.PreIncrementAssignChecked(x), x).Compile();
                AssertEx.Throws<OverflowException>(() => f(int.MaxValue));
            }

            {
                var f = Expression.Lambda<Func<int, int>>(CSharpExpression.PostDecrementAssignChecked(x), x).Compile();
                AssertEx.Throws<OverflowException>(() => f(int.MinValue));
            }

            {
                var f = Expression.Lambda<Func<int, int>>(CSharpExpression.PostDecrementAssignChecked(x), x).Compile();
                AssertEx.Throws<OverflowException>(() => f(int.MinValue));
            }
        }

        [TestMethod]
        public void AssignUnary_VariousTypes()
        {
            AssertPreIncrementChecked<byte>(41, 42);
            AssertPreIncrementChecked<sbyte>(41, 42);
            AssertPreIncrementChecked<short>(41, 42);
            AssertPreIncrementChecked<ushort>(41, 42);
            AssertPreIncrementChecked<int>(41, 42);
            AssertPreIncrementChecked<uint>(41, 42);
            AssertPreIncrementChecked<long>(41, 42);
            AssertPreIncrementChecked<ulong>(41, 42);
            AssertPreIncrementChecked<float>(41, 42);
            AssertPreIncrementChecked<double>(41, 42);
        }

        private static void AssertPreIncrementChecked<T>(T value, T plusOne)
        {
            var v = Expression.Parameter(typeof(T));
            var f =
                Expression.Lambda<Func<T>>(
                    Expression.Block(
                        new[] { v },
                        Expression.Assign(v, Expression.Constant(value)),
                        CSharpExpression.PreIncrementAssignChecked(v)
                    )
                ).Compile();
            Assert.AreEqual(plusOne, f());
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

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }

        class StaticHolder
        {
#pragma warning disable CS0649
            public static int Value;
#pragma warning restore
        }

        class Holder<T>
        {
            private readonly Action<string> _append;
            public T _value;

            public Holder(Action<string> append, T value)
            {
                _append = append;
                _value = value;
            }

            public T this[int x, int y]
            {
                get
                {
                    Assert.AreEqual(1, x);
                    Assert.AreEqual(2, y);

                    _append("GI");
                    return _value;
                }

                set
                {
                    Assert.AreEqual(1, x);
                    Assert.AreEqual(2, y);

                    _append("SI");
                    _value = value;
                }
            }

            public T Value
            {
                get
                {
                    _append("G");
                    return _value;
                }

                set
                {
                    _append("S");
                    _value = value;
                }
            }
        }
    }
}
