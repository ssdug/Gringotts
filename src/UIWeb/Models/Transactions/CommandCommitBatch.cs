using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class CommitBatchCommand : IAsyncRequest<ICommandResult>
    {
        public int? BatchId { get; private set; }

        public CommitBatchCommand(int? batchId)
        {
            this.BatchId = batchId;
        }
    }

    public class CommitBatchCommandHandler : IAsyncRequestHandler<CommitBatchCommand, ICommandResult>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ISearch<TransactionBatch> batches;

        public CommitBatchCommandHandler(ISearch<TransactionBatch> batches, IUnitOfWork unitOfWork)
        {
            this.batches = batches;
            this.unitOfWork = unitOfWork;
        }

        public ILogger Logger { get; set; }

        public async Task<ICommandResult> Handle(CommitBatchCommand message)
        {
            Logger.Trace("Handle");

            var batch = await batches.GetById(message.BatchId ?? -1)
                .FirstOrDefaultAsync();

            if (batch != null)
            {
                batch.Committed = true;
                await unitOfWork.SaveChangesAsync();
                Logger.Info("Handle::Success Id:{0} Committed:{1}", batch.Id, batch.Committed);
                return new SuccessResult(batch.Id);
            }

            return new FailureResult("Batch {0} not found.".FormatWith(message.BatchId));
        }
    }

    public class CommitReceiptBatchCommandPostProcessor : IAsyncPostRequestHandler<CommitBatchCommand, ICommandResult>
    {
        private readonly ISearch<ReceiptBatch> batches;
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public CommitReceiptBatchCommandPostProcessor(ISearch<ReceiptBatch> batches, IMediator mediator)
        {
            this.batches = batches;
            this.mediator = mediator;
        }

        public async Task Handle(CommitBatchCommand command, ICommandResult response)
        {
            Logger.Trace("Handle");

            if (response.IsFailure)
                return;

            var batch = await batches.GetById(command.BatchId ?? -1)
                .Include(b => b.TransactionSubType)
                .Include(b => b.Transactions)
                .FirstOrDefaultAsync();

            if (batch == null)
                return;

            Task.WaitAll(batch.Transactions.OfType<Receipt>()
                .Select(t => mediator.PublishAsync(new ReceiptAddedNotification(t, commitBatchCommand: command)))
                .ToArray());
        }
    }
}