using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models.Clients;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Restitution
{
    public class AddOrEditRestitutionOrderCommand : IAsyncRequest<ICommandResult>
    {
        public ModelStateDictionary ModelState { get; private set; }
        public RestitutionOrderEditorForm Editor { get; private set; }

        public AddOrEditRestitutionOrderCommand(RestitutionOrderEditorForm editor, ModelStateDictionary modelState)
        {
            this.ModelState = modelState;
            this.Editor = editor;
        }
    }

    public class AddOrEditRestitutionOrderCommandHandler : IAsyncRequestHandler<AddOrEditRestitutionOrderCommand, ICommandResult>
    {
        private readonly ISearch<Order> orders;
        private readonly ISearch<Client> clients;
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;
        public ILogger Logger { get; set; }

        public AddOrEditRestitutionOrderCommandHandler(ISearch<Order> orders, ISearch<Client> clients, ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.orders = orders;
            this.clients = clients;
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public async Task<ICommandResult> Handle(AddOrEditRestitutionOrderCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");

            if (message.Editor.OrderId.HasValue)
                return await Edit(message);

            return await Add(message);
        }

        private async Task<ICommandResult> Add(AddOrEditRestitutionOrderCommand message)
        {
            Logger.Trace("Add");

            var organization = tenantOrganizationProvider.GetTenantOrganization();
            var client = await clients.GetById(message.Editor.ClientId.Value)
                .FirstAsync();
            var residency = client.Residencies
                .First(r => r.OrganizationId == organization.Id);
            var order = message.Editor.BuildOrder(context, organization, residency);

            residency.Orders.Add(order);

            await context.SaveChangesAsync();

            Logger.Info("Add::Success Id:{0}", order.Id);

            return new SuccessResult(order.Id);

        }

        private async Task<ICommandResult> Edit(AddOrEditRestitutionOrderCommand message)
        {
            Logger.Trace("Edit::{0}", message.Editor.OrderId);

            var order = await orders.GetById(message.Editor.OrderId.Value)
                    .FirstAsync();

            message.Editor.UpdateOrder(order, context);

            await context.SaveChangesAsync();

            Logger.Info("Edit::Success Id:{0}", order.Id);

            return new SuccessResult(order.Id);
        }
    }

    public class AddOrEditRestitutionOrderPostHandler : IAsyncPostRequestHandler<AddOrEditRestitutionOrderCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Order> orders;
        private readonly IMediator mediator;

        public AddOrEditRestitutionOrderPostHandler(ISearch<Order> orders, IMediator mediator)
        {
            this.orders = orders;
            this.mediator = mediator;
        }

        public async Task Handle(AddOrEditRestitutionOrderCommand command, ICommandResult result)
        {
            Logger.Trace("Handle");

            if (result.IsFailure)
                return;

            var order = await orders.GetById((int) result.Result).FirstAsync();

            await mediator.PublishAsync(new RestitutionOrderAddedOrUpdatedNotification(command, order));
        }
    }
}