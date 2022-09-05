// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// NB: This is a pruned copy of https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq.Expressions/src/System/Dynamic/Utils/EmptyReadOnlyCollection.cs.

#nullable enable

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace System.Dynamic.Utils
{
    internal static class EmptyReadOnlyCollection<T>
    {
        public static readonly ReadOnlyCollection<T> Instance = new ReadOnlyCollectionBuilder<T>().ToReadOnlyCollection();
    }
}