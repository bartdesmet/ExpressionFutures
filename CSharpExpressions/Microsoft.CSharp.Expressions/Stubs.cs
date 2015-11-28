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
                var args = new object[] { precondition };
                var res = s_0.Invoke(null, args);
                
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
                var args = new object[] { precondition, paramName };
                var res = s_1.Invoke(null, args);
                
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
                var args = new object[] { value, paramName };
                var res = s_2.Invoke(null, args);
                
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
                var args = new object[] { collection, paramName };
                var res = s_3.MakeGenericMethod(typeof(T)).Invoke(null, args);
                
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
                var args = new object[] { array, offset, count, offsetName, countName };
                var res = s_4.MakeGenericMethod(typeof(T)).Invoke(null, args);
                
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
                var args = new object[] { array, arrayName };
                var res = s_5.MakeGenericMethod(typeof(T)).Invoke(null, args);
                
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
                var args = new object[] { methodInfo, delegateType, target };
                var res = s_0.Invoke(null, args);
                return (System.Delegate)res;
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
                var args = new object[] { mi };
                var res = s_1.Invoke(null, args);
                return (System.Type)res;
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
                var args = new object[] { method };
                var res = s_2.Invoke(null, args);
                return (System.Reflection.ParameterInfo[])res;
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
                var args = new object[] { pi };
                var res = s_3.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { type, name, bindingAttr, binder, types, modifiers };
                var res = s_4.Invoke(null, args);
                return (System.Reflection.MethodInfo)res;
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
                var args = new object[] { mi, argTypes };
                var res = s_5.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { enumerable };
                var res = s_0.MakeGenericMethod(typeof(T)).Invoke(null, args);
                return (System.Collections.ObjectModel.ReadOnlyCollection<T>)res;
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
                var args = new object[] { list };
                var res = s_1.MakeGenericMethod(typeof(T)).Invoke(null, args);
                return (System.Int32)res;
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
                var args = new object[] { first, second };
                var res = s_2.MakeGenericMethod(typeof(T)).Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { collection, select };
                var res = s_4.MakeGenericMethod(typeof(T), typeof(U)).Invoke(null, args);
                return (U[])res;
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
                var args = new object[] { array };
                var res = s_8.MakeGenericMethod(typeof(T)).Invoke(null, args);
                return (T[])res;
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
                var args = new object[] { array };
                var res = s_9.MakeGenericMethod(typeof(T)).Invoke(null, args);
                return (T[])res;
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
                var args = new object[] { list, item };
                var res = s_10.MakeGenericMethod(typeof(T)).Invoke(null, args);
                return (T[])res;
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
                var args = new object[] { list, item };
                var res = s_11.MakeGenericMethod(typeof(T)).Invoke(null, args);
                return (T[])res;
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
                var args = new object[] { list };
                var res = s_13.MakeGenericMethod(typeof(T)).Invoke(null, args);
                return (T)res;
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
                var args = new object[] { array };
                var res = s_14.MakeGenericMethod(typeof(T)).Invoke(null, args);
                return (T[])res;
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
                var args = new object[] { type };
                var res = s_0.Invoke(null, args);
                return (System.Type)res;
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
                var args = new object[] { type };
                var res = s_1.Invoke(null, args);
                return (System.Type)res;
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
                var args = new object[] { type };
                var res = s_2.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { type };
                var res = s_3.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { type };
                var res = s_4.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { type };
                var res = s_5.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { type };
                var res = s_6.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { type };
                var res = s_7.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { type };
                var res = s_8.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { t1, t2 };
                var res = s_9.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { dest, src };
                var res = s_10.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { member, instanceType };
                var res = s_11.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { source, dest };
                var res = s_12.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { source, dest };
                var res = s_13.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { t };
                var res = s_14.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { t };
                var res = s_15.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { t };
                var res = s_16.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { t };
                var res = s_17.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { source, dest };
                var res = s_18.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { type };
                var res = s_19.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { left, right };
                var res = s_20.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { left, right };
                var res = s_21.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { source, destination };
                var res = s_22.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { convertFrom, convertToType, implicitOnly };
                var res = s_23.Invoke(null, args);
                return (System.Reflection.MethodInfo)res;
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
                var args = new object[] { methods, typeFrom, typeTo, implicitOnly };
                var res = s_24.Invoke(null, args);
                return (System.Reflection.MethodInfo)res;
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
                var args = new object[] { source, destination };
                var res = s_25.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { source, destination };
                var res = s_26.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { source, destination };
                var res = s_27.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { source, destination };
                var res = s_28.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { type, subType };
                var res = s_29.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { type };
                var res = s_30.Invoke(null, args);
                
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
                var args = new object[] { definition, type };
                var res = s_31.Invoke(null, args);
                return (System.Type)res;
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
                var args = new object[] { type };
                var res = s_32.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { type };
                var res = s_33.Invoke(null, args);
                return (System.Boolean)res;
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
                var args = new object[] { type, name };
                var res = s_34.Invoke(null, args);
                return (System.Reflection.MethodInfo)res;
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
                var args = new object[] { type };
                var res = s_35.Invoke(null, args);
                return (System.Type)res;
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
                var args = new object[] { t };
                var res = s_36.Invoke(null, args);
                return (System.Boolean)res;
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

        private static readonly MethodInfo s_0 = s_mtds["GetProperty"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.MethodInfo) }));

        public static System.Reflection.PropertyInfo GetProperty(System.Reflection.MethodInfo mi)
        {
            try
            {
                var args = new object[] { mi };
                var res = s_0.Invoke(null, args);
                return (System.Reflection.PropertyInfo)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_1 = s_mtds["ValidateStaticOrInstanceMethod"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.Expression), typeof(System.Reflection.MethodInfo) }));

        public static void ValidateStaticOrInstanceMethod(System.Linq.Expressions.Expression instance, System.Reflection.MethodInfo method)
        {
            try
            {
                var args = new object[] { instance, method };
                var res = s_1.Invoke(null, args);
                
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_2 = s_mtds["ValidateCallInstanceType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Reflection.MethodInfo) }));

        public static void ValidateCallInstanceType(System.Type instanceType, System.Reflection.MethodInfo method)
        {
            try
            {
                var args = new object[] { instanceType, method };
                var res = s_2.Invoke(null, args);
                
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_3 = s_mtds["ValidateArgumentTypes"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.MethodBase), typeof(System.Linq.Expressions.ExpressionType), typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression>).MakeByRefType() }));

        public static void ValidateArgumentTypes(System.Reflection.MethodBase method, System.Linq.Expressions.ExpressionType nodeKind, ref System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> arguments)
        {
            try
            {
                var args = new object[] { method, nodeKind, arguments };
                var res = s_3.Invoke(null, args);
                arguments = (System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression>)args[2];
                
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_4 = s_mtds["ValidateOneArgument"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.MethodBase), typeof(System.Linq.Expressions.ExpressionType), typeof(System.Linq.Expressions.Expression), typeof(System.Reflection.ParameterInfo) }));

        public static System.Linq.Expressions.Expression ValidateOneArgument(System.Reflection.MethodBase method, System.Linq.Expressions.ExpressionType nodeKind, System.Linq.Expressions.Expression arg, System.Reflection.ParameterInfo pi)
        {
            try
            {
                var args = new object[] { method, nodeKind, arg, pi };
                var res = s_4.Invoke(null, args);
                return (System.Linq.Expressions.Expression)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_5 = s_mtds["TryQuote"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Linq.Expressions.Expression).MakeByRefType() }));

        public static System.Boolean TryQuote(System.Type parameterType, ref System.Linq.Expressions.Expression argument)
        {
            try
            {
                var args = new object[] { parameterType, argument };
                var res = s_5.Invoke(null, args);
                argument = (System.Linq.Expressions.Expression)args[1];
                return (System.Boolean)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_6 = s_mtds["RequiresCanRead"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.Expression), typeof(System.String) }));

        public static void RequiresCanRead(System.Linq.Expressions.Expression expression, System.String paramName)
        {
            try
            {
                var args = new object[] { expression, paramName };
                var res = s_6.Invoke(null, args);
                
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_7 = s_mtds["RequiresCanRead"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression>), typeof(System.String) }));

        public static void RequiresCanRead(System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression> items, System.String paramName)
        {
            try
            {
                var args = new object[] { items, paramName };
                var res = s_7.Invoke(null, args);
                
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_8 = s_mtds["RequiresCanWrite"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.Expression), typeof(System.String) }));

        public static void RequiresCanWrite(System.Linq.Expressions.Expression expression, System.String paramName)
        {
            try
            {
                var args = new object[] { expression, paramName };
                var res = s_8.Invoke(null, args);
                
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_9 = s_mtds["ValidateIndexedProperty"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.Expression), typeof(System.Reflection.PropertyInfo), typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression>).MakeByRefType() }));

        public static void ValidateIndexedProperty(System.Linq.Expressions.Expression instance, System.Reflection.PropertyInfo property, ref System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> argList)
        {
            try
            {
                var args = new object[] { instance, property, argList };
                var res = s_9.Invoke(null, args);
                argList = (System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression>)args[2];
                
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_10 = s_mtds["GetInvokeMethod"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.Expression) }));

        public static System.Reflection.MethodInfo GetInvokeMethod(System.Linq.Expressions.Expression expression)
        {
            try
            {
                var args = new object[] { expression };
                var res = s_10.Invoke(null, args);
                return (System.Reflection.MethodInfo)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_11 = s_mtds["ValidateMethodInfo"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Reflection.MethodInfo) }));

        public static void ValidateMethodInfo(System.Reflection.MethodInfo method)
        {
            try
            {
                var args = new object[] { method };
                var res = s_11.Invoke(null, args);
                
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

        private static readonly MethodInfo s_0 = s_mtds["NotSupported"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception NotSupported()
        {
            try
            {
                var args = new object[] {  };
                var res = s_0.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_1 = s_mtds["ParameterExpressionNotValidAsDelegate"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object) }));

        public static System.Exception ParameterExpressionNotValidAsDelegate(System.Object p0, System.Object p1)
        {
            try
            {
                var args = new object[] { p0, p1 };
                var res = s_1.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_2 = s_mtds["PropertyNotDefinedForType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object) }));

        public static System.Exception PropertyNotDefinedForType(System.Object p0, System.Object p1)
        {
            try
            {
                var args = new object[] { p0, p1 };
                var res = s_2.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_3 = s_mtds["InstancePropertyNotDefinedForType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object) }));

        public static System.Exception InstancePropertyNotDefinedForType(System.Object p0, System.Object p1)
        {
            try
            {
                var args = new object[] { p0, p1 };
                var res = s_3.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_4 = s_mtds["UnhandledBinary"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object) }));

        public static System.Exception UnhandledBinary(System.Object p0)
        {
            try
            {
                var args = new object[] { p0 };
                var res = s_4.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_5 = s_mtds["UnhandledBinding"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception UnhandledBinding()
        {
            try
            {
                var args = new object[] {  };
                var res = s_5.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_6 = s_mtds["UnhandledUnary"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object) }));

        public static System.Exception UnhandledUnary(System.Object p0)
        {
            try
            {
                var args = new object[] { p0 };
                var res = s_6.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_7 = s_mtds["ArgumentCannotBeOfTypeVoid"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception ArgumentCannotBeOfTypeVoid()
        {
            try
            {
                var args = new object[] {  };
                var res = s_7.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_8 = s_mtds["InvalidLvalue"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object) }));

        public static System.Exception InvalidLvalue(System.Object p0)
        {
            try
            {
                var args = new object[] { p0 };
                var res = s_8.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_9 = s_mtds["TryNotSupportedForMethodsWithRefArgs"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object) }));

        public static System.Exception TryNotSupportedForMethodsWithRefArgs(System.Object p0)
        {
            try
            {
                var args = new object[] { p0 };
                var res = s_9.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_10 = s_mtds["TryNotSupportedForValueTypeInstances"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object) }));

        public static System.Exception TryNotSupportedForValueTypeInstances(System.Object p0)
        {
            try
            {
                var args = new object[] { p0 };
                var res = s_10.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_11 = s_mtds["PropertyCannotHaveRefType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception PropertyCannotHaveRefType()
        {
            try
            {
                var args = new object[] {  };
                var res = s_11.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_12 = s_mtds["AccessorsCannotHaveByRefArgs"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception AccessorsCannotHaveByRefArgs()
        {
            try
            {
                var args = new object[] {  };
                var res = s_12.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_13 = s_mtds["BoundsCannotBeLessThanOne"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception BoundsCannotBeLessThanOne()
        {
            try
            {
                var args = new object[] {  };
                var res = s_13.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_14 = s_mtds["PropertyTypeCannotBeVoid"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception PropertyTypeCannotBeVoid()
        {
            try
            {
                var args = new object[] {  };
                var res = s_14.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_15 = s_mtds["LabelTypeMustBeVoid"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception LabelTypeMustBeVoid()
        {
            try
            {
                var args = new object[] {  };
                var res = s_15.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_16 = s_mtds["DuplicateVariable"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object) }));

        public static System.Exception DuplicateVariable(System.Object p0)
        {
            try
            {
                var args = new object[] { p0 };
                var res = s_16.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_17 = s_mtds["ArgumentMustBeArray"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception ArgumentMustBeArray()
        {
            try
            {
                var args = new object[] {  };
                var res = s_17.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_18 = s_mtds["ArgumentMustBeBoolean"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception ArgumentMustBeBoolean()
        {
            try
            {
                var args = new object[] {  };
                var res = s_18.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_19 = s_mtds["ArgumentMustBeInteger"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception ArgumentMustBeInteger()
        {
            try
            {
                var args = new object[] {  };
                var res = s_19.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_20 = s_mtds["ArgumentMustBeArrayIndexType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception ArgumentMustBeArrayIndexType()
        {
            try
            {
                var args = new object[] {  };
                var res = s_20.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_21 = s_mtds["ArgumentTypesMustMatch"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception ArgumentTypesMustMatch()
        {
            try
            {
                var args = new object[] {  };
                var res = s_21.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_22 = s_mtds["CannotAutoInitializeValueTypeElementThroughProperty"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object) }));

        public static System.Exception CannotAutoInitializeValueTypeElementThroughProperty(System.Object p0)
        {
            try
            {
                var args = new object[] { p0 };
                var res = s_22.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_23 = s_mtds["CannotAutoInitializeValueTypeMemberThroughProperty"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object) }));

        public static System.Exception CannotAutoInitializeValueTypeMemberThroughProperty(System.Object p0)
        {
            try
            {
                var args = new object[] { p0 };
                var res = s_23.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_24 = s_mtds["ExpressionTypeCannotInitializeArrayType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object) }));

        public static System.Exception ExpressionTypeCannotInitializeArrayType(System.Object p0, System.Object p1)
        {
            try
            {
                var args = new object[] { p0, p1 };
                var res = s_24.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_25 = s_mtds["ExpressionTypeDoesNotMatchParameter"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object) }));

        public static System.Exception ExpressionTypeDoesNotMatchParameter(System.Object p0, System.Object p1)
        {
            try
            {
                var args = new object[] { p0, p1 };
                var res = s_25.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_26 = s_mtds["ExpressionTypeDoesNotMatchReturn"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object) }));

        public static System.Exception ExpressionTypeDoesNotMatchReturn(System.Object p0, System.Object p1)
        {
            try
            {
                var args = new object[] { p0, p1 };
                var res = s_26.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_27 = s_mtds["ExpressionTypeDoesNotMatchAssignment"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object) }));

        public static System.Exception ExpressionTypeDoesNotMatchAssignment(System.Object p0, System.Object p1)
        {
            try
            {
                var args = new object[] { p0, p1 };
                var res = s_27.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_28 = s_mtds["InstanceFieldNotDefinedForType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object) }));

        public static System.Exception InstanceFieldNotDefinedForType(System.Object p0, System.Object p1)
        {
            try
            {
                var args = new object[] { p0, p1 };
                var res = s_28.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_29 = s_mtds["FieldInfoNotDefinedForType"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object), typeof(System.Object), typeof(System.Object) }));

        public static System.Exception FieldInfoNotDefinedForType(System.Object p0, System.Object p1, System.Object p2)
        {
            try
            {
                var args = new object[] { p0, p1, p2 };
                var res = s_29.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_30 = s_mtds["IncorrectNumberOfIndexes"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception IncorrectNumberOfIndexes()
        {
            try
            {
                var args = new object[] {  };
                var res = s_30.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_31 = s_mtds["IncorrectNumberOfLambdaDeclarationParameters"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception IncorrectNumberOfLambdaDeclarationParameters()
        {
            try
            {
                var args = new object[] {  };
                var res = s_31.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_32 = s_mtds["IncorrectNumberOfMethodCallArguments"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object) }));

        public static System.Exception IncorrectNumberOfMethodCallArguments(System.Object p0)
        {
            try
            {
                var args = new object[] { p0 };
                var res = s_32.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_33 = s_mtds["LambdaTypeMustBeDerivedFromSystemDelegate"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] {  }));

        public static System.Exception LambdaTypeMustBeDerivedFromSystemDelegate()
        {
            try
            {
                var args = new object[] {  };
                var res = s_33.Invoke(null, args);
                return (System.Exception)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_34 = s_mtds["MemberNotFieldOrProperty"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Object) }));

        public static System.Exception MemberNotFieldOrProperty(System.Object p0)
        {
            try
            {
                var args = new object[] { p0 };
                var res = s_34.Invoke(null, args);
                return (System.Exception)res;
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
    static partial class Strings
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Linq.Expressions.Strings");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly PropertyInfo s_0 = s_typ.GetProperty("ExpressionMustBeWriteable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        public static System.String ExpressionMustBeWriteable
        {
            get
            {
                try
                {
                    return (System.String)s_0.GetValue(null);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
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
                var args = new object[] { types };
                var res = s_0.Invoke(null, args);
                return (System.Type)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_1 = s_mtds["MakeCallSiteDelegate"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression>), typeof(System.Type) }));

        public static System.Type MakeCallSiteDelegate(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> types, System.Type returnType)
        {
            try
            {
                var args = new object[] { types, returnType };
                var res = s_1.Invoke(null, args);
                return (System.Type)res;
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
    partial class StackSpillerStub
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Linq.Expressions.Compiler.StackSpiller");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["AnalyzeLambda"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.LambdaExpression) }));

        public static System.Linq.Expressions.LambdaExpression AnalyzeLambda(System.Linq.Expressions.LambdaExpression lambda)
        {
            try
            {
                var args = new object[] { lambda };
                var res = s_0.Invoke(null, args);
                return (System.Linq.Expressions.LambdaExpression)res;
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
    partial class MemberExpressionStubs
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Linq.Expressions.MemberExpression");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["Make"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.Expression), typeof(System.Reflection.MemberInfo) }));

        public static System.Linq.Expressions.MemberExpression Make(System.Linq.Expressions.Expression expression, System.Reflection.MemberInfo member)
        {
            try
            {
                var args = new object[] { expression, member };
                var res = s_0.Invoke(null, args);
                return (System.Linq.Expressions.MemberExpression)res;
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
    partial class BinaryExpressionStubs
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Linq.Expressions.BinaryExpression");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["Create"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.ExpressionType), typeof(System.Linq.Expressions.Expression), typeof(System.Linq.Expressions.Expression), typeof(System.Type), typeof(System.Reflection.MethodInfo), typeof(System.Linq.Expressions.LambdaExpression) }));

        public static System.Linq.Expressions.Expression Create(System.Linq.Expressions.ExpressionType nodeType, System.Linq.Expressions.Expression left, System.Linq.Expressions.Expression right, System.Type type, System.Reflection.MethodInfo method, System.Linq.Expressions.LambdaExpression conversion)
        {
            try
            {
                var args = new object[] { nodeType, left, right, type, method, conversion };
                var res = s_0.Invoke(null, args);
                return (System.Linq.Expressions.Expression)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
