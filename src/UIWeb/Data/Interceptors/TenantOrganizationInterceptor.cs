using System.Data.Entity.Infrastructure;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models;

namespace Wiz.Gringotts.UIWeb.Data.Interceptors
{
    public class TenantOrganizationInterceptor : ChangeInterceptor<IBelongToOrganization>
    {
        public ILogger Logger { get; set; }

        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public TenantOrganizationInterceptor(ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        protected override void OnBeforeInsert(DbEntityEntry entry, IBelongToOrganization item, InterceptionContext context)
        {
            Logger.Trace("OnBeforeInsert");

            if(item.Organization == null)
                item.Organization = tenantOrganizationProvider.GetTenantOrganization();

            base.OnBeforeInsert(entry, item, context);
        }
    }
}