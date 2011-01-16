using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SuiviCRA.Web.Helpers.Extension;
using System.Configuration;

namespace Mvc.Helper
{
     [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthenticatedOnlyAttribute : AuthorizeAttribute
    {
        public AuthenticatedOnlyAttribute()
        { }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!(filterContext.Result is HttpUnauthorizedResult))
                return;
            if (filterContext.HttpContext.Request.IsAjaxRequest())
                return;
            if (ConfigurationManager.AppSettings["IIS"] == "6")
                filterContext.Result = new RedirectResult("/utilisateur.mvc/Create");
            else
                filterContext.Result = new RedirectResult("/utilisateur/Create");
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            return httpContext.UserIsRegistered() ?? false;
        }
    }
}