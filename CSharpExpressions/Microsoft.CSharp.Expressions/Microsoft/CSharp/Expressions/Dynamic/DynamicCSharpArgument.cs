// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.RuntimeBinder;
using System.Linq.Expressions;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents an argument passed to a dynamically bound operation.
    /// </summary>
    public sealed partial class DynamicCSharpArgument
    {
        internal DynamicCSharpArgument(CSharpArgumentInfoFlags flags, string name, Expression expression)
        {
            Flags = flags;
            Name = name;
            Expression = expression;
        }

        /// <summary>
        /// Gets the flags used to bind the argument.
        /// </summary>
        public CSharpArgumentInfoFlags Flags { get; }

        /// <summary>
        /// Gets the name of the argument.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the expression assigned to the argument.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Gets the runtime binder representation of the argument.
        /// </summary>
        internal CSharpArgumentInfo ArgumentInfo => CSharpArgumentInfo.Create(Flags, Name);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public DynamicCSharpArgument Update(Expression expression)
        {
            if (expression == this.Expression)
            {
                return this;
            }

            return DynamicCSharpExpression.DynamicArgument(expression, Name, Flags);
        }
    }

    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates an object representing a dynamically bound argument.
        /// </summary>
        /// <param name="expression">The expression representing the value of the argument.</param>
        /// <returns>An object representing a dynamically bound argument.</returns>
        public static DynamicCSharpArgument DynamicArgument(Expression expression)
        {
            return DynamicArgument(expression, null, CSharpArgumentInfoFlags.None);
        }

        /// <summary>
        /// Creates an object representing a dynamically bound argument with the specified name.
        /// </summary>
        /// <param name="expression">The expression representing the value of the argument.</param>
        /// <param name="name">The name of the argument to bind.</param>
        /// <returns>An object representing a dynamically bound argument.</returns>
        public static DynamicCSharpArgument DynamicArgument(Expression expression, string name)
        {
            return DynamicArgument(expression, name, CSharpArgumentInfoFlags.None);
        }

        /// <summary>
        /// Creates an object representing a dynamically bound argument with the specified name and the specified argument flags.
        /// </summary>
        /// <param name="expression">The expression representing the value of the argument.</param>
        /// <param name="name">The name of the argument to bind.</param>
        /// <param name="argumentFlags">The argument flags denoting various properties about the dynamically bound argument.</param>
        /// <returns>An object representing a dynamically bound argument.</returns>
        public static DynamicCSharpArgument DynamicArgument(Expression expression, string name, CSharpArgumentInfoFlags argumentFlags)
        {
            RequiresCanRead(expression, nameof(expression));

            return new DynamicCSharpArgument(argumentFlags, name, expression);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="DynamicCSharpArgument" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual DynamicCSharpArgument VisitDynamicArgument(DynamicCSharpArgument node)
        {
            return node.Update(Visit(node.Expression));
        }
    }
}
