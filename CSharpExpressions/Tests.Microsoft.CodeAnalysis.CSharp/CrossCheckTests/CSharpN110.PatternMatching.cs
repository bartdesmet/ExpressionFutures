// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public void CrossCheck_IsExpression_List_Array_Empty()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is []");
            f(null);
            f(new int[0]);
            f(new int[] { 1 });
            f(new int[] { 1, 2 });
        }

        [Fact]
        public void CrossCheck_IsExpression_List_Array_OneElement_Constant()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is [1]");
            f(null);
            f(new int[0]);
            f(new int[] { 1 });
            f(new int[] { 1, 2 });
            f(new int[] { 2, 1 });
        }

        [Fact]
        public void CrossCheck_IsExpression_List_Array_TwoElements_Constant()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is [1, 2]");
            f(null);
            f(new int[0]);
            f(new int[] { 1 });
            f(new int[] { 1, 2 });
            f(new int[] { 2, 1 });
            f(new int[] { 1, 2, 3 });
        }

        [Fact]
        public void CrossCheck_IsExpression_List_Array_TwoElements_ComplexPatterns()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is [<= 0, > 0 and < 4]");
            
            f(null);
            
            f(new int[0]);
            
            f(new int[] { -1 });
            f(new int[] { 0 });
            f(new int[] { 1 });

            for (int i = -2; i <= 5; i++)
            {
                for (int j = -2; j <= 5; j++)
                {
                    f(new int[] { i, j });
                }
            }
            
            f(new int[] { 0, 1, 2 });
        }

        [Fact]
        public void CrossCheck_IsExpression_List_Array_DiscardSome()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is [1, _, _, 4]");
            f(null);
            f(new int[0]);
            f(new int[] { 1 });
            f(new int[] { 1, 2 });
            f(new int[] { 1, 2, 3 });
            f(new int[] { 1, 2, 3, 4 });
            f(new int[] { 0, 2, 3, 4 });
            f(new int[] { 1, 2, 3, 5 });
            f(new int[] { 1, 2, 3, 4, 5 });
        }

        [Fact]
        public void CrossCheck_IsExpression_List_Array_Assign()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is [1, _, 3] ys && ys[1] == 2");
            f(null);
            f(new int[0]);
            f(new int[] { 1 });
            f(new int[] { 1, 2 });
            f(new int[] { 1, 2, 3 });
            f(new int[] { 1, 2, 3, 4 });
        }

        [Fact]
        public void CrossCheck_IsExpression_List_String_Empty()
        {
            var f = Compile<Func<string, bool>>("s => s is []");
            f(null);
            f("");
            f("a");
            f("ab");
        }

        [Fact]
        public void CrossCheck_IsExpression_List_String_OneChar()
        {
            var f = Compile<Func<string, bool>>("s => s is ['a']");
            f(null);
            f("");
            f("a");
            f("b");
            f("ab");
            f("ba");
        }

        [Fact]
        public void CrossCheck_IsExpression_List_String_TwoChars_ComplexPatterns()
        {
            var f = Compile<Func<string, bool>>("s => s is [>= 'a' and <= 'z', (>= '0' and <= '9') or '_']");
            f(null);
            f("");
            f("a");
            f("z");
            f("0");
            f("9");
            f("ab");
            f("a0");
            f("c_");
            f("j5");
            f("z9");
            f("abc");
            f("123");
        }

        [Fact]
        public void CrossCheck_IsExpression_List_List_Empty()
        {
            var f = Compile<Func<List<int>, bool>>("xs => xs is []");
            f(null);
            f(new List<int>());
            f(new List<int> { 1 });
            f(new List<int> { 1, 2 });
        }

        [Fact]
        public void CrossCheck_IsExpression_List_List_SomeElements()
        {
            var f = Compile<Func<List<int>, bool>>("xs => xs is [1, 2, 3]");
            f(null);
            f(new List<int>());
            f(new List<int> { 1 });
            f(new List<int> { 1, 2 });
            f(new List<int> { 1, 2, 3 });
            f(new List<int> { 0, 2, 3 });
            f(new List<int> { 1, 0, 3 });
            f(new List<int> { 1, 2, 0 });
            f(new List<int> { 1, 2, 3, 4 });
        }

        [Fact]
        public void CrossCheck_IsExpression_List_ListOfTuple()
        {
            var f = Compile<Func<List<(string, int)>, bool>>("d => d is [(\"bar\", 42)]");
            f(null);
            f(new List<(string, int)>());
            f(new List<(string, int)> { ( "bar", 42) });
            f(new List<(string, int)> { ( "bar", 43) });
            f(new List<(string, int)> { ( "foo", 42) });
        }

        [Fact]
        public void CrossCheck_IsExpression_List_OddCollection_Empty()
        {
            var f = Compile<Func<OddCollection, bool>>("o => o is []", typeof(OddCollection).Assembly);
            f(null);
            f(new OddCollection());
            f(new OddCollection { Count = 1 });
        }

        [Fact]
        public void CrossCheck_IsExpression_List_OddCollection_Exception()
        {
            var f = Compile<Func<OddCollection, bool>>("o => o is [0]", typeof(OddCollection).Assembly);
            f(null);
            f(new OddCollection());
            AssertEx.Throws<InvalidOperationException>(() => f(new OddCollection { Count = 1 }));
            f(new OddCollection { Count = 2 });
        }

        [Fact]
        public void CrossCheck_IsExpression_List_OddCollection_NoException_Discard()
        {
            // NB: Index at even positions throws exception, but access is not performed for discard patterns.
            var f = Compile<Func<OddCollection, bool>>("o => o is [_, 1, _]", typeof(OddCollection).Assembly);
            f(null);
            f(new OddCollection());
            f(new OddCollection { Count = 1 });
            f(new OddCollection { Count = 2 });
            f(new OddCollection { Count = 3 });
            f(new OddCollection { Count = 4 });
        }

        [Fact]
        public void CrossCheck_IsExpression_List_Struct()
        {
            var f = Compile<Func<StructList<int>, bool>>("xs => xs is [1, 2]", typeof(StructList<>).Assembly);
            f(new StructList<int>(new List<int>()));
            f(new StructList<int>(new List<int> { 1 }));
            f(new StructList<int>(new List<int> { 1, 2 }));
            f(new StructList<int>(new List<int> { 2, 1 }));
            f(new StructList<int>(new List<int> { 1, 2, 3 }));
        }

        [Fact]
        public void CrossCheck_IsExpression_List_Struct_Nullable()
        {
            var f = Compile<Func<StructList<int>?, bool>>("xs => xs is [1, 2]", typeof(StructList<>).Assembly);
            f(null);
            f(new StructList<int>(new List<int>()));
            f(new StructList<int>(new List<int> { 1 }));
            f(new StructList<int>(new List<int> { 1, 2 }));
            f(new StructList<int>(new List<int> { 2, 1 }));
            f(new StructList<int>(new List<int> { 1, 2, 3 }));
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_Array_NonEmpty()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is [..]");
            f(null);
            f(new int[0]);
            f(new int[] { 1 });
            f(new int[] { 1, 2 });
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_Array_Prefix()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is [1, ..]");
            f(null);
            f(new int[0]);
            f(new int[] { 1 });
            f(new int[] { 1, 2 });
            f(new int[] { 2, 1 });
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_Array_Suffix()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is [.., 2]");
            f(null);
            f(new int[0]);
            f(new int[] { 1 });
            f(new int[] { 1, 2 });
            f(new int[] { 2, 1 });
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_Array_PrefixAndSuffix()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is [1, .., 9]");
            f(null);
            f(new int[0]);
            f(new int[] { 1 });
            f(new int[] { 1, 2 });
            f(new int[] { 2, 1 });
            f(new int[] { 1, 9 });
            f(new int[] { 1, 2, 9 });
            f(new int[] { 1, 2, 8, 9 });
        }

