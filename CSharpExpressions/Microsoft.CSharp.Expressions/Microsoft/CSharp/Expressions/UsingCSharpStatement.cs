// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    // TODO: The C# language allows for multiple resources to be declared. Our node doesn't support that yet.
    //       We should revisit this when we enable statement trees and consider introducing nodes for e.g.
    //       local variable declaration. Alternatively, we could expand the construct into nested using nodes
    //       as the language specification describes (at the cost of a true homo-iconic representation).

    /// <summary>
    /// Represents a using statement.
    /// </summary>
    public sealed partial class UsingCSharpStatement : CSharpStatement
    {
        internal UsingCSharpStatement(ParameterExpression variable, Expression resource, Expression body)
        {
            Variable = variable;
            Resource = resource;
            Body = body;
        }

        /// <summary>
        /// Gets the <see cref="ParameterExpression" /> representing the resource (or null if no assignment is performed).
        /// </summary>
        public new ParameterExpression Variable { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the resource.
        /// </summary>
        public Expression Resource { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the body.
        /// </summary>
        public Expression Body { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Using;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitUsing(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="variable">The <see cref="Variable" /> property of the result.</param>
        /// <param name="resource">The <see cref="Resource" /> property of the result.</param>
        /// <param name="body">The <see cref="Body" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public UsingCSharpStatement Update(ParameterExpression variable, Expression resource, Expression body)
        {
            if (variable == this.Variable && resource == this.Resource && body == this.Body)
            {
                return this;
            }

            return CSharpExpression.Using(variable, resource, body);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            var variable = default(ParameterExpression);
            var cleanup = default(Expression);
            var checkNull = false;

            var resourceType = Resource.Type;
            if (resourceType.IsValueType)
            {
                var variableValue = default(Expression);

                if (resourceType.IsNullableType())
                {
                    variable = Variable ?? Expression.Parameter(resourceType);
                    variableValue = Expression.Property(variable, "Value");
                    checkNull = true;
                }
                else
                {
                    variable = Variable ?? Expression.Parameter(resourceType);
                    variableValue = variable;
                }

                var disposeMethod = variableValue.Type.FindDisposeMethod();
                cleanup = Expression.Call(variableValue, disposeMethod);
            }
            else
            {
                variable = Variable ?? Expression.Parameter(typeof(IDisposable));

                // NB: This optimization would be more effective if the expression compiler would emit a `call` instruction,
                //     but the JIT may still optimize it if it realizes the `callvirt` to the resource is predicated by a
                //     prior null check.
                var variableType = variable.Type;
                var disposeMethod = variableType.IsSealed ? variableType.FindDisposeMethod() : typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose));
                cleanup = Expression.Call(variable, disposeMethod);

                checkNull = true;
            }

            if (checkNull)
            {
                cleanup =
                    Expression.IfThen(
                        Expression.NotEqual(variable, Expression.Constant(null, variable.Type)),
                        cleanup
                    );
            }

            var temp = default(ParameterExpression);
            var resource = default(Expression);

            if (variable == Variable)
            {
                // NB: Resource could contain a reference to Variable that needs to be bound in the
                //     enclosing scope. This isn't possible to write in C# due to scoping rules for
                //     variables, but it's valid in the LINQ APIs in general.
                //
                //                          +-------------+
                //                          v             |
                //       Block({ x }, Using(x, R(x), Call(x, foo)))
                //               ^               |
                //               +---------------+
                //
                //     If we're not careful about scoping, we could end up creating:
                //
                //                          +-------------+
                //                          v             |
                //       Block({ x }, Using(x, R(x), Call(x, foo)))
                //                          ^    |
                //                          +----+
                //
                //     So we rewrite the whole thing by adding another temporary variable:
                //
                //                                                        +----------+
                //                                                        v          |
                //       Block({ x }, Block({ t }, Assign(t, R(x)), Using(x, t, Call(x, foo))))
                //               ^                             |
                //               +-----------------------------+
                //
                // NB: We could do a scope tracking visit to Resource to check whether the variable
                //     is being referred to. For now, we'll just apply the additional assignment all
                //     the time, but we could optimize this later (or we could invest in a /o+ type
                //     of flag for all of the expression APIs, so we can optimize the user's code as
                //     well when it exhibits patterns like this; additional Blocks seems common when
                //     generating code from extension nodes of a higher abstraction kind).

                temp = Expression.Parameter(variable.Type);
                resource = temp;
            }
            else
            {
                resource = Resource;
            }

            var res =
                Expression.Block(
                    new[] { variable },
                    Expression.Assign(variable, resource),
                    Expression.TryFinally(
                        Body,
                        cleanup
                    )
                );

            if (temp != null)
            {
                // NB: See remarks above for an explation of the need for this addition scope.

                res =
                    Expression.Block(
                        new[] { temp },
                        Expression.Assign(temp, Resource),
                        res
                    );
            }

            return res;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents a using statement.
        /// </summary>
        /// <param name="resource">The resource managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement Using(Expression resource, Expression body)
        {
            return Using(null, resource, body);
        }

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents a using statement.
        /// </summary>
        /// <param name="variable">The variable containing the resource.</param>
        /// <param name="resource">The resource managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static UsingCSharpStatement Using(ParameterExpression variable, Expression resource, Expression body)
        {
            RequiresCanRead(resource, nameof(resource));
            RequiresCanRead(body, nameof(body));

            var resourceType = resource.Type;

            if (variable != null)
            {
                var variableType = variable.Type;

                ValidateType(variableType);
                ValidateType(resourceType);

                // NB: No non-null value to nullable value assignment allowed here. This is consistent with Assign,
                //     and the C# compiler should insert the Convert node.
                if (!AreReferenceAssignable(variableType, resourceType))
                {
                    throw LinqError.ExpressionTypeDoesNotMatchAssignment(resourceType, variableType);
                }
            }

            var resourceTypeNonNull = resourceType.GetNonNullableType();

            // NB: We don't handle implicit conversions here; the C# compiler can emit a Convert node,
            //     just like it does for those type of conversions in various other places.
            if (!typeof(IDisposable).IsAssignableFrom(resourceTypeNonNull))
            {
                throw LinqError.ExpressionTypeDoesNotMatchAssignment(resourceTypeNonNull, typeof(IDisposable));
            }

            return new UsingCSharpStatement(variable, resource, body);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="UsingCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitUsing(UsingCSharpStatement node)
        {
            return node.Update(VisitAndConvert(node.Variable, nameof(VisitUsing)), Visit(node.Resource), Visit(node.Body));
        }
    }
}
