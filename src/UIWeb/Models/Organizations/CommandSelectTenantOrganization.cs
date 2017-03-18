using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    public class SelectTenantOrganizationCommand : IAsyncRequest<ICommandResult>
    {
        public string ReturnUrl { get; private set; }
        public string TenantKey { get; private set; }

        public SelectTenantOrganizationCommand(string tenantKey, string returnUrl)
        {
            this.ReturnUrl = returnUrl;
            this.TenantKey = tenantKey;
        }
    }


    public class SelectTenantOrganizationCommandHandler : IAsyncRequestHandler<SelectTenantOrganizationCommand, ICommandResult> {
        public ILogger Logger { get; set; }

        private readonly ISearch<Organization> organizations;
        private readonly ITenantKeyManager tenantKeyManager;
        private readonly ITenantUrlHelper tenantUrlHelper;

        public SelectTenantOrganizationCommandHandler(ISearch<Organization> organizations, ITenantKeyManager tenantKeyManager, ITenantUrlHelper tenantUrlHelper)
        {
            this.organizations = organizations;
            this.tenantKeyManager = tenantKeyManager;
            this.tenantUrlHelper = tenantUrlHelper;
        }

        public async Task<ICommandResult> Handle(SelectTenantOrganizationCommand message)
        {
            Logger.Trace("Handle");

            var organization = await organizations.All()
                .FirstOrDefaultAsync(o => o.Abbreviation.Equals(message.TenantKey, StringComparison.InvariantCultureIgnoreCase));

            if (organization != null)
            {
                tenantKeyManager.SetTenantKey(organization.Abbreviation);
                return new SuccessResult(tenantUrlHelper.TransformTenantUrl(message.ReturnUrl, organization.Abbreviation));
            }

            return new FailureResult("Organization Abbreviation '{0}' does not exist.".FormatWith(message.TenantKey));
        }
    }
}