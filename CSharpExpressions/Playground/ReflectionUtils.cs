// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Playground
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
            switch (f.Body)
            {
                case MethodCallExpression mce:
                    return mce.Method;
                case BinaryExpression be:
                    return be.Method;
                case UnaryExpression ue:
                    return ue.Method;
                case NewExpression ne:
                    return ne.Constructor;
                case MemberExpression me:
                    return me.Member;
                case IndexExpression ie:
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
