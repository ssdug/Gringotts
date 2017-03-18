using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Models.Clients;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Restitution
{
    public class RestitutionOrderEditorFormQuery : IAsyncRequest<RestitutionOrderEditorForm>
    {
        public int? ClientId { get; private set; }
        public int? OrderId { get;  private set; }

        public RestitutionOrderEditorFormQuery(int? clientId = null, int? orderId = null)
        {
            this.ClientId = clientId;
            this.OrderId = orderId;
        }
    }

    public class RestitutionOrderEditorFormQueryHandler : IAsyncRequestHandler<RestitutionOrderEditorFormQuery, RestitutionOrderEditorForm>
    {
        private readonly ISearch<Order> orders;
        private readonly ISearch<Client> clients;
        public ILogger Logger { get; set; }

        public RestitutionOrderEditorFormQueryHandler(ISearch<Order> orders, ISearch<Client> clients)
        {
            this.orders = orders;
            this.clients = clients;
        }

        public async Task<RestitutionOrderEditorForm> Handle(RestitutionOrderEditorFormQuery message)
        {
            Logger.Trace("Handle::{0}", message.ClientId);

            if (message.ClientId.HasValue)
            {
                var client = await clients.GetById(message.ClientId.Value)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (client != null)
                    return new RestitutionOrderEditorForm
                    {
                        ClientId = client.Id
                    };
            }

            if (message.OrderId.HasValue)
            {
                var order = await orders.GetById(message.OrderId.Value)
                    .AsNoTracking()
                    .FirstAsync();

                if (order != null)
                    return RestitutionOrderEditorForm.FromOrder(order);
            }

            return null;
        }
    }
}