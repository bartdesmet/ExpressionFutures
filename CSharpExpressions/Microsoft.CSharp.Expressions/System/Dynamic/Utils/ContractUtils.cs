﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// NB: This is a pruned copy of https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq.Expressions/src/System/Dynamic/Utils/ContractUtils.cs.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Dynamic.Utils
{
    internal static class ContractUtils
    {
        /// <summary>
        /// Returns an exception object to be thrown when code is supposed to be unreachable.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static Exception Unreachable
        {
            get
            {
                Debug.Fail("Unreachable");
                return new UnreachableException();
            }
        }

        /// <summary>
        /// Requires the <paramref name="precondition"/> to be <c>true</c>.
        /// </summary>
        /// <param name="precondition">
        /// The precondition to check for being <c>true</c>.
        /// </param>
        /// <param name="paramName">
        /// The parameter name to use in the <see cref="ArgumentException.ParamName"/> property when an exception is thrown.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="precondition"/> is <c>false</c>.
        /// </exception>
        public static void Requires([DoesNotReturnIf(false)] bool precondition, string paramName)
        {
            Debug.Assert(!string.IsNullOrEmpty(paramName));

            if (!precondition)
            {
                throw new ArgumentException("Precondition failed.", paramName);
            }
        }

        /// <summary>
        /// Requires the <paramref name="value"/> to be non-<c>null</c>.
        /// </summary>
        /// <param name="value">
        /// The value to check for being non-<c>null</c>.
        /// </param>
        /// <param name="paramName">
        /// The parameter name to use in the <see cref="ArgumentException.ParamName"/> property when an exception is thrown.
        /// </param>
        /// <param name="index">
        /// The index of the argument being checked for <c>null</c>.
        /// If an exception is thrown, this value is used in <see cref="ArgumentException.ParamName"/> if it's greater than or equal to 0.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public static void RequiresNotNull([NotNull] object? value, string paramName, int index = -1)
        {
            Debug.Assert(!string.IsNullOrEmpty(paramName));

            if (value == null)
            {
                throw new ArgumentNullException(GetParamName(paramName, index));
            }
        }

        /// <summary>
        /// Requires the <paramref name="collection"/> to be non-empty.
        /// </summary>
        /// <param name="collection">
        /// The collection to check for being non-empty.
        /// </param>
        /// <param name="paramName">
        /// The parameter name to use in the <see cref="ArgumentException.ParamName"/> property when an exception is thrown.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the <paramref name="collection"/> is empty.
        /// </exception>
        public static void RequiresNotEmpty<T>([NotNull] ICollection<T>? collection, string paramName)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (collection.Count == 0)
            {
                throw new ArgumentException("Non-empty collection required.", paramName);
            }
        }

        /// <summary>
        /// Requires the <paramref name="array"/> and all its items to be non-<c>null</c>.
        /// </summary>
        /// <param name="array">
        /// The array to check for being non-<c>null</c> and containing non-<c>null</c> items.
        /// </param>
        /// <param name="arrayName">
        /// The parameter name to use in the <see cref="ArgumentException.ParamName"/> property when an exception is thrown.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the <paramref name="array"/> or any of its items is <c>null</c>.
        /// </exception>
        public static void RequiresNotNullItems<T>([NotNull] IList<T> array, string arrayName)
        {
            Debug.Assert(!string.IsNullOrEmpty(arrayName));

            if (array == null)
            {
                throw new ArgumentNullException(arrayName);
            }

            for (int i = 0, n = array.Count; i < n; i++)
            {
                if (array[i] == null)
                {
                    throw new ArgumentNullException(GetParamName(arrayName, i));
                }
            }
        }

        private static string GetParamName(string paramName, int index) => index >= 0 ? $"{paramName}[{index}]" : paramName;

        /// <summary>
        /// Requires the range [offset, offset + count] to be a subset of [0, array.Count].
        /// </summary>
        /// <exception cref="ArgumentNullException">Array is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Offset or count are out of range.</exception>
        public static void RequiresArrayRange<T>([NotNull] IList<T> array, int offset, int count, string offsetName, string countName)
        {
            Debug.Assert(!string.IsNullOrEmpty(offsetName));
            Debug.Assert(!string.IsNullOrEmpty(countName));
            Debug.Assert(array != null);

            if (count < 0)
                throw new ArgumentOutOfRangeException(countName);
            if (offset < 0 || array.Count - offset < count)
                throw new ArgumentOutOfRangeException(offsetName);
        }
    }
}