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
    public class DoTests
    {
        [Fact]
        public void Do_Factory_ArgumentChecking()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();

            // null
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Do(default(Expression), test));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Do(body, default(Expression)));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Do(default(Expression), test, breakLabel));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Do(body, default(Expression), breakLabel));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Do(default(Expression), test, breakLabel, continueLabel));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Do(body, default(Expression), breakLabel, continueLabel));

            // non-bool
            Assert.Throws<ArgumentException>(() => CSharpExpression.Do(body, Expression.Default(typeof(int))));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Do(body, Expression.Default(typeof(int)), breakLabel));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Do(body, Expression.Default(typeof(int)), breakLabel, continueLabel));

            // labels must be void
            Assert.Throws<ArgumentException>(() => CSharpExpression.Do(body, test, Expression.Label(typeof(int))));
            Assert.Throws<ArgumentException>(() => CSharpExpression.Do(body, test, breakLabel, Expression.Label(typeof(int))));
        }

        [Fact]
        public void Do_Properties()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();

            {
                var res = CSharpExpression.Do(body, test);

                Assert.Equal(CSharpExpressionType.Do, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.Same(test, res.Test);
                Assert.Same(body, res.Body);
                Assert.Null(res.BreakLabel);
                Assert.Null(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.Do(body, test, breakLabel);

                Assert.Equal(CSharpExpressionType.Do, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.Same(test, res.Test);
                Assert.Same(body, res.Body);
                Assert.Same(breakLabel, res.BreakLabel);
                Assert.Null(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.Do(body, test, breakLabel, continueLabel);

                Assert.Equal(CSharpExpressionType.Do, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.Same(test, res.Test);
                Assert.Same(body, res.Body);
                Assert.Same(breakLabel, res.BreakLabel);
                Assert.Same(continueLabel, res.ContinueLabel);
            }
        }

        [Fact]
        public void Do_Update()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();
            var res = CSharpExpression.Do(body, test, breakLabel, continueLabel);

            Assert.Same(res, res.Update(res.BreakLabel, res.ContinueLabel, res.Body, res.Test, res.Locals));

            var newTest = Expression.Constant(true);
            var newBody = Expression.Empty();
            var newBreakLabel = Expression.Label();
            var newContinueLabel = Expression.Label();

            var upd1 = res.Update(newBreakLabel, res.ContinueLabel, res.Body, res.Test, res.Locals);
            var upd2 = res.Update(res.BreakLabel, newContinueLabel, res.Body, res.Test, res.Locals);
            var upd3 = res.Update(res.BreakLabel, res.ContinueLabel, newBody, res.Test, res.Locals);
            var upd4 = res.Update(res.BreakLabel, res.ContinueLabel, res.Body, newTest, res.Locals);

            Assert.Same(newBreakLabel, upd1.BreakLabel);
            Assert.Same(res.ContinueLabel, upd1.ContinueLabel);
            Assert.Same(res.Body, upd1.Body);
            Assert.Same(res.Test, upd1.Test);
            Assert.Same(res.Locals, upd1.Locals);

            Assert.Same(res.BreakLabel, upd2.BreakLabel);
            Assert.Same(newContinueLabel, upd2.ContinueLabel);
            Assert.Same(res.Body, upd2.Body);
            Assert.Same(res.Test, upd2.Test);
            Assert.Same(res.Locals, upd2.Locals);

            Assert.Same(res.BreakLabel, upd3.BreakLabel);
            Assert.Same(res.ContinueLabel, upd3.ContinueLabel);
            Assert.Same(newBody, upd3.Body);
            Assert.Same(res.Test, upd3.Test);
            Assert.Same(res.Locals, upd3.Locals);

            Assert.Same(res.BreakLabel, upd4.BreakLabel);
            Assert.Same(res.ContinueLabel, upd4.ContinueLabel);
            Assert.Same(res.Body, upd4.Body);
            Assert.Same(newTest, upd4.Test);
            Assert.Same(res.Locals, upd4.Locals);
        }

        [Fact]
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

        [Fact]
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
            Assert.Equal(expected, res);
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.Equal(expected, res);
        }

        [Fact]
        public void Do_Visitor()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var res = CSharpExpression.Do(body, test);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
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
