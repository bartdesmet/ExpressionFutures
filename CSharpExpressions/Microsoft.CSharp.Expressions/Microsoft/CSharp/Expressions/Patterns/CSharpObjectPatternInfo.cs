// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    public sealed class CSharpObjectPatternInfo
    {
        internal CSharpObjectPatternInfo(CSharpPatternInfo info, ParameterExpression variable)
        {
            Info = info;
            Variable = variable;
        }

        public CSharpPatternInfo Info { get; }
        public ParameterExpression Variable { get; }
    }

    partial class CSharpPattern
    {
        public static CSharpObjectPatternInfo ObjectPatternInfo(CSharpPatternInfo info, ParameterExpression variable)
        {
            // TODO: If there's a variable, its type should be the narrowed type.
            // TODO: Support no variable.

            info ??= PatternInfo(typeof(object), variable.Type);

            return new CSharpObjectPatternInfo(info, variable);
        }
    }
}
