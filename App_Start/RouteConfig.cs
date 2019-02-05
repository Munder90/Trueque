using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TAT001
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Details",
               url: "Solicitudes/Details/{id}/{ejercicio}",
               defaults: new { controller = "Solicitudes", action = "Details", id = UrlParameter.Optional, ejercicio = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                 name: "Procesa",
                 url: "{controller}/{action}/{id}/{accion}"
             );

        }
    }
}