#if NET6_0_OR_GREATER // Compilation to a Func<...> delegate relies on RuntimeHelpers.GetSubArray.
        [Fact]
        public void CrossCheck_IsExpression_Slice_Array_VarPattern()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is [1, .. var s, 9] && s.Length == 0");
            f(null);
            f(new int[0]);
            f(new int[] { 1 });
            f(new int[] { 1, 2 });
            f(new int[] { 2, 1 });
            f(new int[] { 1, 9 });
            f(new int[] { 1, 2, 9 });
            f(new int[] { 1, 2, 8, 9 });
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_Array_PropertyPattern()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is [1, .. { Length: 1 }, 9]");
            f(null);
            f(new int[0]);
            f(new int[] { 1 });
            f(new int[] { 1, 2 });
            f(new int[] { 2, 1 });
            f(new int[] { 1, 9 });
            f(new int[] { 1, 2, 9 });
            f(new int[] { 1, 2, 8, 9 });
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_Array_ListPattern()
        {
            var f = Compile<Func<int[], bool>>("xs => xs is [1, .. [2, .., 8], 9]");
            f(null);
            f(new int[0]);
            f(new int[] { 1 });
            f(new int[] { 1, 2 });
            f(new int[] { 2, 1 });
            f(new int[] { 1, 9 });
            f(new int[] { 1, 2, 9 });
            f(new int[] { 1, 2, 8, 9 });
            f(new int[] { 1, 2, 7, 8, 9 });
            f(new int[] { 1, 0, 7, 8, 9 });
            f(new int[] { 1, 2, 7, 0, 9 });
        }
