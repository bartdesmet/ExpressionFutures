// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

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

        public static Expression<T> CreateExpression<T>(Expression body, string? name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters)
        {
            return Expression.Lambda<T>(body, name, tailCall, parameters);
        }

        public static IndexExpression CreateIndexExpression(Expression instance, PropertyInfo indexer, IList<Expression> arguments)
        {
            return Expression.MakeIndex(instance, indexer, arguments);
        }

        public static BinaryExpression CreateAssignBinaryExpression(Expression left, Expression right)
        {
            return Expression.Assign(left, right);
        }

        public static NewExpression CreateNewExpression(ConstructorInfo constructor, IList<Expression> arguments, ReadOnlyCollection<MemberInfo> members)
        {
            if (members == null)
            {
                return Expression.New(constructor, arguments);
            }
            else
            {
                return Expression.New(constructor, arguments, members);
            }
        }

        public static UnaryExpression CreateUnaryExpression(ExpressionType nodeType, Expression expression, Type type, MethodInfo method)
        {
            return Expression.MakeUnary(nodeType, expression, type, method);
        }

        public static LoopExpression CreateLoopExpression(Expression body, LabelTarget @break, LabelTarget @continue)
        {
            return Expression.Loop(body, @break, @continue);
        }

        public static SwitchCase CreateSwitchCase(Expression body, ReadOnlyCollection<Expression> testValues)
        {
            return Expression.SwitchCase(body, testValues);
        }

        public static SwitchExpression CreateSwitchExpression(Type type, Expression switchValue, Expression defaultBody, MethodInfo comparison, ReadOnlyCollection<SwitchCase> cases)
        {
            return Expression.Switch(type, switchValue, defaultBody, comparison, cases);
        }

        public static TryExpression CreateTryExpression(Type type, Expression body, Expression @finally, Expression fault, ReadOnlyCollection<CatchBlock> handlers)
        {
            return Expression.MakeTry(type, body, @finally, fault, handlers);
        }

        public static BlockExpression CreateSpilledExpressionBlock(IList<Expression> expressions)
        {
            return Expression.Block(expressions);
        }
    }
}