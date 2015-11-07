// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    internal static class Reducer
    {
        public static Expression Reduce(Expression expression)
        {
            return Impl.Instance.Visit(expression);
        }

        class Impl : ExpressionVisitor
        {
            public static readonly ExpressionVisitor Instance = new Impl();

            protected override Expression VisitExtension(Expression node)
            {
                var csharp = node as CSharpExpression;
                if (csharp != null && csharp.CSharpNodeType == CSharpExpressionType.Await)
                {
                    var await = (AwaitCSharpExpression)csharp;
                    return await.Update(Visit(await.Operand));
                }

                return base.VisitExtension(node);
            }
        }
    }
}
