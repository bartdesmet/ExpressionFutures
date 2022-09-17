﻿// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ExpressionUtils;

#pragma warning disable CA1720 // Identifier contains type name (use of Object property).

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a call to either static or an instance method.
    /// </summary>
    public sealed partial class MethodCallCSharpExpression : CSharpExpression
    {
        // TODO: optimized layout for cases where all arguments are specified in order?
        //       could allocate and swap the ROC<ParameterAssignment> in lieu of a ROC<Expression>

        internal MethodCallCSharpExpression(Expression? @object, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
        {
            Object = @object;
            Method = method;
            Arguments = arguments;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Call;

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => Method.ReturnType;

        /// <summary>
        /// Gets the <see cref="Expression" /> that represents the instance for instance method calls or null for static method calls.
        /// </summary>
        public Expression? Object { get; }

        /// <summary>
        /// Gets the <see cref="MethodInfo" /> for the method to be called.
        /// </summary>
        public MethodInfo Method { get; }

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
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitMethodCall(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="object">The <see cref="Object" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public MethodCallCSharpExpression Update(Expression? @object, IEnumerable<ParameterAssignment>? arguments)
        {
            if (@object == Object && SameElements(ref arguments, Arguments))
            {
                return this;
            }

            return CSharpExpression.Call(@object, Method, arguments);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var parameters = Method.GetParametersCached();

            var res = BindArguments((obj, args) => Expression.Call(obj, Method, args), Object, parameters, Arguments);

            return res;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="MethodCallCSharpExpression" /> that represents a call to a static method.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> objects that represent the call arguments.</param>
        /// <returns>A <see cref="MethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Call" /> and the <see cref="MethodCallCSharpExpression.Object" />, <see cref="MethodCallCSharpExpression.Method" />, and <see cref="MethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static MethodCallCSharpExpression Call(MethodInfo method, params ParameterAssignment[]? arguments) => Call(instance: null, method, (IEnumerable<ParameterAssignment>?)arguments);

        /// <summary>
        /// Creates a <see cref="MethodCallCSharpExpression" /> that represents a call to a static method.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="MethodCallCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="MethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Call" /> and the <see cref="MethodCallCSharpExpression.Object" />, <see cref="MethodCallCSharpExpression.Method" />, and <see cref="MethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static MethodCallCSharpExpression Call(MethodInfo method, IEnumerable<ParameterAssignment>? arguments) => Call(instance: null, method, arguments);

        /// <summary>
        /// Creates a <see cref="MethodCallCSharpExpression" /> that represents a method call.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance for an instance call. (pass null for a static method).</param>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> objects that represent the call arguments.</param>
        /// <returns>A <see cref="MethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Call" /> and the <see cref="MethodCallCSharpExpression.Object" />, <see cref="MethodCallCSharpExpression.Method" />, and <see cref="MethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static MethodCallCSharpExpression Call(Expression? instance, MethodInfo method, params ParameterAssignment[]? arguments) => Call(instance, method, (IEnumerable<ParameterAssignment>?)arguments);

        /// <summary>
        /// Creates a <see cref="MethodCallCSharpExpression" /> that represents a method call.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance for an instance call. (pass null for a static method).</param>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="MethodCallCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="MethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Call" /> and the <see cref="MethodCallCSharpExpression.Object" />, <see cref="MethodCallCSharpExpression.Method" />, and <see cref="MethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static MethodCallCSharpExpression Call(Expression? instance, MethodInfo method, IEnumerable<ParameterAssignment>? arguments)
        {
            RequiresNotNull(method, nameof(method));

            ValidateMethodInfo(method, nameof(method));
            ValidateStaticOrInstanceMethod(instance, method);

            return MakeCall(instance, method, arguments);
        }

        /// <summary>
        /// Creates a <see cref="MethodCallCSharpExpression" /> that represents a call to a static method.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the call arguments.</param>
        /// <returns>A <see cref="MethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Call" /> and the <see cref="MethodCallCSharpExpression.Object" />, <see cref="MethodCallCSharpExpression.Method" />, and <see cref="MethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static new MethodCallCSharpExpression Call(MethodInfo method, Expression[]? arguments) =>
            // NB: no params array to avoid overload resolution ambiguity
            Call(null, method, (IEnumerable<Expression>?)arguments);

        /// <summary>
        /// Creates a <see cref="MethodCallCSharpExpression" /> that represents a call to a static method.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="Expression" /> objects to use to populate the <see cref="MethodCallCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="MethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Call" /> and the <see cref="MethodCallCSharpExpression.Object" />, <see cref="MethodCallCSharpExpression.Method" />, and <see cref="MethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static new MethodCallCSharpExpression Call(MethodInfo method, IEnumerable<Expression>? arguments) => Call(instance: null, method, arguments);

        /// <summary>
        /// Creates a <see cref="MethodCallCSharpExpression" /> that represents a method call.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance for an instance call. (pass null for a static method).</param>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the call arguments.</param>
        /// <returns>A <see cref="MethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Call" /> and the <see cref="MethodCallCSharpExpression.Object" />, <see cref="MethodCallCSharpExpression.Method" />, and <see cref="MethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static new MethodCallCSharpExpression Call(Expression? instance, MethodInfo method, Expression[]? arguments) =>
            // NB: no params array to avoid overload resolution ambiguity
            Call(instance, method, (IEnumerable<Expression>?)arguments);

        /// <summary>
        /// Creates a <see cref="MethodCallCSharpExpression" /> that represents a method call.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance for an instance call. (pass null for a static method).</param>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="Expression" /> objects to use to populate the <see cref="MethodCallCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="MethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Call" /> and the <see cref="MethodCallCSharpExpression.Object" />, <see cref="MethodCallCSharpExpression.Method" />, and <see cref="MethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static new MethodCallCSharpExpression Call(Expression? instance, MethodInfo method, IEnumerable<Expression>? arguments)
        {
            RequiresNotNull(method, nameof(method));

            ValidateMethodInfo(method, nameof(method));
            ValidateStaticOrInstanceMethod(instance, method);

            var bindings = GetParameterBindings(method, arguments);

            return MakeCall(instance, method, bindings);
        }

        private static MethodCallCSharpExpression MakeCall(Expression? instance, MethodInfo method, IEnumerable<ParameterAssignment>? arguments)
        {
            var argList = arguments.ToReadOnly();
            ValidateParameterBindings(method, argList);

            return new MethodCallCSharpExpression(instance, method, argList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="MethodCallCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitMethodCall(MethodCallCSharpExpression node) =>
            node.Update(
                Visit(node.Object),
                Visit(node.Arguments, VisitParameterAssignment)
            );
    }
}
