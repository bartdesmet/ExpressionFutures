// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Base class for C# conditional loop statement nodes.
    /// </summary>
    public abstract partial class ConditionalLoopCSharpStatement : LoopCSharpStatement
    {
        internal ConditionalLoopCSharpStatement(Expression? test, Expression body, LabelTarget? breakLabel, LabelTarget? continueLabel, ReadOnlyCollection<ParameterExpression> locals)
            : base(body, breakLabel, continueLabel)
        {
            Test = test;
            Locals = locals;
        }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the loop condition.
        /// </summary>
        public Expression? Test { get; }

        /// <summary>
        /// Gets a collection of <see cref="ParameterExpression"/> representing the variables that are in scope of the loop.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Locals { get; }
    }

    partial class CSharpExpression
    {
        private static void ValidateLoop(Expression? test, Expression body, LabelTarget? @break, LabelTarget? @continue, bool optionalTest = false)
        {
            ValidateCondition(test, optionalTest);

            ValidateLoop(body, @break, @continue);
        }
    }
}
