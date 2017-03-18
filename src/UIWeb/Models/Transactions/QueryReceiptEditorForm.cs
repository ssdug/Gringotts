using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class ReceiptEditorFormQuery : IAsyncRequest<ReceiptEditorForm>
    {
        public int? AccountId { get; private set; }

        public ReceiptEditorFormQuery(int? accountId = null)
        {
            this.AccountId = accountId;
        }
    }

    public class ReceiptEditorFromQueryHandler : IAsyncRequestHandler<ReceiptEditorFormQuery, ReceiptEditorForm>
    {
        private readonly ISearch<Account> accounts;
        private readonly ILookup<TransactionSubType> subtypes;
        public ILogger Logger { get; set; }

        public ReceiptEditorFromQueryHandler(ISearch<Account> accounts, ILookup<TransactionSubType> subtypes)
        {
            this.accounts = accounts;
            this.subtypes = subtypes;
        }

        public async Task<ReceiptEditorForm> Handle(ReceiptEditorFormQuery message)
        {
            Logger.Trace("Handle::{0}", message.AccountId);

            var receiptTypes = GetReceiptTypes();
            var account = await accounts.GetById(message.AccountId.Value)
                .AsNoTracking()
                .FirstAsync();


            return new ReceiptEditorForm
            {
                IsClientAccount = account is ClientAccount,
                AccountId = account.Id,
                ParentFundId = account.Fund.Id,
                AvailableTypes = receiptTypes
            };
        }

        private ReceiptType[] GetReceiptTypes()
        {
            return subtypes.All.OfType<ReceiptType>()
                .Where(t => t.UserSelectable)
                .ToArray();
        }
    }
}