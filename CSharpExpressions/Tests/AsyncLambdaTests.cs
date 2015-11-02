// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public partial class AsyncLambdaTests
    {
        [TestMethod]
        public void AsyncLambda_Factory_InferDelegateType()
        {
            var e1 = CSharpExpression.AsyncLambda(Expression.Empty());
            Assert.AreEqual(e1.Type, typeof(Func<Task>));
            Assert.IsInstanceOfType(e1, typeof(AsyncCSharpExpression<Func<Task>>));

            var e2 = CSharpExpression.AsyncLambda(Expression.Default(typeof(int)));
            Assert.AreEqual(e2.Type, typeof(Func<Task<int>>));
            Assert.IsInstanceOfType(e2, typeof(AsyncCSharpExpression<Func<Task<int>>>));

            var p = Expression.Parameter(typeof(string));

            var e3 = CSharpExpression.AsyncLambda(Expression.Empty(), p);
            Assert.AreEqual(e3.Type, typeof(Func<string, Task>));
            Assert.IsInstanceOfType(e3, typeof(AsyncCSharpExpression<Func<string, Task>>));

            var e4 = CSharpExpression.AsyncLambda(Expression.Default(typeof(int)), p);
            Assert.AreEqual(e4.Type, typeof(Func<string, Task<int>>));
            Assert.IsInstanceOfType(e4, typeof(AsyncCSharpExpression<Func<string, Task<int>>>));
        }

        [TestMethod]
        public void AsyncLambda_Factory_ArgumentChecking_NoDuplicateParameters()
        {
            var p = Expression.Parameter(typeof(int));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(Expression.Empty(), p, p));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(Expression.Empty(), new[] { p, p }.AsEnumerable()));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(typeof(Action<int, int>), Expression.Empty(), p, p));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(typeof(Action<int, int>), Expression.Empty(), new[] { p, p }.AsEnumerable()));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Action<int, int>>(Expression.Empty(), p, p));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Action<int, int>>(Expression.Empty(), new[] { p, p }.AsEnumerable()));
        }

        [TestMethod]
        public void AsyncLambda_Factory_ArgumentChecking_NoRefParameters()
        {
            var p = Expression.Parameter(typeof(int).MakeByRefType());

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(Expression.Empty(), p));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(Expression.Empty(), new[] { p }.AsEnumerable()));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(typeof(ByRef), Expression.Empty(), p));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(typeof(ByRef), Expression.Empty(), new[] { p }.AsEnumerable()));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<ByRef>(Expression.Empty(), p));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<ByRef>(Expression.Empty(), new[] { p }.AsEnumerable()));
        }

        [TestMethod]
        public void AsyncLambda_Factory_ArgumentChecking_BodyNotNull()
        {
            var p = Expression.Parameter(typeof(int));

            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.AsyncLambda(default(Expression), p));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.AsyncLambda(default(Expression), new[] { p }.AsEnumerable()));

            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.AsyncLambda(typeof(Action<int>), default(Expression), p));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.AsyncLambda(typeof(Action<int>), default(Expression), new[] { p }.AsEnumerable()));

            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.AsyncLambda<Action<int>>(default(Expression), p));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.AsyncLambda<Action<int>>(default(Expression), new[] { p }.AsEnumerable()));
        }

        [TestMethod]
        public void AsyncLambda_Factory_ArgumentChecking_MustHaveDelegateType()
        {
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<int>(Expression.Empty()));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<MulticastDelegate>(Expression.Empty()));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Delegate>(Expression.Empty()));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(typeof(int), Expression.Empty()));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(typeof(MulticastDelegate), Expression.Empty()));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(typeof(Delegate), Expression.Empty()));
        }

        [TestMethod]
        public void AsyncLambda_Factory_ArgumentChecking_CompatibleSignature()
        {
            var p = Expression.Parameter(typeof(int));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Action>(Expression.Empty(), p));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Action<int>>(Expression.Empty()));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Action<string>>(Expression.Empty(), p));
        }

        [TestMethod]
        public void AsyncLambda_Factory_ArgumentChecking_CompatibleReturnType()
        {
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Empty()));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Func<Task<long>>>(Expression.Constant(1)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Func<Task<string>>>(Expression.Constant(1)));
        }

        [TestMethod]
        public void AsyncLambda_Factory_ArgumentChecking_MustHaveAsyncReturnType()
        {
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Func<int>>(Expression.Empty()));
        }

        [TestMethod]
        public void AsyncLambda_Factory_Covariance()
        {
            var res = CSharpExpression.AsyncLambda<Func<Task<object>>>(Expression.Constant("bar"));
            Assert.AreEqual("bar", res.Compile()().Result);
        }

        [TestMethod]
        public void AsyncLambda_Factory_CanQuote()
        {
            var e = Expression.Lambda<Func<int>>(Expression.Constant(42));
            var res = CSharpExpression.AsyncLambda<Func<Task<Expression<Func<int>>>>>(e);
            Assert.AreEqual(ExpressionType.Quote, res.Body.NodeType);
            Assert.AreSame(e, ((UnaryExpression)res.Body).Operand);
        }

        [TestMethod]
        public void AsyncLambda_Properties()
        {
            var body = Expression.Constant(42);
            var parameters = new[] { Expression.Parameter(typeof(int)) };
            var res = CSharpExpression.AsyncLambda(body, parameters);
            Assert.AreEqual(CSharpExpressionType.AsyncLambda, res.CSharpNodeType);
            Assert.AreSame(body, res.Body);
            Assert.IsTrue(parameters.SequenceEqual(res.Parameters));
            Assert.AreEqual(typeof(Func<int, Task<int>>), res.Type);
        }

        [TestMethod]
        public void AsyncLambda_Update()
        {
            var body = Expression.Constant(42);
            var parameters = new[] { Expression.Parameter(typeof(int)) };
            var res = CSharpExpression.AsyncLambda<Func<int, Task<int>>>(body, parameters);

            Assert.AreSame(res, res.Update(res.Body, res.Parameters));

            var newBody = Expression.Constant(42);
            var newParameters = new[] { Expression.Parameter(typeof(int)) };

            var upd1 = res.Update(newBody, res.Parameters);
            Assert.AreSame(newBody, upd1.Body);
            Assert.AreSame(res.Parameters, upd1.Parameters);

            var upd2 = res.Update(res.Body, newParameters);
            Assert.AreSame(res.Body, upd2.Body);
            Assert.IsTrue(newParameters.SequenceEqual(upd2.Parameters));
        }

        [TestMethod]
        public void AsyncLambda_Visitor()
        {
            var res = CSharpExpression.AsyncLambda(Expression.Empty());

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        delegate void ByRef(ref int x);

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitAsyncLambda<TDelegate>(AsyncCSharpExpression<TDelegate> node)
            {
                Visited = true;

                return base.VisitAsyncLambda(node);
            }
        }
    }
}