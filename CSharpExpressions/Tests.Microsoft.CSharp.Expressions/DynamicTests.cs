// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Tests
{
    [TestClass]
    public partial class DynamicTests
    {
        [TestMethod]
        public void Dynamic_InvokeMember()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvokeMember(p, "Substring", Expression.Constant(1));
            Assert.AreEqual(CSharpExpressionType.DynamicInvokeMember, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.AreEqual("ar", f("bar"));
        }

        [TestMethod]
        public void Dynamic_InvokeMember_Generic()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvokeMember(p, "Bar", new[] { typeof(int) });
            Assert.AreEqual(CSharpExpressionType.DynamicInvokeMember, d.CSharpNodeType);

            AssertNoChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.AreEqual("Int32", f(new Foo()));
        }

        [TestMethod]
        public void Dynamic_InvokeMember_Factories()
        {
            var p = Expression.Parameter(typeof(object));
            var m = "bar";
            var a = Expression.Constant(1);
            var d = DynamicCSharpExpression.DynamicArgument(a);
            var t = new Type[0];

            var es = new[]
            {
                DynamicCSharpExpression.DynamicInvokeMember(p, m, new[] { a }),
                DynamicCSharpExpression.DynamicInvokeMember(p, m, new[] { a }.AsEnumerable()),
                DynamicCSharpExpression.DynamicInvokeMember(p, m, new[] { d }),
                DynamicCSharpExpression.DynamicInvokeMember(p, m, new[] { d }.AsEnumerable()),
                DynamicCSharpExpression.DynamicInvokeMember(p, m, t, new[] { a }),
                DynamicCSharpExpression.DynamicInvokeMember(p, m, t, new[] { a }.AsEnumerable()),
                DynamicCSharpExpression.DynamicInvokeMember(p, m, t, new[] { d }),
                DynamicCSharpExpression.DynamicInvokeMember(p, m, t, new[] { d }.AsEnumerable()),
                DynamicCSharpExpression.DynamicInvokeMember(p, m, t, new[] { d }.AsEnumerable(), CSharpBinderFlags.None),
                DynamicCSharpExpression.DynamicInvokeMember(p, m, t, new[] { d }.AsEnumerable(), CSharpBinderFlags.None, null),
            };

            foreach (var e in es)
            {
                Assert.AreEqual(CSharpExpressionType.DynamicInvokeMember, e.CSharpNodeType);

                Assert.AreSame(p, e.Object);
                Assert.IsNull(e.Target);

                Assert.AreEqual(m, e.Name);
                Assert.AreEqual(0, e.TypeArguments.Count);

                Assert.AreEqual(1, e.Arguments.Count);
                Assert.AreSame(a, e.Arguments[0].Expression);

                Assert.IsNull(e.Context);
                Assert.AreEqual(CSharpBinderFlags.None, e.Flags);
            }
        }

        [TestMethod]
        public void Dynamic_InvokeMember_Static()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvokeMember(typeof(string), "Concat", p, Expression.Constant("!"));
            Assert.AreEqual(CSharpExpressionType.DynamicInvokeMember, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.AreEqual("bar!", f("bar"));
        }

        [TestMethod]
        public void Dynamic_InvokeMember_Static_Generic()
        {
            var d = DynamicCSharpExpression.DynamicInvokeMember(typeof(Foo), "Qux", new[] { typeof(int) });
            Assert.AreEqual(CSharpExpressionType.DynamicInvokeMember, d.CSharpNodeType);

            AssertNoChange(d);

            var e = Expression.Lambda<Func<object>>(d);
            var f = e.Compile();
            Assert.AreEqual("Int32", f());
        }

        [TestMethod]
        public void Dynamic_InvokeMember_Static_Factories()
        {
            var p = Expression.Parameter(typeof(object));
            var m = "bar";
            var a = Expression.Constant(1);
            var d = DynamicCSharpExpression.DynamicArgument(a);
            var t = new Type[0];

            var es = new[]
            {
                DynamicCSharpExpression.DynamicInvokeMember(typeof(string), m, new[] { a }),
                DynamicCSharpExpression.DynamicInvokeMember(typeof(string), m, new[] { a }.AsEnumerable()),
                DynamicCSharpExpression.DynamicInvokeMember(typeof(string), m, new[] { d }),
                DynamicCSharpExpression.DynamicInvokeMember(typeof(string), m, new[] { d }.AsEnumerable()),
                DynamicCSharpExpression.DynamicInvokeMember(typeof(string), m, t, new[] { a }),
                DynamicCSharpExpression.DynamicInvokeMember(typeof(string), m, t, new[] { a }.AsEnumerable()),
                DynamicCSharpExpression.DynamicInvokeMember(typeof(string), m, t, new[] { d }),
                DynamicCSharpExpression.DynamicInvokeMember(typeof(string), m, t, new[] { d }.AsEnumerable()),
                DynamicCSharpExpression.DynamicInvokeMember(typeof(string), m, t, new[] { d }.AsEnumerable(), CSharpBinderFlags.None),
                DynamicCSharpExpression.DynamicInvokeMember(typeof(string), m, t, new[] { d }.AsEnumerable(), CSharpBinderFlags.None, null),
            };

            foreach (var e in es)
            {
                Assert.AreEqual(CSharpExpressionType.DynamicInvokeMember, e.CSharpNodeType);

                Assert.AreSame(typeof(string), e.Target);
                Assert.IsNull(e.Object);

                Assert.AreEqual(m, e.Name);
                Assert.AreEqual(0, e.TypeArguments.Count);

                Assert.AreEqual(1, e.Arguments.Count);
                Assert.AreSame(a, e.Arguments[0].Expression);

                Assert.IsNull(e.Context);
                Assert.AreEqual(CSharpBinderFlags.None, e.Flags);
            }
        }

        [TestMethod]
        public void Dynamic_InvokeMember_Struct1()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvokeMember(p, "X", new Type[0], new DynamicCSharpArgument[0], CSharpBinderFlags.ResultDiscarded);

            var e = Expression.Lambda<Func<object, int>>(Expression.Block(typeof(int), d, Expression.Field(Expression.Convert(p, typeof(Bar)), "Value")), p);
            var f = e.Compile();
            var b = new Bar();
            Assert.AreEqual(Dynamic_InvokeMember_Struct1_Compiled(b), f(b));
        }

        private static int Dynamic_InvokeMember_Struct1_Compiled(dynamic p)
        {
            p.X();
            return p.Value;
        }

        [TestMethod]
        public void Dynamic_InvokeMember_Struct2()
        {
            var p = Expression.Parameter(typeof(Bar));

            var d = DynamicCSharpExpression.DynamicInvokeMember(p, "X", new Type[0], new[] { DynamicCSharpExpression.DynamicArgument(Expression.Constant(0)) }, CSharpBinderFlags.ResultDiscarded);

            var e = Expression.Lambda<Func<Bar, int>>(Expression.Block(typeof(int), d, Expression.Field(p, "Value")), p);
            var f = e.Compile();
            var b = new Bar();
            Assert.AreEqual(Dynamic_InvokeMember_Struct2_Compiled(b, 0), f(b));
        }

        private static int Dynamic_InvokeMember_Struct2_Compiled(Bar p, dynamic d)
        {
            p.X(d);
            return p.Value;
        }

        [TestMethod]
        public void Dynamic_InvokeMember_ByRef()
        {
            var p = Expression.Parameter(typeof(int));

            var d = DynamicCSharpExpression.DynamicInvokeMember(typeof(Interlocked), "Exchange", new Type[0], new[] { DynamicCSharpExpression.DynamicArgument(p, null, CSharpArgumentInfoFlags.IsRef), DynamicCSharpExpression.DynamicArgument(Expression.Constant(42)) });

            var e = Expression.Lambda<Func<int, int>>(Expression.Block(typeof(int), d, p), p);
            var f = e.Compile();
            Assert.AreEqual(42, f(0));
        }

        [TestMethod]
        public void Dynamic_InvokeMember_Out()
        {
            var p = Expression.Parameter(typeof(string));
            var q = Expression.Parameter(typeof(int));

            var d = DynamicCSharpExpression.DynamicInvokeMember(typeof(int), "TryParse", new Type[0], new[] { DynamicCSharpExpression.DynamicArgument(p), DynamicCSharpExpression.DynamicArgument(q, null, CSharpArgumentInfoFlags.IsOut) });

            var e = Expression.Lambda<Func<string, int>>(Expression.Block(typeof(int), new[] { q }, d, q), p);
            var f = e.Compile();
            Assert.AreEqual(42, f("42"));
        }

        [TestMethod]
        public void Dynamic_Invoke()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvoke(p, Expression.Constant(1));
            Assert.AreEqual(CSharpExpressionType.DynamicInvoke, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(42, f(new Func<int, int>(x => x + 41)));
        }

        [TestMethod]
        public void Dynamic_Invoke_Factories()
        {
            var p = Expression.Parameter(typeof(object));
            var a = Expression.Constant(1);
            var d = DynamicCSharpExpression.DynamicArgument(a);

            var es = new[]
            {
                DynamicCSharpExpression.DynamicInvoke(p, new[] { a }),
                DynamicCSharpExpression.DynamicInvoke(p, new[] { a }.AsEnumerable()),
                DynamicCSharpExpression.DynamicInvoke(p, new[] { d }),
                DynamicCSharpExpression.DynamicInvoke(p, new[] { d }.AsEnumerable()),
                DynamicCSharpExpression.DynamicInvoke(p, new[] { d }.AsEnumerable(), CSharpBinderFlags.None),
                DynamicCSharpExpression.DynamicInvoke(p, new[] { d }.AsEnumerable(), CSharpBinderFlags.None, null),
            };

            foreach (var e in es)
            {
                Assert.AreEqual(CSharpExpressionType.DynamicInvoke, e.CSharpNodeType);

                Assert.AreSame(p, e.Expression);

                Assert.AreEqual(1, e.Arguments.Count);
                Assert.AreSame(a, e.Arguments[0].Expression);

                Assert.IsNull(e.Context);
                Assert.AreEqual(CSharpBinderFlags.None, e.Flags);
            }
        }

        [TestMethod]
        public void Dynamic_Unary()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Negate, p);
            Assert.AreEqual(CSharpExpressionType.DynamicUnary, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(42, f(-42));
            Assert.AreEqual(TimeSpan.FromSeconds(42), f(TimeSpan.FromSeconds(-42)));
        }

        [TestMethod]
        public void Dynamic_Unary_Compile()
        {
            // TODO: assignment nodes

            var vals = new[] { 1, 2, 3, 4, 5 }; // TODO: include exceptional cases

            foreach (var x in vals)
            {
                AssertUnary(ExpressionType.OnesComplement, x, ~x);
                AssertUnary(ExpressionType.UnaryPlus, x, +x);
                AssertUnary(ExpressionType.Negate, x, -x);
                AssertUnary(ExpressionType.NegateChecked, x, checked(-x));
                AssertUnary(ExpressionType.Increment, x, x + 1);
                AssertUnary(ExpressionType.Decrement, x, x - 1);
            }

            var bools = new[] { false, true };

            foreach (var b in bools)
            {
                AssertUnary(ExpressionType.Not, b, !b);
                AssertUnary<bool>(ExpressionType.IsTrue, b, b == true);
                AssertUnary<bool>(ExpressionType.IsFalse, b, b == false);
            }
        }

        private void AssertUnary(ExpressionType nodeType, object o, object expected)
        {
            AssertUnary<object>(nodeType, o, expected);
        }

        private void AssertUnary<R>(ExpressionType nodeType, object o, object expected)
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.MakeDynamicUnary(nodeType, p);

            var e = Expression.Lambda<Func<object, R>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(expected, f(o));
        }

        [TestMethod]
        public void Dynamic_Unary_Factory_ArgumentChecking()
        {
            AssertEx.Throws<NotSupportedException>(() => DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Add, Expression.Constant(0)));
        }

        [TestMethod]
        public void Dynamic_Unary_Factories()
        {
            var p = Expression.Parameter(typeof(object));
            var d = DynamicCSharpExpression.DynamicArgument(p);

            var es = new[]
            {
                DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Negate, p),
                DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Negate, d),
                DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Negate, d, CSharpBinderFlags.None),
                DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Negate, d, CSharpBinderFlags.None, null),
            };

            foreach (var e in es)
            {
                Assert.AreEqual(CSharpExpressionType.DynamicUnary, e.CSharpNodeType);

                Assert.AreSame(p, e.Operand.Expression);

                Assert.AreEqual(ExpressionType.Negate, e.OperationNodeType);

                Assert.IsNull(e.Context);
                Assert.AreEqual(CSharpBinderFlags.None, e.Flags);
            }
        }

        [TestMethod]
        public void Dynamic_Binary()
        {
            var p = Expression.Parameter(typeof(object));
            var q = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Add, p, q);
            Assert.AreEqual(CSharpExpressionType.DynamicBinary, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object, object>>(d, p, q);
            var f = e.Compile();
            Assert.AreEqual(3, f(1, 2));
            Assert.AreEqual("ab", f("a", "b"));
            Assert.AreEqual(new DateTime(1983, 2, 11), f(new DateTime(1983, 2, 10), TimeSpan.FromDays(1)));
        }

        [TestMethod]
        public void Dynamic_Binary_Compile()
        {
            // TODO: assignment nodes

            var vals = new[] { 1, 2, 3, 4, 5 }; // TODO: include exceptional cases
            
            foreach (var x in vals)
            {
                foreach (var y in vals)
                {
                    AssertBinary(ExpressionType.Add, x, y, x + y);
                    AssertBinary(ExpressionType.AddChecked, x, y, checked(x + y));
                    AssertBinary(ExpressionType.Subtract, x, y, x - y);
                    AssertBinary(ExpressionType.SubtractChecked, x, y, checked(x - y));
                    AssertBinary(ExpressionType.Multiply, x, y, x * y);
                    AssertBinary(ExpressionType.MultiplyChecked, x, y, checked(x * y));
                    AssertBinary(ExpressionType.Divide, x, y, x / y);
                    AssertBinary(ExpressionType.Modulo, x, y, x % y);
                    AssertBinary(ExpressionType.LeftShift, x, y, x << y);
                    AssertBinary(ExpressionType.RightShift, x, y, x >> y);
                    AssertBinary(ExpressionType.And, x, y, x & y);
                    AssertBinary(ExpressionType.Or, x, y, x | y);
                    AssertBinary(ExpressionType.ExclusiveOr, x, y, x ^ y);
                    AssertBinary(ExpressionType.LessThan, x, y, x < y);
                    AssertBinary(ExpressionType.LessThanOrEqual, x, y, x <= y);
                    AssertBinary(ExpressionType.GreaterThan, x, y, x > y);
                    AssertBinary(ExpressionType.GreaterThanOrEqual, x, y, x >= y);
                    AssertBinary(ExpressionType.Equal, x, y, x == y);
                    AssertBinary(ExpressionType.NotEqual, x, y, x != y);
                }
            }

            var bools = new[] { false, true };

            foreach (var l in bools)
            {
                foreach (var r in bools)
                {
                    AssertBinary(ExpressionType.And, l, r, l & r);
                    AssertBinary(ExpressionType.Or, l, r, l | r);
                    AssertBinary(ExpressionType.AndAlso, l, r, l && r);
                    AssertBinary(ExpressionType.OrElse, l, r, l || r);
                }
            }
        }

        private void AssertBinary(ExpressionType nodeType, object l, object r, object expected)
        {
            var p = Expression.Parameter(typeof(object));
            var q = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.MakeDynamicBinary(nodeType, p, q);

            var e = Expression.Lambda<Func<object, object, object>>(d, p, q);
            var f = e.Compile();
            Assert.AreEqual(expected, f(l, r));
        }

        [TestMethod]
        public void Dynamic_Binary_Factory_ArgumentChecking()
        {
            AssertEx.Throws<NotSupportedException>(() => DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Negate, Expression.Constant(0), Expression.Constant(0)));
        }

        [TestMethod]
        public void Dynamic_Binary_Factories()
        {
            var p = Expression.Parameter(typeof(object));
            var q = Expression.Parameter(typeof(object));
            var l = DynamicCSharpExpression.DynamicArgument(p);
            var r = DynamicCSharpExpression.DynamicArgument(q);

            var es = new[]
            {
                DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Add, p, q),
                DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Add, l, r),
                DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Add, l, r, CSharpBinderFlags.None),
                DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Add, l, r, CSharpBinderFlags.None, null),
            };

            foreach (var e in es)
            {
                Assert.AreEqual(CSharpExpressionType.DynamicBinary, e.CSharpNodeType);

                Assert.AreSame(p, e.Left.Expression);
                Assert.AreSame(q, e.Right.Expression);

                Assert.AreEqual(ExpressionType.Add, e.OperationNodeType);

                Assert.IsNull(e.Context);
                Assert.AreEqual(CSharpBinderFlags.None, e.Flags);
            }
        }

        [TestMethod]
        public void Dynamic_InvokeConstructor()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvokeConstructor(typeof(TimeSpan), p);
            Assert.AreEqual(CSharpExpressionType.DynamicInvokeConstructor, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, TimeSpan>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(TimeSpan.FromSeconds(42), f(TimeSpan.FromSeconds(42).Ticks));
        }

        [TestMethod]
        public void Dynamic_InvokeConstructor_Factories()
        {
            var a = Expression.Constant(1);
            var d = DynamicCSharpExpression.DynamicArgument(a);
            var t = typeof(TimeSpan);

            var es = new[]
            {
                DynamicCSharpExpression.DynamicInvokeConstructor(t, new[] { a }),
                DynamicCSharpExpression.DynamicInvokeConstructor(t, new[] { a }.AsEnumerable()),
                DynamicCSharpExpression.DynamicInvokeConstructor(t, new[] { d }),
                DynamicCSharpExpression.DynamicInvokeConstructor(t, new[] { d }.AsEnumerable()),
                DynamicCSharpExpression.DynamicInvokeConstructor(t, new[] { d }.AsEnumerable(), CSharpBinderFlags.None),
                DynamicCSharpExpression.DynamicInvokeConstructor(t, new[] { d }.AsEnumerable(), CSharpBinderFlags.None, null),
            };

            foreach (var e in es)
            {
                Assert.AreEqual(CSharpExpressionType.DynamicInvokeConstructor, e.CSharpNodeType);

                Assert.AreSame(t, e.Type);

                Assert.AreEqual(1, e.Arguments.Count);
                Assert.AreSame(a, e.Arguments[0].Expression);

                Assert.IsNull(e.Context);
                Assert.AreEqual(CSharpBinderFlags.None, e.Flags);
            }
        }

        [TestMethod]
        public void Dynamic_GetMember()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicGetMember(p, "TotalSeconds");
            Assert.AreEqual(CSharpExpressionType.DynamicGetMember, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(42.0, f(TimeSpan.FromSeconds(42)));
        }

        [TestMethod]
        public void Dynamic_GetMember_Factories()
        {
            var p = Expression.Parameter(typeof(object));
            var m = "bar";
            var a = Expression.Constant(1);
            var d = DynamicCSharpExpression.DynamicArgument(a);

            var es = new[]
            {
                DynamicCSharpExpression.DynamicGetMember(p, m, new[] { a }),
                DynamicCSharpExpression.DynamicGetMember(p, m, new[] { a }.AsEnumerable()),
                DynamicCSharpExpression.DynamicGetMember(p, m, new[] { d }),
                DynamicCSharpExpression.DynamicGetMember(p, m, new[] { d }.AsEnumerable()),
                DynamicCSharpExpression.DynamicGetMember(p, m, new[] { d }.AsEnumerable(), CSharpBinderFlags.None),
                DynamicCSharpExpression.DynamicGetMember(p, m, new[] { d }.AsEnumerable(), CSharpBinderFlags.None, null),
            };

            foreach (var e in es)
            {
                Assert.AreEqual(CSharpExpressionType.DynamicGetMember, e.CSharpNodeType);

                Assert.AreSame(p, e.Object);

                Assert.AreSame(m, e.Name);

                Assert.AreEqual(1, e.Arguments.Count);
                Assert.AreSame(a, e.Arguments[0].Expression);

                Assert.IsNull(e.Context);
                Assert.AreEqual(CSharpBinderFlags.None, e.Flags);
            }
        }

        [TestMethod]
        public void Dynamic_GetIndex()
        {
            var p = Expression.Parameter(typeof(object));
            var q = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicGetIndex(p, q);
            Assert.AreEqual(CSharpExpressionType.DynamicGetIndex, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object, object>>(d, p, q);
            var f = e.Compile();
            Assert.AreEqual(5, f(new[] { 2, 3, 5 }, 2));
            Assert.AreEqual(21, f(new Dictionary<string, int> { { "Bart", 21 } }, "Bart"));
        }

        [TestMethod]
        public void Dynamic_GetIndex_Factories()
        {
            var p = Expression.Parameter(typeof(object));
            var a = Expression.Constant(1);
            var d = DynamicCSharpExpression.DynamicArgument(a);

            var es = new[]
            {
                DynamicCSharpExpression.DynamicGetIndex(p, new[] { a }),
                DynamicCSharpExpression.DynamicGetIndex(p, new[] { a }.AsEnumerable()),
                DynamicCSharpExpression.DynamicGetIndex(p, new[] { d }),
                DynamicCSharpExpression.DynamicGetIndex(p, new[] { d }.AsEnumerable()),
                DynamicCSharpExpression.DynamicGetIndex(p, new[] { d }.AsEnumerable(), CSharpBinderFlags.None),
                DynamicCSharpExpression.DynamicGetIndex(p, new[] { d }.AsEnumerable(), CSharpBinderFlags.None, null),
            };

            foreach (var e in es)
            {
                Assert.AreEqual(CSharpExpressionType.DynamicGetIndex, e.CSharpNodeType);

                Assert.AreSame(p, e.Object);

                Assert.AreEqual(1, e.Arguments.Count);
                Assert.AreSame(a, e.Arguments[0].Expression);

                Assert.IsNull(e.Context);
                Assert.AreEqual(CSharpBinderFlags.None, e.Flags);
            }
        }

        [TestMethod]
        public void Dynamic_Convert()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicConvert(p, typeof(DateTimeOffset));
            Assert.AreEqual(CSharpExpressionType.DynamicConvert, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, DateTimeOffset>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(new DateTime(1983, 2, 11), f(new DateTime(1983, 2, 11)));
            Assert.AreEqual(new DateTime(1983, 2, 11), f((DateTimeOffset)new DateTime(1983, 2, 11)));
        }

        [TestMethod]
        public void Dynamic_Convert_Factories()
        {
            var p = Expression.Parameter(typeof(object));
            var t = typeof(int);

            var es = new[]
            {
                DynamicCSharpExpression.DynamicConvert(p, t),
                DynamicCSharpExpression.DynamicConvert(p, t, CSharpBinderFlags.None),
                DynamicCSharpExpression.DynamicConvert(p, t, CSharpBinderFlags.None, null),
            };

            foreach (var e in es)
            {
                Assert.AreEqual(CSharpExpressionType.DynamicConvert, e.CSharpNodeType);

                Assert.AreSame(p, e.Expression);

                Assert.AreSame(t, e.Type);

                Assert.IsNull(e.Context);
                Assert.AreEqual(CSharpBinderFlags.None, e.Flags);
            }
        }

        [TestMethod]
        public void Dynamic_Argument_Factories()
        {
            var c = Expression.Constant(1);
            var n = "x";
            var f = CSharpArgumentInfoFlags.IsRef;

            {
                var es = new[]
                {
                    DynamicCSharpExpression.DynamicArgument(c),
                    DynamicCSharpExpression.DynamicArgument(c, null),
                    DynamicCSharpExpression.DynamicArgument(c, null, CSharpArgumentInfoFlags.None),
                };

                foreach (var e in es)
                {
                    Assert.AreSame(c, e.Expression);
                    Assert.IsNull(e.Name);
                    Assert.AreEqual(CSharpArgumentInfoFlags.None, e.Flags);
                }
            }

            {
                var es = new[]
                {
                    DynamicCSharpExpression.DynamicArgument(c, n),
                    DynamicCSharpExpression.DynamicArgument(c, n, CSharpArgumentInfoFlags.None),
                };

                foreach (var e in es)
                {
                    Assert.AreSame(c, e.Expression);
                    Assert.AreSame(n, e.Name);
                    Assert.AreEqual(CSharpArgumentInfoFlags.None, e.Flags);
                }
            }

            {
                var es = new[]
                {
                    DynamicCSharpExpression.DynamicArgument(c, n, f),
                };

                foreach (var e in es)
                {
                    Assert.AreSame(c, e.Expression);
                    Assert.AreSame(n, e.Name);
                    Assert.AreEqual(f, e.Flags);
                }
            }
        }

        static void AssertNoChange(CSharpExpression e)
        {
            var r = new Nop().Visit(e);
            Assert.AreSame(e, r);
        }

        static void AssertChange(CSharpExpression e)
        {
            var r = new Change().Visit(e);
            Assert.AreNotSame(e, r);
            Assert.AreEqual(r.ToString(), e.ToString()); // TODO: use a DebugView when we add it
        }

        class Nop : CSharpExpressionVisitor
        {
        }

        class Change : CSharpExpressionVisitor
        {
            protected override Expression VisitConstant(ConstantExpression node)
            {
                return Expression.Constant(node.Value, node.Type);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return Expression.Parameter(node.Type, node.Name);
            }
        }
    }

    public class Foo
    {
        public string Bar<T>()
        {
            return typeof(T).Name;
        }

        public static string Qux<T>()
        {
            return typeof(T).Name;
        }
    }

    public struct Bar
    {
        public int Value;

        public void X()
        {
            Value = 42;
        }

        public void X(int x)
        {
            Value = 42;
        }

        public int this[int x]
        {
            get
            {
                Value = 42;
                return 0;
            }
        }

        public int Y
        {
            get
            {
                Value = 42;
                return 0;
            }
        }
    }
}
