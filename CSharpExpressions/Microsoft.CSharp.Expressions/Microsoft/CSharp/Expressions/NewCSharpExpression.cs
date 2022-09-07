// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a constructor call.
    /// </summary>
    public sealed partial class NewCSharpExpression : CSharpExpression
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
        public override Type Type => Constructor.DeclaringType!;

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
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitNew(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public NewCSharpExpression Update(IEnumerable<ParameterAssignment>? arguments)
        {
            if (SameElements(ref arguments, Arguments))
            {
                return this;
            }

            return CSharpExpression.New(Constructor, arguments);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var parameters = Constructor.GetParametersCached();

            var res = BindArguments((_, args) => Expression.New(Constructor, args), null, parameters, Arguments);

            return res;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="NewCSharpExpression" /> that represents calling the specified constructor with the specified arguments.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructorInfo"/> to set the <see cref="NewCSharpExpression.Constructor"/> property equal to.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> objects that represent the call arguments.</param>
        /// <returns>A <see cref="NewCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.New" /> and <see cref="NewCSharpExpression.Constructor" /> and <see cref="NewCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static NewCSharpExpression New(ConstructorInfo constructor, params ParameterAssignment[]? arguments) => New(constructor, (IEnumerable<ParameterAssignment>?)arguments);

        /// <summary>
        /// Creates a <see cref="NewCSharpExpression" /> that represents calling the specified constructor with the specified arguments.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructorInfo"/> to set the <see cref="NewCSharpExpression.Constructor"/> property equal to.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="NewCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="NewCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.New" /> and the <see cref="NewCSharpExpression.Constructor" /> and <see cref="NewCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static NewCSharpExpression New(ConstructorInfo constructor, IEnumerable<ParameterAssignment>? arguments)
        {
            RequiresNotNull(constructor, nameof(constructor));

            ValidateConstructor(constructor);

            return MakeNew(constructor, arguments);
        }

        /// <summary>
        /// Creates a <see cref="NewCSharpExpression" /> that represents calling the specified constructor with the specified arguments.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructorInfo"/> to set the <see cref="NewCSharpExpression.Constructor"/> property equal to.</param>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the call arguments.</param>
        /// <returns>A <see cref="NewCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.New" /> and <see cref="NewCSharpExpression.Constructor" /> and <see cref="NewCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static new NewCSharpExpression New(ConstructorInfo constructor, Expression[]? arguments) =>
            // NB: no params array to avoid overload resolution ambiguity
            New(constructor, (IEnumerable<Expression>?)arguments);

        /// <summary>
        /// Creates a <see cref="NewCSharpExpression" /> that represents calling the specified constructor with the specified arguments.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructorInfo"/> to set the <see cref="NewCSharpExpression.Constructor"/> property equal to.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="Expression" /> objects to use to populate the <see cref="NewCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="NewCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.New" /> and the <see cref="NewCSharpExpression.Constructor" /> and <see cref="NewCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static new NewCSharpExpression New(ConstructorInfo constructor, IEnumerable<Expression>? arguments)
        {
            RequiresNotNull(constructor, nameof(constructor));

            ValidateConstructor(constructor);

            var bindings = GetParameterBindings(constructor, arguments);

            return MakeNew(constructor, bindings);
        }

        private static NewCSharpExpression MakeNew(ConstructorInfo constructor, IEnumerable<ParameterAssignment>? arguments)
        {
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
        protected internal virtual Expression VisitNew(NewCSharpExpression node) =>
            node.Update(
                Visit(node.Arguments, VisitParameterAssignment)
            );
    }
}
