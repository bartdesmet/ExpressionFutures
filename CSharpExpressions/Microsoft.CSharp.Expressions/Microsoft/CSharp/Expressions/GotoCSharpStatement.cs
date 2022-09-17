// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ErrorUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a goto statement.
    /// </summary>
    public abstract partial class GotoCSharpStatement : CSharpStatement
    {
        internal GotoCSharpStatement(LabelTarget? target) => Target = target;

        /// <summary>
        /// Gets a <see cref="LabelTarget"/> representing the target label to jump to.
        /// </summary>
        public LabelTarget? Target { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Goto;

        /// <summary>
        /// Gets the kind of the goto statement.
        /// </summary>
        public abstract CSharpGotoKind Kind { get; }

        /// <summary>
        /// Gets a value that indicates whether the expression tree node can be reduced. 
        /// </summary>
        public override bool CanReduce => Target != null;

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore() => Target == null ? throw Error.GotoCanOnlyBeReducedInSwitch() : Expression.Goto(Target);
    }

    /// <summary>
    /// Represents a goto statement with a label target.
    /// </summary>
    public sealed partial class GotoLabelCSharpStatement : GotoCSharpStatement
    {
        internal GotoLabelCSharpStatement(LabelTarget target)
            : base(target)
        {
        }

        /// <summary>
        /// Gets the kind of the goto statement. Always return <see cref="CSharpGotoKind.GotoLabel"/>.
        /// </summary>
        public override CSharpGotoKind Kind => CSharpGotoKind.GotoLabel;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitGotoLabel(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="target">The <see cref="GotoCSharpStatement.Target" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public GotoLabelCSharpStatement Update(LabelTarget target)
        {
            if (target == Target)
            {
                return this;
            }

            return CSharpExpression.GotoLabel(target);
        }
    }

    /// <summary>
    /// Represents a goto statement with a switch statement case target.
    /// </summary>
    public sealed partial class GotoCaseCSharpStatement : GotoCSharpStatement
    {
        internal GotoCaseCSharpStatement(object? value, LabelTarget? target)
            : base(target)
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
        public object? Value { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitGotoCase(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="target">The <see cref="GotoCSharpStatement.Target" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public GotoCaseCSharpStatement Update(LabelTarget? target)
        {
            if (target == Target)
            {
                return this;
            }

            return CSharpExpression.GotoCase(Value, target);
        }
    }

    /// <summary>
    /// Represents a goto statement with a switch statement default case target.
    /// </summary>
    public sealed partial class GotoDefaultCSharpStatement : GotoCSharpStatement
    {
        internal GotoDefaultCSharpStatement(LabelTarget? target)
            : base(target)
        {
        }

        /// <summary>
        /// Gets the kind of the goto statement. Always return <see cref="CSharpGotoKind.GotoDefault"/>.
        /// </summary>
        public override CSharpGotoKind Kind => CSharpGotoKind.GotoDefault;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitGotoDefault(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="target">The <see cref="GotoCSharpStatement.Target" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public GotoDefaultCSharpStatement Update(LabelTarget? target)
        {
            if (target == Target)
            {
                return this;
            }

            return CSharpExpression.GotoDefault(target);
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
                throw LabelTypeMustBeVoid(nameof(target));
            }

            return new GotoLabelCSharpStatement(target);
        }

        /// <summary>
        /// Creates a <see cref="GotoCaseCSharpStatement"/> that represents a goto statement.
        /// </summary>
        /// <param name="value">The test value of the switch case to jump to.</param>
        /// <returns>The created <see cref="GotoCaseCSharpStatement"/>.</returns>
        public static GotoCaseCSharpStatement GotoCase(object? value) => GotoCase(value, target: null);

        /// <summary>
        /// Creates a <see cref="GotoCaseCSharpStatement"/> that represents a goto statement.
        /// </summary>
        /// <param name="value">The test value of the switch case to jump to.</param>
        /// <param name="target">The label to jump to.</param>
        /// <returns>The created <see cref="GotoCaseCSharpStatement"/>.</returns>
        public static GotoCaseCSharpStatement GotoCase(object? value, LabelTarget? target)
        {
            if (value != null)
            {
                CheckValidSwitchType(value.GetType(), nameof(value));
            }

            return new GotoCaseCSharpStatement(value, target);
        }

        /// <summary>
        /// Creates a <see cref="GotoDefaultCSharpStatement"/> that represents a goto statement.
        /// </summary>
        /// <returns>The created <see cref="GotoDefaultCSharpStatement"/>.</returns>
        public static GotoDefaultCSharpStatement GotoDefault() => GotoDefault(target: null);

        /// <summary>
        /// Creates a <see cref="GotoDefaultCSharpStatement"/> that represents a goto statement.
        /// </summary>
        /// <param name="target">The label to jump to.</param>
        /// <returns>The created <see cref="GotoDefaultCSharpStatement"/>.</returns>
        public static GotoDefaultCSharpStatement GotoDefault(LabelTarget? target)
        {
            return new GotoDefaultCSharpStatement(target);
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
        protected internal virtual Expression VisitGotoLabel(GotoLabelCSharpStatement node) =>
            node.Update(
                VisitLabelTarget(node.Target!) // NB: Always non-null for GotoLabel.
            );

        /// <summary>
        /// Visits the children of the <see cref="GotoCaseCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitGotoCase(GotoCaseCSharpStatement node) =>
            node.Update(
                VisitLabelTarget(node.Target)
            );

        /// <summary>
        /// Visits the children of the <see cref="GotoDefaultCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitGotoDefault(GotoDefaultCSharpStatement node) =>
            node.Update(
                VisitLabelTarget(node.Target)
            );
    }
}
