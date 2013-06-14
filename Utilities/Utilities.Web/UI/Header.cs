using System;
using System.Text;
using System.Web;
using Mvc.Helper.Grid;
using Mvc.Helper.Pagination;
using Mvc.Helper.Search;

namespace Mvc.Helper.UI
{
    public class Header : IHtmlString
    {
        private const string selectFormat = @"Afficher {0} éléments";
        private const string textSearch = @"Rechercher : ";
        private readonly IPagination pagination;
        private readonly ISearch search;
        private readonly Func<int, string, string> urlBuilder;

        public Header(IPagination pagination, ISearch search, Func<int, string, string> urlBuilder)
        {
            this.pagination = pagination;
            this.search = search;
            this.urlBuilder = urlBuilder;
        }

        public Header(IGrid grid)
            : this(
                grid.Pagination, grid.Search,
                (s, i) => grid.Url(grid.Pagination.PageNumber, s, grid.Sort.Column, grid.Sort.Direction, i))
        {
        }

        public string ToHtmlString()
        {
            return Render();
        }

        public override string ToString()
        {
            return Render();
        }

        private string Render()
        {
            return
                string.Format(
                    @"<div class='grid-header fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix'>{0}</div>",
                    RenderBody());
        }

        private string RenderBody()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(@"<div class='dataTables_length'>{0}</div>",
                string.Format(selectFormat, RenderSelect()));
            builder.AppendFormat(
                @"<div class='dataTables_filter'>{0}<input type='search' href='{1}' value='{2}' /></div>", textSearch,
                urlBuilder(pagination.PageSize, "{0}"), search.Input);
            return builder.ToString();
        }

        private string RenderSelect()
        {
            return string.Format(@"<select size='1'>{0}</select>", RenderInnerSelect());
        }

        private string RenderInnerSelect()
        {
            var builder = new StringBuilder();
            RenderOption(builder, 10);
            RenderOption(builder, 25);
            RenderOption(builder, 50);
            RenderOption(builder, 100);
            return builder.ToString();
        }

        private void RenderOption(StringBuilder builder, int size)
        {
            builder.AppendFormat(@"<option value='{0}'{1}>{2}</option>", urlBuilder(size, search.Input),
                pagination.PageSize == size ? " selected" : "", size);
        }
    }
}