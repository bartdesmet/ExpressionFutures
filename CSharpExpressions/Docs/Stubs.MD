# Framework Stubs

This page describes the approach taken by this prototype to access non-public helper methods in the BCL, as well as future considerations.

## Goal

In order to reuse many of the internal helpers in the System.Linq.Expressions API, we call them using private reflection in this prototype, awaiting a final solution using `InternalsVisibleTo` or exposing some of these for more general use. Examples include type system helpers, various checks for use in expression factories, etc. The goal of this approach is to minimize cloning of code wherever possible.

## Mechanism

Stubs are generated using the [Stubs.tt T4 file](../Microsoft.CSharp.Expressions/Stubs.tt) which includes a list of types and members to generate stubs for. The stubs simply obtain a reference to the `MemberInfo` for the specified member, cache it in a static field, and wrap it with an API that's similar to the original one. When disambiguation is needed, e.g. for a type name, a suffix can be added to the stub type name. Any `TargetInvocationException` thrown by the late-bound invocation gets unwrapped into its `InnerException` instead.

## Future directions

Going forward, we should investigate whether an `InternalsVisibleTo` approach for `Microsoft.CSharp.Expressions` in `System.Core` is a reasonable approach. It points against the flow of the layer map of assemblies for sure, but that's not unheard of with `InternalsVisibleTo` (nor does it cause a cyclic dependency during build). It may also be helpful to consider exposing some members in a `CompilerServices` binary. Good examples include type system stuff a la `TypeUtils` which are also useful to implementers of LINQ providers and would readily be usable by extended C# (and VB?) expression tree libraries.