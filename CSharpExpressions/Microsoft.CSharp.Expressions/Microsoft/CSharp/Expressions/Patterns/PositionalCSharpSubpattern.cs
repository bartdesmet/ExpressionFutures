// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Dynamic.Utils;
using System.Reflection;
using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a subpattern that matches a position in a tuple or deconstructible object.
    /// </summary>
    public abstract partial class PositionalCSharpSubpattern : CSharpSubpattern
    {
        internal PositionalCSharpSubpattern(CSharpPattern pattern)
            : base(pattern)
        {
        }

        /// <summary>
        /// Gets the type of the subpattern.
        /// </summary>
        public override CSharpSubpatternType SubpatternType => CSharpSubpatternType.Positional;

        /// <summary>
        /// Gets the field of the tuple corresponding to the position being matched.
        /// </summary>
        public abstract TupleFieldInfo Field { get; }

        /// <summary>
        /// Gets the parameter of the Deconstruct method corresponding to the position being matched.
        /// </summary>
        public abstract ParameterInfo Parameter { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpSubpattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitPositionalSubpattern(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="pattern">The <see cref="Pattern" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public PositionalCSharpSubpattern Update(CSharpPattern pattern)
        {
            if (pattern == Pattern)
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
        // REVIEW: The parameter order is not left-to-right compared to the language grammar.

        /// <summary>
        /// Creates a positional subpattern that matches a position computed by the index of its occurrence in the parent pattern.
        /// </summary>
        /// <param name="pattern">The pattern to apply to the object in the corresponding position.</param>
        /// <returns>A <see cref="PositionalCSharpSubpattern" /> representing a positional subpattern.</returns>
        public static PositionalCSharpSubpattern PositionalSubpattern(CSharpPattern pattern)
        {
            RequiresNotNull(pattern, nameof(pattern));

            return new PositionalCSharpSubpattern.Simple(pattern);
        }

        /// <summary>
        /// Creates a positional subpattern that matches a component of a tuple.
        /// </summary>
        /// <param name="pattern">The pattern to apply to the object in the corresponding position.</param>
        /// <param name="field">The field in the tuple to match.</param>
        /// <returns>A <see cref="PositionalCSharpSubpattern" /> representing a positional subpattern.</returns>
        public static PositionalCSharpSubpattern PositionalSubpattern(CSharpPattern pattern, TupleFieldInfo field)
        {
            RequiresNotNull(pattern, nameof(pattern));
            RequiresNotNull(field, nameof(field));

            if (field.Index < 0)
                throw Error.TupleFieldIndexMustBePositive();

            return new PositionalCSharpSubpattern.WithField(pattern, field);
        }

        /// <summary>
        /// Creates a positional subpattern that matches a component extracted by calling a Deconstruct method.
        /// </summary>
        /// <param name="pattern">The pattern to apply to the object in the corresponding position.</param>
        /// <param name="parameter">The parameter on the Deconstruct method used to extract the object to match on.</param>
        /// <returns>A <see cref="PositionalCSharpSubpattern" /> representing a positional subpattern.</returns>
        public static PositionalCSharpSubpattern PositionalSubpattern(CSharpPattern pattern, ParameterInfo parameter)
        {
            RequiresNotNull(pattern, nameof(pattern));
            RequiresNotNull(parameter, nameof(parameter));

            if (!(parameter.Member is MethodInfo))
                throw Error.PositionalPatternParameterMustBeOnMethod(parameter);

            if (!parameter.IsOut)
                throw Error.PositionalPatternParameterMustBeOut(parameter);

            return new PositionalCSharpSubpattern.WithParameter(pattern, parameter);
        }

        /// <summary>
        /// Creates a positional subpattern that matches a component extracted by calling a Deconstruct method.
        /// </summary>
        /// <param name="pattern">The pattern to apply to the object in the corresponding position.</param>
        /// <param name="method">The Deconstruct method used to extract the object to match on.</param>
        /// <param name="parameterIndex">The index of the parameter on the Deconstruct method used to extract the object to match on.</param>
        /// <returns>A <see cref="PositionalCSharpSubpattern" /> representing a positional subpattern.</returns>
        public static PositionalCSharpSubpattern PositionalSubpattern(CSharpPattern pattern, MethodInfo method, int parameterIndex)
        {
            RequiresNotNull(method, nameof(method));

            var parameters = method.GetParametersCached();

            if (parameterIndex < 0 || parameterIndex >= parameters.Length)
            {
                throw Error.ParameterIndexOutOfBounds(parameterIndex, method.Name);
            }

            return PositionalSubpattern(pattern, parameters[parameterIndex]);
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
        protected internal virtual PositionalCSharpSubpattern VisitPositionalSubpattern(PositionalCSharpSubpattern node) => node.Update(VisitPattern(node.Pattern));
    }
}
