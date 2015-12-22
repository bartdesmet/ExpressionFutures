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

    public static bool DynamicInvokeWithGeneric<T>(Func<string, string> log, int x)
    {
        log($"{typeof(T)} with int x = {x}");
        return true;
    }

    public static int DynamicInvokeWithGeneric<T>(Func<string, string> log, bool b)
    {
        log($"{typeof(T)} with bool b = {b}");
        return 42;
    }

    public static object DynamicInvokeWithGeneric<T>(Func<string, string> log, string s)
    {
        log($"{typeof(T)} with string s = {s}");
        return null;
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

public class Truthy
{
    private readonly bool _value;

    public Truthy(bool value)
    {
        _value = value;
    }

    public static bool operator true(Truthy b)
    {
        return b._value;
    }

    public static bool operator false(Truthy b)
    {
        return !b._value;
    }
}

public class Booleany
{
    private readonly bool _value;

    public Booleany(bool value)
    {
        _value = value;
    }

    public static implicit operator bool (Booleany b)
    {
        return b._value;
    }
}

public class Inty
{
    private readonly int _value;

    public Inty(int value)
    {
        _value = value;
    }

    public static implicit operator int (Inty b)
    {
        return b._value;
    }
}

public class ResourceClass : IDisposable
{
    private readonly Func<string, string> _log;

    public ResourceClass(Func<string, string> log)
    {
        _log = log;
        _log(".ctor");
    }

    public void Do(bool b)
    {
        if (b)
            throw new DivideByZeroException();
    }

    public void Dispose()
    {
        _log("Dispose");
    }
}

public struct ResourceStruct : IDisposable
{
    private readonly Func<string, string> _log;

    public ResourceStruct(Func<string, string> log)
    {
        _log = log;
        _log(".ctor");
    }

    public void Do(bool b)
    {
        if (b)
            throw new DivideByZeroException();
    }

    public void Dispose()
    {
        _log("Dispose");
    }
}

public class DynamicInvoker
{
    public T Return<T>(T value)
    {
        return value;
    }
}

public class MyEnumerable
{
    private readonly int _count;

    public MyEnumerable(int count)
    {
        _count = count;
    }

    public MyEnumerator GetEnumerator()
    {
        return new MyEnumerator(_count);
    }
}

public class MyEnumerator
{
    private int _count;

    public MyEnumerator(int count)
    {
        _count = count;
    }

    public bool MoveNext()
    {
        return _count-- > 0;
    }

    public int Current
    {
        get { return _count; }
    }
}

public class MyEnumerableValue
{
    private readonly int _count;

    public MyEnumerableValue(int count)
    {
        _count = count;
    }

    public MyEnumeratorValue GetEnumerator()
    {
        return new MyEnumeratorValue(_count);
    }
}

public struct MyEnumeratorValue
{
    private int _count;

    public MyEnumeratorValue(int count)
    {
        _count = count;
    }

    public bool MoveNext()
    {
        return _count-- > 0;
    }

    public int Current
    {
        get { return _count; }
    }
}