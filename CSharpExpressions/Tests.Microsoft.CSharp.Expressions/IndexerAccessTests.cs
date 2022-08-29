// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Xunit;
using static Tests.TestHelpers;

namespace Tests
{
    public partial class IndexerAccessTests
    {
        private static readonly PropertyInfo StringLength = typeof(string).GetProperty(nameof(string.Length));
        private static readonly PropertyInfo StringChars = typeof(string).GetProperty("Chars");
        private static readonly MethodInfo StringSubstring = typeof(string).GetMethod(nameof(string.Substring), new[] { typeof(int), typeof(int) });

        private static readonly PropertyInfo CIndexer = typeof(C).GetProperty("Item", new[] { typeof(int) });
        private static readonly PropertyInfo CLength = typeof(C).GetProperty(nameof(C.Length));
        private static readonly MethodInfo CSlice = typeof(C).GetMethod(nameof(C.Slice), new[] { typeof(int), typeof(int) });

        private static readonly PropertyInfo SliceAndIndexStringIndexer = typeof(SliceAndIndexString).GetProperty("Item", new[] { typeof(int) });
        private static readonly PropertyInfo SliceAndIndexStringLength = typeof(SliceAndIndexString).GetProperty(nameof(SliceAndIndexString.Length));
        private static readonly PropertyInfo SliceAndIndexStringValue = typeof(SliceAndIndexString).GetProperty(nameof(SliceAndIndexString.Value));
        private static readonly MethodInfo SliceAndIndexStringSlice = typeof(SliceAndIndexString).GetMethod(nameof(SliceAndIndexString.Slice), new[] { typeof(int), typeof(int) });

        private static readonly PropertyInfo SliceAndIndexListIndexer = typeof(SliceAndIndexList).GetProperty("Item", new[] { typeof(int) });
        private static readonly PropertyInfo SliceAndIndexListLength = typeof(SliceAndIndexList).GetProperty(nameof(SliceAndIndexList.Length));
        private static readonly PropertyInfo SliceAndIndexListValue = typeof(SliceAndIndexList).GetProperty(nameof(SliceAndIndexList.Value));
        private static readonly MethodInfo SliceAndIndexListSlice = typeof(SliceAndIndexList).GetMethod(nameof(SliceAndIndexList.Slice), new[] { typeof(int), typeof(int) });

        private static readonly PropertyInfo SliceAndIndexListStructIndexer = typeof(SliceAndIndexListStruct).GetProperty("Item", new[] { typeof(int) });
        private static readonly PropertyInfo SliceAndIndexListStructLength = typeof(SliceAndIndexListStruct).GetProperty(nameof(SliceAndIndexListStruct.Length));
        private static readonly PropertyInfo SliceAndIndexListStructValue = typeof(SliceAndIndexListStruct).GetProperty(nameof(SliceAndIndexListStruct.Value));
        private static readonly MethodInfo SliceAndIndexListStructSlice = typeof(SliceAndIndexListStruct).GetMethod(nameof(SliceAndIndexListStruct.Slice), new[] { typeof(int), typeof(int) });

        [Fact]
        public void IndexerAccess_Factory_ArgumentChecking()
        {
            var s = Expression.Parameter(typeof(string));
            var i = Expression.Parameter(typeof(Index));
            var r = Expression.Parameter(typeof(Range));
            var o = Expression.Parameter(typeof(object));
            var c = Expression.Parameter(typeof(C));
            var a = Expression.Parameter(new { Length = 0 }.GetType());
            var t = Expression.Parameter(typeof(SliceAndIndexList));

            // null checks
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.IndexerAccess(null, i, StringLength, StringChars));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.IndexerAccess(s, null, StringLength, StringChars));

