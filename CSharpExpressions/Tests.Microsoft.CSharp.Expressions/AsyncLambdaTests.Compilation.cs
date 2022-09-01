// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Tests.ReflectionUtils;

namespace Tests
{
    partial class AsyncLambdaTests
    {
        [Fact]
        public void AsyncLambda_Compilation_NotInFilter()
        {
            var p = Expression.Parameter(typeof(Exception));

            var expr = Expression.TryCatch(
                Expression.Empty(),
                Expression.Catch(
                    p,
                    Expression.Empty(),
                    CSharpExpression.Await(Expression.Constant(Task.FromResult(true)))
                )
            );

            var e = CSharpExpression.AsyncLambda<Func<Task>>(expr);
            Assert.Throws<InvalidOperationException>(() => e.Compile());
        }

        [Fact]
        public void AsyncLambda_Compilation_NotInLock()
        {
            var p = Expression.Parameter(typeof(Exception));

            var expr = CSharpExpression.Lock(
                Expression.Default(typeof(object)),
                CSharpExpression.Await(Expression.Constant(Task.FromResult(42)))
            );

            var e = CSharpExpression.AsyncLambda<Func<Task>>(expr);
            Assert.Throws<InvalidOperationException>(() => e.Compile());
        }

        [Fact]
        public void AsyncLambda_Compilation_NotInLambda()
        {
            var p = Expression.Parameter(typeof(Exception));

            var expr = Expression.Lambda(
                CSharpExpression.Await(Expression.Constant(Task.FromResult(42)))
            );

            var e = CSharpExpression.AsyncLambda<Func<Task<Func<int>>>>(expr);
            Assert.Throws<InvalidOperationException>(() => e.Compile());
        }

        [Fact]
        public void AsyncLambda_Compilation_NotInSwitchCaseTestValue()
        {
            var p = Expression.Parameter(typeof(Exception));

            var expr = Expression.Lambda(
                Expression.Switch(
                    Expression.Constant(0),
                    Expression.Constant(1),
                    Expression.SwitchCase(
                        Expression.Constant(1),
                        Expression.Constant(0)
                    ),
                    Expression.SwitchCase(
                        Expression.Constant(1),
                        CSharpExpression.Await(Expression.Constant(Task.FromResult(42)))
                    )
                )
            );

            var e = CSharpExpression.AsyncLambda<Func<Task<Func<int>>>>(expr);
            Assert.Throws<InvalidOperationException>(() => e.Compile());
        }

        [Fact]
        public void AsyncLambda_Compilation_NestedLambda()
        {
            var p = Expression.Parameter(typeof(Exception));

            var expr = Expression.Invoke(Expression.Lambda(Expression.Constant(42)));

            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(expr);
            Assert.Equal(42, e.Compile()().Result);
        }

        [Fact]
        public void AsyncLambda_Compilation_NestedAsyncLambda()
        {
            var p = Expression.Parameter(typeof(Exception));

            var expr = CSharpExpression.AsyncLambda(
                CSharpExpression.Await(Expression.Constant(Task.FromResult(42)))
            );

            var e = CSharpExpression.AsyncLambda<Func<Task<Func<Task<int>>>>>(expr);
            Assert.Equal(42, e.Compile()().Result().Result);
        }

        [Fact(Skip = "Used to be NotSupportedException but may be fixed now. Review https://github.com/dotnet/coreclr/issues/1764.")]
        public void AsyncLambda_Compilation_NotInFilter_NoFalsePositive()
        {
            var p = Expression.Parameter(typeof(Exception));

            var expr = Expression.TryCatch(
                Expression.Empty(),
                Expression.Catch(
                    p,
                    CSharpExpression.Await(Expression.Constant(Task.FromResult(true), typeof(Task))),
                    Expression.Constant(true)
                )
            );

            var e = CSharpExpression.AsyncLambda<Func<Task>>(expr);

            // DynamicMethod does not support BeginExceptFilterBlock (see https://github.com/dotnet/coreclr/issues/1764)
            Assert.Throws<NotSupportedException>(() => e.Compile());
        }

        [Fact]
        public void AsyncLambda_Compilation_InLockResource()
        {
            var p = Expression.Parameter(typeof(Exception));

            var expr = CSharpExpression.Lock(
                CSharpExpression.Await(Expression.Constant(Task.FromResult(new object()))),
                Expression.Empty()
            );

            var e = CSharpExpression.AsyncLambda<Func<Task>>(expr);

            e.Compile()();
        }

        [Fact]
        public void AsyncLambda_Compilation_Simple0()
        {
            var p = Expression.Parameter(typeof(int));

            var e1 = CSharpExpression.AsyncLambda<Func<int, Task<int>>>(p, p);
            var e2 = CSharpExpression.AsyncLambda(typeof(Func<int, Task<int>>), p, p);
            var e3 = CSharpExpression.AsyncLambda<Func<int, Task<int>>>(p, p);

            foreach (AsyncCSharpExpression<Func<int, Task<int>>> e in new[] { e1, e2, e3 })
            {
                Assert.Equal(42, e.Compile()(42).Result);
            }
        }

