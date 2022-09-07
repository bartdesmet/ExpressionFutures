// Prototyping extended expression trees for C#.
//
// bartde - September 2022

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    internal static partial class Helpers
    {
        //
        // NB: The facilities below are a possible "emulation" of ref locals using a ref struct Span<T> wrapper that
        //     refers to a single object. The async lambda rewriter knows about this wrapper in order to avoid hoisting
        //     the wrapper to the heap. The better solution would be proper support for ref locals in BlockExpression.
        //

        private static bool SupportsRefLocals =>
#if NETCORE
            true;
#else
            false;
#endif

        private static RefLocalAccessExpression CreateRefLocalAccess(Expression expression, Func<Expression, Expression> access, List<ParameterExpression> temps, List<Expression> stmts)
        {
            var type = expression.Type;
            var typeByRef = type.MakeByRefType();

            var refHolderType = typeof(RefHolder<>).MakeGenericType(type);
            var refHolderCtor = refHolderType.GetConstructor(new[] { typeByRef })!;
            var refHolder = Expression.New(refHolderCtor, expression);

            // NB: This shape of variable and assignment is detected by RefLocalRewriter. If making changes here,
            //     make sure to update the detection logic accordingly.

            var refHolderTemp = Expression.Parameter(refHolder.Type, "__ref");
            var refHolderAssign = Expression.Assign(refHolderTemp, refHolder);

            temps.Add(refHolderTemp);
            stmts.Add(refHolderAssign);

            return new RefLocalAccessExpression(refHolderTemp, access);
        }
    }
}
