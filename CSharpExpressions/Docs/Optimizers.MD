# Expression Optimizers

This page describes some of the optimizations that are included in this project, including a description of their working, why they're useful, and what future directions could be.

## Typical patterns to optimize

Use of reducible nodes to implement custom expression nodes can lead to the introduction of suboptimal patterns when compiling the entire expression. Examples include the use of block expressions which could get deeply nested, emitting non-void expressions whose result gets discarded, etc.

Note that the lambda compiler in the BCL has more context to work with. For example, it can emit an expression as void, so it doesn't have to bother producing a result. A typical example of where this is helpful is in post-increment assignment operations like `i++`. If the result of the expression is discarded, no block expression with an extra temporary variable has to be emitted.

## Optimizers in this project

Two complimentary optimizers can be found in this project. Both are described below.

### Async lambda compiler optimizer

One optimizer deals specifically with async lambdas and flattens nested blocks that are created by reducing await nodes in its body and by performing various transformations to build the state machine.

This optimizer can be found in the `Microsoft\CSharp\Expressions\Compiler` folder and always runs when reducing an `AsyncLambdaCSharpExpression` node. It's strictly optional and could be excluded but is kept to make generated code more optimal and readable upon inspection.

Note that this optimizer could be extended to do more optimizations similar to the ones done in the regular lambda compiler, given that those nodes expose a public `Compile` method. However, one should keep in mind that the regular lambda compiler still runs for asynchronous lambdas, because their `Compile` method simply reduces the node to a synchronous lambda that uses an async method builder underneath.

### General-purpose optimizers

A set of general-purpose expression tree optimizers are included as well. Those are described in greater detail below and are available to users via an `Optimize` extension method on `Expression`.

The goal of these optimizers is to be reusable outside the realm of C#-specific nodes as well. Right now, these are included in the `Microsoft.CSharp.Expressions` assembly but could be split into a separate assembly to make their use pay for play. Furthermore, the split into a C#-specific and general-purpose portion may be possible, though some optimizers benefit from knowing about both domains of expressions.

#### Block optimization

Block optimization flattens nested blocks into possible to reduce the number of nodes in the tree. To do so, it will merge the expressions in a child block with its parent if possible, unpack single expressions nested in blocks, and rewrite any empty blocks that may arise from this into `Default` nodes.

Several restrictions apply such as blocks that have specific strong typing behavior (e.g. discarding a result or treating the result as less derived). In addition we don't fuse scopes blocks with variables in order to avoid having to deal with variable conflicts and possible creation of scopes that are bigger than necessary (which could have a negative effect on compilation).

This optimization is aware of various scoped constructs, including `BlockExpression` and `CSharpBlockExpression` but also `CSharpSwitchCase`. Any future nodes that have block-like semantics should be included in the optimization for it to be most effective.

#### Loop optimization

We perform very minimal loop optimization as well. In particular, we try to take away unnecessary `break` and `continue` labels that are not referenced and rewrite typical `for` loop patterns with post-increment assignments into pre-increment ones.

The former optimization of eliminating `break` and `continue` labels could be applied by default in the reduction of `For`, `While`, `Do`, and `ForEach` C#-specific nodes but by keeping it separate, we can apply it for DLR `Loop` nodes as well.

The latter optimization of rewriting post-increment assignments in `For` loops eliminates the additional block and temporary variable that arises when such iterator expressions are reduced. For example:

```csharp
for (int i = 0; i < 10; i++)
{
    // Body
}
```

would reduce into something like this:

```csharp
    int i = 0;
    goto tst;
bdy:
    // Body
cnt:
    {
        t = i;
        i = t + 1;
        t;
    }
tst:
    if (i < 10)
        goto bdy;
brk:
    ;
```

When applying the loop optimizations, we can take away the unnecessary labels and also turn the post-increment assignment into a pre-increment assignment, like this:

```csharp
    int i = 0;
    goto tst;
bdy:
    // Body
    i = i + 1;
tst:
    if (i < 10)
        goto bdy;
```

This optimization could be applied in a more general context as well by analyzing whether an expression's result gets discarded. In particular, consider the reduced code for the post-increment assignment:

```csharp
    {
        t = i;
        i = t + 1;
        t;
    }
```

In this block expression, the result `t` is not consumed, so the final expression in the block can be taken away. In its turn, this enables the elimination of the `t` variable because no side-effects get reordered by doing so, and we're left with:

```csharp
    i = i + 1;
```

Right now, we don't perform any such general-purpose optimization but we definitely could.

#### Exception handler optimization

TODO

## TODO

- Limited options to optimize in the expression space
- More generally useful, move to separate assembly?
- Options to optimize at instruction level (IL or common emitter for compiler and interpreter)
- Comparison to C# optimizations
- Expense of closures and async state machines in expressions vs compile-time