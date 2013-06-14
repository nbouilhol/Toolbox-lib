using System;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;

namespace Mvc.Helper
{
    public class MvcChart : IDisposable, IHtmlString
    {
        public MvcChart(Page page, Chart chart)
        {
            chart.Page = page;
            Chart = chart;
        }

        public Chart Chart { get; set; }

        public void Dispose()
        {
            Chart.Dispose();
            GC.SuppressFinalize(Chart);
        }

        public string ToHtmlString()
        {
            return RenderChart();
        }

        public override string ToString()
        {
            return RenderChart();
        }

        /// <summary>
        ///     Renders control to a string.
        /// </summary>
        /// <returns></returns>
        public string RenderChart()
        {
            using (var swriter = new StringWriter(CultureInfo.CurrentCulture))
            {
                using (var writer = new HtmlTextWriter(swriter))
                    Chart.RenderControl(writer);
                return swriter.ToString();
            }
        }
    }
}