        [Fact]
        public void AsyncLambda_Compilation_Simple1()
        {
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Constant(42));
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(42, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Simple2()
        {
            var e = CSharpExpression.AsyncLambda<Func<Task>>(Expression.Constant(42));
            var f = e.Compile();
            var t = f();
            t.Wait();
        }

        [Fact]
        public void AsyncLambda_Compilation_Simple3()
        {
            var log = new List<string>();
            var a = (Expression<Action>)(() => log.Add("OK"));

            var e = CSharpExpression.AsyncLambda<Func<Task>>(a.Body);
            var f = e.Compile();
            var t = f();

            // Add happens on sync code path
            Assert.True(new[] { "OK" }.SequenceEqual(log));

            t.Wait();
        }

        [Fact]
        public void AsyncLambda_Compilation_Simple4()
        {
            var log = new List<string>();
            var a = (Expression<Action>)(() => log.Add("OK"));

            var e = CSharpExpression.AsyncLambda<Action>(a.Body);
            var f = e.Compile();
            f();

            // Add happens on sync code path
            Assert.True(new[] { "OK" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_Simple5()
        {
            var v = Expression.Constant(Task.FromResult(42));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(CSharpExpression.Await(v));
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(42, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Simple6()
        {
            var l = Expression.Label(typeof(int));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Block(Expression.Return(l, Expression.Constant(42)), Expression.Label(l, Expression.Constant(0))));
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(42, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_CustomGetAwaiter()
        {
            var m = MethodInfoOf(() => GetAwaiter<int>(default(Task<int>)));
            var v = Expression.Constant(Task.FromResult(42));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(CSharpExpression.Await(v, m));
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(42, r);
        }

        private static TaskAwaiter<T> GetAwaiter<T>(Task<T> task)
        {
            return task.GetAwaiter();
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Unary1()
        {
            var v = Expression.Constant(Task.FromResult(1));
            var neg = Expression.Negate(CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(neg);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(-1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Convert1()
        {
            var v = Expression.Constant(Task.FromResult((object)1));
            var conv = Expression.Convert(CSharpExpression.Await(v), typeof(int));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(conv);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Convert2()
        {
            var v = Expression.Constant(Task.FromResult(1L));
            var conv = Expression.ConvertChecked(CSharpExpression.Await(v), typeof(int));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(conv);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Unbox()
        {
            var v = Expression.Constant(Task.FromResult((object)1));
            var conv = Expression.Unbox(CSharpExpression.Await(v), typeof(int));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(conv);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Unary_More()
        {
            SpillUnary(Expression.UnaryPlus, 28, +28);
            SpillUnary(Expression.Negate, 28, -28);
            SpillUnary(Expression.NegateChecked, 28, checked(-28));
            SpillUnary(Expression.OnesComplement, 28, ~28);
            SpillUnary(Expression.Increment, 28, 28 + 1);
            SpillUnary(Expression.Decrement, 28, 28 - 1);
            SpillUnary(Expression.Not, true, !true);
            SpillUnary(Expression.IsTrue, true, true == true);
            SpillUnary(Expression.IsFalse, true, true == false);
            SpillUnary(Expression.ArrayLength, new[] { 42 }, new[] { 42 }.Length);
        }

        private void SpillUnary<T, R>(Func<Expression, Expression> factory, T operand, R result)
        {
            var oc = (Expression)Expression.Constant(operand);

            var oa = (Expression)CSharpExpression.Await(Expression.Constant(Task.FromResult(operand)));

            foreach (var op in new[]
            {
                oc,
                oa
            })
            {
                var e = CSharpExpression.AsyncLambda<Func<Task<R>>>(factory(op));
                var f = e.Compile();
                var t = f();
                var r = t.Result;
                Assert.Equal(result, r);
            }
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Binary1()
        {
            var v1 = Expression.Constant(Task.FromResult(1));
            var v2 = Expression.Constant(Task.FromResult(2));
            var add = Expression.Add(CSharpExpression.Await(v1), CSharpExpression.Await(v2));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(add);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(3, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Binary2()
        {
            var v = Expression.Constant(Task.FromResult(1));
            var add = Expression.Add(CSharpExpression.Await(v), Expression.Constant(2));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(add);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(3, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Binary3()
        {
            var v = Expression.Constant(Task.FromResult(1));
            var add = Expression.Add(Expression.Constant(2), CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(add);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(3, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Binary_More()
        {
            SpillBinary(Expression.Add, 28, 41, 28 + 41);
            SpillBinary(Expression.AddChecked, 28, 41, checked(28 + 41));
            SpillBinary(Expression.Subtract, 28, 41, 28 - 41);
            SpillBinary(Expression.SubtractChecked, 28, 41, checked(28 - 41));
            SpillBinary(Expression.Multiply, 28, 41, 28 * 41);
            SpillBinary(Expression.MultiplyChecked, 28, 41, checked(28 * 41));
            SpillBinary(Expression.Divide, 28, 41, 28 / 41);
            SpillBinary(Expression.Modulo, 28, 41, checked(28 % 41));
            SpillBinary(Expression.And, 28, 41, 28 & 41);
            SpillBinary(Expression.Or, 28, 41, 28 | 41);
            SpillBinary(Expression.ExclusiveOr, 28, 41, 28 ^ 41);
            SpillBinary(Expression.LeftShift, 28, 3, 28 << 3);
            SpillBinary(Expression.RightShift, 28, 3, 28 >> 3);
            SpillBinary(Expression.LessThan, 28, 41, 28 < 41);
            SpillBinary(Expression.LessThanOrEqual, 28, 41, 28 <= 41);
            SpillBinary(Expression.GreaterThan, 28, 41, 28 > 41);
            SpillBinary(Expression.GreaterThanOrEqual, 28, 41, 28 >= 41);
            SpillBinary(Expression.Equal, 28, 41, 28 == 41);
            SpillBinary(Expression.NotEqual, 28, 41, 28 != 41);
            SpillBinary(Expression.Power, 28.0, 41.0, Math.Pow(28.0, 41.0));
            SpillBinary(Expression.ArrayIndex, new[] { 28, 41 }, 1, new[] { 28, 41 }[1]);
        }

        private void SpillBinary<T1, T2, R>(Func<Expression, Expression, Expression> factory, T1 left, T2 right, R result)
        {
            var lc = (Expression)Expression.Constant(left);
            var rc = (Expression)Expression.Constant(right);

            var la = (Expression)CSharpExpression.Await(Expression.Constant(Task.FromResult(left)));
            var ra = (Expression)CSharpExpression.Await(Expression.Constant(Task.FromResult(right)));

            foreach (var ops in new[]
            {
                new[] { lc, rc },
                new[] { lc, ra },
                new[] { la, rc },
                new[] { la, ra },
            })
            {
                var e = CSharpExpression.AsyncLambda<Func<Task<R>>>(factory(ops[0], ops[1]));
                var f = e.Compile();
                var t = f();
                var r = t.Result;
                Assert.Equal(result, r);
            }
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Member1()
        {
            var v = Expression.Constant(Task.FromResult("bar"));
            var length = Expression.Property(CSharpExpression.Await(v), "Length");
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(length);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(3, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_LogicalBinary1()
        {
            var v = Expression.Constant(Task.FromResult(true));
            var andAlso = Expression.AndAlso(Expression.Constant(true), CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<bool>>>(andAlso);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.True(r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_LogicalBinary2()
        {
            var v = Expression.Constant(Task.FromResult(true));
            var orElse = Expression.OrElse(CSharpExpression.Await(v), Expression.Constant(true));
            var e = CSharpExpression.AsyncLambda<Func<Task<bool>>>(orElse);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.True(r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_LogicalBinary3()
        {
            var v = Expression.Constant(Task.FromResult(default(string)));
            var coalesce = Expression.Coalesce(CSharpExpression.Await(v), Expression.Constant("bar"));
            var e = CSharpExpression.AsyncLambda<Func<Task<string>>>(coalesce);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal("bar", r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Condition1()
        {
            var v = Expression.Constant(Task.FromResult(true));
            var condition = Expression.Condition(CSharpExpression.Await(v), Expression.Constant(1), Expression.Constant(2));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(condition);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Condition2()
        {
            var v = Expression.Constant(Task.FromResult(1));
            var condition = Expression.Condition(Expression.Constant(true), CSharpExpression.Await(v), Expression.Constant(2));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(condition);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Condition3()
        {
            var v = Expression.Constant(Task.FromResult(1));
            var condition = Expression.Condition(Expression.Constant(false), Expression.Constant(2), CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(condition);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_TypeBinary1()
        {
            var v = Expression.Constant(Task.FromResult((object)1));
            var typeIs = Expression.TypeIs(CSharpExpression.Await(v), typeof(int));
            var e = CSharpExpression.AsyncLambda<Func<Task<bool>>>(typeIs);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.True(r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_TypeBinary2()
        {
            var v = Expression.Constant(Task.FromResult((object)1));
            var typeEqual = Expression.TypeEqual(CSharpExpression.Await(v), typeof(int));
            var e = CSharpExpression.AsyncLambda<Func<Task<bool>>>(typeEqual);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.True(r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_ListInit1()
        {
            var v = Expression.Constant(Task.FromResult(1));
            var listInit = Expression.ListInit(Expression.New(typeof(List<int>).GetConstructor(new Type[0])), CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<List<int>>>>(listInit);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r[0]);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_MemberInit1()
        {
            var elem = typeof(int);
            var sb = typeof(StrongBox<>).MakeGenericType(elem);
            var v = Expression.Constant(Task.FromResult(1));
            var a = CSharpExpression.Await(v);
            var bind = Expression.Bind(sb.GetField("Value"), a);
            var memberInit = Expression.MemberInit(Expression.New(sb.GetConstructor(new Type[0])), bind);
            var e = CSharpExpression.AsyncLambda<Func<Task<StrongBox<int>>>>(memberInit);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r.Value);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_MemberInit2()
        {
            var elem = typeof(int);
            var sb = typeof(StrongBox<>).MakeGenericType(elem);
            var v = Expression.Constant(Task.FromResult(1));
            var a = CSharpExpression.Await(v);
            var bind = Expression.Bind(sb.GetField("Value"), Expression.Constant(2));
            var memberInit = Expression.MemberInit(Expression.New(sb.GetConstructor(new[] { elem }), a), bind);
            var e = CSharpExpression.AsyncLambda<Func<Task<StrongBox<int>>>>(memberInit);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(2, r.Value);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_MemberInit3()
        {
            var elem = typeof(StrongBox<int>);
            var sb = typeof(StrongBox<>).MakeGenericType(elem);
            var v = Expression.Constant(Task.FromResult(1));
            var a = CSharpExpression.Await(v);
            var bind = Expression.MemberBind(sb.GetField("Value"), Expression.Bind(typeof(StrongBox<int>).GetField("Value"), a));
            var memberInit = Expression.MemberInit(Expression.New(sb.GetConstructor(new[] { elem }), Expression.New(elem.GetConstructor(new Type[0]))), bind);
            var e = CSharpExpression.AsyncLambda<Func<Task<StrongBox<StrongBox<int>>>>>(memberInit);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r.Value.Value);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_MemberInit4()
        {
            var elem = typeof(List<int>);
            var sb = typeof(StrongBox<>).MakeGenericType(elem);
            var v = Expression.Constant(Task.FromResult(1));
            var a = CSharpExpression.Await(v);
            var bind = Expression.ListBind(sb.GetField("Value"), Expression.ElementInit(typeof(List<int>).GetMethod("Add"), a));
            var memberInit = Expression.MemberInit(Expression.New(sb.GetConstructor(new[] { elem }), Expression.New(elem.GetConstructor(new Type[0]))), bind);
            var e = CSharpExpression.AsyncLambda<Func<Task<StrongBox<List<int>>>>>(memberInit);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r.Value[0]);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Assign1()
        {
            var p = Expression.Parameter(typeof(int));
            var v = Expression.Constant(Task.FromResult(1));
            var assign = Expression.Assign(p, CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Block(new[] { p }, assign, p));
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Assign2()
        {
            var p = Expression.Parameter(typeof(int[]));
            var a = Expression.NewArrayInit(typeof(int), Expression.Constant(1));
            var i = Expression.ArrayAccess(p, Expression.Constant(0));
            var v = Expression.Constant(Task.FromResult(1));
            var assign = Expression.Assign(i, CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Block(new[] { p }, Expression.Assign(p, a), assign, i));
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Assign3()
        {
            var p = Expression.Parameter(typeof(StrongBox<int>));
            var a = Expression.New(typeof(StrongBox<int>).GetConstructor(new Type[0]));
            var i = Expression.Field(p, "Value");
            var v = Expression.Constant(Task.FromResult(1));
            var assign = Expression.Assign(i, CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Block(new[] { p }, Expression.Assign(p, a), assign, i));
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_New1()
        {
            var v = Expression.Constant(Task.FromResult(1));
            var oc = Expression.New(typeof(StrongBox<int>).GetConstructor(new[] { typeof(int) }), CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<StrongBox<int>>>>(oc);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r.Value);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_NewArray1()
        {
            var v = Expression.Constant(Task.FromResult(1));
            var na = Expression.NewArrayInit(typeof(int), CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<int[]>>>(na);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r[0]);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_NewArray2()
        {
            var v = Expression.Constant(Task.FromResult(1));
            var na = Expression.NewArrayBounds(typeof(int), CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<int[]>>>(na);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Single(r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Invoke1()
        {
            var v = Expression.Constant(Task.FromResult(new Func<int, int>(x => x + 1)));
            var i = Expression.Invoke(CSharpExpression.Await(v), Expression.Constant(2));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(i);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(3, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Invoke2()
        {
            var v = Expression.Constant(Task.FromResult(2));
            var i = Expression.Invoke(Expression.Constant(new Func<int, int>(x => x + 1)), CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(i);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(3, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Index1()
        {
            var v = Expression.Constant(Task.FromResult(new List<int> { 1 }));
            var i = Expression.MakeIndex(CSharpExpression.Await(v), typeof(List<int>).GetProperty("Item"), new Expression[] { Expression.Constant(0) });
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(i);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Index2()
        {
            var v = Expression.Constant(Task.FromResult(0));
            var i = Expression.MakeIndex(Expression.Constant(new List<int> { 1 }), typeof(List<int>).GetProperty("Item"), new Expression[] { CSharpExpression.Await(v) });
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(i);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Dynamic1()
        {
            var v = Expression.Constant(Task.FromResult(1));
            var dyn = DynamicCSharpExpression.DynamicAdd(CSharpExpression.Await(v), Expression.Constant(2)).Reduce();
            var e = CSharpExpression.AsyncLambda<Func<Task<object>>>(dyn);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(3, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Dynamic2()
        {
            var v = Expression.Constant(Task.FromResult(1));
            var dyn = DynamicCSharpExpression.DynamicAdd(Expression.Constant(2), CSharpExpression.Await(v)).Reduce();
            var e = CSharpExpression.AsyncLambda<Func<Task<object>>>(dyn);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(3, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Switch1()
        {
            var v = Expression.Constant(Task.FromResult(0));
            var sw = Expression.Switch(CSharpExpression.Await(v), Expression.Constant(1), Expression.SwitchCase(Expression.Constant(2), Expression.Constant(1)));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(sw);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Switch2()
        {
            var v = Expression.Constant(Task.FromResult(1));
            var sw = Expression.Switch(Expression.Constant(0), CSharpExpression.Await(v), Expression.SwitchCase(Expression.Constant(2), Expression.Constant(1)));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(sw);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_Switch3()
        {
            var v = Expression.Constant(Task.FromResult(2));
            var sw = Expression.Switch(Expression.Constant(1), Expression.Constant(1), Expression.SwitchCase(CSharpExpression.Await(v), Expression.Constant(1)));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(sw);
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(2, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_CallByRef_Local()
        {
            var v = Expression.Constant(Task.FromResult(42));
            var x = Expression.Parameter(typeof(int));
            var method = typeof(Interlocked).GetMethod(nameof(Interlocked.Exchange), new[] { typeof(int).MakeByRefType(), typeof(int) });
            var call = Expression.Call(method, x, CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Block(new[] { x }, call, x));
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(42, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_CallByRef_Array()
        {
            var v = Expression.Constant(Task.FromResult(42));
            var xs = Expression.Parameter(typeof(int[]));
            var newArray = Expression.Assign(xs, Expression.NewArrayBounds(typeof(int), Expression.Constant(1)));
            var elem = Expression.ArrayAccess(xs, Expression.Constant(0));
            var method = typeof(Interlocked).GetMethod(nameof(Interlocked.Exchange), new[] { typeof(int).MakeByRefType(), typeof(int) });
            var call = Expression.Call(method, elem, CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Block(new[] { xs }, newArray, call, elem));
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(42, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_CallByRef_Array_HasBoundsCheckBeforeAwait()
        {
            var v = Expression.Constant(Task.FromException<int>(new Exception("Shouldn't observe me!")));
            var xs = Expression.Parameter(typeof(int[]));
            var newArray = Expression.Assign(xs, Expression.NewArrayBounds(typeof(int), Expression.Constant(0)));
            var elem = Expression.ArrayAccess(xs, Expression.Constant(0));
            var method = typeof(Interlocked).GetMethod(nameof(Interlocked.Exchange), new[] { typeof(int).MakeByRefType(), typeof(int) });
            var call = Expression.Call(method, elem, CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task>>(Expression.Block(new[] { xs }, newArray, call));
            var f = e.Compile();
            Assert.Throws<IndexOutOfRangeException>(() => f().GetAwaiter().GetResult());
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_CallByRef_Field()
        {
            var v = Expression.Constant(Task.FromResult(42));
            var b = Expression.Parameter(typeof(StrongBox<int>));
            var newBox = Expression.Assign(b, Expression.New(b.Type.GetConstructor(new[] { typeof(int) }), Expression.Constant(1)));
            var elem = Expression.Field(b, b.Type.GetField(nameof(StrongBox<int>.Value)));
            var method = typeof(Interlocked).GetMethod(nameof(Interlocked.Exchange), new[] { typeof(int).MakeByRefType(), typeof(int) });
            var call = Expression.Call(method, elem, CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Block(new[] { b }, newBox, call, elem));
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(42, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_Spilling_CallByRef_HasNullCheckBeforeAwait()
        {
            var v = Expression.Constant(Task.FromException<int>(new Exception("Shouldn't observe me!")));
            var b = Expression.Parameter(typeof(StrongBox<int>));
            var newBox = Expression.Assign(b, Expression.Default(b.Type));
            var elem = Expression.Field(b, typeof(StrongBox<int>).GetField(nameof(StrongBox<int>.Value)));
            var method = typeof(Interlocked).GetMethod(nameof(Interlocked.Exchange), new[] { typeof(int).MakeByRefType(), typeof(int) });
            var call = Expression.Call(method, elem, CSharpExpression.Await(v));
            var e = CSharpExpression.AsyncLambda<Func<Task>>(Expression.Block(new[] { b }, newBox, call));
            var f = e.Compile();
            var t = f();
            Assert.Throws<NullReferenceException>(() => f().GetAwaiter().GetResult());
        }

        [Fact]
        public void AsyncLambda_Compilation_Hoisting()
        {
            var fromResultMethod = MethodInfoOf(() => Task.FromResult(default(int)));
            var i = Expression.Parameter(typeof(int));
            var res = Expression.Parameter(typeof(int));
            var brk = Expression.Label();
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.Block(
                    new[] { i, res },
                    Expression.Assign(i, Expression.Constant(0)),
                    Expression.Loop(
                        Expression.Block(
                            Expression.IfThen(
                                Expression.Equal(i, Expression.Constant(10)),
                                Expression.Break(brk)
                            ),
                            Expression.AddAssign(
                                res,
                                CSharpExpression.Await(Expression.Call(fromResultMethod, i))
                            ),
                            Expression.PostIncrementAssign(i)
                        ), brk
                    ),
                    res
                )
            );
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(Enumerable.Range(0, 10).Sum(), r);
        }

        [Fact]
        public void AsyncLambda_Compilation_HoistingParameters()
        {
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var x = Expression.Parameter(typeof(int));
            var e = CSharpExpression.AsyncLambda<Func<int, Task<int>>>(
                Expression.Block(
                    CSharpExpression.Await(yield),
                    x
                ),
                x
            );
            var f = e.Compile();
            var t = f(42);
            var r = t.Result;
            Assert.Equal(42, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_ResumeInTry1()
        {
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.Block(
                    Expression.TryCatch(
                        Expression.Block(
                            CSharpExpression.Await(yield),
                            Expression.Constant(42)
                        ),
                        Expression.Catch(Expression.Parameter(typeof(Exception)),
                            Expression.Constant(-1)
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(42, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_ResumeInTry2()
        {
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.Block(
                    Expression.TryCatch(
                        Expression.Block(
                            CSharpExpression.Await(yield),
                            Expression.Divide(Expression.Constant(1), Expression.Constant(0))
                        ),
                        Expression.Catch(Expression.Parameter(typeof(DivideByZeroException)),
                            Expression.Constant(-1)
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(-1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_ResumeInTry3()
        {
            var p = Expression.Parameter(typeof(D));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<D, Task<int>>>(
                Expression.Block(
                    Expression.TryFinally(
                        Expression.Block(
                            CSharpExpression.Await(yield),
                            Expression.Call(p, typeof(D).GetMethod("Do"))
                        ),
                        Expression.Call(p, typeof(D).GetMethod("Dispose"))
                    )
                ),
                p
            );
            var d = new D();
            var f = e.Compile();
            var t = f(d);
            var r = t.Result;
            Assert.Equal(42, r);
            Assert.True(d.IsDisposed);
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInFinally1()
        {
            var p = Expression.Parameter(typeof(D));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<D, Task<int>>>(
                Expression.TryFinally(
                    Expression.Call(p, typeof(D).GetMethod("Do")),
                    Expression.Block(
                        CSharpExpression.Await(yield),
                        Expression.Call(p, typeof(D).GetMethod("Dispose"))
                    )
                ),
                p
            );
            var d = new D();
            var f = e.Compile();
            var t = f(d);
            var r = t.Result;
            Assert.Equal(42, r);
            Assert.True(d.IsDisposed);
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInFinally2()
        {
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.TryCatch(
                    Expression.TryFinally(
                        Expression.Divide(Expression.Constant(1), Expression.Constant(0)),
                        CSharpExpression.Await(yield)
                    ),
                    Expression.Catch(Expression.Parameter(typeof(DivideByZeroException)),
                        Expression.Constant(-1)
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(-1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInFinally3()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryFinally(
                    Expression.Call(logExpr, add, Expression.Constant("T")),
                    Expression.Block(
                        Expression.Call(logExpr, add, Expression.Constant("FB")),
                        CSharpExpression.Await(yield),
                        Expression.Call(logExpr, add, Expression.Constant("FE"))
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            t.Wait();
            Assert.True(new[] { "T", "FB", "FE" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInFinally4()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryCatch(
                    Expression.TryFinally(
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("T")),
                            Expression.Throw(Expression.Constant(new Exception()))
                        ),
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("FB")),
                            CSharpExpression.Await(yield),
                            Expression.Call(logExpr, add, Expression.Constant("FE"))
                        )
                    ),
                    Expression.Catch(Expression.Parameter(typeof(Exception)),
                        Expression.Call(logExpr, add, Expression.Constant("C"))
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            t.Wait();
            Assert.True(new[] { "T", "FB", "FE", "C" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInFinally5()
        {
            var p = Expression.Parameter(typeof(int));
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<int, Task<int>>>(
                Expression.TryFinally(
                    Expression.Block(
                        Expression.Call(logExpr, add, Expression.Constant("T")),
                        Expression.Divide(Expression.Constant(42), p)
                    ),
                    Expression.Block(
                        Expression.Call(logExpr, add, Expression.Constant("FB")),
                        CSharpExpression.Await(yield),
                        Expression.Call(logExpr, add, Expression.Constant("FE"))
                    )
                ),
                p
            );
            var f = e.Compile();

            var t1 = f(1);
            Assert.Equal(42, t1.Result);
            Assert.True(new[] { "T", "FB", "FE" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInFault1()
        {
            var p = Expression.Parameter(typeof(D));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<D, Task<int>>>(
                Expression.TryFault(
                    Expression.Call(p, typeof(D).GetMethod("Do")),
                    Expression.Block(
                        CSharpExpression.Await(yield),
                        Expression.Call(p, typeof(D).GetMethod("Dispose"))
                    )
                ),
                p
            );
            var d = new D();
            var f = e.Compile();
            var t = f(d);
            var r = t.Result;
            Assert.Equal(42, r);
            Assert.False(d.IsDisposed);
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInFault2()
        {
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.TryCatch(
                    Expression.TryFault(
                        Expression.Divide(Expression.Constant(1), Expression.Constant(0)),
                        CSharpExpression.Await(yield)
                    ),
                    Expression.Catch(Expression.Parameter(typeof(DivideByZeroException)),
                        Expression.Constant(-1)
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(-1, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInFault3()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryFault(
                    Expression.Call(logExpr, add, Expression.Constant("T")),
                    Expression.Block(
                        Expression.Call(logExpr, add, Expression.Constant("FB")),
                        CSharpExpression.Await(yield),
                        Expression.Call(logExpr, add, Expression.Constant("FE"))
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            t.Wait();
            Assert.True(new[] { "T" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInFault4()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryCatch(
                    Expression.TryFault(
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("T")),
                            Expression.Throw(Expression.Constant(new Exception()))
                        ),
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("FB")),
                            CSharpExpression.Await(yield),
                            Expression.Call(logExpr, add, Expression.Constant("FE"))
                        )
                    ),
                    Expression.Catch(Expression.Parameter(typeof(Exception)),
                        Expression.Call(logExpr, add, Expression.Constant("C"))
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            t.Wait();
            Assert.True(new[] { "T", "FB", "FE", "C" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInFault5()
        {
            var p = Expression.Parameter(typeof(int));
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<int, Task<int>>>(
                Expression.TryFault(
                    Expression.Block(
                        Expression.Call(logExpr, add, Expression.Constant("T")),
                        Expression.Divide(Expression.Constant(42), p)
                    ),
                    Expression.Block(
                        Expression.Call(logExpr, add, Expression.Constant("FB")),
                        CSharpExpression.Await(yield),
                        Expression.Call(logExpr, add, Expression.Constant("FE"))
                    )
                ),
                p
            );
            var f = e.Compile();

            var t1 = f(1);
            Assert.Equal(42, t1.Result);
            Assert.True(new[] { "T" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_NestedFinally()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryFinally(
                    Expression.Block(
                        Expression.Call(logExpr, add, Expression.Constant("T1"))
                    ),
                    Expression.Block(
                        Expression.Call(logExpr, add, Expression.Constant("FB1")),
                         Expression.TryFinally(
                            Expression.Block(
                                Expression.Call(logExpr, add, Expression.Constant("T2"))
                            ),
                            Expression.Block(
                                Expression.Call(logExpr, add, Expression.Constant("FB2")),
                                CSharpExpression.Await(yield),
                                Expression.Call(logExpr, add, Expression.Constant("FE2"))
                            )
                        ),
                        Expression.Call(logExpr, add, Expression.Constant("FE1"))
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            t.Wait();
            Assert.True(new[] { "T1", "FB1", "T2", "FB2", "FE2", "FE1" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_UnpendBranch1()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var lbl = Expression.Label();
            var brk = Expression.Label();
            var cnt = Expression.Label();
            var lbm = Expression.Label();
            var e = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.Block(
                    Expression.TryFinally(
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("T")),
                            Expression.Goto(lbm),
                            Expression.Loop(Expression.Empty(), brk, cnt),
                            Expression.Label(lbm),
                            Expression.Goto(lbl)
                        ),
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("FB")),
                            CSharpExpression.Await(yield),
                            Expression.Call(logExpr, add, Expression.Constant("FE"))
                        )
                    ),
                    Expression.Call(logExpr, add, Expression.Constant("X")),
                    Expression.Label(lbl),
                    Expression.Call(logExpr, add, Expression.Constant("O"))
                )
            );
            var f = e.Compile();
            var t = f();
            t.Wait();
            Assert.True(new[] { "T", "FB", "FE", "O" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_UnpendBranch2()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var lbl = Expression.Label(typeof(int), "l");
            var brk = Expression.Label("b");
            var cnt = Expression.Label("c");
            var lbm = Expression.Label("m");
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.Block(
                    Expression.TryFinally(
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("T")),
                            Expression.Goto(lbm),
                            Expression.Loop(Expression.Empty(), brk, cnt),
                            Expression.Label(lbm),
                            Expression.Goto(lbl, Expression.Constant(42))
                        ),
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("FB")),
                            CSharpExpression.Await(yield),
                            Expression.Call(logExpr, add, Expression.Constant("FE"))
                        )
                    ),
                    Expression.Call(logExpr, add, Expression.Constant("X")),
                    Expression.Label(lbl, Expression.Constant(0))
                )
            );
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(42, r);
            Assert.True(new[] { "T", "FB", "FE" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_ReducibleNodes()
        {
            var fromResultMethod = MethodInfoOf(() => Task.FromResult(default(int)));
            var i = Expression.Parameter(typeof(int));
            var res = Expression.Parameter(typeof(int));
            var brk = Expression.Label();
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.Block(
                    new[] { i, res },
                    Expression.Assign(i, Expression.Constant(0)),
                    CSharpExpression.While(Expression.LessThan(i, Expression.Constant(10)),
                        Expression.Block(
                            Expression.AddAssign(
                                res,
                                CSharpExpression.Await(Expression.Call(fromResultMethod, i))
                            ),
                            Expression.PostIncrementAssign(i)
                        ), brk
                    ),
                    res
                )
            );
            var f = e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(Enumerable.Range(0, 10).Sum(), r);
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInCatch1()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryCatch(
                    Expression.Call(logExpr, add, Expression.Constant("T")),
                    Expression.Catch(
                        typeof(Exception),
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("CB")),
                            CSharpExpression.Await(yield),
                            Expression.Call(logExpr, add, Expression.Constant("CE"))
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            t.Wait();
            Assert.True(new[] { "T" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInCatch2()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryCatch(
                    Expression.Block(
                        typeof(void),
                        Expression.Call(logExpr, add, Expression.Constant("T")),
                        Expression.Divide(Expression.Constant(1), Expression.Constant(0))
                    ),
                    Expression.Catch(
                        typeof(DivideByZeroException),
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("CB")),
                            CSharpExpression.Await(yield),
                            Expression.Call(logExpr, add, Expression.Constant("CE"))
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            t.Wait();
            Assert.True(new[] { "T", "CB", "CE" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInCatch3()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryCatchFinally(
                    Expression.Block(
                        typeof(void),
                        Expression.Call(logExpr, add, Expression.Constant("T")),
                        Expression.Divide(Expression.Constant(1), Expression.Constant(0))
                    ),
                    Expression.Block(
                        Expression.Call(logExpr, add, Expression.Constant("F"))
                    ),
                    Expression.Catch(
                        typeof(DivideByZeroException),
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("CB")),
                            CSharpExpression.Await(yield),
                            Expression.Call(logExpr, add, Expression.Constant("CE"))
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            t.Wait();
            Assert.True(new[] { "T", "CB", "CE", "F" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInCatch4()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryCatch(
                    Expression.Block(
                        typeof(void),
                        Expression.Call(logExpr, add, Expression.Constant("T")),
                        Expression.Divide(Expression.Constant(1), Expression.Constant(0))
                    ),
                    Expression.Catch(
                        typeof(InvalidOperationException),
                        Expression.Call(logExpr, add, Expression.Constant("IO"))
                    ),
                    Expression.Catch(
                        typeof(DivideByZeroException),
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("CB")),
                            CSharpExpression.Await(yield),
                            Expression.Call(logExpr, add, Expression.Constant("CE"))
                        )
                    ),
                    Expression.Catch(
                        typeof(ArgumentNullException),
                        Expression.Call(logExpr, add, Expression.Constant("AN"))
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            t.Wait();
            Assert.True(new[] { "T", "CB", "CE" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInCatch5()
        {
            var ex = Expression.Parameter(typeof(DivideByZeroException));
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryCatch(
                    Expression.Block(
                        typeof(void),
                        Expression.Call(logExpr, add, Expression.Constant("T")),
                        Expression.Divide(Expression.Constant(1), Expression.Constant(0))
                    ),
                    Expression.Catch(
                        ex,
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("CB")),
                            Expression.Call(logExpr, add, Expression.Property(ex, "Message")),
                            CSharpExpression.Await(yield),
                            Expression.Call(logExpr, add, Expression.Property(ex, "Message")),
                            Expression.Call(logExpr, add, Expression.Constant("CE"))
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            t.Wait();
            var m = new DivideByZeroException().Message;
            Assert.True(new[] { "T", "CB", m, m, "CE" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInCatch6()
        {
            var ex = Expression.Parameter(typeof(DivideByZeroException));
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryCatch(
                    Expression.Block(
                        typeof(void),
                        Expression.Call(logExpr, add, Expression.Constant("T")),
                        Expression.Divide(Expression.Constant(1), Expression.Constant(0))
                    ),
                    Expression.Catch(
                        typeof(DivideByZeroException),
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("CB")),
                            CSharpExpression.Await(yield),
                            Expression.Rethrow(),
                            Expression.Call(logExpr, add, Expression.Constant("CE"))
                        )
                    ),
                    Expression.Catch(
                        ex,
                        Expression.Call(logExpr, add, Expression.Property(ex, "Message"))
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            t.Wait();
            var m = new DivideByZeroException().Message;
            Assert.True(new[] { "T", "CB", m }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInCatch7()
        {
            var p = Expression.Parameter(typeof(int));
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var yield = ((Expression<Func<YieldAwaitable>>)(() => Task.Yield())).Body;
            var e = CSharpExpression.AsyncLambda<Func<int, Task<int>>>(
                Expression.TryCatch(
                    Expression.Block(
                        Expression.Call(logExpr, add, Expression.Constant("T")),
                        Expression.Divide(Expression.Constant(42), p)
                    ),
                    Expression.Catch(
                        typeof(DivideByZeroException),
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("C")),
                            CSharpExpression.Await(yield),
                            Expression.Constant(-1)
                        )
                    )
                ),
                p
            );
            var f = e.Compile();

            var t1 = f(1);
            Assert.Equal(42, t1.Result);
            Assert.True(new[] { "T" }.SequenceEqual(log));

            log.Clear();

            var t2 = f(0);
            Assert.Equal(-1, t2.Result);
            Assert.True(new[] { "T", "C" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AwaitInCatch8()
        {
            foreach (var b in new[] { false, true })
            {
                var log = new List<string>();
                var logExpr = Expression.Constant(log);
                var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
                var yield = ((Expression<Func<A>>)(() => new A(b))).Body;
                var e = CSharpExpression.AsyncLambda<Func<Task>>(
                    Expression.TryCatchFinally(
                        Expression.Block(
                            typeof(void),
                            Expression.Call(logExpr, add, Expression.Constant("T")),
                            Expression.Divide(Expression.Constant(1), Expression.Constant(0))
                        ),
                        Expression.Block(
                            Expression.Call(logExpr, add, Expression.Constant("F"))
                        ),
                        Expression.Catch(
                            typeof(DivideByZeroException),
                            Expression.Block(
                                Expression.Call(logExpr, add, Expression.Constant("CB")),
                                CSharpExpression.Await(yield),
                                Expression.Call(logExpr, add, Expression.Constant("CE"))
                            )
                        )
                    )
                );
                var f = e.Compile();
                var t = f();
                t.Wait();
                Assert.True(new[] { "T", "CB", "CE", "F" }.SequenceEqual(log));
            }
        }

        [Fact]
        public void AsyncLambda_Compilation_NoStaticType()
        {
            var e = CSharpExpression.AsyncLambda(Expression.Constant(42));
            var f = (Func<Task<int>>)e.Compile();
            var t = f();
            var r = t.Result;
            Assert.Equal(42, r);
        }

        [Fact]
        public void AsyncLambda_Compilation_AndAlso_ShortCircuit_Boolean()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var run = MethodInfoOf(() => Task.Run(default(Func<bool>)));
            var e = CSharpExpression.AsyncLambda<Func<Task<bool>>>(
                Expression.AndAlso(
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<bool>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("L")),
                                    Expression.Constant(false)
                                )
                            )
                        )
                    ),
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<bool>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("R")),
                                    Expression.Constant(true)
                                )
                            )
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            Assert.False(t.Result);
            Assert.True(new[] { "L" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AndAlso_ShortCircuit_NullableBoolean()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var run = MethodInfoOf(() => Task.Run(default(Func<bool?>)));
            var e = CSharpExpression.AsyncLambda<Func<Task<bool?>>>(
                Expression.AndAlso(
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<bool?>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("L")),
                                    Expression.Constant(false, typeof(bool?))
                                )
                            )
                        )
                    ),
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<bool?>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("R")),
                                    Expression.Constant(true, typeof(bool?))
                                )
                            )
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            Assert.Equal(false, t.Result);
            Assert.True(new[] { "L" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_AndAlso_ShortCircuit_Struct()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var run = MethodInfoOf(() => Task.Run(default(Func<B>)));
            var e = CSharpExpression.AsyncLambda<Func<Task<B>>>(
                Expression.AndAlso(
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<B>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("L")),
                                    Expression.Constant(new B(false))
                                )
                            )
                        )
                    ),
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<B>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("R")),
                                    Expression.Constant(new B(true))
                                )
                            )
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            Assert.False((bool)t.Result);
            Assert.True(new[] { "L" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_OrElse_ShortCircuit_Boolean()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var run = MethodInfoOf(() => Task.Run(default(Func<bool>)));
            var e = CSharpExpression.AsyncLambda<Func<Task<bool>>>(
                Expression.OrElse(
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<bool>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("L")),
                                    Expression.Constant(true)
                                )
                            )
                        )
                    ),
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<bool>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("R")),
                                    Expression.Constant(false)
                                )
                            )
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            Assert.True(t.Result);
            Assert.True(new[] { "L" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_OrElse_ShortCircuit_NullableBoolean()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var run = MethodInfoOf(() => Task.Run(default(Func<bool?>)));
            var e = CSharpExpression.AsyncLambda<Func<Task<bool?>>>(
                Expression.OrElse(
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<bool?>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("L")),
                                    Expression.Constant(true, typeof(bool?))
                                )
                            )
                        )
                    ),
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<bool?>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("R")),
                                    Expression.Constant(false, typeof(bool?))
                                )
                            )
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            Assert.Equal(true, t.Result);
            Assert.True(new[] { "L" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_OrElse_ShortCircuit_Struct()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var run = MethodInfoOf(() => Task.Run(default(Func<B>)));
            var e = CSharpExpression.AsyncLambda<Func<Task<B>>>(
                Expression.OrElse(
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<B>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("L")),
                                    Expression.Constant(new B(true))
                                )
                            )
                        )
                    ),
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<B>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("R")),
                                    Expression.Constant(new B(false))
                                )
                            )
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            Assert.True((bool)t.Result);
            Assert.True(new[] { "L" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_Coalesce_ShortCircuit_String()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var run = MethodInfoOf(() => Task.Run(default(Func<string>)));
            var e = CSharpExpression.AsyncLambda<Func<Task<string>>>(
                Expression.Coalesce(
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<string>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("L")),
                                    Expression.Constant("A")
                                )
                            )
                        )
                    ),
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<string>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("R")),
                                    Expression.Constant("B")
                                )
                            )
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            Assert.Equal("A", t.Result);
            Assert.True(new[] { "L" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_Coalesce_ShortCircuit_NullableInt32_NullableInt32()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var run = MethodInfoOf(() => Task.Run(default(Func<int?>)));
            var e = CSharpExpression.AsyncLambda<Func<Task<int?>>>(
                Expression.Coalesce(
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<int?>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("L")),
                                    Expression.Constant(42, typeof(int?))
                                )
                            )
                        )
                    ),
                    CSharpExpression.Await(
                        Expression.Call(
                            run,
                            Expression.Lambda<Func<int?>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("R")),
                                    Expression.Constant(43, typeof(int?))
                                )
                            )
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            Assert.Equal(42, t.Result);
            Assert.True(new[] { "L" }.SequenceEqual(log));
        }

        [Fact]
        public void AsyncLambda_Compilation_Coalesce_ShortCircuit_NullableInt32_Int32()
        {
            var log = new List<string>();
            var logExpr = Expression.Constant(log);
            var add = MethodInfoOf((List<string> ss) => ss.Add(default(string)));
            var runN = MethodInfoOf(() => Task.Run(default(Func<int?>)));
            var runV = MethodInfoOf(() => Task.Run(default(Func<int>)));
            var e = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.Coalesce(
                    CSharpExpression.Await(
                        Expression.Call(
                            runN,
                            Expression.Lambda<Func<int?>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("L")),
                                    Expression.Constant(42, typeof(int?))
                                )
                            )
                        )
                    ),
                    CSharpExpression.Await(
                        Expression.Call(
                            runV,
                            Expression.Lambda<Func<int>>(
                                Expression.Block(
                                    Expression.Call(logExpr, add, Expression.Constant("R")),
                                    Expression.Constant(43, typeof(int))
                                )
                            )
                        )
                    )
                )
            );
            var f = e.Compile();
            var t = f();
            Assert.Equal(42, t.Result);
            Assert.True(new[] { "L" }.SequenceEqual(log));
        }

        class D : IDisposable
        {
            public bool IsDisposed;

            public int Do()
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("this");

                return 42;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        // TODO: make every test use those awaiters as well

        class A
        {
            private readonly bool _b;

            public A(bool b)
            {
                _b = b;
            }

            public W GetAwaiter() => new W(_b);
        }

        class W : INotifyCompletion
        {
            public W(bool b)
            {
                IsCompleted = b;
            }

            public bool IsCompleted { get; }
            public int GetResult() => 42;
            public void OnCompleted(Action continuation) => continuation();
        }

        struct B
        {
            private readonly bool _b;

            public B(bool b)
            {
                _b = b;
            }

            public static B operator &(B a, B b) => new B(a._b & b._b);
            public static B operator |(B a, B b) => new B(a._b | b._b);
            public static bool operator true(B a) => a._b;
            public static bool operator false(B a) => !a._b;
            public static explicit operator bool(B a) => a._b;
        }
    }
}
