using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Models.Funds;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class BatchReceiptEditorFormQuery : IAsyncRequest<BatchReceiptEditorForm>
    {
        public int? FundId { get; private set; }
        public int? BatchId { get; private set; }

        public BatchReceiptEditorFormQuery(int? fundId = null, int? batchId = null)
        {
            this.FundId = fundId;
            BatchId = batchId;
        }
    }

    public class BatchReceiptEditorFormQueryHandler : IAsyncRequestHandler<BatchReceiptEditorFormQuery, BatchReceiptEditorForm>
    {
        private readonly ISearch<ReceiptBatch> batches;
        private readonly ISearch<Receipt> receipts;
        private readonly ISearch<Fund> funds;
        private readonly ILookup<ReceiptType> receiptTypes;

        public ILogger Logger { get; set; }

        public BatchReceiptEditorFormQueryHandler(ISearch<ReceiptBatch> batches, ISearch<Receipt> receipts, ISearch<Fund> funds, ILookup<ReceiptType> receiptTypes)
        {
            this.batches = batches;
            this.receipts = receipts;
            this.funds = funds;
            this.receiptTypes = receiptTypes;
        }

        public async Task<BatchReceiptEditorForm> Handle(BatchReceiptEditorFormQuery query)
        {
            Logger.Trace("Handle::{0}", query.FundId);

            if (query.FundId.HasValue)
            {
                return await GetForFund(query.FundId.Value);
            }

            if (query.BatchId.HasValue)
            {
                return await GetForBatch(query.BatchId.Value);
            }

            return null;
        }

        private ReceiptType[] GetReceiptTypes()
        {
            return receiptTypes.All
                .Where(t => t.UserSelectable)
                .ToArray();
        }

        private async Task<BatchReceiptEditorForm> GetForFund(int fundId)
        {
            var expenseTypes = GetReceiptTypes();
            var fund = await funds.GetById(fundId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (fund is SubsidiaryFund)
            {
                return new SubsidiaryBatchReceiptEditorForm
                {
                    FundId = fund.Id,
                    AvailableTypes = expenseTypes,
                    TotalAmount = 0
                };
            }

            if (fund is ClientFund)
            {
                return new ClientBatchReceiptEditorForm
                {
                    FundId = fund.Id,
                    AvailableTypes = expenseTypes,
                    TotalAmount = 0
                };
            }

            return null;
        }

        private async Task<BatchReceiptEditorForm> GetForBatch(int batchId)
        {
            var expenseTypes = GetReceiptTypes();
            var batch = await batches.GetById(batchId)
                .Where(b => !b.Committed)
                .Include(b => b.Fund)
                .Include(b => b.TransactionSubType)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (batch == null)
                return null;

            var transactions = await receipts.All()
                .Include(e => e.ReceiptSource)
                .Include(e => e.Account)
                .Where(e => e.TransactionBatchId == batch.Id)
                .AsNoTracking()
                .ToArrayAsync();

            if (batch.Fund is SubsidiaryFund)
            {
                return SubsidiaryBatchReceiptEditorForm.FromBatch(batch, transactions, expenseTypes);
            }

            if (batch.Fund is ClientFund)
            {
                return ClientBatchReceiptEditorForm.FromBatch(batch, transactions, expenseTypes);
            }

            return null;
        }
    }
}