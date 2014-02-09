using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NietoYostenMvc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                null,
                url: "news",
                defaults: new { controller = "Home", action = "ShowSection", section = "news" }
            );

            routes.MapRoute(
                null,
                url: "sections/{section}",
                defaults: new { controller = "Home", action = "ShowSection", section = "citas" }
            );

            routes.MapRoute(
                null,
                url: "pictures",
                defaults: new { controller = "Pictures", action = "Index" }
            );

            routes.MapRoute(
                null,
                url: "pictures/{album}",
                defaults: new { controller = "Pictures", action = "ShowAlbum" }
            );

            routes.MapRoute(
                null,
                url: "pictures/{album}/{picture}",
                defaults: new { controller = "Pictures", action = "ShowPicture" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}