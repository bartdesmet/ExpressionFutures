// Prototyping extended expression trees for C#.
//
// bartde - December 2015

// NB: These get included in the runtime compilations of snippets to evaluate using Roslyn.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

    public static int OptionalParams(int x = 42, string y = "bar", bool z = true)
    {
        return x + y.Length + (z ? 1 : 0);
    }

    public static int Add(int a, int b)
    {
        return a + b;
    }
}

public class NamedAndOptionalParameters
{
    public NamedAndOptionalParameters(int x = 42, string y = "bar", bool z = true)
    {
        Value = x + y.Length + (z ? 1 : 0);
    }

    public int Value { get; }
}

public class IndexerWithNamedAndOptionalParameters
{
    public int this[int x = 42, string y = "bar", bool z = true]
    {
        get { return x + y.Length + (z ? 1 : 0); }
    }
}

public delegate int DelegateWithNamedAndOptionalParameters(int x = 42, string y = "bar", bool z = true);

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

public interface IAwaitable
{
    IAwaiter GetAwaiter();
}

public interface IAwaiter : INotifyCompletion
{
    bool IsCompleted { get; }
    void GetResult();
}

public class AsyncAwaitable : IAwaitable
{
    public static IAwaitable Instance => new AsyncAwaitable();

    public IAwaiter GetAwaiter() => new Awaiter();

    class Awaiter : IAwaiter
    {
        public bool IsCompleted => false;
        public void GetResult() { }
        public void OnCompleted(Action continuation) => Task.Run(continuation);
    }
}

public class SyncAwaitable : IAwaitable
{
    public static IAwaitable Instance => new SyncAwaitable();

    public IAwaiter GetAwaiter() => new Awaiter();

    class Awaiter : IAwaiter
    {
        public bool IsCompleted => true;
        public void GetResult() { }
        public void OnCompleted(Action continuation) => continuation();
    }
}

public class RacingAwaitable : IAwaitable
{
    public static IAwaitable Instance => new RacingAwaitable();

    public IAwaiter GetAwaiter() => new Awaiter();

    class Awaiter : IAwaiter
    {
        public bool IsCompleted => false;
        public void GetResult() { }
        public void OnCompleted(Action continuation) => continuation();
    }
}

public class ThrowingAwaitable : IAwaitable
{
    private readonly Exception _ex;

    public ThrowingAwaitable(Exception ex)
    {
        _ex = ex;
    }

    public IAwaiter GetAwaiter() => new Awaiter(_ex);

    class Awaiter : IAwaiter
    {
        private readonly Exception _ex;

        public Awaiter(Exception ex)
        {
            _ex = ex;
        }

        public bool IsCompleted => false;
        public void GetResult() { throw _ex; }
        public void OnCompleted(Action continuation) => continuation();
    }
}

public class Base
{
}

public class Derived : Base
{
}

public struct MemberInitStruct
{
    private readonly Func<string, string> _log;
    private int _y;

    public MemberInitStruct(Func<string, string> log)
    {
        _log = log;

        X = 0;
        _y = 0;
        Z = new NestedMemberInitStruct(log);
        XS = new List<int>();
    }

    public int X;

    public int Y
    {
        get { _log("get_Y"); return _y; }
        set { _log("set_Y"); _y = value; }
    }

    public NestedMemberInitStruct Z;

    public List<int> XS;
}

public struct NestedMemberInitStruct
{
    private readonly Func<string, string> _log;
    private int _b;

    public NestedMemberInitStruct(Func<string, string> log)
    {
        _log = log;

        A = 0;
        _b = 0;
    }

    public int A;

    public int B
    {
        get { _log("get_B"); return _b; }
        set { _log("set_B"); _b = value; }
    }
}

public struct Point
{
    private readonly Func<string, string> _log;
    private int _x, _y;

    public Point(Func<string, string> log)
    {
        _log = log;
     
        _x = 0;
        _y = 0;
    }

    public int X
    {
        get { _log?.Invoke("get_X"); return _x; }
        set { _log?.Invoke("set_X"); _x = value; }
    }

    public int Y
    {
        get { _log?.Invoke("get_Y"); return _y; }
        set { _log?.Invoke("set_Y"); _y = value; }
    }

    public void Deconstruct(out int x, out int y)
    {
        _log?.Invoke("Deconstruct");
        (x, y) = (_x, _y);
    }
}

public struct Point2D
{
    internal readonly Func<string, string> _log;
    private int _x, _y;

    public Point2D(Func<string, string> log)
    {
        _log = log;

        _x = 0;
        _y = 0;
    }

    public int X
    {
        get { _log?.Invoke("get_X"); return _x; }
        set { _log?.Invoke("set_X"); _x = value; }
    }

    public int Y
    {
        get { _log?.Invoke("get_Y"); return _y; }
        set { _log?.Invoke("set_Y"); _y = value; }
    }
}

public static class Point2DExtensions
{
    public static void Deconstruct(this in Point2D p, out int x, out int y)
    {
        p._log?.Invoke("Deconstruct");
        (x, y) = (p.X, p.Y);
    }
}

public class MyTuple : ITuple
{
    private readonly object[] _values;
    private readonly Func<string, string> _log;

    public MyTuple(object[] values, Func<string, string> log = null)
    {
        _values = values;
        _log = log;
    }

    public object this[int index]
    {
        get
        {
            _log?.Invoke("this[" + index + "]");
            return _values[index];
        }
    }

    public int Length
    {
        get
        {
            _log?.Invoke("Length");
            return _values.Length;
        }
    }
}

public class MyTuple<T1, T2>
{
    private readonly Func<string, string> _log;
    private T1 _item1;
    private T2 _item2;

    public MyTuple(T1 item1, T2 item2, Func<string, string> log = null)
    {
        _item1 = item1;
        _item2 = item2;
        _log = log;
    }

    public T1 Item1
    {
        get { _log?.Invoke(nameof(Item1)); return _item1; }
    }

    public T2 Item2
    {
        get { _log?.Invoke(nameof(Item2)); return _item2; }
    }

    public void Deconstruct(out T1 item1, out T2 item2)
    {
        _log?.Invoke(nameof(Deconstruct));
        (item1, item2) = (_item1, _item2);
    }
}
