// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    internal struct LeaveLabelData
    {
        public int Index;
        public LabelTarget Target;
        public ParameterExpression Value;
    }
}
