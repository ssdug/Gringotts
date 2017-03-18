using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Restitution;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class ExpenseEditorFormQuery : IAsyncRequest<ExpenseEditorForm>
    {
        public int? AccountId { get; private set; }
        public int? OrderId { get; private set; }

        public ExpenseEditorFormQuery(int? accountId = null, int? orderId = null)
        {
            this.AccountId = accountId;
            this.OrderId = orderId;
        }
    }

    public class ExpenseEditorFormQueryHandler : IAsyncRequestHandler<ExpenseEditorFormQuery, ExpenseEditorForm>
    {
        private readonly ISearch<Account> accounts;
        private readonly ILookup<TransactionSubType> subtypes;
        public ILogger Logger { get; set; }

        public ExpenseEditorFormQueryHandler(ISearch<Account> accounts, ILookup<TransactionSubType> subtypes)
        {
            this.accounts = accounts;
            this.subtypes = subtypes;
        }

        public async Task<ExpenseEditorForm> Handle(ExpenseEditorFormQuery message)
        {
            Logger.Trace("Handle::{0}", message.AccountId);

            var expenseTypes = GetExpenseTypes();
            var account = await accounts.GetById(message.AccountId.Value)
                .AsNoTracking()
                .FirstAsync();

            return new ExpenseEditorForm
            {
                IsClientAccount = account is ClientAccount,
                AccountId = account.Id,
                ParentFundId = account.Fund.Id,
                Available = account.Available,
                AvailableTypes = expenseTypes
            };
        }

        private ExpenseType[] GetExpenseTypes()
        {
            return subtypes.All.OfType<ExpenseType>()
                .Where(t => t.UserSelectable)
                .ToArray();
        }
    }

    public class ExpenseEditorFormRestitutionOrderPostProcessor: IAsyncPostRequestHandler<ExpenseEditorFormQuery, ExpenseEditorForm>
    {
        private readonly ISearch<Order> orders;
        public ILogger Logger { get; set; }

        public ExpenseEditorFormRestitutionOrderPostProcessor(ISearch<Order> orders)
        {
            this.orders = orders;
        }

        public async Task Handle(ExpenseEditorFormQuery command, ExpenseEditorForm response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            if (!command.OrderId.HasValue)
                return;

            var order = await orders.GetById(command.OrderId.Value)
                .Include(o => o.Payee)
                .AsNoTracking()
                .FirstAsync();

            response.OrderId = order.Id;
            response.PayeeId = order.Payee.Id;
            response.PayeeName = order.Payee.Name;
            response.Memo = order.OrderNumber;
        }
    }
}