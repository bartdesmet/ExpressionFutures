// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a dynamically bound conversion to a static type.
    /// </summary>
    public sealed class ConvertDynamicCSharpExpression : DynamicCSharpExpression
    {
        internal ConvertDynamicCSharpExpression(Type context, CSharpBinderFlags binderFlags, Expression expression, Type type)
            : base(context, binderFlags)
        {
            Type = type;
            Expression = expression;
        }

        /// <summary>
        /// Gets the <see cref="Type" /> to convert to.
        /// </summary>
        public override Type Type { get; }

        /// <summary>
        /// Gets the expression to convert.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.DynamicConvert;

        /// <summary>
        /// Reduces the dynamic expression to a binder and a set of arguments to apply the operation to.
        /// </summary>
        /// <param name="binder">The binder used to perform the dynamic operation.</param>
        /// <param name="arguments">The arguments to apply the dynamic operation to.</param>
        protected override void ReduceDynamic(out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[] argumentTypes)
        {
            binder = Binder.Convert(Flags, Type, Context);
            arguments = new[] { Expression };
            argumentTypes = null;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitDynamicConvert(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ConvertDynamicCSharpExpression Update(Expression expression)
        {
            if (expression == this.Expression)
            {
                return this;
            }

            return DynamicCSharpExpression.DynamicConvert(expression, Type, Flags, Context);
        }
    }

    partial class DynamicCSharpExpression
    {
        // TODO: specialized overloads to aid with flags for explicit / implicit conversions?

        /// <summary>
        /// Creates a new expression representing a dynamically bound conversion to a static type.
        /// </summary>
        /// <param name="expression">The expression representing the object to convert.</param>
        /// <param name="type">The type to convert to.</param>
        /// <returns>A new expression representing a dynamically bound conversion to a static type.</returns>
        public static ConvertDynamicCSharpExpression DynamicConvert(Expression expression, Type type)
        {
            return DynamicConvert(expression, type, CSharpBinderFlags.None, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound conversion to a static type with the specified binder flags.
        /// </summary>
        /// <param name="expression">The expression representing the object to convert.</param>
        /// <param name="type">The type to convert to.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound conversion to a static type.</returns>
        public static ConvertDynamicCSharpExpression DynamicConvert(Expression expression, Type type, CSharpBinderFlags binderFlags)
        {
            return DynamicConvert(expression, type, binderFlags, null);
        }

        /// <summary>
        /// Creates a new expression representing a dynamically bound conversion to a static type with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="expression">The expression representing the object to convert.</param>
        /// <param name="type">The type to convert to.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound conversion to a static type.</returns>
        public static ConvertDynamicCSharpExpression DynamicConvert(Expression expression, Type type, CSharpBinderFlags binderFlags, Type context)
        {
            RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(type, nameof(type));

            ValidateType(type);

            return new ConvertDynamicCSharpExpression(context, binderFlags, expression, type);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ConvertDynamicCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDynamicConvert(ConvertDynamicCSharpExpression node)
        {
            return node.Update(Visit(node.Expression));
        }
    }
}
