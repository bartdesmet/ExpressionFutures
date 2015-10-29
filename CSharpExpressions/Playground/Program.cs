// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    class Program
    {
        static void Main()
        {
            Call();
        }

        static void Call()
        {
            var mtd = typeof(Math).GetMethod("Min", new[] { typeof(int), typeof(int) });

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var arg0 = CSharpExpression.Bind(val1, Expression.Constant(1));
            var arg1 = CSharpExpression.Bind(val2, Expression.Constant(2));

            var call = CSharpExpression.Call(mtd, arg1, arg0);
        }
    }
}
