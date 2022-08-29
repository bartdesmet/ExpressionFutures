// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.CSharp.Expressions;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    public class InterpolatedStringTests
    {
        [Fact]
        public void InterpolatedString_Factory_ArgumentChecking()
        {
            // null checks
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.InterpolationStringLiteral(value: null));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.InterpolationStringInsert(value: null));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.InterpolationStringInsert(value: null, format: "X"));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.InterpolationStringInsert(value: null, format: "X", alignment: 1));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.InterpolationStringInsert(value: null, alignment: 1));

            // empty format string
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.InterpolationStringInsert(Expression.Constant(1), format: ""));

            // invalid type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.InterpolatedString(typeof(int)));
        }

        [Fact]
        public void InterpolatedString_Update()
        {
            var i = Expression.Constant(1);
            var e1 = CSharpExpression.InterpolationStringInsert(i);

            var e2 = e1.Update(i);
            Assert.Same(e1, e2);

            var j = Expression.Constant(2);

            var e3 = e1.Update(j);
            Assert.NotSame(e1, e3);
            Assert.Same(j, e3.Value);

            var s1 = CSharpExpression.InterpolatedString(e1);

            var s2 = s1.Update(new[] { e1 });
            Assert.Same(s1, s2);

            var s3 = s1.Update(new[] { e3 });
            Assert.NotSame(s1, s3);
            Assert.True(new[] { e3 }.SequenceEqual(s3.Interpolations));
        }

        [Fact]
        public void InterpolatedString_Visitor()
        {
            var literal = CSharpExpression.InterpolationStringLiteral("x = ");

            var i = Expression.Constant(1);
            var insert = CSharpExpression.InterpolationStringInsert(i);

            var res = CSharpExpression.InterpolatedString(literal, insert);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.VisitedInterpolatedString);
            Assert.True(v.VisitedInterpolationStringInsert);
            Assert.True(v.VisitedInterpolationStringLiteral);
        }

        class V : CSharpExpressionVisitor
        {
            public bool VisitedInterpolatedString = false;
            public bool VisitedInterpolationStringInsert = true;
            public bool VisitedInterpolationStringLiteral = true;

            protected internal override Expression VisitInterpolatedString(InterpolatedStringCSharpExpression node)
            {
                VisitedInterpolatedString = true;

                return base.VisitInterpolatedString(node);
            }

            protected internal override Interpolation VisitInterpolationStringInsert(InterpolationStringInsert node)
            {
                VisitedInterpolationStringInsert = true;

                return base.VisitInterpolationStringInsert(node);
            }

            protected internal override Interpolation VisitInterpolationStringLiteral(InterpolationStringLiteral node)
            {
                VisitedInterpolationStringLiteral = true;

                return base.VisitInterpolationStringLiteral(node);
            }
        }

        [Fact]
        public void InterpolatedString_Compile()
        {
            var res =
                CSharpExpression.InterpolatedString(
                    CSharpExpression.InterpolationStringLiteral("a = "),
                    CSharpExpression.InterpolationStringInsert(Expression.Constant(42)),
                    CSharpExpression.InterpolationStringLiteral("; b = "),
                    CSharpExpression.InterpolationStringInsert(Expression.Constant(42), "X"),
                    CSharpExpression.InterpolationStringLiteral("; c = "),
                    CSharpExpression.InterpolationStringInsert(Expression.Constant(42), -5),
                    CSharpExpression.InterpolationStringLiteral("; d = "),
                    CSharpExpression.InterpolationStringInsert(Expression.Constant(42), "X", -5)
                );

            var f = Expression.Lambda<Func<string>>(res).Compile();

            Assert.Equal($"a = {42}; b = {42:X}; c = {42,-5}; d = {42,-5:X}", f());
        }

        [Fact]
        public void InterpolatedString_Compile_IFormattable()
        {
            var res =
                CSharpExpression.InterpolatedString(
                    typeof(IFormattable),
                    CSharpExpression.InterpolationStringLiteral("a = "),
                    CSharpExpression.InterpolationStringInsert(Expression.Constant(42)),
                    CSharpExpression.InterpolationStringLiteral("; b = "),
                    CSharpExpression.InterpolationStringInsert(Expression.Constant(42), "X"),
                    CSharpExpression.InterpolationStringLiteral("; c = "),
                    CSharpExpression.InterpolationStringInsert(Expression.Constant(42), -5),
                    CSharpExpression.InterpolationStringLiteral("; d = "),
                    CSharpExpression.InterpolationStringInsert(Expression.Constant(42), "X", -5)
                );

            var f = Expression.Lambda<Func<IFormattable>>(res).Compile();

            var s = f().ToString("{0}", CultureInfo.CurrentCulture);

            Assert.Equal($"a = {42}; b = {42:X}; c = {42,-5}; d = {42,-5:X}", s);
        }

        [Fact]
        public void InterpolatedString_Compile_FormattableString()
        {
            var res =
                CSharpExpression.InterpolatedString(
                    typeof(FormattableString),
                    CSharpExpression.InterpolationStringLiteral("a = "),
                    CSharpExpression.InterpolationStringInsert(Expression.Constant(42)),
                    CSharpExpression.InterpolationStringLiteral("; b = "),
                    CSharpExpression.InterpolationStringInsert(Expression.Constant(42), "X"),
                    CSharpExpression.InterpolationStringLiteral("; c = "),
                    CSharpExpression.InterpolationStringInsert(Expression.Constant(42), -5),
                    CSharpExpression.InterpolationStringLiteral("; d = "),
                    CSharpExpression.InterpolationStringInsert(Expression.Constant(42), "X", -5)
                );

            var f = Expression.Lambda<Func<FormattableString>>(res).Compile();

            var s = f().ToString(CultureInfo.CurrentCulture);

            Assert.Equal($"a = {42}; b = {42:X}; c = {42,-5}; d = {42,-5:X}", s);
        }
    }
}
