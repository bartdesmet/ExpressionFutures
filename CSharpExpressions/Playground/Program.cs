// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.CSharp.Expressions
{
    class Program
    {
        private static readonly MethodInfo s_writeLine = typeof(Console).GetMethod("WriteLine", new[] { typeof(string) });

        static void Main()
        {
            Call();
            Invoke();
        }

        static void Call()
        {
            Call1();
            Call2();
            Call3();
            Call4();
            Call5();
        }

        static void Call1()
        {
            var mtd = InfoOf(() => Math.Min(default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));

            var call = CSharpExpression.Call(mtd, arg1, arg0);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static void Call2()
        {
            var mtd = InfoOf(() => Math.Min(default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));

            var call = CSharpExpression.Call(mtd, arg0, arg1);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static void Call3()
        {
            var mtd = InfoOf(() => F(default(int), default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));

            var call = CSharpExpression.Call(mtd, arg1, arg0);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static void Call4()
        {
            var mtd = InfoOf(() => F(default(int), default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));

            var call = CSharpExpression.Call(mtd, arg0, arg1);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static void Call5()
        {
            var mtd = InfoOf(() => F(default(int), default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];
            var val3 = mtd.GetParameters()[2];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));
            var arg2 = CSharpExpression.Bind(val3, Log(Expression.Constant(3), "C"));

            var call = CSharpExpression.Call(mtd, arg2, arg0, arg1);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static void Invoke()
        {
            Invoke1();
            Invoke2();
        }

        static void Invoke1()
        {
            var f = new Func<int, int, int>((a, b) => a + b);
            var mtd = InfoOf(() => f.Invoke(default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));

            var call = CSharpExpression.Invoke(Expression.Constant(f), arg1, arg0);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static void Invoke2()
        {
            var f = new Func<int, int, int>((a, b) => a + b);
            var mtd = InfoOf(() => f.Invoke(default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));

            var call = CSharpExpression.Invoke(Expression.Constant(f), arg0, arg1);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static int F(int x, int y, int z = 42)
        {
            return x * y - z;
        }

        static Expression Log(Expression expression, string log)
        {
            return Expression.Block(Expression.Call(s_writeLine, Expression.Constant(log, typeof(string))), expression);
        }

        internal static MethodInfo InfoOf<R>(Expression<Func<R>> f)
        {
            return ((MethodCallExpression)f.Body).Method;
        }

        internal static MethodInfo InfoOf(Expression<Action> f)
        {
            return ((MethodCallExpression)f.Body).Method;
        }

        internal static MethodInfo InfoOf<T, R>(Expression<Func<T, R>> f)
        {
            return ((MethodCallExpression)f.Body).Method;
        }

        internal static MethodInfo InfoOf<T>(Expression<Action<T>> f)
        {
            return ((MethodCallExpression)f.Body).Method;
        }
    }
}