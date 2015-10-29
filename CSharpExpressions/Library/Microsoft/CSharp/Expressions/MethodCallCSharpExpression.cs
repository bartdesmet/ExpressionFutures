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

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a call to either static or an instance method.
    /// </summary>
    public sealed class MethodCallCSharpExpression : CSharpExpression
    {
        internal MethodCallCSharpExpression(Expression @object, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
        {
            Object = @object;
            Method = method;
            Arguments = arguments;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType
        {
            get { return CSharpExpressionType.Call; }
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => Method.ReturnType;

        /// <summary>
        /// Gets the <see cref="Expression" /> that represents the instance for instance method calls or null for static method calls.
        /// </summary>
        public Expression Object { get; }

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
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitMethodCall(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="object">The <see cref="Object" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public MethodCallCSharpExpression Update(Expression @object, IEnumerable<ParameterAssignment> arguments)
        {
            if (@object == Object && arguments == Arguments)
            {
                return this;
            }

            return CSharpExpression.Call(@object, Method, arguments);
        }

        /// <summary>
        /// Reduces the call expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var inOrder = true;
            var lastPosition = -1;

            foreach (var argument in Arguments)
            {
                if (argument.Parameter.Position < lastPosition)
                {
                    inOrder = false;
                    break;
                }

                lastPosition = argument.Parameter.Position;
            }

            var parameters = Method.GetParametersCached();

            if (inOrder)
            {
                var args = new Expression[parameters.Length];

                foreach (var argument in Arguments)
                {
                    args[argument.Parameter.Position] = argument.Expression;
                }

                FillOptionalParameters(parameters, args);

                return Expression.Call(Object, Method, args);
            }
            else
            {
                var isStatic = Method.IsStatic;

                var vars = new ParameterExpression[(isStatic ? 0 : 1) + Arguments.Count];
                var exprs = new Expression[vars.Length + 1];
                var args = new Expression[parameters.Length];

                var obj = default(ParameterExpression);
                var i = 0;
                if (!isStatic)
                {
                    obj = Expression.Parameter(Object.Type, "obj");
                    vars[i] = obj;
                    exprs[i] = Expression.Assign(obj, Object);

                    i++;
                }

                foreach (var argument in Arguments)
                {
                    var parameter = argument.Parameter;
                    var expression = argument.Expression;

                    var var = Expression.Parameter(argument.Expression.Type, parameter.Name);
                    vars[i] = var;
                    exprs[i] = Expression.Assign(var, expression);

                    args[parameter.Position] = var;

                    i++;
                }

                FillOptionalParameters(parameters, args);

                exprs[i] = Expression.Call(obj, Method, args);

                return Expression.Block(vars, exprs);
            }
        }

        private static void FillOptionalParameters(ParameterInfo[] parameters, Expression[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    var parameter = parameters[i];
                    args[i] = Expression.Constant(parameter.DefaultValue, parameter.ParameterType);
                }
            }
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="MethodCallCSharpExpression" /> that represents a call to a static method.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> that represents the call arguments.</param>
        ///<returns>A <see cref="MethodCallCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.CSharpExpression.CSharpNodeType" /> property equal to <see cref="F:Microsoft.CSharp.Expressions.CSharpExpressionType.Call" /> and the <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Object" />, <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Method" />, and <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static MethodCallCSharpExpression Call(MethodInfo method, params ParameterAssignment[] arguments)
        {
            return Call(null, method, (IEnumerable<ParameterAssignment>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="MethodCallCSharpExpression" /> that represents a call to a static method.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Arguments" /> collection.</param>
        ///<returns>A <see cref="MethodCallCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.CSharpExpression.CSharpNodeType" /> property equal to <see cref="F:Microsoft.CSharp.Expressions.CSharpExpressionType.Call" /> and the <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Object" />, <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Method" />, and <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static MethodCallCSharpExpression Call(MethodInfo method, IEnumerable<ParameterAssignment> arguments)
        {
            return Call(null, method, (IEnumerable<ParameterAssignment>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="MethodCallCSharpExpression" /> that represents a method call.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance for an instance call. (pass null for a static method).</param>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> that represents the call arguments.</param>
        ///<returns>A <see cref="MethodCallCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.CSharpExpression.CSharpNodeType" /> property equal to <see cref="F:Microsoft.CSharp.Expressions.CSharpExpressionType.Call" /> and the <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Object" />, <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Method" />, and <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static MethodCallCSharpExpression Call(Expression instance, MethodInfo method, params ParameterAssignment[] arguments)
        {
            return Call(instance, method, (IEnumerable<ParameterAssignment>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="MethodCallCSharpExpression" /> that represents a method call.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance for an instance call. (pass null for a static method).</param>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the target method.</param>
        /// <param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Arguments" /> collection.</param>
        ///<returns>A <see cref="MethodCallCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.CSharpExpression.CSharpNodeType" /> property equal to <see cref="F:Microsoft.CSharp.Expressions.CSharpExpressionType.Call" /> and the <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Object" />, <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Method" />, and <see cref="P:Microsoft.CSharp.Expressions.MethodCallCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static MethodCallCSharpExpression Call(Expression instance, MethodInfo method, IEnumerable<ParameterAssignment> arguments)
        {
            ContractUtils.RequiresNotNull(method, nameof(method));

            ValidateMethodInfo(method);
            ValidateStaticOrInstanceMethod(instance, method);

            var argList = arguments.ToReadOnly();
            ValidateParameterBindings(method, argList);

            return new MethodCallCSharpExpression(instance, method, argList);
        }

        private static void ValidateParameterBindings(MethodBase method, ReadOnlyCollection<ParameterAssignment> argList)
        {
            var boundParameters = new HashSet<ParameterInfo>();

            foreach (var arg in argList)
            {
                var parameter = arg.Parameter;

                if (parameter.Member != method)
                {
                    throw Error.ParameterNotDefinedForMethod(parameter.Name, method.Name);
                }

                if (!boundParameters.Add(parameter))
                {
                    throw Error.DuplicateParameterBinding(parameter.Name);
                }
            }

            var parameters = method.GetParametersCached();

            foreach (var parameter in parameters)
            {
                if (!boundParameters.Contains(parameter) && (!parameter.IsOptional || !parameter.HasDefaultValue))
                {
                    throw Error.UnboundParameter(parameter.Name);
                }
            }
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
        protected internal virtual Expression VisitMethodCall(MethodCallCSharpExpression node)
        {
            return node.Update(Visit(node.Object), Visit(node.Arguments, VisitParameterAssignment));
        }
    }
}
