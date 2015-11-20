// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class ForTests
    {
        [TestMethod]
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

        [TestMethod]
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

                Assert.AreEqual(CSharpExpressionType.For, res.CSharpNodeType);
                Assert.AreEqual(typeof(void), res.Type);
                Assert.IsTrue(new[] { i }.SequenceEqual(res.Variables));
                Assert.IsTrue(initializers.SequenceEqual(res.Initializers));
                Assert.AreSame(test, res.Test);
                Assert.IsTrue(iterators.SequenceEqual(res.Iterators));
                Assert.AreSame(body, res.Body);
                Assert.IsNull(res.BreakLabel);
                Assert.IsNull(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.For(initializers, test, iterators, body, breakLabel);

                Assert.AreEqual(CSharpExpressionType.For, res.CSharpNodeType);
                Assert.AreEqual(typeof(void), res.Type);
                Assert.IsTrue(new[] { i }.SequenceEqual(res.Variables));
                Assert.IsTrue(initializers.SequenceEqual(res.Initializers));
                Assert.AreSame(test, res.Test);
                Assert.IsTrue(iterators.SequenceEqual(res.Iterators));
                Assert.AreSame(body, res.Body);
                Assert.AreSame(breakLabel, res.BreakLabel);
                Assert.IsNull(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.For(initializers, test, iterators, body, breakLabel, continueLabel);

                Assert.AreEqual(CSharpExpressionType.For, res.CSharpNodeType);
                Assert.AreEqual(typeof(void), res.Type);
                Assert.IsTrue(new[] { i }.SequenceEqual(res.Variables));
                Assert.IsTrue(initializers.SequenceEqual(res.Initializers));
                Assert.AreSame(test, res.Test);
                Assert.IsTrue(iterators.SequenceEqual(res.Iterators));
                Assert.AreSame(body, res.Body);
                Assert.AreSame(breakLabel, res.BreakLabel);
                Assert.AreSame(continueLabel, res.ContinueLabel);
            }

            {
                var res = CSharpExpression.For(new[] { i }, initializers, test, iterators, body);

                Assert.AreEqual(CSharpExpressionType.For, res.CSharpNodeType);
                Assert.AreEqual(typeof(void), res.Type);
                Assert.IsTrue(new[] { i }.SequenceEqual(res.Variables));
                Assert.IsTrue(initializers.SequenceEqual(res.Initializers));
                Assert.AreSame(test, res.Test);
                Assert.IsTrue(iterators.SequenceEqual(res.Iterators));
                Assert.AreSame(body, res.Body);
                Assert.IsNull(res.BreakLabel);
                Assert.IsNull(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.For(new[] { i }, initializers, test, iterators, body, breakLabel);

                Assert.AreEqual(CSharpExpressionType.For, res.CSharpNodeType);
                Assert.AreEqual(typeof(void), res.Type);
                Assert.IsTrue(new[] { i }.SequenceEqual(res.Variables));
                Assert.IsTrue(initializers.SequenceEqual(res.Initializers));
                Assert.AreSame(test, res.Test);
                Assert.IsTrue(iterators.SequenceEqual(res.Iterators));
                Assert.AreSame(body, res.Body);
                Assert.AreSame(breakLabel, res.BreakLabel);
                Assert.IsNull(res.ContinueLabel);
            }

            {
                var res = CSharpExpression.For(new[] { i }, initializers, test, iterators, body, breakLabel, continueLabel);

                Assert.AreEqual(CSharpExpressionType.For, res.CSharpNodeType);
                Assert.AreEqual(typeof(void), res.Type);
                Assert.IsTrue(new[] { i }.SequenceEqual(res.Variables));
                Assert.IsTrue(initializers.SequenceEqual(res.Initializers));
                Assert.AreSame(test, res.Test);
                Assert.IsTrue(iterators.SequenceEqual(res.Iterators));
                Assert.AreSame(body, res.Body);
                Assert.AreSame(breakLabel, res.BreakLabel);
                Assert.AreSame(continueLabel, res.ContinueLabel);
            }
        }

        [TestMethod]
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

            Assert.AreSame(res, res.Update(res.BreakLabel, res.ContinueLabel, res.Variables, res.Initializers, res.Test, res.Iterators, res.Body));

            var newVariables = new[] { i };
            var newInitializers = new[] { Expression.Assign(i, Expression.Constant(0)) };
            var newIterators = new[] { Expression.PostIncrementAssign(i) };
            var newTest = Expression.Constant(true);
            var newBody = Expression.Empty();
            var newBreakLabel = Expression.Label();
            var newContinueLabel = Expression.Label();

            var upd1 = res.Update(newBreakLabel, res.ContinueLabel, res.Variables, res.Initializers, res.Test, res.Iterators, res.Body);
            var upd2 = res.Update(res.BreakLabel, newContinueLabel, res.Variables, res.Initializers, res.Test, res.Iterators, res.Body);
            var upd3 = res.Update(res.BreakLabel, res.ContinueLabel, newVariables, res.Initializers, res.Test, res.Iterators, res.Body);
            var upd4 = res.Update(res.BreakLabel, res.ContinueLabel, res.Variables, newInitializers, res.Test, res.Iterators, res.Body);
            var upd5 = res.Update(res.BreakLabel, res.ContinueLabel, res.Variables, res.Initializers, newTest, res.Iterators, res.Body);
            var upd6 = res.Update(res.BreakLabel, res.ContinueLabel, res.Variables, res.Initializers, res.Test, newIterators, res.Body);
            var upd7 = res.Update(res.BreakLabel, res.ContinueLabel, res.Variables, res.Initializers, res.Test, res.Iterators, newBody);

            Assert.AreSame(newBreakLabel, upd1.BreakLabel);
            Assert.AreSame(res.ContinueLabel, upd1.ContinueLabel);
            Assert.AreSame(res.Variables, upd1.Variables);
            Assert.AreSame(res.Initializers, upd1.Initializers);
            Assert.AreSame(res.Test, upd1.Test);
            Assert.AreSame(res.Iterators, upd1.Iterators);
            Assert.AreSame(res.Body, upd1.Body);

            Assert.AreSame(res.BreakLabel, upd2.BreakLabel);
            Assert.AreSame(newContinueLabel, upd2.ContinueLabel);
            Assert.AreSame(res.Variables, upd2.Variables);
            Assert.AreSame(res.Initializers, upd2.Initializers);
            Assert.AreSame(res.Test, upd2.Test);
            Assert.AreSame(res.Iterators, upd2.Iterators);
            Assert.AreSame(res.Body, upd2.Body);

            Assert.AreSame(res.BreakLabel, upd3.BreakLabel);
            Assert.AreSame(res.ContinueLabel, upd3.ContinueLabel);
            Assert.IsTrue(newVariables.SequenceEqual(upd3.Variables));
            Assert.AreSame(res.Initializers, upd3.Initializers);
            Assert.AreSame(res.Test, upd3.Test);
            Assert.AreSame(res.Iterators, upd3.Iterators);
            Assert.AreSame(res.Body, upd3.Body);

            Assert.AreSame(res.BreakLabel, upd4.BreakLabel);
            Assert.AreSame(res.ContinueLabel, upd4.ContinueLabel);
            Assert.AreSame(res.Variables, upd4.Variables);
            Assert.IsTrue(newInitializers.SequenceEqual(upd4.Initializers));
            Assert.AreSame(res.Test, upd4.Test);
            Assert.AreSame(res.Iterators, upd4.Iterators);
            Assert.AreSame(res.Body, upd4.Body);

            Assert.AreSame(res.BreakLabel, upd5.BreakLabel);
            Assert.AreSame(res.ContinueLabel, upd5.ContinueLabel);
            Assert.AreSame(res.Variables, upd5.Variables);
            Assert.AreSame(res.Initializers, upd5.Initializers);
            Assert.AreSame(newTest, upd5.Test);
            Assert.AreSame(res.Iterators, upd5.Iterators);
            Assert.AreSame(res.Body, upd5.Body);

            Assert.AreSame(res.BreakLabel, upd6.BreakLabel);
            Assert.AreSame(res.ContinueLabel, upd6.ContinueLabel);
            Assert.AreSame(res.Variables, upd6.Variables);
            Assert.AreSame(res.Initializers, upd6.Initializers);
            Assert.AreSame(res.Test, upd6.Test);
            Assert.IsTrue(newIterators.SequenceEqual(upd6.Iterators));
            Assert.AreSame(res.Body, upd6.Body);

            Assert.AreSame(res.BreakLabel, upd7.BreakLabel);
            Assert.AreSame(res.ContinueLabel, upd7.ContinueLabel);
            Assert.AreSame(res.Variables, upd7.Variables);
            Assert.AreSame(res.Initializers, upd7.Initializers);
            Assert.AreSame(res.Test, upd7.Test);
            Assert.AreSame(res.Iterators, upd7.Iterators);
            Assert.AreSame(newBody, upd7.Body);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
            Assert.AreEqual(expected, res);
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        public void For_Visitor()
        {
            var test = Expression.Constant(true);
            var body = Expression.Empty();
            var res = CSharpExpression.For(null, test, null, body);

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
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
