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

            var builderVar = Expression.Parameter(builderType);
            var stateMachineVar = Expression.Parameter(typeof(RuntimeAsyncStateMachine));
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

            var res = Expression.Lambda<TDelegate>(rewritten, Parameters);
            return res;
        }

        private Expression RewriteBody(ParameterExpression stateVar, ParameterExpression builderVar, ParameterExpression stateMachineVar, out IEnumerable<ParameterExpression> variables)
        {
            // TODO: Enter/leave sequences for Try blocks
            //       Timing for finally handlers; prevent premature execution
            //       C# 6.0 features - await in catch and finally
            //       Reject await in filters

            const int ExprCount = 1 /* TryCatch */ + 2 /* state = -2; SetResult */ + 1 /* Label */;

            var locals = Array.Empty<ParameterExpression>();
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

            var labelIndex = 0;
            var resumeList = new List<SwitchCase>();

            var getLabel = new Func<StateMachineState>(() =>
            {
                var label = Expression.Label();
                var index = labelIndex++;

                var jump = Expression.Block(
                    Expression.Assign(stateVar, Helpers.CreateConstantInt32(-1)),
                    Expression.Goto(label)
                );
                resumeList.Add(Expression.SwitchCase(jump, Helpers.CreateConstantInt32(index)));

                return new StateMachineState
                {
                    Label = label,
                    Index = index
                };
            });

            var awaitOnCompletedMethod = builderVar.Type.GetMethod("AwaitOnCompleted", BindingFlags.Public | BindingFlags.Instance);
            var awaitOnCompletedArgs = new Type[] { default(Type), typeof(RuntimeAsyncStateMachine) };

            var onCompletedFactory = new Func<Expression, Expression>(awaiter =>
            {
                awaitOnCompletedArgs[0] = awaiter.Type;
                var awaitOnCompletedMethodClosed = awaitOnCompletedMethod.MakeGenericMethod(awaitOnCompletedArgs);
                return Expression.Call(builderVar, awaitOnCompletedMethodClosed, awaiter, stateMachineVar);
            });

            var bright = new ShadowEliminator().Visit(Body);
            var spilled = Spiller.Spill(bright);

            var awaitRewriter = new AwaitRewriter(stateVar, getLabel, getVariable, onCompletedFactory, exit);
            var rewrittenBody = awaitRewriter.Visit(spilled);

            var newBody = rewrittenBody;
            if (Body.Type != typeof(void) && builderVar.Type.IsGenericType /* if not ATMB<T>, no result assignment needed */)
            {
                result = Expression.Parameter(Body.Type);
                newBody = Expression.Assign(result, rewrittenBody);
                locals = new[] { result };
            }
            else
            {
                locals = Array.Empty<ParameterExpression>();
            }

            exprs = new Expression[ExprCount];

            if (result != null)
            {
                newBody = AssignmentPercolator.Instance.Visit(newBody);
            }

            var i = 0;

            var jumpTable =
                resumeList.Count > 0 ? Expression.Switch(stateVar, resumeList.ToArray()) : (Expression)Expression.Empty();

            exprs[i++] =
                Expression.TryCatch(
                    Expression.Block(
                        jumpTable,
                        newBody,
                        Expression.Empty()
                    ),
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

        class AssignmentPercolator : CSharpExpressionVisitor
        {
            public static readonly ExpressionVisitor Instance = new AssignmentPercolator();

            protected override Expression VisitBinary(BinaryExpression node)
            {
                var res = base.VisitBinary(node);

                if (res.NodeType == ExpressionType.Assign)
                {
                    var b = (BinaryExpression)res;
                    if (b.Right.NodeType == ExpressionType.Block && b.Conversion == null)
                    {
                        res = Percolate(b.Left, b.Right);
                    }
                }

                return res;
            }

            private static Expression Percolate(Expression result, Expression expr)
            {
                if (expr.NodeType == ExpressionType.Block)
                {
                    var block = (BlockExpression)expr;
                    var n = block.Expressions.Count;
                    var res = Percolate(result, block.Expressions[n - 1]);
                    return block.Update(block.Variables, block.Expressions.Take(n - 1).Concat(new[] { res }));
                }
                else
                {
                    return Expression.Assign(result, expr);
                }
            }
        }

        struct StateMachineState
        {
            public LabelTarget Label;
            public int Index;
        }

        class AwaitRewriter : CSharpExpressionVisitor
        {
            private readonly Func<StateMachineState> _labelFactory;
            private readonly Func<Type, ParameterExpression> _variableFactory;
            private readonly ParameterExpression _stateVariable;
            private readonly Func<Expression, Expression> _onCompletedFactory;
            private readonly LabelTarget _exit;
            private readonly Stack<StrongBox<bool>> _awaitInBlock = new Stack<StrongBox<bool>>();

            public AwaitRewriter(ParameterExpression stateVariable, Func<StateMachineState> labelFactory, Func<Type, ParameterExpression> variableFactory, Func<Expression, Expression> onCompletedFactory, LabelTarget exit)
            {
                _labelFactory = labelFactory;
                _variableFactory = variableFactory;
                _stateVariable = stateVariable;
                _onCompletedFactory = onCompletedFactory;
                _exit = exit;
            }

            public HashSet<ParameterExpression> HoistedVariables { get; } = new HashSet<ParameterExpression>();

            // TODO: CatchBlock also introduces scope; [Async]Lambda hoists by itself.
            // TODO: Deal with Using blocks as well.

            protected override Expression VisitBlock(BlockExpression node)
            {
                _awaitInBlock.Push(new StrongBox<bool>());

                var res = (BlockExpression)base.VisitBlock(node);

                var b = _awaitInBlock.Pop();
                if (b.Value)
                {
                    if (node.Variables.Count > 0)
                    {
                        foreach (var p in node.Variables)
                        {
                            // NB: We eliminated shadowed variables higher up. If we'd hoist shadowed variables up as-is,
                            //     their scoped meaning would get lost. To solve this, we detect shadowing first and
                            //     rewrite the expression to get rid of it.
                            if (!HoistedVariables.Add(p))
                            {
                                throw ContractUtils.Unreachable;
                            }
                        }

                        res = res.Update(Array.Empty<ParameterExpression>(), res.Expressions);
                    }

                    if (_awaitInBlock.Count > 0)
                    {
                        _awaitInBlock.Peek().Value = true;
                    }
                }

                return res;
            }

            protected internal override Expression VisitAwait(AwaitCSharpExpression node)
            {
                if (_awaitInBlock.Count > 0)
                {
                    _awaitInBlock.Peek().Value = true;
                }

                var getAwaiter = node.ReduceGetAwaiter();
                var awaiterVar = _variableFactory(getAwaiter.Type);
                var isCompleted = node.ReduceIsCompleted(awaiterVar);
                var getResult = node.ReduceGetResult(awaiterVar);

                var continueLabel = _labelFactory();
                
                var res =
                    Expression.Block(
                        Expression.Assign(awaiterVar, getAwaiter),
                        Expression.IfThen(Expression.Not(isCompleted),
                            Expression.Block(
                                Expression.Assign(_stateVariable, Helpers.CreateConstantInt32(continueLabel.Index)),
                                _onCompletedFactory(awaiterVar),
                                Expression.Return(_exit)
                            )
                        ),
                        Expression.Label(continueLabel.Label),
                        getResult
                    );

                return res;
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

            ValidateAsyncLambdaArgs(delegateType, ref body, parameterList);

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

            ValidateAsyncLambdaArgs(typeof(TDelegate), ref body, parameterList);

            return new AsyncCSharpExpression<TDelegate>(body, parameterList);
        }

        private static void ValidateAsyncLambdaArgs(Type delegateType, ref Expression body, ReadOnlyCollection<ParameterExpression> parameters)
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
