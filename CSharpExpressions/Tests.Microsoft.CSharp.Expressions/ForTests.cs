// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using static Tests.TestHelpers;

namespace Tests
{
    public class ForTests
    {
        [Fact]
        public void For_Factory_ArgumentChecking()
        {
            var i = Expression.Parameter(typeof(int));
            var initializers = new[] { Expression.Assign(i, Expression.Constant(0)) };
            var iterators = new[] { Expression.PostIncrementAssign(i) };
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.For(initializers, test, iterators, default(Expression)));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.For(initializers, test, iterators, default(Expression), breakLabel));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.For(initializers, test, iterators, default(Expression), breakLabel, continueLabel));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.For(new[] { i }, initializers, test, iterators, default(Expression)));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.For(new[] { i }, initializers, test, iterators, default(Expression), breakLabel));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.For(new[] { i }, initializers, test, iterators, default(Expression), breakLabel, continueLabel));

            // non-bool
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(initializers, Expression.Default(typeof(int)), iterators, body));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(initializers, Expression.Default(typeof(int)), iterators, body, breakLabel));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(initializers, Expression.Default(typeof(int)), iterators, body, breakLabel, continueLabel));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(new[] { i }, initializers, Expression.Default(typeof(int)), iterators, body));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(new[] { i }, initializers, Expression.Default(typeof(int)), iterators, body, breakLabel));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(new[] { i }, initializers, Expression.Default(typeof(int)), iterators, body, breakLabel, continueLabel));

            // labels must be void
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(initializers, test, iterators, body, Expression.Label(typeof(int))));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(initializers, test, iterators, body, breakLabel, Expression.Label(typeof(int))));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(new[] { i }, initializers, test, iterators, body, Expression.Label(typeof(int))));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(new[] { i }, initializers, test, iterators, body, breakLabel, Expression.Label(typeof(int))));

            // duplicate variable
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(initializers.Concat(initializers), test, iterators, body));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(new[] { i, i }, initializers, test, iterators, body, breakLabel, continueLabel));

            // invalid initializer
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(new[] { Expression.Add(i, i) }, test, iterators, body));

            // duplicate labels
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(initializers, test, iterators, body, breakLabel, breakLabel));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.For(new[] { i }, initializers, test, iterators, body, breakLabel, breakLabel));
        }

        [Fact]
        public void For_Properties()
        {
            var i = Expression.Parameter(typeof(int));
            var initializers = new[] { Expression.Assign(i, Expression.Constant(0)) };
            var iterators = new[] { Expression.PostIncrementAssign(i) };
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();

            {
                var res = CSharpExpression.For(initializers, test, iterators, body);

                Assert.Equal(CSharpExpressionType.For, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.True(new[] { i }.SequenceEqual(res.Variables));
                Assert.True(initializers.SequenceEqual(res.Initializers));
                Assert.Same(test, res.Test);
                Assert.True(iterators.SequenceEqual(res.Iterators));
                Assert.Same(body, res.Body);
                Assert.Null(res.BreakLabel);
                Assert.Null(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.For(initializers, test, iterators, body, breakLabel);

                Assert.Equal(CSharpExpressionType.For, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.True(new[] { i }.SequenceEqual(res.Variables));
                Assert.True(initializers.SequenceEqual(res.Initializers));
                Assert.Same(test, res.Test);
                Assert.True(iterators.SequenceEqual(res.Iterators));
                Assert.Same(body, res.Body);
                Assert.Same(breakLabel, res.BreakLabel);
                Assert.Null(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.For(initializers, test, iterators, body, breakLabel, continueLabel);

                Assert.Equal(CSharpExpressionType.For, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.True(new[] { i }.SequenceEqual(res.Variables));
                Assert.True(initializers.SequenceEqual(res.Initializers));
                Assert.Same(test, res.Test);
                Assert.True(iterators.SequenceEqual(res.Iterators));
                Assert.Same(body, res.Body);
                Assert.Same(breakLabel, res.BreakLabel);
                Assert.Same(continueLabel, res.ContinueLabel);
            }

            {
                var res = CSharpExpression.For(new[] { i }, initializers, test, iterators, body);

                Assert.Equal(CSharpExpressionType.For, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.True(new[] { i }.SequenceEqual(res.Variables));
                Assert.True(initializers.SequenceEqual(res.Initializers));
                Assert.Same(test, res.Test);
                Assert.True(iterators.SequenceEqual(res.Iterators));
                Assert.Same(body, res.Body);
                Assert.Null(res.BreakLabel);
                Assert.Null(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.For(new[] { i }, initializers, test, iterators, body, breakLabel);

                Assert.Equal(CSharpExpressionType.For, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.True(new[] { i }.SequenceEqual(res.Variables));
                Assert.True(initializers.SequenceEqual(res.Initializers));
                Assert.Same(test, res.Test);
                Assert.True(iterators.SequenceEqual(res.Iterators));
                Assert.Same(body, res.Body);
                Assert.Same(breakLabel, res.BreakLabel);
                Assert.Null(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.For(new[] { i }, initializers, test, iterators, body, breakLabel, continueLabel);

                Assert.Equal(CSharpExpressionType.For, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.True(new[] { i }.SequenceEqual(res.Variables));
                Assert.True(initializers.SequenceEqual(res.Initializers));
                Assert.Same(test, res.Test);
                Assert.True(iterators.SequenceEqual(res.Iterators));
                Assert.Same(body, res.Body);
                Assert.Same(breakLabel, res.BreakLabel);
                Assert.Same(continueLabel, res.ContinueLabel);
            }
        }

        [Fact]
        public void For_Update()
        {
            var i = Expression.Parameter(typeof(int));
            var variables = new[] { i };
            var initializers = new[] { Expression.Assign(i, Expression.Constant(0)) };
            var iterators = new[] { Expression.PostIncrementAssign(i) };
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();
            var res = CSharpExpression.For(variables, initializers, test, iterators, body, breakLabel, continueLabel);

            Assert.Same(res, res.Update(res.BreakLabel, res.ContinueLabel, res.Variables, res.Initializers, res.Test, res.Iterators, res.Body, res.Locals));

            var newVariables = new[] { i };
            var newInitializers = new[] { Expression.Assign(i, Expression.Constant(0)) };
            var newIterators = new[] { Expression.PostIncrementAssign(i) };
            var newTest = Expression.Constant(true);
            var newBody = Expression.Empty();
            var newBreakLabel = Expression.Label();
            var newContinueLabel = Expression.Label();

            var upd1 = res.Update(newBreakLabel, res.ContinueLabel, res.Variables, res.Initializers, res.Test, res.Iterators, res.Body, res.Locals);
            var upd2 = res.Update(res.BreakLabel, newContinueLabel, res.Variables, res.Initializers, res.Test, res.Iterators, res.Body, res.Locals);
            var upd3 = res.Update(res.BreakLabel, res.ContinueLabel, newVariables, res.Initializers, res.Test, res.Iterators, res.Body, res.Locals);
            var upd4 = res.Update(res.BreakLabel, res.ContinueLabel, res.Variables, newInitializers, res.Test, res.Iterators, res.Body, res.Locals);
            var upd5 = res.Update(res.BreakLabel, res.ContinueLabel, res.Variables, res.Initializers, newTest, res.Iterators, res.Body, res.Locals);
            var upd6 = res.Update(res.BreakLabel, res.ContinueLabel, res.Variables, res.Initializers, res.Test, newIterators, res.Body, res.Locals);
            var upd7 = res.Update(res.BreakLabel, res.ContinueLabel, res.Variables, res.Initializers, res.Test, res.Iterators, newBody, res.Locals);

            Assert.Same(newBreakLabel, upd1.BreakLabel);
            Assert.Same(res.ContinueLabel, upd1.ContinueLabel);
            Assert.Same(res.Variables, upd1.Variables);
            Assert.Same(res.Initializers, upd1.Initializers);
            Assert.Same(res.Test, upd1.Test);
            Assert.Same(res.Iterators, upd1.Iterators);
            Assert.Same(res.Body, upd1.Body);
            Assert.Same(res.Locals, upd1.Locals);

            Assert.Same(res.BreakLabel, upd2.BreakLabel);
            Assert.Same(newContinueLabel, upd2.ContinueLabel);
            Assert.Same(res.Variables, upd2.Variables);
            Assert.Same(res.Initializers, upd2.Initializers);
            Assert.Same(res.Test, upd2.Test);
            Assert.Same(res.Iterators, upd2.Iterators);
            Assert.Same(res.Body, upd2.Body);
            Assert.Same(res.Locals, upd2.Locals);

            Assert.Same(res.BreakLabel, upd3.BreakLabel);
            Assert.Same(res.ContinueLabel, upd3.ContinueLabel);
            Assert.True(newVariables.SequenceEqual(upd3.Variables));
            Assert.Same(res.Initializers, upd3.Initializers);
            Assert.Same(res.Test, upd3.Test);
            Assert.Same(res.Iterators, upd3.Iterators);
            Assert.Same(res.Body, upd3.Body);
            Assert.Same(res.Locals, upd3.Locals);

            Assert.Same(res.BreakLabel, upd4.BreakLabel);
            Assert.Same(res.ContinueLabel, upd4.ContinueLabel);
            Assert.Same(res.Variables, upd4.Variables);
            Assert.True(newInitializers.SequenceEqual(upd4.Initializers));
            Assert.Same(res.Test, upd4.Test);
            Assert.Same(res.Iterators, upd4.Iterators);
            Assert.Same(res.Body, upd4.Body);
            Assert.Same(res.Locals, upd4.Locals);

            Assert.Same(res.BreakLabel, upd5.BreakLabel);
            Assert.Same(res.ContinueLabel, upd5.ContinueLabel);
            Assert.Same(res.Variables, upd5.Variables);
            Assert.Same(res.Initializers, upd5.Initializers);
            Assert.Same(newTest, upd5.Test);
            Assert.Same(res.Iterators, upd5.Iterators);
            Assert.Same(res.Body, upd5.Body);
            Assert.Same(res.Locals, upd5.Locals);

            Assert.Same(res.BreakLabel, upd6.BreakLabel);
            Assert.Same(res.ContinueLabel, upd6.ContinueLabel);
            Assert.Same(res.Variables, upd6.Variables);
            Assert.Same(res.Initializers, upd6.Initializers);
            Assert.Same(res.Test, upd6.Test);
            Assert.True(newIterators.SequenceEqual(upd6.Iterators));
            Assert.Same(res.Body, upd6.Body);
            Assert.Same(res.Locals, upd6.Locals);

            Assert.Same(res.BreakLabel, upd7.BreakLabel);
            Assert.Same(res.ContinueLabel, upd7.ContinueLabel);
            Assert.Same(res.Variables, upd7.Variables);
            Assert.Same(res.Initializers, upd7.Initializers);
            Assert.Same(res.Test, upd7.Test);
            Assert.Same(res.Iterators, upd7.Iterators);
            Assert.Same(newBody, upd7.Body);
            Assert.Same(res.Locals, upd7.Locals);
        }

        [Fact]
        public void For_Compile()
        {
            var i = Expression.Parameter(typeof(int));

            AssertCompile(log =>
                CSharpExpression.For(
                    new[] { Expression.Assign(i, Expression.Constant(0)) },
                    Expression.NotEqual(i, Expression.Constant(0)),
                    null,
                    log("B")
                ),
                new LogAndResult<object> { Log = { } }
            );

            AssertCompile(log =>
                CSharpExpression.For(
                    new[] { Expression.Assign(i, Expression.Constant(0)) },
                    Expression.LessThan(i, Expression.Constant(3)),
                    new[] { Expression.PostIncrementAssign(i) },
                    log("B")
                ),
                new LogAndResult<object> { Log = { "B", "B", "B" } }
            );
        }

        [Fact]
        public void For_Compile_BreakContinue()
        {
            var i = Expression.Parameter(typeof(int));
            var brk = Expression.Label();
            var cnt = Expression.Label();

            AssertCompile((log, append) =>
                CSharpExpression.For(
                    new[] { Expression.Assign(i, Expression.Constant(0)) },
                    Expression.LessThan(i, Expression.Constant(10)),
                    new[] { Expression.PostIncrementAssign(i) },
                    Expression.Block(
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
                ),
                new LogAndResult<object> { Log = { "1", "3", "5" } }
            );
        }

        [Fact]
        public void For_Compile_Infinite()
        {
            var i = Expression.Parameter(typeof(int));
            var brk = Expression.Label();
            var cnt = Expression.Label();

            AssertCompile((log, append) =>
                CSharpExpression.For(
                    new[] { Expression.Assign(i, Expression.Constant(0)) },
                    null,
                    new[] { Expression.PostIncrementAssign(i) },
                    Expression.Block(
                        Expression.IfThen(
                            Expression.GreaterThan(i, Expression.Constant(5)),
                            Expression.Break(brk)
                        ),
                        Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                    ),
                    brk,
                    cnt
                ),
                new LogAndResult<object> { Log = { "0", "1", "2", "3", "4", "5" } }
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
        public void For_Visitor()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var res = CSharpExpression.For(null, test, null, body);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitFor(ForCSharpStatement node)
            {
                Visited = true;

                return base.VisitFor(node);
            }
        }
    }
}
