using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Models.Funds;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Accounts
{
    public class AccountEditorFormQuery : IAsyncRequest<AccountEditorForm>
    {
        public int? AccountId { get; private set; }
        public int? ParentFundId { get; private set; }

        public AccountEditorFormQuery(int? parentFundId = null, int? accountId = null)
        {
            this.AccountId = accountId;
            this.ParentFundId = parentFundId;
        }
    }

    public class AccountEditorFormQueryHandler:IAsyncRequestHandler<AccountEditorFormQuery, AccountEditorForm>
    {
        private readonly ISearch<Account> accounts;
        private readonly ISearch<Fund> funds;
        public ILogger Logger { get; set; }

        public AccountEditorFormQueryHandler(ISearch<Account> accounts, ISearch<Fund> funds)
        {
            this.accounts = accounts;
            this.funds = funds;
        }

        public async Task<AccountEditorForm> Handle(AccountEditorFormQuery message)
        {
            Logger.Trace("Handle");

            if (message.AccountId.HasValue)
            {
                var account = await accounts.GetById(message.AccountId.Value)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                var fund = await funds.All().OfType<SubsidiaryFund>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Subsidiaries.Any(s => s.Id == message.AccountId.Value));

                return AccountEditorForm.FromFund(fund, account);
            }

            if (message.ParentFundId.HasValue)
            {
                var fund = await funds.GetById(message.ParentFundId.Value)
                    .OfType<SubsidiaryFund>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                return AccountEditorForm.FromFund(fund);
            }

            return null;
        }
    }
}