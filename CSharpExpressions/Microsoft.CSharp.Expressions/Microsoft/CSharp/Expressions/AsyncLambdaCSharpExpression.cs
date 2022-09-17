// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Expressions.Compiler;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.CSharp.Expressions.Compiler;

using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

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
        public Delegate Compile() => CompileCore();

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

        internal static AsyncCSharpExpression<TDelegate> Create(Expression body, ReadOnlyCollection<ParameterExpression> parameters) => new AsyncCSharpExpression<TDelegate>(body, parameters);

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitAsyncLambda(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="body">The <see cref="AsyncLambdaCSharpExpression.Body" /> property of the result.</param>
        /// <param name="parameters">The <see cref="AsyncLambdaCSharpExpression.Parameters" /> property of the result. </param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public AsyncCSharpExpression<TDelegate> Update(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (body == Body && SameElements(ref parameters!, Parameters))
            {
                return this;
            }

            return CSharpExpression.AsyncLambda<TDelegate>(body, parameters);
        }

        /// <summary>
        /// Compiles the asynchronous lambda expression described by the expression tree into executable code and produces a delegate that represents the lambda expression.
        /// </summary>
        /// <returns>A delegate of type <typeparamref name="TDelegate" /> that represents the compiled asynchronous lambda expression described by the <see cref="AsyncCSharpExpression{TDelegate}" />.</returns>
        public new TDelegate Compile() => (TDelegate)(object)CompileCore();

        /// <summary>
        /// Produces a delegate that represents the asynchronous lambda expression.
        /// </summary>
        /// <returns>A <see cref="Delegate" /> that contains the compiled version of the asynchronous lambda expression.</returns>
        protected override Delegate CompileCore() => ((LambdaExpression)ReduceCore()).Compile();

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
            //
            // First, check to see whether await expressions occur in forbidden places such as the body
            // of a lock statement or a filter of a catch clause. This is done using a C# visitor so we
            // can analyze the original (non-reduced) intent of the nodes.
            //
            AwaitChecker.Check(Body);

            const int ExprCount = 1 /* new builder */ + 1 /* new state machine */ + 1 /* initial state */ + 1 /* start state machine */;

            //
            // Next, analyze what kind of async method builder we need by analyzing the return type of
            // the async lambda. Based on this we will determine how many expressions we need in the
            // rewritten top-level async method which sets up the builder, state machine, and kicks off
            // the async logic.
            //
            var invokeMethod = typeof(TDelegate).GetMethod("Invoke")!;
            var returnType = invokeMethod.ReturnType;

            var builderInfo = GetAsyncMethodBuilderInfo(returnType);
            var builderType = builderInfo.BuilderType;

            Expression[] exprs;

            if (returnType == typeof(void))
            {
                Debug.Assert(builderType == typeof(AsyncVoidMethodBuilder));
                exprs = new Expression[ExprCount];
            }
            else
            {
                exprs = new Expression[ExprCount + 1];
            }

            var i = 0;

            var builderVar = Expression.Parameter(builderType, "__builder");
            var stateMachineVar = Expression.Parameter(typeof(RuntimeAsyncStateMachine), "__statemachine");
            var stateVar = Expression.Parameter(typeof(int), "__state");

            //
            // __builder = ATMB.Create();
            //
            var createBuilder = Expression.Assign(builderVar, Expression.Call(builderInfo.Create));
            exprs[i++] = createBuilder;

            //
            // This is where we rewrite the body of the async lambda into an Action delegate we can pass
            // to the runtime async state machine. The collection of variables returned by the rewrite
            // step will contain all the variables that need to be hoisted to the heap. We do this by
            // simply declaring them in the scope around the rewritten (synchronous) body lambda, thus
            // causing the lambda compiler to create a closure.
            //
            // Note we could take the rewritten body and the variables collection and construct a struct
            // implementing IAsyncStateMachine here. However, we don't readily have access to a TypeBuilder
            // for the dynamically generated method created by the lambda compiler higher up, so we'd
            // have to go through some hoops here. For now, we'll rely on a delegate with a closure that
            // consists of a set of StrongBox objects and gets passed to a RuntimeAsyncStateMachine. We
            // can decide to optimize this using a struct later (but then it may be worth making closures
            // in the lambda compiler a bit cheaper by creating a display class as well).
            //
            var body = RewriteBody(builderInfo, stateVar, builderVar, stateMachineVar, out IEnumerable<ParameterExpression> variables);

            //
            // __statemachine = new RuntimeAsyncStateMachine(body);
            //
            var stateMachineCtor = stateMachineVar.Type.GetConstructor(new[] { typeof(Action) })!;
            var createStateMachine = Expression.Assign(stateMachineVar, Expression.New(stateMachineCtor, body));
            exprs[i++] = createStateMachine;

            //
            // __state = -1;
            //
            exprs[i++] = Expression.Assign(stateVar, CreateConstantInt32(-1));

            //
            // __builder.Start(ref __statemachine);
            //
            var startMethod = builderInfo.Start;
            exprs[i++] = Expression.Call(builderVar, startMethod.MakeGenericMethod(typeof(RuntimeAsyncStateMachine)), stateMachineVar);

            //
            // return __builder.Task;
            //
            if (returnType != typeof(void))
            {
                exprs[i] = Expression.Property(builderVar, builderInfo.Task);
            }

            //
            // The body of the top-level reduced method contains the setup and kick off of the state
            // machine. Note the variables returned from the rewrite step of the body are included here
            // in order to hoist them to the heap.
            //

            // REVIEW: Should we ensure all hoisted variables get assigned default values? This could
            //         matter if the resulting lambda gets inlined in an invocation expression and is
            //         invoked repeatedly, causing locals to be reused. Or should we assume definite
            //         assignment here (or enforce it)?
            var rewritten = Expression.Block(new[] { builderVar, stateMachineVar, stateVar }.Concat(variables), exprs);

            //
            // Finally, run a fairly trivial optimizer to reduce excessively nested blocks that were
            // introduced by the rewrite steps above. This is strictly optional and we could consider
            // an optimization flag of LambdaExpression and AsyncLambdaCSharpExpression that's more
            // generally useful (see ExpressionOptimizationExtensions for an early sketch of some of
            // these optimizations).
            //
            var optimized = Optimizer.Optimize(rewritten);

            //
            // The result is a synchronous lambda that kicks off the async state machine and returns the
            // resulting task (if non-void-returning).
            //
            var res = Expression.Lambda<TDelegate>(optimized, Parameters);
            return res;
        }

        private Expression RewriteBody(AsyncMethodBuilderInfo builderInfo, ParameterExpression stateVar, ParameterExpression builderVar, ParameterExpression stateMachineVar, out IEnumerable<ParameterExpression> variables)
        {
            const int ExprCount = 1 /* local state var */ + 1 /* TryCatch */ + 2 /* state = -2; SetResult */ + 1 /* Label */;

            var locals = default(ParameterExpression[]);
            var exprs = default(Expression[]);

            var result = default(ParameterExpression);
            var ex = Expression.Parameter(typeof(Exception), "exception");

            var exit = Expression.Label("__exit");

            //
            // Keep a collection and a helper function to create variables that are hoisted to the heap
            // for use by await sites. Because only one await site can be active at a time, we can reuse
            // variables introduced for these, e.g. for awaiters of the same type.
            //
            // NB: We can replace the getVariable helper function with a local function in C# 7.0 if we
            //     get that feature.
            //
            var hoistedVars = new Dictionary<Type, ParameterExpression>();

            var getVariable = new Func<Type, string, ParameterExpression>((t, s) =>
            {
                if (!hoistedVars.TryGetValue(t, out ParameterExpression? p))
                {
                    p = Expression.Parameter(t, s + hoistedVars.Count);
                    hoistedVars.Add(t, p);
                }

                return p;
            });

            //
            // Some helpers to call AwaitOnCompleted on the async method builder for use by each await site in
            // the asynchronous code path, e.g.
            //
            //   if (!awaiter.IsCompleted)
            //   {
            //     __state = n;
            //     __builder.AwaitOnCompleted<AwaiterType, RuntimeAsyncStateMachine>(ref awaiter, ref __statemachine);
            //   }
            //
            // NB: We can replace the onCompletedFactory helper function with a local function in C# 7.0 if we
            //     get that feature.
            //
            // REVIEW: Do we have any option to call UnsafeAwaitOnCompleted at runtime, i.e. can we detect
            //         the cases where we can do this and can we do it wrt security restrictions on code
            //         that gets emitted dynamically?
            //
            var awaitOnCompletedMethod = builderInfo.AwaitOnCompleted;
            var awaitOnCompletedArgs = new Type[] { default!, typeof(RuntimeAsyncStateMachine) };

            var onCompletedFactory = new Func<Expression, Expression>(awaiter =>
            {
                awaitOnCompletedArgs[0] = awaiter.Type;
                var awaitOnCompletedMethodClosed = awaitOnCompletedMethod.MakeGenericMethod(awaitOnCompletedArgs);
                return Expression.Call(builderVar, awaitOnCompletedMethodClosed, awaiter, stateMachineVar);
            });

            //
            // First, reduce all nodes in the body except for await nodes. This makes subsequent rewrite
            // steps easier because we reduce to the known subset of LINQ nodes.
            //
            var reduced = Reducer.Reduce(Body);

            //
            // Next, make sure we don't end up hoisting RefHolder<T> instances that represent ref locals
            // using a Span<T> inside. Note that this step would still exist if we had proper support for
            // ref locals in BlockExpression; we need to hunt down these cases and rewrite them.
            //
            var noRefLocals = RefLocalRewriter.Rewrite(reduced);

            //
            // Next, rewrite exception handlers to synthetic equivalents where needed. This supports the
            // C# 6.0 features to await in catch and finally handlers (in addition to fault handlers in
            // order to support all LINQ nodes, which can be restricted if we want).
            //
            // This step also deals with pending branches out of exception handlers in order to properly
            // 'leave' protected regions and execute the branch after the exception handling construct.
            //
            var lowered = new CatchRewriter().Visit(noRefLocals);
            lowered = new FinallyAndFaultRewriter().Visit(lowered);

            //
            // Next, eliminate any aliasing of variables that relies on the nesting of scoped nodes in
            // the LINQ APIs (e.g. nested blocks with reused ParmeterExpression nodes). We do this so we
            // don't have to worry about hoisting variables out of the async lambda body and causing the
            // meaning of the hoisted variable to change to another use of the same variable in a scoped
            // tree node higher up. This can happen during stack spilling, e.g.
            //
            //   {
            //     int x;                   // @0
            //     {
            //       int x;                 // @0 - same instance shadowing x in outer block
            //       F(x, await t);
            //     }
            //   }
            //
            // ==>
            //
            //   int x;                     // @0 hoisted to heap by stack spilling
            //   () =>
            //   {
            //     int x;                   // !!! the binding of x has now changed to the declaration
            //     __spill0 = x;            // !!! in the inner block
            //     __spill1 = await t;
            //     F(__spill0, __spill1);
            //   }
            //
            var aliasFree = AliasEliminator.Eliminate(lowered);

            //
            // Next, perform stack spilling in order to be able to pause the asynchronous method in the
            // middle of an expression without changing the left-to-right subexpression evaluation
            // semantics dictated by the C# language specification, e.g.
            //
            //   Console.ReadLine() + await Task.FromResult(Console.ReadLine)
            //
            // The first side-effect of reading from the console should happen before the second one
            // in the async operation.
            //
            var spilled = Spiller.Spill(aliasFree);

            //
            // Next, rewrite await expressions to the awaiter pattern with IsCompleted, OnCompleted,
            // and GetResult. This is where the heavy lifting (quite literally so) takes place and the
            // state machine is built. Other than rewriting await expressions, this step also takes care
            // of emitting the switch table for reentering the state machine, reentering nested try
            // blocks, and hoisting of locals. For more information, see AwaitRewriter.
            //
            // Note we need to introduce another local to keep the state of the async state machine in
            // order to deal with reentrancy of the async state machine via the OnCompleted call on an
            // awaiter while we're still exiting the state machine. This is a subtle race which we avoid
            // by making all decisions about jumps and state transitions based on a local copy of the
            // hoisted state variable used by the state machine:
            //
            //   int __localState = __state;
            //   switch (__localState)
            //   {
            //     ...
            //   }
            //
            // NB: Right now, locals used in await sites get hoisted to the heap eagerly rather than
            //     getting hoisted upon taking the asynchronous code path. This is an opportunity for
            //     future optimization, together with the use of a struct for the async state machine.
            //
            var localStateVar = Expression.Parameter(typeof(int), "__localState");
            var awaitRewriter = new AwaitRewriter(localStateVar, stateVar, getVariable, onCompletedFactory, exit);
            var rewrittenBody = awaitRewriter.Visit(spilled);

            //
            // Next, store the result of the rewritten body if the async method is non-void-returning.
            // Note this assignment will typically have a RHS which contains a non-void block expression
            // that originated from running the AwaitRewriter.
            //
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

            //
            // Next, we need to rewrite branching involving typed labels and percolate assignments in
            // order to avoid reduced await expressions causing branching into non-void expressions
            // which is not allowed in the lambda compiler. An example os this is shown in the comments
            // for AssignmentPercolator.
            //
            newBody = new TypedLabelRewriter().Visit(newBody);
            newBody = AssignmentPercolator.Percolate(newBody);

            var i = 0;

            //
            // Next, put the jump table to resume the async state machine on top of the rewritten body
            // returned from the AwaitRewriter. Note that the AwaitRewriter takes care of emitting the
            // nested resume jump tables for try statements, so we just have to stick the top-level
            // table around the body here. We don't do this in AwaitRewriter just to reduce the amount
            // of expression tree cloning incurred by TypedLabelRewriter and AssignmentPercolator given
            // that we know the switch tables don't contain any expressions that need such rewriting.
            //
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
                newBody = CreateVoid(newBody);
            }

            //
            // int __localState = __state;
            //
            exprs[i++] =
                Expression.Assign(localStateVar, stateVar);

            //
            // try
            // {
            //    // body
            // }
            // catch (Exception ex)
            // {
            //    __state = -2;
            //    __builder.SetException(ex);
            //    goto __exit;
            // }
            //
            exprs[i++] =
                Expression.TryCatch(
                    newBody,
                    Expression.Catch(ex,
                        Expression.Block(
                            Expression.Assign(stateVar, CreateConstantInt32(-2)),
                            Expression.Call(builderVar, builderInfo.SetException, ex),
                            Expression.Return(exit)
                        )
                    )
                );

            //
            // __state = -2;
            //
            exprs[i++] = Expression.Assign(stateVar, CreateConstantInt32(-2));

            //
            // __builder.SetResult(__result);
            //
            if (result != null)
            {
                exprs[i++] = Expression.Call(builderVar, builderInfo.SetResult, result);
            }
            else
            {
                exprs[i++] = Expression.Call(builderVar, builderInfo.SetResult);
            }

            //
            // __exit:
            //   return;
            //
            exprs[i++] = Expression.Label(exit);

            //
            // Finally, create the Action with the rewritten async lambda body that gets passed to the
            // runtime async state machine and hoist any newly introduced variables for awaiters and
            // such to the outer scope in order to get them stored on the heap rather than the stack.
            //
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
        public static AsyncLambdaCSharpExpression AsyncLambda(Expression body, params ParameterExpression[]? parameters) => AsyncLambda(body, (IEnumerable<ParameterExpression>?)parameters);

        /// <summary>
        /// Creates an <see cref="AsyncLambdaCSharpExpression" /> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression" /> to set the <see cref="AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterExpression" /> objects to use to populate the <see cref="AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="AsyncLambdaCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to AsyncLambda and the <see cref="AsyncLambdaCSharpExpression.Body" /> and <see cref="AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static AsyncLambdaCSharpExpression AsyncLambda(Expression body, IEnumerable<ParameterExpression>? parameters)
        {
            ContractUtils.RequiresNotNull(body, nameof(body));

            var parameterList = parameters.ToReadOnly();

            var count = parameterList.Count;
            var types = new Type[count + 1];

            if (count > 0)
            {
                var set = new HashSet<ParameterExpression>(count);

                for (var i = 0; i < count; i++)
                {
                    var parameter = parameterList[i];

                    ValidateAsyncParameter(parameter, nameof(parameters), i);

                    types[i] = parameter.Type;

                    if (set.Contains(parameter))
                        throw DuplicateVariable(parameter, nameof(parameters), i);

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

            return CreateAsyncLambda(Expression.GetDelegateType(types), body, parameterList);
        }

        /// <summary>
        /// Creates an <see cref="AsyncLambdaCSharpExpression" /> using the specified delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="Type" /> that represents a delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="Expression" /> to set the <see cref="AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An array of <see cref="ParameterExpression" /> objects to use to populate the <see cref="AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="AsyncLambdaCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to AsyncLambda and the <see cref="AsyncLambdaCSharpExpression.Body" /> and <see cref="AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncLambdaCSharpExpression AsyncLambda(Type delegateType, Expression body, params ParameterExpression[]? parameters) => AsyncLambda(delegateType, body, (IEnumerable<ParameterExpression>?)parameters);

        /// <summary>
        /// Creates an <see cref="AsyncLambdaCSharpExpression" /> using the specified delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="Type" /> that represents a delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="Expression" /> to set the <see cref="AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterExpression" /> objects to use to populate the <see cref="AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="AsyncLambdaCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to AsyncLambda and the <see cref="AsyncLambdaCSharpExpression.Body" /> and <see cref="AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncLambdaCSharpExpression AsyncLambda(Type delegateType, Expression body, IEnumerable<ParameterExpression>? parameters)
        {
#pragma warning disable CA1062 // Validate arguments of public methods. (See bug https://github.com/dotnet/roslyn-analyzers/issues/6163)
            var parameterList = parameters.ToReadOnly();

            ValidateAsyncLambdaArgs(delegateType, ref body, parameterList);

            return CreateAsyncLambda(delegateType, body, parameterList);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        /// <summary>
        /// Creates an <see cref="AsyncCSharpExpression{TDelegate}" /> where the delegate type is known at compile time.
        /// </summary>
        /// <param name="body">An <see cref="Expression" /> to set the <see cref="AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An array of <see cref="ParameterExpression" /> objects to use to populate the <see cref="AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="AsyncLambdaCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to AsyncLambda and the <see cref="AsyncLambdaCSharpExpression.Body" /> and <see cref="AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncCSharpExpression<TDelegate> AsyncLambda<TDelegate>(Expression body, params ParameterExpression[]? parameters) => AsyncLambda<TDelegate>(body, (IEnumerable<ParameterExpression>?)parameters);

        /// <summary>
        /// Creates an <see cref="AsyncCSharpExpression{TDelegate}" /> where the delegate type is known at compile time.
        /// </summary>
        /// <param name="body">An <see cref="Expression" /> to set the <see cref="AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterExpression" /> objects to use to populate the <see cref="AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="AsyncLambdaCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to AsyncLambda and the <see cref="AsyncLambdaCSharpExpression.Body" /> and <see cref="AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncCSharpExpression<TDelegate> AsyncLambda<TDelegate>(Expression body, IEnumerable<ParameterExpression>? parameters)
        {
#pragma warning disable CA1062 // Validate arguments of public methods. (See bug https://github.com/dotnet/roslyn-analyzers/issues/6163)
            var parameterList = parameters.ToReadOnly();

            ValidateAsyncLambdaArgs(typeof(TDelegate), ref body, parameterList);

            return new AsyncCSharpExpression<TDelegate>(body, parameterList);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}" /> where the delegate type is known at compile time.
        /// </summary>
        /// <param name="isAsync">Indicates whether the resulting lambda is synchronous or asynchronous.</param>
        /// <param name="body">An <see cref="Expression" /> representing the body of the lambda.</param>
        /// <param name="parameters">An array of <see cref="ParameterExpression" /> objects that represent the parameters passed to the lambda.</param>
        /// <returns>An expression representing a lambda with the specified body and parameters.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(bool isAsync, Expression body, params ParameterExpression[]? parameters)
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

        private static void ValidateAsyncLambdaArgs([NotNull] Type? delegateType, ref Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            ContractUtils.RequiresNotNull(delegateType, nameof(delegateType));
            RequiresCanRead(body, nameof(body));

            if (!typeof(MulticastDelegate).IsAssignableFrom(delegateType) || delegateType == typeof(MulticastDelegate))
                throw LambdaTypeMustBeDerivedFromSystemDelegate(nameof(delegateType));

            var count = parameters.Count;

            var method = delegateType.GetMethod("Invoke")!; // TODO: use cache from LINQ
            var parametersCached = method.GetParametersCached();

            if (parametersCached.Length != 0)
            {
                if (parametersCached.Length != count)
                    throw IncorrectNumberOfLambdaDeclarationParameters();

                var set = new HashSet<ParameterExpression>(count);

                for (var i = 0; i < count; i++)
                {
                    var parameter = parameters[i];
                    ValidateAsyncParameter(parameter, nameof(parameters), i);

                    var parameterType = parametersCached[i].ParameterType;

                    if (!AreReferenceAssignable(parameter.Type, parameterType))
                        throw ParameterExpressionNotValidAsDelegate(parameter.Type, parameterType, nameof(parameters), i);

                    if (set.Contains(parameter))
                        throw DuplicateVariable(parameter, nameof(parameters), i);

                    set.Add(parameter);
                }
            }
            else if (count > 0)
            {
                throw IncorrectNumberOfLambdaDeclarationParameters();
            }

            if (IsTaskLikeType(method.ReturnType, out var resultType))
            {
                if (resultType != typeof(void) && !AreReferenceAssignable(resultType, body.Type) && !TryQuote(resultType, ref body))
                    throw ExpressionTypeDoesNotMatchReturn(body.Type, method.ReturnType);
            }
            else
            {
                throw Error.AsyncLambdaInvalidReturnType(method.ReturnType, nameof(delegateType));
            }
        }

        private static AsyncLambdaCSharpExpression CreateAsyncLambda(Type delegateType, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            // TODO: use cache and lambda factory functionality from LINQ
            var create = typeof(AsyncCSharpExpression<>).MakeGenericType(delegateType).GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic)!;
            return (AsyncLambdaCSharpExpression)create.Invoke(null, new object[] { body, parameters })!;
        }

        private static void ValidateAsyncParameter(ParameterExpression parameter, string paramName, int index)
        {
            RequiresCanRead(parameter, paramName, index);

            if (parameter.IsByRef)
                throw Error.AsyncLambdaCantHaveByRefParameter(parameter, paramName, index);
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
        protected internal virtual Expression VisitAsyncLambda<TDelegate>(AsyncCSharpExpression<TDelegate> node) =>
            node.Update(
                Visit(node.Body),
                VisitAndConvert(node.Parameters, nameof(VisitAsyncLambda))
            );
    }
}
