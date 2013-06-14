using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Utilities.Extensions
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> list)
        {
            if (list == null)
                return null;
            return list.Where(x => !EqualityComparer<T>.Default.Equals(x, default (T)));
        }

        public static List<T> InList<T>(this T item)
        {
            return new List<T> { item };
        }

        public static List<T> WithItems<T>(this List<T> list, params T[] items)
        {
            list.AddRange(items);
            return list;
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T toReplace, T replaceWith)
        {
            return source.Select(item => Equals(item, toReplace) ? replaceWith : item);
        }

        public static IEnumerable<TResult> SelectManySafe<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);

            return source.Select(selector).WhereNotNull().SelectMany(x => x);
        }

        public static ICollection<T> Takes<T>(this ICollection<T> source, Func<IEnumerable<T>, IEnumerable<T>> filter)
        {
            Contract.Requires(source != null);
            Contract.Requires(filter != null);

            List<T> result = filter(source).ToList();
            source = source.Except(result).ToList();
            return result;
        }

        public static IEnumerable<T> Aggregate<T>(this IEnumerable<T> source, int following, Func<IEnumerable<T>, T> aggregate)
        {
            Contract.Requires(source != null);
            Contract.Requires(aggregate != null);

            IEnumerable<T> enumerable = source as IList<T> ?? source.ToList();
            for (int cpt = 0; enumerable.Count() > cpt; cpt += following)
                yield return aggregate(enumerable.Skip(cpt).Take(following));
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;

            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }

            return -1;
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) throw new ArgumentNullException("comparer");

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext()) throw new InvalidOperationException("Sequence contains no elements");
                TSource max = sourceIterator.Current;
                TKey maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);

                    if (comparer.Compare(candidateProjected, maxKey) <= 0) continue;
                    max = candidate;
                    maxKey = candidateProjected;
                }

                return max;
            }
        }

        //public static IDictionary<TKey, TElement> ToMyDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        //{
        //    IEqualityComparer<TKey> comparer = EqualityComparer<TKey>.Default;
        //    ICollection<TSource> list = source as ICollection<TSource>;
        //    var ret = list == null ? new Dictionary<TKey, TElement>(comparer)
        //                           : new Dictionary<TKey, TElement>(list.Count, comparer);
        //    foreach (TSource item in source)
        //        ret.Add(keySelector(item), elementSelector(item));
        //    return ret;
        //}

        //public static IDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        //{
        //    comparer = comparer ?? EqualityComparer<TKey>.Default;
        //    ICollection<TSource> list = source as ICollection<TSource>;
        //    var ret = list == null ? new Dictionary<TKey, TElement>(comparer)
        //                           : new Dictionary<TKey, TElement>(list.Count, comparer);
        //    foreach (TSource item in source)
        //        ret.Add(keySelector(item), elementSelector(item));
        //    return ret;
        //}
    }

    //public class SourceWoResultAndResult<T>
    //{
    //    public IList<T> SourceWithoutResult { get; set; }
    //    public IList<T> Result { get; set; }
    //}
}