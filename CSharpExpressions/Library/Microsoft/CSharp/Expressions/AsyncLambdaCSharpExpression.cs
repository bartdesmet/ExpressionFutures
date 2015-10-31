// Prototyping extended expression trees for C#.
//
// bartde - October 2015

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
    public abstract class AsyncLambdaCSharpExpression : CSharpExpression
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
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression" /> that represents the body of the lambda expression.</returns>
        public Expression Body { get; }

        /// <summary>
        /// Gets the parameters of the lambda expression.
        /// </summary>
		/// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects that represent the parameters of the lambda expression.</returns>
        public ReadOnlyCollection<ParameterExpression> Parameters { get; }

        /// <summary>
        /// Produces a delegate that represents the asynchronous lambda expression.
        /// </summary>
        /// <returns>A <see cref="T:System.Delegate" /> that contains the compiled version of the asynchronous lambda expression.</returns>
        public Delegate Compile()
        {
            return ((LambdaExpression)Reduce()).Compile();
        }
    }

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
        /// <param name="body">The <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property of the result.</param>
        /// <param name="parameters">The <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> property of the result. </param>
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
        /// <returns>A delegate of type <paramref name="TDelegate" /> that represents the compiled asynchronous lambda expression described by the <see cref="T:Microsoft.CSharp.Expressions.AsyncCSharpExpression`1" />.</returns>
        public new TDelegate Compile()
        {
            return ((Expression<TDelegate>)Reduce()).Compile();
        }

        /// <summary>
        /// Reduces the call expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            const int ExprCount = 1 /* new builder */ + 1 /* new state machine */ + 1 /* start state machine */;

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

            var builderVar = Expression.Parameter(builderType);
            var stateMachineVar = Expression.Parameter(typeof(RuntimeAsyncStateMachine));

            var builderCreateMethod = builderType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            var createBuilder = Expression.Assign(builderVar, Expression.Call(builderCreateMethod));
            exprs[i++] = createBuilder;

            var body = RewriteBody(builderVar);

            var stateMachineCtor = stateMachineVar.Type.GetConstructor(new[] { typeof(Action) });
            var createStateMachine = Expression.Assign(stateMachineVar, Expression.New(stateMachineCtor, body));
            exprs[i++] = createStateMachine;

            var startMethod = builderType.GetMethod("Start", BindingFlags.Public | BindingFlags.Instance);
            exprs[i++] = Expression.Call(builderVar, startMethod.MakeGenericMethod(typeof(RuntimeAsyncStateMachine)), stateMachineVar);

            if (returnType != typeof(void))
            {
                exprs[i] = Expression.Property(builderVar, "Task");
            }

            var rewritten = Expression.Block(new[] { builderVar, stateMachineVar }, exprs);

            var res = Expression.Lambda<TDelegate>(rewritten, Parameters);
            return res;
        }

        private Expression RewriteBody(ParameterExpression builderVar)
        {
            // TODO: split body into blocks based on await boundaries, spill stack, and generate state machine

            const int ExprCount = 1 /* TryCatch */ + 1 /* Label */;

            var vars = Array.Empty<ParameterExpression>();
            var exprs = default(Expression[]);

            var result = default(ParameterExpression);
            var ex = Expression.Parameter(typeof(Exception));

            var exit = Expression.Label();

            var hoistedVars = new Dictionary<Type, ParameterExpression>();

            var getVariable = new Func<Type, ParameterExpression>(t =>
            {
                var p = default(ParameterExpression);
                if (!hoistedVars.TryGetValue(t, out p))
                {
                    p = Expression.Parameter(t);
                    hoistedVars.Add(t, p);
                }

                return p;
            });

            // TODO: parameterize await rewriter with proper allocators
            //       - labels end up in the switch table using async state machine state numbers
            var rewrittenBody = new AwaitRewriter(Expression.Label, getVariable).Visit(Body);

            var newBody = rewrittenBody;
            if (Body.Type != typeof(void) && builderVar.Type.IsGenericType /* if not ATMB<T>, no result assignment needed */)
            {
                result = Expression.Parameter(Body.Type);
                newBody = Expression.Assign(result, rewrittenBody);
                vars = new[] { result };
                exprs = new Expression[ExprCount + 1];
            }
            else
            {
                exprs = new Expression[ExprCount];
            }

            var i = 0;

            exprs[i++] =
                Expression.TryCatch(
                    Expression.Block(
                        newBody,
                        Expression.Empty()
                    ),
                    Expression.Catch(ex,
                        Expression.Block(
                            Expression.Call(builderVar, builderVar.Type.GetMethod("SetException"), ex),
                            Expression.Return(exit)
                        )
                    )
                );

            if (result != null)
            {
                exprs[i++] = Expression.Call(builderVar, builderVar.Type.GetMethod("SetResult"), result);
            }

            exprs[i++] = Expression.Label(exit);

            var body = Expression.Block(vars.Concat(hoistedVars.Values), exprs);
            var res = Expression.Lambda<Action>(body);
            return res;
        }

        class AwaitRewriter : CSharpExpressionVisitor
        {
            private readonly Func<LabelTarget> _labelFactory;
            private readonly Func<Type, ParameterExpression> _variableFactory;

            public AwaitRewriter(Func<LabelTarget> labelFactory, Func<Type, ParameterExpression> variableFactory)
            {
                _labelFactory = labelFactory;
                _variableFactory = variableFactory;
            }

            protected internal override Expression VisitAwait(AwaitCSharpExpression node)
            {
                var getAwaiter = node.ReduceGetAwaiter();
                var awaiterVar = _variableFactory(getAwaiter.Type);
                var isCompleted = node.ReduceIsCompleted(awaiterVar);
                var getResult = node.ReduceGetResult(awaiterVar);

                var continueLabel = _labelFactory();

                var res =
                    Expression.Block(
                        Expression.Assign(awaiterVar, getAwaiter),
                        Expression.IfThen(Expression.Not(isCompleted),
                            Expression.Throw(Expression.Constant(new NotImplementedException())) // TODO: AwaitOnCompleted call
                        ),
                        Expression.Label(continueLabel),
                        getResult
                    );

                return res;

                // TODO:
                //
                // - get new state machine index + label
                // - set state to -1
                // - emit GetAwaiter and store in hoisted variable; try to reuse variables of same type
                // - check IsCompleted
                //   - if not completed:
                //     - set state
                //     - spill stack (separate pass; can reuse LINQ's stack spiller?)
                //     - call [Unsafe]AwaitOnCompleted
                //   - else
                //     - fall through
            }

            protected internal override Expression VisitAsyncLambda<T>(AsyncCSharpExpression<T> node)
            {
                // NB: Keep hands off nested lambdas
                return node;
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                // NB: Keep hands off nested lambdas
                return node;
            }
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An array of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.NodeType" /> property equal to AsyncLambda and the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> and <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncLambdaCSharpExpression AsyncLambda(Expression body, params ParameterExpression[] parameters)
        {
            return AsyncLambda(body, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.NodeType" /> property equal to AsyncLambda and the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> and <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static AsyncLambdaCSharpExpression AsyncLambda(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            ContractUtils.RequiresNotNull(body, nameof(body));

            var parameterList = parameters.ToReadOnly<ParameterExpression>();

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
        /// Creates an <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> using the specified delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="T:System.Type" /> that represents a delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An array of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.NodeType" /> property equal to AsyncLambda and the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> and <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncLambdaCSharpExpression AsyncLambda(Type delegateType, Expression body, params ParameterExpression[] parameters)
        {
            return AsyncLambda(delegateType, body, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> using the specified delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="T:System.Type" /> that represents a delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.NodeType" /> property equal to AsyncLambda and the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> and <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncLambdaCSharpExpression AsyncLambda(Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
        {
            var parameterList = parameters.ToReadOnly();

            ValidAsyncLambdaArgs(delegateType, ref body, parameterList);

            return CreateAsyncLambda(delegateType, body, parameterList);
        }

        /// <summary>
        /// Creates an <see cref="T:Microsoft.CSharp.Expressions.AsyncCSharpExpression`1" /> where the delegate type is known at compile time.
        /// </summary>
        /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An array of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="T:AsyncLambdaCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.NodeType" /> property equal to AsyncLambda and the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> and <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncCSharpExpression<TDelegate> AsyncLambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
        {
            return AsyncLambda<TDelegate>(body, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="T:Microsoft.CSharp.Expressions.AsyncCSharpExpression`1" /> where the delegate type is known at compile time.
        /// </summary>
        /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="T:AsyncLambdaCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.NodeType" /> property equal to AsyncLambda and the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> and <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncCSharpExpression<TDelegate> AsyncLambda<TDelegate>(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            var parameterList = parameters.ToReadOnly();

            ValidAsyncLambdaArgs(typeof(TDelegate), ref body, parameterList);

            return new AsyncCSharpExpression<TDelegate>(body, parameterList);
        }

        private static void ValidAsyncLambdaArgs(Type delegateType, ref Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            ContractUtils.RequiresNotNull(delegateType, nameof(delegateType));
            RequiresCanRead(body, "body");

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
            else if (returnType != typeof(void) && returnType == typeof(Task))
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
