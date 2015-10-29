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
                s_3.Invoke(null, new object[] { collection, paramName });
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
                s_4.Invoke(null, new object[] { array, offset, count, offsetName, countName });
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
                s_5.Invoke(null, new object[] { array, arrayName });
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