#endif

        [Fact]
        public void CrossCheck_IsExpression_Slice_String_NonEmpty()
        {
            var f = Compile<Func<string, bool>>("s => s is [..]");
            f(null);
            f("");
            f("a");
            f("ab");
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_String_Prefix()
        {
            var f = Compile<Func<string, bool>>("s => s is ['a', ..]");
            f(null);
            f("");
            f("a");
            f("b");
            f("ab");
            f("ba");
            f("abc");
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_StringSuffix()
        {
            var f = Compile<Func<string, bool>>("s => s is [.., 'z']");
            f(null);
            f("");
            f("z");
            f("y");
            f("yz");
            f("zy");
            f("xyz");
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_String_PrefixAndSuffix()
        {
            var f = Compile<Func<string, bool>>("s => s is ['a', .., 'z']");
            f(null);
            f("");
            f("a");
            f("z");
            f("ab");
            f("yz");
            f("az");
            f("abz");
            f("ayz");
            f("abyz");
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_String_VarPatter()
        {
            var f = Compile<Func<string, bool>>("s => s is ['a', .. var t, 'z'] && t.Length > 0");
            f(null);
            f("");
            f("a");
            f("z");
            f("ab");
            f("yz");
            f("az");
            f("abz");
            f("ayz");
            f("abyz");
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_String_PropertyPattern()
        {
            var f = Compile<Func<string, bool>>("s => s is ['a', .. { Length: > 0 }, 'z']");
            f(null);
            f("");
            f("a");
            f("z");
            f("ab");
            f("yz");
            f("az");
            f("abz");
            f("ayz");
            f("abyz");
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_String_ListPattern()
        {
            var f = Compile<Func<string, bool>>("s => s is ['a', .. ['b', .., 'y'], 'z']");
            f(null);
            f("");
            f("a");
            f("z");
            f("ab");
            f("yz");
            f("az");
            f("abz");
            f("ayz");
            f("abyz");
            f("acxz");
            f("abcxyz");
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_CustomType_RangeIndexer()
        {
            var f = Compile<Func<StructList<int>, bool>>("xs => xs is [0, .. [1, .., 8], 9]", typeof(StructList<>).Assembly);
            f(new StructList<int>(new List<int>()));
            f(new StructList<int>(new List<int> { 0 }));
            f(new StructList<int>(new List<int> { 0, 1 }));
            f(new StructList<int>(new List<int> { 0, 1, 2 }));
            f(new StructList<int>(new List<int> { 0, 1, 2, 7 }));
            f(new StructList<int>(new List<int> { 0, 1, 2, 7, 8 }));
            f(new StructList<int>(new List<int> { 0, 1, 2, 7, 8, 9 }));
            f(new StructList<int>(new List<int> { -1, 1, 2, 7, 8, 9 }));
            f(new StructList<int>(new List<int> { 0, -1, 2, 7, 8, 9 }));
            f(new StructList<int>(new List<int> { 0, 1, -1, 7, 8, 9 }));
            f(new StructList<int>(new List<int> { 0, 1, 2, 7, -1, 9 }));
            f(new StructList<int>(new List<int> { 0, 1, 2, 7, 8, -9 }));
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_CustomType_SliceMethod()
        {
            var f = Compile<Func<StructListWithSlice<int>, bool>>("xs => xs is [0, .. [1, .., 8], 9]", typeof(StructListWithSlice<>).Assembly);
            f(new StructListWithSlice<int>(new List<int>()));
            f(new StructListWithSlice<int>(new List<int> { 0 }));
            f(new StructListWithSlice<int>(new List<int> { 0, 1 }));
            f(new StructListWithSlice<int>(new List<int> { 0, 1, 2 }));
            f(new StructListWithSlice<int>(new List<int> { 0, 1, 2, 7 }));
            f(new StructListWithSlice<int>(new List<int> { 0, 1, 2, 7, 8 }));
            f(new StructListWithSlice<int>(new List<int> { 0, 1, 2, 7, 8, 9 }));
            f(new StructListWithSlice<int>(new List<int> { -1, 1, 2, 7, 8, 9 }));
            f(new StructListWithSlice<int>(new List<int> { 0, -1, 2, 7, 8, 9 }));
            f(new StructListWithSlice<int>(new List<int> { 0, 1, -1, 7, 8, 9 }));
            f(new StructListWithSlice<int>(new List<int> { 0, 1, 2, 7, -1, 9 }));
            f(new StructListWithSlice<int>(new List<int> { 0, 1, 2, 7, 8, -9 }));
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_CustomType_NoException()
        {
            // NB: Slice in even position does throw exception, but we don't access it so it skips evaluation.
            var f = Compile<Func<OddCollection, bool>>("xs => xs is [_, _, .., _]", typeof(OddCollection).Assembly);
            f(null);
            f(new OddCollection { Count = 0 });
            f(new OddCollection { Count = 1 });
            f(new OddCollection { Count = 2 });
            f(new OddCollection { Count = 3 });
            f(new OddCollection { Count = 4 });
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_CustomType_NoException_Discard()
        {
            // NB: Slice in even position does throw exception, but we don't access it so it skips evaluation.
            var f = Compile<Func<OddCollection, bool>>("xs => xs is [_, _, .. _, _]", typeof(OddCollection).Assembly);
            f(null);
            f(new OddCollection { Count = 0 });
            f(new OddCollection { Count = 1 });
            f(new OddCollection { Count = 2 });
            f(new OddCollection { Count = 3 });
            f(new OddCollection { Count = 4 });
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_CustomType_NoException_VarPattern()
        {
            // NB: Slice in odd position does not throw exception.
            var f = Compile<Func<OddCollection, bool>>("xs => xs is [_, .. var ys, _]", typeof(OddCollection).Assembly);
            f(null);
            f(new OddCollection { Count = 0 });
            f(new OddCollection { Count = 1 });
            f(new OddCollection { Count = 2 });
            f(new OddCollection { Count = 3 });
        }

        [Fact]
        public void CrossCheck_IsExpression_Slice_CustomType_Exception_VarPattern()
        {
            // NB: Slice in even position does throw exception.
            var f = Compile<Func<OddCollection, bool>>("xs => xs is [_, _, .. var ys, _]", typeof(OddCollection).Assembly);
            f(null);
            f(new OddCollection { Count = 0 });
            f(new OddCollection { Count = 1 });
            f(new OddCollection { Count = 2 });
            AssertEx.Throws<InvalidOperationException>(() => f(new OddCollection { Count = 3 }));
            AssertEx.Throws<InvalidOperationException>(() => f(new OddCollection { Count = 4 }));
        }
    }
}

public sealed class OddCollection
{
    public int Count { get; set; }

    public int this[int i] => i % 2 == 0 ? throw new InvalidOperationException() : i;

    public OddCollection this[Range range]
    {
        get
        {
            var (offset, length) = range.GetOffsetAndLength(Count);
            return offset % 2 == 0 ? throw new InvalidOperationException() : new OddCollection { Count = length };
        }
    }
}

public struct StructList<T>
{
    private readonly List<T> _list;

    public StructList(List<T> list) => _list = list;

    public int Count => _list.Count;

    public T this[int i] => _list[i];
    public StructList<T> this[Range range] => Slice(range);

    public StructList<T> Slice(Range range)
    {
        var (offset, length) = range.GetOffsetAndLength(_list.Count);
        return new StructList<T>(_list.Skip(offset).Take(length).ToList());
    }
}

public struct StructListWithSlice<T>
{
    private readonly List<T> _list;

    public StructListWithSlice(List<T> list) => _list = list;

    public int Count => _list.Count;

    public T this[int i] => _list[i];

    public StructListWithSlice<T> Slice(int offset, int length)
    {
        return new StructListWithSlice<T>(_list.Skip(offset).Take(length).ToList());
    }
}

