﻿// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public partial class AsyncLambdaTests
    {
        [Fact]
        public void Lambda_Factory()
        {
            var p = Expression.Parameter(typeof(int));

            var sync = CSharpExpression.Lambda<Func<int, int>>(false, p, p);
            Assert.Equal(42, sync.Compile()(42));

            var async = CSharpExpression.Lambda<Func<int, Task<int>>>(true, p, p);
            Assert.Equal(42, async.Compile()(42).Result);
        }

        [Fact]
        public void AsyncLambda_Factory_InferDelegateType()
        {
            var e1 = CSharpExpression.AsyncLambda(Expression.Empty());
            Assert.Equal(typeof(Func<Task>), e1.Type);
            Assert.True(e1 is AsyncCSharpExpression<Func<Task>>);

            var e2 = CSharpExpression.AsyncLambda(Expression.Default(typeof(int)));
            Assert.Equal(typeof(Func<Task<int>>), e2.Type);
            Assert.True(e2 is AsyncCSharpExpression<Func<Task<int>>>);

            var p = Expression.Parameter(typeof(string));

            var e3 = CSharpExpression.AsyncLambda(Expression.Empty(), p);
            Assert.Equal(typeof(Func<string, Task>), e3.Type);
            Assert.True(e3 is AsyncCSharpExpression<Func<string, Task>>);

            var e4 = CSharpExpression.AsyncLambda(Expression.Default(typeof(int)), p);
            Assert.Equal(typeof(Func<string, Task<int>>), e4.Type);
            Assert.True(e4 is AsyncCSharpExpression<Func<string, Task<int>>>);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void AsyncLambda_Factory_ArgumentChecking_MustHaveDelegateType()
        {
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<int>(Expression.Empty()));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<MulticastDelegate>(Expression.Empty()));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Delegate>(Expression.Empty()));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(typeof(int), Expression.Empty()));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(typeof(MulticastDelegate), Expression.Empty()));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda(typeof(Delegate), Expression.Empty()));
        }

        [Fact]
        public void AsyncLambda_Factory_ArgumentChecking_CompatibleSignature()
        {
            var p = Expression.Parameter(typeof(int));

            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Action>(Expression.Empty(), p));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Action<int>>(Expression.Empty()));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Action<string>>(Expression.Empty(), p));
        }

        [Fact]
        public void AsyncLambda_Factory_ArgumentChecking_CompatibleReturnType()
        {
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Empty()));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Func<Task<long>>>(Expression.Constant(1)));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Func<Task<string>>>(Expression.Constant(1)));
        }

        [Fact]
        public void AsyncLambda_Factory_ArgumentChecking_MustHaveAsyncReturnType()
        {
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.AsyncLambda<Func<int>>(Expression.Empty()));
        }

        [Fact]
        public void AsyncLambda_Factory_Covariance()
        {
            var res = CSharpExpression.AsyncLambda<Func<Task<object>>>(Expression.Constant("bar"));
            Assert.Equal("bar", res.Compile()().Result);
        }

        [Fact]
        public void AsyncLambda_Factory_CanQuote()
        {
            var e = Expression.Lambda<Func<int>>(Expression.Constant(42));
            var res = CSharpExpression.AsyncLambda<Func<Task<Expression<Func<int>>>>>(e);
            Assert.Equal(ExpressionType.Quote, res.Body.NodeType);
            Assert.Same(e, ((UnaryExpression)res.Body).Operand);
        }

        [Fact]
        public void AsyncLambda_Properties()
        {
            var body = Expression.Constant(42);
            var parameters = new[] { Expression.Parameter(typeof(int)) };
            var res = CSharpExpression.AsyncLambda(body, parameters);
            Assert.Equal(CSharpExpressionType.AsyncLambda, res.CSharpNodeType);
            Assert.Same(body, res.Body);
            Assert.True(parameters.SequenceEqual(res.Parameters));
            Assert.Equal(typeof(Func<int, Task<int>>), res.Type);
        }

        [Fact]
        public void AsyncLambda_Update()
        {
            var body = Expression.Constant(42);
            var parameters = new[] { Expression.Parameter(typeof(int)) };
            var res = CSharpExpression.AsyncLambda<Func<int, Task<int>>>(body, parameters);

            Assert.Same(res, res.Update(res.Body, res.Parameters));

            var newBody = Expression.Constant(42);
            var newParameters = new[] { Expression.Parameter(typeof(int)) };

            var upd1 = res.Update(newBody, res.Parameters);
            Assert.Same(newBody, upd1.Body);
            Assert.Same(res.Parameters, upd1.Parameters);

            var upd2 = res.Update(res.Body, newParameters);
            Assert.Same(res.Body, upd2.Body);
            Assert.True(newParameters.SequenceEqual(upd2.Parameters));
        }

        [Fact]
        public void AsyncLambda_Visitor()
        {
            var res = CSharpExpression.AsyncLambda(Expression.Empty());

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
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