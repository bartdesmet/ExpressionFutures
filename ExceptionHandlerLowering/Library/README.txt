// Prototyping support for interpretation of TryExpression with a fault handler or exception filters.
//
// bartde - October 2015

This solution prototypes one possible approach to enable compilation and interpretation of TryExpression
nodes with fault handlers and exception filters. Support for this is currently limited in the following
cases:

- When compiling to a DynamicMethod, because of a limitation in the MethodBuilder derived class.
- When interpreting using the new Compile(true) API, because of a missing implementation.

The following links are used to discuss this:

- https://github.com/dotnet/corefx/issues/3838 - Compiler for both cases
- https://github.com/dotnet/corefx/issues/3840 - Interpreter for fault handler
- https://github.com/dotnet/corefx/issues/3841 - Interpreter for exception filters

Both cases could possibly be solved as follows:

- Investigate why DynamicMethod has the limitation and try to remove it.
- Emulate fault handlers and filters in the interpreter.

Zooming in to the interpreter case, one possible approach would be to compile a TryExpression using a
pattern that allows to handle all cases, e.g.

    TryCatch(          ==>   try
       body,                 {
       CatchBlock(              body;
          ex,                }
          handler            catch (E ex)
       )                     {
    )                           handler;
                             }

    TryCatch(         ==>    try
       body,                 {
       CatchBlock,              body;
           ex,               }
           handler,          catch (E ex) when (filter(e))
           filter            {
       )                        handler;
    )                        }

    TryFinally(       ==>    try
       body,                 {
       handler                  body;
    )                        }
                             finally
                             {
                                handler;
                             }

    TryFault(         ==>    var success = false;
       body,                 try
       handler               {
    )                           body;
                                success = true;
                             }
                             finally
                             {
                                if (!success)
                                {
                                   handler;
                                }
                             }

For all of these, the constituents can get compiled to instruction ranges in the interpreter and the
exception handling instruction can have specializations for the four cases mentioned above. Also note
that the current interpreter code can "bundle" the handling of many catch blocks and a finally clause,
but this will need to change in order for the filters to be executed according to CLR semantics. I.e.
we can't turn a filter into a "catch-and-evaluation-filter" because it messes up the evaluation order
of filters and finally/fault handlers higher up.

The approach sketched above could likely made to work. However, the limitation for compiler is to be
investigated more. When emitting code to a MethodBuilder, things just work out fine. However, when we
emit to a DynamicMethod we stumble upon limitations for BeginFaultBlock and BeginExceptFilterBlock as
discussed on https://github.com/dotnet/coreclr/issues/1764.

In the meantime though, I've created this alternative approach based on lowering the expression to a
more primitive form. It has the benefit that it works for any compilation mode, because we can chain
it in front of the core compilation logic. In practice, this would have to be done using an internal-
only Reduce method, similar to the lowering of some BinaryExpression nodes. Lowering can be deferred
until we encounter a TryExpression during compilation, as to avoid a deep rewrite of the tree. One of
the drawbacks of this approach is that it relies on helper methods (see ExceptionHandling.cs) in the
runtime which take delegate-typed parameters. This can lead to closure allocation with hoisting of
variables that'd otherwise could remain locals.

For the following examples, we'll denote:

    EHL = ExceptionHandlingLowering
	HR = HandlerRewriter

Lowering of a TryFault node looks as follows:

        EHL[TryFault(b, f)]
    -->
	    with lhd = Parameter(typeof(LeaveHandlerData)) in
	    Block(lhd)
	    {
		  Assign(lhd, Call(ExceptionHandling.TryFault, HR[b, out dispatch], () => f));
		  dispatch(lhd);
        }

The handler rewriter step (HR) is more involved and involves locating all GotoExpression nodes within
a given subexpression that escape the try expression. Those jumps can target different labels and can
carry results of different types. We can't perform a Goto across delegate boundaries, so we turn the
body of the protected region into a function that returns a LeaveHandlerData describing the control
transfer we'd like to happen upon returning from the protected region, i.e.:

    struct LeaveHandlerData
	{
	    public int Index;     // an index for the label to jump to upon returning from the helper
		public object Value;  // the value to carry with the jump
	}

This explains the signature of TryFault (and similar for TryFilter):

    static LeaveHandlerData TryFault(Func<LeaveHandlerData> body, Action handler)

As part of rewriting the body, each label escaping the TryExpression is assigned an index. Each Goto
to any of those labels is turned into a return expression from the resulting body expression, pairing
up the target label index with the value of the expression passed to the Goto (or void represented as
a null reference of type System.Object). The rewrite also produces a dispatch table that switches on
the returned target label index to perform a Goto after returning from TryFault, also performing a
conversion to match the destination label's type. Obviously this control transfer mechanism can cause
boxing for value types.

    dispatch = Switch(Member(lhd.Index), Expression.Convert(lhd.Value, t) /* default case */)
	{
	  SwitchCase(Goto(l0, Convert(lhd.Value, l0.Type)), Constant(i0)),
	  ...
	  SwitchCase(Goto(lN, Convert(lhd.Value, lN.Type)), Constant(iN)),
	};

Various optimizations can be performed:

- No switch table is needed when there are no gotos escaping the protected region.
- Homogeneously typed gotos out of the protected region can be expressed using LeaveHandlerData<T>.

In order to be able to perform the rewrite described above, filtered exception handlers get unflattened
into nested handlers such that we can deal with a single filtered exception handler at a time.