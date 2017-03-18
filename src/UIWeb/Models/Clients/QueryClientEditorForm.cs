using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    public class ClientEditorFormQuery : IAsyncRequest<ClientEditorForm>
    {
        public int? ClientId { get; private set; }

        public ClientEditorFormQuery(int? clientId = null)
        {
            this.ClientId = clientId;
        }
    }

    public class ClientEditorFormQueryHandler : IAsyncRequestHandler<ClientEditorFormQuery, ClientEditorForm>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Client> clients;
        private readonly ILookup<ClientIdentifierType> identifierTypes;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public ClientEditorFormQueryHandler(ISearch<Client> clients, ILookup<ClientIdentifierType> identifierTypes, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.clients = clients;
            this.identifierTypes = identifierTypes;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public async Task<ClientEditorForm> Handle(ClientEditorFormQuery message)
        {
            Logger.Trace("Handle");

            var organization = tenantOrganizationProvider.GetTenantOrganization();
            var identifierTypes = GetIdentifierTypes();
            
            if (message.ClientId.HasValue)
            {
                var client = await GetClient(message.ClientId.Value);
                if(client != null)
                    return ClientEditorForm.FromClient(client, organization, identifierTypes);
            }

            return new ClientEditorForm
            {
                IdentifierTypes = identifierTypes
            };
        }

        private async Task<Client> GetClient(int clientId)
        {
            Logger.Trace("GetClient::{0}",clientId);

            var result = await clients.GetById(clientId)
                .Include(c => c.Identifiers)
                .Include(c => c.Residencies)
                .Include("Residencies.Attorneys")
                .Include("Residencies.Guardians")
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (result == null)
                Logger.Warn("client {0} not found", clientId);

            return result;
        }

        private ClientEditorForm.IdentifierType[]  GetIdentifierTypes()
        {
            Logger.Trace("GetIdentifierTypes");

            var result = identifierTypes.All
                .Select(t => new ClientEditorForm.IdentifierType
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToArray();

            if (!result.Any())
                Logger.Warn("no client identifier types found");

            return result;
        }
    }
}