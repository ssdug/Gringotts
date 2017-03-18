using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Payees
{
    public class PayeeDetailsQuery : IAsyncRequest<PayeeDetails>
    {
        public int PayeeId { get; private set; }

        public PayeeDetailsQuery(int payeeId)
        {
            this.PayeeId = payeeId;
        }
    }

    public class PayeeDetailsQueryHandler : IAsyncRequestHandler<PayeeDetailsQuery, PayeeDetails>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Payee> payees;
        private readonly ISearch<Client> clients;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public PayeeDetailsQueryHandler(ISearch<Payee> payees, ISearch<Client> clients, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.payees = payees;
            this.clients = clients;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public async Task<PayeeDetails> Handle(PayeeDetailsQuery query)
        {
            Logger.Trace("Handle::{0}", query.PayeeId);

            var organization = tenantOrganizationProvider.GetTenantOrganization();

            var payee = await payees.GetById(query.PayeeId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (payee != null)
                return new PayeeDetails
                {
                    Payee = payee,
                    AttorneyClients = await GetAttorneyClients(payee, organization),
                    GuardianClients = await GetGuardianClients(payee, organization)
                };

            return null;
        }

        private Task<List<Client>> GetAttorneyClients(Payee payee, Organization organization)
        {
            return clients.All()
                .Where(c => c.Residencies.Any(r => r.IsActive && r.OrganizationId == organization.Id))
                .Where(c => c.Residencies.Any(r => r.Attorneys.Any(a => a.Id == payee.Id)))
                .AsNoTracking()
                .ToListAsync();
        }

        private Task<List<Client>> GetGuardianClients(Payee payee, Organization organization)
        {
            return clients.All()
                .Where(c => c.Residencies.Any(r => r.IsActive && r.OrganizationId == organization.Id))
                .Where(c => c.Residencies.Any(r => r.Attorneys.Any(a => a.Id == payee.Id)))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}