using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    public class ClientActivationToggledNotification : IAsyncNotification
    {
        public Client Client { get; private set; }

        public ClientActivationToggledNotification(Client client)
        {
            this.Client = client;
        }
    }
}