using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Models.Features;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    public class OrganizationEditorFormQuery : IAsyncRequest<OrganizationEditorForm>
    {
        public int? OrganizationId { get; set; }
        public int? ParentOrganizationId { get; set; }

        public OrganizationEditorFormQuery(int? organizationId = null, int? parentOrganizationId = null)
        {
            OrganizationId = organizationId;
            ParentOrganizationId = parentOrganizationId;
        }
    }

    public class OrganizationEditorFormQueryHandler : IAsyncRequestHandler<OrganizationEditorFormQuery, OrganizationEditorForm>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Organization> organizations;
        private readonly ILookup<Feature> features;


        public OrganizationEditorFormQueryHandler(ISearch<Organization> organizations, ILookup<Feature> features)
        {
            this.organizations = organizations;
            this.features = features;
        }

        public async Task<OrganizationEditorForm> Handle(OrganizationEditorFormQuery message)
        {
            Logger.Trace("Handle");

            var availableFeatures =  features.All.ToArray();

            if (message.OrganizationId.HasValue)
            {
                var organization = await GetOrganization(message.OrganizationId.Value);
                if(organization != null)
                    return OrganizationEditorForm.FromOrganization(organization, availableFeatures);
            }

            if (message.ParentOrganizationId.HasValue)
            {
                var parent = await GetOrganization(message.ParentOrganizationId.Value);
                if(parent != null)
                    return OrganizationEditorForm.FromParentOrganization(parent, availableFeatures);
            }

            return new OrganizationEditorForm {AvailableFeatures = availableFeatures};
        }

        private async Task<Organization> GetOrganization(int organizationId)
        {
            Logger.Trace("GetOrganization::{0}", organizationId);

            var result = await organizations.GetById(organizationId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (result == null)
                Logger.Warn("organization {0} not found", organizationId);

            return result;
        }
    }
}