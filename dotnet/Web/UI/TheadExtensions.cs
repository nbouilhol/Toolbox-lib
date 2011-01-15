using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc.Helper.Sorting;
using System.Linq.Expressions;
using System.Reflection;

namespace Mvc.Helper.UI
{
    public static class TheadExtensions
    {
        public static Thead Thead(this HtmlHelper helper, string text)
        {
            return new Thead(text);
        }

        public static Thead Thead(this HtmlHelper helper, ISort sort, string text, string column, Func<string, SortDirection?, string> urlBuilder)
        {
            return new Thead(sort, text, column, urlBuilder);
        }

        public static Thead Thead(this HtmlHelper helper, ISort sort, string text, string column, Func<string, SortDirection?, string> urlBuilder, string @class)
        {
            return new Thead(sort, text, column, urlBuilder, @class);
        }
    }
}