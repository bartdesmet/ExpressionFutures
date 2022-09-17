// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents the initialization of a member, e.g. for use in a with expression.
    /// </summary>
    public sealed partial class MemberInitializer
    {
        internal MemberInitializer(MemberInfo member, Expression expression)
        {
            Member = member;
            Expression = expression;
        }

        /// <summary>
        /// Gets the member to be initialized.
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> which represents the object being assigned to the member.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public MemberInitializer Update(Expression expression)
        {
            if (expression == Expression)
            {
                return this;
            }

            return CSharpExpression.MemberInitializer(Member, expression);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.MemberInitializer"/> binding the specified value to the given member.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo"/> for the member which is being assigned to.</param>
        /// <param name="expression">The value to be assigned to <paramref name="member"/>.</param>
        /// <returns>The created <see cref="Microsoft.CSharp.Expressions.MemberInitializer"/>.</returns>
        public static MemberInitializer MemberInitializer(MemberInfo member, Expression expression)
        {
            RequiresNotNull(member, nameof(member));

            //
            // NB: System.Linq.Expressions fails to checks for static members in ValidateSettableFieldOrPropertyMember.
            //
            //       class Foo { public static int Bar { get; set; } }
            //
            //       Expression.Lambda<Func<Foo>>(
            //         Expression.MemberInit(
            //           Expression.New(typeof(Foo)),
            //           Expression.Bind(typeof(Foo).GetProperty("Bar"), Expression.Constant(1))
            //         )
            //       ).Compile()
            //
            //     throws 'System.Security.VerificationException: Operation could destabilize the runtime.'
            //
            //     We add this check here.
            //

            Type memberType;

            switch (member)
            {
                case FieldInfo f:
                    memberType = f.FieldType;

                    if (f.IsStatic)
                        throw Error.MemberInitializerMemberMustNotBeStatic(member.Name, nameof(member));

                    break;

                case PropertyInfo p:
                    memberType = p.PropertyType;

                    var accessor = p.GetGetMethod(nonPublic: true) ?? p.GetSetMethod(nonPublic: true);
                    if (accessor == null)
                        throw PropertyDoesNotHaveAccessor(p, nameof(member));
                    
                    if (accessor.IsStatic)
                        throw Error.MemberInitializerMemberMustNotBeStatic(member.Name, nameof(member));

                    if (p.GetIndexParameters().Length > 0)
                        throw Error.MemberInitializerMemberMustNotBeIndexer(member.Name, nameof(member));

                    break;

                default:
                    throw ArgumentMustBeFieldInfoOrPropertyInfo(nameof(member));
            }

            RequiresCanRead(expression, nameof(expression));

            //
            // NB: We don't check here for an accessible setter in order to support anonymous types where
            //     the getter is specified instead.
            //

            if (!memberType.IsAssignableFrom(expression.Type))
                throw ArgumentTypesMustMatch(nameof(expression));

            if (member.DeclaringType == null)
                throw Error.NotAMemberOfAnyType(member, nameof(member));

            ValidateType(member.DeclaringType, nameof(member));

            return new MemberInitializer(member, expression);
        }

        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.MemberInitializer"/> binding the specified value to the given property.
        /// </summary>
        /// <param name="propertyAccessor">The <see cref="MethodInfo"/> for the property get or set method of the property which is being assigned to.</param>
        /// <param name="expression">The value to be assigned to the property.</param>
        /// <returns>The created <see cref="Microsoft.CSharp.Expressions.MemberInitializer"/>.</returns>
        public static MemberInitializer MemberInitializer(MethodInfo propertyAccessor, Expression expression)
        {
            RequiresNotNull(propertyAccessor, nameof(propertyAccessor));
            RequiresNotNull(expression, nameof(expression));

            ValidateMethodInfo(propertyAccessor, nameof(propertyAccessor));

            return MemberInitializer(GetProperty(propertyAccessor, nameof(propertyAccessor)), expression);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="MemberInitializer" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual MemberInitializer VisitMemberInitializer(MemberInitializer node) =>
            node.Update(
                Visit(node.Expression)
            );
    }
}
