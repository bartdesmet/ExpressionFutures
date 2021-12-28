﻿// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class VarPatternTests
    {
        [TestMethod]
        public void VarPattern_ArgumentChecking()
        {
            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpPattern.Var(info: null));
            AssertEx.Throws<ArgumentNullException>(() => CSharpPattern.Var(variable: null));

            // by-ref type
            AssertEx.Throws<ArgumentException>(() => CSharpPattern.Var(variable: Expression.Variable(typeof(int).MakeByRefType())));
            AssertEx.Throws<ArgumentException>(() => CSharpPattern.Var(info: null, variable: Expression.Variable(typeof(int).MakeByRefType())));

            // invalid typing
            AssertEx.Throws<ArgumentException>(() => CSharpPattern.Var(CSharpPattern.ObjectPatternInfo(CSharpPattern.PatternInfo(typeof(object), typeof(int)), variable: null)));
            AssertEx.Throws<ArgumentException>(() => CSharpPattern.Var(CSharpPattern.ObjectPatternInfo(CSharpPattern.PatternInfo(typeof(int), typeof(int)), Expression.Variable(typeof(long)))));
        }

        [TestMethod]
        public void VarPattern_Properties()
        {
            var t = Expression.Variable(typeof(int));
            var p = CSharpPattern.Var(CSharpPattern.ObjectPatternInfo(CSharpPattern.PatternInfo(typeof(int), typeof(int)), variable: t));

            Assert.AreEqual(typeof(int), p.InputType);
            Assert.AreEqual(typeof(int), p.NarrowedType);
            Assert.AreSame(t, p.Variable);
        }

        [TestMethod]
        public void VarPattern_Update()
        {
            var t = Expression.Variable(typeof(int));
            var p = CSharpPattern.Var(CSharpPattern.ObjectPatternInfo(CSharpPattern.PatternInfo(typeof(int), typeof(int)), variable: t));

            Assert.AreSame(p, p.Update(t));

            var u = Expression.Variable(typeof(int));
            var q = p.Update(u);

            Assert.AreNotSame(p, q);
            Assert.AreSame(u, q.Variable);
        }

        [TestMethod]
        public void VarPattern_Update_Invalid()
        {
            var t = Expression.Variable(typeof(int));
            var p = CSharpPattern.Var(CSharpPattern.ObjectPatternInfo(CSharpPattern.PatternInfo(typeof(int), typeof(int)), variable: t));

            var u = Expression.Variable(typeof(long));
            AssertEx.Throws<ArgumentException>(() => p.Update(u));
        }

        [TestMethod]
        public void VarPattern_ChangeType_NoVariable()
        {
            var p = CSharpPattern.Var(CSharpPattern.ObjectPatternInfo(CSharpPattern.PatternInfo(typeof(int), typeof(int)), variable: null));

            Assert.AreSame(p, p.ChangeType(typeof(int)));

            var q = (VarCSharpPattern)p.ChangeType(typeof(long));

            Assert.AreNotSame(p, q);
            Assert.AreEqual(typeof(long), q.InputType);
            Assert.AreEqual(typeof(long), q.NarrowedType);
            Assert.IsNull(q.Variable);
        }

        [TestMethod]
        public void VarPattern_ChangeType_Variable()
        {
            var t = Expression.Variable(typeof(int));
            var p = CSharpPattern.Var(CSharpPattern.ObjectPatternInfo(CSharpPattern.PatternInfo(typeof(int), typeof(int)), variable: t));

            AssertEx.Throws<ArgumentException>(() => p.ChangeType(typeof(long)));
        }

        [TestMethod]
        public void VarPattern_Visitor()
        {
            var t = Expression.Variable(typeof(int));
            var p = CSharpPattern.Var(CSharpPattern.ObjectPatternInfo(CSharpPattern.PatternInfo(typeof(int), typeof(int)), variable: t));

            var visitor = new V();

            visitor.VisitPattern(p);

            Assert.IsTrue(visitor.Visited);
        }

        [TestMethod]
        public void VarPattern_Reduce_NoVariable()
        {
            var res = CSharpPattern.Var(CSharpPattern.ObjectPatternInfo(CSharpPattern.PatternInfo(typeof(int), typeof(int)), variable: null));

            AssertCompile((log, append) =>
            {
                var obj = Expression.Block(log("O"), Expression.Constant(42));
                return res.Reduce(obj);
            }, new LogAndResult<bool> { Value = true, Log = { "O" } });
        }

        [TestMethod]
        public void VarPattern_Reduce_Variable()
        {
            var t = Expression.Variable(typeof(int));
            var res = CSharpPattern.Var(CSharpPattern.ObjectPatternInfo(CSharpPattern.PatternInfo(typeof(int), typeof(int)), variable: t));

            AssertCompile((log, append) =>
            {
                var check = Expression.Variable(typeof(bool));
                var obj = Expression.Block(log("O"), Expression.Constant(42));
                return Expression.Block(
                    new[] { check, t },
                    Expression.Assign(check, res.Reduce(obj)),
                    Expression.Invoke(
                        append,
                        CSharpExpression.InterpolatedString(
                            CSharpExpression.InterpolationStringLiteral("check = "),
                            CSharpExpression.InterpolationStringInsert(check),
                            CSharpExpression.InterpolationStringLiteral("; value = "),
                            CSharpExpression.InterpolationStringInsert(t)
                        )
                    ),
                    check
                );
            }, new LogAndResult<bool> { Value = true, Log = { "O", "check = True; value = 42" } });
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<bool> expected)
        {
            var res = WithLog<bool>(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override CSharpPattern VisitVarPattern(VarCSharpPattern node)
            {
                Visited = true;

                return base.VisitVarPattern(node);
            }
        }
    }
}
