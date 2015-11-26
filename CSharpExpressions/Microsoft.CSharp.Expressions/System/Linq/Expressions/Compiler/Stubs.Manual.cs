// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions.Compiler;
using System.Reflection;

namespace System.Linq.Expressions
{
    [ExcludeFromCodeCoverage]
    internal static class ExpressionExtensions
    {
        public static TypeCode GetTypeCode(this Type type)
        {
            return Type.GetTypeCode(type);
        }

        public static LambdaExpression Accept(this LambdaExpression expression, StackSpiller spiller)
        {
            return (LambdaExpression)new SpillVisitor(spiller).Visit(expression);
        }

        class SpillVisitor : ExpressionVisitor
        {
            private readonly StackSpiller _spiller;

            public SpillVisitor(StackSpiller spiller)
            {
                _spiller = spiller;
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                return _spiller.Rewrite(node);
            }
        }

        private static readonly MethodInfo s_InvocationExpression_Rewrite = typeof(InvocationExpression).GetMethod("Rewrite", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, new[] { typeof(Expression), typeof(Expression[]) }, null);

        public static InvocationExpression Rewrite(this InvocationExpression expression, Expression lambda, Expression[] arguments)
        {
            try
            {
                return (InvocationExpression)s_InvocationExpression_Rewrite.Invoke(expression, new object[] { lambda, arguments });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly PropertyInfo s_InvocationExpression_LambdaOperand = typeof(InvocationExpression).GetProperty("LambdaOperand", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        public static LambdaExpression LambdaOperand(this InvocationExpression expression)
        {
            try
            {
                return (LambdaExpression)s_InvocationExpression_LambdaOperand.GetValue(expression);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_MethodCallExpression_Rewrite = typeof(MethodCallExpression).GetMethod("Rewrite", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, new[] { typeof(Expression), typeof(IList<Expression>) }, null);

        public static MethodCallExpression Rewrite(this MethodCallExpression expression, Expression instance, IList<Expression> args)
        {
            try
            {
                return (MethodCallExpression)s_MethodCallExpression_Rewrite.Invoke(expression, new object[] { instance, args });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_BlockExpression_Rewrite = typeof(BlockExpression).GetMethod("Rewrite", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, new[] { typeof(ReadOnlyCollection<ParameterExpression>), typeof(Expression[]) }, null);

        public static BlockExpression Rewrite(this BlockExpression expression, ReadOnlyCollection<ParameterExpression> variables, Expression[] args)
        {
            try
            {
                return (BlockExpression)s_BlockExpression_Rewrite.Invoke(expression, new object[] { variables, args });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly MethodInfo s_BlockExpression_GetExpression = typeof(BlockExpression).GetMethod("GetExpression", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, new[] { typeof(int) }, null);

        public static Expression GetExpression(this BlockExpression expression, int index)
        {
            try
            {
                return (Expression)s_BlockExpression_GetExpression.Invoke(expression, new object[] { index });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static readonly PropertyInfo s_BlockExpression_ExpressionCount = typeof(BlockExpression).GetProperty("ExpressionCount", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        public static int ExpressionCount(this BlockExpression expression)
        {
            try
            {
                return (int)s_BlockExpression_ExpressionCount.GetValue(expression);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public static Expression<T> CreateExpression<T>(Expression body, string name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters)
        {
#if LINQ
            return new Expression<T>(body, name, tailCall, parameters);
#else
            return Expression.Lambda<T>(body, name, tailCall, parameters);
#endif
        }

        public static IndexExpression CreateIndexExpression(Expression instance, PropertyInfo indexer, IList<Expression> arguments)
        {
#if LINQ
            return new IndexExpression(instance, indexer, arguments);
#else
            return Expression.MakeIndex(instance, indexer, arguments);
#endif
        }

        public static BinaryExpression CreateAssignBinaryExpression(Expression left, Expression right)
        {
#if LINQ
            return new AssignBinaryExpression(left, right);
#else
            return Expression.Assign(left, right);
#endif
        }

        public static NewExpression CreateNewExpression(ConstructorInfo constructor, IList<Expression> arguments, ReadOnlyCollection<MemberInfo> members)
        {
#if LINQ
            return new NewExpression(constructor, arguments, members);
#else
            if (members == null)
            {
                return Expression.New(constructor, arguments);
            }
            else
            {
                return Expression.New(constructor, arguments, members);
            }
#endif
        }

        public static UnaryExpression CreateUnaryExpression(ExpressionType nodeType, Expression expression, Type type, MethodInfo method)
        {
#if LINQ
            return new UnaryExpression(nodeType, expression, type, method);
#else
            return Expression.MakeUnary(nodeType, expression, type, method);
#endif
        }

        public static LoopExpression CreateLoopExpression(Expression body, LabelTarget @break, LabelTarget @continue)
        {
#if LINQ
            return new LoopExpression(body, @break, @continue);
#else
            return Expression.Loop(body, @break, @continue);
#endif
        }

        public static SwitchCase CreateSwitchCase(Expression body, ReadOnlyCollection<Expression> testValues)
        {
#if LINQ
            return new SwitchCase(body, testValues);
#else
            return Expression.SwitchCase(body, testValues);
#endif
        }

        public static SwitchExpression CreateSwitchExpression(Type type, Expression switchValue, Expression defaultBody, MethodInfo comparison, ReadOnlyCollection<SwitchCase> cases)
        {
#if LINQ
            return new SwitchExpression(type, switchValue, defaultBody, comparison, cases);
#else
            return Expression.Switch(type, switchValue, defaultBody, comparison, cases);
#endif
        }

        public static TryExpression CreateTryExpression(Type type, Expression body, Expression @finally, Expression fault, ReadOnlyCollection<CatchBlock> handlers)
        {
#if LINQ
            return new TryExpression(type, body, @finally, fault, handlers);
#else
            return Expression.MakeTry(type, body, @finally, fault, handlers);
#endif
        }

        public static BlockExpression CreateSpilledExpressionBlock(IList<Expression> expressions)
        {
#if LINQ
            return new SpilledExpressionBlock(expressions);
#else
            return Expression.Block(expressions);
#endif
        }
    }
}