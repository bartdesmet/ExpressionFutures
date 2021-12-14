// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using static System.Dynamic.Utils.ContractUtils;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a property or field access for use in a property subpattern.
    /// </summary>
    public sealed class PropertyCSharpSubpatternMember
    {
        internal PropertyCSharpSubpatternMember(PropertyCSharpSubpatternMember receiver, MemberInfo member)
        {
            Receiver = receiver;
            Member = member;
        }

        /// <summary>
        /// Gets the receiver to access the member on, if any.
        /// </summary>
        public PropertyCSharpSubpatternMember Receiver { get; }

        /// <summary>
        /// Gets the member accessed by the subpattern.
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// Gets the type returned by the member.
        /// </summary>
        public Type Type => Member switch
        {
            PropertyInfo p => p.PropertyType,
            FieldInfo f => f.FieldType,
            _ => throw Unreachable,
        };

        /// <summary>
        /// Gets a value indicating whether the property represents a Count or Length property.
        /// </summary>
        public bool IsLengthOrCount
        {
            get
            {
                if (Member is PropertyInfo p && p.PropertyType == typeof(int))
                {
                    if (p.Name == nameof(ICollection.Count) || p.Name == nameof(Array.Length))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        internal Expression Reduce(Expression @object)
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
        /// <summary>
        /// Creates a property or field access for use in a property subpattern.
        /// </summary>
        /// <param name="member">The member to access.</param>
        /// <returns>A <see cref="PropertyCSharpSubpatternMember" /> representing a member accessed by a property subpattern.</returns>
        public static PropertyCSharpSubpatternMember PropertySubpatternMember(MemberInfo member) => PropertySubpatternMember(receiver: null, member);

        /// <summary>
        /// Creates a nested property or field access for use in a property subpattern.
        /// </summary>
        /// <param name="receiver">The subpattern member to access the member on.</param>
        /// <param name="member">The member to access.</param>
        /// <returns>A <see cref="PropertyCSharpSubpatternMember" /> representing a member accessed by a property subpattern.</returns>
        public static PropertyCSharpSubpatternMember PropertySubpatternMember(PropertyCSharpSubpatternMember receiver, MemberInfo member)
        {
            RequiresNotNull(member, nameof(member));

            if (member is MethodInfo m)
            {
                member = GetProperty(m);
            }

            switch (member)
            {
                case PropertyInfo p:
                    if (!p.CanRead)
                        throw Error.PropertyPatternMemberShouldBeReadable(p);
                    if (p.GetGetMethod().IsStatic)
                        throw Error.PropertyPatternMemberShouldNotBeStatic(p);
                    if (p.GetIndexParameters().Length > 0)
                        throw Error.PropertyPatternMemberShouldNotBeIndexer(p);
                    break;
                case FieldInfo f:
                    if (f.IsStatic)
                        throw Error.PropertyPatternMemberShouldNotBeStatic(f);
                    break;
                default:
                    throw LinqError.MemberNotFieldOrProperty(member);
            }

            if (receiver != null)
            {
                if (!TypeUtils.IsValidInstanceType(member, receiver.Type))
                {
                    throw Error.PropertyPatternMemberIsNotCompatibleWithReceiver(member, receiver.Type);
                }
            }

            return new PropertyCSharpSubpatternMember(receiver, member);
        }
    }
}
