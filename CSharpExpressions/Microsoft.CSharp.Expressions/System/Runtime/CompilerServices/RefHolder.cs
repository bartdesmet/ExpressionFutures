// Prototyping extended expression trees for C#.
//
// bartde - December 2021

/*
 * This provides a potential workaround for the lack of ref locals in expression trees, by storing
 * a local RefHolder<T> (which shall not be hoisted to the heap, so we'll need extra handling for
 * the async lambda rewriting) to hold the reference for subsequent assignment through Value. E.g.
 * we could reduce deconstructing assignments like `(t.x, t.y) = expr` where `t` is a tuple type,
 * and we need to store `ref T t1 = ref t.x` for subsequent assignment after evaluating `expr`.
 *
 * An example of a self-contained test is shown below.
 *

    var t = Expression.Parameter(typeof((int, string)[]), "t");
    var i = Expression.Parameter(typeof(int), "i");
    var x = Expression.Parameter(typeof(int), "x");

    var refHolderType = typeof(RefHolder<int>);
    var refHolderCtor = refHolderType.GetConstructor(new[] { typeof(int).MakeByRefType() })!;
    var refHolderProp = refHolderType.GetProperty(nameof(RefHolder<int>.Value))!;

    var r = Expression.Parameter(refHolderType, "r");
    var variable = Expression.Field(Expression.ArrayAccess(t, i), t.Type.GetElementType()!.GetField("Item1")!);

    var body =
        Expression.Block(
            new[] { r },
            Expression.Assign(r, Expression.New(refHolderCtor, variable)),
            Expression.Assign(Expression.Property(r, refHolderProp), x)
        );

    var e = Expression.Lambda<Action<(int, string)[], int, int>>(body, t, i, x);
    var f = e.Compile();

    var xs = new (int, string)[] { (0, ""), (0, "") };
    f(xs, 1, 42);
    Console.WriteLine(xs[1].Item1);

 */


namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Holds a reference to an object.
    /// </summary>
    /// <typeparam name="TObject">Type of the object being referenced.</typeparam>
    public ref struct RefHolder<TObject>
    {
#if NETCORE
        private readonly Span<TObject> _span;
#endif

        /// <summary>
        /// Creates a wrapper around a reference.
        /// </summary>
        /// <param name="value">The object to refer to.</param>
        public RefHolder(ref TObject value)
        {
#if NETCORE
            _span = System.Runtime.InteropServices.MemoryMarshal.CreateSpan<TObject>(ref value, 1);
#else
            throw NotSupported;
#endif
        }

        /// <summary>
        /// Assigns a value to the referenced object.
        /// </summary>
        public TObject Value
        {
#if NETCORE
            set => _span[0] = value;
#else
            set => throw NotSupported;
#endif
        }

        /// <summary>
        /// Invokes an action that accepts a reference to the object.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument to pass to the action.</typeparam>
        /// <param name="action">The action to invoke where the first parameter is a reference to the object.</param>
        /// <param name="arg">The argument to pass to the second parameter of <paramref name="action"/>.</param>
        public void Invoke<TArgument>(ActionByRef<TObject, TArgument> action, TArgument arg)
        {
#if NETCORE
            action(ref _span[0], arg);
#else
            throw NotSupported;
#endif
        }

#if !NETCORE
        private static Exception NotSupported => new NotSupportedException("Ref locals are not supported.");
#endif
    }
}
