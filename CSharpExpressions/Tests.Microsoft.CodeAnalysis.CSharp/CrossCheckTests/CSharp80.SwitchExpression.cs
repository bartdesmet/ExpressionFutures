﻿// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void CrossCheck_SwitchExpression_Positional()
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

        // TODO: Continue to add tests derived from samples.
    }
}

public abstract class Vehicle { }
public class Car : Vehicle { }
public class Truck : Vehicle { }
public class Bike : Vehicle { }
