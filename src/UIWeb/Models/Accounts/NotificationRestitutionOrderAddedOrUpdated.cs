using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Features;
using Wiz.Gringotts.UIWeb.Models.Restitution;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Accounts
{
    public class NotificationEnsureClientRestitutionHandler : IAsyncNotificationHandler<RestitutionOrderAddedOrUpdatedNotification>
    {
        public ILogger Logger { get; set; }

        private readonly IFeatureService featureService;
        private readonly ILookup<AccountType> accountTypeLookup;
        private readonly IUnitOfWork unitOfWork;

        public NotificationEnsureClientRestitutionHandler(IFeatureService featureService, ILookup<AccountType> accountTypeLookup, IUnitOfWork unitOfWork)
        {
            this.featureService = featureService;
            this.accountTypeLookup = accountTypeLookup;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(RestitutionOrderAddedOrUpdatedNotification notification)
        {
            Logger.Trace("Handle");

            if (!featureService.IsEnabled(Feature.Restitution))
                return;

            var residency = notification.Order.Residency;
            var type = accountTypeLookup.All.First(t => t.Name == AccountType.Restitution);

            if (residency.Accounts.Any(a => a.AccountType.Name == type.Name))
                return;

            var account = new ClientAccount
            {
                Fund = residency.Accounts.Select(a => a.Fund).First(),
                Residency = residency,
                Name = type.Name,
                AccountType = type
            };

            residency.Accounts.Add(account);

            await unitOfWork.SaveChangesAsync();

        }
    }
}