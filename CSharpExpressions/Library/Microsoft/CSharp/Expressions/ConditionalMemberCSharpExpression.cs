// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents conditional (null-propagating) access to a member.
    /// </summary>
    public abstract class ConditionalMemberCSharpExpression : ConditionalAccessCSharpExpression
    {
        internal ConditionalMemberCSharpExpression(Expression expression, MemberInfo member)
            : base(expression)
        {
            Member = member;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.ConditionalMemberAccess;

        /// <summary>
        /// Gets the field or property to be accessed.
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitConditionalMember(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="ConditionalAccessCSharpExpression.Expression" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ConditionalMemberCSharpExpression Update(Expression expression)
        {
            if (expression == Expression)
            {
                return this;
            }

            return CSharpExpression.MakeConditionalMemberAccess(expression, Member);
        }

        /// <summary>
        /// Reduces the expression to an unconditional non-null access on the specified expression.
        /// </summary>
        /// <param name="nonNull">Non-null expression to apply the access to.</param>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceAccess(Expression nonNull) => Expression.MakeMemberAccess(nonNull, Member);

        internal static ConditionalMemberCSharpExpression Make(Expression expression, MemberInfo member)
        {
            var field = member as FieldInfo;
            if (field != null)
            {
                return new FieldExpression(expression, field);
            }
            else
            {
                return new PropertyExpression(expression, (PropertyInfo)member);
            }
        }

        class FieldExpression : ConditionalMemberCSharpExpression
        {
            public FieldExpression(Expression receiver, MemberInfo member)
                : base(receiver, member)
            {
            }

            protected override Type UnderlyingType => ((FieldInfo)Member).FieldType;
        }

        class PropertyExpression : ConditionalMemberCSharpExpression
        {
            public PropertyExpression(Expression receiver, MemberInfo member)
                : base(receiver, member)
            {
            }

            protected override Type UnderlyingType => ((PropertyInfo)Member).PropertyType;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="ConditionalMemberCSharpExpression" /> that represents a conditional (null-propagating) member lookup.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> that specifies the instance to access the member of.</param>
        /// <param name="member">The <see cref="MemberInfo" /> representing the member to access conditionally.</param>
        /// <returns>A <see cref="ConditionalMemberCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalMemberAccess" /> and the <see cref="Microsoft.CSharp.Expressions.ConditionalAccessCSharpExpression.Receiver" /> and <see cref="Microsoft.CSharp.Expressions.ConditionalMemberCSharpExpression.Member" /> properties set to the specified values.</returns>
        public static ConditionalMemberCSharpExpression MakeConditionalMemberAccess(Expression expression, MemberInfo member)
        {
            ContractUtils.RequiresNotNull(member, "member");

            var fieldInfo = member as FieldInfo;
            if (fieldInfo != null)
            {
                return ConditionalField(expression, fieldInfo);
            }

            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                return ConditionalProperty(expression, propertyInfo);
            }

            throw LinqError.MemberNotFieldOrProperty(member);
        }

        /// <summary>
        /// Creates a <see cref="ConditionalMemberCSharpExpression" /> that represents a conditional (null-propagating) field lookup.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> that specifies the instance to access the member of.</param>
        /// <param name="field">The <see cref="FieldInfo" /> representing the field to access conditionally.</param>
        /// <returns>A <see cref="ConditionalMemberCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalMemberAccess" /> and the <see cref="ConditionalAccessCSharpExpression.Expression" /> and <see cref="ConditionalMemberCSharpExpression.Member" /> properties set to the specified values.</returns>
        public static ConditionalMemberCSharpExpression ConditionalField(Expression expression, FieldInfo field)
        {
            RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(field, nameof(field));

            if (field.IsStatic)
            {
                throw Error.ConditionalAccessRequiresNonStaticMember();
            }

            var type = expression.Type.GetNonNullableType();

            if (!TypeUtils.AreReferenceAssignable(field.DeclaringType, type))
            {
                throw LinqError.FieldInfoNotDefinedForType(field.DeclaringType, field.Name, type);
            }

            return ConditionalMemberCSharpExpression.Make(expression, field);
        }

        /// <summary>
        /// Creates a <see cref="ConditionalMemberCSharpExpression" /> that represents a conditional (null-propagating) field lookup.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> that specifies the instance to access the member of.</param>
        /// <param name="fieldName">The name of the field to access conditionally.</param>
        /// <returns>A <see cref="ConditionalMemberCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalMemberAccess" />, the <see cref="ConditionalAccessCSharpExpression.Expression" /> property set to <paramref name="expression" />, and the <see cref="ConditionalMemberCSharpExpression.Member" /> property set to the <see cref="FieldInfo" /> that represents the field denoted by <paramref name="fieldName" />.</returns>
        public static ConditionalMemberCSharpExpression ConditionalField(Expression expression, string fieldName)
        {
            RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(fieldName, nameof(fieldName));

            var type = expression.Type.GetNonNullableType();

            var field = type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (field == null)
            {
                field = type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            }

            if (field == null)
            {
                throw LinqError.InstanceFieldNotDefinedForType(fieldName, type);
            }

            return ConditionalField(expression, field);
        }

        /// <summary>
        /// Creates a <see cref="ConditionalMemberCSharpExpression" /> that represents a conditional (null-propagating) property lookup.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> that specifies the instance to access the member of.</param>
        /// <param name="property">The <see cref="PropertyInfo" /> representing the property to access conditionally.</param>
        /// <returns>A <see cref="ConditionalMemberCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalMemberAccess" /> and the <see cref="ConditionalAccessCSharpExpression.Receiver" /> and <see cref="Microsoft.CSharp.Expressions.ConditionalMemberCSharpExpression.Member" /> properties set to the specified values.</returns>
        public static ConditionalMemberCSharpExpression ConditionalProperty(Expression expression, PropertyInfo property)
        {
            RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(property, nameof(property));

            if (!property.CanRead)
            {
                throw Error.ConditionalAccessRequiresReadableProperty();
            }

            if (property.GetGetMethod(true).IsStatic)
            {
                throw Error.ConditionalAccessRequiresNonStaticMember();
            }

            var type = expression.Type.GetNonNullableType();

            if (!TypeUtils.IsValidInstanceType(property, type))
            {
                throw LinqError.PropertyNotDefinedForType(property, type);
            }

            return ConditionalMemberCSharpExpression.Make(expression, property);
        }

        /// <summary>
        /// Creates a <see cref= "ConditionalMemberCSharpExpression" /> that represents a conditional (null-propagating) property lookup.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> whose <see cref="Expression.Type" /> contains a property named <paramref name="propertyName" />.</param>
        /// <param name="propertyName">The name of a property to be accessed.</param>
        /// <returns>A <see cref="ConditionalMemberCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalMemberAccess" />, the <see cref="ConditionalAccessCSharpExpression.Expression" /> property set to <paramref name="expression" />, and the <see cref="Member" /> property set to the <see cref="PropertyInfo" /> that represents the property denoted by <paramref name="propertyName" />.</returns>
        public static ConditionalMemberCSharpExpression ConditionalProperty(Expression expression, string propertyName)
        {
            RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(propertyName, nameof(propertyName));

            var type = expression.Type.GetNonNullableType();

            var property = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (property == null)
            {
                property = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            }

            if (property == null)
            {
                throw LinqError.InstancePropertyNotDefinedForType(propertyName, type);
            }

            return ConditionalProperty(expression, property);
        }

        /// <summary>
        /// Creates a <see cref= "ConditionalMemberCSharpExpression" /> that represents a conditional (null-propagating) property lookup by using a property accessor method.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to set the <see cref="Expression" /> property equal to. This can be null for static properties.</param>
        /// <param name="propertyAccessor">The <see cref="MethodInfo" /> that represents a property accessor method.</param>
        /// <returns>A <see cref="ConditionalMemberCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalMemberAccess" />, the <see cref="ConditionalAccessCSharpExpression.Expression" /> property set to <paramref name="expression" /> and the <see cref="Member" /> property set to the <see cref="PropertyInfo" /> that represents the property accessed in <paramref name="propertyAccessor" />.</returns>
        public static ConditionalMemberCSharpExpression ConditionalProperty(Expression expression, MethodInfo propertyAccessor)
        {
            ContractUtils.RequiresNotNull(propertyAccessor, nameof(propertyAccessor));

            ValidateMethodInfo(propertyAccessor);

            return ConditionalProperty(expression, GetProperty(propertyAccessor));
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ConditionalMemberCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitConditionalMember(ConditionalMemberCSharpExpression node)
        {
            return node.Update(Visit(node.Expression));
        }
    }
}
