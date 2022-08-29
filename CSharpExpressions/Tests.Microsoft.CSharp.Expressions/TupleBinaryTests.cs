// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using Microsoft.CSharp.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using static Tests.TestHelpers;

namespace Tests
{
    public partial class TupleBinaryTests
    {
        [Fact]
        public void TupleBinary_Factory_ArgumentChecking()
        {
            // null checks
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.TupleEqual(null, Expression.Constant((1, 2))));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, 2)), null));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.TupleEqual(null, Expression.Constant((1, 2)), new LambdaExpression[0]));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, 2)), null, new LambdaExpression[0]));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, 2)), Expression.Constant((1, 2)), new LambdaExpression[2] { null, null }));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.TupleNotEqual(null, Expression.Constant((1, 2))));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, 2)), null));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.TupleNotEqual(null, Expression.Constant((1, 2)), new LambdaExpression[0]));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, 2)), null, new LambdaExpression[0]));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, 2)), Expression.Constant((1, 2)), new LambdaExpression[2] { null, null }));

            // not a tuple type
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleEqual(Expression.Constant(1), Expression.Constant((1, 2))));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleEqual(Expression.Constant(1), Expression.Constant((1, 2)), new LambdaExpression[0]));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, 2)), Expression.Constant(1), new LambdaExpression[0]));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleNotEqual(Expression.Constant(1), Expression.Constant((1, 2))));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleNotEqual(Expression.Constant(1), Expression.Constant((1, 2)), new LambdaExpression[0]));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, 2)), Expression.Constant(1), new LambdaExpression[0]));

            // mismatched tuple arities
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, 2)), Expression.Constant((1, 2, 3))));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, 2)), Expression.Constant((1, 2, 3)), new LambdaExpression[0]));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, 2)), Expression.Constant((1, 2, 3))));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, 2)), Expression.Constant((1, 2, 3)), new LambdaExpression[0]));

            // invalid check count
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, 2)), Expression.Constant((1, 2)), new LambdaExpression[0]));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, 2)), Expression.Constant((1, 2)), new LambdaExpression[0]));

            // invalid check signature
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, "")), Expression.Constant((1, "")), new LambdaExpression[] { (Expression<Func<int>>)(() => 1), (Expression<Func<string, string, bool>>)((s1, s2) => s1 == s2) }));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, "")), Expression.Constant((1, "")), new LambdaExpression[] { (Expression<Func<int, int>>)(x => x), (Expression<Func<string, string, bool>>)((s1, s2) => s1 == s2) }));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, "")), Expression.Constant((1, "")), new LambdaExpression[] { (Expression<Func<int, int, int, bool>>)((x, y, z) => x == y), (Expression<Func<string, string, bool>>)((s1, s2) => s1 == s2) }));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, "")), Expression.Constant((1, "")), new LambdaExpression[] { (Expression<Func<int>>)(() => 1), (Expression<Func<string, string, bool>>)((s1, s2) => s1 == s2) }));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, "")), Expression.Constant((1, "")), new LambdaExpression[] { (Expression<Func<int, int>>)(x => x), (Expression<Func<string, string, bool>>)((s1, s2) => s1 == s2) }));
            Assert.Throws<ArgumentException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, "")), Expression.Constant((1, "")), new LambdaExpression[] { (Expression<Func<int, int, int, bool>>)((x, y, z) => x == y), (Expression<Func<string, string, bool>>)((s1, s2) => s1 == s2) }));

            // invalid check parameter types
            // NB: exception type derived from LINQ helpers
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, "")), Expression.Constant((1, "")), new LambdaExpression[] { (Expression<Func<char, char, bool>>)((c1, c2) => c1 == c2), (Expression<Func<string, string, bool>>)((s1, s2) => s1 == s2) }));
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, "")), Expression.Constant((1, "")), new LambdaExpression[] { (Expression<Func<int, int, bool>>)((i1, i2) => i1 == i2), (Expression<Func<long, long, bool>>)((l1, l2) => l1 == l2) }));
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, "")), Expression.Constant((1, "")), new LambdaExpression[] { (Expression<Func<char, char, bool>>)((c1, c2) => c1 == c2), (Expression<Func<string, string, bool>>)((s1, s2) => s1 == s2) }));
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, "")), Expression.Constant((1, "")), new LambdaExpression[] { (Expression<Func<int, int, bool>>)((i1, i2) => i1 == i2), (Expression<Func<long, long, bool>>)((l1, l2) => l1 == l2) }));

            // invalid check return type
            // NB: exception type derived from LINQ helpers
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, "")), Expression.Constant((1, "")), new LambdaExpression[] { (Expression<Func<int, int, int>>)((x, y) => x + y), (Expression<Func<string, string, bool>>)((s1, s2) => s1 == s2) }));
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, "")), Expression.Constant((1, "")), new LambdaExpression[] { (Expression<Func<int, int, int>>)((x, y) => x + y), (Expression<Func<string, string, bool>>)((s1, s2) => s1 == s2) }));

            // can't infer equality check
            // NB: exception type derived from LINQ helpers
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.TupleEqual(Expression.Constant((1L, 2)), Expression.Constant((1, 2))));
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, (2, 3))), Expression.Constant((1, 2L))));
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.TupleEqual(Expression.Constant((1, 2L)), Expression.Constant((1, (2, 3)))));
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1L, 2)), Expression.Constant((1, 2))));
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, (2, 3))), Expression.Constant((1, 2L))));
            Assert.Throws<InvalidOperationException>(() => CSharpExpression.TupleNotEqual(Expression.Constant((1, 2L)), Expression.Constant((1, (2, 3)))));

            // TODO: Contravariance allowed for checks?
        }

        [Fact]
        public void TupleBinary_Factory_Properties()
        {
            var left = Expression.Constant((1, "bar"));
            var right = Expression.Constant((2, "foo"));
            var check1 = (Expression<Func<int, int, bool>>)((l, r) => l == r);
            var check2 = (Expression<Func<string, string, bool>>)((l, r) => l == r);

            var eq = CSharpExpression.TupleEqual(left, right, new LambdaExpression[] { check1, check2 });

            Assert.Equal(CSharpExpressionType.TupleEqual, eq.CSharpNodeType);
            Assert.Equal(typeof(bool), eq.Type);
            Assert.Same(left, eq.Left);
            Assert.Same(right, eq.Right);
            Assert.Equal(2, eq.EqualityChecks.Count);
            Assert.Same(check1, eq.EqualityChecks[0]);
            Assert.Same(check2, eq.EqualityChecks[1]);
            Assert.False(eq.IsLifted);

            var ne = CSharpExpression.TupleNotEqual(left, right, new LambdaExpression[] { check1, check2 });

            Assert.Equal(CSharpExpressionType.TupleNotEqual, ne.CSharpNodeType);
            Assert.Equal(typeof(bool), ne.Type);
            Assert.Same(left, ne.Left);
            Assert.Same(right, ne.Right);
            Assert.Equal(2, ne.EqualityChecks.Count);
            Assert.Same(check1, ne.EqualityChecks[0]);
            Assert.Same(check2, ne.EqualityChecks[1]);
            Assert.False(ne.IsLifted);

            var nullableType = typeof(Nullable<>).MakeGenericType(left.Type);
            var nullableLeft = Expression.Convert(left, nullableType);
            var nullableRight = Expression.Convert(right, nullableType);

            var liftedEq = CSharpExpression.TupleEqual(nullableLeft, nullableRight, new LambdaExpression[] { check1, check2 });

            Assert.Equal(CSharpExpressionType.TupleEqual, liftedEq.CSharpNodeType);
            Assert.Equal(typeof(bool), liftedEq.Type);
            Assert.Same(nullableLeft, liftedEq.Left);
            Assert.Same(nullableRight, liftedEq.Right);
            Assert.Equal(2, liftedEq.EqualityChecks.Count);
            Assert.Same(check1, liftedEq.EqualityChecks[0]);
            Assert.Same(check2, liftedEq.EqualityChecks[1]);
            Assert.True(liftedEq.IsLifted);

            var liftedNe = CSharpExpression.TupleNotEqual(nullableLeft, nullableRight, new LambdaExpression[] { check1, check2 });

            Assert.Equal(CSharpExpressionType.TupleNotEqual, liftedNe.CSharpNodeType);
            Assert.Equal(typeof(bool), liftedNe.Type);
            Assert.Same(nullableLeft, liftedNe.Left);
            Assert.Same(nullableRight, liftedNe.Right);
            Assert.Equal(2, liftedNe.EqualityChecks.Count);
            Assert.Same(check1, liftedNe.EqualityChecks[0]);
            Assert.Same(check2, liftedNe.EqualityChecks[1]);
            Assert.True(liftedNe.IsLifted);
        }

        [Fact]
        public void TupleBinary_Factory_Properties_InferChecks()
        {
            var left = Expression.Constant((1, "bar"));
            var right = Expression.Constant((2, "foo"));

            var eq = CSharpExpression.TupleEqual(left, right);

            Assert.Equal(CSharpExpressionType.TupleEqual, eq.CSharpNodeType);
            Assert.Equal(typeof(bool), eq.Type);
            Assert.Same(left, eq.Left);
            Assert.Same(right, eq.Right);
            Assert.Equal(2, eq.EqualityChecks.Count);
            AssertCheck(eq.EqualityChecks[0], typeof(int), ExpressionType.Equal);
            AssertCheck(eq.EqualityChecks[1], typeof(string), ExpressionType.Equal);
            Assert.False(eq.IsLifted);

            var ne = CSharpExpression.TupleNotEqual(left, right);

            Assert.Equal(CSharpExpressionType.TupleNotEqual, ne.CSharpNodeType);
            Assert.Equal(typeof(bool), ne.Type);
            Assert.Same(left, ne.Left);
            Assert.Same(right, ne.Right);
            Assert.Equal(2, ne.EqualityChecks.Count);
            AssertCheck(ne.EqualityChecks[0], typeof(int), ExpressionType.NotEqual);
            AssertCheck(ne.EqualityChecks[1], typeof(string), ExpressionType.NotEqual);
            Assert.False(ne.IsLifted);

            var nullableType = typeof(Nullable<>).MakeGenericType(left.Type);
            var nullableLeft = Expression.Convert(left, nullableType);
            var nullableRight = Expression.Convert(right, nullableType);

            var liftedEq = CSharpExpression.TupleEqual(nullableLeft, nullableRight);

            Assert.Equal(CSharpExpressionType.TupleEqual, liftedEq.CSharpNodeType);
            Assert.Equal(typeof(bool), liftedEq.Type);
            Assert.Same(nullableLeft, liftedEq.Left);
            Assert.Same(nullableRight, liftedEq.Right);
            Assert.Equal(2, liftedEq.EqualityChecks.Count);
            AssertCheck(liftedEq.EqualityChecks[0], typeof(int), ExpressionType.Equal);
            AssertCheck(liftedEq.EqualityChecks[1], typeof(string), ExpressionType.Equal);
            Assert.True(liftedEq.IsLifted);

            var liftedNe = CSharpExpression.TupleNotEqual(nullableLeft, nullableRight);

            Assert.Equal(CSharpExpressionType.TupleNotEqual, liftedNe.CSharpNodeType);
            Assert.Equal(typeof(bool), liftedNe.Type);
            Assert.Same(nullableLeft, liftedNe.Left);
            Assert.Same(nullableRight, liftedNe.Right);
            Assert.Equal(2, liftedNe.EqualityChecks.Count);
            AssertCheck(liftedNe.EqualityChecks[0], typeof(int), ExpressionType.NotEqual);
            AssertCheck(liftedNe.EqualityChecks[1], typeof(string), ExpressionType.NotEqual);
            Assert.True(liftedNe.IsLifted);

            void AssertCheck(LambdaExpression check, Type type, ExpressionType kind)
            {
                Assert.Equal(2, check.Parameters.Count);
                Assert.Equal(type, check.Parameters[0].Type);
                Assert.Equal(type, check.Parameters[1].Type);
                var b = check.Body as BinaryExpression;
                Assert.NotNull(b);
                Assert.True(b.NodeType == kind);
                Assert.True(b.Left == check.Parameters[0]);
                Assert.True(b.Right == check.Parameters[1]);
            }
        }

        [Fact]
        public void TupleBinary_Factory_Properties_InferChecks_Nested()
        {
            var left = Expression.Constant((1, ("bar", TimeSpan.FromSeconds(1))));
            var right = Expression.Constant((2, ("foo", TimeSpan.FromSeconds(2))));

            var eq = CSharpExpression.TupleEqual(left, right);

            Assert.Equal(CSharpExpressionType.TupleEqual, eq.CSharpNodeType);
            Assert.Equal(typeof(bool), eq.Type);
            Assert.Same(left, eq.Left);
            Assert.Same(right, eq.Right);
            Assert.Equal(2, eq.EqualityChecks.Count);
            AssertCheck(eq.EqualityChecks[0], typeof(int), ExpressionType.Equal);
            AssertNestedCheck(eq.EqualityChecks[1], typeof(ValueTuple<string, TimeSpan>), CSharpExpressionType.TupleEqual);
            Assert.False(eq.IsLifted);

            var ne = CSharpExpression.TupleNotEqual(left, right);

            Assert.Equal(CSharpExpressionType.TupleNotEqual, ne.CSharpNodeType);
            Assert.Equal(typeof(bool), ne.Type);
            Assert.Same(left, ne.Left);
            Assert.Same(right, ne.Right);
            Assert.Equal(2, ne.EqualityChecks.Count);
            AssertCheck(ne.EqualityChecks[0], typeof(int), ExpressionType.NotEqual);
            AssertNestedCheck(ne.EqualityChecks[1], typeof(ValueTuple<string, TimeSpan>), CSharpExpressionType.TupleNotEqual);
            Assert.False(ne.IsLifted);

            var nullableType = typeof(Nullable<>).MakeGenericType(left.Type);
            var nullableLeft = Expression.Convert(left, nullableType);
            var nullableRight = Expression.Convert(right, nullableType);

            var liftedEq = CSharpExpression.TupleEqual(nullableLeft, nullableRight);

            Assert.Equal(CSharpExpressionType.TupleEqual, liftedEq.CSharpNodeType);
            Assert.Equal(typeof(bool), liftedEq.Type);
            Assert.Same(nullableLeft, liftedEq.Left);
            Assert.Same(nullableRight, liftedEq.Right);
            Assert.Equal(2, liftedEq.EqualityChecks.Count);
            AssertCheck(liftedEq.EqualityChecks[0], typeof(int), ExpressionType.Equal);
            AssertNestedCheck(liftedEq.EqualityChecks[1], typeof(ValueTuple<string, TimeSpan>), CSharpExpressionType.TupleEqual);
            Assert.True(liftedEq.IsLifted);

            var liftedNe = CSharpExpression.TupleNotEqual(nullableLeft, nullableRight);

            Assert.Equal(CSharpExpressionType.TupleNotEqual, liftedNe.CSharpNodeType);
            Assert.Equal(typeof(bool), liftedNe.Type);
            Assert.Same(nullableLeft, liftedNe.Left);
            Assert.Same(nullableRight, liftedNe.Right);
            Assert.Equal(2, liftedNe.EqualityChecks.Count);
            AssertCheck(liftedNe.EqualityChecks[0], typeof(int), ExpressionType.NotEqual);
            AssertNestedCheck(liftedNe.EqualityChecks[1], typeof(ValueTuple<string, TimeSpan>), CSharpExpressionType.TupleNotEqual);
            Assert.True(liftedNe.IsLifted);

            void AssertCheck(LambdaExpression check, Type type, ExpressionType kind)
            {
                Assert.Equal(2, check.Parameters.Count);
                Assert.Equal(type, check.Parameters[0].Type);
                Assert.Equal(type, check.Parameters[1].Type);
                var b = check.Body as BinaryExpression;
                Assert.NotNull(b);
                Assert.True(b.NodeType == kind);
                Assert.True(b.Left == check.Parameters[0]);
                Assert.True(b.Right == check.Parameters[1]);
            }

            void AssertNestedCheck(LambdaExpression check, Type type, CSharpExpressionType kind)
            {
                Assert.Equal(2, check.Parameters.Count);
                Assert.Equal(type, check.Parameters[0].Type);
                Assert.Equal(type, check.Parameters[1].Type);
                var b = check.Body as TupleBinaryCSharpExpression;
                Assert.NotNull(b);
                Assert.True(b.CSharpNodeType == kind);
                Assert.True(b.Left == check.Parameters[0]);
                Assert.True(b.Right == check.Parameters[1]);

                var types = Helpers.GetTupleComponentTypes(type).ToArray();

                for (var i = 0; i < types.Length; i++)
                {
                    AssertCheck(b.EqualityChecks[i], types[i], kind == CSharpExpressionType.TupleEqual ? ExpressionType.Equal : ExpressionType.NotEqual);
                }
            }
        }

        [Fact]
        public void TupleBinary_Update()
        {
            var left = Expression.Constant((1, "bar"));
            var right = Expression.Constant((2, "foo"));
            var check1 = (Expression<Func<int, int, bool>>)((l, r) => l == r);
            var check2 = (Expression<Func<string, string, bool>>)((l, r) => l == r);

            var it = CSharpExpression.TupleEqual(left, right, new LambdaExpression[] { check1, check2 });

            Assert.Same(it, it.Update(it.Left, it.Right, it.EqualityChecks));

            var newOperand = Expression.Constant((3, "qux"));

            var new1 = it.Update(newOperand, it.Right, it.EqualityChecks);
            Assert.Same(newOperand, new1.Left);
            Assert.Same(it.Right, new1.Right);
            Assert.Same(it.EqualityChecks, new1.EqualityChecks);

            var new2 = it.Update(it.Left, newOperand, it.EqualityChecks);
            Assert.Same(it.Left, new2.Left);
            Assert.Same(newOperand, new2.Right);
            Assert.Same(it.EqualityChecks, new2.EqualityChecks);

            var check3 = (Expression<Func<string, string, bool>>)((l, r) => l == r);
            var newEqualityChecks = new LambdaExpression[] { check1, check3 };
            var new3 = it.Update(it.Left, it.Right, newEqualityChecks);
            Assert.Same(it.Left, new3.Left);
            Assert.Same(it.Right, new3.Right);
            Assert.Equal(2, it.EqualityChecks.Count);
            Assert.Same(check1, new3.EqualityChecks[0]);
            Assert.Same(check3, new3.EqualityChecks[1]);
        }

        [Fact]
        public void TupleBinary_Visitor()
        {
            var left = Expression.Constant((1, "bar"));
            var right = Expression.Constant((2, "foo"));
            var check1 = (Expression<Func<int, int, bool>>)((l, r) => l == r);
            var check2 = (Expression<Func<string, string, bool>>)((l, r) => l == r);

            var res = CSharpExpression.TupleEqual(left, right, new LambdaExpression[] { check1, check2 });

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitTupleBinary(TupleBinaryCSharpExpression node)
            {
                Visited = true;

                return base.VisitTupleBinary(node);
            }
        }

        [Fact]
        public void TupleBinary_Reduce_NonNull_Arity2()
        {
            var tuples = new[]
            {
                (1, null),
                (1, "bar"),
                (1, "foo"),
                (2, null),
                (2, "bar"),
                (2, "foo"),
            };

            var tupleType = tuples[0].GetType();

            var left = Expression.Parameter(tupleType, "left");
            var right = Expression.Parameter(tupleType, "right");

            var bodyEq = CSharpExpression.TupleEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l == r), (Expression<Func<string, string, bool>>)((l, r) => l == r));
            var bodyNe = CSharpExpression.TupleNotEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l != r), (Expression<Func<string, string, bool>>)((l, r) => l != r));

            var eq = Expression.Lambda<Func<(int, string), (int, string), bool>>(bodyEq, left, right).Compile();
            var ne = Expression.Lambda<Func<(int, string), (int, string), bool>>(bodyNe, left, right).Compile();

            foreach (var l in tuples)
            {
                foreach (var r in tuples)
                {
                    Assert.Equal(l == r, eq(l, r));
                    Assert.Equal(l != r, ne(l, r));
                }
            }
        }

        [Fact]
        public void TupleBinary_Reduce_NonNull_Arity3()
        {
            var date1 = new DateTime(1983, 2, 11);
            var date2 = new DateTime(2019, 12, 2);

            var tuples = new[]
            {
                (1, null, date1),
                (1, null, date2),
                (1, "bar", date1),
                (1, "bar", date2),
                (1, "foo", date1),
                (1, "foo", date2),
                (2, null, date1),
                (2, null, date2),
                (2, "bar", date1),
                (2, "bar", date2),
                (2, "foo", date1),
                (2, "foo", date2),
            };

            var tupleType = tuples[0].GetType();

            var left = Expression.Parameter(tupleType, "left");
            var right = Expression.Parameter(tupleType, "right");

            var bodyEq = CSharpExpression.TupleEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l == r), (Expression<Func<string, string, bool>>)((l, r) => l == r), (Expression<Func<DateTime, DateTime, bool>>)((l, r) => l == r));
            var bodyNe = CSharpExpression.TupleNotEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l != r), (Expression<Func<string, string, bool>>)((l, r) => l != r), (Expression<Func<DateTime, DateTime, bool>>)((l, r) => l != r));

            var eq = Expression.Lambda<Func<(int, string, DateTime), (int, string, DateTime), bool>>(bodyEq, left, right).Compile();
            var ne = Expression.Lambda<Func<(int, string, DateTime), (int, string, DateTime), bool>>(bodyNe, left, right).Compile();

            foreach (var l in tuples)
            {
                foreach (var r in tuples)
                {
                    Assert.Equal(l == r, eq(l, r));
                    Assert.Equal(l != r, ne(l, r));
                }
            }
        }

        [Fact]
        public void TupleBinary_Reduce_NonNull_Arity10()
        {
            var tuples = new[]
            {
                (0, 1, 2, 3, 4, 5, 6, 7, 8, 9),
                (-1, 1, 2, 3, 4, 5, 6, 7, 8, 9),
                (0, 1, 2, 3, -4, 5, 6, 7, 8, 9),
                (0, 1, 2, 3, 4, 5, 6, 7, -8, 9),
            };

            var tupleType = tuples[0].GetType();

            var left = Expression.Parameter(tupleType, "left");
            var right = Expression.Parameter(tupleType, "right");

            var testEq = (Expression<Func<int, int, bool>>)((l, r) => l == r);
            var testNe = (Expression<Func<int, int, bool>>)((l, r) => l != r);

            var bodyEq = CSharpExpression.TupleEqual(left, right, Enumerable.Repeat(testEq, 10));
            var bodyNe = CSharpExpression.TupleNotEqual(left, right, Enumerable.Repeat(testNe, 10));

            var eq = Expression.Lambda<Func<(int, int, int, int, int, int, int, int, int, int), (int, int, int, int, int, int, int, int, int, int), bool>>(bodyEq, left, right).Compile();
            var ne = Expression.Lambda<Func<(int, int, int, int, int, int, int, int, int, int), (int, int, int, int, int, int, int, int, int, int), bool>>(bodyNe, left, right).Compile();

            foreach (var l in tuples)
            {
                foreach (var r in tuples)
                {
                    Assert.Equal(l == r, eq(l, r));
                    Assert.Equal(l != r, ne(l, r));
                }
            }
        }

        [Fact]
        public void TupleBinary_Reduce_Null_Arity2()
        {
            var tuples = new (int, string)?[]
            {
                (1, null),
                (1, "bar"),
                (1, "foo"),
                (2, null),
                (2, "bar"),
                (2, "foo"),
            };

            var tupleType = tuples[0].GetType();
            var nullableTupleType = typeof(Nullable<>).MakeGenericType(tupleType);

            // Left null, right non-null
            {
                var left = Expression.Parameter(nullableTupleType, "left");
                var right = Expression.Parameter(tupleType, "right");

                var bodyEq = CSharpExpression.TupleEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l == r), (Expression<Func<string, string, bool>>)((l, r) => l == r));
                var bodyNe = CSharpExpression.TupleNotEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l != r), (Expression<Func<string, string, bool>>)((l, r) => l != r));

                var lambda = Expression.Lambda<Func<(int, string)?, (int, string), bool>>(bodyEq, left, right);
                var res = new V().Visit(lambda);

                var eq = Expression.Lambda<Func<(int, string)?, (int, string), bool>>(bodyEq, left, right).Compile();
                var ne = Expression.Lambda<Func<(int, string)?, (int, string), bool>>(bodyNe, left, right).Compile();

                foreach (var l in tuples)
                {
                    foreach (var r in tuples)
                    {
                        Assert.Equal(l == r, eq(l, r.Value));
                        Assert.Equal(l != r, ne(l, r.Value));
                    }
                }

                foreach (var r in tuples)
                {
                    Assert.Equal(null == r, eq(null, r.Value));
                    Assert.Equal(null != r, ne(null, r.Value));
                }
            }

            // Left non-null, right null
            {
                var left = Expression.Parameter(tupleType, "left");
                var right = Expression.Parameter(nullableTupleType, "right");

                var bodyEq = CSharpExpression.TupleEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l == r), (Expression<Func<string, string, bool>>)((l, r) => l == r));
                var bodyNe = CSharpExpression.TupleNotEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l != r), (Expression<Func<string, string, bool>>)((l, r) => l != r));

                var eq = Expression.Lambda<Func<(int, string), (int, string)?, bool>>(bodyEq, left, right).Compile();
                var ne = Expression.Lambda<Func<(int, string), (int, string)?, bool>>(bodyNe, left, right).Compile();

                foreach (var l in tuples)
                {
                    foreach (var r in tuples)
                    {
                        Assert.Equal(l == r, eq(l.Value, r));
                        Assert.Equal(l != r, ne(l.Value, r));
                    }
                }

                foreach (var l in tuples)
                {
                    Assert.Equal(l == null, eq(l.Value, null));
                    Assert.Equal(l != null, ne(l.Value, null));
                }
            }

            // Left null, right null
            {
                var left = Expression.Parameter(nullableTupleType, "left");
                var right = Expression.Parameter(nullableTupleType, "right");

                var bodyEq = CSharpExpression.TupleEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l == r), (Expression<Func<string, string, bool>>)((l, r) => l == r));
                var bodyNe = CSharpExpression.TupleNotEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l != r), (Expression<Func<string, string, bool>>)((l, r) => l != r));

                var eq = Expression.Lambda<Func<(int, string)?, (int, string)?, bool>>(bodyEq, left, right).Compile();
                var ne = Expression.Lambda<Func<(int, string)?, (int, string)?, bool>>(bodyNe, left, right).Compile();

                foreach (var l in tuples.Prepend(null))
                {
                    foreach (var r in tuples.Prepend(null))
                    {
                        Assert.Equal(l == r, eq(l, r));
                        Assert.Equal(l != r, ne(l, r));
                    }
                }
            }
        }

        [Fact]
        public void TupleBinary_Reduce_Null_Optimize_RemoveLiftToNullConversion()
        {
            var tuples = new (int, string)?[]
            {
                (1, null),
                (1, "bar"),
                (1, "foo"),
                (2, null),
                (2, "bar"),
                (2, "foo"),
            };

            var tupleType = tuples[0].GetType();
            var nullableTupleType = typeof(Nullable<>).MakeGenericType(tupleType);

            // Left null, right non-null
            {
                var left = Expression.Parameter(nullableTupleType, "left");
                var right = Expression.Parameter(tupleType, "right");

                var liftRight = Expression.Convert(right, nullableTupleType);

                var bodyEq = CSharpExpression.TupleEqual(left, liftRight, (Expression<Func<int, int, bool>>)((l, r) => l == r), (Expression<Func<string, string, bool>>)((l, r) => l == r));
                var bodyNe = CSharpExpression.TupleNotEqual(left, liftRight, (Expression<Func<int, int, bool>>)((l, r) => l != r), (Expression<Func<string, string, bool>>)((l, r) => l != r));

                var lambda = Expression.Lambda<Func<(int, string)?, (int, string), bool>>(bodyEq, left, right);
                var res = new V().Visit(lambda);

                var eq = Expression.Lambda<Func<(int, string)?, (int, string), bool>>(bodyEq, left, right).Compile();
                var ne = Expression.Lambda<Func<(int, string)?, (int, string), bool>>(bodyNe, left, right).Compile();

                foreach (var l in tuples)
                {
                    foreach (var r in tuples)
                    {
                        Assert.Equal(l == r, eq(l, r.Value));
                        Assert.Equal(l != r, ne(l, r.Value));
                    }
                }

                foreach (var r in tuples)
                {
                    Assert.Equal(null == r, eq(null, r.Value));
                    Assert.Equal(null != r, ne(null, r.Value));
                }
            }

            // Left non-null, right null
            {
                var left = Expression.Parameter(tupleType, "left");
                var right = Expression.Parameter(nullableTupleType, "right");

                var liftLeft = Expression.Convert(left, nullableTupleType);

                var bodyEq = CSharpExpression.TupleEqual(liftLeft, right, (Expression<Func<int, int, bool>>)((l, r) => l == r), (Expression<Func<string, string, bool>>)((l, r) => l == r));
                var bodyNe = CSharpExpression.TupleNotEqual(liftLeft, right, (Expression<Func<int, int, bool>>)((l, r) => l != r), (Expression<Func<string, string, bool>>)((l, r) => l != r));

                var eq = Expression.Lambda<Func<(int, string), (int, string)?, bool>>(bodyEq, left, right).Compile();
                var ne = Expression.Lambda<Func<(int, string), (int, string)?, bool>>(bodyNe, left, right).Compile();

                foreach (var l in tuples)
                {
                    foreach (var r in tuples)
                    {
                        Assert.Equal(l == r, eq(l.Value, r));
                        Assert.Equal(l != r, ne(l.Value, r));
                    }
                }

                foreach (var l in tuples)
                {
                    Assert.Equal(l == null, eq(l.Value, null));
                    Assert.Equal(l != null, ne(l.Value, null));
                }
            }
        }

        [Fact]
        public void TupleBinary_Reduce_Nested()
        {
            var tuples = new[]
            {
                (1, (null, 1L)),
                (1, (null, 2L)),
                (1, ("bar", 1L)),
                (1, ("bar", 2L)),
                (1, ("foo", 1L)),
                (1, ("foo", 2L)),
                (2, (null, 1L)),
                (2, (null, 2L)),
                (2, ("bar", 1L)),
                (2, ("bar", 2L)),
                (2, ("foo", 1L)),
                (2, ("foo", 2L)),
            };

            var tupleType = tuples[0].GetType();
            var nestedTupleType = tupleType.GetGenericArguments()[1];

            var nestedLeft = Expression.Parameter(nestedTupleType, "left");
            var nestedRight = Expression.Parameter(nestedTupleType, "right");

            var nestedEq = CSharpExpression.TupleEqual(nestedLeft, nestedRight, (Expression<Func<string, string, bool>>)((l, r) => l == r), (Expression<Func<long, long, bool>>)((l, r) => l == r));
            var nestedNe = CSharpExpression.TupleNotEqual(nestedLeft, nestedRight, (Expression<Func<string, string, bool>>)((l, r) => l != r), (Expression<Func<long, long, bool>>)((l, r) => l != r));

            var nestedEqCheck = Expression.Lambda<Func<(string, long), (string, long), bool>>(nestedEq, nestedLeft, nestedRight);
            var nestedNeCheck = Expression.Lambda<Func<(string, long), (string, long), bool>>(nestedNe, nestedLeft, nestedRight);

            var left = Expression.Parameter(tupleType, "left");
            var right = Expression.Parameter(tupleType, "right");

            var bodyEq = CSharpExpression.TupleEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l == r), nestedEqCheck);
            var bodyNe = CSharpExpression.TupleNotEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l != r), nestedNeCheck);

            var eq = Expression.Lambda<Func<(int, (string, long)), (int, (string, long)), bool>>(bodyEq, left, right).Compile();
            var ne = Expression.Lambda<Func<(int, (string, long)), (int, (string, long)), bool>>(bodyNe, left, right).Compile();

            foreach (var l in tuples)
            {
                foreach (var r in tuples)
                {
                    Assert.Equal(l == r, eq(l, r));
                    Assert.Equal(l != r, ne(l, r));
                }
            }
        }

        [Fact]
        public void TupleBinary_Reduce_Nested_Nullable()
        {
            var tuples = new (int, (string, long)?)[]
            {
                (1, null),
                (1, (null, 1L)),
                (1, (null, 2L)),
                (1, ("bar", 1L)),
                (1, ("bar", 2L)),
                (1, ("foo", 1L)),
                (1, ("foo", 2L)),
                (2, null),
                (2, (null, 1L)),
                (2, (null, 2L)),
                (2, ("bar", 1L)),
                (2, ("bar", 2L)),
                (2, ("foo", 1L)),
                (2, ("foo", 2L)),
            };

            var tupleType = tuples[0].GetType();
            var nestedTupleType = tupleType.GetGenericArguments()[1];

            var nestedLeft = Expression.Parameter(nestedTupleType, "left");
            var nestedRight = Expression.Parameter(nestedTupleType, "right");

            var nestedEq = CSharpExpression.TupleEqual(nestedLeft, nestedRight, (Expression<Func<string, string, bool>>)((l, r) => l == r), (Expression<Func<long, long, bool>>)((l, r) => l == r));
            var nestedNe = CSharpExpression.TupleNotEqual(nestedLeft, nestedRight, (Expression<Func<string, string, bool>>)((l, r) => l != r), (Expression<Func<long, long, bool>>)((l, r) => l != r));

            var nestedEqCheck = Expression.Lambda<Func<(string, long)?, (string, long)?, bool>>(nestedEq, nestedLeft, nestedRight);
            var nestedNeCheck = Expression.Lambda<Func<(string, long)?, (string, long)?, bool>>(nestedNe, nestedLeft, nestedRight);

            var left = Expression.Parameter(tupleType, "left");
            var right = Expression.Parameter(tupleType, "right");

            var bodyEq = CSharpExpression.TupleEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l == r), nestedEqCheck);
            var bodyNe = CSharpExpression.TupleNotEqual(left, right, (Expression<Func<int, int, bool>>)((l, r) => l != r), nestedNeCheck);

            var eq = Expression.Lambda<Func<(int, (string, long)?), (int, (string, long)?), bool>>(bodyEq, left, right).Compile();
            var ne = Expression.Lambda<Func<(int, (string, long)?), (int, (string, long)?), bool>>(bodyNe, left, right).Compile();

            foreach (var l in tuples)
            {
                foreach (var r in tuples)
                {
                    Assert.Equal(l == r, eq(l, r));
                    Assert.Equal(l != r, ne(l, r));
                }
            }
        }

        [Fact]
        public void TupleBinary_SideEffects_ShortCircuit()
        {
            var l = Expression.Parameter(typeof(int));
            var r = Expression.Parameter(typeof(int));

            // Equal

            AssertCompile((log, append) =>
            {
                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.Equal(l, r)), l, r));

                var left = Expression.Block(log("L"), Expression.Constant((1, 2)));
                var right = Expression.Block(log("R"), Expression.Constant((1, 2)));

                return CSharpExpression.TupleEqual(left, right, checksEq);
            }, new LogAndResult<bool> { Log = { "L", "R", "1", "2" }, Value = true });

            AssertCompile((log, append) =>
            {
                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.Equal(l, r)), l, r));

                var left = Expression.Block(log("L"), Expression.Constant((1, 2)));
                var right = Expression.Block(log("R"), Expression.Constant((-1, 2)));

                return CSharpExpression.TupleEqual(left, right, checksEq);
            }, new LogAndResult<bool> { Log = { "L", "R", "1" }, Value = false });

            AssertCompile((log, append) =>
            {
                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.Equal(l, r)), l, r));

                var left = Expression.Block(log("L"), Expression.Constant((1, 2)));
                var right = Expression.Block(log("R"), Expression.Constant((1, -2)));

                return CSharpExpression.TupleEqual(left, right, checksEq);
            }, new LogAndResult<bool> { Log = { "L", "R", "1", "2" }, Value = false });

            // NotEqual

            AssertCompile((log, append) =>
            {
                var checksNe = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.NotEqual(l, r)), l, r));

                var left = Expression.Block(log("L"), Expression.Constant((1, 2)));
                var right = Expression.Block(log("R"), Expression.Constant((1, 2)));

                return CSharpExpression.TupleNotEqual(left, right, checksNe);
            }, new LogAndResult<bool> { Log = { "L", "R", "1", "2" }, Value = false });

            AssertCompile((log, append) =>
            {
                var checksNe = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.NotEqual(l, r)), l, r));

                var left = Expression.Block(log("L"), Expression.Constant((1, 2)));
                var right = Expression.Block(log("R"), Expression.Constant((-1, 2)));

                return CSharpExpression.TupleNotEqual(left, right, checksNe);
            }, new LogAndResult<bool> { Log = { "L", "R", "1" }, Value = true });

            AssertCompile((log, append) =>
            {
                var checksNe = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.NotEqual(l, r)), l, r));

                var left = Expression.Block(log("L"), Expression.Constant((1, 2)));
                var right = Expression.Block(log("R"), Expression.Constant((1, -2)));

                return CSharpExpression.TupleNotEqual(left, right, checksNe);
            }, new LogAndResult<bool> { Log = { "L", "R", "1", "2" }, Value = true });
        }

        [Fact]
        public void TupleBinary_SideEffects_TupleLiteral()
        {
            var l = Expression.Parameter(typeof(int));
            var r = Expression.Parameter(typeof(int));

            AssertCompile((log, append) =>
            {
                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.Equal(l, r)), l, r));

                var left = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("L.I1"), Expression.Constant(1)), Expression.Block(log("L.I2"), Expression.Constant(2)) }, null);
                var right = Expression.Block(log("R"), Expression.Constant((1, 2)));

                return CSharpExpression.TupleEqual(left, right, checksEq);
            }, new LogAndResult<bool> { Log = { "L.I1", "L.I2", "R", "1", "2" }, Value = true });

            AssertCompile((log, append) =>
            {
                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.Equal(l, r)), l, r));

                var left = Expression.Block(log("L"), Expression.Constant((1, 2)));
                var right = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("R.I1"), Expression.Constant(1)), Expression.Block(log("R.I2"), Expression.Constant(2)) }, null);

                return CSharpExpression.TupleEqual(left, right, checksEq);
            }, new LogAndResult<bool> { Log = { "L", "R.I1", "R.I2", "1", "2" }, Value = true });

            AssertCompile((log, append) =>
            {
                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.Equal(l, r)), l, r));

                var left = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("L.I1"), Expression.Constant(1)), Expression.Block(log("L.I2"), Expression.Constant(2)) }, null);
                var right = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("R.I1"), Expression.Constant(1)), Expression.Block(log("R.I2"), Expression.Constant(2)) }, null);

                return CSharpExpression.TupleEqual(left, right, checksEq);
            }, new LogAndResult<bool> { Log = { "L.I1", "L.I2", "R.I1", "R.I2", "1", "2" }, Value = true });
        }

        [Fact]
        public void TupleBinary_SideEffects_TupleLiteral_Pure()
        {
            var l = Expression.Parameter(typeof(int));
            var r = Expression.Parameter(typeof(int));

            AssertCompile((log, append) =>
            {
                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.Equal(l, r)), l, r));

                var left = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Constant(1), Expression.Block(log("L.I2"), Expression.Constant(2)) }, null);
                var right = Expression.Block(log("R"), Expression.Constant((1, 2)));

                return CSharpExpression.TupleEqual(left, right, checksEq);
            }, new LogAndResult<bool> { Log = { "L.I2", "R", "1", "2" }, Value = true });

            AssertCompile((log, append) =>
            {
                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.Equal(l, r)), l, r));

                var left = Expression.Block(log("L"), Expression.Constant((1, 2)));
                var right = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("R.I1"), Expression.Constant(1)), Expression.Constant(2) }, null);

                return CSharpExpression.TupleEqual(left, right, checksEq);
            }, new LogAndResult<bool> { Log = { "L", "R.I1", "1", "2" }, Value = true });

            AssertCompile((log, append) =>
            {
                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.Equal(l, r)), l, r));

                var left = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("L.I1"), Expression.Constant(1)), Expression.Constant(2) }, null);
                var right = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Constant(1), Expression.Block(log("R.I2"), Expression.Constant(2)) }, null);

                return CSharpExpression.TupleEqual(left, right, checksEq);
            }, new LogAndResult<bool> { Log = { "L.I1", "R.I2", "1", "2" }, Value = true });

            AssertCompile((log, append) =>
            {
                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(log(i.ToString()), Expression.Equal(l, r)), l, r));

                var left = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Constant(1), Expression.Constant(2) }, null);
                var right = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Constant(1), Expression.Constant(2) }, null);

                return CSharpExpression.TupleEqual(left, right, checksEq);
            }, new LogAndResult<bool> { Log = { "1", "2" }, Value = true });
        }

        [Fact]
        public void TupleBinary_SideEffects_TupleLiteral_Nested()
        {
            // ((int, int), (int, int))

            var l = Expression.Parameter(typeof(int));
            var r = Expression.Parameter(typeof(int));

            var lNested = Expression.Parameter(typeof((int, int)));
            var rNested = Expression.Parameter(typeof((int, int)));

            AssertCompile((log, append) =>
            {
                var appendEq = Expression.Invoke(append, CSharpExpression.InterpolatedString(CSharpExpression.InterpolationStringInsert(l, format: null, alignment: null), CSharpExpression.InterpolationStringLiteral(" == "), CSharpExpression.InterpolationStringInsert(r, format: null, alignment: null)));

                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(appendEq, Expression.Equal(l, r)), l, r));
                var checksNested = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<(int, int), (int, int), bool>>(CSharpExpression.TupleEqual(lNested, rNested, checksEq), lNested, rNested));

                var leftNested1 = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("L.I1.I1"), Expression.Constant(1)), Expression.Block(log("L.I1.I2"), Expression.Constant(2)) }, null);
                var leftNested2 = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("L.I2.I1"), Expression.Constant(3)), Expression.Block(log("L.I2.I2"), Expression.Constant(4)) }, null);
                var left = CSharpExpression.TupleLiteral(typeof(((int, int), (int, int))), new Expression[] { leftNested1, leftNested2 }, null);

                var rightNested1 = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("R.I1.I1"), Expression.Constant(1)), Expression.Block(log("R.I1.I2"), Expression.Constant(2)) }, null);
                var rightNested2 = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("R.I2.I1"), Expression.Constant(3)), Expression.Block(log("R.I2.I2"), Expression.Constant(4)) }, null);
                var right = CSharpExpression.TupleLiteral(typeof(((int, int), (int, int))), new Expression[] { rightNested1, rightNested2 }, null);

                return CSharpExpression.TupleEqual(left, right, checksNested);
            }, new LogAndResult<bool> { Log = { "L.I1.I1", "L.I1.I2", "L.I2.I1", "L.I2.I2", "R.I1.I1", "R.I1.I2", "R.I2.I1", "R.I2.I2", "1 == 1", "2 == 2", "3 == 3", "4 == 4" }, Value = true });

            AssertCompile((log, append) =>
            {
                var appendEq = Expression.Invoke(append, CSharpExpression.InterpolatedString(CSharpExpression.InterpolationStringInsert(l, format: null, alignment: null), CSharpExpression.InterpolationStringLiteral(" == "), CSharpExpression.InterpolationStringInsert(r, format: null, alignment: null)));

                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(appendEq, Expression.Equal(l, r)), l, r));
                var checksNested = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<(int, int), (int, int), bool>>(CSharpExpression.TupleEqual(lNested, rNested, checksEq), lNested, rNested));

                var leftNested1 = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("L.I1.I1"), Expression.Constant(1)), Expression.Block(log("L.I1.I2"), Expression.Constant(2)) }, null);
                var leftNested2 = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("L.I2.I1"), Expression.Constant(3)), Expression.Block(log("L.I2.I2"), Expression.Constant(4)) }, null);
                var left = CSharpExpression.TupleLiteral(typeof(((int, int), (int, int))), new Expression[] { leftNested1, leftNested2 }, null);

                var rightNested1 = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("R.I1.I1"), Expression.Constant(1)), Expression.Block(log("R.I1.I2"), Expression.Constant(-2)) }, null);
                var rightNested2 = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("R.I2.I1"), Expression.Constant(3)), Expression.Block(log("R.I2.I2"), Expression.Constant(4)) }, null);
                var right = CSharpExpression.TupleLiteral(typeof(((int, int), (int, int))), new Expression[] { rightNested1, rightNested2 }, null);

                return CSharpExpression.TupleEqual(left, right, checksNested);
            }, new LogAndResult<bool> { Log = { "L.I1.I1", "L.I1.I2", "L.I2.I1", "L.I2.I2", "R.I1.I1", "R.I1.I2", "R.I2.I1", "R.I2.I2", "1 == 1", "2 == -2" }, Value = false });

            AssertCompile((log, append) =>
            {
                var appendEq = Expression.Invoke(append, CSharpExpression.InterpolatedString(CSharpExpression.InterpolationStringInsert(l, format: null, alignment: null), CSharpExpression.InterpolationStringLiteral(" == "), CSharpExpression.InterpolationStringInsert(r, format: null, alignment: null)));

                var checksEq = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<int, int, bool>>(Expression.Block(appendEq, Expression.Equal(l, r)), l, r));
                var checksNested = Enumerable.Range(1, 2).Select(i => Expression.Lambda<Func<(int, int), (int, int), bool>>(CSharpExpression.TupleEqual(lNested, rNested, checksEq), lNested, rNested));

                var leftNested1 = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("L.I1.I1"), Expression.Constant(1)), Expression.Block(log("L.I1.I2"), Expression.Constant(2)) }, null);
                var leftNested2 = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("L.I2.I1"), Expression.Constant(3)), Expression.Block(log("L.I2.I2"), Expression.Constant(4)) }, null);
                var left = CSharpExpression.TupleLiteral(typeof(((int, int), (int, int))), new Expression[] { leftNested1, leftNested2 }, null);

                var rightNested1 = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("R.I1.I1"), Expression.Constant(1)), Expression.Block(log("R.I1.I2"), Expression.Constant(2)) }, null);
                var rightNested2 = CSharpExpression.TupleLiteral(typeof((int, int)), new Expression[] { Expression.Block(log("R.I2.I1"), Expression.Constant(-3)), Expression.Block(log("R.I2.I2"), Expression.Constant(4)) }, null);
                var right = CSharpExpression.TupleLiteral(typeof(((int, int), (int, int))), new Expression[] { rightNested1, rightNested2 }, null);

                return CSharpExpression.TupleEqual(left, right, checksNested);
            }, new LogAndResult<bool> { Log = { "L.I1.I1", "L.I1.I2", "L.I2.I1", "L.I2.I2", "R.I1.I1", "R.I1.I2", "R.I2.I1", "R.I2.I2", "1 == 1", "2 == 2", "3 == -3" }, Value = false });
        }

        private void AssertCompile<T>(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<T> expected)
        {
            var res = WithLog<T>(createExpression).Compile()();
            Assert.Equal(expected, res);
        }

        // TODO: tests for various optimizations
        // TODO: tests with nested tuple literals containing nullable, e.g. (int, (int, int)?)
        // TODO: tests for dynamic equality
    }

    /*
     * TODO: Compiler test cases.
     * 
        // Non-lifted opaque
        (Expression<Func<(int,DateTime),(int,DateTime),bool>>)((t1, t2) => t1 == t2)
        (Expression<Func<(int,DateTime),(int,DateTime),bool>>)((t1, t2) => t1 != t2)

        // Non-lifted with literal
        (Expression<Func<(int,DateTime),bool>>)(t => t == (1,DateTime.Now))
        (Expression<Func<(int,DateTime),bool>>)(t => t != (1,DateTime.Now))
        (Expression<Func<(int,DateTime),bool>>)(t => (1,DateTime.Now) == t)
        (Expression<Func<(int,DateTime),bool>>)(t => (1,DateTime.Now) != t)

        // Lifted
        (Expression<Func<(int,DateTime)?,(int,DateTime)?,bool>>)((t1, t2) => t1 == t2)
        (Expression<Func<(int,DateTime)?,(int,DateTime)?,bool>>)((t1, t2) => t1 != t2)

        // Lifted with conversions
        (Expression<Func<(int,DateTime),(int,DateTime)?,bool>>)((t1, t2) => t1 == t2)
        (Expression<Func<(int,DateTime),(int,DateTime)?,bool>>)((t1, t2) => t1 != t2)
        (Expression<Func<(int,DateTime)?,(int,DateTime),bool>>)((t1, t2) => t1 == t2)
        (Expression<Func<(int,DateTime)?,(int,DateTime),bool>>)((t1, t2) => t1 != t2)

        // Nested
        (Expression<Func<(int,(string,DateTime)),(int,(string,DateTime)),bool>>)((t1, t2) => t1 == t2)
        (Expression<Func<(int,(string,DateTime)),(int,(string,DateTime)),bool>>)((t1, t2) => t1 != t2)

        // TODO: null literals
     * 
     */
}
