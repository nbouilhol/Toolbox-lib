using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc.Helper.Pagination;

namespace Mvc.Helper.UI
{
    public static class HeaderExtensions
    {
        public static Header Header(this HtmlHelper helper, IPagination pagination, Func<int, string> urlBuilderForSize, Func<string, string> urlBuilderForSearch)
        {
            return new Header(pagination, urlBuilderForSize, urlBuilderForSearch);
        }
    }
}