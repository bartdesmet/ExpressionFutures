// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
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

        static void StackSpilling1()
        {
            var t = CSharpExpression.Await(Expression.Constant(Task.FromResult(42)));
            var r = Expression.Add(t, t);
            var x = Spiller.Spill(r);
        }

        static void StackSpilling2()
        {
            var t = CSharpExpression.Await(CSharpExpression.Await(Expression.Constant(Task.FromResult(Task.FromResult(42)))));
            var r = Expression.Add(t, t);
            var x = Spiller.Spill(r);
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

        static int F(int x,int y, int z)
        {
            throw new NotImplementedException();
        }

        class TaskRewriter : ExpressionVisitor
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
    }
}
