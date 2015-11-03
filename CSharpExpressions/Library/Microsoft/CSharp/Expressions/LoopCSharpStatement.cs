// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;
using static System.Dynamic.Utils.ContractUtils;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Base class for C# loop statement nodes.
    /// </summary>
    public abstract class LoopCSharpStatement : CSharpStatement
    {
        internal LoopCSharpStatement(Expression body, LabelTarget breakLabel, LabelTarget continueLabel)
        {
            Body = body;
            BreakLabel = breakLabel;
            ContinueLabel = continueLabel;
        }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the body of the loop.
        /// </summary>
        public Expression Body { get; }

        /// <summary>
        /// Gets the break target used by the loop body.
        /// </summary>
        public LabelTarget BreakLabel { get; }

        /// <summary>
        /// Gets the continue target used by the loop body.
        /// </summary>
        public LabelTarget ContinueLabel { get; }
    }

    partial class CSharpExpression
    {
        internal static void ValidateLoop(Expression body, LabelTarget @break, LabelTarget @continue)
        {
            RequiresCanRead(body, nameof(body));
            
            if (@break != null && @break.Type != typeof(void))
            {
                // DESIGN: C# statement behavior; can be revisited.
                throw LinqError.LabelTypeMustBeVoid();
            }

            if (@continue != null && @continue.Type != typeof(void))
            {
                throw LinqError.LabelTypeMustBeVoid();
            }
        }
    }
}
