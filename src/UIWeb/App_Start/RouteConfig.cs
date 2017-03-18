using System.Web.Mvc;
using System.Web.Routing;

namespace Wiz.Gringotts.UIWeb
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Select_Tenant_Route",
                url: "Organizations/Select/{tenantkey}",
                defaults: new { controller = "Organizations", action = "Select", tenantKey = UrlParameter.Optional },
                constraints: new { tenantkey = @"[A-Za-z]+|" } //tenantkey should be a word or nothing
            );

            routes.MapRoute(
                name: "Tenant_Route",
                url: "{tenant}/{controller}/{action}/{id}",
                defaults: new { id = UrlParameter.Optional },
                constraints: new { action = @"[A-Za-z]+", id = @"\d+|" } //action should be a word, id should be a number or nothing
            );

            routes.MapRoute(
                name: "Default_Strict",
                url: "{controller}/{action}/{id}",
                defaults: new { id = UrlParameter.Optional },
                constraints: new { id = @"\d+|" } //id should be a number or nothing
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Root", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d+|" } //id should be a number or nothing
            );
        }
    }
}
