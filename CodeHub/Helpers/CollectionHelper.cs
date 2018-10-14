using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CodeHub.Helpers
{
    class UniqueCollection
           : Collection<object>
    {

        public new void Add(object item)
        {
            if (Items.Contains(item))
            {
                throw new InvalidOperationException($"{item} already exists");
            }

            base.Add(item);
        }
        protected override void InsertItem(int index, object item)
        {
            if (Items.Contains(item))
            {
                throw new InvalidOperationException($"{item} already exists");
            }

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, object item)
        {
            if (Items.Contains(item))
            {
                throw new InvalidOperationException($"{item} already exists");
            }

            base.SetItem(index, item);
        }
    }
    class UniqueCollection<T>
        : Collection<T>
    {

        public new void Add(T item)
        {
            if (Items.Contains(item))
            {
                throw new InvalidOperationException($"{item} already exists");
            }

            base.Add(item);
        }
        protected override void InsertItem(int index, T item)
        {
            if (Items.Contains(item))
            {
                throw new InvalidOperationException($"{item} already exists");
            }

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            if (Items.Contains(item))
            {
                throw new InvalidOperationException($"{item} already exists");
            }

            base.SetItem(index, item);
        }
    }
    static class CollectionHelper
    {
        public static ICollection Combine(this ICollection collection, IEnumerable combinableCollection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (combinableCollection == null)
            {
                throw new ArgumentNullException(nameof(combinableCollection));
            }

            var result = new UniqueCollection();


            if (collection.Count > 0)
            {
                collection.ForEach(c => result.Add(c));
            }
            if (combinableCollection.Count() > 0)
            {
                combinableCollection.ForEach(o =>
                {
                    if (!result.Contains(o))
                    {
                        result.Add(o);
                    }
                });
            }
            return result;
        }
        public static ICollection<T> Combine<T>(this ICollection<T> collection, IEnumerable<T> combinableCollection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (combinableCollection == null)
            {
                throw new ArgumentNullException(nameof(combinableCollection));
            }
            var result = new UniqueCollection<T>();
            if (collection.Count > 0)
            {
                collection.ForEach(c => result.Add(c));
            }
            if (combinableCollection.Count() > 0)
            {
                combinableCollection.ForEach(o =>
                {
                    if (!result.Contains(o))
                    {
                        result.Add(o);
                    }
                });
            }
            return result;
        }
        public static bool Contains<T>(this ICollection<T> collection, T item)
        {
            foreach (var c in collection)
            {
                if (item.Equals(c))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool Contains<T, TEqualityComparer>(this ICollection<T> collection, T item, TEqualityComparer comparer)
            where TEqualityComparer : IEqualityComparer<T>
        {
            foreach (var c in collection)
            {
                return comparer?.Equals(item, c) ?? item.Equals(c);
            }
            return false;
        }
        public static bool Contains<T>(this IEnumerable<T> collection, T item)
        {
            foreach (var c in collection)
            {
                if (item.Equals(c))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool Contains<T, TEqualityComparer>(this IEnumerable<T> collection, T item, TEqualityComparer comparer)
            where TEqualityComparer : IEqualityComparer<T>
        {
            foreach (var c in collection)
            {
                return comparer?.Equals(item, c) ?? item.Equals(c);
            }
            return false;
        }

        public static void ForEach(this ICollection collection, Action<object> predicate)
        {
            foreach (var item in collection)
            {
                predicate(item);
            }
        }
        public static void ForEach<T>(this ICollection<T> enumerable, Action<T> predicate)
        {
            foreach (var item in enumerable)
            {
                predicate(item);
            }
        }

        public static void ForEach(this IEnumerable collection, Action<object> predicate)
        {
            foreach (var item in collection)
            {
                predicate(item);
            }
        }
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> predicate)
        {
            foreach (var item in enumerable)
            {
                predicate(item);
            }
        }
        public static void ForEach(this IQueryable queryable, Action<object> predicate)
        {
            foreach (var item in queryable)
            {
                predicate(item);
            }
        }

        public static void ForEach<T>(this IQueryable<T> queryable, Action<T> predicate)
        {
            foreach (var item in queryable)
            {
                predicate(item);
            }
        }
    }
}
