// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Microsoft.CSharp.RuntimeBinder;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    internal static class DynamicHelpers
    {
        public static Expression MakeDynamic(Type type, CallSiteBinder binder, IEnumerable<Expression> arguments, Type[]? argumentTypes)
        {
            if (argumentTypes == null)
            {
                return Expression.Dynamic(binder, type, arguments);
            }
            else
            {
                var types = new Type[argumentTypes.Length + 2];
                types[0] = typeof(CallSite);
                Array.Copy(argumentTypes, 0, types, 1, argumentTypes.Length);
                types[^1] = type;

                var delegateType = Expression.GetDelegateType(types);
                
                return Expression.MakeDynamic(delegateType, binder, arguments);
            }
        }

        public static Expression ReduceDynamicAssignment(DynamicCSharpArgument left, Func<Expression, Expression> functionalOp, CSharpBinderFlags flags, bool prefix = true)
        {
            var lhs = MakeWriteable(left.Expression);

            return lhs switch
            {
                GetMemberDynamicCSharpExpression dynamicMember => ReduceDynamicMember(dynamicMember, functionalOp, flags, prefix),
                GetIndexDynamicCSharpExpression dynamicIndex => ReduceDynamicIndex(dynamicIndex, functionalOp, flags, prefix),
                _ => Helpers.ReduceAssignment(lhs, functionalOp, prefix),
            };
        }

        private static Expression ReduceDynamicMember(GetMemberDynamicCSharpExpression member, Func<Expression, Expression> functionalOp, CSharpBinderFlags flags, bool prefix)
        {
            var i = CopyArguments(member.Object, member.Arguments, prefix, out DynamicCSharpArgument[] args, out Expression[] block, out ParameterExpression[] temps);

            member = member.Update(temps[0], args.ToReadOnlyUnsafe());

            if (prefix)
            {
                block[i++] = member.ReduceAssignment(functionalOp(member), flags);
            }
            else
            {
                var lastTemp = temps[i] = Expression.Parameter(member.Type, "__temp");

                block[i] = Expression.Assign(temps[i], member);
                i++;

                block[i++] = member.ReduceAssignment(functionalOp(lastTemp), flags);
                block[i++] = lastTemp;
            }

            var res = Expression.Block(temps, block);
            return res;
        }

        private static Expression ReduceDynamicIndex(GetIndexDynamicCSharpExpression index, Func<Expression, Expression> functionalOp, CSharpBinderFlags flags, bool prefix)
        {
            var i = CopyArguments(index.Object, index.Arguments, prefix, out DynamicCSharpArgument[] args, out Expression[] block, out ParameterExpression[] temps);

            index = index.Update(temps[0], args.ToReadOnlyUnsafe());

            if (prefix)
            {
                block[i++] = index.ReduceAssignment(functionalOp(index), flags);
            }
            else
            {
                var lastTemp = temps[i] = Expression.Parameter(index.Type, "__index");

                block[i] = Expression.Assign(temps[i], index);
                i++;

                block[i++] = index.ReduceAssignment(functionalOp(lastTemp), flags);
                block[i++] = lastTemp;
            }

            var res = Expression.Block(temps, block);
            return res;
        }

        public static void CopyReceiverArgument(Expression receiver, CSharpArgumentInfo[] argumentInfos, Expression[] expressions, ref Type[]? argumentTypes)
        {
            var receiverFlags = CSharpArgumentInfoFlags.None;
            if (IsReceiverByRef(receiver))
            {
                receiverFlags |= CSharpArgumentInfoFlags.IsRef;
                argumentTypes = new Type[argumentInfos.Length];
                argumentTypes[0] = receiver.Type.MakeByRefType();
            }

            expressions[0] = receiver;
            argumentInfos[0] = CSharpArgumentInfo.Create(receiverFlags, null);
        }

        private static int CopyArguments(Expression receiver, ReadOnlyCollection<DynamicCSharpArgument> arguments, bool prefix, out DynamicCSharpArgument[] args, out Expression[] block, out ParameterExpression[] temps)
        {
            var n = arguments.Count;

            args = new DynamicCSharpArgument[n];
            block = new Expression[n + (prefix ? 2 : 4)];
            temps = new ParameterExpression[n + (prefix ? 1 : 2)];

            // ISSUE: If `receiver` is a value type, we're creating a copy here. How do we capture it
            //        as a `ref` instead? See https://github.com/dotnet/corefx/issues/4984 for this issue
            //        in the LINQ API as well.

            var i = 0;
            temps[i] = Expression.Parameter(receiver.Type, "__object");
            block[i] = Expression.Assign(temps[i], receiver);
            i++;

            while (i <= n)
            {
                var arg = arguments[i - 1];
                temps[i] = Expression.Parameter(arg.Expression.Type, "__arg" + i);
                args[i - 1] = arg.Update(temps[i]);
                block[i] = Expression.Assign(temps[i], arg.Expression);
                i++;
            }

            return i;
        }

        public static void CopyArguments(ReadOnlyCollection<DynamicCSharpArgument> arguments, CSharpArgumentInfo[] argumentInfos, Expression[] expressions, ref Type[]? argumentTypes)
        {
            var n = arguments.Count;

            for (var i = 0; i < n; i++)
            {
                var argument = arguments[i];
                argumentInfos[i + 1] = argument.ArgumentInfo;
                expressions[i + 1] = argument.Expression;

                if ((argument.Flags & (CSharpArgumentInfoFlags.IsRef | CSharpArgumentInfoFlags.IsOut)) != 0)
                {
                    if (argumentTypes == null)
                    {
                        argumentTypes = new Type[argumentInfos.Length];
                        argumentTypes[0] = expressions[0].Type;

                        for (var j = 0; j < i; j++)
                        {
                            argumentTypes[j + 1] = arguments[j].Expression.Type;
                        }
                    }

                    argumentTypes[i + 1] = argument.Expression.Type.MakeByRefType();
                }
                else if (argumentTypes != null)
                {
                    argumentTypes[i + 1] = argument.Expression.Type;
                }
            }
        }

        public static void RequiresCanWrite(Expression expression, string paramName)
        {
            ContractUtils.RequiresNotNull(expression, paramName);

            if (expression is GetIndexDynamicCSharpExpression or GetMemberDynamicCSharpExpression)
            {
                return;
            }

            Helpers.RequiresCanWrite(expression, paramName);
        }

        public static void RequiresCanRead(Expression expression, string paramName)
        {
            ExpressionUtils.RequiresCanRead(expression, paramName);
        }

        public static bool IsReceiverByRef(Expression expression)
        {
            // NB: Mimics behavior of GetReceiverRefKind in the C# compiler.

            // DESIGN: Should we keep the ArgumentInfo object for the receiver in our dynamic nodes instead?
            //         Makes the shape of the tree wonky though, so we attempt to infer the behavior here.

            if (expression.Type.IsValueType)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Parameter:
                    case ExpressionType.ArrayIndex:
                        return true;
                }
            }

            return false;
        }
    }
}
