// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Diagnostics.CodeAnalysis;
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
        [return: NotNullIfNotNull("expression")] // TODO: C# 11.0 nameof
        XNode? GetDebugView(Expression? expression);

        /// <summary>
        /// Gets the debug view for the specified label.
        /// </summary>
        /// <param name="label">The label to get a debug view for.</param>
        /// <returns>Debug view for the specified label.</returns>
        [return: NotNullIfNotNull("label")] // TODO: C# 11.0 nameof
        XNode? GetDebugView(LabelTarget? label);

        /// <summary>
        /// Gets the debug view for the specified member binding.
        /// </summary>
        /// <param name="binding">The member binding to get a debug view for.</param>
        /// <returns>Debug view for the specified member binding.</returns>
        [return: NotNullIfNotNull("binding")] // TODO: C# 11.0 nameof
        XNode? GetDebugView(MemberBinding? binding);

        /// <summary>
        /// Gets the debug view for the specified element initializer.
        /// </summary>
        /// <param name="initializer">The element initializer to get a debug view for.</param>
        /// <returns>Debug view for the specified element initializer.</returns>
        [return: NotNullIfNotNull("initializer")] // TODO: C# 11.0 nameof
        XNode? GetDebugView(ElementInit? initializer);

        /// <summary>
        /// Gets the debug view for the specified catch block.
        /// </summary>
        /// <param name="catchBlock">The catch block to get a debug view for.</param>
        /// <returns>Debug view for the specified catch block.</returns>
        [return: NotNullIfNotNull("catchBlock")] // TODO: C# 11.0 nameof
        XNode? GetDebugView(CatchBlock? catchBlock);

        /// <summary>
        /// Gets the debug view for the specified switch case.
        /// </summary>
        /// <param name="switchCase">The switch case to get a debug view for.</param>
        /// <returns>Debug view for the specified switch case.</returns>
        [return: NotNullIfNotNull("switchCase")] // TODO: C# 11.0 nameof
        XNode? GetDebugView(SwitchCase? switchCase);

        /// <summary>
        /// Gets a unique instance identifier for the specified object.
        /// </summary>
        /// <param name="value">The object to get an instance identifier for.</param>
        /// <returns>A unique instance identifier for the specified object.</returns>
        int MakeInstanceId(object value);
    }
}
