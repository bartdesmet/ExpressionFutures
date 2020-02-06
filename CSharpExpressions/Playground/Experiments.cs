// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.Expressions.Compiler;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Playground
{
    static class Experiments
    {
        public static void StackSpilling()
        {
            StackSpilling1();
            StackSpilling2();
            StackSpilling3();
            StackSpilling4();
        }

        public static void Shadowing()
        {
            Shadowing1();
        }

        static void StackSpilling1()
        {
            var t = CSharpExpression.Await(Expression.Constant(Task.FromResult(42)));
            var r = Expression.Add(t, t);
            _ = Spiller.Spill(r);
        }

        static void StackSpilling2()
        {
            var t = CSharpExpression.Await(CSharpExpression.Await(Expression.Constant(Task.FromResult(Task.FromResult(42)))));
            var r = Expression.Add(t, t);
            _ = Spiller.Spill(r);
        }

        static void StackSpilling3()
        {
            var e = (Expression<Func<int>>)(() => Task.FromResult(1).Result + Task.FromResult(2 + Task.FromResult(3).Result).Result);
            var r = new TaskRewriter().Visit(e.Body);
            var x = Spiller.Spill(r);
        }

        static void StackSpilling4()
        {
            var e = (Expression<Func<int>>)(() => F(Math.Abs(-1), Task.FromResult(2).Result, Math.Abs(-3)));
            var r = new TaskRewriter().Visit(e.Body);
            var x = Spiller.Spill(r);
        }

        static void Shadowing1()
        {
            var p0 = Expression.Parameter(typeof(int));
            var p1 = Expression.Parameter(typeof(int));
            var p2 = Expression.Parameter(typeof(int));
            var p3 = Expression.Parameter(typeof(int));
            var e = Expression.Block(new[] { p0, p1 }, Expression.Block(new[] { p1, p2 }, Expression.Block(new[] { p2, p3 }, Expression.Block(new[] { p3, p1 }, Expression.Add(p0, Expression.Multiply(p1, Expression.Subtract(p2, p3)))))));
            _ = ShadowEliminator.Eliminate(e);
        }

        static void ArrayInitOptimization()
        {
            // See comments in NewMultidimensionalArrayInit node for more information about the goal of this
            // optimization experiment.

            foreach (var i in Enumerable.Range(1, 10).Select(x => x * 100))
            {
                ArrayInitOptimization(i, 100000);
            }
        }

        static void ArrayInitOptimization(int elementCount, int iterationCount)
        {
            var e = Expression.NewArrayInit(typeof(int), Enumerable.Range(0, elementCount).Select(i => Expression.Constant(i)));

            var sw = Stopwatch.StartNew();

            var f = Expression.Lambda<Func<int[]>>(e);
            var g = f.Compile();

            //Console.WriteLine($"Compile({elementCount}) = {sw.ElapsedMilliseconds}ms");

            sw.Restart();

            for (var i = 0; i < iterationCount; i++)
            {
                g();
            }

            Console.WriteLine($"[RAW] new int[{elementCount}] x {iterationCount} = {sw.ElapsedMilliseconds}ms");

            sw.Restart();

            var o = ArrayInitOptimizer.Instance.VisitAndConvert(f, nameof(ArrayInitOptimization));
            var h = o.Compile();

            //Console.WriteLine($"Optimize({elementCount}) = {sw.ElapsedMilliseconds}ms");

            sw.Restart();

            for (var i = 0; i < iterationCount; i++)
            {
                h();
            }

            Console.WriteLine($"[OPT] new int[{elementCount}] x {iterationCount} = {sw.ElapsedMilliseconds}ms");

            Console.WriteLine();
        }

        static int F(int x,int y, int z)
        {
            throw new NotImplementedException();
        }

        class TaskRewriter : BetterExpressionVisitor
        {
            protected override Expression VisitMember(MemberExpression node)
            {
                var member = node.Member;
                var type = member.DeclaringType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    if (member.Name == "Result")
                    {
                        return CSharpExpression.Await(Visit(node.Expression));
                    }
                }

                return base.VisitMember(node);
            }
        }

        class ArrayInitOptimizer : ExpressionVisitor
        {
            public static readonly ExpressionVisitor Instance = new ArrayInitOptimizer();

            protected override Expression VisitNewArray(NewArrayExpression node)
            {
                if (node.NodeType == ExpressionType.NewArrayInit)
                {
                    if (node.Type == typeof(int[]))
                    {
                        if (node.Expressions.All(e => e.NodeType == ExpressionType.Constant))
                        {
                            var values = node.Expressions.Select(e => (int)((ConstantExpression)e).Value).ToArray();
                            return Expression.Convert(Expression.Call(Expression.Constant(values), typeof(Array).GetMethod("Clone")), node.Type);
                        }
                    }
                }

                return base.VisitNewArray(node);
            }
        }
    }
}
