// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Collections.Generic;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    public static partial class TestCases
    {
        // TODO: use these in the compiler tests
        public static IEnumerable<string> UnaryAssign
        {
            get
            {
                { _ = (Func<byte, byte>)(x => x++); }
                yield return "(Expression<Func<byte, byte>>)(x => x++)";

                { _ = (Func<byte, byte>)(x => x--); }
                yield return "(Expression<Func<byte, byte>>)(x => x--)";

                { _ = (Func<byte, byte>)(x => ++x); }
                yield return "(Expression<Func<byte, byte>>)(x => ++x)";

                { _ = (Func<byte, byte>)(x => --x); }
                yield return "(Expression<Func<byte, byte>>)(x => --x)";

                { _ = (Func<byte, byte>)(x => checked(x++)); }
                yield return "(Expression<Func<byte, byte>>)(x => checked(x++))";

                { _ = (Func<byte, byte>)(x => checked(x--)); }
                yield return "(Expression<Func<byte, byte>>)(x => checked(x--))";

                { _ = (Func<byte, byte>)(x => checked(++x)); }
                yield return "(Expression<Func<byte, byte>>)(x => checked(++x))";

                { _ = (Func<byte, byte>)(x => checked(--x)); }
                yield return "(Expression<Func<byte, byte>>)(x => checked(--x))";

                { _ = (Func<sbyte, sbyte>)(x => x++); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => x++)";

                { _ = (Func<sbyte, sbyte>)(x => x--); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => x--)";

                { _ = (Func<sbyte, sbyte>)(x => ++x); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => ++x)";

                { _ = (Func<sbyte, sbyte>)(x => --x); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => --x)";

                { _ = (Func<sbyte, sbyte>)(x => checked(x++)); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => checked(x++))";

                { _ = (Func<sbyte, sbyte>)(x => checked(x--)); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => checked(x--))";

                { _ = (Func<sbyte, sbyte>)(x => checked(++x)); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => checked(++x))";

                { _ = (Func<sbyte, sbyte>)(x => checked(--x)); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => checked(--x))";

                { _ = (Func<short, short>)(x => x++); }
                yield return "(Expression<Func<short, short>>)(x => x++)";

                { _ = (Func<short, short>)(x => x--); }
                yield return "(Expression<Func<short, short>>)(x => x--)";

                { _ = (Func<short, short>)(x => ++x); }
                yield return "(Expression<Func<short, short>>)(x => ++x)";

                { _ = (Func<short, short>)(x => --x); }
                yield return "(Expression<Func<short, short>>)(x => --x)";

                { _ = (Func<short, short>)(x => checked(x++)); }
                yield return "(Expression<Func<short, short>>)(x => checked(x++))";

                { _ = (Func<short, short>)(x => checked(x--)); }
                yield return "(Expression<Func<short, short>>)(x => checked(x--))";

                { _ = (Func<short, short>)(x => checked(++x)); }
                yield return "(Expression<Func<short, short>>)(x => checked(++x))";

                { _ = (Func<short, short>)(x => checked(--x)); }
                yield return "(Expression<Func<short, short>>)(x => checked(--x))";

                { _ = (Func<ushort, ushort>)(x => x++); }
                yield return "(Expression<Func<ushort, ushort>>)(x => x++)";

                { _ = (Func<ushort, ushort>)(x => x--); }
                yield return "(Expression<Func<ushort, ushort>>)(x => x--)";

                { _ = (Func<ushort, ushort>)(x => ++x); }
                yield return "(Expression<Func<ushort, ushort>>)(x => ++x)";

                { _ = (Func<ushort, ushort>)(x => --x); }
                yield return "(Expression<Func<ushort, ushort>>)(x => --x)";

                { _ = (Func<ushort, ushort>)(x => checked(x++)); }
                yield return "(Expression<Func<ushort, ushort>>)(x => checked(x++))";

                { _ = (Func<ushort, ushort>)(x => checked(x--)); }
                yield return "(Expression<Func<ushort, ushort>>)(x => checked(x--))";

                { _ = (Func<ushort, ushort>)(x => checked(++x)); }
                yield return "(Expression<Func<ushort, ushort>>)(x => checked(++x))";

                { _ = (Func<ushort, ushort>)(x => checked(--x)); }
                yield return "(Expression<Func<ushort, ushort>>)(x => checked(--x))";

                { _ = (Func<char, char>)(x => x++); }
                yield return "(Expression<Func<char, char>>)(x => x++)";

                { _ = (Func<char, char>)(x => x--); }
                yield return "(Expression<Func<char, char>>)(x => x--)";

                { _ = (Func<char, char>)(x => ++x); }
                yield return "(Expression<Func<char, char>>)(x => ++x)";

                { _ = (Func<char, char>)(x => --x); }
                yield return "(Expression<Func<char, char>>)(x => --x)";

                { _ = (Func<char, char>)(x => checked(x++)); }
                yield return "(Expression<Func<char, char>>)(x => checked(x++))";

                { _ = (Func<char, char>)(x => checked(x--)); }
                yield return "(Expression<Func<char, char>>)(x => checked(x--))";

                { _ = (Func<char, char>)(x => checked(++x)); }
                yield return "(Expression<Func<char, char>>)(x => checked(++x))";

                { _ = (Func<char, char>)(x => checked(--x)); }
                yield return "(Expression<Func<char, char>>)(x => checked(--x))";

                { _ = (Func<int, int>)(x => x++); }
                yield return "(Expression<Func<int, int>>)(x => x++)";

                { _ = (Func<int, int>)(x => x--); }
                yield return "(Expression<Func<int, int>>)(x => x--)";

                { _ = (Func<int, int>)(x => ++x); }
                yield return "(Expression<Func<int, int>>)(x => ++x)";

                { _ = (Func<int, int>)(x => --x); }
                yield return "(Expression<Func<int, int>>)(x => --x)";

                { _ = (Func<int, int>)(x => checked(x++)); }
                yield return "(Expression<Func<int, int>>)(x => checked(x++))";

                { _ = (Func<int, int>)(x => checked(x--)); }
                yield return "(Expression<Func<int, int>>)(x => checked(x--))";

                { _ = (Func<int, int>)(x => checked(++x)); }
                yield return "(Expression<Func<int, int>>)(x => checked(++x))";

                { _ = (Func<int, int>)(x => checked(--x)); }
                yield return "(Expression<Func<int, int>>)(x => checked(--x))";

                { _ = (Func<uint, uint>)(x => x++); }
                yield return "(Expression<Func<uint, uint>>)(x => x++)";

                { _ = (Func<uint, uint>)(x => x--); }
                yield return "(Expression<Func<uint, uint>>)(x => x--)";

                { _ = (Func<uint, uint>)(x => ++x); }
                yield return "(Expression<Func<uint, uint>>)(x => ++x)";

                { _ = (Func<uint, uint>)(x => --x); }
                yield return "(Expression<Func<uint, uint>>)(x => --x)";

                { _ = (Func<uint, uint>)(x => checked(x++)); }
                yield return "(Expression<Func<uint, uint>>)(x => checked(x++))";

                { _ = (Func<uint, uint>)(x => checked(x--)); }
                yield return "(Expression<Func<uint, uint>>)(x => checked(x--))";

                { _ = (Func<uint, uint>)(x => checked(++x)); }
                yield return "(Expression<Func<uint, uint>>)(x => checked(++x))";

                { _ = (Func<uint, uint>)(x => checked(--x)); }
                yield return "(Expression<Func<uint, uint>>)(x => checked(--x))";

                { _ = (Func<long, long>)(x => x++); }
                yield return "(Expression<Func<long, long>>)(x => x++)";

                { _ = (Func<long, long>)(x => x--); }
                yield return "(Expression<Func<long, long>>)(x => x--)";

                { _ = (Func<long, long>)(x => ++x); }
                yield return "(Expression<Func<long, long>>)(x => ++x)";

                { _ = (Func<long, long>)(x => --x); }
                yield return "(Expression<Func<long, long>>)(x => --x)";

                { _ = (Func<long, long>)(x => checked(x++)); }
                yield return "(Expression<Func<long, long>>)(x => checked(x++))";

                { _ = (Func<long, long>)(x => checked(x--)); }
                yield return "(Expression<Func<long, long>>)(x => checked(x--))";

                { _ = (Func<long, long>)(x => checked(++x)); }
                yield return "(Expression<Func<long, long>>)(x => checked(++x))";

                { _ = (Func<long, long>)(x => checked(--x)); }
                yield return "(Expression<Func<long, long>>)(x => checked(--x))";

                { _ = (Func<ulong, ulong>)(x => x++); }
                yield return "(Expression<Func<ulong, ulong>>)(x => x++)";

                { _ = (Func<ulong, ulong>)(x => x--); }
                yield return "(Expression<Func<ulong, ulong>>)(x => x--)";

                { _ = (Func<ulong, ulong>)(x => ++x); }
                yield return "(Expression<Func<ulong, ulong>>)(x => ++x)";

                { _ = (Func<ulong, ulong>)(x => --x); }
                yield return "(Expression<Func<ulong, ulong>>)(x => --x)";

                { _ = (Func<ulong, ulong>)(x => checked(x++)); }
                yield return "(Expression<Func<ulong, ulong>>)(x => checked(x++))";

                { _ = (Func<ulong, ulong>)(x => checked(x--)); }
                yield return "(Expression<Func<ulong, ulong>>)(x => checked(x--))";

                { _ = (Func<ulong, ulong>)(x => checked(++x)); }
                yield return "(Expression<Func<ulong, ulong>>)(x => checked(++x))";

                { _ = (Func<ulong, ulong>)(x => checked(--x)); }
                yield return "(Expression<Func<ulong, ulong>>)(x => checked(--x))";

                { _ = (Func<float, float>)(x => x++); }
                yield return "(Expression<Func<float, float>>)(x => x++)";

                { _ = (Func<float, float>)(x => x--); }
                yield return "(Expression<Func<float, float>>)(x => x--)";

                { _ = (Func<float, float>)(x => ++x); }
                yield return "(Expression<Func<float, float>>)(x => ++x)";

                { _ = (Func<float, float>)(x => --x); }
                yield return "(Expression<Func<float, float>>)(x => --x)";

                { _ = (Func<float, float>)(x => checked(x++)); }
                yield return "(Expression<Func<float, float>>)(x => checked(x++))";

                { _ = (Func<float, float>)(x => checked(x--)); }
                yield return "(Expression<Func<float, float>>)(x => checked(x--))";

                { _ = (Func<float, float>)(x => checked(++x)); }
                yield return "(Expression<Func<float, float>>)(x => checked(++x))";

                { _ = (Func<float, float>)(x => checked(--x)); }
                yield return "(Expression<Func<float, float>>)(x => checked(--x))";

                { _ = (Func<double, double>)(x => x++); }
                yield return "(Expression<Func<double, double>>)(x => x++)";

                { _ = (Func<double, double>)(x => x--); }
                yield return "(Expression<Func<double, double>>)(x => x--)";

                { _ = (Func<double, double>)(x => ++x); }
                yield return "(Expression<Func<double, double>>)(x => ++x)";

                { _ = (Func<double, double>)(x => --x); }
                yield return "(Expression<Func<double, double>>)(x => --x)";

                { _ = (Func<double, double>)(x => checked(x++)); }
                yield return "(Expression<Func<double, double>>)(x => checked(x++))";

                { _ = (Func<double, double>)(x => checked(x--)); }
                yield return "(Expression<Func<double, double>>)(x => checked(x--))";

                { _ = (Func<double, double>)(x => checked(++x)); }
                yield return "(Expression<Func<double, double>>)(x => checked(++x))";

                { _ = (Func<double, double>)(x => checked(--x)); }
                yield return "(Expression<Func<double, double>>)(x => checked(--x))";

                { _ = (Func<decimal, decimal>)(x => x++); }
                yield return "(Expression<Func<decimal, decimal>>)(x => x++)";

                { _ = (Func<decimal, decimal>)(x => x--); }
                yield return "(Expression<Func<decimal, decimal>>)(x => x--)";

                { _ = (Func<decimal, decimal>)(x => ++x); }
                yield return "(Expression<Func<decimal, decimal>>)(x => ++x)";

                { _ = (Func<decimal, decimal>)(x => --x); }
                yield return "(Expression<Func<decimal, decimal>>)(x => --x)";

                { _ = (Func<decimal, decimal>)(x => checked(x++)); }
                yield return "(Expression<Func<decimal, decimal>>)(x => checked(x++))";

                { _ = (Func<decimal, decimal>)(x => checked(x--)); }
                yield return "(Expression<Func<decimal, decimal>>)(x => checked(x--))";

                { _ = (Func<decimal, decimal>)(x => checked(++x)); }
                yield return "(Expression<Func<decimal, decimal>>)(x => checked(++x))";

                { _ = (Func<decimal, decimal>)(x => checked(--x)); }
                yield return "(Expression<Func<decimal, decimal>>)(x => checked(--x))";

            }
        }
    }
}