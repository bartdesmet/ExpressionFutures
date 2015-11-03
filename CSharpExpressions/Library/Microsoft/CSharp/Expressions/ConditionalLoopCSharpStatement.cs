// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Base class for C# conditional loop statement nodes.
    /// </summary>
    public abstract class ConditionalLoopCSharpStatement : LoopCSharpStatement
    {
        internal ConditionalLoopCSharpStatement(Expression test, Expression body, LabelTarget breakLabel, LabelTarget continueLabel)
            : base(body, breakLabel, continueLabel)
        {
            Test = test;
        }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the loop condition.
        /// </summary>
        public Expression Test { get; }
    }

    partial class CSharpExpression
    {
        private static void ValidateLoop(Expression test, Expression body, LabelTarget @break, LabelTarget @continue)
        {
            ValidateCondition(test);

            ValidateLoop(body, @break, @continue);
        }
    }
}
