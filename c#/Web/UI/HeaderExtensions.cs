using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc.Helper.Pagination;
using Mvc.Helper.Search;
using Mvc.Helper.Grid;

namespace Mvc.Helper.UI
{
    public static class HeaderExtensions
    {
        public static Header Header(this HtmlHelper helper, IGrid grid)
        {
            return new Header(grid);
        }
    }
}