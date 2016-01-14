# Project Rationale

This project aims to increase the number of language constructs that are supported in expression trees which were introduced in C# 3.0 and .NET 3.5 as part of the LINQ project.

## A quick intro to expression trees

Expression tree enable capturing a code fragment as a data structure that's usable for runtime analysis and translation, for example in the context of a LINQ provider. This is known in many other languages as *quotations*, most notably in LISP.

In order to capture an expression tree, both C# and Visual Basic support converting a lambda expression to either a delegate or expression tree, as shown below:

```csharp
           Func<int>  f = () => 42;
Expression<Func<int>> e = () => 42;
```

Note that the syntax to represent an anonymous function delegate or an expression tree is exactly the same. This property is known as *homo-iconicity* meaning "same representation".

The object model for expression trees is defined in the .NET Framework in the `System.Linq.Expressions` namespace. Given their general purpose nature, the use of `Linq` in the namespace is merely a historical artifact. Note that expression tree nodes are immutable which enables for reuse, safe sharing, and upfront type checking during construction via factory patterns.

An expression visitor is provided in the .NET Framework (starting with .NET 4.0) to traverse expression trees using a double-dispatch pattern underneath, for example when performing an anlysis.

Starting with .NET 4.0, the Dynamic Language Runtime (DLR) is built on top of the expression tree APIs, leading to the introduction of statement-like node constructs such as `Loop`, `Block`, `Switch`, `Goto`, and `Try`. Even though these are typically referred to as statements, these nodes can evaluate to have a value and can be classified as expressions, e.g. for use in Python and Ruby.

One powerful capability of expression trees is their ability to get compiled and evaluated at runtime. For this purpose, the `LambdaExpression` and `Expression<TDelegate>` types expose `Compile` methods which return delegates. This enables very powerful runtime meta-programming scenarios, including (but not limited to) partial local evaluation of LINQ queries and efficient execution of dynamically typed programs with the DLR:

```csharp
Expression<Func<int>> e = () => 42;
           Func<int>  f = e.Compile();
```

## Current limitations

Language feature support in expression trees depends on two pieces:

* Having expression tree nodes to model language constructs in soem runtime library
* Having support for converting a lambda expression containing these language constructs to an expression tree

Right now, the supported language constructs are pretty much the expression subset of the C# language version 3.0. This excludes statement trees (even though .NET 4.0 introduced these constructs in the runtime library) and expression constructs added after version 3.0. Below is a list of unsupported constructs:

* Statement nodes (1.0 and beyond)
* Compound assignment expressions (1.0)
* Increment and decrement expressions (1.0)
* Multi-dimensional array initializers (1.0)
* Named and optional parameters (4.0)
* Dynamically bound operations (4.0)
* Async lambdas and await expressions (5.0)
* Conditional access expressions (6.0)
* Dictionary initializers (6.0)

## Extending expression tree support

TODO

## Design considerations

TODO

Roslyn analyzers to reject new nodes for legacy query providers