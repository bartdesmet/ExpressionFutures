// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

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
        private static void ValidateLoop(Expression test, Expression body, ref LabelTarget @break, LabelTarget @continue)
        {
            RequiresCanRead(test, nameof(test));
            
            // TODO: We can be more flexible and allow the rules in C# spec 7.20.
            //       Note that this behavior is the same as IfThen, but we could also add C# specific nodes for those,
            //       with the more flexible construction behavior.
            if (test.Type != typeof(bool))
            {
                throw LinqError.ArgumentMustBeBoolean();
            }

            ValidateLoop(body, ref @break, @continue);
        }
    }
}
