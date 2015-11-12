# C# Expression API

This project contains C#-specific extensions to the `System.Linq.Expressions` API to support C# language constructs that were added after C# 3.0. It only contains the runtime library; the C# compiler changes required to support assignment of lambda expressions containing those language constructs to an `Expression<TDelegate>` is maintained separately.

## API Basics

The top-level namespace of the API is `Microsoft.CSharp.Expressions`. Unless specified otherwise, all references to types in the description below will assume this namespace.

### CSharpExpression

All C#-specific expression tree node types derive from `CSharpExpression` which in turn derives from `System.Linq.Expressions.Expression`. This type also provides factory methods for the various C#-specific expression tree nodes. Users familiar with the LINQ expression tree API should feel familiar with this design immediately.

In order to distinguish different node kinds, the `CSharpExpression` type exposes an instance property `CSharpNodeType` of enumeration type `CSharpExpressionType`. The `NodeType` property derived from `System.Linq.Expressions.Expression` always returns `System.Linq.Expressions.ExpressionType.Extension`.

### Visitors

Analogous to the LINQ expression tree API, an expression visitor is provided using an abstract base class called `CSharpExpressionVisitor`, which is derived from `System.Linq.Expressions.ExpressionVisitor` and overrides `VisitExtension` to dispatch into protected virtual `Visit` methods for each C# node type. It uses the typical dispatch pattern whereby the `CSharpExpression` types expose an `Accept` method.

### Compilation

Support for runtime compilation through the `Compile` methods on `System.Linq.Expressions.LambdaExpression` and `System.Linq.Expressions.Expression<TDelegate>` is provided by making the C#-specific expression tree nodes reducible. The `Reduce` method returns a more primitive LINQ expression tree which the compiler and interpreter in the LINQ API can deal with. By using this implementation strategy, the LINQ APIs don't have to change.

Note that compilation of asynchronous lambda expressions is supported through `Compile` methods on `AsyncLambdaCSharpExpression` and `AsyncCSharpExpression<TDelegate>`. This works around the limitation of LINQ's lambda expression nodes being sealed types. For more information, read on.

Also note that some language constructs such as indexer initializers introduced in C# 6.0 can't be provided as reducible extensions to the LINQ APIs. For more information, read on.

## Supported Language Features

In the following sections, we'll describe the C# language features that are supported in the `Microsoft.CSharp.Expressions` API, excluding those that are already supported by the LINQ expression APIs.

### C# 3.0

#### Multi-dimensional array initializers

One omission in the LINQ expression API is support for multi-dimensional array initializers. An example of this restriction is shown below:

```csharp
Expression<Func<int[,]>> f = () => new int[2, 2] { { 1, 2 }, { 3, 4 } };
```

This will fail to compile with:

```
error CS0838: An expression tree may not contain a multidimensional array initializer
```

Note that the bounds of the array have to be compile-time constants. Therefore, the number of expected elements is known statically.

In order to lift this restriction, we introduced `NewMultidimensionalArrayInitCSharpExpression` which contains a list of expressions denoting the array elements listed in row-major order. The node is parameterized on an array of `Int32` bounds for the array's dimensions. An example of creating an instance of this node type is shown below:

```csharp
CSharpExpression.NewMultidimensionalArrayInit(
  typeof(int), new[] { 2, 2 },
  Expression.Constant(1), Expression.Constant(2),
  Expression.Constant(3), Expression.Constant(4)
);
```

The `GetExpression(int[])` method can be used to retrieve an expression representing an element in the array. This is useful when analyzing an instance of the node type.

Reduction of the node produces a `Block` expression containing a `NewArrayBounds` expression to instantiate an empty multi-dimensional array, followed by a series of `Assign` binary expressions that assign the elements of the array via `ArrayAccess` index expressions. The expressions representing the elements are evaluated in left-to-right row-major order as the language prescribes.

### C# 4.0

#### Named and optional parameters

An example of this restriction is shown below:

```csharp
Expression<Func<int>> f = () => F(2) + F(y: 3, x: 4);
```

This will fail to compile with:

```
error CS0853: An expression tree may not contain a named argument specification
error CS0854: An expression tree may not contain a call or invocation that uses optional arguments
```

This restriction exists for method calls, delegate invocations, object creation, and indexer lookups.

In order to lift this restriction, we introduce C#-specific variants of `Call`, `Invoke`, `New`, and `Index`. Those APIs are completely analogous to the LINQ equivalents, except for the `Arguments` property's type. Rather than storing a collection of `Expression`s to denote the arguments, we store a collection of `ParameterAssignment` objects.

A `ParameterAssignment` object is created through the `Bind` factory method, analogous to the concept of a `MemberAssignment` created via a `Bind` method in LINQ for use in object initializer expressions. It represents the assignment of an `Expression` to an argument represented by a `ParameterInfo`.

Factory methods for `Call`, `Invoke`, `New`, and `Index` have overloads that accept a `ParameterAssignment[]` or `IEnumerable<ParameterAssignment>` in places where LINQ APIs have `Expression`-based equivalents. In addition, the `Expression`-based overloads exist as well (and hide the LINQ APIs methods) which enables the use of optional parameters without having to create parameter assignments.

