using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    public class ClientAddedOrUpdatedNotification : IAsyncNotification
    {
        public AddOrEditClientCommand Request { get; private set; }
        public Client Client { get; private set; }

        public ClientAddedOrUpdatedNotification(AddOrEditClientCommand request, Client client)
        {
            this.Request = request;
            this.Client = client;
        }
    }
}