            // can't find Length or Count property
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.IndexerAccess(o, i, default(PropertyInfo), null));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.IndexerAccess(o, i, default(MethodInfo), null));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.IndexerAccess(o, r, default(PropertyInfo), null));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.IndexerAccess(o, r, default(MethodInfo), null));

            // can't find indexer or slice
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.IndexerAccess(a, i, default(PropertyInfo), null));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.IndexerAccess(a, i, default(MethodInfo), null));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.IndexerAccess(a, r, default(PropertyInfo), null));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.IndexerAccess(a, r, default(MethodInfo), null));

            // auto-resolve indexer
            var strIndx1 = CSharpExpression.IndexerAccess(s, i, default(MethodInfo), null);
            Assert.Equal(StringLength, strIndx1.LengthOrCount);
            Assert.Equal(StringChars, strIndx1.IndexOrSlice);

            var strIndx2 = CSharpExpression.IndexerAccess(s, i, default(PropertyInfo), null);
            Assert.Equal(StringLength, strIndx2.LengthOrCount);
            Assert.Equal(StringChars, strIndx2.IndexOrSlice);

            var lstIndx1 = CSharpExpression.IndexerAccess(t, i, default(MethodInfo), null);
            Assert.Equal(SliceAndIndexListLength, lstIndx1.LengthOrCount);
            Assert.Equal(SliceAndIndexListIndexer, lstIndx1.IndexOrSlice);

            var lstIndx2 = CSharpExpression.IndexerAccess(t, i, default(PropertyInfo), null);
            Assert.Equal(SliceAndIndexListLength, lstIndx2.LengthOrCount);
            Assert.Equal(SliceAndIndexListIndexer, lstIndx2.IndexOrSlice);

            // auto-resolve slice
            var strSlice1 = CSharpExpression.IndexerAccess(s, r, default(MethodInfo), null);
            Assert.Equal(StringLength, strSlice1.LengthOrCount);
            Assert.Equal(StringSubstring, strSlice1.IndexOrSlice);

            var strSlice2 = CSharpExpression.IndexerAccess(s, r, default(PropertyInfo), null);
            Assert.Equal(StringLength, strSlice2.LengthOrCount);
            Assert.Equal(StringSubstring, strSlice2.IndexOrSlice);

            var lstSlice1 = CSharpExpression.IndexerAccess(t, r, default(MethodInfo), null);
            Assert.Equal(SliceAndIndexListLength, lstSlice1.LengthOrCount);
            Assert.Equal(SliceAndIndexListSlice, lstSlice1.IndexOrSlice);

            var lstSlice2 = CSharpExpression.IndexerAccess(t, r, default(PropertyInfo), null);
            Assert.Equal(SliceAndIndexListLength, lstSlice2.LengthOrCount);
            Assert.Equal(SliceAndIndexListSlice, lstSlice2.IndexOrSlice);

            // the following are valid
            Assert.NotNull(CSharpExpression.IndexerAccess(s, i, StringLength, StringChars));
            Assert.NotNull(CSharpExpression.IndexerAccess(s, r, StringLength, StringSubstring));

            Assert.NotNull(CSharpExpression.IndexerAccess(c, i, CLength, CIndexer));
            Assert.NotNull(CSharpExpression.IndexerAccess(c, i, CLength.GetMethod, CIndexer));
            Assert.NotNull(CSharpExpression.IndexerAccess(c, i, CLength, CIndexer.GetMethod));
            Assert.NotNull(CSharpExpression.IndexerAccess(c, r, CLength, CSlice));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(s, o, StringLength, StringChars)); // invalid index type

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, i, typeof(C).GetProperty(nameof(C.LengthWriteOnly)), CIndexer)); // no getter
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, i, typeof(C).GetProperty(nameof(C.LengthString)), CIndexer)); // not int
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, i, typeof(C).GetProperty(nameof(C.LengthStatic)), CIndexer)); // not instance
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, i, typeof(C).GetProperty("Item", new[] { typeof(string) }), CIndexer)); // not a property
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, i, StringLength, CIndexer)); // wrong declaring type

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, i, CLength, typeof(C).GetField(nameof(C.LengthField)))); // not an indexer
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, i, CLength, CSlice)); // not an indexer
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, i, CLength, typeof(C).GetProperty("Item", new[] { typeof(string) }))); // not int
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, i, CLength, typeof(C).GetProperty("Item", new[] { typeof(int), typeof(int) }))); // not (int)
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, i, CLength, StringChars)); // wrong declaring type

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, r, CLength, CLength)); // not a method
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, r, CLength, StringSubstring)); // wrong declaring type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, r, CLength, typeof(C).GetField(nameof(C.LengthField)))); // not a method
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, r, CLength, typeof(C).GetMethod(nameof(C.SliceStatic)))); // not instance
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, r, CLength, typeof(C).GetMethod(nameof(C.Slice0)))); // not (int, int)
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, r, CLength, typeof(C).GetMethod(nameof(C.Slice1)))); // not (int, int)
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, r, CLength, typeof(C).GetMethod(nameof(C.Slice2X)))); // not (int, int)
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, r, CLength, typeof(C).GetMethod(nameof(C.Slice2Y)))); // not (int, int)
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.IndexerAccess(c, r, CLength, typeof(C).GetMethod(nameof(C.Slice3)))); // not (int, int)
        }

        [Fact]
        public void IndexerAccess_Factory_Properties()
        {
            var s = Expression.Parameter(typeof(string));
            var i = Expression.Parameter(typeof(Index));
            var r = Expression.Parameter(typeof(Range));

            var e1 = CSharpExpression.IndexerAccess(s, i, StringLength, StringChars);

            Assert.Same(s, e1.Object);
            Assert.Same(i, e1.Argument);
            Assert.Same(StringLength, e1.LengthOrCount);
            Assert.Same(StringChars, e1.IndexOrSlice);

            Assert.Equal(CSharpExpressionType.IndexerAccess, e1.CSharpNodeType);
            Assert.Equal(typeof(char), e1.Type);

            var e2 = CSharpExpression.IndexerAccess(s, i, StringLength.GetMethod, StringChars.GetMethod);

            Assert.Same(s, e2.Object);
            Assert.Same(i, e2.Argument);
            Assert.Same(StringLength, e2.LengthOrCount);
            Assert.Same(StringChars, e2.IndexOrSlice);

            Assert.Equal(CSharpExpressionType.IndexerAccess, e2.CSharpNodeType);
            Assert.Equal(typeof(char), e2.Type);

            var e3 = CSharpExpression.IndexerAccess(s, r, StringLength, StringSubstring);

            Assert.Same(s, e3.Object);
            Assert.Same(r, e3.Argument);
            Assert.Same(StringLength, e3.LengthOrCount);
            Assert.Same(StringSubstring, e3.IndexOrSlice);

            Assert.Equal(CSharpExpressionType.IndexerAccess, e3.CSharpNodeType);
            Assert.Equal(typeof(string), e3.Type);
        }

        [Fact]
        public void IndexerAccess_Update()
        {
            var s1 = Expression.Parameter(typeof(string));
            var i1 = Expression.Parameter(typeof(Index));

            var s2 = Expression.Parameter(typeof(string));
            var i2 = Expression.Parameter(typeof(Index));

            var a1 = CSharpExpression.IndexerAccess(s1, i1, StringLength, StringChars);
            Assert.Same(a1, a1.Update(a1.Object, a1.Argument));

            var a2 = a1.Update(s2, a1.Argument);
            Assert.Same(s2, a2.Object);
            Assert.Same(a1.Argument, a2.Argument);

            var a3 = a1.Update(a1.Object, i2);
            Assert.Same(a1.Object, a3.Object);
            Assert.Same(i2, a3.Argument);
        }

        [Fact]
        public void IndexerAccess_Visitor()
        {
            var s = Expression.Parameter(typeof(string));
            var i = Expression.Parameter(typeof(Index));

            var res = CSharpExpression.IndexerAccess(s, i, StringLength, StringChars);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitIndexerAccess(IndexerAccessCSharpExpression node)
            {
                Visited = true;

                return base.VisitIndexerAccess(node);
            }
        }

        [Fact]
        public void IndexerAccess_Basics_String_Index()
        {
            var str = "foobar";

            //
            // Constants
            //

            for (int i = 0; i < str.Length; i++)
            {
                var obj = Expression.Constant(str);
                var index = Expression.Constant(new Index(i));

                var f = Expression.Lambda<Func<char>>(CSharpExpression.IndexerAccess(obj, index, StringLength, StringChars));

                var res = f.Compile()();

                Assert.Equal(str[i], res);
            }

            for (int i = 1; i <= str.Length; i++)
            {
                var j = new Index(i, fromEnd: true);

                var obj = Expression.Constant(str);
                var index = Expression.Constant(j);

                var f = Expression.Lambda<Func<char>>(CSharpExpression.IndexerAccess(obj, index, StringLength, StringChars));

                var res = f.Compile()();

                Assert.Equal(str[j.GetOffset(str.Length)], res);
            }

            for (int i = 1; i <= str.Length; i++)
            {
                var j = new Index(i, fromEnd: true);

                var obj = Expression.Constant(str);
                var index = CSharpExpression.FromEndIndex(Expression.Constant(i));

                var f = Expression.Lambda<Func<char>>(CSharpExpression.IndexerAccess(obj, index, StringLength, StringChars));

                var res = f.Compile()();

                Assert.Equal(str[j.GetOffset(str.Length)], res);
            }

            // Variables

            for (int i = 0; i < str.Length; i++)
            {
                var obj = Expression.Parameter(typeof(string));
                var index = Expression.Parameter(typeof(Index));

                var f = Expression.Lambda<Func<string, Index, char>>(CSharpExpression.IndexerAccess(obj, index, StringLength, StringChars), obj, index);

                var res = f.Compile()(str, new Index(i));

                Assert.Equal(str[i], res);
            }

            for (int i = 1; i <= str.Length; i++)
            {
                var j = new Index(i, fromEnd: true);

                var obj = Expression.Parameter(typeof(string));
                var index = Expression.Parameter(typeof(Index));

                var f = Expression.Lambda<Func<string, Index, char>>(CSharpExpression.IndexerAccess(obj, index, StringLength, StringChars), obj, index);

                var res = f.Compile()(str, j);

                Assert.Equal(str[j.GetOffset(str.Length)], res);
            }

            for (int i = 1; i <= str.Length; i++)
            {
                var j = new Index(i, fromEnd: true);

                var obj = Expression.Parameter(typeof(string));
                var index = Expression.Parameter(typeof(int));

                var indexFromEnd = CSharpExpression.FromEndIndex(index);

                var f = Expression.Lambda<Func<string, int, char>>(CSharpExpression.IndexerAccess(obj, indexFromEnd, StringLength, StringChars), obj, index);

                var res = f.Compile()(str, i);

                Assert.Equal(str[j.GetOffset(str.Length)], res);
            }

            // Other

            for (int i = 0; i < str.Length; i++)
            {
                var log = "";

                var obj = Expression.Invoke(Expression.Constant(new Func<string>(() => { log += "S"; return str; })));
                var index = Expression.Invoke(Expression.Constant(new Func<Index>(() => { log += "I"; return new Index(i); })));

                var f = Expression.Lambda<Func<char>>(CSharpExpression.IndexerAccess(obj, index, StringLength, StringChars));

                var res = f.Compile()();

                Assert.Equal(str[i], res);
                Assert.Equal("SI", log);
            }

            for (int i = 1; i <= str.Length; i++)
            {
                var j = new Index(i, fromEnd: true);

                var log = "";

                var obj = Expression.Invoke(Expression.Constant(new Func<string>(() => { log += "S"; return str; })));
                var index = Expression.Invoke(Expression.Constant(new Func<Index>(() => { log += "I"; return new Index(i, fromEnd: true); })));

                var f = Expression.Lambda<Func<char>>(CSharpExpression.IndexerAccess(obj, index, StringLength, StringChars));

                var res = f.Compile()();

                Assert.Equal(str[j.GetOffset(str.Length)], res);
                Assert.Equal("SI", log);
            }

            for (int i = 1; i <= str.Length; i++)
            {
                var j = new Index(i, fromEnd: true);

                var log = "";

                var obj = Expression.Invoke(Expression.Constant(new Func<string>(() => { log += "S"; return str; })));
                var index = Expression.Invoke(Expression.Constant(new Func<int>(() => { log += "I"; return i; })));

                var indexFromEnd = CSharpExpression.FromEndIndex(index);

                var f = Expression.Lambda<Func<char>>(CSharpExpression.IndexerAccess(obj, indexFromEnd, StringLength, StringChars));

                var res = f.Compile()();

                Assert.Equal(str[j.GetOffset(str.Length)], res);
                Assert.Equal("SI", log);
            }
        }

        [Fact]
        public void IndexerAccess_Basics_String_Index_SideEffects()
        {
            var str = "foobar";

            for (int i = 0; i < str.Length; i++)
            {
                AssertCompile<char>((log, append) =>
                {
                    var obj = Expression.Block(log("O"), Expression.Constant(str));
                    var index = Expression.Block(log("I"), Expression.Constant(new Index(i)));

                    return CSharpExpression.IndexerAccess(obj, index, StringLength, StringChars);
                }, new LogAndResult<char> { Log = { "O", "I" }, Value = str[i] });
            }

            for (int i = 1; i <= str.Length; i++)
            {
                var j = new Index(i, fromEnd: true);

                AssertCompile<char>((log, append) =>
                {
                    var obj = Expression.Block(log("O"), Expression.Constant(str));
                    var index = Expression.Block(log("I"), Expression.Constant(j));

                    return CSharpExpression.IndexerAccess(obj, index, StringLength, StringChars);
                }, new LogAndResult<char> { Log = { "O", "I" }, Value = str[j.GetOffset(str.Length)] });
            }
        }

        [Fact]
        public void IndexerAccess_Index_SideEffects()
        {
            var ctor = typeof(SliceAndIndexString).GetConstructors().Single();

            var str = "foobar";

            // str[3]

            AssertCompile<char>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var index = Expression.Constant(new Index(3));

                return CSharpExpression.IndexerAccess(obj, index, SliceAndIndexStringLength, SliceAndIndexStringIndexer);
            }, new LogAndResult<char> { Log = { "O", "this[3] get()" }, Value = 'b' });

            // str[^3]

            AssertCompile<char>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var index = Expression.Constant(new Index(3, fromEnd: true));

                return CSharpExpression.IndexerAccess(obj, index, SliceAndIndexStringLength, SliceAndIndexStringIndexer);
            }, new LogAndResult<char> { Log = { "O", "Length", "this[3] get()" }, Value = 'b' });

            // str[i]

            AssertCompile<char>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var index = Expression.Block(log("I"), Expression.Constant(new Index(3)));

                return CSharpExpression.IndexerAccess(obj, index, SliceAndIndexStringLength, SliceAndIndexStringIndexer);
            }, new LogAndResult<char> { Log = { "O", "I", "Length", "this[3] get()" }, Value = 'b' });

            // str[^i]

            AssertCompile<char>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var index = Expression.Block(log("I"), Expression.Constant(new Index(3, fromEnd: true)));

                return CSharpExpression.IndexerAccess(obj, index, SliceAndIndexStringLength, SliceAndIndexStringIndexer);
            }, new LogAndResult<char> { Log = { "O", "I", "Length", "this[3] get()" }, Value = 'b' });

            // str[^i] using FromEndIndex

            AssertCompile<char>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var index = CSharpExpression.FromEndIndex(Expression.Block(log("E"), Expression.Constant(3)));

                return CSharpExpression.IndexerAccess(obj, index, SliceAndIndexStringLength, SliceAndIndexStringIndexer);
            }, new LogAndResult<char> { Log = { "O", "E", "Length", "this[3] get()" }, Value = 'b' });
        }

        [Fact]
        public void IndexerAccess_Basics_String_Range()
        {
            var str = "foobar";

            var indexes = (from i in Enumerable.Range(0, str.Length + 2)
                           from j in new[] { new Index(i), new Index(i, fromEnd: true) }
                           select (Index?)j
                          ).Prepend(null);

            var ranges = from start in indexes
                         from end in indexes
                         select (start, end);

            foreach (var (start, end) in ranges)
            {
                Range rng;

                if (start == null)
                {
                    rng = end == null ? Range.All : Range.EndAt(end.Value);
                }
                else if (end == null)
                {
                    rng = Range.StartAt(start.Value);
                }
                else
                {
                    rng = new Range(start.Value, end.Value);
                }

                var (offset, length) = GetOffsetAndLengthNoFail(rng, str.Length);
                var actual = new Func<string>(() => str.Substring(offset, length));

                // Constant

                {
                    var obj = Expression.Constant(str);
                    var range = Expression.Constant(rng);

                    var expr = CSharpExpression.IndexerAccess(obj, range, StringLength, StringSubstring);

                    var f = Expression.Lambda<Func<string>>(expr).Compile();

                    AssertEval(actual, f);
                }

                // Variable

                {
                    var obj = Expression.Parameter(typeof(string));
                    var range = Expression.Parameter(typeof(Range));

                    var expr = CSharpExpression.IndexerAccess(obj, range, StringLength, StringSubstring);

                    var f = Expression.Lambda<Func<string, Range, string>>(expr, obj, range).Compile();

                    AssertEval(actual, () => f(str, rng));
                }

                // Other

                {
                    var obj = Expression.Invoke(Expression.Constant(new Func<string>(() => str)));
                    var range = Expression.Invoke(Expression.Constant(new Func<Range>(() => rng)));

                    var expr = CSharpExpression.IndexerAccess(obj, range, StringLength, StringSubstring);

                    var f = Expression.Lambda<Func<string>>(expr).Compile();

                    AssertEval(actual, f);
                }

                // Helper function to get RangeExpression values

                List<Expression> GetRangeExprs(Range range, Expression startExpr, Expression endExpr)
                {
                    var rangeExprs = new List<Expression>
                    {
                        CSharpExpression.Range(startExpr, endExpr)
                    };

                    if (range.Start.Equals(Index.Start))
                    {
                        if (range.End.Equals(Index.End))
                        {
                            rangeExprs.Add(CSharpExpression.Range(null, null));
                        }
                        else
                        {
                            rangeExprs.Add(CSharpExpression.Range(null, endExpr));
                        }
                    }
                    else if (range.End.Equals(Index.End))
                    {
                        rangeExprs.Add(CSharpExpression.Range(startExpr, null));
                    }

                    return rangeExprs;
                }

                // RangeExpression with constant Index operands

                {
                    var startExpr = Expression.Constant(rng.Start);
                    var endExpr = Expression.Constant(rng.End);

                    var obj = Expression.Constant(str);

                    foreach (var range in GetRangeExprs(rng, startExpr, endExpr))
                    {
                        var expr = CSharpExpression.IndexerAccess(obj, range, StringLength, StringSubstring);

                        var f = Expression.Lambda<Func<string>>(expr).Compile();

                        AssertEval(actual, f);
                    }
                }

                // RangeExpression with non-constant Index operands

                {
                    var startExpr = Expression.Invoke(Expression.Constant(new Func<Index>(() => rng.Start)));
                    var endExpr = Expression.Invoke(Expression.Constant(new Func<Index>(() => rng.End)));

                    var obj = Expression.Constant(str);

                    foreach (var range in GetRangeExprs(rng, startExpr, endExpr))
                    {
                        var expr = CSharpExpression.IndexerAccess(obj, range, StringLength, StringSubstring);

                        var f = Expression.Lambda<Func<string>>(expr).Compile();

                        AssertEval(actual, f);
                    }
                }

                // Helper function to wrap a value

                Expression WrapValue<T>(T obj, bool useConst)
                {
                    return useConst ? (Expression)Expression.Constant(obj) : Expression.Invoke(Expression.Constant(new Func<T>(() => obj)));
                }

                // Helper function to get a FromEndExpression, when applicable

                Expression GetHat(Index i, bool useConst)
                {
                    return i.IsFromEnd ? CSharpExpression.FromEndIndex(WrapValue(i.Value, useConst)) : WrapValue(i, useConst);
                }

                // RangeExpression with FromEndExpression

                foreach (var useConst in new[] { true, false })
                {
                    var startExpr = GetHat(rng.Start, useConst);
                    var endExpr = GetHat(rng.End, useConst);

                    var obj = Expression.Constant(str);

                    foreach (var range in GetRangeExprs(rng, startExpr, endExpr))
                    {
                        var expr = CSharpExpression.IndexerAccess(obj, range, StringLength, StringSubstring);

                        var f = Expression.Lambda<Func<string>>(expr).Compile();

                        AssertEval(actual, f);
                    }
                }

                // Helper function to get int conversion expression, when applicable

                Expression GetIntToIndexConvert(Index i, bool useConst)
                {
                    return i.IsFromEnd ? WrapValue(i, useConst) : Expression.Convert(WrapValue(i.Value, useConst), typeof(Index));
                }

                // RangeExpression with int conversions

                foreach (var useConst in new[] { true, false })
                {
                    var startExpr = GetIntToIndexConvert(rng.Start, useConst);
                    var endExpr = GetIntToIndexConvert(rng.End, useConst);

                    var obj = Expression.Constant(str);

                    foreach (var range in GetRangeExprs(rng, startExpr, endExpr))
                    {
                        var expr = CSharpExpression.IndexerAccess(obj, range, StringLength, StringSubstring);

                        var f = Expression.Lambda<Func<string>>(expr).Compile();

                        AssertEval(actual, f);
                    }
                }
            }
        }

        [Fact]
        public void IndexerAccess_Basics_String_Range_SideEffects()
        {
            var str = "foobar";

            // str[range]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.Constant(str));
                var range = Expression.Block(log("R"), Expression.Constant(Range.All));

                return CSharpExpression.IndexerAccess(obj, range, StringLength, StringSubstring);
            }, new LogAndResult<string> { Log = { "O", "R" }, Value = str });

            // str[i..j]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.Constant(str));
                var start = Expression.Block(log("S"), Expression.Constant(new Index(0)));
                var end = Expression.Block(log("E"), Expression.Constant(new Index(0, fromEnd: true)));
                var range = CSharpExpression.Range(start, end);

                return CSharpExpression.IndexerAccess(obj, range, StringLength, StringSubstring);
            }, new LogAndResult<string> { Log = { "O", "S", "E" }, Value = str });

            // str[i..^j]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.Constant(str));
                var start = Expression.Block(log("S"), Expression.Constant(new Index(0)));
                var end = CSharpExpression.FromEndIndex(Expression.Block(log("E"), Expression.Constant(0)));
                var range = CSharpExpression.Range(start, end);

                return CSharpExpression.IndexerAccess(obj, range, StringLength, StringSubstring);
            }, new LogAndResult<string> { Log = { "O", "S", "E" }, Value = str });

            // str[^i..j]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.Constant(str));
                var start = CSharpExpression.FromEndIndex(Expression.Block(log("S"), Expression.Constant(str.Length)));
                var end = Expression.Block(log("E"), Expression.Constant(new Index(0, fromEnd: true)));
                var range = CSharpExpression.Range(start, end);

                return CSharpExpression.IndexerAccess(obj, range, StringLength, StringSubstring);
            }, new LogAndResult<string> { Log = { "O", "S", "E" }, Value = str });

            // str[^i..^j]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.Constant(str));
                var start = CSharpExpression.FromEndIndex(Expression.Block(log("S"), Expression.Constant(str.Length)));
                var end = CSharpExpression.FromEndIndex(Expression.Block(log("E"), Expression.Constant(0)));
                var range = CSharpExpression.Range(start, end);

                return CSharpExpression.IndexerAccess(obj, range, StringLength, StringSubstring);
            }, new LogAndResult<string> { Log = { "O", "S", "E" }, Value = str });
        }


        [Fact]
        public void IndexerAccess_Range_SideEffects()
        {
            var ctor = typeof(SliceAndIndexString).GetConstructors().Single();

            var str = "foobar";

            // str[range]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var range = Expression.Block(log("R"), Expression.Constant(Range.All));

                return CSharpExpression.IndexerAccess(obj, range, SliceAndIndexStringLength, SliceAndIndexStringSlice);
            }, new LogAndResult<string> { Log = { "O", "Length", "R", "Slice(0,6)" }, Value = str });

            // NB: When index expressions in a RangeExpression are opaque, we always need Length to get the offset.

            // str[i..j]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var start = Expression.Block(log("S"), Expression.Constant(new Index(0)));
                var end = Expression.Block(log("E"), Expression.Constant(new Index(0, fromEnd: true)));
                var range = CSharpExpression.Range(start, end);

                return CSharpExpression.IndexerAccess(obj, range, SliceAndIndexStringLength, SliceAndIndexStringSlice);
            }, new LogAndResult<string> { Log = { "O", "Length", "S", "E", "Slice(0,6)" }, Value = str });

            // str[i..^j]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var start = Expression.Block(log("S"), Expression.Constant(new Index(0)));
                var end = CSharpExpression.FromEndIndex(Expression.Block(log("E"), Expression.Constant(0)));
                var range = CSharpExpression.Range(start, end);

                return CSharpExpression.IndexerAccess(obj, range, SliceAndIndexStringLength, SliceAndIndexStringSlice);
            }, new LogAndResult<string> { Log = { "O", "Length", "S", "E", "Slice(0,6)" }, Value = str });

            // str[^i..j]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var start = CSharpExpression.FromEndIndex(Expression.Block(log("S"), Expression.Constant(str.Length)));
                var end = Expression.Block(log("E"), Expression.Constant(new Index(0, fromEnd: true)));
                var range = CSharpExpression.Range(start, end);

                return CSharpExpression.IndexerAccess(obj, range, SliceAndIndexStringLength, SliceAndIndexStringSlice);
            }, new LogAndResult<string> { Log = { "O", "Length", "S", "E", "Slice(0,6)" }, Value = str });

            // str[^i..^j]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var start = CSharpExpression.FromEndIndex(Expression.Block(log("S"), Expression.Constant(str.Length)));
                var end = CSharpExpression.FromEndIndex(Expression.Block(log("E"), Expression.Constant(0)));
                var range = CSharpExpression.Range(start, end);

                return CSharpExpression.IndexerAccess(obj, range, SliceAndIndexStringLength, SliceAndIndexStringSlice);
            }, new LogAndResult<string> { Log = { "O", "Length", "S", "E", "Slice(0,6)" }, Value = str });

            // NB: When index expressions in a RangeExpression are transparent, we may be able to elide use of Length to get the offset.

            // str[1..4]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var start = Expression.Constant(new Index(1));
                var end = Expression.Constant(new Index(4));
                var range = CSharpExpression.Range(start, end);

                return CSharpExpression.IndexerAccess(obj, range, SliceAndIndexStringLength, SliceAndIndexStringSlice);
            }, new LogAndResult<string> { Log = { "O", "Slice(1,3)" }, Value = str.Substring(1, 3) });

            // str[1..^2]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var start = Expression.Constant(new Index(1));
                var end = Expression.Constant(new Index(2, fromEnd: true));
                var range = CSharpExpression.Range(start, end);

                return CSharpExpression.IndexerAccess(obj, range, SliceAndIndexStringLength, SliceAndIndexStringSlice);
            }, new LogAndResult<string> { Log = { "O", "Length", "Slice(1,3)" }, Value = str.Substring(1, 3) });

            // str[^5..4]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var start = Expression.Constant(new Index(5, fromEnd: true));
                var end = Expression.Constant(new Index(4));
                var range = CSharpExpression.Range(start, end);

                return CSharpExpression.IndexerAccess(obj, range, SliceAndIndexStringLength, SliceAndIndexStringSlice);
            }, new LogAndResult<string> { Log = { "O", "Length", "Slice(1,3)" }, Value = str.Substring(1, 3) });

            // str[^5..^2]

            AssertCompile<string>((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.New(ctor, Expression.Constant(str), append));
                var start = Expression.Constant(new Index(5, fromEnd: true));
                var end = Expression.Constant(new Index(2, fromEnd: true));
                var range = CSharpExpression.Range(start, end);

                return CSharpExpression.IndexerAccess(obj, range, SliceAndIndexStringLength, SliceAndIndexStringSlice);
            }, new LogAndResult<string> { Log = { "O", "Length", "Slice(1,3)" }, Value = str.Substring(1, 3) });
        }

        [Fact]
        public void IndexerAccess_Index_Assign()
        {
            var SliceAndIndexStringCtor = typeof(SliceAndIndexString).GetConstructors().Single();
            var SliceAndIndexListCtor = typeof(SliceAndIndexList).GetConstructors().Single();

            var str = "foobar";

            // str[i] = 'p'

            AssertCompile<string>((log, append) =>
            {
                var var = Expression.Parameter(typeof(SliceAndIndexString));

                var obj = Expression.Block(log("O"), var);
                var index = Expression.Block(log("I"), Expression.Constant(new Index(2)));

                var it = CSharpExpression.IndexerAccess(obj, index, SliceAndIndexStringLength, SliceAndIndexStringIndexer);

                return Expression.Block(
                    new[] { var },
                    Expression.Assign(var, Expression.New(SliceAndIndexStringCtor, Expression.Constant(str), append)),
                    CSharpExpression.Assign(it, Expression.Constant('p')),
                    Expression.Property(var, SliceAndIndexStringValue)
                );
            }, new LogAndResult<string> { Log = { "O", "Length", "I", "this[2] set(p)" }, Value = "fopbar" });

            // str[2] = 'p'

            AssertCompile<string>((log, append) =>
            {
                var var = Expression.Parameter(typeof(SliceAndIndexString));

                var obj = Expression.Block(log("O"), var);
                var index = Expression.Constant(new Index(2));

                var it = CSharpExpression.IndexerAccess(obj, index, SliceAndIndexStringLength, SliceAndIndexStringIndexer);

                return Expression.Block(
                    new[] { var },
                    Expression.Assign(var, Expression.New(SliceAndIndexStringCtor, Expression.Constant(str), append)),
                    CSharpExpression.Assign(it, Expression.Constant('p')),
                    Expression.Property(var, SliceAndIndexStringValue)
                );
            }, new LogAndResult<string> { Log = { "O", "this[2] set(p)" }, Value = "fopbar" });

            // str[^4] = 'p'

            AssertCompile<string>((log, append) =>
            {
                var var = Expression.Parameter(typeof(SliceAndIndexString));

                var obj = Expression.Block(log("O"), var);
                var index = Expression.Constant(new Index(4, fromEnd: true));

                var it = CSharpExpression.IndexerAccess(obj, index, SliceAndIndexStringLength, SliceAndIndexStringIndexer);

                return Expression.Block(
                    new[] { var },
                    Expression.Assign(var, Expression.New(SliceAndIndexStringCtor, Expression.Constant(str), append)),
                    CSharpExpression.Assign(it, Expression.Constant('p')),
                    Expression.Property(var, SliceAndIndexStringValue)
                );
            }, new LogAndResult<string> { Log = { "O", "Length", "this[2] set(p)" }, Value = "fopbar" });

            var xs = new List<int> { 2, 3, 5, 7, 11, 13 };

            // xs[i] = -1

            AssertCompile<string>((log, append) =>
            {
                var var = Expression.Parameter(typeof(SliceAndIndexList));

                var obj = Expression.Block(log("O"), var);
                var index = Expression.Block(log("I"), Expression.Constant(new Index(2)));

                var it = CSharpExpression.IndexerAccess(obj, index, SliceAndIndexListLength, SliceAndIndexListIndexer);

                return Expression.Block(
                    new[] { var },
                    Expression.Assign(var, Expression.New(SliceAndIndexListCtor, Expression.Constant(xs), append)),
                    CSharpExpression.Assign(it, Expression.Constant(-1)),
                    Expression.Property(var, SliceAndIndexListValue)
                );
            }, new LogAndResult<string> { Log = { "O", "Length", "I", "this[2] set(-1)" }, Value = "{2,3,-1,7,11,13}" });

            // xs[i] *= 2

            AssertCompile<string>((log, append) =>
            {
                var var = Expression.Parameter(typeof(SliceAndIndexList));

                var obj = Expression.Block(log("O"), var);
                var index = Expression.Block(log("I"), Expression.Constant(new Index(2)));

                var it = CSharpExpression.IndexerAccess(obj, index, SliceAndIndexListLength, SliceAndIndexListIndexer);

                return Expression.Block(
                    new[] { var },
                    Expression.Assign(var, Expression.New(SliceAndIndexListCtor, Expression.Constant(xs), append)),
                    CSharpExpression.MultiplyAssign(it, Expression.Block(log("M"), Expression.Constant(2))),
                    Expression.Property(var, SliceAndIndexListValue)
                );
            }, new LogAndResult<string> { Log = { "O", "Length", "I", "this[2] get()", "M", "this[2] set(10)" }, Value = "{2,3,10,7,11,13}" });

            // xs[2] *= 2

            AssertCompile<string>((log, append) =>
            {
                var var = Expression.Parameter(typeof(SliceAndIndexList));

                var obj = Expression.Block(log("O"), var);
                var index = Expression.Constant(new Index(2));

                var it = CSharpExpression.IndexerAccess(obj, index, SliceAndIndexListLength, SliceAndIndexListIndexer);

                return Expression.Block(
                    new[] { var },
                    Expression.Assign(var, Expression.New(SliceAndIndexListCtor, Expression.Constant(xs), append)),
                    CSharpExpression.MultiplyAssign(it, Expression.Block(log("M"), Expression.Constant(2))),
                    Expression.Property(var, SliceAndIndexListValue)
                );
            }, new LogAndResult<string> { Log = { "O", "this[2] get()", "M", "this[2] set(10)" }, Value = "{2,3,10,7,11,13}" });

            // xs[^4] *= 2

            AssertCompile<string>((log, append) =>
            {
                var var = Expression.Parameter(typeof(SliceAndIndexList));

                var obj = Expression.Block(log("O"), var);
                var index = Expression.Constant(new Index(4, fromEnd: true));

                var it = CSharpExpression.IndexerAccess(obj, index, SliceAndIndexListLength, SliceAndIndexListIndexer);

                return Expression.Block(
                    new[] { var },
                    Expression.Assign(var, Expression.New(SliceAndIndexListCtor, Expression.Constant(xs), append)),
                    CSharpExpression.MultiplyAssign(it, Expression.Block(log("M"), Expression.Constant(2))),
                    Expression.Property(var, SliceAndIndexListValue)
                );
            }, new LogAndResult<string> { Log = { "O", "Length", "this[2] get()", "M", "this[2] set(10)" }, Value = "{2,3,10,7,11,13}" });
        }

        [Fact]
        public void IndexerAccess_Index_MutableStruct()
        {
            var SliceAndIndexListStructCtor = typeof(SliceAndIndexListStruct).GetConstructors().Single();
            var SliceAndIndexListStructLog = typeof(SliceAndIndexListStruct).GetProperty(nameof(SliceAndIndexListStruct.Log));

            var xs = new List<int> { 2, 3, 5, 7, 11, 13 };

            AssertCompile<string>((log, append) =>
            {
                var var = Expression.Parameter(typeof(SliceAndIndexListStruct));

                var obj = Expression.Block(log("O"), var);
                var index = Expression.Block(log("I"), Expression.Constant(new Index(2)));

                var it = CSharpExpression.IndexerAccess(obj, index, SliceAndIndexListStructLength, SliceAndIndexListStructIndexer);

                return Expression.Block(
                    new[] { var },
                    Expression.Assign(var, Expression.New(SliceAndIndexListStructCtor, Expression.Constant(xs), append)),
                    it,
                    Expression.Property(var, SliceAndIndexListStructLog)
                );
            }, new LogAndResult<string> { Log = { "O", "I", "Length", "this[2] get()" }, Value = "Length;this[2] get();" });
        }

        private static (int Offset, int Length) GetOffsetAndLengthNoFail(Range range, int length)
        {
            int start;
            Index startIndex = range.Start;
            if (startIndex.IsFromEnd)
                start = length - startIndex.Value;
            else
                start = startIndex.Value;

            int end;
            Index endIndex = range.End;
            if (endIndex.IsFromEnd)
                end = length - endIndex.Value;
            else
                end = endIndex.Value;

            return (start, end - start);
        }

        private static void AssertEval<T>(Func<T> expected, Func<T> actual)
        {
            Type expectedException = null;
            T expectedVal = default;

            try
            {
                expectedVal = expected();
            }
            catch (Exception e)
            {
                expectedException = e.GetType();
            }

            Type actualException = null;
            T actualVal = default;

            try
            {
                actualVal = actual();
            }
            catch (Exception e)
            {
                actualException = e.GetType();
            }

            if (expectedException == null)
            {
                Assert.Null(actualException);
                Assert.Equal(expectedVal, actualVal);
            }
            else
            {
                Assert.Equal(expectedException, actualException);
            }
        }

        private void AssertCompile<T>(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<T> expected)
        {
            var res = WithLog<T>(createExpression).Compile()();
            Assert.Equal(expected, res);
        }

        class C
        {
            public int this[int index] => 0;

            public int Length { get; set; }

#pragma warning disable CS0649 // Assigned through expression trees.
            public int LengthField;
#pragma warning restore

            public string LengthString { get; set; }

            public int LengthWriteOnly { set { } }

            public static int LengthStatic => 0;

            public int this[int i, int j] => 0;

            public int this[string x] => 0;

            public string Slice(int x, int y) => null;

            public static string SliceStatic(int x, int y) => null;

            public string Slice0() => null;
            public string Slice1(int x) => null;
            public string Slice2X(int x, bool b) => null;
            public string Slice2Y(bool b, int y) => null;
            public string Slice3(int x, int y, int z) => null;
        }

        class SliceAndIndexString
        {
            private string _value;
            private readonly Action<string> _log;

            public SliceAndIndexString(string value, Action<string> log)
            {
                _value = value;
                _log = log;
            }

            public int Length
            {
                get
                {
                    _log("Length");
                    return _value.Length;
                }
            }

            public char this[int i]
            {
                get
                {
                    _log($"this[{i}] get()");
                    return _value[i];
                }

                set
                {
                    _log($"this[{i}] set({value})");
                    var sb = new StringBuilder(_value);
                    sb[i] = value;
                    _value = sb.ToString();
                }
            }

            public string Slice(int x, int y)
            {
                _log($"Slice({x},{y})");
                return _value.Substring(x, y);
            }

            public string Value => _value;
        }

        class SliceAndIndexList
        {
            private readonly List<int> _value;
            private readonly Action<string> _log;

            public SliceAndIndexList(IEnumerable<int> value, Action<string> log)
            {
                _value = value.ToList(); // NB: Copy to ensure stability across tests
                _log = log;
            }

            public int Length
            {
                get
                {
                    _log("Length");
                    return _value.Count;
                }
            }

            public int this[int i]
            {
                get
                {
                    _log($"this[{i}] get()");
                    return _value[i];
                }

                set
                {
                    _log($"this[{i}] set({value})");
                    _value[i] = value;
                }
            }

            public List<int> Slice(int x, int y)
            {
                _log($"Slice({x},{y})");
                return _value.Skip(x).Take(y).ToList();
            }

            public string Value => "{" + string.Join(",", _value) + "}";
        }

        struct SliceAndIndexListStruct
        {
            private readonly List<int> _value;
            private readonly Action<string> _log;
            private readonly StringBuilder _sb;

            public SliceAndIndexListStruct(IEnumerable<int> value, Action<string> log)
            {
                _value = value.ToList(); // NB: Copy to ensure stability across tests
                var sb = new StringBuilder();
                _sb = sb;
                _log = log + new Action<string>(str => sb.Append(str).Append(';'));
            }

            public string Log => _sb.ToString();

            public int Length
            {
                get
                {
                    _log("Length");
                    return _value.Count;
                }
            }

            public int this[int i]
            {
                get
                {
                    _log($"this[{i}] get()");
                    return _value[i];
                }

                set
                {
                    _log($"this[{i}] set({value})");
                    _value[i] = value;
                }
            }

            public List<int> Slice(int x, int y)
            {
                _log($"Slice({x},{y})");
                return _value.Skip(x).Take(y).ToList();
            }

            public string Value => "{" + string.Join(",", _value) + "}";
        }
    }
}
