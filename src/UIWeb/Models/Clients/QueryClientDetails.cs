using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Restitution;
using Wiz.Gringotts.UIWeb.Models.Users;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    public class ClientDetailsQuery : IAsyncRequest<ClientDetails>
    {
        public int ClientId { get; private set; }

        public ClientDetailsQuery(int clientId)
        {
            this.ClientId = clientId;
        }
    }

    public class ClientDetailsQueryHandler : IAsyncRequestHandler<ClientDetailsQuery, ClientDetails>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Client> clients;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public ClientDetailsQueryHandler(ISearch<Client> clients, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.clients = clients;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public async Task<ClientDetails> Handle(ClientDetailsQuery query)
        {
            Logger.Trace("Handle");

            var organization = tenantOrganizationProvider.GetTenantOrganization();

            var client = await clients.GetById(query.ClientId)
                .Include(c => c.Identifiers)
                .Include(c => c.Residencies)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (client != null)
                return new ClientDetails
                {
                    Client = client,
                    Residency = client.Residencies
                    .First(r => r.OrganizationId == organization.Id)
                };

            return null;
        }
    }

    public class ClientDetailsAccountsPostProcessor: IAsyncPostRequestHandler<ClientDetailsQuery, ClientDetails>
    {
        private readonly ISearch<ClientAccount> accounts;
        public ILogger Logger { get; set; }

        public ClientDetailsAccountsPostProcessor(ISearch<ClientAccount> accounts)
        {
            this.accounts = accounts;
        }

        public async Task Handle(ClientDetailsQuery command, ClientDetails response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            response.Accounts = await accounts.All()
                .Where(a => a.Residency.Client.Id == response.Client.Id)
                .AsNoTracking()
                .ToArrayAsync();
        }
    }

    public class ClientDetailsRestitutionOrdersPostProcessor : IAsyncPostRequestHandler<ClientDetailsQuery, ClientDetails>
    {
        public ILogger Logger { get; set; }


        private readonly ISearch<Order> orders;

        public ClientDetailsRestitutionOrdersPostProcessor(ISearch<Order> orders)
        {
            this.orders = orders;
        }

        public async Task Handle(ClientDetailsQuery command, ClientDetails response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            response.Orders = await orders.All()
                .Where(o => !o.IsSatified)
                .Where(o => o.Residency.Client.Id == response.Client.Id)
                .OrderByDescending(o => o.IsPropertyDamage)
                .ThenBy(o => o.Created)
                .AsNoTracking()
                .ToArrayAsync();
        }
    }

    public class ClientDetailsViewModelPostProccesor : IAsyncPostRequestHandler<ClientDetailsQuery, ClientDetails>
    {
        public ILogger Logger { get; set; }

        private readonly IUserRepository userRepository;

        public ClientDetailsViewModelPostProccesor(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task Handle(ClientDetailsQuery command, ClientDetails response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            await Task.Run(() =>
            {
                if (!string.IsNullOrWhiteSpace(response.Client.CreatedBy))
                    response.CreatedBy = GetActiveDirectoryUser(response.Client.CreatedBy);

                if (!string.IsNullOrWhiteSpace(response.Client.UpdatedBy))
                    response.UpdatedBy = GetActiveDirectoryUser(response.Client.CreatedBy);
            });

        }

        private User GetActiveDirectoryUser(string samAccountName)
        {
            Logger.Trace("GetActiveDirectoryUser::{0}", samAccountName);

            return userRepository.FindByUser(samAccountName);
        }
    }
}