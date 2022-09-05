// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal static class ElementInitStub
    {
        private static readonly ConstructorInfo s_ctor = typeof(ElementInit).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single();

        public static ElementInit Create(MethodInfo addMethod, ReadOnlyCollection<Expression> arguments)
        {
            return (ElementInit)s_ctor.Invoke(new object[] { addMethod, arguments });
        }
    }

    internal static class BinaryExpressionExtensions
    {
        private static readonly Type s_typ = typeof(BinaryExpression);
        private static readonly MethodInfo s_2 = s_typ.GetMethod("ReduceUserdefinedLifted", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)!;

        public static Expression ReduceUserdefinedLifted(this BinaryExpression obj)
        {
            try
            {
                return (Expression)s_2.Invoke(obj, Array.Empty<object>())!;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException!;
            }
        }
    }

    internal static class MemberExpressionStubs
    {
        private static readonly Type s_typ = typeof(MemberExpression);
        private static readonly ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["Make"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(Expression), typeof(MemberInfo) }));

        public static MemberExpression Make(Expression expression, MemberInfo member)
        {
            try
            {
                var args = new object[] { expression, member };
                var res = s_0.Invoke(null, args)!;
                return (MemberExpression)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException!;
            }
        }

    }

    internal static class BinaryExpressionStubs
    {
        private static readonly Type s_typ = typeof(BinaryExpression);
        private static readonly ILookup<string, MethodInfo> s_mtds = s_typ.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).ToLookup(m => m.Name);

        private static readonly MethodInfo s_0 = s_mtds["Create"].Single(m => m.IsStatic && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(ExpressionType), typeof(Expression), typeof(Expression), typeof(Type), typeof(MethodInfo), typeof(LambdaExpression) }));

        public static Expression Create(ExpressionType nodeType, Expression left, Expression right, Type? type, MethodInfo? method, LambdaExpression? conversion)
        {
            try
            {
                var args = new object?[] { nodeType, left, right, type, method, conversion };
                var res = s_0.Invoke(null, args)!;
                return (Expression)res;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException!;
            }
        }

    }
}
