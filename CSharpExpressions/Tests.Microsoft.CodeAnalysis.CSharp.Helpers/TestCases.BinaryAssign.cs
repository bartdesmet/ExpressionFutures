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
        public static IEnumerable<string> BinaryAssign
        {
            get
            {
#pragma warning disable 0675

                { var f = (Func<byte, byte, byte>)((x, y) => x += y); }
                yield return "(Expression<Func<byte, byte, byte>>)((x, y) => x += y)";

                { var f = (Func<byte, byte, byte>)((x, y) => x -= y); }
                yield return "(Expression<Func<byte, byte, byte>>)((x, y) => x -= y)";

                { var f = (Func<byte, byte, byte>)((x, y) => x *= y); }
                yield return "(Expression<Func<byte, byte, byte>>)((x, y) => x *= y)";

                { var f = (Func<byte, byte, byte>)((x, y) => x /= y); }
                yield return "(Expression<Func<byte, byte, byte>>)((x, y) => x /= y)";

                { var f = (Func<byte, byte, byte>)((x, y) => x %= y); }
                yield return "(Expression<Func<byte, byte, byte>>)((x, y) => x %= y)";

                { var f = (Func<byte, byte, byte>)((x, y) => x &= y); }
                yield return "(Expression<Func<byte, byte, byte>>)((x, y) => x &= y)";

                { var f = (Func<byte, byte, byte>)((x, y) => x |= y); }
                yield return "(Expression<Func<byte, byte, byte>>)((x, y) => x |= y)";

                { var f = (Func<byte, byte, byte>)((x, y) => x ^= y); }
                yield return "(Expression<Func<byte, byte, byte>>)((x, y) => x ^= y)";

                { var f = (Func<byte, byte, byte>)((x, y) => checked(x += y)); }
                yield return "(Expression<Func<byte, byte, byte>>)((x, y) => checked(x += y))";

                { var f = (Func<byte, byte, byte>)((x, y) => checked(x -= y)); }
                yield return "(Expression<Func<byte, byte, byte>>)((x, y) => checked(x -= y))";

                { var f = (Func<byte, byte, byte>)((x, y) => checked(x *= y)); }
                yield return "(Expression<Func<byte, byte, byte>>)((x, y) => checked(x *= y))";

                { var f = (Func<sbyte, sbyte, sbyte>)((x, y) => x += y); }
                yield return "(Expression<Func<sbyte, sbyte, sbyte>>)((x, y) => x += y)";

                { var f = (Func<sbyte, sbyte, sbyte>)((x, y) => x -= y); }
                yield return "(Expression<Func<sbyte, sbyte, sbyte>>)((x, y) => x -= y)";

                { var f = (Func<sbyte, sbyte, sbyte>)((x, y) => x *= y); }
                yield return "(Expression<Func<sbyte, sbyte, sbyte>>)((x, y) => x *= y)";

                { var f = (Func<sbyte, sbyte, sbyte>)((x, y) => x /= y); }
                yield return "(Expression<Func<sbyte, sbyte, sbyte>>)((x, y) => x /= y)";

                { var f = (Func<sbyte, sbyte, sbyte>)((x, y) => x %= y); }
                yield return "(Expression<Func<sbyte, sbyte, sbyte>>)((x, y) => x %= y)";

                { var f = (Func<sbyte, sbyte, sbyte>)((x, y) => x &= y); }
                yield return "(Expression<Func<sbyte, sbyte, sbyte>>)((x, y) => x &= y)";

                { var f = (Func<sbyte, sbyte, sbyte>)((x, y) => x |= y); }
                yield return "(Expression<Func<sbyte, sbyte, sbyte>>)((x, y) => x |= y)";

                { var f = (Func<sbyte, sbyte, sbyte>)((x, y) => x ^= y); }
                yield return "(Expression<Func<sbyte, sbyte, sbyte>>)((x, y) => x ^= y)";

                { var f = (Func<sbyte, sbyte, sbyte>)((x, y) => checked(x += y)); }
                yield return "(Expression<Func<sbyte, sbyte, sbyte>>)((x, y) => checked(x += y))";

                { var f = (Func<sbyte, sbyte, sbyte>)((x, y) => checked(x -= y)); }
                yield return "(Expression<Func<sbyte, sbyte, sbyte>>)((x, y) => checked(x -= y))";

                { var f = (Func<sbyte, sbyte, sbyte>)((x, y) => checked(x *= y)); }
                yield return "(Expression<Func<sbyte, sbyte, sbyte>>)((x, y) => checked(x *= y))";

                { var f = (Func<short, short, short>)((x, y) => x += y); }
                yield return "(Expression<Func<short, short, short>>)((x, y) => x += y)";

                { var f = (Func<short, short, short>)((x, y) => x -= y); }
                yield return "(Expression<Func<short, short, short>>)((x, y) => x -= y)";

                { var f = (Func<short, short, short>)((x, y) => x *= y); }
                yield return "(Expression<Func<short, short, short>>)((x, y) => x *= y)";

                { var f = (Func<short, short, short>)((x, y) => x /= y); }
                yield return "(Expression<Func<short, short, short>>)((x, y) => x /= y)";

                { var f = (Func<short, short, short>)((x, y) => x %= y); }
                yield return "(Expression<Func<short, short, short>>)((x, y) => x %= y)";

                { var f = (Func<short, short, short>)((x, y) => x &= y); }
                yield return "(Expression<Func<short, short, short>>)((x, y) => x &= y)";

                { var f = (Func<short, short, short>)((x, y) => x |= y); }
                yield return "(Expression<Func<short, short, short>>)((x, y) => x |= y)";

                { var f = (Func<short, short, short>)((x, y) => x ^= y); }
                yield return "(Expression<Func<short, short, short>>)((x, y) => x ^= y)";

                { var f = (Func<short, short, short>)((x, y) => checked(x += y)); }
                yield return "(Expression<Func<short, short, short>>)((x, y) => checked(x += y))";

                { var f = (Func<short, short, short>)((x, y) => checked(x -= y)); }
                yield return "(Expression<Func<short, short, short>>)((x, y) => checked(x -= y))";

                { var f = (Func<short, short, short>)((x, y) => checked(x *= y)); }
                yield return "(Expression<Func<short, short, short>>)((x, y) => checked(x *= y))";

                { var f = (Func<ushort, ushort, ushort>)((x, y) => x += y); }
                yield return "(Expression<Func<ushort, ushort, ushort>>)((x, y) => x += y)";

                { var f = (Func<ushort, ushort, ushort>)((x, y) => x -= y); }
                yield return "(Expression<Func<ushort, ushort, ushort>>)((x, y) => x -= y)";

                { var f = (Func<ushort, ushort, ushort>)((x, y) => x *= y); }
                yield return "(Expression<Func<ushort, ushort, ushort>>)((x, y) => x *= y)";

                { var f = (Func<ushort, ushort, ushort>)((x, y) => x /= y); }
                yield return "(Expression<Func<ushort, ushort, ushort>>)((x, y) => x /= y)";

                { var f = (Func<ushort, ushort, ushort>)((x, y) => x %= y); }
                yield return "(Expression<Func<ushort, ushort, ushort>>)((x, y) => x %= y)";

                { var f = (Func<ushort, ushort, ushort>)((x, y) => x &= y); }
                yield return "(Expression<Func<ushort, ushort, ushort>>)((x, y) => x &= y)";

                { var f = (Func<ushort, ushort, ushort>)((x, y) => x |= y); }
                yield return "(Expression<Func<ushort, ushort, ushort>>)((x, y) => x |= y)";

                { var f = (Func<ushort, ushort, ushort>)((x, y) => x ^= y); }
                yield return "(Expression<Func<ushort, ushort, ushort>>)((x, y) => x ^= y)";

                { var f = (Func<ushort, ushort, ushort>)((x, y) => checked(x += y)); }
                yield return "(Expression<Func<ushort, ushort, ushort>>)((x, y) => checked(x += y))";

                { var f = (Func<ushort, ushort, ushort>)((x, y) => checked(x -= y)); }
                yield return "(Expression<Func<ushort, ushort, ushort>>)((x, y) => checked(x -= y))";

                { var f = (Func<ushort, ushort, ushort>)((x, y) => checked(x *= y)); }
                yield return "(Expression<Func<ushort, ushort, ushort>>)((x, y) => checked(x *= y))";

                { var f = (Func<char, char, char>)((x, y) => x += y); }
                yield return "(Expression<Func<char, char, char>>)((x, y) => x += y)";

                { var f = (Func<char, char, char>)((x, y) => x -= y); }
                yield return "(Expression<Func<char, char, char>>)((x, y) => x -= y)";

                { var f = (Func<char, char, char>)((x, y) => x *= y); }
                yield return "(Expression<Func<char, char, char>>)((x, y) => x *= y)";

                { var f = (Func<char, char, char>)((x, y) => x /= y); }
                yield return "(Expression<Func<char, char, char>>)((x, y) => x /= y)";

                { var f = (Func<char, char, char>)((x, y) => x %= y); }
                yield return "(Expression<Func<char, char, char>>)((x, y) => x %= y)";

                { var f = (Func<char, char, char>)((x, y) => x &= y); }
                yield return "(Expression<Func<char, char, char>>)((x, y) => x &= y)";

                { var f = (Func<char, char, char>)((x, y) => x |= y); }
                yield return "(Expression<Func<char, char, char>>)((x, y) => x |= y)";

                { var f = (Func<char, char, char>)((x, y) => x ^= y); }
                yield return "(Expression<Func<char, char, char>>)((x, y) => x ^= y)";

                { var f = (Func<char, char, char>)((x, y) => checked(x += y)); }
                yield return "(Expression<Func<char, char, char>>)((x, y) => checked(x += y))";

                { var f = (Func<char, char, char>)((x, y) => checked(x -= y)); }
                yield return "(Expression<Func<char, char, char>>)((x, y) => checked(x -= y))";

                { var f = (Func<char, char, char>)((x, y) => checked(x *= y)); }
                yield return "(Expression<Func<char, char, char>>)((x, y) => checked(x *= y))";

                { var f = (Func<int, int, int>)((x, y) => x += y); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => x += y)";

                { var f = (Func<int, int, int>)((x, y) => x -= y); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => x -= y)";

                { var f = (Func<int, int, int>)((x, y) => x *= y); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => x *= y)";

                { var f = (Func<int, int, int>)((x, y) => x /= y); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => x /= y)";

                { var f = (Func<int, int, int>)((x, y) => x %= y); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => x %= y)";

                { var f = (Func<int, int, int>)((x, y) => x &= y); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => x &= y)";

                { var f = (Func<int, int, int>)((x, y) => x |= y); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => x |= y)";

                { var f = (Func<int, int, int>)((x, y) => x ^= y); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => x ^= y)";

                { var f = (Func<int, int, int>)((x, y) => checked(x += y)); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => checked(x += y))";

                { var f = (Func<int, int, int>)((x, y) => checked(x -= y)); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => checked(x -= y))";

                { var f = (Func<int, int, int>)((x, y) => checked(x *= y)); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => checked(x *= y))";

                { var f = (Func<int, int, int>)((x, y) => x <<= y); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => x <<= y)";

                { var f = (Func<int, int, int>)((x, y) => x >>= y); }
                yield return "(Expression<Func<int, int, int>>)((x, y) => x >>= y)";

                { var f = (Func<uint, uint, uint>)((x, y) => x += y); }
                yield return "(Expression<Func<uint, uint, uint>>)((x, y) => x += y)";

                { var f = (Func<uint, uint, uint>)((x, y) => x -= y); }
                yield return "(Expression<Func<uint, uint, uint>>)((x, y) => x -= y)";

                { var f = (Func<uint, uint, uint>)((x, y) => x *= y); }
                yield return "(Expression<Func<uint, uint, uint>>)((x, y) => x *= y)";

                { var f = (Func<uint, uint, uint>)((x, y) => x /= y); }
                yield return "(Expression<Func<uint, uint, uint>>)((x, y) => x /= y)";

                { var f = (Func<uint, uint, uint>)((x, y) => x %= y); }
                yield return "(Expression<Func<uint, uint, uint>>)((x, y) => x %= y)";

                { var f = (Func<uint, uint, uint>)((x, y) => x &= y); }
                yield return "(Expression<Func<uint, uint, uint>>)((x, y) => x &= y)";

                { var f = (Func<uint, uint, uint>)((x, y) => x |= y); }
                yield return "(Expression<Func<uint, uint, uint>>)((x, y) => x |= y)";

                { var f = (Func<uint, uint, uint>)((x, y) => x ^= y); }
                yield return "(Expression<Func<uint, uint, uint>>)((x, y) => x ^= y)";

                { var f = (Func<uint, uint, uint>)((x, y) => checked(x += y)); }
                yield return "(Expression<Func<uint, uint, uint>>)((x, y) => checked(x += y))";

                { var f = (Func<uint, uint, uint>)((x, y) => checked(x -= y)); }
                yield return "(Expression<Func<uint, uint, uint>>)((x, y) => checked(x -= y))";

                { var f = (Func<uint, uint, uint>)((x, y) => checked(x *= y)); }
                yield return "(Expression<Func<uint, uint, uint>>)((x, y) => checked(x *= y))";

                { var f = (Func<uint, int, uint>)((x, y) => x <<= y); }
                yield return "(Expression<Func<uint, int, uint>>)((x, y) => x <<= y)";

                { var f = (Func<uint, int, uint>)((x, y) => x >>= y); }
                yield return "(Expression<Func<uint, int, uint>>)((x, y) => x >>= y)";

                { var f = (Func<long, long, long>)((x, y) => x += y); }
                yield return "(Expression<Func<long, long, long>>)((x, y) => x += y)";

                { var f = (Func<long, long, long>)((x, y) => x -= y); }
                yield return "(Expression<Func<long, long, long>>)((x, y) => x -= y)";

                { var f = (Func<long, long, long>)((x, y) => x *= y); }
                yield return "(Expression<Func<long, long, long>>)((x, y) => x *= y)";

                { var f = (Func<long, long, long>)((x, y) => x /= y); }
                yield return "(Expression<Func<long, long, long>>)((x, y) => x /= y)";

                { var f = (Func<long, long, long>)((x, y) => x %= y); }
                yield return "(Expression<Func<long, long, long>>)((x, y) => x %= y)";

                { var f = (Func<long, long, long>)((x, y) => x &= y); }
                yield return "(Expression<Func<long, long, long>>)((x, y) => x &= y)";

                { var f = (Func<long, long, long>)((x, y) => x |= y); }
                yield return "(Expression<Func<long, long, long>>)((x, y) => x |= y)";

                { var f = (Func<long, long, long>)((x, y) => x ^= y); }
                yield return "(Expression<Func<long, long, long>>)((x, y) => x ^= y)";

                { var f = (Func<long, long, long>)((x, y) => checked(x += y)); }
                yield return "(Expression<Func<long, long, long>>)((x, y) => checked(x += y))";

                { var f = (Func<long, long, long>)((x, y) => checked(x -= y)); }
                yield return "(Expression<Func<long, long, long>>)((x, y) => checked(x -= y))";

                { var f = (Func<long, long, long>)((x, y) => checked(x *= y)); }
                yield return "(Expression<Func<long, long, long>>)((x, y) => checked(x *= y))";

                { var f = (Func<long, int, long>)((x, y) => x <<= y); }
                yield return "(Expression<Func<long, int, long>>)((x, y) => x <<= y)";

                { var f = (Func<long, int, long>)((x, y) => x >>= y); }
                yield return "(Expression<Func<long, int, long>>)((x, y) => x >>= y)";

                { var f = (Func<ulong, ulong, ulong>)((x, y) => x += y); }
                yield return "(Expression<Func<ulong, ulong, ulong>>)((x, y) => x += y)";

                { var f = (Func<ulong, ulong, ulong>)((x, y) => x -= y); }
                yield return "(Expression<Func<ulong, ulong, ulong>>)((x, y) => x -= y)";

                { var f = (Func<ulong, ulong, ulong>)((x, y) => x *= y); }
                yield return "(Expression<Func<ulong, ulong, ulong>>)((x, y) => x *= y)";

                { var f = (Func<ulong, ulong, ulong>)((x, y) => x /= y); }
                yield return "(Expression<Func<ulong, ulong, ulong>>)((x, y) => x /= y)";

                { var f = (Func<ulong, ulong, ulong>)((x, y) => x %= y); }
                yield return "(Expression<Func<ulong, ulong, ulong>>)((x, y) => x %= y)";

                { var f = (Func<ulong, ulong, ulong>)((x, y) => x &= y); }
                yield return "(Expression<Func<ulong, ulong, ulong>>)((x, y) => x &= y)";

                { var f = (Func<ulong, ulong, ulong>)((x, y) => x |= y); }
                yield return "(Expression<Func<ulong, ulong, ulong>>)((x, y) => x |= y)";

                { var f = (Func<ulong, ulong, ulong>)((x, y) => x ^= y); }
                yield return "(Expression<Func<ulong, ulong, ulong>>)((x, y) => x ^= y)";

                { var f = (Func<ulong, ulong, ulong>)((x, y) => checked(x += y)); }
                yield return "(Expression<Func<ulong, ulong, ulong>>)((x, y) => checked(x += y))";

                { var f = (Func<ulong, ulong, ulong>)((x, y) => checked(x -= y)); }
                yield return "(Expression<Func<ulong, ulong, ulong>>)((x, y) => checked(x -= y))";

                { var f = (Func<ulong, ulong, ulong>)((x, y) => checked(x *= y)); }
                yield return "(Expression<Func<ulong, ulong, ulong>>)((x, y) => checked(x *= y))";

                { var f = (Func<ulong, int, ulong>)((x, y) => x <<= y); }
                yield return "(Expression<Func<ulong, int, ulong>>)((x, y) => x <<= y)";

                { var f = (Func<ulong, int, ulong>)((x, y) => x >>= y); }
                yield return "(Expression<Func<ulong, int, ulong>>)((x, y) => x >>= y)";

                { var f = (Func<float, float, float>)((x, y) => x += y); }
                yield return "(Expression<Func<float, float, float>>)((x, y) => x += y)";

                { var f = (Func<float, float, float>)((x, y) => x -= y); }
                yield return "(Expression<Func<float, float, float>>)((x, y) => x -= y)";

                { var f = (Func<float, float, float>)((x, y) => x *= y); }
                yield return "(Expression<Func<float, float, float>>)((x, y) => x *= y)";

                { var f = (Func<float, float, float>)((x, y) => x /= y); }
                yield return "(Expression<Func<float, float, float>>)((x, y) => x /= y)";

                { var f = (Func<float, float, float>)((x, y) => x %= y); }
                yield return "(Expression<Func<float, float, float>>)((x, y) => x %= y)";

                { var f = (Func<float, float, float>)((x, y) => checked(x += y)); }
                yield return "(Expression<Func<float, float, float>>)((x, y) => checked(x += y))";

                { var f = (Func<float, float, float>)((x, y) => checked(x -= y)); }
                yield return "(Expression<Func<float, float, float>>)((x, y) => checked(x -= y))";

                { var f = (Func<float, float, float>)((x, y) => checked(x *= y)); }
                yield return "(Expression<Func<float, float, float>>)((x, y) => checked(x *= y))";

                { var f = (Func<double, double, double>)((x, y) => x += y); }
                yield return "(Expression<Func<double, double, double>>)((x, y) => x += y)";

                { var f = (Func<double, double, double>)((x, y) => x -= y); }
                yield return "(Expression<Func<double, double, double>>)((x, y) => x -= y)";

                { var f = (Func<double, double, double>)((x, y) => x *= y); }
                yield return "(Expression<Func<double, double, double>>)((x, y) => x *= y)";

                { var f = (Func<double, double, double>)((x, y) => x /= y); }
                yield return "(Expression<Func<double, double, double>>)((x, y) => x /= y)";

                { var f = (Func<double, double, double>)((x, y) => x %= y); }
                yield return "(Expression<Func<double, double, double>>)((x, y) => x %= y)";

                { var f = (Func<double, double, double>)((x, y) => checked(x += y)); }
                yield return "(Expression<Func<double, double, double>>)((x, y) => checked(x += y))";

                { var f = (Func<double, double, double>)((x, y) => checked(x -= y)); }
                yield return "(Expression<Func<double, double, double>>)((x, y) => checked(x -= y))";

                { var f = (Func<double, double, double>)((x, y) => checked(x *= y)); }
                yield return "(Expression<Func<double, double, double>>)((x, y) => checked(x *= y))";

                { var f = (Func<decimal, decimal, decimal>)((x, y) => x += y); }
                yield return "(Expression<Func<decimal, decimal, decimal>>)((x, y) => x += y)";

                { var f = (Func<decimal, decimal, decimal>)((x, y) => x -= y); }
                yield return "(Expression<Func<decimal, decimal, decimal>>)((x, y) => x -= y)";

                { var f = (Func<decimal, decimal, decimal>)((x, y) => x *= y); }
                yield return "(Expression<Func<decimal, decimal, decimal>>)((x, y) => x *= y)";

                { var f = (Func<decimal, decimal, decimal>)((x, y) => x /= y); }
                yield return "(Expression<Func<decimal, decimal, decimal>>)((x, y) => x /= y)";

                { var f = (Func<decimal, decimal, decimal>)((x, y) => x %= y); }
                yield return "(Expression<Func<decimal, decimal, decimal>>)((x, y) => x %= y)";

                { var f = (Func<decimal, decimal, decimal>)((x, y) => checked(x += y)); }
                yield return "(Expression<Func<decimal, decimal, decimal>>)((x, y) => checked(x += y))";

                { var f = (Func<decimal, decimal, decimal>)((x, y) => checked(x -= y)); }
                yield return "(Expression<Func<decimal, decimal, decimal>>)((x, y) => checked(x -= y))";

                { var f = (Func<decimal, decimal, decimal>)((x, y) => checked(x *= y)); }
                yield return "(Expression<Func<decimal, decimal, decimal>>)((x, y) => checked(x *= y))";

#pragma warning restore 0675
            }
        }
    }
}