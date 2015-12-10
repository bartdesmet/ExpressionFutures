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
    static class TypeUtils2
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Dynamic.Utils.TypeUtils");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["HasReferenceConversion"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Type), typeof(System.Type) }));

        public static System.Boolean HasReferenceConversion(System.Type source, System.Type dest)
        {
            try
            {
                var args = new object[] { source, dest };
                var res = s_0.Invoke(null, args);
                return (System.Boolean)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
