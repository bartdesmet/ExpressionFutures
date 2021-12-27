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
    public class FromEndIndexTests
    {
        [TestMethod]
        public void FromEndIndex_Factory_ArgumentChecking()
        {
            // null checks
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.FromEndIndex(operand: null));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.FromEndIndex(operand: null, method: null, type: null));

            // invalid operand type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(42L)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(42L), method: null, type: null));

            // invalid type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0, typeof(int)), method: null, typeof(int)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0, typeof(int)), method: null, typeof(Index?)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0, typeof(int?)), method: null, typeof(Index)));

            // invalid method
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.Generic)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.DoesNotReturnIndex)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.NotStatic)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.NoArgs)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.OneArgInvalid)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.TwoArgsInvalid1)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.TwoArgsInvalid2)), typeof(Index)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.FromEndIndex(Expression.Constant(0), typeof(InvalidIndexFactoryMethods).GetMethod(nameof(InvalidIndexFactoryMethods.TooManyArgs)), typeof(Index)));
        }

        [TestMethod]
        public void FromEndIndex_Compile()
        {
            foreach (var e in new Func<Expression, FromEndIndexCSharpExpression>[]
            {
                arg => CSharpExpression.FromEndIndex(arg),
                arg => CSharpExpression.FromEndIndex(arg, method: null, typeof(Index)),
                arg => CSharpExpression.FromEndIndex(arg, method: typeof(Index).GetConstructor(new[] { typeof(int), typeof(bool) }), typeof(Index)),
                arg => CSharpExpression.FromEndIndex(arg, method: typeof(Index).GetMethod(nameof(Index.FromEnd)), typeof(Index)),
            })
            {
                foreach (var i in new (Expression op, Index value)[]
                {
                    (Expression.Default(typeof(int)), Index.FromEnd(0)),
                    (Expression.Constant(0), Index.FromEnd(0)),
                    (Expression.Constant(1), Index.FromEnd(1)),
                })
                {
                    var expr = e(i.op);
                    Func<Index> f = Expression.Lambda<Func<Index>>(expr).Compile();
                    var res = f();
                    Assert.AreEqual(i.value, res);
                }
            }
        }

        [TestMethod]
        public void FromEndIndex_Compile_Lifted()
        {
            foreach (var e in new Func<Expression, FromEndIndexCSharpExpression>[]
            {
                arg => CSharpExpression.FromEndIndex(arg),
                arg => CSharpExpression.FromEndIndex(arg, method: null, typeof(Index?)),
                arg => CSharpExpression.FromEndIndex(arg, method: typeof(Index).GetConstructor(new[] { typeof(int), typeof(bool) }), typeof(Index?)),
                arg => CSharpExpression.FromEndIndex(arg, method: typeof(Index).GetMethod(nameof(Index.FromEnd)), typeof(Index?)),
            })
            {
                foreach (var i in new (Expression op, Index? value)[]
                {
                    (Expression.Default(typeof(int?)), null),
                    (Expression.Constant(0, typeof(int?)), Index.FromEnd(0)),
                    (Expression.Constant(1, typeof(int?)), Index.FromEnd(1)),
                })
                {
                    var expr = e(i.op);
                    Func<Index?> f = Expression.Lambda<Func<Index?>>(expr).Compile();
                    var res = f();
                    Assert.AreEqual(i.value, res);
                }
            }
        }

        class InvalidIndexFactoryMethods
        {
            public void Generic<T>() { }
            public int DoesNotReturnIndex() => 42;
            public Index NotStatic() => new Index(42);
            public static Index NoArgs() => new Index(42);
            public static Index OneArgInvalid(long x) => new Index((int)x);
            public static Index TwoArgsInvalid1(int x, int y) => new Index((int)x);
            public static Index TwoArgsInvalid2(long x, bool b) => new Index((int)x);
            public static Index TooManyArgs(int x, bool b, int y) => new Index(x, b);
        }
    }
}
