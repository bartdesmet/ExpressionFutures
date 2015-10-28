// Prototyping support for interpretation of TryExpression with a fault handler or exception filters.
//
// bartde - October 2015

using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    class Program
    {
        static void Main()
        {
            var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("bar"), AssemblyBuilderAccess.Save);
            var mod = asm.DefineDynamicModule("bar.dll");
            var typ = mod.DefineType("Foo");

            var lblRet = Expression.Label(typeof(int));
            var lbl1 = Expression.Label(typeof(int));
            var ex = Expression.Parameter(typeof(Exception));

            var expressions = new Expression[]
            {
                Expression.TryFault(
                    Expression.Constant(1),
                    Expression.Constant(2)
                ),
                Expression.Block(
                    Expression.TryFault(
                        Expression.Goto(lblRet, Expression.Constant(1)),
                        Expression.Constant(2)
                    ),
                    Expression.Label(lblRet, Expression.Constant(0))
                ),
                Expression.Block(
                    Expression.TryFault(
                        Expression.Block(
                            Expression.Goto(lblRet, Expression.Constant(1)),
                            Expression.Constant(2)
                        ),
                        Expression.Constant(3)
                    ),
                    Expression.Label(lblRet, Expression.Constant(0))
                ),
                Expression.Block(
                    Expression.TryFault(
                        Expression.Block(
                            Expression.Goto(lbl1, Expression.Constant(1)),
                            Expression.Goto(lblRet, Expression.Constant(2))
                        ),
                        Expression.Constant(3)
                    ),
                    Expression.Label(lbl1, Expression.Constant(-1)),
                    Expression.Label(lblRet, Expression.Constant(0))
                ),
                Expression.TryCatch(
                    Expression.Constant(1),
                    Expression.Catch(ex, Expression.Constant(2), Expression.Constant(true))
                ),
            };

            var i = 0;
            foreach (var expression in expressions)
            {
                var mtd = typ.DefineMethod("M" + i, MethodAttributes.Public | MethodAttributes.Static, typeof(int), new Type[0]);
                Expression.Lambda<Func<int>>(expression).CompileToMethod(mtd);

                var rewritten = new ExceptionHandlingLowering().Visit(expression);
                var cw = Expression.Lambda<Func<int>>(rewritten).Compile();
                cw();

                i++;
            }
        }
    }
}