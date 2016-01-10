# Testing Strategy

This page describes the various testing approaches taken to ensure both the quality of the Roslyn compiler changes as well as the underlying expression API targeted by the extensions.

## Testing the runtime library

In order to test the runtime library, we need to focus on various areas, which are elaborated on below.

### Factories

One class of tests deals with asserting the correctness of the factory methods on `CSharpExpression`, `CSharpStatement`, and `DynamicCSharpExpression` without making any assumptions about those being called by the code emitted by the Roslyn compiler. This ensures those factory methods are generally useful for constructing expression trees.

Typical checks include checking whether operands to newly created nodes are readable or writeable, have types that are compatible with the operation represented by the node, etc. This code makes extensive use of stubs that call into the `System.Linq.Expressions` APIs which we don't test and assume to be correct. Note that errors thrown by factories are either reused from the `System.Linq.Expressions` APIs or are new ones generated via the [Error.tt T4 file](../Microsoft.CSharp.Expressions/Error.tt) file. We check for the expected exception type in our tests.

Tests for the factory methods exercise a bunch of negate cases where an invalid node could be created, e.g. one that has does not type check or would violate type safety. In addition, the indirect factory code path of calling `Update` methods is also exercised.

### Nodes

properties etc.

### Utilities

visitors etc.

### Reduction support

compile and eval
assert DebugViews (T4)
overlap with Roslyn E2E tests

## Testing the Roslyn changes

Besides the runtime library, we also have tests for the changes made to the Roslyn compiler in order to support the conversion of lambda expressions containing the newly supported nodes to expression trees.

### Cross-check tests

Cross-check tests for code fragments (T4)

### Gaps

integration of test suite in the Roslyn fork