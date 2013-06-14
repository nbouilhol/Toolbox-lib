using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;

namespace Mvc.Helper
{
    public static class HtmlHelperExtensions
    {
        public static MvcChart Chart(this HtmlHelper html, Page page, Chart chart)
        {
            return new MvcChart(page, chart);
        }

        public static string Script(this HtmlHelper html, string srcRelease, params string[] srcDebugs)
        {
#if DEBUG
            var builder = new StringBuilder();
            foreach (string srcDebug in srcDebugs)
                builder.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", srcDebug);
            return builder.ToString();
#else
            return string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", srcRelease);
#endif
        }

        public static string Css(this HtmlHelper html, string media, string hrefRelease, params string[] hrefDebugs)
        {
#if DEBUG
            var builder = new StringBuilder();
            foreach (string hrefDebug in hrefDebugs)
                builder.AppendFormat("<link rel=\"stylesheet\" type=\"text/css\" media=\"{0}\" href=\"{1}\"/>", media,
                    hrefDebug);
            return builder.ToString();
#else
            return string.Format("<link rel=\"stylesheet\" type=\"text/css\" media=\"{0}\" href=\"{1}\"/>", media, hrefRelease);
#endif
        }
    }
}