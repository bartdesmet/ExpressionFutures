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
    [System.CodeDom.Compiler.GeneratedCode("Stub generator", "1.0")]
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
    [System.CodeDom.Compiler.GeneratedCode("Stub generator", "1.0")]
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
    [System.CodeDom.Compiler.GeneratedCode("Stub generator", "1.0")]
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
namespace System.Dynamic.Utils
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Stub generator", "1.0")]
    static class TypeUtils
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Dynamic.Utils.TypeUtils");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["GetNonNullableType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Type GetNonNullableType(this System.Type type)
        {
            try
            {
                return (System.Type)s_0.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_1 = s_mtds["GetNullableType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Type GetNullableType(System.Type type)
        {
            try
            {
                return (System.Type)s_1.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_2 = s_mtds["IsNullableType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsNullableType(this System.Type type)
        {
            try
            {
                return (System.Boolean)s_2.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_3 = s_mtds["IsBool"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsBool(System.Type type)
        {
            try
            {
                return (System.Boolean)s_3.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_4 = s_mtds["IsNumeric"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsNumeric(System.Type type)
        {
            try
            {
                return (System.Boolean)s_4.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_5 = s_mtds["IsInteger"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsInteger(System.Type type)
        {
            try
            {
                return (System.Boolean)s_5.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_6 = s_mtds["IsArithmetic"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsArithmetic(System.Type type)
        {
            try
            {
                return (System.Boolean)s_6.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_7 = s_mtds["IsUnsignedInt"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsUnsignedInt(System.Type type)
        {
            try
            {
                return (System.Boolean)s_7.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_8 = s_mtds["IsIntegerOrBool"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsIntegerOrBool(System.Type type)
        {
            try
            {
                return (System.Boolean)s_8.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_9 = s_mtds["AreEquivalent"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean AreEquivalent(System.Type t1, System.Type t2)
        {
            try
            {
                return (System.Boolean)s_9.Invoke(null, new object[] { t1, t2 });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_10 = s_mtds["AreReferenceAssignable"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean AreReferenceAssignable(System.Type dest, System.Type src)
        {
            try
            {
                return (System.Boolean)s_10.Invoke(null, new object[] { dest, src });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_11 = s_mtds["IsValidInstanceType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.MemberInfo), typeof(System.Type) }));

        public static System.Boolean IsValidInstanceType(System.Reflection.MemberInfo member, System.Type instanceType)
        {
            try
            {
                return (System.Boolean)s_11.Invoke(null, new object[] { member, instanceType });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_12 = s_mtds["HasIdentityPrimitiveOrNullableConversion"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean HasIdentityPrimitiveOrNullableConversion(System.Type source, System.Type dest)
        {
            try
            {
                return (System.Boolean)s_12.Invoke(null, new object[] { source, dest });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_13 = s_mtds["HasReferenceConversion"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean HasReferenceConversion(System.Type source, System.Type dest)
        {
            try
            {
                return (System.Boolean)s_13.Invoke(null, new object[] { source, dest });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_14 = s_mtds["IsCovariant"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsCovariant(System.Type t)
        {
            try
            {
                return (System.Boolean)s_14.Invoke(null, new object[] { t });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_15 = s_mtds["IsContravariant"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsContravariant(System.Type t)
        {
            try
            {
                return (System.Boolean)s_15.Invoke(null, new object[] { t });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_16 = s_mtds["IsInvariant"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsInvariant(System.Type t)
        {
            try
            {
                return (System.Boolean)s_16.Invoke(null, new object[] { t });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_17 = s_mtds["IsDelegate"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsDelegate(System.Type t)
        {
            try
            {
                return (System.Boolean)s_17.Invoke(null, new object[] { t });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_18 = s_mtds["IsLegalExplicitVariantDelegateConversion"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean IsLegalExplicitVariantDelegateConversion(System.Type source, System.Type dest)
        {
            try
            {
                return (System.Boolean)s_18.Invoke(null, new object[] { source, dest });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_19 = s_mtds["IsConvertible"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsConvertible(System.Type type)
        {
            try
            {
                return (System.Boolean)s_19.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_20 = s_mtds["HasReferenceEquality"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean HasReferenceEquality(System.Type left, System.Type right)
        {
            try
            {
                return (System.Boolean)s_20.Invoke(null, new object[] { left, right });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_21 = s_mtds["HasBuiltInEqualityOperator"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean HasBuiltInEqualityOperator(System.Type left, System.Type right)
        {
            try
            {
                return (System.Boolean)s_21.Invoke(null, new object[] { left, right });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_22 = s_mtds["IsImplicitlyConvertible"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean IsImplicitlyConvertible(System.Type source, System.Type destination)
        {
            try
            {
                return (System.Boolean)s_22.Invoke(null, new object[] { source, destination });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_23 = s_mtds["GetUserDefinedCoercionMethod"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type), typeof(System.Boolean) }));

        public static System.Reflection.MethodInfo GetUserDefinedCoercionMethod(System.Type convertFrom, System.Type convertToType, System.Boolean implicitOnly)
        {
            try
            {
                return (System.Reflection.MethodInfo)s_23.Invoke(null, new object[] { convertFrom, convertToType, implicitOnly });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_24 = s_mtds["FindConversionOperator"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.MethodInfo[]), typeof(System.Type), typeof(System.Type), typeof(System.Boolean) }));

        public static System.Reflection.MethodInfo FindConversionOperator(System.Reflection.MethodInfo[] methods, System.Type typeFrom, System.Type typeTo, System.Boolean implicitOnly)
        {
            try
            {
                return (System.Reflection.MethodInfo)s_24.Invoke(null, new object[] { methods, typeFrom, typeTo, implicitOnly });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_25 = s_mtds["IsImplicitNumericConversion"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean IsImplicitNumericConversion(System.Type source, System.Type destination)
        {
            try
            {
                return (System.Boolean)s_25.Invoke(null, new object[] { source, destination });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_26 = s_mtds["IsImplicitReferenceConversion"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean IsImplicitReferenceConversion(System.Type source, System.Type destination)
        {
            try
            {
                return (System.Boolean)s_26.Invoke(null, new object[] { source, destination });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_27 = s_mtds["IsImplicitBoxingConversion"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean IsImplicitBoxingConversion(System.Type source, System.Type destination)
        {
            try
            {
                return (System.Boolean)s_27.Invoke(null, new object[] { source, destination });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_28 = s_mtds["IsImplicitNullableConversion"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean IsImplicitNullableConversion(System.Type source, System.Type destination)
        {
            try
            {
                return (System.Boolean)s_28.Invoke(null, new object[] { source, destination });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_29 = s_mtds["IsSameOrSubclass"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean IsSameOrSubclass(System.Type type, System.Type subType)
        {
            try
            {
                return (System.Boolean)s_29.Invoke(null, new object[] { type, subType });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_30 = s_mtds["ValidateType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static void ValidateType(System.Type type)
        {
            try
            {
                s_30.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_31 = s_mtds["FindGenericType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Type FindGenericType(System.Type definition, System.Type type)
        {
            try
            {
                return (System.Type)s_31.Invoke(null, new object[] { definition, type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_32 = s_mtds["IsUnsigned"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsUnsigned(System.Type type)
        {
            try
            {
                return (System.Boolean)s_32.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_33 = s_mtds["IsFloatingPoint"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean IsFloatingPoint(System.Type type)
        {
            try
            {
                return (System.Boolean)s_33.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_34 = s_mtds["GetBooleanOperator"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.String) }));

        public static System.Reflection.MethodInfo GetBooleanOperator(System.Type type, System.String name)
        {
            try
            {
                return (System.Reflection.MethodInfo)s_34.Invoke(null, new object[] { type, name });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_35 = s_mtds["GetNonRefType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Type GetNonRefType(this System.Type type)
        {
            try
            {
                return (System.Type)s_35.Invoke(null, new object[] { type });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_36 = s_mtds["CanCache"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type) }));

        public static System.Boolean CanCache(this System.Type t)
        {
            try
            {
                return (System.Boolean)s_36.Invoke(null, new object[] { t });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
namespace System.Linq.Expressions
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Stub generator", "1.0")]
    class ExpressionStubs
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Linq.Expressions.Expression");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["ValidateStaticOrInstanceMethod"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.Expression), typeof(System.Reflection.MethodInfo) }));

        public static void ValidateStaticOrInstanceMethod(System.Linq.Expressions.Expression instance, System.Reflection.MethodInfo method)
        {
            try
            {
                s_0.Invoke(null, new object[] { instance, method });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_1 = s_mtds["ValidateCallInstanceType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Reflection.MethodInfo) }));

        public static void ValidateCallInstanceType(System.Type instanceType, System.Reflection.MethodInfo method)
        {
            try
            {
                s_1.Invoke(null, new object[] { instanceType, method });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_2 = s_mtds["ValidateOneArgument"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.MethodBase), typeof(System.Linq.Expressions.ExpressionType), typeof(System.Linq.Expressions.Expression), typeof(System.Reflection.ParameterInfo) }));

        public static System.Linq.Expressions.Expression ValidateOneArgument(System.Reflection.MethodBase method, System.Linq.Expressions.ExpressionType nodeKind, System.Linq.Expressions.Expression arg, System.Reflection.ParameterInfo pi)
        {
            try
            {
                return (System.Linq.Expressions.Expression)s_2.Invoke(null, new object[] { method, nodeKind, arg, pi });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_3 = s_mtds["TryQuote"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Linq.Expressions.Expression).MakeByRefType() }));

        public static System.Boolean TryQuote(System.Type parameterType, ref System.Linq.Expressions.Expression argument)
        {
            try
            {
                return (System.Boolean)s_3.Invoke(null, new object[] { parameterType, argument });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_4 = s_mtds["RequiresCanRead"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.Expression), typeof(System.String) }));

        public static void RequiresCanRead(System.Linq.Expressions.Expression expression, System.String paramName)
        {
            try
            {
                s_4.Invoke(null, new object[] { expression, paramName });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_5 = s_mtds["RequiresCanRead"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression>), typeof(System.String) }));

        public static void RequiresCanRead(System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression> items, System.String paramName)
        {
            try
            {
                s_5.Invoke(null, new object[] { items, paramName });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_6 = s_mtds["GetInvokeMethod"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.Expression) }));

        public static System.Reflection.MethodInfo GetInvokeMethod(System.Linq.Expressions.Expression expression)
        {
            try
            {
                return (System.Reflection.MethodInfo)s_6.Invoke(null, new object[] { expression });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_7 = s_mtds["ValidateMethodInfo"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.MethodInfo) }));

        public static void ValidateMethodInfo(System.Reflection.MethodInfo method)
        {
            try
            {
                s_7.Invoke(null, new object[] { method });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
namespace System.Linq.Expressions
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Stub generator", "1.0")]
    static partial class Error
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Linq.Expressions.Error");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["ParameterExpressionNotValidAsDelegate"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object) }));

        public static System.Exception ParameterExpressionNotValidAsDelegate(System.Object p0, System.Object p1)
        {
            try
            {
                return (System.Exception)s_0.Invoke(null, new object[] { p0, p1 });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_1 = s_mtds["ArgumentCannotBeOfTypeVoid"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception ArgumentCannotBeOfTypeVoid()
        {
            try
            {
                return (System.Exception)s_1.Invoke(null, new object[] {  });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_2 = s_mtds["PropertyCannotHaveRefType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception PropertyCannotHaveRefType()
        {
            try
            {
                return (System.Exception)s_2.Invoke(null, new object[] {  });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_3 = s_mtds["AccessorsCannotHaveByRefArgs"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception AccessorsCannotHaveByRefArgs()
        {
            try
            {
                return (System.Exception)s_3.Invoke(null, new object[] {  });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_4 = s_mtds["BoundsCannotBeLessThanOne"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception BoundsCannotBeLessThanOne()
        {
            try
            {
                return (System.Exception)s_4.Invoke(null, new object[] {  });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_5 = s_mtds["PropertyTypeCannotBeVoid"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception PropertyTypeCannotBeVoid()
        {
            try
            {
                return (System.Exception)s_5.Invoke(null, new object[] {  });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_6 = s_mtds["DuplicateVariable"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object) }));

        public static System.Exception DuplicateVariable(System.Object p0)
        {
            try
            {
                return (System.Exception)s_6.Invoke(null, new object[] { p0 });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_7 = s_mtds["ArgumentMustBeInteger"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception ArgumentMustBeInteger()
        {
            try
            {
                return (System.Exception)s_7.Invoke(null, new object[] {  });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_8 = s_mtds["ExpressionTypeCannotInitializeArrayType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object) }));

        public static System.Exception ExpressionTypeCannotInitializeArrayType(System.Object p0, System.Object p1)
        {
            try
            {
                return (System.Exception)s_8.Invoke(null, new object[] { p0, p1 });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_9 = s_mtds["ExpressionTypeDoesNotMatchParameter"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object) }));

        public static System.Exception ExpressionTypeDoesNotMatchParameter(System.Object p0, System.Object p1)
        {
            try
            {
                return (System.Exception)s_9.Invoke(null, new object[] { p0, p1 });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_10 = s_mtds["ExpressionTypeDoesNotMatchReturn"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object) }));

        public static System.Exception ExpressionTypeDoesNotMatchReturn(System.Object p0, System.Object p1)
        {
            try
            {
                return (System.Exception)s_10.Invoke(null, new object[] { p0, p1 });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_11 = s_mtds["IncorrectNumberOfLambdaDeclarationParameters"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception IncorrectNumberOfLambdaDeclarationParameters()
        {
            try
            {
                return (System.Exception)s_11.Invoke(null, new object[] {  });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_12 = s_mtds["LambdaTypeMustBeDerivedFromSystemDelegate"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception LambdaTypeMustBeDerivedFromSystemDelegate()
        {
            try
            {
                return (System.Exception)s_12.Invoke(null, new object[] {  });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
namespace System.Linq.Expressions.Compiler
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Stub generator", "1.0")]
    static partial class DelegateHelpers
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Linq.Expressions.Compiler.DelegateHelpers");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["MakeDelegateType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type[]) }));

        public static System.Type MakeDelegateType(System.Type[] types)
        {
            try
            {
                return (System.Type)s_0.Invoke(null, new object[] { types });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
namespace System.Linq.Expressions.Compiler
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Stub generator", "1.0")]
    partial class StackSpiller
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Linq.Expressions.Compiler.StackSpiller");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["AnalyzeLambda"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.LambdaExpression) }));

        public static System.Linq.Expressions.LambdaExpression AnalyzeLambda(System.Linq.Expressions.LambdaExpression lambda)
        {
            try
            {
                return (System.Linq.Expressions.LambdaExpression)s_0.Invoke(null, new object[] { lambda });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
