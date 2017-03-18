using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    public class SelectTenantOrganizationQuery : IAsyncRequest<OrganizationTenantList>
    {
        public string ReturnUrl { get; private set; }

        public SelectTenantOrganizationQuery(string returnUrl)
        {
            this.ReturnUrl = returnUrl;
        }
    }

    public class SelectTenantOrganizationQueryHandler : IAsyncRequestHandler<SelectTenantOrganizationQuery, OrganizationTenantList> 
    {
        public ILogger Logger { get; set; }

        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public SelectTenantOrganizationQueryHandler(ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public Task<OrganizationTenantList> Handle(SelectTenantOrganizationQuery message)
        {
            Logger.Trace("Handle");

            var tenantOrganizations = tenantOrganizationProvider.GetAllTenantOrganizationsForUser();
            return Task.FromResult(new OrganizationTenantList(message.ReturnUrl, tenantOrganizations));
         
        }
    }
}