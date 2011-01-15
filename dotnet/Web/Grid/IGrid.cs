using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Helper.Pagination;
using Mvc.Helper.Sorting;
using System.Linq.Expressions;
using System.Collections;
using Mvc.Helper.Search;

namespace Mvc.Helper.Grid
{
    public interface IGrid<TSource> : IGrid, IEnumerable<TSource>
    {
        IGrid<TSource> OrderBy<TKey>(string propertyName, SortDirection? direction, Expression<Func<TSource, TKey>> keySelector, SortDirection? keySelectorDirection);
        IGrid<TSource> OrderBy(string propertyName, SortDirection? direction, string defaultPropertyName, SortDirection? defaultDirection);
        IGrid<TSource> OrderBy<TKey>(string propertyName, SortDirection? direction, Expression<Func<TSource, TKey>> keySelector);
        IGrid<TSource> AsPagination(int? pageNumber);
        IGrid<TSource> AsPagination(int? pageNumber, int? pageSize);
        IGrid<TSource> ExecuteQuery();
        IGrid<TSource> SearchFor(string input, Expression<Func<TSource, bool>> filter);
        /// <summary>
        /// Need LinqKit
        /// </summary>
        /// <param name="input"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IGrid<TSource> SearchFor(string input, Expression<Func<TSource, string, bool>> filter);
        IGrid<TResult> ExecuteQuery<TResult>();
    }

    public interface IGrid : IEnumerable
    {
        ISort Sort { get; }
        IPagination Pagination { get; }
        ISearch Search { get; }
    }
}