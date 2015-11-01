// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class AwaitTests
    {
        [TestMethod]
        public void Await_Factory_ArgumentChecking()
        {
            var expr = Expression.Default(typeof(Task<int>));
            var getAwaiter = expr.Type.GetMethod("GetAwaiter");

            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Await(default(Expression)));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Await(default(Expression), getAwaiter));
        }

        [TestMethod]
        public void Await_Factory_AwaitPatternCheck()
        {
            foreach (var t in new[] { typeof(A1), typeof(A2), typeof(A3) })
            {
                var getAwaiter = t.GetMethod("GetAwaiter");
                AssertEx.Throws<ArgumentException>(() => CSharpExpression.Await(Expression.Default(t)));
                AssertEx.Throws<ArgumentException>(() => CSharpExpression.Await(Expression.Default(t), getAwaiter));

                if (getAwaiter.IsGenericMethod)
                {
                    AssertEx.Throws<ArgumentException>(() => CSharpExpression.Await(Expression.Default(t), getAwaiter.MakeGenericMethod(typeof(int))));
                }
            }

            foreach (var getAwaiter in this.GetType().GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Where(m => m.Name.StartsWith("GetAwaiterA4")))
            {
                AssertEx.Throws<ArgumentException>(() => CSharpExpression.Await(Expression.Default(typeof(A4)), getAwaiter));

                if (getAwaiter.IsGenericMethod)
                {
                    AssertEx.Throws<ArgumentException>(() => CSharpExpression.Await(Expression.Default(typeof(A4)), getAwaiter.MakeGenericMethod(typeof(int))));
                }
            }

            foreach (var t in new[] { typeof(A4), typeof(A5), typeof(A6), typeof(A7), typeof(A8), typeof(A9), typeof(A10), typeof(A11), typeof(A12) })
            {
                AssertEx.Throws<ArgumentException>(() => CSharpExpression.Await(Expression.Default(t)));
            }
        }

        [TestMethod]
        public void Await_Properties()
        {
            var e = Expression.Default(typeof(Task<int>));
            var expr = CSharpExpression.Await(e);
            Assert.AreEqual(CSharpExpressionType.Await, expr.CSharpNodeType);
            Assert.AreSame(e, expr.Operand);
            Assert.IsNotNull(expr.GetAwaiterMethod);
        }

        [TestMethod]
        public void Await_Update()
        {
            var e = Expression.Default(typeof(Task<int>));
            var expr = CSharpExpression.Await(e);
            Assert.AreSame(expr, expr.Update(e));

            var f = Expression.Default(typeof(Task<int>));
            var upd = expr.Update(f);
            Assert.AreNotSame(upd, expr);
            Assert.AreSame(f, upd.Operand);
        }

        [TestMethod]
        public void Await_CantReduce()
        {
            var e = Expression.Default(typeof(Task<int>));
            var expr = CSharpExpression.Await(e);

            Assert.IsFalse(expr.CanReduce);
            Assert.AreSame(expr, expr.Reduce());

            var f = Expression.Lambda<Func<int>>(expr);
            AssertEx.Throws<ArgumentException>(() => f.Compile());
        }

        [TestMethod]
        public void Await_Visitor()
        {
            var res = CSharpExpression.Await(Expression.Default(typeof(Task<int>)));

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected override Expression VisitAwait(AwaitCSharpExpression node)
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

        class A12
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

                public unsafe int* GetResult()
                {
                    return null;
                }
            }
        }
    }
}
