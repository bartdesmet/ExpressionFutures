// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Base class for patterns that assign the result to a variable upon a successful match.
    /// </summary>
    public abstract class CSharpObjectPattern : CSharpPattern
    {
        internal CSharpObjectPattern(CSharpObjectPatternInfo info)
            : base(info.Info)
        {
            Variable = info.Variable;
        }

        /// <summary>
        /// Gets the variable to assign to.
        /// </summary>
        public ParameterExpression Variable { get; }
    }
}
