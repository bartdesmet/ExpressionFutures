# Framework Extensions

This page discusses suggested extensions to the expression tree facilities in the .NET Framework in order to make the feature set more complete and to address some common shortcomings.

## Changes to existing nodes

Adding support for some newer language features is quite hard without changing existing nodes that have shipped in the .NET Framework. In this section, we discuss those nodes and the suggested changes.

### Dictionary initializers

Dictionary initializers were added in C# 6.0 as an extension to object initializer expressions. These are modeled using `MemberInitExpression` nodes with a `Bindings` collection consisting of `MemberBinding` instances.

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

Implementation of extension nodes often requires rigorous type checking in factories. Unfortunately many of the helper methods that are used by the `System.Linq.Expressions` API and the DLR are inaccessible publicly. While some may not be suited for public consumption, it may make sense to review some of them (especially the more tricky ones) for broader use.

For an overview of the helpers we required when building the C#-specific expression nodes, see [Stubs](Stubs.MD). In the short term, it may suffice to have `InternalsVisibleTo` friend access to these helpers in order to avoid code duplication.

## Extensible DebugView

The `DebugView` property on `Expression` lacks a good extensibility story, mnaking it hard to inspect extension nodes. It'd be great to invest in an extensible visualization mechanism for expressions, including extension nodes.

For more information on an initial implementation of such functionality for our C#-specific expression nodes, see [Debugging](Debugging.MD).

## Sharing nodes across languages

Right now, we haven't invested in providing extension nodes for Visual Basic to support language constructs introduced after version 9.0 of the language (the "LINQ release"). When we do so, we'll likely encounter many nodes that are very similar to the C# extension nodes provided in this project.

If we decide to work on Visual Basic support, we have to consider whether some nodes should be moved to a common assembly in order to allow for reuse. We have a couple of possibilities to organize the code:

* Have just one assembly for C# and Visual Basic nodes, much like `System.Linq.Expressions` became a namespace with nodes shared across both languages. Note that some language constructs in Visual Basic never got first-class treatment, e.g. string equality checks which use `MethodCallExpression` nodes with references to the Visual Basic runtime library.
* Have a common assembly for shared functionality used by C# and Visual Basic nodes, including common helpers, base classes, etc. Language-specific assemblies are layered on top of this, allowing for the modeling of language-specific nodes whose semantics are (subtly) different across both languages.
* Have two language-speicifc assemblies with the only shared assemblies being those containing the `System.Linq.Expressions` functionality in the BCL. This could lead to a lot of duplication across implementations of extension nodes for both languages, especially since the languages are aiming to co-evolve right now.

The second approach seems to strike a good middle-ground. A concrete example of reuse across both languages would be the machinery used to reduce async lambda expressions with await expressions.