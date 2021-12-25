// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a subpattern that matches a property or field on an object.
    /// </summary>
    public sealed partial class PropertyCSharpSubpattern : CSharpSubpattern
    {
        internal PropertyCSharpSubpattern(CSharpPattern pattern, PropertyCSharpSubpatternMember member, bool isLengthOrCount)
            : base(pattern)
        {
            Member = member;
            IsLengthOrCount = isLengthOrCount;
        }

        /// <summary>
        /// Gets the type of the subpattern.
        /// </summary>
        public override CSharpSubpatternType SubpatternType => CSharpSubpatternType.Property;

        /// <summary>
        /// Gets the property or field to match on.
        /// </summary>
        public PropertyCSharpSubpatternMember Member { get; }

        /// <summary>
        /// Gets a value indicating whether the member is a length or count property, which is guaranteed to return a positive integer value.
        /// </summary>
        public bool IsLengthOrCount { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpSubpattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitPropertySubpattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="pattern">The <see cref="Pattern" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public PropertyCSharpSubpattern Update(CSharpPattern pattern)
        {
            if (pattern == Pattern)
            {
                return this;
            }

            return CSharpPattern.PropertySubpattern(pattern, Member, IsLengthOrCount);
        }

        internal Expression Reduce(Expression @object, List<ParameterExpression> vars, List<Expression> stmts, Action<Expression> addFailIfNot)
        {
            // NB: LengthOrCount is only used to refine the range of Int32 to >= 0. We don't yet use it for reduction.

            Expression createTest(Expression obj)
            {
                return Pattern.Reduce(Member.Reduce(obj, vars, stmts, addFailIfNot));
            }

            return PatternHelpers.Reduce(@object, createTest, vars, stmts);
        }
    }

    partial class CSharpPattern
    {
        // REVIEW: The parameter order is not left-to-right compared to the language grammar.

        /// <summary>
        /// Creates a property subpattern that matches a property or field on an object.
        /// </summary>
        /// <param name="pattern">The pattern to apply to the object in the corresponding position.</param>
        /// <param name="member">The property or field to match on.</param>
        /// <param name="isLengthOrCount">A value indicating whether the member is a length or count property.</param>
        /// <returns>A <see cref="PropertyCSharpSubpattern" /> representing a property subpattern.</returns>
        public static PropertyCSharpSubpattern PropertySubpattern(CSharpPattern pattern, PropertyCSharpSubpatternMember member, bool isLengthOrCount)
        {
            RequiresNotNull(pattern, nameof(pattern));
            RequiresNotNull(member, nameof(member));

            RequiresCompatiblePatternTypes(member.Type, pattern.InputType);

            return new PropertyCSharpSubpattern(pattern, member, isLengthOrCount);
        }

        /// <summary>
        /// Creates a property subpattern that matches a property or field on an object.
        /// </summary>
        /// <param name="pattern">The pattern to apply to the object in the corresponding position.</param>
        /// <param name="member">The property or field to match on.</param>
        /// <returns>A <see cref="PropertyCSharpSubpattern" /> representing a property subpattern.</returns>
        public static PropertyCSharpSubpattern PropertySubpattern(CSharpPattern pattern, MemberInfo member)
        {
            var subpatternMember = PropertySubpatternMember(member);

            return PropertySubpattern(pattern, subpatternMember, subpatternMember.IsLengthOrCount);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="PropertyCSharpSubpattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual PropertyCSharpSubpattern VisitPropertySubpattern(PropertyCSharpSubpattern node) =>
            node.Update(
                VisitPattern(node.Pattern)
            );
    }
}
