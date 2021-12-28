﻿// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class InterpolatedStringTests
    {
        [TestMethod]
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

        [TestMethod]
        public void InterpolatedString_Update()
        {
            var i = Expression.Constant(1);
            var e1 = CSharpExpression.InterpolationStringInsert(i);

            var e2 = e1.Update(i);
            Assert.AreSame(e1, e2);

            var j = Expression.Constant(2);

            var e3 = e1.Update(j);
            Assert.AreNotSame(e1, e3);
            Assert.AreSame(j, e3.Value);

            var s1 = CSharpExpression.InterpolatedString(e1);

            var s2 = s1.Update(new[] { e1 });
            Assert.AreSame(s1, s2);

            var s3 = s1.Update(new[] { e3 });
            Assert.AreNotSame(s1, s3);
            Assert.IsTrue(new[] { e3 }.SequenceEqual(s3.Interpolations));
        }

        [TestMethod]
        public void InterpolatedString_Visitor()
        {
            var literal = CSharpExpression.InterpolationStringLiteral("x = ");

            var i = Expression.Constant(1);
            var insert = CSharpExpression.InterpolationStringInsert(i);

            var res = CSharpExpression.InterpolatedString(literal, insert);

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.VisitedInterpolatedString);
            Assert.IsTrue(v.VisitedInterpolationStringInsert);
            Assert.IsTrue(v.VisitedInterpolationStringLiteral);
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

        [TestMethod]
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

            Assert.AreEqual($"a = {42}; b = {42:X}; c = {42,-5}; d = {42,-5:X}", f());
        }

        [TestMethod]
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

            Assert.AreEqual($"a = {42}; b = {42:X}; c = {42,-5}; d = {42,-5:X}", s);
        }

        [TestMethod]
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

            Assert.AreEqual($"a = {42}; b = {42:X}; c = {42,-5}; d = {42,-5:X}", s);
        }
    }
}
