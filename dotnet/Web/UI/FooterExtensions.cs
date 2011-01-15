using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc.Helper.Pagination;

namespace Mvc.Helper.UI
{
    public static class FooterExtensions
    {
        public static Footer Footer(this HtmlHelper helper, IPagination pagination, Func<int, string> urlBuilder)
        {
            return new Footer(pagination, urlBuilder);
        }
    }
}