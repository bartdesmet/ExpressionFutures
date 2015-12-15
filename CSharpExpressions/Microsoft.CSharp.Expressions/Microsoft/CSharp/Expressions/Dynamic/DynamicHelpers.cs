// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Linq.Expressions.Compiler;
using System.Runtime.CompilerServices;

namespace Microsoft.CSharp.Expressions
{
    internal static class DynamicHelpers
    {
        public static Expression MakeDynamic(Type type, CallSiteBinder binder, IEnumerable<Expression> arguments, Type[] argumentTypes)
        {
            if (argumentTypes == null)
            {
                return Expression.Dynamic(binder, type, arguments);
            }
            else
            {
                // NB: This is a trick to leverage MakeCallSiteDelegate; we should refactor it to take in an array of types.
                var args = argumentTypes.Map(a => (Expression)Expression.Default(a)).ToReadOnly();
                var delegateType = DelegateHelpers.MakeCallSiteDelegate(args, type);
                return Expression.MakeDynamic(delegateType, binder, arguments);
            }
        }

        public static Expression ReduceDynamicAssignment(DynamicCSharpArgument left, Func<Expression, Expression> functionalOp, CSharpBinderFlags flags, bool prefix = true)
        {
            var lhs = left.Expression;

            var dynamicMember = lhs as GetMemberDynamicCSharpExpression;
            if (dynamicMember != null)
            {
                return ReduceDynamicMember(dynamicMember, functionalOp, flags, prefix);
            }

            var dynamicIndex = lhs as GetIndexDynamicCSharpExpression;
            if (dynamicIndex != null)
            {
                return ReduceDynamicIndex(dynamicIndex, functionalOp, flags, prefix);
            }

            return Helpers.ReduceAssignment(lhs, functionalOp, prefix);
        }

        private static Expression ReduceDynamicMember(GetMemberDynamicCSharpExpression member, Func<Expression, Expression> functionalOp, CSharpBinderFlags flags, bool prefix)
        {
            var args = default(DynamicCSharpArgument[]);
            var block = default(Expression[]);
            var temps = default(ParameterExpression[]);
            var i = CopyArguments(member.Object, member.Arguments, prefix, out args, out block, out temps);

            member = member.Update(temps[0], new TrueReadOnlyCollection<DynamicCSharpArgument>(args));

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
            var args = default(DynamicCSharpArgument[]);
            var block = default(Expression[]);
            var temps = default(ParameterExpression[]);
            var i = CopyArguments(index.Object, index.Arguments, prefix, out args, out block, out temps);

            index = index.Update(temps[0], new TrueReadOnlyCollection<DynamicCSharpArgument>(args));

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

        public static void RequiresCanWrite(Expression expression, string paramName)
        {
            ContractUtils.RequiresNotNull(expression, paramName);

            if (expression is GetIndexDynamicCSharpExpression)
            {
                return;
            }

            if (expression is GetMemberDynamicCSharpExpression)
            {
                return;
            }

            Helpers.RequiresCanWrite(expression, paramName);
        }

        public static void RequiresCanRead(Expression expression, string paramName)
        {
            ExpressionStubs.RequiresCanRead(expression, paramName);
        }
    }
}
