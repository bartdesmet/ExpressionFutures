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
            var mtd = MethodInfoOf(() => Math.Min(default(int), default(int)));

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
            var mtd = MethodInfoOf(() => Math.Min(default(int), default(int)));

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
            var mtd = MethodInfoOf(() => F(default(int), default(int), default(int)));

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
            var mtd = MethodInfoOf(() => F(default(int), default(int), default(int)));

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
            var mtd = MethodInfoOf(() => F(default(int), default(int), default(int)));

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
            var mtd = MethodInfoOf(() => f.Invoke(default(int), default(int)));

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
            var mtd = MethodInfoOf(() => f.Invoke(default(int), default(int)));

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

        internal static MethodInfo MethodInfoOf<R>(Expression<Func<R>> f)
        {
            return (MethodInfo)InfoOf(f);
        }

        internal static MethodInfo MethodInfoOf(Expression<Action> f)
        {
            return (MethodInfo)InfoOf(f);
        }

        internal static MethodInfo MethodInfoOf<T, R>(Expression<Func<T, R>> f)
        {
            return (MethodInfo)InfoOf(f);
        }

        internal static MethodInfo MethodInfoOf<T>(Expression<Action<T>> f)
        {
            return (MethodInfo)InfoOf(f);
        }

        internal static ConstructorInfo ConstructorInfoOf<R>(Expression<Func<R>> f)
        {
            return (ConstructorInfo)InfoOf(f);
        }

        internal static ConstructorInfo ConstructorInfoOf(Expression<Action> f)
        {
            return (ConstructorInfo)InfoOf(f);
        }

        internal static ConstructorInfo ConstructorInfoOf<T, R>(Expression<Func<T, R>> f)
        {
            return (ConstructorInfo)InfoOf(f);
        }

        internal static ConstructorInfo ConstructorInfoOf<T>(Expression<Action<T>> f)
        {
            return (ConstructorInfo)InfoOf(f);
        }

        internal static PropertyInfo PropertyInfoOf<R>(Expression<Func<R>> f)
        {
            return (PropertyInfo)InfoOf(f);
        }

        internal static PropertyInfo PropertyInfoOf(Expression<Action> f)
        {
            return (PropertyInfo)InfoOf(f);
        }

        internal static PropertyInfo PropertyInfoOf<T, R>(Expression<Func<T, R>> f)
        {
            return (PropertyInfo)InfoOf(f);
        }

        internal static PropertyInfo PropertyInfoOf<T>(Expression<Action<T>> f)
        {
            return (PropertyInfo)InfoOf(f);
        }

        internal static FieldInfo FieldInfoOf<R>(Expression<Func<R>> f)
        {
            return (FieldInfo)InfoOf(f);
        }

        internal static FieldInfo FieldInfoOf(Expression<Action> f)
        {
            return (FieldInfo)InfoOf(f);
        }

        internal static FieldInfo FieldInfoOf<T, R>(Expression<Func<T, R>> f)
        {
            return (FieldInfo)InfoOf(f);
        }

        internal static FieldInfo FieldInfoOf<T>(Expression<Action<T>> f)
        {
            return (FieldInfo)InfoOf(f);
        }

        internal static MemberInfo InfoOf<R>(Expression<Func<R>> f)
        {
            return InfoOf((LambdaExpression)f);
        }

        internal static MemberInfo InfoOf(Expression<Action> f)
        {
            return InfoOf((LambdaExpression)f);
        }

        internal static MemberInfo InfoOf<T, R>(Expression<Func<T, R>> f)
        {
            return InfoOf((LambdaExpression)f);
        }

        internal static MemberInfo InfoOf<T>(Expression<Action<T>> f)
        {
            return InfoOf((LambdaExpression)f);
        }

        private static MemberInfo InfoOf(LambdaExpression f)
        {
            var body = f.Body;

            var mce = default(MethodCallExpression);
            var be = default(BinaryExpression);
            var ue = default(UnaryExpression);
            var ne = default(NewExpression);
            var me = default(MemberExpression);
            var ie = default(IndexExpression);
            if ((mce = body as MethodCallExpression) != null)
            {
                return mce.Method;
            }
            else if ((be = body as BinaryExpression) != null)
            {
                return be.Method;
            }
            else if ((ue = body as UnaryExpression) != null)
            {
                return ue.Method;
            }
            else if ((ne = body as NewExpression) != null)
            {
                return ne.Constructor;
            }
            else if ((me = body as MemberExpression) != null)
            {
                return me.Member;
            }
            else if ((ie = body as IndexExpression) != null)
            {
                return ie.Indexer;
            }

            return null;
        }
    }
}