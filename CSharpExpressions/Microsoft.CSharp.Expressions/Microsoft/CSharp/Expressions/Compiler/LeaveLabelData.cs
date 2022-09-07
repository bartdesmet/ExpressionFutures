// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Struct to keep track of branching information used during rewrites of e.g. branches out of protected regions
    /// in exception handling constructs. This information is used to pend and unpend branches.
    /// </summary>
    internal struct LeaveLabelData
    {
        public int Index;
        public LabelTarget Target;
        public ParameterExpression? Value;
    }
}
