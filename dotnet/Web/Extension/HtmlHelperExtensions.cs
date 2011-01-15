using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using System.Web;
using System.Web.UI.DataVisualization.Charting;

namespace SuiviIncidentsProd.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcChart Chart(this HtmlHelper html, Page page, Chart chart)
        {
            return new MvcChart(page, chart);
        }
    }
}
