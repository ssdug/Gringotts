using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    public class ToggleClientIsActiveCommand : IAsyncRequest<ICommandResult>
    {
        public int? ClientId { get; private set; }

        public ToggleClientIsActiveCommand(int? clientId)
        {
            this.ClientId = clientId;
        }
    }

    public class ToggleClientIsActiveCommandHandler : IAsyncRequestHandler<ToggleClientIsActiveCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Client> clients;
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public ToggleClientIsActiveCommandHandler(ISearch<Client> clients, ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.clients = clients;
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public async Task<ICommandResult> Handle(ToggleClientIsActiveCommand command)
        {
            Logger.Trace("Handle");

            var organization = tenantOrganizationProvider.GetTenantOrganization();
            var client = await clients.GetById(command.ClientId.Value)
                .FirstOrDefaultAsync();
            
            if (client != null && organization != null)
            {
                client.Residencies
                    .Where(r => r.OrganizationId == organization.Id)
                    .ForEach(r =>
                    {
                        r.IsActive = !r.IsActive;
                    });

                await context.SaveChangesAsync();
                Logger.Info("Handle::Success Id:{0} IsActive:Toggled", client.Id);
                return new SuccessResult(client.Id);
            }

            return new FailureResult("Client {0} not found.".FormatWith(command.ClientId));
        }
    }

    public class ToggleClientAccountsPostHandler : IAsyncPostRequestHandler<ToggleClientIsActiveCommand, ICommandResult>
    {
        private readonly ISearch<Client> clients;
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public ToggleClientAccountsPostHandler(ISearch<Client> clients, IMediator mediator)
        {
            this.clients = clients;
            this.mediator = mediator;
        }

        public async Task Handle(ToggleClientIsActiveCommand command, ICommandResult result)
        {
            Logger.Trace("Handle");

            if (result.IsFailure)
                return;

            var client = await clients.GetById((int) result.Result)
                .FirstAsync();

            await mediator.PublishAsync(new ClientActivationToggledNotification(client));
        }
    }
}