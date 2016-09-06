using System.Web.Mvc;
using System.Web.Routing;

namespace Traffix.Common
{
    public class CommonRouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Home", "", new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "NotFound",
                url: "notfound",
                defaults: new { controller = "Home", action = "NotFound" }
            );

            routes.MapRoute(
                name: "Error",
                url: "Error",
                defaults: new { controller = "Home", action = "Error" }
            );
        }
    }
}