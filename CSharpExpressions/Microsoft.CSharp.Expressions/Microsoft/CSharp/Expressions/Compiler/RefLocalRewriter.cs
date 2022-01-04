// Prototyping extended expression trees for C#.
//
// bartde - January 2022

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.CSharp.Expressions.Compiler;

namespace Microsoft.CSharp.Expressions
{
    internal static class RefLocalRewriter
    {
        public static Expression Rewrite(Expression expression) => new Impl().Visit(expression);

        private sealed class Impl : ShallowVisitor
        {
            private readonly Stack<Dictionary<ParameterExpression, ReplacementInfo>> _refLocalHolders = new Stack<Dictionary<ParameterExpression, ReplacementInfo>>();

            protected internal override Expression VisitAwait(AwaitCSharpExpression node)
            {
                return node;
            }

            protected override Expression VisitBlock(BlockExpression node)
            {
                var replacements = default(Dictionary<ParameterExpression, ReplacementInfo>);

                foreach (var variable in node.Variables)
                {
                    var type = variable.Type;

                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(RefHolder<>))
                    {
                        replacements ??= new Dictionary<ParameterExpression, ReplacementInfo>();
                        replacements.Add(variable, new ReplacementInfo());
                    }
                }

                if (replacements != null)
                {
                    _refLocalHolders.Push(replacements);

                    var expressions = Visit(node.Expressions);

                    var updatedReplacements = _refLocalHolders.Pop();

                    Debug.Assert(updatedReplacements == replacements);

                    var variables = new List<ParameterExpression>();

                    foreach (var variable in node.Variables)
                    {
                        if (updatedReplacements.TryGetValue(variable, out var replacement))
                        {
                            variables.AddRange(replacement.Temps);
                        }
                        else
                        {
                            variables.Add(variable);
                        }
                    }

                    return node.Update(variables, expressions);
                }

                return base.VisitBlock(node);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                if (node.NodeType == ExpressionType.Assign && _refLocalHolders.Count > 0)
                {
                    var nearestScope = _refLocalHolders.Peek();

                    if (TryRewriteRefHolderTempCreation(node, nearestScope, out var rewrittenRefLocalCreate))
                    {
                        return rewrittenRefLocalCreate;
                    }
                    else if (TryRewriteRefHolderAssignment(node, nearestScope, out var rewrittenRefLocalAssign))
                    {
                        return rewrittenRefLocalAssign;
                    }
                }

                return base.VisitBinary(node);
            }

            private bool TryRewriteRefHolderTempCreation(BinaryExpression node, Dictionary<ParameterExpression, ReplacementInfo> nearestScope, out Expression result)
            {
                // NB: This detects the pattern introduced by CreateRefLocalAccess in Helpers.cs
                //
                //       temp = new RefHolder<T>(ref expr)
                //
                //     and builds a replacement for expr which may involve adding new temporaries.
                //
                //     The Replacement object keeps track of new temporaries and a replacement
                //     expression which is used when finding the call to temp.Invoke(...) that's
                //     used to access the ref local.

                if (node.Left is ParameterExpression p && nearestScope.TryGetValue(p, out var replacement))
                {
                    Debug.Assert(node.Right is NewExpression);
                    Debug.Assert(replacement.Replacement is null);

                    var right = (NewExpression)node.Right;

                    Debug.Assert(right.Arguments.Count == 1);

                    var temps = replacement.Temps;
                    var stmts = new List<Expression>();

                    bool shouldEvalExpressionForSideEffects = false;

                    replacement.Replacement = Rewrite(right.Arguments[0], isByRef: true, temps, stmts, ref shouldEvalExpressionForSideEffects);

                    if (shouldEvalExpressionForSideEffects)
                    {
                        stmts.Add(replacement.Replacement);
                    }

                    if (stmts.Count == 0)
                    {
                        result = Expression.Empty();
                        return true;
                    }
                    else if (stmts.Count == 1)
                    {
                        // NB: Assignments of temporaries are interior expressions, so we don't need to worry about
                        //     affecting the Type of a BlockExpression.
                        result = stmts[0];
                        return true;
                    }
                    else
                    {
                        result = Helpers.CreateVoid(stmts);
                        return true;
                    }
                }

                result = null;
                return false;
            }

