// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    class Reducer : ExpressionVisitor
    {
        public static readonly ExpressionVisitor Instance = new Reducer();

        private Reducer()
        {
        }

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
