// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    internal static partial class Helpers
    {
        private static List<Type>? s_tupleTypes;

        private static List<Type> TupleTypes => s_tupleTypes ??= new List<Type> {
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>),
        };

        private static Dictionary<Type, int>? s_tupleArities;

        private static Dictionary<Type, int> TupleArities => s_tupleArities ??= new Dictionary<Type, int> {
            { typeof(ValueTuple<>),        1 },
            { typeof(ValueTuple<,>),       2 },
            { typeof(ValueTuple<,,>),      3 },
            { typeof(ValueTuple<,,,>),     4 },
            { typeof(ValueTuple<,,,,>),    5 },
            { typeof(ValueTuple<,,,,,>),   6 },
            { typeof(ValueTuple<,,,,,,>),  7 },
            { typeof(ValueTuple<,,,,,,,>), 7 }, // NB: Not a mistake, there are only 7 values, and one rest slot.
        };

        private static string[]? s_tupleItemNames;

        private static string[] TupleItemNames => s_tupleItemNames ??= new[] {
            "Item1",
            "Item2",
            "Item3",
            "Item4",
            "Item5",
            "Item6",
            "Item7",
        };

        public static readonly Type MaxTupleType = typeof(ValueTuple<,,,,,,,>);

        public const int ValueTupleRestPosition = 8;

        public static bool IsTupleType(Type type)
        {
            if (!type.IsConstructedGenericType)
            {
                return false;
            }

            var def = type.GetGenericTypeDefinition();

            if (!TupleTypes.Contains(def))
            {
                return false;
            }

            if (def == MaxTupleType)
            {
                return IsTupleType(type.GetGenericArguments()[^1]);
            }

            return true;
        }

        public static int GetTupleArity(Type type)
        {
            Debug.Assert(IsTupleType(type));

            var def = type.GetGenericTypeDefinition();

            if (def == MaxTupleType)
            {
                return TupleArities[def] + GetTupleArity(type.GetGenericArguments()[^1]);
            }

            return TupleArities[def];
        }

        public static IEnumerable<Type> GetTupleComponentTypes(Type type)
        {
            Debug.Assert(IsTupleType(type));

            while (type != null)
            {
                var def = type.GetGenericTypeDefinition();
                var args = type.GetGenericArguments();

                if (def == MaxTupleType)
                {
                    for (int i = 0; i < args.Length - 1; i++)
                    {
                        yield return args[i];
                    }

                    type = args[^1];
                }
                else
                {
                    foreach (var arg in args)
                    {
                        yield return arg;
                    }

                    yield break;
                }
            }
        }

        public static Expression GetTupleItemAccess(Expression tuple, int item)
        {
            while (true)
            {
                if (item < TupleItemNames.Length)
                {
                    return Expression.Field(tuple, TupleItemNames[item]);
                }
                else
                {
                    tuple = Expression.Field(tuple, "Rest");
                    item -= TupleItemNames.Length;
                }
            }
        }

        private static int NumberOfValueTuples(int numElements, out int remainder)
        {
            remainder = (numElements - 1) % (ValueTupleRestPosition - 1) + 1;

            return (numElements - 1) / (ValueTupleRestPosition - 1) + 1;
        }

        public static Type MakeTupleType(Type[] elementTypes)
        {
            int chainLength = NumberOfValueTuples(elementTypes.Length, out int remainder);

            var firstTupleType = TupleTypes[remainder - 1];

            var args = new Type[remainder];
            CopyElementTypes(args, (chainLength - 1) * (ValueTupleRestPosition - 1), remainder);

            var tupleType = firstTupleType.MakeGenericType(args);

            int loop = chainLength - 1;

            while (loop > 0)
            {
                args = new Type[ValueTupleRestPosition];
                CopyElementTypes(args, (loop - 1) * (ValueTupleRestPosition - 1), ValueTupleRestPosition - 1);
                args[ValueTupleRestPosition - 1] = tupleType;

                tupleType = MaxTupleType.MakeGenericType(args);

                loop--;
            }

            return tupleType;

            void CopyElementTypes(Type[] target, int offset, int count)
            {
                for (var i = 0; i < count; i++)
                {
                    target[i] = elementTypes[offset + i];
                }
            }
        }
    }
}
