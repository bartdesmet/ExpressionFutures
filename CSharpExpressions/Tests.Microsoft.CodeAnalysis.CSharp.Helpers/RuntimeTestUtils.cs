﻿// Prototyping extended expression trees for C#.
//
// bartde - December 2015

// NB: These get included in the runtime compilations of snippets to evaluate using Roslyn.

struct WeakBox<T>
{
    public WeakBox(T value) { Value = value; }

    public T this[int ignored]
    {
        get { return Value; }
        set { Value = value; }
    }

    public T Value;
}