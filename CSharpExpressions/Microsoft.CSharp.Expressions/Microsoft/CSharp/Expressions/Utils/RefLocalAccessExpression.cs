// Prototyping extended expression trees for C#.
//
// bartde - September 2022

#nullable enable

using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.CSharp.Expressions
{
    internal sealed class RefLocalAccessExpression : Expression
    {
        private readonly ParameterExpression _refHolder;
        private readonly ParameterExpression _objByRef;
        private readonly Expression _access;

        public RefLocalAccessExpression(ParameterExpression refHolder, Func<Expression, Expression> createAccess)
        {
            _refHolder = refHolder;

            var memberExprTypeByRef = refHolder.Type.GetGenericArguments()[0].MakeByRefType();

            _objByRef = Expression.Parameter(memberExprTypeByRef, "__obj");
            _access = createAccess(_objByRef);
        }

        public override Type Type => _access.Type;

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override bool CanReduce => false;

        public Expression Assign(Expression valueExpr)
        {
            var value = Expression.Parameter(_access.Type, "__value");
            var assign = Expression.Assign(_access, value);

            var actionByRefType = typeof(ActionByRef<,>).MakeGenericType(_objByRef.Type, _access.Type);
            var assignAction = Expression.Lambda(actionByRefType, assign, _objByRef, value);
            var invoke = _refHolder.Type.GetMethod(nameof(RefHolder<int>.Invoke))!.MakeGenericMethod(_access.Type);

            return Expression.Call(_refHolder, invoke, assignAction, valueExpr);
        }
    }
}
