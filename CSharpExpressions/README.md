# C# Expression API

This project contains C#-specific extensions to the `System.Linq.Expressions` API to support C# language constructs that were added after C# 3.0. It only contains the runtime library; the C# compiler changes required to support assignment of lambda expressions containing those language constructs to an `Expression<TDelegate>` is maintained separately in the [ExpressionTrees](https://github.com/bartdesmet/roslyn/tree/ExpressionTrees) branch of my Roslyn fork.

This page describes the C# expression API in much detail. For other topics, see:

- [Project rationale](Docs/Context.MD) for more information about the utility and goals of this project
- [Design principles](Docs/Design.MD) for a discussion about the design of the API
- [Framework stubs](Docs/Stubs.MD) for technical details about accessing certain .NET Framework APIs
- [Testing strategy](Docs/Testing.MD) for details on how the API and Roslyn integration is tested
- [Debugging support](Docs/Debugging.MD) for more information about the `DebugView` property and proxies
- [RoslynPad utility](Docs/RoslynPad.MD) for an introduction to the RoslynPad utility
- [Expression optimizers](Docs/Optimizers.MD) for a discussion about some expression optimizations applied
- [Future directions](Docs/Future.MD) for possible next steps
- [Framework extensions](Docs/BCL.MD) for suggested changes and/or additions to the .NET Framework APIs
- [Miscellaneous thoughts](Docs/Misc.MD) for various blurbs that don't fit anywhere else

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

A `ParameterAssignment` object is created through the `Bind` factory method, analogous to the concept of a `MemberAssignment` created via a `Bind` method in LINQ for use in object initializer expressions. It represents the assignment of an `Expression` argument to a parameter represented by a `ParameterInfo`.

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

An overload of `Bind` taking a `MethodBase` and `int` is provided for the compiler to emit code against given that it's not directly possible to get a `ParameterInfo` object via `ldtoken` instructions. The `int` parameter represents the zero-based parameter index to which the argument is assigned.

Reduction of those nodes first analyzes whether all arguments are supplied in the right order, ignoring those that don't have side-effects upon their evaluation (e.g. `Constant`, `Default`, or `Parameter`). In case the order matches, the corresponding LINQ expression node is returned. If the order doesn't match, a `Block` expression is emitted, containing `Assign` binary expressions that evaluate the argument expressions in the specified order and assign them to `Variable` parameter expressions prior to emitting the underlying LINQ expression node.

Each of those expression types supports by-ref parameters and write-backs to `Parameter`, `Index`, `MemberAccess`, `ArrayIndex`, and `Call` (for `ArrayAccess` via the `Array.Get` method) nodes. Strictly speaking the use of write-backs to e.g. a settable property isn't required for C# but those nodes could be promoted to the LINQ APIs in order to be used by VB as well.

Note that none of those expression types are recognized by the LINQ expression API and compiler as assignable targets. If we want to support assignment expressions, we either have to enlighten the LINQ APIs about the assignability of those new node types (by providing an extensibility story for `set` operations against those) or use a C#-specific version of `Assign`. This library takes the latter route by introducing an `AssignBinaryCSharpExpression` node, which enables assignment to `Index` expressions with named and optional parameters, as discussed later in this document.

Finally, the use of those nodes in by-ref positions is not supported either, but that's less of an issue for C#-specific expressions given that the C# language does not support passing e.g. (non-array) indexing expressions by reference.

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

