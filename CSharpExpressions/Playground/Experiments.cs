// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Playground
{
    static class Experiments
    {
        public static void StackSpilling()
        {
            var t = CSharpExpression.Await(Expression.Constant(Task.FromResult(42)));
            var r = Expression.Add(t, t);
            var x = Spiller.Spill(r);
        }
    }
}
