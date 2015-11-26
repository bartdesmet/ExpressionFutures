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
    /// Represents a dynamically bound invocation of a delegate or lambda.
    /// </summary>
    public sealed partial class InvokeDynamicCSharpExpression : DynamicCSharpExpression
    {
        internal InvokeDynamicCSharpExpression(Type context, CSharpBinderFlags binderFlags, Expression @object, ReadOnlyCollection<DynamicCSharpArgument> arguments)
            : base(context, binderFlags)
        {
            Expression = @object;
            Arguments = arguments;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.DynamicInvoke;

        /// <summary>
        /// Gets the expression representing the delegate or lambda to invoke.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Gets the arguments to pass to the lambda or delegate invocation.
        /// </summary>
        public ReadOnlyCollection<DynamicCSharpArgument> Arguments { get; }

        /// <summary>
        /// Reduces the dynamic expression to a binder and a set of arguments to apply the operation to.
        /// </summary>
        /// <param name="binder">The binder used to perform the dynamic operation.</param>
        /// <param name="arguments">The arguments to apply the dynamic operation to.</param>
        /// <param name="argumentTypes">The types of the arguments to use for the dynamic call site. Return null to infer types.</param>
        protected override void ReduceDynamic(out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[] argumentTypes)
        {
            var n = Arguments.Count;

            var argumentInfos = new CSharpArgumentInfo[n + 1];
            var expressions = new Expression[n + 1];

            expressions[0] = Expression;
            argumentInfos[0] = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);

            argumentTypes = null;
            CopyArguments(Arguments, argumentInfos, expressions, ref argumentTypes);

            binder = Binder.Invoke(Flags, Context, argumentInfos);
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
            return visitor.VisitDynamicInvoke(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public InvokeDynamicCSharpExpression Update(Expression expression, IEnumerable<DynamicCSharpArgument> arguments)
        {
            if (expression == this.Expression && arguments == this.Arguments)
            {
                return this;
            }

            return DynamicCSharpExpression.DynamicInvoke(expression, arguments, Flags, Context);
        }
    }

    partial class DynamicCSharpExpression
    {
        // TODO: Rationalize overload hell

        /// <summary>
        /// Creates a new expression representing a dynamically lambda or delegate invocation.
        /// </summary>
        /// <param name="expression">The expression representing the lambda or delegate to invoke.</param>
        /// <param name="arguments">An array of expressions representing the arguments passed to the lambda or delegate upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound lambda or delegate invocation.</returns>
        public static InvokeDynamicCSharpExpression DynamicInvoke(Expression expression, params Expression[] arguments)
        {
            return DynamicInvoke(expression, GetDynamicArguments(arguments), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically lambda or delegate invocation.
        /// </summary>
        /// <param name="expression">The expression representing the lambda or delegate to invoke.</param>
        /// <param name="arguments">An enumerable sequence of expressions representing the arguments passed to the lambda or delegate upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound lambda or delegate invocation.</returns>
        public static InvokeDynamicCSharpExpression DynamicInvoke(Expression expression, IEnumerable<Expression> arguments)
        {
            return DynamicInvoke(expression, GetDynamicArguments(arguments), CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically lambda or delegate invocation.
        /// </summary>
        /// <param name="expression">The expression representing the lambda or delegate to invoke.</param>
        /// <param name="arguments">An array of dynamic arguments representing the arguments passed to the lambda or delegate upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound lambda or delegate invocation.</returns>
        public static InvokeDynamicCSharpExpression DynamicInvoke(Expression expression, DynamicCSharpArgument[] arguments)
        {
            return DynamicInvoke(expression, arguments, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically lambda or delegate invocation.
        /// </summary>
        /// <param name="expression">The expression representing the lambda or delegate to invoke.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the lambda or delegate upon invocation.</param>
        /// <returns>A new expression representing a dynamically bound lambda or delegate invocation.</returns>
        public static InvokeDynamicCSharpExpression DynamicInvoke(Expression expression, IEnumerable<DynamicCSharpArgument> arguments)
        {
            return DynamicInvoke(expression, arguments, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically lambda or delegate invocation with the specified binder flags.
        /// </summary>
        /// <param name="expression">The expression representing the lambda or delegate to invoke.</param>
        /// <param name="arguments">An array of dynamic arguments representing the arguments passed to the lambda or delegate upon invocation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound lambda or delegate invocation.</returns>
        public static InvokeDynamicCSharpExpression DynamicInvoke(Expression expression, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags)
        {
            return DynamicInvoke(expression, arguments, binderFlags, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically lambda or delegate invocation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="expression">The expression representing the lambda or delegate to invoke.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the lambda or delegate upon invocation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound lambda or delegate invocation.</returns>
        public static InvokeDynamicCSharpExpression DynamicInvoke(Expression expression, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags, Type context)
        {
            RequiresCanRead(expression, nameof(expression));

            var argList = arguments.ToReadOnly();

            return new InvokeDynamicCSharpExpression(context, binderFlags, expression, argList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="InvokeDynamicCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDynamicInvoke(InvokeDynamicCSharpExpression node)
        {
            return node.Update(Visit(node.Expression), Visit(node.Arguments, VisitDynamicArgument));
        }
    }
}
