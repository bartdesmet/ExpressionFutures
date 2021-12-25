// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Microsoft.CSharp.RuntimeBinder;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a dynamically bound invocation of a constructor.
    /// </summary>
    public sealed partial class InvokeConstructorDynamicCSharpExpression : DynamicCSharpExpression
    {
        internal InvokeConstructorDynamicCSharpExpression(Type context, CSharpBinderFlags binderFlags, Type type, ReadOnlyCollection<DynamicCSharpArgument> arguments)
            : base(context, binderFlags)
        {
            Type = type;
            Arguments = arguments;
        }

        /// <summary>
        /// Gets the <see cref="Type" /> containing the constructor to bind to.
        /// </summary>
        public override Type Type { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.DynamicInvokeConstructor;

        /// <summary>
        /// Gets the arguments to pass to the constructor.
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

            expressions[0] = Expression.Constant(Type, typeof(Type));
            argumentInfos[0] = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null);

            argumentTypes = null;
            CopyArguments(Arguments, argumentInfos, expressions, ref argumentTypes);

            binder = Binder.InvokeConstructor(Flags, Context, argumentInfos);
            arguments = expressions;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitDynamicInvokeConstructor(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public InvokeConstructorDynamicCSharpExpression Update(IEnumerable<DynamicCSharpArgument> arguments)
        {
            if (SameElements(ref arguments, Arguments))
            {
                return this;
            }

            return DynamicCSharpExpression.DynamicInvokeConstructor(Type, arguments, Flags, Context);
        }
    }

    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates a new expression representing a dynamically bound constructor invocation.
        /// </summary>
        /// <param name="type">The type of the object to instantiate.</param>
        /// <param name="arguments">An array of expressions representing the arguments passed to the constructor upon object creation.</param>
        /// <returns>A new expression representing a dynamically bound constructor invocation.</returns>
        public static InvokeConstructorDynamicCSharpExpression DynamicInvokeConstructor(Type type, params Expression[] arguments) =>
            DynamicInvokeConstructor(type, GetDynamicArguments(arguments), CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound constructor invocation.
        /// </summary>
        /// <param name="type">The type of the object to instantiate.</param>
        /// <param name="arguments">An enumerable sequence of expressions representing the arguments passed to the constructor upon object creation.</param>
        /// <returns>A new expression representing a dynamically bound constructor invocation.</returns>
        public static InvokeConstructorDynamicCSharpExpression DynamicInvokeConstructor(Type type, IEnumerable<Expression> arguments) =>
            DynamicInvokeConstructor(type, GetDynamicArguments(arguments), CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound constructor invocation.
        /// </summary>
        /// <param name="type">The type of the object to instantiate.</param>
        /// <param name="arguments">An array of expressions dynamic arguments the arguments passed to the constructor upon object creation.</param>
        /// <returns>A new expression representing a dynamically bound constructor invocation.</returns>
        public static InvokeConstructorDynamicCSharpExpression DynamicInvokeConstructor(Type type, DynamicCSharpArgument[] arguments) =>
            DynamicInvokeConstructor(type, arguments, CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound constructor invocation.
        /// </summary>
        /// <param name="type">The type of the object to instantiate.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the constructor upon object creation.</param>
        /// <returns>A new expression representing a dynamically bound constructor invocation.</returns>
        public static InvokeConstructorDynamicCSharpExpression DynamicInvokeConstructor(Type type, IEnumerable<DynamicCSharpArgument> arguments) =>
            DynamicInvokeConstructor(type, arguments, CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound constructor invocation with the specified binder flags.
        /// </summary>
        /// <param name="type">The type of the object to instantiate.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the constructor upon object creation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound constructor invocation.</returns>
        public static InvokeConstructorDynamicCSharpExpression DynamicInvokeConstructor(Type type, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags) =>
            DynamicInvokeConstructor(type, arguments, binderFlags, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound constructor invocation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="type">The type of the object to instantiate.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the constructor upon object creation.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound constructor invocation.</returns>
        public static InvokeConstructorDynamicCSharpExpression DynamicInvokeConstructor(Type type, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags, Type context)
        {
            RequiresNotNull(type, nameof(type));

            ValidateType(type);

            var argList = arguments.ToReadOnly();

            return new InvokeConstructorDynamicCSharpExpression(context, binderFlags, type, argList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="InvokeConstructorDynamicCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDynamicInvokeConstructor(InvokeConstructorDynamicCSharpExpression node) =>
            node.Update(
                Visit(node.Arguments, VisitDynamicArgument)
            );
    }
}
