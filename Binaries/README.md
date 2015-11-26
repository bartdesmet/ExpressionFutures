# Binaries folder

This is the drop location for custom Roslyn builds that need to be tested for implicit conversions of lambda expressions to expression trees using the new C# expression APIs.

In order to test a custom build of the compiler, replace all binaries in this folder, rebuild the solution, and make edits to CompilerTests.Generated.tt to include expressions that leverage the new APIs, e.g.

```csharp
// A statement tree that will use the CSharpStatement.Block factory.
(Expression<Action>)(() => {})
```