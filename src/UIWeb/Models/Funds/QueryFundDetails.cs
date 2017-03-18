using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using MediatR;
using PagedList;

namespace Wiz.Gringotts.UIWeb.Models.Funds
{
    public class FundDetailsQuery : IAsyncRequest<FundDetails>
    {
        public SearchPager Pager { get; private set; }
        public int FundId { get; private set; }

        public FundDetailsQuery(int fundId, SearchPager searchPager)
        {
            this.FundId = fundId;
            this.Pager = searchPager;
        }
    }

    public class FundDetailsQueryHandler : IAsyncRequestHandler<FundDetailsQuery, FundDetails>
    {
        private readonly ISearch<Fund> funds;
        public ILogger Logger { get; set; }

        public FundDetailsQueryHandler(ISearch<Fund> funds)
        {
            this.funds = funds;
        }

        public async Task<FundDetails> Handle(FundDetailsQuery query)
        {
            Logger.Trace("Handle::{0}", query.FundId);

            var fund = await funds.GetById(query.FundId)
                .Include(f => f.FundType)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (fund != null)
                return new FundDetails
                {
                    Fund = fund,
                    Pager = query.Pager
                };

            return null;
        }
    }

    public class FundSubsidiaryAccountsPostProcessor : IAsyncPostRequestHandler<FundDetailsQuery, FundDetails>
    {
        private readonly ISearch<SubsidiaryAccount> accounts;
        public ILogger Logger { get; set; }

        public FundSubsidiaryAccountsPostProcessor(ISearch<SubsidiaryAccount> accounts)
        {
            this.accounts = accounts;
        }

        public async Task Handle(FundDetailsQuery command, FundDetails response)
        {
            Logger.Trace("Handle");

            if (response == null || !(response.Fund is SubsidiaryFund))
                return;

            response.Items = await GetAccounts(command, response.Fund);
        }

        private Task<IPagedList<FundSubsidiary>> GetAccounts(FundDetailsQuery reqeust, Fund fund)
        {
            Logger.Trace("GetAccounts");

            var fundAccounts = accounts.GetBySearch(reqeust.Pager)
                .Where(a => a.FundId == fund.Id)
                .OrderBy(a => a.Name)
                .AsNoTracking()
                .Select(s => new FundSubsidiary
                {
                    Id = s.Id,
                    Name = s.Name,
                    BankNumbers = new[] { s.BankNumber },
                    Total = s.Total,
                    Encumbered = s.Encumbered,
                    Available = s.Available
                });

            return fundAccounts.ToPagedListAsync(reqeust.Pager.Page, reqeust.Pager.PageSize);

        }

    }

    public class FundClientSubsidiaryAccountsPostProcessor : IAsyncPostRequestHandler<FundDetailsQuery, FundDetails>
    {

        private readonly ISearch<ClientAccount> accounts;

        public ILogger Logger { get; set; }

        public FundClientSubsidiaryAccountsPostProcessor(ISearch<ClientAccount> accounts)
        {
            this.accounts = accounts;
        }

        public async Task Handle(FundDetailsQuery command, FundDetails response)
        {
            Logger.Trace("Handle");

            if (response == null || !(response.Fund is ClientFund))
                return;

            response.Items = await GetAccounts(command, response.Fund); 
        }

        private Task<IPagedList<FundSubsidiary>> GetAccounts(FundDetailsQuery reqeust, Fund fund)
        {
            Logger.Trace("GetAccounts");

            var clientAccounts = accounts.GetBySearch(reqeust.Pager)
                .Where(a => a.FundId == fund.Id);

            //Do roll up into view model
            var items = from a in clientAccounts
                        group a by a.Residency.Client into c
                        orderby c.Key.LastName
                        select new FundClientSubsidiary
                        {
                            Id = c.Key.Id,
                            Name = c.Key.LastName + ", " + c.Key.FirstName + " " + c.Key.MiddleName,
                            BankNumbers = c.Where(x => x.BankNumber != null).Select(x => x.BankNumber),
                            Accounts = c,
                            Total = c.Sum(x => x.Total),
                            Encumbered = c.Sum(x => x.Encumbered),
                            Available = c.Sum(x => x.Available)
                        };
            
            return items.AsQueryable().ToPagedListAsync<FundSubsidiary>(reqeust.Pager.Page, reqeust.Pager.PageSize);
        }
    }

    public class FundTransactionBatchesPostProcessor : IAsyncPostRequestHandler<FundDetailsQuery, FundDetails>
    {
        private readonly ISearch<ExpenseBatch> expenseBatches;
        private readonly ISearch<ReceiptBatch> receiptBatches;
        public ILogger Logger { get; set; }

        public FundTransactionBatchesPostProcessor(ISearch<ExpenseBatch> expenseBatches, ISearch<ReceiptBatch> receiptBatches)
        {
            this.expenseBatches = expenseBatches;
            this.receiptBatches = receiptBatches;
        }

        public async Task Handle(FundDetailsQuery command, FundDetails response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            var matchingReceiptBatches = receiptBatches.GetBySearch(command.Pager)
                .Where(b => b.FundId == command.FundId)
                .Cast<TransactionBatch>();

            var matchingExpenseBatches = expenseBatches.GetBySearch(command.Pager)
                .Where(b => b.FundId == command.FundId)
                .Cast<TransactionBatch>();

            var items = matchingReceiptBatches.Union(matchingExpenseBatches)
                .OrderByDescending(t => t.Effective)
                .ThenByDescending(t => t.Updated)
                .AsNoTracking();

            response.Batches = await items.ToPagedListAsync(command.Pager.Page, command.Pager.PageSize);
        }
    }
}