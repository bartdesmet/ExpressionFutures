// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class DoTests
    {
        [TestMethod]
        public void Do_Factory_ArgumentChecking()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Do(default(Expression), test));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Do(body, default(Expression)));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Do(default(Expression), test, breakLabel));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Do(body, default(Expression), breakLabel));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Do(default(Expression), test, breakLabel, continueLabel));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Do(body, default(Expression), breakLabel, continueLabel));

            // non-bool
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Do(body, Expression.Default(typeof(int))));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Do(body, Expression.Default(typeof(int)), breakLabel));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Do(body, Expression.Default(typeof(int)), breakLabel, continueLabel));

            // labels must be void
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Do(body, test, Expression.Label(typeof(int))));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Do(body, test, breakLabel, Expression.Label(typeof(int))));
        }

        [TestMethod]
        public void Do_Properties()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();

            {
                var res = CSharpExpression.Do(body, test);

                Assert.AreEqual(CSharpExpressionType.Do, res.CSharpNodeType);
                Assert.AreEqual(typeof(void), res.Type);
                Assert.AreSame(test, res.Test);
                Assert.AreSame(body, res.Body);
                Assert.IsNull(res.BreakLabel);
                Assert.IsNull(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.Do(body, test, breakLabel);

                Assert.AreEqual(CSharpExpressionType.Do, res.CSharpNodeType);
                Assert.AreEqual(typeof(void), res.Type);
                Assert.AreSame(test, res.Test);
                Assert.AreSame(body, res.Body);
                Assert.AreSame(breakLabel, res.BreakLabel);
                Assert.IsNull(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.Do(body, test, breakLabel, continueLabel);

                Assert.AreEqual(CSharpExpressionType.Do, res.CSharpNodeType);
                Assert.AreEqual(typeof(void), res.Type);
                Assert.AreSame(test, res.Test);
                Assert.AreSame(body, res.Body);
                Assert.AreSame(breakLabel, res.BreakLabel);
                Assert.AreSame(continueLabel, res.ContinueLabel);
            }
        }

        [TestMethod]
        public void Do_Update()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();
            var res = CSharpExpression.Do(body, test, breakLabel, continueLabel);

            Assert.AreSame(res, res.Update(res.BreakLabel, res.ContinueLabel, res.Body, res.Test));

            var newTest = Expression.Constant(true);
            var newBody = Expression.Empty();
            var newBreakLabel = Expression.Label();
            var newContinueLabel = Expression.Label();

            var upd1 = res.Update(newBreakLabel, res.ContinueLabel, res.Body, res.Test);
            var upd2 = res.Update(res.BreakLabel, newContinueLabel, res.Body, res.Test);
            var upd3 = res.Update(res.BreakLabel, res.ContinueLabel, newBody, res.Test);
            var upd4 = res.Update(res.BreakLabel, res.ContinueLabel, res.Body, newTest);

            Assert.AreSame(newBreakLabel, upd1.BreakLabel);
            Assert.AreSame(res.ContinueLabel, upd1.ContinueLabel);
            Assert.AreSame(res.Body, upd1.Body);
            Assert.AreSame(res.Test, upd1.Test);

            Assert.AreSame(res.BreakLabel, upd2.BreakLabel);
            Assert.AreSame(newContinueLabel, upd2.ContinueLabel);
            Assert.AreSame(res.Body, upd2.Body);
            Assert.AreSame(res.Test, upd2.Test);

            Assert.AreSame(res.BreakLabel, upd3.BreakLabel);
            Assert.AreSame(res.ContinueLabel, upd3.ContinueLabel);
            Assert.AreSame(newBody, upd3.Body);
            Assert.AreSame(res.Test, upd3.Test);

            Assert.AreSame(res.BreakLabel, upd4.BreakLabel);
            Assert.AreSame(res.ContinueLabel, upd4.ContinueLabel);
            Assert.AreSame(res.Body, upd4.Body);
            Assert.AreSame(newTest, upd4.Test);
        }

        [TestMethod]
        public void Do_Compile()
        {
            var i = Expression.Parameter(typeof(int));

            AssertCompile(log =>
                Expression.Block(
                    new[] { i },
                    CSharpExpression.Do(
                        log("B"),
                        Expression.NotEqual(i, Expression.Constant(0))
                    )
                ),
                new LogAndResult<object> { Log = { "B" } }
            );

            AssertCompile(log =>
                Expression.Block(
                    new[] { i },
                    CSharpExpression.Do(
                        Expression.Block(
                            log("B"),
                            Expression.PostIncrementAssign(i)
                        ),
                        Expression.LessThan(i, Expression.Constant(3))
                    )
                ),
                new LogAndResult<object> { Log = { "B", "B", "B" } }
            );
        }

        [TestMethod]
        public void Do_Compile_BreakContinue()
        {
            var i = Expression.Parameter(typeof(int));
            var brk = Expression.Label();
            var cnt = Expression.Label();

            AssertCompile((log, append) =>
                Expression.Block(
                    new[] { i },
                    CSharpExpression.Do(
                        Expression.Block(
                            Expression.PostIncrementAssign(i),
                            Expression.IfThen(
                                Expression.Equal(Expression.Modulo(i, Expression.Constant(2)), Expression.Constant(0)),
                                Expression.Continue(cnt)
                            ),
                            Expression.IfThen(
                                Expression.GreaterThan(i, Expression.Constant(5)),
                                Expression.Break(brk)
                            ),
                            Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                        ),
                        Expression.LessThan(i, Expression.Constant(10)),
                        brk,
                        cnt
                    )
                ),
                new LogAndResult<object> { Log = { "1", "3", "5" } }
            );
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        public void Do_Visitor()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var res = CSharpExpression.Do(body, test);

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitDo(DoCSharpStatement node)
            {
                Visited = true;

                return base.VisitDo(node);
            }
        }
    }
}
