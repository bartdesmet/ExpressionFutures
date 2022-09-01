// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a with expression.
    /// </summary>
    public sealed partial class WithCSharpExpression : CSharpExpression
    {
        internal WithCSharpExpression(Expression @object, ReadOnlyCollection<MemberInitializer> initializers, MethodInfo clone, ReadOnlyCollection<MemberInfo> members)
        {
            Object = @object;
            Initializers = initializers;
            Clone = clone;
            Members = members;
        }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the object to clone and mutate.
        /// </summary>
        public Expression Object { get; }

        /// <summary>
        /// Gets a collection of member initializers.
        /// </summary>
        public ReadOnlyCollection<MemberInitializer> Initializers { get; }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> representing the method used to clone the object.
        /// </summary>
        public MethodInfo Clone { get; }

        /// <summary>
        /// Gets a collection of members corresponding to the constructor arguments if <see cref="Type"/> is an anonymous type.
        /// </summary>
        public ReadOnlyCollection<MemberInfo> Members { get; }

        /// <summary>
        /// Gets the static type of the expression.
        /// </summary>
        public override Type Type => Object.Type;

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.With;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitWith(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="object">The <see cref="Object" /> property of the result.</param>
        /// <param name="initializers">The <see cref="Initializers" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public WithCSharpExpression Update(Expression @object, IEnumerable<MemberInitializer> initializers)
        {
            if (@object == Object && SameElements(ref initializers, Initializers))
            {
                return this;
            }

            return Members != null
                ? CSharpExpression.With(@object, Members, initializers)
                : CSharpExpression.With(@object, Clone, initializers);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            if (Members != null)
            {
                return ReduceAnonymousType();
            }
            else if (Object.Type.IsValueType)
            {
                return ReduceValueType();
            }
            else
            {
                return ReduceReferenceType();
            }
        }

        private Expression ReduceAnonymousType()
        {
            var initCount = Initializers.Count;

            var vars = new List<ParameterExpression>(initCount + 1);
            var stmts = new List<Expression>(initCount + 2);

            var obj = Expression.Variable(Object.Type, "__obj");

            vars.Add(obj);
            stmts.Add(Expression.Assign(obj, Object));

            var memberExpressions = new Dictionary<MemberInfo, Expression>();

            for (var i = 0; i < initCount; i++)
            {
                var initializer = Initializers[i];
                var expr = initializer.Expression;

                if (!expr.IsPure())
                {
                    var tmp = Expression.Variable(expr.Type, "__init" + i);

                    vars.Add(tmp);
                    stmts.Add(Expression.Assign(tmp, expr));

                    expr = tmp;
                }

                //
                // NB: If there are duplicate assignments, the last one wins. We do not check for uniqueness,
                //     which is similar to MemberInitExpression.
                //

                memberExpressions[initializer.Member] = expr;
            }

            var memberCount = Members.Count;

            var args = new Expression[memberCount];
            var memberTypes = new Type[memberCount];

            for (var i = 0; i < memberCount; i++)
            {
                var member = Members[i];

                memberTypes[i] = member is PropertyInfo p ? p.PropertyType : ((FieldInfo)member).FieldType;

                if (memberExpressions.TryGetValue(member, out var expr))
                {
                    args[i] = expr;
                }
                else
                {
                    args[i] = Expression.MakeMemberAccess(obj, member);
                }
            }

            var ctor = Object.Type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, binder: null, memberTypes, modifiers: null);

            stmts.Add(Expression.New(ctor, args, Members));

            return Helpers.Comma(vars, stmts);
        }

        private Expression ReduceValueType() => ReduceWithClone(Object);

        private Expression ReduceReferenceType() => ReduceWithClone(Expression.Call(Object, Clone));

        private Expression ReduceWithClone(Expression clone)
        {
            var stmts = new List<Expression>(Initializers.Count + 2);

            var obj = Expression.Variable(Object.Type, "__obj");

            stmts.Add(Expression.Assign(obj, clone));

            ReduceMemberInitializers(obj, stmts);

            stmts.Add(obj);

            return Helpers.Comma(new List<ParameterExpression>(1) { obj }, stmts);
        }

        private void ReduceMemberInitializers(ParameterExpression obj, List<Expression> stmts)
        {
            foreach (var initializer in Initializers)
            {
                stmts.Add(Expression.Assign(Expression.MakeMemberAccess(obj, initializer.Member), initializer.Expression));
            }
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="WithCSharpExpression"/> that represents a with expression.
        /// </summary>
        /// <param name="object">The expression representing the object to clone and mutate.</param>
        /// <param name="initializers">The initializers used to mutate the cloned object.</param>
        /// <returns>The created <see cref="WithCSharpExpression"/>.</returns>
        public static WithCSharpExpression With(Expression @object, params MemberInitializer[] initializers) => With(@object, clone: null, initializers);

        /// <summary>
        /// Creates a <see cref="WithCSharpExpression"/> that represents a with expression.
        /// </summary>
        /// <param name="object">The expression representing the object to clone and mutate.</param>
        /// <param name="initializers">The initializers used to mutate the cloned object.</param>
        /// <returns>The created <see cref="WithCSharpExpression"/>.</returns>
        public static WithCSharpExpression With(Expression @object, IEnumerable<MemberInitializer> initializers) => With(@object, clone: null, initializers);

        /// <summary>
        /// Creates a <see cref="WithCSharpExpression"/> that represents a with expression.
        /// </summary>
        /// <param name="object">The expression representing the object to clone and mutate.</param>
        /// <param name="clone">The method used to clone the object.</param>
        /// <param name="initializers">The initializers used to mutate the cloned object.</param>
        /// <returns>The created <see cref="WithCSharpExpression"/>.</returns>
        public static WithCSharpExpression With(Expression @object, MethodInfo clone, params MemberInitializer[] initializers) => With(@object, clone, (IEnumerable<MemberInitializer>)initializers);

        /// <summary>
        /// Creates a <see cref="WithCSharpExpression"/> that represents a with expression.
        /// </summary>
        /// <param name="object">The expression representing the object to clone and mutate.</param>
        /// <param name="clone">The method used to clone the object.</param>
        /// <param name="initializers">The initializers used to mutate the cloned object.</param>
        /// <returns>The created <see cref="WithCSharpExpression"/>.</returns>
        public static WithCSharpExpression With(Expression @object, MethodInfo clone, IEnumerable<MemberInitializer> initializers)
        {
            RequiresNotNull(@object, nameof(@object));
            RequiresNotNull(initializers, nameof(initializers));

            var initializersCollection = initializers.ToReadOnly();

            ValidateWithReceiverAndInitializers(@object, initializersCollection, requiresCanAssign: true);

            if (@object.Type.IsValueType)
            {
                if (clone != null)
                    throw Error.WithExpressionCannotHaveCloneForValueType(@object.Type);
            }
            else
            {
                clone ??= @object.Type.GetNonGenericMethod("Clone", BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes);

                if (clone == null)
                    throw Error.WithExpressionShouldHaveClone(@object.Type);

                ValidateMethodInfo(clone, nameof(clone));

                if (clone.IsStatic)
                    throw Error.CloneMethodMustNotBeStatic(clone.Name);

                if (!clone.DeclaringType.IsAssignableFrom(@object.Type))
                    throw NotAMemberOfType(clone.Name, @object.Type, nameof(clone));

                if (clone.GetParametersCached().Length != 0)
                    throw Error.CloneMethodShouldHaveNoParameters(clone.Name);

                if (!clone.ReturnType.HasReferenceConversionTo(@object.Type))
                    throw Error.CloneMethodShouldReturnCompatibleType(clone.Name, @object.Type);
            }

            return new WithCSharpExpression(@object, initializersCollection, clone, members: null);
        }

        /// <summary>
        /// Creates a <see cref="WithCSharpExpression"/> that represents a with expression applied to an anonymous type.
        /// </summary>
        /// <param name="object">The expression representing the object to clone and mutate.</param>
        /// <param name="members">The members of the anonymous type in the order of their assignment by the constructor parameters.</param>
        /// <param name="initializers">The initializers used to mutate the cloned object.</param>
        /// <returns>The created <see cref="WithCSharpExpression"/>.</returns>
        public static WithCSharpExpression With(Expression @object, IEnumerable<MemberInfo> members, params MemberInitializer[] initializers) => With(@object, members, (IEnumerable<MemberInitializer>)initializers);

        /// <summary>
        /// Creates a <see cref="WithCSharpExpression"/> that represents a with expression applied to an anonymous type.
        /// </summary>
        /// <param name="object">The expression representing the object to clone and mutate.</param>
        /// <param name="members">The members of the anonymous type in the order of their assignment by the constructor parameters.</param>
        /// <param name="initializers">The initializers used to mutate the cloned object.</param>
        /// <returns>The created <see cref="WithCSharpExpression"/>.</returns>
        public static WithCSharpExpression With(Expression @object, IEnumerable<MemberInfo> members, IEnumerable<MemberInitializer> initializers)
        {
            RequiresNotNull(@object, nameof(@object));
            RequiresNotNull(members, nameof(members));
            RequiresNotNull(initializers, nameof(initializers));

            var membersCollection = members.ToReadOnly();
            var initializersCollection = initializers.ToReadOnly();

            ValidateWithReceiverAndInitializers(@object, initializersCollection, requiresCanAssign: false);

            var membersCount = membersCollection.Count;

            var newMembers = new MemberInfo[membersCount];
            var memberTypes = new Type[membersCount];

            for (var i = 0; i < membersCount; i++)
            {
                var member = membersCollection[i];

                RequiresNotNull(member, nameof(members), i);

                if (!AreEquivalent(member.DeclaringType, @object.Type))
                    throw ArgumentMemberNotDeclOnType(member.Name, @object.Type.Name, nameof(members), i);

                ValidateAnonymousTypeMember(ref member, out var memberType, nameof(members), i);

                newMembers[i] = member;
                memberTypes[i] = memberType;
            }

            membersCollection = newMembers.ToReadOnly();

            var ctor = @object.Type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, binder: null, memberTypes, modifiers: null);

            if (ctor == null)
                throw Error.NoAnonymousTypeConstructorFound(@object.Type);

            return new WithCSharpExpression(@object, initializersCollection, clone: null, membersCollection);
        }

        private static void ValidateWithReceiverAndInitializers(Expression @object, ReadOnlyCollection<MemberInitializer> initializers, bool requiresCanAssign)
        {
            RequiresCanRead(@object, nameof(@object));

            for (int i = 0, count = initializers.Count; i < count; i++)
            {
                var initializer = initializers[i];

                RequiresNotNull(initializer, nameof(initializers), i);

                var member = initializer.Member;

                if (!member.DeclaringType.IsAssignableFrom(@object.Type))
                    throw NotAMemberOfType(member.Name, @object.Type, nameof(initializers));

                if (requiresCanAssign)
                {
                    switch (member)
                    {
                        case FieldInfo f:
                            if (f.IsInitOnly || f.IsLiteral)
                                throw Error.MemberInitializerMemberMustBeWriteable(member.Name);

                            break;
                        case PropertyInfo p:
                            if (!p.CanWrite)
                                throw Error.MemberInitializerMemberMustBeWriteable(member.Name);

                            break;
                    }
                }
            }
        }

        private static void ValidateAnonymousTypeMember(ref MemberInfo member, out Type memberType, string paramName, int index)
        {
            if (member is FieldInfo field)
            {
                if (field.IsStatic)
                {
                    throw ArgumentMustBeInstanceMember(paramName, index);
                }
                memberType = field.FieldType;
                return;
            }

            if (member is PropertyInfo pi)
            {
                if (!pi.CanRead)
                {
                    throw PropertyDoesNotHaveGetter(pi, paramName, index);
                }
                if (pi.GetGetMethod()!.IsStatic)
                {
                    throw ArgumentMustBeInstanceMember(paramName, index);
                }
                memberType = pi.PropertyType;
                return;
            }

            if (member is MethodInfo method)
            {
                if (method.IsStatic)
                {
                    throw ArgumentMustBeInstanceMember(paramName, index);
                }

                PropertyInfo prop = GetProperty(method, paramName, index);
                member = prop;
                memberType = prop.PropertyType;
                return;
            }
            throw ArgumentMustBeFieldInfoOrPropertyInfoOrMethod(paramName, index);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="WithCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitWith(WithCSharpExpression node) =>
            node.Update(
                Visit(node.Object),
                Visit(node.Initializers, VisitMemberInitializer)
            );
    }
}
