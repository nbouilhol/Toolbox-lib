using System.Web.Mvc;
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