using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using SuiviCRA.Domain;
using SuiviCRA.Web.Models;
using SuiviCRA.Web.Helpers.Extension;
using System.Configuration;

namespace Mvc.Helper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AdminOnlyAttribute : AuthorizeAttribute
    {
        public AdminOnlyAttribute()
        { }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!(filterContext.Result is HttpUnauthorizedResult))
                return;
            if (filterContext.HttpContext.Request.IsAjaxRequest())
                return;
            if (ConfigurationManager.AppSettings["IIS"] == "6")
                filterContext.Result = new RedirectResult("/");
            else
                filterContext.Result = new RedirectResult("/");
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            if (httpContext.UserIsAdmin() == null)
                return false;
            return httpContext.UserIsAdmin() ?? false;
        }
    }
}
