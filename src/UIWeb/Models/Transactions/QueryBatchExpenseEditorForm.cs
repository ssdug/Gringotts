using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Models.Funds;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class BatchExpenseEditorFormQuery : IAsyncRequest<BatchExpenseEditorForm>
    {

        public int? FundId { get; private set; }
        public int? BatchId { get; private set; }

        public BatchExpenseEditorFormQuery(int? fundId = null, int? batchId = null)
        {
            this.FundId = fundId;
            BatchId = batchId;
        }
    }

    public class BatchExpenseEditorFormQueryHandler : IAsyncRequestHandler<BatchExpenseEditorFormQuery, BatchExpenseEditorForm>
    {
        private readonly ISearch<ExpenseBatch> batches;
        private readonly ISearch<Expense> expenses;
        private readonly ISearch<Fund> funds;
        private readonly ILookup<ExpenseType> expenseTypes;

        public ILogger Logger { get; set; }

        public BatchExpenseEditorFormQueryHandler(ISearch<ExpenseBatch> batches, ISearch<Expense> expenses, ISearch<Fund> funds, ILookup<ExpenseType> expenseTypes)
        {
            this.batches = batches;
            this.expenses = expenses;
            this.funds = funds;
            this.expenseTypes = expenseTypes;
        }

        public async Task<BatchExpenseEditorForm> Handle(BatchExpenseEditorFormQuery query)
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

        private ExpenseType[] GetExpenseTypes()
        {
            return expenseTypes.All
                .Where(t => t.UserSelectable)
                .ToArray();
        }

        private async Task<BatchExpenseEditorForm> GetForFund(int fundId)
        {
            var expenseTypes = GetExpenseTypes();
            var fund = await funds.GetById(fundId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (fund is SubsidiaryFund)
            {
                return new SubsidiaryBatchExpenseEditorForm
                {
                    FundId = fund.Id,
                    AvailableTypes = expenseTypes,
                    TotalAmount = 0
                };
            }

            if (fund is ClientFund)
            {
                return new ClientBatchExpenseEditorForm
                {
                    FundId = fund.Id,
                    AvailableTypes = expenseTypes,
                    TotalAmount = 0
                };
            }

            return null;
        }

        private async Task<BatchExpenseEditorForm> GetForBatch(int batchId)
        {
            var expenseTypes = GetExpenseTypes();
            var batch = await batches.GetById(batchId)
                .Where(b => !b.Committed)
                .Include(b => b.Fund)
                .Include(b => b.Payee)
                .Include(b => b.TransactionSubType)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (batch == null)
                return null;

            var transactions = await expenses.All()
                .Include(e => e.ExpenseCategory)
                .Include(e => e.Account)
                .Where(e => e.TransactionBatchId == batch.Id)
                .AsNoTracking()
                .ToArrayAsync();

            if (batch.Fund is SubsidiaryFund)
            {
                return SubsidiaryBatchExpenseEditorForm.FromBatch(batch, transactions, expenseTypes);
            }

            if (batch.Fund is ClientFund)
            {
                return ClientBatchExpenseEditorForm.FromBatch(batch, transactions, expenseTypes);
            }

            return null;
        }
    }
}