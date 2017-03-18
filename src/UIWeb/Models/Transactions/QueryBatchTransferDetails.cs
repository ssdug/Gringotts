using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Users;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class BatchTransferDetailsQuery : IAsyncRequest<BatchTransferDetails>
    {
        public int? BatchId { get; private set; }

        public BatchTransferDetailsQuery(int? batchId)
        {
            this.BatchId = batchId;
        }
    }

    public class BatchTransferDetailsQueryHandler : IAsyncRequestHandler<BatchTransferDetailsQuery, BatchTransferDetails>
    {
        private readonly ISearch<TransferBatch> batches;
        public ILogger Logger { get; set; }

        public BatchTransferDetailsQueryHandler(ISearch<TransferBatch> batches)
        {
            this.batches = batches;
        }

        public async Task<BatchTransferDetails> Handle(BatchTransferDetailsQuery message)
        {
            Logger.Trace("Handle");

            var batch = await batches.GetById(message.BatchId.Value)
                .Include(b => b.TransactionSubType)
                .Include(b => b.Transactions)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (batch != null)
            {
                return new BatchTransferDetails
                {
                    Batch = batch
                };
            }

            return null;
        }
    }

    public class BatchTransferDetailsViewModelPostProccesor : IAsyncPostRequestHandler<BatchTransferDetailsQuery, BatchTransferDetails>
    {
        public ILogger Logger { get; set; }

        private readonly IUserRepository userRepository;

        public BatchTransferDetailsViewModelPostProccesor(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task Handle(BatchTransferDetailsQuery command, BatchTransferDetails response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            await Task.Run(() =>
            {
                if (!string.IsNullOrWhiteSpace(response.Batch.CreatedBy))
                    response.CreatedBy = GetActiveDirectoryUser(response.Batch.CreatedBy);

                if (!string.IsNullOrWhiteSpace(response.Batch.UpdatedBy))
                    response.UpdatedBy = GetActiveDirectoryUser(response.Batch.CreatedBy);
            });

        }

        private User GetActiveDirectoryUser(string samAccountName)
        {
            Logger.Trace("GetActiveDirectoryUser::{0}", samAccountName);

            return userRepository.FindByUser(samAccountName);
        }
    }
}