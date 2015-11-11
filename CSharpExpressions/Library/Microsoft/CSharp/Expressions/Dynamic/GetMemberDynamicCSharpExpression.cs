// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static System.Linq.Expressions.ExpressionStubs;
using static Microsoft.CSharp.Expressions.Helpers;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a dynamically bound lookup of a member.
    /// </summary>
    public sealed class GetMemberDynamicCSharpExpression : DynamicCSharpExpression
    {
        internal GetMemberDynamicCSharpExpression(Type context, CSharpBinderFlags binderFlags, Expression @object, string name, ReadOnlyCollection<DynamicCSharpArgument> arguments)
            : base(context, binderFlags)
        {
            Object = @object;
            Name = name;
            Arguments = arguments;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.DynamicGetMember;

        /// <summary>
        /// Gets the expression representing the object to retrieve the member on.
        /// </summary>
        public Expression Object { get; }

        /// <summary>
        /// Gets the name of the member to lookup.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the arguments to pass to the invoked member (used for indexed properties).
        /// </summary>
        public ReadOnlyCollection<DynamicCSharpArgument> Arguments { get; }

        /// <summary>
        /// Reduces the dynamic expression to a binder and a set of arguments to apply the operation to.
        /// </summary>
        /// <param name="binder">The binder used to perform the dynamic operation.</param>
        /// <param name="arguments">The arguments to apply the dynamic operation to.</param>
        protected override void ReduceDynamic(out CallSiteBinder binder, out IEnumerable<Expression> arguments)
        {
            var n = Arguments.Count;

            var argumentInfos = new CSharpArgumentInfo[n + 1];
            var expressions = new Expression[n + 1];

            expressions[0] = Object;
            argumentInfos[0] = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);

            CopyArguments(Arguments, argumentInfos, expressions);

            binder = Binder.GetMember(Flags, Name, Context, argumentInfos);
            arguments = expressions;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitDynamicGetMember(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="object">The <see cref="Object" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public GetMemberDynamicCSharpExpression Update(Expression @object, IEnumerable<DynamicCSharpArgument> arguments)
        {
            if (@object == this.Object && arguments == this.Arguments)
            {
                return this;
            }

            return DynamicCSharpExpression.DynamicGetMember(@object, Name, arguments, Flags, Context);
        }
    }

    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates a new expression representing a dynamically bound member lookup.
        /// </summary>
        /// <param name="object">The expression representing the object to retrieve the member on.</param>
        /// <param name="name">The name of the member to lookup.</param>
        /// <param name="arguments">An array of expressions representing the arguments passed to the member upon lookup.</param>
        /// <returns>A new expression representing a dynamically bound member lookup.</returns>
        public static GetMemberDynamicCSharpExpression DynamicGetMember(Expression @object, string name, params Expression[] arguments)
        {
            return DynamicGetMember(@object, name, GetDynamicArguments(arguments), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound member lookup.
        /// </summary>
        /// <param name="object">The expression representing the object to retrieve the member on.</param>
        /// <param name="name">The name of the member to lookup.</param>
        /// <param name="arguments">An enumerable sequence of expressions representing the arguments passed to the member upon lookup.</param>
        /// <returns>A new expression representing a dynamically bound member lookup.</returns>
        public static GetMemberDynamicCSharpExpression DynamicGetMember(Expression @object, string name, IEnumerable<Expression> arguments)
        {
            return DynamicGetMember(@object, name, GetDynamicArguments(arguments), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound member lookup.
        /// </summary>
        /// <param name="object">The expression representing the object to retrieve the member on.</param>
        /// <param name="name">The name of the member to lookup.</param>
        /// <param name="arguments">An array of dynamic arguments representing the arguments passed to the member upon lookup.</param>
        /// <returns>A new expression representing a dynamically bound member lookup.</returns>
        public static GetMemberDynamicCSharpExpression DynamicGetMember(Expression @object, string name, DynamicCSharpArgument[] arguments)
        {
            return DynamicGetMember(@object, name, arguments, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound member lookup.
        /// </summary>
        /// <param name="object">The expression representing the object to retrieve the member on.</param>
        /// <param name="name">The name of the member to lookup.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the member upon lookup.</param>
        /// <returns>A new expression representing a dynamically bound member lookup.</returns>
        public static GetMemberDynamicCSharpExpression DynamicGetMember(Expression @object, string name, IEnumerable<DynamicCSharpArgument> arguments)
        {
            return DynamicGetMember(@object, name, arguments, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound member lookup with the specified binder flags.
        /// </summary>
        /// <param name="object">The expression representing the object to retrieve the member on.</param>
        /// <param name="name">The name of the member to lookup.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the member upon lookup.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound member lookup.</returns>
        public static GetMemberDynamicCSharpExpression DynamicGetMember(Expression @object, string name, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags)
        {
            return DynamicGetMember(@object, name, arguments, binderFlags, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound member lookup with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="object">The expression representing the object to retrieve the member on.</param>
        /// <param name="name">The name of the member to lookup.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the member upon lookup.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound member lookup.</returns>
        public static GetMemberDynamicCSharpExpression DynamicGetMember(Expression @object, string name, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags, Type context)
        {
            RequiresCanRead(@object, nameof(@object));
            ContractUtils.RequiresNotNull(name, nameof(name));

            var argList = arguments.ToReadOnly();

            return new GetMemberDynamicCSharpExpression(context, binderFlags, @object, name, argList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="GetMemberDynamicCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDynamicGetMember(GetMemberDynamicCSharpExpression node)
        {
            return node.Update(Visit(node.Object), Visit(node.Arguments, VisitDynamicArgument));
        }
    }
}
