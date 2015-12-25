// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions.Compiler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Expressions.Compiler;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents an asynchronous lambda expression.
    /// </summary>
    public abstract partial class AsyncLambdaCSharpExpression : CSharpExpression
    {
        internal AsyncLambdaCSharpExpression(Type delegateType, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            Type = delegateType;
            Body = body;
            Parameters = parameters;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.AsyncLambda;

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type { get; }

        /// <summary>
        /// Gets the body of the lambda expression.
        /// </summary>
		/// <returns>An <see cref="Expression" /> that represents the body of the lambda expression.</returns>
        public Expression Body { get; }

        /// <summary>
        /// Gets the parameters of the lambda expression.
        /// </summary>
		/// <returns>A <see cref="ReadOnlyCollection{T}" /> of <see cref="ParameterExpression" /> objects that represent the parameters of the lambda expression.</returns>
        public ReadOnlyCollection<ParameterExpression> Parameters { get; }

        /// <summary>
        /// Produces a delegate that represents the asynchronous lambda expression.
        /// </summary>
        /// <returns>A <see cref="Delegate" /> that contains the compiled version of the asynchronous lambda expression.</returns>
        public Delegate Compile()
        {
            return CompileCore();
        }

        /// <summary>
        /// Produces a delegate that represents the asynchronous lambda expression.
        /// </summary>
        /// <returns>A <see cref="Delegate" /> that contains the compiled version of the asynchronous lambda expression.</returns>
        protected abstract Delegate CompileCore();
    }

    // NB: Expression<TDelegate> is sealed, so we can't achieve compatibility with that type here.
    //     As a result, top-level async lambdas won't be assignable to Expression<TDelegate> and
    //     we would have to educate the C# compiler about AsyncCSharpExpression<TDelegate>. This
    //     would also be a spec addition. Alternatively, we have to unseal Expression<TDelegate>
    //     in order for a conversion of an async lambda to the base type to work. The base type
    //     then needs a virtual for the Compile method.

    /// <summary>
    /// Represents an asynchronous lambda expression.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate represented by this expression.</typeparam>
    public sealed class AsyncCSharpExpression<TDelegate> : AsyncLambdaCSharpExpression
    {
        internal AsyncCSharpExpression(Expression body, ReadOnlyCollection<ParameterExpression> parameters)
            : base(typeof(TDelegate), body, parameters)
        {
        }

        internal static AsyncCSharpExpression<TDelegate> Create(Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            return new AsyncCSharpExpression<TDelegate>(body, parameters);
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitAsyncLambda(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="body">The <see cref="AsyncLambdaCSharpExpression.Body" /> property of the result.</param>
        /// <param name="parameters">The <see cref="AsyncLambdaCSharpExpression.Parameters" /> property of the result. </param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public AsyncCSharpExpression<TDelegate> Update(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (body == base.Body && parameters == base.Parameters)
            {
                return this;
            }

            return CSharpExpression.AsyncLambda<TDelegate>(body, parameters);
        }

        /// <summary>
        /// Compiles the asynchronous lambda expression described by the expression tree into executable code and produces a delegate that represents the lambda expression.
        /// </summary>
        /// <returns>A delegate of type <typeparamref name="TDelegate" /> that represents the compiled asynchronous lambda expression described by the <see cref="AsyncCSharpExpression{TDelegate}" />.</returns>
        public new TDelegate Compile()
        {
            return (TDelegate)(object)CompileCore();
        }

        /// <summary>
        /// Produces a delegate that represents the asynchronous lambda expression.
        /// </summary>
        /// <returns>A <see cref="Delegate" /> that contains the compiled version of the asynchronous lambda expression.</returns>
        protected override Delegate CompileCore()
        {
            return ((LambdaExpression)ReduceCore()).Compile();
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            // TODO: Investigate why an Invoke(AsyncLambda(x, x), x) causes invalid results to be returned
            //       if we don't hide the reduced expression in a block as we do below. It looks like the
            //       culprit is in StackSpiller.RewriteInvocationExpression where it tries to inline the
            //       body of the lambda in case invocation target is a lambda.
            return Expression.Block(ReduceCore());
        }

        private Expression ReduceCore()
        {
            AwaitChecker.Check(Body);

            const int ExprCount = 1 /* new builder */ + 1 /* new state machine */ + 1 /* initial state */ + 1 /* start state machine */;

            var invokeMethod = typeof(TDelegate).GetMethod("Invoke");
            var returnType = invokeMethod.ReturnType;

            var builderType = default(Type);
            var exprs = default(Expression[]);

            if (returnType == typeof(void))
            {
                builderType = typeof(AsyncVoidMethodBuilder);
                exprs = new Expression[ExprCount];
            }
            else if (returnType == typeof(Task))
            {
                builderType = typeof(AsyncTaskMethodBuilder);
                exprs = new Expression[ExprCount + 1];
            }
            else
            {
                Debug.Assert(returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>));
                builderType = typeof(AsyncTaskMethodBuilder<>).MakeGenericType(returnType.GetGenericArguments()[0]);
                exprs = new Expression[ExprCount + 1];
            }

            var i = 0;

            var builderVar = Expression.Parameter(builderType, "__builder");
            var stateMachineVar = Expression.Parameter(typeof(RuntimeAsyncStateMachine), "__statemachine");
            var stateVar = Expression.Parameter(typeof(int), "__state");

            var builderCreateMethod = builderType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            var createBuilder = Expression.Assign(builderVar, Expression.Call(builderCreateMethod));
            exprs[i++] = createBuilder;

            var variables = default(IEnumerable<ParameterExpression>);
            var body = RewriteBody(stateVar, builderVar, stateMachineVar, out variables);

            var stateMachineCtor = stateMachineVar.Type.GetConstructor(new[] { typeof(Action) });
            var createStateMachine = Expression.Assign(stateMachineVar, Expression.New(stateMachineCtor, body));
            exprs[i++] = createStateMachine;

            exprs[i++] = Expression.Assign(stateVar, Helpers.CreateConstantInt32(-1));

            var startMethod = builderType.GetMethod("Start", BindingFlags.Public | BindingFlags.Instance);
            exprs[i++] = Expression.Call(builderVar, startMethod.MakeGenericMethod(typeof(RuntimeAsyncStateMachine)), stateMachineVar);

            if (returnType != typeof(void))
            {
                exprs[i] = Expression.Property(builderVar, "Task");
            }

            var rewritten = Expression.Block(new[] { builderVar, stateMachineVar, stateVar }.Concat(variables), exprs);

            var optimized = Optimizer.Optimize(rewritten);

            var res = Expression.Lambda<TDelegate>(optimized, Parameters);
            return res;
        }

        private Expression RewriteBody(ParameterExpression stateVar, ParameterExpression builderVar, ParameterExpression stateMachineVar, out IEnumerable<ParameterExpression> variables)
        {
            const int ExprCount = 1 /* local state var */ + 1 /* TryCatch */ + 2 /* state = -2; SetResult */ + 1 /* Label */;

            var locals = default(ParameterExpression[]);
            var exprs = default(Expression[]);

            var result = default(ParameterExpression);
            var ex = Expression.Parameter(typeof(Exception), "exception");

            var exit = Expression.Label("__exit");

            var hoistedVars = new Dictionary<Type, ParameterExpression>();

            var getVariable = new Func<Type, string, ParameterExpression>((t, s) =>
            {
                var p = default(ParameterExpression);
                if (!hoistedVars.TryGetValue(t, out p))
                {
                    p = Expression.Parameter(t, s + hoistedVars.Count);
                    hoistedVars.Add(t, p);
                }

                return p;
            });

            var awaitOnCompletedMethod = builderVar.Type.GetMethod("AwaitOnCompleted", BindingFlags.Public | BindingFlags.Instance);
            var awaitOnCompletedArgs = new Type[] { default(Type), typeof(RuntimeAsyncStateMachine) };

            var onCompletedFactory = new Func<Expression, Expression>(awaiter =>
            {
                awaitOnCompletedArgs[0] = awaiter.Type;
                var awaitOnCompletedMethodClosed = awaitOnCompletedMethod.MakeGenericMethod(awaitOnCompletedArgs);
                return Expression.Call(builderVar, awaitOnCompletedMethodClosed, awaiter, stateMachineVar);
            });

            var reduced = Reducer.Reduce(Body);

            var lowered = new CatchRewriter().Visit(reduced);
            lowered = new FinallyAndFaultRewriter().Visit(lowered);

            var bright = AliasEliminator.Eliminate(lowered);
            var spilled = Spiller.Spill(bright);

            var localStateVar = Expression.Parameter(typeof(int), "__localState");
            var awaitRewriter = new AwaitRewriter(localStateVar, stateVar, getVariable, onCompletedFactory, exit);
            var rewrittenBody = awaitRewriter.Visit(spilled);

            var newBody = rewrittenBody;
            if (Body.Type != typeof(void) && builderVar.Type.IsGenericType /* if not ATMB<T>, no result assignment needed */)
            {
                result = Expression.Parameter(Body.Type, "__result");
                newBody = Expression.Assign(result, rewrittenBody);
                locals = new[] { localStateVar, result };
            }
            else
            {
                locals = new[] { localStateVar };
            }

            exprs = new Expression[ExprCount];

            // NB: We need to rewrite branching involving typed labels and percolate assignments in
            //     order to avoid reduced await expressions causing branching into non-void expressions
            //     which is not allowed in the lambda compiler.
            newBody = new TypedLabelRewriter().Visit(newBody);
            newBody = AssignmentPercolator.Percolate(newBody);

            var i = 0;

            var resumeList = awaitRewriter.ResumeList;

            if (resumeList.Count > 0)
            {
                newBody =
                    Expression.Block(
                        typeof(void),
                        Expression.Switch(stateVar, resumeList.ToArray()),
                        newBody
                    );
            }
            else
            {
                newBody = Helpers.CreateVoid(newBody);
            }

            exprs[i++] =
                Expression.Assign(localStateVar, stateVar);

            exprs[i++] =
                Expression.TryCatch(
                    newBody,
                    Expression.Catch(ex,
                        Expression.Block(
                            Expression.Assign(stateVar, Helpers.CreateConstantInt32(-2)),
                            Expression.Call(builderVar, builderVar.Type.GetMethod("SetException"), ex),
                            Expression.Return(exit)
                        )
                    )
                );

            exprs[i++] = Expression.Assign(stateVar, Helpers.CreateConstantInt32(-2));

            if (result != null)
            {
                exprs[i++] = Expression.Call(builderVar, builderVar.Type.GetMethod("SetResult"), result);
            }
            else
            {
                exprs[i++] = Expression.Call(builderVar, builderVar.Type.GetMethod("SetResult"));
            }

            exprs[i++] = Expression.Label(exit);

            var body = Expression.Block(locals, exprs);
            var res = Expression.Lambda<Action>(body);

            variables = hoistedVars.Values.Concat(awaitRewriter.HoistedVariables);
            return res;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an <see cref="AsyncLambdaCSharpExpression" /> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression" /> to set the <see cref="AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An array of <see cref="ParameterExpression" /> objects to use to populate the <see cref="AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="AsyncLambdaCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to AsyncLambda and the <see cref="AsyncLambdaCSharpExpression.Body" /> and <see cref="AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncLambdaCSharpExpression AsyncLambda(Expression body, params ParameterExpression[] parameters)
        {
            return AsyncLambda(body, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="AsyncLambdaCSharpExpression" /> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression" /> to set the <see cref="AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterExpression" /> objects to use to populate the <see cref="AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="AsyncLambdaCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to AsyncLambda and the <see cref="AsyncLambdaCSharpExpression.Body" /> and <see cref="AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static AsyncLambdaCSharpExpression AsyncLambda(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            ContractUtils.RequiresNotNull(body, nameof(body));

            var parameterList = parameters.ToReadOnly();

            var count = parameterList.Count;
            var types = new Type[count + 1];

            if (count > 0)
            {
                var set = new Set<ParameterExpression>(count);

                for (var i = 0; i < count; i++)
                {
                    var parameter = parameterList[i];

                    ValidateAsyncParameter(parameter);

                    types[i] = parameter.Type;

                    if (set.Contains(parameter))
                    {
                        throw LinqError.DuplicateVariable(parameter);
                    }

                    set.Add(parameter);
                }
            }

            if (body.Type == typeof(void))
            {
                types[count] = typeof(Task); // REVIEW: OK to default to Task?
            }
            else
            {
                types[count] = typeof(Task<>).MakeGenericType(body.Type);
            }

            return CreateAsyncLambda(DelegateHelpers.MakeDelegateType(types), body, parameterList);
        }

        /// <summary>
        /// Creates an <see cref="AsyncLambdaCSharpExpression" /> using the specified delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="Type" /> that represents a delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="Expression" /> to set the <see cref="AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An array of <see cref="ParameterExpression" /> objects to use to populate the <see cref="AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="AsyncLambdaCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to AsyncLambda and the <see cref="AsyncLambdaCSharpExpression.Body" /> and <see cref="AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncLambdaCSharpExpression AsyncLambda(Type delegateType, Expression body, params ParameterExpression[] parameters)
        {
            return AsyncLambda(delegateType, body, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="AsyncLambdaCSharpExpression" /> using the specified delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="Type" /> that represents a delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="Expression" /> to set the <see cref="AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterExpression" /> objects to use to populate the <see cref="AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="AsyncLambdaCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to AsyncLambda and the <see cref="AsyncLambdaCSharpExpression.Body" /> and <see cref="AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncLambdaCSharpExpression AsyncLambda(Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
        {
            var parameterList = parameters.ToReadOnly();

            ValidateAsyncLambdaArgs(delegateType, ref body, parameterList);

            return CreateAsyncLambda(delegateType, body, parameterList);
        }

        /// <summary>
        /// Creates an <see cref="AsyncCSharpExpression{TDelegate}" /> where the delegate type is known at compile time.
        /// </summary>
        /// <param name="body">An <see cref="Expression" /> to set the <see cref="AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An array of <see cref="ParameterExpression" /> objects to use to populate the <see cref="AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="AsyncLambdaCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to AsyncLambda and the <see cref="AsyncLambdaCSharpExpression.Body" /> and <see cref="AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncCSharpExpression<TDelegate> AsyncLambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
        {
            return AsyncLambda<TDelegate>(body, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="AsyncCSharpExpression{TDelegate}" /> where the delegate type is known at compile time.
        /// </summary>
        /// <param name="body">An <see cref="Expression" /> to set the <see cref="AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterExpression" /> objects to use to populate the <see cref="AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="AsyncLambdaCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to AsyncLambda and the <see cref="AsyncLambdaCSharpExpression.Body" /> and <see cref="AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncCSharpExpression<TDelegate> AsyncLambda<TDelegate>(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            var parameterList = parameters.ToReadOnly();

            ValidateAsyncLambdaArgs(typeof(TDelegate), ref body, parameterList);

            return new AsyncCSharpExpression<TDelegate>(body, parameterList);
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}" /> where the delegate type is known at compile time.
        /// </summary>
        /// <param name="isAsync">Indicates whether the resulting lambda is synchronous or asynchronous.</param>
        /// <param name="body">An <see cref="Expression" /> representing the body of the lambda.</param>
        /// <param name="parameters">An array of <see cref="ParameterExpression" /> objects that represent the parameters passed to the lambda.</param>
        /// <returns>An expression representing a lambda with the specified body and parameters.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(bool isAsync, Expression body, params ParameterExpression[] parameters)
        {
            // NB: If we were to omit the isAsync parameter, we could try to check the TDelegate return type and
            //     match it with the body type to determine whether the intent is to create a sync or an async
            //     lambda. However, for a return type of void, it's unclear what the intent is. We could resort
            //     to scanning the body looking for await expressions. None of these techniques aligns well with
            //     the explicitness in the language wrt the async modifier, so we shy away from any such smarts.
            //
            // NB: Ultimately, this overload should likely go away and the language should bind to AsyncLambda in
            //     case of emitting an expression tree for an async lambda. Right now, the overload exists to have
            //     assignment compatibility with Expression<T> for async lambdas, so we don't have to extend the
            //     language with AsyncCSharpExpression<T> as a type to consider for lambda convertibility. Notice
            //     though that the async case here causes an expression tree to be created that's not very natural
            //     to say the least. It does, however, compile and evaluate just fine at runtime. In order to lift
            //     this restriction, we have to either extend LINQ to support async Expression<T> or unseal it to
            //     allow for our library to create derived async variants.

            if (isAsync)
            {
                var async = CSharpExpression.AsyncLambda<TDelegate>(body, parameters);
                return Expression.Lambda<TDelegate>(Expression.Invoke(async, parameters), parameters);
            }
            else
            {
                return Expression.Lambda<TDelegate>(body, parameters);
            }
        }

        private static void ValidateAsyncLambdaArgs(Type delegateType, ref Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            ContractUtils.RequiresNotNull(delegateType, nameof(delegateType));
            RequiresCanRead(body, nameof(body));

            if (!typeof(MulticastDelegate).IsAssignableFrom(delegateType) || delegateType == typeof(MulticastDelegate))
            {
                throw LinqError.LambdaTypeMustBeDerivedFromSystemDelegate();
            }

            var count = parameters.Count;

            var method = delegateType.GetMethod("Invoke"); // TODO: use cache from LINQ
            var parametersCached = method.GetParametersCached();

            if (parametersCached.Length != 0)
            {
                if (parametersCached.Length != count)
                {
                    throw LinqError.IncorrectNumberOfLambdaDeclarationParameters();
                }

                var set = new Set<ParameterExpression>(count);

                for (var i = 0; i < count; i++)
                {
                    var parameter = parameters[i];
                    ValidateAsyncParameter(parameter);

                    var parameterType = parametersCached[i].ParameterType;

                    if (!TypeUtils.AreReferenceAssignable(parameter.Type, parameterType))
                    {
                        throw LinqError.ParameterExpressionNotValidAsDelegate(parameter.Type, parameterType);
                    }

                    if (set.Contains(parameter))
                    {
                        throw LinqError.DuplicateVariable(parameter);
                    }

                    set.Add(parameter);
                }
            }
            else if (count > 0)
            {
                throw LinqError.IncorrectNumberOfLambdaDeclarationParameters();
            }

            var returnType = method.ReturnType;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = returnType.GetGenericArguments()[0];

                if (!TypeUtils.AreReferenceAssignable(resultType, body.Type) && !TryQuote(resultType, ref body))
                {
                    throw LinqError.ExpressionTypeDoesNotMatchReturn(body.Type, method.ReturnType);
                }
            }
            else if (returnType != typeof(void) && returnType != typeof(Task))
            {
                throw Error.AsyncLambdaInvalidReturnType(returnType);
            }
        }

        private static AsyncLambdaCSharpExpression CreateAsyncLambda(Type delegateType, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            // TODO: use cache and lambda factory functionality from LINQ
            var create = typeof(AsyncCSharpExpression<>).MakeGenericType(delegateType).GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic);
            return (AsyncLambdaCSharpExpression)create.Invoke(null, new object[] { body, parameters });
        }

        private static void ValidateAsyncParameter(ParameterExpression parameter)
        {
            RequiresCanRead(parameter, "parameters");

            if (parameter.IsByRef)
            {
                throw Error.AsyncLambdaCantHaveByRefParameter(parameter);
            }
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="AsyncCSharpExpression{TDelegate}" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitAsyncLambda<TDelegate>(AsyncCSharpExpression<TDelegate> node)
        {
            return node.Update(Visit(node.Body), VisitAndConvert<ParameterExpression>(node.Parameters, nameof(VisitAsyncLambda)));
        }
    }
}
