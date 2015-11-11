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

namespace Tests
{
    [TestClass]
    public class DynamicTests
    {
        [TestMethod]
        public void Dynamic_InvokeMember()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvokeMember(p, "Substring", Expression.Constant(1));
            Assert.AreEqual(CSharpExpressionType.DynamicInvokeMember, d.CSharpNodeType);

            AssertNoChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.AreEqual("ar", f("bar"));
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

                Assert.AreEqual(m, e.Name);
                Assert.AreEqual(0, e.TypeArguments.Count);

                Assert.AreEqual(1, e.Arguments.Count);
                Assert.AreSame(a, e.Arguments[0].Expression);

                Assert.IsNull(e.Context);
                Assert.AreEqual(CSharpBinderFlags.None, e.Flags);
            }
        }

        [TestMethod]
        public void Dynamic_Invoke()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvoke(p, Expression.Constant(1));
            Assert.AreEqual(CSharpExpressionType.DynamicInvoke, d.CSharpNodeType);

            AssertNoChange(d);

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

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(42, f(-42));
            Assert.AreEqual(TimeSpan.FromSeconds(42), f(TimeSpan.FromSeconds(-42)));
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

            var e = Expression.Lambda<Func<object, object, object>>(d, p, q);
            var f = e.Compile();
            Assert.AreEqual(3, f(1, 2));
            Assert.AreEqual("ab", f("a", "b"));
            Assert.AreEqual(new DateTime(1983, 2, 11), f(new DateTime(1983, 2, 10), TimeSpan.FromDays(1)));
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

            var e = Expression.Lambda<Func<object, DateTimeOffset>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(new DateTime(1983, 2, 11), f(new DateTime(1983, 2, 11)));
            Assert.AreEqual(new DateTime(1983, 2, 11), f((DateTimeOffset)new DateTime(1983, 2, 11)));
        }

        static void AssertNoChange(CSharpExpression e)
        {
            var r = new Nop().Visit(e);
            Assert.AreSame(e, r);
        }

        class Nop : CSharpExpressionVisitor
        {
        }
    }
}
