using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace BouilholLib.Helper
{
    public static class IEnumerableExtension
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
		
        public static IEnumerable<TSource> Update<TSource>(this IEnumerable<TSource> outer, Action<TSource> updator)
        {
            foreach (var item in outer)
                updator(item);
            return outer;
        }

        public static IEnumerable<TSource> ForAll<TSource>(this IEnumerable<TSource> list, Action<TSource> action)
        {
            foreach (var item in list)
                action(item);
            return list;
        }

        /// <summary>
        /// F# - Monad
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="m"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static IEnumerable<TU> SelectMany<T, TU>(this IEnumerable<T> m, Func<T, IEnumerable<TU>> k)
        {
            foreach (T x in m)
                foreach (TU y in k(x))
                    yield return y;
        }

        /// <summary>
        /// F# - Seq.zip
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<TArg1, TArg2>> Zip<TArg1, TArg2>(this IEnumerable<TArg1> arg1, IEnumerable<TArg2> arg2, Func<TArg1, TArg2, Tuple<TArg1, TArg2>> func)
        {
            return arg1.Map2(arg2, func);
        }

        /// <summary>
        /// F# - List.map -> Linq Select
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="items"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> items, Func<T, TResult> func)
        {
            Guard.AgainstNull(items, "items");
            Guard.AgainstNull(func, "func");

            foreach (T item in items)
                yield return func(item);
        }

        /// <summary>
        /// F# - List.mapi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="items"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> MapIndex<T, TResult>(this IEnumerable<T> items, Func<int, T, TResult> func)
        {
            Guard.AgainstNull(items, "items");
            Guard.AgainstNull(func, "func");

            int index = 0;
            foreach (T item in items)
            {
                yield return func(index, item);
                index++;
            }
        }

        /// <summary>
        /// F# - Seq.map2
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Map2<TArg1, TArg2, TResult>(this IEnumerable<TArg1> arg1, IEnumerable<TArg2> arg2, Func<TArg1, TArg2, TResult> func)
        {
            IEnumerator<TArg1> e1 = arg1.GetEnumerator();
            IEnumerator<TArg2> e2 = arg2.GetEnumerator();
            var s = new SequenceMapEnumerator<TArg1, TArg2, TResult>(e1, e2, func);

            while (s.MoveNext())
                yield return s.Current;
        }

        // Seq.fold -> Aggregate
        /// <summary>
        /// F# - Seq.fold -> Linq Aggregate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="items"></param>
        /// <param name="func"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static T Fold<T, TU>(this IEnumerable<TU> items, Func<T, TU, T> func, T acc)
        {
            Guard.AgainstNull(items, "items");
            Guard.AgainstNull(func, "func");

            foreach (TU item in items)
                acc = func(acc, item);

            return acc;
        }

        /// <summary>
        /// F# - List.fold_left -> Linq Aggregate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static T FoldLeft<T, TU>(this IList<TU> list, Func<T, TU, T> func, T acc)
        {
            Guard.AgainstNull(list, "list");
            Guard.AgainstNull(func, "func");

            for (int index = 0; index < list.Count; index++)
                acc = func(acc, list[index]);

            return acc;
        }

        /// <summary>
        /// F# - List.fold_right
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static T FoldRight<T, TU>(this IList<TU> list, Func<T, TU, T> func, T acc)
        {
            Guard.AgainstNull(list, "list");
            Guard.AgainstNull(func, "func");

            for (int index = list.Count - 1; index >= 0; index--)
                acc = func(acc, list[index]);

            return acc;
        }

        /// <summary>
        /// F# - List.iter -> .ToList().ForEach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            Guard.AgainstNull(items, "items");
            Guard.AgainstNull(action, "action");

            foreach (T item in items)
                action(item);

            return items;
        }

        /// <summary>
        /// F# - List.iteri
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> ForIndex<T>(this IEnumerable<T> items, Action<int, T> action)
        {
            Guard.AgainstNull(items, "items");
            Guard.AgainstNull(action, "action");

            int index = 0;
            foreach (T item in items)
            {
                action(index, item);
                index++;
            }

            return items;
        }

        /// <summary>
        /// F# - List.filter -> Linq Where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            Guard.AgainstNull(items, "items");
            Guard.AgainstNull(predicate, "predicate");

            foreach (T item in items)
            {
                if (predicate(item))
                    yield return item;
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        /// <summary>
        /// ForEach with item != null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> ForNotNull<T>(this IEnumerable<T> items, Action<T> action) where T : class
        {
            Guard.AgainstNull(items, "items");
            Guard.AgainstNull(action, "action");

            foreach (T item in items)
                if (item != null)
                    action(item);

            return items;
        }

        public static IEnumerable<TEntite> Sort<TEntite>(this IEnumerable<TEntite> source, string sortExpression)
        {
            Guard.AgainstNullOrEmpty(sortExpression, "sortExpression");

            PropertyInfo property = typeof(TEntite).GetProperty(sortExpression);
            Type sorterType = typeof(GenericSorter<,>).MakeGenericType(typeof(TEntite), property.PropertyType);
            object sorterObject = Activator.CreateInstance(sorterType);
            return sorterType.GetMethod("Sort", new[] { typeof(TEntite), typeof(string) }).Invoke(sorterObject, new object[] { source, sortExpression }) as IEnumerable<TEntite>;
        }

        public static IEnumerable<T> Page<T>(this IEnumerable<T> list, int page, int pageSize, string sortexpression)
        {
            if (list == null || !list.Any() || page < 0 || pageSize < 0)
                return null;
            Expression<Func<T, object>> keySelector = GetKeySelector<T>(sortexpression);
            if (keySelector == null)
                return list.AsQueryable().Skip((page - 1)).Take(pageSize);
            return IsAsc(sortexpression)
                       ? list.AsQueryable().OrderBy(keySelector).Skip((page - 1)).Take(pageSize)
                       : list.AsQueryable().OrderByDescending(keySelector).Skip((page - 1)).Take(pageSize);
        }

        public static Expression<Func<T, object>> GetKeySelector<T>(this string sortexpression)
        {
            if (string.IsNullOrEmpty(sortexpression))
                return null;
            if (sortexpression.ToLowerInvariant().EndsWith(" desc"))
                sortexpression = sortexpression.Split(' ')[0];

            ParameterExpression param = Expression.Parameter(typeof(T), "item");
            return Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Property(param, sortexpression), typeof(object)), param);
        }

        public static bool IsAsc(this string sortexpression)
        {
            if (string.IsNullOrEmpty(sortexpression))
                return true;
            return sortexpression.ToLowerInvariant().EndsWith(" desc")
                       ? false
                       : true;
        }


        #region Nested type: GenericSorter

        private class GenericSorter<TEntite, TPt>
        {
            public static IEnumerable<TEntite> Sort(IEnumerable<TEntite> source, string sortExpression)
            {
                Guard.AgainstNullOrEmpty(sortExpression, "sortExpression");

                bool sortDirectionAsc = sortExpression.Contains(" DESC");
                sortExpression.ToUpper().Replace(" DESC", string.Empty);
                sortExpression.ToUpper().Replace(" ASC", string.Empty);

                ParameterExpression param = Expression.Parameter(typeof(TEntite), "item");
                Expression<Func<TEntite, TPt>> sortLambda = Expression.Lambda<Func<TEntite, TPt>>(Expression.Convert(Expression.Property(param, sortExpression), typeof(TPt)), param);

                return sortDirectionAsc
                           ? source.OfType<TEntite>().AsQueryable().OrderBy(sortLambda)
                           : source.OfType<TEntite>().AsQueryable().OrderByDescending(sortLambda);
            }
        }

        #endregion
    }
}
