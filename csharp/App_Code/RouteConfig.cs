using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

/// <summary>
/// Summary description for RouteConfig
/// </summary>
public partial class AppConfig
{
	public static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        routes.MapRoute(
            "Default",
            "{page}/{id}",
            new { controller = "Default", action = "index", page = UrlParameter.Optional, id = UrlParameter.Optional },
            new { page = "(?!x).*"}
        );

        routes.MapRoute(
            "actions",
            "x/{action}/{id}",
            new { controller = "Default", action = "index", id = UrlParameter.Optional }
        );
    }
}