using System.Web.Mvc;

namespace MvcHelper
{
    public static class ViewExtension
    {
        public static bool IsUrl(this ViewContext context, string action, string controller)
        {
            return context.RouteData.Values["controller"].ToString() == controller && context.RouteData.Values["action"].ToString() == action;
        }

        public static bool IsUrl(this ViewContext context, string controller)
        {
            return context.RouteData.Values["controller"].ToString() == controller;
        }
    }
}