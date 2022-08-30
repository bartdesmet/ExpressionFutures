// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Xunit;
using System;
using System.Linq;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    // NB: The tests cross-check the outcome of evaluating a lambda expression - specified as a string in the
    //     test cases - in two ways. First, by converting the lambda expression to a delegate type and running
    //     IL code produced by the compiler. Second, by converting the lambda expression to an expression tree
    //     using the extended expression tree support in our modified Roslyn build and compiling the expression
    //     at runtime (therefore invoking our Reduce methods).
    //
    //     It is assumed that the outcome has proper equality defined (i.e. EqualityComparer<T>.Default should
    //     return a meaningful equality comparer to assert evaluation outcomes against each other). If the
    //     evaluation results in an exception, its type is cross-checked.
    //
    //     In addition to cross-checking the evaluation outcome, a log is maintained and cross-checked, which
    //     is useful to assert the order of side-effects. The code fragments can write to this log by means of
    //     the Log method and the Return method (to prepend returning a value of type T with a logging side-
    //     effect).

    partial class CompilerTests
    {
        // NB: Tests in this file are derived from https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns.

        [Fact]
        public void CrossCheck_SwitchExpression_DeclarationPatternWithDiscard()
        {
            var f = Compile<Func<Vehicle, decimal>>(@"
                vehicle => vehicle switch
                {
                    Car _ => 2.00m,
                    Truck _ => 7.50m,
                    null => throw new ArgumentNullException(nameof(vehicle)),
                    _ => throw new ArgumentException(""Unknown type of a vehicle"", nameof(vehicle)),
                }
            ", typeof(Vehicle).Assembly);

            AssertEx.Throws<ArgumentNullException>(() => f(null));
            AssertEx.Throws<ArgumentException>(() => f(new Bike()));
            f(new Car());
            f(new Truck());
        }

        [Fact]
        public void CrossCheck_SwitchExpression_TypePattern()
        {
            var f = Compile<Func<Vehicle, decimal>>(@"
                vehicle => vehicle switch
                {
                    Car => 2.00m,
                    Truck => 7.50m,
                    null => throw new ArgumentNullException(nameof(vehicle)),
                    _ => throw new ArgumentException(""Unknown type of a vehicle"", nameof(vehicle)),
                }
            ", typeof(Vehicle).Assembly);

            AssertEx.Throws<ArgumentNullException>(() => f(null));
            AssertEx.Throws<ArgumentException>(() => f(new Bike()));
            f(new Car());
            f(new Truck());
        }

        [Fact]
        public void CrossCheck_SwitchExpression_ConstantPattern()
        {
            var f = Compile<Func<int, decimal>>(@"
                visitorCount => visitorCount switch
                {
                    1 => 12.0m,
                    2 => 20.0m,
                    3 => 27.0m,
                    4 => 32.0m,
                    0 => 0.0m,
                    _ => throw new ArgumentException($""Not supported number of visitors: {visitorCount}"", nameof(visitorCount)),
                }
            ");

            AssertEx.Throws<ArgumentException>(() => f(-1));

            for (var i = 0; i <= 4; i++)
            {
                f(i);
            }

            AssertEx.Throws<ArgumentException>(() => f(5));
        }

        [Fact]
        public void CrossCheck_SwitchExpression_RelationalPattern_Double()
        {
            var f = Compile<Func<double, string>>(@"
                measurement => measurement switch
                {
                    < -4.0 => ""Too low"",
                    > 10.0 => ""Too high"",
                    double.NaN => ""Unknown"",
                    _ => ""Acceptable"",
                }
            ");

            f(double.NaN);

            for (var d = -20.0; d <= 20.0; d += 0.5)
            {
                f(d);
            }
        }

        [Fact]
        public void CrossCheck_SwitchExpression_RelationalPattern_AndOr()
        {
            var f = Compile<Func<DateTime, string>>(@"
                date => date.Month switch
                {
                    >= 3 and < 6 => ""spring"",
                    >= 6 and < 9 => ""summer"",
                    >= 9 and < 12 => ""autumn"",
                    12 or (>= 1 and < 3) => ""winter"",
                    _ => throw new ArgumentOutOfRangeException(nameof(date), $""Date with unexpected month: {date.Month}.""),
                }
            ");

            for (var m = 1; m <= 12; m++)
            {
                f(new DateTime(1983, m, 11));
            }
        }

        [Fact]
        public void CrossCheck_SwitchExpression_And()
        {
            var f = Compile<Func<double, string>>(@"
                measurement => measurement switch
                {
                    < -40.0 => ""Too low"",
                    >= -40.0 and < 0 => ""Low"",
                    >= 0 and < 10.0 => ""Acceptable"",
                    >= 10.0 and < 20.0 => ""High"",
                    >= 20.0 => ""Too high"",
                    double.NaN => ""Unknown"",
                }
            ");

            for (var d = -50.0; d <= 50.0; d += 1.0)
            {
                f(d);
            }
        }

        [Fact]
        public void CrossCheck_SwitchExpression_Or()
        {
            var f = Compile<Func<DateTime, string>>(@"
                date => date.Month switch
                {
                    3 or 4 or 5 => ""spring"",
                    6 or 7 or 8 => ""summer"",
                    9 or 10 or 11 => ""autumn"",
                    12 or 1 or 2 => ""winter"",
                    _ => throw new ArgumentOutOfRangeException(nameof(date), $""Date with unexpected month: {date.Month}.""),
                }
            ");

            for (var m = 1; m <= 12; m++)
            {
                f(new DateTime(1983, m, 11));
            }
        }

        [Fact]
        public void CrossCheck_SwitchExpression_Property()
        {
            var f = Compile<Func<object, string>>(@"
                input => input switch
                {
                    string { Length: >= 5 } s => s.Substring(0, 5),
                    string s => s,

                    ICollection<char> { Count: >= 5 } symbols => new string(symbols.Take(5).ToArray()),
                    ICollection<char> symbols => new string(symbols.ToArray()),

                    null => throw new ArgumentNullException(nameof(input)),
                    _ => throw new ArgumentException(""Not supported input type.""),
                }
            ");

            f("Hello, world!");
            f("Hi!");
            f(new[] { '1', '2', '3', '4', '5', '6', '7' });
            f(new[] { 'a', 'b', 'c' });
        }

        [Fact]
        public void CrossCheck_SwitchExpression_Positional_Deconstruct()
        {
            var f = Compile<Func<Point, string>>(@"
                point => point switch
                {
                    (0, 0) => ""Origin"",
                    (1, 0) => ""positive X basis end"",
                    (0, 1) => ""positive Y basis end"",
                    _ => ""Just a point"",
                }
            ");

            f(new Point { X = 0, Y = 0 });
            f(new Point { X = 1, Y = 0 });
            f(new Point { X = 0, Y = 1 });
            f(new Point { X = 1, Y = 1 });
        }

        [Fact]
        public void CrossCheck_SwitchExpression_Positional_Tuple()
        {
            var f = Compile<Func<int, DateTime, decimal>>(@"
                (groupSize, visitDate) => (groupSize, visitDate.DayOfWeek) switch
                {
                    (<= 0, _) => throw new ArgumentException(""Group size must be positive.""),
                    (_, DayOfWeek.Saturday or DayOfWeek.Sunday) => 0.0m,
                    (>= 5 and < 10, DayOfWeek.Monday) => 20.0m,
                    (>= 10, DayOfWeek.Monday) => 30.0m,
                    (>= 5 and < 10, _) => 12.0m,
                    (>= 10, _) => 15.0m,
                    _ => 0.0m,
                }
            ");

            AssertEx.Throws<ArgumentException>(() => f(-1, new DateTime(1983, 2, 11)));
            AssertEx.Throws<ArgumentException>(() => f(0, new DateTime(1983, 2, 11)));

            for (int groupSize = 1; groupSize <= 12; groupSize++)
            {
                for (int day = 1; day <= 7; day++)
                {
                    var visitDate = new DateTime(1983, 2, day);

                    f(groupSize, visitDate);
                }
            }
        }

        [Fact]
        public void CrossCheck_SwitchExpression_Positional_Declaration()
        {
            var f = Compile<Func<object, string>>(@"
                point => point switch
                {
                    Point2DRecord (> 0, > 0) p => p.ToString(),
                    Point3DRecord (> 0, > 0, > 0) p => p.ToString(),
                    _ => string.Empty,
                }
            ", typeof(Point2DRecord).Assembly);

            f(null);
            f(42);

            for (int x = 0; x < 1; x++)
            {
                for (int y = 0; y < 1; y++)
                {
                    f(new Point2DRecord(x, y));

                    for (int z = 0; z < 1; z++)
                    {
                        f(new Point3DRecord(x, y, z));
                    }
                }
            }
        }

        [Fact]
        public void CrossCheck_IsExpression_PositionalAndProperty()
        {
            var f = Compile<Func<WeightedPoint, bool>>("point => point is (>= 1, >= 2) { Weight: >= 3.0 }", typeof(WeightedPoint).Assembly);

            f(null);
            f(new WeightedPoint(0, 0) { Weight = 0.0 });
            f(new WeightedPoint(1, 0) { Weight = 0.0 });
            f(new WeightedPoint(0, 2) { Weight = 0.0 });
            f(new WeightedPoint(0, 0) { Weight = 3.0 });
            f(new WeightedPoint(1, 2) { Weight = 0.0 });
            f(new WeightedPoint(0, 2) { Weight = 3.0 });
            f(new WeightedPoint(1, 2) { Weight = 3.0 });
            f(new WeightedPoint(2, 3) { Weight = 4.0 });
        }

        [Fact]
        public void CrossCheck_IsExpression_PositionalAndProperty_Declaration()
        {
            var f = Compile<Action<WeightedPoint>>(@"
                point =>
                {
                    if (point is (>= 1, >= 2) { Weight: >= 3.0 } p)
                    {
                        Log(p.ToString());
                    }
                }", typeof(WeightedPoint).Assembly);

            f(null);
            f(new WeightedPoint(0, 0) { Weight = 0.0 });
            f(new WeightedPoint(1, 0) { Weight = 0.0 });
            f(new WeightedPoint(0, 2) { Weight = 0.0 });
            f(new WeightedPoint(0, 0) { Weight = 3.0 });
            f(new WeightedPoint(1, 2) { Weight = 0.0 });
            f(new WeightedPoint(0, 2) { Weight = 3.0 });
            f(new WeightedPoint(1, 2) { Weight = 3.0 });
            f(new WeightedPoint(2, 3) { Weight = 4.0 });
        }

        [Fact]
        public void CrossCheck_IsExpression_Var()
        {
            var f = Compile<Func<Func<int, int[]>, int, int, bool>>(@"
                (SimulateDataFetch, id, absLimit) =>
                    SimulateDataFetch(id) is var results
                    && results.Min() >= -absLimit
                    && results.Max() <= absLimit");

            for (int id = 0; id < 10; id++)
            {
                for (int absLimit = -10; absLimit <= 10; absLimit++)
                {
                    f(SimulateDataFetch, id, absLimit);
                }
            }

            int[] SimulateDataFetch(int id)
            {
                var rand = new Random(id);
                return Enumerable
                           .Range(start: 0, count: 5)
                           .Select(s => rand.Next(minValue: -10, maxValue: 11))
                           .ToArray();
            }
        }

        [Fact]
        public void CrossCheck_SwitchExpression_Var()
        {
            var f = Compile<Func<Point2DRecord, Point2DRecord>>(@"
                point => point switch
                {
                    var (x, y) when x < y => new Point2DRecord(-x, y),
                    var (x, y) when x > y => new Point2DRecord(x, -y),
                    var (x, y) => new Point2DRecord(x, y),
                }
            ", typeof(Point2DRecord).Assembly);

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    f(new Point2DRecord(x, y));
                }
            }
        }

        [Fact]
        public void CrossCheck_SwitchExpression_Discard()
        {
            var f = Compile<Func<DayOfWeek?, decimal>>(@"
                dayOfWeek => dayOfWeek switch
                {
                    DayOfWeek.Monday => 0.5m,
                    DayOfWeek.Tuesday => 12.5m,
                    DayOfWeek.Wednesday => 7.5m,
                    DayOfWeek.Thursday => 12.5m,
                    DayOfWeek.Friday => 5.0m,
                    DayOfWeek.Saturday => 2.5m,
                    DayOfWeek.Sunday => 2.0m,
                    _ => 0.0m,
                }
            ");

            f(null);
            f(DayOfWeek.Monday);
            f(DayOfWeek.Tuesday);
            f(DayOfWeek.Wednesday);
            f(DayOfWeek.Thursday);
            f(DayOfWeek.Friday);
            f(DayOfWeek.Saturday);
            f(DayOfWeek.Sunday);
        }
    }
}

public abstract class Vehicle { }
public class Car : Vehicle { }
public class Truck : Vehicle { }
public class Bike : Vehicle { }

public class Point2DRecord : IEquatable<Point2DRecord>
{
    public Point2DRecord(int x, int y) => (X, Y) = (x, y);

    public int X { get; }
    public int Y { get; }

    public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);

    public override bool Equals(object obj) => obj is Point2DRecord p && Equals(p);

    public override int GetHashCode() => (X, Y).GetHashCode();

    public bool Equals(Point2DRecord other) => other != null && (X, Y) == (other.X, other.Y);

    public override string ToString() => $"{{X = {X}, Y = {Y}}}";
}

public class Point3DRecord : IEquatable<Point3DRecord>
{
    public Point3DRecord(int x, int y, int z) => (X, Y, Z) = (x, y, z);

    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    public void Deconstruct(out int x, out int y, out int z) => (x, y, z) = (X, Y, Z);

    public override bool Equals(object obj) => obj is Point3DRecord p && Equals(p);

    public override int GetHashCode() => (X, Y).GetHashCode();

    public bool Equals(Point3DRecord other) => other != null && (X, Y, Z) == (other.X, other.Y, other.Z);


    public override string ToString() => $"{{X = {X}, Y = {Y}, Z = {Z}}}";
}

public class WeightedPoint
{
    public WeightedPoint(int x, int y) => (X, Y) = (x, y);

    public int X { get; }
    public int Y { get; }

    public double Weight { get; set; }

    public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);

    public override string ToString() => $"{{X = {X}, Y = {Y}, Weight = {Weight}}}";
}
