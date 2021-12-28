// Prototyping extended expression trees for C#.
//
// bartde - December 2021

#if NOTYET

/*
 * This provides a potential workaround for the lack of ref locals in expression trees, by storing
 * a local RefHolder<T> (which shall not be hoisted to the heap, so we'll need extra handling for
 * the async lambda rewriting) to hold the reference for subsequent assignment through Value. E.g.
 * we could reduce deconstructing assignments like `(t.x, t.y) = expr` where `t` is a tuple type,
 * and we need to store `ref T t1 = ref t.x` for subsequent assignment after evaluating `expr`.
 *
 * An example of a self-container test is shown below.
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

using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
    public ref struct RefHolder<T>
    {
        private readonly Span<T> _span;

        public RefHolder(ref T value)
        {
            _span = MemoryMarshal.CreateSpan<T>(ref value, 1);
        }

        public T Value
        {
            set => _span[0] = value;
        }
    }
}

#endif
