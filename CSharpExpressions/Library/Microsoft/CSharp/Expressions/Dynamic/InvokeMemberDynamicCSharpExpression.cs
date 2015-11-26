// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;
using static Microsoft.CSharp.Expressions.Helpers;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a dynamically bound invocation of a member.
    /// </summary>
    public abstract partial class InvokeMemberDynamicCSharpExpression : DynamicCSharpExpression
    {
        internal InvokeMemberDynamicCSharpExpression(Type context, CSharpBinderFlags binderFlags, string name, ReadOnlyCollection<Type> typeArguments, ReadOnlyCollection<DynamicCSharpArgument> arguments)
            : base(context, binderFlags)
        {
            Name = name;
            TypeArguments = typeArguments;
            Arguments = arguments;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.DynamicInvokeMember;

        /// <summary>
        /// Gets the expression representing the object to invoke the member on. (Or null for static members.)
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Object", Justification = "Analogous to various nodes in LINQ.")]
        public virtual Expression Object => null;

        /// <summary>
        /// Gets the type to invoke the member on. (Or null for instance members.)
        /// </summary>
        public virtual Type Target => null;

        /// <summary>
        /// Gets the name of the member to invoke.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type arguments to pass to the invocation of the member.
        /// </summary>
        public ReadOnlyCollection<Type> TypeArguments { get; }

        /// <summary>
        /// Gets the arguments to pass to the invoked member.
        /// </summary>
        public ReadOnlyCollection<DynamicCSharpArgument> Arguments { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitDynamicInvokeMember(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="object">The <see cref="Object" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "object", Justification = "Analogous to some nodes in LINQ.")]
        public abstract InvokeMemberDynamicCSharpExpression Update(Expression @object, IEnumerable<DynamicCSharpArgument> arguments);
    }

    internal class InvokeInstanceMemberDynamicCSharpExpression : InvokeMemberDynamicCSharpExpression
    {
        internal InvokeInstanceMemberDynamicCSharpExpression(Type context, CSharpBinderFlags binderFlags, Expression @object, string name, ReadOnlyCollection<Type> typeArguments, ReadOnlyCollection<DynamicCSharpArgument> arguments)
            : base(context, binderFlags, name, typeArguments, arguments)
        {
            Object = @object;
        }

        public override Expression Object { get; }

        protected override void ReduceDynamic(out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[] argumentTypes)
        {
            var n = Arguments.Count;

            var argumentInfos = new CSharpArgumentInfo[n + 1];
            var expressions = new Expression[n + 1];

            argumentTypes = null;
            CopyReceiverArgument(Object, argumentInfos, expressions, ref argumentTypes);
            CopyArguments(Arguments, argumentInfos, expressions, ref argumentTypes);

            binder = Binder.InvokeMember(Flags, Name, TypeArguments, Context, argumentInfos);
            arguments = expressions;
        }

        public override InvokeMemberDynamicCSharpExpression Update(Expression @object, IEnumerable<DynamicCSharpArgument> arguments)
        {
            if (@object == this.Object && arguments == this.Arguments)
            {
                return this;
            }

            return DynamicCSharpExpression.DynamicInvokeMember(@object, Name, TypeArguments, arguments, Flags, Context);
        }
    }

    internal class InvokeStaticMemberDynamicCSharpExpression : InvokeMemberDynamicCSharpExpression
    {
        internal InvokeStaticMemberDynamicCSharpExpression(Type context, CSharpBinderFlags binderFlags, Type type, string name, ReadOnlyCollection<Type> typeArguments, ReadOnlyCollection<DynamicCSharpArgument> arguments)
            : base(context, binderFlags, name, typeArguments, arguments)
        {
            Target = type;
        }

        public override Type Target { get; }

        protected override void ReduceDynamic(out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[] argumentTypes)
        {
            var n = Arguments.Count;

            var argumentInfos = new CSharpArgumentInfo[n + 1];
            var expressions = new Expression[n + 1];

            expressions[0] = Expression.Constant(Target, typeof(Type));
            argumentInfos[0] = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null);

            argumentTypes = null;
            CopyArguments(Arguments, argumentInfos, expressions, ref argumentTypes);

            binder = Binder.InvokeMember(Flags, Name, TypeArguments, Context, argumentInfos);
            arguments = expressions;
        }

        public override InvokeMemberDynamicCSharpExpression Update(Expression @object, IEnumerable<DynamicCSharpArgument> arguments)
        {
            if (@object == this.Object && arguments == this.Arguments)
            {
                return this;
            }

            return DynamicCSharpExpression.DynamicInvokeMember(Target, Name, TypeArguments, arguments, Flags, Context);
        }
    }

    partial class DynamicCSharpExpression
    {
        // TODO: Rationalize overload hell

        /// <summary>
        /// Creates a new expression representing a dynamically bound member invocation.
        /// </summary>
        /// <param name="object">The expression representing the object to invoke the member on.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="arguments">An array of expressions representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Expression @object, string name, params Expression[] arguments)
        {
            return DynamicInvokeMember(@object, name, null, GetDynamicArguments(arguments), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound member invocation.
        /// </summary>
        /// <param name="object">The expression representing the object to invoke the member on.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="arguments">An enumerable sequence of expressions representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Expression @object, string name, IEnumerable<Expression> arguments)
        {
            return DynamicInvokeMember(@object, name, null, GetDynamicArguments(arguments), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound member invocation.
        /// </summary>
        /// <param name="object">The expression representing the object to invoke the member on.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="arguments">An array of dynamic arguments representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Expression @object, string name, DynamicCSharpArgument[] arguments)
        {
            return DynamicInvokeMember(@object, name, null, arguments, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound member invocation.
        /// </summary>
        /// <param name="object">The expression representing the object to invoke the member on.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Expression @object, string name, IEnumerable<DynamicCSharpArgument> arguments)
        {
            return DynamicInvokeMember(@object, name, null, arguments, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound (possibly generic) member invocation.
        /// </summary>
        /// <param name="object">The expression representing the object to invoke the member on.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">An enumerable sequence of type arguments to pass to the generic member. (Specify null for non-generic members.)</param>
        /// <param name="arguments">An array of expressions representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Expression @object, string name, IEnumerable<Type> typeArguments, params Expression[] arguments)
        {
            return DynamicInvokeMember(@object, name, typeArguments, GetDynamicArguments(arguments), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound (possibly generic) member invocation.
        /// </summary>
        /// <param name="object">The expression representing the object to invoke the member on.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">An enumerable sequence of type arguments to pass to the generic member. (Specify null for non-generic members.)</param>
        /// <param name="arguments">An enumerable sequence of expressions representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Expression @object, string name, IEnumerable<Type> typeArguments, IEnumerable<Expression> arguments)
        {
            return DynamicInvokeMember(@object, name, typeArguments, GetDynamicArguments(arguments), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound (possibly generic) member invocation.
        /// </summary>
        /// <param name="object">The expression representing the object to invoke the member on.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">An enumerable sequence of type arguments to pass to the generic member. (Specify null for non-generic members.)</param>
        /// <param name="arguments">An array of dynamic arguments representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Expression @object, string name, IEnumerable<Type> typeArguments, DynamicCSharpArgument[] arguments)
        {
            return DynamicInvokeMember(@object, name, typeArguments, arguments, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound (possibly generic) member invocation.
        /// </summary>
        /// <param name="object">The expression representing the object to invoke the member on.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">An enumerable sequence of type arguments to pass to the generic member. (Specify null for non-generic members.)</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Expression @object, string name, IEnumerable<Type> typeArguments, IEnumerable<DynamicCSharpArgument> arguments)
        {
            return DynamicInvokeMember(@object, name, typeArguments, arguments, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound (possibly generic) member invocation with the specified binder flags.
        /// </summary>
        /// <param name="object">The expression representing the object to invoke the member on.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">An enumerable sequence of type arguments to pass to the generic member. (Specify null for non-generic members.)</param>
        /// <param name="arguments">An array of dynamic arguments representing the arguments passed to the member upon invocation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Expression @object, string name, IEnumerable<Type> typeArguments, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags)
        {
            return DynamicInvokeMember(@object, name, typeArguments, arguments, binderFlags, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound (possibly generic) member invocation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="object">The expression representing the object to invoke the member on.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">An enumerable sequence of type arguments to pass to the generic member. (Specify null for non-generic members.)</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the member upon invocation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Expression @object, string name, IEnumerable<Type> typeArguments, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags, Type context)
        {
            RequiresCanRead(@object, nameof(@object));
            ContractUtils.RequiresNotNull(name, nameof(name));

            var typeArgList = typeArguments.ToReadOnly();

            foreach (var type in typeArgList)
            {
                ValidateType(type);
            }

            var argList = arguments.ToReadOnly();

            return new InvokeInstanceMemberDynamicCSharpExpression(context, binderFlags, @object, name, typeArgList, argList);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound member invocation.
        /// </summary>
        /// <param name="type">The type containing the static member to invoke.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="arguments">An array of expressions representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Type type, string name, params Expression[] arguments)
        {
            return DynamicInvokeMember(type, name, null, GetDynamicArguments(arguments), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound member invocation.
        /// </summary>
        /// <param name="type">The type containing the static member to invoke.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="arguments">An enumerable sequence of expressions representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Type type, string name, IEnumerable<Expression> arguments)
        {
            return DynamicInvokeMember(type, name, null, GetDynamicArguments(arguments), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound member invocation.
        /// </summary>
        /// <param name="type">The type containing the static member to invoke.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="arguments">An array of dynamic arguments representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Type type, string name, DynamicCSharpArgument[] arguments)
        {
            return DynamicInvokeMember(type, name, null, arguments, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound member invocation.
        /// </summary>
        /// <param name="type">The type containing the static member to invoke.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Type type, string name, IEnumerable<DynamicCSharpArgument> arguments)
        {
            return DynamicInvokeMember(type, name, null, arguments, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound (possibly generic) member invocation.
        /// </summary>
        /// <param name="type">The type containing the static member to invoke.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">An enumerable sequence of type arguments to pass to the generic member. (Specify null for non-generic members.)</param>
        /// <param name="arguments">An array of expressions representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Type type, string name, IEnumerable<Type> typeArguments, params Expression[] arguments)
        {
            return DynamicInvokeMember(type, name, typeArguments, GetDynamicArguments(arguments), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound (possibly generic) member invocation.
        /// </summary>
        /// <param name="type">The type containing the static member to invoke.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">An enumerable sequence of type arguments to pass to the generic member. (Specify null for non-generic members.)</param>
        /// <param name="arguments">An enumerable sequence of expressions representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Type type, string name, IEnumerable<Type> typeArguments, IEnumerable<Expression> arguments)
        {
            return DynamicInvokeMember(type, name, typeArguments, GetDynamicArguments(arguments), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound (possibly generic) member invocation.
        /// </summary>
        /// <param name="type">The type containing the static member to invoke.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">An enumerable sequence of type arguments to pass to the generic member. (Specify null for non-generic members.)</param>
        /// <param name="arguments">An array of dynamic arguments representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Type type, string name, IEnumerable<Type> typeArguments, DynamicCSharpArgument[] arguments)
        {
            return DynamicInvokeMember(type, name, typeArguments, arguments, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound (possibly generic) member invocation.
        /// </summary>
        /// <param name="type">The type containing the static member to invoke.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">An enumerable sequence of type arguments to pass to the generic member. (Specify null for non-generic members.)</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the member upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Type type, string name, IEnumerable<Type> typeArguments, IEnumerable<DynamicCSharpArgument> arguments)
        {
            return DynamicInvokeMember(type, name, typeArguments, arguments, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound (possibly generic) member invocation with the specified binder flags.
        /// </summary>
        /// <param name="type">The type containing the static member to invoke.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">An enumerable sequence of type arguments to pass to the generic member. (Specify null for non-generic members.)</param>
        /// <param name="arguments">An array of dynamic arguments representing the arguments passed to the member upon invocation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Type type, string name, IEnumerable<Type> typeArguments, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags)
        {
            return DynamicInvokeMember(type, name, typeArguments, arguments, binderFlags, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound (possibly generic) member invocation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="type">The type containing the static member to invoke.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">An enumerable sequence of type arguments to pass to the generic member. (Specify null for non-generic members.)</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the member upon invocation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound member invocation.</returns>
        public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Type type, string name, IEnumerable<Type> typeArguments, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags, Type context)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));
            ContractUtils.RequiresNotNull(name, nameof(name));

            ValidateType(type);

            var typeArgList = typeArguments.ToReadOnly();

            foreach (var typeArg in typeArgList)
            {
                ValidateType(typeArg);
            }

            var argList = arguments.ToReadOnly();

            return new InvokeStaticMemberDynamicCSharpExpression(context, binderFlags, type, name, typeArgList, argList);
        }

        private static ReadOnlyCollection<DynamicCSharpArgument> GetDynamicArguments(IEnumerable<Expression> arguments)
        {
            return arguments.Select(DynamicArgument).ToReadOnly();
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="InvokeMemberDynamicCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDynamicInvokeMember(InvokeMemberDynamicCSharpExpression node)
        {
            return node.Update(Visit(node.Object), Visit(node.Arguments, VisitDynamicArgument));
        }
    }
}
