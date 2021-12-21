// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Linq.Expressions;
using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a local declaration with an initializer expression.
    /// </summary>
    public sealed partial class LocalDeclaration
    {
        internal LocalDeclaration(ParameterExpression variable, Expression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        /// <summary>
        /// Gets the variable that is being declared.
        /// </summary>
        public ParameterExpression Variable { get; }

        /// <summary>
        /// Gets the initializer expression representing the value to assign to the variable.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="variable">The <see cref="Variable" /> property of the result.</param>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public LocalDeclaration Update(ParameterExpression variable, Expression expression)
        {
            if (variable == Variable && expression == Expression)
            {
                return this;
            }

            return CSharpExpression.LocalDeclaration(variable, expression);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="LocalDeclaration"/> that represents a local declaration.
        /// </summary>
        /// <param name="variable">The variable that is being declared.</param>
        /// <param name="expression">The initializer expression representing the value to assign to the variable.</param>
        /// <returns>The created <see cref="LocalDeclaration"/>.</returns>
        public static LocalDeclaration LocalDeclaration(ParameterExpression variable, Expression expression)
        {
            RequiresNotNull(variable, nameof(variable));
            RequiresCanRead(expression, nameof(expression));

            // NB: No non-null value to nullable value assignment allowed here. This is consistent with Assign,
            //     and the C# compiler should insert the Convert node.
            if (!AreReferenceAssignable(variable.Type, expression.Type))
            {
                throw LinqError.ExpressionTypeDoesNotMatchAssignment(expression.Type, variable.Type);
            }

            return new LocalDeclaration(variable, expression);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="LocalDeclaration" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected internal virtual LocalDeclaration VisitLocalDeclaration(LocalDeclaration node)
        {
            return node.Update(VisitAndConvert(node.Variable, nameof(VisitLocalDeclaration)), Visit(node.Expression));
        }
    }
}
