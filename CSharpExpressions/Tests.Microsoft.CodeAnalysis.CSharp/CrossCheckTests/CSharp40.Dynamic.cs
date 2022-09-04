// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.CSharp.RuntimeBinder;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    // NB: The tests cross-check the outcome of evaluating a lambda expression - specified as a string in the
    //     test cases - in two ways. First, by converting the lambda expression to a delegate type and running
    //     IL code produced by the compiler. Second, by converting the lambda expression to an expression tree
    //     using the extended expression tree support in our modified Roslyn build and compiling the expression
    //     at runtime (therefore invoking our Reduce methods).
    //
    //     It is assumed that the outcome has proper equality defined (i.e. EqualityComparer<T>.Default should
    //     return a meaningful equality comparer to assert evaluation outcomes against each other). If the
    //     evaluation results in an exception, its type is cross-checked.
    //
    //     In addition to cross-checking the evaluation outcome, a log is maintained and cross-checked, which
    //     is useful to assert the order of side-effects. The code fragments can write to this log by means of
    //     the Log method and the Return method (to prepend returning a value of type T with a logging side-
    //     effect).

    partial class CompilerTests
    {
        // TODO: with compile-time constants
        // TODO: index, invoke, new with ref/out parameters

        #region Unary

        [Fact]
        public void CrossCheck_Dynamic_Unary_Negate()
        {
            var f = CrossCheck_Dynamic_UnaryCore("-");

            var values = Integers.Where(o => o.GetType() != typeof(ulong)).Concat(Floats);

            foreach (var value in values)
            {
                f(value);
            }

            f(TimeSpan.FromSeconds(42));
        }

        [Fact]
        public void CrossCheck_Dynamic_Unary_NegateChecked()
        {
            // NB: byte, sbyte, ushort, short widen to int
            // NB: negate of uint becomes long
            // NB: negate of ulong not supported

            AssertDynamicUnaryCheckedThrows<int>("-", int.MinValue);
            AssertDynamicUnaryCheckedThrows<long>("-", long.MinValue);
        }

        [Fact]
        public void CrossCheck_Dynamic_Unary_UnaryPlus()
        {
            var f = CrossCheck_Dynamic_UnaryCore("+");

            var values = Integers.Concat(Floats);

            foreach (var value in values)
            {
                f(value);
            }

            f(TimeSpan.FromSeconds(42));
        }

        [Fact]
        public void CrossCheck_Dynamic_Unary_OnesComplement()
        {
            var f = CrossCheck_Dynamic_UnaryCore("~");

            var values = Integers;

            foreach (var value in values)
            {
                f(value);
            }
        }

        [Fact]
        public void CrossCheck_Dynamic_Unary_Not()
        {
            var f = CrossCheck_Dynamic_UnaryCore("!");

            var values = Booleans;

            foreach (var value in values)
            {
                f(value);
            }
        }

        private void AssertDynamicUnaryCheckedThrows<T>(string op, T operand)
        {
            var f = CrossCheck_Dynamic_UnaryCore_Checked(op);

            AssertEx.Throws<OverflowException>(() => f(operand));
        }

        #endregion

        #region Binary

        [Fact]
        public void CrossCheck_Dynamic_Binary_Add()
        {
            var f = CrossCheck_Dynamic_BinaryCore("+");

            var ls = Integers.Concat(Floats);
            var rs = Integers2.Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(new DateTime(1983, 2, 11), TimeSpan.FromDays(32 * 365.25));
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(1));
            f(ConsoleColor.Red, 1);
            f("foo", "bar");
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_AddChecked()
        {
            // NB: byte, sbyte, ushort, short widen to int

            AssertDynamicBinaryCheckedThrows<int>("+", int.MaxValue, 1);
            AssertDynamicBinaryCheckedThrows<uint>("+", uint.MaxValue, 1);
            AssertDynamicBinaryCheckedThrows<long>("+", long.MaxValue, 1);
            AssertDynamicBinaryCheckedThrows<ulong>("+", ulong.MaxValue, 1);
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_Subtract()
        {
            var f = CrossCheck_Dynamic_BinaryCore("-");

            var ls = Integers.Concat(Floats);
            var rs = Integers2.Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(new DateTime(1983, 2, 11), TimeSpan.FromDays(32 * 365.25));
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(1));
            f(ConsoleColor.Red, 1);
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_SubtractChecked()
        {
            // NB: byte, sbyte, ushort, short widen to int

            AssertDynamicBinaryCheckedThrows<int>("-", int.MinValue, 1);
            AssertDynamicBinaryCheckedThrows<uint>("-", uint.MinValue, 1);
            AssertDynamicBinaryCheckedThrows<long>("-", long.MinValue, 1);
            AssertDynamicBinaryCheckedThrows<ulong>("-", ulong.MinValue, 1);
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_Multiply()
        {
            var f = CrossCheck_Dynamic_BinaryCore("*");

            var ls = Integers.Concat(Floats);
            var rs = Integers2.Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_MultiplyChecked()
        {
            // NB: byte, sbyte, ushort, short widen to int

            AssertDynamicBinaryCheckedThrows<int>("*", int.MaxValue, 2);
            AssertDynamicBinaryCheckedThrows<uint>("*", uint.MaxValue, 2);
            AssertDynamicBinaryCheckedThrows<long>("*", long.MaxValue, 2);
            AssertDynamicBinaryCheckedThrows<ulong>("*", ulong.MaxValue, 2);
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_Divide()
        {
            var f = CrossCheck_Dynamic_BinaryCore("/");

            var ls = Integers.Concat(Floats);
            var rs = Integers2.Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_Modulo()
        {
            var f = CrossCheck_Dynamic_BinaryCore("%");

            var ls = Integers.Concat(Floats);
            var rs = Integers2.Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_And()
        {
            var f = CrossCheck_Dynamic_BinaryCore("&");

            var ls = Integers.Concat(Booleans).Concat(Booleans);
            var rs = Integers2.Concat(Booleans).Concat(Booleans.Reverse());
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(BindingFlags.Public | BindingFlags.Instance, BindingFlags.Instance);
            f(BindingFlags.Public | BindingFlags.Instance, BindingFlags.Static);
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_Or()
        {
            var f = CrossCheck_Dynamic_BinaryCore("|");

            var ls = Integers.Concat(Booleans).Concat(Booleans);
            var rs = Integers2.Concat(Booleans).Concat(Booleans.Reverse());
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(BindingFlags.Public, BindingFlags.Instance);
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_ExclusiveOr()
        {
            var f = CrossCheck_Dynamic_BinaryCore("^");

            var ls = Integers.Concat(Booleans).Concat(Booleans);
            var rs = Integers2.Concat(Booleans).Concat(Booleans.Reverse());
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(BindingFlags.Public, BindingFlags.Instance);
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_AndAlso()
        {
            var f = CrossCheck_Dynamic_BinaryCore("&&");

            var ls = Booleans;
            var rs = Booleans;
            var values = from l in ls from r in rs select new { l, r };

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_OrElse()
        {
            var f = CrossCheck_Dynamic_BinaryCore("||");

            var ls = Booleans;
            var rs = Booleans;
            var values = from l in ls from r in rs select new { l, r };

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_LeftShift()
        {
            var f = CrossCheck_Dynamic_BinaryCore("<<");

            foreach (var s in new[] { 0, 1, 2, 3, 0x1F, 0x20 })
            {
                f((int)42, s);
                f((uint)42, s);
            }

            foreach (var s in new[] { 0, 1, 2, 3, 0x3F, 0x40 })
            {
                f((long)42, s);
                f((ulong)42, s);
            }
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_RightShift()
        {
            var f = CrossCheck_Dynamic_BinaryCore(">>");

            foreach (var s in new[] { 0, 1, 2, 3, 0x1F, 0x20 })
            {
                f((int)42, s);
                f((uint)42, s);
            }

            foreach (var s in new[] { 0, 1, 2, 3, 0x3F, 0x40 })
            {
                f((long)42, s);
                f((ulong)42, s);
            }
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_Equal()
        {
            var f = CrossCheck_Dynamic_BinaryCore("==");

            var ls = Integers.Concat(Integers).Concat(Floats).Concat(Floats).Concat(Booleans).Concat(Booleans);
            var rs = Integers.Concat(Integers2).Concat(Floats).Concat(Floats2).Concat(Booleans).Concat(Booleans.Reverse());
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            // TODO: null cases (doesn't work with Return<T> because T can't be inferred)

            f("bar", "bar");
            f("bar", "foo");
            f(TimeSpan.Zero, TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(42));
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_NotEqual()
        {
            var f = CrossCheck_Dynamic_BinaryCore("!=");

            var ls = Integers.Concat(Integers).Concat(Floats).Concat(Floats).Concat(Booleans).Concat(Booleans);
            var rs = Integers.Concat(Integers2).Concat(Floats).Concat(Floats2).Concat(Booleans).Concat(Booleans.Reverse());
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            // TODO: null cases (doesn't work with Return<T> because T can't be inferred)

            f("bar", "bar");
            f("bar", "foo");
            f(TimeSpan.Zero, TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(42));
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_LessThan()
        {
            var f = CrossCheck_Dynamic_BinaryCore("<");

            var ls = Integers.Concat(Integers).Concat(Floats).Concat(Floats);
            var rs = Integers.Concat(Integers2).Concat(Floats).Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(TimeSpan.Zero, TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(42));
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_LessThanOrEqual()
        {
            var f = CrossCheck_Dynamic_BinaryCore("<=");

            var ls = Integers.Concat(Integers).Concat(Floats).Concat(Floats);
            var rs = Integers.Concat(Integers2).Concat(Floats).Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(TimeSpan.Zero, TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(42));
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_GreaterThan()
        {
            var f = CrossCheck_Dynamic_BinaryCore(">");

            var ls = Integers.Concat(Integers).Concat(Floats).Concat(Floats);
            var rs = Integers.Concat(Integers2).Concat(Floats).Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(TimeSpan.Zero, TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(42));
        }

        [Fact]
        public void CrossCheck_Dynamic_Binary_GreaterThanOrEqual()
        {
            var f = CrossCheck_Dynamic_BinaryCore(">=");

            var ls = Integers.Concat(Integers).Concat(Floats).Concat(Floats);
            var rs = Integers.Concat(Integers2).Concat(Floats).Concat(Floats2);
            var values = ls.Zip(rs, (l, r) => new { l, r });

            foreach (var lr in values)
            {
                f(lr.l, lr.r);
            }

            f(TimeSpan.Zero, TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42), TimeSpan.FromSeconds(42));
        }

        private void AssertDynamicBinaryCheckedThrows<T>(string op, T left, T right)
        {
            var f = CrossCheck_Dynamic_BinaryCore_Checked(op);

            AssertEx.Throws<OverflowException>(() => f(left, right));
        }

        #endregion

        #region Member

        [Fact]
        public void CrossCheck_Dynamic_GetMember1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => d.Length");

            f("");
            f("bar");
            f(new int[0]);
            f(new int[42]);
        }

        [Fact]
        public void CrossCheck_Dynamic_GetMember2()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => d.Value");

            f(new StrongBox<int>());
            f(new StrongBox<int>(42));
        }

        [Fact]
        public void CrossCheck_Dynamic_GetMember3()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => d.Ticks");

            f(TimeSpan.Zero);
            f(TimeSpan.FromSeconds(42));
        }

        #endregion

        #region Index

        [Fact]
        public void CrossCheck_Dynamic_GetIndex1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => d[1]");

            f("bar");
            f(new int[2] { 41, 42 });
            f(new List<int> { 41, 42 });
            f(new Dictionary<int, string> { { 1, "one" } });

            AssertEx.Throws<IndexOutOfRangeException>(() => f(""));
            AssertEx.Throws<IndexOutOfRangeException>(() => f(new int[0]));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f(new List<int>()));
            AssertEx.Throws<KeyNotFoundException>(() => f(new Dictionary<int, string> { { 0, "zero" } }));
        }

        [Fact]
        public void CrossCheck_Dynamic_GetIndex2()
        {
            var f = Compile<Func<int[], dynamic, dynamic>>("(int[] xs, dynamic d) => xs[d]");

            f(new int[2] { 41, 42 }, 0);
            f(new int[2] { 41, 42 }, 1);

            AssertEx.Throws<IndexOutOfRangeException>(() => f(new int[0], 0));
        }

        [Fact]
        public void CrossCheck_Dynamic_GetIndex3()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic, dynamic>>("(dynamic d, dynamic e, dynamic f) => d[e, f]");

            f(new int[1, 1] { { 42 } }, 0, 0);
            f(new long[1, 2] { { 41, 42 } }, 0, 1);
            f(new double[2, 1] { { 41 }, { 42 } }, 1, 0);
            f(new string[2, 2] { { "40", "41" }, { "43", "42" } }, 1, 1);
        }

        [Fact]
        public void CrossCheck_Dynamic_GetIndex4()
        {
            var f = Compile<Func<int[,], dynamic, dynamic, dynamic>>("(int[,] xs, dynamic e, dynamic f) => xs[e, f]");

            f(new int[1, 1] { { 42 } }, 0, 0);
        }

        [Fact]
        public void CrossCheck_Dynamic_GetIndex5()
        {
            var f = Compile<Func<dynamic, int, int, dynamic>>("(dynamic d, int e, int f) => d[e, f]");

            f(new int[1, 1] { { 42 } }, 0, 0);
        }

        [Fact]
        public void CrossCheck_Dynamic_GetIndex_Named()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic>>("(dynamic xs, dynamic index) => xs[index: Return(index)]");

            f(new List<int> { 2, 3, 5 }, 1);
        }

        #endregion

        #region Invoke

        [Fact]
        public void CrossCheck_Dynamic_Invoke1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => d(42)");

            f(new Func<int, int>(x => x + 1));
            f(new Func<int, string>(x => x.ToString()));
            f(new Func<long, long>(x => x * 2L));

            AssertEx.Throws<RuntimeBinderException>(() => f(null));
        }

        [Fact]
        public void CrossCheck_Dynamic_Invoke2()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic>>("(dynamic d, dynamic e) => d(e)");

            f(new Func<int, int>(x => x + 1), 42);
            f(new Func<int, string>(x => x.ToString()), 42);
            f(new Func<long, long>(x => x * 2L), 42);

            f(new Func<string, string>(s => s.ToUpper()), "bar");
            f(new Func<string, string>(s => s ?? "null"), null);

            AssertEx.Throws<NullReferenceException>(() => f(new Func<string, string>(s => s.ToUpper()), null));
        }

        [Fact]
        public void CrossCheck_Dynamic_Invoke3()
        {
            var f = Compile<Func<Func<int, int>, dynamic, dynamic>>("(Func<int, int> f, dynamic d) => f(d)");

            f(new Func<int, int>(x => x + 1), 42);
            f(new Func<int, int>(x => x + 1), (byte)42);

            // REVIEW: RuntimeBinderException (C#) != NullReferenceException (ET)
            // AssertEx.Throws<NullReferenceException>(() => f(null, 42));
        }

        [Fact]
        public void CrossCheck_Dynamic_Invoke_Named()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic, dynamic, dynamic>>("(dynamic f, dynamic arg1, dynamic arg2, dynamic arg3) => f(arg2: Return(arg2), arg3: Return(arg3), arg1: Return(arg1))");

            f(new Func<int, int, int, int>((a, b, c) => a * b + c), 2, 3, 5);
        }

        #endregion

        #region InvokeMember

        [Fact]
        public void CrossCheck_Dynamic_InvokeMember_ToString()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => d.ToString()");

            f(new object());
            f(42);
            f(42L);
            f("foo");
            f(true);
            f(new DateTime(1983, 2, 11));
            f(ConsoleColor.Red);
        }

        [Fact]
        public void CrossCheck_Dynamic_InvokeMember_Static()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => int.Parse(d)");

            f("42");
            AssertEx.Throws<FormatException>(() => f("bar"));
        }

        [Fact]
        public void CrossCheck_Dynamic_InvokeMember_Out()
        {
            var f = Compile<Action<dynamic>>(@"(dynamic d) => {
    int res;
    bool b = /* dynamic convert */ int.TryParse(d, out res);

    Log(b);
    Log(res);
}");

            f("42");
            f("bar");
        }

        [Fact]
        public void CrossCheck_Dynamic_InvokeMember_Ref1()
        {
            var f = Compile<Action<dynamic>>(@"(dynamic d) => {
    int value = 41;
    int res = /* dynamic convert */ System.Threading.Interlocked.Exchange(ref value, d);

    Log(value);
    Log(res);
}");

            f(42);
        }

        [Fact]
        public void CrossCheck_Dynamic_InvokeMember_Ref2()
        {
            var f = Compile<Action<dynamic, dynamic>>(@"(dynamic location, dynamic value) => {
    var res = System.Threading.Interlocked.Exchange(ref location, value);

    Log(location);
    Log(res);
}");

            f(41, 42);
        }

        [Fact]
        public void CrossCheck_Dynamic_InvokeMember_Overloads()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => Math.Abs(d)");

            f(-42);
            f(-42L);
            f(-42.0);
            f(-42.0m);
        }

        [Fact]
        public void CrossCheck_Dynamic_InvokeMember_Static_Generic()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic d) => Utils.DynamicInvokeWithGeneric<double>(new Func<string, string>(Log), d)");

            f(42);
            f(true);
            f("bar");
        }

        [Fact]
        public void CrossCheck_Dynamic_InvokeMember_Instance_Generic()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic>>("(dynamic d, dynamic x) => d.Return<int>(x)");

            f(new DynamicInvoker(), 42);
        }

        [Fact]
        public void CrossCheck_Dynamic_InvokeMember_Named()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic, dynamic>>("(dynamic s, dynamic startIndex, dynamic length) => s.Substring(length: Return(length), startIndex: Return(startIndex))");

            f("foobar", 2, 3);
        }

        #endregion

        #region InvokeConstructor

        [Fact]
        public void CrossCheck_Dynamic_InvokeConstructor1()
        {
            var f = Compile<Func<dynamic, TimeSpan>>("(dynamic d) => new TimeSpan(d)");

            f(42);
            f(42L);
        }

        [Fact]
        public void CrossCheck_Dynamic_InvokeConstructor2()
        {
            var f = Compile<Func<dynamic, dynamic, DateTimeOffset>>("(dynamic d1, dynamic d2) => new DateTimeOffset(d1, d2)");

            var dt = new DateTime(1983, 2, 11);

            f(dt, TimeSpan.FromHours(1));
            f(dt.Ticks, TimeSpan.FromHours(1));
        }

        [Fact]
        public void CrossCheck_Dynamic_InvokeConstructor_Named()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic, TimeSpan>>("(dynamic hours, dynamic minutes, dynamic seconds) => new TimeSpan(minutes: Return(minutes), hours: Return(hours), seconds: Return(seconds))");

            f(2, 3, 5);
        }

        #endregion

        #region Convert

        [Fact]
        public void CrossCheck_Dynamic_Convert1()
        {
            var f = Compile<Func<dynamic, DateTimeOffset>>("(dynamic d) => (DateTimeOffset)d");

            f(new DateTime(1983, 2, 11));
            f(new DateTimeOffset(new DateTime(1983, 2, 11), TimeSpan.FromHours(1)));
        }

        [Fact]
        public void CrossCheck_Dynamic_Convert2()
        {
            var f = Compile<Func<dynamic, int>>("(dynamic d) => checked((int)d)");

            f(42);
            f(42L);
            AssertEx.Throws<OverflowException>(() => f(int.MaxValue + 1L));
        }

        #endregion

        #region Assignment

        [Fact]
        public void CrossCheck_Dynamic_Assign1()
        {
            var f = Compile<Action<dynamic>>("(dynamic d) => { Log<object>(d); var e = d; Log<object>(e); }");

            f(1);
            f("bar");
            f(null);
        }

        [Fact]
        public void CrossCheck_Dynamic_Assign2()
        {
            var f = Compile<Action<int>>("(int x) => { Log(x); dynamic d = x; Log(d); }");

            f(42);
        }

        [Fact]
        public void CrossCheck_Dynamic_Assign3()
        {
            var f = Compile<Action<dynamic>>("(dynamic d) => { Log(d); int x = d; Log(x); }"); // also has convert

            f(42);
        }

        #endregion

        #region Compound assignment

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_Variable1()
        {
            var f = Compile<Func<dynamic, dynamic, dynamic>>("(dynamic d, dynamic e) => { Log(d += Return(e)); return d; }");

            f(41, 1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_Variable2()
        {
            var f = Compile<Func<int, dynamic, dynamic>>("(int d, dynamic e) => { Log(d += Return(e)); return d; }");

            f(41, 1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_Variable3()
        {
            var f = Compile<Func<dynamic, int, dynamic>>("(dynamic d, int e) => { Log(d += Return(e)); return d; }");

            f(41, 1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_ArrayIndex1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { dynamic d = new[] { 41 }; Log(d[Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_ArrayIndex2()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { int[] d = new[] { 41 }; Log(d[Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_ArrayIndex3()
        {
            var f = Compile<Func<int, dynamic>>("(int e) => { dynamic d = new[] { 41 }; Log(d[Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_Index1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { dynamic d = new List<int> { 41 }; Log(d[Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_Index2()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { List<int> d = new List<int> { 41 }; Log(d[Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_Index3()
        {
            var f = Compile<Func<int, dynamic>>("(int e) => { dynamic d = new List<int> { 41 }; Log(d[Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_CSharpIndex1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { dynamic d = new List<int> { 41 }; Log(d[index: Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_CSharpIndex2()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { List<int> d = new List<int> { 41 }; Log(d[index: Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_CSharpIndex3()
        {
            var f = Compile<Func<int, dynamic>>("(int e) => { dynamic d = new List<int> { 41 }; Log(d[index: Return(0)] += Return(e)); return d[0]; }");

            f(1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_Member1()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { dynamic d = new StrongBox<int> { Value = 41 }; Log(d.Value += Return(e)); return d.Value; }");

            f(1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_Member2()
        {
            var f = Compile<Func<dynamic, dynamic>>("(dynamic e) => { StrongBox<int> d = new StrongBox<int> { Value = 41 }; Log(d.Value += Return(e)); return d.Value; }");

            f(1);
        }

        [Fact]
        public void CrossCheck_Dynamic_CompoundAssign_Member3()
        {
            var f = Compile<Func<int, dynamic>>("(int e) => { dynamic d = new StrongBox<int> { Value = 41 }; Log(d.Value += Return(e)); return d.Value; }");

            f(1);
        }

        // TODO: all operator kinds
        // TODO: checked variations
        // TODO: event handlers

        #endregion

        #region Unary increment/decrement

        // TODO
        // TODO: checked variations

        #endregion

        private Func<dynamic, dynamic> CrossCheck_Dynamic_UnaryCore(string op)
        {
            return Compile<Func<dynamic, dynamic>>($"(dynamic d) => {op}Return(d)");
        }

        private Func<dynamic, dynamic> CrossCheck_Dynamic_UnaryCore_Checked(string op)
        {
            return Compile<Func<dynamic, dynamic>>($"(dynamic d) => checked({op}Return(d))");
        }

        private Func<dynamic, dynamic, dynamic> CrossCheck_Dynamic_BinaryCore(string op)
        {
            return Compile<Func<dynamic, dynamic, dynamic>>($"(dynamic l, dynamic r) => Return(l) {op} Return(r)");
        }

        private Func<dynamic, dynamic, dynamic> CrossCheck_Dynamic_BinaryCore_Checked(string op)
        {
            return Compile<Func<dynamic, dynamic, dynamic>>($"(dynamic l, dynamic r) => checked(Return(l) {op} Return(r))");
        }

        private readonly IEnumerable<object> Integers = new object[]
        {
            (byte)42,
            (sbyte)42,
            (ushort)42,
            (short)42,
            (uint)42,
            (int)42,
            (ulong)42,
            (long)42,
        };

        private readonly IEnumerable<object> Integers2 = new object[]
        {
            (byte)3,
            (sbyte)3,
            (ushort)3,
            (short)3,
            (uint)3,
            (int)3,
            (ulong)3,
            (long)3,
        };

        private readonly IEnumerable<object> Floats = new object[]
        {
            (float)3.14,
            (double)3.14,
            (decimal)3.14
        };

        private readonly IEnumerable<object> Floats2 = new object[]
        {
            (float)2.72,
            (double)2.72,
            (decimal)2.72
        };

        private readonly IEnumerable<object> Booleans = new object[]
        {
            false,
            true,
        };

        // TODO: should we add char cases as well?
    }
}
