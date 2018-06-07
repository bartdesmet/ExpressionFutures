# Expression Types

This project contains examples for the *expression types* feature implemented at [bartdesmet/roslyn/ExpressionTreeLikeTypes](https://github.com/bartdesmet/roslyn/blob/ExpressionTreeLikeTypes/docs/features/expression-types.md).

In order to build this project, the Roslyn fork at the link above has to be used.

## Example

As shown in the sample code in this project, one can write:

```csharp
Quote<Func<int, int>> f = x => x + 42;
```

Provided `Quote<T>` is an *expression type* and its corresponding builder type has factory methods for `Parameter`, `Lambda`, `Add`, and `Constant` because these language features are used in the lambda expression to be captured as an expression tree.

```csharp
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
}
```

## Expression Factory Methods

The compiler binds to a set of well-defined factory methods when lowering expression trees. These factories are documented at [ExpressionFactory.cs](https://github.com/bartdesmet/ExpressionFutures/blob/master/ExpressionLikeTypes/ExpressionLikeTypes/ExpressionFactory.cs).

Going forward, we'll create:

* A specification that describes the mapping between C# language features and the corresponding factory calls (including known quirks that are retained for compatibility).
* Proposals for factory methods that correspond to new C# language features. See the [CSharpExpressions](https://github.com/bartdesmet/ExpressionFutures/tree/master/CSharpExpressions) project for work in this area.