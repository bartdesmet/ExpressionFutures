// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using static Microsoft.CSharp.Expressions.Helpers;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a constructor call.
    /// </summary>
    public sealed class NewCSharpExpression : CSharpExpression
    {
        internal NewCSharpExpression(ConstructorInfo constructor, ReadOnlyCollection<ParameterAssignment> arguments)
        {
            Constructor = constructor;
            Arguments = arguments;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.New;

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => Constructor.DeclaringType;

        /// <summary>
        /// Gets the <see cref="ConstructorInfo" /> for the constructor to be called.
        /// </summary>
        public ConstructorInfo Constructor { get; }

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
            return visitor.VisitNew(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public NewCSharpExpression Update(IEnumerable<ParameterAssignment> arguments)
        {
            if (arguments == Arguments)
            {
                return this;
            }

            return CSharpExpression.New(Constructor, arguments);
        }

        /// <summary>
        /// Reduces the call expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var parameters = Constructor.GetParametersCached();

            if (CheckArgumentsInOrder(Arguments))
            {
                var args = new Expression[parameters.Length];

                foreach (var argument in Arguments)
                {
                    args[argument.Parameter.Position] = argument.Expression;
                }

                FillOptionalParameters(parameters, args);

                return Expression.New(Constructor, args);
            }
            else
            {
                var vars = new ParameterExpression[Arguments.Count];
                var exprs = new Expression[vars.Length + 1];
                var args = new Expression[parameters.Length];

                var i = 0;
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

                exprs[i] = Expression.New(Constructor, args);

                return Expression.Block(vars, exprs);
            }
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="NewCSharpExpression" /> that represents calling the specified constructor with the specified arguments.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructorInfo"/> to set the <see cref="P:Constructor"/> property equal to.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> that represents the call arguments.</param>
        ///<returns>A <see cref="NewCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.CSharpExpression.CSharpNodeType" /> property equal to <see cref="F:Microsoft.CSharp.Expressions.CSharpExpressionType.New" /> and <see cref="P:Microsoft.CSharp.Expressions.NewCSharpExpression.Constructor" /> and <see cref="P:Microsoft.CSharp.Expressions.NewCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static NewCSharpExpression New(ConstructorInfo constructor, params ParameterAssignment[] arguments)
        {
            return New(constructor, (IEnumerable<ParameterAssignment>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="NewCSharpExpression" /> that represents calling the specified constructor with the specified arguments.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructorInfo"/> to set the <see cref="P:Constructor"/> property equal to.</param>
        /// <param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="P:Microsoft.CSharp.Expressions.NewCSharpExpression.Arguments" /> collection.</param>
        ///<returns>A <see cref="NewCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.CSharpExpression.CSharpNodeType" /> property equal to <see cref="F:Microsoft.CSharp.Expressions.CSharpExpressionType.New" /> and the <see cref="P:Microsoft.CSharp.Expressions.NewCSharpExpression.Constructor" /> and <see cref="P:Microsoft.CSharp.Expressions.NewCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static NewCSharpExpression New(ConstructorInfo constructor, IEnumerable<ParameterAssignment> arguments)
        {
            ContractUtils.RequiresNotNull(constructor, nameof(constructor));

            ValidateConstructor(constructor);

            var argList = arguments.ToReadOnly();
            ValidateParameterBindings(constructor, argList);

            return new NewCSharpExpression(constructor, argList);
        }

        private static void ValidateConstructor(ConstructorInfo constructor)
        {
            if (constructor.IsStatic)
                throw Error.NonStaticConstructorRequired();
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="NewCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Nonsense API guidance provided by FxCop.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitNew(NewCSharpExpression node)
        {
            return node.Update(Visit(node.Arguments, VisitParameterAssignment));
        }
    }
}
