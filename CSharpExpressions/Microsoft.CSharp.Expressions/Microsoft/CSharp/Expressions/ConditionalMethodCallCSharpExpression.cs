// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Microsoft.CSharp.Expressions.Helpers;
using static System.Linq.Expressions.ExpressionStubs;
using System;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a conditional (null-propagating) call to a method.
    /// </summary>
    public sealed partial class ConditionalMethodCallCSharpExpression : ConditionalAccessCSharpExpression<MethodCallCSharpExpression>
    {
        internal ConditionalMethodCallCSharpExpression(Expression expression, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
            : this(expression, MakeReceiver(expression), method, arguments)
        {
        }

        private ConditionalMethodCallCSharpExpression(Expression expression, ConditionalReceiver receiver, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
            : this(expression, receiver, MakeAccess(receiver, method, arguments))
        {
        }

        private ConditionalMethodCallCSharpExpression(Expression expression, ConditionalReceiver receiver, MethodCallCSharpExpression access)
            : base(expression, receiver, access)
        {
        }

        private static MethodCallCSharpExpression MakeAccess(ConditionalReceiver receiver, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
        {
            if (method.IsStatic && method.IsDefined(typeof(ExtensionAttribute)))
            {
                var thisPar = method.GetParametersCached()[0];
                var thisArg = CSharpExpression.Bind(thisPar, receiver);

                var newArgs = new ParameterAssignment[arguments.Count + 1];
                newArgs[0] = thisArg;

                var i = 1;
                foreach (var arg in arguments)
                {
                    newArgs[i++] = arg;
                }

                var newArguments = new TrueReadOnlyCollection<ParameterAssignment>(newArgs);
                
                return CSharpExpression.Call(null, method, newArguments); // TODO: call ctor directly
            }
            else
            {
                return CSharpExpression.Call(receiver, method, arguments); // TODO: call ctor directly
            }
        }

        internal static ConditionalMethodCallCSharpExpression Make(Expression expression, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
        {
            return new ConditionalMethodCallCSharpExpression(expression, method, arguments); // TODO: remove layer of indirection if not needed
        }

        /// <summary>
        /// Gets the <see cref="Expression" /> that represents the instance whose method is called.
        /// </summary>
        public Expression Expression => Receiver; // TODO: Remove in favor of Object

        /// <summary>
        /// Gets the <see cref="Expression" /> that represents the instance accessed by the method call.
        /// </summary>
        public Expression Object => Receiver; // NB: Just an alias for familiarity with MethodCallExpression

        /// <summary>
        /// Gets the method to be called.
        /// </summary>
        public MethodInfo Method => WhenNotNull.Method;

        /// <summary>
        /// Gets a collection of argument assignments.
        /// </summary>
        public ReadOnlyCollection<ParameterAssignment> Arguments => WhenNotNull.Arguments;

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="ConditionalAccessCSharpExpression{MethodCallExpression}.Receiver" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ConditionalMethodCallCSharpExpression Update(Expression expression, IEnumerable<ParameterAssignment> arguments)
        {
            if (expression == Expression && arguments == Arguments)
            {
                return this;
            }

            return CSharpExpression.ConditionalCall(expression, Method, arguments);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        internal override Expression AcceptConditionalAccess(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitConditionalMethodCall(this);
        }

        internal override ConditionalAccessCSharpExpression<MethodCallCSharpExpression> Rewrite(Expression receiver, ConditionalReceiver nonNullReceiver, MethodCallCSharpExpression whenNotNull)
        {
            return new ConditionalMethodCallCSharpExpression(receiver, nonNullReceiver, whenNotNull);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="ConditionalMethodCallCSharpExpression" /> that represents a conditional (null-propagating) method call.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to call the method on.</param>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> objects that represent the call arguments.</param>
        /// <returns>A <see cref="ConditionalMethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="ConditionalAccessCSharpExpression{MethodCallExpression}.Receiver" /> and the <see cref="ConditionalMethodCallCSharpExpression.Object" />, <see cref="ConditionalMethodCallCSharpExpression.Method" />, and <see cref="ConditionalMethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static ConditionalMethodCallCSharpExpression ConditionalCall(Expression instance, MethodInfo method, params ParameterAssignment[] arguments)
        {
            return ConditionalCall(instance, method, (IEnumerable<ParameterAssignment>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="ConditionalMethodCallCSharpExpression" /> that represents a conditional (null-propagating) method call.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to call the method on.</param>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="ConditionalMethodCallCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="ConditionalMethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="ConditionalAccessCSharpExpression{MethodCallExpression}.Receiver" /> and the <see cref="ConditionalMethodCallCSharpExpression.Object" />, <see cref="ConditionalMethodCallCSharpExpression.Method" />, and <see cref="ConditionalMethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static ConditionalMethodCallCSharpExpression ConditionalCall(Expression instance, MethodInfo method, IEnumerable<ParameterAssignment> arguments)
        {
            ValidateConditionalMethod(ref instance, method);

            return MakeConditionalMethodCall(instance, method, arguments);
        }

        /// <summary>
        /// Creates a <see cref="ConditionalMethodCallCSharpExpression" /> that represents a conditional (null-propagating) method call.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to call the method on.</param>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the call arguments.</param>
        /// <returns>A <see cref="ConditionalMethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="ConditionalAccessCSharpExpression{MethodCallExpression}.Receiver" /> and the <see cref="ConditionalMethodCallCSharpExpression.Object" />, <see cref="ConditionalMethodCallCSharpExpression.Method" />, and <see cref="ConditionalMethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static ConditionalMethodCallCSharpExpression ConditionalCall(Expression instance, MethodInfo method, Expression[] arguments)
        {
            // NB: no params array to avoid overload resolution ambiguity
            return ConditionalCall(instance, method, (IEnumerable<Expression>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="ConditionalMethodCallCSharpExpression" /> that represents a conditional (null-propagating) method call.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to call the method on.</param>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="Expression" /> objects to use to populate the <see cref="ConditionalMethodCallCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="ConditionalMethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="ConditionalAccessCSharpExpression{MethodCallExpression}.Receiver" /> and the <see cref="ConditionalMethodCallCSharpExpression.Object" />, <see cref="ConditionalMethodCallCSharpExpression.Method" />, and <see cref="ConditionalMethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static ConditionalMethodCallCSharpExpression ConditionalCall(Expression instance, MethodInfo method, IEnumerable<Expression> arguments)
        {
            ValidateConditionalMethod(ref instance, method);

            var parameters = method.GetParametersCached();

            // DESIGN: Should the regular CSharpExpression.Call factories allow for calling extension methods like this as well?

            if (method.IsStatic)
            {
                parameters = parameters.RemoveFirst(); // NB: Skip "this" parameters which should be in "instance".
            }

            var bindings = GetParameterBindings(parameters, arguments);

            return MakeConditionalMethodCall(instance, method, bindings);
        }

        private static void ValidateConditionalMethod(ref Expression instance, MethodInfo method)
        {
            ContractUtils.RequiresNotNull(method, nameof(method));

            ValidateMethodInfo(method);

            if (method.IsStatic)
            {
                var parameters = method.GetParametersCached();

                if (!method.IsDefined(typeof(ExtensionAttribute), false) || parameters.Length == 0 /* NB: someone could craft a method with [ExtensionAttribute] in IL */)
                {
                    throw Error.ConditionalAccessRequiresNonStaticMember();
                }

                if (instance == null)
                {
                    throw Error.ExtensionMethodRequiresInstance();
                }

                instance = ValidateOneArgument(parameters[0], instance);
            }
            else
            {
                RequiresCanRead(instance, nameof(instance));

                var type = instance.Type.GetNonNullReceiverType();
                ValidateCallInstanceType(type, method);
            }
        }

        private static ConditionalMethodCallCSharpExpression MakeConditionalMethodCall(Expression instance, MethodInfo method, IEnumerable<ParameterAssignment> arguments)
        {
            var argList = arguments.ToReadOnly();
            ValidateParameterBindings(method, argList, extensionMethod: method.IsStatic);

            return ConditionalMethodCallCSharpExpression.Make(instance, method, argList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ConditionalMethodCallCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitConditionalMethodCall(ConditionalMethodCallCSharpExpression node)
        {
            return node.Update(Visit(node.Expression), Visit(node.Arguments, VisitParameterAssignment));
        }
    }
}
