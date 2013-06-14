using System;
using System.Web;
using System.Web.Mvc;
using Mvc.Helper.Grid;
using Mvc.Helper.Sorting;

namespace Mvc.Helper.UI
{
    public class Thead : IHtmlString
    {
        private readonly ISort sort;
        private readonly string text;
        private readonly string column;
        private readonly Func<string, SortDirection?, string> urlBuilder;
        private readonly SortDirection? direction;
        private readonly string @class;

        public Thead(string text)
        {
            this.text = text;
        }

        public Thead(IGrid grid, string text, string column)
            : this(grid.Sort, text, column, (c, d) => grid.Url(grid.Pagination.PageNumber, grid.Pagination.PageSize, c, d, grid.Search.Input), null)
        { }

        public Thead(IGrid grid, string text, string column, string @class)
            : this(grid.Sort, text, column, (c, d) => grid.Url(grid.Pagination.PageNumber, grid.Pagination.PageSize, c, d, grid.Search.Input), @class)
        { }

        public Thead(ISort sort, string text, string column, Func<string, SortDirection?, string> urlBuilder)
            : this(sort, text, column, urlBuilder, null)
        { }

        public Thead(ISort sort, string text, string column, Func<string, SortDirection?, string> urlBuilder, string @class)
        {
            this.sort = sort;
            this.text = text;
            this.column = column;
            this.urlBuilder = urlBuilder;
            this.direction = sort.GetSortDataFor(column);
            this.@class = @class;
        }

        public override string ToString()
        {
            return Render();
        }

        private string Render()
        {
            if (string.IsNullOrEmpty(column))
                return string.Format(@"<th class='ui-state-default {0}'><div class='DataTables_sort_wrapper'>{1}</div></th>", @class, text);
            return string.Format(@"<th class='ui-state-default {0}'><div class='DataTables_sort_wrapper'>{1}{2}</div></th>", @class, CreatePageLink(column, direction, text), RenderSortIcon(direction));
        }

        private string RenderSortIcon(SortDirection? direction)
        {
            if (direction == SortDirection.Descending)
                return @"<span class='css_right ui-icon ui-icon-triangle-1-n'></span>";
            if (direction == SortDirection.Ascending)
                return @"<span class='css_right ui-icon ui-icon-triangle-1-s'></span>";
            return @"<span class='css_right ui-icon ui-icon-carat-2-n-s'></span>";
        }

        private string CreatePageLink(string column, SortDirection? direction, string text)
        {
            var builder = new TagBuilder("a");
            builder.SetInnerText(text);
            builder.MergeAttribute("href", urlBuilder(column, direction));
            return builder.ToString(TagRenderMode.Normal);
        }

        public string ToHtmlString()
        {
            return Render();
        }
    }
}