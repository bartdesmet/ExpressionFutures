// Prototyping extended expression trees for C#.
//
// bartde - November 2015

#nullable enable

using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    // NB: We have a separate node for switch statements in order to address a few shortcomings in LINQ:
    //     - Ability to have an empty switch
    //     - Support for "goto case" and "goto default" in case bodies

    /// <summary>
    /// Base class for switch statements.
    /// </summary>
    public abstract partial class SwitchCSharpStatementBase : CSharpStatement
    {
        internal SwitchCSharpStatementBase(Expression switchValue, LabelTarget breakLabel, ReadOnlyCollection<ParameterExpression> variables)
        {
            SwitchValue = switchValue;
            BreakLabel = breakLabel;
            Variables = variables;
        }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the value to be tested against each case.
        /// </summary>
        public Expression SwitchValue { get; }

        /// <summary>
        /// Gets the <see cref="LabelTarget"/> representing the break label of the switch statement.
        /// </summary>
        public LabelTarget BreakLabel { get; }

        /// <summary>
        /// Gets a collection of variables in scope for the switch cases.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Variables { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Switch;
    }
}
