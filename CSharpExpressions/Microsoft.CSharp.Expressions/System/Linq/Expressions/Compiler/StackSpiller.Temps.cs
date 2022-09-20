// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// NB: This is a port of the LINQ stack spiller but with a number of changes. See Spiller.cs for some design notes.
//     Changes are clearly marked with #if LINQ conditions in order to make it possible to reuse across codebases.

using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using static System.Linq.Expressions.ExpressionExtensions;
#if !LINQ
using static Microsoft.CSharp.Expressions.Helpers;
#endif

namespace System.Linq.Expressions.Compiler
{
    internal partial class StackSpiller
    {
        private class TempMaker
        {
            /// <summary>
            /// Current temporary variable
            /// </summary>
            private int _temp;

            /// <summary>
            /// List of free temporary variables. These can be recycled for new temps.
            /// </summary>
            private List<ParameterExpression>? _freeTemps;

            /// <summary>
            /// Stack of currently active temporary variables.
            /// </summary>
            private Stack<ParameterExpression>? _usedTemps;

            /// <summary>
            /// List of all temps created by stackspiller for this rule/lambda
            /// </summary>
            private readonly List<ParameterExpression> _temps = new();

            internal List<ParameterExpression> Temps
            {
                get { return _temps; }
            }

            internal ParameterExpression Temp(Type type)
            {
                ParameterExpression temp;
                if (_freeTemps != null)
                {
                    // Recycle from the free-list if possible.
                    for (int i = _freeTemps.Count - 1; i >= 0; i--)
                    {
                        temp = _freeTemps[i];
                        if (temp.Type == type)
                        {
                            _freeTemps.RemoveAt(i);
                            return UseTemp(temp);
                        }
                    }
                }
                // Not on the free-list, create a brand new one.
                temp = Expression.Variable(type, "$temp$" + _temp++);
                _temps.Add(temp);
                return UseTemp(temp);
            }

            private ParameterExpression UseTemp(ParameterExpression temp)
            {
                Debug.Assert(_freeTemps == null || !_freeTemps.Contains(temp));
                Debug.Assert(_usedTemps == null || !_usedTemps.Contains(temp));

                _usedTemps ??= new Stack<ParameterExpression>();
                _usedTemps.Push(temp);
                return temp;
            }

            private void FreeTemp(ParameterExpression temp)
            {
                Debug.Assert(_freeTemps == null || !_freeTemps.Contains(temp));
                _freeTemps ??= new List<ParameterExpression>();
                _freeTemps.Add(temp);
            }

            internal int Mark()
            {
                return _usedTemps != null ? _usedTemps.Count : 0;
            }

            // Free temporaries created since the last marking. 
            // This is a performance optimization to lower the overall number of tempories needed.
            internal void Free(int mark)
            {
                // (_usedTemps != null) ==> (mark <= _usedTemps.Count)
                Debug.Assert(_usedTemps == null || mark <= _usedTemps.Count);
                // (_usedTemps == null) ==> (mark == 0)
                Debug.Assert(mark == 0 || _usedTemps != null);

                if (_usedTemps != null)
                {
                    while (mark < _usedTemps.Count)
                    {
                        FreeTemp(_usedTemps.Pop());
                    }
                }
            }

            [Conditional("DEBUG")]
            internal void VerifyTemps()
            {
                Debug.Assert(_usedTemps == null || _usedTemps.Count == 0);
            }
        }


        /// <summary>
        /// Rewrites child expressions, spilling them into temps if needed. The
        /// stack starts in the inital state, and after the first subexpression
        /// is added it is change to non-empty. This behavior can be overridden
        /// by setting the stack manually between adds.
        /// 
        /// When all children have been added, the caller should rewrite the 
        /// node if Rewrite is true. Then, it should call Finish with either
        /// the original expression or the rewritten expression. Finish will call
        /// Expression.Comma if necessary and return a new Result.
        /// </summary>
        private class ChildRewriter
        {
            private readonly StackSpiller _self;
            private readonly Expression?[] _expressions;
            private int _expressionsCount;
            private List<Expression>? _comma;
            private RewriteAction _action;
            private Stack _stack;
            private bool _done;

            internal ChildRewriter(StackSpiller self, Stack stack, int count)
            {
                _self = self;
                _stack = stack;
                _expressions = new Expression?[count];
            }

            internal void Add(Expression? node)
            {
                Debug.Assert(!_done);

                if (node == null)
                {
                    _expressions[_expressionsCount++] = null;
                    return;
                }

                Result exp = _self.RewriteExpression(node, _stack);
                _action |= exp.Action;
                _stack = Stack.NonEmpty;

                // track items in case we need to copy or spill stack
                _expressions[_expressionsCount++] = exp.Node;
            }

            internal void Add(IList<Expression> expressions)
            {
                for (int i = 0, count = expressions.Count; i < count; i++)
                {
                    Add(expressions[i]);
                }
            }

            internal void AddArguments(IArgumentProvider expressions)
            {
                for (int i = 0, count = expressions.ArgumentCount; i < count; i++)
                {
                    Add(expressions.GetArgument(i));
                }
            }

