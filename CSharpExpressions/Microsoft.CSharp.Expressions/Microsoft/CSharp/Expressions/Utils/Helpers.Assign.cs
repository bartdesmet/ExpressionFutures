// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    internal static partial class Helpers
    {
        public static Expression ReduceAssign(Expression lhs, Func<Expression, Expression> assign)
        {
            // NB: This is a simplified form of ReduceAssignment without support for conversions or a differentation of prefix/postfix assignments.
            //     It also assumes the lhs is not a mutable value type that needs by ref access treatment.

            var temps = new List<ParameterExpression>();
            var stmts = new List<Expression>();

            var expr = ReduceAssign(lhs, temps, stmts);

            stmts.Add(assign(expr));

            return Comma(temps, stmts);
        }

        public static Expression ReduceAssign(Expression lhs, List<ParameterExpression> temps, List<Expression> stmts, bool supportByRef = false)
        {
            return lhs.NodeType switch
            {
                ExpressionType.MemberAccess => ReduceAssignMember(),
                ExpressionType.Index => ReduceAssignIndex(),
                ExpressionType.Parameter => lhs,
                _ => lhs switch
                {
                    IndexCSharpExpression index => index.ReduceAssign(temps, stmts),
                    ArrayAccessCSharpExpression arrayAccess => arrayAccess.ReduceAssign(temps, stmts),
                    IndexerAccessCSharpExpression indexerAccess => indexerAccess.ReduceAssign(temps, stmts),
                    _ => throw ContractUtils.Unreachable,
                },
            };

            Expression ReduceAssignMember()
            {
                var member = (MemberExpression)lhs;

                if (member.Expression == null)
                {
                    return member;
                }
                else
                {
                    if (NeedByRefAssign(member.Expression))
                    {
                        if (supportByRef)
                        {
                            if (SupportsRefLocals)
                            {
                                return member.Member.MemberType == MemberTypes.Field
                                    ? CreateRefLocalAccess(member, objByRef => objByRef, temps, stmts)
                                    : CreateRefLocalAccess(member.Expression, objByRef => member.Update(objByRef), temps, stmts);
                            }
                            else
                            {
                                // NB: This makes cases like '(t.x, t.y) = expr' work, where t is a tuple type.
                                return member;
                            }
                        }

                        throw ContractUtils.Unreachable;
                    }

                    var lhsTemp = Expression.Parameter(member.Expression.Type, "__lhs");
                    var lhsAssign = Expression.Assign(lhsTemp, member.Expression);

                    temps.Add(lhsTemp);
                    stmts.Add(lhsAssign);

                    return member.Update(lhsTemp);
                }
            }

            Expression ReduceAssignIndex()
            {
                var index = (IndexExpression)lhs;

                if (NeedByRefAssign(index.Object))
                {
                    throw ContractUtils.Unreachable;
                }

                var n = index.Arguments.Count;
                var args = new Expression[n];

                var obj = index.Object;

                if (obj != null)
                {
                    var temp = Expression.Parameter(obj.Type, "__object");
                    temps.Add(temp);
                    stmts.Add(Expression.Assign(temp, obj));

                    obj = temp;
                }

                for (var j = 0; j < n; j++)
                {
                    var arg = index.Arguments[j];

                    if (IsPure(arg))
                    {
                        args[j] = arg;
                    }
                    else
                    {
                        var temp = Expression.Parameter(arg.Type, "__arg" + j);
                        temps.Add(temp);
                        stmts.Add(Expression.Assign(temp, arg));

                        args[j] = temp;
                    }
                }

                return index.Update(obj!, args.ToReadOnlyUnsafe());
            }
        }

        public static Expression ReduceAssignment(Expression lhs, Func<Expression, Expression> functionalOp, bool prefix = true, LambdaExpression? leftConversion = null)
        {
            return lhs.NodeType switch
            {
                ExpressionType.MemberAccess => ReduceMember(lhs, functionalOp, prefix, leftConversion),
                ExpressionType.Index => ReduceIndex(lhs, functionalOp, prefix, leftConversion),
                ExpressionType.Parameter => ReduceVariable(lhs, functionalOp, prefix, leftConversion),
                _ => lhs switch
                {
                    IndexCSharpExpression index => ReduceIndexCSharp(index, functionalOp, prefix, leftConversion),
                    ArrayAccessCSharpExpression arrayAccess => ReduceArrayAccessCSharp(arrayAccess, functionalOp, prefix, leftConversion),
                    IndexerAccessCSharpExpression indexerAccess => ReduceIndexerAccessCSharp(indexerAccess, functionalOp, prefix, leftConversion),
                    _ => throw ContractUtils.Unreachable,
                },
            };
        }

        private static Expression ReduceMember(Expression lhs, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression? leftConversion)
        {
            var res = default(Expression);

            var member = (MemberExpression)lhs;

            if (member.Expression == null)
            {
                res = ReduceVariable(lhs, functionalOp, prefix, leftConversion);
            }
            else
            {
                if (NeedByRefAssign(member.Expression))
                {
                    // NB: See https://github.com/dotnet/corefx/issues/4984 for a discussion on the need to deal
                    //     with by-ref assignment LHS expressions like this.

                    var lhsTemp = Expression.Parameter(member.Type, "__lhs");
                    var op = functionalOp(WithLeftConversion(lhsTemp, leftConversion));
                    var method = typeof(RuntimeOpsEx).GetMethod(prefix ? nameof(RuntimeOpsEx.PreAssignByRef) : nameof(RuntimeOpsEx.PostAssignByRef))!; // TODO: well-known members
                    method = method.MakeGenericMethod(member.Type);
                    res = Expression.Call(method, member, Expression.Lambda(op, lhsTemp));
                }
                else
                {
                    var lhsTemp = Expression.Parameter(member.Expression.Type, "__lhs");
                    var lhsAssign = Expression.Assign(lhsTemp, member.Expression);
                    member = member.Update(lhsTemp);

                    if (prefix)
                    {
                        res =
                            Expression.Block(
                                new[] { lhsTemp },
                                lhsAssign,
                                Expression.Assign(member, functionalOp(WithLeftConversion(member, leftConversion)))
                            );
                    }
                    else
                    {
                        var temp = Expression.Parameter(member.Type, "__temp");

                        res =
                            Expression.Block(
                                new[] { lhsTemp, temp },
                                lhsAssign,
                                Expression.Assign(temp, member),
                                Expression.Assign(member, functionalOp(WithLeftConversion(temp, leftConversion))),
                                temp
                            );
                    }
                }
            }

            return res;
        }

        private static Expression ReduceIndex(Expression lhs, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression? leftConversion)
        {
            var index = (IndexExpression)lhs;

            var isByRef = false;

            if (NeedByRefAssign(index.Object))
            {
                // NB: See https://github.com/dotnet/corefx/issues/4984 for a discussion on the need to deal
                //     with by-ref assignment LHS expressions like this.

                // NB: For arrays, the check won't pass. For value types, this means there's an indexer and
                //     we still have to perform a get and a set (i.e. we can't get a ref to the location that's
                //     being indexed into). However, we don't want to operate on a copy of the struct. It now
                //     depends on the nature of `index.Object` what we can do. If it's a variable or a field,
                //     we can get the location as a reference. We don't have to check for this though because
                //     the LINQ expression APIs allow passing anything by reference.

                isByRef = true;
            }

            var n = index.Arguments.Count;
            var args = new Expression[n];
            var block = new List<Expression>();
            var temps = new List<ParameterExpression>();

            Expression? receiver = null;
            ParameterExpression? obj = null;

            if (index.Object != null)
            {
                receiver = index.Object;

                if (!isByRef)
                {
                    obj = Expression.Parameter(receiver.Type, "__object");
                    temps.Add(obj);

                    block.Add(Expression.Assign(obj, receiver));
                }
                else
                {
                    obj = Expression.Parameter(receiver.Type.MakeByRefType(), "__object");
                }
            }

            for (var j = 0; j < n; j++)
            {
                var arg = index.Arguments[j];

                if (IsPure(arg))
                {
                    args[j] = arg;
                }
                else
                {
                    var temp = Expression.Parameter(arg.Type, "__arg" + j);
                    temps.Add(temp);

                    block.Add(Expression.Assign(temp, arg));
                    args[j] = temp;
                }
            }

            // NB: LINQ has some inconsistencies with null support for object on a static indexer. The factory
            //     for Property supports null, but Update doesn't (though passing null would work).

            index = index.Update(obj!, args.ToReadOnlyUnsafe());

            if (prefix)
            {
                block.Add(Expression.Assign(index, functionalOp(WithLeftConversion(index, leftConversion))));
            }
            else
            {
                var lastTemp = Expression.Parameter(index.Type, "__index");
                temps.Add(lastTemp);

                block.Add(Expression.Assign(lastTemp, index));
                block.Add(Expression.Assign(index, functionalOp(WithLeftConversion(lastTemp, leftConversion))));
                block.Add(lastTemp);
            }

            var res = (Expression)Expression.Block(temps, block);

            if (isByRef)
            {
                Debug.Assert(obj != null);
                Debug.Assert(receiver != null);

                var method = typeof(RuntimeOpsEx).GetMethod(nameof(RuntimeOpsEx.WithByRef))!; // TODO: well-known members
                method = method.MakeGenericMethod(obj.Type, res.Type);
                var delegateType = typeof(FuncByRef<,>).MakeGenericType(obj.Type, res.Type);

                // NB: The introduction of a lambda to lift the computation to the WithByRef helper method can be
                //     expensive because of closure creation. This scenario with mutable structs and indexers should
                //     be quite rare though.

                res = Expression.Call(method, receiver, Expression.Lambda(delegateType, res, obj));
            }

            return res;
        }

        private static bool NeedByRefAssign(Expression? lhs)
        {
            // NB: Block doesn't support by-ref locals, so we have to use a helper method to perform the assignment
            //     to the target without causing repeated re-evaluation of the LHS.

            if (lhs != null && lhs.Type.IsValueType && !lhs.Type.IsNullableType())
            {
                return true;
            }

            return false;
        }

        private static Expression ReduceVariable(Expression lhs, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression? leftConversion)
        {
            Expression res;

            if (prefix)
            {
                res = Expression.Assign(lhs, functionalOp(WithLeftConversion(lhs, leftConversion)));
            }
            else
            {
                var temp = Expression.Parameter(lhs.Type, "__temp");
                res =
                    Expression.Block(
                        new[] { temp },
                        Expression.Assign(temp, lhs),
                        Expression.Assign(lhs, functionalOp(WithLeftConversion(temp, leftConversion))),
                        temp
                    );
            }

            return res;
        }

        private static Expression ReduceIndexCSharp(IndexCSharpExpression index, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression? leftConversion)
        {
            return index.ReduceAssign(x => ReduceVariable(x, functionalOp, prefix, leftConversion));
        }

        private static Expression ReduceArrayAccessCSharp(ArrayAccessCSharpExpression arrayAccess, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression? leftConversion)
        {
            return arrayAccess.ReduceAssign(x => ReduceVariable(x, functionalOp, prefix, leftConversion));
        }

        private static Expression ReduceIndexerAccessCSharp(IndexerAccessCSharpExpression indexerAccess, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression? leftConversion)
        {
            return indexerAccess.ReduceAssign(x => ReduceVariable(x, functionalOp, prefix, leftConversion));
        }

        private static Expression WithLeftConversion(Expression expression, LambdaExpression? leftConversion)
        {
            if (leftConversion != null)
            {
                expression = Apply(leftConversion, expression);
            }

            return expression;
        }
    }
}
