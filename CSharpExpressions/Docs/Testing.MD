# Testing Strategy

This page describes the various testing approaches taken to ensure both the quality of the Roslyn compiler changes as well as the underlying expression API targeted by the extensions.

## Testing the runtime library

In order to test the runtime library, we need to focus on various areas, which are elaborated on below.

### Factories

various type safety checks

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