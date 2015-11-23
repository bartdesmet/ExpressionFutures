// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Linq.Expressions;
using static System.Dynamic.Utils.ContractUtils;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a goto statement.
    /// </summary>
    public abstract class GotoCSharpStatement : CSharpStatement
    {
        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Goto;

        /// <summary>
        /// Gets the kind of the goto statement.
        /// </summary>
        public abstract CSharpGotoKind Kind { get; }
    }

    /// <summary>
    /// Represents a goto statement with a label target.
    /// </summary>
    public sealed class GotoLabelCSharpStatement : GotoCSharpStatement
    {
        internal GotoLabelCSharpStatement(LabelTarget target)
        {
            Target = target;
        }

        /// <summary>
        /// Gets the kind of the goto statement. Always return <see cref="CSharpGotoKind.GotoLabel"/>.
        /// </summary>
        public override CSharpGotoKind Kind => CSharpGotoKind.GotoLabel;

        /// <summary>
        /// Gets a <see cref="LabelTarget"/> representing the target label to jump to.
        /// </summary>
        public LabelTarget Target { get; }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore() => Expression.Goto(Target);

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitGotoLabel(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="target">The <see cref="Target" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public GotoLabelCSharpStatement Update(LabelTarget target)
        {
            if (target == this.Target)
            {
                return this;
            }

            return CSharpExpression.GotoLabel(target);
        }
    }

    /// <summary>
    /// Represents a goto statement with a switch statement case target.
    /// </summary>
    public sealed class GotoCaseCSharpStatement : GotoCSharpStatement
    {
        internal GotoCaseCSharpStatement(object value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the kind of the goto statement. Always return <see cref="CSharpGotoKind.GotoCase"/>.
        /// </summary>
        public override CSharpGotoKind Kind => CSharpGotoKind.GotoCase;

        /// <summary>
        /// Gets the value of the case to jump to.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets a value that indicates whether the expression tree node can be reduced. 
        /// </summary>
        public override bool CanReduce => false;

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            throw Error.GotoCanOnlyBeReducedInSwitch();
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitGotoCase(this);
        }
    }

    /// <summary>
    /// Represents a goto statement with a switch statement default case target.
    /// </summary>
    public sealed class GotoDefaultCSharpStatement : GotoCSharpStatement
    {
        internal GotoDefaultCSharpStatement()
        {
        }

        /// <summary>
        /// Gets the kind of the goto statement. Always return <see cref="CSharpGotoKind.GotoDefault"/>.
        /// </summary>
        public override CSharpGotoKind Kind => CSharpGotoKind.GotoDefault;

        /// <summary>
        /// Gets a value that indicates whether the expression tree node can be reduced. 
        /// </summary>
        public override bool CanReduce => false;

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            throw Error.GotoCanOnlyBeReducedInSwitch();
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitGotoDefault(this);
        }
    }

    /// <summary>
    /// Indicates the kind of goto statement.
    /// </summary>
    public enum CSharpGotoKind
    {
        /// <summary>
        /// Goto with a label target.
        /// </summary>
        GotoLabel,

        /// <summary>
        /// Goto for a case with a test valye in a switch statement.
        /// </summary>
        GotoCase,

        /// <summary>
        /// Goto for the default case in a switch statement.
        /// </summary>
        GotoDefault,
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="GotoLabelCSharpStatement"/> that represents a goto statement.
        /// </summary>
        /// <param name="target">The label to jump to.</param>
        /// <returns>The created <see cref="GotoLabelCSharpStatement"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static GotoLabelCSharpStatement GotoLabel(LabelTarget target)
        {
            RequiresNotNull(target, nameof(target));
            
            if (target.Type != typeof(void))
            {
                throw LinqError.LabelTypeMustBeVoid();
            }

            return new GotoLabelCSharpStatement(target);
        }

        /// <summary>
        /// Creates a <see cref="GotoCaseCSharpStatement"/> that represents a goto statement.
        /// </summary>
        /// <param name="value">The test value of the switch case to jump to.</param>
        /// <returns>The created <see cref="GotoCaseCSharpStatement"/>.</returns>
        public static GotoCaseCSharpStatement GotoCase(object value)
        {
            if (value != null)
            {
                CheckValidSwitchType(value.GetType());
            }

            return new GotoCaseCSharpStatement(value);
        }

        /// <summary>
        /// Creates a <see cref="GotoDefaultCSharpStatement"/> that represents a goto statement.
        /// </summary>
        /// <returns>The created <see cref="GotoDefaultCSharpStatement"/>.</returns>
        public static GotoDefaultCSharpStatement GotoDefault()
        {
            // NB: Much like Empty, returning a new instance each time. This makes object reference identity usable as a key in a dictionary.

            return new GotoDefaultCSharpStatement();
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="GotoLabelCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitGotoLabel(GotoLabelCSharpStatement node)
        {
            return node.Update(VisitLabelTarget(node.Target));
        }

        /// <summary>
        /// Visits the children of the <see cref="GotoCaseCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitGotoCase(GotoCaseCSharpStatement node)
        {
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="GotoDefaultCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitGotoDefault(GotoDefaultCSharpStatement node)
        {
            return node;
        }
    }
}
