﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace Mvc.Helper
{
    public static class RouteExtension
    {
        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults)
        {
            return routes.MapRouteLowerCase(name, url, defaults, null);
        }

        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults, string constraints)
        {
            Route route = new LowercaseRoute(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints)
            };

            routes.Add(name, route);

            return route;
        }
    }

    public class LowercaseRoute : Route
    {
        public LowercaseRoute(string url, IRouteHandler routeHandler) : base(url, routeHandler) { }
        public LowercaseRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler) : base(url, defaults, routeHandler) { }
        public LowercaseRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler) : base(url, defaults, constraints, routeHandler) { }
        public LowercaseRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler) : base(url, defaults, constraints, dataTokens, routeHandler) { }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            VirtualPathData path = base.GetVirtualPath(requestContext, values);

            if (path != null)
                path.VirtualPath = path.VirtualPath.ToLowerInvariant();

            return path;
        }
    }
}