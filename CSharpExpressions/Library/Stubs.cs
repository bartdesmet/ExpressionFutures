// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Linq;
using System.Reflection;

// NOTE: Code generated in this file is not product code; it unblocks using existing APIs that are not
//       visible to the prototype.

namespace System.Dynamic.Utils
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    static class ContractUtils
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Dynamic.Utils.ContractUtils");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["Requires"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Boolean) }));

        public static void Requires(System.Boolean precondition)
        {
            try
            {
                s_0.Invoke(null, new object[] { precondition });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_1 = s_mtds["Requires"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Boolean), typeof(System.String) }));

        public static void Requires(System.Boolean precondition, System.String paramName)
        {
            try
            {
                s_1.Invoke(null, new object[] { precondition, paramName });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_2 = s_mtds["RequiresNotNull"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.String) }));

        public static void RequiresNotNull(System.Object value, System.String paramName)
        {
            try
            {
                s_2.Invoke(null, new object[] { value, paramName });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_3 = s_mtds["RequiresNotEmpty"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static void RequiresNotEmpty<T>(System.Collections.Generic.ICollection<T> collection, System.String paramName)
        {
            try
            {
                s_3.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { collection, paramName });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_4 = s_mtds["RequiresArrayRange"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static void RequiresArrayRange<T>(System.Collections.Generic.IList<T> array, System.Int32 offset, System.Int32 count, System.String offsetName, System.String countName)
        {
            try
            {
                s_4.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { array, offset, count, offsetName, countName });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_5 = s_mtds["RequiresNotNullItems"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static void RequiresNotNullItems<T>(System.Collections.Generic.IList<T> array, System.String arrayName)
        {
            try
            {
                s_5.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { array, arrayName });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly PropertyInfo s_6 = s_typ.GetProperty("Unreachable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        public static System.Exception Unreachable
        {
            get
            {
                try
                {
                    return (System.Exception)s_6.GetValue(null);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
        }
    }
}
namespace System.Dynamic.Utils
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    static class TypeExtensions
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Dynamic.Utils.TypeExtensions");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["CreateDelegate"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.MethodInfo), typeof(System.Type), typeof(System.Object) }));

        public static System.Delegate CreateDelegate(this System.Reflection.MethodInfo methodInfo, System.Type delegateType, System.Object target)
        {
            try
            {
                return (System.Delegate)s_0.Invoke(null, new object[] { methodInfo, delegateType, target });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_1 = s_mtds["GetReturnType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.MethodBase) }));

        public static System.Type GetReturnType(this System.Reflection.MethodBase mi)
        {
            try
            {
                return (System.Type)s_1.Invoke(null, new object[] { mi });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_2 = s_mtds["GetParametersCached"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.MethodBase) }));

        public static System.Reflection.ParameterInfo[] GetParametersCached(this System.Reflection.MethodBase method)
        {
            try
            {
                return (System.Reflection.ParameterInfo[])s_2.Invoke(null, new object[] { method });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_3 = s_mtds["IsByRefParameter"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.ParameterInfo) }));

        public static System.Boolean IsByRefParameter(this System.Reflection.ParameterInfo pi)
        {
            try
            {
                return (System.Boolean)s_3.Invoke(null, new object[] { pi });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_4 = s_mtds["GetMethodValidated"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.String), typeof(System.Reflection.BindingFlags), typeof(System.Reflection.Binder), typeof(System.Type[]), typeof(System.Reflection.ParameterModifier[]) }));

        public static System.Reflection.MethodInfo GetMethodValidated(this System.Type type, System.String name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Type[] types, System.Reflection.ParameterModifier[] modifiers)
        {
            try
            {
                return (System.Reflection.MethodInfo)s_4.Invoke(null, new object[] { type, name, bindingAttr, binder, types, modifiers });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_5 = s_mtds["MatchesArgumentTypes"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.MethodInfo), typeof(System.Type[]) }));

        public static System.Boolean MatchesArgumentTypes(this System.Reflection.MethodInfo mi, System.Type[] argTypes)
        {
            try
            {
                return (System.Boolean)s_5.Invoke(null, new object[] { mi, argTypes });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
namespace System.Dynamic.Utils
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    static class CollectionExtensions
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Dynamic.Utils.CollectionExtensions");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["ToReadOnly"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static System.Collections.ObjectModel.ReadOnlyCollection<T> ToReadOnly<T>(this System.Collections.Generic.IEnumerable<T> enumerable)
        {
            try
            {
                return (System.Collections.ObjectModel.ReadOnlyCollection<T>)s_0.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { enumerable });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_1 = s_mtds["ListHashCode"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static System.Int32 ListHashCode<T>(this System.Collections.Generic.IEnumerable<T> list)
        {
            try
            {
                return (System.Int32)s_1.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { list });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_2 = s_mtds["ListEquals"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static System.Boolean ListEquals<T>(this System.Collections.Generic.ICollection<T> first, System.Collections.Generic.ICollection<T> second)
        {
            try
            {
                return (System.Boolean)s_2.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { first, second });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        // Omitted LINQ method System.Collections.Generic.IEnumerable`1[U] Select[T,U](System.Collections.Generic.IEnumerable`1[T], System.Func`2[T,U])

        private static readonly MethodInfo s_4 = s_mtds["Map"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 2);

        public static U[] Map<T, U>(this System.Collections.Generic.ICollection<T> collection, System.Func<T, U> select)
        {
            try
            {
                return (U[])s_4.MakeGenericMethod(typeof(T), typeof(U)).Invoke(null, new object[] { collection, select });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        // Omitted LINQ method System.Collections.Generic.IEnumerable`1[T] Where[T](System.Collections.Generic.IEnumerable`1[T], System.Func`2[T,System.Boolean])

        // Omitted LINQ method Boolean Any[T](System.Collections.Generic.IEnumerable`1[T], System.Func`2[T,System.Boolean])

        // Omitted LINQ method Boolean All[T](System.Collections.Generic.IEnumerable`1[T], System.Func`2[T,System.Boolean])

        private static readonly MethodInfo s_8 = s_mtds["RemoveFirst"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static T[] RemoveFirst<T>(this T[] array)
        {
            try
            {
                return (T[])s_8.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { array });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_9 = s_mtds["RemoveLast"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static T[] RemoveLast<T>(this T[] array)
        {
            try
            {
                return (T[])s_9.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { array });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_10 = s_mtds["AddFirst"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static T[] AddFirst<T>(this System.Collections.Generic.IList<T> list, T item)
        {
            try
            {
                return (T[])s_10.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { list, item });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_11 = s_mtds["AddLast"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static T[] AddLast<T>(this System.Collections.Generic.IList<T> list, T item)
        {
            try
            {
                return (T[])s_11.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { list, item });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        // Omitted LINQ method T First[T](System.Collections.Generic.IEnumerable`1[T])

        private static readonly MethodInfo s_13 = s_mtds["Last"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static T Last<T>(this System.Collections.Generic.IList<T> list)
        {
            try
            {
                return (T)s_13.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { list });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_14 = s_mtds["Copy"].Single(m => m.IsStatic && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static T[] Copy<T>(this T[] array)
        {
            try
            {
                return (T[])s_14.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { array });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