All factory methods check whether all required parameters are specified exactly once. Parameter assignments perform assignment compatiblity checks and support implicit quotation of expressions assigned to `Expression<TDelegate>` parameters, analogous to the behavior in the LINQ expression API.

An example of using the API to describe calls with named and optional parameters is shown below:

```csharp
Expression.Add(
  CSharpExpression.Call(
    f,
    Expression.Constant(2)
  ),
  CSharpExpression.Call(
    f,
    CSharpExpression.Bind(y, Expression.Constant(3)),
    CSharpExpression.Bind(x, Expression.Constant(4))
  )
)
```

where `f` is a `MethodInfo` describing `F` and `x` and `y` are `ParameterInfo` objects describing the `x` and `y` parameters of `F`.

An overload of `Bind` taking a `MethodInfo` and `string` is provided for the compiler to emit code against given that it's not directly possible to get a `ParameterInfo` object via `ldtoken` instructions.

Reduction of those nodes first analyzes whether all arguments are supplied in the right order, ignoring those that don't have side-effects upon their evaluation (e.g. `Constant`, `Default`, or `Parameter`). In case the order matches, the corresponding LINQ expression node is returned. If the order doesn't match, a `Block` expression is emitted, containing `Assign` binary expressions that evaluate the argument expressions in the specified order and assign them to `Variable` parameter expressions prior to emitting the underlying LINQ expression node.

Each of those expression types supports by-ref parameters and write-backs to `Parameter`, `Index`, `MemberAccess`, `ArrayIndex`, and `Call` (for `ArrayAccess` via the `Array.Get` method) nodes. Strictly speaking this isn't required for C# but those nodes could be promoted to the LINQ APIs in order to be used by VB as well.

Note that none of those expression types are recognized by the LINQ expression API and compiler as assignable targets. If we want to support assignment expressions, we either have to enlighten the LINQ APIs about the assignability of those new node types (by providing an extensibility story for `set` operations against those) or use a C#-specific version of `Assign`. The use of those nodes in by-ref positions is not supported either, but that's less of an issue for C#-specific expressions given that the C# language does not support passing e.g. indexing expressions by reference.

#### Dynamic

Even though a `Dynamic` node type was added in the .NET 4.0 revision of the LINQ expression APIs, the C# language does not support emitting expression trees containing dynamic operations, as illustrated below:

```csharp
Expression<Action<dynamic>> f = x => Console.WriteLine(x);
```

This will fail to compile with:

```
error CS1963: An expression tree may not contain a dynamic operation
```

Various operations can be performed in a late-bound fashion using `dynamic`, including invocations, invocation of members, lookup of members, indexing, object creation, unary operators, binary operators, and conversions.

In order to support C# `dynamic` in expression trees, we have a few options. We could simply piggyback on the DLR's support for the `Dynamic` expression kind, specifying a `CallSiteBinder` argument using the `Microsoft.CSharp.RuntimeBinder.Binder` factories. While this would work to provide runtime compilation support, it leaves the resulting expression tree in a rather cumbersome shape for expression analysis and translation, e.g. in a LINQ provider. One would have to perform type checks against the `CallSiteBinder` in order to reverse engineer the user intent. This itself is hampered by the fact that the C# runtime binder types are marked as `internal` so C#-specific information is not accessible.

Instead of going down the route of emitting calls to the `Expression.Dynamic` factory, we decided to model the dynamic operations as first-class node types in the C# expression API. This improves the experience for expression analysis and translation by retaining the user intent in the shape of the resulting tree. Reduction of those nodes ultimately falls back to emitting `Dynamic` nodes, thus leveraging the C# runtime binder and the DLR.

All of the `dynamic` support is provided through `DynamicCSharpExpression` which derives from `CSharpExpression` and provides factory methods analogous to the LINQ factories but prefixed with `Dynamic`. For example, `Add` becomes `DynamicAdd`. We could flatten the class hierarchy for the factory methods, though the current design could allow for separating out the `dynamic` support in a separate library. When not referenced, the C# compiler would fail resolving the required factory methods, thus making the `dynamic` feature unavailable in expression trees. This opt-in mechanism could come in handy.

Dynamic node types are mirrored after the `Microsoft.CSharp.RuntimeBinder.Binder` factory methods, so we have dynamically bound nodes for `BinaryOperation`, `Convert`, `GetIndex`, `GetMember`, `Invoke`, `InvokeConstructor`, `InvokeMember`, and `UnaryOperation`. Some of these could be revisited in the C# expression API to get nomenclature that aligns better with the existing LINQ expression API's, e.g. using `Call` in lieu of `InvokeMember`.

Some dynamic operations require `Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo` objects to be passed, describing an argument's name and its behavior such as `ref` or `out` usage. This is modeled via a `DynamicCSharpArgument` that can be created via the `DynamicArgument` factory by specifying a `string`, an `Expression`, and a `CSharpArgumentInfoFlags` value. This node allows for inspection of e.g. the name of the argument during expression analysis but ultimately reduces into a `CSharpArgumentInfo` object. Note that the `CSharpArgumentInfoFlags` are exposed in the C# expression API as-is; this type is publicly accessible anyway.

