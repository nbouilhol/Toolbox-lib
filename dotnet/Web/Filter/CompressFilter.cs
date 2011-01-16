using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.IO;

namespace Mvc.Helper
{
    internal sealed class CompressFilter : ActionFilterAttribute
    {
        private const string GZIP = "gzip";
        private const string DEFLATE = "deflate";

        /// <summary>
        ///   A regular expression to localize all whitespace preceeding HTML tag endings.
        /// </summary>
        private static readonly Regex RegexBetweenTags = new Regex(@">\s+", RegexOptions.Compiled);

        /// <summary>
        ///   A regular expression to localize all whitespace preceeding a line break.
        /// </summary>
        private static readonly Regex RegexLineBreaks = new Regex(@"\n\s+", RegexOptions.Compiled);

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");
            Compress(filterContext.HttpContext.Request, filterContext.HttpContext.Response);
        }

        private void Compress(HttpRequestBase request, HttpResponseBase response)
        {
            if (IsMethodCall(request) || (request.Browser.IsBrowser("IE") && request.Browser.MajorVersion <= 6))
                return;
            if (IsEncodingAccepted(request, DEFLATE))
            {
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                SetEncoding(DEFLATE);
            }
            else if (IsEncodingAccepted(request, GZIP))
            {
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                SetEncoding(GZIP);
            }
        }

        private static bool IsMethodCall(HttpRequestBase request)
        {
            if (string.IsNullOrEmpty(request.PathInfo))
                return false;
            return request.ContentType.StartsWith("application/json;", StringComparison.OrdinalIgnoreCase) || string.Equals(request.ContentType, "application/json", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///   Checks the request headers to see if the specified
        ///   encoding is accepted by the client.
        /// </summary>
        private static bool IsEncodingAccepted(HttpRequestBase request, string encoding)
        {
            return request.Headers["Accept-encoding"] != null && request.Headers["Accept-encoding"].Contains(encoding);
        }

        /// <summary>
        ///   Adds the specified encoding to the response headers.
        /// </summary>
        /// <param name = "encoding"></param>
        private static void SetEncoding(string encoding)
        {
            HttpContext.Current.Response.AppendHeader("Content-encoding", encoding);
            HttpContext.Current.Response.Cache.VaryByHeaders["Accept-encoding"] = true;
        }

        /// <summary>
        ///   Removes whitespace from the specified string of HTML.
        /// </summary>
        /// <param name = "html">The HTML string to remove white space from.</param>
        /// <returns>The specified HTML string stripped from all whitespace.</returns>
        public static string RemoveWhitespaceFromHtml(string html)
        {
            html = RegexBetweenTags.Replace(html, ">");
            html = RegexLineBreaks.Replace(html, string.Empty);

            return html;
        }
    }
}
