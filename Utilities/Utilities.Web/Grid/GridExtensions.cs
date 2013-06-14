using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Routing;
using Mvc.Helper.Sorting;

namespace Mvc.Helper.Grid
{
    public static class GridExtensions
    {
        private const string orderBy = "OrderBy";
        private const string orderByDesc = "OrderByDescending";

        public static IGrid<T> ToList<T>(this IGrid<T> grid)
        {
            return grid.ExecuteQuery();
        }

        public static IGrid<TResult> ToList<TSource, TResult>(this IGrid<TSource> grid)
        {
            return grid.ExecuteQuery<TResult>();
        }

        public static IGrid<T> SearchFor<T>(this IQueryable<T> source, string input, Expression<Func<T, bool>> filter)
        {
            return new LazyGrid<T>(source).SearchFor(input, filter);
        }

        public static IGrid<TSource> BuildUrl<TSource>(this IGrid<TSource> grid, RequestContext context, string route, string action)
        {
            return grid.BuildUrl(context, route, action);
        }

        /// <summary>
        /// Need LinqKit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="input"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IGrid<T> SearchFor<T>(this IQueryable<T> source, string input, Expression<Func<T, string, bool>> filter)
        {
            return new LazyGrid<T>(source).SearchFor(input, filter);
        }

        public static IGrid<T> AsPagination<T>(this IQueryable<T> source, int pageNumber)
        {
            return new LazyGrid<T>(source).AsPagination(pageNumber);
        }

        public static IGrid<T> AsPagination<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            return new LazyGrid<T>(source).AsPagination(pageNumber, pageSize);
        }

        public static IGrid<T> OrderBy<T, TKey>(this IQueryable<T> source, string propertyName, SortDirection? direction, Expression<Func<T, TKey>> keySelector)
        {
            return new LazyGrid<T>(source).OrderBy(propertyName, direction, keySelector);
        }

        public static IGrid<T> OrderBy<T, TKey>(this IQueryable<T> source, string propertyName, SortDirection? direction, Expression<Func<T, TKey>> keySelector, SortDirection? keySelectorDirection)
        {
            return new LazyGrid<T>(source).OrderBy(propertyName, direction, keySelector, keySelectorDirection);
        }

        public static IGrid<T> OrderBy<T>(this IQueryable<T> source, string propertyName, SortDirection? direction, string defaultPropertyName, SortDirection? defaultDirection)
        {
            return new LazyGrid<T>(source).OrderBy(propertyName, direction, defaultPropertyName, defaultDirection);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> datasource, string propertyName, SortDirection direction)
        {
            if (string.IsNullOrEmpty(propertyName))
                return datasource;
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "p");
            Expression expr = parameter;
            foreach (string prop in propertyName.Split('.'))
            {
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            var orderByExp = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T), type), expr, parameter);
            string methodToInvoke = direction == SortDirection.Ascending ? orderBy : orderByDesc;
            var orderByCall = Expression.Call(typeof(Queryable),
                methodToInvoke,
                new[] { typeof(T), type },
                datasource.Expression,
                Expression.Quote(orderByExp));
            return datasource.Provider.CreateQuery<T>(orderByCall);
        }

        /// <summary>
        /// Need LinqKit
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, string, bool>> predicate, string input)
        {
            throw new NotImplementedException();
            //if (source == null)
            //    throw new ArgumentNullException("source");
            //if (predicate == null)
            //    throw new ArgumentNullException("predicate");
            //if (string.IsNullOrEmpty(input))
            //    return source;

            //var parameterSource = Expression.Parameter(typeof(TSource), null);
            //var xpr = Expression.Invoke(predicate, parameterSource, Expression.Constant(input, typeof(string)));

            //return source.Where(Expression.Lambda<Func<TSource, bool>>(xpr.Expand(), parameterSource));
        }
    }
}