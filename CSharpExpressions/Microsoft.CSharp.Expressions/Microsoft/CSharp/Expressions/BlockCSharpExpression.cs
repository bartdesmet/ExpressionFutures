// Prototyping extended expression trees for C#.
//
// bartde - November 2015

#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    // DESIGN: This node could be removed by fusing it with custom Lambda nodes for statements lambdas emitted by the C#
    //         compiler. Its current use is to emit the body of a statement lambda without introducing artificial nodes
    //         for the compiler-generated return label, e.g.
    //
    //           e = () => { ; }
    //
    //         would translate into
    //
    //           var @return = Label();
    //           e = Lambda<Action>(Block(Empty(), Return(@return), Label(@return)));
    //                                             ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //
    //         By using this special node, we can turn the top-level block in a lambda into one that hoists up the return
    //         label as a property of the block rather than an expression in it, e.g.
    //
    //           var @return = Label();
    //           e = Lambda<Action>(Block(new[] { Empty() }, @return));
    //                                                       ^^^^^^^
    //
    //         Note that the shape of the tree is homo-iconic for expressions and statements, as desired. The reduction of
    //         the node still turns it into a regular Block with a label.
    //
    //         We can consider the following alternatives and/or simplifications:
    //
    //           1. Do away with Return(label, value) and have Return(value) instead; the top-level block can find all of
    //              those label-free Return statements and bind them to the return label of the closest enclosing lambda.
    //              We'd still need a custom node to drive the reduction into a regular block, or use point 2 below.
    //           2. Define specializations of LambdaExpression to support statement bodies. The custom Lambda node would
    //              have a Statements collection (rather than a Body) and could have a return label defined or use the
    //              strategy outlined in point 1 above. This would require unsealing Expression<T>, which is also needed
    //              for async lambda support.
    //           3. Keep this construct as it may be more generally applicable, e.g. for the body of a local function as
    //              suggested for C# 7.0.

    /// <summary>
    /// Represents a block with a return label.
    /// </summary>
    public sealed partial class BlockCSharpExpression : CSharpExpression
    {
        // DESIGN: The name of the Statements property may be misguiding. If no label is present, we infer the type of
        //         the node from the last statement, expecting it to be non-void, thus treating it as an expression.
        //         We could either rename the property to Expressions or require a return label to be present, so the
        //         type is always inferred from the return label. That'd make the node not usable as a substitute for
        //         a regular Block.

        internal BlockCSharpExpression(ReadOnlyCollection<ParameterExpression> variables, ReadOnlyCollection<Expression> statements, LabelTarget? returnLabel)
        {
            Variables = variables;
            Statements = statements;
            ReturnLabel = returnLabel;
        }

        /// <summary>
        /// Gets a collection of variables declared in the block.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Variables { get; }

        /// <summary>
        /// Gets a collection of statements in the block.
        /// </summary>
        public ReadOnlyCollection<Expression> Statements { get; }

        /// <summary>
        /// Gets the return label to exit the block.
        /// </summary>
        public LabelTarget? ReturnLabel { get; }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => ReturnLabel?.Type ?? (Statements.Count > 0 ? Statements.Last().Type : typeof(void));

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Block;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitBlock(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="variables">The <see cref="Variables" /> property of the result.</param>
        /// <param name="statements">The <see cref="Statements" /> property of the result.</param>
        /// <param name="returnLabel">The <see cref="ReturnLabel" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public BlockCSharpExpression Update(IEnumerable<ParameterExpression> variables, IEnumerable<Expression> statements, LabelTarget? returnLabel)
        {
            if (SameElements(ref variables, Variables) && SameElements(ref statements, Statements) && returnLabel == ReturnLabel)
            {
                return this;
            }

            return CSharpExpression.Block(variables, statements, returnLabel);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var statementCount = Statements.Count;

            ReadOnlyCollection<Expression> statements;

            if (ReturnLabel == null)
            {
                if (statementCount == 0)
                {
                    // NB: Can ignore variables; there's no expression that can use them anyway, and they don't have side-effects.
                    return Expression.Empty();
                }
                else
                {
                    statements = Statements;
                }
            }
            else
            {
                var returnLabel = ReturnLabel.Type != typeof(void)
                    ? Expression.Label(ReturnLabel, Expression.Default(ReturnLabel.Type))
                    : Expression.Label(ReturnLabel);

                if (statementCount == 0)
                {
                    // NB: Can ignore variables; there's no expression that can use them anyway, and they don't have side-effects.
                    return returnLabel;
                }
                else
                {
                    var lastStatement = Statements[statementCount - 1];

                    // REVIEW: The check below is problematic and works around the case where Roslyn generates a block for an
                    //         expression lambda body that has locals, e.g. "o => o is string s && s.Length > 0". This ends
                    //         up having a block with locals and a return label, which ends up in the tree. As a workaround,
                    //         we promote the last statement of the block to be an implicit return (not unlike Block in the
                    //         LINQ expression tree APIs) if the types line up. However, it'd likely be better if compiler-
                    //         generated blocks (with generated return labels etc.) don't end up in the tree, and we may want
                    //         to then have a Lambda node that has a Locals collection so we can do away with the top-level
                    //         block as a coathanger for local variables.

                    if (ReturnLabel.Type != typeof(void) && AreReferenceAssignable(ReturnLabel.Type, lastStatement.Type))
                    {
                        var newStatements = new Expression[statementCount + 1];

                        for (var i = 0; i < statementCount - 1; i++)
                        {
                            newStatements[i] = Statements[i];
                        }

                        newStatements[statementCount - 1] = Expression.Return(ReturnLabel, lastStatement);
                        newStatements[statementCount] = returnLabel;

                        statements = newStatements.ToReadOnlyUnsafe();
                    }
                    else
                    {
                        statements = Statements.Append(returnLabel).ToArray().ToReadOnlyUnsafe();
                    }
                }
            }

            if (Variables.Count == 0)
            {
                return Expression.Block(Type, statements);
            }
            else
            {
                // REVIEW: Should we ensure all variables get assigned default values? Cf. when it's used
                //         in a loop and the locals don't get reinitialized. Or should we assume there's
                //         definite assignment (or enforce it)?
                return Expression.Block(Type, Variables, statements);
            }
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="BlockCSharpExpression"/> that represents a block.
        /// </summary>
        /// <param name="statements">The statements in the block.</param>
        /// <param name="returnLabel">The return label used to exist the block.</param>
        /// <returns>The created <see cref="BlockCSharpExpression"/>.</returns>
        public static BlockCSharpExpression Block(IEnumerable<Expression> statements, LabelTarget? returnLabel) => Block(Array.Empty<ParameterExpression>(), statements, returnLabel);

        /// <summary>
        /// Creates a <see cref="BlockCSharpExpression"/> that represents a block.
        /// </summary>
        /// <param name="variables">The variables declared in the block.</param>
        /// <param name="statements">The statements in the block.</param>
        /// <param name="returnLabel">The return label used to exist the block.</param>
        /// <returns>The created <see cref="BlockCSharpExpression"/>.</returns>
        public static BlockCSharpExpression Block(IEnumerable<ParameterExpression> variables, IEnumerable<Expression> statements, LabelTarget? returnLabel)
        {
            var variablesList = CheckUniqueVariables(variables, nameof(variables));

            var statementsList = statements.ToReadOnly();
            RequiresNotNullItems(statementsList, nameof(statements));

            return new BlockCSharpExpression(variablesList, statementsList, returnLabel);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="BlockCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitBlock(BlockCSharpExpression node) =>
            node.Update(
                VisitAndConvert(node.Variables, nameof(VisitBlock)),
                Visit(node.Statements),
                VisitLabelTarget(node.ReturnLabel)
            );
    }
}