> Note: The thinking around the opt-in mechanism has changed a bit with the introduction of the [*expression tree like types* prototype](https://github.com/bartdesmet/roslyn/tree/ExpressionTreeLikeTypes) where custom expression builder types can be provided. If we decide to go that route in the C# language and compiler, then opting in or out of dynamic becomes a matter of providing custom expression builder types that have or lack the `Dynamic*` factory methods (or overloads of factory methods without the `Dynamic*` prefix but with an additional "info" object that captures late binding information).

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

Reduction of dynamically bound expressions uses a `Dynamic` node underneath. To specialize the emitted expression for each dynamically bound node type, the `DynamicCSharpExpression` base class provides a `ReduceDynamic` method which returns a `CallSiteBinder` and an `IEnumerable<Expression>` with the expressions to pass to the `Dynamic` node. The returned binders are obtained from the `Microsoft.CSharp.RuntimeBinder.Binder` factory methods.

##### Dynamic assignments

Assignments involving dynamic operands are represented using the `AssignBinaryDynamicCSharpExpression` and `AssignUnaryDynamicCSharpExpression` nodes. Creation of those nodes is supported via factories on `DynamicCSharpExpression` that are dynamic variants of the assignments found in the DLR, e.g. `DynamicAddAssign`. These nodes require assignable left-hand side expressions, which include the C#-specific nodes with `DynamicGetIndex` and `DynamicGetMember` node types, as well as the valid assignments targets in the DLR.

Reduction of dynamic assignments with a dynamically typed left-hand side uses `ReduceAssignment` helper methods which emit `Dynamic` expressions parameterized by `SetMember` and `SetIndex` runtime binders. Dynamic variants of `AddAssign` and `SubtractAssign` will also emit a `Dynamic` node with an `IsEvent` binder in order to check whether the target of the assignment represents in event. If that's the case, a dynamically bound `InvokeMember` operation is carried out to add or remove the event handler specified in the right-hand side.

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

From a typing point of view, an asynchronous lambda requires its return type to be `void`, `Task`, `Task<T>`, or a *task-like type* (using `AsyncMethodBuilderAttribute`, added in C# 7.0, see further down in this document) with the `Body` being assignable to the task result type (i.e. `T` for `Task<T>`, or the parameter type of `SetResult` on the builder type for a *task-like* type). In terms of compilation, the `Compile` methods perform a rewrite similar to the one carried out by the C# compiler, as described further on.

Note that the introduction of custom node types for asynchronous lambdas was required in order to provide proper compilation support and to allow for the specialized type checking due to the return type being lifted over `Task<T>`. Alternatively, we could provide for an extensibility store in the LINQ expression API's `Lambda` nodes or push down the asynchronous lambda support the LINQ expression APIs so that VB can also benefit from it.

##### Await expressions

Await expressions are of type `AwaitCSharpExpression` and support the awaiter pattern as described in the C# language specification. As such, the required binding information for the awaiter pattern is represented through an `AwaitInfo` object constructed by a corresponding factory method. This factory provides various overloads that enable specifying an awaiter type, a custom `GetAwaiter` method (including support for extension methods), or all of the members used for the await pattern, including the `GetResult` and `IsCompleted` members on the awaiter type. The resulting `AwaitInfo` object is passed to the `Await` factory, which also accepts the `Expression` representing the operand of the `await` expression.

The typing of await expressions follows the awaiter pattern and obtains the return type of the `GetResult` method on the awaiter. This allows those nodes to be composed with any existing LINQ expression nodes.

A derived class `DynamicAwaitInfo` with a corresponding `DynamicAwaitInfo` factory method on `DynamicCSharpExpression` is provided to await a dynamically typed operand. An `AwaitCSharpExpression` using `DynamicAwaitInfo` node reduces the required calls to `GetAwaiter`, `IsCompleted`, and `GetResult` using more primitive `DynamicCSharpExpression` nodes. Convenience overloads named `DynamicAwait` are provided for symmetry with other dynamically bound operations (e.g. `DynamicAdd`).

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

This overload is put in place for use by the C# compiler as a stop-gap measure for assignment compatibility of async lambda expressions to `Expression<TDelegate>`. The returned expression is simply an `Expression<TDelegate>` whose body is an `InvocationExpression` wrapping the underlying C#-specific `AsyncCSharpExpression<TDelegate>`. Unless we have an extensibility store for LINQ's `Expression<TDelegate>` we have to use this unnatural pattern (which does not look good at all in the resulting tree of course) to achieve assignment compatibility. Alternatively, we have to introduce assignment compatibility with `AsyncCSharpExpression<TDelegate>` which would expand the language specification for lambda expressions and still require changes to the LINQ APIs to support implicit quoting of expression arguments assigned to expression-typed parameters.

> Note: Enabling the compiler to bind to other factory methods has become a possibility due to the introduction of the [*expression tree like types* prototype](https://github.com/bartdesmet/roslyn/tree/ExpressionTreeLikeTypes). Using this mechanism, a C# expression builder type could be provided, where the `Lambda<TDelegate>` factory returns a `CSharpExpression<TDelegate>` which can either represent a synchronous or an asynchronous lambda expression.

##### Compilation

Compilation of async lambdas through the `Compile` methods is carried out via the `Reduce` method which reduces the node to a classic LINQ `Expression<TDelegate>`. By having the `Reduce` method perform this reduction, an async lambda can occur in a bigger (synchronous) expression tree. Await expressions in async lambdas get reduced using the awaiter pattern.

Prior to carrying out any reduction, we run the expression through a checker that looks for the use of `Await` expressions in forbidden places, e.g. the `Filter` of a `CatchBlock` node. Like many other steps mentioned below, this step proceeds in a shallow way, i.e. it doesn't recurse into nested lambdas of any kind.

The first step in compiling an async lambda is the creation of the appropriate asynchronous method builder from the `System.Runtime.CompilerServices` namespace. The outermost structure of the generated (synchronous) lambda contains the code to instantiate the builder, invoke the `Start` method, and return its `Task` property (if any).

In order to generate an `IAsyncStateMachine` we use a `RuntimeStateMachine` type added to the `System.Runtime.CompilerServices` namespace in our assembly. This class implements the `IAsyncStateMachine` interface's `MoveNext` method by invoking an `Action` delegate supplied to the constructor. All of the state that has to be kept across await boundaries is made part of the closure of the delegate supplied to the constructor. This way, we avoid having to generate a state machine type on the fly, which would require access to a `TypeBuilder`.

Although generating a custom state machine type would be more efficient (e.g. by using a struct and implementing `SetStateMachine` as well), it would introduce non-trivial complexity, require access to the `TypeBuilder` which is encapsulated in the LINQ lambda compiler, and prevent the same code from working with the expression interpreter as well. Note that the lambda compiler doesn't generate a full-blown display class for closures either (though it could).

The next step in the compilation of an async lambda is the rewrite of the `Body` expression. This proceeds in a few steps. First, we reduce all nodes in the `Body` except for the `Await` nodes. By doing so, we only have to worry about LINQ nodes in the steps that follow. Second, we lower various constructs to a more primitive form in order to support their use in asynchronous lambdas. Those include `Try` expressions with `Await` expressions in any of the `Handlers` or `Finally` or `Fault` expressions. Third, we perform stack spilling in order to be able to await while having intermediate expression evaluation results on the stack. Finally, we transform the resulting lowered `Body` by rewriting all `Await` expressions into the awaiter pattern using a state machine. Some other manipulations of the expression are deemed implementation details and are omitted from this description.

#### C# 6.0

##### Conditional Access Expressions

Conditional access expressions, also known as "null-propagating operators", are not supported in expression trees as shown below:

```csharp
Expression<Func<string, int?>> f = s => s?.Length;
```

This fails to compile with:

```
error CS8072: An expression tree lambda may not contain a null propagating operator.
```

Null-propagation is supported for method invocation, member access, and indexing operations in C#. Delegate invocation using null-propagation is not supported due to syntactic ambiguities by can be achieved by using method invocation syntax to call `Invoke`.

Note that null-propagating operators have a right associative nature in C#, an example of which is shown below:

```csharp
Func<DateTimeOffset?, int> f = dto => dto?.Offset.Hours;
```

This cannot be rewritten by inserting left-most parentheses, e.g.:

```csharp
Func<DateTimeOffset?, int> f = dto => (dto?.Offset).Hours;   // `TimeSpan?` doesn't have a property `Hours`
```

Therefore we need to support 'chains' of operations (i.e. `.Offset.Hours` in the example above) that are conditionally accessed based on whether the receiver is null (i.e. `dto?` in the example above).

In order to support null-propagating operators, we introduce a `ConditionalAccessCSharpExpression` which has three properties. The first property, `Receiver`, contains an `Expression` describing the conditionally accessed receiver which should have a reference type or a nullable value type. The `NonNullReceiver` property is of type `ConditionalReceiver` and provides an object that can be referred to within `WhenNotNull` which is an `Expression` denoting the conditionally performed operation(s).

The `ConditionalAccessCSharpExpression` is derived from `ConditionalAccessCSharpExpression<TExpression>` which allows for strong typing of the `WhenNotNull` to a more derived expression type. This is merely a convenience enabling factory methods to create commonly used conditional access nodes which involve only a single operation. Nesting these effectively results in a left-associative chain of conditional access operations. The general-purpose `ConditionalAccessCSharpExpression` node closes `TExpression` over `Expression`.

An example of using a convenience factory is shown below:

```csharp
CSharpExpression.ConditionalProperty(
  s,
  typeof(string).GetProperty("Length")
)
```

This factory call returns a `ConditionalMemberCSharpExpression` which inherits from `ConditionalAccessCSharpExpression<MemberExpression>`. This is merely shorthand for the creation of a `ConditionalReceiver` and a `ConditionalAccess` as shown below:

```csharp
var sNotNull = CSharpExpression.ConditionalReceiver(typeof(string));
var expr = CSharpExpression.ConditionalAccess(
  s,
  sNotNull,
  Expression.Property(
    sNotNull,
    typeof(string).GetProperty("Length")
  )
);
```

The reduction of null-propagating operators emits a `Block` expression with an `Assign` binary expression to assign the result of evaluating the `Receiver` to a `Variable` parameter expression. Next, an `IfThen` conditional expression is used to check the variable against a null value. If the variable is not null, the underlying operation is invoked by substituting references to `NonNullReceiver` in `WhenNotNull` for the created variable. The result of the underlying operation is lifted to null using a `Convert` unary expression if necessary.

##### Indexer Initializers

Indexer (or dictionary) initializers were introduced as an addition to object initializer expressions that were introduced in C# 3.0. They are not supported in expression trees:

```csharp
Expression<Func<Dictionary<string, int>>> f = () => new Dictionary<string, int> { ["Bart"] = 21 };
```

This fails to compile with:

```
error CS8074: An expression tree lambda may not contain a dictionary initializer.
```

Our options to support this are limited given the non-extensible nature of `MemberBinding` nodes in the LINQ expression API. We'd need proper support for reduction of custom binding nodes in order to provide an extension to set of bindings supported by the `MemberInitExpression` node.

To work around this limitation and get a glimpse of what it may look like, we've hijacked `ElementInit` and tricked it into calling a specified indexer's `set` method. Unfortunately, `ElementInit` is only supported in `ListInitExpression` nodes and `MemberListBinding` nodes so mileage is limited.

An example of this hack is shown below:

```csharp
Expression.ListInit(
  Expression.New(
    typeof(Dictionary<string, int>).GetConstructor(Array.Empty<Type>())
  ),
  CSharpExpression.IndexInit(
    typeof(Dictionary<string, int>).GetProperty("Item"),
    Expression.Constant("Bart"),
    Expression.Constant(21)
  )
)
```

In order to make this work for real, we need extension support for the `MemberBinding` hierarchy of types. This would likely involve a `Reduce` capability that's usable by the expression compiler to emit write operations against the initialized object. This type of reduction is a bit different from the one on `Expression` given that it wouldn't reduce into a node of its own kind, i.e. another `MemberBinding`.

> Note: Enabling the compiler to bind to other factory methods has become a possibility due to the introduction of the [*expression tree like types* prototype](https://github.com/bartdesmet/roslyn/tree/ExpressionTreeLikeTypes). Using this mechanism, we could provide a C# expression builder type with an alternative `MemberInit` factory method that uses a C#-specific `MemberBinding` class hierarchy including support for indexer initializers.


##### Await in Catch and Finally

Our support for `Await` in an `AsyncLambda` (cf. the C# 5.0 section above) includes support for using `Await` in the `Handlers` and/or `Finally` portions of a `Try` expression.

The compilation of such constructs is based on a lowering step where we translate the `Try` expression into a more primitive form with catch and rethrow constructs using `ExceptionDispatchInfo`. We also support pending branches out of the `Body` of a `Try` expression while ensuring the timely execution of the `Finally` handler, if any.

All of this is very similar to the C# compiler approach of supporting `await` in `catch` and `finally` blocks. Two notable differences are our support for `Await` in a `Fault` handler and our support for non-void `Try` expressions which are permitted by the DLR.

##### Interpolated Strings

Right now, interpolated strings are being lowered to `string.Format` invocations in expression trees. This has the drawback that any library that wishes to translate a string interpolation needs to reverse engineer a format string literal. When referencing `Microsoft.CSharp.Expressions`, we capture interpolated strings in a non-lowered form as an `InterpolatedStringCSharpExpression`, whose `Reduce` method will produce a `MethodCallExpression` for the equivalent `string.Format` call.

For example:

```csharp
Expression<Func<string, int, string>> e = (name, age) => $"{name} is {age} years old.";
```

traditionally gets translated as:

```csharp
Expression.Call(
  string_format_method,
  Expression.Constant("{0} is {1} years old."),
  name,
  Expression.Convert(age, typeof(object))
)
```

where the chosen overload of `string.Format` is decided by binding steps in the C# compiler. When referencing `Microsoft.CSharp.Expressions`, the expression is translated as:

```csharp
CSharpExpression.InterpolatedString(
  typeof(string),
  CSharpExpression.InterpolationStringInsert(name, null, null),
  CSharpExpression.InterpolationStringLiteral(" is "),
  CSharpExpression.InterpolationStringInsert(age, null, null),
  CSharpExpression.InterpolationStringLiteral(" years old.")
)
```

where `InterpolationStringInsert` takes three parameters, one for the expression to interpolate, followed by an optional format string and alignment specifier (of type `int?`). After reduction of the `InterpolatedStringCSharpExpression`, the equivalent `MethodCallExpression` for `string.Format` call is generated.

Interpolated strings also support conversion to `FormattableString` or `IFormattable`, causing lowering to `FormattableStringFactory.Create` invocations in expression trees. This has similar issues as the `string.Format` lowering, so we support capturing the interpolated string as a `InterpolatedStringCSharpExpression` whose `Type` property is `FormattableString` of `IFormattable`.

For example:

```csharp
Expression<Func<string, int, FormattableString>> e = (name, age) => $"{name} is {age} years old.";
```

traditionally gets translated as:

```csharp
Expression.Call(
  formattable_string_factory_create_method,
  Expression.Constant("{0} is {1} years old."),
  Expression.NewArrayInit(
    typeof(object),
    name,
    Expression.Convert(age, typeof(object))
  )
)
```

When referencing `Microsoft.CSharp.Expressions`, the expression is translated as:

```csharp
CSharpExpression.InterpolatedString(
  typeof(FormattableString),
  CSharpExpression.InterpolationStringInsert(name, null, null),
  CSharpExpression.InterpolationStringLiteral(" is "),
  CSharpExpression.InterpolationStringInsert(age, null, null),
  CSharpExpression.InterpolationStringLiteral(" years old.")
)
```

where the first argument indicates the converted type. Reduction of the expression results in a `MethodCallExpression` for the `FormattableStringFactory.Create` method.

#### C# 7.0

##### Throw Expressions

Throw expressions are not supported in expression trees as shown below:

```csharp
Expression<Func<string, string>> f = o => o ?? new Exception();
```

This fails to compile with:

```
error CS8188: An expression tree may not contain a throw-expression.
```

Support for throw expressions is added by using the `Expression.Throw(Expression, Type)` factory method that already exists in the BCL:

```csharp
Expression.Throw(
  expression,
  typeof(string)
)
```

##### Discard Expressions

Discard expressions are not supported in expression trees as shown below:

```csharp
Expression<Func<string, bool>> f = s => int.TryParse(s, out int _);
```

This fails to compile with:

```
error CS8207: An expression tree may not contain a discard.
```

Support for discard expressions is added by a new `CSharpExpression.Discard(Type)` method:

```csharp
CSharpExpression.Discard(typeof(int))
```

Nodes of this type reduce to a valid assignment target using a `Discard<T>` helper type. Optimizers can prevent this reduction by removing assignments or by introducing temporary locals in blocks.

##### Generalized Async Return Types

Task-like return types on async lambdas are supported using the `AsyncMethodBuilderAttribute` type to select a builder type. For example, this enables the use of `ValueTask<T>` as the return type of an async lambda:

```csharp
Expression<Func<Task<int>, ValueTask<int>>> f = t => 2 * await t;
```

Upon reduction of the `AsyncLambdaCSharpExpression` node, the async state machine is built using the custom builder type's `Create`, `Start`, `AwaitOnCompleted`, `SetResult`, and `SetException` methods. The task-like object returned is obtained using the builder's `Task` property.

##### Tuples

Tuple literals are not supported in expression trees as shown below:

```csharp
Expression<Func<(int, int)>> f = () => (x: 1, y: 2);
```

This fails to compile with:

```
error CS8143: An expression tree may not contain a tuple literal.
```

Support for tuple literal expressions is added by a new `CSharpExpression.TupleLiteral(Type, IEnumerable<Expression>, IEnumerable<string>)` method:

```csharp
CSharpExpression.TupleLiteral(typeof(ValueTuple<int, int>), new[] { Expression.Constant(1), Expression.Constant(2) }, new[] { "x", "y" })
```

When names are specified for the tuple fields, those are made available in `ArgumentNames`. The factory method accepts these optional tuple field names as the last parameter, mirroring the `New` factory in `System.Linq.Expressions` when used to represent the instantiation of an anonymous type (where the assigned members as specified in the last parameter of type `IEnumerable<MemberInfo>`).

Nodes of this type reduce to a `NewExpression`, or a nesting of `NewExpression` nodes for tuples with more than 7 components.

Tuple conversions are not supported in expression trees as shown below:

```csharp
Expression<Func<(int, DateTime), (long, DateTimeOffset)>> f = t => t;
```

This fails to compile with:

```
error CS8144: An expression tree may not contain a tuple conversion.
```

Support for tuple conversion expressions is added by a new `CSharpExpression.TupleConvert(Expression, Type, IEnumerable<LambdaExpression>)` method:

```csharp
var p1 = Expression.Parameter(typeof(int));
var convert1 = Expression.Lambda<Func<int, long>>(Expression.Convert(p1), p1);
var p2 = Expression.Parameter(typeof(DateTime));
var convert2 = Expression.Lambda<Func<DateTime, DateTimeOffset>>(Expression.Convert(p2), p2);
CSharpExpression.TupleConvert(t, typeof(ValueTuple<long, long>), new[] { convert1, convert2 })
```

The lambda expressions array passed to `TupleConvert` represents the conversions for the tuple elements. Similar to `UnaryExpression` nodes with a `Convert` node type, this node supports lifted operations. That is, the following conversions are valid:

* `T` to `U`
* `T?` to `U`
* `T` to `U?`
* `T?` to `U?`

where `T` and `U` are tuple types. If `T` and `U` are the same, identity functions can be used for the element conversion lambda expressions. Alternatively, `Expression.Convert` can be used for conversions between `T` and `T?`.

Nodes of this type reduce to a `TupleLiteralCSharpExpression` that constructs a value of the target tuple type, using conversions applied to elements extracted from the tuple operand.

##### Deconstructing Assignment

Deconstructing assignment is not supported in expression trees as shown below:

```csharp
Expression<Action<(int, int), int, int>> e = (t, x, y) => (x, y) = t;
```

This fails to compile with:

```
error CS8143: An expression tree may not contain a tuple literal.
```

while highlighting the `(x, y)` literal on the left hand side of the assignment.

Support for deconstruction assignment is added by a new `CSharpExpression.DeconstructionAssignment(Type type, TupleLiteralCSharpExpression left, Expression right, DeconstructionConversion conversion)` method:

```csharp
var lhs = CSharpExpression.TupleLiteral(typeof(ValueTuple<int, int>), new[] { x, y }, argumentNames: null);
var p1 = Expression.Parameter(typeof(int));
var convert1 = CSharpExpression.Convert(Expression.Lambda(p1, p1));
var p2 = Expression.Parameter(typeof(int));
var convert2 = CSharpExpression.Convert(Expression.Lambda(p2, p2));
var convert = CSharpExpression.Deconstruct(convert1, convert2);
CSharpExpression.DeconstructionAssignment(typeof(ValueTuple<int, int>), lhs, t, convert)
```

The first parameter of type `Type` represents the result of the assignment, which is a `ValueTuple<int, int>`. This is similar to every other assignment in C# which produces a result (that may get discarded in an expression statement).

The second parameter represents the left-hand side of the assignment, which is a `TupleLiteral` that can have arbitrary levels of nesting. The components of these tuples are the variables to assign to, where every assignable node is supported.

The third parameter represents the right-hand side of the assignment, which can be any expression with a type that's deconstructible, e.g. tuples or types with a `Deconstruct` method (such as record types).

The fourth and final parameter represents the conversions to apply to the components obtained by recursively deconstructing the right-hand side according to the shape of the tuple literal on the left-hand side.

In the example shown above, the expression being deconstructed is of an `(int, int)` tuple type, where the components get assigned to variables `x` and `y` in the `(x, y)` tuple literal on the left. Trivial identity conversions are applied to these components, represented using `CSharpExpression.Convert(LambdaExpression)`:

```csharp
var px = Expression.Parameter(typeof(int));
CSharpExpression.Convert(Expression.Lambda(px, px))
```

If the left-hand side of the assignment in the sample were to contain, for example, a variable of type `long`:

```csharp
(long x, int y) = t;
```

then the resulting conversion for component `x` would involve an `Expression.Convert` node:

```csharp
var px = Expression.Parameter(typeof(int));
CSharpExpression.Convert(Expression.Lambda(Expression.Convert(px, typeof(long)), px))
```

The `CSharpExpression.Convert` method returns a node of type `SimpleConversion` which derives from the `Conversion` base class. To combine different `Conversion` instances to apply to the respective components of a deconstruction step, a `DeconstructionConversion` is used:

```csharp
var px = Expression.Parameter(typeof(int));
var convertX = CSharpExpression.Convert(Expression.Lambda(px, px));
var py = Expression.Parameter(typeof(int));
var convertY = CSharpExpression.Convert(Expression.Lambda(py, py));
CSharpExpression.Deconstruct(convertX, convertY)
```

The `CSharpExpression.Deconstruct` method has several overloads to support deconstructing tuples or types with a `Deconstruct` method. In case of tuples, the `Deconstruct(params Conversion[] conversions)` overload is used, where the `Conversion[]` array contains the conversiond applied to the components of the tuple.

For types with a custom `Deconstruct` method, the `Deconstruct(LambdaExpression deconstruct, params Conversion[] conversions)` overload is used where the `deconstruct` parameter represents the invocation of the `Deconstruct` method. For example, consider the following example:

```csharp
Expression<Action<Point>> e = p =>
{
  var (x, y) = p;
  Console.WriteLine($"({x},{y})");
};
```

where `Point` is a type with a `void Deconstruct(out int x, out int y)` deconstruction instance method. (Note that extension methods are also supported.) The deconstruction assignment is translated to:

```csharp
// Left hand side.
var x = Expression.Parameter(typeof(int), x);
var y = Expression.Parameter(typeof(int), y);
var lhs = CSharpExpression.TupleLiteral(typeof(ValueTuple<int, int>), new[] { x, y }, argumentNames: null);

// Deconstruct
var obj = Expression.Parameter(typeof(Point));
var out1 = Expression.Parameter(typeof(int).MakeByRefType());
var out2 = Expression.Parameter(typeof(int).MakeByRefType());
var deconstruct = CSharpExpression.DeconstructLambda(Expression.Call(obj, /* methodinfoof(Point.Deconstruct) */, out1, out2));

// Element conversions
var p1 = Expression.Parameter(typeof(int));
var convert1 = CSharpExpression.Convert(Expression.Lambda(p1, p1));
var p2 = Expression.Parameter(typeof(int));
var convert2 = CSharpExpression.Convert(Expression.Lambda(p2, p2));

// Conversion
var convert = CSharpExpression.Deconstruct(deconstruct, convert1, convert2);

// Assignment
CSharpExpression.DeconstructionAssignment(typeof(ValueTuple<int, int>), lhs, p, convert)
```

In here, the `DeconstructLambda` is a specialized factory for `LambdaExpression` nodes where the return type is `void` and all input parameters are passed by reference. This corresponds to a snippet to invoke the `Deconstruct` method, like this:

```csharp
DeconstructAction<Point, int, int> deconstruct = (Point p, ref int x, ref int y) => p.Deconstruct(ref x, ref y);
```

The factory uses `DeconstructAction<TInput, TOutput1, ..., TOutputN>` delegate types for arities 2 through 16 in order to avoid runtime code generation of custom delegate types.

Upon reduction of the `DeconstructionAssignmentCSharpExpression` node, any deconstruction lambdas representing an invocation to a `Deconstruct` method get inlined.

Finally, note that deconstruction assignment is also supported in `foreach` loops using a `ForEachCSharpStatement.Deconstruction` property that represents the deconstruction step applied to an element of the collection enumeration in order to assign the loop variables in `ForEachCSharpStatement.Variables`.

##### Pattern Matching

The `is` pattern matching operator is not supported in expression trees as shown below:

```csharp
Expression<Func<object, bool>> f = o => o is int x;
```

This fails to compile with:

```
error CS8122: An expression tree may not contain an 'is' pattern-matching operator.
```

Support for `is` pattern matching is added by a new `CSharpExpression.IsPattern(Expression, CSharpPattern)` method, where `CSharpPattern` is the base class for patterns. In the sample above, a `DeclarationCSharpPattern` will be constructed to represent the `int x` pattern:

```csharp
var p = Expression.Parameter(typeof(object));
var x = Expression.Parameter(typeof(int), "x");
var declaration = CSharpPattern.Declaration(x);
CSharpExpression.IsPattern(p, declaration)
```

Other supported C# 7.0 pattern types are:

* `ConstantCSharpPattern` which is parameterized on a `ConstantExpression` to check for a constant, and,
* `VarCSharpPattern` for patterns that always match and introduce a new local variable.

For example,

```csharp
o is 42
```

is translated into

```csharp
CSharpExpression.IsPattern(o, CSharpPattern.Constant(Expression.Constant(42)))
```

and

```csharp
o is var x
```

is translated into

```csharp
CSharpExpression.IsPattern(o, CSharpPattern.Var(Expression.Parameter(x, o.Type)))
```

More pattern types were introduced in subsequent versions of C# and are covered further down in this document.

##### Local Functions

We do not add support to call local functions from an expression tree lambda, nor to declare a local function within an expression tree lambda. For example:

```csharp
void F() {}
Expression<Action> a = () => F();
```

fails to compile with

```
error CS8110: An expression tree may not contain a reference to a local function.
```

Merely defining a local function with an expression tree lambda, as in

```csharp
Expression<Action> a = () =>
{
  void F() {}
};
```

will cause the local function declaration to be erased in the resulting expression tree as an `Expression.Empty()` node.

Adding support for local functions has several pitfalls:

* Invoking a local function defined outside an expression tree would require the closure over local variables (if the local function is not `static`, as introduced in C# 8.0) to be created as a class (rather than a struct). The expression tree would then reference this closure object using a `ConstantExpression` node, used for the receiver of the `MethodCallExpression` targeting the synthesized local function method.
* Defining a local function within an expression tree may result in a closure over variables declared within the expression tree lambda, so the whole definition needs to be quoted within the expression tree. The naive approach would be to turn these into nested expression tree lambdas, but there are several complexities:
  - Local functions become delegate-typed variables that need to be declared and assigned at the beginning of the block (because local functions can be defined anywhere; they're effectively statement nodes). We'd likely have to keep them on nodes such as `BlockCSharpExpression`, and use the reduction of such nodes to turn them into local variables and assignment statements.
  - Local functions can be generic. Expression trees can't be used to define open generic delegates and expression tree factory methods will guard against `System.Type` instances that represent open generics or generic type parameters. One way around this is to chase down all instantiations of the local function and specialize for all of them, resulting in duplication of the code. Or, we could reject generic local functions.

#### C# 7.1

No new features were added that impact expression trees.

#### C# 7.2

##### Non-trailing Named Arguments

TODO - Pending verification.

#### C# 7.3

##### Tuples equality and inequality

Tuple equality and inequality expressions are not supported in expression trees as shown below:

```csharp
Expression<Func<(int, DateTime), (int, DateTime), bool>> f = (t1, t2) => t1 == t2;
```

This fails to compile with:

```
error CS8382: An expression tree may not contain a tuple == or != operator.
```

Support for tuple equality and inequality expressions is added by new `CSharpExpression.TupleEqual(Expression, Expression, IEnumerable<LambdaExpression>)` and `CSharpExpression.TupleNotEqual(Expression, Expression, IEnumerable<LambdaExpression>)` methods:

```csharp
var p1 = Expression.Parameter(typeof(int));
var p2 = Expression.Parameter(typeof(int));
var check1 = Expression.Lambda<Func<int, long>>(Expression.Equal(p1, p2), p1, p2);
var p3 = Expression.Parameter(typeof(DateTime));
var p4 = Expression.Parameter(typeof(DateTime));
var check2 = Expression.Lambda<Func<DateTime, DateTimeOffset>>(Expression.Equal(p3, p4), p3, pt4);
CSharpExpression.TupleEqual(t1, t2, new LambdaExpression[] { check1, check2 })
```

The lambda expressions array passed to `TupleEqual` or `TupleNotEqual` represents the equality checks for the tuple elements. Similar to `BinaryExpression` nodes with an `Equal` or `NotEqual` node type, this node supports lifted operations where equality tests are performed on nullable tuple types.

Nodes of this type reduce to a logical conjunction or disjunction using `AndAlso` (in case of `TupleEqual`) or `OrElse` (in case of `TupleNotEqual`) expressions to combine equality checks applied to elements extracted from the tuple operands, while guaranteeing a left-to-right evaluation of side-effects.

#### C# 8.0

##### Null coalescing Assignment

Null coalescing assignment expressions are not supported in expression trees as shown below:

```csharp
Expression<Func<string, string>> f = s => s ??= "foo";
```

This fails to compile with:

```
error CS8642: An expression tree may not contain a null coalescing assignment
```

Support for null coalescing assignment expressions is added by a new `CSharpExpression.NullCoalescingAssignment(Expression, Expression)` method:

```csharp
CSharpExpression.NullCoalescingAssignment(s, Expression.Constant("foo"))
```

Support for `dynamic` is provided through `DynamicCSharpExpression.NullCoalescingAssignment` methods.

##### Recursive pattern matching

Support for positional and property patterns was added by extending the `CSharpPattern` class hierarchy to support:

* `DiscardCSharpPattern` to represent a `_` pattern,
* `RecursiveCSharpPattern` to support positional and property patterns, and,
* `ITupleCSharpPattern` to support positional property patterns using `ITuple`.

Recursive patterns use subpatterns of type `PositionalCSharpSubpattern` and `PropertyCSharpSubpattern` to represent the recursive application of patterns.

More pattern types were introduced in subsequent versions of C# and are covered further down in this document.

##### Switch Expressions

TODO - Implementation pending.

##### Using Declarations

TODO - Document implementation.

##### Indices and Ranges

Construction of indexes using the `^` expression is supported using a `FromEndIndexCSharpExpression` node type. For example:

```csharp
Expression<Func<int, Index>> f = i => ^i;
```

is translated into:

```csharp
CSharpExpression.FromEndIndex(i, /* methodinfoof(System.Index..ctor(int, bool)) */, typeof(Index))
```

Lifting to a nullable `Index?` type is supported as well.

Construction of ranges using the `..` expression is supported using a `RangeCSharpExpression` node type. For example:

```csharp
Expression<Func<Index, Index, Range>> f = (i, j) => i..j;
```

is translated into:

```csharp
CSharpExpression.Range(i, j, /* methodinfoof(System.Range..ctor(Index, Index)) */, typeof(Range))
```

Other variants such as `..`, `i..`, and `..j` are represented by omitting the left and/or right operands, and using a `MethodInfo` parameter representing either `Range.All`'s get method, `Range.FromStart`, or `Range.FromEnd`. Lifting to a nullable `Range?` type is supported as well.

Indexing and slicing operations for arrays are supported using an `ArrayAccessCSharpExpression` node type. For example:

```csharp
Expression<Func<int[], Index, int>> f = (xs, i) => xs[i];
```

is translated into:

```csharp
CSharpExpression.ArrayAccess(xs, i)
```

Note that `ArrayAccess` indexing using `int` or `Index` types supports assignment using the `AssignBinaryCSharpExpression` and `AssignUnaryCSharpExpression` nodes, as well as being passed as a `ref` parameter using the C# nodes for `Call`, `Invoke`, and `New`.

Slicing using a `Range` value is supported as well. For example:

```csharp
Expression<Func<int[], Range, int[]>> f = (xs, r) => xs[r];
```

is translated into:

```csharp
CSharpExpression.ArrayAccess(xs, r)
```

Indexing and slicing operations for other types (including `string`) are supported using an `IndexerAccessCSharpExpression` node type. For example:

```csharp
Expression<Func<string, Index, char>> f = (s, i) => s[i];
```

is translated into:

```csharp
CSharpExpression.IndexerAccess(s, i, /* methodinfoof(System.String.get_Length) */, /* methodinfoof(System.String.get_Chars) */)
```

where the `MethodInfo` parameters represent the `get` accessor of the `Length` or `Count` property, and the `get` accessor of the indexer. When used for a slicing operation, the second `MethodInfo` parameter represents the `Slice` method (or, in case of `string`, the `Substring` method). For example:

```csharp
Expression<Func<string, Range, string>> f = (s, r) => s[r];
```

is translated into:

```csharp
CSharpExpression.IndexerAccess(s, i, /* methodinfoof(System.String.get_Length) */, /* methodinfoof(System.String.Substring) */)
```

##### `await using`

TODO - Fully implemented; add documentation.

##### `await foreach`

TODO - Fully implemented; add documentation.

#### C# 9.0

##### Pattern matching enhancements

Support for relational, `and`, `or`, and `not` patterns was added by extending the `CSharpPattern` class hierarchy to support:

* `BinaryCSharpPattern` with kinds `CSharpPatternType.And` and `CSharpPatternType.Or` to support `and` and `or`,
* `NotCSharpPattern` to support `not`, and,
* `RelationalCSharpPattern` to support relational patterns using `<`, `<=`, `>`, or `>=`.

#### C# 10.0

##### Extended property patterns

Support for extended property patterns was added by supporting a chain of member lookups in `PropertyCSharpSubpatternMember`.

#### C# 11.0

##### List patterns

TODO - Fully implemented; add documentation.

#### Statement Trees

Statements have existed in C# since day zero, but have never been supported in expression trees. With the DLR refresh of the LINQ expression API in .NET 4.0, various statement constructs have been modeled, including `Block`, `Try`, `Loop`, etc. The C# compiler has not been updated to support emitting expression trees containing those, as illustrated below:

```csharp
Expression<Action> f = () => {};
```

This fails to compile with:

```
error CS0834: A lambda expression with a statement body cannot be converted to an expression tree
```

Various constructs are properly supported by the DLR nodes already, e.g. `Block`, `Try`, `IfThenElse`, and `Switch`. Note that the example shown above may require some tweaks to those nodes, e.g. supporting a `Block` with no statements inside of it.

However, some node types are not supported directly or too primitive to compile to. In particular, `Loop` is not a great compilation target for `while`, `do`, `for`, and `foreach`. Similarly, `Try` is not a great compilation target for `using`. Instead, we like to model those C# statements as their own nodes.

Each statement node derives from `CSharpStatement` which ensures that the `Type` of the expression is reported as `void`. It does so by overriding the `Reduce` method and delegating the reduction step to a `ReduceCore` method, wrapping its result in a `void`-typed `Block` expression if needed.

Note that factories for statements are exposed on the `CSharpExpression` class. Alternatively, we could move those to the `CSharpStatement` class in order to provide for an opt-in mechanism by moving the statement node implementations to a separate assembly (similar to the approach taken for `dynamic`).

##### Loops

We provide support for various loop constructs, including `while`, `do`, `for`, and `foreach`. Each of these nodes derives from `LoopCSharpStatement` which exposes `Body`, `BreakLabel`, and `ContinueLabel` properties. The loops that rely on a condition are derived from a `ConditionalLoopCSharpStatement` base class which exposes a `Test` property holding the expression representing the loop condition.

###### While

The `WhileCSharpStatement` node represents a `while` conditional loop deriving from `ConditionalLoopCSharpStatement`. It reduces into a `Loop` expression with an `IfThen` expression inside to check for loop termination conditions.

An example of creating a `While` statement is shown below:

```csharp
CSharpStatement.While(
  Expression.LessThan(i, Expression.Constant(10)),
  Expression.Block(
    Expression.Call(writeLine, i),
    Expression.PostIncrementAssign(i)
  )
)
```

###### Do

The `DoCSharpStatement` node represents a `do` conditional loop deriving from `ConditionalLoopCSharpStatement`. It reduces into a `Block` expression holding the `Body` of the loop, an `IfThen` check for the loop termination condition, and various `Label` expressions denoting the `break` and `continue` labels.

An example of creating a `Do` statement is shown below:

```csharp
CSharpStatement.Do(
  Expression.Block(
    Expression.Call(writeLine, i),
    Expression.PostIncrementAssign(i)
  ),
  Expression.LessThan(i, Expression.Constant(10))
)
```

###### For

The `ForCSharpStatement` node represents a `for` loop deriving from `ConditionalLoopCSharpStatement`. It reduces into a `Block` expression performing the initialization steps, executing the `Body` of the loop, checking for the loop termination condition, executing the iterators, and various `Label` expressions denoting the `break` and `continue` labels.

To create a `For` loop node, one specifies zero or more initializers, an optional `Boolean` loop termination condition, and zero or more iterator expressions. The initializers are modeled as `Binary` expression nodes whose kind should be `Assign` and whose `Left` property should be of the `Parameter` kind.

An example of creating a `For` expression is shown below:

```csharp
CSharpExpression.For(
  new[] { Expression.Assign(x, Expression.Constant(0)) },
  Expression.LessThan(i, Expression.Constant(10)),
  new[] { Expression.PostIncrementAssign(i) },
  Expression.Call(writeLine, i)
)
```

###### ForEach

The `ForEachLoopCSharpStatement` node represents a `foreach` loop deriving from `LoopCSharpStatement`. It supports the enumerator pattern as specified in the C# language specification. Currently, the factory methods perform lookups for the `GetEnumerator`, `MoveNext`, `Current`, and `Dispose` members conform the iterator pattern. Overloads could be specified that specify those members using reflection objects.

Reduction of a `ForEach` node uses a strategy similar to the C# compiler's. Special cases are provided for enumeration over arrays, strings, or objects implementing `IEnumerable` or `IEnumerable<T>`. The general case supports any enumerator pattern.

Conform the C# specification, the reduced expression emits a call to `IDisposable.Dispose` if the enumerator implements `IDisposable`. It avoids boxing by calling the method implementing `IDisposable.Dispose` in case the enumerator is a value type. It also supports a conversion of the `Current` property of the enumerator to the type of the specified loop variable.

An example of creating a `ForEach` statement is shown below:

```csharp
CSharpStatement.ForEach(
  x,
  Expression.Constant(new[] { 1, 2, 3 }),
  Expression.Call(writeLine, x)
)
```

where `x` is a `ParameterExpression` of type `int`. Overloads are provided which allow the specification of `break` and `continue` labels.

Note that the factory methods don't check for assignment to the iteration variable or passing the iteration variable in a `ref` or `out` parameter. Doing so would require a visit of the specified `Body`, which could lead to the early reduction of `Extension` nodes (too eager) or skip those nodes to avoid the reduction (too shallow). We could still add such a check to the `Reduce` code path where reduction of `Extension` nodes is carried out anyway.

##### Using

The `UsingCSharpStatement` node represents a `using` statement. It contains a `Variable` which is a `Parameter` node whose type should be assignment compatible with `IDisposable`. The `Resource` property holds the expression representing the resource to dispose after the execution of the `Body`.

Reduction of a `Using` node consists of a `Block` evaluating the `Resource` and assigning it to the `Variable`, followed by a `TryFinally` expression which executes the `Body` in the protected region and disposes the resource held in the `Variable` (after an `IfThen` null-check for non-value types) in the finally handler. Boxing of the resource held in the `Variable` is avoided in case its type is a value type.

An example of creating a `Using` statement is shown below:

```csharp
CSharpStatement.Using(
  fs,
  Expression.Call(fileOpen, "foo.txt"),
  Expression.Call(printAllLines, fs)
)
```

where `fs` is a `ParameterExpression` of type `FileStream`.

Note that the factory methods don't check for assignment to the resource variable or passing the resource variable in a `ref` or `out` parameter. Doing so would require a visit of the specified `Body`, which could lead to the early reduction of `Extension` nodes (too eager) or skip those nodes to avoid the reduction (too shallow). We could still add such a check to the `Reduce` code path where reduction of `Extension` nodes is carried out anyway.

##### Lock

The `LockCSharpStatement` node represents a `lock` statement. It contains an `Expression` to synchronize on prior to executing a `Body` expression. Factory methods ensure that the specified `Expression` is not a value type which would incur boxing.

Reduction of a `Lock` node consists of a `Block` containing a call to the `Monitor.Enter(object, ref bool)` method, followed by a `TryFinally` expression containing the execution of the `Body` in the protected region and a call to `Monitor.Exit(object)` in the finally handler. A `lockTaken` variable is introduced to perform an `IfThen` check prior to calling `Monitor.Exit(object)`. This is completely analogous to the code emitted by the C# compiler.

An example of creating a `Lock` statement is shown below:

```csharp
CSharpStatement.Lock(
  Expression.Constant(gate),
  Expression.Call(foo)
)
```

##### Switch

Even though the DLR has a `SwitchExpression`, we've introduced a custom construct for C# to capture some specific semantics. The `SwitchCSharpStatement` node represents a `switch` statement. It contains a `SwitchValue` to switch on, a `BreakLabel` to denote the label to break out of the `switch` and a `Cases` collection containing the switch cases.

Differences from the DLR node include:
- C# switch statements have type `void`.
- C# switch statements can have no cases with test values and/or no default case (i.e. they can be emptier).
- C# switch statements support `GotoCase` and `GotoDefault` control flow.
- C# switch statements can have cases that include test values and a default label.

An example of creating a `Switch` statement is shown below:

```csharp
CSharpStatement.Switch(
  switchValue,
  breakLabel,
  CSharpStatement.SwitchCase(
    Expression.Call(cout, "Even"),
    0, 2, 4
  ),
  CSharpStatement.SwitchCase(
    CSharpStatement.GotoCase(0),
    6, 8
  ),
  CSharpStatement.SwitchCase(
    Expression.Call(cout, "Odd"),
    1, 3, 5
  ),
  CSharpStatement.SwitchCase(
    CSharpStatement.GotoCase(1),
    7, 9
  ),
  CSharpStatement.SwitchCase(
    CSharpStatement.GotoDefault(),
    -1
  ),
  CSharpStatement.SwitchCaseDefault(
    Expression.Call(cout, "Default")
  )
)
```

where `switchValue` is the expression of type `int` representing the value to switch on, and `breakLabel` is a `LabelTarget` of type `void`.

Note that the support for `GotoCase` and `GotoDefault` is realized by the reduction phase of the node. It'd be hard to provide this type of control flow in a DLR `SwitchExpression` without relying on a node higher up (e.g. a custom `Lambda` node) to perform a rewrite of the entire body. The reduction process is described in more detail below.

Factory methods for `SwitchCSharpStatement` check whether all test values for cases are unique and consistently typed. The type of the switch value and the cases is checked against the supported governing types conform the C# language specification. Null test values are supported for governing types that are either `string` or a nullable value type.

Each switch case is described as a `CSharpSwitchCase` node. Unlike the DLR `SwitchCase`, the `TestValues` collection contains constant values of type `object` rather than `Expression` nodes. This reflects the restriction of the C# language whereby test cases have to be compile-time constants. Alternatively, we could use `Expression` nodes here and require them to be of type `Constant`. Each node also contains a `Body` expression containg the body of the switch case.

Factory methods for `CSharpSwitchCase` test for the presence of at least one test value and check the governing type of the test values to be valid and consistent.

Note that `CSharpSwitchCase` nodes don't have a `LabelTarget` property for use in `Goto` statements targeting the case. Instead, we decided to support a `GotoCase` node with a `Value` property of type `object` referring to the case to jump to. This retains the original user intent which is useful when expression trees are used to translate C# statements to a foreign language, preserving the exact `goto case` statements.

Reduction of a `Switch` node is non-trivial and involves a lowering step whereby the cases of the `switch` are analyzed for `goto case` and `goto default` control transfers. For any cases that are jump targets, a `LabelTarget` is created, a `Label` expression is prepended to the case body, and the `GotoCase` and `GotoDefault` statements are rewritten to `Goto` expressions that jump to these labels. A `Block` expression wraps the lowered form of the `Switch` node and appends a `Label` expression for the `BreakLabel`.

Generally speaking, the `Switch` node reduces into the DLR equivalent, therefore keeping the rewrite minimally invasive. This is done in order to make consuming the C# expression through a (reducing) `ExpressionVisitor` reasonable given that we have a good DLR construct to emit (unlike for `AsyncLambda` where we have to do an overhaul and the reduced form isn't human-readable).

However, we optimize a few cases that the DLR fails to optimize for. In particular, we have special treatment for switches with a nullable governing type in order to emit an `IfThenElse` with null-checking logic wrapping the `Switch` on non-null values. The DLR doesn't optimize this case and emits a chain of `IfThenElse` nodes rather than leveraging the `switch` IL instruction. This optimization is analogous to the one in the C# and VB compilers.

##### Block

A specialized block expression `BlockCSharpExpression` has been added as well. It's very similar to the `BlockExpression` in the DLR but it allows for the specification of a return label as a `LabelTarget`. This is useful to keep the shape of the tree as close as possible to what the user wrote without relying on a `BlockExpression` with a separate `LabelExpression` for the return label at the end. In this way, the C# specific block expression is similar to the loop constructs in the DLR where the `LabelExpression` nodes are implied.

The Roslyn fork with extended expression tree support emits `BlockCSharpExpression` nodes for the top-level block nodes of statement-bodied lambdas. An alternative design could be to represent statement lambdas as separate nodes which have a `ReturnLabel` in addition to the `Parameters` collection and the `Body` expression because this type of block is mostly useful when paired with a lambda expression anyway (although the reduction is straightforward and independent of the containing lambda).

Reduction of the `BlockCSharpExpression` emits a `LabelExpression` after all the expressions specified in the `Statements` collection. The label never has a user-supplied default value; it is assumed control flow can never fall into the `LabelExpression` without a `Goto` (of the return kind) branching to it. Reduction steps could fuse an immediately preceding return statement by absorbing its specified return value in order to avoid emission of multiple `ret` instructions, or more generally perform control flow analysis to prune out unreachable code.

### Miscellaneous

We also introduce specialized assignment nodes to deal with C#-specific cases. These operations are implemented via `AssignBinaryCSharpExpression` and `AssignUnaryCSharpExpression` nodes. The former are used for compound assignments, the latter for increment and decrement operations.

#### Binary assignment nodes

Binary compound assignment support does exist in the DLR but it is more restrictive than what's allowed in C#. In particular, compound assignments are supported for strings (`AddAssign` nodes using `Concat` methods underneath), delegates (using `Delegate.Combine` and `Delegate.Remove`), and integral types of 16 bits and below (using widening and narrowing conversions, including support for checked variants). In order to support the C# semantics of compound assignments, the `AssignBinaryCSharpExpression` node exposes `LeftConversion` and `FinalConversion` lambda expressions as properties, conform C# specification section 7.17.2.

Reduction of the binary assignment nodes uses regular assignment and compound assignments nodes from the DLR after applying the necessary conversions. Implementation methods such as `String.Concat` and `Delegate.Combine` and `Delegate.Remove` are passed to the underlying nodes. In case the left-hand side of the assignment is a `IndexCSharpExpression` (with named and optional parameters), the internal `ReduceAssign` method is used in order to ensure proper evaluation order of indexer arguments and the right-hand side.

Note there some more trickiness around compound assignments involving a left-hand side of a mutable value type, e.g. when applying the assignment to a field. For those cases, calls to helper methods on `RuntimeOpsEx` are emitted during reduction, e.g. `WithByRef`. For more information, see the comments in the code.

#### Unary assignment nodes

Unary increment and decrement operators also exist in the DLR but with a few notable omissions. First, the `byte`, `char`, and `sbyte` types are not supported for the operand of the unary assignment node. Second, checked variants are not available. For those reasons, a specialized `AssignUnaryCSharpExpression` node is provided, with node types including checked variants such as `PostIncrementAssignChecked`.

Reduction of the unary assignment nodes uses the corresponding DLR nodes when applicable, e.g. for non-checked operations applied to a DLR-assignable node with a supported type (e.g. `PostIncrementAssign` on a `ParameterExpression` of type `Int32`). Checked variants, C#-specific supported types (via widening and narrowing conversions), and C#-specific assignment targets (i.e. `IndexCSharpExpression`) are reduced via the corresponding binary compound assignment operations with an right-hand side with a constant value of `1`.

Note there some more trickiness around compound assignments involving a left-hand side of a mutable value type, e.g. when applying the assignment to a field. For those cases, calls to helper methods on `RuntimeOpsEx` are emitted during reduction, e.g. `PostAssignByRef`. For more information, see the comments in the code.
