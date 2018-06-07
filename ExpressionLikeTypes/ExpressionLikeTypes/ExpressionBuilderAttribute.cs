//
// Examples of the expression type feature discussed at
// https://github.com/bartdesmet/roslyn/blob/ExpressionTreeLikeTypes/docs/features/expression-types.md
//
// bartde - June 2018
//

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExpressionBuilderAttribute : Attribute
    {
        public ExpressionBuilderAttribute(Type type) { }
    }
}
