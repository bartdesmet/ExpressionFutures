// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Reflection;

namespace Microsoft.CSharp.Expressions
{
    public abstract class PositionalCSharpSubpattern : CSharpSubpattern
    {
        internal PositionalCSharpSubpattern(CSharpPattern pattern)
            : base(pattern)
        {
        }

        public override CSharpSubpatternType SubpatternType => CSharpSubpatternType.Positional;

        public abstract TupleFieldInfo Field { get; }
        public abstract ParameterInfo Parameter { get; }

        protected internal override CSharpSubpattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitPositionalSubpattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="pattern">The <see cref="Pattern" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public PositionalCSharpSubpattern Update(CSharpPattern pattern)
        {
            if (pattern == this.Pattern)
            {
                return this;
            }

            return Rewrite(pattern);
        }

        protected abstract PositionalCSharpSubpattern Rewrite(CSharpPattern pattern);

        internal sealed class WithField : PositionalCSharpSubpattern
        {
            internal WithField(CSharpPattern pattern, TupleFieldInfo field)
                : base(pattern)
            {
                Field = field;
            }

            public override TupleFieldInfo Field { get; }
            public override ParameterInfo Parameter => null;

            protected override PositionalCSharpSubpattern Rewrite(CSharpPattern pattern) => CSharpPattern.PositionalSubpattern(pattern, Field);
        }

        internal sealed class WithParameter : PositionalCSharpSubpattern
        {
            internal WithParameter(CSharpPattern pattern, ParameterInfo parameter)
                : base(pattern)
            {
                Parameter = parameter;
            }

            public override TupleFieldInfo Field => null;
            public override ParameterInfo Parameter { get; }

            protected override PositionalCSharpSubpattern Rewrite(CSharpPattern pattern) => CSharpPattern.PositionalSubpattern(pattern, Parameter);
        }

        internal sealed class Simple : PositionalCSharpSubpattern
        {
            internal Simple(CSharpPattern pattern)
                : base(pattern)
            {
            }

            public override TupleFieldInfo Field => null;
            public override ParameterInfo Parameter => null;

            protected override PositionalCSharpSubpattern Rewrite(CSharpPattern pattern) => CSharpPattern.PositionalSubpattern(pattern);
        }
    }

    partial class CSharpPattern
    {
        public static PositionalCSharpSubpattern PositionalSubpattern(CSharpPattern pattern)
        {
            return new PositionalCSharpSubpattern.Simple(pattern);
        }

        public static PositionalCSharpSubpattern PositionalSubpattern(CSharpPattern pattern, string fieldName, int fieldIndex)
        {
            return PositionalSubpattern(pattern, new TupleFieldInfo(fieldName, fieldIndex));
        }

        public static PositionalCSharpSubpattern PositionalSubpattern(CSharpPattern pattern, TupleFieldInfo field)
        {
            return new PositionalCSharpSubpattern.WithField(pattern, field);
        }

        public static PositionalCSharpSubpattern PositionalSubpattern(CSharpPattern pattern, ParameterInfo parameter)
        {
            return new PositionalCSharpSubpattern.WithParameter(pattern, parameter);
        }

        public static PositionalCSharpSubpattern PositionalSubpattern(CSharpPattern pattern, MethodInfo method, int parameterIndex)
        {
            return PositionalSubpattern(pattern, method.GetParameters()[parameterIndex]);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="PositionalCSharpSubpattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual PositionalCSharpSubpattern VisitPositionalSubpattern(PositionalCSharpSubpattern node)
        {
            return node.Update(VisitPattern(node.Pattern));
        }
    }
}
