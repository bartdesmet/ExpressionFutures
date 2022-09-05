// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Linq;
using System.Reflection;

// NOTE: Code generated in this file is not product code; it unblocks using existing APIs that are not
//       visible to the prototype.

namespace System.Linq.Expressions.Compiler
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Stub generator", "1.0")]
    partial class StackSpillerStub
    {
        private static Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static Type s_typ = s_asm.GetType("System.Linq.Expressions.Compiler.StackSpiller");
        private static ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly Lazy<MethodInfo> s_0 = new Lazy<MethodInfo>(() => s_mtds["AnalyzeLambda"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.LambdaExpression) })));

        public static System.Linq.Expressions.LambdaExpression AnalyzeLambda(System.Linq.Expressions.LambdaExpression lambda)
        {
            try
            {
                var args = new object[] { lambda };
                var res = s_0.Value.Invoke(null, args);
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

        private static readonly Lazy<MethodInfo> s_0 = new Lazy<MethodInfo>(() => s_mtds["Make"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.Expression), typeof(System.Reflection.MemberInfo) })));

        public static System.Linq.Expressions.MemberExpression Make(System.Linq.Expressions.Expression expression, System.Reflection.MemberInfo member)
        {
            try
            {
                var args = new object[] { expression, member };
                var res = s_0.Value.Invoke(null, args);
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

        private static readonly Lazy<MethodInfo> s_0 = new Lazy<MethodInfo>(() => s_mtds["Create"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(System.Linq.Expressions.ExpressionType), typeof(System.Linq.Expressions.Expression), typeof(System.Linq.Expressions.Expression), typeof(System.Type), typeof(System.Reflection.MethodInfo), typeof(System.Linq.Expressions.LambdaExpression) })));

        public static System.Linq.Expressions.Expression Create(System.Linq.Expressions.ExpressionType nodeType, System.Linq.Expressions.Expression left, System.Linq.Expressions.Expression right, System.Type type, System.Reflection.MethodInfo method, System.Linq.Expressions.LambdaExpression conversion)
        {
            try
            {
                var args = new object[] { nodeType, left, right, type, method, conversion };
                var res = s_0.Value.Invoke(null, args);
                return (System.Linq.Expressions.Expression)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
