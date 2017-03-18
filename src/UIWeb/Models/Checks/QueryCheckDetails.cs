using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using MediatR;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Models.Transactions;

namespace Wiz.Gringotts.UIWeb.Models.Checks
{
    public class CheckDetailsQuery : IAsyncRequest<CheckDetails>
    {
        public int checkId { get; private set; }
        public CheckDetailsQuery(int checkId)
        {
            this.checkId = checkId;
        }
    }

    public class CheckDetailsQueryHandler : IAsyncRequestHandler<CheckDetailsQuery, CheckDetails>
    {
        public ILogger logger { get; set; }
        public readonly ISearch<Check> checks;
        public CheckDetailsQueryHandler(ISearch<Check> checks)
        {
            this.checks = checks;
        }
        public async Task<CheckDetails> Handle(CheckDetailsQuery query)
        {
            var check = await checks.GetById(query.checkId)
                                .Include(a => a.Expense)
                                .Include(b => b.ExpenseBatch)
                                .AsNoTracking()
                                .FirstOrDefaultAsync();
            if (check == null) return null;
            return new CheckDetails
            {
                Amount = check.Amount,
                CheckNumber = check.CheckNumber,
                PaidTo = check.PaidTo.Length > 100 ? check.PaidTo.Substring(0,100) : check.PaidTo,
                Memo = check.Memo.Length >100 ? check.Memo.Substring(0,100) : check.Memo,
                PrintedBy = check.PrintedBy,
                PrintedDate = check.Created.ToShortDateString(),
                Expenses = check.TransactionBatchId == null ? new Expense[] { check.Expense } : check.ExpenseBatch.Transactions.OfType<Expense>().ToArray()

            };
        }
    }
}