using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Checks
{
    public class CheckEditorFormQuery : IAsyncRequest<CheckEditorForm>
    {
        public int? Id { get; private set; }
        public bool? IsBatchPrint { get; private set; }
        public CheckEditorFormQuery(int? Id, bool? IsBatchPrint)
        {
            this.Id = Id;
            this.IsBatchPrint = IsBatchPrint;
        }
    }

    public class CheckEditorFromQueryHandler : IAsyncRequestHandler<CheckEditorFormQuery, CheckEditorForm>
    {
        private readonly ISearch<Expense> expenses;
        private readonly ISearch<ExpenseBatch> expenseBatches;
        private readonly IUserRepository userRepository;
        public ILogger Logger { get; set; }

        public CheckEditorFromQueryHandler(ISearch<Expense> expenses, ISearch<ExpenseBatch> expenseBatches, IUserRepository userRepository)
        {
            this.expenses = expenses;
            this.expenseBatches = expenseBatches;
            this.userRepository = userRepository;
        }

        public async Task<CheckEditorForm> Handle(CheckEditorFormQuery message)
        {
            Logger.Trace("Handle::{0}", message.Id);
            return message.IsBatchPrint.Value? 
                  await BuildCheckEditorFormFromExpenseBatch(message) 
                : await BuildCheckEditorFormFromExpense(message);
        }

        private async Task <CheckEditorForm> BuildCheckEditorFormFromExpense(CheckEditorFormQuery message )
        {
            var expense = await expenses.GetById(message.Id.Value)
                                        .Include(e => e.Checks)
                                         .FirstOrDefaultAsync();

            var preparedBy = userRepository.CurrentUser().UserName;

            if (expense.Checks.Any())
            {
                var check = expense.Checks
                            .OrderByDescending(c => c.Created)
                            .First();
                //reprint
                return new CheckEditorForm
                {
                    CheckId = check.Id,
                    Amount = check.Amount.ToString("0.00"),
                    TransactionId = check.TransactionId,
                    AccountId = check.AccountId,
                    CheckNumber = check.CheckNumber.ToString(),
                    Memo = check.Memo,
                    FundId = check.FundId,
                    PaidTo = check.PaidTo,
                    PrintedBy = check.PrintedBy,
                    PreparedBy = preparedBy,
                    TransactionBatchId = check.TransactionBatchId,
                    WasPrinted = true

                };
            }
            else
            {
                //initial print
                return new CheckEditorForm
                {
                    Amount = expense.Amount.ToString("0.00"),
                    TransactionId = expense.Id,
                    AccountId = expense.AccountId,
                    CheckNumber = "",
                    Memo = expense.Memo,
                    FundId = expense.FundId,
                    PrintedBy = preparedBy,
                    PreparedBy = preparedBy,
                    PaidTo = expense.Payee.Name,
                    TransactionBatchId = null,
                    WasPrinted = false

                };
            }
        }


        private async Task<CheckEditorForm> BuildCheckEditorFormFromExpenseBatch(CheckEditorFormQuery message)
        {
            var expenseBatch = await expenseBatches.GetById(message.Id.Value)
                                        .Where(b => b.Committed)
                                        .Include(e => e.Checks)
                                         .FirstOrDefaultAsync();

            var preparedBy = userRepository.CurrentUser().UserName;
            
            //if check exists, reprint
            if (expenseBatch.Checks.Any())
            {
                var check = expenseBatch.Checks
                            .OrderByDescending(c => c.Created)
                            .First();
                return new CheckEditorForm
                {
                    CheckId = check.Id,
                    Amount = check.Amount.ToString("0.00"),
                    TransactionId = check.TransactionId,
                    AccountId = check.AccountId,
                    CheckNumber = check.CheckNumber.ToString(),
                    Memo = check.Memo,
                    FundId = check.FundId,
                    PaidTo = check.PaidTo,
                    PrintedBy = check.PrintedBy,
                    PreparedBy = preparedBy,
                    TransactionBatchId = check.TransactionBatchId,
                    WasPrinted = true

                };
            }
            else // Building a new check
            {
                //initial print
                return new CheckEditorForm
                {
                    Amount = expenseBatch.ExpectedAmount.ToString("0.00"),
                    TransactionId = null,
                    AccountId = null,
                    TransactionBatchId = expenseBatch.Id,
                    CheckNumber = "",
                    Memo = expenseBatch.Memo,
                    FundId = expenseBatch.FundId,
                    PrintedBy = preparedBy,
                    PreparedBy = preparedBy,
                    PaidTo = expenseBatch.Payee.Name,
                    WasPrinted = false

                };
            }
        }
    }
}