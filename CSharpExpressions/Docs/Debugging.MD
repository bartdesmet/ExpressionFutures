# Debugging Support

This page describes various features available in this prototype to aid in debugging expression trees by inspection.

## Existing debugging features

The BCL has two features that aid in the debugging of expression trees: a `DebugView` property and debugger proxies for use by the IDE.

### DebugView property

Expression trees in `System.Linq.Expressions` already have a `DebugView` property which can be inspected from the debugger. It prints a representation of the expression tree to a `StringBuilder` in a syntactically concise manner. For example:

```csharp
Expression<Func<int, double>> f = x => checked(Math.Sin(x * 2));
```

has a `DebugView` representation that looks like this:

```
.Lambda #Lambda1<System.Func`2[System.Int32,System.Double]>(System.Int32 $x) {
    .Call System.Math.Sin((System.Double)($x #* 2))
}
```

Note it captures a good deal of information but it has a few restrictions:

* Methods implementing unary and binary operators are not represented
* Parameter expressions and label targets are referred to by name, not by reference
* Extension nodes are eagerly reduced prior to printing them

All of this makes sense for the current use cases of `DebugView` for quick inspection at runtime. However, we'd be better off if we had a richer `DebugView` capability with the following properties:

* A more complete representation of the tree including many of the nodes' properties in order to do textual asserts for expected trees in testing
* Extensibility for extension nodes to participate in producing a DebugView representation without causing them to get reduced eagerly

### Debugger proxies

In addition to the `DebugView` property, expression tree types in `System.Linq.Expressions` are annotated with the `DebuggerTypeProxy` attribute. The proxy types provide a view on the various properties of the expression node, including the `DebugView` property in order to make it only accessible at debug time from the IDE.

## A new DebugView approach

This prototype has a new approach to `DebugView` that makes it more extensible and usable for asserts in test code, based on syntactic equality.

### DebugView method

To address the concerns listed above, we've introduced a new `DebugView` extension method in a new `System.Linq.Expressions.Debugging` assembly:

```csharp
namespace System.Linq.Expressions
{
    public static class ExpressionDebugViewExtensions
	{
        public static XNode DebugView(this Expression expression) { ... }
    }
}
```

The use of `XNode` enables producing a tree representation of the expression which is isomorphic to the original. In particular, `XElement` child nodes are used for children of a tree node (e.g. `Left` and `Right` for a `BinaryExpression`) while `XAttribute` nodes are used for other properties (e.g. `Method` for a `BinaryExpression`). This may be one of the rare remaining cases where XML has a good use :-).

### Extensibility story

In order to deal with extension nodes, two interfaces are provided that may be implemented by extension nodes to participate in the visualization of an extension node without causing early reduction. The first interface is `IDebugViewExpression` to be implemented by the node itself:

```csharp
public interface IDebugViewExpression
{
    XNode Accept(IDebugViewExpressionVisitor visitor);
}
```

The second interface is a visitor interface that enables recursing into child nodes of different kinds while also providing access to some utilities:

```csharp
public interface IDebugViewExpressionVisitor
{
    XNode GetDebugView(Expression expression);
    XNode GetDebugView(LabelTarget label);
    XNode GetDebugView(MemberBinding binding);
    XNode GetDebugView(ElementInit initializer);
    XNode GetDebugView(CatchBlock catchBlock);
    XNode GetDebugView(SwitchCase switchCase);
    int MakeInstanceId(object value);
}
```

In here, the overloads to `GetDebugView` support the various nodes that occur in an expression tree and make up for the shape of the tree. Besides `Expression`, this includes some tear off nodes such as `ElementInit`. The `MakeInstanceId` helper method enables assigning a unique integer identifier to an object based on its reference equality. This is useful to render nodes that have reference equality semantics such as `LabelTarget`, `ParameterExpression`, and `ConditionalReceiver` (in the C# API).

An example of a `DebugView` output for the expression used earlier is shown below:

```
<Lambda Type="System.Func`2[System.Int32,System.Double]">
  <Parameters>
    <Parameter Type="System.Int32" Id="0" Name="x" />
  </Parameters>
  <Body>
    <Call Type="System.Double" Method="Double Sin(Double)">
      <Arguments>
        <Convert Type="System.Double">
          <Operand>
            <MultiplyChecked Type="System.Int32">
              <Left>
                <Parameter Type="System.Int32" Id="0" Name="x" />
              </Left>
              <Right>
                <Constant Type="System.Int32" Value="2" />
              </Right>
            </MultiplyChecked>
          </Operand>
        </Convert>
      </Arguments>
    </Call>
  </Body>
</Lambda>
```

Note the presence of the `Id` attribute on the `Parameter` nodes to indicate the reference equality.

Adding in a C#-specific node, for example a named parameter specification for the `a` parameter (likely a name predating any API design guidelines) to `Math.Sin`, does not change the ability to get a non-reduced `DebugView` as shown below:

```csharp
Expression<Func<double, double>> f = x => Math.Sin(a: x * 2);
```

produces the following `DebugView` output:

```
<Lambda Type="System.Func`2[System.Double,System.Double]">
  <Parameters>
    <Parameter Type="System.Double" Id="0" Name="x" />
  </Parameters>
  <Body>
    <CSharpCall Type="System.Double" Method="Double Sin(Double)">
      <Arguments>
        <ParameterAssignment Parameter="Double a">
          <Expression>
            <Multiply Type="System.Double">
              <Left>
                <Parameter Type="System.Double" Id="0" Name="x" />
              </Left>
              <Right>
                <Constant Type="System.Double" Value="2" />
              </Right>
            </Multiply>
          </Expression>
        </ParameterAssignment>
      </Arguments>
    </CSharpCall>
  </Body>
</Lambda>
```

Note the presence of a `CSharpCall` node in the output. All C#-specific nodes implement the `IDebugViewExpression` interface

### Future considerations

One negative aspect of this approach is the dependency on `System.Xml.Linq` which also drags in the `System.Xml` assembly. I'd much prefer to cut this dependency going forward, either by introducing a format-neutral tree representation (that could be mapped onto XML or JSON or anything of the user's liking) or by changing the debugging visitor to be based on a `StringBuilder` textual emitter (much like `DebugViewWriter` in the BCL today).

In order to make future refactoring easier, the organization of the code is such that all debugging aspects are isolated in the `CSharpExpression.Debugging.*` files. It would suffice to conditionally compile these in order to cut the dependency on `System.Xml`.

Nonetheless, with the introduction of extension nodes for language-specific constructs, it seems a worthy investment to build an extensible `DebugView` capability, ideally integrated with the existing functionality (rather than adding a new way to achieve similar goals). The current implementation is a pragmatic choice to make inspection of the new extension nodes easier, e.g. via [RoslynPad](RoslynPad.MD).

## ToCSharp decompiler

Besides the `DebugView` extension method, we also provide a series of `ToCSharp` extension methods that print the expression as a C#-like code fragment. It goes without saying that decompilation of an expression tree to C# is not always possible given that the expression APIs are more expressive than the subset of constructs supported by the C# language. This is particularly the case for the DLR expressions. As such, the returned string is provided as a best-effort C#-like visualization that's useful for debugging.

The various overloads of `ToCSharp` are shown below:

```csharp
namespace System.Linq.Expressions
{
    public static class ExpressionCSharpExtensions
	{
        public static string ToCSharp(this Expression expression) { ... }
		public static string ToCSharp(this Expression expression, params string[] namespaces) { ... }
    }
}
```

In addition to extension methods for `Expression`, similar methods are provided for `Type` in order to print the name of a type using C# syntax, usable for visualization of variable declarations. This method, too, has a best-effort nature given that the CLR type system is more expressive than the subset exposed in C#.

The main use of the decompiler right now is for the [RoslynPad](RoslynPad.MD) utility used when debugging this library's functionality. Just as is the case for the `DebugView` extension method, its implementation is separated cleanly in the project structure in order to make it easy to exclude in certain build flavors. Nonetheless, functionality like this can come in handy, so it may be worth shipping it as a separate library.

Some of the known restrictions of `ToCSharp` are listed below:

* Limited support to print nodes that are not supported in C#; instead, we print the semantically closest construct in C# (e.g. `Loop` as a block with some `goto` statements and labels).
* No deep tracking of the `checked` context in order to exclude non-checked subexpressions; we could emit `unchecked` around such subexpressions but haven't implemented this yet.
* Heuristics are applied to determine how to print types, assuming anonymous types can always be inferred; this could be generalized to emit more concise code in many cases.
* Limited decompilation of some patterns such as query expressions; instead, chains of method calls are emitted, which may include invalid identifiers for e.g. transparent identifiers.
* Printing of variable declarations separate from initializers due to the way `Block` expressions specify their `Variables` collection; we could improve this by pushing down declarations.
* No notion of a `dynamic` type because we lack this information at runtime for variables; instead, we should detect dynamic use sites and influence the declaration site based on it.
* Automatic assignment of names to labels and parameters that have no specified name, but without checking for name conflicts.
* No checks for naming conflicts across different imported namespaces; the abbreviation of type names can cause ambiguity.

Without doubt, there are more such restrictions. Nonetheless, the current decompiler does a decent enough job for it to be usable in the context of debugging tools such as RoslynPad.

## Debugging proxies

Similar to the `System.Linq.Expressions` API, we provide debugging proxy types for each of the C#-specific expression nodes by annotating them using a `DebuggerTypeProxy` attribute. Generating of those types is automated using the [DebuggerTypeProxies.tt T4 file](../Microsoft.CSharp.Expressions/Microsoft/CSharp/Expressions/DebuggerTypeProxies.tt).

Besides exposing the various properties of the node being visualized, the debugger proxy type also exposes the `DebugView` property which indirectly leverages the `DebugView` extension method described above.

In order to make it possible to exclude the debugging support from the product, all related functionality is emitted in separate files that can easily be excluded from the compilation.