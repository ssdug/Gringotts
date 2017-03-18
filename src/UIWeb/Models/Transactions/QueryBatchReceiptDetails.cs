using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Users;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class BatchReceiptDetailsQuery : IAsyncRequest<BatchReceiptDetails>
    {
        public int? BatchId { get; private set; }

        public BatchReceiptDetailsQuery(int? batchId)
        {
            this.BatchId = batchId;
        }
    }

    public class BatchReceiptDetailsQueryHandler : IAsyncRequestHandler<BatchReceiptDetailsQuery, BatchReceiptDetails>
    {
        private readonly ISearch<ReceiptBatch> batches;
        public ILogger Logger { get; set; }

        public BatchReceiptDetailsQueryHandler(ISearch<ReceiptBatch> batches)
        {
            this.batches = batches;
        }

        public async Task<BatchReceiptDetails> Handle(BatchReceiptDetailsQuery message)
        {
            Logger.Trace("Handle");

            var batch = await batches.GetById(message.BatchId.Value)
                .Include(b => b.TransactionSubType)
                .Include(b => b.Transactions)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (batch != null)
            {
                return new BatchReceiptDetails
                {
                    Batch = batch
                };
            }

            return null;
        }
    }

    public class BatchReceiptDetailsViewModelPostProccesor : IAsyncPostRequestHandler<BatchReceiptDetailsQuery, BatchReceiptDetails>
    {
        public ILogger Logger { get; set; }

        private readonly IUserRepository userRepository;

        public BatchReceiptDetailsViewModelPostProccesor(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task Handle(BatchReceiptDetailsQuery command, BatchReceiptDetails response)
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