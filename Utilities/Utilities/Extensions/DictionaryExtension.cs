using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Utilities.Extensions
{
    public static class DictionaryExtension
    {
        public static void TryAdd<TKey, TElement>(this ConcurrentDictionary<TKey, TElement> dic,
            IDictionary<TKey, TElement> dicToAdd)
        {
            Contract.Requires(dic != null);

            if (dicToAdd == null) return;
            foreach (var kvToAdd in dicToAdd)
                dic.TryAdd(kvToAdd.Key, kvToAdd.Value);
        }

        public static TElement TryTakeValue<TKey, TElement>(this IDictionary<TKey, TElement> dic, TKey key,
            TElement returnIfFailed)
        {
            Contract.Requires(dic != null);

            TElement value;
            if (!dic.TryGetValue(key, out value))
                return returnIfFailed;
            dic.Remove(key);
            return value;
        }

        public static TElement TryTakeValue<TKey, TElement>(this IDictionary<TKey, TElement> dic, TKey key)
        {
            Contract.Requires(dic != null);

            TElement value;
            if (!dic.TryGetValue(key, out value))
                return default(TElement);
            dic.Remove(key);
            return value;
        }

        public static TElement TryGetValue<TKey, TElement>(this IDictionary<TKey, TElement> dic, TKey key,
            TElement returnIfFailed)
        {
            Contract.Requires(dic != null);
            Contract.Ensures(Contract.Result<TElement>() != null);

            TElement value;
            if (!dic.TryGetValue(key, out value))
                return returnIfFailed;
            return value;
        }

        public static TElement TryGetValue<TKey, TElement>(this IDictionary<TKey, TElement> dic, TKey key)
        {
            Contract.Requires(dic != null);

            TElement value;
            if (!dic.TryGetValue(key, out value))
                return default(TElement);
            return value;
        }

        public static IEnumerable<TElement> TryGetValue<TKey, TElement>(this ILookup<TKey, TElement> dic, TKey key)
        {
            Contract.Requires(dic != null);

            if (dic.Contains(key))
                return dic[key];
            return Enumerable.Empty<TElement>();
        }

        public static TProperty TryGetProperty<TKey, TElement, TProperty>(this IDictionary<TKey, TElement> dic, TKey key,
            Func<TElement, TProperty> selector)
        {
            Contract.Requires(dic != null);
            Contract.Requires(key != null);
            Contract.Requires(selector != null);

            TElement value;
            if (!dic.TryGetValue(key, out value))
                return default(TProperty);
            return selector(value);
        }

        public static IDictionary<TKey, TElement> Combine<TKey, TElement>(this IDictionary<TKey, TElement> dic1,
            IDictionary<TKey, TElement> dic2, Func<TElement, TElement, TElement> selector)
        {
            Contract.Requires(dic1 != null);
            Contract.Requires(dic2 != null);

            return dic1.Join(dic2, d => d.Key, d => d.Key,
                (outer, inner) => new KeyValuePair<TKey, TElement>(outer.Key, selector(outer.Value, inner.Value)))
                .ToDictionary();
        }

        public static IEnumerable<KeyValuePair<TKey, TElement>> Combine<TKey, TElement>(
            this IEnumerable<KeyValuePair<TKey, TElement>> dic1, IEnumerable<KeyValuePair<TKey, TElement>> dic2,
            Func<TElement, TElement, TElement> selector)
        {
            Contract.Requires(dic1 != null);
            Contract.Requires(dic2 != null);

            return dic1.Join(dic2, d => d.Key, d => d.Key,
                (outer, inner) => new KeyValuePair<TKey, TElement>(outer.Key, selector(outer.Value, inner.Value)));
        }

        public static IDictionary<TKey, TElement> ToDictionary<TKey, TElement>(
            this IEnumerable<KeyValuePair<TKey, TElement>> dic)
        {
            Contract.Requires(dic != null);

            return dic.ToDictionary(x => x.Key, x => x.Value);
        }

        public static IDictionary<TKey, IEnumerable<TElement>> GroupByToDictionary<TKey, TElement>(
            this IEnumerable<TElement> list, Func<TElement, TKey> selector)
        {
            if (list == null) return null;

            return list.GroupBy(x => selector(x)).ToDictionary(x => x.Key, x => x.ToList() as IEnumerable<TElement>);
        }

        public static IDictionary<TKey, TElement> GroupByToDictionary<TKey, TElement>(this IEnumerable<TElement> list,
            Func<TElement, TKey> selector, Func<IEnumerable<TElement>, TElement> condition)
        {
            if (list == null) return null;

            return list.GroupBy(x => selector(x)).ToDictionary(x => x.Key, x => condition(x));
        }
    }
}