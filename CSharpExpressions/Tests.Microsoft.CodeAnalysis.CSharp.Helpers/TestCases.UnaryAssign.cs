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
                { var f = (Func<byte, byte>)(x => x++); }
                yield return "(Expression<Func<byte, byte>>)(x => x++)";

                { var f = (Func<byte, byte>)(x => x--); }
                yield return "(Expression<Func<byte, byte>>)(x => x--)";

                { var f = (Func<byte, byte>)(x => ++x); }
                yield return "(Expression<Func<byte, byte>>)(x => ++x)";

                { var f = (Func<byte, byte>)(x => --x); }
                yield return "(Expression<Func<byte, byte>>)(x => --x)";

                { var f = (Func<byte, byte>)(x => checked(x++)); }
                yield return "(Expression<Func<byte, byte>>)(x => checked(x++))";

                { var f = (Func<byte, byte>)(x => checked(x--)); }
                yield return "(Expression<Func<byte, byte>>)(x => checked(x--))";

                { var f = (Func<byte, byte>)(x => checked(++x)); }
                yield return "(Expression<Func<byte, byte>>)(x => checked(++x))";

                { var f = (Func<byte, byte>)(x => checked(--x)); }
                yield return "(Expression<Func<byte, byte>>)(x => checked(--x))";

                { var f = (Func<sbyte, sbyte>)(x => x++); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => x++)";

                { var f = (Func<sbyte, sbyte>)(x => x--); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => x--)";

                { var f = (Func<sbyte, sbyte>)(x => ++x); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => ++x)";

                { var f = (Func<sbyte, sbyte>)(x => --x); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => --x)";

                { var f = (Func<sbyte, sbyte>)(x => checked(x++)); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => checked(x++))";

                { var f = (Func<sbyte, sbyte>)(x => checked(x--)); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => checked(x--))";

                { var f = (Func<sbyte, sbyte>)(x => checked(++x)); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => checked(++x))";

                { var f = (Func<sbyte, sbyte>)(x => checked(--x)); }
                yield return "(Expression<Func<sbyte, sbyte>>)(x => checked(--x))";

                { var f = (Func<short, short>)(x => x++); }
                yield return "(Expression<Func<short, short>>)(x => x++)";

                { var f = (Func<short, short>)(x => x--); }
                yield return "(Expression<Func<short, short>>)(x => x--)";

                { var f = (Func<short, short>)(x => ++x); }
                yield return "(Expression<Func<short, short>>)(x => ++x)";

                { var f = (Func<short, short>)(x => --x); }
                yield return "(Expression<Func<short, short>>)(x => --x)";

                { var f = (Func<short, short>)(x => checked(x++)); }
                yield return "(Expression<Func<short, short>>)(x => checked(x++))";

                { var f = (Func<short, short>)(x => checked(x--)); }
                yield return "(Expression<Func<short, short>>)(x => checked(x--))";

                { var f = (Func<short, short>)(x => checked(++x)); }
                yield return "(Expression<Func<short, short>>)(x => checked(++x))";

                { var f = (Func<short, short>)(x => checked(--x)); }
                yield return "(Expression<Func<short, short>>)(x => checked(--x))";

                { var f = (Func<ushort, ushort>)(x => x++); }
                yield return "(Expression<Func<ushort, ushort>>)(x => x++)";

                { var f = (Func<ushort, ushort>)(x => x--); }
                yield return "(Expression<Func<ushort, ushort>>)(x => x--)";

                { var f = (Func<ushort, ushort>)(x => ++x); }
                yield return "(Expression<Func<ushort, ushort>>)(x => ++x)";

                { var f = (Func<ushort, ushort>)(x => --x); }
                yield return "(Expression<Func<ushort, ushort>>)(x => --x)";

                { var f = (Func<ushort, ushort>)(x => checked(x++)); }
                yield return "(Expression<Func<ushort, ushort>>)(x => checked(x++))";

                { var f = (Func<ushort, ushort>)(x => checked(x--)); }
                yield return "(Expression<Func<ushort, ushort>>)(x => checked(x--))";

                { var f = (Func<ushort, ushort>)(x => checked(++x)); }
                yield return "(Expression<Func<ushort, ushort>>)(x => checked(++x))";

                { var f = (Func<ushort, ushort>)(x => checked(--x)); }
                yield return "(Expression<Func<ushort, ushort>>)(x => checked(--x))";

                { var f = (Func<char, char>)(x => x++); }
                yield return "(Expression<Func<char, char>>)(x => x++)";

                { var f = (Func<char, char>)(x => x--); }
                yield return "(Expression<Func<char, char>>)(x => x--)";

                { var f = (Func<char, char>)(x => ++x); }
                yield return "(Expression<Func<char, char>>)(x => ++x)";

                { var f = (Func<char, char>)(x => --x); }
                yield return "(Expression<Func<char, char>>)(x => --x)";

                { var f = (Func<char, char>)(x => checked(x++)); }
                yield return "(Expression<Func<char, char>>)(x => checked(x++))";

                { var f = (Func<char, char>)(x => checked(x--)); }
                yield return "(Expression<Func<char, char>>)(x => checked(x--))";

                { var f = (Func<char, char>)(x => checked(++x)); }
                yield return "(Expression<Func<char, char>>)(x => checked(++x))";

                { var f = (Func<char, char>)(x => checked(--x)); }
                yield return "(Expression<Func<char, char>>)(x => checked(--x))";

                { var f = (Func<int, int>)(x => x++); }
                yield return "(Expression<Func<int, int>>)(x => x++)";

                { var f = (Func<int, int>)(x => x--); }
                yield return "(Expression<Func<int, int>>)(x => x--)";

                { var f = (Func<int, int>)(x => ++x); }
                yield return "(Expression<Func<int, int>>)(x => ++x)";

                { var f = (Func<int, int>)(x => --x); }
                yield return "(Expression<Func<int, int>>)(x => --x)";

                { var f = (Func<int, int>)(x => checked(x++)); }
                yield return "(Expression<Func<int, int>>)(x => checked(x++))";

                { var f = (Func<int, int>)(x => checked(x--)); }
                yield return "(Expression<Func<int, int>>)(x => checked(x--))";

                { var f = (Func<int, int>)(x => checked(++x)); }
                yield return "(Expression<Func<int, int>>)(x => checked(++x))";

                { var f = (Func<int, int>)(x => checked(--x)); }
                yield return "(Expression<Func<int, int>>)(x => checked(--x))";

                { var f = (Func<uint, uint>)(x => x++); }
                yield return "(Expression<Func<uint, uint>>)(x => x++)";

                { var f = (Func<uint, uint>)(x => x--); }
                yield return "(Expression<Func<uint, uint>>)(x => x--)";

                { var f = (Func<uint, uint>)(x => ++x); }
                yield return "(Expression<Func<uint, uint>>)(x => ++x)";

                { var f = (Func<uint, uint>)(x => --x); }
                yield return "(Expression<Func<uint, uint>>)(x => --x)";

                { var f = (Func<uint, uint>)(x => checked(x++)); }
                yield return "(Expression<Func<uint, uint>>)(x => checked(x++))";

                { var f = (Func<uint, uint>)(x => checked(x--)); }
                yield return "(Expression<Func<uint, uint>>)(x => checked(x--))";

                { var f = (Func<uint, uint>)(x => checked(++x)); }
                yield return "(Expression<Func<uint, uint>>)(x => checked(++x))";

                { var f = (Func<uint, uint>)(x => checked(--x)); }
                yield return "(Expression<Func<uint, uint>>)(x => checked(--x))";

                { var f = (Func<long, long>)(x => x++); }
                yield return "(Expression<Func<long, long>>)(x => x++)";

                { var f = (Func<long, long>)(x => x--); }
                yield return "(Expression<Func<long, long>>)(x => x--)";

                { var f = (Func<long, long>)(x => ++x); }
                yield return "(Expression<Func<long, long>>)(x => ++x)";

                { var f = (Func<long, long>)(x => --x); }
                yield return "(Expression<Func<long, long>>)(x => --x)";

                { var f = (Func<long, long>)(x => checked(x++)); }
                yield return "(Expression<Func<long, long>>)(x => checked(x++))";

                { var f = (Func<long, long>)(x => checked(x--)); }
                yield return "(Expression<Func<long, long>>)(x => checked(x--))";

                { var f = (Func<long, long>)(x => checked(++x)); }
                yield return "(Expression<Func<long, long>>)(x => checked(++x))";

                { var f = (Func<long, long>)(x => checked(--x)); }
                yield return "(Expression<Func<long, long>>)(x => checked(--x))";

                { var f = (Func<ulong, ulong>)(x => x++); }
                yield return "(Expression<Func<ulong, ulong>>)(x => x++)";

                { var f = (Func<ulong, ulong>)(x => x--); }
                yield return "(Expression<Func<ulong, ulong>>)(x => x--)";

                { var f = (Func<ulong, ulong>)(x => ++x); }
                yield return "(Expression<Func<ulong, ulong>>)(x => ++x)";

                { var f = (Func<ulong, ulong>)(x => --x); }
                yield return "(Expression<Func<ulong, ulong>>)(x => --x)";

                { var f = (Func<ulong, ulong>)(x => checked(x++)); }
                yield return "(Expression<Func<ulong, ulong>>)(x => checked(x++))";

                { var f = (Func<ulong, ulong>)(x => checked(x--)); }
                yield return "(Expression<Func<ulong, ulong>>)(x => checked(x--))";

                { var f = (Func<ulong, ulong>)(x => checked(++x)); }
                yield return "(Expression<Func<ulong, ulong>>)(x => checked(++x))";

                { var f = (Func<ulong, ulong>)(x => checked(--x)); }
                yield return "(Expression<Func<ulong, ulong>>)(x => checked(--x))";

                { var f = (Func<float, float>)(x => x++); }
                yield return "(Expression<Func<float, float>>)(x => x++)";

                { var f = (Func<float, float>)(x => x--); }
                yield return "(Expression<Func<float, float>>)(x => x--)";

                { var f = (Func<float, float>)(x => ++x); }
                yield return "(Expression<Func<float, float>>)(x => ++x)";

                { var f = (Func<float, float>)(x => --x); }
                yield return "(Expression<Func<float, float>>)(x => --x)";

                { var f = (Func<float, float>)(x => checked(x++)); }
                yield return "(Expression<Func<float, float>>)(x => checked(x++))";

                { var f = (Func<float, float>)(x => checked(x--)); }
                yield return "(Expression<Func<float, float>>)(x => checked(x--))";

                { var f = (Func<float, float>)(x => checked(++x)); }
                yield return "(Expression<Func<float, float>>)(x => checked(++x))";

                { var f = (Func<float, float>)(x => checked(--x)); }
                yield return "(Expression<Func<float, float>>)(x => checked(--x))";

                { var f = (Func<double, double>)(x => x++); }
                yield return "(Expression<Func<double, double>>)(x => x++)";

                { var f = (Func<double, double>)(x => x--); }
                yield return "(Expression<Func<double, double>>)(x => x--)";

                { var f = (Func<double, double>)(x => ++x); }
                yield return "(Expression<Func<double, double>>)(x => ++x)";

                { var f = (Func<double, double>)(x => --x); }
                yield return "(Expression<Func<double, double>>)(x => --x)";

                { var f = (Func<double, double>)(x => checked(x++)); }
                yield return "(Expression<Func<double, double>>)(x => checked(x++))";

                { var f = (Func<double, double>)(x => checked(x--)); }
                yield return "(Expression<Func<double, double>>)(x => checked(x--))";

                { var f = (Func<double, double>)(x => checked(++x)); }
                yield return "(Expression<Func<double, double>>)(x => checked(++x))";

                { var f = (Func<double, double>)(x => checked(--x)); }
                yield return "(Expression<Func<double, double>>)(x => checked(--x))";

                { var f = (Func<decimal, decimal>)(x => x++); }
                yield return "(Expression<Func<decimal, decimal>>)(x => x++)";

                { var f = (Func<decimal, decimal>)(x => x--); }
                yield return "(Expression<Func<decimal, decimal>>)(x => x--)";

                { var f = (Func<decimal, decimal>)(x => ++x); }
                yield return "(Expression<Func<decimal, decimal>>)(x => ++x)";

                { var f = (Func<decimal, decimal>)(x => --x); }
                yield return "(Expression<Func<decimal, decimal>>)(x => --x)";

                { var f = (Func<decimal, decimal>)(x => checked(x++)); }
                yield return "(Expression<Func<decimal, decimal>>)(x => checked(x++))";

                { var f = (Func<decimal, decimal>)(x => checked(x--)); }
                yield return "(Expression<Func<decimal, decimal>>)(x => checked(x--))";

                { var f = (Func<decimal, decimal>)(x => checked(++x)); }
                yield return "(Expression<Func<decimal, decimal>>)(x => checked(++x))";

                { var f = (Func<decimal, decimal>)(x => checked(--x)); }
                yield return "(Expression<Func<decimal, decimal>>)(x => checked(--x))";

            }
        }
    }
}