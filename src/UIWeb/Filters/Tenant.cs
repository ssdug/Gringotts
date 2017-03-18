using System.Web.Mvc;
using System.Web.Routing;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models.Organizations;

namespace Wiz.Gringotts.UIWeb.Filters
{
    /// <summary>
    /// Ensures that requests for routes that require a tenant key
    /// have an tenant key set. If no tenant key is present, user 
    /// is redirected to the tenant selection page. Once selected
    /// the user will be returned to their original destination.
    /// </summary>
    public class Tenant : ActionFilterAttribute
    {
        public ILogger Logger { get; set; }
        public ITenantOrganizationProvider TenantOrganizationProvider { get; set; }
        public ITenantUrlHelper TenantUrlHelper { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Logger.Trace("OnActionExecuting");

            var organization = TenantOrganizationProvider.GetTenantOrganization();
            var availableOrganizations = TenantOrganizationProvider.GetAllTenantOrganizationsForUser();

            if (organization != null && filterContext.RouteData.Values.ContainsKey("tenant"))
            {
                filterContext.Controller.ViewBag.Organization = organization;
                filterContext.Controller.ViewBag.TenantBreadcrumbs = 
                    new TenantBreadcrumb{ CurrentTenantOrganization  = organization, AvailableTenantOrganizations = availableOrganizations };
                return;
            }
                


            if (organization != null)
            {
                filterContext.Result = new RedirectResult(TenantUrlHelper.GetRedirectUrl(organization));
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult("Select_Tenant_Route", 
                    new RouteValueDictionary{
                    { "ReturnUrl", TenantUrlHelper.GetReturnUrl() }
                });
            }
        }
    }
}