            private void EnsureDone()
            {
                // done adding arguments, build the comma if necessary
                if (!_done)
                {
                    _done = true;

                    if (_action == RewriteAction.SpillStack)
                    {
                        Expression?[] clone = _expressions;
                        int count = clone.Length;
                        List<Expression> comma = new(count + 1);
                        for (int i = 0; i < count; i++)
                        {
                            if (clone[i] != null)
                            {
#if !LINQ
                                if (_byRef != null && _byRef[i])
                                {
                                    clone[i] = SpillByRef(comma, clone[i]!);
                                }
                                else
                                {
#endif
                                    clone[i] = _self.ToTemp(clone[i]!, out Expression temp);
                                    comma.Add(temp);
#if !LINQ
                                }
#endif
                            }
                        }
                        comma.Capacity = comma.Count + 1;
                        _comma = comma;
                    }
                }
            }

#if !LINQ
            private Expression SpillByRef(List<Expression> comma, Expression expression)
            {
                List<Expression>? writebacks = null;
                RewriteByRefArgument(null, ref expression, (type, name) => _self.MakeTemp(type), comma, ref writebacks);

                if (writebacks != null)
                {
                    // TODO: Make a call on supporting writebacks or not and either remove them altogether
                    //       or provide a good error message here.
                    throw new NotSupportedException();
                }

                return expression;
            }

            // NB: This means of tracking by-ref positions may seem adhoc because it really is an adhoc
            //     bolt-on approach in order to mimize the number of changes to the LINQ stack spiller.

            private bool[]? _byRef;

            internal void MarkByRef(int index)
            {
                // NB: could use a bit vector if we want to optimize
                _byRef ??= new bool[_expressions.Length];

                _byRef[index] = true;
            }
#endif

            internal bool Rewrite
            {
                get { return _action != RewriteAction.None; }
            }

            internal RewriteAction Action
            {
                get { return _action; }
            }

            internal Result Finish(Expression expr)
            {
                EnsureDone();

                if (_action == RewriteAction.SpillStack)
                {
                    Debug.Assert(_comma != null && _comma.Capacity == _comma.Count + 1);
                    _comma.Add(expr);
                    expr = MakeBlock(_comma);
                }

                return new Result(_action, expr);
            }

            internal Expression this[int index]
            {
                get
                {
                    EnsureDone();
                    if (index < 0)
                    {
                        index += _expressions.Length;
                    }
                    return _expressions[index]!;
                }
            }

            internal Expression[] this[int first, int last]
            {
                get
                {
                    EnsureDone();
                    if (last < 0)
                    {
                        last += _expressions.Length;
                    }
                    int count = last - first + 1;
                    ContractUtils.RequiresArrayRange(_expressions, first, count, "first", "last");

                    if (count == _expressions.Length)
                    {
                        Debug.Assert(first == 0);
                        // if the entire array is requested just return it so we don't make a new array
                        return _expressions!;
                    }

                    Expression[] clone = new Expression[count];
                    Array.Copy(_expressions, first, clone, 0, count);
                    return clone;
                }
            }
        }


        private ParameterExpression MakeTemp(Type type)
        {
            return _tm.Temp(type);
        }

        private int Mark()
        {
            return _tm.Mark();
        }

        private void Free(int mark)
        {
            _tm.Free(mark);
        }

        [Conditional("DEBUG")]
        private void VerifyTemps()
        {
            _tm.VerifyTemps();
        }

        /// <summary>
        /// Will perform:
        ///     save: temp = expression
        ///     return value: temp
        /// </summary>
        private ParameterExpression ToTemp(Expression expression, out Expression save)
        {
            ParameterExpression temp = MakeTemp(expression.Type);
            save = Expression.Assign(temp, expression);
            return temp;
        }

        /// <summary>
        /// Creates a special block that is marked as not allowing jumps in.
        /// This should not be used for rewriting BlockExpression itself, or
        /// anything else that supports jumping.
        /// </summary>
        private static Expression MakeBlock(params Expression[] expressions)
        {
            return MakeBlock((IList<Expression>)expressions);
        }

        /// <summary>
        /// Creates a special block that is marked as not allowing jumps in.
        /// This should not be used for rewriting BlockExpression itself, or
        /// anything else that supports jumping.
        /// </summary>
        private static Expression MakeBlock(IList<Expression> expressions)
        {
            return CreateSpilledExpressionBlock(expressions);
        }
    }

#if LINQ
    /// <summary>
    /// A special subtype of BlockExpression that indicates to the compiler
    /// that this block is a spilled expression and should not allow jumps in.
    /// </summary>
    internal sealed class SpilledExpressionBlock : BlockN
    {
        internal SpilledExpressionBlock(IList<Expression> expressions)
            : base(expressions)
        {
        }
        internal override BlockExpression Rewrite(ReadOnlyCollection<ParameterExpression> variables, Expression[] args)
        {
            throw ContractUtils.Unreachable;
        }
    }
#endif
}
