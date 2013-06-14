using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Utilities.Extensions;

namespace Utilities.Functional
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T value)
        {
            Contract.Requires(value != null);

            yield return value;
        }

        public static IEnumerable<B> Bind<A, B>(this IEnumerable<A> a, Func<A, IEnumerable<B>> func)
        {
            Contract.Requires(a != null);
            Contract.Requires(func != null);

            foreach (A aval in a)
                foreach (B bval in func(aval))
                    yield return bval;
        }

        public static IEnumerable<C> SelectMany<A, B, C>(this IEnumerable<A> a, Func<A, IEnumerable<B>> func,
            Func<A, B, C> select)
        {
            Contract.Requires(a != null);
            Contract.Requires(func != null);
            Contract.Requires(select != null);

            return a.Bind(aval => func(aval).Bind(bval => select(aval, bval).ToEnumerable()));
        }

        public static IList<T> AddFluently<T>(this IList<T> list, T item)
        {
            Contract.Requires(list != null);
            Contract.Requires(item != null);

            list.Add(item);
            return list;
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            Contract.Requires(items != null);
            Contract.Requires(action != null);

            foreach (T item in items)
                action(item);
        }

        public static IEnumerable<T> Unless<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            Contract.Requires(list != null);
            Contract.Requires(predicate != null);

            return list.Where(x => !predicate(x));
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> list)
            where T : class
        {
            Contract.Requires(list != null);

            return list.Where(x => x != null);
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> list)
            where T : struct
        {
            Contract.Requires(list != null);

            return list.Where(x => x.HasValue).Select(x => x.Value);
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> list)
        {
            Contract.Requires(list != null);

            return list.ToDictionary(x => x.Key, x => x.Value);
        }

        public static IEnumerable<T> Recurse<T>(this T list, Func<T, IEnumerable<T>> selector)
        {
            Contract.Requires(list != null);
            Contract.Requires(selector != null);

            return list.AsEnumerable().Recurse(selector);
        }

        public static IEnumerable<T> Recurse<T>(this IEnumerable<T> list, Func<T, IEnumerable<T>> selector)
        {
            Contract.Requires(list != null);
            Contract.Requires(selector != null);

            return
                list.SelectMany(x => x.AsEnumerable().Concat((selector(x) ?? Enumerable.Empty<T>()).Recurse(selector)));
        }

        public static IEnumerable<T> Squash<T>(this IEnumerable<T?> list)
            where T : struct
        {
            Contract.Requires(list != null);

            return list.Where(x => x.HasValue).Select(x => x.Value);
        }
    }
}