// Prototyping extended expression trees for C#.
//
// bartde - December 2015

namespace System.Linq.Expressions
{
    /// <summary>
    /// Interface for expressions that are printable as C# fragments.
    /// </summary>
    public interface ICSharpPrintableExpression
    {
        /// <summary>
        /// Gets the precedence level of the expression.
        /// </summary>
        int Precedence { get; }

        /// <summary>
        /// Gets a value indicating whether the node represents a statement.
        /// </summary>
        bool IsStatement { get; }

        /// <summary>
        /// Gets a value indicating whether the node represents an operation that supports overflow checking.
        /// </summary>
        bool HasCheckedMode { get; }

        /// <summary>
        /// Gets a value indicating whether the node performs overflow checking.
        /// </summary>
        bool IsChecked { get; }

        /// <summary>
        /// Gets a value indicating whether the node represents a lambda expression.
        /// </summary>
        bool IsLambda { get; }

        /// <summary>
        /// Gets a value indicating whether the node represents a block expression.
        /// </summary>
        bool IsBlock { get; }

        /// <summary>
        /// Dispatches the current node to the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor to dispatch to.</param>
        /// <returns>The result of visiting the node.</returns>
        void Accept(ICSharpPrintingVisitor visitor);
    }
}
