using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using Wiz.Gringotts.UIWeb.Models.Users;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Accounts
{
    public class AccountDetailsQuery: IAsyncRequest<AccountDetails>
    {
        public int AccountId { get; private set; }
        public TransactionsSearchPager Pager { get; private set; }

        public AccountDetailsQuery(int accountId, TransactionsSearchPager searchPager)
        {
            this.AccountId = accountId;
            this.Pager = searchPager;
        }
    }

    public class AccountDetailsQueryHandler : IAsyncRequestHandler<AccountDetailsQuery, AccountDetails>
    {
        private readonly ISearch<Account> accounts;
        public ILogger Logger { get; set; }

        public AccountDetailsQueryHandler(ISearch<Account> accounts)
        {
            this.accounts = accounts;
        }

        public async Task<AccountDetails> Handle(AccountDetailsQuery query)
        {
            Logger.Trace("Handle::{0}", query.AccountId);

            var account = await accounts.GetById(query.AccountId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (account == null)
                return null;

            return new AccountDetails
            {
                Account = account,
                Fund = account.Fund,
                Pager = query.Pager
            };
        }
    }

    public class AccountDetailsViewModelPostProcessor : IAsyncPostRequestHandler<AccountDetailsQuery, AccountDetails>
    {
        private readonly IUserRepository userRepository;
        public ILogger Logger { get; set; }

        public AccountDetailsViewModelPostProcessor(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task Handle(AccountDetailsQuery command, AccountDetails response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            await Task.Run(() =>
            {
                if (!string.IsNullOrWhiteSpace(response.Account.CreatedBy))
                    response.CreatedBy = GetActiveDirectoryUser(response.Account.CreatedBy);

                if (!string.IsNullOrWhiteSpace(response.Account.UpdatedBy))
                    response.UpdatedBy = GetActiveDirectoryUser(response.Account.CreatedBy);
            });
        }

        private User GetActiveDirectoryUser(string samAccountName)
        {
            Logger.Trace("GetActiveDirectoryUser::{0}", samAccountName);

            return userRepository.FindByUser(samAccountName);
        }
    }

    public class AccountTransactionsPostProcessor : IAsyncPostRequestHandler<AccountDetailsQuery, AccountDetails>
    {
        private readonly ISearch<Expense> expenses;
        private readonly ISearch<Receipt> receipts;
        public ILogger Logger { get; set; }

        public AccountTransactionsPostProcessor(ISearch<Expense> expenses, ISearch<Receipt> receipts)
        {
            this.expenses = expenses;
            this.receipts = receipts;
        }

        public async Task Handle(AccountDetailsQuery command, AccountDetails response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            var matchingReceipts = receipts.GetBySearch(command.Pager)
                .Include(r => r.Batch)
                .Where(r => r.AccountId == command.AccountId)
                .Cast<Transaction>();

            var matchingExpenses = expenses.GetBySearch(command.Pager)
                .Include(e => e.Batch)
                .Where(r => r.AccountId == command.AccountId)
                .Cast<Transaction>();

            var items = matchingReceipts.Union(matchingExpenses) 
                .OrderByDescending(t => t.Effective)
                .ThenByDescending(t => t.Updated)
                .AsNoTracking();

            response.Items = await items.ToPagedListAsync(command.Pager.Page, command.Pager.PageSize);
        }
    }

    public class VoidExpenseEditorFormPostProcessor : IAsyncPostRequestHandler<AccountDetailsQuery, AccountDetails>
    {
        private readonly ILookup<VoidType> voidTypes;
        public ILogger Logger { get; set; }

        public VoidExpenseEditorFormPostProcessor(ILookup<VoidType>  voidTypes)
        {
            this.voidTypes = voidTypes;
        }

        public Task Handle(AccountDetailsQuery command, AccountDetails response)
        {
            Logger.Trace("Handle");

            return Task.Run(() =>
            {
                if (response == null)
                    return;

                var types = voidTypes.All.ToArray();

                response.VoidExpenseEditorForm = new VoidExpenseEditorForm
                {
                    AccountId = command.AccountId,
                    AvailableTypes = types
                };
            });
        }
    }
}