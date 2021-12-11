// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Linq.Expressions;
using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a pattern that checks convertibility to a type.
    /// </summary>
    public sealed class TypeCSharpPattern : CSharpPattern
    {
        internal TypeCSharpPattern(CSharpPatternInfo info, Type type)
            : base(info)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public override CSharpPatternType PatternType => CSharpPatternType.Type;

        /// <summary>
        /// Gets the type to check for.
        /// </summary>
        public new Type Type { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpPattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitTypePattern(this);

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

            // NB: This can fail if there's a variable and the input type is not compatible. See Var factory.

            return CSharpPattern.Type(PatternInfo(inputType, Type), Type);
        }

        /// <summary>
        /// Reduces the pattern by applying it the specified object.
        /// </summary>
        /// <param name="object">The object to match the pattern against.</param>
        /// <returns>The expression representing the pattern applied to the specified object.</returns>
        internal override Expression Reduce(Expression @object)
        {
            return Expression.TypeIs(@object, Type);
        }
    }

    partial class CSharpPattern
    {
        /// <summary>
        /// Creates a pattern that checks convertibility to a type.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="type">The type to check for.</param>
        /// <returns>A <see cref="TypeCSharpPattern" /> that represents a pattern that checks for a type.</returns>
        public static TypeCSharpPattern Type(CSharpPatternInfo info, Type type)
        {
            RequiresNotNull(type, nameof(type));

            ValidatePatternType(type);

            if (info != null)
            {
                RequiresCompatiblePatternTypes(type, info.NarrowedType);
            }
            else
            {
                info = PatternInfo(typeof(object), type);
            }

            return new TypeCSharpPattern(info, type);
        }

        /// <summary>
        /// Creates a pattern that checks convertibility to a type.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <returns>A <see cref="TypeCSharpPattern" /> that represents a pattern that checks for a type.</returns>
        public static TypeCSharpPattern Type(Type type) => Type(info: null, type);
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="TypeCSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitTypePattern(TypeCSharpPattern node) => node;
    }
}
