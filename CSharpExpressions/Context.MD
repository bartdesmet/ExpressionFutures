# Project Rationale

This project aims to increase the number of language constructs that are supported in expression trees which were introduced in C# 3.0 and .NET 3.5 as part of the LINQ project.

## A quick intro to expression trees

TODO

```csharp
           Func<int>  f = () => 42;
Expression<Func<int>> e = () => 42;
```

Homo-iconicity
Quotations
Visitors

## Current limitations

TODO

Expression subset as it existed in C# 3.0
No statement nodes

## Extending expression tree support

TODO

## Design considerations

TODO

Roslyn analyzers to reject new nodes for legacy query providers