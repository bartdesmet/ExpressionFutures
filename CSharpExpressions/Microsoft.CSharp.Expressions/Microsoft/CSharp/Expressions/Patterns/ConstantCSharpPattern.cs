// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;
    using static PatternHelpers;

    /// <summary>
    /// Represents a pattern that checks for equality to a constant value.
    /// </summary>
    public sealed partial class ConstantCSharpPattern : CSharpPattern
    {
        internal ConstantCSharpPattern(CSharpPatternInfo info, ConstantExpression value)
            : base(info)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public override CSharpPatternType PatternType => CSharpPatternType.Constant;

        /// <summary>
        /// Gets the value used for the constant check.
        /// </summary>
        public ConstantExpression Value { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpPattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitConstantPattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="value">The <see cref="Value" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ConstantCSharpPattern Update(ConstantExpression value)
        {
            if (value == Value)
            {
                return this;
            }

            return CSharpPattern.Constant(_info, value);
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

            return CSharpPattern.Constant(PatternInfo(inputType, NarrowedType), Value);
        }

        /// <summary>
        /// Reduces the pattern by applying it the specified object.
        /// </summary>
        /// <param name="object">The object to match the pattern against.</param>
        /// <returns>The expression representing the pattern applied to the specified object.</returns>
        internal override Expression Reduce(Expression @object)
        {
            if (Value.Value == null)
            {
                // REVIEW: Value types, nullable value types, etc.
                //         Ensure no operator== is called.
                return Expression.Equal(Expression.Convert(@object, typeof(object)), ConstantNull);
            }
            else
            {
                MethodInfo? CheckNaN() => Value.Value switch
                {
                    float f when float.IsNaN(f) => WellKnownMembers.FloatIsNaN,
                    double d when double.IsNaN(d) => WellKnownMembers.DoubleIsNaN,
                    _ => null
                };

                var checkNaN = CheckNaN();

                if (checkNaN != null)
                {
                    return Expression.Call(checkNaN, @object);
                }

                return RelationalCSharpPattern.MakeTest(this, @object, ExpressionType.Equal, Value);
            }
        }
    }

    partial class CSharpPattern
    {
        /// <summary>
        /// Creates a constant pattern that checks for equality with the given constant value.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="value">The value used for the constant check.</param>
        /// <returns>A <see cref="ConstantCSharpPattern" /> representing a constant pattern.</returns>
        public static ConstantCSharpPattern Constant(CSharpPatternInfo? info, ConstantExpression value)
        {
            RequiresNotNull(value, nameof(value));

            CheckConstant(value, isRelational: false);

            if (info != null)
            {
                var inputType = value.Value == null ? info.InputType : value.Type;
                RequiresCompatiblePatternTypes(inputType, info.NarrowedType);
            }
            else
            {
                info = PatternInfo(typeof(object), value.Type);
            }

            return new ConstantCSharpPattern(info, value);
        }

        /// <summary>
        /// Creates a constant pattern that checks for equality with the given constant value.
        /// </summary>
        /// <param name="value">The value used for the constant check.</param>
        /// <returns>A <see cref="ConstantCSharpPattern" /> representing a constant pattern.</returns>
        public static ConstantCSharpPattern Constant(ConstantExpression value) => Constant(info: null, value);

        /// <summary>
        /// Creates a pattern that checks for null.
        /// </summary>
        /// <returns>A pattern that checks for null.</returns>
        public static ConstantCSharpPattern Null() => Constant(info: null, ConstantNull);

        // NB: Omitting 'NotNull' because it could be represented as 'is not null' or 'is { }'.
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ConstantCSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitConstantPattern(ConstantCSharpPattern node) =>
            node.Update(
                VisitAndConvert(node.Value, nameof(ConstantCSharpPattern))
            );
    }
}