            private bool TryRewriteRefHolderAssignment(BinaryExpression node, Dictionary<ParameterExpression, ReplacementInfo> nearestScope, out Expression result)
            {
                // NB: This detects another use case of RefHolder<T> used for assignment
                //
                //       temp.Value = value
                //
                //     and rewrites it to use the replacement expression that represents obj.

                if (node.Left is MemberExpression m && m.Expression is ParameterExpression p && nearestScope.TryGetValue(p, out var replacement))
                {
                    Debug.Assert(m.Member is PropertyInfo prop && prop.Name == nameof(RefHolder<int>.Value));

                    var expr = replacement.Replacement;

                    result = Expression.Assign(expr, node.Right);
                    return true;
                }

                result = null;
                return false;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                // NB: This detects the pattern introduced by CreateRefLocalAccess in Helpers.cs
                //
                //       temp.Invoke((ref obj, T value) => /* use */)
                //
                //     and rewrites it to use the replacement expression that represents obj.

                if (node.Object is ParameterExpression p && _refLocalHolders.Count > 0)
                {
                    var nearestScope = _refLocalHolders.Peek();

                    if (nearestScope.TryGetValue(p, out var replacement) && node.Method.Name == nameof(RefHolder<int>.Invoke))
                    {
                        Debug.Assert(node.Arguments.Count == 2);
                        Debug.Assert(node.Arguments[0] is LambdaExpression);

                        var action = (LambdaExpression)node.Arguments[0];
                        var argument = node.Arguments[1];

                        var expr = replacement.Replacement;

                        if (action.Body is BinaryExpression assign &&
                            assign.NodeType == ExpressionType.Assign &&
                            assign.Left == action.Parameters[0] &&
                            assign.Right == action.Parameters[1])
                        {
                            return assign.Update(expr, assign.Conversion, argument);
                        }

                        return Expression.Invoke(action, expr, argument);
                    }
                }
                
                return base.VisitMethodCall(node);
            }

            private Expression Rewrite(Expression expression, bool isByRef, List<ParameterExpression> temps, List<Expression> stmts, ref bool shouldEvalExpressionForSideEffects)
            {
                if (expression is MemberExpression m && m.Member is FieldInfo f)
                {
                    if (f.IsStatic)
                    {
                        if (isByRef || f.IsInitOnly)
                        {
                            return expression;
                        }
                    }
                    else
                    {
                        if (isByRef)
                        {
                            var obj = m.Expression;

                            var isValueType = obj.Type.IsValueType;

                            var newObj = Rewrite(obj, isValueType && isByRef, temps, stmts, ref shouldEvalExpressionForSideEffects);

                            if (!isValueType)
                            {
                                shouldEvalExpressionForSideEffects = true; // for null check
                            }

                            return m.Update(newObj);
                        }
                    }
                }
                else if (expression is ParameterExpression)
                {
                    // NB: We don't want to make a copy and assume that any variable that crosses await sites will get
                    //     hoisted anyway, so it's safe to return it as-is.
                    if (expression.Type.IsValueType)
                    {
                        return expression;
                    }
                }
                else
                {
                    if (expression.NodeType == ExpressionType.ArrayIndex)
                    {
                        var arrayIndex = (BinaryExpression)expression;
                        expression = Expression.ArrayAccess(arrayIndex.Left, arrayIndex.Right);
                    }

                    if (expression is IndexExpression i && i.Indexer == null)
                    {
                        var newObj = Rewrite(i.Object, isByRef: false, temps, stmts, ref shouldEvalExpressionForSideEffects);

                        var newIndexes = new List<Expression>(i.Arguments.Count);

                        foreach (var index in i.Arguments)
                        {
                            var newIndex = Rewrite(index, isByRef: false, temps, stmts, ref shouldEvalExpressionForSideEffects);

                            newIndexes.Add(newIndex);
                        }

                        shouldEvalExpressionForSideEffects = true; // for bounds check

                        return i.Update(newObj, newIndexes);
                    }
                }
                
                if (Helpers.IsPure(expression))
                {
                    return expression;
                }

                var temp = Expression.Parameter(expression.Type, "t");
                
                temps.Add(temp);
                stmts.Add(Expression.Assign(temp, expression));
                
                return temp;
            }

            private sealed class ReplacementInfo
            {
                public List<ParameterExpression> Temps { get; } = new List<ParameterExpression>();
                public Expression Replacement { get; set; }
            }
        }
    }
}
