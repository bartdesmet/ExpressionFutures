// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Rewriter for await expressions by desugaring those into the await pattern.
    /// </summary>
    /// <remarks>
    /// Rewriting await expressions entails the following steps:
    /// - Desugaring await expressions using the await pattern of GetAwaiter, IsCompleted, and GetResult
    /// - Dispatching to the builder's AwaitOnCompleted method upon non-synchronous completion of the awaitable
    /// - Introducing a state machine state and a label target for the continuation of the await operation
    /// - Hoisting of local variables in blocks that contain asynchronous operations
    /// - Emission of jump tables in Try expressions in order to reenter upon resumption
    /// - Building a jump table for the caller to embed in the top-level rewritten lambda body
    /// </remarks>
    internal class AwaitRewriter : ShallowVisitor
    {
        private readonly Func<Type, string, ParameterExpression> _variableFactory;
        private readonly ParameterExpression _localStateVariable;
        private readonly ParameterExpression _stateVariable;
        private readonly Func<Expression, Expression> _onCompletedFactory;
        private readonly LabelTarget _exit;
        private readonly Stack<StrongBox<bool>> _awaitInBlock = new Stack<StrongBox<bool>>();
        private readonly Stack<IList<SwitchCase>> _jumpTables = new Stack<IList<SwitchCase>>();
        private int _labelIndex;

        public AwaitRewriter(ParameterExpression localStateVariable, ParameterExpression stateVariable, Func<Type, string, ParameterExpression> variableFactory, Func<Expression, Expression> onCompletedFactory, LabelTarget exit)
        {
            _variableFactory = variableFactory;
            _localStateVariable = localStateVariable;
            _stateVariable = stateVariable;
            _onCompletedFactory = onCompletedFactory;
            _exit = exit;
            _jumpTables.Push(new List<SwitchCase>());
        }

        public HashSet<ParameterExpression> HoistedVariables { get; } = new HashSet<ParameterExpression>();

        public IList<SwitchCase> ResumeList
        {
            get
            {
                return _jumpTables.Peek();
            }
        }

        // NB: We don't have to deal with CatchBlock nodes which also introduce scope because the AwaitRewriter
        //     runs after lowering Try blocks. When lowering a CatchBlock containing an Await node, its exception
        //     variable gets hoisted to the enclosing Block. If there's no Await, there's nothing to worry about.

        // NB: We don't have to deal with Using, For, and ForEach blocks because the AwaitRewriter runs after the
        //     reduction step.

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
        protected internal override Expression VisitAwait(AwaitCSharpExpression node)
        {
            const int ExprCount = 1 /* GetAwaiter */ + 1 /* IsCompleted */ + 1 /* Label */ + 1 /* GetResult */ + 2 /* Cleanup */ + 1 /* Result */;

            if (_awaitInBlock.Count > 0)
            {
                _awaitInBlock.Peek().Value = true;
            }

            var getAwaiter = node.ReduceGetAwaiter();
            var awaiterVar = _variableFactory(getAwaiter.Type, "__awaiter");
            var isCompleted = node.ReduceIsCompleted(awaiterVar);
            var getResult = node.ReduceGetResult(awaiterVar);

            var vars = Array.Empty<ParameterExpression>();
            var exprs = new Expression[ExprCount];

            if (getResult.Type != typeof(void))
            {
                var resultVar = Expression.Parameter(getResult.Type, "__result");
                getResult = Expression.Assign(resultVar, getResult);
                vars = new[] { resultVar };
                exprs[exprs.Length - 1] = resultVar;
            }
            else
            {
                exprs[exprs.Length - 1] = Expression.Empty();
            }

            var continueLabel = GetLabel();

            var i = 0;

            exprs[i++] =
                Expression.Assign(awaiterVar, getAwaiter);
            exprs[i++] =
                Expression.IfThen(Expression.Not(isCompleted),
                    Expression.Block(
                        UpdateState(continueLabel.Index),
                        _onCompletedFactory(awaiterVar),
                        Expression.Return(_exit)
                    )
                );
            exprs[i++] =
                Expression.Label(continueLabel.Label);
            exprs[i++] =
                getResult;
            exprs[i++] =
                Expression.Assign(awaiterVar, Expression.Default(awaiterVar.Type));
            exprs[i++] =
                UpdateState(-1);

            var res = Expression.Block(vars, exprs);
            return res;
        }

        protected override Expression VisitTry(TryExpression node)
        {
            _jumpTables.Push(new List<SwitchCase>());

            var res = base.VisitTry(node);

            var table = _jumpTables.Pop();

            if (table.Count > 0)
            {
                var dispatch = Expression.Switch(_localStateVariable, table.ToArray());

                var originalTry = (TryExpression)res;
                var newTry = originalTry.Update(
                    Expression.Block(
                        dispatch,
                        originalTry.Body
                    ),
                    originalTry.Handlers,
                    RewriteHandler(originalTry.Finally),
                    RewriteHandler(originalTry.Fault)
                );

                var beforeTry = Expression.Label("__enterTry");
                var enterTry = Expression.Goto(beforeTry);

                var previousTable = _jumpTables.Peek();
                foreach (var jump in table)
                {
                    var index = (int)((ConstantExpression)jump.TestValues.Single()).Value; // TODO: keep different data structure to avoid casts?
                    previousTable.Add(Expression.SwitchCase(enterTry, Helpers.CreateConstantInt32(index)));
                }

                res = Expression.Block(
                    Expression.Label(beforeTry),
                    newTry
                );
            }

            return res;
        }

        private Expression RewriteHandler(Expression original)
        {
            var res = original;

            if (original != null)
            {
                res =
                    Expression.IfThen(
                        Expression.LessThan(_localStateVariable, Helpers.CreateConstantInt32(0)),
                        original
                    );
            }

            return res;
        }

        private StateMachineState GetLabel()
        {
            var index = _labelIndex++;
            var label = Expression.Label("__state" + index);

            var jump = Expression.Block(
                UpdateState(-1),
                Expression.Goto(label)
            );

            ResumeList.Add(Expression.SwitchCase(jump, Helpers.CreateConstantInt32(index)));

            return new StateMachineState
            {
                Label = label,
                Index = index
            };
        }

        private Expression UpdateState(int value)
        {
            return Expression.Assign(_localStateVariable, Expression.Assign(_stateVariable, Helpers.CreateConstantInt32(value)));
        }

        struct StateMachineState
        {
            public LabelTarget Label;
            public int Index;
        }
    }
}
