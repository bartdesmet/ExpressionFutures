# Expression Tree API Futures

This repo contains various suggested expression tree improvements for `System.Linq.Expressions`, including:

- `Microsoft.CSharp.Expressions` with C# specific expression nodes for language constructs added after C# 3.0 (see [CSharpExpressions](https://github.com/bartdesmet/ExpressionFutures/tree/master/CSharpExpressions))
- Fixes to work around some of the existing limitations in the expression compiler, e.g. for `TryFault` (see [ExceptionHandlerLowering](https://github.com/bartdesmet/ExpressionFutures/tree/master/ExceptionHandlerLowering))
- An example for a proposed *expression types* language feature which enables other types to be used as expression trees (see [ExpressionLikeTypes](https://github.com/bartdesmet/ExpressionFutures/tree/master/ExpressionLikeTypes)).
- Tools to manipulate expression trees. These were shipped as part of Reaqtive in the [Nuqleon libraries](https://github.com/reaqtive/reaqtor/tree/main/Nuqleon).
