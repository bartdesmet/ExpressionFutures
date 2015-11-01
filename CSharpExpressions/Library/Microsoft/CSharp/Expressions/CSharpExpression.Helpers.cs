// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Linq.Expressions;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    partial class CSharpExpression
    {
        private static void ValidateCondition(Expression test)
        {
            RequiresCanRead(test, nameof(test));

            // TODO: We can be more flexible and allow the rules in C# spec 7.20.
            //       Note that this behavior is the same as IfThen, but we could also add C# specific nodes for those,
            //       with the more flexible construction behavior.
            if (test.Type != typeof(bool))
            {
                throw LinqError.ArgumentMustBeBoolean();
            }
        }
    }
}
