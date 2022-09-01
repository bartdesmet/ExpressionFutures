// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
// NB: This is a tweaked copy of https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq.Expressions/src/System/Dynamic/Utils/CollectionExtensions.cs
//     which works around the unavailability of TrueReadOnlyCollection<T> to avoid copies in ToReadOnly. This could be much improved if ToReadOnly was made available
//     publicly.
//

#nullable enable

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace System.Dynamic.Utils
{
    internal static class CollectionExtensions
    {
        public static ReadOnlyCollection<T> AddFirst<T>(this ReadOnlyCollection<T> list, T item)
        {
            T[] res = new T[list.Count + 1];
            res[0] = item;
            list.CopyTo(res, 1);
            return res.ToReadOnlyUnsafe();
        }

        public static T[] AddFirst<T>(this T[] array, T item)
        {
            T[] res = new T[array.Length + 1];
            res[0] = item;
            array.CopyTo(res, 1);
            return res;
        }

        public static T[] RemoveFirst<T>(this T[] array)
        {
            T[] result = new T[array.Length - 1];
            Array.Copy(array, 1, result, 0, result.Length);
            return result;
        }

        public static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T>? enumerable)
        {
            if (enumerable == null)
            {
                return EmptyReadOnlyCollection<T>.Instance;
            }

            if (enumerable is ReadOnlyCollectionBuilder<T> builder)
            {
                return builder.ToReadOnlyCollection();
            }

            if (enumerable is ReadOnlyCollection<T> roc && IsTrueReadOnlyCollection(roc))
            {
                return roc;
            }

            return new ReadOnlyCollectionBuilder<T>(enumerable).ToReadOnlyCollection();
        }

        public static ReadOnlyCollection<T> ToReadOnlyUnsafe<T>(this T[] array)
        {
            return new ReadOnlyCollectionBuilder<T>(array).ToReadOnlyCollection();
        }

        private static bool IsTrueReadOnlyCollection<T>(ReadOnlyCollection<T> roc)
        {
            return roc.GetType() == EmptyReadOnlyCollection<T>.Instance.GetType();
        }
    }
}