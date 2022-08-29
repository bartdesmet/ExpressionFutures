// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq.Expressions;
using Xunit;
using static Tests.TestHelpers;

namespace Tests
{
    public class WhileTests
    {
        [Fact]
        public void While_Factory_ArgumentChecking()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.While(test, default(Expression)));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.While(default(Expression), body));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.While(test, default(Expression), breakLabel));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.While(default(Expression), body, breakLabel));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.While(test, default(Expression), breakLabel, continueLabel));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.While(default(Expression), body, breakLabel, continueLabel));

            // non-bool
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.While(Expression.Default(typeof(int)), body));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.While(Expression.Default(typeof(int)), body, breakLabel));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.While(Expression.Default(typeof(int)), body, breakLabel, continueLabel));

            // labels must be void
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.While(test, body, Expression.Label(typeof(int))));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.While(test, body, breakLabel, Expression.Label(typeof(int))));
        }

        [Fact]
        public void While_Properties()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();

            {
                var res = CSharpExpression.While(test, body);

                Assert.Equal(CSharpExpressionType.While, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.Same(test, res.Test);
                Assert.Same(body, res.Body);
                Assert.Null(res.BreakLabel);
                Assert.Null(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.While(test, body, breakLabel);

                Assert.Equal(CSharpExpressionType.While, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.Same(test, res.Test);
                Assert.Same(body, res.Body);
                Assert.Same(breakLabel, res.BreakLabel);
                Assert.Null(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.While(test, body, breakLabel, continueLabel);

                Assert.Equal(CSharpExpressionType.While, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.Same(test, res.Test);
                Assert.Same(body, res.Body);
                Assert.Same(breakLabel, res.BreakLabel);
                Assert.Same(continueLabel, res.ContinueLabel);
            }
        }

        [Fact]
        public void While_Update()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();
            var res = CSharpExpression.While(test, body, breakLabel, continueLabel);

            Assert.Same(res, res.Update(res.BreakLabel, res.ContinueLabel, res.Test, res.Body, res.Locals));

            var newTest = Expression.Constant(true);
            var newBody = Expression.Empty();
            var newBreakLabel = Expression.Label();
            var newContinueLabel = Expression.Label();

            var upd1 = res.Update(newBreakLabel, res.ContinueLabel, res.Test, res.Body, res.Locals);
            var upd2 = res.Update(res.BreakLabel, newContinueLabel, res.Test, res.Body, res.Locals);
            var upd3 = res.Update(res.BreakLabel, res.ContinueLabel, newTest, res.Body, res.Locals);
            var upd4 = res.Update(res.BreakLabel, res.ContinueLabel, res.Test, newBody, res.Locals);

            Assert.Same(newBreakLabel, upd1.BreakLabel);
            Assert.Same(res.ContinueLabel, upd1.ContinueLabel);
            Assert.Same(res.Test, upd1.Test);
            Assert.Same(res.Body, upd1.Body);
            Assert.Same(res.Locals, upd1.Locals);

            Assert.Same(res.BreakLabel, upd2.BreakLabel);
            Assert.Same(newContinueLabel, upd2.ContinueLabel);
            Assert.Same(res.Test, upd2.Test);
            Assert.Same(res.Body, upd2.Body);
            Assert.Same(res.Locals, upd2.Locals);

            Assert.Same(res.BreakLabel, upd3.BreakLabel);
            Assert.Same(res.ContinueLabel, upd3.ContinueLabel);
            Assert.Same(newTest, upd3.Test);
            Assert.Same(res.Body, upd3.Body);
            Assert.Same(res.Locals, upd3.Locals);

            Assert.Same(res.BreakLabel, upd4.BreakLabel);
            Assert.Same(res.ContinueLabel, upd4.ContinueLabel);
            Assert.Same(res.Test, upd4.Test);
            Assert.Same(newBody, upd4.Body);
            Assert.Same(res.Locals, upd4.Locals);
        }

        [Fact]
        public void While_Compile()
        {
            var i = Expression.Parameter(typeof(int));

            AssertCompile(log =>
                Expression.Block(
                    new[] { i },
                    CSharpExpression.While(
                        Expression.NotEqual(i, Expression.Constant(0)),
                        log("B")
                    )
                ),
                new LogAndResult<object> { Log = { } }
            );

            AssertCompile(log =>
                Expression.Block(
                    new[] { i },
                    CSharpExpression.While(
                        Expression.LessThan(i, Expression.Constant(3)),
                        Expression.Block(
                            log("B"),
                            Expression.PostIncrementAssign(i)
                        )
                    )
                ),
                new LogAndResult<object> { Log = { "B", "B", "B" } }
            );
        }

        [Fact]
        public void While_Compile_BreakContinue()
        {
            var i = Expression.Parameter(typeof(int));
            var brk = Expression.Label();
            var cnt = Expression.Label();

            AssertCompile((log, append) =>
                Expression.Block(
                    new[] { i },
                    CSharpExpression.While(
                        Expression.LessThan(i, Expression.Constant(10)),
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
            Assert.Equal(expected, res);
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.Equal(expected, res);
        }

        [Fact]
        public void While_Visitor()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var res = CSharpExpression.While(test, body);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitWhile(WhileCSharpStatement node)
            {
                Visited = true;

                return base.VisitWhile(node);
            }
        }
    }
}
