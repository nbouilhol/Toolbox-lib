using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc.Helper.Sorting;
using System.Linq.Expressions;
using System.Reflection;
using Mvc.Helper.Grid;

namespace Mvc.Helper.UI
{
    public static class TheadExtensions
    {
        public static Thead Thead(this HtmlHelper helper, string text)
        {
            return new Thead(text);
        }

        public static Thead Thead(this HtmlHelper helper, IGrid grid, string text, string column)
        {
            return new Thead(grid, text, column);
        }

        public static Thead Thead(this HtmlHelper helper, IGrid grid, string text, string column, string @class)
        {
            return new Thead(grid, text, column, @class);
        }
    }
}