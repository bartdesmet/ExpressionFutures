// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a switch expression.
    /// </summary>
    public sealed partial class SwitchCSharpExpression : CSharpExpression
    {
        internal SwitchCSharpExpression(Type type, Expression expression, ReadOnlyCollection<SwitchExpressionArm> arms)
        {
            Type = type;
            Expression = expression;
            Arms = arms;
        }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the object to switch on.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Gets a collection of switch expression arms.
        /// </summary>
        public ReadOnlyCollection<SwitchExpressionArm> Arms { get; }

        /// <summary>
        /// Gets the static type of the expression.
        /// </summary>
        public override Type Type { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.SwitchExpression;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitSwitchExpression(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <param name="arms">The <see cref="Arms" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public SwitchCSharpExpression Update(Expression expression, ReadOnlyCollection<SwitchExpressionArm> arms)
        {
            if (expression == this.Expression && arms == this.Arms)
            {
                return this;
            }

            return CSharpExpression.SwitchExpression(Type, expression, arms);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            //
            // NB: The code below is a poor man's linear evaluation of all the arms. It can be improved by building a DAG
            //     similar to the code gen in Roslyn.
            //

            var stmts = new List<Expression>();

            var obj = Expression.Parameter(Expression.Type, "__obj");
            stmts.Add(Expression.Assign(obj, Expression));

            var returnLabel = Expression.Label(Type, "__ret");

            foreach (var arm in Arms)
            {
                var test = arm.Pattern.Reduce(obj);

                if (arm.WhenClause != null)
                {
                    // NB: This happens for e.g. discard.
                    if (IsAlwaysTrue(test))
                    {
                        test = arm.WhenClause;
                    }
                    else
                    {
                        test = Expression.AndAlso(test, arm.WhenClause);
                    }
                }

                var returnValue =
                    Expression.Goto(
                        returnLabel,
                        arm.Value // REVIEW: Check if we need conversions here.
                    );

                var checkAndReturn = IsAlwaysTrue(test)
                    ? (Expression)returnValue
                    : Expression.IfThen(
                        test,
                        returnValue
                    );

                var armHandler =
                    Expression.Block(
                        typeof(void),
                        arm.Variables,
                        checkAndReturn
                    );

                stmts.Add(armHandler);

                static bool IsAlwaysTrue(Expression e) => e is ConstantExpression c && (bool)c.Value;
            }

            var throwExpr =
                Expression.Throw(
                    Expression.New(
                        SwitchExpressionExceptionCtor,
                        Expression.Convert(obj, typeof(object))
                    ),
                    Type
                );

            stmts.Add(Expression.Label(returnLabel, throwExpr));

            return Expression.Block(Type, new[] { obj }, stmts);
        }

        private static ConstructorInfo s_switchExpressionExceptionCtor;

        private static ConstructorInfo SwitchExpressionExceptionCtor =>
            s_switchExpressionExceptionCtor ??= typeof(SwitchExpressionException).GetConstructor(new[] { typeof(object) });
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="SwitchCSharpExpression"/> that represents a switch expression.
        /// </summary>
        /// <param name="type">The type of the expression.</param>
        /// <param name="expression">The expression representing the object to switch on.</param>
        /// <param name="arms">The arms representing the patterns to match.</param>
        /// <returns>The created <see cref="SwitchCSharpExpression"/>.</returns>
        public static SwitchCSharpExpression SwitchExpression(Type type, Expression expression, params SwitchExpressionArm[] arms) => SwitchExpression(type, expression, (IEnumerable<SwitchExpressionArm>)arms);

        /// <summary>
        /// Creates a <see cref="SwitchCSharpExpression"/> that represents a switch expression.
        /// </summary>
        /// <param name="type">The type of the expression.</param>
        /// <param name="expression">The expression representing the object to switch on.</param>
        /// <param name="arms">The arms representing the patterns to match.</param>
        /// <returns>The created <see cref="SwitchCSharpExpression"/>.</returns>
        public static SwitchCSharpExpression SwitchExpression(Type type, Expression expression, IEnumerable<SwitchExpressionArm> arms)
        {
            // CONSIDER: Overload that infers the type from all the arms.

            RequiresNotNull(type, nameof(type));
            ValidateType(type);

            if (type == typeof(void))
                throw Error.SwitchExpressionTypeShouldNotBeVoid();

            RequiresCanRead(expression, nameof(expression));
            RequiresNotNull(arms, nameof(arms));

            var armsCollection = arms.ToReadOnly();

            RequiresNotNullItems(armsCollection, nameof(arms));

            for (int i = 0, n = armsCollection.Count; i < n; i++)
            {
                var arm = armsCollection[i];

                if (!TypeUtils.AreReferenceAssignable(arm.Pattern.InputType, expression.Type))
                    throw Error.SwitchExpressionArmPatternInputNotCompatibleWithSwitchExpressionInput(i, arm.Pattern.InputType, expression.Type);

                if (!TypeUtils.AreReferenceAssignable(type, arm.Value.Type))
                    throw Error.SwitchExpressionArmValueNotCompatibleWithSwitchExpressionResult(i, arm.Value.Type, type);
            }

            return new SwitchCSharpExpression(type, expression, armsCollection);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="SwitchCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitSwitchExpression(SwitchCSharpExpression node)
        {
            return node.Update(Visit(node.Expression), Visit(node.Arms, VisitSwitchExpressionArm));
        }
    }
}
