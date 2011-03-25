using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc.Helper.Pagination;
using Mvc.Helper.Grid;

namespace Mvc.Helper.UI
{
    public static class FooterExtensions
    {
        public static Footer Footer(this HtmlHelper helper, IGrid grid)
        {
            return new Footer(grid);
        }
    }
}