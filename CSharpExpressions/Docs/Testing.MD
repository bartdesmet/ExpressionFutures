# Testing Strategy

This page describes the various testing approaches taken to ensure both the quality of the Roslyn compiler changes as well as the underlying expression API targeted by the extensions.

## Testing the runtime library

In order to test the runtime library, we need to focus on various areas, which are elaborated on below.

### Factories

One class of tests deals with asserting the correctness of the factory methods on `CSharpExpression`, `CSharpStatement`, and `DynamicCSharpExpression` without making any assumptions about those being called by the code emitted by the Roslyn compiler. This ensures those factory methods are generally useful for constructing expression trees.

Typical checks include checking whether operands to newly created nodes are readable or writeable, have types that are compatible with the operation represented by the node, etc. This code makes extensive use of stubs that call into the `System.Linq.Expressions` APIs which we don't test and assume to be correct. Note that errors thrown by factories are either reused from the `System.Linq.Expressions` APIs or are new ones generated via the [Error.tt T4 file](../Microsoft.CSharp.Expressions/Error.tt) file. We check for the expected exception type in our tests.

Tests for the factory methods exercise a bunch of negate cases where an invalid node could be created, e.g. one that has does not type check or would violate type safety. In addition, the indirect factory code path of calling `Update` methods is also exercised.

### Nodes

Test cases for the various node types are quite straightforward and include checking for the behavior of various properties. Note that when we optimize the layout of the nodes, this becomes more important given that many properties will become virtual members.

### Utilities

Besides the nodes, we also test various utilities both in a black-box and white-box manner. For the former, a good example is the `CSharpExpressionVisitor` which is tested in conjunction with `Accept` and `Update` methods on nodes. We merely test for the double-dispatch behavior into the right visitor method; combined with tests for `Update` methods that should suffice to ensure correctness of visitor behavior and tree rewriting.

For some internal utilities, such as `Helpers`, we use a white-box test approach and take advantage of `InternalsVisibleTo` visibility of `Microsoft.CSharp.Expressions` granted to the test projects. Note that some of those utilities would have value to more expression developers, so we could consider to make some of them public. This is discussed more in [BCL](BCL.MD).

### Reduction support

The biggest areas for testing are Roslyn compiler integration and runtime reduction and compilation of the new expression nodes. This is where most of the complexity is and where we need to exhaustively test many cases (e.g. variations for many types). We achieve this testing in two ways: one that doesn't depend on Roslyn changes and purely focuses on reduction and runtime compilation logic, and one that takes an end-to-end approach.

Reduction support is tested on a node-by-node basis, in conjunction with `Compile` calls which indirectly rely on `Reduce` behavior. These tests can be recognized by `_Compile` in the method name and typically make use of a helper method called `AssertCompile`. This method has a few overloads and performs the following tasks:

- Instrument the given expression with a logging capability, described below.
- Wrap the resulting expression in a `Lambda` node in order to be able to compile it.
- Compile and evaluate the resulting lambda expression.
- Assert equality of actual and expected results.

The logging facility is provided by another helper method called `WithLog`. It wraps a given expression in a `Block` that instantiates a `List<string>` that can be written to by the test code. It then evaluates the expression and bundles the evaluation result with the log to return it in a `LogAndResult<T>` object (where `T` is the type of the result, and `object` is used for `void` returning delegates, using a `null` reference for the result).

An example of the `AssertCompile` behavior is shown here:

```csharp
// Taken from While_Compile

AssertCompile(log =>
    Expression.Block(
        new[] { i },
        CSharpExpression.While(
            Expression.LessThan(i, Expression.Constant(3)),
            Expression.Block(
                log("B"),
                Expression.PostIncrementAssign(i)
            )
        )
    ),
    new LogAndResult<object> { Log = { "B", "B", "B" } }
);
```

This shows the use of the logging facility to ensure branching behavior etc. Overloads of `AssertCompile` allow for writing tests that leverage the log in different ways. For example:

```csharp
// Taken from ForEach_Compile_List4

var i = Expression.Parameter(typeof(int?));
var collection = Expression.Constant(new List<int> { 2, 3, 5 });

AssertCompile((log, append) =>
    CSharpExpression.ForEach(
        i,
        collection,
        Expression.Invoke(append, Expression.Call(Expression.Property(i, "Value"), typeof(int).GetMethod("ToString", Array.Empty<Type>())))
    ),
    new LogAndResult<object> { Log = { "2", "3", "5" } }
);
```

