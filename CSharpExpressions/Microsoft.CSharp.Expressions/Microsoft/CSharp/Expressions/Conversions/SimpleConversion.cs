// Prototyping extended expression trees for C#.
//
// bartde - December 2021

#nullable enable

using System;
using System.Linq.Expressions;
using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a simple conversion of an object, i.e. a conversion that does not involve deconstruction.
    /// </summary>
    public sealed partial class SimpleConversion : Conversion
    {
        internal SimpleConversion(LambdaExpression conversion)
        {
            Conversion = conversion;
        }

        /// <summary>
        /// Gets the input type of the conversion.
        /// </summary>
        public override Type InputType => Conversion.Parameters[0].Type;

        /// <summary>
        /// Gets the result type of the conversion.
        /// </summary>
        public override Type ResultType => Conversion.ReturnType;

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the conversion of an object.
        /// </summary>
        public LambdaExpression Conversion { get; }

        /// <summary>
        /// Creates a new conversion that is like this one, but using the supplied children. If all of the children are the same, it will return this conversion.
        /// </summary>
        /// <param name="conversion">The <see cref="Conversion" /> property of the result.</param>
        /// <returns>This conversion if no children changed, or an conversion with the updated children.</returns>
        public SimpleConversion Update(LambdaExpression conversion)
        {
            if (conversion == Conversion)
            {
                return this;
            }

            return CSharpExpression.Convert(conversion);
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override Conversion Accept(CSharpExpressionVisitor visitor) => visitor.VisitSimpleConversion(this);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="SimpleConversion" />.
        /// </summary>
        /// <param name="node">The conversion to visit.</param>
        /// <returns>The modified conversion, if it or any subexpression was modified; otherwise, returns the original conversion.</returns>
        protected internal virtual Conversion VisitSimpleConversion(SimpleConversion node)
        {
            return node.Update(VisitAndConvert(node.Conversion, nameof(VisitSimpleConversion)));
        }
    }

    partial class CSharpExpression
    {
        // CONSIDER: Should we introduce CSharpConversion and use that as the factory?

        /// <summary>
        /// Creates a simple conversion using the specified <paramref name="conversion"/> lambda.
        /// </summary>
        /// <param name="conversion">The lambda expression representing the conversion.</param>
        /// <returns>A <see cref="SimpleConversion"/> object.</returns>
        public static SimpleConversion Convert(LambdaExpression conversion)
        {
            RequiresNotNull(conversion, nameof(conversion));

            if (conversion.Parameters.Count != 1)
                throw Error.ConversionShouldHaveOneParameter();

            if (conversion.ReturnType == typeof(void))
                throw Error.ConversionCannotReturnVoid();

            return new SimpleConversion(conversion);
        }

        // CONSIDER: Add specialized derived types of SimpleConversion for trivial conversions:
        //
        //             Identity:          (T x) => x
        //             Convert[Checked]:  (T x) => (R)x    with/without Method
        //             Nullable:          (T x) => (T?)x
    }
}
