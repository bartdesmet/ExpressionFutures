// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class BlockTests
    {
        [TestMethod]
        public void Block_Factory_ArgumentChecking()
        {
            var vars = Array.Empty<ParameterExpression>();
            var exprs = Array.Empty<Expression>();
            var ret = Expression.Label();

            var p = Expression.Parameter(typeof(int));
            var dups = new[] { p, p };
            var hasNull = new Expression[] { Expression.Constant(1), null, Expression.Constant(2) };

            // duplicate variable
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Block(dups, exprs, ret));

            // null item
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Block(hasNull, ret));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Block(dups, hasNull, ret));
        }

        [TestMethod]
        public void Block_Properties()
        {
            // no vars, no expr, no label
            {
                var vars = Array.Empty<ParameterExpression>();
                var exprs = Array.Empty<Expression>();

                var b1 = CSharpStatement.Block(exprs, null);
                var b2 = CSharpStatement.Block(vars, exprs, null);

                foreach (var b in new[] { b1, b2 })
                {
                    Assert.AreEqual(CSharpExpressionType.Block, b.CSharpNodeType);
                    Assert.AreEqual(typeof(void), b.Type);
                    Assert.IsNull(b.ReturnLabel);
                    Assert.IsTrue(vars.SequenceEqual(b.Variables));
                    Assert.IsTrue(exprs.SequenceEqual(b.Statements));
                }
            }

            // no vars, no expr, void label
            {
                var vars = Array.Empty<ParameterExpression>();
                var exprs = Array.Empty<Expression>();
                var ret = Expression.Label();

                var b1 = CSharpStatement.Block(exprs, ret);
                var b2 = CSharpStatement.Block(vars, exprs, ret);

                foreach (var b in new[] { b1, b2 })
                {
                    Assert.AreEqual(CSharpExpressionType.Block, b.CSharpNodeType);
                    Assert.AreEqual(typeof(void), b.Type);
                    Assert.AreSame(ret, b.ReturnLabel);
                    Assert.IsTrue(vars.SequenceEqual(b.Variables));
                    Assert.IsTrue(exprs.SequenceEqual(b.Statements));
                }
            }

            // no vars, no expr, int label
            {
                var vars = Array.Empty<ParameterExpression>();
                var exprs = Array.Empty<Expression>();
                var ret = Expression.Label(typeof(int));

                var b1 = CSharpStatement.Block(exprs, ret);
                var b2 = CSharpStatement.Block(vars, exprs, ret);

                foreach (var b in new[] { b1, b2 })
                {
                    Assert.AreEqual(CSharpExpressionType.Block, b.CSharpNodeType);
                    Assert.AreEqual(typeof(int), b.Type);
                    Assert.AreSame(ret, b.ReturnLabel);
                    Assert.IsTrue(vars.SequenceEqual(b.Variables));
                    Assert.IsTrue(exprs.SequenceEqual(b.Statements));
                }
            }

            // no vars, one expr, no label
            {
                var vars = Array.Empty<ParameterExpression>();
                var exprs = new[] { Expression.Constant("bar") };

                var b1 = CSharpStatement.Block(exprs, null);
                var b2 = CSharpStatement.Block(vars, exprs, null);

                foreach (var b in new[] { b1, b2 })
                {
                    Assert.AreEqual(CSharpExpressionType.Block, b.CSharpNodeType);
                    Assert.AreEqual(typeof(string), b.Type);
                    Assert.IsNull(b.ReturnLabel);
                    Assert.IsTrue(vars.SequenceEqual(b.Variables));
                    Assert.IsTrue(exprs.SequenceEqual(b.Statements));
                }
            }

            // no vars, one expr, void label
            {
                var vars = Array.Empty<ParameterExpression>();
                var exprs = new Expression[] { Expression.Empty() };
                var ret = Expression.Label();

                var b1 = CSharpStatement.Block(exprs, ret);
                var b2 = CSharpStatement.Block(vars, exprs, ret);

                foreach (var b in new[] { b1, b2 })
                {
                    Assert.AreEqual(CSharpExpressionType.Block, b.CSharpNodeType);
                    Assert.AreEqual(typeof(void), b.Type);
                    Assert.AreSame(ret, b.ReturnLabel);
                    Assert.IsTrue(vars.SequenceEqual(b.Variables));
                    Assert.IsTrue(exprs.SequenceEqual(b.Statements));
                }
            }

            // no vars, one expr, int label
            {
                var vars = Array.Empty<ParameterExpression>();
                var exprs = new[] { Expression.Constant("bar") };
                var ret = Expression.Label(typeof(int));

                var b1 = CSharpStatement.Block(exprs, ret);
                var b2 = CSharpStatement.Block(vars, exprs, ret);

                foreach (var b in new[] { b1, b2 })
                {
                    Assert.AreEqual(CSharpExpressionType.Block, b.CSharpNodeType);
                    Assert.AreEqual(typeof(int), b.Type);
                    Assert.AreSame(ret, b.ReturnLabel);
                    Assert.IsTrue(vars.SequenceEqual(b.Variables));
                    Assert.IsTrue(exprs.SequenceEqual(b.Statements));
                }
            }

            // one var, one expr, no label
            {
                var vars = new[] { Expression.Parameter(typeof(int)) };
                var exprs = new[] { Expression.Constant("bar") };

                var b = CSharpStatement.Block(vars, exprs, null);

                Assert.AreEqual(CSharpExpressionType.Block, b.CSharpNodeType);
                Assert.AreEqual(typeof(string), b.Type);
                Assert.IsNull(b.ReturnLabel);
                Assert.IsTrue(vars.SequenceEqual(b.Variables));
                Assert.IsTrue(exprs.SequenceEqual(b.Statements));
            }

            // one var, one expr, void label
            {
                var vars = new[] { Expression.Parameter(typeof(int)) };
                var exprs = new Expression[] { Expression.Empty() };
                var ret = Expression.Label();

                var b = CSharpStatement.Block(vars, exprs, ret);

                Assert.AreEqual(CSharpExpressionType.Block, b.CSharpNodeType);
                Assert.AreEqual(typeof(void), b.Type);
                Assert.AreSame(ret, b.ReturnLabel);
                Assert.IsTrue(vars.SequenceEqual(b.Variables));
                Assert.IsTrue(exprs.SequenceEqual(b.Statements));
            }

            // one var, one expr, int label
            {
                var vars = new[] { Expression.Parameter(typeof(int)) };
                var exprs = new[] { Expression.Constant("bar") };
                var ret = Expression.Label(typeof(int));

                var b = CSharpStatement.Block(vars, exprs, ret);

                Assert.AreEqual(CSharpExpressionType.Block, b.CSharpNodeType);
                Assert.AreEqual(typeof(int), b.Type);
                Assert.AreSame(ret, b.ReturnLabel);
                Assert.IsTrue(vars.SequenceEqual(b.Variables));
                Assert.IsTrue(exprs.SequenceEqual(b.Statements));
            }
        }

        [TestMethod]
        public void Block_Update()
        {
            var vars = new[] { Expression.Parameter(typeof(int)) };
            var exprs = new[] { Expression.Constant("bar") };
            var ret = Expression.Label(typeof(int));
            var res = CSharpStatement.Block(vars, exprs, ret);

            Assert.AreSame(res, res.Update(res.Variables, res.Statements, res.ReturnLabel));

            var newVars = new[] { Expression.Parameter(typeof(int)) };
            var newExprs = new[] { Expression.Constant("bar") };
            var newRet = Expression.Label(typeof(int));

            var upd1 = res.Update(newVars, res.Statements, res.ReturnLabel);
            var upd2 = res.Update(res.Variables, newExprs, res.ReturnLabel);
            var upd3 = res.Update(res.Variables, res.Statements, newRet);

            Assert.IsTrue(newVars.SequenceEqual(upd1.Variables));
            Assert.IsTrue(res.Statements.SequenceEqual(upd1.Statements));
            Assert.AreSame(res.ReturnLabel, upd1.ReturnLabel);

            Assert.IsTrue(res.Variables.SequenceEqual(upd2.Variables));
            Assert.IsTrue(newExprs.SequenceEqual(upd2.Statements));
            Assert.AreSame(res.ReturnLabel, upd2.ReturnLabel);

            Assert.IsTrue(res.Variables.SequenceEqual(upd3.Variables));
            Assert.IsTrue(res.Statements.SequenceEqual(upd3.Statements));
            Assert.AreSame(newRet, upd3.ReturnLabel);
        }

        [TestMethod]
        public void Block_Compile1()
        {
            var p = Expression.Parameter(typeof(int));
            var l = Expression.Label(typeof(int));
            var b = CSharpStatement.Block(Array.Empty<ParameterExpression>(), new Expression[] { Expression.Return(l, p) }, l);

            var f = Expression.Lambda<Func<int, int>>(b, p).Compile();

            Assert.AreEqual(42, f(42));
        }

        [TestMethod]
        public void Block_Compile2()
        {
            var p = Expression.Parameter(typeof(int));
            var q = Expression.Parameter(typeof(int));
            var l = Expression.Label(typeof(int));
            var b = CSharpStatement.Block(new[] { q }, new Expression[] { Expression.Assign(q, p), Expression.Return(l, q) }, l);

            var f = Expression.Lambda<Func<int, int>>(b, p).Compile();

            Assert.AreEqual(42, f(42));
        }

        [TestMethod]
        public void Block_Compile3()
        {
            var l = Expression.Label();

            AssertCompileVoid(log =>
                CSharpStatement.Block(
                    Array.Empty<ParameterExpression>(),
                    new Expression[]
                    {
                        log("E"),
                        Expression.Return(l),
                        log("X"),
                    },
                    l
                ),
                new LogAndResult<object> { Log = { "E" } }
            );
        }

        [TestMethod]
        public void Block_Compile4()
        {
            var l = Expression.Label(typeof(int));

            AssertCompile<int>(log =>
                CSharpStatement.Block(
                    Array.Empty<ParameterExpression>(),
                    new Expression[]
                    {
                        log("E"),
                        Expression.Return(l, Expression.Constant(42)),
                        log("X"),
                    },
                    l
                ),
                new LogAndResult<int> { Value = 42, Log = { "E" } }
            );
        }

        [TestMethod]
        public void Block_Compile5()
        {
            var l = Expression.Label();

            AssertCompileVoid(log =>
                CSharpStatement.Block(
                    Array.Empty<ParameterExpression>(),
                    Array.Empty<Expression>(),
                    l
                ),
                new LogAndResult<object> { Log = { } }
            );
        }

        [TestMethod]
        public void Block_Compile6()
        {
            var l = Expression.Label(typeof(int));

            AssertCompile<int>(log =>
                CSharpStatement.Block(
                    Array.Empty<ParameterExpression>(),
                    Array.Empty<Expression>(),
                    l
                ),
                new LogAndResult<int> { Value = 0, Log = { } }
            );
        }

        [TestMethod]
        public void Block_Compile7()
        {
            AssertCompileVoid(log =>
                CSharpStatement.Block(
                    Array.Empty<ParameterExpression>(),
                    Array.Empty<Expression>(),
                    default(LabelTarget) // too subtle? if omitted, binds to regular Block
                ),
                new LogAndResult<object> { Log = { } }
            );
        }

        [TestMethod]
        public void Block_Compile8()
        {
            AssertCompile<int>(log =>
                CSharpStatement.Block(
                    Array.Empty<ParameterExpression>(),
                    new[]
                    {
                        log("E"),
                        Expression.Constant(42)
                    },
                    default(LabelTarget) // too subtle? if omitted, binds to regular Block
                ),
                new LogAndResult<int> { Value = 42, Log = { "E" } }
            );
        }

        [TestMethod]
        public void Block_Compile9()
        {
            var x = Expression.Parameter(typeof(int));

            AssertCompile<int>(log =>
                CSharpStatement.Block(
                    new[] { x },
                    new Expression[]
                    {
                        log("E"),
                        Expression.Assign(x, Expression.Constant(42)),
                        x
                    },
                    default(LabelTarget) // too subtle? if omitted, binds to regular Block
                ),
                new LogAndResult<int> { Value = 42, Log = { "E" } }
            );
        }

        private void AssertCompileVoid(Func<Func<string, Expression>, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }

        private void AssertCompile<T>(Func<Func<string, Expression>, Expression> createExpression, LogAndResult<T> expected)
        {
            var res = WithLog<T>(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        public void Block_Visitor()
        {
            var vars = new[] { Expression.Parameter(typeof(int)) };
            var exprs = new[] { Expression.Constant("bar") };
            var ret = Expression.Label(typeof(int));
            var res = CSharpStatement.Block(vars, exprs, ret);

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitBlock(BlockCSharpExpression node)
            {
                Visited = true;

                return base.VisitBlock(node);
            }
        }
    }
}
