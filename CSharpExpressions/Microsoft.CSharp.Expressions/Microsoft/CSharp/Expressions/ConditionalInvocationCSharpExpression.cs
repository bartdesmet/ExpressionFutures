// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.ExpressionStubs;
using static Microsoft.CSharp.Expressions.Helpers;

namespace Microsoft.CSharp.Expressions
{
    // DESIGN: C# doesn't have a conditional invocation operator due to syntactic issues that prohibited its introduction;
    //         however, we don't have to keep this limitation at the expression level. Should the compiler emit this node
    //         type for conditional method calls to the Invoke method?

    /// <summary>
    /// Represents an expression that applies a delegate or lambda expression to a list of argument expressions.
    /// </summary>
#if OLD_CONDITIONAL
    public sealed partial class ConditionalInvocationCSharpExpression : OldConditionalAccessCSharpExpression
    {
        private readonly MethodInfo _invokeMethod;

        internal ConditionalInvocationCSharpExpression(Expression expression, ReadOnlyCollection<ParameterAssignment> arguments, MethodInfo invokeMethod)
            : base(expression)
        {
            Arguments = arguments;
            _invokeMethod = invokeMethod;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.ConditionalInvoke;

        /// <summary>
        /// Gets the result type of the underlying access.
        /// </summary>
        protected override Type UnderlyingType => _invokeMethod.ReturnType;

        /// <summary>
        /// Gets a collection of argument assignments.
        /// </summary>
        public ReadOnlyCollection<ParameterAssignment> Arguments { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitConditionalInvocation(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ConditionalInvocationCSharpExpression Update(Expression expression, IEnumerable<ParameterAssignment> arguments)
        {
            if (expression == Expression && arguments == Arguments)
            {
                return this;
            }

            return CSharpExpression.ConditionalInvoke(expression, arguments);
        }

        /// <summary>
        /// Reduces the expression to an unconditional non-null access on the specified expression.
        /// </summary>
        /// <param name="nonNull">Non-null expression to apply the access to.</param>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceAccess(Expression nonNull) => CSharpExpression.Invoke(nonNull, Arguments);
    }
#else
    public sealed partial class ConditionalInvocationCSharpExpression : ConditionalAccessCSharpExpression<InvocationCSharpExpression>
    {
        internal ConditionalInvocationCSharpExpression(Expression expression, ReadOnlyCollection<ParameterAssignment> arguments, MethodInfo invokeMethod)
            : this(expression, MakeReceiver(expression), arguments)
        {
        }

        private ConditionalInvocationCSharpExpression(Expression expression, ConditionalReceiver receiver, ReadOnlyCollection<ParameterAssignment> arguments)
            : base(expression, receiver, MakeAccess(receiver, arguments))
        {
        }

        private static InvocationCSharpExpression MakeAccess(ConditionalReceiver receiver, ReadOnlyCollection<ParameterAssignment> arguments)
        {
            return CSharpExpression.Invoke(receiver, arguments); // TODO: call ctor directly
        }

        /// <summary>
        /// Gets the <see cref="Expression" /> that represents the instance for instance method calls or null for static method calls.
        /// </summary>
        public Expression Expression => Receiver;

        /// <summary>
        /// Gets a collection of argument assignments.
        /// </summary>
        public ReadOnlyCollection<ParameterAssignment> Arguments => WhenNotNull.Arguments;

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ConditionalInvocationCSharpExpression Update(Expression expression, IEnumerable<ParameterAssignment> arguments)
        {
            if (expression == Expression && arguments == Arguments)
            {
                return this;
            }

            return CSharpExpression.ConditionalInvoke(expression, arguments);
        }

        // TODO: Rewrite virtual
    }
#endif
    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an <see cref="ConditionalInvocationCSharpExpression" /> that applies a delegate or lambda expression to a list of argument expressions.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> that represents the delegate or lambda expression to be applied.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> objects that represent the arguments that the delegate or lambda expression is applied to.</param>
        /// <returns>An <see cref="ConditionalInvocationCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalInvoke" /> and the <see cref="OldConditionalAccessCSharpExpression.Expression" /> and <see cref="ConditionalInvocationCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static ConditionalInvocationCSharpExpression ConditionalInvoke(Expression expression, params ParameterAssignment[] arguments)
        {
            return ConditionalInvoke(expression, (IEnumerable<ParameterAssignment>)arguments);
        }

        /// <summary>
        /// Creates an <see cref="ConditionalInvocationCSharpExpression" /> that applies a delegate or lambda expression to a list of argument expressions.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> that represents the delegate or lambda expression to be applied.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="ConditionalInvocationCSharpExpression.Arguments" /> collection.</param>
        /// <returns>An <see cref="ConditionalInvocationCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalInvoke" /> and the <see cref="OldConditionalAccessCSharpExpression.Expression" /> and <see cref="ConditionalInvocationCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static ConditionalInvocationCSharpExpression ConditionalInvoke(Expression expression, IEnumerable<ParameterAssignment> arguments)
        {
            RequiresCanRead(expression, nameof(expression));

            var method = GetConditionalInvokeMethod(expression);

            return MakeConditionalInvoke(expression, arguments, method);
        }

        /// <summary>
        /// Creates an <see cref="ConditionalInvocationCSharpExpression" /> that applies a delegate or lambda expression to a list of argument expressions.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> that represents the delegate or lambda expression to be applied.</param>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the arguments that the delegate or lambda expression is applied to.</param>
        /// <returns>An <see cref="ConditionalInvocationCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalInvoke" /> and the <see cref="OldConditionalAccessCSharpExpression.Expression" /> and <see cref="ConditionalInvocationCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static ConditionalInvocationCSharpExpression ConditionalInvoke(Expression expression, Expression[] arguments)
        {
            // NB: no params array to avoid overload resolution ambiguity
            return ConditionalInvoke(expression, (IEnumerable<Expression>)arguments);
        }

        /// <summary>
        /// Creates an <see cref="ConditionalInvocationCSharpExpression" /> that applies a delegate or lambda expression to a list of argument expressions.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> that represents the delegate or lambda expression to be applied.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="Expression" /> objects to use to populate the <see cref="ConditionalInvocationCSharpExpression.Arguments" /> collection.</param>
        /// <returns>An <see cref="ConditionalInvocationCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalInvoke" /> and the <see cref="OldConditionalAccessCSharpExpression.Expression" /> and <see cref="ConditionalInvocationCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static ConditionalInvocationCSharpExpression ConditionalInvoke(Expression expression, IEnumerable<Expression> arguments)
        {
            RequiresCanRead(expression, nameof(expression));

            var method = GetConditionalInvokeMethod(expression);

            var bindings = GetParameterBindings(method.GetParametersCached(), arguments);

            return MakeConditionalInvoke(expression, bindings, method);
        }

        private static MethodInfo GetConditionalInvokeMethod(Expression expression)
        {
            var type = expression.Type.GetNonNullReceiverType();
            var nonNull = Expression.Default(type); // NB: trick to be able to leverage the LINQ helper method; could benefit from refactoring
            return GetInvokeMethod(nonNull);
        }

        private static ConditionalInvocationCSharpExpression MakeConditionalInvoke(Expression expression, IEnumerable<ParameterAssignment> arguments, MethodInfo method)
        {
            var argList = arguments.ToReadOnly();
            ValidateParameterBindings(method, argList);

            return new ConditionalInvocationCSharpExpression(expression, argList, method);
        }
    }
#if OLD_CONDITIONAL
    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ConditionalInvocationCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitConditionalInvocation(ConditionalInvocationCSharpExpression node)
        {
            return node.Update(Visit(node.Expression), Visit(node.Arguments, VisitParameterAssignment));
        }
    }
#endif
}
