using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Utilities
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> list)
        {
            if (list == null)
                return null;

            return list.Where(x => x != null);
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
            Contract.Requires(filter != null);

            ICollection<T> result = filter(source).ToList();
            source = source.Except(result).ToList();
            return result;
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
