// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.CSharp.Expressions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    public class RangeTests
    {
        [Fact]
        public void Range_Factory_ArgumentChecking()
        {
            var arg1 = Expression.Constant(new Index(0));
            var arg2 = Expression.Constant(new Index(1));
            var argn1 = Expression.Constant(new Index(0), typeof(Index?));
            var argn2 = Expression.Constant(new Index(1), typeof(Index?));

            // invalid method
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.Generic)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.DoesNotReturnRange)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.NotStatic)), typeof(Range)));

            // invalid method (argument count mismatch)
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, null, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.NoArgs)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, null, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.TwoArgs)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, null, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.ThreeArgs)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(null, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.NoArgs)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(null, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.TwoArgs)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(null, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.ThreeArgs)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.NoArgs)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.OneArg)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.ThreeArgs)), typeof(Range)));

            // invalid method (argument type)
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, null, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.OneArgInvalid)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(null, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.OneArgInvalid)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.TwoArgsInvalid1)), typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.TwoArgsInvalid2)), typeof(Range)));

            // invalid type
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, method: null, typeof(int)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, method: null, typeof(Index)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, method: null, typeof(Range?)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(argn1, arg2, method: null, typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, argn2, method: null, typeof(Range)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(argn1, argn2, method: null, typeof(Range)));

            // invalid operand type
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(Expression.Constant(0L), null));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Range(null, Expression.Constant(1L)));
        }

        [Fact]
        public void Range_Factory_Int32()
        {
            var range = CSharpExpression.Range(Expression.Constant(1), Expression.Constant(2));
            Assert.True(range.Left is UnaryExpression ul && ul.NodeType == ExpressionType.Convert);
            Assert.True(range.Right is UnaryExpression ur && ur.NodeType == ExpressionType.Convert);

            var rangen = CSharpExpression.Range(Expression.Constant(1, typeof(int?)), Expression.Constant(2, typeof(int?)));
            Assert.True(rangen.Left is UnaryExpression uln && uln.NodeType == ExpressionType.Convert);
            Assert.True(rangen.Right is UnaryExpression urn && urn.NodeType == ExpressionType.Convert);
        }

        [Fact]
        public void Range_Update()
        {
            var from1 = Expression.Constant(new Index(1));
            var from2 = Expression.Constant(new Index(3));
            var to1 = Expression.Constant(new Index(2));
            var to2 = Expression.Constant(new Index(4));

            var e1 = CSharpExpression.Range(from1, to1);

            var e2 = e1.Update(from1, to1);
            Assert.Same(e1, e2);

            var e3 = e1.Update(from2, to1);
            Assert.NotSame(e1, e3);
            Assert.Same(from2, e3.Left);
            Assert.Same(to1, e3.Right);

            var e4 = e1.Update(from1, to2);
            Assert.NotSame(e1, e4);
            Assert.Same(from1, e4.Left);
            Assert.Same(to2, e4.Right);

            var e5 = e1.Update(from2, to2);
            Assert.NotSame(e1, e5);
            Assert.Same(from2, e5.Left);
            Assert.Same(to2, e5.Right);
        }

        [Fact]
        public void Range_Visitor()
        {
            var from = new Index(1);
            var to = new Index(2);
            var res = CSharpExpression.Range(Expression.Constant(from), Expression.Constant(to));

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitRange(RangeCSharpExpression node)
            {
                Visited = true;

                return base.VisitRange(node);
            }
        }

        [Fact]
        public void Range_Compile()
        {
            var idx1 = new Index(1);
            var idx2 = new Index(2);
            var arg1 = Expression.Constant(idx1);
            var arg2 = Expression.Constant(idx2);

            foreach (var e in new Func<Expression, Expression, RangeCSharpExpression>[]
            {
                (l, r) => CSharpExpression.Range(l, r),
                (l, r) => CSharpExpression.Range(l, r, method: null, typeof(Range)),
            })
            {
                foreach (var i in new (Expression start, Expression end, Range value)[]
                {
                    (arg1, arg2, new Range(idx1, idx2)),
                    (arg1, null, Range.StartAt(idx1)),
                    (null, arg2, Range.EndAt(idx2)),
                    (null, null, Range.All),
                })
                {
                    var expr = e(i.start, i.end);
                    Func<Range> f = Expression.Lambda<Func<Range>>(expr).Compile();
                    var res = f();
                    Assert.Equal(i.value, res);
                }
            }

            foreach (var e in new Func<Expression, Expression, RangeCSharpExpression>[]
            {
                (l, r) => CSharpExpression.Range(l, r),
                (l, r) => CSharpExpression.Range(l, r, method: null, typeof(Range)),
                (l, r) => CSharpExpression.Range(l, r, method: typeof(Range).GetConstructor(new[] { typeof(Index), typeof(Index) }), typeof(Range)),
            })
            {
                foreach (var i in new (Expression start, Expression end, Range value)[]
                {
                    (arg1, arg2, new Range(idx1, idx2)),
                })
                {
                    var expr = e(i.start, i.end);
                    Func<Range> f = Expression.Lambda<Func<Range>>(expr).Compile();
                    var res = f();
                    Assert.Equal(i.value, res);
                }
            }

            foreach (var e in new Func<Expression, RangeCSharpExpression>[]
            {
                l => CSharpExpression.Range(l, null),
                l => CSharpExpression.Range(l, null, method: null, typeof(Range)),
                l => CSharpExpression.Range(l, null, method: typeof(Range).GetMethod(nameof(Range.StartAt)), typeof(Range)),
            })
            {
                foreach (var i in new (Expression start, Range value)[]
                {
                    (arg1, Range.StartAt(idx1)),
                })
                {
                    var expr = e(i.start);
                    Func<Range> f = Expression.Lambda<Func<Range>>(expr).Compile();
                    var res = f();
                    Assert.Equal(i.value, res);
                }
            }

            foreach (var e in new Func<Expression, RangeCSharpExpression>[]
            {
                r => CSharpExpression.Range(null, r),
                r => CSharpExpression.Range(null, r, method: null, typeof(Range)),
                r => CSharpExpression.Range(null, r, method: typeof(Range).GetMethod(nameof(Range.EndAt)), typeof(Range)),
            })
            {
                foreach (var i in new (Expression end, Range value)[]
                {
                    (arg2, Range.EndAt(idx2)),
                })
                {
                    var expr = e(i.end);
                    Func<Range> f = Expression.Lambda<Func<Range>>(expr).Compile();
                    var res = f();
                    Assert.Equal(i.value, res);
                }
            }

            foreach (var e in new Func<RangeCSharpExpression>[]
            {
                () => CSharpExpression.Range(null, null),
                () => CSharpExpression.Range(null, null, method: null, typeof(Range)),
                () => CSharpExpression.Range(null, null, method: typeof(Range).GetProperty(nameof(Range.All)).GetGetMethod(), typeof(Range)),
            })
            {
                var expr = e();
                var f = Expression.Lambda<Func<Range>>(expr).Compile();
                var res = f();
                Assert.Equal(Range.All, res);
            }
        }

        [Fact]
        public void Range_Compile_Lifted()
        {
            var idx1 = new Index(1);
            var idx2 = new Index(2);
            var arg1 = Expression.Constant(idx1);
            var arg2 = Expression.Constant(idx2);
            var arg1n = Expression.Constant(idx1, typeof(Index?));
            var arg2n = Expression.Constant(idx2, typeof(Index?));
            var argn = Expression.Constant(null, typeof(Index?));

            foreach (var e in new Func<Expression, Expression, RangeCSharpExpression>[]
            {
                (l, r) => CSharpExpression.Range(l, r),
                (l, r) => CSharpExpression.Range(l, r, method: null, typeof(Range?)),
            })
            {
                foreach (var i in new (Expression start, Expression end, Range? value)[]
                {
                    (arg1n, arg2, new Range(idx1, idx2)),
                    (arg1, arg2n, new Range(idx1, idx2)),
                    (arg1n, arg2n, new Range(idx1, idx2)),
                    (arg1n, null, Range.StartAt(idx1)),
                    (null, arg2n, Range.EndAt(idx2)),

                    (argn, arg2, null),
                    (arg1, argn, null),
                    (argn, argn, null),
                })
                {
                    var expr = e(i.start, i.end);
                    var f = Expression.Lambda<Func<Range?>>(expr).Compile();
                    var res = f();
                    Assert.Equal(i.value, res);
                }
            }

            foreach (var e in new Func<Expression, Expression, RangeCSharpExpression>[]
            {
                (l, r) => CSharpExpression.Range(l, r),
                (l, r) => CSharpExpression.Range(l, r, method: null, typeof(Range?)),
                (l, r) => CSharpExpression.Range(l, r, method: typeof(Range).GetConstructor(new[] { typeof(Index), typeof(Index) }), typeof(Range?)),
            })
            {
                foreach (var i in new (Expression start, Expression end, Range? value)[]
                {
                    (arg1n, arg2, new Range(idx1, idx2)),
                    (arg1, arg2n, new Range(idx1, idx2)),
                    (arg1n, arg2n, new Range(idx1, idx2)),

                    (argn, arg2, null),
                    (arg1, argn, null),
                    (argn, argn, null),
                })
                {
                    var expr = e(i.start, i.end);
                    var f = Expression.Lambda<Func<Range?>>(expr).Compile();
                    var res = f();
                    Assert.Equal(i.value, res);
                }
            }

            foreach (var e in new Func<Expression, RangeCSharpExpression>[]
            {
                l => CSharpExpression.Range(l, null),
                l => CSharpExpression.Range(l, null, method: null, typeof(Range?)),
                l => CSharpExpression.Range(l, null, method: typeof(Range).GetMethod(nameof(Range.StartAt)), typeof(Range?)),
            })
            {
                foreach (var i in new (Expression start, Range? value)[]
                {
                    (arg1n, Range.StartAt(idx1)),
                    (argn, null),
                })
                {
                    var expr = e(i.start);
                    var f = Expression.Lambda<Func<Range?>>(expr).Compile();
                    var res = f();
                    Assert.Equal(i.value, res);
                }
            }

            foreach (var e in new Func<Expression, RangeCSharpExpression>[]
            {
                r => CSharpExpression.Range(null, r),
                r => CSharpExpression.Range(null, r, method: null, typeof(Range?)),
                r => CSharpExpression.Range(null, r, method: typeof(Range).GetMethod(nameof(Range.EndAt)), typeof(Range?)),
            })
            {
                foreach (var i in new (Expression end, Range? value)[]
                {
                    (arg2n, Range.EndAt(idx2)),
                    (argn, null),
                })
                {
                    var expr = e(i.end);
                    var f = Expression.Lambda<Func<Range?>>(expr).Compile();
                    var res = f();
                    Assert.Equal(i.value, res);
                }
            }
        }

        class InvalidRangeFactoryMethods
        {
            public void Generic<T>() { }
            public int DoesNotReturnRange() => 42;
            public Range NotStatic() => new Range(1, 2);
            public static Range NoArgs() => new Range();
            public static Range OneArg(Index i) => Range.StartAt(i);
            public static Range TwoArgs(Index i, Index j) => new Range(i, j);
            public static Range ThreeArgs(Index i, Index j, Index k) => new Range(i, j);
            public static Range OneArgInvalid(int x) => throw new Exception();
            public static Range TwoArgsInvalid1(int i, Index j) => throw new Exception();
            public static Range TwoArgsInvalid2(Index i, int j) => throw new Exception();
        }
    }
}