To illustrate this API, consider the most verbose `DynamicInvokeMember` factory methods, shown below:

```csharp
// Instance call
public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Expression @object, string name, IEnumerable<Type> typeArguments, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags, Type context);

// Static call
public static InvokeMemberDynamicCSharpExpression DynamicInvokeMember(Type type, string name, IEnumerable<Type> typeArguments, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags, Type context);
```

Note that a separate method exists for instance and static member invocations. Rather than modeling the receiver type for a static call as a dynamic argument (as done by the C# compiler when emitting a call to for `Binder.InvokeMember`) we decided to hoist this information up to the expression tree node. Effectively, the combination of `type`, `name`, and `typeArguments` is the equivalent of a `MethodInfo` in the statically bound case. This leads to a familiar API for those used to the LINQ expression API for `Call`.

An example of using the API to make a dynamically-bound call is shown below:

```csharp
DynamicCSharpExpression.DynamicInvokeMember(
  typeof(Console),
  "WriteLine",
  x
)
```

Overloads are provided that allow for easy construction of those node types, e.g. as used in the example above. The more verbose equivalent would be:

```csharp
DynamicCSharpExpression.DynamicInvokeMember(
  typeof(Console),
  "WriteLine",
  Array.Empty<Type>(),
  DynamicCSharpExpression.DynamicArgument(
    x,
    null,
    CSharpArgumentInfoFlags.None
  ),
  CSharpBinderFlags.None,
  typeof(Program)
)
```

It goes without saying the the `CSharpExpressionVisitor` allows for visitation of all the nodes types mentioned above.

### C# 5.0

#### Async and Await

Lambda expressions with the `async` modifier and `await` expressions are not supported in expressions. For example, consider the following piece of code:

```csharp
Expression<Func<Task<int>>> f = async () => await Task.FromResult(42);
```

This fails to compile with:

```
error CS1989: Async lambda expressions cannot be converted to expression trees
```

##### Async lambdas

Support for asynchronous lambdas is added by the introduction of an `AsyncLambdaCSharpExpression` type and a `AsyncCSharpExpression<TDelegate>` type derived from it. These are analogous to the `LambdaExpression` and `Expression<TDelegate>` types in the LINQ expression API. Their behavior differs both in terms of typing and in terms of compilation.

From a typing point of view, an asynchronous lambda requires its return type to be `void`, `Task`, or `Task<T>` with the `Body` being assignable to `T` in the latter case. In terms of compilation, the `Compile` methods perform a rewrite similar to the one carried out by the C# compiler, as described further on.

Note that the introduction of custom node types for asynchronous lambdas was required in order to provide proper compilation support and to allow for the specialized type checking due to the return type being lifted over `Task<T>`. Alternatively, we could provide for an extensibility store in the LINQ expression API's `Lambda` nodes or push down the asynchronous lambda support the LINQ expression APIs so that VB can also benefit from it.

##### Await expressions

Await expressions are of type `AwaitCSharpExpression` and support the awaiter pattern as described in the C# language specification. As such, a custom `GetAwaiter` method can be specified on the `Await` factory, including support for extension methods. If left unspecified, the factory will try to find a suitable method. The `GetResult` and `IsCompleted` members on the awaiter type are discovered by the factory as well.

The typing of await expressions follows the awaiter pattern and obtains the return type of the `GetResult` method on the awaiter. This allows those nodes to be composed with any existing LINQ expression nodes.

Await expression nodes are not reducible; the reduction of the closest enclosing async lambda is responsible for its reduction into the await pattern within the generated state machine (see further).

##### Example

Prior to discussing the compilation support for asynchronous lambdas with await expressions, let's have a look at an example:

```csharp
CSharpExpression.AsyncLambda<Func<Task<int>>>(
  CSharpExpression.Await(
    Expression.Call(
      fromResult,
      Expression.Constant(42)
    )
  )
)
```

In addition to the various `AsyncLambda` factory overloads, a single `Lambda` overload with an `isAsync` parameter is provided as well:

```csharp
public static Expression<TDelegate> Lambda<TDelegate>(bool isAsync, Expression body, params ParameterExpression[] parameters);
```

This overload is put in place for use by the C# compiler as a stop-gap measure for assignment compatibility of async lambda expressions to `Expression<TDelegate>`. The returned expression is simply an `Expression<TDelegate>` whose body is an `InvocationExpression` wrapping the underlying C#-specific `AsyncCSharpExpression<TDelegate>`. Unless we have an extensibility store for LINQ's `Expression<TDelegate>` we have to use this unnatural pattern (which does not look good at all in the resulting tree of course) to achieve assignment compatilbity. Alternatively, we have to introduce assignment compatibility with `AsyncCSharpExpression<TDelegate>` which would expand the language specification for lambda expressions and still require changes to the LINQ APIs to support implicit quoting of expression arguments assigned to expression-typed parameters.

