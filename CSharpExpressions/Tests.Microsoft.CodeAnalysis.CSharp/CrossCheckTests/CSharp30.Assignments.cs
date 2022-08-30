// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Xunit;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
        #region Assignment

        [Fact]
        public void CrossCheck_Assignment_Primitives()
        {
            CrossCheck_Assignment_Core<byte>()(42);
            CrossCheck_Assignment_Core<sbyte>()(42);
            CrossCheck_Assignment_Core<ushort>()(42);
            CrossCheck_Assignment_Core<short>()(42);
            CrossCheck_Assignment_Core<uint>()(42);
            CrossCheck_Assignment_Core<int>()(42);
            CrossCheck_Assignment_Core<ulong>()(42UL);
            CrossCheck_Assignment_Core<long>()(42L);
            CrossCheck_Assignment_Core<float>()(42.0f);
            CrossCheck_Assignment_Core<double>()(42.0d);
            CrossCheck_Assignment_Core<decimal>()(42.0m);
            CrossCheck_Assignment_Core<bool>()(true);
            CrossCheck_Assignment_Core<char>()('a');
            CrossCheck_Assignment_Core<string>()("bar");
        }

        [Fact]
        public void CrossCheck_Assignment_More()
        {
            CrossCheck_Assignment_Core<byte?>()(42);
            CrossCheck_Assignment_Core<byte?>()(null);
            CrossCheck_Assignment_Core<ConsoleColor>()(ConsoleColor.Red);
            CrossCheck_Assignment_Core<ConsoleColor?>()(ConsoleColor.Red);
        }

        private Func<T, T> CrossCheck_Assignment_Core<T>()
        {
            var t = typeof(T).ToCSharp();

            var f1 = Compile<Func<T, T>>("x => { var y = x; return y; }");
            var f2 = Compile<Func<T, T>>($"x => {{ var b = new StrongBox<{t}>(); b.Value = x; return b.Value; }}");
            var f3 = Compile<Func<T, T>>($"x => {{ var b = new WeakBox<{t}>(); b.Value = x; return b.Value; }}");
            var f4 = Compile<Func<T, T>>($"x => {{ var a = new {t}[1]; a[0] = x; return a[0]; }}");
            var f5 = Compile<Func<T, T>>($"x => {{ var a = new {t}[1, 1]; a[0, 0] = x; return a[0, 0]; }}");
            var f6 = Compile<Func<T, T>>($"x => {{ var l = new List<{t}> {{ default({t}) }}; l[0] = x; return l[0]; }}");
            var f7 = Compile<Func<T, T>>($"x => {{ var l = new List<{t}> {{ default({t}) }}; l[index: 0] = x; return l[index: 0]; }}");
            var f8 = Compile<Func<T, T>>($"x => {{ var b = new WeakBox<{t}>(); b[0] = x; return b[0]; }}");

            return f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8;
        }

        #endregion

        #region Compound assignment

        // TODO: compound with conversions
        // TODO: compound with char

        [Fact]
        public void CrossCheck_CompoundAssignment_Integral()
        {
            Parallel.ForEach(new[] { "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=" }, op =>
            {
                CrossCheck_CompoundAssignment_Core<byte>(op)(43, 3);
                CrossCheck_CompoundAssignment_Core<sbyte>(op)(43, 3);
                CrossCheck_CompoundAssignment_Core<ushort>(op)(43, 3);
                CrossCheck_CompoundAssignment_Core<short>(op)(43, 3);
                CrossCheck_CompoundAssignment_Core<uint>(op)(43, 3);
                CrossCheck_CompoundAssignment_Core<int>(op)(43, 3);
                CrossCheck_CompoundAssignment_Core<ulong>(op)(43UL, 3);
                CrossCheck_CompoundAssignment_Core<long>(op)(43L, 3);
            });
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_Integral_Nullable()
        {
            Parallel.ForEach(new[] { "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=" }, op =>
            {
                var f1 = CrossCheck_CompoundAssignment_Core<byte?>(op);
                var f2 = CrossCheck_CompoundAssignment_Core<sbyte?>(op);
                var f3 = CrossCheck_CompoundAssignment_Core<ushort?>(op);
                var f4 = CrossCheck_CompoundAssignment_Core<short?>(op);
                var f5 = CrossCheck_CompoundAssignment_Core<uint?>(op);
                var f6 = CrossCheck_CompoundAssignment_Core<int?>(op);
                var f7 = CrossCheck_CompoundAssignment_Core<ulong?>(op);
                var f8 = CrossCheck_CompoundAssignment_Core<long?>(op);

                f1(43, 3);
                f2(43, 3);
                f3(43, 3);
                f4(43, 3);
                f5(43, 3);
                f6(43, 3);
                f7(43, 3);
                f8(43, 3);

                f1(43, null);
                f2(43, null);
                f3(43, null);
                f4(43, null);
                f5(43, null);
                f6(43, null);
                f7(43, null);
                f8(43, null);

                f1(null, 3);
                f2(null, 3);
                f3(null, 3);
                f4(null, 3);
                f5(null, 3);
                f6(null, 3);
                f7(null, 3);
                f8(null, 3);
            });
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_Integral_Overflow()
        {
            AssertCheckedThrows<byte>("+=", byte.MaxValue, 1);
            AssertCheckedThrows<sbyte>("+=", sbyte.MaxValue, 1);
            AssertCheckedThrows<ushort>("+=", ushort.MaxValue, 1);
            AssertCheckedThrows<short>("+=", short.MaxValue, 1);
            AssertCheckedThrows<uint>("+=", uint.MaxValue, 1);
            AssertCheckedThrows<int>("+=", int.MaxValue, 1);
            AssertCheckedThrows<ulong>("+=", ulong.MaxValue, 1);
            AssertCheckedThrows<long>("+=", long.MaxValue, 1);

            AssertCheckedThrows<byte>("-=", byte.MinValue, 1);
            AssertCheckedThrows<sbyte>("-=", sbyte.MinValue, 1);
            AssertCheckedThrows<ushort>("-=", ushort.MinValue, 1);
            AssertCheckedThrows<short>("-=", short.MinValue, 1);
            AssertCheckedThrows<uint>("-=", uint.MinValue, 1);
            AssertCheckedThrows<int>("-=", int.MinValue, 1);
            AssertCheckedThrows<ulong>("-=", ulong.MinValue, 1);
            AssertCheckedThrows<long>("-=", long.MinValue, 1);

            AssertCheckedThrows<byte>("*=", byte.MaxValue, 2);
            AssertCheckedThrows<sbyte>("*=", sbyte.MaxValue, 2);
            AssertCheckedThrows<ushort>("*=", ushort.MaxValue, 2);
            AssertCheckedThrows<short>("*=", short.MaxValue, 2);
            AssertCheckedThrows<uint>("*=", uint.MaxValue, 2);
            AssertCheckedThrows<int>("*=", int.MaxValue, 2);
            AssertCheckedThrows<ulong>("*=", ulong.MaxValue, 2);
            AssertCheckedThrows<long>("*=", long.MaxValue, 2);
        }

        private void AssertCheckedThrows<T>(string op, T value, T rhs)
        {
            AssertEx.Throws<OverflowException>(() => CrossCheck_CompoundAssignment_Checked_Core<T>(op)(value, rhs));
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_Float()
        {
            Parallel.ForEach(new[] { "+=", "-=", "*=", "/=", "%=" }, op =>
            {
                CrossCheck_CompoundAssignment_Core<float>(op)(42.0f, 3);
                CrossCheck_CompoundAssignment_Core<double>(op)(42.0d, 3);
                CrossCheck_CompoundAssignment_Core<decimal>(op)(42.0m, 3);
            });
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_Boolean()
        {
            Parallel.ForEach(new[] { "&=", "|=", "^=" }, op =>
            {
                var f = CrossCheck_CompoundAssignment_Core<bool>(op);

                foreach (var b1 in new[] { false, true })
                {
                    foreach (var b2 in new[] { false, true })
                    {
                        f(b1, b2);
                    }
                }
            });
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_Shift()
        {
            Parallel.ForEach(new[] { "<<=", ">>=" }, op =>
            {
                CrossCheck_CompoundAssignment_Core<int, int>(op)(42, 1);
                CrossCheck_CompoundAssignment_Core<uint, int>(op)(42, 1);
                CrossCheck_CompoundAssignment_Core<long, int>(op)(42, 1);
                CrossCheck_CompoundAssignment_Core<ulong, int>(op)(42, 1);
            });
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_String1()
        {
            var f = Compile<Func<string, string, string>>("(s, t) => { Log(s += t); return s; }");

            f("foo", "bar");
            f("", "bar");
            f("foo", "");
            f(null, "bar");
            f("foo", null);
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_String2()
        {
            var f = Compile<Func<string, object, string>>("(s, t) => { Log(s += t); return s; }");

            f("foo", "bar");
            f("foo", 42);
            f("foo", true);
            f("foo", null);
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_Delegate()
        {
            var f = Compile<Action>(@"() => {
    Action a = () => { Log(""a""); };
    Action b = () => { Log(""b""); };
    a += b;
    a();
    a -= b;
    a();
}");
            f();
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_Enum_Add()
        {
            var f = Compile<Func<ConsoleColor, int, ConsoleColor>>("(e, u) => { Log(e += u); return e; }");

            f(ConsoleColor.Red, 1);
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_Enum_Subtract1()
        {
            var f = Compile<Func<ConsoleColor, int, ConsoleColor>>("(e, u) => { Log(e -= u); return e; }");

            f(ConsoleColor.Red, 1);
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_Enum_Subtract2()
        {
            var f = Compile<Func<ConsoleColor, ConsoleColor, ConsoleColor>>("(e1, e2) => { Log(e1 -= e2); return e1; }");

            f(ConsoleColor.Red, ConsoleColor.Blue);
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_Custom()
        {
            var f = Compile<Func<DateTime, TimeSpan, DateTime>>("(dt, ts) => { Log(dt += ts); return dt; }");

            f(new DateTime(1983, 2, 11), TimeSpan.FromHours(1));
        }

        [Fact]
        public void CrossCheck_CompoundAssignment_Custom_Nullable()
        {
            var f = Compile<Func<DateTime?, TimeSpan?, DateTime?>>("(dt, ts) => { Log(dt += ts); return dt; }");

            f(new DateTime(1983, 2, 11), TimeSpan.FromHours(1));
            f(new DateTime(1983, 2, 11), null);
            f(null, TimeSpan.FromHours(1));
            f(null, null);
        }

        [Fact]
        public void CrossCheck_CompoundAssignment()
        {
            var f = Compile<Func<int, int>>(@"i =>
{
    var b = new StrongBox<int>(i);
    Log(b.Value);
    var res = b.Value += Return(1);
    Log(res);
    return b.Value;
}");
            f(0);
            f(41);
        }

        [Fact] // See https://github.com/dotnet/corefx/issues/4984 for a relevant discussion
        public void CrossCheck_Issue4984_Binary_Repro1()
        {
            var f = Compile<Func<int, int>>(@"i =>
{
    var b = new WeakBox<int>();
    Log(b.Value);
    var res = b.Value += Return(1);
    Log(res);
    return b.Value;
}");
            f(0);
            f(41);
        }

        [Fact] // See https://github.com/dotnet/corefx/issues/4984 for a relevant discussion
        public void CrossCheck_Issue4984_Binary_Repro2()
        {
            var f = Compile<Func<int, int>>(@"i =>
{
    var b = new WeakBox<int>();
    Log(b[0]);
    var res = b[0] += Return(1);
    Log(res);
    return b[0];
}");
            f(0);
            f(41);
        }

        private Func<T, T, string> CrossCheck_CompoundAssignment_Core<T>(string op)
        {
            var t = typeof(T).ToCSharp();

            var f1 = Compile<Func<T, T, string>>($"(x, r) => {{ var y = x; var z = Log(y {op} r); return $\"({{y}},{{z}})\"; }}");
            var f2 = Compile<Func<T, T, string>>($"(x, r) => {{ var b = new StrongBox<{t}>(); var z = Log(b.Value {op} r); return $\"({{b.Value}},{{z}})\"; }}");
            var f3 = Compile<Func<T, T, string>>($"(x, r) => {{ var b = new WeakBox<{t}>(); var z = Log(b.Value {op} r); return $\"({{b.Value}},{{z}})\"; }}");
            var f4 = Compile<Func<T, T, string>>($"(x, r) => {{ var a = new {t}[1]; a[0] = x; var z = Log(a[0] {op} r); return $\"({{a[0]}},{{z}})\"; }}");
            var f5 = Compile<Func<T, T, string>>($"(x, r) => {{ var a = new {t}[1, 1]; a[0, 0] = x; var z = Log(a[0, 0] {op} r); return $\"({{a[0, 0]}},{{z}})\"; }}");
            var f6 = Compile<Func<T, T, string>>($"(x, r) => {{ var l = new List<{t}> {{ default({t}) }}; l[0] = x; var z = Log(l[0] {op} r); return $\"({{l[0]}},{{z}})\"; }}");
            var f7 = Compile<Func<T, T, string>>($"(x, r) => {{ var l = new List<{t}> {{ default({t}) }}; l[index: 0] = x; var z = Log(l[index: 0] {op} r); return $\"({{l[index: 0]}},{{z}})\"; }}");
            var f8 = Compile<Func<T, T, string>>($"(x, r) => {{ var b = new WeakBox<{t}>(); var z = Log(b[0] {op} r); return $\"({{b[0]}},{{z}})\"; }}");

            return f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8;
        }

        private Func<T, T, string> CrossCheck_CompoundAssignment_Checked_Core<T>(string op)
        {
            var t = typeof(T).ToCSharp();

            var f1 = Compile<Func<T, T, string>>($"(x, r) => {{ var y = x; var z = Log(checked(y {op} r)); return $\"({{y}},{{z}})\"; }}");
            var f2 = Compile<Func<T, T, string>>($"(x, r) => {{ var b = new StrongBox<{t}>(); var z = Log(checked(b.Value {op} r)); return $\"({{b.Value}},{{z}})\"; }}");
            var f3 = Compile<Func<T, T, string>>($"(x, r) => {{ var b = new WeakBox<{t}>(); var z = Log(checked(b.Value {op} r)); return $\"({{b.Value}},{{z}})\"; }}");
            var f4 = Compile<Func<T, T, string>>($"(x, r) => {{ var a = new {t}[1]; a[0] = x; var z = Log(checked(a[0] {op} r)); return $\"({{a[0]}},{{z}})\"; }}");
            var f5 = Compile<Func<T, T, string>>($"(x, r) => {{ var a = new {t}[1, 1]; a[0, 0] = x; var z = Log(checked(a[0, 0] {op} r)); return $\"({{a[0, 0]}},{{z}})\"; }}");
            var f6 = Compile<Func<T, T, string>>($"(x, r) => {{ var l = new List<{t}> {{ default({t}) }}; l[0] = x; var z = Log(checked(l[0] {op} r)); return $\"({{l[0]}},{{z}})\"; }}");
            var f7 = Compile<Func<T, T, string>>($"(x, r) => {{ var l = new List<{t}> {{ default({t}) }}; l[index: 0] = x; var z = Log(checked(l[index: 0] {op} r)); return $\"({{l[index: 0]}},{{z}})\"; }}");
            var f8 = Compile<Func<T, T, string>>($"(x, r) => {{ var b = new WeakBox<{t}>(); var z = Log(checked(b[0] {op} r)); return $\"({{b[0]}},{{z}})\"; }}");

            return f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8;
        }

        private Func<T, R, string> CrossCheck_CompoundAssignment_Core<T, R>(string op)
        {
            var t = typeof(T).ToCSharp();

            var f1 = Compile<Func<T, R, string>>($"(x, r) => {{ var y = x; var z = Log(y {op} r); return $\"({{y}},{{z}})\"; }}");
            var f2 = Compile<Func<T, R, string>>($"(x, r) => {{ var b = new StrongBox<{t}>(); var z = Log(b.Value {op} r); return $\"({{b.Value}},{{z}})\"; }}");
            var f3 = Compile<Func<T, R, string>>($"(x, r) => {{ var b = new WeakBox<{t}>(); var z = Log(b.Value {op} r); return $\"({{b.Value}},{{z}})\"; }}");
            var f4 = Compile<Func<T, R, string>>($"(x, r) => {{ var a = new {t}[1]; a[0] = x; var z = Log(a[0] {op} r); return $\"({{a[0]}},{{z}})\"; }}");
            var f5 = Compile<Func<T, R, string>>($"(x, r) => {{ var a = new {t}[1, 1]; a[0, 0] = x; var z = Log(a[0, 0] {op} r); return $\"({{a[0, 0]}},{{z}})\"; }}");
            var f6 = Compile<Func<T, R, string>>($"(x, r) => {{ var l = new List<{t}> {{ default({t}) }}; l[0] = x; var z = Log(l[0] {op} r); return $\"({{l[0]}},{{z}})\"; }}");
            var f7 = Compile<Func<T, R, string>>($"(x, r) => {{ var l = new List<{t}> {{ default({t}) }}; l[index: 0] = x; var z = Log(l[index: 0] {op} r); return $\"({{l[index: 0]}},{{z}})\"; }}");
            var f8 = Compile<Func<T, R, string>>($"(x, r) => {{ var b = new WeakBox<{t}>(); var z = Log(b[0] {op} r); return $\"({{b[0]}},{{z}})\"; }}");

            return f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8;
        }

        #endregion

        #region Unary increment/decrement

        // TODO: conversions

        [Fact]
        public void CrossCheck_UnaryAssignment_Integral()
        {
            Parallel.ForEach(new[] { "++", "--" }, op =>
            {
                CrossCheck_UnaryPreAssignment_Core<byte>(op)(43);
                CrossCheck_UnaryPreAssignment_Core<sbyte>(op)(43);
                CrossCheck_UnaryPreAssignment_Core<ushort>(op)(43);
                CrossCheck_UnaryPreAssignment_Core<short>(op)(43);
                CrossCheck_UnaryPreAssignment_Core<uint>(op)(43);
                CrossCheck_UnaryPreAssignment_Core<int>(op)(43);
                CrossCheck_UnaryPreAssignment_Core<ulong>(op)(43UL);
                CrossCheck_UnaryPreAssignment_Core<long>(op)(43L);
                CrossCheck_UnaryPreAssignment_Core<ConsoleColor>(op)(ConsoleColor.Red);

                CrossCheck_UnaryPostAssignment_Core<byte>(op)(43);
                CrossCheck_UnaryPostAssignment_Core<sbyte>(op)(43);
                CrossCheck_UnaryPostAssignment_Core<ushort>(op)(43);
                CrossCheck_UnaryPostAssignment_Core<short>(op)(43);
                CrossCheck_UnaryPostAssignment_Core<uint>(op)(43);
                CrossCheck_UnaryPostAssignment_Core<int>(op)(43);
                CrossCheck_UnaryPostAssignment_Core<ulong>(op)(43UL);
                CrossCheck_UnaryPostAssignment_Core<long>(op)(43L);
                CrossCheck_UnaryPostAssignment_Core<ConsoleColor>(op)(ConsoleColor.Red);
            });
        }

        [Fact]
        public void CrossCheck_UnaryAssignment_Integral_Nullable()
        {
            Parallel.ForEach(new[] { "++", "--" }, op =>
            {
                {
                    var f1 = CrossCheck_UnaryPreAssignment_Core<byte?>(op);
                    var f2 = CrossCheck_UnaryPreAssignment_Core<sbyte?>(op);
                    var f3 = CrossCheck_UnaryPreAssignment_Core<ushort?>(op);
                    var f4 = CrossCheck_UnaryPreAssignment_Core<short?>(op);
                    var f5 = CrossCheck_UnaryPreAssignment_Core<uint?>(op);
                    var f6 = CrossCheck_UnaryPreAssignment_Core<int?>(op);
                    var f7 = CrossCheck_UnaryPreAssignment_Core<ulong?>(op);
                    var f8 = CrossCheck_UnaryPreAssignment_Core<long?>(op);
                    var f9 = CrossCheck_UnaryPreAssignment_Core<ConsoleColor?>(op);

                    f1(43);
                    f2(43);
                    f3(43);
                    f4(43);
                    f5(43);
                    f6(43);
                    f7(43);
                    f8(43);
                    f9(ConsoleColor.Red);

                    f1(null);
                    f2(null);
                    f3(null);
                    f4(null);
                    f5(null);
                    f6(null);
                    f7(null);
                    f8(null);
                    f9(null);
                }

                {
                    var f1 = CrossCheck_UnaryPostAssignment_Core<byte?>(op);
                    var f2 = CrossCheck_UnaryPostAssignment_Core<sbyte?>(op);
                    var f3 = CrossCheck_UnaryPostAssignment_Core<ushort?>(op);
                    var f4 = CrossCheck_UnaryPostAssignment_Core<short?>(op);
                    var f5 = CrossCheck_UnaryPostAssignment_Core<uint?>(op);
                    var f6 = CrossCheck_UnaryPostAssignment_Core<int?>(op);
                    var f7 = CrossCheck_UnaryPostAssignment_Core<ulong?>(op);
                    var f8 = CrossCheck_UnaryPostAssignment_Core<long?>(op);
                    var f9 = CrossCheck_UnaryPostAssignment_Core<ConsoleColor?>(op);

                    f1(43);
                    f2(43);
                    f3(43);
                    f4(43);
                    f5(43);
                    f6(43);
                    f7(43);
                    f8(43);
                    f9(ConsoleColor.Red);

                    f1(null);
                    f2(null);
                    f3(null);
                    f4(null);
                    f5(null);
                    f6(null);
                    f7(null);
                    f8(null);
                    f9(null);
                }
            });
        }

        [Fact]
        public void CrossCheck_UnaryAssignment_Integral_Overflow()
        {
            AssertCheckedThrows<byte>("++", byte.MaxValue);
            AssertCheckedThrows<sbyte>("++", sbyte.MaxValue);
            AssertCheckedThrows<ushort>("++", ushort.MaxValue);
            AssertCheckedThrows<short>("++", short.MaxValue);
            AssertCheckedThrows<uint>("++", uint.MaxValue);
            AssertCheckedThrows<int>("++", int.MaxValue);
            AssertCheckedThrows<ulong>("++", ulong.MaxValue);
            AssertCheckedThrows<long>("++", long.MaxValue);

            AssertCheckedThrows<byte>("--", byte.MinValue);
            AssertCheckedThrows<sbyte>("--", sbyte.MinValue);
            AssertCheckedThrows<ushort>("--", ushort.MinValue);
            AssertCheckedThrows<short>("--", short.MinValue);
            AssertCheckedThrows<uint>("--", uint.MinValue);
            AssertCheckedThrows<int>("--", int.MinValue);
            AssertCheckedThrows<ulong>("--", ulong.MinValue);
            AssertCheckedThrows<long>("--", long.MinValue);
        }

        private void AssertCheckedThrows<T>(string op, T value)
        {
            AssertEx.Throws<OverflowException>(() => CrossCheck_UnaryPreAssignment_Checked_Core<T>(op)(value));
            AssertEx.Throws<OverflowException>(() => CrossCheck_UnaryPostAssignment_Checked_Core<T>(op)(value));
        }

        [Fact]
        public void CrossCheck_UnaryAssignment_Float()
        {
            Parallel.ForEach(new[] { "++", "--" }, op =>
            {
                CrossCheck_UnaryPreAssignment_Core<float>(op)(42.0f);
                CrossCheck_UnaryPreAssignment_Core<double>(op)(42.0d);
                CrossCheck_UnaryPreAssignment_Core<decimal>(op)(42.0m);

                CrossCheck_UnaryPostAssignment_Core<float>(op)(42.0f);
                CrossCheck_UnaryPostAssignment_Core<double>(op)(42.0d);
                CrossCheck_UnaryPostAssignment_Core<decimal>(op)(42.0m);
            });
        }

        [Fact] // See https://github.com/dotnet/corefx/issues/4984 for a relevant discussion
        public void CrossCheck_Issue4984_Unary_Repro1()
        {
            var f = Compile<Func<int, int>>(@"i =>
{
    var b = new WeakBox<int>();
    Log(b.Value);
    var res = b.Value++;
    Log(res);
    return b.Value;
}");
            f(0);
            f(41);
        }

        [Fact] // See https://github.com/dotnet/corefx/issues/4984 for a relevant discussion
        public void CrossCheck_Issue4984_Unary_Repro2()
        {
            var f = Compile<Func<int, int>>(@"i =>
{
    var b = new WeakBox<int>();
    Log(b[0]);
    var res = b[0]++;
    Log(res);
    return b[0];
}");
            f(0);
            f(41);
        }

        private Func<T, string> CrossCheck_UnaryPreAssignment_Core<T>(string op)
        {
            return CrossCheck_UnaryAssignment_Core<T>(op, null);
        }

        private Func<T, string> CrossCheck_UnaryPostAssignment_Core<T>(string op)
        {
            return CrossCheck_UnaryAssignment_Core<T>(null, op);
        }

        private Func<T, string> CrossCheck_UnaryAssignment_Core<T>(string pre, string post)
        {
            var t = typeof(T).ToCSharp();

            var f1 = Compile<Func<T, string>>($"x => {{ var y = x; var z = Log({pre}y{post}); return $\"({{y}},{{z}})\"; }}");
            var f2 = Compile<Func<T, string>>($"x => {{ var b = new StrongBox<{t}>(); var z = Log({pre}b.Value{post}); return $\"({{b.Value}},{{z}})\"; }}");
            var f3 = Compile<Func<T, string>>($"x => {{ var b = new WeakBox<{t}>(); var z = Log({pre}b.Value{post}); return $\"({{b.Value}},{{z}})\"; }}");
            var f4 = Compile<Func<T, string>>($"x => {{ var a = new {t}[1]; a[0] = x; var z = Log({pre}a[0]{post}); return $\"({{a[0]}},{{z}})\"; }}");
            var f5 = Compile<Func<T, string>>($"x => {{ var a = new {t}[1, 1]; a[0, 0] = x; var z = Log({pre}a[0, 0]{post}); return $\"({{a[0, 0]}},{{z}})\"; }}");
            var f6 = Compile<Func<T, string>>($"x => {{ var l = new List<{t}> {{ default({t}) }}; l[0] = x; var z = Log({pre}l[0]{post}); return $\"({{l[0]}},{{z}})\"; }}");
            var f7 = Compile<Func<T, string>>($"x => {{ var l = new List<{t}> {{ default({t}) }}; l[index: 0] = x; var z = Log({pre}l[index: 0]{post}); return $\"({{l[index: 0]}},{{z}})\"; }}");

            return f1 + f2 + f3 + f4 + f5 + f6 + f7;
        }

        private Func<T, string> CrossCheck_UnaryPreAssignment_Checked_Core<T>(string op)
        {
            return CrossCheck_UnaryAssignment_Checked_Core<T>(op, null);
        }

        private Func<T, string> CrossCheck_UnaryPostAssignment_Checked_Core<T>(string op)
        {
            return CrossCheck_UnaryAssignment_Checked_Core<T>(null, op);
        }

        private Func<T, string> CrossCheck_UnaryAssignment_Checked_Core<T>(string pre, string post)
        {
            var t = typeof(T).ToCSharp();

            var f1 = Compile<Func<T, string>>($"x => {{ var y = x; var z = Log(checked({pre}y{post})); return $\"({{y}},{{z}})\"; }}");
            var f2 = Compile<Func<T, string>>($"x => {{ var b = new StrongBox<{t}>(); var z = Log(checked({pre}b.Value{post})); return $\"({{b.Value}},{{z}})\"; }}");
            var f3 = Compile<Func<T, string>>($"x => {{ var b = new WeakBox<{t}>(); var z = Log(checked({pre}b.Value{post})); return $\"({{b.Value}},{{z}})\"; }}");
            var f4 = Compile<Func<T, string>>($"x => {{ var a = new {t}[1]; a[0] = x; var z = Log(checked({pre}a[0]{post})); return $\"({{a[0]}},{{z}})\"; }}");
            var f5 = Compile<Func<T, string>>($"x => {{ var a = new {t}[1, 1]; a[0, 0] = x; var z = Log(checked({pre}a[0, 0]{post})); return $\"({{a[0, 0]}},{{z}})\"; }}");
            var f6 = Compile<Func<T, string>>($"x => {{ var l = new List<{t}> {{ default({t}) }}; l[0] = x; var z = Log(checked({pre}l[0]{post})); return $\"({{l[0]}},{{z}})\"; }}");
            var f7 = Compile<Func<T, string>>($"x => {{ var l = new List<{t}> {{ default({t}) }}; l[index: 0] = x; var z = Log(checked({pre}l[index: 0]{post})); return $\"({{l[index: 0]}},{{z}})\"; }}");

            return f1 + f2 + f3 + f4 + f5 + f6 + f7;
        }

        #endregion
    }
}
