// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

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

    internal static class ElementInitStub
    {
        private static readonly ConstructorInfo s_ctor = typeof(ElementInit).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single();

        public static ElementInit Create(MethodInfo addMethod, ReadOnlyCollection<Expression> arguments)
        {
            return (ElementInit)s_ctor.Invoke(new object[] { addMethod, arguments });
        }
    }

    internal static class BinaryExpressionExtensions
    {
        private static readonly Assembly s_asm = Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        private static readonly Type s_typ = s_asm.GetType("System.Linq.Expressions.BinaryExpression");
        private static readonly PropertyInfo s_1 = s_typ.GetProperty("IsLiftedLogical", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        private static readonly MethodInfo s_2 = s_typ.GetMethod("ReduceUserdefinedLifted", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        public static System.Boolean get_IsLiftedLogical(this System.Linq.Expressions.BinaryExpression obj)
        {
            try
            {
                return (System.Boolean)s_1.GetValue(obj);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public static System.Linq.Expressions.Expression ReduceUserdefinedLifted(this System.Linq.Expressions.BinaryExpression obj)
        {
            try
            {
                return (System.Linq.Expressions.Expression)s_2.Invoke(obj, new object[0]);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
