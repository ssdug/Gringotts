using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Glimpse.Core.Extensions;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class TransferAmountCommand : IAsyncRequest<ICommandResult>
    {
        public TrasferAmountEditorForm Editor { get; private set; }
        public ModelStateDictionary ModelState { get; private set; }

        public TransferAmountCommand(TrasferAmountEditorForm editor, ModelStateDictionary modelState)
        {
            this.Editor = editor;
            this.ModelState = modelState;
        }
    }

    public class TransferAmountCommandHandler : IAsyncRequestHandler<TransferAmountCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Payee> payees;
        private readonly ISearch<Account> accounts;
        private readonly ApplicationDbContext context;

        public TransferAmountCommandHandler(ISearch<Payee> payees, ISearch<Account> accounts, ApplicationDbContext context)
        {
            this.payees = payees;
            this.accounts = accounts;
            this.context = context;
        }

        public async Task<ICommandResult> Handle(TransferAmountCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Unable to transfer {0:C} from account {1} to account {2}"
                    .FormatWith(message.Editor.Amount, message.Editor.FromAccountId, message.Editor.ToAccountId));

            var payee = await payees.GetById(message.Editor.PayeeId)
                .FirstAsync();

            var fromAccount = await accounts.GetById(message.Editor.FromAccountId)
                .FirstAsync();

            var expeneseType = await context.TransactionSubTypes.OfType<ExpenseType>()
                .FirstAsync(t => t.Name == ExpenseType.Transfer);
            
            var expense = new Expense
            {
                ExpenseType = expeneseType,
                Amount = message.Editor.Amount,
                Memo = "Transfer",
                Fund = fromAccount.Fund,
                Account = fromAccount,
                Payee = payee
            };

            fromAccount.AddExpense(expense);

            var toAccount = await accounts.GetById(message.Editor.ToAccountId)
                .FirstAsync();

            var receiptType = await context.TransactionSubTypes.OfType<ReceiptType>()
                .FirstAsync(t => t.Name == ReceiptType.Transfer);

            var receipt = new Receipt
            {
                ReceiptType = receiptType,
                Amount = message.Editor.Amount,
                ReceivedFrom = fromAccount.Name,
                Fund = fromAccount.Fund,
                Account = toAccount
            };

            toAccount.AddReceipt(receipt);

            var transferBatch = new TransferBatch
            {
                ParentId = message.Editor.ParentBatchId,
                TriggerId = message.Editor.TriggerTransactionId,
                TriggerAccountId = message.Editor.TriggerTransactionAccountId,
                FundId = fromAccount.FundId,
                ExpectedAmount = 0,
                FromAccount = fromAccount,
                ToAccount = toAccount,
                Effective = DateTime.UtcNow,
                Committed = true,
                Transactions = new Transaction[]{ expense, receipt }
            };

            context.Batches.Add(transferBatch);

            await context.SaveChangesAsync();

            return await Task.FromResult(new SuccessResult(transferBatch.Id));
        }
    }

    public class BatchTransferReferenceNumberPostProcessor : IAsyncPostRequestHandler<TransferAmountCommand,ICommandResult>
    {
        private readonly ISearch<TransferBatch> batches;
        private readonly ApplicationDbContext context;
        public ILogger Logger { get; set; }

        public BatchTransferReferenceNumberPostProcessor(ISearch<TransferBatch> batches, ApplicationDbContext context)
        {
            this.batches = batches;
            this.context = context;
        }

        public async Task Handle(TransferAmountCommand command, ICommandResult result)
        {
            Logger.Trace("Handle");

            if (result.IsFailure)
                return;

            var batch = await batches.GetById(result.Result.CastOrDefault<int>())
                .Include(b => b.Transactions)
                .Include(b => b.Organization)
                .FirstOrDefaultAsync();

            if (batch != null)
            {
 
                AddBatchReferenceNumbers(batch);

                await context.SaveChangesAsync();
            }
        }

        private string GenerateReferenceNumber(TransferBatch batch)
        {
            return "{0}-{1}".FormatWith(batch.Organization.Abbreviation, batch.Id.ToString("D9"));
        }

        private void AddBatchReferenceNumbers(TransferBatch batch)
        {
            var referenceNumber = GenerateReferenceNumber(batch);
            batch.BatchReferenceNumber = referenceNumber;

            batch.Transactions
                .ForEach((t, i) =>
                {
                    t.BatchReferenceNumber = "{0}-{1}".FormatWith(referenceNumber, i + 1);
                });
        }
    }
}