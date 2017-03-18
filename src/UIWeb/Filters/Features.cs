using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models.Features;

namespace Wiz.Gringotts.UIWeb.Filters
{
    public class Features : ActionFilterAttribute
    {
        public ILogger Logger { get; set; }

        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public Features(ApplicationDbContext context, 
            ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Logger.Trace("OnActionExecuting");

            var tenant = tenantOrganizationProvider.GetTenantOrganization();

            if (tenant == null) return;

            filterContext.Controller.ViewBag.Features = 
                new FeatureChecker(tenant.Features.ToHashSet());
        }
    }
}