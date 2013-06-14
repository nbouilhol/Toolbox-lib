using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using Mvc.Helper.Pagination;
using Mvc.Helper.Search;
using Mvc.Helper.Sorting;

namespace Mvc.Helper.Grid
{
    [Serializable]
    public class LazyGrid<T> : IGrid<T>
    {
        public const int DefaultPageSize = 25;
        private IQueryable<T> query;
        private IPagination pagination;
        private ISort sort;
        private ISearch search;
        private Func<int, int, string, Mvc.Helper.Sorting.SortDirection?, string, string> url;
        private IEnumerable<T> result;

        public LazyGrid(IQueryable<T> query)
        {
            this.query = query;
        }

        public LazyGrid(IGrid grid, IEnumerable<T> result)
        {
            this.search = grid.Search;
            this.pagination = grid.Pagination;
            this.sort = grid.Sort;
            this.url = grid.Url;
            this.result = result;
        }

        public IGrid<T> BuildUrl(RequestContext context, string route, string action)
        {
            url = (p, s, c, d, i) => new UrlHelper(context).RouteUrl(route, new RouteValueDictionary { { "action", action }, { "page", p }, { "size", s }, { "column", c }, { "direction", d }, { "search", i } });
            return this;
        }

        public IGrid<T> OrderBy<TKey>(string propertyName, SortDirection? direction, Expression<Func<T, TKey>> keySelector)
        {
            return OrderBy(propertyName, direction, keySelector, null);
        }

        public IGrid<T> OrderBy(string propertyName, SortDirection? direction, string defaultPropertyName, SortDirection? defaultDirection)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                propertyName = defaultPropertyName;
                direction = defaultDirection;
            }
            query = query.OrderBy(propertyName, direction ?? SortDirection.Descending);
            sort = new SortStorage(propertyName, direction);

            return this;
        }

        public IGrid<T> OrderBy<TKey>(string propertyName, SortDirection? direction, Expression<Func<T, TKey>> keySelector, SortDirection? keySelectorDirection)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                if (keySelectorDirection == null || keySelectorDirection.Value == SortDirection.Ascending)
                    query = query.OrderBy(keySelector);
                else
                    query = query.OrderByDescending(keySelector);
            }
            else
                query = query.OrderBy(propertyName, direction ?? SortDirection.Descending);
            sort = new SortStorage(propertyName, direction);

            return this;
        }

        public IGrid<T> AsPagination(int? pageNumber)
        {
            return AsPagination(pageNumber, DefaultPageSize);
        }

        public IGrid<T> AsPagination(int? pageNumber, int? pageSize)
        {
            int number = pageNumber.GetValueOrDefault(1);
            int size = pageSize.GetValueOrDefault(DefaultPageSize);

            if (!query.Any())
                pagination = new PaginationStorage(number, size, 0, 1);
            else
            {
                int totalItems = query.Count();
                int totalPages = (int)Math.Ceiling(((double)totalItems) / size);

                if (totalPages < number)
                    number = totalPages;

                query = query.Skip((number - 1) * size).Take(size);
                pagination = new PaginationStorage(number, size, totalItems, totalPages);
            }

            return this;
        }

        public IGrid<T> SearchFor(string input, Expression<Func<T, bool>> filter)
        {
            if (!string.IsNullOrEmpty(input))
                query = query.Where(filter);
            search = new SearchStorage(input);
            return this;
        }

        /// <summary>
        /// Need LinqKit
        /// </summary>
        /// <param name="input"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IGrid<T> SearchFor(string input, Expression<Func<T, string, bool>> filter)
        {
            if (!string.IsNullOrEmpty(input))
                query = query.Where(filter, input);
            search = new SearchStorage(input);
            return this;
        }

        public IGrid<T> ExecuteQuery()
        {
            result = query.ToList();
            return this;
        }

        public IGrid<TResult> ExecuteQuery<TResult>()
        {
            result = query.ToList();
            return new LazyGrid<TResult>(this, Mapper.Map<IEnumerable<T>, IEnumerable<TResult>>(result));
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (var item in GetResult())
                yield return item;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        public Func<int, int, string, Mvc.Helper.Sorting.SortDirection?, string, string> Url
        {
            get { return url; }
        }

        public ISort Sort
        {
            get { return sort; }
        }

        public IPagination Pagination
        {
            get { return pagination; }
        }

        public ISearch Search
        {
            get { return search; }
        }

        private IEnumerable<T> GetResult()
        {
            if (result == null)
                return query;
            return result;
        }
    }
}