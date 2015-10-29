// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.ExpressionStubs;

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
            ContractUtils.RequiresNotNull(parameter, nameof(parameter));

            expression = ValidateOneArgument(parameter, expression);

            return new ParameterAssignment(parameter, expression);
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
            // NB: This overload is needed for the compiler to emit factory calls;
            //     we can't emit a `ldtoken` instruction to obtain a ParameterInfo.

            ContractUtils.RequiresNotNull(method, nameof(method));
            ContractUtils.RequiresNotNull(parameter, nameof(parameter));

            var parameterInfo = default(ParameterInfo);

            foreach (var candidate in method.GetParametersCached())
            {
                if (candidate.Name == parameter)
                {
                    parameterInfo = candidate;
                    break;
                }
            }

            if (parameterInfo == null)
            {
                throw Error.ParameterNotDefinedForMethod(parameter, method.Name);
            }

            return Bind(parameterInfo, expression);
        }

        private static Expression ValidateOneArgument(ParameterInfo parameter, Expression expression)
        {
            RequiresCanRead(expression, nameof(expression));

            var pType = parameter.ParameterType;

            if (pType.IsByRef)
            {
                pType = pType.GetElementType();
            }

            TypeUtils.ValidateType(pType);

            if (!TypeUtils.AreReferenceAssignable(pType, expression.Type))
            {
                if (!TryQuote(pType, ref expression))
                {
                    throw Error.ExpressionTypeDoesNotMatchParameter(expression.Type, pType);
                }
            }

            return expression;
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
