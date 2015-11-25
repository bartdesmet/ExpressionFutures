// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Xml.Linq;

namespace System.Linq.Expressions
{
    // DESIGN: Maybe we just add an ExpressionVisitor<T> to the BCL?

    /// <summary>
    /// Interface for expression visitors that produce a debug view.
    /// </summary>
    public interface IDebugViewExpressionVisitor
    {
        /// <summary>
        /// Gets the debug view for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to get a debug view for.</param>
        /// <returns>Debug view for the specified expression.</returns>
        XNode GetDebugView(Expression expression);

        /// <summary>
        /// Gets the debug view for the specified label.
        /// </summary>
        /// <param name="label">The label to get a debug view for.</param>
        /// <returns>Debug view for the specified label.</returns>
        XNode GetDebugView(LabelTarget label);

        /// <summary>
        /// Gets the debug view for the specified member binding.
        /// </summary>
        /// <param name="binding">The member binding to get a debug view for.</param>
        /// <returns>Debug view for the specified member binding.</returns>
        XNode GetDebugView(MemberBinding binding);

        /// <summary>
        /// Gets the debug view for the specified element initializer.
        /// </summary>
        /// <param name="initializer">The element initializer to get a debug view for.</param>
        /// <returns>Debug view for the specified element initializer.</returns>
        XNode GetDebugView(ElementInit initializer);

        /// <summary>
        /// Gets the debug view for the specified catch block.
        /// </summary>
        /// <param name="catchBlock">The catch block to get a debug view for.</param>
        /// <returns>Debug view for the specified catch block.</returns>
        XNode GetDebugView(CatchBlock catchBlock);

        /// <summary>
        /// Gets the debug view for the specified switch case.
        /// </summary>
        /// <param name="switchCase">The switch case to get a debug view for.</param>
        /// <returns>Debug view for the specified switch case.</returns>
        XNode GetDebugView(SwitchCase switchCase);
    }
}
