using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Clients;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Accounts
{
    public class NotificationClientActivationToggledHandler : IAsyncNotificationHandler<ClientActivationToggledNotification>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ISearch<Account> accounts;
        public ILogger Logger { get; set; }

        public NotificationClientActivationToggledHandler(IUnitOfWork unitOfWork, ISearch<Account> accounts)
        {
            this.unitOfWork = unitOfWork;
            this.accounts = accounts;
        }

        public Task Handle(ClientActivationToggledNotification notification)
        {
            Logger.Trace("Handle");
            
            accounts.All().OfType<ClientAccount>()
                .Where(a => a.Residency.ClientId == notification.Client.Id)
                .ForEach(a =>
                {
                    a.IsActive = !a.IsActive;
                });

            return unitOfWork.SaveChangesAsync();
        }

    }
}