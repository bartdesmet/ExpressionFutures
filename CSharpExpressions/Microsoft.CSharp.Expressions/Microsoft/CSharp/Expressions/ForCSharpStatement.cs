// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using LinqError = System.Linq.Expressions.Error;
using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a for loop.
    /// </summary>
    public sealed partial class ForCSharpStatement : ConditionalLoopCSharpStatement
    {
        internal ForCSharpStatement(ReadOnlyCollection<ParameterExpression> variables, ReadOnlyCollection<Expression> initializers, Expression test, ReadOnlyCollection<Expression> iterators, Expression body, LabelTarget breakLabel, LabelTarget continueLabel, ReadOnlyCollection<ParameterExpression> innerLocals)
            : base(test, body, breakLabel, continueLabel, innerLocals)
        {
            Variables = variables;
            Initializers = initializers;
            Iterators = iterators;
        }

        /// <summary>
        /// Gets a collection of variables in scope for the loop.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Variables { get; }

        /// <summary>
        /// Gets a collection of loop initializer expressions.
        /// </summary>
        public ReadOnlyCollection<Expression> Initializers { get; }

        /// <summary>
        /// Gets a collection of loop iterator expressions.
        /// </summary>
        public ReadOnlyCollection<Expression> Iterators { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.For;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitFor(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="breakLabel">The <see cref="LoopCSharpStatement.BreakLabel" /> property of the result.</param>
        /// <param name="continueLabel">The <see cref="LoopCSharpStatement.ContinueLabel" /> property of the result.</param>
        /// <param name="variables">The <see cref="ForCSharpStatement.Variables" /> property of the result.</param>
        /// <param name="initializers">The <see cref="ForCSharpStatement.Initializers" /> property of the result.</param>
        /// <param name="test">The <see cref="ConditionalLoopCSharpStatement.Test" /> property of the result.</param>
        /// <param name="iterators">The <see cref="ForCSharpStatement.Iterators" /> property of the result.</param>
        /// <param name="body">The <see cref="LoopCSharpStatement.Body" /> property of the result.</param>
        /// <param name="locals">The <see cref="ConditionalLoopCSharpStatement.Locals" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ForCSharpStatement Update(LabelTarget breakLabel, LabelTarget continueLabel, IEnumerable<ParameterExpression> variables, IEnumerable<Expression> initializers, Expression test, IEnumerable<Expression> iterators, Expression body, IEnumerable<ParameterExpression> locals)
        {
            if (breakLabel == this.BreakLabel && continueLabel == this.ContinueLabel && variables == this.Variables && initializers == this.Initializers && test == this.Test && iterators == this.Iterators && body == this.Body && Helpers.SameElements(ref locals, this.Locals))
            {
                return this;
            }

            return CSharpExpression.For(variables, initializers, test, iterators, body, breakLabel, continueLabel, locals);
        }

        internal static ForCSharpStatement Make(ReadOnlyCollection<ParameterExpression> variables, ReadOnlyCollection<Expression> initializers, Expression test, ReadOnlyCollection<Expression> iterators, Expression body, LabelTarget breakLabel, LabelTarget continueLabel, ReadOnlyCollection<ParameterExpression> innerLocals)
        {
            // TODO: optimized nodes for for(;;) and loops with a single initializer and single iterator

            return new ForCSharpStatement(variables, initializers, test, iterators, body, breakLabel, continueLabel, innerLocals);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            var begin = Expression.Label("__begin");
            var test = Test != null ? Expression.Label("__test") : null;

            var expressions = new List<Expression>();

            foreach (var initializer in Initializers)
            {
                expressions.Add(initializer);
            }

            var innerBlock = Locals.Count > 0 ? new List<Expression>() : expressions;

            if (test != null)
            {
                innerBlock.Add(Expression.Goto(test));
            }

            innerBlock.Add(Expression.Label(begin));
            innerBlock.Add(Body);

            if (ContinueLabel != null)
            {
                innerBlock.Add(Expression.Label(ContinueLabel));
            }

            foreach (var iterator in Iterators)
            {
                innerBlock.Add(iterator);
            }

            if (test != null)
            {
                innerBlock.Add(Expression.Label(test));
                innerBlock.Add(Expression.IfThen(Test, Expression.Goto(begin)));
            }
            else
            {
                innerBlock.Add(Expression.Goto(begin));
            }

            if (Locals.Count > 0)
            {
                expressions.Add(Expression.Block(typeof(void), Locals, innerBlock));
            }

            if (BreakLabel != null)
            {
                expressions.Add(Expression.Label(BreakLabel));
            }

            // NB: The scoping of the variables here could be considered to be in conflict with
            //     C# spec section 8.8.3 when interpreted wearing an Expression API hat:
            //
            //       "The scope of a local variable declared by a for-initializer starts at the
            //        local-variable-declarator for the variable and extends to the end of the
            //        embedded statement. The scope includes the for-condition and the for-iterator."
            //
            //     Considering that Parameter nodes have reference equality in the Expression API,
            //     this could be interpreted to have the following binding:
            //
            //               +-----------------+
            //               v                 |
            //       Block({ x, y }, For({ x = x + 1, y = x + y }, x < y, { x++, y-- }, F(x, y))
            //                             ^              |        |        |             |
            //                             +--------------+--------+--------+-------------+
            //
            //     and similar for variable y. First of all, this is not expressible in C# because
            //     the scoping rules for variables are based on their lexical name. By bundling
            //     all variables in one top-level collection here, binding means:
            //
            //                             +---+
            //                             v   |
            //       Block({ x, y }, For({ x = x + 1, y = x + y }, x < y, { x++, y-- }, F(x, y))
            //                             ^              |        |        |             |
            //                             +--------------+--------+--------+-------------+
            //
            //     which will always cause x to have value 1 given the Expression API doesn't do
            //     any definite assignment checks. The reason we don't try to create a series of
            //     nested scopes is because the initializer expressions can contain regular Assign
            //     nodes and it would be perceived strange that those have a special meaning when
            //     used in the Initializers section of a For loop.
            //
            //     If we want to support the binding shown above, we'd likely want to model the
            //     local-variable-declarator construct as a separate node where the rhs is bound in
            //     the enclosing scope and the lhs (which has to be a Parameter) is introduced in
            //     a new scope for the successor expressions to use. A parent node such as For can
            //     then reduce the declarator, establishing the scope for the bound variables, and
            //     reduce the for-condition, for-iterator, and embedded-statement in that scope.
            //
            //     However, given that this is the C#-specific expression API, we can live with
            //     this oddity (as it may be perceived by Expression API fanatics). If one really
            //     wants to have the binding effect described above, an additional variable can
            //     be introduced or an alpha-substitution for Parameter nodes can be carried out.
            //
            //     Note that the scoping for Using, Switch, and ForEach is a bit more natural for
            //     users of Expression APIs given that their Resource, SwitchValue, and Collection
            //     nodes (hereafter referred to as the "source") are bound in the enclosing scope
            //     and any variables introduced by those constructs are brought in scope beyond
            //     evaluation of the "source".

            // REVIEW: Should we ensure all variables get assigned default values? Cf. when it's used
            //         in a loop and the locals don't get reinitialized. Or should we assume there's
            //         definite assignment (or enforce it)?
            var res = Expression.Block(typeof(void), Variables, expressions);

            return res;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="ForCSharpStatement"/> that represents a for loop.
        /// </summary>
        /// <param name="initializers">The loop initializers.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="iterators">The loop iterators.</param>
        /// <param name="body">The body of the loop.</param>
        /// <returns>The created <see cref="ForCSharpStatement"/>.</returns>
        public static ForCSharpStatement For(IEnumerable<BinaryExpression> initializers, Expression test, IEnumerable<Expression> iterators, Expression body) =>
            For(initializers, test, iterators, body, @break: null, @continue: null, locals: null);

        /// <summary>
        /// Creates a <see cref="ForCSharpStatement"/> that represents a for loop.
        /// </summary>
        /// <param name="initializers">The loop initializers.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="iterators">The loop iterators.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <returns>The created <see cref="ForCSharpStatement"/>.</returns>
        public static ForCSharpStatement For(IEnumerable<BinaryExpression> initializers, Expression test, IEnumerable<Expression> iterators, Expression body, LabelTarget @break) =>
            For(initializers, test, iterators, body, @break, @continue: null, locals: null);

        /// <summary>
        /// Creates a <see cref="ForCSharpStatement"/> that represents a for loop.
        /// </summary>
        /// <param name="initializers">The loop initializers.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="iterators">The loop iterators.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <returns>The created <see cref="ForCSharpStatement"/>.</returns>
        public static ForCSharpStatement For(IEnumerable<BinaryExpression> initializers, Expression test, IEnumerable<Expression> iterators, Expression body, LabelTarget @break, LabelTarget @continue)
            => For(initializers, test, iterators, body, @break, @continue, locals: null);

        /// <summary>
        /// Creates a <see cref="ForCSharpStatement"/> that represents a for loop.
        /// </summary>
        /// <param name="initializers">The loop initializers.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="iterators">The loop iterators.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <param name="locals">The variables that are in scope of the loop.</param>
        /// <returns>The created <see cref="ForCSharpStatement"/>.</returns>
        public static ForCSharpStatement For(IEnumerable<BinaryExpression> initializers, Expression test, IEnumerable<Expression> iterators, Expression body, LabelTarget @break, LabelTarget @continue, IEnumerable<ParameterExpression> locals)
        {
            ValidateLoop(test, body, @break, @continue, optionalTest: true);

            // NB: While C# requires all initializers to be of the same type, we don't quite need that restriction here.
            //     This can be revisited. We will check whether all initializers are simple assignments though.

            var initializerList = initializers.ToReadOnly<Expression>();
            RequiresNotNullItems(initializerList, nameof(initializers));

            var uniqueVariables = new HashSet<ParameterExpression>();
            var variables = new List<ParameterExpression>();

            foreach (BinaryExpression initializer in initializerList)
            {
                if (initializer.NodeType != ExpressionType.Assign || initializer.Left.NodeType != ExpressionType.Parameter)
                {
                    throw Error.InvalidInitializer();
                }

                var variable = (ParameterExpression)initializer.Left;

                if (!uniqueVariables.Add(variable))
                {
                    throw LinqError.DuplicateVariable(variable);
                }

                // NB: We keep them in the order specified and don't rely on the hash set.
                variables.Add(variable);
            }

            var variableList = variables.ToReadOnly();

            var iteratorList = iterators.ToReadOnly();
            RequiresNotNullItems(iteratorList, nameof(iterators));

            var localsList = CheckUniqueVariables(locals, nameof(locals));

            return ForCSharpStatement.Make(variableList, initializerList, test, iteratorList, body, @break, @continue, localsList);
        }

        /// <summary>
        /// Creates a <see cref="ForCSharpStatement"/> that represents a for loop.
        /// </summary>
        /// <param name="variables">The variables in scope of the loop.</param>
        /// <param name="initializers">The loop initializers.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="iterators">The loop iterators.</param>
        /// <param name="body">The body of the loop.</param>
        /// <returns>The created <see cref="ForCSharpStatement"/>.</returns>
        public static ForCSharpStatement For(IEnumerable<ParameterExpression> variables, IEnumerable<Expression> initializers, Expression test, IEnumerable<Expression> iterators, Expression body)
            => For(variables, initializers, test, iterators, body, @break: null, @continue: null, locals: null);

        /// <summary>
        /// Creates a <see cref="ForCSharpStatement"/> that represents a for loop.
        /// </summary>
        /// <param name="variables">The variables in scope of the loop.</param>
        /// <param name="initializers">The loop initializers.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="iterators">The loop iterators.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <returns>The created <see cref="ForCSharpStatement"/>.</returns>
        public static ForCSharpStatement For(IEnumerable<ParameterExpression> variables, IEnumerable<Expression> initializers, Expression test, IEnumerable<Expression> iterators, Expression body, LabelTarget @break)
            => For(variables, initializers, test, iterators, body, @break, @continue: null, locals: null);

        /// <summary>
        /// Creates a <see cref="ForCSharpStatement"/> that represents a for loop.
        /// </summary>
        /// <param name="variables">The variables in scope of the loop.</param>
        /// <param name="initializers">The loop initializers.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="iterators">The loop iterators.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <returns>The created <see cref="ForCSharpStatement"/>.</returns>
        public static ForCSharpStatement For(IEnumerable<ParameterExpression> variables, IEnumerable<Expression> initializers, Expression test, IEnumerable<Expression> iterators, Expression body, LabelTarget @break, LabelTarget @continue)
            => For(variables, initializers, test, iterators, body, @break, @continue, locals: null);

        /// <summary>
        /// Creates a <see cref="ForCSharpStatement"/> that represents a for loop.
        /// </summary>
        /// <param name="variables">The variables in scope of the loop.</param>
        /// <param name="initializers">The loop initializers.</param>
        /// <param name="test">The condition of the loop.</param>
        /// <param name="iterators">The loop iterators.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <param name="locals">The variables that are in scope of the loop.</param>
        /// <returns>The created <see cref="ForCSharpStatement"/>.</returns>
        public static ForCSharpStatement For(IEnumerable<ParameterExpression> variables, IEnumerable<Expression> initializers, Expression test, IEnumerable<Expression> iterators, Expression body, LabelTarget @break, LabelTarget @continue, IEnumerable<ParameterExpression> locals)
        {
            ValidateLoop(test, body, @break, @continue, optionalTest: true);

            // NB: While C# requires all initializers to be of the same type, we don't quite need that restriction here.
            //     This can be revisited. We will check whether all initializers are simple assignments though.

            var variableList = CheckUniqueVariables(variables, nameof(variables));

            var initializerList = initializers.ToReadOnly();
            RequiresNotNullItems(initializerList, nameof(initializers));

            var iteratorList = iterators.ToReadOnly();
            RequiresNotNullItems(iteratorList, nameof(iterators));

            var localsList = CheckUniqueVariables(locals, nameof(locals));

            return ForCSharpStatement.Make(variableList, initializerList, test, iteratorList, body, @break, @continue, localsList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ForCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitFor(ForCSharpStatement node)
        {
            return node.Update(VisitLabelTarget(node.BreakLabel), VisitLabelTarget(node.ContinueLabel), VisitAndConvert(node.Variables, nameof(VisitFor)), Visit(node.Initializers), Visit(node.Test), Visit(node.Iterators), Visit(node.Body), VisitAndConvert(node.Locals, nameof(VisitFor)));
        }
    }
}
