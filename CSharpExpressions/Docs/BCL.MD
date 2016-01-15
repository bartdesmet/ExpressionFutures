# Framework Extensions

This page discusses suggested extensions to the expression tree facilities in the .NET Framework in order to make the feature set more complete and to address some common shortcomings.

## Changes to existing nodes

Adding support for some newer language features is quite hard without changing existing nodes that have shipped in the .NET Framework. In this section, we discuss those nodes and the suggested changes.

### Dictionary initializers

Dictionary initializers were added in C# 6.0 as an extensio to object initializer expressions. These are modeled using `MemberInitExpression` nodes with a `Bindings` collection consisting of `MemberBinding` instances.

Unlike the `Expression` nodes, `MemberBinding` nodes are not extensible, i.e. there's no `Extension` value for `BindingType` or a `Reduce` mechanism. One issue is the pretty tight coupling of these nodes with their parent and the lambda compiler for compilation purposes.

We can either add support for dictionary initializers by adding a new `BindingType` and a corresponding node in the .NET Framework, or come up with an extensibility story. In the former case, there's the concern about existing LINQ providers making assumptions about the supported `BindingType` values, e.g. in a `switch` statement. In the latter case, it's unclear how reduction for an extension member binding would work because existing bindings are not a good reduction target.

Alternatively, we could clone the `MemberInitExpression` functionality in the C#-specific API in order to accommodate for dictionary initializers. Reduction of this extension node could target the BCL's `MemberInitExpression` if no dictionary initializers are present; otherwise, it'd reduce to a `Block` containing a sequence of statements performing the initialization logic. Unfortunately, this results in cloning a bunch of functionality from the BCL to the C# runtime library.

### Checked variants of increment and decrement assignments

One notable omission from the expression tree API are checked variants of `[Pre|Post][Increment|Decrement]Assign` nodes. It'd make sense to add these nodes to the BCL as a good reduction target for simple variants of such unary assignments in C#.

Note that we still need custom node types in the C# API to deal with differences in lvalue treatment and variable classification, and to deal with C#'s support for widening and narrowing conversions when performing these operations on operands of type `byte` etc.

For more information about these nodes in the C# API and restrictions in the BCL, see comments in the source code.

### Element initializers with extension Add methods

C# 6.0 added support for element initializers with an extension `Add` method. The `ElementInit` node in the BCL does not support extension methods. We could either improve support in the BCL or clone `ListInitExpression` functionality in the C# runtime library.

Considerations are similar to those for dictionary initializers: incompatibilities for existing LINQ providers when they encounter extension methods for `ElementInit.AddMethod` properties and the concern about cloning an existing node solely to add support for this feature.

Note that `MemberInitExpression` and have `MemberListBinding` children which also rely on `ElementInit` so the two initializer expression nodes are joined at the hip and both are considered in a mutually recursive fashion when performing compilation. As such, it'd make sense to consider inclusion of both features in the BCL rather than discussing them one by one.

## Reuse of various utilities

Access to common helpers

## Extensible DebugView

Extensible DebugView printing

## Sharing nodes across languages

Pushing down async lambda support

Etc.