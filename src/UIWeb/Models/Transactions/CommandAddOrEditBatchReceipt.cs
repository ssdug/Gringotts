using System.Collections.Generic;
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
using Glimpse.Core.Extensions;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class AddOrEditBatchReceiptCommand : IAsyncRequest<ICommandResult>
    {
        public BatchReceiptEditorForm Editor { get; set; }
        public ModelStateDictionary ModelState { get; set; }

        public AddOrEditBatchReceiptCommand(BatchReceiptEditorForm editor, ModelStateDictionary modelState)
        {
            Editor = editor;
            ModelState = modelState;
        }
    }

    public class AddOrEditBatchReceiptCommandHandler :IAsyncRequestHandler<AddOrEditBatchReceiptCommand, ICommandResult>
    {
        private readonly ISearch<Account> accounts;
        private readonly ISearch<ReceiptBatch> batches;
        private readonly ApplicationDbContext context;
        public ILogger Logger { get; set; }

        public AddOrEditBatchReceiptCommandHandler(ISearch<Account> accounts, ISearch<ReceiptBatch> batches, ApplicationDbContext context)
        {
            this.accounts = accounts;
            this.batches = batches;
            this.context = context;
        }

        public async Task<ICommandResult> Handle(AddOrEditBatchReceiptCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");

            if (message.Editor.BatchId.HasValue)
            {
                return await Edit(message);
            }

            return await Add(message);
        }

        private Task<ICommandResult> Add(AddOrEditBatchReceiptCommand message)
        {
            if (message.Editor is ClientBatchReceiptEditorForm)
            { return Add(message.Editor as ClientBatchReceiptEditorForm); }

            if (message.Editor is SubsidiaryBatchReceiptEditorForm)
            { return Add(message.Editor as SubsidiaryBatchReceiptEditorForm); }

            return Task.FromResult((ICommandResult)new FailureResult("Unknown editor type: {0}".FormatWith(message.Editor.GetType())));
        }

        private async Task<ICommandResult> Add(SubsidiaryBatchReceiptEditorForm editor)
        {
            var batch = await CreateBatch(editor);
            await CreateTransactions(editor, batch);

            return new SuccessResult(batch.Id);
        }

        private Task CreateTransactions(SubsidiaryBatchReceiptEditorForm editor, ReceiptBatch batch)
        {
            var batchAccounts = GetAccounts(editor);

            editor.Transactions
                  .Select(t => t.BuildReceipt(batch, context))
                  .ForEach(t =>
                  {
                      batchAccounts.First(x => x.Id == t.AccountId)
                        .AddReceipt(t);
                  });

            return context.SaveChangesAsync();
        }

        private IEnumerable<Account> GetAccounts(SubsidiaryBatchReceiptEditorForm editor)
        {
            var ids = editor.Transactions
                .Select(t => t.AccountId)
                .ToArray();

            return accounts.All()
                .Where(a => ids.Contains(a.Id))
                .ToArray();
        }

        private async Task<ICommandResult> Add(ClientBatchReceiptEditorForm editor)
        {
            var batch = await CreateBatch(editor);
            await CreateTransactions(editor, batch);

            return new SuccessResult(batch.Id);
        }

        private Task CreateTransactions(ClientBatchReceiptEditorForm editor, ReceiptBatch batch)
        {
            var batchAccounts = GetAccounts(editor);

            editor.Transactions
                  .Select(t => t.BuildReceipt(batch, context))
                  .ForEach(t =>
                  {
                      batchAccounts.First(x => x.Id == t.AccountId)
                        .AddReceipt(t);
                  });

            return context.SaveChangesAsync();
        }

        private IEnumerable<Account> GetAccounts(ClientBatchReceiptEditorForm editor)
        {
            var ids = editor.Transactions
                .Select(t => t.AccountId)
                .ToArray();

            return accounts.All()
                .Where(a => ids.Contains(a.Id))
                .ToArray();
        }

        private async Task<ReceiptBatch> CreateBatch(BatchReceiptEditorForm editor)
        {
            var batch = editor.BuildBatch(context);

            context.Batches.Add(batch);

            await context.SaveChangesAsync();

            return batch;
        }

        private async Task<ICommandResult> Edit(AddOrEditBatchReceiptCommand message)
        {
            Logger.Trace("Edit::{0}", message.Editor.BatchId);

            var batch = await batches.GetById(message.Editor.BatchId.Value)
                .Include(b => b.Transactions)
                .FirstOrDefaultAsync();

            if (batch.Committed)
            {
                return new FailureResult("Batch {0} is committed and may not be edited.".FormatWith(batch.Id));
            }

            message.Editor.UpdateBatch(batch, context);

            await context.SaveChangesAsync();

            return new SuccessResult(batch.Id);
        }
    }

    public class BatchReceiptReferenceNumberPostProecssor : IAsyncPostRequestHandler<AddOrEditBatchReceiptCommand, ICommandResult>
    {
        private readonly ISearch<ReceiptBatch> batches;
        private readonly ApplicationDbContext context;
        public ILogger Logger { get; set; }

        public BatchReceiptReferenceNumberPostProecssor(ISearch<ReceiptBatch> batches, ApplicationDbContext context)
        {
            this.batches = batches;
            this.context = context;
        }

        public async Task Handle(AddOrEditBatchReceiptCommand command, ICommandResult result)
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
                if (command.Editor.BatchId.HasValue)
                {
                    EditBatchReferenceNumbers(batch);
                }
                else
                {
                    AddBatchReferenceNumbers(batch);
                }

                await context.SaveChangesAsync();
            }

        }

        private string GenerateReferenceNumber(ReceiptBatch batch)
        {
            return "{0}-{1}".FormatWith(batch.Organization.Abbreviation, batch.Id.ToString("D9"));
        }

        private void EditBatchReferenceNumbers(ReceiptBatch batch)
        {
            var referenceNumber = batch.BatchReferenceNumber ?? GenerateReferenceNumber(batch);
            batch.Transactions
                .ForEach((t, i) =>
                {
                    t.BatchReferenceNumber = "{0}-{1}".FormatWith(referenceNumber, i + 1);
                });
        }

        private void AddBatchReferenceNumbers(ReceiptBatch batch)
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