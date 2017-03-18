using System;
using System.IO;
using System.Web;
using System.Web.Routing;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Models.Organizations;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy
{
    public interface ITenantUrlHelper
    {
        string GetReturnUrl();
        string GetRedirectUrl(Organization organization);
        string TransformTenantUrl(string virtualPath, string tenantKey);
    }

    public class TenantUrlHelper : ITenantUrlHelper
    {
        public ILogger Logger { get; set; }

        private readonly RequestContext requestContext;

        public TenantUrlHelper(RequestContext requestContext)
        {
            this.requestContext = requestContext;
        }

        public string GetReturnUrl()
        {
            Logger.Trace("GetReturnUrl");

            if (requestContext.RouteData.Values.ContainsKey("tenant"))
                requestContext.RouteData.Values.Remove("tenant");
            return ResolveStrictRoute(requestContext.RouteData.Values);
        }

        public string GetRedirectUrl(Organization organization)
        {
            Logger.Trace("GetRedirectUrl::{0}", organization.Abbreviation);

            var route = new RouteValueDictionary(requestContext.RouteData.Values)
            {
                {"tenant", organization.Abbreviation }
            };

            return ResolveStrictTenantRoute(route);
        }

        public string TransformTenantUrl(string virtualPath, string tenantKey)
        {
            Logger.Trace("TransformTenantUrl::{0}-{1}", virtualPath, tenantKey);

            var routeData = GetRouteDataForVirtualPath(virtualPath);

            if (routeData.Values.ContainsKey("tenant"))
                routeData.Values["tenant"] = tenantKey;
            else
                routeData.Values.Add("tenant", tenantKey);

            return ResolveStrictTenantRoute(routeData.Values);
        }

        private RouteData GetRouteDataForVirtualPath(string virtualPath)
        {
            var builder = new UriBuilder(requestContext.HttpContext.Request.Url)
            {
                Path = virtualPath,
                Query = string.Empty
            };

            var httpContext = new HttpContext(new HttpRequest(null, builder.Uri.ToString(), null), new HttpResponse(new StringWriter()));
            
            return RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
        }

        private string ResolveStrictTenantRoute(RouteValueDictionary route)
        {
            return RouteTable.Routes.GetVirtualPath(requestContext, "Tenant_Route", route).VirtualPath;
            
        }

        private string ResolveStrictRoute(RouteValueDictionary route)
        {
            return RouteTable.Routes.GetVirtualPath(requestContext, "Default_Strict", route).VirtualPath;
        }
    }
}