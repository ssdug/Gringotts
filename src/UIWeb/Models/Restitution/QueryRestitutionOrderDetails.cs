using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using Wiz.Gringotts.UIWeb.Models.Users;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Restitution
{
    public class RestitutionOrderDetailsQuery : IAsyncRequest<RestitutionOrderDetails>
    {
        public int OrderId { get; private set; }

        public TransactionsSearchPager Pager { get; private set; }

        public RestitutionOrderDetailsQuery(int orderId, TransactionsSearchPager searchPager)
        {
            this.OrderId = orderId;
            this.Pager = searchPager;
        }
    }

    public class RestitutionOrderDetailsQueryHandler : IAsyncRequestHandler<RestitutionOrderDetailsQuery, RestitutionOrderDetails>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Order> orders;

        public RestitutionOrderDetailsQueryHandler(ISearch<Order> orders)
        {
            this.orders = orders;
        }

        public async Task<RestitutionOrderDetails> Handle(RestitutionOrderDetailsQuery query)
        {
            Logger.Trace("Handle::{0}", query.OrderId);

            var order = await orders.GetById(query.OrderId)
                .Include(o => o.Payee)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (order != null)
                return new RestitutionOrderDetails
                {
                    Order = order,
                    Pager = query.Pager
                };

            return null;
        }
    }

    public class RestitutionOrderDetailsViewModelPostProccesor : IAsyncPostRequestHandler<RestitutionOrderDetailsQuery, RestitutionOrderDetails>
    {
        public ILogger Logger { get; set; }

        private readonly IUserRepository userRepository;

        public RestitutionOrderDetailsViewModelPostProccesor(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task Handle(RestitutionOrderDetailsQuery command, RestitutionOrderDetails response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            await Task.Run(() =>
            {
                if (!string.IsNullOrWhiteSpace(response.Order.CreatedBy))
                    response.CreatedBy = GetActiveDirectoryUser(response.Order.CreatedBy);

                if (!string.IsNullOrWhiteSpace(response.Order.UpdatedBy))
                    response.UpdatedBy = GetActiveDirectoryUser(response.Order.CreatedBy);
            });

        }

        private User GetActiveDirectoryUser(string samAccountName)
        {
            Logger.Trace("GetActiveDirectoryUser::{0}", samAccountName);

            return userRepository.FindByUser(samAccountName);
        }
    }

    public class RestitutionOrderPaymentTransactionsPostProcessor :
        IAsyncPostRequestHandler<RestitutionOrderDetailsQuery, RestitutionOrderDetails>
    {
        private readonly ISearch<Expense> expenses;
        public ILogger Logger { get; set; }

        public RestitutionOrderPaymentTransactionsPostProcessor(ISearch<Expense> expenses)
        {
            this.expenses = expenses;
        }

        public async Task Handle(RestitutionOrderDetailsQuery command, RestitutionOrderDetails response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            var account = response.Order.Residency
                .Accounts.First(a => a.AccountType.Name == AccountType.Restitution);

            var items = expenses.GetBySearch(command.Pager)
                .Where(e => e.AccountId == account.Id)
                .Cast<Transaction>()
                .AsNoTracking()
                .OrderByDescending(t => t.Created);

            response.Items = await items.ToPagedListAsync(command.Pager.Page, command.Pager.PageSize);
        }

    }
}