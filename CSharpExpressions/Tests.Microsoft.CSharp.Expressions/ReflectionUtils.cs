// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Tests
{
    // TODO: keep at central location

    static class ReflectionUtils
    {
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
            return GetProperty(InfoOf(f));
        }

        internal static PropertyInfo PropertyInfoOf(Expression<Action> f)
        {
            return GetProperty(InfoOf(f));
        }

        internal static PropertyInfo PropertyInfoOf<T, R>(Expression<Func<T, R>> f)
        {
            return GetProperty(InfoOf(f));
        }

        internal static PropertyInfo PropertyInfoOf<T>(Expression<Action<T>> f)
        {
            return GetProperty(InfoOf(f));
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

        private static PropertyInfo GetProperty(MemberInfo member)
        {
            var method = member as MethodInfo;
            if (method != null)
            {
                foreach (var property in member.DeclaringType.GetProperties())
                {
                    if (property.GetGetMethod(true) == method || property.GetSetMethod(true) == method)
                    {
                        return property;
                    }
                }
            }

            return (PropertyInfo)member;
        }
    }
}
