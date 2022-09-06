// Prototyping extended expression trees for C#.
//
// bartde - December 2021

#nullable enable

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Base class for patterns that assign the result to a variable upon a successful match.
    /// </summary>
    public abstract partial class CSharpObjectPattern : CSharpPattern
    {
        /// <summary>
        /// Pattern info.
        /// </summary>
        private readonly CSharpObjectPatternInfo _objectInfo;

        internal CSharpObjectPattern(CSharpObjectPatternInfo info)
            : base(info.Info)
        {
            _objectInfo = info;
        }

        /// <summary>
        /// Gets the variable to assign to.
        /// </summary>
        public ParameterExpression? Variable => _objectInfo.Variable;
    }
}
