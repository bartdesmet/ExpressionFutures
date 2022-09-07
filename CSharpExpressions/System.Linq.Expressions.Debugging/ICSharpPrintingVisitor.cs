// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Interface for expression visitors that produce a C# syntax representation.
    /// </summary>
    public interface ICSharpPrintingVisitor
    {
        /// <summary>
        /// Gets or sets a value indicating whether the visitor is currently in a checked context.
        /// </summary>
        bool InCheckedContext { get; set; }

        /// <summary>
        /// Increases the indentation level.
        /// </summary>
        void Indent();

        /// <summary>
        /// Decreases the indentation level.
        /// </summary>
        void Dedent();

        /// <summary>
        /// Emits a new line to the printer.
        /// </summary>
        void NewLine();

        /// <summary>
        /// Emits the specified string to the printer.
        /// </summary>
        /// <param name="value">The string to emit.</param>
        void Out(string? value);

        /// <summary>
        /// Emits the specified value to the printer.
        /// </summary>
        /// <param name="value">The value to emit.</param>
        /// <param name="type">The type to emit the value as.</param>
        void Literal(object? value, Type type);

        /// <summary>
        /// Emits syntax for a static method call.
        /// </summary>
        /// <param name="method">The method to call.</param>
        /// <param name="args">The arguments passed to the method.</param>
        void StaticMethodCall(MethodInfo method, params Expression[] args);

        /// <summary>
        /// Gets the C# representation of the specified type.
        /// </summary>
        /// <param name="type">The type to get a C# representation of.</param>
        /// <returns>A C# representation of the specified type.</returns>
        string ToCSharp(Type type);

        /// <summary>
        /// Visits the specified expression tree node,treating it either as a statement or an expression, and emits it to the printer.
        /// </summary>
        /// <param name="node">The expression tree node to visit.</param>
        /// <returns>The original expression.</returns>
        [return: NotNullIfNotNull("node")] // TODO: C# 11.0 nameof
        Expression? Visit(Expression? node);

        /// <summary>
        /// Visits the specified expression tree node, treating it as an expression, and emits it to the printer.
        /// </summary>
        /// <param name="node">The expression tree node to visit.</param>
        /// <returns>The original expression.</returns>
        Expression VisitExpression(Expression node);

        /// <summary>
        /// Visits the specified expression tree node ensuring the emission of curly braces if it's not a block.
        /// </summary>
        /// <param name="node">The expression tree node to visit.</param>
        /// <param name="needsCurlies">Indicates whether emission of curly braces around the node is required.</param>
        void VisitBlockLike(Expression node, bool needsCurlies = false);

        /// <summary>
        /// Visits the specified expression tree node and introduces parentheses if needed.
        /// </summary>
        /// <param name="parent">The parent of the expression tree node to visit.</param>
        /// <param name="nodeToVisit">The expression tree node to visit.</param>
        void ParenthesizedVisit(Expression parent, Expression nodeToVisit);

        /// <summary>
        /// Gets a name to refer to the specified label.
        /// </summary>
        /// <param name="target">The label to refer to.</param>
        /// <param name="declarationSite">Indicates whether the name is retrieved for the label's declaration site.</param>
        /// <returns>A name to refer to the specified label.</returns>
        string GetLabelName(LabelTarget target, bool declarationSite = false);

        /// <summary>
        /// Gets a name to refer to the specified parameter.
        /// </summary>
        /// <param name="node">The parameter to refer to.</param>
        /// <param name="declarationSite">Indicates whether the name is retrieved for the parameter's declaration site.</param>
        /// <returns>A name to refer to the specified parameter.</returns>
        string GetVariableName(ParameterExpression node, bool declarationSite = false);

        /// <summary>
        /// Pushes a break label onto the break label stack used for Goto statement analysis.
        /// </summary>
        /// <param name="target">The break label to push.</param>
        void PushBreak(LabelTarget? target);

        /// <summary>
        /// Pushes a continue label onto the continue label stack used for Goto statement analysis.
        /// </summary>
        /// <param name="target">The continue label to push.</param>
        void PushContinue(LabelTarget? target);

        /// <summary>
        /// Pops the last break label from the break label stack used for Goto statement analysis.
        /// </summary>
        void PopBreak();

        /// <summary>
        /// Pops the last continue label from the break label stack used for Goto statement analysis.
        /// </summary>
        void PopContinue();

        /// <summary>
        /// Gets a value indicating whether the specified expression tree node represents a block.
        /// </summary>
        /// <param name="node">The expression tree node to check.</param>
        /// <returns>true if the specified expression tree node represents a block; otherwise, false.</returns>
        bool IsBlock(Expression node);

        /// <summary>
        /// Gets a value indicating whether the specified expression tree node represents a statement.
        /// </summary>
        /// <param name="node">The expression tree node to check.</param>
        /// <returns>true if the specified expression tree node represents a statement; otherwise, false.</returns>
        bool IsStatement(Expression node);
    }
}
