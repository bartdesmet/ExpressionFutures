// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
    // NOTE: This could should be removed in the product and use the real TrueReadOnlyCollection<T>.

    [ExcludeFromCodeCoverage]
    internal sealed class TrueReadOnlyCollection<T> : ReadOnlyCollection<T>
    {
        internal TrueReadOnlyCollection(T[] list) : base(list)
        {
        }
    }
}

namespace System.Linq.Expressions
{
    // NOTE: This could should be removed in the product and use the real Set<T>,
    //       or that one's necessity should be revisited in favor of HashSet<T>.

    [ExcludeFromCodeCoverage]
    internal sealed class Set<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        private readonly Dictionary<T, object> _data;

        public int Count
        {
            get
            {
                return this._data.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        internal Set()
        {
            this._data = new Dictionary<T, object>();
        }

        internal Set(IEqualityComparer<T> comparer)
        {
            this._data = new Dictionary<T, object>(comparer);
        }

        internal Set(IList<T> list)
        {
            this._data = new Dictionary<T, object>(list.Count);
            foreach (T current in list)
            {
                this.Add(current);
            }
        }

        internal Set(IEnumerable<T> list)
        {
            this._data = new Dictionary<T, object>();
            foreach (T current in list)
            {
                this.Add(current);
            }
        }

        internal Set(int capacity)
        {
            this._data = new Dictionary<T, object>(capacity);
        }

        public void Add(T item)
        {
            this._data[item] = null;
        }

        public void Clear()
        {
            this._data.Clear();
        }

        public bool Contains(T item)
        {
            return this._data.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._data.Keys.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return this._data.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._data.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._data.Keys.GetEnumerator();
        }
    }
}