In here, an `append` parameter is supplied which allows for the creation of a call to the `Add` method on the underlying log, such that runtime variables can be logged. This append expression can be assigned to an `Action<string>` making it possible to pass the logging facility into a constructor for instance:

```csharp
// Taken from ForEach_Compile_Pattern1

var i = Expression.Parameter(typeof(int));

AssertCompile((log, append) =>
    CSharpExpression.ForEach(
        i,
        Expression.New(typeof(MyEnumerable1).GetConstructor(new[] { typeof(Action<string>) }), append),
        Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
    ),
    new LogAndResult<object> { Log = { "C", "E", "M", "C", "1", "M", "C", "2", "M", "C", "3", "M" } }
);
```

This code fragment shows a `foreach` loop over an instance of a type that implements the iteration pattern. The constructor of that type, `MyEnumerable1`, takes in an `Action<string>` to write to the log. Members such as `MoveNext` and `Current` use this to perform logging when being invoked. This enables verifying the sequencing of interaction patterns between the reduced code and the object being accessed.

### DebugView and ToCSharp tests

In order to test both the expression factories and the `DebugView` support (see [Debugging](Debugging.MD)), we also generate a series of tests via the `Tests.Meta.tt` file. This file generates a T4 by itself, which ultimately produces a .cs file.

The reason for using two steps is to produce an intermediate text transformation file that can rely on the runtime library to call `DebugView` when emitting test assert code. While this generates tests that should never fail immediately after regenerating them, it does allow to catch regressions in both factory and `DebugView` behavior, because T4 files are not processed unless one makes a change to them from the IDE.

We follow a similar approach to test `ToCSharp` behavior via the `ToCSharp.Meta.tt` file.

## Testing the Roslyn changes

Besides the runtime library, we also have tests for the changes made to the Roslyn compiler in order to support the conversion of lambda expressions containing the newly supported nodes to expression trees.

### Cross-check tests

The majority of tests for Roslyn changes are supplied in the form of cross-check tests which start from a code fragment that represents a lambda expression and asserts it behavior using the following steps:

- Construct a C# string representation of the delegate type compatible with the lambda expression
- Create a code fragment for a class containing two properties, assigning the lambda expression to a delegate type and an expression type
- Compile the generated code fragment using the modified copy of the Roslyn compiler
- Load the resulting assembly and reflect on the class to obtain the value for both properties
- Use `Compile` to compile the expression tree obtained from one of the properties
- Cross-check the outcome of evaluating both delegates

For example, consider the lambda expression `() => 42` of type `Func<int>`. The generated code will look like:

```csharp
class Expressions
{
    public static Func<int> Function => () => 42;
    public static Expression<Func<int>> Expression => () => 42;
}
```

The essence of the test assert is this:

```csharp
assert(Function(), Expression.Compile()())
```

Under the hood, the assert mechanism wraps both delegates into a single delegate which cross-checks outcomes for every invocation. This enables a test case to compile the same lambda expression one time but evaluate it many times, for example to check the behavior of arithmetic expressions with many parameter assignment combinations.

Cross-check tests are both written by hand and generated from a T4 file; both can be found by looking for the `CompilerTest` files. A typical example is shown below:

```csharp
// Taken from CrossCheck_ConditionalAccess_Member1
var f = Compile<Func<string, int?>>("s => Return(s)?.Length");
f("bar");
f(null);
```

The `Compile` method is the helper method that performs the compilation, evaluation, and cross-checking behavior for the specified lambda expression. The returned delegate will perform all asserts, so the test case itself typically doesn't contain any asserts by itself.

Also note the use of the `Return` method. Lambda expressions passed to `Compile` have access to two helper methods: `Return` and `Log`. The former simply evaluates its argument but has the side-effect of writing a textual representation of the argument to a log. The latter takes a `string` and writes to the log. The created log is subject to cross-checking as well and can be used to assert the presence, absence, and order of evaluation of subexpressions.

The T4 generated test cases are simply supplied using an array of lambda expression code fragments in the .tt file.

### Gaps

integration of test suite in the Roslyn fork