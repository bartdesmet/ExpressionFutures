// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class AwaitTests
    {
        [Fact]
        public void Await_Factory_ArgumentChecking()
        {
            var expr = Expression.Default(typeof(Task<int>));
            var getAwaiter = expr.Type.GetMethod("GetAwaiter");

            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Await(default(Expression)));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.Await(default(Expression), getAwaiter));
        }

        [Fact]
        public void Await_Factory_AwaitPatternCheck()
        {
            foreach (var t in new[] { typeof(A1), typeof(A2), typeof(A3) })
            {
                var getAwaiter = t.GetMethod("GetAwaiter");
                Assert.Throws<ArgumentException>(() => CSharpExpression.Await(Expression.Default(t)));
                Assert.Throws<ArgumentException>(() => CSharpExpression.Await(Expression.Default(t), getAwaiter));

                if (getAwaiter.IsGenericMethod)
                {
                    Assert.Throws<ArgumentException>(() => CSharpExpression.Await(Expression.Default(t), getAwaiter.MakeGenericMethod(typeof(int))));
                }
            }

            foreach (var getAwaiter in this.GetType().GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Where(m => m.Name.StartsWith("GetAwaiterA4")))
            {
                Assert.Throws<ArgumentException>(() => CSharpExpression.Await(Expression.Default(typeof(A4)), getAwaiter));

                if (getAwaiter.IsGenericMethod)
                {
                    Assert.Throws<ArgumentException>(() => CSharpExpression.Await(Expression.Default(typeof(A4)), getAwaiter.MakeGenericMethod(typeof(int))));
                }
            }

            foreach (var t in new[] { typeof(A4), typeof(A5), typeof(A6), typeof(A7), typeof(A8), typeof(A9), typeof(A10), typeof(A11) })
            {
                Assert.ThrowsAny<ArgumentException>(() => CSharpExpression.Await(Expression.Default(t)));
            }
        }

        [Fact]
        public void Await_Properties()
        {
            var e = Expression.Default(typeof(Task<int>));
            var expr = CSharpExpression.Await(e);
            Assert.Equal(CSharpExpressionType.Await, expr.CSharpNodeType);
            Assert.Same(e, expr.Operand);
            Assert.NotNull(expr.Info);
        }

        [Fact]
        public void Await_Update()
        {
            var e = Expression.Default(typeof(Task<int>));
            var expr = CSharpExpression.Await(e);
            Assert.Same(expr, expr.Update(e, expr.Info));

            var f = Expression.Default(typeof(Task<int>));
            var upd = expr.Update(f, expr.Info);
            Assert.NotSame(upd, expr);
            Assert.Same(f, upd.Operand);
        }

        [Fact]
        public void Await_CantReduce()
        {
            var e = Expression.Default(typeof(Task<int>));
            var expr = CSharpExpression.Await(e);

            Assert.False(expr.CanReduce);
            Assert.Same(expr, expr.Reduce());

            var f = Expression.Lambda<Func<int>>(expr);
            Assert.Throws<ArgumentException>(() => f.Compile());
        }

        [Fact]
        public void Await_Visitor()
        {
            var res = CSharpExpression.Await(Expression.Default(typeof(Task<int>)));

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitAwait(AwaitCSharpExpression node)
            {
                Visited = true;

                return base.VisitAwait(node);
            }
        }

        class A1
        {
            public void GetAwaiter() { }
        }

        class A2
        {
            public void GetAwaiter(int x) { }
        }

        class A3
        {
            public void GetAwaiter<T>() { }
        }

        class A4
        {
        }

        static void GetAwaiterA4(Task<int> a)
        {
        }

        static void GetAwaiterA4(A4 a)
        {
        }

        static void GetAwaiterA4(A4 a, int x)
        {
        }

        static void GetAwaiterA4<T>(A4 a)
        {
        }

        class A5
        {
            public A GetAwaiter()
            {
                throw new NotImplementedException();
            }

            public class A
            {
            }
        }

        class A6
        {
            public A GetAwaiter()
            {
                throw new NotImplementedException();
            }

            public class A : INotifyCompletion
            {
                public void OnCompleted(Action continuation)
                {
                }
            }
        }

        class A7
        {
            public A GetAwaiter()
            {
                throw new NotImplementedException();
            }

            public class A : INotifyCompletion
            {
                public void OnCompleted(Action continuation)
                {
                }

                public int IsCompleted { get; }
            }
        }

        class A8
        {
            public A GetAwaiter()
            {
                throw new NotImplementedException();
            }

            public class A : INotifyCompletion
            {
                public void OnCompleted(Action continuation)
                {
                }

                public bool IsCompleted { set { } }
            }
        }

        class A9
        {
            public A GetAwaiter()
            {
                throw new NotImplementedException();
            }

            public class A : INotifyCompletion
            {
                public void OnCompleted(Action continuation)
                {
                }

                [IndexerName("IsCompleted")]
                public bool this[int x]
                {
                    get { throw new NotImplementedException(); }
                }
            }
        }

        class A10
        {
            public A GetAwaiter()
            {
                throw new NotImplementedException();
            }

            public class A : INotifyCompletion
            {
                public void OnCompleted(Action continuation)
                {
                }

                public bool IsCompleted { get; }
            }
        }

        class A11
        {
            public A GetAwaiter()
            {
                throw new NotImplementedException();
            }

            public class A : INotifyCompletion
            {
                public void OnCompleted(Action continuation)
                {
                }

                public bool IsCompleted { get; }

                public void GetResult<T>()
                {
                }
            }
        }
    }
}
