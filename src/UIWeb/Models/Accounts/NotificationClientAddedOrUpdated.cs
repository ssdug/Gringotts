using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Features;
using Wiz.Gringotts.UIWeb.Models.Funds;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Accounts
{
    public class NotificationEnsureClientAccountsHandler : IAsyncNotificationHandler<ClientAddedOrUpdatedNotification>
    {
        public ILogger Logger { get; set; }
        
        private readonly ISearch<Account> accounts;
        private readonly ISearch<Fund> funds;
        private readonly ILookup<AccountType> accountTypeLookup;
        private readonly IUnitOfWork unitOfWork;
        private readonly IFeatureService featureService;

        public NotificationEnsureClientAccountsHandler(IUnitOfWork unitOfWork, IFeatureService featureService, ISearch<Account> accounts, ISearch<Fund> funds, ILookup<AccountType> accountTypeLookup)
        {
            this.unitOfWork = unitOfWork;
            this.featureService = featureService;
            this.accounts = accounts;
            this.funds = funds;
            this.accountTypeLookup = accountTypeLookup;
        }

        public async Task Handle(ClientAddedOrUpdatedNotification notification)
        {
            Logger.Trace("Handle");

            if (!featureService.IsEnabled(Feature.Funds))
                return;

            var fund = await funds.All().OfType<ClientFund>()
                .FirstOrDefaultAsync();

            var existingAccounts = await accounts.All().OfType<ClientAccount>()
                .Where(a => a.Residency.Client.Id == notification.Client.Id)
                .ToArrayAsync();

            var accountTypes = accountTypeLookup.All
                .Where(a => a.IsDefault)
                .ToArray();

            accountTypes.Where(t => existingAccounts.All(a => a.AccountType.Id != t.Id))
                .ForEach(accountType =>
                {
                    var account = accountType.BuildClientAccount(notification.Client, fund.Organization);

                    if (account.Name.Equals("Savings", StringComparison.InvariantCultureIgnoreCase))
                    {
                        account.BankNumber = notification.Request.Editor.BankAccount;
                    }

                    account.Fund = fund;
                    fund.ClientAccounts.Add(account);

                    Logger.Info("Created {0}  for client {1}, {2}",
                        account.Name, notification.Client.LastName, notification.Client.FirstName);
                });

            await unitOfWork.SaveChangesAsync();
        }
    }
}