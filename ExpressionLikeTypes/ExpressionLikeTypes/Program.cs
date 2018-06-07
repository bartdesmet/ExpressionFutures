//
// Examples of the expression types feature discussed at
// https://github.com/bartdesmet/roslyn/blob/ExpressionTreeLikeTypes/docs/features/expression-types.md
//
// bartde - June 2018
//

using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ExpressionLikeTypes
{
    class Program
    {
        static void Main()
        {
            //
            // Without the *expression types* feature, the next line produces:
            //
            // error CS1660:  Cannot convert lambda expression to type 'Quote<Func<int, int>>' because it is not a delegate type
            //
            Quote<Func<int, int>> f = x => x + 42;

            //
            // With the *expression types* feature, the use of a language construct that does not
            // have a matching factory method on the expression type's builder results in:
            //
            // error CS0117: 'Quote' does not contain a definition for 'Subtract'
            //
            // Quote<Func<int, int>> g = x => x - 42;
        }
    }
}

public static class Quote
{
    public static ConstantExpression Constant(object value, Type type) => Expression.Constant(value, type);
    public static ParameterExpression Parameter(Type type, string name) => Expression.Parameter(type, name);
    public static BinaryExpression Add(Expression left, Expression right) => Expression.Add(left, right);
    public static Quote<T> Lambda<T>(Expression body, ParameterExpression[] parameters) => new Quote<T>(Expression.Lambda<T>(body, parameters));
}

[ExpressionBuilder(typeof(Quote))]
public sealed class Quote<T>
{
    public Quote(Expression<T> expression) => Expression = expression;

    public Expression<T> Expression { get; }

    public Expression Body => Expression.Body;
    public ReadOnlyCollection<ParameterExpression> Parameters => Expression.Parameters;
}
