// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class RangeTests
    {
        [TestMethod]
        public void Range_Factory_ArgumentChecking()
        {
            var arg1 = Expression.Constant(new Index(0));
            var arg2 = Expression.Constant(new Index(1));
            var argn1 = Expression.Constant(new Index(0), typeof(Index?));
            var argn2 = Expression.Constant(new Index(1), typeof(Index?));

            // invalid method
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.Generic)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.DoesNotReturnRange)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.NotStatic)), typeof(Range)));

            // invalid method (argument count mismatch)
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, null, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.NoArgs)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, null, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.TwoArgs)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, null, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.ThreeArgs)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(null, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.NoArgs)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(null, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.TwoArgs)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(null, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.ThreeArgs)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.NoArgs)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.OneArg)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.ThreeArgs)), typeof(Range)));

            // invalid method (argument type)
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, null, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.OneArgInvalid)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(null, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.OneArgInvalid)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.TwoArgsInvalid1)), typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, typeof(InvalidRangeFactoryMethods).GetMethod(nameof(InvalidRangeFactoryMethods.TwoArgsInvalid2)), typeof(Range)));

            // invalid type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, method: null, typeof(int)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, method: null, typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, arg2, method: null, typeof(Range?)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(argn1, arg2, method: null, typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(arg1, argn2, method: null, typeof(Range)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(argn1, argn2, method: null, typeof(Range)));

            // invalid operand type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(Expression.Constant(0L), null));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Range(null, Expression.Constant(1L)));
        }

        [TestMethod]
        public void Range_Factory_Int32()
        {
            var range = CSharpExpression.Range(Expression.Constant(1), Expression.Constant(2));
            Assert.IsTrue(range.Left is UnaryExpression ul && ul.NodeType == ExpressionType.Convert);
            Assert.IsTrue(range.Right is UnaryExpression ur && ur.NodeType == ExpressionType.Convert);

            var rangen = CSharpExpression.Range(Expression.Constant(1, typeof(int?)), Expression.Constant(2, typeof(int?)));
            Assert.IsTrue(rangen.Left is UnaryExpression uln && uln.NodeType == ExpressionType.Convert);
            Assert.IsTrue(rangen.Right is UnaryExpression urn && urn.NodeType == ExpressionType.Convert);
        }

        [TestMethod]
        public void Range_Update()
        {
            var from1 = Expression.Constant(new Index(1));
            var from2 = Expression.Constant(new Index(3));
            var to1 = Expression.Constant(new Index(2));
            var to2 = Expression.Constant(new Index(4));

            var e1 = CSharpExpression.Range(from1, to1);

            var e2 = e1.Update(from1, to1);
            Assert.AreSame(e1, e2);

            var e3 = e1.Update(from2, to1);
            Assert.AreNotSame(e1, e3);
            Assert.AreSame(from2, e3.Left);
            Assert.AreSame(to1, e3.Right);

            var e4 = e1.Update(from1, to2);
            Assert.AreNotSame(e1, e4);
            Assert.AreSame(from1, e4.Left);
            Assert.AreSame(to2, e4.Right);

            var e5 = e1.Update(from2, to2);
            Assert.AreNotSame(e1, e5);
            Assert.AreSame(from2, e5.Left);
            Assert.AreSame(to2, e5.Right);
        }

        [TestMethod]
        public void Range_Visitor()
        {
            var from = new Index(1);
            var to = new Index(2);
            var res = CSharpExpression.Range(Expression.Constant(from), Expression.Constant(to));

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
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
