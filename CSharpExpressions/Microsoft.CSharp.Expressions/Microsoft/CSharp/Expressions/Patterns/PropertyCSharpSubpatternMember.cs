// Prototyping extended expression trees for C#.
//
// bartde - December 2021

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    using static PatternHelpers;

    /// <summary>
    /// Represents a property or field access for use in a property subpattern.
    /// </summary>
    public abstract class PropertyCSharpSubpatternMember
    {
        internal PropertyCSharpSubpatternMember(PropertyCSharpSubpatternMember? receiver) => Receiver = receiver;

        /// <summary>
        /// Gets the receiver to access the member on, if any.
        /// </summary>
        public PropertyCSharpSubpatternMember? Receiver { get; }

        /// <summary>
        /// Gets the member accessed by the subpattern.
        /// </summary>
        public abstract MemberInfo? Member { get; }

        /// <summary>
        /// Gets the tuple field accessed by the subpattern.
        /// </summary>
        public abstract TupleFieldInfo? TupleField { get; }

        /// <summary>
        /// Gets the type returned by the member.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Gets a value indicating whether the property represents a Count or Length property.
        /// </summary>
        public abstract bool IsLengthOrCount { get; }

        internal Expression Reduce(Expression @object, List<ParameterExpression> vars, List<Expression> stmts, LabelTarget exit)
        {
            if (Receiver != null)
            {
                @object = Receiver.Reduce(@object, vars, stmts, exit);
            }

            return PatternHelpers.Reduce(@object, obj =>
            {
                // NB: The parent pattern checks for null on the input to the left-most property, but for all subsequent accesses
                //     we need to take care of the checks. An alternative strategy would have been to lower { A.B: p } to { A: { B: p } }
                //     and let the checks naturally emerge.

                if (Receiver != null)
                {
                    if (obj.Type.IsNullableType())
                    {
                        var nonNullType = obj.Type.GetNonNullableType();

                        AddFailIfNot(Expression.TypeIs(obj, nonNullType), exit, stmts);
                        obj = Expression.Convert(obj, nonNullType);
                    }
                    else if (!obj.Type.IsValueType)
                    {
                        AddFailIfNot(Expression.ReferenceNotEqual(obj, Expression.Constant(null, obj.Type)), exit, stmts);
                    }
                }

                return ReduceCore(obj);
            }, vars, stmts);
        }

        internal abstract Expression ReduceCore(Expression @object);

        internal sealed class WithMemberInfo : PropertyCSharpSubpatternMember
        {
            private readonly MemberInfo _member;

            public WithMemberInfo(PropertyCSharpSubpatternMember? receiver, MemberInfo member)
                : base(receiver)
            {
                _member = member;
            }

            public override MemberInfo? Member => _member;

            public override TupleFieldInfo? TupleField => null;

            public override Type Type => _member switch
            {
                PropertyInfo p => p.PropertyType,
                FieldInfo f => f.FieldType,
                _ => throw Unreachable,
            };

            public override bool IsLengthOrCount
            {
                get
                {
                    if (_member is PropertyInfo p && p.PropertyType == typeof(int))
                    {
                        if (p.Name == nameof(ICollection.Count) || p.Name == nameof(Array.Length))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            internal override Expression ReduceCore(Expression @object)
            {
                return Expression.MakeMemberAccess(@object, _member);
            }
        }

        internal sealed class WithTupleField : PropertyCSharpSubpatternMember
        {
            private readonly TupleFieldInfo _field;

            public WithTupleField(PropertyCSharpSubpatternMember? receiver, TupleFieldInfo field)
                : base(receiver)
            {
                _field = field;
            }

            public override MemberInfo? Member => null;

            public override TupleFieldInfo? TupleField => _field;

            public override Type Type => _field.Type;

            public override bool IsLengthOrCount => false;

            internal override Expression ReduceCore(Expression @object)
            {
                return Helpers.GetTupleItemAccess(@object, _field.Index);
            }
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
        /// Creates a tuple field access for use in a property subpattern.
        /// </summary>
        /// <param name="field">The tuple field to access.</param>
        /// <returns>A <see cref="PropertyCSharpSubpatternMember" /> representing a tuple field accessed by a property subpattern.</returns>
        public static PropertyCSharpSubpatternMember PropertySubpatternMember(TupleFieldInfo field) => PropertySubpatternMember(receiver: null, field);

        /// <summary>
        /// Creates a nested property or field access for use in a property subpattern.
        /// </summary>
        /// <param name="receiver">The subpattern member to access the member on.</param>
        /// <param name="member">The member to access.</param>
        /// <returns>A <see cref="PropertyCSharpSubpatternMember" /> representing a member accessed by a property subpattern.</returns>
        public static PropertyCSharpSubpatternMember PropertySubpatternMember(PropertyCSharpSubpatternMember? receiver, MemberInfo member)
        {
            RequiresNotNull(member, nameof(member));

            if (member is MethodInfo m)
            {
                member = GetProperty(m, nameof(member));
            }

            switch (member)
            {
                case PropertyInfo p:
                    if (!p.CanRead)
                        throw Error.PropertyPatternMemberShouldBeReadable(p);
                    if (p.GetGetMethod()!.IsStatic)
                        throw Error.PropertyPatternMemberShouldNotBeStatic(p);
                    if (p.GetIndexParameters().Length > 0)
                        throw Error.PropertyPatternMemberShouldNotBeIndexer(p);
                    break;
                case FieldInfo f:
                    if (f.IsStatic)
                        throw Error.PropertyPatternMemberShouldNotBeStatic(f);
                    break;
                default:
                    throw MemberNotFieldOrProperty(member, nameof(member));
            }

            if (receiver != null)
            {
                var nonNullReceiverType = receiver.Type.GetNonNullableType();

                if (!TypeUtils.IsValidInstanceType(member, nonNullReceiverType))
                {
                    throw Error.PropertyPatternMemberIsNotCompatibleWithReceiver(member, nonNullReceiverType);
                }
            }

            return new PropertyCSharpSubpatternMember.WithMemberInfo(receiver, member);
        }

        /// <summary>
        /// Creates a nested tuple field access for use in a property subpattern.
        /// </summary>
        /// <param name="receiver">The subpattern member to access the member on.</param>
        /// <param name="field">The tuple field to access.</param>
        /// <returns>A <see cref="PropertyCSharpSubpatternMember" /> representing a tuple field accessed by a property subpattern.</returns>
        public static PropertyCSharpSubpatternMember PropertySubpatternMember(PropertyCSharpSubpatternMember? receiver, TupleFieldInfo field)
        {
            RequiresNotNull(field, nameof(field));

            return new PropertyCSharpSubpatternMember.WithTupleField(receiver, field);
        }
    }
}
