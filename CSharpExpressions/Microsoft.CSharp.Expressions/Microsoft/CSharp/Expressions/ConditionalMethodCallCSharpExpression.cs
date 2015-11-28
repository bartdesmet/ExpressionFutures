// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Microsoft.CSharp.Expressions.Helpers;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a conditional (null-propagating) call to a method.
    /// </summary>
#if OLD_CONDITIONAL
    public abstract partial class ConditionalMethodCallCSharpExpression : OldConditionalAccessCSharpExpression
    {
        internal ConditionalMethodCallCSharpExpression(Expression expression, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
            : base(expression)
        {
            Method = method;
            Arguments = arguments;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.ConditionalCall;

        /// <summary>
        /// Gets the <see cref="Expression" /> that represents the instance accessed by the method call.
        /// </summary>
        public Expression Object => Expression; // NB: Just an alias for familiarity with MethodCallExpression

        /// <summary>
        /// Gets the method to be called.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Gets a collection of argument assignments.
        /// </summary>
        public ReadOnlyCollection<ParameterAssignment> Arguments { get; }

        /// <summary>
        /// Gets the result type of the underlying access.
        /// </summary>
        protected override Type UnderlyingType => Method.ReturnType;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitConditionalMethodCall(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="OldConditionalAccessCSharpExpression.Expression" /> property of the result.</param>
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

        class InstanceMethodCall : ConditionalMethodCallCSharpExpression
        {
            internal InstanceMethodCall(Expression expression, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
                : base(expression, method, arguments)
            {
            }

            protected override Expression ReduceAccess(Expression nonNull) => CSharpExpression.Call(nonNull, Method, Arguments);
        }

        class ExtensionMethodCall : ConditionalMethodCallCSharpExpression
        {
            internal ExtensionMethodCall(Expression expression, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
                : base(expression, method, arguments)
            {
            }

            // DESIGN: Unlike LINQ's MethodCallExpression, the Object property won't be null for a static extension method.
            //         We could make it more similar by making the Object property virtual and returning null. We'd also have
            //         to mask the underlying Arguments so it includes a binding for the "this" parameter. Finally, the logic
            //         in the visitor would have to be revisited so the duplication of the binding for "this" in the Expression
            //         property and the Arguments collection doesn't result in an error (we could use a Rewrite pattern a la
            //         BlockExpression in lieu of an Update).

            protected override Expression ReduceAccess(Expression nonNull) => CSharpExpression.Call(Method, new[] { BindThis(nonNull) }.Concat(Arguments));

            private ParameterAssignment BindThis(Expression expression)
            {
                var thisParameter = Method.GetParametersCached()[0];
                return CSharpExpression.Bind(thisParameter, expression);
            }
        }

        internal static ConditionalMethodCallCSharpExpression Make(Expression expression, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
        {
            if (method.IsStatic)
            {
                return new ExtensionMethodCall(expression, method, arguments);
            }
            else
            {
                return new InstanceMethodCall(expression, method, arguments);
            }
        }
    }
#else
    public sealed partial class ConditionalMethodCallCSharpExpression : ConditionalAccessCSharpExpression<MethodCallCSharpExpression>
    {
        internal ConditionalMethodCallCSharpExpression(Expression expression, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
            : this(expression, MakeReceiver(expression), method, arguments)
        {
        }

        private ConditionalMethodCallCSharpExpression(Expression expression, ConditionalReceiver receiver, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
            : base(expression, receiver, MakeAccess(receiver, method, arguments))
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
        /// <param name="expression">The <see cref="OldConditionalAccessCSharpExpression.Expression" /> property of the result.</param>
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

        // TODO: Rewrite virtual
    }
#endif
    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="ConditionalMethodCallCSharpExpression" /> that represents a conditional (null-propagating) method call.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to call the method on.</param>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> objects that represent the call arguments.</param>
        /// <returns>A <see cref="ConditionalMethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalCall" /> and the <see cref="ConditionalMethodCallCSharpExpression.Object" />, <see cref="ConditionalMethodCallCSharpExpression.Method" />, and <see cref="ConditionalMethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
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
        /// <returns>A <see cref="ConditionalMethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalCall" /> and the <see cref="ConditionalMethodCallCSharpExpression.Object" />, <see cref="ConditionalMethodCallCSharpExpression.Method" />, and <see cref="ConditionalMethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
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
        /// <returns>A <see cref="ConditionalMethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalCall" /> and the <see cref="ConditionalMethodCallCSharpExpression.Object" />, <see cref="ConditionalMethodCallCSharpExpression.Method" />, and <see cref="ConditionalMethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
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
        /// <returns>A <see cref="ConditionalMethodCallCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.ConditionalCall" /> and the <see cref="ConditionalMethodCallCSharpExpression.Object" />, <see cref="ConditionalMethodCallCSharpExpression.Method" />, and <see cref="ConditionalMethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
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
#if OLD_CONDITIONAL
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
#endif
}
