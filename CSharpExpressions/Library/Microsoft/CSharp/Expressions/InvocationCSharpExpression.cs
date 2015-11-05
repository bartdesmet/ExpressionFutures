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
    /// <summary>
    /// Represents an expression that applies a delegate or lambda expression to a list of argument expressions.
    /// </summary>
    public sealed class InvocationCSharpExpression : CSharpExpression
    {
        private readonly MethodInfo _invokeMethod;

        internal InvocationCSharpExpression(Expression expression, ReadOnlyCollection<ParameterAssignment> arguments, MethodInfo invokeMethod)
        {
            Expression = expression;
            Arguments = arguments;
            _invokeMethod = invokeMethod;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Invoke;

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => _invokeMethod.ReturnType;

        /// <summary>
        /// Gets the <see cref="Expression" /> that represents the instance for instance method calls or null for static method calls.
        /// </summary>
        public Expression Expression { get; }

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
            return visitor.VisitInvocation(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public InvocationCSharpExpression Update(Expression expression, IEnumerable<ParameterAssignment> arguments)
        {
            if (expression == Expression && arguments == Arguments)
            {
                return this;
            }

            return CSharpExpression.Invoke(expression, arguments);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var parameters = _invokeMethod.GetParametersCached();

            var res = BindArguments((obj, args) => Expression.Invoke(obj, args), Expression, parameters, Arguments);

            return res;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an <see cref="InvocationCSharpExpression" /> that applies a delegate or lambda expression to a list of argument expressions.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> that represents the delegate or lambda expression to be applied.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> that represents the arguments that the delegate or lambda expression is applied to.</param>
        /// <returns>An <see cref="InvocationCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.CSharpExpression.CSharpNodeType" /> property equal to <see cref="F:Microsoft.CSharp.Expressions.CSharpExpressionType.Invoke" /> and the <see cref="P:Microsoft.CSharp.Expressions.InvocationCSharpExpression.Expression" /> and <see cref="P:Microsoft.CSharp.Expressions.InvocationCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static InvocationCSharpExpression Invoke(Expression expression, params ParameterAssignment[] arguments)
        {
            return Invoke(expression, (IEnumerable<ParameterAssignment>)arguments);
        }

        /// <summary>
        /// Creates an <see cref="InvocationCSharpExpression" /> that applies a delegate or lambda expression to a list of argument expressions.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> that represents the delegate or lambda expression to be applied.</param>
        /// <param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="P:Microsoft.CSharp.Expressions.InvocationCSharpExpression.Arguments" /> collection.</param>
        /// <returns>An <see cref="InvocationCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.CSharpExpression.CSharpNodeType" /> property equal to <see cref="F:Microsoft.CSharp.Expressions.CSharpExpressionType.Invoke" /> and the <see cref="P:Microsoft.CSharp.Expressions.InvocationCSharpExpression.Object" /> and <see cref="P:Microsoft.CSharp.Expressions.InvocationCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static InvocationCSharpExpression Invoke(Expression expression, IEnumerable<ParameterAssignment> arguments)
        {
            RequiresCanRead(expression, "expression");

            var method = GetInvokeMethod(expression);

            var argList = arguments.ToReadOnly();
            ValidateParameterBindings(method, argList);

            return new InvocationCSharpExpression(expression, argList, method);
        }

        // TODO: add overloads with just Expression[] or IEnumerable<Expression>
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="InvocationCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitInvocation(InvocationCSharpExpression node)
        {
            return node.Update(Visit(node.Expression), Visit(node.Arguments, VisitParameterAssignment));
        }
    }
}
