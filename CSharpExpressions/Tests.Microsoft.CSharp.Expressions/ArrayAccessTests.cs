// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public partial class ArrayAccessTests
    {
        [TestMethod]
        public void ArrayAccess_Factory_ArgumentChecking()
        {
            var xs = Expression.Parameter(typeof(int[]));
            var xss = Expression.Parameter(typeof(int[,]));

            var i = Expression.Constant(1);
            var s = Expression.Constant("foo");
            var j = Expression.Constant(new Index());
            var r = Expression.Constant(new Range());

            // the following are valid
            Assert.IsNotNull(CSharpExpression.ArrayAccess(xs, i));
            Assert.IsNotNull(CSharpExpression.ArrayAccess(xs, j));
            Assert.IsNotNull(CSharpExpression.ArrayAccess(xs, r));
            Assert.IsNotNull(CSharpExpression.ArrayAccess(xss, i, i));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ArrayAccess(xs, s)); // invalid index type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ArrayAccess(xss, i)); // wrong rank
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ArrayAccess(xss, j, j)); // no Index support for multi-dimensional
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ArrayAccess(xss, r, r)); // no Range support for multi-dimensional
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ArrayAccess(s, i)); // invalid array type
        }

        // TODO: Tests when used in assignment positions, e.g. xs[1..2] = i is invalid.

        [TestMethod]
        public void ArrayAccess_Factory_Properties()
        {
            var xs = Expression.Parameter(typeof(string[]));
            var xss = Expression.Parameter(typeof(string[,]));

            var i = Expression.Constant(1);
            var k = Expression.Constant(2);
            var j = Expression.Constant(new Index());
            var r = Expression.Constant(new Range());

            var a1 = CSharpExpression.ArrayAccess(xs, i);
            Assert.AreEqual(CSharpExpressionType.ArrayAccess, a1.CSharpNodeType);
            Assert.AreEqual(typeof(string), a1.Type);
            Assert.AreSame(xs, a1.Array);
            Assert.AreEqual(1, a1.Indexes.Count);
            Assert.AreSame(i, a1.Indexes[0]);

            var a2 = CSharpExpression.ArrayAccess(xs, j);
            Assert.AreEqual(CSharpExpressionType.ArrayAccess, a2.CSharpNodeType);
            Assert.AreEqual(typeof(string), a2.Type);
            Assert.AreSame(xs, a2.Array);
            Assert.AreEqual(1, a2.Indexes.Count);
            Assert.AreSame(j, a2.Indexes[0]);

            var a3 = CSharpExpression.ArrayAccess(xs, r);
            Assert.AreEqual(CSharpExpressionType.ArrayAccess, a3.CSharpNodeType);
            Assert.AreEqual(typeof(string[]), a3.Type);
            Assert.AreSame(xs, a3.Array);
            Assert.AreEqual(1, a3.Indexes.Count);
            Assert.AreSame(r, a3.Indexes[0]);

            var a4 = CSharpExpression.ArrayAccess(xss, i, k);
            Assert.AreEqual(CSharpExpressionType.ArrayAccess, a4.CSharpNodeType);
            Assert.AreEqual(typeof(string), a4.Type);
            Assert.AreSame(xss, a4.Array);
            Assert.AreEqual(2, a4.Indexes.Count);
            Assert.AreSame(i, a4.Indexes[0]);
            Assert.AreSame(k, a4.Indexes[1]);
        }

        [TestMethod]
        public void ArrayAccess_Update()
        {
            var xs = Expression.Parameter(typeof(string[]));
            var xss = Expression.Parameter(typeof(string[,]));

            var ys = Expression.Parameter(typeof(string[]));
            var yss = Expression.Parameter(typeof(string[,]));

            var i1 = Expression.Constant(1);
            var j1 = Expression.Constant(2);

            var i2 = Expression.Constant(1);
            var j2 = Expression.Constant(2);

            var a1 = CSharpExpression.ArrayAccess(xs, i1);
            Assert.AreSame(a1, a1.Update(a1.Array, a1.Indexes));

            var a2 = CSharpExpression.ArrayAccess(xss, i1, j1);
            Assert.AreSame(a2, a2.Update(a2.Array, a2.Indexes));

            var a3 = a1.Update(ys, a1.Indexes);
            Assert.AreSame(ys, a3.Array);
            Assert.AreSame(a1.Indexes[0], a3.Indexes[0]);

            var a4 = a1.Update(a1.Array, new[] { i2 });
            Assert.AreSame(a1.Array, a4.Array);
            Assert.AreSame(i2, a4.Indexes[0]);

            var a5 = a2.Update(yss, a2.Indexes);
            Assert.AreSame(yss, a5.Array);
            Assert.AreSame(a2.Indexes[0], a5.Indexes[0]);
            Assert.AreSame(a2.Indexes[1], a5.Indexes[1]);

            var a6 = a2.Update(a2.Array, new[] { i2, j2 });
            Assert.AreSame(a2.Array, a6.Array);
            Assert.AreSame(i2, a6.Indexes[0]);
            Assert.AreSame(j2, a6.Indexes[1]);
        }

        [TestMethod]
        public void ArrayAccess_Visitor()
        {
            var xs = Expression.Parameter(typeof(string[]));
            var i = Expression.Constant(1);

            var res = CSharpExpression.ArrayAccess(xs, i);

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitArrayAccess(ArrayAccessCSharpExpression node)
            {
                Visited = true;

                return base.VisitArrayAccess(node);
            }
        }

        [TestMethod]
        public void ArrayAccess_Reduce_Single_Int32()
        {
            var xs = Expression.Parameter(typeof(string[]));
            var i = Expression.Constant(1);

            var res = CSharpExpression.ArrayAccess(xs, i);

            var red = res.Reduce();

            Assert.IsTrue(red is IndexExpression);

            var idx = (IndexExpression)red;
            Assert.AreSame(xs, idx.Object);
            Assert.IsNull(idx.Indexer);
            Assert.AreEqual(1, idx.Arguments.Count);
            Assert.AreSame(i, idx.Arguments[0]);
        }

        [TestMethod]
        public void ArrayAccess_Reduce_Multi_Int32()
        {
            var xss = Expression.Parameter(typeof(string[,]));
            var i = Expression.Constant(1);
            var j = Expression.Constant(2);

            var res = CSharpExpression.ArrayAccess(xss, i, j);

            var red = res.Reduce();

            Assert.IsTrue(red is IndexExpression);

            var idx = (IndexExpression)red;
            Assert.AreSame(xss, idx.Object);
            Assert.IsNull(idx.Indexer);
            Assert.AreEqual(2, idx.Arguments.Count);
            Assert.AreSame(i, idx.Arguments[0]);
            Assert.AreSame(j, idx.Arguments[1]);
        }

        [TestMethod]
        public void ArrayAccess_Read_Int32()
        {
            var xs = Expression.Parameter(typeof(string[]));
            var i = Expression.Parameter(typeof(int));

            var res = Expression.Lambda<Func<string[], int, string>>(CSharpExpression.ArrayAccess(xs, i), xs, i);

            var f = res.Compile();

            var vals = new string[] { "bar", "foo", "qux" };

            Assert.AreEqual(vals[0], f(vals, 0));
            Assert.AreEqual(vals[1], f(vals, 1));
            Assert.AreEqual(vals[2], f(vals, 2));
        }

        [TestMethod]
        public void ArrayAccess_Assign_Int32()
        {
            var xs = Expression.Parameter(typeof(string[]));
            var i = Expression.Parameter(typeof(int));
            var s = Expression.Parameter(typeof(string));

            var res = Expression.Lambda<Func<string[], int, string, string>>(CSharpExpression.Assign(CSharpExpression.ArrayAccess(xs, i), s), xs, i, s);

            var f = res.Compile();

            var vals = new string[] { "bar", "foo", "qux" };

            for (int j = 0; j < vals.Length; j++)
            {
                var newVal = vals[j].ToUpper();

                var assignRes = f(vals, j, newVal);

                Assert.AreEqual(newVal, assignRes);
                Assert.AreEqual(newVal, vals[j]);
            }
        }

        [TestMethod]
        public void ArrayAccess_CompoundAssign_Int32()
        {
            var xs = Expression.Parameter(typeof(int[]));
            var i = Expression.Parameter(typeof(int));

            var res = Expression.Lambda<Func<int[], int, int>>(CSharpExpression.AddAssign(CSharpExpression.ArrayAccess(xs, i), Expression.Constant(1)), xs, i);

            var f = res.Compile();

            var vals = new int[] { 2, 3, 5, 7 };

            for (int j = 0; j < vals.Length; j++)
            {
                var newVal = vals[j] + 1;

                var assignRes = f(vals, j);

                Assert.AreEqual(newVal, assignRes);
                Assert.AreEqual(newVal, vals[j]);
            }
        }

        [TestMethod]
        public void ArrayAccess_UnaryAssign_Post_Int32()
        {
            var xs = Expression.Parameter(typeof(int[]));
            var i = Expression.Parameter(typeof(int));

            var res = Expression.Lambda<Func<int[], int, int>>(CSharpExpression.PostIncrementAssign(CSharpExpression.ArrayAccess(xs, i)), xs, i);

            var f = res.Compile();

            var vals = new int[] { 2, 3, 5, 7 };

            for (int j = 0; j < vals.Length; j++)
            {
                var oldVal = vals[j];
                var newVal = oldVal + 1;

                var assignRes = f(vals, j);

                Assert.AreEqual(oldVal, assignRes);
                Assert.AreEqual(newVal, vals[j]);
            }
        }

        [TestMethod]
        public void ArrayAccess_UnaryAssign_Pre_Int32()
        {
            var xs = Expression.Parameter(typeof(int[]));
            var i = Expression.Parameter(typeof(int));

            var res = Expression.Lambda<Func<int[], int, int>>(CSharpExpression.PreIncrementAssign(CSharpExpression.ArrayAccess(xs, i)), xs, i);

            var f = res.Compile();

            var vals = new int[] { 2, 3, 5, 7 };

            for (int j = 0; j < vals.Length; j++)
            {
                var newVal = vals[j] + 1;

                var assignRes = f(vals, j);

                Assert.AreEqual(newVal, assignRes);
                Assert.AreEqual(newVal, vals[j]);
            }
        }

        [TestMethod]
        public void ArrayAccess_Read_SystemIndex()
        {
            var xs = Expression.Parameter(typeof(string[]));
            var i = Expression.Parameter(typeof(Index));

            var res = Expression.Lambda<Func<string[], Index, string>>(CSharpExpression.ArrayAccess(xs, i), xs, i);

            var f = res.Compile();

            var vals = new string[] { "bar", "foo", "qux" };

            Assert.AreEqual(vals[0], f(vals, new Index()));

            Assert.AreEqual(vals[0], f(vals, new Index(0)));
            Assert.AreEqual(vals[1], f(vals, new Index(1)));
            Assert.AreEqual(vals[2], f(vals, new Index(2)));

            Assert.AreEqual(vals[0], f(vals, new Index(3, fromEnd: true)));
            Assert.AreEqual(vals[1], f(vals, new Index(2, fromEnd: true)));
            Assert.AreEqual(vals[2], f(vals, new Index(1, fromEnd: true)));
        }

        [TestMethod]
        public void ArrayAccess_Assign_SystemIndex()
        {
            var xs = Expression.Parameter(typeof(string[]));
            var i = Expression.Parameter(typeof(Index));
            var s = Expression.Parameter(typeof(string));

            var res = Expression.Lambda<Func<string[], Index, string, string>>(CSharpExpression.Assign(CSharpExpression.ArrayAccess(xs, i), s), xs, i, s);

            var f = res.Compile();

            var vals = new string[] { "bar", "foo", "qux" };

            for (int j = 0; j < vals.Length; j++)
            {
                var newVal = vals[j].ToUpper();

                var assignRes = f(vals, new Index(j), newVal);

                Assert.AreEqual(newVal, assignRes);
                Assert.AreEqual(newVal, vals[j]);
            }

            for (int j = 1; j <= vals.Length; j++)
            {
                var k = new Index(j, fromEnd: true);

                var newVal = vals[k].ToLower();

                var assignRes = f(vals, k, newVal);

                Assert.AreEqual(newVal, assignRes);
                Assert.AreEqual(newVal, vals[k]);
            }
        }

        [TestMethod]
        public void ArrayAccess_CompoundAssign_SystemIndex()
        {
            var xs = Expression.Parameter(typeof(int[]));
            var i = Expression.Parameter(typeof(Index));

            var res = Expression.Lambda<Func<int[], Index, int>>(CSharpExpression.MultiplyAssign(CSharpExpression.ArrayAccess(xs, i), Expression.Constant(2)), xs, i);

            var f = res.Compile();

            var vals = new int[] { 2, 3, 5, 7 };

            for (int j = 0; j < vals.Length; j++)
            {
                var newVal = vals[j] * 2;

                var assignRes = f(vals, new Index(j));

                Assert.AreEqual(newVal, assignRes);
                Assert.AreEqual(newVal, vals[j]);
            }

            for (int j = 1; j <= vals.Length; j++)
            {
                var k = new Index(j, fromEnd: true);

                var newVal = vals[k] * 2;

                var assignRes = f(vals, k);

                Assert.AreEqual(newVal, assignRes);
                Assert.AreEqual(newVal, vals[k]);
            }
        }

        [TestMethod]
        public void ArrayAccess_UnaryAssign_Post_SystemIndex()
        {
            var xs = Expression.Parameter(typeof(int[]));
            var i = Expression.Parameter(typeof(Index));

            var res = Expression.Lambda<Func<int[], Index, int>>(CSharpExpression.PostIncrementAssign(CSharpExpression.ArrayAccess(xs, i)), xs, i);

            var f = res.Compile();

            var vals = new int[] { 2, 3, 5, 7 };

            for (int j = 0; j < vals.Length; j++)
            {
                var oldVal = vals[j];
                var newVal = oldVal + 1;

                var assignRes = f(vals, new Index(j));

                Assert.AreEqual(oldVal, assignRes);
                Assert.AreEqual(newVal, vals[j]);
            }

            for (int j = 1; j <= vals.Length; j++)
            {
                var k = new Index(j, fromEnd: true);

                var oldVal = vals[k];
                var newVal = oldVal + 1;

                var assignRes = f(vals, k);

                Assert.AreEqual(oldVal, assignRes);
                Assert.AreEqual(newVal, vals[k]);
            }
        }

        [TestMethod]
        public void ArrayAccess_UnaryAssign_Pre_SystemIndex()
        {
            var xs = Expression.Parameter(typeof(int[]));
            var i = Expression.Parameter(typeof(Index));

            var res = Expression.Lambda<Func<int[], Index, int>>(CSharpExpression.PreIncrementAssign(CSharpExpression.ArrayAccess(xs, i)), xs, i);

            var f = res.Compile();

            var vals = new int[] { 2, 3, 5, 7 };

            for (int j = 0; j < vals.Length; j++)
            {
                var newVal = vals[j] + 1;

                var assignRes = f(vals, new Index(j));

                Assert.AreEqual(newVal, assignRes);
                Assert.AreEqual(newVal, vals[j]);
            }

            for (int j = 1; j <= vals.Length; j++)
            {
                var k = new Index(j, fromEnd: true);

                var newVal = vals[k] + 1;

                var assignRes = f(vals, k);

                Assert.AreEqual(newVal, assignRes);
                Assert.AreEqual(newVal, vals[k]);
            }
        }

        [TestMethod]
        public void ArrayAccess_Read_FromEnd()
        {
            var xs = Expression.Parameter(typeof(string[]));
            var i = Expression.Parameter(typeof(int));

            var res = Expression.Lambda<Func<string[], int, string>>(CSharpExpression.ArrayAccess(xs, CSharpExpression.FromEndIndex(i)), xs, i);

            var f = res.Compile();

            var vals = new string[] { "bar", "foo", "qux" };

            Assert.AreEqual(vals[0], f(vals, 3));
            Assert.AreEqual(vals[1], f(vals, 2));
            Assert.AreEqual(vals[2], f(vals, 1));
        }

        [TestMethod]
        public void ArrayAccess_Assign_FromEnd()
        {
            var xs = Expression.Parameter(typeof(string[]));
            var i = Expression.Parameter(typeof(int));
            var s = Expression.Parameter(typeof(string));

            var res = Expression.Lambda<Func<string[], int, string, string>>(CSharpExpression.Assign(CSharpExpression.ArrayAccess(xs, CSharpExpression.FromEndIndex(i)), s), xs, i, s);

            var f = res.Compile();

            var vals = new string[] { "bar", "foo", "qux" };

            for (int j = 1; j <= vals.Length; j++)
            {
                var k = Index.FromEnd(j);

                var newVal = vals[k].ToUpper();

                var assignRes = f(vals, j, newVal);

                Assert.AreEqual(newVal, assignRes);
                Assert.AreEqual(newVal, vals[k]);
            }
        }

        [TestMethod]
        public void ArrayAccess_Read_SystemRange()
        {
            var xs = Expression.Parameter(typeof(string[]));
            var i = Expression.Parameter(typeof(Range));

            var res = Expression.Lambda<Func<string[], Range, string[]>>(CSharpExpression.ArrayAccess(xs, i), xs, i);

            var f = res.Compile();

            var vals = new string[] { "bar", "foo", "qux" };

            Assert.IsTrue(vals.SequenceEqual(f(vals, Range.All)));

            Assert.IsTrue(vals.Skip(0).SequenceEqual(f(vals, Range.StartAt(0))));
            Assert.IsTrue(vals.Skip(1).SequenceEqual(f(vals, Range.StartAt(1))));
            Assert.IsTrue(vals.Skip(2).SequenceEqual(f(vals, Range.StartAt(2))));

            Assert.IsTrue(vals.Take(0).SequenceEqual(f(vals, Range.EndAt(0))));
            Assert.IsTrue(vals.Take(1).SequenceEqual(f(vals, Range.EndAt(1))));
            Assert.IsTrue(vals.Take(2).SequenceEqual(f(vals, Range.EndAt(2))));
        }

        [TestMethod]
        [Ignore] // BUG: Lowered form of ArrayAccess produces a Block which doesn't get passed by ref correctly.
        public void ArrayAccess_ByRef()
        {
            var xs = Expression.Parameter(typeof(int[]));
            var i = Expression.Parameter(typeof(Index));

            var inc = typeof(Interlocked).GetMethod(nameof(Interlocked.Increment), new[] { typeof(int).MakeByRefType() });

            var res = Expression.Lambda<Func<int[], Index, int>>(Expression.Call(inc, CSharpExpression.ArrayAccess(xs, i)), xs, i);

            var f = res.Compile();

            var vals = new int[] { 41 };

            int val = f(vals, 0);

            Assert.AreEqual(42, val);
            Assert.AreEqual(42, vals[0]);

            //
            // BUG: Lowering of ArrayAccess results in a BlockExpression (a "comma"), but Expression.Call does not account for taking
            //      a reference to the final expression. Instead, we end up with a reference to a temporary (and not even a write-back
            //      because Block is read-only).
            //
            //      To fix this, we'd need to rewrite the MethodCallExpression (or other expressions that support by-ref parameters) by
            //      lifting the lowered form of ArrayAccess up. E.g.
            //
            //        Interlocked.Exchange(ref xs[i])
            //
            //      becomes
            //
            //        Interlocked.Exchange(ref {
            //          var __arr = xs;
            //          var __idx = i;
            //          __arr[__idx]
            //        })
            //
            //      without such a fix, but should be rearranged by spilling all arguments to the MethodCallExpression to locals in a
            //      new surrounding block, e.g.
            //
            //        {
            //          var __arr = xs;
            //          var __idx = i;
            //          Interlocked.Exchange(__arr[__idx])
            //        }
            //
            //      To achieve this, we need a reducible node higher up, which would require us to venture into CSharpExpression.Lambda
            //      when this library is referenced, such that the top-level C# lambda can be reduced into a regular LambdaExpression,
            //      while performing rewrites as the one shown above. We already do so for async lambdas, but this may be a compatibility
            //      concern for synchronous lambdas (also, the type is different).
            //
            //      An alternative would be to properly support ref locals in BlockExpression.
            //
        }

        [TestMethod]
        public void ArrayAccess_ByRef_CSharpNodes_SystemIndex()
        {
            //
            // NB: Unlike the test case above, this works because we can reduce CSharpExpression variants of MethodCall, New, and Invoke
            //     while properly accounting for by-ref passing of ArrayAccess.
            //
            //     The corresponding changes to the Roslyn compiler account for by-ref passing of ArrayAccess nodes that involve an Index
            //     operand, causing calls to be made to CSharpExpression.* factories for Call, New, and Invoke.  E.g.
            //
            //        Interlocked.Exchange(ref xs[i])
            //
            //     where i is of type Index, becomes
            //
            //        CSharpExpression.Call(xchg, CSharpExpression.ArrayAccess(...))
            //
            //     rather than
            //
            //        Expression.Call(xchg, CSharpExpression.ArrayAccess(...))
            //
            //     This triggers custom reduction logic in the C# variants of Call, New, and Invoke nodes.
            //
            //     However, this does not fix the previous test case where one explicitly uses Expression.Call. For this to be fixed, we
            //     either need:
            //
            //       1. Guaranteed custom reduction at a higher node (e.g. lambda).
            //       2. Changes to ref semantics in System.Linq.Expressions, e.g. allowing to peek into a comma node.
            //       3. Changes to Reduce logic in System.Linq.Expressions, e.g. custom logic for lval reduciton versus rval reduction.
            //       4. Push down Index/Range support to IndexExpression and piggyback on its by-ref support.
            //

            var xs = Expression.Parameter(typeof(int[]));
            var i = Expression.Parameter(typeof(Index));

            var inc = typeof(Interlocked).GetMethod(nameof(Interlocked.Increment), new[] { typeof(int).MakeByRefType() });

            MethodCallCSharpExpression mce = CSharpExpression.Call(inc, new Expression[] { CSharpExpression.ArrayAccess(xs, i) });

            var res = Expression.Lambda<Func<int[], Index, int>>(mce, xs, i);

            var f = res.Compile();

            var vals = new int[] { 41 };

            int val = f(vals, 0);

            Assert.AreEqual(42, val);
            Assert.AreEqual(42, vals[0]);
        }

        [TestMethod]
        public void ArrayAccess_ByRef_CSharpNodes_Int()
        {
            var xs = Expression.Parameter(typeof(int[]));
            var i = Expression.Parameter(typeof(int));

            var inc = typeof(Interlocked).GetMethod(nameof(Interlocked.Increment), new[] { typeof(int).MakeByRefType() });

            MethodCallCSharpExpression mce = CSharpExpression.Call(inc, new Expression[] { CSharpExpression.ArrayAccess(xs, i) });

            var res = Expression.Lambda<Func<int[], int, int>>(mce, xs, i);

            var f = res.Compile();

            var vals = new int[] { 41 };

            int val = f(vals, 0);

            Assert.AreEqual(42, val);
            Assert.AreEqual(42, vals[0]);
        }

        [TestMethod]
        public void ArrayAccess_Read_SideEffects()
        {
            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[] { 42 }));
                var index = Expression.Block(log("I"), Expression.Constant(0));

                return CSharpExpression.ArrayAccess(array, index);
            }, new LogAndResult<object> { Log = { "A", "I" } });

            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[] { 42 }));
                var index = Expression.Block(log("I"), Expression.Constant(new Index(0)));

                return CSharpExpression.ArrayAccess(array, index);
            }, new LogAndResult<object> { Log = { "A", "I" } });

            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[] { 42 }));
                var range = Expression.Block(log("R"), Expression.Constant(Range.All));

                return CSharpExpression.ArrayAccess(array, range);
            }, new LogAndResult<object> { Log = { "A", "R" } });

            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[,] { { 42 } }));
                var index1 = Expression.Block(log("I1"), Expression.Constant(0));
                var index2 = Expression.Block(log("I2"), Expression.Constant(0));

                return CSharpExpression.ArrayAccess(array, index1, index2);
            }, new LogAndResult<object> { Log = { "A", "I1", "I2" } });
        }

        [TestMethod]
        public void ArrayAccess_Assign_SideEffects()
        {
            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[] { 42 }));
                var index = Expression.Block(log("I"), Expression.Constant(0));
                var val = Expression.Block(log("V"), Expression.Constant(43));

                return CSharpExpression.Assign(CSharpExpression.ArrayAccess(array, index), val);
            }, new LogAndResult<object> { Log = { "A", "I", "V" } });

            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[] { 42 }));
                var index = Expression.Block(log("I"), Expression.Constant(new Index(0)));
                var val = Expression.Block(log("V"), Expression.Constant(43));

                return CSharpExpression.Assign(CSharpExpression.ArrayAccess(array, index), val);
            }, new LogAndResult<object> { Log = { "A", "I", "V" } });


            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[,] { { 42 } }));
                var index1 = Expression.Block(log("I1"), Expression.Constant(0));
                var index2 = Expression.Block(log("I2"), Expression.Constant(0));
                var val = Expression.Block(log("V"), Expression.Constant(43));

                return CSharpExpression.Assign(CSharpExpression.ArrayAccess(array, index1, index2), val);
            }, new LogAndResult<object> { Log = { "A", "I1", "I2", "V" } });
        }

        [TestMethod]
        public void ArrayAccess_CompoundAssign_SideEffects()
        {
            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[] { 42 }));
                var index = Expression.Block(log("I"), Expression.Constant(0));
                var val = Expression.Block(log("V"), Expression.Constant(43));

                return CSharpExpression.AddAssign(CSharpExpression.ArrayAccess(array, index), val);
            }, new LogAndResult<object> { Log = { "A", "I", "V" } });

            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[] { 42 }));
                var index = Expression.Block(log("I"), Expression.Constant(new Index(0)));
                var val = Expression.Block(log("V"), Expression.Constant(43));

                return CSharpExpression.AddAssign(CSharpExpression.ArrayAccess(array, index), val);
            }, new LogAndResult<object> { Log = { "A", "I", "V" } });


            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[,] { { 42 } }));
                var index1 = Expression.Block(log("I1"), Expression.Constant(0));
                var index2 = Expression.Block(log("I2"), Expression.Constant(0));
                var val = Expression.Block(log("V"), Expression.Constant(43));

                return CSharpExpression.AddAssign(CSharpExpression.ArrayAccess(array, index1, index2), val);
            }, new LogAndResult<object> { Log = { "A", "I1", "I2", "V" } });
        }

        [TestMethod]
        public void ArrayAccess_UnaryAssign_SideEffects()
        {
            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[] { 42 }));
                var index = Expression.Block(log("I"), Expression.Constant(0));

                return CSharpExpression.PostIncrementAssign(CSharpExpression.ArrayAccess(array, index));
            }, new LogAndResult<object> { Log = { "A", "I" } });

            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[] { 42 }));
                var index = Expression.Block(log("I"), Expression.Constant(new Index(0)));

                return CSharpExpression.PostIncrementAssign(CSharpExpression.ArrayAccess(array, index));
            }, new LogAndResult<object> { Log = { "A", "I" } });


            AssertCompile((log, append) =>
            {
                var array = Expression.Block(log("A"), Expression.Constant(new[,] { { 42 } }));
                var index1 = Expression.Block(log("I1"), Expression.Constant(0));
                var index2 = Expression.Block(log("I2"), Expression.Constant(0));

                return CSharpExpression.PostIncrementAssign(CSharpExpression.ArrayAccess(array, index1, index2));
            }, new LogAndResult<object> { Log = { "A", "I1", "I2" } });
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }
    }
}
