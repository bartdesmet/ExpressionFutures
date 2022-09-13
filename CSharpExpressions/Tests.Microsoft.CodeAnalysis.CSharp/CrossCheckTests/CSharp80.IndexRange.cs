// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Microsoft.CodeAnalysis.CSharp;

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
        [Fact]
        public void CrossCheck_Index_ImplicitConversion()
        {
            var f = Compile<Func<int, Index>>("i => i");
            f(1);
        }

        [Fact]
        public void CrossCheck_Index_ImplicitConversion_Lifted()
        {
            var f = Compile<Func<int?, Index?>>("i => i");
            f(null);
            f(1);
        }

        [Fact]
        public void CrossCheck_Index_FromEnd()
        {
            var f = Compile<Func<int, Index>>("i => ^i");
            f(1);

            var g = Compile<Func<int, Index>>("i => ^Return(i)");
            g(1);
        }

        [Fact]
        public void CrossCheck_Index_FromEnd_Lifted()
        {
            var f = Compile<Func<int?, Index?>>("i => ^i");
            f(1);

            var g = Compile<Func<int?, Index?>>("i => ^Return(i)");
            g(1);
        }

        [Fact]
        public void CrossCheck_Range()
        {
            Compile<Func<Range>>("() => ..")();

            Compile<Func<Range>>("() => 1..")();
            Compile<Func<Range>>("() => ..2")();

            Compile<Func<Range>>("() => 1..2")();

            Compile<Func<Range>>("() => Return(1)..")();
            Compile<Func<Range>>("() => ..Return(2)")();

            Compile<Func<Range>>("() => Return(1)..Return(2)")();
        }

        [Fact]
        public void CrossCheck_Range_Index()
        {
            Compile<Func<Index, Range>>("i => i..")(1);
            Compile<Func<Index, Range>>("i => ..i")(2);

            Compile<Func<Index, Index, Range>>("(i, j) => i..j")(1, 2);

            Compile<Func<Index, Range>>("i => Return(i)..")(1);
            Compile<Func<Index, Range>>("i => ..Return(i)")(2);

            Compile<Func<Index, Index, Range>>("(i, j) => Return(i)..Return(j)")(1, 2);
        }

        [Fact]
        public void CrossCheck_Range_Lifted()
        {
            var f1 = Compile<Func<Index?, Range?>>("i => i..");
            f1(null);
            f1(1);

            var f2 = Compile<Func<Index?, Range?>>("i => ..i");
            f2(null);
            f2(2);

            var f3 = Compile<Func<Index?, Index?, Range?>>("(i, j) => i..j");
            f3(null, null);
            f3(null, 2);
            f3(1, null);
            f3(1, 2);

            var f4 = Compile<Func<Index?, Range?>>("i => Return(i)..");
            f4(null);
            f4(1);

            var f5 = Compile<Func<Index?, Range?>>("i => ..Return(i)");
            f5(null);
            f5(2);

            var f6 = Compile<Func<Index?, Index?, Range?>>("(i, j) => Return(i)..Return(j)");
            f6(null, null);
            f6(null, 2);
            f6(1, null);
            f6(1, 2);
        }

        [Fact]
        public void CrossCheck_IndexerAccess_Array_Index()
        {
            var f1 = Compile<Func<int[], Index, int>>("(xs, i) => xs[i]");
            f1(new[] { 1, 2, 3, 4, 5 }, 2);
            f1(new[] { 1, 2, 3, 4, 5 }, new Index(2, fromEnd: true));
            AssertEx.Throws<NullReferenceException>(() => f1(null, 2));
            AssertEx.Throws<IndexOutOfRangeException>(() => f1(new[] { 1, 2 }, 2));
            AssertEx.Throws<IndexOutOfRangeException>(() => f1(new[] { 1, 2 }, new Index(3, fromEnd: true)));
            
            var f2 = Compile<Func<int[], Index, int>>("(xs, i) => Return(xs, \"A\")[Return(i, \"I\")]");
            f2(new[] { 1, 2, 3, 4, 5 }, 2);
            f2(new[] { 1, 2, 3, 4, 5 }, new Index(2, fromEnd: true));
            AssertEx.Throws<NullReferenceException>(() => f2(null, 2));
            AssertEx.Throws<IndexOutOfRangeException>(() => f2(new[] { 1, 2 }, 2));
            AssertEx.Throws<IndexOutOfRangeException>(() => f2(new[] { 1, 2 }, new Index(3, fromEnd: true)));

            var f3 = Compile<Func<int[], int>>("xs => Return(xs, \"A\")[2]");
            f3(new[] { 1, 2, 3, 4, 5 });
            AssertEx.Throws<NullReferenceException>(() => f3(null));
            AssertEx.Throws<IndexOutOfRangeException>(() => f3(new[] { 1, 2 }));

            var f4 = Compile<Func<int[], int>>("xs => Return(xs, \"A\")[^3]");
            f4(new[] { 1, 2, 3, 4, 5 });
            AssertEx.Throws<NullReferenceException>(() => f4(null));
            AssertEx.Throws<IndexOutOfRangeException>(() => f4(new[] { 1 }));
        }

        [Fact]
        public void CrossCheck_IndexerAccess_List_Index()
        {
            var f1 = Compile<Func<List<int>, Index, int>>("(xs, i) => xs[i]");
            f1(new List<int> { 1, 2, 3, 4, 5 }, 2);
            f1(new List<int> { 1, 2, 3, 4, 5 }, new Index(2, fromEnd: true));
            AssertEx.Throws<NullReferenceException>(() => f1(null, 2));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f1(new List<int> { 1, 2 }, 2));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f1(new List<int> { 1, 2 }, new Index(3, fromEnd: true)));

            var f2 = Compile<Func<List<int>, Index, int>>("(xs, i) => Return(xs, \"A\")[Return(i, \"I\")]");
            f2(new List<int> { 1, 2, 3, 4, 5 }, 2);
            f2(new List<int> { 1, 2, 3, 4, 5 }, new Index(2, fromEnd: true));
            AssertEx.Throws<NullReferenceException>(() => f2(null, 2));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f2(new List<int> { 1, 2 }, 2));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f2(new List<int> { 1, 2 }, new Index(3, fromEnd: true)));

            var f3 = Compile<Func<List<int>, int>>("xs => Return(xs, \"A\")[2]");
            f3(new List<int> { 1, 2, 3, 4, 5 });
            AssertEx.Throws<NullReferenceException>(() => f3(null));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f3(new List<int> { 1, 2 }));

            var f4 = Compile<Func<List<int>, int>>("xs => Return(xs, \"A\")[^3]");
            f4(new List<int> { 1, 2, 3, 4, 5 });
            AssertEx.Throws<NullReferenceException>(() => f4(null));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f4(new List<int> { 1, 2 }));
        }

        [Fact]
        public void CrossCheck_IndexerAccess_String_Index()
        {
            var f1 = Compile<Func<string, Index, char>>("(s, i) => s[i]");
            f1("foobarqux", 2);
            f1("foobarqux", new Index(2, fromEnd: true));
            AssertEx.Throws<NullReferenceException>(() => f1(null, 2));
            AssertEx.Throws<IndexOutOfRangeException>(() => f1("", 2));
            AssertEx.Throws<IndexOutOfRangeException>(() => f1("bar", 7));
            AssertEx.Throws<IndexOutOfRangeException>(() => f1("bar", new Index(7, fromEnd: true)));

            var f2 = Compile<Func<string, Index, char>>("(s, i) => Return(s, \"A\")[Return(i, \"I\")]");
            f2("foobarqux", 2);
            f2("foobarqux", new Index(2, fromEnd: true));
            AssertEx.Throws<NullReferenceException>(() => f2(null, 2));
            AssertEx.Throws<IndexOutOfRangeException>(() => f2("", 2));
            AssertEx.Throws<IndexOutOfRangeException>(() => f2("bar", 7));
            AssertEx.Throws<IndexOutOfRangeException>(() => f2("bar", new Index(7, fromEnd: true)));

            var f3 = Compile<Func<string, char>>("s => Return(s, \"A\")[2]");
            f3("foobarqux");
            AssertEx.Throws<NullReferenceException>(() => f3(null));
            AssertEx.Throws<IndexOutOfRangeException>(() => f3(""));
            AssertEx.Throws<IndexOutOfRangeException>(() => f3("ba"));

            var f4 = Compile<Func<string, char>>("s => Return(s, \"A\")[^3]");
            f4("foobarqux");
            AssertEx.Throws<NullReferenceException>(() => f4(null));
            AssertEx.Throws<IndexOutOfRangeException>(() => f4(""));
            AssertEx.Throws<IndexOutOfRangeException>(() => f4("ba"));
        }

        [Fact]
        public void CrossCheck_IndexerAccess_Array_Slice()
        {
            var xs = Enumerable.Range(7, 50).ToArray();

            var f1 = Compile<Func<int[], Range, string>>("(xs, i) => string.Join(',', xs[i])");
            f1(xs, new Range(9, 16));
        }

        [Fact]
        public void CrossCheck_IndexerAccess_String_Slice()
        {
            var f1 = Compile<Func<string, Range, string>>("(s, i) => s[i]");
            f1("foobarqux", Range.All);
            f1("foobarqux", Range.StartAt(2));
            f1("foobarqux", Range.StartAt(Index.FromEnd(2)));
            f1("foobarqux", Range.EndAt(5));
            f1("foobarqux", Range.EndAt(Index.FromEnd(5)));
            f1("foobarqux", new Range(2, 5));
            AssertEx.Throws<NullReferenceException>(() => f1(null, Range.All));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f1("foobarqux", Range.StartAt(10)));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f1("foobarqux", Range.StartAt(Index.FromEnd(10))));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f1("foobarqux", Range.EndAt(10)));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f1("foobarqux", Range.EndAt(Index.FromEnd(10))));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f1("foobarqux", new Range(10, 21)));

            var f2 = Compile<Func<string, string>>("s => Return(s)[Return(2)..Return(3)]");
            f2("foobarqux");
            AssertEx.Throws<NullReferenceException>(() => f2(null));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f2(""));
        }

        [Fact]
        public void CrossCheck_IndexerAccess_CustomString_Slice()
        {
            var f1 = Compile<Func<MySliceableString, Range, MySliceableString>>("(s, i) => s[i]", typeof(MySliceableString).Assembly);
            f1("foobarqux", Range.All);
            f1("foobarqux", Range.StartAt(2));
            f1("foobarqux", Range.StartAt(Index.FromEnd(2)));
            f1("foobarqux", Range.EndAt(5));
            f1("foobarqux", Range.EndAt(Index.FromEnd(5)));
            f1("foobarqux", new Range(2, 5));
            AssertEx.Throws<NullReferenceException>(() => f1(null, Range.All));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f1("foobarqux", Range.StartAt(10)));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f1("foobarqux", Range.StartAt(Index.FromEnd(10))));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f1("foobarqux", Range.EndAt(10)));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f1("foobarqux", Range.EndAt(Index.FromEnd(10))));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f1("foobarqux", new Range(10, 21)));

            var f2 = Compile<Func<MySliceableString, MySliceableString>>("s => Return(s)[Return(2)..Return(3)]", typeof(MySliceableString).Assembly);
            f2("foobarqux");
            AssertEx.Throws<NullReferenceException>(() => f2(null));
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f2(""));
        }
    }
}

public sealed class MySliceableString : IEquatable<MySliceableString>
{
    private readonly string _str;

    public MySliceableString(string str) => _str = str;

    public static implicit operator MySliceableString(string s) => new MySliceableString(s);

    public int Length => _str.Length;

    public string Slice(int offset, int length) => _str.Substring(offset, length);

    public bool Equals(MySliceableString other) => other != null && other._str == _str;

    public override bool Equals(object obj) => Equals(obj as MySliceableString);

    public override int GetHashCode() => _str?.GetHashCode() ?? 0;
}
