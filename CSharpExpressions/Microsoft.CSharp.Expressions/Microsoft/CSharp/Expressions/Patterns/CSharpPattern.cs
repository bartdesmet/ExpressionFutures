// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Linq.Expressions;

using static System.Dynamic.Utils.TypeUtils;

using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a pattern that can be used for pattern matching expressions and statements.
    /// </summary>
    public abstract partial class CSharpPattern
    {
        protected readonly CSharpPatternInfo _info;

        internal CSharpPattern(CSharpPatternInfo info) => _info = info;

        /// <summary>
        /// Gets the <see cref="Type" /> of the input expressions handled by the pattern.
        /// </summary>
        public Type InputType => _info.InputType;

        /// <summary>
        /// Gets the <see cref="Type" /> the pattern narrows the input type to in case of a successful match.
        /// </summary>
        public Type NarrowedType => _info.NarrowedType;

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public abstract CSharpPatternType PatternType { get; }

        /// <summary>
        /// Changes the input type to the specified type.
        /// </summary>
        /// <remarks>
        /// This functionality can be used when a pattern is pass to an expression or statement that applies the pattern.
        /// </remarks>
        /// <param name="inputType">The new input type.</param>
        /// <returns>The original pattern rewritten to use the specified input type.</returns>
        public abstract CSharpPattern ChangeType(Type inputType);

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal abstract CSharpPattern Accept(CSharpExpressionVisitor visitor);

        /// <summary>
        /// Reduces the pattern by applying it the specified object.
        /// </summary>
        /// <param name="object">The object to match the pattern against.</param>
        /// <returns>The expression representing the pattern applied to the specified object.</returns>
        internal abstract Expression Reduce(Expression @object);

        internal static void RequiresCompatiblePatternTypes(Type firstType, Type secondType)
        {
            if (firstType != secondType)
                throw Error.PatternTypeMismatch(firstType, secondType);
        }

        internal static void ValidatePatternType(Type type)
        {
            ValidateType(type);

            if (type.IsByRef)
                throw Error.TypeMustNotBeByRef();
            if (type.IsPointer)
                throw Error.TypeMustNotBePointer();
            if (type == typeof(void))
                throw LinqError.ArgumentCannotBeOfTypeVoid();
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="CSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitPattern(CSharpPattern node) => node.Accept(this);
    }
}
