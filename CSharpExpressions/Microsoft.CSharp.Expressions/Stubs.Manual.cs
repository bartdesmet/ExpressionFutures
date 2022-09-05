// Prototyping extended expression trees for C#.
//
// bartde - October 2015

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
        private static readonly PropertyInfo s_1 = s_typ.GetProperty("IsLiftedLogical", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        private static readonly MethodInfo s_2 = s_typ.GetMethod("ReduceUserdefinedLifted", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        public static bool get_IsLiftedLogical(this BinaryExpression obj)
        {
            try
            {
                return (bool)s_1.GetValue(obj);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public static Expression ReduceUserdefinedLifted(this BinaryExpression obj)
        {
            try
            {
                return (Expression)s_2.Invoke(obj, Array.Empty<object>());
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
