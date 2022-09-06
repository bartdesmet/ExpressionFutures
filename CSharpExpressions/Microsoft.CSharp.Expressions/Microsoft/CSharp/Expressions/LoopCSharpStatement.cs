// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System.Linq.Expressions;

using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Base class for C# loop statement nodes.
    /// </summary>
    public abstract partial class LoopCSharpStatement : CSharpStatement
    {
        internal LoopCSharpStatement(Expression body, LabelTarget? breakLabel, LabelTarget? continueLabel)
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
        public LabelTarget? BreakLabel { get; }

        /// <summary>
        /// Gets the continue target used by the loop body.
        /// </summary>
        public LabelTarget? ContinueLabel { get; }
    }

    partial class CSharpExpression
    {
        internal static void ValidateLoop(Expression body, LabelTarget? @break, LabelTarget? @continue)
        {
            RequiresCanRead(body, nameof(body));
            
            // DESIGN: C# statement behavior; can be revisited.
            if (@break != null && @break.Type != typeof(void))
                throw LabelTypeMustBeVoid(nameof(@break));

            if (@continue != null && @continue.Type != typeof(void))
                throw LabelTypeMustBeVoid(nameof(@continue));

            if (@break != null && @continue != null && @break == @continue)
                throw Error.DuplicateLabels();
        }
    }
}
