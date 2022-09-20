// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.CSharp.Expressions.Compiler;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a using statement.
    /// </summary>
    public abstract partial class UsingCSharpStatement : CSharpStatement
    {
        internal UsingCSharpStatement(ReadOnlyCollection<ParameterExpression> variables, Expression body, AwaitInfo? awaitInfo, LambdaExpression? patternDispose)
        {
            Variables = variables;
            Body = body;
            AwaitInfo = awaitInfo;
            PatternDispose = patternDispose;
        }

        /// <summary>
        /// Gets a collection of <see cref="ParameterExpression" /> nodes representing the local variables introduced by the statement.
        /// </summary>
        /// <remarks>
        /// This collection contains declared resources, if any, as well as locals introduced in <see cref="Resource"/> or <see cref="Declarations"/>.
        /// </remarks>
        public ReadOnlyCollection<ParameterExpression> Variables { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the resource if the statement is of the form <c>[await] using (expr) body</c>.
        /// </summary>
        public abstract Expression? Resource { get; }

        /// <summary>
        /// Gets a collection of <see cref="LocalDeclaration" /> declarations representing the resources if the statement is of the form <c>[await] using (declarations) body</c>.
        /// </summary>
        public abstract ReadOnlyCollection<LocalDeclaration>? Declarations { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the body.
        /// </summary>
        public Expression Body { get; }

        /// <summary>
        /// Gets the information required to await the DisposeAsync operation for await using statements.
        /// </summary>
        public new AwaitInfo? AwaitInfo { get; }

        /// <summary>
        /// Gets the (optional) <see cref="LambdaExpression"/> representing how to call the dispose method.
        /// </summary>
        public LambdaExpression? PatternDispose { get; }

        /// <summary>
        /// Gets a Boolean indicating whether the using statement is asynchronous.
        /// </summary>
        public bool IsAsync => AwaitInfo != null;

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
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitUsing(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="variables">The <see cref="Variables" /> property of the result.</param>
        /// <param name="resource">The <see cref="Resource" /> property of the result.</param>
        /// <param name="declarations">The <see cref="Declarations" /> property of the result.</param>
        /// <param name="body">The <see cref="Body" /> property of the result.</param>
        /// <param name="awaitInfo">The <see cref="AwaitInfo"/> property of the result.</param>
        /// <param name="patternDispose">The <see cref="PatternDispose"/> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public UsingCSharpStatement Update(IEnumerable<ParameterExpression> variables, Expression? resource, IEnumerable<LocalDeclaration>? declarations, Expression body, AwaitInfo? awaitInfo, LambdaExpression? patternDispose)
        {
            if (SameElements(ref variables!, Variables) && resource == Resource && SameElements(ref declarations!, Declarations) && body == Body && awaitInfo == AwaitInfo && patternDispose == PatternDispose)
            {
                return this;
            }

            return Make(variables, resource, declarations, body, awaitInfo, patternDispose);
        }

        internal static UsingCSharpStatement Make(IEnumerable<ParameterExpression>? variables, Expression? resource, IEnumerable<LocalDeclaration>? resources, Expression body, AwaitInfo? awaitInfo, LambdaExpression? patternDispose)
        {
            if (patternDispose != null)
            {
                if (patternDispose.Parameters.Count != 1)
                    throw Error.UsingPatternDisposeShouldHaveOneParameter(nameof(patternDispose));
            }

            var variablesList = CheckUniqueVariables(variables, nameof(variables));

            if (resource != null)
            {
                if (resources != null)
                    throw Error.InvalidUsingStatement();

                return WithResource.Make(variablesList, resource, body, awaitInfo, patternDispose);
            }

            if (resources != null)
            {
                var resourcesList = resources.ToReadOnly();

                return WithResources.Make(variablesList, resourcesList, body, awaitInfo, patternDispose);
            }

            throw Error.InvalidUsingStatement();
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        protected override Expression ReduceCore()
        {
            var declaredVariables = new HashSet<ParameterExpression>();

            var res = ReduceAndFlatten(declaredVariables);

            var variables = new List<ParameterExpression>();

            foreach (var variable in Variables)
            {
                if (!declaredVariables.Contains(variable))
                {
                    variables.Add(variable);
                }
            }

            return Expression.Block(variables, res);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression by flattening any nested using statements.
        /// </summary>
        /// <param name="declaredVariables">The set that will be updated to contain all declared variables.</param>
        /// <returns>The reduced expression.</returns>
        protected abstract Expression ReduceAndFlatten(HashSet<ParameterExpression> declaredVariables);

        /// <summary>
        /// Reduces a single using statement to a simpler expression.
        /// </summary>
        /// <param name="variable">The variable of the resource.</param>
        /// <param name="resource">The resource expression.</param>
        /// <param name="body">The body of the using statement.</param>
        /// <param name="declaredVariables">The set that will be updated to contain all declared variables.</param>
        /// <returns>The reduced expression.</returns>
        private protected Expression ReduceSingle(ParameterExpression? variable, Expression resource, Expression body, HashSet<ParameterExpression> declaredVariables)
        {
            var madeTempVariable = false;

            ParameterExpression MakeTempIfNull(ParameterExpression? variable, Type type)
            {
                if (variable == null)
                {
                    variable = Expression.Parameter(type, "__resource");
                    madeTempVariable = true;
                }
                else
                {
                    declaredVariables.Add(variable);
                }

                return variable;
            }

            Expression CallPatternDispose(Expression nonNullResource)
            {
                return ParameterSubstitutor.Substitute(PatternDispose.Body, PatternDispose.Parameters[0], nonNullResource);
            }

            Expression cleanup;
            var checkNull = false;

            var resourceType = resource.Type;

            if (resourceType.IsValueType)
            {
                variable = MakeTempIfNull(variable, resourceType);

                Expression variableValue;

                if (resourceType.IsNullableType())
                {
                    variableValue = Helpers.MakeNullableGetValueOrDefault(variable);
                    checkNull = true;
                }
                else
                {
                    variableValue = variable;
                }

                if (PatternDispose != null)
                {
                    cleanup = CallPatternDispose(variableValue);
                }
                else
                {
                    var disposeMethod = variableValue.Type.FindDisposeMethod(IsAsync);
                    cleanup = Expression.Call(variableValue, disposeMethod);
                }
            }
            else
            {
                var disposableInterface = IsAsync ? typeof(IAsyncDisposable) : typeof(IDisposable);

                variable = MakeTempIfNull(variable, disposableInterface);

                // NB: This optimization would be more effective if the expression compiler would emit a `call` instruction,
                //     but the JIT may still optimize it if it realizes the `callvirt` to the resource is predicated by a
                //     prior null check.
                var variableType = variable.Type;

                if (PatternDispose != null)
                {
                    cleanup = CallPatternDispose(variable);
                }
                else
                {
                    var disposeMethod =
                        variableType.IsSealed
                            ? variableType.FindDisposeMethod(IsAsync)
                            : (IsAsync ? WellKnownMembers.DisposeAsyncMethod : WellKnownMembers.DisposeMethod);

                    cleanup = Expression.Call(variable, disposeMethod);
                }

                checkNull = true;
            }

            if (IsAsync)
            {
                cleanup = Await(cleanup);
            }

            if (checkNull)
            {
                cleanup =
                    Expression.IfThen(
                        Expression.NotEqual(variable, Expression.Constant(null, variable.Type)),
                        cleanup
                    );
            }

            ParameterExpression? temp = default;
            Expression innerResource;

            if (!madeTempVariable)
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

                temp = Expression.Parameter(variable.Type, "__temp");
                innerResource = temp;
            }
            else
            {
                innerResource = resource;
            }

            var res =
                Expression.Block(
                    new[] { variable },
                    Expression.Assign(variable, innerResource),
                    Expression.TryFinally(
                        body,
                        cleanup
                    )
                );

            if (temp != null)
            {
                // NB: See remarks above for an explation of the need for this additional scope.

                res =
                    Expression.Block(
                        new[] { temp },
                        Expression.Assign(temp, resource),
                        res
                    );
            }

            return res;
        }

        private sealed class WithResource : UsingCSharpStatement
        {
            private readonly Expression _resource;

            internal WithResource(ReadOnlyCollection<ParameterExpression> variables, Expression resource, Expression body, AwaitInfo? awaitInfo, LambdaExpression? patternDispose)
                : base(variables, body, awaitInfo, patternDispose)
            {
                _resource = resource;
            }

            public override Expression? Resource => _resource;
            public override ReadOnlyCollection<LocalDeclaration>? Declarations => null;

            internal static WithResource Make(ReadOnlyCollection<ParameterExpression> variables, Expression resource, Expression body, AwaitInfo? awaitInfo, LambdaExpression? patternDispose)
            {
                RequiresCanRead(resource, nameof(resource));

                CheckUsingResourceType(resource.Type, awaitInfo, patternDispose);

                RequiresCanRead(body, nameof(body));

                return new WithResource(variables, resource, body, awaitInfo, patternDispose);
            }

            protected override Expression ReduceAndFlatten(HashSet<ParameterExpression> declaredVariables)
            {
                return ReduceSingle(variable: null, _resource, Body, declaredVariables);
            }
        }

        private sealed class WithResources : UsingCSharpStatement
        {
            private readonly ReadOnlyCollection<LocalDeclaration> _declarations;

            internal WithResources(ReadOnlyCollection<ParameterExpression> variables, ReadOnlyCollection<LocalDeclaration> resources, Expression body, AwaitInfo? awaitInfo, LambdaExpression? patternDispose)
                : base(variables, body, awaitInfo, patternDispose)
            {
                _declarations = resources;
            }

            public override Expression? Resource => null;
            public override ReadOnlyCollection<LocalDeclaration> Declarations => _declarations;

            internal static WithResources Make(ReadOnlyCollection<ParameterExpression> variables, ReadOnlyCollection<LocalDeclaration> resources, Expression body, AwaitInfo? awaitInfo, LambdaExpression? patternDispose)
            {
                RequiresNotNullItems(resources, nameof(resources));
                RequiresNotEmpty(resources, nameof(resources));

                var resourceType = default(Type);

                for (int i = 0, n = resources.Count; i < n; i++)
                {
                    var declaration = resources[i];

                    var declType = declaration.Variable.Type;

                    ValidateType(declType, nameof(resources));

                    if (resourceType == null)
                    {
                        resourceType = declType;
                    }
                    else if (resourceType != declType)
                    {
                        // NB: `using (ResourceType r1 = e1, r2 = e2, ...)`.
                        throw Error.UsingVariableDeclarationsShouldBeConsistentlyTyped(nameof(resources), i);
                    }

                    //
                    // REVIEW: This is cumbersome and makes "declaration" a misnomer. It'd likely be better to only use variables for
                    //         additional locals, thus excluding the declared resource variables. The real issue is that we piggyback
                    //         on Roslyn to establish these scopes for locals (e.g. introduced through `out` variables or in patterns
                    //         using `var` or declarations), and here we're getting a union of variables. Our using node is the first
                    //         node to have a first-class notion of "declarations" to reflect the C# grammar.
                    //
                    if (!variables.Contains(declaration.Variable))
                    {
                        throw Error.UsingVariableNotInScope(declaration.Variable, nameof(resources), i);
                    }
                }

                CheckUsingResourceType(resourceType! /* NB: At least one resource. */, awaitInfo, patternDispose);

                RequiresCanRead(body, nameof(body));

                return new WithResources(variables, resources, body, awaitInfo, patternDispose);
            }

            protected override Expression ReduceAndFlatten(HashSet<ParameterExpression> declaredVariables)
            {
                var expr = Body;

                for (int n = _declarations.Count, i = n - 1; i >= 0; i--)
                {
                    var resource = _declarations[i];

                    expr = ReduceSingle(resource.Variable, resource.Expression, expr, declaredVariables);
                }

                return expr;
            }
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
        public static UsingCSharpStatement Using(Expression resource, Expression body) =>
            Using(variables: null, resource, body, patternDispose: null);

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents a using statement.
        /// </summary>
        /// <param name="variable">The variable containing the resource.</param>
        /// <param name="resource">The resource managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement Using(ParameterExpression variable, Expression resource, Expression body)
        {
            RequiresNotNull(variable, nameof(variable));

            var variables = new[] { variable };
            var declaration = LocalDeclaration(variable, resource);

            return Using(variables, new[] { declaration }, body, patternDispose: null);
        }

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents an await using statement.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the DisposeAsync operation.</param>
        /// <param name="resource">The resource managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement AwaitUsing(AwaitInfo awaitInfo, Expression resource, Expression body) =>
            AwaitUsing(awaitInfo, variables: null, resource, body, patternDispose: null);

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents an await using statement.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the DisposeAsync operation.</param>
        /// <param name="variable">The variable containing the resource.</param>
        /// <param name="resource">The resource managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement AwaitUsing(AwaitInfo awaitInfo, ParameterExpression variable, Expression resource, Expression body)
        {
            RequiresNotNull(variable, nameof(variable));

            var variables = new[] { variable };
            var declaration = LocalDeclaration(variable, resource);

            return AwaitUsing(awaitInfo, variables, new[] { declaration }, body, patternDispose: null);
        }

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents a using statement.
        /// </summary>
        /// <param name="variables">The variables introduced by the statement.</param>
        /// <param name="resource">The resource managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement Using(IEnumerable<ParameterExpression>? variables, Expression resource, Expression body)
            => Using(variables, resource, body, patternDispose: null);

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents a using statement.
        /// </summary>
        /// <param name="variables">The variables introduced by the statement.</param>
        /// <param name="resource">The resource managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <param name="patternDispose">The (optional) lambda expression representing how to call the dispose method.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement Using(IEnumerable<ParameterExpression>? variables, Expression resource, Expression body, LambdaExpression? patternDispose)
            => Using(awaitInfo: null, variables, resource, body, patternDispose);

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents a using statement.
        /// </summary>
        /// <param name="variables">The variables introduced by the statement.</param>
        /// <param name="declarations">The resources managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement Using(IEnumerable<ParameterExpression>? variables, IEnumerable<LocalDeclaration> declarations, Expression body)
            => Using(variables, declarations, body, patternDispose: null);

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents a using statement.
        /// </summary>
        /// <param name="variables">The variables introduced by the statement.</param>
        /// <param name="declarations">The resources managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <param name="patternDispose">The (optional) lambda expression representing how to call the dispose method.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement Using(IEnumerable<ParameterExpression>? variables, IEnumerable<LocalDeclaration> declarations, Expression body, LambdaExpression? patternDispose)
            => Using(awaitInfo: null, variables, declarations, body, patternDispose);

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents an await using statement.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the DisposeAsync operation.</param>
        /// <param name="variables">The variables introduced by the statement.</param>
        /// <param name="resource">The resource managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement AwaitUsing(AwaitInfo? awaitInfo, IEnumerable<ParameterExpression>? variables, Expression resource, Expression body)
            => AwaitUsing(awaitInfo, variables, resource, body, patternDispose: null);

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents an await using statement.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the DisposeAsync operation.</param>
        /// <param name="variables">The variables introduced by the statement.</param>
        /// <param name="resource">The resource managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <param name="patternDispose">The (optional) lambda expression representing how to call the dispose method.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement AwaitUsing(AwaitInfo? awaitInfo, IEnumerable<ParameterExpression>? variables, Expression resource, Expression body, LambdaExpression? patternDispose)
        {
            RequiresNotNull(resource, nameof(resource));

            AssertUsingAwaitInfo(ref awaitInfo, patternDispose);

            return Using(awaitInfo, variables, resource, body, patternDispose);
        }

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents an await using statement.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the DisposeAsync operation.</param>
        /// <param name="variables">The variables introduced by the statement.</param>
        /// <param name="declarations">The resources managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement AwaitUsing(AwaitInfo? awaitInfo, IEnumerable<ParameterExpression>? variables, IEnumerable<LocalDeclaration> declarations, Expression body)
            => AwaitUsing(awaitInfo, variables, declarations, body, patternDispose: null);

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents an await using statement.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the DisposeAsync operation.</param>
        /// <param name="variables">The variables introduced by the statement.</param>
        /// <param name="declarations">The resources managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <param name="patternDispose">The (optional) lambda expression representing how to call the dispose method.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement AwaitUsing(AwaitInfo? awaitInfo, IEnumerable<ParameterExpression>? variables, IEnumerable<LocalDeclaration> declarations, Expression body, LambdaExpression? patternDispose)
        {
            RequiresNotNull(declarations, nameof(declarations));

            AssertUsingAwaitInfo(ref awaitInfo, patternDispose);

            return Using(awaitInfo, variables, declarations, body, patternDispose);
        }

        // NB: The Roslyn compiler binds to the overloads below.

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents a using or an await using statement.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the DisposeAsync operation, or null if not asynchronous.</param>
        /// <param name="variables">The variables introduced by the statement.</param>
        /// <param name="resource">The resource managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <param name="patternDispose">The (optional) lambda expression representing how to call the dispose method.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement Using(AwaitInfo? awaitInfo, IEnumerable<ParameterExpression>? variables, Expression resource, Expression body, LambdaExpression? patternDispose)
        {
            RequiresNotNull(resource, nameof(resource));

            return UsingCSharpStatement.Make(variables, resource, resources: null, body, awaitInfo, patternDispose);
        }

        /// <summary>
        /// Creates a <see cref="UsingCSharpStatement"/> that represents a using or an await using statement.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the DisposeAsync operation, or null if not asynchronous.</param>
        /// <param name="variables">The variables introduced by the statement.</param>
        /// <param name="declarations">The resources managed by the statement.</param>
        /// <param name="body">The body of the statement.</param>
        /// <param name="patternDispose">The (optional) lambda expression representing how to call the dispose method.</param>
        /// <returns>The created <see cref="UsingCSharpStatement"/>.</returns>
        public static UsingCSharpStatement Using(AwaitInfo? awaitInfo, IEnumerable<ParameterExpression>? variables, IEnumerable<LocalDeclaration> declarations, Expression body, LambdaExpression? patternDispose)
        {
            RequiresNotNull(declarations, nameof(declarations));

            return UsingCSharpStatement.Make(variables, resource: null, declarations, body, awaitInfo, patternDispose);
        }

        private static void AssertUsingAwaitInfo(ref AwaitInfo? awaitInfo, LambdaExpression? patternDispose)
        {
            awaitInfo ??= AwaitInfo(patternDispose?.ReturnType ?? typeof(ValueTask));
        }

        internal static void CheckUsingResourceType(Type resourceType, AwaitInfo? awaitInfo, LambdaExpression? patternDispose, bool allowConvertToDisposable = false)
        {
            ValidateType(resourceType, nameof(resourceType));

            var resourceTypeNonNull = resourceType.GetNonNullableType();

            Type disposeReturnType;

            if (patternDispose != null)
            {
                var patternDisposeInputType = patternDispose.Parameters[0].Type;

                if (!AreReferenceAssignable(patternDisposeInputType, resourceTypeNonNull))
                    throw Error.UsingPatternDisposeInputNotCompatibleWithResource(patternDisposeInputType, resourceTypeNonNull, nameof(patternDispose));

                disposeReturnType = patternDispose.ReturnType;
            }
            else
            {
                Type disposableInterface;

                if (awaitInfo != null)
                {
                    disposableInterface = typeof(IAsyncDisposable);
                    disposeReturnType = typeof(ValueTask);
                }
                else
                {
                    disposableInterface = typeof(IDisposable);
                    disposeReturnType = typeof(void);
                }

                if (allowConvertToDisposable)
                {
                    // NB: In the case of foreach, we allow for a conversion to a disposable interface to be emitted.
                    //     While this conversion or type check can sometimes be elided at runtime, we call the factory
                    //     here for its side-effect of doing the neccessary checks.
                    _ = Expression.Convert(Expression.Variable(resourceType), disposableInterface);
                }
                else
                {
                    // NB: We don't handle implicit conversions here; the C# compiler can emit a Convert node,
                    //     just like it does for those type of conversions in various other places.
                    if (!disposableInterface.IsAssignableFrom(resourceTypeNonNull))
                    {
                        throw ExpressionTypeDoesNotMatchAssignment(resourceTypeNonNull, disposableInterface);
                    }
                }
            }

            if (awaitInfo != null)
            {
                awaitInfo.RequiresCanBind(Expression.Parameter(disposeReturnType));
            }
            else
            {
                if (disposeReturnType != typeof(void))
                    throw Error.UsingDisposeShouldReturnVoid(nameof(patternDispose));
            }
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
        protected internal virtual Expression VisitUsing(UsingCSharpStatement node) =>
            node.Update(
                VisitAndConvert(node.Variables, nameof(VisitUsing)),
                Visit(node.Resource),
                node.Declarations != null ? Visit(node.Declarations, VisitLocalDeclaration) : null,
                Visit(node.Body),
                VisitAwaitInfo(node.AwaitInfo),
                VisitAndConvert(node.PatternDispose, nameof(VisitUsing))
            );
    }
}
