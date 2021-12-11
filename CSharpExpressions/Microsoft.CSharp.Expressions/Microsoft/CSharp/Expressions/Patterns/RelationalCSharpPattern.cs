// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    using static PatternHelpers;

    /// <summary>
    /// Represents a relational pattern that compares against a constant value.
    /// </summary>
    public sealed class RelationalCSharpPattern : CSharpPattern
    {
        internal RelationalCSharpPattern(CSharpPatternInfo info, CSharpPatternType patternType, ConstantExpression value)
            : base(info)
        {
            PatternType = patternType;
            Value = value;
        }

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public override CSharpPatternType PatternType { get; }

        /// <summary>
        /// Gets the value used for the comparison.
        /// </summary>
        public ConstantExpression Value { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpPattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitRelationalPattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="value">The <see cref="Value" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public RelationalCSharpPattern Update(ConstantExpression value)
        {
            if (value == this.Value)
            {
                return this;
            }

            return Make(PatternInfo(InputType, NarrowedType), PatternType, value);
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
            if (inputType == this.InputType)
            {
                return this;
            }

            return CSharpPattern.Relational(PatternInfo(inputType, NarrowedType), PatternType, Value);
        }

        internal override Expression Reduce(Expression @object)
        {
            var op = PatternType switch
            {
                CSharpPatternType.LessThan => ExpressionType.LessThan,
                CSharpPatternType.LessThanOrEqual => ExpressionType.LessThanOrEqual,
                CSharpPatternType.GreaterThan => ExpressionType.GreaterThan,
                CSharpPatternType.GreaterThanOrEqual => ExpressionType.GreaterThanOrEqual,
                _ => throw ContractUtils.Unreachable
            };

            return MakeTest(this, @object, op, Value);
        }

        internal static Expression MakeTest(CSharpPattern pattern, Expression @object, ExpressionType op, ConstantExpression value)
        {
            Expression createTest(Expression obj)
            {
                if (pattern.InputType == pattern.NarrowedType)
                {
                    if (obj.Type != pattern.InputType)
                    {
                        obj = Expression.Convert(obj, pattern.InputType);
                    }

                    return
                        Expression.MakeBinary(
                            op,
                            obj,
                            value
                        );
                }
                else
                {
                    return
                        Expression.AndAlso(
                            Expression.TypeIs(obj, pattern.NarrowedType),
                            Expression.MakeBinary(
                                op,
                                Expression.Convert(obj, pattern.NarrowedType),
                                value
                            )
                        );
                }
            }

            return PatternHelpers.Reduce(@object, createTest);
        }

        internal static RelationalCSharpPattern Make(CSharpPatternInfo info, CSharpPatternType type, ConstantExpression value)
        {
            RequiresNotNull(value, nameof(value));

            CheckConstant(value, isRelational: true);

            if (info != null)
            {
                RequiresCompatiblePatternTypes(value.Type, info.NarrowedType);
            }
            else
            {
                info = PatternInfo(typeof(object), value.Type);
            }

            return new RelationalCSharpPattern(info, type, value);
        }
    }

    partial class CSharpPattern
    {
        /// <summary>
        /// Creates a relational less than pattern that compares against a constant value.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="value">The value to compare with.</param>
        /// <returns>A <see cref="RelationalCSharpPattern" /> representing a relational pattern.</returns>
        public static RelationalCSharpPattern LessThan(CSharpPatternInfo info, ConstantExpression value) => RelationalCSharpPattern.Make(info, CSharpPatternType.LessThan, value);

        /// <summary>
        /// Creates a relational less than or equal pattern that compares against a constant value.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="value">The value to compare with.</param>
        /// <returns>A <see cref="RelationalCSharpPattern" /> representing a relational pattern.</returns>
        public static RelationalCSharpPattern LessThanOrEqual(CSharpPatternInfo info, ConstantExpression value) => RelationalCSharpPattern.Make(info, CSharpPatternType.LessThanOrEqual, value);

        /// <summary>
        /// Creates a relational greater than pattern that compares against a constant value.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="value">The value to compare with.</param>
        /// <returns>A <see cref="RelationalCSharpPattern" /> representing a relational pattern.</returns>
        public static RelationalCSharpPattern GreaterThan(CSharpPatternInfo info, ConstantExpression value) => RelationalCSharpPattern.Make(info, CSharpPatternType.GreaterThan, value);

        /// <summary>
        /// Creates a relational greater than or equal pattern that compares against a constant value.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="value">The value to compare with.</param>
        /// <returns>A <see cref="RelationalCSharpPattern" /> representing a relational pattern.</returns>
        public static RelationalCSharpPattern GreaterThanOrEqual(CSharpPatternInfo info, ConstantExpression value) => RelationalCSharpPattern.Make(info, CSharpPatternType.GreaterThanOrEqual, value);

        /// <summary>
        /// Creates a relational less than pattern that compares against a constant value.
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>A <see cref="RelationalCSharpPattern" /> representing a relational pattern.</returns>
        public static RelationalCSharpPattern LessThan(ConstantExpression value) => LessThan(info: null, value);

        /// <summary>
        /// Creates a relational less than or equal pattern that compares against a constant value.
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>A <see cref="RelationalCSharpPattern" /> representing a relational pattern.</returns>
        public static RelationalCSharpPattern LessThanOrEqual(ConstantExpression value) => LessThanOrEqual(info: null, value);

        /// <summary>
        /// Creates a relational greater than pattern that compares against a constant value.
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>A <see cref="RelationalCSharpPattern" /> representing a relational pattern.</returns>
        public static RelationalCSharpPattern GreaterThan(ConstantExpression value) => GreaterThan(info: null, value);

        /// <summary>
        /// Creates a relational greater than or equal pattern that compares against a constant value.
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>A <see cref="RelationalCSharpPattern" /> representing a relational pattern.</returns>
        public static RelationalCSharpPattern GreaterThanOrEqual(ConstantExpression value) => GreaterThanOrEqual(info: null, value);

        /// <summary>
        /// Creates a relational pattern that compares against a constant value.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="type">The type of the pattern.</param>
        /// <param name="value">The value to compare with.</param>
        /// <returns>A <see cref="RelationalCSharpPattern" /> representing a relational pattern.</returns>
        public static RelationalCSharpPattern Relational(CSharpPatternInfo info, CSharpPatternType type, ConstantExpression value)
        {
            switch (type)
            {
                case CSharpPatternType.LessThan:
                case CSharpPatternType.LessThanOrEqual:
                case CSharpPatternType.GreaterThan:
                case CSharpPatternType.GreaterThanOrEqual:
                    return RelationalCSharpPattern.Make(info, type, value);
                default:
                    throw Error.InvalidRelationalPatternType(type);
            }
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="RelationalCSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitRelationalPattern(RelationalCSharpPattern node) => node.Update(VisitAndConvert(node.Value, nameof(VisitRelationalPattern)));
    }
}
