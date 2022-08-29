﻿// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using Xunit;

namespace Tests
{
    public partial class DynamicTests
    {
        [Fact]
        public void Dynamic_InvokeMember()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvokeMember(p, "Substring", Expression.Constant(1));
            Assert.Equal(CSharpExpressionType.DynamicInvokeMember, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.Equal("ar", f("bar"));
        }

        [Fact]
        public void Dynamic_InvokeMember_Generic()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvokeMember(p, "Bar", new[] { typeof(int) });
            Assert.Equal(CSharpExpressionType.DynamicInvokeMember, d.CSharpNodeType);

            AssertNoChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.Equal("Int32", f(new Foo()));
        }

        [Fact]
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
                Assert.Equal(CSharpExpressionType.DynamicInvokeMember, e.CSharpNodeType);

                Assert.Same(p, e.Object);
                Assert.Null(e.Target);

                Assert.Equal(m, e.Name);
                Assert.Empty(e.TypeArguments);

                Assert.Single(e.Arguments);
                Assert.Same(a, e.Arguments[0].Expression);

                Assert.Null(e.Context);
                Assert.Equal(CSharpBinderFlags.None, e.Flags);
            }
        }

        [Fact]
        public void Dynamic_InvokeMember_Static()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvokeMember(typeof(string), "Concat", p, Expression.Constant("!"));
            Assert.Equal(CSharpExpressionType.DynamicInvokeMember, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.Equal("bar!", f("bar"));
        }

        [Fact]
        public void Dynamic_InvokeMember_Static_Generic()
        {
            var d = DynamicCSharpExpression.DynamicInvokeMember(typeof(Foo), "Qux", new[] { typeof(int) });
            Assert.Equal(CSharpExpressionType.DynamicInvokeMember, d.CSharpNodeType);

            AssertNoChange(d);

            var e = Expression.Lambda<Func<object>>(d);
            var f = e.Compile();
            Assert.Equal("Int32", f());
        }

        [Fact]
        public void Dynamic_InvokeMember_Static_Factories()
        {
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
                Assert.Equal(CSharpExpressionType.DynamicInvokeMember, e.CSharpNodeType);

                Assert.Same(typeof(string), e.Target);
                Assert.Null(e.Object);

                Assert.Equal(m, e.Name);
                Assert.Empty(e.TypeArguments);

                Assert.Single(e.Arguments);
                Assert.Same(a, e.Arguments[0].Expression);

                Assert.Null(e.Context);
                Assert.Equal(CSharpBinderFlags.None, e.Flags);
            }
        }

        [Fact]
        public void Dynamic_InvokeMember_Struct1()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvokeMember(p, "X", new Type[0], new DynamicCSharpArgument[0], CSharpBinderFlags.ResultDiscarded);

            var e = Expression.Lambda<Func<object, int>>(Expression.Block(typeof(int), d, Expression.Field(Expression.Convert(p, typeof(Bar)), "Value")), p);
            var f = e.Compile();
            var b = new Bar();
            Assert.Equal(Dynamic_InvokeMember_Struct1_Compiled(b), f(b));
        }

        private static int Dynamic_InvokeMember_Struct1_Compiled(dynamic p)
        {
            p.X();
            return p.Value;
        }

        [Fact]
        public void Dynamic_InvokeMember_Struct2()
        {
            var p = Expression.Parameter(typeof(Bar));

            var d = DynamicCSharpExpression.DynamicInvokeMember(p, "X", new Type[0], new[] { DynamicCSharpExpression.DynamicArgument(Expression.Constant(0)) }, CSharpBinderFlags.ResultDiscarded);

            var e = Expression.Lambda<Func<Bar, int>>(Expression.Block(typeof(int), d, Expression.Field(p, "Value")), p);
            var f = e.Compile();
            var b = new Bar();
            Assert.Equal(Dynamic_InvokeMember_Struct2_Compiled(b, 0), f(b));
        }

        private static int Dynamic_InvokeMember_Struct2_Compiled(Bar p, dynamic d)
        {
            p.X(d);
            return p.Value;
        }

        [Fact]
        public void Dynamic_InvokeMember_ByRef()
        {
            var p = Expression.Parameter(typeof(int));

            var d = DynamicCSharpExpression.DynamicInvokeMember(typeof(Interlocked), "Exchange", new Type[0], new[] { DynamicCSharpExpression.DynamicArgument(p, null, CSharpArgumentInfoFlags.IsRef), DynamicCSharpExpression.DynamicArgument(Expression.Constant(42)) });

            var e = Expression.Lambda<Func<int, int>>(Expression.Block(typeof(int), d, p), p);
            var f = e.Compile();
            Assert.Equal(42, f(0));
        }

        [Fact]
        public void Dynamic_InvokeMember_Out()
        {
            var p = Expression.Parameter(typeof(string));
            var q = Expression.Parameter(typeof(int));

            var d = DynamicCSharpExpression.DynamicInvokeMember(typeof(int), "TryParse", new Type[0], new[] { DynamicCSharpExpression.DynamicArgument(p), DynamicCSharpExpression.DynamicArgument(q, null, CSharpArgumentInfoFlags.IsOut) });

            var e = Expression.Lambda<Func<string, int>>(Expression.Block(typeof(int), new[] { q }, d, q), p);
            var f = e.Compile();
            Assert.Equal(42, f("42"));
        }

        [Fact]
        public void Dynamic_Invoke()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvoke(p, Expression.Constant(1));
            Assert.Equal(CSharpExpressionType.DynamicInvoke, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.Equal(42, f(new Func<int, int>(x => x + 41)));
        }

        [Fact]
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
                Assert.Equal(CSharpExpressionType.DynamicInvoke, e.CSharpNodeType);

                Assert.Same(p, e.Expression);

                Assert.Single(e.Arguments);
                Assert.Same(a, e.Arguments[0].Expression);

                Assert.Null(e.Context);
                Assert.Equal(CSharpBinderFlags.None, e.Flags);
            }
        }

        [Fact]
        public void Dynamic_Unary()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Negate, p);
            Assert.Equal(CSharpExpressionType.DynamicUnary, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.Equal(42, f(-42));
            Assert.Equal(TimeSpan.FromSeconds(42), f(TimeSpan.FromSeconds(-42)));
        }

        [Fact]
        public void Dynamic_Unary_Compile()
        {
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
            Assert.Equal(expected, f(o));
        }

        [Fact]
        public void Dynamic_Unary_Factory_ArgumentChecking()
        {
            AssertEx.Throws<NotSupportedException>(() => DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Add, Expression.Constant(0)));
        }

        [Fact]
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
                Assert.Equal(CSharpExpressionType.DynamicUnary, e.CSharpNodeType);

                Assert.Same(p, e.Operand.Expression);

                Assert.Equal(ExpressionType.Negate, e.OperationNodeType);

                Assert.Null(e.Context);
                Assert.Equal(CSharpBinderFlags.None, e.Flags);
            }
        }

        [Fact]
        public void Dynamic_Binary()
        {
            var p = Expression.Parameter(typeof(object));
            var q = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Add, p, q);
            Assert.Equal(CSharpExpressionType.DynamicBinary, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object, object>>(d, p, q);
            var f = e.Compile();
            Assert.Equal(3, f(1, 2));
            Assert.Equal("ab", f("a", "b"));
            Assert.Equal(new DateTime(1983, 2, 11), f(new DateTime(1983, 2, 10), TimeSpan.FromDays(1)));
        }

        [Fact]
        public void Dynamic_Binary_Compile()
        {
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
            Assert.Equal(expected, f(l, r));
        }

        [Fact]
        public void Dynamic_Binary_Factory_ArgumentChecking()
        {
            AssertEx.Throws<NotSupportedException>(() => DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Negate, Expression.Constant(0), Expression.Constant(0)));
        }

        [Fact]
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
                Assert.Equal(CSharpExpressionType.DynamicBinary, e.CSharpNodeType);

                Assert.Same(p, e.Left.Expression);
                Assert.Same(q, e.Right.Expression);

                Assert.Equal(ExpressionType.Add, e.OperationNodeType);

                Assert.Null(e.Context);
                Assert.Equal(CSharpBinderFlags.None, e.Flags);
            }
        }

        [Fact]
        public void Dynamic_InvokeConstructor()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicInvokeConstructor(typeof(TimeSpan), p);
            Assert.Equal(CSharpExpressionType.DynamicInvokeConstructor, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, TimeSpan>>(d, p);
            var f = e.Compile();
            Assert.Equal(TimeSpan.FromSeconds(42), f(TimeSpan.FromSeconds(42).Ticks));
        }

        [Fact]
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
                Assert.Equal(CSharpExpressionType.DynamicInvokeConstructor, e.CSharpNodeType);

                Assert.Same(t, e.Type);

                Assert.Single(e.Arguments);
                Assert.Same(a, e.Arguments[0].Expression);

                Assert.Null(e.Context);
                Assert.Equal(CSharpBinderFlags.None, e.Flags);
            }
        }

        [Fact]
        public void Dynamic_GetMember()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicGetMember(p, "TotalSeconds");
            Assert.Equal(CSharpExpressionType.DynamicGetMember, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.Equal(42.0, f(TimeSpan.FromSeconds(42)));
        }

        [Fact]
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
                Assert.Equal(CSharpExpressionType.DynamicGetMember, e.CSharpNodeType);

                Assert.Same(p, e.Object);

                Assert.Same(m, e.Name);

                Assert.Single(e.Arguments);
                Assert.Same(a, e.Arguments[0].Expression);

                Assert.Null(e.Context);
                Assert.Equal(CSharpBinderFlags.None, e.Flags);
            }
        }

        [Fact]
        public void Dynamic_GetIndex()
        {
            var p = Expression.Parameter(typeof(object));
            var q = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicGetIndex(p, q);
            Assert.Equal(CSharpExpressionType.DynamicGetIndex, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, object, object>>(d, p, q);
            var f = e.Compile();
            Assert.Equal(5, f(new[] { 2, 3, 5 }, 2));
            Assert.Equal(21, f(new Dictionary<string, int> { { "Bart", 21 } }, "Bart"));
        }

        [Fact]
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
                Assert.Equal(CSharpExpressionType.DynamicGetIndex, e.CSharpNodeType);

                Assert.Same(p, e.Object);

                Assert.Single(e.Arguments);
                Assert.Same(a, e.Arguments[0].Expression);

                Assert.Null(e.Context);
                Assert.Equal(CSharpBinderFlags.None, e.Flags);
            }
        }

        [Fact]
        public void Dynamic_Convert()
        {
            var p = Expression.Parameter(typeof(object));

            var d = DynamicCSharpExpression.DynamicConvert(p, typeof(DateTimeOffset));
            Assert.Equal(CSharpExpressionType.DynamicConvert, d.CSharpNodeType);

            AssertNoChange(d);
            AssertChange(d);

            var e = Expression.Lambda<Func<object, DateTimeOffset>>(d, p);
            var f = e.Compile();
            Assert.Equal(new DateTime(1983, 2, 11), f(new DateTime(1983, 2, 11)));
            Assert.Equal(new DateTime(1983, 2, 11), f((DateTimeOffset)new DateTime(1983, 2, 11)));
        }

        [Fact]
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
                Assert.Equal(CSharpExpressionType.DynamicConvert, e.CSharpNodeType);

                Assert.Same(p, e.Expression);

                Assert.Same(t, e.Type);

                Assert.Null(e.Context);
                Assert.Equal(CSharpBinderFlags.None, e.Flags);
            }
        }

        [Fact]
        public void Dynamic_Argument_Factories()
        {
            var c = Expression.Constant(1);
            var n = "x";
            var f = CSharpArgumentInfoFlags.IsRef;

            // REVIEW: Is this desired behavior?
            var expectedFlags = CSharpArgumentInfoFlags.Constant | CSharpArgumentInfoFlags.UseCompileTimeType;

            {
                var es = new[]
                {
                    DynamicCSharpExpression.DynamicArgument(c),
                    DynamicCSharpExpression.DynamicArgument(c, null),
                };

                foreach (var e in es)
                {
                    Assert.Same(c, e.Expression);
                    Assert.Null(e.Name);
                    Assert.Equal(expectedFlags, e.Flags);
                }
            }

            {
                var es = new[]
                {
                    DynamicCSharpExpression.DynamicArgument(c, n),
                };

                foreach (var e in es)
                {
                    Assert.Same(c, e.Expression);
                    Assert.Same(n, e.Name);
                    Assert.Equal(expectedFlags, e.Flags);
                }
            }

            {
                var es = new[]
                {
                    DynamicCSharpExpression.DynamicArgument(c, n, f),
                };

                foreach (var e in es)
                {
                    Assert.Same(c, e.Expression);
                    Assert.Same(n, e.Name);
                    Assert.Equal(f, e.Flags);
                }
            }
        }

        [Fact]
        public void Dynamic_UnaryAssign_Visitors()
        {
            var p = Expression.Parameter(typeof(object));

            var es = new[]
            {
                DynamicCSharpExpression.DynamicPostDecrementAssign(p),
                DynamicCSharpExpression.DynamicPostDecrementAssignChecked(p),
                DynamicCSharpExpression.DynamicPostIncrementAssign(p),
                DynamicCSharpExpression.DynamicPostIncrementAssignChecked(p),
                DynamicCSharpExpression.DynamicPreDecrementAssign(p),
                DynamicCSharpExpression.DynamicPreDecrementAssignChecked(p),
                DynamicCSharpExpression.DynamicPreIncrementAssign(p),
                DynamicCSharpExpression.DynamicPreIncrementAssignChecked(p),
            };

            foreach (var e in es)
            {
                Assert.Equal(CSharpExpressionType.DynamicUnaryAssign, e.CSharpNodeType);

                Assert.Same(p, e.Operand.Expression);

                AssertNoChange(e);
                AssertChange(e);
            }
        }

        [Fact]
        public void Dynamic_UnaryAssign_Compile_PostIncrement_Variable()
        {
            var p = Expression.Parameter(typeof(object));

            var c = Expression.Convert(Expression.Constant(41), typeof(object));
            var a = Expression.Assign(p, c);

            var i = DynamicCSharpExpression.DynamicPostIncrementAssign(p);

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, i, Expression.Constant(","), p);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal("41,42", f());
        }

        [Fact]
        public void Dynamic_UnaryAssign_Compile_PreDecrementChecked_Variable()
        {
            // TODO: test for overflow behavior

            var p = Expression.Parameter(typeof(object));

            var c = Expression.Convert(Expression.Constant(43), typeof(object));
            var a = Expression.Assign(p, c);

            var i = DynamicCSharpExpression.DynamicPreDecrementAssignChecked(p);

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, i, Expression.Constant(","), p);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal("42,42", f());
        }

        [Fact]
        public void Dynamic_UnaryAssign_Compile_PostDecrement_DynamicMember()
        {
            var p = Expression.Parameter(typeof(object));

            var v = typeof(StrongBox<int>).GetField("Value");
            var m = Expression.Convert(Expression.Field(Expression.Convert(p, v.DeclaringType), v), typeof(object));
            var c = Expression.MemberInit(Expression.New(typeof(StrongBox<int>)), Expression.Bind(v, Expression.Constant(43)));
            var a = Expression.Assign(p, c);

            var z = DynamicCSharpExpression.DynamicGetMember(p, "Value");
            var i = DynamicCSharpExpression.DynamicPostDecrementAssign(z);

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, i, Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal("43,42", f());
        }

        [Fact]
        public void Dynamic_UnaryAssign_Compile_PreIncrement_DynamicMember()
        {
            var p = Expression.Parameter(typeof(object));

            var v = typeof(StrongBox<int>).GetField("Value");
            var m = Expression.Convert(Expression.Field(Expression.Convert(p, v.DeclaringType), v), typeof(object));
            var c = Expression.MemberInit(Expression.New(typeof(StrongBox<int>)), Expression.Bind(v, Expression.Constant(41)));
            var a = Expression.Assign(p, c);

            var z = DynamicCSharpExpression.DynamicGetMember(p, "Value");
            var i = DynamicCSharpExpression.DynamicPreIncrementAssign(z);

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, i, Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal("42,42", f());
        }

        [Fact]
        public void Dynamic_UnaryAssign_Compile_PostDecrement_DynamicIndex()
        {
            var p = Expression.Parameter(typeof(object));

            var v = typeof(List<int>).GetMethod("Add", new[] { typeof(int) });
            var m = Expression.Convert(Expression.MakeIndex(Expression.Convert(p, v.DeclaringType), typeof(List<int>).GetProperty("Item"), new[] { Expression.Constant(0) }), typeof(object));
            var c = Expression.ListInit(Expression.New(typeof(List<int>)), Expression.ElementInit(v, Expression.Constant(43)));
            var a = Expression.Assign(p, c);

            var z = DynamicCSharpExpression.DynamicGetIndex(p, Expression.Constant(0));
            var i = DynamicCSharpExpression.DynamicPostDecrementAssign(z);

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, i, Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal("43,42", f());
        }

        [Fact]
        public void Dynamic_UnaryAssign_Compile_PreIncrement_DynamicIndex()
        {
            var p = Expression.Parameter(typeof(object));

            var v = typeof(List<int>).GetMethod("Add", new[] { typeof(int) });
            var m = Expression.Convert(Expression.MakeIndex(Expression.Convert(p, v.DeclaringType), typeof(List<int>).GetProperty("Item"), new[] { Expression.Constant(0) }), typeof(object));
            var c = Expression.ListInit(Expression.New(typeof(List<int>)), Expression.ElementInit(v, Expression.Constant(41)));
            var a = Expression.Assign(p, c);

            var z = DynamicCSharpExpression.DynamicGetIndex(p, Expression.Constant(0));
            var i = DynamicCSharpExpression.DynamicPreIncrementAssign(z);

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, i, Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal("42,42", f());
        }

        [Fact]
        public void Dynamic_BinaryAssign_Visitors()
        {
            var p = Expression.Parameter(typeof(object));
            var x = Expression.Constant(1);

            var es = new[]
            {
                DynamicCSharpExpression.DynamicAssign(p, x),
                DynamicCSharpExpression.DynamicAddAssign(p, x),
                DynamicCSharpExpression.DynamicAddAssignChecked(p, x),
                DynamicCSharpExpression.DynamicSubtractAssign(p, x),
                DynamicCSharpExpression.DynamicSubtractAssignChecked(p, x),
                DynamicCSharpExpression.DynamicMultiplyAssign(p, x),
                DynamicCSharpExpression.DynamicMultiplyAssignChecked(p, x),
                DynamicCSharpExpression.DynamicDivideAssign(p, x),
                DynamicCSharpExpression.DynamicModuloAssign(p, x),
                DynamicCSharpExpression.DynamicAndAssign(p, x),
                DynamicCSharpExpression.DynamicOrAssign(p, x),
                DynamicCSharpExpression.DynamicExclusiveOrAssign(p, x),
                DynamicCSharpExpression.DynamicLeftShiftAssign(p, x),
                DynamicCSharpExpression.DynamicRightShiftAssign(p, x),
            };

            foreach (var e in es)
            {
                Assert.Equal(CSharpExpressionType.DynamicBinaryAssign, e.CSharpNodeType);

                Assert.Same(p, e.Left.Expression);
                Assert.Same(x, e.Right.Expression);

                AssertNoChange(e);
                AssertChange(e);
            }
        }

        [Fact]
        public void Dynamic_BinaryAssign_Compile_Variable()
        {
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicAddAssign, 41, 1, 42);
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicAddAssignChecked, 41, 1, 42);
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicSubtractAssign, 43, 1, 42);
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicSubtractAssignChecked, 43, 1, 42);
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicMultiplyAssign, 21, 2, 42);
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicMultiplyAssignChecked, 21, 2, 42);
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicDivideAssign, 84, 2, 42);
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicModuloAssign, 85, 43, 42);
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicAndAssign, 255, 42, 42);
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicOrAssign, 40, 2, 42);
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicExclusiveOrAssign, 41, 3, 42);
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicLeftShiftAssign, 21, 1, 42);
            Dynamic_BinaryAssign_Compile_Variable_Core(DynamicCSharpExpression.DynamicRightShiftAssign, 84, 1, 42);
        }

        [Fact]
        public void Dynamic_BinaryAssign_Compile_DynamicMember()
        {
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicAddAssign, 41, 1, 42);
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicAddAssignChecked, 41, 1, 42);
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicSubtractAssign, 43, 1, 42);
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicSubtractAssignChecked, 43, 1, 42);
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicMultiplyAssign, 21, 2, 42);
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicMultiplyAssignChecked, 21, 2, 42);
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicDivideAssign, 84, 2, 42);
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicModuloAssign, 85, 43, 42);
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicAndAssign, 255, 42, 42);
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicOrAssign, 40, 2, 42);
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicExclusiveOrAssign, 41, 3, 42);
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicLeftShiftAssign, 21, 1, 42);
            Dynamic_BinaryAssign_Compile_DynamicMember_Core(DynamicCSharpExpression.DynamicRightShiftAssign, 84, 1, 42);
        }

        [Fact]
        public void Dynamic_BinaryAssign_Compile_Member()
        {
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicAddAssign, 41, 1, 42);
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicAddAssignChecked, 41, 1, 42);
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicSubtractAssign, 43, 1, 42);
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicSubtractAssignChecked, 43, 1, 42);
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicMultiplyAssign, 21, 2, 42);
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicMultiplyAssignChecked, 21, 2, 42);
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicDivideAssign, 84, 2, 42);
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicModuloAssign, 85, 43, 42);
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicAndAssign, 255, 42, 42);
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicOrAssign, 40, 2, 42);
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicExclusiveOrAssign, 41, 3, 42);
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicLeftShiftAssign, 21, 1, 42);
            Dynamic_BinaryAssign_Compile_Member_Core(DynamicCSharpExpression.DynamicRightShiftAssign, 84, 1, 42);
        }

        [Fact]
        public void Dynamic_BinaryAssign_Compile_DynamicIndex()
        {
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicAddAssign, 41, 1, 42);
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicAddAssignChecked, 41, 1, 42);
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicSubtractAssign, 43, 1, 42);
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicSubtractAssignChecked, 43, 1, 42);
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicMultiplyAssign, 21, 2, 42);
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicMultiplyAssignChecked, 21, 2, 42);
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicDivideAssign, 84, 2, 42);
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicModuloAssign, 85, 43, 42);
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicAndAssign, 255, 42, 42);
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicOrAssign, 40, 2, 42);
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicExclusiveOrAssign, 41, 3, 42);
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicLeftShiftAssign, 21, 1, 42);
            Dynamic_BinaryAssign_Compile_DynamicIndex_Core(DynamicCSharpExpression.DynamicRightShiftAssign, 84, 1, 42);
        }

        [Fact]
        public void Dynamic_BinaryAssign_Compile_Index()
        {
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicAddAssign, 41, 1, 42);
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicAddAssignChecked, 41, 1, 42);
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicSubtractAssign, 43, 1, 42);
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicSubtractAssignChecked, 43, 1, 42);
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicMultiplyAssign, 21, 2, 42);
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicMultiplyAssignChecked, 21, 2, 42);
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicDivideAssign, 84, 2, 42);
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicModuloAssign, 85, 43, 42);
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicAndAssign, 255, 42, 42);
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicOrAssign, 40, 2, 42);
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicExclusiveOrAssign, 41, 3, 42);
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicLeftShiftAssign, 21, 1, 42);
            Dynamic_BinaryAssign_Compile_Index_Core(DynamicCSharpExpression.DynamicRightShiftAssign, 84, 1, 42);
        }

        [Fact]
        public void Dynamic_BinaryAssign_Event()
        {
            var o = new WithEvent();
            var i = 0;
            var a = new Action(() => { i++; });

            var p = Expression.Parameter(typeof(object));
            var q = Expression.Parameter(typeof(object));
            var m = DynamicCSharpExpression.DynamicGetMember(p, "Event");

            {
                var z = DynamicCSharpExpression.DynamicAddAssign(m, q);
                var e = Expression.Lambda<Action<object, object>>(z, p, q);
                var f = e.Compile();

                f(o, a);
                o.Do();
                Assert.Equal(1, i);
            }

            {
                var z = DynamicCSharpExpression.DynamicSubtractAssign(m, q);
                var e = Expression.Lambda<Action<object, object>>(z, p, q);
                var f = e.Compile();

                f(o, a);
                o.Do();
                Assert.Equal(1, i);
            }
        }

        [Fact]
        public void Dynamic_BinaryAssign_Assign_DynamicMember()
        {
            var p = Expression.Parameter(typeof(object));

            var v = typeof(StrongBox<int>).GetField("Value");
            var m = Expression.Convert(Expression.Field(Expression.Convert(p, v.DeclaringType), v), typeof(object));
            var c = Expression.New(typeof(StrongBox<int>));
            var a = Expression.Assign(p, c);

            var z = DynamicCSharpExpression.DynamicGetMember(p, "Value");
            var i = DynamicCSharpExpression.DynamicAssign(z, Expression.Constant(42));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal("42,42", f());
        }

        [Fact]
        public void Dynamic_BinaryAssign_Assign_Member()
        {
            var p = Expression.Parameter(typeof(StrongBox<int>));

            var v = typeof(StrongBox<int>).GetField("Value");
            var x = Expression.Field(p, v);
            var m = Expression.Convert(x, typeof(object));
            var c = Expression.New(typeof(StrongBox<int>));
            var a = Expression.Assign(p, c);

            var i = DynamicCSharpExpression.DynamicAssign(x, Expression.Constant(42, typeof(object)));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal("42,42", f());
        }

        [Fact]
        public void Dynamic_BinaryAssign_Assign_DynamicIndex()
        {
            var p = Expression.Parameter(typeof(object));

            var v = typeof(List<int>).GetProperty("Item");
            var x = Expression.Property(Expression.Convert(p, v.DeclaringType), v, Expression.Constant(0));
            var m = Expression.Convert(x, typeof(object));
            var c = Expression.New(typeof(List<int>).GetConstructor(new[] { typeof(IEnumerable<int>) }), Expression.Constant(new[] { 0 }));
            var a = Expression.Assign(p, c);

            var z = DynamicCSharpExpression.DynamicGetIndex(p, Expression.Constant(0));
            var i = DynamicCSharpExpression.DynamicAssign(z, Expression.Constant(42));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal("42,42", f());
        }

        [Fact]
        public void Dynamic_BinaryAssign_Assign_Index()
        {
            var p = Expression.Parameter(typeof(List<int>));

            var v = typeof(List<int>).GetProperty("Item");
            var x = Expression.Property(p, v, Expression.Constant(0));
            var m = Expression.Convert(x, typeof(object));
            var c = Expression.New(typeof(List<int>).GetConstructor(new[] { typeof(IEnumerable<int>) }), Expression.Constant(new[] { 0 }));
            var a = Expression.Assign(p, c);

            var i = DynamicCSharpExpression.DynamicAssign(x, Expression.Constant(42, typeof(object)));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal("42,42", f());
        }

        [Fact]
        public void Dynamic_BinaryAssign_NullCoalescingAssign_DynamicMember_Nullable()
        {
            var p = Expression.Parameter(typeof(object));
            var x = Expression.Parameter(typeof(int?));

            var v = typeof(StrongBox<int?>).GetField("Value");
            var m = Expression.Convert(Expression.Field(Expression.Convert(p, v.DeclaringType), v), typeof(object));
            var c = Expression.MemberInit(Expression.New(typeof(StrongBox<int?>)), Expression.Bind(v, x));
            var a = Expression.Assign(p, c);

            var z = DynamicCSharpExpression.DynamicGetMember(p, "Value");
            var i = DynamicCSharpExpression.DynamicNullCoalescingAssign(z, Expression.Constant(42));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<int?, string>>(b, x);
            var f = e.Compile();

            Assert.Equal("42,42", f(null));
            Assert.Equal("99,99", f(99));
        }

        [Fact]
        public void Dynamic_BinaryAssign_NullCoalescingAssign_Member_Nullable()
        {
            var p = Expression.Parameter(typeof(StrongBox<int?>));
            var y = Expression.Parameter(typeof(int?));

            var v = typeof(StrongBox<int?>).GetField("Value");
            var x = Expression.Field(p, v);
            var m = Expression.Convert(x, typeof(object));
            var c = Expression.MemberInit(Expression.New(typeof(StrongBox<int?>)), Expression.Bind(v, y));
            var a = Expression.Assign(p, c);

            var i = DynamicCSharpExpression.DynamicNullCoalescingAssign(x, Expression.Constant(42, typeof(int?)));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<int?, string>>(b, y);
            var f = e.Compile();

            Assert.Equal("42,42", f(null));
            Assert.Equal("99,99", f(99));
        }

        [Fact]
        public void Dynamic_BinaryAssign_NullCoalescingAssign_DynamicMember_Reference()
        {
            var p = Expression.Parameter(typeof(object));
            var x = Expression.Parameter(typeof(string));

            var v = typeof(StrongBox<string>).GetField("Value");
            var m = Expression.Convert(Expression.Field(Expression.Convert(p, v.DeclaringType), v), typeof(object));
            var c = Expression.MemberInit(Expression.New(typeof(StrongBox<string>)), Expression.Bind(v, x));
            var a = Expression.Assign(p, c);

            var z = DynamicCSharpExpression.DynamicGetMember(p, "Value");
            var i = DynamicCSharpExpression.DynamicNullCoalescingAssign(z, Expression.Constant("foo"));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string, string>>(b, x);
            var f = e.Compile();

            Assert.Equal("foo,foo", f(null));
            Assert.Equal("bar,bar", f("bar"));
        }

        [Fact]
        public void Dynamic_BinaryAssign_NullCoalescingAssign_Member_Reference()
        {
            var p = Expression.Parameter(typeof(StrongBox<string>));
            var y = Expression.Parameter(typeof(string));

            var v = typeof(StrongBox<string>).GetField("Value");
            var x = Expression.Field(p, v);
            var m = Expression.Convert(x, typeof(object));
            var c = Expression.MemberInit(Expression.New(typeof(StrongBox<string>)), Expression.Bind(v, y));
            var a = Expression.Assign(p, c);

            var i = DynamicCSharpExpression.DynamicNullCoalescingAssign(x, Expression.Constant("foo"));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string, string>>(b, y);
            var f = e.Compile();

            Assert.Equal("foo,foo", f(null));
            Assert.Equal("bar,bar", f("bar"));
        }

        [Fact]
        public void Dynamic_BinaryAssign_NullCoalescingAssign_DynamicIndex_Nullable()
        {
            var p = Expression.Parameter(typeof(object));
            var y = Expression.Parameter(typeof(int?));

            var v = typeof(List<int?>).GetProperty("Item");
            var x = Expression.Property(Expression.Convert(p, v.DeclaringType), v, Expression.Constant(0));
            var m = Expression.Convert(x, typeof(object));
            var c = Expression.ListInit(Expression.New(typeof(List<int?>)), Expression.ElementInit(typeof(List<int?>).GetMethod("Add"), y));
            var a = Expression.Assign(p, c);

            var z = DynamicCSharpExpression.DynamicGetIndex(p, Expression.Constant(0));
            var i = DynamicCSharpExpression.DynamicNullCoalescingAssign(z, Expression.Constant(42));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<int?, string>>(b, y);
            var f = e.Compile();

            Assert.Equal("42,42", f(null));
            Assert.Equal("99,99", f(99));
        }

        [Fact]
        public void Dynamic_BinaryAssign_NullCoalescingAssign_Index_Nullable()
        {
            var p = Expression.Parameter(typeof(List<int?>));
            var y = Expression.Parameter(typeof(int?));

            var v = typeof(List<int?>).GetProperty("Item");
            var x = Expression.Property(p, v, Expression.Constant(0));
            var m = Expression.Convert(x, typeof(object));
            var c = Expression.ListInit(Expression.New(typeof(List<int?>)), Expression.ElementInit(typeof(List<int?>).GetMethod("Add"), y));
            var a = Expression.Assign(p, c);

            var i = DynamicCSharpExpression.DynamicNullCoalescingAssign(x, Expression.Constant(42, typeof(int?)));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<int?, string>>(b, y);
            var f = e.Compile();

            Assert.Equal("42,42", f(null));
            Assert.Equal("99,99", f(99));
        }

        [Fact]
        public void Dynamic_BinaryAssign_NullCoalescingAssign_DynamicIndex_Reference()
        {
            var p = Expression.Parameter(typeof(object));
            var y = Expression.Parameter(typeof(string));

            var v = typeof(List<string>).GetProperty("Item");
            var x = Expression.Property(Expression.Convert(p, v.DeclaringType), v, Expression.Constant(0));
            var m = Expression.Convert(x, typeof(object));
            var c = Expression.ListInit(Expression.New(typeof(List<string>)), Expression.ElementInit(typeof(List<string>).GetMethod("Add"), y));
            var a = Expression.Assign(p, c);

            var z = DynamicCSharpExpression.DynamicGetIndex(p, Expression.Constant(0));
            var i = DynamicCSharpExpression.DynamicNullCoalescingAssign(z, Expression.Constant("foo"));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string, string>>(b, y);
            var f = e.Compile();

            Assert.Equal("foo,foo", f(null));
            Assert.Equal("bar,bar", f("bar"));
        }

        [Fact]
        public void Dynamic_BinaryAssign_NullCoalescingAssign_Index_Reference()
        {
            var p = Expression.Parameter(typeof(List<string>));
            var y = Expression.Parameter(typeof(string));

            var v = typeof(List<string>).GetProperty("Item");
            var x = Expression.Property(p, v, Expression.Constant(0));
            var m = Expression.Convert(x, typeof(object));
            var c = Expression.ListInit(Expression.New(typeof(List<string>)), Expression.ElementInit(typeof(List<string>).GetMethod("Add"), y));
            var a = Expression.Assign(p, c);

            var i = DynamicCSharpExpression.DynamicNullCoalescingAssign(x, Expression.Constant("foo"));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string, string>>(b, y);
            var f = e.Compile();

            Assert.Equal("foo,foo", f(null));
            Assert.Equal("bar,bar", f("bar"));
        }

        private void Dynamic_BinaryAssign_Compile_Variable_Core<TLeft, TRight>(Func<Expression, Expression, AssignBinaryDynamicCSharpExpression> factory, TLeft left, TRight right, TLeft res)
        {
            var p = Expression.Parameter(typeof(object));

            var c = Expression.Convert(Expression.Constant(left, typeof(TLeft)), typeof(object));
            var a = Expression.Assign(p, c);

            var i = factory(p, Expression.Constant(right, typeof(TRight)));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), p);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal($"{res},{res}", f());
        }

        private void Dynamic_BinaryAssign_Compile_DynamicMember_Core<TLeft, TRight>(Func<Expression, Expression, AssignBinaryDynamicCSharpExpression> factory, TLeft left, TRight right, TLeft res)
        {
            var p = Expression.Parameter(typeof(object));

            var v = typeof(StrongBox<TLeft>).GetField("Value");
            var m = Expression.Convert(Expression.Field(Expression.Convert(p, v.DeclaringType), v), typeof(object));
            var c = Expression.MemberInit(Expression.New(typeof(StrongBox<TLeft>)), Expression.Bind(v, Expression.Constant(left, typeof(TLeft))));
            var a = Expression.Assign(p, c);

            var z = DynamicCSharpExpression.DynamicGetMember(p, "Value");
            var i = factory(z, Expression.Constant(right, typeof(TRight)));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal($"{res},{res}", f());
        }

        private void Dynamic_BinaryAssign_Compile_Member_Core<TLeft, TRight>(Func<Expression, Expression, AssignBinaryDynamicCSharpExpression> factory, TLeft left, TRight right, TLeft res)
        {
            var p = Expression.Parameter(typeof(StrongBox<TLeft>));

            var v = typeof(StrongBox<TLeft>).GetField("Value");
            var z = Expression.Field(p, v);
            var m = Expression.Convert(z, typeof(object));
            var c = Expression.MemberInit(Expression.New(typeof(StrongBox<TLeft>)), Expression.Bind(v, Expression.Constant(left, typeof(TLeft))));
            var a = Expression.Assign(p, c);

            var i = factory(z, Expression.Constant(right, typeof(object)));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal($"{res},{res}", f());
        }

        private void Dynamic_BinaryAssign_Compile_DynamicIndex_Core<TLeft, TRight>(Func<Expression, Expression, AssignBinaryDynamicCSharpExpression> factory, TLeft left, TRight right, TLeft res)
        {
            var p = Expression.Parameter(typeof(object));

            var v = typeof(List<TLeft>).GetMethod("Add", new[] { typeof(TLeft) });
            var m = Expression.Convert(Expression.MakeIndex(Expression.Convert(p, v.DeclaringType), typeof(List<TLeft>).GetProperty("Item"), new[] { Expression.Constant(0) }), typeof(object));
            var c = Expression.ListInit(Expression.New(typeof(List<TLeft>)), Expression.ElementInit(v, Expression.Constant(left, typeof(TLeft))));
            var a = Expression.Assign(p, c);

            var z = DynamicCSharpExpression.DynamicGetIndex(p, Expression.Constant(0));
            var i = factory(z, Expression.Constant(right, typeof(TRight)));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal($"{res},{res}", f());
        }

        private void Dynamic_BinaryAssign_Compile_Index_Core<TLeft, TRight>(Func<Expression, Expression, AssignBinaryDynamicCSharpExpression> factory, TLeft left, TRight right, TLeft res)
        {
            var p = Expression.Parameter(typeof(List<TLeft>));

            var v = typeof(List<TLeft>).GetMethod("Add", new[] { typeof(TLeft) });
            var z = Expression.MakeIndex(p, typeof(List<TLeft>).GetProperty("Item"), new[] { Expression.Constant(0) });
            var m = Expression.Convert(z, typeof(object));
            var c = Expression.ListInit(Expression.New(typeof(List<TLeft>)), Expression.ElementInit(v, Expression.Constant(left, typeof(TLeft))));
            var a = Expression.Assign(p, c);

            var i = factory(z, Expression.Constant(right, typeof(object)));

            var d = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
            var s = Expression.Call(d, Expression.Convert(i, typeof(object)), Expression.Constant(","), m);

            var b = Expression.Block(new[] { p }, a, s);

            var e = Expression.Lambda<Func<string>>(b);
            var f = e.Compile();

            Assert.Equal($"{res},{res}", f());
        }

        static void AssertNoChange(CSharpExpression e)
        {
            var r = new Nop().Visit(e);
            Assert.Same(e, r);
        }

        static void AssertChange(CSharpExpression e)
        {
            var r = new Change().Visit(e);
            Assert.NotSame(e, r);
            Assert.Equal(r.ToString(), e.ToString()); // TODO: use a DebugView when we add it
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

    public class WithEvent
    {
        public event Action Event;

        public void Do()
        {
            Event?.Invoke();
        }
    }
}
