// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents assignment to a parameter of a method.
    /// </summary>
    public sealed class ParameterAssignment
    {
        internal ParameterAssignment(ParameterInfo parameter, Expression expression)
        {
            Parameter = parameter;
            Expression = expression;
        }

        /// <summary>
        /// Gets the parameter to be initialized.
        /// </summary>
        public ParameterInfo Parameter { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> which represents the object being assigned to the parameter.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ParameterAssignment Update(Expression expression)
        {
            if (expression == Expression)
            {
                return this;
            }

            return CSharpExpression.Bind(Parameter, expression);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="ParameterAssignment"/> binding the specified value to the given parameter.
        /// </summary>
        /// <param name="parameter">The <see cref="ParameterInfo"/> for the parameter which is being assigned to.</param>
        /// <param name="expression">The value to be assigned to <paramref name="parameter"/>.</param>
        /// <returns>The created <see cref="ParameterAssignment"/>.</returns>
        public static ParameterAssignment Bind(ParameterInfo parameter, Expression expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a <see cref="ParameterAssignment"/> binding the specified value to the given parameter.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> for the method whose <paramref name="parameter"/> is being assigned to.</param>
        /// <param name="parameter">The name of the parameter on <paramref name="method"/> which is being assigned to.</param>
        /// <param name="expression">The value to be assigned to <paramref name="parameter"/>.</param>
        /// <returns>The created <see cref="ParameterAssignment"/>.</returns>
        public static ParameterAssignment Bind(MethodInfo method, string parameter, Expression expression)
        {
            // NB: Needed for the compiler to emit factory calls; no ldtoken to get ParameterInfo.
            throw new NotImplementedException();
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ParameterAssignment" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected virtual ParameterAssignment VisitParameterAssignment(ParameterAssignment node)
        {
            return node.Update(Visit(node.Expression));
        }
    }
}
