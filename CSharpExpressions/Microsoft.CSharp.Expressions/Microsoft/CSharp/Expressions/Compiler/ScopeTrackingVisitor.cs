// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Base class for visitors that track variable scopes.
    /// </summary>
    internal abstract class ScopeTrackingVisitor : CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits a <see cref="BlockExpression"/>, keeping track of the variables declared in <see cref="BlockExpression.Variables"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The result of visiting the expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected override Expression VisitBlock(BlockExpression node)
        {
            PushScope(node.Variables);

            var res = base.VisitBlock(node);

            PopScope(node.Variables);

            return res;
        }

        /// <summary>
        /// Visits a <see cref="CatchBlock"/>, keeping track of the (optional) variable declared in <see cref="CatchBlock.Variable"/>.
        /// </summary>
        /// <param name="node">The node to visit.</param>
        /// <returns>The result of visiting the node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            PushScope(node.Variable);

            var res = base.VisitCatchBlock(node);

            PopScope(node.Variable);

            return res;
        }

        /// <summary>
        /// Visits a <see cref="LambdaExpression"/>, keeping track of the variables declared in <see cref="LambdaExpression.Parameters"/>.
        /// </summary>
        /// <typeparam name="T">The type of the delegate represented by the lambda expression.</typeparam>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The result of visiting the expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            PushScope(node.Parameters);

            var res = base.VisitLambda(node);

            PopScope(node.Parameters);

            return res;
        }

        // NB: Strictly speaking, we don't need to handle C# nodes here (other than keeping Await unreduced),
        //     because the shadow eliminator runs after the reducer. However, we keep those here for general
        //     utility and also to deal with the case where we may reshuffle rewrite steps. We could #if them
        //     out if we want to reduce code size.

        /// <summary>
        /// Visits a <see cref="BlockCSharpExpression"/>, keeping track of the variables declared in <see cref="BlockCSharpExpression.Variables"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The result of visiting the expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitBlock(BlockCSharpExpression node)
        {
            PushScope(node.Variables);

            var res = base.VisitBlock(node);

            PopScope(node.Variables);

            return res;
        }

        /// <summary>
        /// Visits a <see cref="UsingCSharpStatement"/>, keeping track of the (optional) variable declared in <see cref="UsingCSharpStatement.Variable"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The result of visiting the expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitUsing(UsingCSharpStatement node)
        {
            // NB: The variable is not in scope of the resource. In a LINQ expression tree, the ParameterExpression
            //     could be reused in several nested scopes, so the same variable could be used within the resource
            //     expression and bind to a declaration in a surrounding scope.

            var resource = Visit(node.Resource);

            PushScope(node.Variable);

            var res =
                node.Update(
                    VisitAndConvert(node.Variable, nameof(VisitUsing)),
                    resource,
                    Visit(node.Body),
                    node.AwaitInfo
                );

            PopScope(node.Variable);

            return res;
        }

        /// <summary>
        /// Visits a <see cref="ForEachCSharpStatement"/>, keeping track of the variable declared in <see cref="ForEachCSharpStatement.Variable"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The result of visiting the expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitForEach(ForEachCSharpStatement node)
        {
            // NB: The variable is not in scope of the collection. In a LINQ expression tree, the ParameterExpression
            //     could be reused in several nested scopes, so the same variable could be used within the collection
            //     expression and bind to a declaration in a surrounding scope.

            var collection = Visit(node.Collection);

            PushScope(node.Variables);

            var res = 
                node.Update(
                    VisitLabelTarget(node.BreakLabel),
                    VisitLabelTarget(node.ContinueLabel),
                    VisitAndConvert(node.Variables, nameof(VisitForEach)),
                    collection,
                    VisitAndConvert(node.Conversion, nameof(VisitForEach)),
                    Visit(node.Body),
                    VisitAndConvert(node.Deconstruction, nameof(VisitForEach)),
                    VisitAwaitInfo(node.AwaitInfo)
                );

            PopScope(node.Variables);

            return res;
        }

        /// <summary>
        /// Visits a <see cref="ForCSharpStatement"/>, keeping track of the variables declared in <see cref="ForCSharpStatement.Variables"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The result of visiting the expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitFor(ForCSharpStatement node)
        {
            // NB: See notes in ForCSharpStatement.ReduceCore for the scoping rules applied here.

            PushScope(node.Variables);

            var res = base.VisitFor(node);

            PopScope(node.Variables);

            return res;
        }

        /// <summary>
        /// Visits a <see cref="SwitchCSharpStatement"/>, keeping track of the variables declared in <see cref="SwitchCSharpStatement.Variables"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The result of visiting the expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitSwitch(SwitchCSharpStatement node)
        {
            // NB: The variables are not in scope of the switch value. In a LINQ expression tree, the ParameterExpressions
            //     could be reused in several nested scopes, so the same variables could be used within the switch value
            //     expression and bind to a declaration in a surrounding scope.

            var switchValue = Visit(node.SwitchValue);

            PushScope(node.Variables);

            var res =
                node.Update(
                    switchValue,
                    VisitLabelTarget(node.BreakLabel),
                    VisitAndConvert(node.Variables, nameof(VisitSwitch)),
                    Visit(node.Cases, VisitSwitchCase)
                );

            PopScope(node.Variables);

            return res;
        }

        /// <summary>
        /// Visits a <see cref="ParameterExpression"/> representing a variable, either in a declaration or use site.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The result of visiting the expression.</returns>
        /// <remarks>
        /// Derived classes can override the <see cref="Push(IEnumerable{ParameterExpression})"/> and <see cref="Pop()"/> methods to analyze variables in declaration positions.
        /// </remarks>
        protected abstract override Expression VisitParameter(ParameterExpression node);

        /// <summary>
        /// Pushes a single variable that is in scope.
        /// </summary>
        /// <param name="variable">A single variable that is in scope.</param>
        protected virtual void Push(ParameterExpression variable)
        {
            Push(new[] { variable });
        }

        /// <summary>
        /// Pushes a collection of variables that are in scope.
        /// </summary>
        /// <param name="variables">A collection of variables that are in scope.</param>
        protected abstract void Push(IEnumerable<ParameterExpression> variables);

        /// <summary>
        /// Pops the nearest enclosing scope of variables.
        /// </summary>
        protected abstract void Pop();

        /// <summary>
        /// Pushes a collection of variables that are in scope.
        /// </summary>
        /// <param name="variables">A collection of variables that are in scope.</param>
        /// <remarks>
        /// If the <paramref name="variables"/> collection is empty, this method is a no-op.
        /// </remarks>
        private void PushScope(IReadOnlyCollection<ParameterExpression> variables)
        {
            if (variables.Count > 0)
            {
                Push(variables);
            }
        }

        /// <summary>
        /// Pushes a single variable that is in scope.
        /// </summary>
        /// <param name="variable">A single variable that is in scope.</param>
        /// <remarks>
        /// If the <paramref name="variable"/> is <c>null</c>, this method is a no-op.
        /// </remarks>
        private void PushScope(ParameterExpression variable)
        {
            if (variable != null)
            {
                Push(variable);
            }
        }

        /// <summary>
        /// Pops the nearest enclosing scope of variables.
        /// </summary>
        /// <param name="variables">A collection of variables that are no longer in scope.</param>
        /// <remarks>
        /// If the <paramref name="variables"/> collection is empty, this method is a no-op.
        /// </remarks>
        private void PopScope(IReadOnlyCollection<ParameterExpression> variables)
        {
            if (variables.Count > 0)
            {
                Pop();
            }
        }

        /// <summary>
        /// Pops the nearest enclosing scope containing a single variable.
        /// </summary>
        /// <param name="variable">A single variable that is no longer in scope.</param>
        /// <remarks>
        /// If the <paramref name="variable"/> is <c>null</c>, this method is a no-op.
        /// </remarks>
        private void PopScope(ParameterExpression variable)
        {
            if (variable != null)
            {
                Pop();
            }
        }
    }
}
