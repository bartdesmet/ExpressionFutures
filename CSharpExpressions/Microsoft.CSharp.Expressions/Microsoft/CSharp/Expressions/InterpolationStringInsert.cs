// Prototyping extended expression trees for C#.
//
// bartde - February 2020

#nullable enable

using System.Linq.Expressions;

using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents an interpolation containing a string insert with optional format and alignment specifiers.
    /// </summary>
    public sealed partial class InterpolationStringInsert : Interpolation
    {
        internal InterpolationStringInsert(Expression value, string? format, int? alignment)
        {
            Value = value;
            Format = format;
            Alignment = alignment;
        }

        /// <summary>
        /// Gets the expression representing the value being interpolated.
        /// </summary>
        public Expression Value { get; }

        /// <summary>
        /// Gets the format specifier, if any.
        /// </summary>
        public string? Format { get; }

        /// <summary>
        /// Gets the alignment specifier, if any.
        /// </summary>
        public int? Alignment { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Interpolation Accept(CSharpExpressionVisitor visitor) => visitor.VisitInterpolationStringInsert(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="value">The <see cref="Value" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public InterpolationStringInsert Update(Expression value)
        {
            if (value == Value)
            {
                return this;
            }

            return CSharpExpression.InterpolationStringInsert(value, Format, Alignment);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.InterpolationStringInsert"/> that represents an interpolation containing a string insert.
        /// </summary>
        /// <param name="value">The expression representing the value being interpolated.</param>
        /// <returns>An instance of the <see cref="Microsoft.CSharp.Expressions.InterpolationStringInsert"/>.</returns>
        public static InterpolationStringInsert InterpolationStringInsert(Expression value) => InterpolationStringInsert(value, format: null, alignment: null);

        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.InterpolationStringInsert"/> that represents an interpolation containing a string insert with an optional format specifier.
        /// </summary>
        /// <param name="value">The expression representing the value being interpolated.</param>
        /// <param name="format">The expression representing the format specifier, if any.</param>
        /// <returns>An instance of the <see cref="Microsoft.CSharp.Expressions.InterpolationStringInsert"/>.</returns>
        public static InterpolationStringInsert InterpolationStringInsert(Expression value, string? format) => InterpolationStringInsert(value, format, alignment: null);

        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.InterpolationStringInsert"/> that represents an interpolation containing a string insert with an optional alignment specifier.
        /// </summary>
        /// <param name="value">The expression representing the value being interpolated.</param>
        /// <param name="alignment">The expression representing the alignment specifier, if any.</param>
        /// <returns>An instance of the <see cref="Microsoft.CSharp.Expressions.InterpolationStringInsert"/>.</returns>
        public static InterpolationStringInsert InterpolationStringInsert(Expression value, int? alignment) => InterpolationStringInsert(value, format: null, alignment);

        /// <summary>
        /// Creates a <see cref="Microsoft.CSharp.Expressions.InterpolationStringInsert"/> that represents an interpolation containing a string insert with optional format and alignment specifiers.
        /// </summary>
        /// <param name="value">The expression representing the value being interpolated.</param>
        /// <param name="format">The expression representing the format specifier, if any.</param>
        /// <param name="alignment">The expression representing the alignment specifier, if any.</param>
        /// <returns>An instance of the <see cref="Microsoft.CSharp.Expressions.InterpolationStringInsert"/>.</returns>
        public static InterpolationStringInsert InterpolationStringInsert(Expression value, string? format, int? alignment)
        {
            RequiresCanRead(value, nameof(value));

            if (format != null && format.Length == 0)
                throw Error.EmptyFormatSpecifier();

            return new InterpolationStringInsert(value, format, alignment);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="InterpolationStringInsert" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Interpolation VisitInterpolationStringInsert(InterpolationStringInsert node) =>
            node.Update(
                Visit(node.Value)
            );
    }
}
