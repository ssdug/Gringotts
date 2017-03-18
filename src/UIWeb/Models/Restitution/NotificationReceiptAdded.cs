using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Features;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Restitution
{
    public class ReceiptAddedRestitutionNotificationHandler : IAsyncNotificationHandler<ReceiptAddedNotification>
    {
        public ILogger Logger { get; set; }

        private readonly ApplicationDbContext context;
        private readonly IFeatureService featureService;
        private readonly IMediator mediator;

        public ReceiptAddedRestitutionNotificationHandler(ApplicationDbContext context, IFeatureService featureService, IMediator mediator)
        {
            this.context = context;
            this.featureService = featureService;
            this.mediator = mediator;
        }

        public async Task Handle(ReceiptAddedNotification notification)
        {
            Logger.Trace("Handle");

            if (!featureService.IsEnabled(Feature.Restitution))
                return;

            if (notification.Receipt.ReceiptType.Name != ReceiptType.Payroll)
                return;

            if (!(notification.Receipt.Account is ClientAccount))
                return;

            var receipt = notification.Receipt;
            var account = receipt.Account as ClientAccount;
            var residency = account.Residency;

            var order = await context.Orders
                .Where(o => !o.IsSatified)
                .Where(o => o.OrganizationId == receipt.OrganizationId)
                .Where(o => o.Residency.ClientId == residency.ClientId)
                .OrderByDescending(o => o.IsPropertyDamage)
                .ThenBy(o => o.Created)
                .FirstOrDefaultAsync();

            if (order != null)
            {
                var restitutionAccount = residency.Accounts.First(a => a.AccountType.Name == AccountType.Restitution);
                var form = new TrasferAmountEditorForm
                {
                    ParentBatchId = receipt.TransactionBatchId,
                    TriggerTransactionId = receipt.Id,
                    TriggerTransactionAccountId = receipt.AccountId,
                    FromAccountId = account.Id,
                    ToAccountId = restitutionAccount.Id,
                    PayeeId = order.PayeeId,
                    Amount = GetAmount(receipt.Amount, order.WithholdingPercent)
                };

                var result = await mediator.SendAsync(new TransferAmountCommand(form, new ModelStateDictionary()));

                if (result is FailureResult)
                    throw new RestitutionOrderTransferException(result.Result.ToString());
            }


            await context.SaveChangesAsync();
        }

        public static decimal GetAmount(decimal amount, int withholdingPercent)
        {
            var percent = withholdingPercent/100M;
            var result = Math.Round(amount * percent, 2);
            return result;
        }
    }

    public class RestitutionOrderTransferException : Exception
    {
        public RestitutionOrderTransferException(string message): base(message)
        { }
    }
}