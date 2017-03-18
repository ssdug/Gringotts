using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Users;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class BatchExpenseDetailsQuery : IAsyncRequest<BatchExpenseDetails>
    {
        public int? BatchId { get; private set; }

        public BatchExpenseDetailsQuery(int? batchId)
        {
            this.BatchId = batchId;
        }
    }

    public class BatchExpenseDetailsQueryHandler: IAsyncRequestHandler<BatchExpenseDetailsQuery, BatchExpenseDetails>
    {
        private readonly ISearch<ExpenseBatch> batches;
        public ILogger Logger { get; set; }

        public BatchExpenseDetailsQueryHandler(ISearch<ExpenseBatch> batches)
        {
            this.batches = batches;
        }

        public async Task<BatchExpenseDetails> Handle(BatchExpenseDetailsQuery message)
        {
            Logger.Trace("Handle");

            var batch = await batches.GetById(message.BatchId.Value)
                .Include(b => b.TransactionSubType)
                .Include(b => b.Payee)
                .Include(b => b.Transactions)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (batch != null)
            {
                return new BatchExpenseDetails
                {
                    Batch = batch
                };
            }

            return null;
        }
    }

    public class BatchExpenseDetailsViewModelPostProccesor : IAsyncPostRequestHandler<BatchExpenseDetailsQuery, BatchExpenseDetails>
    {
        public ILogger Logger { get; set; }

        private readonly IUserRepository userRepository;

        public BatchExpenseDetailsViewModelPostProccesor(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task Handle(BatchExpenseDetailsQuery command, BatchExpenseDetails response)
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