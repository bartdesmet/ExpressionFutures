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
