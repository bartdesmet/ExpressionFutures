// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    public sealed class PropertyCSharpSubpatternMember
    {
        internal PropertyCSharpSubpatternMember(PropertyCSharpSubpatternMember receiver, MemberInfo member)
        {
            Receiver = receiver;
            Member = member;
        }

        public PropertyCSharpSubpatternMember Receiver { get; }
        public MemberInfo Member { get; }

        public Expression Reduce(Expression @object)
        {
            if (Receiver != null)
            {
                @object = Receiver.Reduce(@object);
            }

            return Expression.MakeMemberAccess(@object, Member);
        }
    }

    partial class CSharpPattern
    {
        public static PropertyCSharpSubpatternMember PropertySubpatternMember(MemberInfo member) => PropertySubpatternMember(receiver: null, member);

        public static PropertyCSharpSubpatternMember PropertySubpatternMember(PropertyCSharpSubpatternMember receiver, MemberInfo member)
        {
            if (member is MethodInfo m)
            {
                member = GetProperty(m);
            }

            return new PropertyCSharpSubpatternMember(receiver, member);
        }
    }
}
