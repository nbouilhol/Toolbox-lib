using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Mvc.Helper.Pagination;

namespace Mvc.Helper.UI
{
    public class Header : IHtmlString 
    {
        private const string selectFormat = @"Afficher {0} éléments";
        private const string textSearch = @"Rechercher : ";
        private Func<int, string> urlBuilderForSize;
        private Func<string, string> urlBuilderForSearch;
        private IPagination pagination;

        public Header(IPagination pagination, Func<int, string> urlBuilderForSize, Func<string, string> urlBuilderForSearch)
        {
            this.pagination = pagination;
            this.urlBuilderForSize = urlBuilderForSize;
            this.urlBuilderForSearch = urlBuilderForSearch;
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
            return string.Format(@"<div class='grid-header fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix'>{0}</div>", RenderBody());
        }

        private string RenderBody()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(@"<div class='dataTables_length'>{0}</div>", string.Format(selectFormat, RenderSelect()));
            builder.AppendFormat(@"<div class='dataTables_filter'>{0}<input type='text' href='{1}'></div>", textSearch, urlBuilderForSearch("{0}"));
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
            builder.AppendFormat(@"<option value='{0}'{1}>{2}</option>", urlBuilderForSize(size), pagination.PageSize == size ? " selected" : "", size);
        }
    }
}