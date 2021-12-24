// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Linq.Expressions;
using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a pattern that checks convertibility to a type and assigns to a variable.
    /// </summary>
    public sealed partial class DeclarationCSharpPattern : CSharpObjectPattern
    {
        internal DeclarationCSharpPattern(CSharpObjectPatternInfo info, Type type)
            : base(info)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public override CSharpPatternType PatternType => CSharpPatternType.Declaration;

        /// <summary>
        /// Gets the type to check for.
        /// </summary>
        public new Type Type { get; set; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpPattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitDeclarationPattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="variable">The <see cref="Variable" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public DeclarationCSharpPattern Update(ParameterExpression variable)
        {
            if (variable == Variable)
            {
                return this;
            }

            return CSharpPattern.Declaration(ObjectPatternInfo(PatternInfo(InputType, NarrowedType), variable), Type);
        }

        /// <summary>
        /// Changes the input type to the specified type.
        /// </summary>
        /// <remarks>
        /// This functionality can be used when a pattern is pass to an expression or statement that applies the pattern.
        /// </remarks>
        /// <param name="inputType">The new input type.</param>
        /// <returns>The original pattern rewritten to use the specified input type.</returns>
        public override CSharpPattern ChangeType(Type inputType)
        {
            if (inputType == InputType)
            {
                return this;
            }

            // NB: This can fail if the variable and the input type is not compatible. See Declaration factory.

            return CSharpPattern.Declaration(ObjectPatternInfo(PatternInfo(inputType, Type), Variable), Type);
        }

        internal override Expression Reduce(Expression @object)
        {
            Expression createTest(Expression obj)
            {
                if (Variable != null)
                {
                    var test = Expression.Parameter(typeof(bool), "__test");

                    return
                        Expression.Block(
                            new[] { test },
                            Expression.Assign(test, Expression.TypeIs(obj, Type)),
                            Expression.IfThen(
                                test,
                                Expression.Assign(Variable, Expression.Convert(obj, Type))
                            ),
                            test
                        );
                }
                else
                {
                    return Expression.TypeIs(obj, Type);
                }
            }

            return PatternHelpers.Reduce(@object, createTest);
        }
    }

    partial class CSharpPattern
    {
        // REVIEW: Type cannot be nullable.

        /// <summary>
        /// Creates a pattern that checks convertibility to a type and assigns to a variable.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="type">The type to check for.</param>
        /// <returns>A <see cref="DeclarationCSharpPattern" /> that represents a pattern that checks convertibility to a type and assigns to a variable.</returns>
        public static DeclarationCSharpPattern Declaration(CSharpObjectPatternInfo info, Type type)
        {
            RequiresNotNull(info, nameof(info));
            RequiresNotNull(type, nameof(type));

            ValidatePatternType(type);

            var variableType = info.Variable?.Type;

            if (variableType != null)
            {
                RequiresCompatiblePatternTypes(type, variableType);

                if (variableType != info.Info.NarrowedType)
                    throw Error.CannotAssignPatternResultToVariable(variableType, info.Info.NarrowedType);
            }

            return new DeclarationCSharpPattern(info, type);
        }

        /// <summary>
        /// Creates a pattern that checks convertibility to a type and assigns to a variable.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="variable">The variable to assign to.</param>
        /// <returns>A <see cref="DeclarationCSharpPattern" /> that represents a pattern that checks convertibility to a type and assigns to a variable.</returns>
        public static DeclarationCSharpPattern Declaration(CSharpPatternInfo info, ParameterExpression variable)
        {
            RequiresNotNull(variable, nameof(variable));

            info ??= PatternInfo(variable.Type, variable.Type);

            return Declaration(ObjectPatternInfo(info, variable), variable.Type);
        }

        /// <summary>
        /// Creates a pattern that checks convertibility to a type and assigns to a variable.
        /// </summary>
        /// <param name="variable">The variable to assign to.</param>
        /// <returns>A <see cref="DeclarationCSharpPattern" /> that represents a pattern that checks convertibility to a type and assigns to a variable.</returns>
        public static DeclarationCSharpPattern Declaration(ParameterExpression variable) => Declaration(info: null, variable);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="DeclarationCSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitDeclarationPattern(DeclarationCSharpPattern node) => node.Update(VisitAndConvert(node.Variable, nameof(VisitDeclarationPattern)));
    }
}
