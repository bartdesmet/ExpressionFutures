// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    public sealed class PropertyCSharpSubpattern : CSharpSubpattern
    {
        internal PropertyCSharpSubpattern(CSharpPattern pattern, PropertyCSharpSubpatternMember member, bool isLengthOrCount)
            : base(pattern)
        {
            Member = member;
            IsLengthOrCount = isLengthOrCount;
        }

        public override CSharpSubpatternType SubpatternType => CSharpSubpatternType.Property;

        public PropertyCSharpSubpatternMember Member { get; }
        public bool IsLengthOrCount { get; }

        protected internal override CSharpSubpattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitPropertySubpattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="pattern">The <see cref="Pattern" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public PropertyCSharpSubpattern Update(CSharpPattern pattern)
        {
            if (pattern == this.Pattern)
            {
                return this;
            }

            return CSharpPattern.PropertySubpattern(pattern, Member, IsLengthOrCount);
        }

        internal Expression Reduce(Expression @object)
        {
            // NB: LengthOrCount is only used to refine the range of Int32 to >= 0. We don't yet use it for reduction.

            Expression createTest(Expression obj)
            {
                return Pattern.Reduce(Member.Reduce(obj));
            }

            return PatternHelpers.Reduce(@object, createTest);
        }
    }

    partial class CSharpPattern
    {
        public static PropertyCSharpSubpattern PropertySubpattern(CSharpPattern pattern, PropertyCSharpSubpatternMember member, bool isLengthOrCount)
        {
            return new PropertyCSharpSubpattern(pattern, member, isLengthOrCount);
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
        protected internal virtual PropertyCSharpSubpattern VisitPropertySubpattern(PropertyCSharpSubpattern node)
        {
            return node.Update(VisitPattern(node.Pattern));
        }
    }
}
