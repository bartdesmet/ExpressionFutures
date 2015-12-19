// Prototyping extended expression trees for C#.
//
// bartde - December 2015

// NB: These get included in the runtime compilations of snippets to evaluate using Roslyn.

using System;
using System.Collections.Generic;

public struct WeakBox<T>
{
    public WeakBox(T value) { Value = value; }

    public T this[int ignored]
    {
        get { return Value; }
        set { Value = value; }
    }

    public T Value;
}

public static class Utils
{
    public static int NamedParamByRef(ref int x, int y)
    {
        return x + y;
    }
}

public class Conditional : IEquatable<Conditional>
{
    private readonly int _budget;

    public Conditional(int budget)
    {
        _budget = budget;
    }

    public Conditional this[int ignored]
    {
        get
        {
            return _budget == 0 ? null : new Conditional(_budget - 1);
        }
    }

    public Conditional Bar
    {
        get
        {
            return _budget == 0 ? null : new Conditional(_budget - 1);
        }
    }

    public Conditional Foo(int ignored)
    {
        return _budget == 0 ? null : new Conditional(_budget - 1);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Conditional);
    }

    public bool Equals(Conditional other)
    {
        if (other == null)
        {
            return false;
        }

        return this._budget == other._budget;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}